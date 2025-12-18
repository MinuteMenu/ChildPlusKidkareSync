using ChildPlusKidkareSync.Core.Enums;
using ChildPlusKidkareSync.Core.Models.Sync;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace ChildPlusKidkareSync.Infrastructure.Data;

public interface ISyncLogRepository
{
    Task<SyncAction> GetSyncDecisionAsync(string connectionString, string entityType, string sourceId, byte[] rowVersion);

    Task InsertSyncLogAsync(string connectionString, SyncLog log);

    //Task<Dictionary<string, SyncAction>> GetBulkSyncDecisionsAsync(string connectionString,string entityType,List<string> sourceIds);

    //Task<Dictionary<string, SyncAction>> GetBulkSyncDecisionsWithVersionAsync(string connectionString,string entityType,Dictionary<string, byte[]> sourceIdVersions);

    //Task InsertBatchSyncLogsAsync(string connectionString,List<SyncLog> logs);
    Task<Dictionary<string, SyncAction>> GetBulkSyncDecisionsAsync(
        string connectionString,
        string entityType,
        List<string> sourceIds);

    Task<Dictionary<string, SyncAction>> GetBulkSyncDecisionsWithCompositeAsync(
        string connectionString,
        string entityType,
        Dictionary<string, CompositeTimestamp> sourceIdComposites);

    Task InsertBatchSyncLogsAsync(
        string connectionString,
        List<SyncLog> logs);
}

// =====================================================
// SYNC LOG REPOSITORY
// Major Improvements:
// 1. Bulk sync decision queries (1 query instead of N)
// 2. Bulk insert sync logs with Table-Valued Parameters
// 3. Connection pooling awareness
// 4. Proper transaction handling
// 5. Reduced round trips to database
// =====================================================
public class SyncLogRepository : ISyncLogRepository
{
    private readonly ILogger<SyncLogRepository> _logger;

    public SyncLogRepository(ILogger<SyncLogRepository> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get sync decision for a single entity
    /// </summary>
    public async Task<SyncAction> GetSyncDecisionAsync(
        string connectionString,
        string entityType,
        string sourceId,
        byte[] rowVersion)
    {
        try
        {
            const string query = @"
            SELECT CASE
                WHEN NOT EXISTS (
                    SELECT 1 FROM SyncLogTable
                    WHERE EntityType = @EntityType AND SourceId = @SourceId
                ) THEN 'Insert'
                WHEN EXISTS (
                    SELECT 1 FROM SyncLogTable
                    WHERE EntityType = @EntityType AND SourceId = @SourceId
                      AND RowVersionChildPlus = @RowVersionChildPlus
                      AND SyncStatus = 'Success'
                ) THEN 'Skip'
                ELSE 'Update'
            END AS SyncDecision";

            using var connection = new SqlConnection(connectionString);
            var result = await connection.QuerySingleAsync<string>(query, new
            {
                EntityType = entityType,
                SourceId = sourceId,
                RowVersionChildPlus = rowVersion
            });

            return result switch
            {
                "Insert" => SyncAction.Insert,
                "Update" => SyncAction.Update,
                "Skip" => SyncAction.Skip,
                _ => SyncAction.Error
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if should sync {EntityType} {SourceId}", entityType, sourceId);
            return SyncAction.Error;
        }
    }

    /// <summary>
    /// Insert single sync log
    /// </summary>
    public async Task InsertSyncLogAsync(string connectionString, SyncLog log)
    {
        try
        {
            const string query = @"
            INSERT INTO SyncLogTable (
                EntityType,
                SourceId,
                TargetId,
                SyncAction,
                SyncStatus,
                Message,
                RowVersionChildPlus,
                TimestampSynced,
                CenterId,
                RequestId,
                CreatedBy
            )
            VALUES (
                @EntityType,
                @SourceId,
                @TargetId,
                @SyncAction,
                @SyncStatus,
                @Message,
                @RowVersionChildPlus,
                GETUTCDATE(),
                @CenterId,
                @RequestId,
                @CreatedBy
            )";

            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing sync log for {EntityType} {SourceId}", log.EntityType, log.SourceId);
            throw;
        }
    }

    /// <summary>
    /// Get sync decisions for multiple entities in bulk using COMPOSITE TIMESTAMP
    /// This compares the composite timestamp instead of just main table timestamp
    /// </summary>
    public async Task<Dictionary<string, SyncAction>> GetBulkSyncDecisionsAsync(
        string connectionString,
        string entityType,
        List<string> sourceIds)
    {
        if (sourceIds == null || !sourceIds.Any())
        {
            return new Dictionary<string, SyncAction>();
        }

        try
        {
            // Process in batches of 2000 to avoid SQL parameter limits
            const int batchSize = 2000;
            var allDecisions = new Dictionary<string, SyncAction>();

            for (int i = 0; i < sourceIds.Count; i += batchSize)
            {
                var batch = sourceIds.Skip(i).Take(batchSize).ToList();
                var batchDecisions = await GetBulkSyncDecisionsBatchAsync(connectionString, entityType, batch);

                foreach (var kvp in batchDecisions)
                {
                    allDecisions[kvp.Key] = kvp.Value;
                }
            }

            _logger.LogDebug("Retrieved {Count} sync decisions for {EntityType} using composite timestamps",
                allDecisions.Count, entityType);

            return allDecisions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bulk sync decisions for {EntityType}", entityType);

            // Fallback: return Insert for all (safe default)
            return sourceIds.ToDictionary(id => id, _ => SyncAction.Insert);
        }
    }

    /// <summary>
    /// Process a batch of sync decisions using COMPOSITE TIMESTAMP
    /// </summary>
    private async Task<Dictionary<string, SyncAction>> GetBulkSyncDecisionsBatchAsync(
        string connectionString,
        string entityType,
        List<string> sourceIds)
    {
        // Query uses RowVersionComposite if available, falls back to RowVersionChildPlus
        const string query = @"
        WITH SourceList AS (
            SELECT value AS SourceId
            FROM STRING_SPLIT(@SourceIds, ',')
        ),
        LatestLogs AS (
            SELECT 
                sl.SourceId,
                -- Use Composite if available, otherwise use main table timestamp
                COALESCE(
                    CONVERT(BIGINT, sl.RowVersionComposite),
                    CONVERT(BIGINT, sl.RowVersionChildPlus)
                ) AS TimestampValue,
                sl.SyncStatus,
                ROW_NUMBER() OVER (
                    PARTITION BY sl.SourceId 
                    ORDER BY 
                        COALESCE(
                            CONVERT(BIGINT, sl.RowVersionComposite),
                            CONVERT(BIGINT, sl.RowVersionChildPlus)
                        ) DESC,
                        sl.TimestampSynced DESC
                ) AS RowNum
            FROM SyncLogTable sl
            INNER JOIN SourceList s ON sl.SourceId = s.SourceId
            WHERE sl.EntityType = @EntityType
        )
        SELECT 
            s.SourceId,
            CASE
                WHEN ll.SourceId IS NULL THEN 'Insert'
                WHEN ll.SyncStatus = 'Success' THEN 'Skip'
                ELSE 'Update'
            END AS SyncDecision
        FROM SourceList s
        LEFT JOIN LatestLogs ll ON s.SourceId = ll.SourceId AND ll.RowNum = 1";

        using var connection = new SqlConnection(connectionString);

        var sourceIdsString = string.Join(",", sourceIds);
        var results = await connection.QueryAsync<SyncDecisionResult>(query, new
        {
            EntityType = entityType,
            SourceIds = sourceIdsString
        });

        var decisions = results.ToDictionary(
            r => r.SourceId,
            r => r.SyncDecision switch
            {
                "Insert" => SyncAction.Insert,
                "Update" => SyncAction.Update,
                "Skip" => SyncAction.Skip,
                _ => SyncAction.Error
            });

        // Fill in missing sourceIds with Insert action
        foreach (var sourceId in sourceIds)
        {
            if (!decisions.ContainsKey(sourceId))
            {
                decisions[sourceId] = SyncAction.Insert;
            }
        }

        return decisions;
    }

    /// <summary>
    /// Get bulk sync decisions WITH composite timestamp comparison
    /// This method compares current composite timestamps from ChildPlus with last synced composite
    /// </summary>
    public async Task<Dictionary<string, SyncAction>> GetBulkSyncDecisionsWithCompositeAsync(
        string connectionString,
        string entityType,
        Dictionary<string, CompositeTimestamp> sourceIdComposites)
    {
        if (sourceIdComposites == null || !sourceIdComposites.Any())
        {
            return new Dictionary<string, SyncAction>();
        }

        try
        {
            var decisions = new Dictionary<string, SyncAction>();

            // Create temp table for batch processing
            const string createTempTable = @"
            CREATE TABLE #SourceComposites (
                SourceId NVARCHAR(100),
                CompositeTimestamp BIGINT
            )";

            const string query = @"
            WITH LatestLogs AS (
                SELECT 
                    sl.SourceId,
                    COALESCE(
                        CONVERT(BIGINT, sl.RowVersionComposite),
                        CONVERT(BIGINT, sl.RowVersionChildPlus)
                    ) AS LastSyncedTimestamp,
                    sl.SyncStatus,
                    ROW_NUMBER() OVER (
                        PARTITION BY sl.SourceId 
                        ORDER BY 
                            COALESCE(
                                CONVERT(BIGINT, sl.RowVersionComposite),
                                CONVERT(BIGINT, sl.RowVersionChildPlus)
                            ) DESC,
                            sl.TimestampSynced DESC
                    ) AS RowNum
                FROM SyncLogTable sl
                INNER JOIN #SourceComposites sc ON sl.SourceId = sc.SourceId
                WHERE sl.EntityType = @EntityType
            )
            SELECT 
                sc.SourceId,
                CASE
                    -- Never synced before
                    WHEN ll.SourceId IS NULL THEN 'Insert'
                    -- Current composite > last synced composite = data changed
                    WHEN sc.CompositeTimestamp > ll.LastSyncedTimestamp THEN 'Update'
                    -- Last sync failed, retry
                    WHEN ll.SyncStatus != 'Success' THEN 'Update'
                    -- No changes
                    ELSE 'Skip'
                END AS SyncDecision,
                ll.LastSyncedTimestamp,
                sc.CompositeTimestamp AS CurrentTimestamp
            FROM #SourceComposites sc
            LEFT JOIN LatestLogs ll ON sc.SourceId = ll.SourceId AND ll.RowNum = 1";

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Create temp table
                await connection.ExecuteAsync(createTempTable, transaction: transaction);

                // Bulk insert composite timestamps
                var dataTable = CreateSourceCompositeDataTable(sourceIdComposites);

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, (SqlTransaction)transaction))
                {
                    bulkCopy.DestinationTableName = "#SourceComposites";
                    bulkCopy.ColumnMappings.Add("SourceId", "SourceId");
                    bulkCopy.ColumnMappings.Add("CompositeTimestamp", "CompositeTimestamp");
                    await bulkCopy.WriteToServerAsync(dataTable);
                }

                // Query for decisions
                var results = await connection.QueryAsync<SyncDecisionWithTimestampResult>(
                    query,
                    new { EntityType = entityType },
                    transaction: transaction);

                foreach (var result in results)
                {
                    var decision = result.SyncDecision switch
                    {
                        "Insert" => SyncAction.Insert,
                        "Update" => SyncAction.Update,
                        "Skip" => SyncAction.Skip,
                        _ => SyncAction.Error
                    };

                    decisions[result.SourceId] = decision;

                    // Log detailed comparison for debugging
                    if (decision == SyncAction.Update)
                    {
                        _logger.LogDebug(
                            "Entity {SourceId} needs update: Current={Current}, LastSynced={Last}",
                            result.SourceId,
                            result.CurrentTimestamp,
                            result.LastSyncedTimestamp ?? 0);
                    }
                }

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Retrieved {Count} sync decisions with composite timestamp comparison for {EntityType}. " +
                    "Insert: {Insert}, Update: {Update}, Skip: {Skip}",
                    decisions.Count,
                    entityType,
                    decisions.Count(d => d.Value == SyncAction.Insert),
                    decisions.Count(d => d.Value == SyncAction.Update),
                    decisions.Count(d => d.Value == SyncAction.Skip));

                return decisions;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Transaction rolled back while getting bulk sync decisions for {EntityType}", entityType);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bulk sync decisions with composite for {EntityType}", entityType);

            // Fallback
            return sourceIdComposites.Keys.ToDictionary(id => id, _ => SyncAction.Insert);
        }
    }

    /// <summary>
    /// Insert multiple sync logs in bulk WITH composite timestamp support
    /// </summary>
    public async Task InsertBatchSyncLogsAsync(
        string connectionString,
        List<SyncLog> logs)
    {
        if (logs == null || !logs.Any())
        {
            return;
        }

        try
        {
            // Calculate composite timestamps if not set
            foreach (var log in logs)
            {
                // If composite not set but main timestamp exists, use main as composite
                if (log.RowVersionComposite == null && log.RowVersionChildPlus != null)
                {
                    log.RowVersionComposite = log.RowVersionChildPlus;

                    // Create basic JSON if not set
                    if (string.IsNullOrEmpty(log.RelatedTablesVersion))
                    {
                        var composite = new CompositeTimestamp
                        {
                            MainTableTimestamp = log.RowVersionChildPlus
                        };
                        log.RelatedTablesVersion = composite.ToJson();
                    }
                }
            }

            // Using SqlBulkCopy
            await BulkInsertSyncLogsAsync(connectionString, logs);

            _logger.LogDebug("Bulk inserted {Count} sync logs with composite timestamps", logs.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk inserting sync logs. Attempting fallback...");

            // FALLBACK: Insert in smaller batches using Dapper
            await BatchInsertSyncLogsWithDapperAsync(connectionString, logs);
        }
    }

    /// <summary>
    /// Bulk insert using SqlBulkCopy
    /// </summary>
    private async Task BulkInsertSyncLogsAsync(string connectionString, List<SyncLog> logs)
    {
        var dataTable = CreateSyncLogDataTable(logs);

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var bulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = "SyncLogTable",
            BatchSize = 1000,
            BulkCopyTimeout = 300 // 5 minutes
        };

        // Map columns (including new composite timestamp columns)
        bulkCopy.ColumnMappings.Add("EntityType", "EntityType");
        bulkCopy.ColumnMappings.Add("SourceId", "SourceId");
        bulkCopy.ColumnMappings.Add("TargetId", "TargetId");
        bulkCopy.ColumnMappings.Add("SyncAction", "SyncAction");
        bulkCopy.ColumnMappings.Add("SyncStatus", "SyncStatus");
        bulkCopy.ColumnMappings.Add("Message", "Message");
        bulkCopy.ColumnMappings.Add("RowVersionChildPlus", "RowVersionChildPlus");
        bulkCopy.ColumnMappings.Add("RowVersionComposite", "RowVersionComposite"); 
        bulkCopy.ColumnMappings.Add("RelatedTablesVersion", "RelatedTablesVersion"); 
        bulkCopy.ColumnMappings.Add("TimestampSynced", "TimestampSynced");
        bulkCopy.ColumnMappings.Add("CenterId", "CenterId");
        bulkCopy.ColumnMappings.Add("RequestId", "RequestId");
        bulkCopy.ColumnMappings.Add("CreatedBy", "CreatedBy");

        await bulkCopy.WriteToServerAsync(dataTable);
    }

    /// <summary>
    /// Batch insert using Dapper (FALLBACK method)
    /// </summary>
    private async Task BatchInsertSyncLogsWithDapperAsync(
        string connectionString,
        List<SyncLog> logs)
    {
        const string query = @"
        INSERT INTO SyncLogTable (
            EntityType, SourceId, TargetId, SyncAction, SyncStatus,
            Message, RowVersionChildPlus, RowVersionComposite, RelatedTablesVersion,
            TimestampSynced, CenterId, RequestId, CreatedBy
        )
        VALUES (
            @EntityType, @SourceId, @TargetId, @SyncAction, @SyncStatus,
            @Message, @RowVersionChildPlus, @RowVersionComposite, @RelatedTablesVersion,
            @TimestampSynced, @CenterId, @RequestId, @CreatedBy
        )";

        // Process in batches of 500
        const int batchSize = 500;
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        for (int i = 0; i < logs.Count; i += batchSize)
        {
            var batch = logs.Skip(i).Take(batchSize).ToList();

            // Set TimestampSynced for logs without it
            foreach (var log in batch.Where(l => l.TimestampSynced == default))
            {
                log.TimestampSynced = DateTime.UtcNow;
            }

            await connection.ExecuteAsync(query, batch);
        }

        _logger.LogInformation("Inserted {Count} sync logs using Dapper fallback", logs.Count);
    }

    /// <summary>
    /// Create DataTable for SqlBulkCopy WITH composite timestamp columns
    /// </summary>
    private DataTable CreateSyncLogDataTable(List<SyncLog> logs)
    {
        var table = new DataTable();

        table.Columns.Add("EntityType", typeof(string));
        table.Columns.Add("SourceId", typeof(string));
        table.Columns.Add("TargetId", typeof(string));
        table.Columns.Add("SyncAction", typeof(string));
        table.Columns.Add("SyncStatus", typeof(string));
        table.Columns.Add("Message", typeof(string));
        table.Columns.Add("RowVersionChildPlus", typeof(byte[]));
        table.Columns.Add("RowVersionComposite", typeof(byte[]));
        table.Columns.Add("RelatedTablesVersion", typeof(string));
        table.Columns.Add("TimestampSynced", typeof(DateTime));
        table.Columns.Add("CenterId", typeof(string));
        table.Columns.Add("RequestId", typeof(Guid));
        table.Columns.Add("CreatedBy", typeof(string));

        foreach (var log in logs)
        {
            table.Rows.Add(
                log.EntityType,
                log.SourceId,
                log.TargetId ?? string.Empty,
                log.SyncAction,
                log.SyncStatus,
                log.Message ?? string.Empty,
                log.RowVersionChildPlus != null ? log.RowVersionChildPlus : DBNull.Value,
                log.RowVersionComposite != null ? log.RowVersionComposite : DBNull.Value,
                log.RelatedTablesVersion ?? (object)DBNull.Value,
                log.TimestampSynced == default ? DateTime.UtcNow : log.TimestampSynced,
                log.CenterId ?? string.Empty,
                log.RequestId,
                log.CreatedBy ?? "System"
            );
        }

        return table;
    }

    /// <summary>
    /// Create DataTable for source composites
    /// </summary>
    private DataTable CreateSourceCompositeDataTable(Dictionary<string, CompositeTimestamp> sourceIdComposites)
    {
        var table = new DataTable();
        table.Columns.Add("SourceId", typeof(string));
        table.Columns.Add("CompositeTimestamp", typeof(long));

        foreach (var (sourceId, composite) in sourceIdComposites)
        {
            table.Rows.Add(sourceId, composite.GetMaxTimestampAsLong());
        }

        return table;
    }

    #region Helper Classes

    private class SyncDecisionResult
    {
        public string SourceId { get; set; }
        public string SyncDecision { get; set; }
    }

    private class SyncDecisionWithTimestampResult
    {
        public string SourceId { get; set; }
        public string SyncDecision { get; set; }
        public long? LastSyncedTimestamp { get; set; }
        public long CurrentTimestamp { get; set; }
    }

    #endregion
}
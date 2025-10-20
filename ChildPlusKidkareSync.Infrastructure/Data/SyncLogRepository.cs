using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ChildPlusKidkareSync.Core.Models.Sync;
using ChildPlusKidkareSync.Core.Enums;

namespace ChildPlusKidkareSync.Infrastructure.Data;

// ==================== SYNC LOG REPOSITORY ====================
public interface ISyncLogRepository
{
    Task<SyncLog> GetLastSyncLogAsync(string connectionString, string entityType, string sourceId);
    Task<bool> ShouldSyncAsync(string connectionString, string entityType, string sourceId, byte[] timestamp);
    Task InsertSyncLogAsync(string connectionString, SyncLog log);
    Task InsertBatchSyncLogsAsync(string connectionString, List<SyncLog> logs);
}

public class SyncLogRepository : ISyncLogRepository
{
    private readonly ILogger<SyncLogRepository> _logger;

    public SyncLogRepository(ILogger<SyncLogRepository> logger)
    {
        _logger = logger;
    }

    public async Task<SyncLog> GetLastSyncLogAsync(string connectionString, string entityType, string sourceId)
    {
        try
        {
            const string query = @"
                SELECT TOP 1 * FROM SyncLogTable
                WHERE EntityType = @EntityType AND SourceId = @SourceId
                ORDER BY TimestampSynced DESC";

            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<SyncLog>(
                query,
                new { EntityType = entityType, SourceId = sourceId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting last sync log for {EntityType} {SourceId}", entityType, sourceId);
            return null;
        }
    }

    public async Task<bool> ShouldSyncAsync(string connectionString, string entityType, string sourceId, byte[] timestamp)
    {
        try
        {
            var lastLog = await GetLastSyncLogAsync(connectionString, entityType, sourceId);

            if (lastLog == null)
                return true;

            if (lastLog.TimestampChildPlus == null)
                return true;

            // Convert byte[] timestamp to DateTime for comparison
            var currentTimestamp = ConvertTimestampToDateTime(timestamp);
            return currentTimestamp > lastLog.TimestampChildPlus.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if should sync {EntityType} {SourceId}", entityType, sourceId);
            return true; // Default to sync on error
        }
    }

    public async Task InsertSyncLogAsync(string connectionString, SyncLog log)
    {
        try
        {
            const string query = @"
                INSERT INTO SyncLogTable 
                (EntityType, SourceId, TargetId, SyncAction, SyncStatus, Message, 
                 TimestampChildPlus, CenterId, RequestId, CreatedBy)
                VALUES 
                (@EntityType, @SourceId, @TargetId, @SyncAction, @SyncStatus, @Message, 
                 @TimestampChildPlus, @CenterId, @RequestId, @CreatedBy)";

            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting sync log for {EntityType} {SourceId}", log.EntityType, log.SourceId);
            throw;
        }
    }

    public async Task InsertBatchSyncLogsAsync(string connectionString, List<SyncLog> logs)
    {
        try
        {
            const string query = @"
                INSERT INTO SyncLogTable 
                (EntityType, SourceId, TargetId, SyncAction, SyncStatus, Message, 
                 TimestampChildPlus, CenterId, RequestId, CreatedBy)
                VALUES 
                (@EntityType, @SourceId, @TargetId, @SyncAction, @SyncStatus, @Message, 
                 @TimestampChildPlus, @CenterId, @RequestId, @CreatedBy)";

            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting batch sync logs");
            throw;
        }
    }

    private DateTime ConvertTimestampToDateTime(byte[] timestamp)
    {
        if (timestamp == null || timestamp.Length != 8)
            return DateTime.MinValue;

        // SQL Server rowversion/timestamp is a 8-byte binary value
        // Convert to long and then to DateTime
        var ticks = BitConverter.ToInt64(timestamp.Reverse().ToArray(), 0);
        return new DateTime(1900, 1, 1).AddTicks(ticks);
    }
}
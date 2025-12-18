namespace ChildPlusKidkareSync.Core.Models.Sync;

/// <summary>
/// SyncLog model with composite timestamp support
/// </summary>
public class SyncLog
{
    public int LogId { get; set; }
    public string EntityType { get; set; }
    public string SourceId { get; set; }
    public string TargetId { get; set; }
    public string SyncAction { get; set; }      // Insert, Update, Skip, Error
    public string SyncStatus { get; set; }      // Success, Failed
    public string Message { get; set; }

    // Main table timestamp (byte[] ROWVERSION)
    public byte[] RowVersionChildPlus { get; set; }

    // Composite timestamp (byte[] - MAX of all related timestamps)
    public byte[] RowVersionComposite { get; set; }

    // JSON detail of all related table timestamps
    public string RelatedTablesVersion { get; set; }

    public DateTime TimestampSynced { get; set; } = DateTime.UtcNow;
    public string CenterId { get; set; }
    public Guid RequestId { get; set; }
    public string CreatedBy { get; set; }
}

public class SyncResult
{
    public Guid RequestId { get; set; }
    public string TenantId { get; set; }
    public int TotalRecords { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public int SkippedCount { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<string> Errors { get; set; } = new();
    public bool IsSuccess => FailedCount == 0;
}
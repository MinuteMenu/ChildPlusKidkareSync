namespace ChildPlusKidkareSync.Core.Models.Sync;

public class SyncLog
{
    public int LogId { get; set; }
    public string EntityType { get; set; }
    public string SourceId { get; set; }
    public string TargetId { get; set; }
    public string SyncAction { get; set; }
    public string SyncStatus { get; set; }
    public string Message { get; set; }
    public DateTime? TimestampChildPlus { get; set; }
    public DateTime TimestampSynced { get; set; }
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
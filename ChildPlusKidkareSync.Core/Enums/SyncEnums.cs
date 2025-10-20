namespace ChildPlusKidkareSync.Core.Enums;

public enum SyncAction
{
    Insert,
    Update,
    Skip,
    Error
}

public enum SyncStatus
{
    Success,
    Failed
}

public enum EntityType
{
    Center,
    Staff,
    Child,
    Guardian,
    Enrollment,
    Attendance
}
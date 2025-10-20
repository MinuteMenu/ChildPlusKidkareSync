namespace ChildPlusKidkareSync.Core.Constants;

public static class SyncConstants
{
    public const string SystemName = "ChildPlus-Kidkare-Sync";

    public static class ApiEndpoints
    {
        public const string SaveCenter = "/cxSponsor/cxservice/center/saveCenter";
        public const string AddStaff = "/cxSponsor/centerstaff/add";
        public const string UpdateStaff = "/cxSponsor/centerstaff/update";
        public const string GetRole = "/cxSponsor/roles/list";
        public const string AddRole = "/cxSponsor/roles/add";
        public const string SavePermission = "/cxSponsor/permissions/save";
        public const string FinalizeImport = "/cxSponsor/child/finalizeImport";
    }

    public static class SqlQueries
    {
        public const string GetSites = @"
            SELECT AgencyID, CenterId, CenterName, Address, City, State, ZipCode, 
                   Phone, Email, Timestamp, LastModified
            FROM Sites
            WHERE AgencyID = @AgencyId AND IsActive = 1";

        public const string GetStaffByCenterId = @"
            SELECT StaffId, CenterId, FirstName, LastName, Email, Phone, 
                   Position, HireDate, IsActive, Timestamp, LastModified
            FROM Staff
            WHERE CenterId = @CenterId AND IsActive = 1";

        public const string GetChildrenByCenterId = @"
            SELECT ChildId, CenterId, FirstName, LastName, DateOfBirth, 
                   Gender, Address, City, State, ZipCode, Timestamp, LastModified
            FROM Children
            WHERE CenterId = @CenterId AND IsActive = 1";

        public const string GetGuardiansByChildId = @"
            SELECT GuardianId, ChildId, FirstName, LastName, Relationship, 
                   Phone, Email, IsPrimary, Timestamp, LastModified
            FROM Guardians
            WHERE ChildId = @ChildId";

        public const string GetEnrollmentsByChildId = @"
            SELECT EnrollmentId, ChildId, CenterId, EnrollmentDate, 
                   StartDate, EndDate, Status, Timestamp, LastModified
            FROM Enrollments
            WHERE ChildId = @ChildId";

        public const string GetAttendanceByChildId = @"
            SELECT AttendanceId, ChildId, CenterId, AttendanceDate, 
                   CheckInTime, CheckOutTime, Status, Timestamp, LastModified
            FROM Attendance
            WHERE ChildId = @ChildId 
            AND AttendanceDate >= DATEADD(MONTH, -3, GETDATE())";
    }
}
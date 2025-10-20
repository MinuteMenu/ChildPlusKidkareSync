namespace ChildPlusKidkareSync.Core.Models.ChildPlus
{
    public class ChildPlusAttendance
    {
        public string AttendanceId { get; set; }
        public string ChildId { get; set; }
        public string CenterId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public string Status { get; set; }
        public byte[] Timestamp { get; set; }
        public DateTime? LastModified { get; set; }
    }
}

namespace ChildPlusKidkareSync.Core.Models.ChildPlus
{
    public class ChildPlusEnrollment
    {
        public string EnrollmentId { get; set; }
        public string ChildId { get; set; }
        public string CenterId { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public byte[] Timestamp { get; set; }
        public DateTime? LastModified { get; set; }
    }
}

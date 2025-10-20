using ChildPlusKidkareSync.Core.Models.ChildPlus;

namespace ChildPlusKikareSync.Core.Models.ChildPlus
{
    public class ChildPlusChild
    {
        public string ChildId { get; set; }
        public string CenterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public byte[] Timestamp { get; set; }
        public DateTime? LastModified { get; set; }

        public List<ChildPlusGuardian> Guardians { get; set; } = new();
        public List<ChildPlusEnrollment> Enrollments { get; set; } = new();
        public List<ChildPlusAttendance> Attendance { get; set; } = new();
    }
}

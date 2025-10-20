namespace ChildPlusKidkareSync.Core.Models.ChildPlus
{
    public class ChildPlusGuardian
    {
        public string GuardianId { get; set; }
        public string ChildId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Relationship { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsPrimary { get; set; }
        public byte[] Timestamp { get; set; }
        public DateTime? LastModified { get; set; }
    }
}

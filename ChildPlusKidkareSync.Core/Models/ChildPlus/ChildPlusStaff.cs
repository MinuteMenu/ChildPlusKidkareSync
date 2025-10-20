namespace ChildPlusKidkareSync.Core.Models.ChildPlus
{
    public class ChildPlusStaff
    {
        public string StaffId { get; set; }
        public string CenterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public DateTime? HireDate { get; set; }
        public bool IsActive { get; set; }
        public byte[] Timestamp { get; set; }
        public DateTime? LastModified { get; set; }
    }
}

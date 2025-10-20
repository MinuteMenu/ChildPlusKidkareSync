namespace ChildPlusKidkareSync.Core.Models.ChildPlus;

public class ChildPlusSite
{
    public string CenterId { get; set; }
    public string CenterName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public byte[] Timestamp { get; set; }
    public DateTime? LastModified { get; set; }
}
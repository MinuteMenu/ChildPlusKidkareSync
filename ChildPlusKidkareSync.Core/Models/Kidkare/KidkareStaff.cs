using ChildPlusKidkareSync.Core.Enums;

namespace ChildPlusKidkareSync.Core.Models.Kidkare
{
    public class CenterStaffAddRequest : CentersRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public short? StaffTypeCode { get; set; }
    }

    public class CenterStaffUpdateRequest : CentersRequest
    {
        public CenterStaffModel centerStaff { get; set; }
        public bool EmailChanged { get; set; }
    }

    public class CenterStaffModel
    {
        public int StaffId { get; set; }
        public int ClientId { get; set; }
        public int CenterId { get; set; }
        public int UserId { get; set; }

        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string WorkPhone { get; set; }
        public string WorkPhoneExt { get; set; }
        public string HomePhone { get; set; }
        public string CellPhone { get; set; }

        public string AddressLine { get; set; }
        public string CityName { get; set; }
        public StateCode State { get; set; }
        public string ZipCode { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public int? StaffTypeCode { get; set; }
        public string StaffTypeName { get; set; }

        public Gender Gender { get; set; }

        public DateTime? HiredDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsCenterAdmin { get; set; }

        public int? ClassroomId { get; set; }
        public bool? IsInCustomRole { get; set; }

        public object UserPermissions { get; set; }
        public bool AccountingAccess { get; set; }
        public short? SponsorStaffTypeCode { get; set; }
    }
}

namespace ChildPlusKidkareSync.Core.Models.Kidkare
{
    public class RoleAddRequest : CentersRequest
    {
        public string RoleName { get; set; }
    }

    public class RoleAddResponse
    {
        public PermissionCategories Permissions { get; set; }
        public int RoleId { get; set; }
    }

    public class Permission
    {
        public bool? HasRight { get; set; }
        public int? RightId { get; set; }
        public string RightName { get; set; }
        public string RightCategory { get; set; }
    }

    public class PermissionCategories
    {
        public List<Permission> CenterAdminPermissions { get; set; }
        public List<Permission> FoodProgramPurchasesPermissions { get; set; }
        public List<Permission> ReportingPermissions { get; set; }
        public List<Permission> ChildrenPermissions { get; set; }
        public List<Permission> DeveloperPermissions { get; set; }

    }

    public class RolesListResponse
    {
        public List<RoleModel> Roles { get; set; }
    }

    public class RoleModel : CentersRequest
    {
        public int RoleCode { get; set; }
        public string RoleName { get; set; }
        public object Permissions { get; set; }
    }

    public class SaveStaffPermissionRequest : CentersRequest
    {
        public int UserId { get; set; }
        public int RightId { get; set; }
        public string RightName { get; set; }
        public bool HasRight { get; set; }
    }

    public static class RolePermissionsFactory
    {
        public static Dictionary<string, List<SaveStaffPermissionRequest>> InitializeRolesAndPermissionsForCenter(int centerId)
        {
            return new Dictionary<string, List<SaveStaffPermissionRequest>>()
            {
                // ═══════════════════════════════════════════════════════
                // TEACHER - Student & Attendance Management
                // ═══════════════════════════════════════════════════════
                {
                    "Teacher", new List<SaveStaffPermissionRequest>()
                    {
                        new SaveStaffPermissionRequest { CenterId = centerId, RightName = "Estimate Attendance", RightId = 35, HasRight = true, UserId = 0 },
                        new SaveStaffPermissionRequest { CenterId = centerId, RightName = "Record Center Attendance", RightId = 16, HasRight = true, UserId = 0 },
                        new SaveStaffPermissionRequest { CenterId = centerId, RightName = "Record Meal Delivery/Pickup", RightId = 149, HasRight = true, UserId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Modify Vendors/Receipts", rightId = 18, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "View Vendors/Receipts", rightId = 30, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Change Claim Month", rightId = 27, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Submit Center Claim", rightId = 25, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "View Claims", rightId = 3, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Online Enrollment", rightId = 114, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest { CenterId = centerId, RightName = "Record Center Menus", RightId = 17, HasRight = true, UserId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Apply Milk Audit", rightId = 13, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "View Milk Audit", rightId = 14, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Upgrade Software", rightId = 26, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "View/Modify Center Staff", rightId = 28, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Scan Forms", rightId = 31, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Assign Classrooms", rightId = 110, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Change Child Number", rightId = 46, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Delete Children", rightId = 22, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Enroll Children", rightId = 36, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Manage Formula Types", rightId = 45, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Modify Child Info", rightId = 9, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Reactivate Children", rightId = 39, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Show School Name", rightId = 112, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Withdraw Children", rightId = 40, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Send Messages (KidKare only)", rightId = 131, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: 5 Day Attendance Report", rightId = 141, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Actual vs Estimate Meal Count Summary", rightId = 65, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Blank Attendance + Meal Count Worksheet", rightId = 54, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Center Daily Meal Count Report", rightId = 140, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Daily Attendance + Meal Count Report", rightId = 53, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Estimated Meal Count Summary", rightId = 64, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: In/Out Times Daily Report", rightId = 66, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: In/Out Times Weekly Report", rightId = 67, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Local Ingredients Report", rightId = 147, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Monthly Claimed Attendance Only Report", rightId = 55, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Monthly Claimed Meal Count Summary", rightId = 56, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Monthly Claimed Meal Counts by Age Group", rightId = 57, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Monthly Claimed Meal Counts by Child", rightId = 58, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Monthly Paid Attendance Only Report", rightId = 59, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Monthly Paid Meal Counts by Age Group", rightId = 60, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Monthly Paid Meal Counts by Child", rightId = 62, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Weekly Attendance + Meal Count - One Class", rightId = 69, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Weekly Attendance + Meal Count Report", rightId = 52, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Weekly Paid Attendance + Meal Counts", rightId = 63, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Checkbook: Tax Summary", rightId = 86, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Blank Child Enrollment Form", rightId = 77, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Child IEF/Child Enrollment Report", rightId = 78, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Child List Export", rightId = 75, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Child Racial Counts Summary - Per Center", rightId = 81, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Child Roster", rightId = 73, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Children Claimed Without Absence", rightId = 84, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Children Not Claimed", rightId = 85, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: IEF List", rightId = 76, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Race and Ethnicity Report for Each Child in Attendance", rightId = 139, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: School Age Breakfast Approval", rightId = 83, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Verify FRP Consistent Within Family", rightId = 79, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Claims Payment Details (KidKare only)", rightId = 135, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Claims: Claim Error Report", rightId = 146, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Center Monthly Menu Plan", rightId = 92, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Center Weekly Menu", rightId = 93, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Center Weekly Menu - Infants Only", rightId = 95, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Center Weekly Menu - NonInfants Only", rightId = 94, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Daily Transportation Log", rightId = 143, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Infant Feeding Report", rightId = 142, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Master Menu Monthly Plan", rightId = 144, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Master Menu Weekly Plan", rightId = 96, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Menu Notes Report", rightId = 102, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Menu Production Record", rightId = 97, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Menu Production Record - Infants Only", rightId = 99, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Menu Production Record - NonInfants Only", rightId = 98, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Weekly Quantities Required - Per Center", rightId = 100, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Rates: CACFP Reimbursement Rates", rightId = 134, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Rates: IEF Poverty", rightId = 133, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Receipts: Blank Labor Tally Worksheet", rightId = 145, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Receipts: Center Receipts Journal", rightId = 104, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Receipts: Labor Tally Sheet", rightId = 106, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest { CenterId = centerId, RightName = "View log messages", RightId = 49, HasRight = true, UserId = 0 }
                    }
                },
                // ═══════════════════════════════════════════════════════
                // OWNER - View/Modify Access
                // ═══════════════════════════════════════════════════════
                { 
                    "Owner/Director", new List<SaveStaffPermissionRequest>()
                    {
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Estimate Attendance", rightId = 35, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest { CenterId = centerId, RightName = "Record Center Attendance", RightId = 16, HasRight = true, UserId = 0 },
                        new SaveStaffPermissionRequest { CenterId = centerId, RightName = "Record Meal Delivery/Pickup", RightId = 149, HasRight = true, UserId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Modify Vendors/Receipts", rightId = 18, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "View Vendors/Receipts", rightId = 30, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest { CenterId = centerId, RightName = "Change Claim Month", RightId = 27, HasRight = true, UserId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Submit Center Claim", RightId = 25, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "View Claims", RightId = 3, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Online Enrollment", RightId = 114, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Record Center Menus", RightId = 17, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Apply Milk Audit", rightId = 13, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "View Milk Audit", rightId = 14, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Upgrade Software", rightId = 26, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "View/Modify Center Staff", RightId = 28, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Scan Forms", RightId = 31, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Assign Classrooms", RightId = 110, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Change Child Number", rightId = 46, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Delete Children", RightId = 22, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Enroll Children", RightId = 36, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Manage Formula Types", RightId = 45, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Modify Child Info", RightId = 9, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Reactivate Children", RightId = 39, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Show School Name", RightId = 112, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Withdraw Children", RightId = 40, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Send Messages (KidKare only)", RightId = 131, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: 5 Day Attendance Report", RightId = 141, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Actual vs Estimate Meal Count Summary", RightId = 65, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Blank Attendance + Meal Count Worksheet", RightId = 54, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Center Daily Meal Count Report", RightId = 140, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Daily Attendance + Meal Count Report", RightId = 53, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Estimated Meal Count Summary", RightId = 64, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: In/Out Times Daily Report", RightId = 66, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: In/Out Times Weekly Report", RightId = 67, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Local Ingredients Report", rightId = 147, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Claimed Attendance Only Report", RightId = 55, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Claimed Meal Count Summary", RightId = 56, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Claimed Meal Counts by Age Group", RightId = 57, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Claimed Meal Counts by Child", RightId = 58, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Paid Attendance Only Report", RightId = 59, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Paid Meal Counts by Age Group", RightId = 60, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Paid Meal Counts by Child", RightId = 62, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Weekly Attendance + Meal Count - One Class", RightId = 69, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Weekly Attendance + Meal Count Report", RightId = 52, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Weekly Paid Attendance + Meal Counts", RightId = 63, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Checkbook: Tax Summary", RightId = 86, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Blank Child Enrollment Form", RightId = 77, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Child IEF/Child Enrollment Report", RightId = 78, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Child List Export", RightId = 75, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Child Racial Counts Summary - Per Center", RightId = 81, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Child Roster", RightId = 73, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Children Claimed Without Absence", RightId = 84, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Children Not Claimed", RightId = 85, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: IEF List", RightId = 76, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Race and Ethnicity Report for Each Child in Attendance", RightId = 139, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: School Age Breakfast Approval", RightId = 83, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Verify FRP Consistent Within Family", RightId = 79, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Claims Payment Details (KidKare only)", rightId = 135, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Claims: Claim Error Report", RightId = 146, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Center Monthly Menu Plan", RightId = 92, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Center Weekly Menu", RightId = 93, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Center Weekly Menu - Infants Only", RightId = 95, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Center Weekly Menu - NonInfants Only", RightId = 94, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Daily Transportation Log", rightId = 143, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Infant Feeding Report", RightId = 142, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Master Menu Monthly Plan", RightId = 144, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Master Menu Weekly Plan", RightId = 96, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Menu Notes Report", RightId = 102, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Menu Production Record", RightId = 97, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Menu Production Record - Infants Only", RightId = 99, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Menu Production Record - NonInfants Only", RightId = 98, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Weekly Quantities Required - Per Center", RightId = 100, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Rates: CACFP Reimbursement Rates", rightId = 134, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Rates: IEF Poverty", RightId = 133, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Receipts: Blank Labor Tally Worksheet", rightId = 145, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Receipts: Center Receipts Journal", RightId = 104, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Receipts: Labor Tally Sheet", RightId = 106, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "View log messages", RightId = 49, HasRight = true, UserId = 0}
                    }
                },
                // ═══════════════════════════════════════════════════════
                // ADMINISTRATOR - Full Access
                // ═══════════════════════════════════════════════════════
                { 
                    "Admin", new List<SaveStaffPermissionRequest>()
                    {
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Estimate Attendance", rightId = 35, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Record Center Attendance", RightId = 16, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Record Meal Delivery/Pickup", RightId = 149, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Modify Vendors/Receipts", rightId = 18, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "View Vendors/Receipts", rightId = 30, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Change Claim Month", RightId = 27, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Submit Center Claim", RightId = 25, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "View Claims", RightId = 3, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Online Enrollment", RightId = 114, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Record Center Menus", rightId = 17, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Apply Milk Audit", rightId = 13, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "View Milk Audit", rightId = 14, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Upgrade Software", rightId = 26, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "View/Modify Center Staff", rightId = 28, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Scan Forms", rightId = 31, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Assign Classrooms", RightId = 110, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Change Child Number", RightId = 46, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Delete Children", RightId = 22, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Enroll Children", RightId = 36, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Manage Formula Types", RightId = 45, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Modify Child Info", RightId = 9, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Reactivate Children", RightId = 39, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Show School Name", RightId = 112, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Withdraw Children", RightId = 40, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Send Messages (KidKare only)", RightId = 131, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: 5 Day Attendance Report", rightId = 141, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Actual vs Estimate Meal Count Summary", rightId = 65, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Blank Attendance + Meal Count Worksheet", rightId = 54, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Center Daily Meal Count Report", rightId = 140, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Daily Attendance + Meal Count Report", rightId = 53, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Attendance: Estimated Meal Count Summary", rightId = 64, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: In/Out Times Daily Report", RightId = 66, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: In/Out Times Weekly Report", RightId = 67, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Local Ingredients Report", RightId = 147, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Claimed Attendance Only Report", RightId = 55, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Claimed Meal Count Summary", RightId = 56, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Claimed Meal Counts by Age Group", RightId = 57, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Claimed Meal Counts by Child", RightId = 58, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Paid Attendance Only Report", RightId = 59, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Paid Meal Counts by Age Group", RightId = 60, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Monthly Paid Meal Counts by Child", RightId = 62, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Weekly Attendance + Meal Count - One Class", RightId = 69, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Weekly Attendance + Meal Count Report", RightId = 52, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Attendance: Weekly Paid Attendance + Meal Counts", RightId = 63, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Checkbook: Tax Summary", rightId = 86, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Blank Child Enrollment Form", rightId = 77, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Child IEF/Child Enrollment Report", rightId = 78, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Child List Export", rightId = 75, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Child Racial Counts Summary - Per Center", rightId = 81, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Child Roster", rightId = 73, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Children: Children Claimed Without Absence", rightId = 84, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Children Not Claimed", RightId = 85, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: IEF List", RightId = 76, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Race and Ethnicity Report for Each Child in Attendance", RightId = 139, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: School Age Breakfast Approval", RightId = 83, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Children: Verify FRP Consistent Within Family", RightId = 79, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Claims Payment Details (KidKare only)", rightId = 135, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Claims: Claim Error Report", RightId = 146, HasRight = true, UserId = 0},
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Center Monthly Menu Plan", rightId = 92, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Center Weekly Menu", rightId = 93, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Center Weekly Menu - Infants Only", rightId = 95, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Center Weekly Menu - NonInfants Only", rightId = 94, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Daily Transportation Log", rightId = 143, hasRight = false, userId = 0 },
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Infant Feeding Report", RightId = 142, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Master Menu Monthly Plan", RightId = 144, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Master Menu Weekly Plan", RightId = 96, HasRight = true, UserId = 0},
                        new SaveStaffPermissionRequest {CenterId = centerId, RightName = "Menus: Menu Notes Report", RightId = 102, HasRight = true, UserId = 0}
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Menu Production Record", rightId = 97, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Menu Production Record - Infants Only", rightId = 99, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Menu Production Record - NonInfants Only", rightId = 98, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Menus: Weekly Quantities Required - Per Center", rightId = 100, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Rates: CACFP Reimbursement Rates", rightId = 134, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Rates: IEF Poverty", rightId = 133, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Receipts: Blank Labor Tally Worksheet", rightId = 145, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Receipts: Center Receipts Journal", rightId = 104, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "Receipts: Labor Tally Sheet", rightId = 106, hasRight = false, userId = 0 },
                        //new SaveStaffPermissionRequest { centerId = centerId, rightName = "View log messages", rightId = 49, hasRight = false, userId = 0 }
                    }
                }
            };
        }
    }

}

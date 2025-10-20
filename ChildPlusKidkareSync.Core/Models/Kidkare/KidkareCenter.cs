using ChildPlusKidkareSync.Core.Enums;

namespace ChildPlusKidkareSync.Core.Models.Kidkare
{
    public class CenterSaveRequest
    {
        public PortCxCenterModel CenterModel { get; set; }
        public List<LicenseModel> LicenseList { get; set; }
        public List<ScheduleModel> ScheduleList { get; set; }
    }

    public class PortCxCenterModel
    {
        public PortCxGeneralModel General { get; set; } = new PortCxGeneralModel();
        public PortCxLicenseModel License { get; set; } = new PortCxLicenseModel();
        public PortCxOversightModel Oversight { get; set; } = new PortCxOversightModel();
        public AttendanceConfigurationModel AttendanceConfiguration { get; set; }
        public class PortCxGeneralModel
        {
            public PortCxCenterInfo CenterInfo { get; set; } = new PortCxCenterInfo();
            public PortCxPrimaryContactInfo PrimaryContact { get; set; }
            public PortCxCenterSiteInfo CenterSiteInfo { get; set; }
            public PortCxCenterNotesInfo CenterNotes { get; set; }
            public PortCxCenterBasicInfo CenterBasic { get; set; }
            public PortCxAdditionalInformation AdditionalInformation { get; set; }
            public PortCxFoodServiceInfo FoodServiceInfo { get; set; }

            public class PortCxCenterInfo
            {
                public int CenterId { get; set; }
                public string ExternalId { get; set; }
                public int? CenterNumber { get; set; }
                public string CenterName { get; set; }
                public string CorporationName { get; set; }
                public short Status { get; set; }
                public short BusinessType { get; set; }
                public short ProfitStatus { get; set; }
                public short ProfitTypeCode { get; set; }
            }

            public class PortCxPrimaryContactInfo
            {
                public string DirectorName { get; set; }
                public string Email { get; set; }
                public string PrimaryPhone { get; set; }
                public string AlternatePhone { get; set; }
                public string Fax { get; set; }
            }

            public class PortCxCenterSiteInfo
            {
                public string SiteAddress { get; set; }
                public string SiteCity { get; set; }
                public short SiteState { get; set; }
                public string SiteZipCode { get; set; }
                public int SiteCounty { get; set; }
                public int PrimarySchoolDistrict { get; set; }
                public string District { get; set; }
                public string CenterWebsite { get; set; }
                public string MailingAddress { get; set; }
                public string MailingCity { get; set; }
                public short MailingState { get; set; }
                public string MailingZipCode { get; set; }
            }

            public class PortCxCenterNotesInfo
            {
                public string Notes { get; set; }
            }

            public class PortCxCenterBasicInfo
            {
                public string StateAgreementNumber { get; set; }
                public string AlternateNumber { get; set; }
                public string FederalTaxID { get; set; }
                public string CenterTitleXX { get; set; }
                public string CenterTitleXIX { get; set; }
                public DateTime? CurrentStartDate { get; set; }
                public DateTime? CurrentEndDate { get; set; }
                public DateTime? AllowedStartDate { get; set; }
                public DateTime? OriginalStartDate { get; set; }
            }

            public class PortCxAdditionalInformation
            {
                public DateTime? SanitationInspectionDate { get; set; }
                public bool SanitationInspectionRequired { get; set; }
                public DateTime? HealthInspectionDate { get; set; }
                public bool HealthInspectionRequired { get; set; }
                public DateTime? FireInspectionDate { get; set; }
                public bool FireInspectionRequired { get; set; }
                public bool EnrichmentActivitiesOffered { get; set; }
                public bool EducationActivitiesOffered { get; set; }
                public string SchoolName { get; set; }
            }

            public class PortCxFoodServiceInfo
            {
                public short ServiceType { get; set; }
                public short ServiceStyle { get; set; }
                public decimal AnnualCost { get; set; }
                public string ContactName { get; set; }
                public string ContactPhone { get; set; }
                public string ContactEmail { get; set; }
            }
        }

        public class PortCxLicenseModel
        {
            public PortCxGeneralInfo GeneralInfo { get; set; } = new PortCxGeneralInfo();
            public List<PortCxLicenseInfo> LicenseInfo { get; set; } = new List<PortCxLicenseInfo>();
            public PortCxHourDayOpenInfo HourDayOpen { get; set; } = new PortCxHourDayOpenInfo();
            public PortCxMealScheduleInfo MealSchedule { get; set; } = new PortCxMealScheduleInfo();
            public PortCxNonCongregateMealInfo NonCongregateMeal { get; set; } = new PortCxNonCongregateMealInfo();

            public class PortCxGeneralInfo
            {
                public string StateSiteNumber { get; set; }
                public int ExtendedCapacity { get; set; }
                public bool RuralOrSelfPrepSite { get; set; }
                public int MasterMenuId { get; set; }
                public string MasterMenuName { get; set; }
                public string LicenseSharedMaxCapacityNumber { get; set; }
            }

            public class PortCxLicenseInfo
            {
                public int LicenseType { get; set; }
                public string LicenseNumber { get; set; }
                public short ProgramType { get; set; }
                public short FundingSource { get; set; }
                public short StartingAgeType { get; set; }
                public int StartingAgeNumber { get; set; }
                public short EndingAgeType { get; set; }
                public int EndingAgeNumber { get; set; }
                public DateTime? StartDate { get; set; }
                public DateTime? EndDate { get; set; }
                public string StateNumber { get; set; }

                public Dictionary<AgeGroupCode, int> MaxCapacity { get; set; }
                public int MaxCapacityTotal
                {
                    get { return MaxCapacity.ContainsKey(AgeGroupCode.MiscGroup) ? MaxCapacity[AgeGroupCode.MiscGroup] : 0; }
                }

                public bool AtRiskSFSPParticipant { get; set; }
                public string AtRiskSFSPNumber { get; set; }

                public List<MealType> ApprovedMeals { get; set; } = new List<MealType>();
                public List<MealType> AtRiskSFSPMeals { get; set; } = new List<MealType>();

                public bool? SchoolFlag { get; set; }
                public bool WaiverFlag { get; set; }
                public int? LicenseMaxApprovedMealNumber { get; set; }

                public List<MealType> WaiverMeals { get; set; } = new List<MealType>();
            }

            public class PortCxHourDayOpenInfo
            {
                public DateTime? OpeningTime { get; set; }
                public DateTime? ClosingTime { get; set; }
                public bool Open24Hours { get; set; }
                public DateTime? NightOpening { get; set; }
                public DateTime? NightClosing { get; set; }
                public List<DayOfWeek> DaysOpen { get; set; } = new List<DayOfWeek>();
                public List<Month> MonthsOpen { get; set; } = new List<Month>();
            }

            public class PortCxMealScheduleInfo
            {
                public short NumOfServing { get; set; }
                public Dictionary<MealType, List<PortCxMealServingTimesModel>> ServingTimes { get; set; } = new Dictionary<MealType, List<PortCxMealServingTimesModel>>();
            }

            public class PortCxNonCongregateMealInfo
            {
                public int ClientId { get; set; }
                public int CenterId { get; set; }
                public bool AllowMealServedFlag { get; set; }
                public bool AllowMondayFlag { get; set; }
                public bool AllowTuesdayFlag { get; set; }
                public bool AllowWednesdayFlag { get; set; }
                public bool AllowThursdayFlag { get; set; }
                public bool AllowFridayFlag { get; set; }
                public bool AllowSaturdayFlag { get; set; }
                public bool AllowSundayFlag { get; set; }
                public List<DayOfWeek> MealDays { get; set; } = new List<DayOfWeek>();
            }
        }

        public class PortCxOversightModel
        {
            public OversightCenterInfo CenterInfo { get; set; } = new OversightCenterInfo();
            public OversightCenterLogin CenterLogin { get; set; } = new OversightCenterLogin();
            public OversightSponsorNote SponsorNote { get; set; } = new OversightSponsorNote();
            public OversightCenterReferralInfo CenterReferralInfo { get; set; } = new OversightCenterReferralInfo();
            public OversightOtherInfo OtherInfo { get; set; } = new OversightOtherInfo();
            public OversightCenterPaymentInfo CenterPaymentInfo { get; set; } = new OversightCenterPaymentInfo();
            public OversightHoldReasonNote HoldReasonNote { get; set; } = new OversightHoldReasonNote();
            public OversightSiteMonitoringCenter MonitoringInfo { get; set; } = new OversightSiteMonitoringCenter();
            public OversightSiteRemovalInfo RemovalInfo { get; set; } = new OversightSiteRemovalInfo();

            public class OversightSiteMonitoringCenter
            {
                public int Monitor { get; set; }
                public DateTime? NextVisitDue { get; set; }
                public short StartMonth { get; set; }
            }

            public class OversightCenterInfo
            {
                public string DrivingInstructions { get; set; }
                public decimal MileageToCenter { get; set; }
                public string MapLocation { get; set; }
                public decimal OverrideAdminRate { get; set; }
                public bool UsesProcare { get; set; }
                public short AdministrationType { get; set; }
                public int OverrideEnrollmentExpirationMonth { get; set; }
                public bool CheckDailyInOutTime { get; set; }
                public short ClaimmingMethod { get; set; }
                public bool SkipMenuEditCheck { get; set; }
            }

            public class OversightCenterLogin
            {
                public string Username { get; set; }
                public string Password { get; set; }
                public bool IsChangePassword { get; set; }
            }

            public class OversightSponsorNote
            {
                public string Notes { get; set; }
            }

            public class OversightCenterReferralInfo
            {
                public string ReferredBy { get; set; }
                public string PreviousSponsorName { get; set; }
            }

            public class OversightOtherInfo
            {
                public short RecordAttendanceDateTimeLimitation { get; set; }
                public bool PreventCenterFromUsingSelectAllRecordAtt { get; set; }
                public bool CenterCanEnrollWithdrawReactiveChildren { get; set; }
                public bool RequireCenterEnterReceiptsFlag { get; set; }
            }

            public class OversightCenterPaymentInfo
            {
                public bool PayViaDirectDeposit { get; set; }
                public short BankAccountType { get; set; }
                public string BankRoutingNumber { get; set; }
                public string BankAccountNumber { get; set; }
            }

            public class OversightHoldReasonNote
            {
                public string Notes { get; set; }
            }

            public class OversightSiteRemovalInfo
            {
                public DateTime? RemovalDate { get; set; }
                public short RemovalReasonCode { get; set; }
            }
        }

        public class AttendanceConfigurationModel
        {
            public short ImportMethod { get; set; }
            public short? ImportSource { get; set; }
            public string Reason { get; set; }
        }
    }

    public class LicenseModel
    {
        public int LicenseType { get; set; }

        public ProgramTypeCode ProgramTypeCode { get; set; }

        public Dictionary<AgeGroupCode, int> MaxCapacity { get; set; }

        public MealType[] ApprovedMeals { get; set; }
        public MealType[] ApprovedMealsAtRisk { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool AtRisk { get; set; }

        public string StateNumber { get; set; }

        public bool? SchoolFlag { get; set; }
        public int StartingAgeNumber { get; set; }
        public AgeTypeCode StartingAgeType { get; set; }
        public int EndingAgeNumber { get; set; }
        public AgeTypeCode EndingAgeType { get; set; }
        public int? LicenseMaxApprovedMealNumber { get; set; }
    }

    public class ScheduleModel
    {
        public Month[] OpenMonths { get; set; }
        public DayOfWeek[] OpenDays { get; set; }
        public ServingModel Servings { get; set; }
        public Dictionary<MealType, List<MealServingTimesModel>> ServingTimes { get; set; }

        public DateTime? Opens { get; set; }
        public DateTime? Closes { get; set; }
    }

    public class PortCxMealServingTimesModel
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }

    public class MealServingTimesModel
    {
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }
    }

    public class ServingModel
    {
        public bool FirstServing { get; set; }
        public bool SecondServing { get; set; }
    }

    public class CentersRequest
    {
        public int UserId { get; set; }
        public int? CenterId { get; set; }
    }

    public class CenterResponse
    {
        public string CenterName { get; set; }
        public string CenterNumber { get; set; }
        public int CenterId { get; set; }

        public CenterStatusCode Status { get; set; }

        public StateCode State { get; set; }

        public bool IsPending { get; set; }
        public bool IsOpenEnrolled { get; set; }
        public bool IsRegular { get; set; }
        public IDictionary<string, bool> Settings { get; set; }
        public bool RequireCenterEnterReceiptsFlag { get; set; }
    }

    /// <summary>
    /// Result of center sync operation containing both action and response data
    /// </summary>
    public class CenterSyncResult
    {
        public SyncAction Action { get; set; }
        public CenterResponse CenterResponse { get; set; }
        public bool IsSuccess => Action == SyncAction.Insert || Action == SyncAction.Update;
    }
}

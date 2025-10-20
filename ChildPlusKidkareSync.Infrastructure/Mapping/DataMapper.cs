using ChildPlusKidkareSync.Core.Enums;
using ChildPlusKidkareSync.Core.Models.ChildPlus;
using ChildPlusKidkareSync.Core.Models.Kidkare;
using ChildPlusKidkareSync.Core.Models.Sync;
using ChildPlusKikareSync.Core.Models.ChildPlus;
using static ChildPlusKidkareSync.Core.Models.Kidkare.PortCxCenterModel;
using static ChildPlusKidkareSync.Core.Models.Kidkare.PortCxCenterModel.PortCxLicenseModel;

namespace ChildPlusKidkareSync.Infrastructure.Mapping
{
    // ==================== DATA MAPPER SERVICE ====================
    public interface IDataMapper
    {
        CenterSaveRequest MapToKidkareCenter(ChildPlusSite site);
        CenterStaffAddRequest MapToKidkareStaff(int centerId, string roleAbbr, RoleModel role, ChildPlusStaff staff);
        ParseResult<CxChildModel> MapToKidkareChild(ChildPlusChild child, int rowNumber = 0);
    }

    public class DataMapper : IDataMapper
    {
        public CenterSaveRequest MapToKidkareCenter(ChildPlusSite site)
        {
            var center = new CenterSaveRequest
            {
                CenterModel = new PortCxCenterModel
                {
                    //    General = new PortCxCenterModel.PortCxGeneralModel
                    //    {
                    //        CenterInfo = new PortCxCenterModel.PortCxGeneralModel.PortCxCenterInfo
                    //        {
                    //            ExternalId = site.Id,
                    //            CenterName = site.Name, //dbo.CENTER.center_name
                    //            CenterNumber = TryParseInt(site.NCESID) is var n && n > 0 ? n : GenerateCenterNumber(site.Name), //dbo.CENTER.center_number
                    //            CorporationName = site.SchoolType,  //dbo.CENTER.corporation_name
                    //            Status = (short)CenterStatusCode.Active,    //dbo.CENTER.center_status_code
                    //            ProfitStatus = MapProfitStatus(site.ProvisionStatus),   //dbo.CENTER.profit_status_code
                    //            ProfitTypeCode = 0,
                    //            BusinessType = MapBusinessType(site.SchoolType) //dbo.CENTER.business_type_code
                    //        },

                    //        PrimaryContact = new PortCxCenterModel.PortCxGeneralModel.PortCxPrimaryContactInfo
                    //        {
                    //            DirectorName = BuildFullName(site.ContactPerson),
                    //            Email = site.ContactEmail?.Address ?? string.Empty, //dbo.CENTER.email
                    //            PrimaryPhone = site.ContactPhone?.Number ?? string.Empty, //dbo.CENTER.phone
                    //            Fax = string.Empty, //dbo.CENTER.fax_phone
                    //            AlternatePhone = site.ContactPhone?.Description ?? string.Empty //dbo.CENTER.alternate_phone
                    //        },

                    //        CenterSiteInfo = new PortCxCenterModel.PortCxGeneralModel.PortCxCenterSiteInfo
                    //        {
                    //            // Mailing address
                    //            MailingAddress = site.ContactAddress?.Line1 ?? string.Empty,                                    //dbo.CENTER.mailing_address_line
                    //            MailingCity = site.ContactAddress?.City ?? string.Empty,                                        //dbo.CENTER.mailing_city_name
                    //            MailingState = MapState(site.ContactAddress?.State),                                            //dbo.CENTER.mailing_state_code
                    //            MailingZipCode = site.ContactAddress?.Zip ?? site.ContactAddress?.PostalCode ?? string.Empty,   //dbo.CENTER.mailing_zip_code

                    //            // Site (physical) address
                    //            SiteAddress = site.ContactAddress?.Line1 ?? string.Empty,                                       //dbo.CENTER.address_line
                    //            SiteCity = site.ContactAddress?.City ?? string.Empty,                                           //dbo.CENTER.city_name
                    //            SiteState = MapState(site.ContactAddress?.State),                                               //dbo.CENTER.state_code
                    //            SiteZipCode = site.ContactAddress?.Zip ?? site.ContactAddress?.PostalCode ?? string.Empty,      //dbo.CENTER.zip_code

                    //            PrimarySchoolDistrict = 12663,                                                                  //dbo.CENTER.school_district_id    (hard-code: Oklahoma Public School District)
                    //            SiteCounty = 982,                                                                               //dbo.CENTER.county_id       (hard-code: Oklahoma County)

                    //            CenterWebsite = site.Url ?? string.Empty  //dbo.CENTER.url
                    //        },

                    //        CenterBasic = new PortCxCenterModel.PortCxGeneralModel.PortCxCenterBasicInfo
                    //        {
                    //            AllowedStartDate = null,                        //dbo.CENTER.cacfp_allowed_start_date
                    //            StateAgreementNumber = site.StatePrId,          //dbo.CENTER.state_assigned_number
                    //            AlternateNumber = site.SiteUid,                 //dbo.CENTER.alternate_assigned_number
                    //            CenterTitleXX = site.Title1Status,              //dbo.CENTER.title_xx_number
                    //            CenterTitleXIX = string.Empty,                  //dbo.CENTER.title_xix_number
                    //            CurrentStartDate = site.LastDayOfFirstQuarter,  //dbo.CENTER.cacfp_current_agreement_start_date
                    //            CurrentEndDate = site.GraduationDate,           //dbo.CENTER.cacfp_current_expiration_start_date
                    //            OriginalStartDate = site.LastDayOfFirstQuarter  //dbo.CENTER.cacfp_original_start_date
                    //        },

                    //        CenterNotes = new PortCxCenterModel.PortCxGeneralModel.PortCxCenterNotesInfo(),

                    //        AdditionalInformation = new PortCxCenterModel.PortCxGeneralModel.PortCxAdditionalInformation
                    //        {
                    //            SchoolName = string.Empty,                                  //dbo.CENTER.school_care_school_name
                    //            EducationActivitiesOffered = (site.LunchMinutes ?? 0) > 0,  //dbo.CENTER.school_care_activities_education_flag
                    //            SanitationInspectionRequired = false,                       //dbo.CENTER.sanitation_required_flag
                    //            HealthInspectionRequired = false,                           //dbo.CENTER.health_inspection_required_flag
                    //            FireInspectionRequired = false,                             //dbo.CENTER.fire_inspection_required_flag
                    //            EnrichmentActivitiesOffered = false,                        //dbo.CENTER.school_care_activities_enrichment_flag
                    //            FireInspectionDate = null,                                  //dbo.CENTER.fire_inspection_expiration_date
                    //            HealthInspectionDate = null,                                //dbo.CENTER.health_inspection_expiration_date
                    //            SanitationInspectionDate = null                             //dbo.CENTER.sanitation_expiration_date
                    //        },

                    //        FoodServiceInfo = new PortCxCenterModel.PortCxGeneralModel.PortCxFoodServiceInfo
                    //        {
                    //            ServiceType = MapFoodServiceType("Food Service Management Company"), //dbo.CENTER.food_service_type_code
                    //            ServiceStyle = MapMealServiceType("Unit (Cafeteria)"),               //dbo.CENTER.meal_service_type_code
                    //            AnnualCost = 0,                                                      //dbo.CENTER.food_service_vendor_approx_annual_cost
                    //            ContactName = BuildFullName(site.PrincipalPerson),                   //dbo.CENTER.food_service_vendor_contact_name
                    //            ContactPhone = site.PrincipalPhone?.Number ?? string.Empty,          //dbo.CENTER.food_service_vendor_contact_phone
                    //            ContactEmail = site.PrincipalEmail?.Address ?? string.Empty          //dbo.CENTER.food_service_vendor_contact_email
                    //        }
                    //    },

                    //    License = new PortCxLicenseModel
                    //    {
                    //        GeneralInfo = new PortCxGeneralInfo
                    //        {
                    //            ExtendedCapacity = 0,
                    //            RuralOrSelfPrepSite = site.OperationalStatus?.Contains("Rural", StringComparison.OrdinalIgnoreCase) == true
                    //                                    || site.ProvisionStatus?.Contains("SelfPrep", StringComparison.OrdinalIgnoreCase) == true,
                    //            MasterMenuId = 0,
                    //            LicenseSharedMaxCapacityNumber = site.Grades?.Count.ToString() ?? string.Empty,
                    //            StateSiteNumber = site.StatePrId
                    //        },

                    //        LicenseInfo = new List<PortCxLicenseInfo>
                    //            {
                    //                new PortCxLicenseInfo
                    //                {
                    //                    LicenseType = 57,
                    //                    ProgramType = (short)ProgramTypeCode.AdultCare,
                    //                    FundingSource = (short)FundingSourceCode.NA,
                    //                    StartDate = null,
                    //                    EndDate = null,
                    //                    MaxCapacity = new Dictionary<AgeGroupCode, int>
                    //                    {
                    //                        { AgeGroupCode.Infant,0 },
                    //                        { AgeGroupCode.Toddler,0 },
                    //                        { AgeGroupCode.Preschool,0 },
                    //                        { AgeGroupCode.SchoolAge,0 },
                    //                        { AgeGroupCode.MiscGroup, 1 }
                    //                    },
                    //                    StartingAgeNumber = 1,
                    //                    StartingAgeType = (short)AgeTypeCode.Years,
                    //                    EndingAgeNumber = 13,
                    //                    EndingAgeType = (short)AgeTypeCode.Years,
                    //                    AtRiskSFSPParticipant = false,
                    //                    ApprovedMeals = new List<MealType>(),
                    //                }
                    //            },
                    //        HourDayOpen = new PortCxHourDayOpenInfo()
                    //        {
                    //            DaysOpen = new List<DayOfWeek>(),
                    //            MonthsOpen = new List<Month>(),
                    //            OpeningTime = null,
                    //            ClosingTime = null,
                    //            NightOpening = null,
                    //            NightClosing = null
                    //        },
                    //        MealSchedule = new PortCxMealScheduleInfo
                    //        {
                    //            NumOfServing = 1,
                    //            ServingTimes = new Dictionary<MealType, List<PortCxMealServingTimesModel>>()
                    //        },
                    //        NonCongregateMeal = new PortCxNonCongregateMealInfo
                    //        {
                    //            ClientId = 0,
                    //            CenterId = 0,
                    //            AllowMealServedFlag = false,
                    //            AllowMondayFlag = false,
                    //            AllowTuesdayFlag = false,
                    //            AllowWednesdayFlag = false,
                    //            AllowThursdayFlag = false,
                    //            AllowFridayFlag = false,
                    //            AllowSaturdayFlag = false,
                    //            AllowSundayFlag = false,
                    //            MealDays = new List<DayOfWeek>()
                    //        }
                    //    },

                    //    Oversight = new PortCxOversightModel
                    //    {
                    //        CenterInfo = new PortCxOversightModel.OversightCenterInfo
                    //        {
                    //            AdministrationType = MapAdministrationType("Unknown"), //dbo.CENTER.administration_type_code
                    //            OverrideAdminRate = site.OperationalStatus?.Contains("Inactive", StringComparison.OrdinalIgnoreCase) == true ? 0 : -1,
                    //            MapLocation = string.Empty  //dbo.CENTER.map_location
                    //        },
                    //        CenterLogin = new PortCxOversightModel.OversightCenterLogin
                    //        {
                    //            IsChangePassword = false,
                    //            Username = GenerateUniqueIdentifier(),  // Generate dynamic username
                    //            Password = GenerateSecurePassword()     // Generate dynamic password
                    //        },
                    //        OtherInfo = new PortCxOversightModel.OversightOtherInfo
                    //        {
                    //            RecordAttendanceDateTimeLimitation = (short)(site.AllowTeacherOverrideAttendance ? 1647 : 0),
                    //            PreventCenterFromUsingSelectAllRecordAtt = false,
                    //            CenterCanEnrollWithdrawReactiveChildren = site.AllowTeacherOverrideEligibility,
                    //            RequireCenterEnterReceiptsFlag = false
                    //        },
                    //        CenterPaymentInfo = new PortCxOversightModel.OversightCenterPaymentInfo
                    //        {
                    //            PayViaDirectDeposit = false,
                    //            BankAccountType = 0
                    //        },
                    //        HoldReasonNote = new PortCxOversightModel.OversightHoldReasonNote
                    //        {
                    //            Notes = string.Empty    //dbo.CENTER.hold_reason_text
                    //        },
                    //        MonitoringInfo = new PortCxOversightModel.OversightSiteMonitoringCenter
                    //        {
                    //            Monitor = 0,    //dbo.CENTER.monitor_staff_id
                    //            StartMonth = 515,
                    //            NextVisitDue = null
                    //        },
                    //        CenterReferralInfo = new PortCxOversightModel.OversightCenterReferralInfo
                    //        {
                    //            PreviousSponsorName = string.Empty          //dbo.CENTER.previous_sponsor_name
                    //        },
                    //        RemovalInfo = new PortCxOversightModel.OversightSiteRemovalInfo
                    //        {
                    //            RemovalDate = null,                                     //dbo.CENTER.removal_date
                    //            RemovalReasonCode = (short)RemovalReasonCode.Unknown    //dbo.CENTER.removal_reason_code
                    //        }
                    //    }
                    //AttendanceConfiguration = new PortCxCenterModel.AttendanceConfigurationModel
                    //{
                    //    ImportMethod = site.AllowTeacherOverrideAttendance ? (short)1 : (short)0,
                    //    Reason = site.AllowTeacherOverrideEligibility ? "Teacher Override Eligibility" : string.Empty
                    //}
                },
                LicenseList = new List<LicenseModel>(),
                ScheduleList = new List<ScheduleModel>()
            };
            return center;
        }

        public CenterStaffAddRequest MapToKidkareStaff(int centerId, string roleAbbr, RoleModel role, ChildPlusStaff staff)
        {
            return new CenterStaffAddRequest
            {
                FirstName = $"{staff.FirstName}{staff.LastName}",
                LastName = roleAbbr,
                StaffTypeCode = (short)role.RoleCode,
                Email = staff.Email ?? string.Empty,
                CenterId = centerId,
                UserId = 0
            };
        }

        public ParseResult<CxChildModel> MapToKidkareChild(ChildPlusChild child, int rowNumber = 0)
        {
            // Create CxChildModel
            var cxChild = new CxChildModel
            {
                Id = 0,//child.ChildId,
                CenterId = int.TryParse(child.CenterId, out var centerId) ? centerId : 0,
                FirstName = child.FirstName,
                LastName = child.LastName,
                BirthDate = child.DateOfBirth,
                //Gender = (short)child.Gender,
                //Contacts = child.Guardians.Select(MapToGuardianRequest).ToList(),
                //Enrollments = child.Enrollments.Select(MapToEnrollmentRequest).ToList(),
                //Attendance = child.Attendance.Select(MapToAttendanceRequest).ToList()
            };

            // Wrap in ParseResult
            return new ParseResult<CxChildModel>
            {
                RowNumber = rowNumber,              // Track position in batch
                Result = cxChild,                   // The data
                Errors = new List<Error>()          // Empty for valid data
            };
        }
    }

    public static class MappingHelper
    {
        public static bool YN2B(this string value)
        {
            return value == "Y";
        }
    }
}
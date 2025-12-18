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
                    General = new PortCxCenterModel.PortCxGeneralModel
                    {
                        CenterInfo = new PortCxGeneralModel.PortCxCenterInfo
                        {
                            ExternalId = site.SiteId,
                            CenterName = site.SiteName,                                 //dbo.CENTER.center_name
                            CenterNumber = 0,                                           //dbo.CENTER.center_number
                            CorporationName = site.corporation_name,                    //dbo.CENTER.corporation_name
                            Status = (short)CenterStatusCode.Active,                    //dbo.CENTER.center_status_code
                            ProfitStatus = MapProfitStatus(site.profit_status_code),    //dbo.CENTER.profit_status_code
                            ProfitTypeCode = (short)ProfitTypeCode.Undefined,           //dbo.CENTER.profit_type_code
                            BusinessType = MapBusinessType(site.business_type_code)     //dbo.CENTER.business_type_code
                        },

                        PrimaryContact = new PortCxGeneralModel.PortCxPrimaryContactInfo
                        {
                            DirectorName = site.director_name_text,                 //dbo.CENTER.director_name_text
                            Email = site.email ?? string.Empty,                     //dbo.CENTER.email
                            PrimaryPhone = site.phone ?? string.Empty,              //dbo.CENTER.phone
                            Fax = site.fax_phone ?? string.Empty,                   //dbo.CENTER.fax_phone
                            AlternatePhone = site.alternate_phone ?? string.Empty   //dbo.CENTER.alternate_phone
                        },

                        CenterSiteInfo = new PortCxGeneralModel.PortCxCenterSiteInfo
                        {
                            // Mailing address
                            MailingAddress = site.mailing_address_line ?? string.Empty, //dbo.CENTER.mailing_address_line
                            MailingCity = site.mailing_city_name ?? string.Empty,       //dbo.CENTER.mailing_city_name
                            MailingState = MapState(site.mailing_state_code),           //dbo.CENTER.mailing_state_code
                            MailingZipCode = site.mailing_zip_code ?? string.Empty,     //dbo.CENTER.mailing_zip_code

                            // Site (physical) address
                            SiteAddress = site.address_line ?? string.Empty,    //dbo.CENTER.address_line
                            SiteCity = site.city_name ?? string.Empty,          //dbo.CENTER.city_name
                            SiteState = MapState(site.state_code),              //dbo.CENTER.state_code
                            SiteZipCode = site.zip_code ?? string.Empty,        //dbo.CENTER.zip_code

                            PrimarySchoolDistrict = 12663,              //dbo.CENTER.school_district_id    (hard-code: Oklahoma Public School District)
                            SiteCounty = 982,                           //dbo.CENTER.county_id       (hard-code: Oklahoma County)

                            CenterWebsite = site.url ?? string.Empty    //dbo.CENTER.url
                        },

                        CenterBasic = new PortCxGeneralModel.PortCxCenterBasicInfo
                        {
                            AllowedStartDate = site.cacfp_allowed_start_date,               //dbo.CENTER.cacfp_allowed_start_date
                            StateAgreementNumber = site.state_assigned_number,              //dbo.CENTER.state_assigned_number
                            AlternateNumber = site.alternate_assigned_number ?? string.Empty, //dbo.CENTER.alternate_assigned_number
                            CenterTitleXX = site.title_xx_number ?? string.Empty,             //dbo.CENTER.title_xx_number
                            CenterTitleXIX = site.title_xix_number ?? string.Empty,           //dbo.CENTER.title_xix_number
                            CurrentStartDate = site.cacfp_current_agreement_start_date,     //dbo.CENTER.cacfp_current_agreement_start_date
                            CurrentEndDate = site.cacfp_current_expiration_start_date,      //dbo.CENTER.cacfp_current_expiration_start_date
                            OriginalStartDate = site.cacfp_original_start_date              //dbo.CENTER.cacfp_original_start_date
                        },

                        CenterNotes = new PortCxGeneralModel.PortCxCenterNotesInfo(),

                        AdditionalInformation = new PortCxGeneralModel.PortCxAdditionalInformation
                        {
                            SchoolName = site.school_care_school_name ?? string.Empty,                  //dbo.CENTER.school_care_school_name
                            EducationActivitiesOffered = site.school_care_activities_education_flag,    //dbo.CENTER.school_care_activities_education_flag
                            SanitationInspectionRequired = site.sanitation_required_flag,               //dbo.CENTER.sanitation_required_flag
                            HealthInspectionRequired = site.health_inspection_required_flag,            //dbo.CENTER.health_inspection_required_flag
                            FireInspectionRequired = site.fire_inspection_required_flag,                //dbo.CENTER.fire_inspection_required_flag
                            EnrichmentActivitiesOffered = site.school_care_activities_enrichment_flag,  //dbo.CENTER.school_care_activities_enrichment_flag
                            FireInspectionDate = site.fire_inspection_expiration_date,                  //dbo.CENTER.fire_inspection_expiration_date
                            HealthInspectionDate = site.health_inspection_expiration_date,              //dbo.CENTER.health_inspection_expiration_date
                            SanitationInspectionDate = site.sanitation_expiration_date                  //dbo.CENTER.sanitation_expiration_date
                        },

                        FoodServiceInfo = new PortCxGeneralModel.PortCxFoodServiceInfo
                        {
                            ServiceType = MapFoodServiceType(site.food_service_type_code),          //dbo.CENTER.food_service_type_code
                            ServiceStyle = MapMealServiceType(site.meal_service_type_code),         //dbo.CENTER.meal_service_type_code
                            AnnualCost = site.food_service_vendor_approx_annual_cost ?? 0,          //dbo.CENTER.food_service_vendor_approx_annual_cost
                            ContactName = site.food_service_vendor_contact_name ?? string.Empty,    //dbo.CENTER.food_service_vendor_contact_name
                            ContactPhone = site.food_service_vendor_contact_phone ?? string.Empty,  //dbo.CENTER.food_service_vendor_contact_phone
                            ContactEmail = site.food_service_vendor_contact_email ?? string.Empty   //dbo.CENTER.food_service_vendor_contact_email
                        }
                    },

                    License = new PortCxLicenseModel
                    {
                        GeneralInfo = new PortCxGeneralInfo
                        {
                            ExtendedCapacity = site.extended_capacity ?? 0,                                         //dbo.CENTER.extended_capacity
                            RuralOrSelfPrepSite = site.RuralOrSelfPrepSite ?? false,                                //dbo.CenterExtendedProperty.RuralOrSelfPrepSite
                            MasterMenuId = site.MasterMenuId ?? 0,                                                  //dbo.MasterMenuClaim.MasterMenuId
                            LicenseSharedMaxCapacityNumber = site.LicenseSharedMaxCapacityNumber ?? string.Empty,   //dbo.CenterExtendedProperty.LicenseSharedMaxCapacityNumber
                            StateSiteNumber = site.StateSiteNumber ?? string.Empty                                  //dbo.CenterExtendedProperty.StateSiteNumber
                        },

                        LicenseInfo = new List<PortCxLicenseInfo>
                        {
                            new PortCxLicenseInfo
                            {
                                LicenseType = 57,                                                           //dbo.CENTER_LICENSE.license_id
                                ProgramType = (short)MapChildPlusProgram(site.program_type_name),           //dbo.CENTER_LICENSE.program_type_code
                                FundingSource = (short)MapChildPlusFundingSource(site.funding_source_code), //dbo.CENTER_LICENSE.funding_source_code
                                StartDate = site.license_start_date,                                        //dbo.CENTER_LICENSE.license_start_date
                                EndDate = site.license_end_date,                                            //dbo.CENTER_LICENSE.license_end_date
                                MaxCapacity = new Dictionary<AgeGroupCode, int>
                                {
                                    { AgeGroupCode.Infant,site.license_max_infants ?? 0 },              //dbo.CENTER_LICENSE.license_max_infants
                                    { AgeGroupCode.Toddler,site.license_max_toddlers ?? 0 },            //dbo.CENTER_LICENSE.license_max_toddlers
                                    { AgeGroupCode.Preschool,site.license_max_preschoolers ?? 0 },      //dbo.CENTER_LICENSE.license_max_preschoolers
                                    { AgeGroupCode.SchoolAge,site.license_max_schoolagers ?? 0 },       //dbo.CENTER_LICENSE.license_max_schoolagers
                                    { AgeGroupCode.MiscGroup, site.license_max_capacity_number ?? 1 }   //dbo.CENTER_LICENSE.license_max_capacity_number
                                },
                                StartingAgeNumber = site.allowed_starting_age_number ?? 1,                  //dbo.CENTER_LICENSE.allowed_starting_age_number
                                StartingAgeType = (short)MapAgeType(site.allowed_starting_age_type_code),   //dbo.CENTER_LICENSE.allowed_starting_age_type_code
                                EndingAgeNumber = site.allowed_ending_age_number ?? 1,                      //dbo.CENTER_LICENSE.allowed_ending_age_number
                                EndingAgeType = (short)MapAgeType(site.allowed_ending_age_type_code),       //dbo.CENTER_LICENSE.allowed_ending_age_type_code
                                AtRiskSFSPParticipant = site.at_risk_flag ?? false,                         //dbo.CENTER_LICENSE.at_risk_flag
                                ApprovedMeals = new List<MealType>(),
                            }
                        },
                        HourDayOpen = MapHourDayOpen(site),
                        MealSchedule = MapMealSchedule(site),
                        NonCongregateMeal = new PortCxNonCongregateMealInfo
                        {
                            ClientId = 0,
                            CenterId = 0,
                            AllowMealServedFlag = site.allow_meal_served_flag?? false,  //dbo.CENTER_NON_CONGREGATE_MEAL.allow_meal_served_flag
                            AllowMondayFlag = site.allow_monday_flag ?? false,          //dbo.CENTER_NON_CONGREGATE_MEAL.allow_monday_flag
                            AllowTuesdayFlag = site.allow_tuesday_flag ?? false,        //dbo.CENTER_NON_CONGREGATE_MEAL.allow_tuesday_flag
                            AllowWednesdayFlag = site.allow_wednesday_flag ?? false,    //dbo.CENTER_NON_CONGREGATE_MEAL.allow_wednesday_flag
                            AllowThursdayFlag = site.allow_thursday_flag ?? false,      //dbo.CENTER_NON_CONGREGATE_MEAL.allow_thursday_flag
                            AllowFridayFlag = site.allow_friday_flag ?? false,          //dbo.CENTER_NON_CONGREGATE_MEAL.allow_friday_flag
                            AllowSaturdayFlag = site.allow_saturday_flag ?? false,      //dbo.CENTER_NON_CONGREGATE_MEAL.allow_saturday_flag
                            AllowSundayFlag = site.allow_sunday_flag ?? false,          //dbo.CENTER_NON_CONGREGATE_MEAL.allow_sunday_flag
                            MealDays = new List<DayOfWeek>()
                        }
                    },

                    Oversight = new PortCxOversightModel
                    {
                        CenterInfo = new PortCxOversightModel.OversightCenterInfo
                        {
                            AdministrationType = MapAdministrationType(site.administration_type_code),  //dbo.CENTER.administration_type_code
                            OverrideAdminRate = site.admin_rate ?? - 1,                                 //dbo.CENTER.admin_rate
                            MapLocation = site.map_location ?? string.Empty                             //dbo.CENTER.map_location
                        },
                        CenterLogin = new PortCxOversightModel.OversightCenterLogin
                        {
                            IsChangePassword = false,
                            Username = GenerateUniqueIdentifier(),  // Generate dynamic username
                            Password = GenerateSecurePassword()     // Generate dynamic password
                        },
                        OtherInfo = new PortCxOversightModel.OversightOtherInfo
                        {
                            RecordAttendanceDateTimeLimitation = MapAttendanceDateTimeLimitation(site.day_of_entry_requirement_code),   //dbo.CENTER.day_of_entry_requirement_code
                            PreventCenterFromUsingSelectAllRecordAtt = site.prevent_select_all_record_attendance_flag ?? false,         //dbo.CENTER.prevent_select_all_record_attendance_flag
                            CenterCanEnrollWithdrawReactiveChildren = site.allow_center_override_enrollment_flag ?? false,              //dbo.CENTER.allow_center_override_enrollment_flag
                            RequireCenterEnterReceiptsFlag = site.require_center_enter_receipts_flag ?? false                           //dbo.CENTER.require_center_enter_receipts_flag
                        },
                        CenterPaymentInfo = new PortCxOversightModel.OversightCenterPaymentInfo
                        {
                            PayViaDirectDeposit = site.use_direct_deposit_flag ?? false,    //dbo.CENTER.use_direct_deposit_flag
                            BankAccountType = MapBankAccountCode(site.bank_account_code),   //dbo.CENTER.bank_account_code
                            BankAccountNumber = site.bank_account_number,                   //dbo.CENTER.bank_account_number
                            BankRoutingNumber = site.bank_routing_number,                   //dbo.CENTER.bank_routing_number
                        },
                        HoldReasonNote = new PortCxOversightModel.OversightHoldReasonNote
                        {
                            Notes = site.hold_reason_text ?? string.Empty    //dbo.CENTER.hold_reason_text
                        },
                        MonitoringInfo = new PortCxOversightModel.OversightSiteMonitoringCenter
                        {
                            Monitor = !string.IsNullOrEmpty(site.monitor_staff_id) ? 0 : -1,    //dbo.CENTER.monitor_staff_id
                            StartMonth = MapReviewYearStartMonth(site.review_year_start_month), //dbo.CENTER.review_year_start_month
                            NextVisitDue = site.next_review_required_date                       //dbo.CENTER.next_review_required_date
                        },
                        CenterReferralInfo = new PortCxOversightModel.OversightCenterReferralInfo
                        {
                            ReferredBy = site.referred_by_text ?? string.Empty,                 //dbo.CENTER.referred_by_text
                            PreviousSponsorName = site.previous_sponsor_name ?? string.Empty    //dbo.CENTER.previous_sponsor_name
                        },
                        RemovalInfo = new PortCxOversightModel.OversightSiteRemovalInfo
                        {
                            RemovalDate = site.removal_date,                        //dbo.CENTER.removal_date
                            RemovalReasonCode = (short)RemovalReasonCode.Unknown    //dbo.CENTER.removal_reason_code
                        }
                    },
                    AttendanceConfiguration = new PortCxCenterModel.AttendanceConfigurationModel
                    {
                        ImportMethod = (short)MapAttendanceImportMethod(site.import_method),    //dbo.ATTENDANCE_CONFIGURATION.import_method
                        ImportSource = (short)MapAttendanceImportMethod(site.import_source),    //dbo.ATTENDANCE_CONFIGURATION.import_source
                        Reason = site.attendance_reason ?? string.Empty                         //dbo.ATTENDANCE_CONFIGURATION.reason
                    }
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

        private static short MapProfitStatus(string profit) => profit?.Trim().ToLowerInvariant() switch
        {
            "non-profit" or "nonprofit" or "non profit" => 1610, // Non-Profit
            "for-profit" or "forprofit" or "for profit" => 1611, // For-Profit
            _ => 1610 // default Non-Profit
        };

        private static short MapBusinessType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return 1631; // Default: Corporation

            var t = type.Trim().ToLowerInvariant();

            if (t.Contains("sole proprietorship") || t.Contains("sole-proprietorship"))
                return 1629; // Sole Proprietorship

            if (t.Contains("partnership"))
                return 1630; // Partnership

            if (t.Contains("corporation") || t.Contains("inc") || t.Contains("incorporated"))
                return 1631; // Corporation

            if (t.Contains("llc") || t.Contains("limited liability company"))
                return 1632; // LLC

            if (t.Contains("public entity") || t.Contains("government"))
                return 1633; // Public Entity (Government)

            if (t.Contains("church") || t.Contains("affiliated"))
                return 1704; // Church Affiliated

            if (t.Contains("private non profit") || t.Contains("private nonprofit") || t.Contains("non-profit"))
                return 1705; // Private Non Profit

            return 1631; // Default fallback: Corporation
        }

        private static short MapState(string stateCode)
        {
            if (string.IsNullOrWhiteSpace(stateCode))
                return (short)StateCode.OK; // Default to Oklahoma

            var code = stateCode.Trim().ToUpperInvariant();

            // Try to parse using enum names (e.g. "OK", "TX", "CA")
            if (System.Enum.TryParse<StateCode>(code, out var parsedState))
                return (short)parsedState;

            switch (code)
            {
                case "OKLAHOMA":
                case "OKLA":
                case "OKLA.":
                    return (short)StateCode.OK;

                case "TEXAS":
                    return (short)StateCode.TX;

                case "CALIFORNIA":
                    return (short)StateCode.CA;

                case "ARKANSAS":
                    return (short)StateCode.AR;

                case "KANSAS":
                    return (short)StateCode.KS;

                case "MISSOURI":
                    return (short)StateCode.MO;

                case "LOUISIANA":
                    return (short)StateCode.LA;

                case "NEW YORK":
                    return (short)StateCode.NY;

                case "FLORIDA":
                    return (short)StateCode.FL;

                case "COLORADO":
                    return (short)StateCode.CO;

                // extend if needed

                default:
                    return (short)StateCode.OK; // fallback default
            }
        }

        private static short MapFoodServiceType(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return 1609; // Default: Food Service Management Company

            var t = code.Trim().ToLowerInvariant();

            if (t.Contains("central kitchen") || t.Contains("central") || t.Contains("kitchen"))
                return 1607; // Central Kitchen

            if (t.Contains("on-site preparation") || t.Contains("on-site") || t.Contains("preparation"))
                return 1606; // On-Site Preparation

            if (t.Contains("food service management company") ||
                t.Contains("service") ||
                t.Contains("management") ||
                t.Contains("company"))
            {
                return 1608; // Food Service Management Company
            }

            if (t.Contains("school food authority") || t.Contains("authority"))
                return 1608; // School Food Authority

            return 1609; // Fallback default
        }

        private static short MapMealServiceType(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return 1623; // Default: Unit (Cafeteria)

            var t = code.Trim().ToLowerInvariant();

            if (t.Contains("unit") || t.Contains("cafeteria"))
                return 1623; // Unit (Cafeteria)

            if (t.Contains("family"))
                return 1624; // Family

            return 1623; // Fallback default
        }

        private static ProgramTypeCode MapChildPlusProgram(string childPlusProgram)
        {
            return childPlusProgram.ToLower() switch
            {
                "head start" => ProgramTypeCode.HeadStart,
                "early head start" => ProgramTypeCode.ChildCare,
                "migrant and seasonal" => ProgramTypeCode.HeadStart, // subtype MSHS
                "pre-kindergarten" => ProgramTypeCode.ChildCare,
                "day care" => ProgramTypeCode.ChildCare,
                "other" => ProgramTypeCode.Unknown,
                _ => ProgramTypeCode.Unknown
            };
        }

        private static FundingSourceCode MapChildPlusFundingSource(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return FundingSourceCode.NA;

            source = source.Trim().ToLower();

            return source switch
            {
                "acf head start" => FundingSourceCode.Migrant,
                "acf early head start" => FundingSourceCode.Migrant,
                "acf head start preschool" => FundingSourceCode.StatePreschool,
                _ => FundingSourceCode.NA
            };
        }

        private static AgeTypeCode MapAgeType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return AgeTypeCode.Undefined;

            return type.Trim().ToUpper() switch
            {
                "W" or "WK" or "WEEK" or "WEEKS" => AgeTypeCode.Weeks,
                "M" or "MO" or "MON" or "MONTH" or "MONTHS" => AgeTypeCode.Months,
                "Y" or "YR" or "YRS" or "YEAR" or "YEARS" => AgeTypeCode.Years,
                _ => AgeTypeCode.Undefined
            };
        }

        private static PortCxHourDayOpenInfo MapHourDayOpen(ChildPlusSite site)
        {
            if (site == null) return new PortCxHourDayOpenInfo();

            var daysOpen = new List<DayOfWeek>();
            if (site.allow_monday_flag == true) daysOpen.Add(DayOfWeek.Monday);         //dbo.CENTER_SCHEDULE.allow_monday_flag
            if (site.allow_tuesday_flag == true) daysOpen.Add(DayOfWeek.Tuesday);       //dbo.CENTER_SCHEDULE.allow_tuesday_flag
            if (site.allow_wednesday_flag == true) daysOpen.Add(DayOfWeek.Wednesday);   //dbo.CENTER_SCHEDULE.allow_wednesday_flag
            if (site.allow_thursday_flag == true) daysOpen.Add(DayOfWeek.Thursday);     //dbo.CENTER_SCHEDULE.allow_thursday_flag
            if (site.allow_friday_flag == true) daysOpen.Add(DayOfWeek.Friday);         //dbo.CENTER_SCHEDULE.allow_friday_flag
            if (site.allow_saturday_flag == true) daysOpen.Add(DayOfWeek.Saturday);     //dbo.CENTER_SCHEDULE.allow_saturday_flag
            if (site.allow_sunday_flag == true) daysOpen.Add(DayOfWeek.Sunday);         //dbo.CENTER_SCHEDULE.allow_sunday_flag

            var monthsOpen = new List<Month>();
            if (site.operates_january_flag == true) monthsOpen.Add(Month.January);      //dbo.CENTER_SCHEDULE.operates_january_flag
            if (site.operates_february_flag == true) monthsOpen.Add(Month.February);    //dbo.CENTER_SCHEDULE.operates_february_flag
            if (site.operates_march_flag == true) monthsOpen.Add(Month.March);          //dbo.CENTER_SCHEDULE.operates_march_flag
            if (site.operates_april_flag == true) monthsOpen.Add(Month.April);          //dbo.CENTER_SCHEDULE.operates_april_flag
            if (site.operates_may_flag == true) monthsOpen.Add(Month.May);              //dbo.CENTER_SCHEDULE.operates_may_flag
            if (site.operates_june_flag == true) monthsOpen.Add(Month.June);            //dbo.CENTER_SCHEDULE.operates_june_flag
            if (site.operates_july_flag == true) monthsOpen.Add(Month.July);            //dbo.CENTER_SCHEDULE.operates_july_flag
            if (site.operates_august_flag == true) monthsOpen.Add(Month.August);        //dbo.CENTER_SCHEDULE.operates_august_flag
            if (site.operates_september_flag == true) monthsOpen.Add(Month.September);  //dbo.CENTER_SCHEDULE.operates_september_flag
            if (site.operates_october_flag == true) monthsOpen.Add(Month.October);      //dbo.CENTER_SCHEDULE.operates_october_flag
            if (site.operates_november_flag == true) monthsOpen.Add(Month.November);    //dbo.CENTER_SCHEDULE.operates_november_flag
            if (site.operates_december_flag == true) monthsOpen.Add(Month.December);    //dbo.CENTER_SCHEDULE.operates_december_flag

            return new PortCxHourDayOpenInfo
            {
                DaysOpen = daysOpen,
                MonthsOpen = monthsOpen,
                OpeningTime = site.open_time,                   //dbo.CENTER_SCHEDULE.open_time
                ClosingTime = site.close_time,                  //dbo.CENTER_SCHEDULE.close_time
                NightOpening = site.night_open_time,            //dbo.CENTER_SCHEDULE.night_open_time
                NightClosing = site.night_close_time,           //dbo.CENTER_SCHEDULE.night_close_time
                Open24Hours = site.open_24_hours_flag ?? false  //dbo.CENTER_SCHEDULE.open_24_hours_flag
            };
        }

        private static PortCxMealScheduleInfo MapMealSchedule(ChildPlusSite site)
        {
            if (site == null)
                return new PortCxMealScheduleInfo();

            var result = new PortCxMealScheduleInfo
            {
                NumOfServing = site.highest_serving_allowed_code,   //dbo.CENTER_SCHEDULE.highest_serving_allowed_code
                ServingTimes = new Dictionary<MealType, List<PortCxMealServingTimesModel>>()
            };

            var mealNames = new Dictionary<MealType, string>
            {
                { MealType.Breakfast, "breakfast" },
                { MealType.AmSnack, "am_snack" },
                { MealType.Lunch, "lunch" },
                { MealType.PmSnack, "pm_snack" },
                { MealType.Dinner, "dinner" },
                { MealType.EveningSnack, "evening_snack" }
            };

            var siteType = typeof(ChildPlusSite);

            foreach (var meal in mealNames)
            {
                var times = new List<PortCxMealServingTimesModel>();

                for (int i = 1; i <= 3; i++)
                {
                    var prefix = i switch
                    {
                        1 => "first",
                        2 => "second",
                        3 => "third",
                        _ => throw new InvalidOperationException()
                    };

                    var startProp = siteType.GetProperty($"{prefix}_standard_{meal.Value}_time");   //dbo.CENTER_SCHEDULE.xxx_standard_yyy_time
                    var endProp = siteType.GetProperty($"{prefix}_ending_{meal.Value}_time");       //dbo.CENTER_SCHEDULE.xxx_ending_yyy_time

                    var start = (DateTime?)startProp?.GetValue(site);
                    var end = (DateTime?)endProp?.GetValue(site);

                    times.Add(new PortCxMealServingTimesModel
                    {
                        Start = start,
                        End = end
                    });
                }

                result.ServingTimes[meal.Key] = times;
            }

            return result;
        }

        private static short MapAdministrationType(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return (short)AdministrationTypeCode.Unknown; // Default: Unknown

            var t = code.Trim().ToLowerInvariant();

            if (t.Contains("separate"))
                return (short)AdministrationTypeCode.SeparatedFromSponsor; // SeparatedFromSponsor

            if (t.Contains("affiliate"))
                return (short)AdministrationTypeCode.AffiliatedWithSponsor; // AffiliatedWithSponsor

            return (short)AdministrationTypeCode.Unknown; // Fallback default
        }

        // Method to generate a unique username
        private static string GenerateUniqueIdentifier()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10); // Shortened GUID for a unique 10-character username
        }

        // Method to generate a secure password using Random
        private static string GenerateSecurePassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_=+!@#$%^*";
            Random random = new Random();
            char[] password = new char[10]; // Password length can be adjusted

            for (int i = 0; i < password.Length; i++)
            {
                password[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(password);
        }

        private static short MapAttendanceDateTimeLimitation(string value) => value?.Trim().ToLowerInvariant() switch
        {
            "none" or "unknown" => (short)RecordAttendanceDateTimeLimitation.None,                              // None
            "by end of day" => (short)RecordAttendanceDateTimeLimitation.ByEndOfDay,                            // By End of Day
            "during meal service times" => (short)RecordAttendanceDateTimeLimitation.DuringMealServiceTimes,    // During Meal Service Times
            "by end of week" => (short)RecordAttendanceDateTimeLimitation.ByEndOfWeek,                          // By End of Week
            _ => (short)RecordAttendanceDateTimeLimitation.None                                                 // default None
        };

        private static short MapBankAccountCode(string code) => code?.Trim().ToLowerInvariant() switch
        {
            "none" or "unknown" => (short)BankAccountCode.Undefined,    // unknown
            "checking" => (short)BankAccountCode.Checking,              // Checking
            "savings" => (short)BankAccountCode.Savings,                // Savings
            "money market" => (short)BankAccountCode.MoneyMarket,       // Money Market
            _ => (short)BankAccountCode.Undefined                       // default Unknown
        };

        private static short MapReviewYearStartMonth(string value)
        {
            var t = value?.Trim().ToLowerInvariant() ?? string.Empty;

            return t switch
            {
                var x when x.Contains("jan") || x.Contains("1") => (short)MonthCode.Jan,
                var x when x.Contains("feb") || x.Contains("2") => (short)MonthCode.Feb,
                var x when x.Contains("mar") || x.Contains("3") => (short)MonthCode.Mar,
                var x when x.Contains("apr") || x.Contains("4") => (short)MonthCode.Apr,
                var x when x.Contains("may") || x.Contains("5") => (short)MonthCode.May,
                var x when x.Contains("jun") || x.Contains("6") => (short)MonthCode.Jun,
                var x when x.Contains("jul") || x.Contains("7") => (short)MonthCode.Jul,
                var x when x.Contains("aug") || x.Contains("8") => (short)MonthCode.Aug,
                var x when x.Contains("sep") || x.Contains("9") => (short)MonthCode.Sep,
                var x when x.Contains("oct") || x.Contains("10") => (short)MonthCode.Oct,
                var x when x.Contains("nov") || x.Contains("11") => (short)MonthCode.Nov,
                var x when x.Contains("dec") || x.Contains("12") => (short)MonthCode.Dec,
                _ => 0
            };
        }

        private static CxAttendanceImportMethod MapAttendanceImportMethod(string code) => code?.Trim().ToLowerInvariant() switch
        {
            "manual" => CxAttendanceImportMethod.Manual,
            "auto" => CxAttendanceImportMethod.Auto,
            "partially" => CxAttendanceImportMethod.Partially,
            _ => CxAttendanceImportMethod.Manual                               
        };

      
    }

    public static class MappingHelper
    {
        public static bool YN2B(this string value)
        {
            return value == "Y";
        }
    }
}
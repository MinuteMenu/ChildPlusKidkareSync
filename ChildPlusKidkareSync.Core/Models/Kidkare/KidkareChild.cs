using ChildPlusKidkareSync.Core.Enums;
using ChildPlusKidkareSync.Infrastructure.Mapping;

namespace ChildPlusKidkareSync.Core.Models.Kidkare
{
    public class CxChildModel
    {
        public int ClientId { get; set; }
        public int CenterId { get; set; }
        public int Id { get; set; }
        public string ChildNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public CxClassroomModel Classroom { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? OriginalEnrollmentDate { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? EnrollmentExpiration { get; set; }
        public DateTime? EnrollmentReportPrintedDate { get; set; }
        public DateTime? FoodProgramStart { get; set; }
        public PaymentSource? PaymentSource { get; set; }
        public DateTime? PaySourceStart { get; set; }
        public DateTime? PaySourceEnd { get; set; }
        public string ChildComment { get; set; }
        public List<Race> Races { get; set; }
        public Ethnicity Ethnicity { get; set; }
        public bool HasSpecialNeeds { get; set; }
        public bool IsChildOfMigrantWorker { get; set; }
        public bool HasSpecialDiet { get; set; }
        public string SpecialDietNotes { get; set; }

        public string DoctorsName { get; set; }
        public string DoctorsPhone { get; set; }
        public bool WillChildStayOvernight { get; set; }
        public string SponsorCommentText { get; set; }
        public DateTime? TitlexxExpirationDate { get; set; }
        public bool? SaveParents { get; set; }
        public List<CxContactModel> Contacts { get; set; }
        public int LanguageCode { get; set; }
        public CxChildAttendanceModel ChildAttendance { get; set; }
        public bool AtRisk { get; set; }
        public DateTime? DevelopmentalReadyDate { get; set; }
        public List<MealType> Meals { get; set; }
        public CxChildSchoolModel ChildSchool { get; set; }
        public CxChildInfantFormulaModel ChildInfantFormula { get; set; }
        public CxChildAllergiesModel ChildAllergies { get; set; }
        public CxMedicalContact Doctor { get; set; }
        public ChildStatus Status { get; set; }
        public bool IsCacfpParticipant { get; set; }
        public bool BreakfastDocumentationRcvd { get; set; }
        public CxImmunizationModel Immunization { get; set; }
        public decimal InOutTimes { get; set; }
        public decimal Balance { get; set; }
        public FrpCategory ReimbursementLevel { get; set; }
        public FrpEligibilityType FrpBasis { get; set; }
        public string QualifyingProgram { get; set; } //benefits_program_case_number
        public DateTime? IefExpirationDate { get; set; }
        public string NewClassroom { get; set; }
        public ChildImage ChildImage { get; set; }

        public DateTime? WithdrawDate { get; set; }
        public string ExternalChildId { get; set; }
        public string ExternalFamilyId { get; set; }

        public bool SpecialNeedsOnFile { get; set; }
        public bool HasMilkAllergy { get; set; }
        public bool IsParentProvideMilkallergy { get; set; }
        public bool IsIssuedOfOklahoma { get; set; }
        public bool UseSubstituteMilk { get; set; }
        public DateTime? MilkallergysettingEffectivedate { get; set; }
        public DateTime? MilkallergysettingExpirationdate { get; set; }
        public List<CxFormModel> Forms { get; set; }
        public DateTime IncomeEligibilityFormSignatureDate { get; set; }
        public int? EnrollmentInvitationId { get; set; }
        public string ContentType { get; set; }
        public string ImageLink { get; set; }
        public List<ClientPolicyModel> ClientPolicy { get; set; }
        public DateTime? SpecialDietExpiration { get; set; }
        public bool SpecialDietOnFile { get; set; }
        //public SIBLINGDataTable SiblingGrid { get; set; }
        public List<CodeDetailListItem> ChildIncomeSignatureDates { get; set; }
        public List<CodeDetailListItem> FRBCategoryCodes { get; set; }
        public List<CodeDetailListItem> FRBEligibilityCodes { get; set; }
        public List<CodeDetailListItem> QualPrograms { get; set; }
        public List<CodeDetailListItem> FosterIncomeFreqs { get; set; }
        public List<CodeDetailListItem> InComeSourceTypeCodes { get; set; }
        //public List<IEFItem> ChildIncomes { get; set; }
        public bool RequestNewIEF { get; set; } // request_ief_flag

        public bool RequestIEF { get; set; }

        public bool IsCMK { get; set; }
        public DateTime? GapCurrentDate { get; set; }
        public DateTime? GapExpirationDate { get; set; }
        public string ClassroomText { get; set; }
        public short? ClientSourceCode { get; set; }
        public int? ClaimErrorId { get; set; }
        public bool NeverActivatedFlag { get; set; }
        public bool IsMissingData
        {
            get
            {
                var sD24 = ClientPolicy?.FirstOrDefault(x => x.PolicyName == Policy.D24)?.PolicyValue;
                EnforceCode pD24;
                Enum.TryParse(sD24, out pD24);
                var sD17 = ClientPolicy?.FirstOrDefault(x => x.PolicyName == Policy.D17)?.PolicyValue;
                CxIgnoreChildInfoCode pD17;
                Enum.TryParse(sD17, out pD17);
                var sD17b = ClientPolicy?.FirstOrDefault(x => x.PolicyName == Policy.D17b)?.PolicyValue;

                bool isMissingCommon = string.IsNullOrWhiteSpace(FirstName)
                    || string.IsNullOrWhiteSpace(LastName)
                    || (Classroom == null && string.IsNullOrEmpty(NewClassroom))
                    || !BirthDate.HasValue
                    || (BirthDate.Value.AddYears(1) > DateTime.UtcNow.Date
                        && !ChildInfantFormula.InfantFormOnFile
                        && (pD24 == EnforceCode.Disallow || pD24 == EnforceCode.Warn));

                bool isNotSponsoredChildAtrisk = ChildAttendance == null || !ChildAttendance.AtRiskAfterSchool;
                bool notRequiredRaceEthnicity = sD17b.YN2B() && (pD17 == CxIgnoreChildInfoCode.Race
                    || pD17 == CxIgnoreChildInfoCode.RaceScheduleGender
                    || pD17 == CxIgnoreChildInfoCode.RaceGender
                    || pD17 == CxIgnoreChildInfoCode.All);

                if (!IsCMK)
                {
                    return isMissingCommon
                        || !EnrollmentDate.HasValue
                        || !EnrollmentExpiration.HasValue
                        || !FoodProgramStart.HasValue
                        || EnrollmentDate < BirthDate
                        || EnrollmentExpiration < DateTime.Today
                        || EnrollmentExpiration < EnrollmentDate
                        || (IefExpirationDate.HasValue && IefExpirationDate < DateTime.Today)
   
                        || (IsCacfpParticipant && (Meals == null || !Meals.Any())
                        || (IsCacfpParticipant && ReimbursementLevel == FrpCategory.None))
                        || (Contacts == null || !Contacts.Any())
                        || (Contacts.Count(x => x.PrimaryGuardian) != 1)
                        || (Contacts != null && Contacts.Any(r => string.IsNullOrWhiteSpace(r.FirstName)
                                                                || string.IsNullOrWhiteSpace(r.LastName)
                                                                || !r.Type.HasValue || r.Type.Value == CxPrimaryGuardianCode.Unknown
                                                                //#259094 KKFP: Red Exclamation Points appear for children with no issues
                                                                //|| (r.PrimaryGuardian && string.IsNullOrWhiteSpace(r.Phone))
                                                                || (r.PrimaryGuardian && string.IsNullOrWhiteSpace(r.Address))
                                                                || (r.PrimaryGuardian && string.IsNullOrWhiteSpace(r.City))
                                                                || (r.PrimaryGuardian && (!r.State.HasValue || r.State.Value == StateCode.Unknown))
                                                                //|| (r.PrimaryGuardian && string.IsNullOrWhiteSpace(r.Zip))
                                                                ));
                }
                else
                {
                    var sF2b = ClientPolicy?.FirstOrDefault(x => x.PolicyName == Policy.F2b)?.PolicyValue;
                    var sS1 = ClientPolicy?.FirstOrDefault(x => x.PolicyName == Policy.S1)?.PolicyValue;
                    var isNotAtriskOnly = !isAtRiskOnly;
                    var childWithdraw = Status == ChildStatus.Withdrawn;

                    return isMissingCommon
                        || !FoodProgramStart.HasValue
                        || (!EnrollmentDate.HasValue && (sD17b.YN2B() || pD17 != CxIgnoreChildInfoCode.All))
                        || (!EnrollmentExpiration.HasValue && isNotSponsoredChildAtrisk)
                        || (string.IsNullOrWhiteSpace(ChildNumber) && isNotSponsoredChildAtrisk && sS1.YN2B())
                        || (EnrollmentExpiration.HasValue && EnrollmentExpiration < DateTime.Today)
                        || (IefExpirationDate.HasValue && IefExpirationDate < DateTime.Today)
                        // Contact
                        || (isNotSponsoredChildAtrisk
                            && (Contacts == null || !Contacts.Any()
                            || (Contacts.Count(x => x.PrimaryGuardian) != 1)
                            || (Contacts != null && Contacts.Any(r => isNotAtriskOnly && (string.IsNullOrWhiteSpace(r.FirstName)
                                    || string.IsNullOrWhiteSpace(r.LastName)
                                    || (r.PrimaryGuardian && (string.IsNullOrWhiteSpace(r.Phone)
                                                            || string.IsNullOrWhiteSpace(r.Address)
                                                            || string.IsNullOrWhiteSpace(r.City)
                                                            || !r.State.HasValue || r.State.Value == StateCode.Unknown)
                                    )
                                    )
                                    || !r.Type.HasValue || r.Type.Value == CxPrimaryGuardianCode.Unknown))
                                    ))
                        // Schedule
                        || (ChildAttendance == null && (!sD17b.YN2B()
                            || (pD17 != CxIgnoreChildInfoCode.Schedule && pD17 != CxIgnoreChildInfoCode.RaceScheduleGender
                            && pD17 != CxIgnoreChildInfoCode.All)))
                        // Cacfp
                        || (IsCacfpParticipant && (Meals == null || !Meals.Any()) && isNotSponsoredChildAtrisk && isNotAtriskOnly
                            && !notRequiredRaceEthnicity)
                        || (IsCacfpParticipant && ReimbursementLevel == FrpCategory.None
                            && sF2b.YN2B() && isNotSponsoredChildAtrisk
                            && Status != ChildStatus.Pending);
                    // Demographics
                    // 305448 Exclamation mark appears for 'Unknown' Ethnicity children
                    //|| ((Races.All(x => x == Race.None) || Ethnicity == Ethnicity.None) && !notRequiredRaceEthnicity && isNotAtriskOnly && isNotSponsoredChildAtrisk);
                }
            }
        }

        private string homePhone;
        public string HomePhone
        {
            get
            {
                return !string.IsNullOrWhiteSpace(homePhone) ? homePhone :
                    Contacts.Where(r => r.PrimaryGuardian).Select(r => r.Phone).FirstOrDefault();
            }
            set { homePhone = value; }
        }

        public int LicenseId { get; set; }
        public short ProgramTypeCode { get; set; }
        public DateTime? LicenseSwitchDate { get; set; }
        public bool isAtRiskOnly { get; set; }

        public string Tab { get; set; }
    }

    public class CxClassroomModel
    {
        public int ClassroomId { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string BuildingName { get; set; }
    }

    public class CxContactModel
    {
        public int ContactId { get; set; }
        public bool PrimaryGuardian { get; set; }
        public bool CanPickup { get; set; }
        public CxPrimaryGuardianCode? Type { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string WorkPhone { get; set; }
        public string WorkPhoneExtension { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public StateCode? State { get; set; }
        public string Comments { get; set; }
        public int ClientId { get; set; }
        public int CenterId { get; set; }
        public bool EmailChanged { get; set; }
        public string SSNTax { get; set; }
    }

    public class CxChildAttendanceModel
    {
        public bool TimesVary { get; set; }
        public bool AtRiskAfterSchool { get; set; }
        public bool WillChildStayOvernight { get; set; }
        public List<DayOfWeek> DaysAttendance { get; set; }
        public CxDaysInCareModel[] DaysInCare { get; set; }
    }

    public class CxDaysInCareModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public CxTimeModel[] Times { get; set; }

        public class CxTimeModel
        {
            public DateTime? DropOff { get; set; }
            public DateTime? PickUp { get; set; }
        }
    }

    public class CxChildSchoolModel
    {
        public int SchoolType { get; set; }
        public string Name { get; set; }
        public int DistrictId { get; set; }
        public string SchoolAssignedNumber { get; set; }
    }

    public class CxChildInfantFormulaModel
    {
        public string ParentFormula { get; set; }
        public string ProviderFormula { get; set; }
        public bool? AcceptProviderFormula { get; set; }
        public bool? AcceptProviderFood { get; set; }
        public bool InfantFormOnFile { get; set; }
    }

    public class CxChildAllergiesModel
    {
        public string OriginalData { get; set; } //workaround. We don't have the ability to save allergies in a separate field

        public List<ChildAllergy> Allergies { get; set; }

        public override string ToString()
        {
            var result = string.Empty;
            const string delimeter = "---";
            if (!string.IsNullOrEmpty(OriginalData))
            {
                result = OriginalData;

                var dataArray = result.Split(new[] { delimeter }, StringSplitOptions.None);
                if (dataArray.Length > 1)
                {
                    var oldValue = $"{delimeter}{dataArray[1]}{delimeter}";
                    result = result.Replace(oldValue, string.Empty);
                }
            }
            var newAllergiesData = string.Join(";", Allergies);
            if (!string.IsNullOrEmpty(newAllergiesData))
            {
                var newValue = $"{delimeter}{newAllergiesData}{delimeter}";
                result += newValue;
            }


            return result;
        }

        public string ToString(string dietText)
        {
            var result = string.Empty;
            const string delimeter = "---";
            if (!string.IsNullOrEmpty(dietText))
            {
                result = dietText;

                var dataArray = result.Split(new[] { delimeter }, StringSplitOptions.None);
                if (dataArray.Length > 1)
                {
                    var oldValue = $"{delimeter}{dataArray[1]}{delimeter}";
                    result = result.Replace(oldValue, string.Empty);
                }
            }
            var newAllergiesData = string.Join(";", Allergies);
            if (!string.IsNullOrEmpty(newAllergiesData))
            {
                var newValue = $"{delimeter}{newAllergiesData}{delimeter}";
                if (!result.EndsWith("\n") && !result.EndsWith("\r") && !result.EndsWith(System.Environment.NewLine))
                {
                    result += System.Environment.NewLine;
                }

                result += newValue;
            }


            return result;
        }

    }

    public class ChildAllergy
    {
        public ChildAllergyType Type { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            var type = Type == ChildAllergyType.Allergy ? "*" : "+";
            return $"{type}{Description}";
        }
    }

    public class CxMedicalContact
    {
        public string Name { get; set; }
        public string Phone { get; set; }

        public string ClinicName { get; set; }
        public string ClinicAddress { get; set; }

        public string Insurance { get; set; }
        public string InsuranceNumber { get; set; }
        public string InsuranceGroup { get; set; }
        public string InsurancePhone { get; set; }
    }

    public class CxImmunizationModel
    {
        public object[] Items { get; set; }
        public DateTime? NextReminderDate { get; set; }
    }

    public class ChildImage
    {
        public Guid ChildId { get; set; }
        //public HxChild Child { get; set; }

        public string ContentType { get; set; }
        public byte[] Data { get; set; }

        public DateTime UploadDate { get; set; }
    }

    public class CxFormModel
    {
        public CompletedFormType FormType { get; set; }
        public int Year { get; set; }
        public CxFormStatus Status { get; set; }
        public Guid? PublicName { get; set; }
    }

    public class ClientPolicyModel
    {
        public int ClientId { get; set; }
        public int ClientPolicyId { get; set; }

        public string PolicyName { get; set; }
        public string CxMode { get; set; }
        public string CxScreen { get; set; }
        public string PolicyValue { get; set; }
        public string FieldName { get; set; }
        public string LabelName { get; set; }
        public string DescriptiveText { get; set; }
        public string StateCode { get; set; }
    }

    public class CodeDetailListItem
    {
        public short sCodeValue { get; set; }
        public string strCodeDisplayName { get; set; }
        public string strCodeName { get; set; }
        public string strSpanishDisplayName { get; set; }
    }

    public class Policy
    {
        public const string D24 = "ENFORCE_INFANT_FORM_CODE";
        public const string D17 = "IGNORE_CHILD_INFO_CODE";
        public const string D17b = "APPLY_IGNORE_CHILD_INFO_TO_CENTER";
        public const string F2b = "CENTERS_CHILD_OVERSIGHT_FLAG";
        public const string S1 = "ALLOW_SCANNING_FLAG";
    }
}

namespace ChildPlusKidkareSync.Core.Enums
{
    public enum AgeGroupCode
    {
        Infant = 620,
        Toddler = 621,
        Preschool = 622,
        SchoolAge = 623,
        MiscGroup = 624
    }

    public enum MealType
    {
        Unspecified = 0,
        Breakfast = 75,
        AmSnack = 76,
        Lunch = 77,
        PmSnack = 78,
        Dinner = 79,
        EveningSnack = 80
    }

    public enum Month
    {
        January = 1,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public enum ProgramTypeCode : short
    {
        Unknown = 0,
        AdultCare = 1612,
        ChildCare,
        AtRisk,
        OSHC,
        HeadStart,
        EmergencyShelter,
        SummerFoodProgram = 2218
    }

    public enum AgeTypeCode : short
    {
        Undefined = 0,
        Weeks = 198,
        Months = 199,
        Years = 200
    }

    public enum Gender : short
    {
        NotSpecified = 0,
        Male = 202,
        Female = 201
    }

    public enum StateCode
    {
        AK = 2,
        AL = 1,
        AR = 38,
        AZ = 45,
        CA = 5,
        CO = 9,
        CT = 29,
        DC = 35,
        DE = 31,
        FL = 39,
        GA = 25,
        HI = 40,
        IA = 20,
        ID = 6,
        IL = 23,
        IN = 21,
        KS = 22,
        KY = 48,
        LA = 17,
        MA = 28,
        MD = 33,
        ME = 44,
        MI = 13,
        MN = 51,
        MO = 19,
        MS = 18,
        MT = 10,
        NC = 50,
        ND = 11,
        NE = 42,
        NH = 26,
        NJ = 43,
        NM = 14,
        NV = 8,
        NY = 49,
        OH = 47,
        OK = 16,
        OR = 4,
        PA = 32,
        RI = 30,
        SC = 37,
        SD = 12,
        TN = 41,
        TX = 15,
        UT = 7,
        VA = 34,
        VT = 27,
        WA = 3,
        WI = 24,
        WV = 36,
        WY = 46,
        Unknown = 0
    }
    public enum PaymentSource : short
    {
        NotSpecified = 0,
        Public = 225,
        Private = 226,
        NoPay = 878
    }

    public enum Race
    {
        None = 0,
        Indian,
        Asian,
        Black,
        PacificIslander,
        White,
        Unknown,
        NotSupplied
    }

    public enum Ethnicity
    {
        None = 0,
        Hispanic,
        NotHispanic,
        Unknown
    }

    public enum CxPrimaryGuardianCode : short
    {
        Unknown = 0,
        Mother = 1637,
        Father = 1638,
        Other = 1639
    }

    public enum ChildAllergyType
    {
        Allergy,
        MedicalCondition
    }

    public enum ChildStatus
    {
        Active = 238,
        Pending = 239,
        EnrollmentIncomplete = 240,
        Withdrawn = 241,
        Unknown = 389
    }

    //FRB_CATEGORY_CODE
    public enum FrpCategory : short
    {
        None = 0,
        Free = 1618,
        Reduced = 1619,
        Paid = 1620
    }

    //FRB_ELIGIBILITY_TYPE_CODE
    public enum FrpEligibilityType : short
    {
        None = 0,
        Income = 914,
        ZeroIncome = 932,
        FoodStamps = 931,    // SNAP
        DirectCert = 939,
        TANF = 934,
        Foster = 918,
        Subsidy = 930,
        Refused = 916,
        Category1 = 943,
        Medicaid = 944,
        Other = 917,
        Incomplete = 945,
        Homeless = 946,
        Headstart = 947,
        EarlyHeadstart = 948
    }

    public enum CompletedFormType
    {
        Unspecified = 0,
        EnrollmentForm = 1,
        EligibilityForm = 2
    }

    public enum CxFormStatus
    {
        Active,
        Expired,
        Pending
    }

    public enum EnforceCode
    {
        Uncpecified = 0,
        Disallow = 54,
        Warn = 55,
        Ignore = 56
    }

    public enum CxIgnoreChildInfoCode
    {
        Undefined = 0,

        Race = 1665,
        Schedule = 1666,
        Gender = 1668,

        RaceGender = 1667,
        RaceScheduleGender = 1664,
        All = 1693
    }

    public enum CenterStatusCode : short
    {
        InProcess = 203, 
        Active = 204,
        Hold = 205,
        Pending = 206,
        Removed = 207,
        Deleted = 290
    }

    public enum ProfitTypeCode : short
    {
        Undefined = 0,
        TitleXXorXIX = 1627,
        FRP = 1628
    }

    public enum FundingSourceCode : short
    {
        NA = 919,
        Alzheimers = 920,
        Latckey = 921,
        Migrant = 922,
        StatePreschool = 923
    }

    public enum ServingCode : short
    {
        First = 215,
        Second = 216,
        Third = 217
    }

    public enum AdministrationTypeCode : short
    {
        Unknown = 0,
        SeparatedFromSponsor = 1625,
        AffiliatedWithSponsor = 1626
    }

    public enum RecordAttendanceDateTimeLimitation : short
    {
        None = 1646,
        ByEndOfDay = 1647,
        DuringMealServiceTimes = 1648,
        ByEndOfWeek = 1706
    }

    public enum BankAccountCode : short
    {
        Undefined = 0,
        Checking = 177,
        Savings = 178,
        MoneyMarket = 179
    }

    public enum MonthCode : short
    {
        None = 0,
        Jan = 506,
        Feb = 507,
        Mar = 508,
        Apr = 509,
        May = 510,
        Jun = 511,
        Jul = 512,
        Aug = 513,
        Sep = 514,
        Oct = 515,
        Nov = 516,
        Dec = 517
    }

    public enum RemovalReasonCode : short
    {
        Unknown = 0,
        OutOfBusiness = 1,
        SwitchingSponsors = 2,
        DroppingTheFoodProgram = 3,
        ClosedByStateOrCounty = 4,
        TerminatedForCause = 5,
        TerminatedForConvenience = 6,
        Other = 7
    }

    public enum CxAttendanceImportMethod
    {
        Manual = 1,
        Auto = 2,
        Partially = 3
    }
}

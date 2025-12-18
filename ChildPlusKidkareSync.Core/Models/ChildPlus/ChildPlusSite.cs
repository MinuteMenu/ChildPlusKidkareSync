namespace ChildPlusKidkareSync.Core.Models.ChildPlus;

public class ChildPlusSite
{
    // PRIMARY KEY
    public string SiteId { get; set; }

    // BASIC INFORMATION
    public string SiteName { get; set; }
    public string profit_status_code { get; set; }
    public string business_type_code { get; set; }
    public string url { get; set; }

    public string state_assigned_number { get; set; }
    public string phone { get; set; }
    public string fax_phone { get; set; }
    public string email { get; set; }
    public string ssn_tax { get; set; }
    public string alternate_phone { get; set; }
    public int center_status_code { get; set; }
    public string alternate_assigned_number { get; set; }
    public string title_xx_number { get; set; }
    public string title_xix_number { get; set; }
    public DateTime? cacfp_allowed_start_date { get; set; }
    public DateTime? cacfp_current_agreement_start_date { get; set; }
    public DateTime? cacfp_current_expiration_start_date { get; set; }
    public DateTime? cacfp_original_start_date { get; set; }
    public string school_care_school_name { get; set; }
    public bool school_care_activities_education_flag { get; set; } = false;
    public bool sanitation_required_flag { get; set; } = false;
    public bool health_inspection_required_flag { get; set; } = false;
    public bool fire_inspection_required_flag { get; set; } = false;
    public bool school_care_activities_enrichment_flag { get; set; } = false;
    public string food_service_type_code { get; set; }
    public decimal? food_service_vendor_approx_annual_cost { get; set; }
    public string food_service_vendor_contact_name { get; set; }
    public string food_service_vendor_contact_phone { get; set; }
    public string food_service_vendor_contact_email { get; set; }
    public int? extended_capacity { get; set; }
    public string meal_service_type_code { get; set; }
    public string hold_reason_text { get; set; }
    public DateTime? next_review_required_date { get; set; }
    public string review_year_start_month { get; set; }
    public string previous_sponsor_name { get; set; }
    public string referred_by_text { get; set; }
    public DateTime? removal_date { get; set; }
    public string removal_reason_code { get; set; }

    // ADDRESS
    public string address_line { get; set; }
    public string city_name { get; set; }
    public string county_name { get; set; }
    public string state_code { get; set; }
    public string zip_code { get; set; }
    public double? latitude_degree { get; set; }
    public double? longitude_degree { get; set; }

    public string mailing_address_line { get; set; }
    public string mailing_city_name { get; set; }
    public string mailing_state_code { get; set; }
    public string mailing_zip_code { get; set; }

    // AGENCY INFO
    public string corporation_name { get; set; }
    public string agency_phone { get; set; }
    public string agency_fax { get; set; }
    public string sponsor_notes_text { get; set; }

    // STAFF
    public string monitor_staff_id { get; set; }
    public string director_name_text { get; set; }

    // LICENSE
    public string license_number { get; set; }
    public string program_type_code { get; set; }
    public string program_type_name { get; set; }
    public string funding_source_code { get; set; }
    public DateTime? license_start_date { get; set; }
    public DateTime? license_end_date { get; set; }
    public int? license_max_capacity_number { get; set; }
    public int? license_max_infants { get; set; }
    public int? license_max_toddlers { get; set; }
    public int? license_max_preschoolers { get; set; }
    public int? license_max_schoolagers { get; set; }
    public bool? at_risk_flag { get; set; }
    public int? allowed_starting_age_number { get; set; }
    public string allowed_starting_age_type_code { get; set; }
    public int? allowed_ending_age_number { get; set; }
    public string allowed_ending_age_type_code { get; set; }

    // INSPECTIONS
    public DateTime? health_inspection_expiration_date { get; set; }
    public DateTime? fire_inspection_expiration_date { get; set; }
    public DateTime? sanitation_expiration_date { get; set; }

    // CENTER_SCHEDULE
    public DateTime? open_time { get; set; }
    public DateTime? close_time { get; set; }
    public bool? open_24_hours_flag { get; set; }
    public bool? allow_monday_flag { get; set; }
    public bool? allow_tuesday_flag { get; set; }
    public bool? allow_wednesday_flag { get; set; }
    public bool? allow_thursday_flag { get; set; }
    public bool? allow_friday_flag { get; set; }
    public bool? allow_saturday_flag { get; set; }
    public bool? allow_sunday_flag { get; set; }
    public DateTime? first_standard_breakfast_time { get; set; }
    public DateTime? first_ending_breakfast_time { get; set; }
    public DateTime? second_standard_breakfast_time { get; set; }
    public DateTime? second_ending_breakfast_time { get; set; }
    public DateTime? first_standard_am_snack_time { get; set; }
    public DateTime? first_ending_am_snack_time { get; set; }
    public DateTime? second_standard_am_snack_time { get; set; }
    public DateTime? second_ending_am_snack_time { get; set; }
    public DateTime? first_standard_lunch_time { get; set; }
    public DateTime? first_ending_lunch_time { get; set; }
    public DateTime? second_standard_lunch_time { get; set; }
    public DateTime? second_ending_lunch_time { get; set; }
    public DateTime? first_standard_pm_snack_time { get; set; }
    public DateTime? first_ending_pm_snack_time { get; set; }
    public DateTime? second_standard_pm_snack_time { get; set; }
    public DateTime? second_ending_pm_snack_time { get; set; }
    public DateTime? first_standard_dinner_time { get; set; }
    public DateTime? first_ending_dinner_time { get; set; }
    public DateTime? second_standard_dinner_time { get; set; }
    public DateTime? second_ending_dinner_time { get; set; }
    public DateTime? first_standard_evening_snack_time { get; set; }
    public DateTime? first_ending_evening_snack_time { get; set; }
    public DateTime? second_standard_evening_snack_time { get; set; }
    public DateTime? second_ending_evening_snack_time { get; set; }
    public DateTime? night_open_time { get; set; }
    public DateTime? night_close_time { get; set; }
    public short highest_serving_allowed_code { get; set; }
    public bool? operates_january_flag { get; set; }
    public bool? operates_february_flag { get; set; }
    public bool? operates_march_flag { get; set; }
    public bool? operates_april_flag { get; set; }
    public bool? operates_may_flag { get; set; }
    public bool? operates_june_flag { get; set; }
    public bool? operates_july_flag { get; set; }
    public bool? operates_august_flag { get; set; }
    public bool? operates_september_flag { get; set; }
    public bool? operates_october_flag { get; set; }
    public bool? operates_november_flag { get; set; }
    public bool? operates_december_flag { get; set; }
    public DateTime? third_standard_breakfast_time { get; set; }
    public DateTime? third_ending_breakfast_time { get; set; }
    public DateTime? third_standard_am_snack_time { get; set; }
    public DateTime? third_ending_am_snack_time { get; set; }
    public DateTime? third_standard_lunch_time { get; set; }
    public DateTime? third_ending_lunch_time { get; set; }
    public DateTime? third_standard_pm_snack_time { get; set; }
    public DateTime? third_ending_pm_snack_time { get; set; }
    public DateTime? third_standard_dinner_time { get; set; }
    public DateTime? third_ending_dinner_time { get; set; }
    public DateTime? third_standard_evening_snack_time { get; set; }
    public DateTime? third_ending_evening_snack_time { get; set; }
    public string administration_type_code { get; set; }
    public decimal? admin_rate { get; set; } 
    public string map_location { get; set; }
    public string day_of_entry_requirement_code { get; set; }
    public bool? prevent_select_all_record_attendance_flag { get; set; } = false;
    public bool? allow_center_override_enrollment_flag { get; set; } = false;
    public bool? require_center_enter_receipts_flag { get; set; } = false;
    public bool? use_direct_deposit_flag { get; set; } = false;
    public string bank_account_code { get; set; }
    public string bank_account_number { get; set; }
    public string bank_routing_number { get; set; }

    // CENTER_NON_CONGREGATE_MEAL
    public bool? allow_meal_served_flag { get; set; }

    // CenterExtendedProperty
    public bool? RuralOrSelfPrepSite { get; set; }
    public string LicenseSharedMaxCapacityNumber { get; set; }
    public string StateSiteNumber { get; set; }

    // ATTENDANCE_CONFIGURATION
    public string import_method { get; set; }
    public string import_source { get; set; }
    public string attendance_reason { get; set; }

    // MasterMenuClaim
    public int? MasterMenuId { get; set; }

    // SYSTEM
    public byte[] Timestamp { get; set; }
    public string AgencyId { get; set; }


}
namespace ChildPlusKidkareSync.Core.Constants;

public static class SyncConstants
{
    public const string SystemName = "ChildPlus-Kidkare-Sync";

    public static class ApiEndpoints
    {
        public const string SaveCenter = "/cxSponsor/cxservice/center/saveCenter";
        public const string AddStaff = "/cxSponsor/centerstaff/add";
        public const string UpdateStaff = "/cxSponsor/centerstaff/update";
        public const string GetRole = "/cxSponsor/roles/list";
        public const string AddRole = "/cxSponsor/roles/add";
        public const string SavePermission = "/cxSponsor/permissions/save";
        public const string FinalizeImport = "/cxSponsor/child/finalizeImport";
    }

    public static class SqlQueries
    {
        public const string GetSites = @"
            WITH SiteData AS (
                SELECT
                    -- =============================================
                    -- PRIMARY KEY & TIMESTAMPS
                    -- =============================================
                    CONVERT(NVARCHAR(36), s.SiteID) AS SiteId,
                    s.TIMESTAMP AS MainTimestamp, -- Main table timestamp
        
                    -- =============================================
                    -- TRACK RELATED TABLE TIMESTAMPS
                    -- Calculate composite timestamp for change detection
                    -- Only include tables that have TIMESTAMP column
                    -- =============================================
                    s.TIMESTAMP AS site_timestamp,
                    a.TIMESTAMP AS agency_timestamp,
        
                    -- Calculate composite (MAX of all related timestamps that exist)
                    (SELECT MAX(ts) FROM (VALUES 
                        (s.TIMESTAMP),
                        (a.TIMESTAMP),
                        -- Classroom has TIMESTAMP
                        ((SELECT MAX(c.TIMESTAMP) FROM Classroom c 
                          WHERE c.SiteID = s.SiteID AND c.Active = 1)),
                        -- ProgramTerm has TIMESTAMP
                        ((SELECT MAX(pt.TIMESTAMP) FROM Classroom c 
                          INNER JOIN ProgramTermClassroom ptc ON c.ClassroomID = ptc.ClassroomID 
                          INNER JOIN ProgramTerm pt ON ptc.ProgramTermID = pt.ProgramTermID
                          WHERE c.SiteID = s.SiteID AND c.Active = 1 AND pt.Active = 1)),
                        -- Code table has TIMESTAMP (for ProgramType)
                        ((SELECT MAX(code.TIMESTAMP) FROM Classroom cls
                          INNER JOIN ProgramTermClassroom ptc ON cls.ClassroomID = ptc.ClassroomID
                          INNER JOIN ProgramTerm pt ON ptc.ProgramTermID = pt.ProgramTermID
                          INNER JOIN Program p ON pt.ProgramID = p.ProgramID
                          INNER JOIN Code code ON p.ProgramTypeCodeID = code.CodeID
                          WHERE cls.SiteID = s.SiteID AND cls.Active = 1 AND pt.Active = 1))
                    ) AS TimestampValues(ts)
                    WHERE ts IS NOT NULL
                    ) AS CompositeTimestamp,
        
                    -- JSON detail of all timestamps for debugging
                    (SELECT 
                        SiteTimestamp = CONVERT(VARCHAR(MAX), s.TIMESTAMP, 1),
                        AgencyTimestamp = CONVERT(VARCHAR(MAX), a.TIMESTAMP, 1),
                        ClassroomTimestamp = CONVERT(VARCHAR(MAX), 
                            (SELECT MAX(c.TIMESTAMP) FROM Classroom c 
                             WHERE c.SiteID = s.SiteID AND c.Active = 1), 1),
                        ProgramTermTimestamp = CONVERT(VARCHAR(MAX),
                            (SELECT MAX(pt.TIMESTAMP) FROM Classroom c 
                             INNER JOIN ProgramTermClassroom ptc ON c.ClassroomID = ptc.ClassroomID 
                             INNER JOIN ProgramTerm pt ON ptc.ProgramTermID = pt.ProgramTermID
                             WHERE c.SiteID = s.SiteID AND c.Active = 1 AND pt.Active = 1), 1),
                        ProgramTypeTimestamp = CONVERT(VARCHAR(MAX),
                            (SELECT MAX(code.TIMESTAMP) FROM Classroom cls
                             INNER JOIN ProgramTermClassroom ptc ON cls.ClassroomID = ptc.ClassroomID
                             INNER JOIN ProgramTerm pt ON ptc.ProgramTermID = pt.ProgramTermID
                             INNER JOIN Program p ON pt.ProgramID = p.ProgramID
                             INNER JOIN Code code ON p.ProgramTypeCodeID = code.CodeID
                             WHERE cls.SiteID = s.SiteID AND cls.Active = 1 AND pt.Active = 1), 1)
                     FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                    ) AS TimestampDetail,
        
                    -- =============================================
                    -- dbo.CENTER - BASIC INFORMATION
                    -- =============================================
                    s.Name AS SiteName,
                    s.LicenseNumber AS state_assigned_number,
                    s.PrimaryPhone AS phone,
                    s.Fax AS fax_phone,
                    s.PrimaryPhoneExt AS alternate_phone,
                    s.Active AS center_status_code,
                    NULL AS current_claim_monthyear,
                    a.Email AS email,
                    a.TaxID AS ssn_tax,
                    NULL AS alternate_assigned_number,
                    a.AgencyName AS corporation_name,
                    a.Website AS url,
                    NULL AS business_type_code,
                    NULL AS profit_status_code,
                    NULL AS profit_type_code,
                    NULL AS title_xx_number,
                    NULL AS title_xix_number,
                    NULL AS food_service_type_code,
                    NULL AS food_service_vendor_contact_name,
                    NULL AS food_service_vendor_contact_phone,
                    NULL AS food_service_vendor_contact_email,
                    NULL AS food_service_vendor_approx_annual_cost,
                    COALESCE(monitor_staff.PersonID, NULLIF(LTRIM(RTRIM(s.ResponsibleStaff)), '')) AS monitor_staff_id,
                    NULL AS pricing_program_flag,
                    NULL AS breakfast_charge,
                    NULL AS lunch_dinner_charge,
                    NULL AS snack_charge,
                    NULL AS meal_service_type_code,
                    NULL AS insurance_start_date,
                    NULL AS insurance_end_date,
                    NULL AS insurance_vendor_name,
                    NULL AS insurance_type_code,
                    NULL AS insurance_description_text,
                    NULL AS administration_type_code,
                    NULL AS cacfp_original_start_date,
                    NULL AS cacfp_allowed_start_date,
                    NULL AS cacfp_current_agreement_start_date,
                    NULL AS cacfp_current_expiration_start_date,
                    NULL AS removal_date,
                    NULL AS removal_reason_code,
                    NULL AS hold_reason_text,
                    NULL AS school_care_school_name,
                    0 AS school_care_activities_enrichment_flag,
                    0 AS school_care_activities_education_flag,
        
                    -- =============================================
                    -- CONSOLIDATED: All Inspections
                    -- =============================================
                    inspection_data.sanitation_expiration_date,
                    CASE WHEN inspection_data.sanitation_expiration_date IS NOT NULL THEN 1 ELSE 0 END AS sanitation_required_flag,
                    inspection_data.health_inspection_expiration_date,
                    CASE WHEN inspection_data.health_inspection_expiration_date IS NOT NULL THEN 1 ELSE 0 END AS health_inspection_required_flag,
                    inspection_data.fire_inspection_expiration_date,
                    CASE WHEN inspection_data.fire_inspection_expiration_date IS NOT NULL THEN 1 ELSE 0 END AS fire_inspection_required_flag,
        
                    NULL AS first_claim_received_date,
        
                    -- Address
                    s.Address AS address_line,
                    s.City AS city_name,
                    NULL AS county_id,
                    s.County AS county_name,
                    s.State AS state_code,
                    s.Zip AS zip_code,
                    NULL AS mailing_addressee_name,
                    NULL AS mailing_address_line,
                    NULL AS mailing_city_name,
                    NULL AS mailing_state_code,
                    NULL AS mailing_zip_code,
                    NULL AS previous_sponsor_name,
                    NULL AS map_location,
                    NULL AS school_district_id,
                    s.Latitude AS latitude_degree,
                    s.Longitude AS longitude_degree,
                    NULL AS driving_directions_text,
                    NULL AS claim_source_code,
                    NULL AS next_review_required_date,
                    NULL AS referred_by_text,
                    NULL AS user_security_group,
                    a.Notes AS sponsor_notes_text,
                    NULL AS comments_text,
                    NULL AS language_code,
                    NULL AS bank_account_code,
                    NULL AS bank_account_number,
                    NULL AS bank_routing_number,
                    NULL AS monthly_check_deduction_amount,
                    0 AS use_direct_deposit_flag,
                    NULL AS minimum_version,
                    NULL AS block_claim_last_legitimized_date,
                    s.ResponsibleStaff AS director_name_text,
                    NULL AS mileage_to_center,
                    NULL AS day_of_entry_requirement_code,
                    NULL AS extended_capacity,
                    NULL AS pro_status_code,
                    NULL AS pro_expiration_date,
                    NULL AS review_year_start_month,
                    NULL AS admin_rate,
                    NULL AS district_number,
                    NULL AS enroll_expiration_month_number,
                    0 AS allow_center_override_enrollment_flag,
                    0 AS check_in_out_times_flag,
                    0 AS re_sync_procare_with_cx,
                    0 AS master_menu_flag,
                    0 AS prevent_select_all_record_attendance_flag,
                    0 AS use_procare_flag,
                    0 AS skip_menu_checks_flag,
                    NULL AS meal_pattern_effective_date,
                    0 AS require_center_enter_receipts_flag,
                    NULL AS SSPS,
        
                    -- =============================================
                    -- dbo.CENTER_LICENSE
                    -- =============================================
                    0 AS license_id,
                    program_type.program_type_code,
                    program_type.program_type_name,
                    0 AS at_risk_flag,
                    funding_data.funding_source_code,
        
                    -- Age Restrictions (from consolidated CTE)
                    ptc_consolidated.allowed_starting_age_type_code,
                    ptc_consolidated.allowed_starting_age_number,
                    ptc_consolidated.allowed_ending_age_type_code,
                    ptc_consolidated.allowed_ending_age_number,
        
                    -- Meal Allowances
                    COALESCE(ptc_consolidated.allow_breakfast_flag, 0) AS allow_breakfast_flag,
                    COALESCE(ptc_consolidated.allow_am_snack_flag, 0) AS allow_am_snack_flag,
                    COALESCE(ptc_consolidated.allow_lunch_flag, 0) AS allow_lunch_flag,
                    COALESCE(ptc_consolidated.allow_pm_snack_flag, 0) AS allow_pm_snack_flag,
                    COALESCE(ptc_consolidated.allow_dinner_flag, 0) AS allow_dinner_flag,
                    0 AS allow_evening_snack_flag,
        
                    -- License Info
                    s.LicenseNumber AS license_number,
                    NULL AS license_start_date,
                    s.LicenseExpirationDate AS license_end_date,
                    CASE WHEN s.LicenseExpirationDate IS NULL THEN 1 ELSE 0 END AS license_does_not_expire_flag,
                    s.LicenseMaximumCapacity AS license_max_capacity_number,
        
                    0 AS waiver_flag,
                    NULL AS license_mod_login_id,
                    NULL AS state_number,
                    NULL AS at_risk_number,
                    NULL AS license_max_infants,
                    NULL AS license_max_toddlers,
                    NULL AS license_max_preschoolers,
                    NULL AS license_max_schoolagers,
        
                    0 AS allow_atrisk_breakfast_flag,
                    0 AS allow_atrisk_am_snack_flag,
                    0 AS allow_atrisk_lunch_flag,
                    0 AS allow_atrisk_pm_snack_flag,
                    0 AS allow_atrisk_dinner_flag,
                    0 AS allow_atrisk_eve_snack_flag,
        
                    1 AS default_license_flag,
                    0 AS school_flag,
                    NULL AS license_max_approved_meal_number,
        
                    0 AS waiver_breakfast_flag,
                    0 AS waiver_am_snack_flag,
                    0 AS waiver_lunch_flag,
                    0 AS waiver_pm_snack_flag,
                    0 AS waiver_dinner_flag,
                    0 AS waiver_eve_snack_flag,
        
                    -- =============================================
                    -- dbo.CenterExtendedProperty
                    -- =============================================
                    s.LicenseNumber AS StateSiteNumber,
                    NULL AS ExternalId,
                    NULL AS RuralOrSelfPrepSite,
                    NULL AS LicenseSharedMaxCapacityNumber,
        
                    -- =============================================
                    -- dbo.CENTER_NON_CONGREGATE_MEAL
                    -- =============================================
                    1 AS allow_meal_served_flag,
        
                    -- =============================================
                    -- dbo.CENTER_SCHEDULE (from consolidated CTE)
                    -- =============================================
                    ptc_consolidated.open_time,
                    ptc_consolidated.close_time,
                    ptc_consolidated.open_24_hours_flag,
                    ptc_consolidated.allow_monday_flag,
                    ptc_consolidated.allow_tuesday_flag,
                    ptc_consolidated.allow_wednesday_flag,
                    ptc_consolidated.allow_thursday_flag,
                    ptc_consolidated.allow_friday_flag,
                    ptc_consolidated.allow_saturday_flag,
                    ptc_consolidated.allow_sunday_flag,
        
                    ptc_consolidated.first_standard_breakfast_time,
                    ptc_consolidated.first_ending_breakfast_time,
                    NULL AS second_standard_breakfast_time,
                    NULL AS second_ending_breakfast_time,
        
                    ptc_consolidated.first_standard_am_snack_time,
                    ptc_consolidated.first_ending_am_snack_time,
                    NULL AS second_standard_am_snack_time,
                    NULL AS second_ending_am_snack_time,
        
                    ptc_consolidated.first_standard_lunch_time,
                    ptc_consolidated.first_ending_lunch_time,
                    NULL AS second_standard_lunch_time,
                    NULL AS second_ending_lunch_time,
        
                    ptc_consolidated.first_standard_pm_snack_time,
                    ptc_consolidated.first_ending_pm_snack_time,
                    NULL AS second_standard_pm_snack_time,
                    NULL AS second_ending_pm_snack_time,
        
                    ptc_consolidated.first_standard_dinner_time,
                    ptc_consolidated.first_ending_dinner_time,
                    NULL AS second_standard_dinner_time,
                    NULL AS second_ending_dinner_time,
        
                    NULL AS first_standard_evening_snack_time,
                    NULL AS first_ending_evening_snack_time,
                    NULL AS second_standard_evening_snack_time,
                    NULL AS second_ending_evening_snack_time,
        
                    ptc_consolidated.night_open_time,
                    ptc_consolidated.night_close_time,
                    ptc_consolidated.highest_serving_allowed_code,
        
                    ptc_consolidated.operates_january_flag,
                    ptc_consolidated.operates_february_flag,
                    ptc_consolidated.operates_march_flag,
                    ptc_consolidated.operates_april_flag,
                    ptc_consolidated.operates_may_flag,
                    ptc_consolidated.operates_june_flag,
                    ptc_consolidated.operates_july_flag,
                    ptc_consolidated.operates_august_flag,
                    ptc_consolidated.operates_september_flag,
                    ptc_consolidated.operates_october_flag,
                    ptc_consolidated.operates_november_flag,
                    ptc_consolidated.operates_december_flag,
        
                    NULL AS third_standard_breakfast_time,
                    NULL AS third_ending_breakfast_time,
                    NULL AS third_standard_am_snack_time,
                    NULL AS third_ending_am_snack_time,
                    NULL AS third_standard_lunch_time,
                    NULL AS third_ending_lunch_time,
                    NULL AS third_standard_pm_snack_time,
                    NULL AS third_ending_pm_snack_time,
                    NULL AS third_standard_dinner_time,
                    NULL AS third_ending_dinner_time,
                    NULL AS third_standard_evening_snack_time,
                    NULL AS third_ending_evening_snack_time,
        
                    NULL AS MasterMenuClaimId,
                    NULL AS MasterMenuId,
        
                    attendance_config.import_method,
                    attendance_config.import_method AS import_source,
                    NULL AS attendance_reason,
        
                    s.SystemDefined AS system_defined,
                    s.FederalInterestEstablished AS federal_interest_established,
                    CONVERT(NVARCHAR(36), s.AgencyID) AS agency_id

                FROM Sites s
                    LEFT JOIN Agency a ON s.AgencyID = a.AgencyID
        
                    -- Monitor Staff
                    OUTER APPLY (
                        SELECT TOP 1 CAST(p.PersonID AS NVARCHAR(36)) AS PersonID
                        FROM dbo.Person p
                        WHERE LTRIM(RTRIM(CONCAT(p.FirstName, ' ', p.LastName))) = LTRIM(RTRIM(s.ResponsibleStaff))
                          AND NULLIF(LTRIM(RTRIM(s.ResponsibleStaff)), '') IS NOT NULL
                    ) AS monitor_staff
        
                    -- CONSOLIDATED: All Inspections in One Query
                    OUTER APPLY (
                        SELECT 
                            MAX(CASE WHEN isetup.Name LIKE '%Health%' THEN id.NextInspection END) AS health_inspection_expiration_date,
                            MAX(CASE WHEN isetup.Name LIKE '%Fire%' THEN id.NextInspection END) AS fire_inspection_expiration_date,
                            MAX(CASE WHEN isetup.Name LIKE '%Sanit%' THEN id.NextInspection END) AS sanitation_expiration_date
                        FROM InspectionData id
                        INNER JOIN InspectionSetup isetup ON id.InspectionID = isetup.InspectionID
                        WHERE id.SiteID = s.SiteID
                    ) AS inspection_data
        
                    -- Funding Source
                    OUTER APPLY (
                        SELECT TOP 1 
                            COALESCE(fs.FundingSourceName, f.FundingName, 'UNKNOWN') AS funding_source_code
                        FROM AgencyFunding af
                        INNER JOIN Funding f ON af.FundingID = f.FundingID
                        INNER JOIN FundingSource fs ON f.FundingSourceID = fs.FundingSourceID
                        WHERE af.AgencyID = s.AgencyID AND af.Active = 1 AND f.Active = 1
                        ORDER BY af.BeginDate DESC
                    ) AS funding_data
        
                    -- Program Type
                    OUTER APPLY (
                        SELECT TOP 1 
                            CONVERT(NVARCHAR(36), c.CodeID) AS program_type_code,
                            c.Description AS program_type_name
                        FROM Classroom cls
                        INNER JOIN ProgramTermClassroom ptc ON cls.ClassroomID = ptc.ClassroomID
                        INNER JOIN ProgramTerm pt ON ptc.ProgramTermID = pt.ProgramTermID
                        INNER JOIN Program p ON pt.ProgramID = p.ProgramID
                        INNER JOIN Code c ON p.ProgramTypeCodeID = c.CodeID AND c.CodeType = 'ProgramType'
                        WHERE cls.SiteID = s.SiteID AND cls.Active = 1 AND pt.Active = 1 AND p.Active = 1 AND c.Active = 1
                        ORDER BY pt.BeginDate DESC
                    ) AS program_type
        
                    -- CONSOLIDATED: ProgramTermClassroom (replaces ptc_agg + center_schedule)
                    OUTER APPLY (
                        SELECT 
                            -- Age Restrictions
                            CASE 
                                WHEN COALESCE(MIN(ptc.MinAgeYears), 0) * 12 + COALESCE(MIN(ptc.MinAgeMonths), 0) = 0 THEN NULL
                                WHEN COALESCE(MIN(ptc.MinAgeYears), 0) * 12 + COALESCE(MIN(ptc.MinAgeMonths), 0) < 2 THEN 'W'
                                WHEN COALESCE(MIN(ptc.MinAgeYears), 0) * 12 + COALESCE(MIN(ptc.MinAgeMonths), 0) < 24 THEN 'M'
                                WHEN COALESCE(MIN(ptc.MinAgeYears), 0) <= 20 THEN 'Y'
                                ELSE 'U'
                            END AS allowed_starting_age_type_code,
                
                            CASE 
                                WHEN COALESCE(MIN(ptc.MinAgeYears), 0) * 12 + COALESCE(MIN(ptc.MinAgeMonths), 0) = 0 THEN 0
                                WHEN COALESCE(MIN(ptc.MinAgeYears), 0) * 12 + COALESCE(MIN(ptc.MinAgeMonths), 0) < 2 
                                    THEN (COALESCE(MIN(ptc.MinAgeYears), 0) * 12 + COALESCE(MIN(ptc.MinAgeMonths), 0)) * 4
                                WHEN COALESCE(MIN(ptc.MinAgeYears), 0) * 12 + COALESCE(MIN(ptc.MinAgeMonths), 0) < 24 
                                    THEN COALESCE(MIN(ptc.MinAgeYears), 0) * 12 + COALESCE(MIN(ptc.MinAgeMonths), 0)
                                WHEN COALESCE(MIN(ptc.MinAgeYears), 0) <= 20 THEN COALESCE(MIN(ptc.MinAgeYears), 0)
                                ELSE 0
                            END AS allowed_starting_age_number,
                
                            CASE 
                                WHEN COALESCE(MAX(ptc.MaxAgeYears), 0) * 12 + COALESCE(MAX(ptc.MaxAgeMonths), 0) = 0 THEN NULL
                                WHEN COALESCE(MAX(ptc.MaxAgeYears), 0) * 12 + COALESCE(MAX(ptc.MaxAgeMonths), 0) < 2 THEN 'W'
                                WHEN COALESCE(MAX(ptc.MaxAgeYears), 0) * 12 + COALESCE(MAX(ptc.MaxAgeMonths), 0) < 24 THEN 'M'
                                WHEN COALESCE(MAX(ptc.MaxAgeYears), 0) <= 20 THEN 'Y'
                                ELSE 'U'
                            END AS allowed_ending_age_type_code,
                
                            CASE 
                                WHEN COALESCE(MAX(ptc.MaxAgeYears), 0) * 12 + COALESCE(MAX(ptc.MaxAgeMonths), 0) = 0 THEN 0
                                WHEN COALESCE(MAX(ptc.MaxAgeYears), 0) * 12 + COALESCE(MAX(ptc.MaxAgeMonths), 0) < 2 
                                    THEN (COALESCE(MAX(ptc.MaxAgeYears), 0) * 12 + COALESCE(MAX(ptc.MaxAgeMonths), 0)) * 4
                                WHEN COALESCE(MAX(ptc.MaxAgeYears), 0) * 12 + COALESCE(MAX(ptc.MaxAgeMonths), 0) < 24 
                                    THEN COALESCE(MAX(ptc.MaxAgeYears), 0) * 12 + COALESCE(MAX(ptc.MaxAgeMonths), 0)
                                WHEN COALESCE(MAX(ptc.MaxAgeYears), 0) <= 20 THEN COALESCE(MAX(ptc.MaxAgeYears), 0)
                                ELSE 0
                            END AS allowed_ending_age_number,
                
                            -- Meal Flags
                            MAX(CAST(COALESCE(ptc.ServesBreakfast, 0) AS INT)) AS allow_breakfast_flag,
                            MAX(CAST(COALESCE(ptc.ServesAMSnack, 0) AS INT)) AS allow_am_snack_flag,
                            MAX(CAST(COALESCE(ptc.ServesLunch, 0) AS INT)) AS allow_lunch_flag,
                            MAX(CAST(COALESCE(ptc.ServesPMSnack, 0) AS INT)) AS allow_pm_snack_flag,
                            MAX(CAST(COALESCE(ptc.ServesSupper, 0) AS INT)) AS allow_dinner_flag,
                
                            -- Operating Days
                            MAX(CAST(COALESCE(ptc.OperatesMondays, 0) AS INT)) AS allow_monday_flag,
                            MAX(CAST(COALESCE(ptc.OperatesTuesdays, 0) AS INT)) AS allow_tuesday_flag,
                            MAX(CAST(COALESCE(ptc.OperatesWednesdays, 0) AS INT)) AS allow_wednesday_flag,
                            MAX(CAST(COALESCE(ptc.OperatesThursdays, 0) AS INT)) AS allow_thursday_flag,
                            MAX(CAST(COALESCE(ptc.OperatesFridays, 0) AS INT)) AS allow_friday_flag,
                            MAX(CAST(COALESCE(ptc.OperatesSaturdays, 0) AS INT)) AS allow_saturday_flag,
                            MAX(CAST(COALESCE(ptc.OperatesSundays, 0) AS INT)) AS allow_sunday_flag,
                
                            -- Operating Hours
                            MIN(ptc.BeginTime) AS open_time,
                            MAX(ptc.EndTime) AS close_time,
                            MAX(CAST(COALESCE(ptc.ServicesFullWorkingDay, 0) AS INT)) AS open_24_hours_flag,
                            MIN(ptc.BeginTime) AS night_open_time,
                            MAX(ptc.EndTime) AS night_close_time,
                
                            -- Meal Times
                            MIN(ptc.BreakfastBeginTime) AS first_standard_breakfast_time,
                            MAX(ptc.BreakfastEndTime) AS first_ending_breakfast_time,
                            MIN(ptc.AMSnackBeginTime) AS first_standard_am_snack_time,
                            MAX(ptc.AMSnackEndTime) AS first_ending_am_snack_time,
                            MIN(ptc.LunchBeginTime) AS first_standard_lunch_time,
                            MAX(ptc.LunchEndTime) AS first_ending_lunch_time,
                            MIN(ptc.PMSnackBeginTime) AS first_standard_pm_snack_time,
                            MAX(ptc.PMSnackEndTime) AS first_ending_pm_snack_time,
                            MIN(ptc.SupperBeginTime) AS first_standard_dinner_time,
                            MAX(ptc.SupperEndTime) AS first_ending_dinner_time,
                
                            MAX(ptc.CenterBasedProgramOptionFewerGreater) AS highest_serving_allowed_code,
                
                            -- Monthly Operations (simplified)
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 1 AND MONTH(ptc.EndDate) >= 1) THEN 1 ELSE 0 END) AS operates_january_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 2 AND MONTH(ptc.EndDate) >= 2) THEN 1 ELSE 0 END) AS operates_february_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 3 AND MONTH(ptc.EndDate) >= 3) THEN 1 ELSE 0 END) AS operates_march_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 4 AND MONTH(ptc.EndDate) >= 4) THEN 1 ELSE 0 END) AS operates_april_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 5 AND MONTH(ptc.EndDate) >= 5) THEN 1 ELSE 0 END) AS operates_may_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 6 AND MONTH(ptc.EndDate) >= 6) THEN 1 ELSE 0 END) AS operates_june_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 7 AND MONTH(ptc.EndDate) >= 7) THEN 1 ELSE 0 END) AS operates_july_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 8 AND MONTH(ptc.EndDate) >= 8) THEN 1 ELSE 0 END) AS operates_august_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 9 AND MONTH(ptc.EndDate) >= 9) THEN 1 ELSE 0 END) AS operates_september_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 10 AND MONTH(ptc.EndDate) >= 10) THEN 1 ELSE 0 END) AS operates_october_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 11 AND MONTH(ptc.EndDate) >= 11) THEN 1 ELSE 0 END) AS operates_november_flag,
                            MAX(CASE WHEN ptc.ServicesFullCalendarYear = 1 OR (MONTH(ptc.BeginDate) <= 12 AND MONTH(ptc.EndDate) >= 12) THEN 1 ELSE 0 END) AS operates_december_flag
                
                        FROM Classroom c
                        INNER JOIN ProgramTermClassroom ptc ON c.ClassroomID = ptc.ClassroomID
                        INNER JOIN ProgramTerm pt ON ptc.ProgramTermID = pt.ProgramTermID
                        WHERE c.SiteID = s.SiteID AND c.Active = 1 AND pt.Active = 1
                    ) AS ptc_consolidated
        
                    -- Attendance Configuration
                    OUTER APPLY (
                        SELECT 
                            CASE 
                                WHEN MAX(CASE WHEN c.AttendanceDefaultEntryMode = 3 THEN 1 ELSE 0 END) = 1 THEN 'Partially'
                                WHEN MAX(CASE WHEN c.AttendanceDefaultEntryMode = 2 THEN 1 ELSE 0 END) = 1 THEN 'Auto'
                                ELSE 'Manual'
                            END AS import_method
                        FROM Classroom c
                        WHERE c.SiteID = s.SiteID AND c.Active = 1
                    ) AS attendance_config

                WHERE s.Active = 1
                  AND a.Active = 1
                  AND a.AgencyID = @AgencyId
            )

            -- Final SELECT with all columns
            SELECT 
                SiteId,
    
                -- Timestamps for Composite Tracking
                MainTimestamp AS site_timestamp,
                CompositeTimestamp,
                TimestampDetail,
    
                -- All other fields
                SiteName, state_assigned_number, phone, fax_phone, alternate_phone,
                center_status_code, current_claim_monthyear, email, ssn_tax,
                alternate_assigned_number, corporation_name, url, business_type_code,
                profit_status_code, profit_type_code, title_xx_number, title_xix_number,
                food_service_type_code, food_service_vendor_contact_name,
                food_service_vendor_contact_phone, food_service_vendor_contact_email,
                food_service_vendor_approx_annual_cost, monitor_staff_id,
                pricing_program_flag, breakfast_charge, lunch_dinner_charge, snack_charge,
                meal_service_type_code, insurance_start_date, insurance_end_date,
                insurance_vendor_name, insurance_type_code, insurance_description_text,
                administration_type_code, cacfp_original_start_date,
                cacfp_allowed_start_date, cacfp_current_agreement_start_date,
                cacfp_current_expiration_start_date, removal_date, removal_reason_code,
                hold_reason_text, school_care_school_name,
                school_care_activities_enrichment_flag, school_care_activities_education_flag,
    
                -- Inspections
                sanitation_expiration_date, sanitation_required_flag,
                health_inspection_expiration_date, health_inspection_required_flag,
                fire_inspection_expiration_date, fire_inspection_required_flag,
                first_claim_received_date,
    
                -- Address
                address_line, city_name, county_id, county_name, state_code, zip_code,
                mailing_addressee_name, mailing_address_line, mailing_city_name,
                mailing_state_code, mailing_zip_code, previous_sponsor_name,
                map_location, school_district_id, latitude_degree, longitude_degree,
                driving_directions_text, claim_source_code, next_review_required_date,
                referred_by_text, user_security_group, sponsor_notes_text, comments_text,
                language_code, bank_account_code, bank_account_number, bank_routing_number,
                monthly_check_deduction_amount, use_direct_deposit_flag, minimum_version,
                block_claim_last_legitimized_date,
                director_name_text, mileage_to_center, day_of_entry_requirement_code,
                extended_capacity, pro_status_code, pro_expiration_date,
                review_year_start_month, admin_rate, district_number,
                enroll_expiration_month_number, allow_center_override_enrollment_flag,
                check_in_out_times_flag, re_sync_procare_with_cx, master_menu_flag,
                prevent_select_all_record_attendance_flag, use_procare_flag,
                skip_menu_checks_flag, meal_pattern_effective_date, 
                require_center_enter_receipts_flag, SSPS,
    
                -- License
                license_id, program_type_code, program_type_name, at_risk_flag,
                funding_source_code, allowed_starting_age_type_code,
                allowed_starting_age_number, allowed_ending_age_type_code,
                allowed_ending_age_number, allow_breakfast_flag, allow_am_snack_flag,
                allow_lunch_flag, allow_pm_snack_flag, allow_dinner_flag,
                allow_evening_snack_flag, license_number, license_start_date,
                license_end_date, license_does_not_expire_flag,
                license_max_capacity_number, waiver_flag, 
                license_mod_login_id, state_number, at_risk_number,
                license_max_infants, license_max_toddlers, license_max_preschoolers,
                license_max_schoolagers, allow_atrisk_breakfast_flag,
                allow_atrisk_am_snack_flag, allow_atrisk_lunch_flag,
                allow_atrisk_pm_snack_flag, allow_atrisk_dinner_flag,
                allow_atrisk_eve_snack_flag, default_license_flag, school_flag,
                license_max_approved_meal_number, waiver_breakfast_flag,
                waiver_am_snack_flag, waiver_lunch_flag, waiver_pm_snack_flag,
                waiver_dinner_flag, waiver_eve_snack_flag,
    
                -- Extended Property
                StateSiteNumber, ExternalId, RuralOrSelfPrepSite,
                LicenseSharedMaxCapacityNumber,
    
                -- Non-Congregate Meal
                allow_meal_served_flag,
    
                -- Schedule
                open_time, close_time, open_24_hours_flag, allow_monday_flag,
                allow_tuesday_flag, allow_wednesday_flag,
                allow_thursday_flag, allow_friday_flag,
                allow_saturday_flag, allow_sunday_flag,
                first_standard_breakfast_time, first_ending_breakfast_time,
                second_standard_breakfast_time, second_ending_breakfast_time,
                first_standard_am_snack_time, first_ending_am_snack_time,
                second_standard_am_snack_time, second_ending_am_snack_time,
                first_standard_lunch_time, first_ending_lunch_time,
                second_standard_lunch_time, second_ending_lunch_time,
                first_standard_pm_snack_time, first_ending_pm_snack_time,
                second_standard_pm_snack_time, second_ending_pm_snack_time,
                first_standard_dinner_time, first_ending_dinner_time,
                second_standard_dinner_time, second_ending_dinner_time,
                first_standard_evening_snack_time, first_ending_evening_snack_time,
                second_standard_evening_snack_time, second_ending_evening_snack_time,
                night_open_time, night_close_time, highest_serving_allowed_code,
                operates_january_flag, operates_february_flag, operates_march_flag,
                operates_april_flag, operates_may_flag, operates_june_flag,
                operates_july_flag, operates_august_flag, operates_september_flag,
                operates_october_flag, operates_november_flag, operates_december_flag,
                third_standard_breakfast_time, third_ending_breakfast_time,
                third_standard_am_snack_time, third_ending_am_snack_time,
                third_standard_lunch_time, third_ending_lunch_time,
                third_standard_pm_snack_time, third_ending_pm_snack_time,
                third_standard_dinner_time, third_ending_dinner_time,
                third_standard_evening_snack_time, third_ending_evening_snack_time,
    
                -- Master Menu
                MasterMenuClaimId, MasterMenuId,
    
                -- Attendance
                import_method, import_source, attendance_reason,
    
                -- System Fields
                system_defined, federal_interest_established, agency_id
    
            FROM SiteData;";


        public const string GetStaffBySiteId = @"
            SELECT StaffId, CenterId, FirstName, LastName, Email, Phone, 
                   Position, HireDate, IsActive, Timestamp, LastModified
            FROM Staff
            WHERE CenterId = @CenterId AND IsActive = 1";

        public const string GetChildrenBySiteId = @"
            SELECT ChildId, CenterId, FirstName, LastName, DateOfBirth, 
                   Gender, Address, City, State, ZipCode, Timestamp, LastModified
            FROM Children
            WHERE CenterId = @CenterId AND IsActive = 1";

        public const string GetGuardiansByChildId = @"
            SELECT GuardianId, ChildId, FirstName, LastName, Relationship, 
                   Phone, Email, IsPrimary, Timestamp, LastModified
            FROM Guardians
            WHERE ChildId = @ChildId";

        public const string GetEnrollmentsByChildId = @"
            SELECT EnrollmentId, ChildId, CenterId, EnrollmentDate, 
                   StartDate, EndDate, Status, Timestamp, LastModified
            FROM Enrollments
            WHERE ChildId = @ChildId";

        public const string GetAttendanceByChildId = @"
            SELECT AttendanceId, ChildId, CenterId, AttendanceDate, 
                   CheckInTime, CheckOutTime, Status, Timestamp, LastModified
            FROM Attendance
            WHERE ChildId = @ChildId 
            AND AttendanceDate >= DATEADD(MONTH, -3, GETDATE())";
    }
}
// DO NOT EDIT MANUALLY! This file was generated by the script "generate_value_set_lookups.rb"

namespace VRDR
{
    /// <summary> ValueSet Helpers </summary>
    public static class ValueSets
    {
        /// <summary> ActivityAtTimeOfDeath </summary>
        public static class ActivityAtTimeOfDeath {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "0", "While engaged in sports activity", VR.CodeSystems.ActivityAtTimeOfDeath },
                { "1", "While engaged in leisure activities.", VR.CodeSystems.ActivityAtTimeOfDeath },
                { "2", "While working for income", VR.CodeSystems.ActivityAtTimeOfDeath },
                { "3", "While engaged in other types of work", VR.CodeSystems.ActivityAtTimeOfDeath },
                { "4", "While resting, sleeping, eating, or engaging in other vital activities", VR.CodeSystems.ActivityAtTimeOfDeath },
                { "8", "While engaged in other specified activities.", VR.CodeSystems.ActivityAtTimeOfDeath },
                { "9", "During unspecified activity", VR.CodeSystems.ActivityAtTimeOfDeath },
                { "UNK", "unknown", VR.CodeSystems.NullFlavor_HL7_V3 },
            };

            /// <summary> While_Engaged_In_Sports_Activity </summary>
            public static string While_Engaged_In_Sports_Activity = "0";
            /// <summary> While_Engaged_In_Leisure_Activities </summary>
            public static string While_Engaged_In_Leisure_Activities = "1";
            /// <summary> While_Working_For_Income </summary>
            public static string While_Working_For_Income = "2";
            /// <summary> While_Engaged_In_Other_Types_Of_Work </summary>
            public static string While_Engaged_In_Other_Types_Of_Work = "3";
            /// <summary> While_Resting_Sleeping_Eating_Or_Engaging_In_Other_Vital_Activities </summary>
            public static string While_Resting_Sleeping_Eating_Or_Engaging_In_Other_Vital_Activities = "4";
            /// <summary> While_Engaged_In_Other_Specified_Activities </summary>
            public static string While_Engaged_In_Other_Specified_Activities = "8";
            /// <summary> During_Unspecified_Activity </summary>
            public static string During_Unspecified_Activity = "9";
            /// <summary> Unknown </summary>
            public static string Unknown = "UNK";
        };

        /// <summary> AdministrativeGender </summary>
        public static class AdministrativeGender {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "male", "Male", VR.CodeSystems.VRDRAdministrativeGender },
                { "female", "Female", VR.CodeSystems.VRDRAdministrativeGender },
                { "unknown", "unknown", VR.CodeSystems.VRDRAdministrativeGender },
            };

            /// <summary> Male </summary>
            public static string Male = "male";
            /// <summary> Female </summary>
            public static string Female = "female";
            /// <summary> Unknown </summary>
            public static string Unknown = "unknown";
        };

        /// <summary> CertifierTypes </summary>
        public static class CertifierTypes {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "455381000124109", "Death certification by medical examiner or coroner (procedure)", VR.CodeSystems.SCT },
                { "434641000124105", "Death certification and verification by physician (procedure)", VR.CodeSystems.SCT },
                { "434651000124107", "Death certification by physician (procedure)", VR.CodeSystems.SCT },
                { "OTH", "Other (Specify)", VR.CodeSystems.NullFlavor_HL7_V3 },
            };

            /// <summary> Death_Certification_By_Medical_Examiner_Or_Coroner_Procedure </summary>
            public static string Death_Certification_By_Medical_Examiner_Or_Coroner_Procedure = "455381000124109";
            /// <summary> Death_Certification_And_Verification_By_Physician_Procedure </summary>
            public static string Death_Certification_And_Verification_By_Physician_Procedure = "434641000124105";
            /// <summary> Death_Certification_By_Physician_Procedure </summary>
            public static string Death_Certification_By_Physician_Procedure = "434651000124107";
            /// <summary> Other_Specify </summary>
            public static string Other_Specify = "OTH";
        };

        /// <summary> ContributoryTobaccoUse </summary>
        public static class ContributoryTobaccoUse {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "373066001", "Yes", VR.CodeSystems.SCT },
                { "373067005", "No", VR.CodeSystems.SCT },
                { "2931005", "Probably", VR.CodeSystems.SCT },
                { "UNK", "Unknown", VR.CodeSystems.NullFlavor_HL7_V3 },
                { "NI", "no information", VR.CodeSystems.NullFlavor_HL7_V3 },
            };

            /// <summary> Yes </summary>
            public static string Yes = "373066001";
            /// <summary> No </summary>
            public static string No = "373067005";
            /// <summary> Probably </summary>
            public static string Probably = "2931005";
            /// <summary> Unknown </summary>
            public static string Unknown = "UNK";
            /// <summary> No_Information </summary>
            public static string No_Information = "NI";
        };

        /// <summary> DateOfDeathDeterminationMethods </summary>
        public static class DateOfDeathDeterminationMethods {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "exact", "Exact", VR.CodeSystems.DateOfDeathDeterminationMethods },
                { "approximate", "Approximate", VR.CodeSystems.DateOfDeathDeterminationMethods },
                { "court-appointed", "Court Appointed", VR.CodeSystems.DateOfDeathDeterminationMethods },
            };

            /// <summary> Exact </summary>
            public static string Exact = "exact";
            /// <summary> Approximate </summary>
            public static string Approximate = "approximate";
            /// <summary> Court_Appointed </summary>
            public static string Court_Appointed = "court-appointed";
        };

        /// <summary> DeathCertificationEventMax </summary>
        public static class DeathCertificationEventMax {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "103693007", "Diagnostic procedure (procedure)", VR.CodeSystems.SCT },
            };

            /// <summary> Diagnostic_Procedure_Procedure </summary>
            public static string Diagnostic_Procedure_Procedure = "103693007";
        };

        /// <summary> DeathCertificationEvent </summary>
        public static class DeathCertificationEvent {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "307930005", "Death certificate (record artifact)", VR.CodeSystems.SCT },
            };

            /// <summary> Death_Certificate_Record_Artifact </summary>
            public static string Death_Certificate_Record_Artifact = "307930005";
        };

        /// <summary> DeathPregnancyStatus </summary>
        public static class DeathPregnancyStatus {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "1", "Not pregnant within past year", VR.CodeSystems.DeathPregnancyStatus },
                { "2", "Pregnant at time of death", VR.CodeSystems.DeathPregnancyStatus },
                { "3", "Not pregnant, but pregnant within 42 days of death", VR.CodeSystems.DeathPregnancyStatus },
                { "4", "Not pregnant, but pregnant 43 days to 1 year before death", VR.CodeSystems.DeathPregnancyStatus },
                { "7", "Not reported on certificate", VR.CodeSystems.DeathPregnancyStatus },
                { "9", "Unknown if pregnant within the past year", VR.CodeSystems.DeathPregnancyStatus },
                { "NA", "Not applicable", VR.CodeSystems.NullFlavor_HL7_V3 },
            };

            /// <summary> Not_Pregnant_Within_Past_Year </summary>
            public static string Not_Pregnant_Within_Past_Year = "1";
            /// <summary> Pregnant_At_Time_Of_Death </summary>
            public static string Pregnant_At_Time_Of_Death = "2";
            /// <summary> Not_Pregnant_But_Pregnant_Within_42_Days_Of_Death </summary>
            public static string Not_Pregnant_But_Pregnant_Within_42_Days_Of_Death = "3";
            /// <summary> Not_Pregnant_But_Pregnant_43_Days_To_1_Year_Before_Death </summary>
            public static string Not_Pregnant_But_Pregnant_43_Days_To_1_Year_Before_Death = "4";
            /// <summary> Not_Reported_On_Certificate </summary>
            public static string Not_Reported_On_Certificate = "7";
            /// <summary> Unknown_If_Pregnant_Within_The_Past_Year </summary>
            public static string Unknown_If_Pregnant_Within_The_Past_Year = "9";
            /// <summary> Not_Applicable </summary>
            public static string Not_Applicable = "NA";
        };

        /// <summary> EditBypass01 </summary>
        public static class EditBypass01 {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "0", "Edit Passed", VR.CodeSystems.VRCLEditFlags },
                { "1", "Edit Failed, Data Queried, and Verified", VR.CodeSystems.VRCLEditFlags },
            };

            /// <summary> Edit_Passed </summary>
            public static string Edit_Passed = "0";
            /// <summary> Edit_Failed_Data_Queried_And_Verified </summary>
            public static string Edit_Failed_Data_Queried_And_Verified = "1";
        };

        /// <summary> EditBypass012 </summary>
        public static class EditBypass012 {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "0", "Edit Passed", VR.CodeSystems.VRCLEditFlags },
                { "1", "Edit Failed, Data Queried, and Verified", VR.CodeSystems.VRCLEditFlags },
                { "2", "Edit Failed, Data Queried, but not Verified", VR.CodeSystems.VRCLEditFlags },
            };

            /// <summary> Edit_Passed </summary>
            public static string Edit_Passed = "0";
            /// <summary> Edit_Failed_Data_Queried_And_Verified </summary>
            public static string Edit_Failed_Data_Queried_And_Verified = "1";
            /// <summary> Edit_Failed_Data_Queried_But_Not_Verified </summary>
            public static string Edit_Failed_Data_Queried_But_Not_Verified = "2";
        };

        /// <summary> EditBypass0124 </summary>
        public static class EditBypass0124 {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "0", "Edit Passed", VR.CodeSystems.VRCLEditFlags },
                { "1", "Edit Failed, Data Queried, and Verified", VR.CodeSystems.VRCLEditFlags },
                { "2", "Edit Failed, Data Queried, but not Verified", VR.CodeSystems.VRCLEditFlags },
                { "4", "Edit Failed, Query Needed", VR.CodeSystems.VRCLEditFlags },
            };

            /// <summary> Edit_Passed </summary>
            public static string Edit_Passed = "0";
            /// <summary> Edit_Failed_Data_Queried_And_Verified </summary>
            public static string Edit_Failed_Data_Queried_And_Verified = "1";
            /// <summary> Edit_Failed_Data_Queried_But_Not_Verified </summary>
            public static string Edit_Failed_Data_Queried_But_Not_Verified = "2";
            /// <summary> Edit_Failed_Query_Needed </summary>
            public static string Edit_Failed_Query_Needed = "4";
        };

        /// <summary> FilingFormat </summary>
        public static class FilingFormat {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "electronic", "Electronic", VR.CodeSystems.FilingFormat },
                { "paper", "Paper", VR.CodeSystems.FilingFormat },
                { "mixed", "Mixed", VR.CodeSystems.FilingFormat },
            };

            /// <summary> Electronic </summary>
            public static string Electronic = "electronic";
            /// <summary> Paper </summary>
            public static string Paper = "paper";
            /// <summary> Mixed </summary>
            public static string Mixed = "mixed";
        };

        /// <summary> IntentionalReject </summary>
        public static class IntentionalReject {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "1", "Reject1", VR.CodeSystems.IntentionalReject },
                { "2", "Reject2", VR.CodeSystems.IntentionalReject },
                { "3", "Reject3", VR.CodeSystems.IntentionalReject },
                { "4", "Reject4", VR.CodeSystems.IntentionalReject },
                { "5", "Reject5", VR.CodeSystems.IntentionalReject },
                { "9", "Reject9", VR.CodeSystems.IntentionalReject },
            };

            /// <summary> Reject1 </summary>
            public static string Reject1 = "1";
            /// <summary> Reject2 </summary>
            public static string Reject2 = "2";
            /// <summary> Reject3 </summary>
            public static string Reject3 = "3";
            /// <summary> Reject4 </summary>
            public static string Reject4 = "4";
            /// <summary> Reject5 </summary>
            public static string Reject5 = "5";
            /// <summary> Reject9 </summary>
            public static string Reject9 = "9";
        };

        /// <summary> MannerOfDeath </summary>
        public static class MannerOfDeath {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "38605008", "Natural death", VR.CodeSystems.SCT },
                { "7878000", "Accidental death", VR.CodeSystems.SCT },
                { "44301001", "Suicide", VR.CodeSystems.SCT },
                { "27935005", "Homicide", VR.CodeSystems.SCT },
                { "185973002", "Patient awaiting investigation", VR.CodeSystems.SCT },
                { "65037004", "Death, manner undetermined", VR.CodeSystems.SCT },
            };

            /// <summary> Natural_Death </summary>
            public static string Natural_Death = "38605008";
            /// <summary> Accidental_Death </summary>
            public static string Accidental_Death = "7878000";
            /// <summary> Suicide </summary>
            public static string Suicide = "44301001";
            /// <summary> Homicide </summary>
            public static string Homicide = "27935005";
            /// <summary> Patient_Awaiting_Investigation </summary>
            public static string Patient_Awaiting_Investigation = "185973002";
            /// <summary> Death_Manner_Undetermined </summary>
            public static string Death_Manner_Undetermined = "65037004";
        };

        /// <summary> MethodOfDisposition </summary>
        public static class MethodOfDisposition {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "449931000124108", "Entombment", VR.CodeSystems.SCT },
                { "449941000124103", "Removal from state", VR.CodeSystems.SCT },
                { "449951000124101", "Donation", VR.CodeSystems.SCT },
                { "449961000124104", "Cremation", VR.CodeSystems.SCT },
                { "449971000124106", "Burial", VR.CodeSystems.SCT },
                { "OTH", "Other", VR.CodeSystems.NullFlavor_HL7_V3 },
                { "UNK", "Unknown", VR.CodeSystems.NullFlavor_HL7_V3 },
            };

            /// <summary> Entombment </summary>
            public static string Entombment = "449931000124108";
            /// <summary> Removal_From_State </summary>
            public static string Removal_From_State = "449941000124103";
            /// <summary> Donation </summary>
            public static string Donation = "449951000124101";
            /// <summary> Cremation </summary>
            public static string Cremation = "449961000124104";
            /// <summary> Burial </summary>
            public static string Burial = "449971000124106";
            /// <summary> Other </summary>
            public static string Other = "OTH";
            /// <summary> Unknown </summary>
            public static string Unknown = "UNK";
        };

        /// <summary> PlaceOfDeath </summary>
        public static class PlaceOfDeath {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "63238001", "Dead on arrival at hospital", VR.CodeSystems.SCT },
                { "440081000124100", "Death in home", VR.CodeSystems.SCT },
                { "440071000124103", "Death in hospice", VR.CodeSystems.SCT },
                { "16983000", "Death in hospital", VR.CodeSystems.SCT },
                { "450391000124102", "Death in hospital-based emergency department or outpatient department", VR.CodeSystems.SCT },
                { "450381000124100", "Death in nursing home or long term care facility", VR.CodeSystems.SCT },
                { "OTH", "Other", VR.CodeSystems.NullFlavor_HL7_V3 },
                { "UNK", "UNK", VR.CodeSystems.NullFlavor_HL7_V3 },
            };

            /// <summary> Dead_On_Arrival_At_Hospital </summary>
            public static string Dead_On_Arrival_At_Hospital = "63238001";
            /// <summary> Death_In_Home </summary>
            public static string Death_In_Home = "440081000124100";
            /// <summary> Death_In_Hospice </summary>
            public static string Death_In_Hospice = "440071000124103";
            /// <summary> Death_In_Hospital </summary>
            public static string Death_In_Hospital = "16983000";
            /// <summary> Death_In_Hospital_Based_Emergency_Department_Or_Outpatient_Department </summary>
            public static string Death_In_Hospital_Based_Emergency_Department_Or_Outpatient_Department = "450391000124102";
            /// <summary> Death_In_Nursing_Home_Or_Long_Term_Care_Facility </summary>
            public static string Death_In_Nursing_Home_Or_Long_Term_Care_Facility = "450381000124100";
            /// <summary> Other </summary>
            public static string Other = "OTH";
            /// <summary> Unk </summary>
            public static string Unk = "UNK";
        };

        /// <summary> PlaceOfInjuryOther </summary>
        public static class PlaceOfInjuryOther {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "D", "Military Residence", VR.CodeSystems.PlaceOfInjury },
                { "E", "Hospital", VR.CodeSystems.PlaceOfInjury },
                { "H", "Garage/Warehouse", VR.CodeSystems.PlaceOfInjury },
                { "J", "Mine/Quarry", VR.CodeSystems.PlaceOfInjury },
                { "L", "Public Recreation Area", VR.CodeSystems.PlaceOfInjury },
                { "M", "Institutional Recreation Area", VR.CodeSystems.PlaceOfInjury },
                { "P", "Other specified place", VR.CodeSystems.PlaceOfInjury },
            };

            /// <summary> Military_Residence </summary>
            public static string Military_Residence = "D";
            /// <summary> Hospital </summary>
            public static string Hospital = "E";
            /// <summary> Garage_Warehouse </summary>
            public static string Garage_Warehouse = "H";
            /// <summary> Mine_Quarry </summary>
            public static string Mine_Quarry = "J";
            /// <summary> Public_Recreation_Area </summary>
            public static string Public_Recreation_Area = "L";
            /// <summary> Institutional_Recreation_Area </summary>
            public static string Institutional_Recreation_Area = "M";
            /// <summary> Other_Specified_Place </summary>
            public static string Other_Specified_Place = "P";
        };

        /// <summary> PlaceOfInjury </summary>
        public static class PlaceOfInjury {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "LA14084-0", "Home", VR.CodeSystems.LOINC },
                { "LA14085-7", "Residential institution", VR.CodeSystems.LOINC },
                { "LA14086-5", "School", VR.CodeSystems.LOINC },
                { "LA14088-1", "Sports or recreational area", VR.CodeSystems.LOINC },
                { "LA14089-9", "Street or highway", VR.CodeSystems.LOINC },
                { "LA14090-7", "Trade or service area", VR.CodeSystems.LOINC },
                { "LA14091-5", "Industrial or construction area", VR.CodeSystems.LOINC },
                { "LA14092-3", "Farm", VR.CodeSystems.LOINC },
                { "LA14093-1", "Unspecified", VR.CodeSystems.LOINC },
                { "OTH", "Other", VR.CodeSystems.NullFlavor_HL7_V3 },
            };

            /// <summary> Home </summary>
            public static string Home = "LA14084-0";
            /// <summary> Residential_Institution </summary>
            public static string Residential_Institution = "LA14085-7";
            /// <summary> School </summary>
            public static string School = "LA14086-5";
            /// <summary> Sports_Or_Recreational_Area </summary>
            public static string Sports_Or_Recreational_Area = "LA14088-1";
            /// <summary> Street_Or_Highway </summary>
            public static string Street_Or_Highway = "LA14089-9";
            /// <summary> Trade_Or_Service_Area </summary>
            public static string Trade_Or_Service_Area = "LA14090-7";
            /// <summary> Industrial_Or_Construction_Area </summary>
            public static string Industrial_Or_Construction_Area = "LA14091-5";
            /// <summary> Farm </summary>
            public static string Farm = "LA14092-3";
            /// <summary> Unspecified </summary>
            public static string Unspecified = "LA14093-1";
            /// <summary> Other </summary>
            public static string Other = "OTH";
        };

        /// <summary> ReplaceStatus </summary>
        public static class ReplaceStatus {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "original", "original record", VR.CodeSystems.ReplaceStatus },
                { "updated", "updated record", VR.CodeSystems.ReplaceStatus },
                { "updated_notforNCHS", "updated record not for nchs", VR.CodeSystems.ReplaceStatus },
            };

            /// <summary> Original_Record </summary>
            public static string Original_Record = "original";
            /// <summary> Updated_Record </summary>
            public static string Updated_Record = "updated";
            /// <summary> Updated_Record_Not_For_Nchs </summary>
            public static string Updated_Record_Not_For_Nchs = "updated_notforNCHS";
        };

        /// <summary> SpouseAlive </summary>
        public static class SpouseAlive {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "Y", "Yes", VR.CodeSystems.YesNo },
                { "N", "No", VR.CodeSystems.YesNo },
                { "UNK", "unknown", VR.CodeSystems.NullFlavor_HL7_V3 },
                { "NA", "not applicable", VR.CodeSystems.NullFlavor_HL7_V3 },
            };

            /// <summary> Yes </summary>
            public static string Yes = "Y";
            /// <summary> No </summary>
            public static string No = "N";
            /// <summary> Unknown </summary>
            public static string Unknown = "UNK";
            /// <summary> Not_Applicable </summary>
            public static string Not_Applicable = "NA";
        };

        /// <summary> SystemReject </summary>
        public static class SystemReject {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "0", "Not Rejected", VR.CodeSystems.SystemReject },
                { "1", "MICAR Reject Dictionary Match", VR.CodeSystems.SystemReject },
                { "2", "ACME Reject", VR.CodeSystems.SystemReject },
                { "3", "MICAR Reject Rule Application", VR.CodeSystems.SystemReject },
                { "4", "Record Reviewed", VR.CodeSystems.SystemReject },
            };

            /// <summary> Not_Rejected </summary>
            public static string Not_Rejected = "0";
            /// <summary> Micar_Reject_Dictionary_Match </summary>
            public static string Micar_Reject_Dictionary_Match = "1";
            /// <summary> Acme_Reject </summary>
            public static string Acme_Reject = "2";
            /// <summary> Micar_Reject_Rule_Application </summary>
            public static string Micar_Reject_Rule_Application = "3";
            /// <summary> Record_Reviewed </summary>
            public static string Record_Reviewed = "4";
        };

        /// <summary> TransaxConversion </summary>
        public static class TransaxConversion {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "3", "Conversion using non-ambivalent table entries", VR.CodeSystems.TransaxConversion },
                { "4", "Conversion using ambivalent table entries", VR.CodeSystems.TransaxConversion },
                { "5", "Duplicate entity-axis codes deleted; no other action involved", VR.CodeSystems.TransaxConversion },
                { "6", "Artificial code conversion; no other action", VR.CodeSystems.TransaxConversion },
            };

            /// <summary> Conversion_Using_Non_Ambivalent_Table_Entries </summary>
            public static string Conversion_Using_Non_Ambivalent_Table_Entries = "3";
            /// <summary> Conversion_Using_Ambivalent_Table_Entries </summary>
            public static string Conversion_Using_Ambivalent_Table_Entries = "4";
            /// <summary> Duplicate_Entity_Axis_Codes_Deleted_No_Other_Action_Involved </summary>
            public static string Duplicate_Entity_Axis_Codes_Deleted_No_Other_Action_Involved = "5";
            /// <summary> Artificial_Code_Conversion_No_Other_Action </summary>
            public static string Artificial_Code_Conversion_No_Other_Action = "6";
        };

        /// <summary> TransportationIncidentRole </summary>
        public static class TransportationIncidentRole {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "236320001", "Vehicle driver", VR.CodeSystems.SCT },
                { "257500003", "Passenger", VR.CodeSystems.SCT },
                { "257518000", "Pedestrian", VR.CodeSystems.SCT },
                { "OTH", "Other", VR.CodeSystems.NullFlavor_HL7_V3 },
                { "UNK", "unknown", VR.CodeSystems.NullFlavor_HL7_V3 },
                { "NA", "not applicable", VR.CodeSystems.NullFlavor_HL7_V3 },
            };

            /// <summary> Vehicle_Driver </summary>
            public static string Vehicle_Driver = "236320001";
            /// <summary> Passenger </summary>
            public static string Passenger = "257500003";
            /// <summary> Pedestrian </summary>
            public static string Pedestrian = "257518000";
            /// <summary> Other </summary>
            public static string Other = "OTH";
            /// <summary> Unknown </summary>
            public static string Unknown = "UNK";
            /// <summary> Not_Applicable </summary>
            public static string Not_Applicable = "NA";
        };

    }
}

// DO NOT EDIT MANUALLY! This file was generated by the script "scripts/generate_value_set_lookups_from_BFDR_IG.rb"

namespace BFDR
{
    /// <summary> ValueSet Helpers </summary>
    public static class ValueSets
    {

        /// <summary> BirthSex </summary>
        public static class BirthSex
        {

            // TODO: These are the current mappings in the IG, these need to be changed to a 1-1 relationship from IJE to FHIR
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "M", "Male", VR.CodeSystems.AdministrativeGender },
                { "F", "Female", VR.CodeSystems.AdministrativeGender },
                { "UNK", "unknown", VR.CodeSystems.NullFlavor_HL7_V3},
                { "ASKU", "asked but unknown", VR.CodeSystems.NullFlavor_HL7_V3},
                { "UNK", "other", VR.CodeSystems.NullFlavor_HL7_V3}
            };
            /// <summary> Male </summary>
            public static string Male = "M";
            /// <summary> Female </summary>
            public static string Female = "F";
            /// <summary> Asked but Unknown </summary>
            public static string AskedButUnknown = "ASKU";
            /// <summary> Other </summary>
            public static string Other = "OTH";
            /// <summary> Unknown </summary>
            public static string Unknown = "UNK";

        };

        /// <summary> PlaceTypeOfBirth </summary>
        public static class PlaceTypeOfBirth
        {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "22232009", "Hospital", VR.CodeSystems.SCT },
                { "91154008", "Free-standing birthing center", VR.CodeSystems.SCT },
                { "408839006", "Planned home birth", VR.CodeSystems.SCT },
                { "408838003", "Unplanned home birth", VR.CodeSystems.SCT },
                { "67190003", "Free-standing clinic", VR.CodeSystems.SCT },
                { "unknownplannedhomebirth", "Unknown if Planned Home Birth", "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-vr-birth-delivery-occurred" },
                { "OTH", "Other", VR.CodeSystems.NullFlavor_HL7_V3 },
                { "UNK", "Unknown", VR.CodeSystems.NullFlavor_HL7_V3 }
            };
            /// <summary> Hospital </summary>
            public static string Hospital = "22232009";
            /// <summary> Free_Standing_Birthing_Center </summary>
            public static string Free_Standing_Birthing_Center = "91154008";
            /// <summary> Planned_Home_Birth </summary>
            public static string Planned_Home_Birth = "408839006";
            /// <summary> Unplanned_Home_Birth </summary>
            public static string Unplanned_Home_Birth = "408838003";
            /// <summary> Free_Standing_Clinic </summary>
            public static string Free_Standing_Clinic = "67190003";
            /// <summary> Unknown_If_Planned_Home_Birth </summary>
            public static string Unknown_If_Planned_Home_Birth = "unknownplannedhomebirth";
            /// <summary> Other </summary>
            public static string Other = "OTH";
            /// <summary> Unknown </summary>
            public static string Unknown = "UNK";
        };

        /// <summary> PayorType </summary>
        public static class PayorType {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "indianhealth", "Indian Health Service or Tribe", VR.CodeSystems.PayorType },
                { "medicaid", "MEDICAID", VR.CodeSystems.PayorType },
                { "nosource", "No Typology Code available for payment source", VR.CodeSystems.PayorType },
                { "othergov", "Other Government (Federal, State, Local not specified)", VR.CodeSystems.PayorType },
                { "privateinsurance", "PRIVATE HEALTH INSURANCE", VR.CodeSystems.PayorType },
                { "selfpay", "Self-pay", VR.CodeSystems.PayorType },
                { "tricare", "TRICARE (CHAMPUS)", VR.CodeSystems.PayorType },
                { "unknown", "Unavailable / Unknown", VR.CodeSystems.PayorType }
            };
            /// <summary> Indian_Health_Service_Or_Tribe </summary>
            public static string  Indian_Health_Service_Or_Tribe = "indianhealth";
            /// <summary> Medicaid </summary>
            public static string  Medicaid = "medicaid";
            /// <summary> No_Typology_Code_Available_For_Payment_Source </summary>
            public static string  No_Typology_Code_Available_For_Payment_Source = "nosource";
            /// <summary> Other_Government_Federal_State_Local_Not_Specified </summary>
            public static string  Other_Government_Federal_State_Local_Not_Specified = "othergov";
            /// <summary> Private_Health_Insurance </summary>
            public static string  Private_Health_Insurance = "privateinsurance";
            /// <summary> Self_Pay </summary>
            public static string  Self_Pay = "selfpay";
            /// <summary> Tricare_Champus </summary>
            public static string  Tricare_Champus = "tricare";
            /// <summary> Unavailable_Unknown </summary>
            public static string  Unavailable_Unknown = "unknown";
        };
    }
}
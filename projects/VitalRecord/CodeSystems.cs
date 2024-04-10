namespace VR
{
    /// <summary>Single definitions for CodeSystem OIDs and URLs used throughout. </summary>
    public static class CodeSystems
    {
        // Code systems defined outside IGs

        /// <summary>SNOMEDCT.</summary>
        public static string SCT = "http://snomed.info/sct";

        /// <summary>LOINC.</summary>
        public static string LOINC = "http://loinc.org";

        /// <summary> ICD10 </summary>
        public static string ICD10 = "http://hl7.org/fhir/sid/icd-10";

        /// <summary>Social Security Numbers.</summary>
        public static string US_SSN = "http://hl7.org/fhir/sid/us-ssn";

        /// <summary>Observation Category. </summary>
        public static string ObservationCategory = "http://terminology.hl7.org/CodeSystem/observation-category";

        /// <summary>HL7 V3 Null Flavor.</summary>
        public static string NullFlavor_HL7_V3 = "http://terminology.hl7.org/CodeSystem/v3-NullFlavor";

        /// <summary>HL7 Data Absent reason.</summary>
        public static string Data_Absent_Reason_HL7_V3 = "http://terminology.hl7.org/CodeSystem/data-absent-reason";

        /// <summary>HL7 Yes No.</summary>
        public static string YesNo_0136HL7_V2 = "http://terminology.hl7.org/CodeSystem/v2-0136";

        /// <summary>PHINVADS Marital Status.</summary>
        public static string PH_MaritalStatus_HL7_2x = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus";

        /// <summary>HL7 Location Physical Type.</summary>
        public static string HL7_location_physical_type = "http://terminology.hl7.org/CodeSystem/location-physical-type";

        /// <summary> US NPI HL7  </summary>
        public static string US_NPI_HL7 = "http://hl7.org/fhir/sid/us-npi";

        /// <summary>HL7 Identifier Type.</summary>
        public static string HL7_identifier_type = "http://terminology.hl7.org/CodeSystem/v2-0203";

        /// <summary>HL7 RoleCode.</summary>
        public static string RoleCode_HL7_V3 = "http://terminology.hl7.org/CodeSystem/v3-RoleCode";

        /// <summary> VRDR Administrative Gender </summary>
        public static string VRDRAdministrativeGender = "http://hl7.org/fhir/administrative-gender";

        /// <summary> VRCL Administrative Gender </summary>
        public static string VRCLAdministrativeGender = "http://terminology.hl7.org/CodeSystem/v3-AdministrativeGender";

        /// <summary> Education Level </summary>
        public static string EducationLevel = "http://terminology.hl7.org/CodeSystem/v3-EducationLevel";

        /// <summary> Degree Licence and Certificate </summary>
        public static string DegreeLicenceAndCertificate = "http://terminology.hl7.org/CodeSystem/v2-0360";

        /// <summary> Units of Measure </summary>
        public static string UnitsOfMeasure = "http://unitsofmeasure.org";

        /// <summary> HL7 Yes No </summary>
        public static string YesNo = "http://terminology.hl7.org/CodeSystem/v2-0136";

        /// <summary> USPS </summary>
        public static string USPS = "https://www.usps.com";

        /// <summary> NAHDO </summary>
        public static string NAHDO = "https://nahdo.org/sopt";

        /// <summary> ActReason </summary>
        public static string ActReason = "http://terminology.hl7.org/CodeSystem/v3-ActReason";

        /// <summary> ProvinceCodes </summary>
        public static string ProvinceCodes = "https://canadapost.ca/CodeSystem/ProvinceCodes";

        // Code systems defined within vital record IGs: VRCL IG

        /// <summary> VRCL Missing Value Reason </summary>
        public static string VRCLMissingValueReason = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-missing-value-reason-vr";

        /// <summary> VRCL Race Code </summary>
        public static string VRCLRaceCode = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-race-code-vr";

        /// <summary> VRCL Race Recode 40 </summary>
        public static string VRCLRaceRecode40 = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-race-recode-40-vr";

        /// <summary> VRCL Hispanic Origin </summary>
        public static string VRCLHispanicOrigin = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-hispanic-origin-vr";

        /// <summary> Input Race and Ethnicity Person </summary>
        public static string InputRaceAndEthnicityPerson = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-local-observation-codes-vr";

        /// <summary> VRCL Edit Flags </summary>
        public static string VRCLEditFlags = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags";

        /// <summary> Local Observation Codes </summary>
        public static string LocalObservationCodes = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-local-observation-codes-vr";

        /// <summary> VRCL Jurisdictions </summary>
        public static string VRCLJurisdictions = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-jurisdictions-vr";

        /// <summary> VRCL States Territories </summary>
        public static string VRCLUSStatesTerritories = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-us-states-territories-vr";

        /// <summary> Component </summary>
        public static string ComponentCodeVR = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/codesystem-vr-component";

        // Code systems defined within vital record IGs: VRDR IG

        /// <summary> Hispanic Origin </summary>
        public static string HispanicOrigin = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-hispanic-origin-cs";

        /// <summary> Jurisdictions </summary>
        public static string Jurisdictions = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-jurisdictions-cs";

        /// <summary> Race Code </summary>
        public static string RaceCode = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-race-code-cs";

        /// <summary> Missing Value Reason </summary>
        public static string MissingValueReason = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-missing-value-reason-cs";

        /// <summary> Race Recode 40 </summary>
        public static string RaceRecode40 = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-race-recode-40-cs";

        /// <summary> Bypass Edit Flag </summary>
        public static string BypassEditFlag = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-bypass-edit-flag-cs";

        /// <summary> Pregnancy Status </summary>
        public static string PregnancyStatus = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-pregnancy-status-cs";

        /// <summary> Filing Format </summary>
        public static string FilingFormat = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-filing-format-cs";

        /// <summary> Composition document sections </summary>
        public static string DocumentSections = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-document-section-cs";

        /// <summary> Replace Status </summary>
        public static string ReplaceStatus = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-replace-status-cs";

        /// <summary> Location Type </summary>
        public static string LocationType = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-location-type-cs";

        /// <summary> Organization Type </summary>
        public static string OrganizationType = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-organization-type-cs";

        /// <summary> Activity at Time of Death </summary>
        public static string ActivityAtTimeOfDeath = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-activity-at-time-of-death-cs";

        /// <summary> Intentional Reject </summary>
        public static string IntentionalReject = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-intentional-reject-cs";

        /// <summary> System Reject </summary>
        public static string SystemReject = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-system-reject-cs";

        /// <summary> Hispanic Origin </summary>
        public static string TransaxConversion = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-transax-conversion-cs";

        /// <summary> Observation Codes </summary>
        public static string ObservationCode = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-observations-cs";

        /// <summary> Component Codes </summary>
        public static string ComponentCode = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-component-cs";

        /// <summary> Component </summary>
        public static string Component = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-component-cs";

        /// <summary> Date of Death Determination Methods </summary>
        public static string DateOfDeathDeterminationMethods = "http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-date-of-death-determination-methods-cs";
    
        /// <summary> Death Pregnancy Status </summary>
        public static string DeathPregnancyStatus = "http://hl7.org/fhir/us/vrdr/CodeSystem/CodeSystem-death-pregnancy-status";

        // Code systems defined within vital record IGs: BFDR IG

        /// <summary> Birth Delivery Occurred </summary>
        public static string BirthDeliveryOccurred = "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-vr-birth-delivery-occurred";

        /// <summary> BFDR Edit Flags </summary>
        public static string BFDREditFlags = "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-edit-flags";

        /// <summary> Fetal Death Cause Or Condition </summary>
        public static string FetalDeathCauseOrCondition = "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-vr-fetal-death-cause-or-condition";

        /// <summary> Informant Relationship To Mother </summary>
        public static string InformantRelationshipToMother = "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-informant-relationship-to-mother";

        /// <summary> Local BFDR Codes </summary>
        public static string LocalBFDRCodes = "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-local-bfdr-codes";

        /// <summary> Admit Source </summary>
        public static string AdmitSource = "http://terminology.hl7.org/CodeSystem/admit-source";

        /// <summary> Discharge Disposition </summary>
        public static string DischargeDisposition = "http://terminology.hl7.org/CodeSystem/discharge-disposition";
    }
}

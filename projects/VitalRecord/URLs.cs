// DO NOT EDIT MANUALLY! This file was generated by the script "tools/generate_url_strings_from_VR_IG.rb"

namespace VR
{
    /// <summary>Profile URLs</summary>
    public static class ProfileURL
    {
        /// <summary>URL for AutopsyPerformedIndicator</summary>
        public const string AutopsyPerformedIndicator = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Observation-autopsy-performed-indicator-vr";

        /// <summary>URL for EducationLevel</summary>
        public const string EducationLevel = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Observation-education-level-vr";

        /// <summary>URL for EmergingIssues</summary>
        public const string EmergingIssues = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Observation-emerging-issues-vr";

        /// <summary>URL for Child</summary>
        public const string Child = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Patient-child-vr";

        /// <summary>URL for Mother</summary>
        public const string Mother = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Patient-mother-vr";

        /// <summary>URL for Patient</summary>
        public const string Patient = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Patient-vr";

        /// <summary>URL for Practitioner</summary>
        public const string Practitioner = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Practitioner-vr";

        /// <summary>URL for RelatedPersonFatherNatural</summary>
        public const string RelatedPersonFatherNatural = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/RelatedPerson-father-natural-vr";

        /// <summary>URL for RelatedPersonFather</summary>
        public const string RelatedPersonFather = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/RelatedPerson-father-vr";

        /// <summary>URL for RelatedPersonMother</summary>
        public const string RelatedPersonMother = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/RelatedPerson-mother-vr";

        /// <summary>URL for RelatedPersonParent</summary>
        public const string RelatedPersonParent = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/RelatedPerson-parent-vr";

        /// <summary>URL for CodedRaceAndEthnicity</summary>
        public const string CodedRaceAndEthnicity = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/coded-race-and-ethnicity-vr";

        /// <summary>URL for InputRaceAndEthnicity</summary>
        public const string InputRaceAndEthnicity = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/input-race-and-ethnicity-vr";

    }

    /// <summary>Extension URLs</summary>
    public class ExtensionURL
    {
        private string _prefix;
        private const string DefaultURLPrefix = "http://hl7.org/fhir/us/vr-common-library";

        /// <summary>Constructor</summary>
        /// <param name="prefix">the prefix to use for extension URLs</param>
        public ExtensionURL(string prefix = DefaultURLPrefix)
        {
            _prefix = prefix;
        }

        // Special case processing for three extension URLs that are different between VR and VRDR
        private string Trim(string url)
        {
            if (_prefix.Equals(DefaultURLPrefix))
            {
                return url;
            }
            if (url.Contains("DatePartAbsentReason") || url.Contains("PartialDate"))
            {
                return url.Replace("Extension","").Replace("VitalRecords","");
            }
            return url;
        }

        /// <summary>URL for BypassEditFlag</summary>
        public string BypassEditFlag => Trim($"{_prefix}/StructureDefinition/BypassEditFlag");

        /// <summary>URL for CityCode</summary>
        public string CityCode => Trim($"{_prefix}/StructureDefinition/CityCode");

        /// <summary>URL for DistrictCode</summary>
        public string DistrictCode => Trim($"{_prefix}/StructureDefinition/DistrictCode");

        /// <summary>URL for PatientFetalDeath</summary>
        public string PatientFetalDeath => Trim($"{_prefix}/StructureDefinition/Extension-patient-fetal-death-vr");

        /// <summary>URL for RelatedpersonBirthplace</summary>
        public string RelatedpersonBirthplace => Trim($"{_prefix}/StructureDefinition/Extension-relatedperson-birthplace-vr");

        /// <summary>URL for RelatedPersonDeceased</summary>
        public string RelatedPersonDeceased => Trim($"{_prefix}/StructureDefinition/Extension-relatedperson-deceased-vr");

        /// <summary>URL for ReportedParentAgeAtDelivery</summary>
        public string ReportedParentAgeAtDelivery => Trim($"{_prefix}/StructureDefinition/Extension-reported-parent-age-at-delivery-vr");

        /// <summary>URL for WithinCityLimitsIndicator</summary>
        public string WithinCityLimitsIndicator => Trim($"{_prefix}/StructureDefinition/Extension-within-city-limits-indicator-vr");

        /// <summary>URL for DatePartAbsentReason</summary>
        public string DatePartAbsentReason => Trim($"{_prefix}/StructureDefinition/ExtensionDatePartAbsentReasonVitalRecords");

        /// <summary>URL for PartialDateTime</summary>
        public string PartialDateTime => Trim($"{_prefix}/StructureDefinition/ExtensionPartialDateTimeVitalRecords");

        /// <summary>URL for PartialDate</summary>
        public string PartialDate => Trim($"{_prefix}/StructureDefinition/ExtensionPartialDateVitalRecords");

        /// <summary>URL for PostDirectional</summary>
        public string PostDirectional => Trim($"{_prefix}/StructureDefinition/PostDirectional");

        /// <summary>URL for PreDirectional</summary>
        public string PreDirectional => Trim($"{_prefix}/StructureDefinition/PreDirectional");

        /// <summary>URL for StreetDesignator</summary>
        public string StreetDesignator => Trim($"{_prefix}/StructureDefinition/StreetDesignator");

        /// <summary>URL for StreetName</summary>
        public string StreetName => Trim($"{_prefix}/StructureDefinition/StreetName");

        /// <summary>URL for StreetNumber</summary>
        public string StreetNumber => Trim($"{_prefix}/StructureDefinition/StreetNumber");

        /// <summary>URL for UnitOrAptNumber</summary>
        public string UnitOrAptNumber => Trim($"{_prefix}/StructureDefinition/UnitOrAptNumber");

        /// <summary>URL for DateDay</summary>
        public string DateDay => $"{_prefix}/StructureDefinition/Date-Day";

        /// <summary>URL for DateYear</summary>
        public string DateYear => $"{_prefix}/StructureDefinition/Date-Year";

        /// <summary>URL for DateMonth</summary>
        public string DateMonth => $"{_prefix}/StructureDefinition/Date-Month";

        /// <summary>URL for DateTime</summary>
        public string DateTime => $"{_prefix}/StructureDefinition/Date-Time";

        /// <summary>URL for PatientBirthTime as defined in the VitalRecords IG</summary>
        public const string PatientBirthTime = "http://hl7.org/fhir/StructureDefinition/patient-birthTime";

        /// <summary>URL for PartialDateTime as defined in the VitalRecords IG</summary>
        public string PartialDateTimeVR => $"{_prefix}/StructureDefinition/ExtensionPartialDateTimeVitalRecords";        

        /// <summary>URL for PartialDateTime Day as defined in the VitalRecords IG</summary>
        public const string PartialDateTimeDayVR = "day";     

        /// <summary>URL for PartialDateTime Month as defined in the VitalRecords IG</summary>
        public const string PartialDateTimeMonthVR = "month";

        /// <summary>URL for PartialDateTime Year as defined in the VitalRecords IG</summary>
        public const string PartialDateTimeYearVR = "year";

        /// <summary>URL for PartialDateTime Time as defined in the VitalRecords IG</summary>
        public const string PartialDateTimeTimeVR = "time";

        /// <summary>URL for LocationJurisdictionId</summary>
        public string LocationJurisdictionId => $"{_prefix}/StructureDefinition/Location-Jurisdiction-Id";
    }

    /// <summary>IG URLs</summary>
    public static class IGURL
    {
        /// <summary>URL for BypassEditFlag</summary>
        public const string BypassEditFlag = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-BypassEditFlag.html";

        /// <summary>URL for CityCode</summary>
        public const string CityCode = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-CityCode.html";

        /// <summary>URL for DistrictCode</summary>
        public const string DistrictCode = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-DistrictCode.html";

        /// <summary>URL for PatientFetalDeath</summary>
        public const string PatientFetalDeath = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Extension-patient-fetal-death-vr.html";

        /// <summary>URL for RelatedpersonBirthplace</summary>
        public const string RelatedpersonBirthplace = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Extension-relatedperson-birthplace-vr.html";

        /// <summary>URL for RelatedPersonDeceased</summary>
        public const string RelatedPersonDeceased = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Extension-relatedperson-deceased-vr.html";

        /// <summary>URL for ReportedParentAgeAtDelivery</summary>
        public const string ReportedParentAgeAtDelivery = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Extension-reported-parent-age-at-delivery-vr.html";

        /// <summary>URL for WithinCityLimitsIndicator</summary>
        public const string WithinCityLimitsIndicator = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Extension-within-city-limits-indicator-vr.html";

        /// <summary>URL for DatePartAbsentReason</summary>
        public const string DatePartAbsentReason = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-ExtensionDatePartAbsentReasonVitalRecords.html";

        /// <summary>URL for PartialDateTime</summary>
        public const string PartialDateTime = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-ExtensionPartialDateTimeVitalRecords.html";

        /// <summary>URL for PartialDate</summary>
        public const string PartialDate = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-ExtensionPartialDateVitalRecords.html";

        /// <summary>URL for AutopsyPerformedIndicator</summary>
        public const string AutopsyPerformedIndicator = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Observation-autopsy-performed-indicator-vr.html";

        /// <summary>URL for EducationLevel</summary>
        public const string EducationLevel = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Observation-education-level-vr.html";

        /// <summary>URL for EmergingIssues</summary>
        public const string EmergingIssues = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Observation-emerging-issues-vr.html";

        /// <summary>URL for Child</summary>
        public const string Child = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Patient-child-vr.html";

        /// <summary>URL for Mother</summary>
        public const string Mother = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Patient-mother-vr.html";

        /// <summary>URL for Patient</summary>
        public const string Patient = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Patient-vr.html";

        /// <summary>URL for PostDirectional</summary>
        public const string PostDirectional = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-PostDirectional.html";

        /// <summary>URL for Practitioner</summary>
        public const string Practitioner = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-Practitioner-vr.html";

        /// <summary>URL for PreDirectional</summary>
        public const string PreDirectional = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-PreDirectional.html";

        /// <summary>URL for RelatedPersonFatherNatural</summary>
        public const string RelatedPersonFatherNatural = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-RelatedPerson-father-natural-vr.html";

        /// <summary>URL for RelatedPersonFather</summary>
        public const string RelatedPersonFather = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-RelatedPerson-father-vr.html";

        /// <summary>URL for RelatedPersonMother</summary>
        public const string RelatedPersonMother = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-RelatedPerson-mother-vr.html";

        /// <summary>URL for RelatedPersonParent</summary>
        public const string RelatedPersonParent = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-RelatedPerson-parent-vr.html";

        /// <summary>URL for StreetDesignator</summary>
        public const string StreetDesignator = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-StreetDesignator.html";

        /// <summary>URL for StreetName</summary>
        public const string StreetName = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-StreetName.html";

        /// <summary>URL for StreetNumber</summary>
        public const string StreetNumber = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-StreetNumber.html";

        /// <summary>URL for UnitOrAptNumber</summary>
        public const string UnitOrAptNumber = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-UnitOrAptNumber.html";

        /// <summary>URL for CodedRaceAndEthnicity</summary>
        public const string CodedRaceAndEthnicity = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-coded-race-and-ethnicity-vr.html";

        /// <summary>URL for InputRaceAndEthnicity</summary>
        public const string InputRaceAndEthnicity = "http://build.fhir.org/ig/HL7/vr-common-library/StructureDefinition-input-race-and-ethnicity-vr.html";

    }
}

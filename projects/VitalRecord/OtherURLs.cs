namespace VR
{
    /// <summary>Profile URLs for non-VRDR Profiles</summary>
    public static class OtherProfileURL
    {
        /// <summary>URL for USCorePractitioner</summary>
        public const string USCorePractitioner = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-practitioner";
    }

    /// <summary>Extension URLs for non-VRDR Profiles</summary>
    public static class OtherExtensionURL
    {
        /// <summary>URL for DataAbsentReason</summary>
        public const string DataAbsentReason = "http://hl7.org/fhir/StructureDefinition/data-absent-reason";

        /// <summary>URL for PatientBirthPlace</summary>
        public const string PatientBirthPlace = "http://hl7.org/fhir/StructureDefinition/patient-birthPlace";

        /// <summary>URL for RelatedPersonBirthPlace</summary>
        public const string RelatedPersonBirthPlace = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Extension-relatedperson-birthplace-vr";

        /// <summary>URL for US Core Birthsex </summary>
        public const string BirthSex = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-birthsex";

        /// <summary>URL for parent role </summary>
        public const string ParentRole = "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Extension-role-vr";
    }

    /// <summary>IG URLs for non-VRDR Profiles</summary>
    public static class OtherIGURL
    {
        /// <summary>URL for USCorePractitioner</summary>
        public const string USCorePractitioner = "http://hl7.org/fhir/us/core/STU4/StructureDefinition-us-core-practitioner.html";

        /// <summary>URL for UsualWork - TODO figure out why this one isn't generated from the IG</summary>
        public const string UsualWork = "https://build.fhir.org/ig/HL7/vr-common-library//StructureDefinition-Observation-usual-work-vr.html";
    }
}

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
    }

}

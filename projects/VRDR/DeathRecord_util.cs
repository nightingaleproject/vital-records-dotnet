// DeathRecord_util.cs
//    Contains utility methods used across the DeathRecords class.

using VR;

namespace VRDR
{
    /// <summary>Class <c>DeathRecord</c> models a FHIR Vital Records Death Reporting (VRDR) Death
    /// Record. This class was designed to help consume and produce death records that follow the
    /// HL7 FHIR Vital Records Death Reporting Implementation Guide, as described at:
    /// http://hl7.org/fhir/us/vrdr and https://github.com/hl7/vrdr.
    /// </summary>
    public partial class DeathRecord
    {
        /// <summary>Helper method to return a JSON string representation of this Death Record. Redundant due to VitalRecord.FromDescription[RecordType], but kept to maintain backwards compatibility.</summary>
        /// <returns>a new Death Record that corresponds to the given descriptive format</returns>
        public static DeathRecord FromDescription(string contents) {
            return FromDescription<DeathRecord>(contents);
        }
        
    }
}

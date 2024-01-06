using Hl7.Fhir.Model;

// DeathRecord_fieldsAndCreateMethods.cs
//     Contains field definitions and associated createXXXX methods used to construct a field

namespace VR
{
    /// <summary>Class <c>VitalRecord</c> models a FHIR Vital Record. 
    /// This class was designed to help consume and produce vital records that follow the
    /// HL7 FHIR Vital Records Implementation Guide, as described at:
    /// TODO include VR IG link
    /// </summary>
    public partial class VitalRecord
    {
        /// <summary>Bundle that contains the vital record.</summary>
        protected Bundle Bundle;

        /// <summary>Composition that described what the Bundle is, as well as keeping references to its contents.</summary>
        protected Composition Composition;

        /// <summary>CompositionSections that define the codes that represent the different sections in the composition, to be overwritten in the child class</summary>
        protected abstract string[] CompositionSections { get; }
    }
}
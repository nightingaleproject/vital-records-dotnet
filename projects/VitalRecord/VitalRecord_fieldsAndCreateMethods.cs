using System.Runtime.CompilerServices;
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
        private static ExtensionURL extensions = new ExtensionURL();
        /// <summary>Overridable instance of ExtensionURL that allow changing the URL prefix by subclasses</summary>
        public virtual ExtensionURL VRExtensionURLs => extensions;
        
        /// <summary>Bundle that contains the vital record.</summary>
        protected Bundle Bundle;

        /// <summary>Composition that described what the Bundle is, as well as keeping references to its contents.</summary>
        protected Composition Composition;

        /// <summary>CompositionSections that define the codes that represent the different sections in the composition, to be overwritten in the child class</summary>
        protected abstract string[] CompositionSections { get; }

        /// <summary>CompositionSectionCodeSystem defines the code system that is used to represent the different sections in the composition, may be overwritten in the child class
        /// 
        /// </summary>
        protected virtual string CompositionSectionCodeSystem { get => VR.CodeSystems.DocumentSections; }
    }
}
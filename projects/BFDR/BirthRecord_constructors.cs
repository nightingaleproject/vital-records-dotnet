using System;
using System.Linq;
using Hl7.Fhir.Model;
using VR;

namespace BFDR
{
    /// <summary>Class <c>BirthRecord</c> is a class designed to help consume and produce birth records
    /// that follow the HL7 FHIR Vital Records Birth Reporting Implementation Guide, as described at:
    /// http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// </summary>
    public partial class BirthRecord : NatalityRecord
    {
        /// <summary>The encounter of the birth.</summary>
        protected Encounter EncounterBirth;

        /// <summary>Default constructor that creates a new, empty BirthRecord.</summary>
        public BirthRecord() : base(ProfileURL.BundleDocumentBirthReport)
        {
            // Start with an empty EncounterBirth.
            CreateBirthEncounter();
            Composition.Encounter = new ResourceReference("urn:uuid:" + EncounterBirth.Id);
            Composition.Encounter.AddExtension(ExtensionURL.ExtensionEncounterMaternityReference, new ResourceReference("urn:uuid:" + EncounterMaternity.Id));
            Bundle.AddResourceEntry(EncounterBirth, "urn:uuid:" + EncounterBirth.Id);
        }

        /// <summary>Constructor that takes a string that represents a FHIR Birth Record in either XML or JSON format.</summary>
        /// <param name="record">represents a FHIR Birth Record in either XML or JSON format.</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <exception cref="ArgumentException">Record is neither valid XML nor JSON.</exception>
        public BirthRecord(string record, bool permissive = false) : base(record, new[] { ProfileURL.BundleDocumentBirthReport }, permissive) {}

        /// <summary>Constructor that takes a FHIR Bundle that represents a FHIR Birth Record.</summary>
        /// <param name="bundle">represents a FHIR Bundle.</param>
        /// <exception cref="ArgumentException">Record is invalid.</exception>
        public BirthRecord(Bundle bundle) : base(bundle) {}

        /// <summary>Return the birth year for this record to be used in the identifier</summary>
        public override uint? GetYear()
        {
            return (uint?)this.BirthYear;
        }
        
        /// <inheritdoc/>
        protected override void RestoreReferences()
        {
            // Restore the common references between Birth Records and Fetal Death Records.
            base.RestoreReferences(ProfileURL.BundleDocumentBirthReport, new[] {ProfileURL.CompositionProviderLiveBirthReport, ProfileURL.CompositionJurisdictionLiveBirthReport}, VR.ProfileURL.Child);
            // Restore BirthRecord specific references.
            string birthEncounterId = Composition?.Encounter?.Reference;
            EncounterBirth = Bundle.Entry.FindAll(entry => entry.Resource is Encounter).ConvertAll(entry => (Encounter) entry.Resource).Find(resource => resource.Meta.Profile.Any(p => p == ProfileURL.EncounterBirth) && birthEncounterId.Contains(resource.Id));
            string encounterMaternityId = ((ResourceReference) Composition?.Encounter?.Extension.FirstOrDefault(e => e.Url == ExtensionURL.ExtensionEncounterMaternityReference)?.Value)?.Reference;
            EncounterMaternity = Bundle.Entry.FindAll(entry => entry.Resource is Encounter).ConvertAll(entry => (Encounter) entry.Resource).Find(resource => resource.Meta.Profile.Any(p => p == ProfileURL.EncounterMaternity) && encounterMaternityId.Contains(resource.Id));
        }

        /// <inheritdoc/>
        protected override void InitializeCompositionAndSubject()
        {
            // Initialize empty BirthRecord specific Subject and Composition.
            Subject = new Patient
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta() {
                    Profile = new[] { VR.ProfileURL.Child }
                }
            };
            Composition.Meta = new Meta
            {
                Profile = new[] { ProfileURL.CompositionJurisdictionLiveBirthReport }
            };
            Composition.Type = new CodeableConcept(CodeSystems.LOINC, "71230-7", "Birth certificate", null);
            Composition.Title = "Birth Certificate";
        }

        /// <summary>Create Birth Encounter.</summary>
        protected Encounter CreateBirthEncounter()
        {
            EncounterBirth = CreateEncounter(ProfileURL.EncounterBirth);
            Extension roleExt = new Extension(BFDR.ExtensionURL.ExtensionRole, new CodeableConcept(CodeSystems.RoleCode_HL7_V3, "CHILD"));
            EncounterBirth.Extension.Add(roleExt);
            return EncounterBirth;
        }
    }
}

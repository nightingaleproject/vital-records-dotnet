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
        public BirthRecord(string record, bool permissive = false) : base(record, permissive) {}

        /// <summary>Constructor that takes a FHIR Bundle that represents a FHIR Birth Record.</summary>
        /// <param name="bundle">represents a FHIR Bundle.</param>
        /// <exception cref="ArgumentException">Record is invalid.</exception>
        public BirthRecord(Bundle bundle) : base(bundle) {}

        /// <summary>Return the birth year for this record to be used in the identifier</summary>
        public override uint? GetYear()
        {
            VitalRecord.ParseDateElements(this.DateOfBirth, out int? year, out _, out _);
            if (year != null)
            {
                return (uint?) year;
            }
            // For cases where we loaded a bundle that has the RecordIdentifier but not the individual fields
            // we can find the value as a substring of the RecordIdentifier
            if (RecordIdentifier != null && RecordIdentifier.Length == 12 && RecordIdentifier.Substring(0, 4) != "0000")
            {
                int parsedYear;
                if (Int32.TryParse(RecordIdentifier.Substring(0, 4), out parsedYear))
                {
                    return (uint?) parsedYear;
                }
            }
            return null;
        }
        
        /// <inheritdoc/>
        protected override void RestoreReferences()
        {
            // Restore the common references between Birth Records and Fetal Death Records.
            base.RestoreReferences();
            // Restore BirthRecord specific references.
            string birthEncounterId = Composition?.Encounter?.Reference;
            EncounterBirth = (Encounter)Bundle.Entry.Find(entry => entry.Resource is Encounter && birthEncounterId.Contains(entry.Resource.Id))?.Resource;
            string maternityEncounterId = ((ResourceReference) Composition?.Encounter?.Extension.FirstOrDefault(e => e.Url == ExtensionURL.ExtensionEncounterMaternityReference)?.Value)?.Reference;
            EncounterMaternity = (Encounter)Bundle.Entry.Find(entry => entry.Resource is Encounter && maternityEncounterId.Contains(entry.Resource.Id))?.Resource;
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
            Composition.Type = new CodeableConcept(CodeSystems.LOINC, "92011-6", "Jurisdiction live birth report Document", null);
            Composition.Title = "Birth Certificate";
        }

        /// <summary>
        /// Initialize sections creates empty sections based on the composition type
        /// These are necessary so resources like Mother and Father can be referenced from somewhere in the composition
        /// </summary>
        protected override void InitializeSections()
        {
            CreateNewSection(MOTHER_PRENATAL_SECTION);
            CreateNewSection(MEDICAL_INFORMATION_SECTION);
            CreateNewSection(NEWBORN_INFORMATION_SECTION);
            CreateNewSection(MOTHER_INFORMATION_SECTION);
            CreateNewSection(FATHER_INFORMATION_SECTION);
            CreateNewSection(EMERGING_ISSUES_SECTION);   
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

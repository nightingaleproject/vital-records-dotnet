using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using VR;

namespace BFDR
{
    /// <summary>Class <c>FetalDeathRecord</c> is a class designed to help consume and produce fetal death
    /// records that follow the HL7 FHIR Vital Records FetalDeath Reporting Implementation Guide, as described
    /// at: http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// </summary>
    public partial class FetalDeathRecord : NatalityRecord
    {
        /// <summary>Default constructor that creates a new, empty FetalDeathRecord.</summary>
        public FetalDeathRecord() : base() {}

        /// <summary>Constructor that takes a string that represents a FHIR FetalDeath Record in either XML or JSON format.</summary>
        /// <param name="record">represents a FHIR FetalDeath Record in either XML or JSON format.</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <exception cref="ArgumentException">Record is neither valid XML nor JSON.</exception>
        public FetalDeathRecord(string record, bool permissive = false) : base(record, permissive) {}

        /// <summary>Constructor that takes a FHIR Bundle that represents a FHIR FetalDeath Record.</summary>
        /// <param name="bundle">represents a FHIR Bundle.</param>
        /// <exception cref="ArgumentException">Record is invalid.</exception>
        public FetalDeathRecord(Bundle bundle) : base(bundle) {}

        /// <summary>Return the birth year for this record to be used in the identifier</summary>
        protected override uint? GetYear()
        {
            // TODO: Uncomment and remove null return when DeliveryYear is implemented
            // return (uint?)this.DeliveryYear;
            return null;
        }

        /// <inheritdoc/>
        protected override void RestoreReferences()
        {
            // Restore FetalDeath specific references.
            List<Patient> patients = Bundle.Entry.FindAll(entry => entry.Resource is Patient).ConvertAll(entry => (Patient) entry.Resource);
            Subject = patients.Find(patient => patient.Meta.Profile.Any(patientProfile => patientProfile == BFDR.ProfileURL.PatientDecedentFetus));
            // Restore the common references between Birth Records and Fetal Death Records.
            base.RestoreReferences();
        }

        /// <inheritdoc/>
        protected override void InitializeCompositionAndSubject()
        {
            // Initialize empty FetalDeathRecord specific objects.
            // Start with an empty decedent fetus. Need reference in Composition.
            Subject = new Patient
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta() {
                    Profile = new[] { BFDR.ProfileURL.PatientDecedentFetus }
                }
            };
            Composition.Meta = new Meta
            {
                Profile = new[] { ProfileURL.CompositionJurisdictionFetalDeathReport }
            };
            Composition.Type = new CodeableConcept(CodeSystems.LOINC, "71230-7", "Fetal Death Report", null);
            Composition.Title = "Fetal Death Report";
        }

        /// <summary>The place of delivery Type.</summary>
        /// <value>Place Where delivery Occurred, type of place or institution. A Dictionary representing a codeable concept of the physical location type:
        /// <para>"code" - The code used to describe this concept.</para>
        /// <para>"system" - The relevant code system.</para>
        /// <para>"display" - The human readable version of this code.</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; locationType = new Dictionary&lt;string, string&gt;();</para>
        /// <para>locationType.Add("code", "22232009");</para>
        /// <para>locationType.Add("system", "http://snomed.info/sct");</para>
        /// <para>locationType.Add("display", "Hospital");</para>
        /// <para>ExampleBirthRecord.BirthPhysicalLocation = locationType;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"The place type the child was born: {ExampleBirthRecord.BirthPhysicalLocation["code"]}");</para>
        /// </example>
        [Property("DeliveryPhysicalLocation", Property.Types.Dictionary, "Delivery Physical Location", "Delivery Physical Location.", true, IGURL.EncounterMaternity, true, 16)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter)", "")]
        public Dictionary<string, string> DeliveryPhysicalLocation
        {
            get => GetPhysicalLocation(EncounterMaternity);
            set => SetPhysicalLocation(EncounterMaternity ?? CreateEncounter(ProfileURL.EncounterMaternity), value);
        }

        /// <summary>Child's Place Of Birth Type Helper</summary>
        /// <value>Child's Place Of Birth Type Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthPhysicalLocationHelper = "Hospital";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child's Place Of Birth Type: {ExampleBirthRecord.BirthPhysicalLocationHelper}");</para>
        /// </example>
        [Property("DeliveryPhysicalLocationHelper", Property.Types.String, "Delivery Physical Location", "Delivery Physical Location Helper.", false, IGURL.EncounterMaternity, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(meta.profile == " + IGURL.EncounterMaternity + ")", "")]
        public string DeliveryPhysicalLocationHelper
        {
            get => GetPhysicalLocationHelper(EncounterMaternity);
            set => SetPhysicalLocationHelper(EncounterMaternity ?? CreateEncounter(ProfileURL.EncounterMaternity), value, BFDR.Mappings.BirthDeliveryOccurred.FHIRToIJE, BFDR.ValueSets.BirthDeliveryOccurred.Codes);
        }
    }
}

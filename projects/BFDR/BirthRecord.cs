using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using VR;
using static Hl7.Fhir.Model.Encounter;

namespace BFDR
{
    /// <summary>Class <c>BirthRecord</c> is a class designed to help consume and produce birth records
    /// that follow the HL7 FHIR Vital Records Birth Reporting Implementation Guide, as described at:
    /// http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// </summary>
    public partial class BirthRecord : NatalityRecord
    {

        /// <summary>Default constructor that creates a new, empty BirthRecord.</summary>
        public BirthRecord() : base()
        {

            // Start with an empty child. Need reference in Composition.

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
        protected override uint? GetYear()
        {
            return (uint?)this.BirthYear;
        }
        
        /// <inheritdoc/>
        protected override void RestoreReferences()
        {
            // Restore BirthRecord specific references.
            List<Patient> patients = Bundle.Entry.FindAll(entry => entry.Resource is Patient).ConvertAll(entry => (Patient) entry.Resource);
            Subject = patients.Find(patient => patient.Meta.Profile.Any(patientProfile => patientProfile == VR.ProfileURL.Child));
            // Restore the common references between Birth Records and Fetal Death Records.
            base.RestoreReferences();
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

        /// <summary>Child's Place Of Birth Type.</summary>
        /// <value>Place Where Birth Occurred, type of place or institution. A Dictionary representing a codeable concept of the physical location type:
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
        [Property("BirthPhysicalLocation", Property.Types.Dictionary, "Birth Physical Location", "Birth Physical Location.", true, IGURL.EncounterBirth, true, 16)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter)", "")]
        public Dictionary<string, string> BirthPhysicalLocation
        {
            get => GetPhysicalLocation(EncounterBirth);
            set => SetPhysicalLocation(EncounterBirth ?? CreateEncounter(ProfileURL.EncounterBirth), value);
        }

        /// <summary>Child's Place Of Birth Type Helper</summary>
        /// <value>Child's Place Of Birth Type Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthPhysicalLocationHelper = "Hospital";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child's Place Of Birth Type: {ExampleBirthRecord.BirthPhysicalLocationHelper}");</para>
        /// </example>
        [Property("BirthPhysicalLocationHelper", Property.Types.String, "Birth Physical Location", "Birth Physical Location Helper.", false, IGURL.EncounterBirth, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(meta.profile == " + IGURL.EncounterBirth + ")", "")]
        public string BirthPhysicalLocationHelper
        {
            get => GetPhysicalLocationHelper(EncounterBirth);
            set => SetPhysicalLocationHelper(EncounterBirth ?? CreateEncounter(ProfileURL.EncounterBirth), value, BFDR.Mappings.BirthDeliveryOccurred.FHIRToIJE, BFDR.ValueSets.BirthDeliveryOccurred.Codes);
        }
    }
}

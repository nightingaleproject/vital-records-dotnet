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

        private const string FETUS_SECTION = "76400-1";

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

        /// <summary>Estimated time of fetal death.</summary>
        /// <value>Estimated time of fetal death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.TimeOfFetalDeath = ;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Sex at Time of Birth: {ExampleBirthRecord.TimeOfFetalDeath}");</para>
        /// </example>
        [Property("Estimated time of fetal death", Property.Types.Dictionary, "Fetal Death", "Estimated time of fetal death.", true, VR.IGURL.PatientFetalDeath, true, 12)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url='" + OtherExtensionURL.BirthSex + "')", "")]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public Dictionary<string, string> TimeOfFetalDeath
        {
            get
            {
                Observation obs = GetObservation("73811-2");
                if (obs != null && obs.Value != null && (obs.Value as CodeableConcept) != null)
                {
                    return CodeableConceptToDict((CodeableConcept)obs.Value);
                }
                return EmptyCodeableDict();  
            }
            set
            {
                Observation obs = GetOrCreateObservation("73811-2", CodeSystems.SCT, "Estimated Time Fetal Death", BFDR.ProfileURL.ObservationFetalDeathTimePoint, FETUS_SECTION);
                obs.Value = DictToCodeableConcept(value);
            }
        }

        /// <summary>Child's Sex at Birth Helper.</summary>
        /// <value>The child's sex at time of birth</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.TimeOfFetalDeathHelper = "female";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Time of Fetal Death: {ExampleBirthRecord.TimeOfFetalDeathHelper}");</para>
        /// </example>
        [Property("Estimated Time of Fetal Death Helper", Property.Types.String, "Fetal Death", "Estimated time of fetal death.", false, VR.IGURL.Child, true, 12)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url='" + OtherExtensionURL.BirthSex + "')", "")]
        public string TimeOfFetalDeathHelper
        {
            get
            {
                if (TimeOfFetalDeath.ContainsKey("code") && !String.IsNullOrWhiteSpace(TimeOfFetalDeath["code"]))
                {
                    return TimeOfFetalDeath["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("TimeOfFetalDeath", value, BFDR.ValueSets.FetalDeathTimePoints.Codes);
                }
            }
        }
    }
}

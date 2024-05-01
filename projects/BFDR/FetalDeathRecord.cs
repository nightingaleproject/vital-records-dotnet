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

        //
        // Fetal Death Cause or Condition Section
        //

        /// <summary>Initiating cause/condition, Rupture of membranes prior to onset of labor.</summary>
        [Property("Rupture of Membranes Prior to Onset of Labor", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Rupture of Membranes Prior to Onset of Labor", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "44223004", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool PrematureRuptureOfMembranes
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Abruptio placenta.</summary>
        [Property("Abruptio Placenta", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Abruptio Placenta", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "415105001", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool AbruptioPlacenta
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Placental insufficiency.</summary>
        [Property("Placental Insufficiency", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Placental Insufficiency", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "237292005", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool PlacentalInsufficiency
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Prolapsed cord.</summary>
        [Property("Prolapsed Cord", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Prolapsed Cord", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "270500004", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool ProlapsedCord
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Chorioamnionitis.</summary>
        [Property("Chorioamnionitis", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Chorioamnionitis", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "11612004", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool ChorioamnionitisCOD
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Other complications of placenta, cord, or membranes.</summary>
        [Property("Other Complications of Placenta, Cord, or Membranes", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Other Complications of Placenta, Cord, or Membranes", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "membranes", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool OtherComplicationsOfPlacentaCordOrMembranes
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Unknown.</summary>
        [Property("Unknown Initiating Cause or Condition", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Unknown", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "UNK", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool InitiatingCauseOrConditionUnknown
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Maternal conditions/diseases literal.</summary>
        [Property("Maternal Conditions Diseases Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Maternal Conditions Diseases Literal", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "maternalconditions", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string MaternalConditionsDiseasesLiteral
        {
            get
            {
                Condition cond = GetCondition("maternalconditions");
                if (cond != null && cond.Code != null && cond.Code.Text != null)
                {
                    return cond.Code.Text.ToString();
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Condition cond = GetCondition("maternalconditions");
                if (cond == null){
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Other complications of placenta, cord, or membranes literal.</summary>
        [Property("Other Complications of Placenta Cord Membranes Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Other Complications of Placenta Cord Membranes Literal", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "membranes", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherComplicationsOfPlacentaCordMembranesLiteral
        {
            get
            {
                Condition cond = GetCondition("membranes");
                if (cond != null && cond.Code != null && cond.Code.Text != null)
                {
                    return cond.Code.Text.ToString();
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Condition cond = GetCondition("membranes");
                if (cond == null){
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Other obstetrical or pregnancy complications literal.</summary>
        [Property("Other Obstetrical or Pregnancy Complications Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Other Obstetrical or Pregnancy Complications Literal", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "obstetricalcomplications", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherObstetricalOrPregnancyComplicationsLiteral
        {
            get
            {
                Condition cond = GetCondition("obstetricalcomplications");
                if (cond != null && cond.Code != null && cond.Code.Text != null)
                {
                    return cond.Code.Text.ToString();
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Condition cond = GetCondition("obstetricalcomplications");
                if (cond == null){
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Fetal anomaly literal.</summary>
        [Property("Fetal Anomaly Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Fetal Anomaly Literal", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "702709008", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string FetalAnomalyLiteral
        {
            get
            {
                Condition cond = GetCondition("702709008");
                if (cond != null && cond.Code != null && cond.Code.Text != null)
                {
                    return cond.Code.Text.ToString();
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Condition cond = GetCondition("702709008");
                if (cond == null){
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Fetal injury literal.</summary>
        [Property("Fetal Injury Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Fetal Injury Literal", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "277489001", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string FetalInjuryLiteral
        {
            get
            {
                Condition cond = GetCondition("277489001");
                if (cond != null && cond.Code != null && cond.Code.Text != null)
                {
                    return cond.Code.Text.ToString();
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Condition cond = GetCondition("277489001");
                if (cond == null){
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Fetal infection literal.</summary>
        [Property("Fetal Infection Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Fetal Infection Literal", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "128270001", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string FetalInfectionLiteral
        {
            get
            {
                Condition cond = GetCondition("128270001");
                if (cond != null && cond.Code != null && cond.Code.Text != null)
                {
                    return cond.Code.Text.ToString();
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Condition cond = GetCondition("128270001");
                if (cond == null){
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Other fetal conditions/disorders literal.</summary>
        [Property("Other Fetal Conditions/Disorders Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Other Fetal Conditions/Disorders Literal", true, IGURL.ConditionFetalDeathCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "fetalconditions", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherFetalConditionsDisordersLiteral
        {
            get
            {
                Condition cond = GetCondition("fetalconditions");
                if (cond != null && cond.Code != null && cond.Code.Text != null)
                {
                    return cond.Code.Text.ToString();
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Condition cond = GetCondition("fetalconditions");
                if (cond == null){
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }
    }
}
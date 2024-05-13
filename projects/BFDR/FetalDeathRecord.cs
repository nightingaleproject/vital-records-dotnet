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

        /// <summary>Were Autopsy or Histological Placental Examination Results Used in Determining the Cause of Fetal Death?</summary>
        /// <value>Were Autopsy or Histological Placental Examination Results Used in Determining the Cause of Fetal Death?</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; autopf = new Dictionary&lt;string, string&gt;();</para>
        /// <para>autopf.Add("code", "Y");</para>
        /// <para>autopf.Add("system", VR.ValueSets.YesNoNotApplicable.Codes);</para>
        /// <para>autopf.Add("display", "Yes");</para>
        /// <para>ExampleFetalDeathRecord.AutopsyorHistologicalExamResultsUsed = autopf;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Were autopsy results used: {ExampleFetalDeathRecord.AutopsyorHistologicalExamResultsUsed}");</para>
        /// </example>
        [Property("AutopsyorHistologicalExamResultsUsed", Property.Types.Dictionary, "Fetus", "AutopsyorHistologicalExamResultsUsed", false, BFDR.IGURL.ObservationAutopsyHistologicalExamResultsUsed, true, 150)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='74498-7')", "")]
        public Dictionary<string, string> AutopsyorHistologicalExamResultsUsed
        {
            get => GetObservationValue("74498-7");
            set => SetObservationValue(value, "74498-7", CodeSystems.LOINC, "Were autopsy or histological placental examinations results used in determining the cause of fetal death?", BFDR.ProfileURL.ObservationAutopsyHistologicalExamResultsUsed, FETUS_SECTION);
        }

        /// <summary>Autopsy or Histological Placental Results Used Helper</summary>
        /// <value>PAutopsy or Histological Placental Results Used Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.AutopsyorHistologicalExamResultsUsedHelper = "yes";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Were autopsy results used: {ExampleFetalDeathRecord.AutopsyorHistologicalExamResultsUsedHelper}");</para>
        /// </example>
        [Property("AutopsyorHistologicalExamResultsUsedHelper", Property.Types.String, "Fetus", "AutopsyorHistologicalExamResultsUsedHelper", false, BFDR.IGURL.ObservationAutopsyHistologicalExamResultsUsed, true, 150)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='74498-7')", "")]
        public string AutopsyorHistologicalExamResultsUsedHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, VR.ValueSets.YesNoNotApplicable.Codes);
        }
        

        /// <summary>Autopsy Performed Indicator.</summary>
        /// <value>autopsy performed indicator. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; code = new Dictionary&lt;string, string&gt;();</para>
        /// <para>code.Add("code", "Y");</para>
        /// <para>code.Add("system", CodeSystems.PH_YesNo_HL7_2x);</para>
        /// <para>code.Add("display", "Yes");</para>
        /// <para>ExampleFetalDeathRecord.AutopsyPerformedIndicator = code;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Autopsy Performed Indicator: {ExampleFetalDeathRecord.AutopsyPerformedIndicator['display']}");</para>
        /// </example>
        [Property("Autopsy Performed Indicator", Property.Types.Dictionary, "Fetus", "Autopsy Performed Indicator.", true, VR.IGURL.AutopsyPerformedIndicator, true, 148)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='85699-7')", "")]
        public Dictionary<string, string> AutopsyPerformedIndicator
        {
            get => GetObservationValue("85699-7");
            set => SetObservationValue(value, "85699-7", CodeSystems.LOINC, "Autopsy Performed Indicator", VR.ProfileURL.AutopsyPerformedIndicator, FETUS_SECTION);
        }

        /// <summary>Autopsy Performed Indicator Helper. This is a helper method, to access the code use the AutopsyPerformedIndicator property.</summary>
        /// <value>autopsy performed indicator. A null value indicates "not applicable".</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.AutopsyPerformedIndicatorHelper = "Y"";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Autopsy Performed Indicator: {ExampleDFetaleathRecord.AutopsyPerformedIndicatorBoolean}");</para>
        /// </example>
        [Property("Autopsy Performed Indicator Helper", Property.Types.String, "Fetus", "Autopsy Performed Indicator.", false, VR.IGURL.AutopsyPerformedIndicator, true, 148)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='85699-7')", "")]
        public string AutopsyPerformedIndicatorHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, VR.ValueSets.YesNoUnknown.Codes);
        }

        /// <summary>Birth Weight.</summary>
        /// <value>The weight of the infant/fetus at birth/delivery.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.BirthWeight = 3000.0;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Birth Weight: {xampleFetalDeathRecord.BirthWeight}");</para>
        /// </example>
        [Property("BirthWeight", Property.Types.Int32, "Fetus", "Birth Weight.", false, BFDR.IGURL.ObservationBirthWeight, true, 143)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8339-4')", "")]
        public override int? BirthWeight
        {
          get => GetWeight("8339-4");
          set => SetWeight("8339-4", value, "g", FETUS_SECTION, Subject.Id);
        }

        /// <summary>Birth Weight Edit Flag.</summary>
        /// <value>the fetus's birth weight edit flag. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; weight = new Dictionary&lt;string, string&gt;();</para>
        /// <para>weight.Add("code", "0");</para>
        /// <para>weight.Add("system", CodeSystems.BypassEditFlag);</para>
        /// <para>weight.Add("display", "Edit Passed");</para>
        /// <para>ExampleFetalDeathRecord.BirthWeightEditFlag = height;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Birth Weight: {ExampleFetalDeathRecord.BirthWeightEditFlag['display']}");</para>
        /// </example>
        [Property("Birth Weight Edit Flag", Property.Types.Dictionary, "Fetus", "Birth Weight Edit Flag.", true, IGURL.ObservationBirthWeight, true, 144)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8302-2')", "")]
        public override Dictionary<string, string> BirthWeightEditFlag
        {
            get => GetWeightEditFlag("8339-4");
            set => SetWeightEditFlag("8339-4", value, FETUS_SECTION, Subject.Id);

        }

        /// <summary>Birth Weight Edit Flag Helper</summary>
        /// <value>Birth Weight Edit Flag.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.BirthWeightEditFlagHelper = "0";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Birth Weight Edit Flag: {ExampleFetalDeathRecord.BirthWeightHelperEditFlag}");</para>
        /// </example>
        [Property("Birth Weight Edit Flag Helper", Property.Types.String, "Fetus", "Birth Weight Edit Flag Helper.", false, IGURL.ObservationBirthWeight, true, 144)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8339-4')", "")]
        public override string BirthWeightEditFlagHelper
        {
            get => GetWeightEditFlagHelper("8339-4");
            set => SetWeightEditFlagHelper("8339-4", value, FETUS_SECTION, Subject.Id);
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

        //
        // Other Fetal Death Cause or Condition Section
        //

        /// <summary>Other significant causes or conditions, Rupture of membranes prior to onset of labor.</summary>
        [Property("Rupture of Membranes Prior to Onset of Labor", Property.Types.Bool, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Rupture of Membranes Prior to Onset of Labor", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "44223004", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool OtherCOD_PrematureRuptureOfMembranes
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Other significant causes or conditions, Abruptio placenta.</summary>
        [Property("Abruptio Placenta", Property.Types.Bool, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Abruptio Placenta", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "415105001", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool OtherCOD_AbruptioPlacenta
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Other significant causes or conditions, Placental insufficiency.</summary>
        [Property("Placental Insufficiency", Property.Types.Bool, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Placental Insufficiency", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "237292005", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool OtherCOD_PlacentalInsufficiency
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Other significant causes or conditions, Prolapsed cord.</summary>
        [Property("Prolapsed Cord", Property.Types.Bool, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Prolapsed Cord", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "270500004", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool OtherCOD_ProlapsedCord
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Other significant causes or conditions, Chorioamnionitis.</summary>
        [Property("Chorioamnionitis", Property.Types.Bool, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Chorioamnionitis", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "11612004", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool OtherCOD_ChorioamnionitisCOD
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Other significant causes or conditions, Other complications of placenta, cord, or membranes.</summary>
        [Property("Other Complications of Placenta, Cord, or Membranes", Property.Types.Bool, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Other Complications of Placenta, Cord, or Membranes", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "membranes", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool OtherCOD_OtherComplicationsOfPlacentaCordOrMembranes
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Other significant causes or conditions, Unknown.</summary>
        [Property("Unknown Other Cause or Condition", Property.Types.Bool, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Unknown", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "UNK", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool OtherCOD_OtherCauseOrConditionUnknown
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Other significant causes or conditions, Maternal conditions/diseases literal.</summary>
        [Property("Maternal Conditions Diseases Literal", Property.Types.String, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Maternal Conditions Diseases Literal", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "maternalconditions", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherCOD_MaternalConditionsDiseasesLiteral
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

        /// <summary>Other significant causes or conditions, Other complications of placenta, cord, or membranes literal.</summary>
        [Property("Other Complications of Placenta Cord Membranes Literal", Property.Types.String, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Other Complications of Placenta Cord Membranes Literal", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "membranes", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherCOD_OtherComplicationsOfPlacentaCordMembranesLiteral
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

        /// <summary>Other significant causes or conditions, Other obstetrical or pregnancy complications literal.</summary>
        [Property("Other Obstetrical or Pregnancy Complications Literal", Property.Types.String, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Other Obstetrical or Pregnancy Complications Literal", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "obstetricalcomplications", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherCOD_OtherObstetricalOrPregnancyComplicationsLiteral
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

        /// <summary>Other significant causes or conditions, Fetal anomaly literal.</summary>
        [Property("Fetal Anomaly Literal", Property.Types.String, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Fetal Anomaly Literal", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "702709008", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherCOD_FetalAnomalyLiteral
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

        /// <summary>Other significant causes or conditions, Fetal injury literal.</summary>
        [Property("Fetal Injury Literal", Property.Types.String, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Fetal Injury Literal", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "277489001", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherCOD_FetalInjuryLiteral
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

        /// <summary>Other significant causes or conditions, Fetal infection literal.</summary>
        [Property("Fetal Infection Literal", Property.Types.String, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Fetal Infection Literal", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "128270001", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherCOD_FetalInfectionLiteral
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

        /// <summary>Other significant causes or conditions, Other fetal conditions/disorders literal.</summary>
        [Property("Other Fetal Conditions/Disorders Literal", Property.Types.String, "Other Significant Cause/Condition",
                  "Other Significant Cause/Condition, Other Fetal Conditions/Disorders Literal", true, IGURL.ConditionFetalDeathOtherCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76061-1", code: "fetalconditions", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherCOD_OtherFetalConditionsDisordersLiteral
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

        /// <summary>Place of Delivery.</summary>
        /// <value>Place of Delivery. A Dictionary representing place of delivery address, containing the following key/value pairs:
        /// <para>"addressLine1" - address, line one</para>
        /// <para>"addressLine2" - address, line two</para>
        /// <para>"addressCity" - address, city</para>
        /// <para>"addressCounty" - address, county</para>
        /// <para>"addressState" - address, state</para>
        /// <para>"addressZip" - address, zip</para>
        /// <para>"addressCountry" - address, country</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; address = new Dictionary&lt;string, string&gt;();</para>
        /// <para>address.Add("addressLine1", "123 Test Street");</para>
        /// <para>address.Add("addressLine2", "Unit 3");</para>
        /// <para>address.Add("addressCity", "Boston");</para>
        /// <para>address.Add("addressCounty", "Suffolk");</para>
        /// <para>address.Add("addressState", "MA");</para>
        /// <para>address.Add("addressZip", "12345");</para>
        /// <para>address.Add("addressCountry", "US");</para>
        /// <para>ExampleBirthRecord.PlaceOfDelivery = address;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"State where baby was delivered: {ExampleBirthRecord.PlaceOfDelivery["addressState"]}");</para>
        /// </example>
        [Property("Place of Delivery", Property.Types.Dictionary, "Place of Delivery", "Place of Delivery.", true, IGURL.LocationBFDR, true, 20)]
        [PropertyParam("addressLine1", "address, line one")]
        [PropertyParam("addressLine2", "address, line two")]
        [PropertyParam("addressCity", "address, city")]
        [PropertyParam("addressCounty", "address, county")]
        [PropertyParam("addressState", "address, state")]
        [PropertyParam("addressZip", "address, zip")]
        [PropertyParam("addressCountry", "address, country")]
        [FHIRPath("Bundle.entry.resource.where($this is Location)", "address")]
        public Dictionary<string, string> PlaceOfDelivery
        {
            get
            {
                Location LocationDelivery = GetFacilityLocation(ValueSets.LocationTypes.Birth_Location);
                return AddressToDict(LocationDelivery?.Address);
            }
            set
            {
                Address d = DictToAddress(value);
                (GetFacilityLocation(ValueSets.LocationTypes.Birth_Location) ?? CreateAndSetLocationBirth(ValueSets.LocationTypes.Birth_Location)).Address = d;
            }
        }
    }
}
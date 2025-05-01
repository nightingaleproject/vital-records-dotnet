using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>Date of Certification.</summary>
        /// <value>the date of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.CertifiedDate = "2023-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of Fetal Death certification: {ExampleFetalDeathRecord.CertificationDate}");</para>
        /// </example>
        [Property("CertificationDate", Property.Types.String, "Fetal Death Certification", "Date of Certification.", true, BFDR.IGURL.CompositionProviderFetalDeathReport, true, 243)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")]
        public string CertificationDate
        {
            get => GetCertificationDate(EncounterMaternity);
            set => SetCertificationDate(EncounterMaternity, value);
        }

        /// <summary>Certified Year</summary>
        /// <value>year of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.CertifiedYear = 2023;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certified Year: {ExampleFetalDeathRecord.CertifiedYear}");</para>
        /// </example>
        [Property("Certified Year", Property.Types.Int32, "Fetal Death Certification", "Certified Year", true, IGURL.EncounterMaternity, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")]
        public int? CertifiedYear
        {
            get => GetCertifiedDateElement(EncounterMaternity, VR.ExtensionURL.PartialDateYearVR);
            set => SetCertifiedDateElement(EncounterMaternity ?? CreateMaternityEncounter(), VR.ExtensionURL.PartialDateYearVR, value);
        }

        /// <summary>Certified Month</summary>
        /// <value>month of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.CertifiedMonth = 10;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certified Month: {ExampleFetalDeathRecord.CertifiedMonth}");</para>
        /// </example>
        [Property("Certified Month", Property.Types.Int32, "Fetal Death Certification", "Certified Month", true, IGURL.EncounterMaternity, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")]
        public int? CertifiedMonth
        {
            get => GetCertifiedDateElement(EncounterMaternity, VR.ExtensionURL.PartialDateMonthVR);
            set => SetCertifiedDateElement(EncounterMaternity ?? CreateMaternityEncounter(), VR.ExtensionURL.PartialDateMonthVR, value);
        }

        /// <summary>Certified Day</summary>
        /// <value>day of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.CertifiedDay = 23;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certified Day: {ExampleFetalDeathRecord.CertifiedDay}");</para>
        /// </example>
        [Property("Certified Day", Property.Types.Int32, "Fetal Death Certification", "Certified Day", true, IGURL.EncounterMaternity, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")]
        public int? CertifiedDay
        {
            get => GetCertifiedDateElement(EncounterMaternity, VR.ExtensionURL.PartialDateDayVR);
            set => SetCertifiedDateElement(EncounterMaternity ?? CreateMaternityEncounter(), VR.ExtensionURL.PartialDateDayVR, value);
        }

        /// <summary>Fetus Legal Name - Given. Middle name should be the last entry.</summary>
        /// <value>the fetus' name (first, etc., middle)</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>string[] names = { "Example", "Something", "Middle" };</para>
        /// <para>ExampleFetalDeathRecord.FetusGivenNames = names;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Fetus Given Name(s): {string.Join(", ", ExampleFetalDeath.FetusGivenNames)}");</para>
        /// </example>
        [Property("FetusGivenNames", Property.Types.StringArr, "Fetus Name", "Fetus' First Name.", true, BFDR.IGURL.PatientDecedentFetus, true, 0)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string[] FetusGivenNames
        {
            get => Subject?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Given?.ToArray() ?? new string[0];
            set => updateGivenHumanName(value, Subject.Name);
        }

        /// <summary>Fetus' Legal Name - Last.</summary>
        /// <value>the fetus' last name</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>string lastName = "Quinn";</para>
        /// <para>ExampleFetalDeathRecord.FetusFamilyName = lastName;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Fetus Family Name(s): {string.Join(", ", ExampleFetalDeathRecord.FetusFamilyName)}");</para>
        /// </example>
        [Property("FetusFamilyName", Property.Types.String, "Fetus Name", "Fetus' Last Name.", true, BFDR.IGURL.PatientDecedentFetus, true, 0)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string FetusFamilyName
        {
            get => Subject?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Family;
            set => updateFamilyName(value, Subject.Name);
        }

        /// <summary>Fetus' Suffix.</summary>
        /// <value>the fetus' suffix</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.FetusSuffix = "Jr.";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Fetus Suffix: {ExampleFetusDeathRecord.FetusSuffix}");</para>
        /// </example>
        [Property("FetusSuffix", Property.Types.String, "Fetus Name", "Fetus' Suffix.", true, BFDR.IGURL.PatientDecedentFetus, true, 6)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string FetusSuffix
        {
            get => Subject?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Suffix.FirstOrDefault();
            set => updateSuffix(value, Subject.Name);
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
        /// <para>ExampleFetalDeathRecord.DeliveryPhysicalLocation = locationType;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"The place type the child was born: {ExampleFetalDeathRecord.DeliveryPhysicalLocation["code"]}");</para>
        /// </example>
        [Property("DeliveryPhysicalLocation", Property.Types.Dictionary, "Delivery Physical Location", "Delivery Physical Location.", true, IGURL.EncounterMaternity, true, 16)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='MTH')", "")]
        public Dictionary<string, string> DeliveryPhysicalLocation
        {
            get => GetPhysicalLocation(EncounterMaternity);
            set => SetPhysicalLocation(EncounterMaternity ?? CreateMaternityEncounter(), value);
        }

        /// <summary>Child's Place Of Birth Type Helper</summary>
        /// <value>Child's Place Of Birth Type Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.DeliveryPhysicalLocationHelper = "Hospital";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child's Place Of Birth Type: {ExampleFetalDeathRecord.DeliveryPhysicalLocationHelper}");</para>
        /// </example>
        [Property("DeliveryPhysicalLocationHelper", Property.Types.String, "Delivery Physical Location", "Delivery Physical Location Helper.", false, IGURL.EncounterMaternity, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='MTH')", "")]
        public string DeliveryPhysicalLocationHelper
        {
            get => GetPhysicalLocationHelper(EncounterMaternity);
            set => SetPhysicalLocationHelper(EncounterMaternity ?? CreateMaternityEncounter(), value, BFDR.Mappings.BirthDeliveryOccurred.FHIRToIJE, BFDR.ValueSets.BirthDeliveryOccurred.Codes);
        }

        /// <summary>Estimated time of fetal death.</summary>
        /// <value>Estimated time of fetal death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.TimeOfFetalDeath = ;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Sex at Time of Birth: {ExampleFetalDeathRecord.TimeOfFetalDeath}");</para>
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

        /// <summary>Estimated time of fetal death Helper.</summary>
        /// <value>Estimated time of fetal death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.TimeOfFetalDeathHelper = "434681000124104";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Time of Fetal Death: {ExampleFetalDeathRecord.TimeOfFetalDeathHelper}");</para>
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
                  "Initiating Cause/Condition, Rupture of Membranes Prior to Onset of Labor", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "44223004", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool PrematureRuptureOfMembranes
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Abruptio placenta.</summary>
        [Property("Abruptio Placenta", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Abruptio Placenta", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "415105001", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool AbruptioPlacenta
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Placental insufficiency.</summary>
        [Property("Placental Insufficiency", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Placental Insufficiency", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "237292005", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool PlacentalInsufficiency
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Prolapsed cord.</summary>
        [Property("Prolapsed Cord", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Prolapsed Cord", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "270500004", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool ProlapsedCord
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Chorioamnionitis.</summary>
        [Property("Chorioamnionitis", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Chorioamnionitis", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "11612004", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool ChorioamnionitisCOD
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Other complications of placenta, cord, or membranes.</summary>
        [Property("Other Complications of Placenta, Cord, or Membranes", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Other Complications of Placenta, Cord, or Membranes", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "membranes", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool OtherComplicationsOfPlacentaCordOrMembranes
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Unknown.</summary>
        [Property("Unknown Initiating Cause or Condition", Property.Types.Bool, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Unknown", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "UNK", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public bool InitiatingCauseOrConditionUnknown
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Initiating cause/condition, Maternal conditions/diseases literal.</summary>
        [Property("Maternal Conditions Diseases Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Maternal Conditions Diseases Literal", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "maternalconditions", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string MaternalConditionsDiseasesLiteral
        {
            get
            {
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
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

        /// <summary>NumberOfFetalDeathsThisDelivery.</summary>
        /// <value>NumberOfFetalDeathsThisDelivery</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.NumberOfFetalDeathsThisDelivery = 1;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"NumberOfFetalDeathsThisDelivery: {ExampleFetalDeathRecord.NumberOfFetalDeathsThisDelivery}");</para>
        /// </example>
        [Property("NumberOfFetalDeathsThisDelivery", Property.Types.Int32, "Number Of Fetal Deaths This Delivery", "Number Of Fetal Deaths This Delivery", true, IGURL.ObservationNumberFetalDeathsThisDelivery, true, 153)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='73772-6')", "")]
        public int? NumberOfFetalDeathsThisDelivery
        {
            get
            {
                Observation obs = GetObservation("73772-6");
                if (obs != null && obs.Value != null)
                {
                    return (obs.Value as Hl7.Fhir.Model.Integer).Value;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                Observation obs = GetOrCreateObservation("73772-6", CodeSystems.LOINC, "Number Of Fetal Deaths This Delivery", BFDR.ProfileURL.ObservationNumberFetalDeathsThisDelivery, FETUS_SECTION, Mother.Id);
                obs.Value = new Hl7.Fhir.Model.Integer(value);
            }
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
        [Property("Autopsy Performed Indicator", Property.Types.Dictionary, "Fetus", "Autopsy Performed Indicator.", true, BFDR.IGURL.ObservationAutopsyPerformedIndicator, true, 148)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='85699-7')", "")]
        public Dictionary<string, string> AutopsyPerformedIndicator
        {
            get => GetObservationValue("85699-7");
            set => SetObservationValue(value, "85699-7", CodeSystems.LOINC, "Autopsy Performed Indicator", BFDR.ProfileURL.ObservationAutopsyPerformedIndicator, FETUS_SECTION);
        }

        /// <summary>Autopsy Performed Indicator Helper. This is a helper method, to access the code use the AutopsyPerformedIndicator property.</summary>
        /// <value>autopsy performed indicator. A null value indicates "not applicable".</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.AutopsyPerformedIndicatorHelper = "Y";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Autopsy Performed Indicator: {ExampleDFetaleathRecord.AutopsyPerformedIndicatorBoolean}");</para>
        /// </example>
        [Property("Autopsy Performed Indicator Helper", Property.Types.String, "Fetus", "Autopsy Performed Indicator.", false, BFDR.IGURL.ObservationAutopsyPerformedIndicator, true, 148)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='85699-7')", "")]
        public string AutopsyPerformedIndicatorHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, BFDR.ValueSets.PerformedNotPerformedPlanned.Codes);
        }

        /// <summary>Histological Placental Exam Performed.</summary>
        /// <value>Histological Placental Exam Performed. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; code = new Dictionary&lt;string, string&gt;();</para>
        /// <para>code.Add("code", "398166005");</para>
        /// <para>code.Add("system", CodeSystems.SCT);</para>
        /// <para>code.Add("display", "Performed");</para>
        /// <para>ExampleFetalDeathRecord.HistologicalPlacentalExamPerformed = code;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Histological Placental Exam Performed: {ExampleFetalDeathRecord.HistologicalPlacentalExamPerformed['display']}");</para>
        /// </example>
        [Property("Histological Placental Exam Performed", Property.Types.Dictionary, "Fetus", "Histological Placental Exam Performed.", true, BFDR.IGURL.ObservationHistologicalPlacentalExamPerformed, true, 149)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='73767-6')", "")]
        public Dictionary<string, string> HistologicalPlacentalExaminationPerformed
        {
            get => GetObservationValue("73767-6");
            set => SetObservationValue(value, "73767-6", CodeSystems.LOINC, "Histological Placental Exam Performed", BFDR.ProfileURL.ObservationHistologicalPlacentalExamPerformed, FETUS_SECTION);
        }

        /// <summary>Histological Placental Exam Performed Helper. This is a helper method, to access the code use the HistologicalPlacentalExamPerformed property.</summary>
        /// <value>Histological Placental Exam Performed.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.HistologicalPlacentalExamPerformedHelper = "398166005";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Histological Placental Exam Performed: {ExampleFetalDeathRecord.HistologicalPlacentalExamPerformed}");</para>
        /// </example>
        [Property("Histological Placental Exam Performed Helper", Property.Types.String, "Fetus", "Histological Placental Exam Performed.", false, BFDR.IGURL.ObservationHistologicalPlacentalExamPerformed, true, 149)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='73767-6')", "")]
        public string HistologicalPlacentalExaminationPerformedHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, BFDR.ValueSets.PerformedNotPerformedPlanned.Codes);
        }

        /// <summary>Fetal Remains Disposition Method.</summary>
        /// <value>Fetal Remains Disposition Method. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; code = new Dictionary&lt;string, string&gt;();</para>
        /// <para>code.Add("code", "449971000124106");</para>
        /// <para>code.Add("system", CodeSystems.SCT);</para>
        /// <para>code.Add("display", "Burial");</para>
        /// <para>ExampleFetalDeathRecord.FetalRemainsDispositionMethod = code;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Fetal Remains Disposition Method: {ExampleFetalDeathRecord.FetalRemainsDispositionMethod['display']}");</para>
        /// </example>
        [Property("Fetal Remains Disposition Method", Property.Types.Dictionary, "Fetus", "Fetal Remains Disposition Method.", true, BFDR.IGURL.ObservationFetalRemainsDispositionMethod, true, 4)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='88241-5')", "")]
        public Dictionary<string, string> FetalRemainsDispositionMethod
        {
            get => GetObservationValue("88241-5");
            set => SetObservationValue(value, "88241-5", CodeSystems.LOINC, "Fetal Remains Disposition Method", BFDR.ProfileURL.ObservationFetalRemainsDispositionMethod, FETUS_SECTION);
        }

        /// <summary>Fetal Remains Disposition Method Helper. This is a helper method, to access the code use the FetalRemainsDispositionMethod property.</summary>
        /// <value>Fetal Remains Disposition Method.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.FetalRemainsDispositionMethodHelper = "449971000124106";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Fetal Remains Disposition Method: {ExampleFetalDeathRecord.FetalRemainsDispositionMethodHelper}");</para>
        /// </example>
        [Property("Fetal Remains Disposition Method Helper", Property.Types.String, "Fetus", "Fetal Remains Disposition Method.", false, BFDR.IGURL.ObservationFetalRemainsDispositionMethod, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='88241-5')", "")]
        public string FetalRemainsDispositionMethodHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, BFDR.ValueSets.FetalRemainsDispositionMethod.Codes);
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
                  "Initiating Cause/Condition, Other Complications of Placenta Cord Membranes Literal", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "membranes", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherComplicationsOfPlacentaCordMembranesLiteral
        {
            get
            {
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Other obstetrical or pregnancy complications literal.</summary>
        [Property("Other Obstetrical or Pregnancy Complications Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Other Obstetrical or Pregnancy Complications Literal", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "obstetricalcomplications", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherObstetricalOrPregnancyComplicationsLiteral
        {
            get
            {
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Fetal anomaly literal.</summary>
        [Property("Fetal Anomaly Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Fetal Anomaly Literal", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "702709008", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string FetalAnomalyLiteral
        {
            get
            {
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Fetal injury literal.</summary>
        [Property("Fetal Injury Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Fetal Injury Literal", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "277489001", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string FetalInjuryLiteral
        {
            get
            {
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Fetal infection literal.</summary>
        [Property("Fetal Infection Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Fetal Infection Literal", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "128270001", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string FetalInfectionLiteral
        {
            get
            {
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
                    cond = (Condition)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                }
                cond.Code.Text = value;
            }
        }

        /// <summary>Initiating cause/condition, Other fetal conditions/disorders literal.</summary>
        [Property("Other Fetal Conditions/Disorders Literal", Property.Types.String, "Initiating Cause/Condition",
                  "Initiating Cause/Condition, Other Fetal Conditions/Disorders Literal", true, IGURL.ConditionFetalDeathInitiatingCauseOrCondition, true, 100)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "76060-3", code: "fetalconditions", section: FETUS_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.DecedentFetus)]
        public string OtherFetalConditionsDisordersLiteral
        {
            get
            {
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
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
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
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
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
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
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
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
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
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
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
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
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
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
                Condition cond = GetCondition();
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
                Condition cond = GetCondition();
                if (cond == null)
                {
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
        /// <para>"addressStnum" - address, stnum</para>
        /// <para>"addressPredir" - address, predir</para>
        /// <para>"addressPostdir" - address, postdir</para>
        /// <para>"addressStname" - address, stname</para>
        /// <para>"addressStrdesig" - address, strdesig</para>
        /// <para>"addressUnitnum" - address, unitnum</para>
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
        /// <para>address.Add("addressStnum", "123");</para>
        /// <para>address.Add("addressPredir", "E");</para>
        /// <para>address.Add("addressPostDir", "SW");</para>
        /// <para>address.Add("addressStname", "Test");</para>
        /// <para>address.Add("addressStrdesig", "Street");</para>
        /// <para>address.Add("addressUnitnum", "3");</para>
        /// <para>ExampleFetalDeathRecord.PlaceOfDelivery = address;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"State where baby was delivered: {ExampleFetalDeathRecord.PlaceOfDelivery["addressState"]}");</para>
        /// </example>
        [Property("Place of Delivery", Property.Types.Dictionary, "Place of Delivery", "Place of Delivery.", true, IGURL.LocationBFDR, true, 20)]
        [PropertyParam("addressLine1", "address, line one")]
        [PropertyParam("addressLine2", "address, line two")]
        [PropertyParam("addressCity", "address, city")]
        [PropertyParam("addressCounty", "address, county")]
        [PropertyParam("addressState", "address, state")]
        [PropertyParam("addressZip", "address, zip")]
        [PropertyParam("addressCountry", "address, country")]
        [PropertyParam("addressStnum", "address, stnum")]
        [PropertyParam("addressPredir", "address, predir")]
        [PropertyParam("addressPostdir", "address, postdir")]
        [PropertyParam("addressStname", "address, stname")]
        [PropertyParam("addressStrdesig", "address, strdesig")]
        [PropertyParam("addressUnitnum", "address, unitnum")]
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

        /// <summary>Decedent Fetus's Date of Delivery.</summary>
        /// <value>the decedent fetus's date of delivery</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.DateOfDelivery = "1940-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Decedent Fetus Date of Delivery: {ExampleFetalDeathRecord.DateOfBirth}");</para>
        /// </example>
        [Property("Date Of Delivery", Property.Types.String, "Fetus Demographics", "Decedent Fetus's Date of Delivery.", true, BFDR.IGURL.PatientDecedentFetus, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate", "")]
        public string DateOfDelivery
        {
            get => GetDateOfDelivery();
            set => SetDateOfDelivery(value);
        }

        /// <summary>Decedent Fetus's Date-Time of Delivery.</summary>
        /// <value>the decedent fetus's date and time of delivery</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.DateTimeOfDelivery = "1940-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Decedent Fetus Date of Delivery: {ExampleFetalDeathRecord.DateOfBirth}");</para>
        /// </example>
        [Property("Date Of Delivery", Property.Types.String, "Fetus Demographics", "Decedent Fetus's Date of Delivery.", true, BFDR.IGURL.PatientDecedentFetus, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate", "")]
        public string DateTimeOfDelivery
        {
            get => GetDateTimeOfDelivery();
            set => SetDateTimeOfDelivery(value);
        }

        /// <summary>Decedent Fetus's BirthSex at fetal death.</summary>
        /// <value>The decedent fetus's BirthSex at time of fetal death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.FetalDeathSex = "female;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Sex at Time of Fetal Death: {ExampleFetalDeathRecord.FetalDeathSex}");</para>
        /// </example>
        [Property("Decedent Fetus Sex At Birth", Property.Types.String, "Fetus Demographics", "Decedent Fetus's Sex at Birth.", true, BFDR.ProfileURL.PatientDecedentFetus, true, 12)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url='" + OtherExtensionURL.BirthSex + "')", "")]
        public string FetalDeathSex
        {
            get => GetBirthSex();
            set => SetBirthSex(value);
        }

        /// <summary>Decedent Fetus's Sex at Fetal Death Helper.</summary>
        /// <value>The decedent fetus's sex at time of fetal death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.FetalDeathSexHelper = "female;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Sex at Time of Fetal Death: {ExampleFetalDeathRecord.FetalDeathSexHelper}");</para>
        /// </example>
        [Property("Decedent Fetus Sex At Birth Helper", Property.Types.String, "Fetus Demographics", "Decedent Fetus's Sex at Birth.", false, BFDR.ProfileURL.PatientDecedentFetus, true, 12)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url='" + OtherExtensionURL.BirthSex + "')", "")]
        public string FetalDeathSexHelper
        {
            get => GetBirthSex();
            set => SetBirthSex(value);
        }

        /// <summary>Multiple birth set order</summary>
        /// <value>The order that the decedent fetus was born if a multiple birth or null if it was a single birth</value>
        /// <example>
        /// <para>ExampleFetalDeathRecord.FetalDeathSetOrder = null; // single birth</para>
        /// <para>ExampleFetalDeathRecord.FetalDeathSetOrder = -1; // unknow whether single or multiple birth</para>
        /// <para>ExampleFetalDeathRecord.FetalDeathSetOrder = 1; // multiple birth, born first</para>
        /// </example>
        [Property("FetalDeathSetOrder", Property.Types.Int32, "Fetus Demographics", "Fetus Demographics, Set Order", true, BFDR.ProfileURL.PatientDecedentFetus, true, 208)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "multipleBirth")]
        public int? FetalDeathSetOrder
        {
            get => GetSetOrder();
            set => SetSetOrder(value);
        }

        /// <summary>Multiple birth set order edit flag</summary>
        /// <value>the multiple birth set order edit flag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; route = new Dictionary&lt;string, string&gt;();</para>
        /// <para>route.Add("code", "queriedCorrect");</para>
        /// <para>route.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");</para>
        /// <para>route.Add("display", "Queried, and Correct");</para>
        /// <para>ExampleFetalDeathRecord.FetalDeathPluralityEditFlag = route;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Multiple birth set order edit flag: {ExampleFetalDeathRecord.FetalDeathPluralityEditFlag}");</para>
        /// </example>
        [Property("FetalDeathPluralityEditFlag", Property.Types.Dictionary, "Fetus Demographics", "Fetus Demographics, Plurality Edit Flag", true, BFDR.ProfileURL.PatientDecedentFetus, true, 211)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).multipleBirth.extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag')", "")]
        public Dictionary<string, string> FetalDeathPluralityEditFlag
        {
            get => GetPluralityEditFlag();
            set => SetPluralityEditFlag(value);
        }

        /// <summary>Multiple birth set order edit flag helper</summary>
        /// <value>the multiple birth set order edit flag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.FetalDeathPluralityEditFlagHelper = "queriedCorrect";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Multiple birth set order edit flag: {ExampleFetalDeathRecord.FetalDeathPluralityEditFlagHelper}");</para>
        /// </example>
        [Property("FetalDeathPluralityEditFlagHelper", Property.Types.String, "Fetus Demographics", "Fetus Demographics, Plurality Edit Flag", false, BFDR.ProfileURL.PatientDecedentFetus, true, 211)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).multipleBirth.extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag')", "")]
        public string FetalDeathPluralityEditFlagHelper
        {
            get => GetPluralityEditFlagHelper();
            set => SetPluralityEditFlagHelper("FetalDeathPluralityEditFlag", value);
        }

        /// <summary>Multiple birth plurality</summary>
        /// <value>Where a patient is a part of a multiple birth, this is the total number of births that occurred in this pregnancy.</value>
        /// <example>
        /// <para>ExampleFetalDeathRecord.FetalDeathPlurality = null; // single birth</para>
        /// <para>ExampleFetalDeathRecord.FetalDeathPlurality = -1; // unknown number of births birth</para>
        /// <para>ExampleFetalDeathRecord.FetalDeathPlurality = 2; // two births for this pregnancy</para>
        /// </example>
        [Property("FetalDeathPlurality", Property.Types.Int32, "Fetus Demographics", "Fetus Demographics, Plurality", true, BFDR.ProfileURL.PatientDecedentFetus, true, 207)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).multipleBirth.extension.where(url = 'http://hl7.org/fhir/StructureDefinition/patient-multipleBirthTotal')", "")]
        public int? FetalDeathPlurality
        {
            get => GetPlurality();
            set => SetPlurality(value);
        }
    }
}
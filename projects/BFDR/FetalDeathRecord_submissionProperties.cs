using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Hl7.Fhir.Model;
using VR;
using Hl7.Fhir.Support;
using static Hl7.Fhir.Model.Encounter;
using Hl7.Fhir.Utility;

// FetalDeathRecord_submissionProperties.cs
// These fields are used primarily for submitting birth records to NCHS.

namespace BFDR
{
    /// <summary>Class <c>FetalDeathRecord</c> is an abstract base class models FHIR Vital Records
    /// Birth Reporting (BFDR) Birth and Fetal Death Records. This class was designed to help consume
    /// and produce FetalDeath records that follow the HL7 FHIR Vital Records Birth Reporting Implementation
    /// Guide, as described at: http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// TODO BFDR STU2 has broken up its birth record bundles, the birth bundle has birthCertificateNumber + required birth compositions,
    /// the fetal death bundle has fetalDeathReportNumber + required fetal death compositions,
    /// the demographic bundle has a fileNumber + requiredCompositionCodedRaceAndEthnicity,
    /// and the cause of death bundle has a fetalDeathReportNumber + required CompositionCodedCauseOfFetalDeath
    /// TODO BFDR STU2 supports usual work and role extension
    /// </summary>
    public partial class FetalDeathRecord : NatalityRecord
    {
        
        //TODO this should move if a fieldsAndCreateMethods is created for fetal death
        /// <summary>Composition Section Constants</summary>
        private const string FETUS_SECTION = "76400-1";

        //
        // Fetus Section
        //

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
        [Property("PAutopsyorHistologicalExamResultsUsedHelper", Property.Types.String, "Fetus", "AutopsyorHistologicalExamResultsUsedHelper", false, BFDR.IGURL.ObservationAutopsyHistologicalExamResultsUsed, true, 150)]
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
        public int? BirthWeight
        {
          get => GetWeight("8339-4");
          set => SetWeight("8339-4", value, "g", FETUS_SECTION, Child.Id);
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
        public Dictionary<string, string> BirthWeightEditFlag
        {
            get
            {
                Observation observation = GetObservation("8339-4");
                Extension extension = observation?.Value?.Extension.FirstOrDefault(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                if (extension != null && extension.Value != null && extension.Value.GetType() == typeof(CodeableConcept))
                {
                    return CodeableConceptToDict((CodeableConcept)extension.Value);
                }
                return EmptyCodeableDict();
            }

            set
            {
                Observation obs = GetOrCreateObservation("8339-4", CodeSystems.LOINC, BFDR.ProfileURL.ObservationBirthWeight, FETUS_SECTION, Child.Id);
                // Create an empty quantity if needed
                if (obs.Value == null || obs.Value as Quantity == null)
                {
                    obs.Value = new Hl7.Fhir.Model.Quantity();
                }
                obs.Value.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                obs.Value.Extension.Add(new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(value)));
            }
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
        public string BirthWeightEditFlagHelper
        {
            get 
            {
              Dictionary<string, string> editFlag = this.BirthWeightEditFlag;
              if (editFlag.ContainsKey("code"))
              {
                  string flagCode = editFlag["code"];
                  if (!String.IsNullOrWhiteSpace(flagCode))
                  {
                      return flagCode;
                  }
              }
              return null;
            }
            set 
            {
                Observation obs = GetOrCreateObservation("8339-4", CodeSystems.LOINC, BFDR.ProfileURL.ObservationBirthWeight, FETUS_SECTION, Child.Id);
                obs.Value.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);

                if (String.IsNullOrEmpty(value))
                {
                    obs.Value.Extension.Add(new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(EmptyCodeDict())));
                    return;
                }

                // Iterate over the allowed options and see if the code supplies is one of them
                string[,] options = BFDR.ValueSets.BirthWeightEditFlags.Codes;
                for (int i = 0; i < options.GetLength(0); i += 1)
                {
                    if (options[i, 0] == value)
                    {
                        // Found it, so call the supplied setter with the appropriate dictionary built based on the code
                        // using the supplied options and return
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("code", value);
                        dict.Add("display", options[i, 1]);
                        dict.Add("system", options[i, 2]);
                        obs.Value.Extension.Add(new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(dict)));
                    }
                }
            }
        }
    }
}

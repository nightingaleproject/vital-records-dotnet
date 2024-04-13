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
      
      
        //TODO this should be in fields and create methods, temporary fix
        /// <summary>Composition Section Constants</summary>
        private const string FETUS_SECTION = "76400-1";
        
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
        [Property("AutopsyorHistologicalExamResultsUsed", Property.Types.Dictionary, "AutopsyorHistologicalExamResultsUsed", "AutopsyorHistologicalExamResultsUsed", false, BFDR.IGURL.ObservationAutopsyHistologicalExamResultsUsed, true, 150)]
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
        [Property("PAutopsyorHistologicalExamResultsUsedHelper", Property.Types.String, "AutopsyorHistologicalExamResultsUsedHelper", "AutopsyorHistologicalExamResultsUsedHelper", false, BFDR.IGURL.ObservationAutopsyHistologicalExamResultsUsed, true, 150)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='74498-7')", "")]
        public string AutopsyorHistologicalExamResultsUsedHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, VR.ValueSets.YesNoNotApplicable.Codes);
        }
    }
}

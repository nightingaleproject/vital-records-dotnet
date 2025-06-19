using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VR;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;

namespace BFDR
{
    public partial class NatalityRecord
    {
        private Dictionary<string, string> GetCodedOccupation(string role)
        {
            Observation observation = GetOrCreateOccupationObservation(role);
            if (observation != null)
            {
                return CodeableConceptToDict((CodeableConcept)observation.Value);
            }
            return EmptyCodeableDict();
        }

        private Dictionary<string, string> GetCodedIndustry(string role)
        {
            Observation observation = GetOrCreateOccupationObservation(role);
            if (observation != null)
            {
                var component = observation.Component.Where(c => CodeableConceptToDict(c.Code)["code"] == "86188-0").FirstOrDefault();
                if (component != null)
                {
                    return CodeableConceptToDict((CodeableConcept)component.Value);
                }
            }
            return EmptyCodeableDict();
        }

        private void SetCodedOccupation(string role, Dictionary<string, string> value)
        {
            Observation observation = GetOrCreateOccupationObservation(role);
            if (observation != null)
            {
                // Preserve any existing Text in the value
                string existingText = (observation.Value as CodeableConcept).Text;
                observation.Value = DictToCodeableConcept(value);
                (observation.Value as CodeableConcept).Text = existingText;
            }
        }

        private void SetCodedIndustry(string role, Dictionary<string, string> value)
        {
            Observation observation = GetOrCreateOccupationObservation(role);
            if (observation != null)
            {
                var component = observation.Component.Where(c => CodeableConceptToDict(c.Code)["code"] == "86188-0").FirstOrDefault();
                if (component == null)
                {
                    component = new Observation.ComponentComponent
                    {
                        Code = new CodeableConcept(CodeSystems.LOINC, "86188-0")
                    };
                    observation.Component.Add(component);
                }
                // Preserve any existing Text in the value
                string existingText = (component.Value as CodeableConcept).Text;
                component.Value = DictToCodeableConcept(value);
                (component.Value as CodeableConcept).Text = existingText;
            }
        }

        // TODO: Add Helpers for each of the below using GetObservationValueHelper; may need a version that doesn't expect codes
        // TODO: Add basic tests for these in addition to the bundle ones

        /// <summary>Coded Occupation of Mother.</summary>
        /// <value>the occupation of the mother as a Dictionary containing coded information</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; occ = new Dictionary&lt;string, string&gt;();</para>
        /// <para>occ.Add("code", "27-2011");</para>
        /// <para>occ.Add("system", "urn:oid:2.16.840.1.114222.4.11.8068");</para>
        /// <para>occ.Add("display", "Actors");</para>
        /// <para>ExampleBirthRecord.MotherCodedOccupation = occ;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Occupation: {ExampleBirthRecord.MotherCodedOccupation["display"]}");</para>
        /// </example>
        [Property("MotherCodedOccupation", Property.Types.Dictionary, "Mother Information", "Mother's Occupation", true, IGURL.ObservationPresentJob, false, 282)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11341-5')", "")]
        public Dictionary<string, string> MotherCodedOccupation
        {
            get => GetCodedOccupation("MTH");
            set => SetCodedOccupation("MTH", value);
        }

        /// <summary>Coded Occupation of Mother Helper.</summary>
        /// <value>the occupation of the mother as a code string</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherCodedOccupationHelper = "27-2011";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Occupation Code: {ExampleBirthRecord.MotherCodedOccupationHelper}");</para>
        /// </example>
        [Property("MotherCodedOccupationHelper", Property.Types.String, "Mother Information", "Mother's Occupation", false, IGURL.ObservationPresentJob, false, 282)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11341-5')", "")]
        public string MotherCodedOccupationHelper
        {
            get => MotherCodedOccupation?["code"];
            set => MotherCodedOccupation = new Dictionary<string, string> { { "code", value }, { "system", "urn:oid:2.16.840.1.114222.4.11.8068" } };
        }

        /// <summary>Coded Occupation of Father.</summary>
        /// <value>the occupation of the father as a Dictionary containing coded information</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; occ = new Dictionary&lt;string, string&gt;();</para>
        /// <para>occ.Add("code", "27-2011");</para>
        /// <para>occ.Add("system", "urn:oid:2.16.840.1.114222.4.11.8068");</para>
        /// <para>occ.Add("display", "Actors");</para>
        /// <para>ExampleBirthRecord.FatherCodedOccupation = occ;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Occupation: {ExampleBirthRecord.FatherCodedOccupation["display"]}");</para>
        /// </example>
        [Property("FatherCodedOccupation", Property.Types.Dictionary, "Father Information", "Father's Occupation", true, IGURL.ObservationPresentJob, false, 282)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11341-5')", "")]
        public Dictionary<string, string> FatherCodedOccupation
        {
            get => GetCodedOccupation("FTH");
            set => SetCodedOccupation("FTH", value);
        }

        /// <summary>Coded Occupation of Father Helper.</summary>
        /// <value>the occupation of the father as a code string</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherCodedOccupationHelper = "27-2011";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Occupation Code: {ExampleBirthRecord.FatherCodedOccupationHelper}");</para>
        /// </example>
        [Property("FatherCodedOccupationHelper", Property.Types.String, "Father Information", "Father's Occupation", false, IGURL.ObservationPresentJob, false, 282)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11341-5')", "")]
        public string FatherCodedOccupationHelper
        {
            get => FatherCodedOccupation?["code"];
            set => FatherCodedOccupation = new Dictionary<string, string> { { "code", value }, { "system", "urn:oid:2.16.840.1.114222.4.11.8068" } };
        }

        /// <summary>Coded Industry of Mother.</summary>
        /// <value>the industry of the mother as a Dictionary containing coded information</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ind = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ind.Add("code", "336411");</para>
        /// <para>ind.Add("system", "urn:oid:2.16.840.1.114222.4.11.8067");</para>
        /// <para>ind.Add("display", "Aircraft Manufacturing");</para>
        /// <para>ExampleBirthRecord.MotherCodedIndustry = ind;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Industry: {ExampleBirthRecord.MotherCodedIndustry["display"]}");</para>
        /// </example>
        [Property("MotherCodedIndustry", Property.Types.Dictionary, "Mother Information", "Mother's Industry", true, IGURL.ObservationPresentJob, false, 282)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11341-5')", "")]
        public Dictionary<string, string> MotherCodedIndustry
        {
            get => GetCodedIndustry("MTH");
            set => SetCodedIndustry("MTH", value);
        }

        /// <summary>Coded Industry of Mother Helper.</summary>
        /// <value>the industry of the mother as a code string</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherCodedIndustryHelper = "27-2011";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Industry Code: {ExampleBirthRecord.MotherCodedIndustryHelper}");</para>
        /// </example>
        [Property("MotherCodedIndustryHelper", Property.Types.String, "Mother Information", "Mother's Industry", false, IGURL.ObservationPresentJob, false, 282)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11341-5')", "")]
        public string MotherCodedIndustryHelper
        {
            get => MotherCodedIndustry?["code"];
            set => MotherCodedIndustry = new Dictionary<string, string> { { "code", value }, { "system", "urn:oid:2.16.840.1.114222.4.11.8067" } };
        }

        /// <summary>Coded Industry of Father.</summary>
        /// <value>the industry of the father as a Dictionary containing coded information</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ind = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ind.Add("code", "336411");</para>
        /// <para>ind.Add("system", "urn:oid:2.16.840.1.114222.4.11.8067");</para>
        /// <para>ind.Add("display", "Aircraft Manufacturing");</para>
        /// <para>ExampleBirthRecord.FatherCodedIndustry = ind;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Industry: {ExampleBirthRecord.FatherCodedIndustry["display"]}");</para>
        /// </example>
        [Property("FatherCodedIndustry", Property.Types.Dictionary, "Father Information", "Father's Industry", true, IGURL.ObservationPresentJob, false, 282)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11341-5')", "")]
        public Dictionary<string, string> FatherCodedIndustry
        {
            get => GetCodedIndustry("FTH");
            set => SetCodedIndustry("FTH", value);
        }

        /// <summary>Coded Industry of Father Helper.</summary>
        /// <value>the industry of the father as a code string</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherCodedIndustryHelper = "27-2011";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Industry Code: {ExampleBirthRecord.FatherCodedIndustryHelper}");</para>
        /// </example>
        [Property("FatherCodedIndustryHelper", Property.Types.String, "Father Information", "Father's Industry", false, IGURL.ObservationPresentJob, false, 282)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11341-5')", "")]
        public string FatherCodedIndustryHelper
        {
            get => FatherCodedIndustry?["code"];
            set => FatherCodedIndustry = new Dictionary<string, string> { { "code", value }, { "system", "urn:oid:2.16.840.1.114222.4.11.8067" } };
        }

    }
}

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath;
using Newtonsoft.Json;
using VR;
using System.Data.SqlTypes;

// FetalDeathRecord_responseOnlyProperties.cs
//    These fields are used ONLY in coded messages sent from NCHS to EDRS corresponding to TRX and MRE content.

namespace BFDR
{
    /// <summary>Class <c>FetalDeathRecord</c> models a FHIR Birth and Fetal Death Records Reporting (BFDR) Fetal Death
    /// Record. This class was designed to help consume and produce fetal death records that follow the
    /// HL7 FHIR Vital Records Birth and Fetal Death Reporting Implementation Guide, as described at:
    /// http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// </summary>
    public partial class FetalDeathRecord
    {
        /// <summary>Decedent Fetus's Coded Initiating Fetal Death Cause or Condition</summary>
        /// <value>Decedent Fetus's Coded Initiating Fetal Death Cause or Condition.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.CodedInitiatingFetalCOD = "I13.1";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Decedent Fetus's Coded Initiating Fetal Death Cause or Condition: {ExampleFetalDeathRecord.CodedInitiatingFetalCOD}");</para>
        /// </example>
        [Property("Coded Initiating Fetal Death Cause or Condition", Property.Types.String, "Coded Content", "Coded Initiating Fetal Death Cause or Condition.", true, IGURL.ObservationCodedInitiatingFetalDeathCauseOrCondition, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='92022-3')", "")]
        public string CodedInitiatingFetalCOD
        {
            get
            {
                Observation CodedInitiatingFetalDeathCauseOrConditionObs = GetObservation("92022-3");
                if (CodedInitiatingFetalDeathCauseOrConditionObs != null && CodedInitiatingFetalDeathCauseOrConditionObs.Value != null && CodedInitiatingFetalDeathCauseOrConditionObs.Value as CodeableConcept != null)
                {
                    string codeableConceptValueCode = CodeableConceptToDict((CodeableConcept)CodedInitiatingFetalDeathCauseOrConditionObs.Value)["code"];
                    if (!String.IsNullOrWhiteSpace(codeableConceptValueCode))
                    {
                      return codeableConceptValueCode;
                    }
                    return null;
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Observation obs = GetOrCreateObservation("92022-3", CodeSystems.LOINC, "Coded initiating cause or condition of fetal death", BFDR.ProfileURL.ObservationCodedInitiatingFetalDeathCauseOrCondition, CODEDCAUSEOFFETALDEATH_SECTION);
                obs.Value = new CodeableConcept(CodeSystems.ICD10, value, null, null);
            }
        }

        /// <summary>Helper to get or create a coded other significant fetal death cause or condition observation.</summary>
        /// <param name="position">the integer to specify the position, or order of mention</param>
        private Observation GetorCreateCodedOtherCODObservation(int position)
        {
            foreach (var ob in Bundle.Entry.Where(entry => entry.Resource is Observation))
            {
                Observation CODobs = (Observation)ob.Resource;
                if (CODobs.Code.Coding.First().Code == "92023-1")
                {
                    var lineNumber = 0;
                    Observation.ComponentComponent lineNumComp = CODobs.Component.Where(c => c.Code.Coding[0].Code == "246268007").FirstOrDefault();
                    if (lineNumComp != null && lineNumComp.Value != null)
                    {
                        lineNumber = Int32.Parse(lineNumComp.Value.ToString());
                    }
                    if (lineNumber == position){
                        return CODobs;
                    }
                }
            }
            Observation obs = new Observation();
            obs.Id = Guid.NewGuid().ToString();
            obs.Meta = new Meta();
            string[] condition_profile = { ProfileURL.ObservationCodedInitiatingFetalDeathCauseOrCondition };
            obs.Meta.Profile = condition_profile;
            obs.Status = ObservationStatus.Final;
            obs.Code = new CodeableConcept(CodeSystems.LOINC, "92023-1", "Coded other significant causes or conditions of fetal death", null);
            obs.Subject = new ResourceReference("urn:uuid:" + Subject.Id);
            obs.Performer.Add(new ResourceReference("urn:uuid:" + Certifier.Id));
            Observation.ComponentComponent component = new Observation.ComponentComponent();
            component.Code = new CodeableConcept(CodeSystems.SCT, "246268007", "Position (attribute)", null);
            component.Value = new Integer(position); 
            obs.Component.Add(component);
            //AddReferenceToComposition(CodCondition.Id, "DeathCertification");
            Bundle.AddResourceEntry(obs, "urn:uuid:" + obs.Id);
            return (obs);
        }

        /// <summary>Get a coded fetal death other significant cause or condition.
        /// <para>
        /// This helper returns the code of the the other significant fetal death cause or condition based on position.
        /// </para>
        /// </summary>
        /// <param name="obs">represents a coded other significant fetal death cause or condition observation</param>
        private string GetCodedOtherCOD(Observation obs)
        {
            if (obs != null && obs.Value != null)
            {
                return (CodeableConceptToDict((CodeableConcept)obs.Value))["code"];
            }
            return null;
        }
        /// <summary>Set a coded fetal death other significant cause or condition.
        /// <para>
        /// This helper sets the code of the the other significant fetal death cause or condition based on position.
        /// </para>
        /// </summary>
        /// <param name="obs">represents a coded other significant fetal death cause or condition observation</param>
        /// <param name="value">the code of the other significant fetal death cause or condition</param>
        /// <param name="position">the integer to specify the position, or order of mention</param>
        private Observation SetCodedOtherCOD(Observation obs, string value, int position)
        {
            if (obs == null)
            {
                obs = GetorCreateCodedOtherCODObservation(position);
            }
            if (!String.IsNullOrWhiteSpace(value))
            {
            obs.Value = new CodeableConcept(CodeSystems.ICD10, value, null, null);
            }
            return obs;
        }

        /// <summary>Coded Fetal Other Cause of Death, First Mentioned.</summary>
        /// <value>the coded other significant cause or conditions of death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.OCOD1 = "P02.1";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cause: {ExampleFetalDeathRecord.OCOD1}");</para>
        /// </example>
        [Property("OCOD1", Property.Types.String, "Coded Content", "Coded other significant causes or conditions- first mentioned", false, BFDR.IGURL.ObservationCodedOtherFetalDeathCauseOrCondition, false, 100)]
        public string OCOD1
        {
            get => GetCodedOtherCOD(GetorCreateCodedOtherCODObservation(1));
            set => SetCodedOtherCOD(GetorCreateCodedOtherCODObservation(1), value, 1);
        }

        /// <summary>Coded Fetal Other Cause of Death, Second Mentioned.</summary>
        /// <value>the coded other significant cause or conditions of death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.OCOD2 = "P02.1";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cause: {ExampleFetalDeathRecord.OCOD2}");</para>
        /// </example>
        [Property("OCOD2", Property.Types.String, "Coded Content", "Coded other significant causes or conditions- second mentioned", false, BFDR.IGURL.ObservationCodedOtherFetalDeathCauseOrCondition, false, 100)]
        public string OCOD2
        {
            get => GetCodedOtherCOD(GetorCreateCodedOtherCODObservation(2));
            set => SetCodedOtherCOD(GetorCreateCodedOtherCODObservation(2), value, 2);
        }

        /// <summary>Coded Fetal Other Cause of Death, Third Mentioned.</summary>
        /// <value>the coded other significant cause or conditions of death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.OCOD3 = "P02.1";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cause: {ExampleFetalDeathRecord.OCOD3}");</para>
        /// </example>
        [Property("OCOD3", Property.Types.String, "Coded Content", "Coded other significant causes or conditions- third mentioned", false, BFDR.IGURL.ObservationCodedOtherFetalDeathCauseOrCondition, false, 100)]
        public string OCOD3
        {
            get => GetCodedOtherCOD(GetorCreateCodedOtherCODObservation(3));
            set => SetCodedOtherCOD(GetorCreateCodedOtherCODObservation(3), value, 3);
        }
        /// <summary>Coded Fetal Other Cause of Death, Fourth Mentioned.</summary>
        /// <value>the coded other significant cause or conditions of death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.OCOD4 = "P02.1";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cause: {ExampleFetalDeathRecord.OCOD4}");</para>
        /// </example>
        [Property("OCOD4", Property.Types.String, "Coded Content", "Coded other significant causes or conditions- fourth mentioned", false, BFDR.IGURL.ObservationCodedOtherFetalDeathCauseOrCondition, false, 100)]
        public string OCOD4
        {
            get => GetCodedOtherCOD(GetorCreateCodedOtherCODObservation(4));
            set => SetCodedOtherCOD(GetorCreateCodedOtherCODObservation(4), value, 4);
        }

        /// <summary>Coded Fetal Other Cause of Death, Fifth Mentioned.</summary>
        /// <value>the coded other significant cause or conditions of death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.OCOD5 = "P02.1";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cause: {ExampleFetalDeathRecord.OCOD5}");</para>
        /// </example>
        [Property("OCOD5", Property.Types.String, "Coded Content", "Coded other significant causes or conditions- fifth mentioned", false, BFDR.IGURL.ObservationCodedOtherFetalDeathCauseOrCondition, false, 100)]
        public string OCOD5
        {
            get => GetCodedOtherCOD(GetorCreateCodedOtherCODObservation(5));
            set => SetCodedOtherCOD(GetorCreateCodedOtherCODObservation(5), value, 5);
        }

        /// <summary>Coded Fetal Other Cause of Death, Sixth Mentioned.</summary>
        /// <value>the coded other significant cause or conditions of death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.OCOD6 = "P02.1";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cause: {ExampleFetalDeathRecord.OCOD6}");</para>
        /// </example>
        [Property("OCOD6", Property.Types.String, "Coded Content", "Coded other significant causes or conditions- sixth mentioned", false, BFDR.IGURL.ObservationCodedOtherFetalDeathCauseOrCondition, false, 100)]
        public string OCOD6
        {
            get => GetCodedOtherCOD(GetorCreateCodedOtherCODObservation(6));
            set => SetCodedOtherCOD(GetorCreateCodedOtherCODObservation(6), value, 6);
        }

        /// <summary>Coded Fetal Other Cause of Death, Seventh Mentioned.</summary>
        /// <value>the coded other significant cause or conditions of death</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleFetalDeathRecord.OCOD7 = "P02.1";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cause: {ExampleFetalDeathRecord.OCOD7}");</para>
        /// </example>
        [Property("OCOD7", Property.Types.String, "Coded Content", "Coded other significant causes or conditions- seventh mentioned", false, BFDR.IGURL.ObservationCodedOtherFetalDeathCauseOrCondition, false, 100)]
        public string OCOD7
        {
            get => GetCodedOtherCOD(GetorCreateCodedOtherCODObservation(7));
            set => SetCodedOtherCOD(GetorCreateCodedOtherCODObservation(7), value, 7);
        }
    }
}
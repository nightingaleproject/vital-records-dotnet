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

// DeathRecord_responseOnlyProperties.cs
//    These fields are used ONLY in coded messages sent from NCHS to EDRS corresponding to TRX and MRE content.

namespace VRDR
{
    /// <summary>Class <c>DeathRecord</c> models a FHIR Vital Records Death Reporting (VRDR) Death
    /// Record. This class was designed to help consume and produce death records that follow the
    /// HL7 FHIR Vital Records Death Reporting Implementation Guide, as described at:
    /// http://hl7.org/fhir/us/vrdr and https://github.com/hl7/vrdr.
    /// </summary>
    public partial class DeathRecord
    {

        /// <summary>Activity at Time of Death.</summary>
        /// <value>the decedent's activity at time of death. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; activity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>elevel.Add("code", "0");</para>
        /// <para>elevel.Add("system", CodeSystems.ActivityAtTimeOfDeath);</para>
        /// <para>elevel.Add("display", "While engaged in sports activity");</para>
        /// <para>ExampleDeathRecord.ActivityAtDeath = activity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Education Level: {ExampleDeathRecord.EducationLevel['display']}");</para>
        /// </example>
        [Property("Activity at Time of Death", Property.Types.Dictionary, "Coded Content", "Decedent's Activity at Time of Death.", true, ProfileURL.ActivityAtTimeOfDeath, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='80626-5')", "")]
        public Dictionary<string, string> ActivityAtDeath
        {
            get
            {
                if (ActivityAtTimeOfDeathObs != null && ActivityAtTimeOfDeathObs.Value != null && ActivityAtTimeOfDeathObs.Value as CodeableConcept != null)
                {
                    return CodeableConceptToDict((CodeableConcept)ActivityAtTimeOfDeathObs.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (ActivityAtTimeOfDeathObs == null)
                {
                    CreateActivityAtTimeOfDeathObs();
                }
                ActivityAtTimeOfDeathObs.Value = DictToCodeableConcept(value);
            }
        }

        /// <summary>Decedent's Activity At Time of Death Helper</summary>
        /// <value>Decedent's Activity at Time of Death.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.ActivityAtDeath = 0;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Decedent's Activity at Time of Death: {ExampleDeathRecord.ActivityAtDeath}");</para>
        /// </example>
        [Property("Activity at Time of Death Helper", Property.Types.String, "Coded Content", "Decedent's Activity at Time of Death.", false, ProfileURL.ActivityAtTimeOfDeath, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='80626-5')", "")]
        public string ActivityAtDeathHelper
        {
            get
            {
                if (ActivityAtDeath.ContainsKey("code") && !String.IsNullOrWhiteSpace(ActivityAtDeath["code"]))
                {
                    return ActivityAtDeath["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("ActivityAtDeath", value, VRDR.ValueSets.ActivityAtTimeOfDeath.Codes);
                }
            }
        }

        /// <summary>Decedent's Automated Underlying Cause of Death</summary>
        /// <value>Decedent's Automated Underlying Cause of Death.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.AutomatedUnderlyingCOD = "I13.1";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Decedent's Automated Underlying Cause of Death: {ExampleDeathRecord.AutomatedUnderlyingCOD}");</para>
        /// </example>
        [Property("Automated Underlying Cause of Death", Property.Types.String, "Coded Content", "Automated Underlying Cause of Death.", true, ProfileURL.AutomatedUnderlyingCauseOfDeath, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='80358-5')", "")]
        public string AutomatedUnderlyingCOD
        {
            get
            {
                if (AutomatedUnderlyingCauseOfDeathObs != null && AutomatedUnderlyingCauseOfDeathObs.Value != null && AutomatedUnderlyingCauseOfDeathObs.Value as CodeableConcept != null)
                {
                    string codeableConceptValueCode = CodeableConceptToDict((CodeableConcept)AutomatedUnderlyingCauseOfDeathObs.Value)["code"];
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
                if (AutomatedUnderlyingCauseOfDeathObs == null)
                {
                    CreateAutomatedUnderlyingCauseOfDeathObs();
                }
                AutomatedUnderlyingCauseOfDeathObs.Value = new CodeableConcept(CodeSystems.ICD10, value, null, null);
            }
        }

        /// <summary>Decedent's Manual Underlying Cause of Death</summary>
        /// <value>Decedent's Manual Underlying Cause of Death.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.ManUnderlyingCOD = "I13.1";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Decedent's Manual Underlying Cause of Death: {ExampleDeathRecord.ManUnderlyingCOD}");</para>
        /// </example>
        [Property("Manual Underlying Cause of Death", Property.Types.String, "Coded Content", "Manual Underlying Cause of Death.", true, ProfileURL.ManualUnderlyingCauseOfDeath, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='80358-580359-3')", "")]
        public string ManUnderlyingCOD
        {
            get
            {
                if (ManualUnderlyingCauseOfDeathObs != null && ManualUnderlyingCauseOfDeathObs.Value != null && ManualUnderlyingCauseOfDeathObs.Value as CodeableConcept != null)
                {
                    string codeableConceptValueCode = CodeableConceptToDict((CodeableConcept)ManualUnderlyingCauseOfDeathObs.Value)["code"];
                    if(String.IsNullOrWhiteSpace(codeableConceptValueCode)){
                      return null;
                    }
                    return codeableConceptValueCode;
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                if (ManualUnderlyingCauseOfDeathObs == null)
                {
                    CreateManualUnderlyingCauseOfDeathObs();
                }
                ManualUnderlyingCauseOfDeathObs.Value = new CodeableConcept(CodeSystems.ICD10, value, null, null);
            }
        }

        /// <summary>Place of Injury.</summary>
        /// <value>Place of Injury. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; elevel = new Dictionary&lt;string, string&gt;();</para>
        /// <para>elevel.Add("code", "LA14084-0");</para>
        /// <para>elevel.Add("system", CodeSystems.LOINC);</para>
        /// <para>elevel.Add("display", "Home");</para>
        /// <para>ExampleDeathRecord.PlaceOfInjury = elevel;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"PlaceOfInjury: {ExampleDeathRecord.PlaceOfInjury['display']}");</para>
        /// </example>
        [Property("Place of Injury", Property.Types.Dictionary, "Coded Content", "Place of Injury.", true, ProfileURL.PlaceOfInjury, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11376-1')", "")]
        public Dictionary<string, string> PlaceOfInjury
        {
            get
            {
                if (PlaceOfInjuryObs != null && PlaceOfInjuryObs.Value != null && PlaceOfInjuryObs.Value as CodeableConcept != null)
                {
                    return CodeableConceptToDict((CodeableConcept)PlaceOfInjuryObs.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (PlaceOfInjuryObs == null)
                {
                    CreatePlaceOfInjuryObs();
                }
                PlaceOfInjuryObs.Value = DictToCodeableConcept(value);
            }
        }

        /// <summary>Decedent's Place of Injury Helper</summary>
        /// <value>Place of Injury.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.PlaceOfInjuryHelper = ValueSets.PlaceOfInjury.Home;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Place of Injury: {ExampleDeathRecord.PlaceOfInjuryHelper}");</para>
        /// </example>
        [Property("Place of Injury Helper", Property.Types.String, "Coded Content", "Place of Injury.", false, ProfileURL.PlaceOfInjury, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11376-1')", "")]
        public string PlaceOfInjuryHelper
        {
            get
            {
                if (PlaceOfInjury.ContainsKey("code") && !String.IsNullOrWhiteSpace(PlaceOfInjury["code"]))
                {
                    return PlaceOfInjury["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("PlaceOfInjury", value, VRDR.ValueSets.PlaceOfInjury.Codes);
                }
            }
        }


        /// <summary>Entity Axis Cause Of Death
        /// <para>Note that record axis codes have an unusual and obscure handling of a Pregnancy flag, for more information see
        /// http://build.fhir.org/ig/HL7/vrdr/branches/master/StructureDefinition-vrdr-record-axis-cause-of-death.html#usage></para>
        /// </summary>
        /// <value>Entity-axis codes</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para> ExampleDeathRecord.EntityAxisCauseOfDeath = new [] {(LineNumber: 2, Position: 1, Code: "T27.3", ECode: true)};</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"First Entity Axis Code: {ExampleDeathRecord.EntityAxisCauseOfDeath.ElementAt(0).Code}");</para>
        /// </example>
        [Property("Entity Axis Cause of Death", Property.Types.Tuple4Arr, "Coded Content", "", true, ProfileURL.EntityAxisCauseOfDeath, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=80356-9)", "")]
        public IEnumerable<(int LineNumber, int Position, string Code, bool ECode)> EntityAxisCauseOfDeath
        {
            get
            {
                List<(int LineNumber, int Position, string Code, bool ECode)> eac = new List<(int LineNumber, int Position, string Code, bool ECode)>();
                if (EntityAxisCauseOfDeathObsList != null)
                {
                    foreach (Observation ob in EntityAxisCauseOfDeathObsList)
                    {
                        int? lineNumber = null;
                        int? position = null;
                        string icd10code = null;
                        bool ecode = false;
                        Observation.ComponentComponent positionComp = ob.Component.Where(c => c.Code.Coding[0].Code == "position").FirstOrDefault();
                        if (positionComp != null && positionComp.Value != null)
                        {
                            position = ((Integer)positionComp.Value).Value;
                        }
                        Observation.ComponentComponent lineNumComp = ob.Component.Where(c => c.Code.Coding[0].Code == "lineNumber").FirstOrDefault();
                        if (lineNumComp != null && lineNumComp.Value != null)
                        {
                            lineNumber = ((Integer)lineNumComp.Value).Value;
                        }
                        CodeableConcept valueCC = (CodeableConcept)ob.Value;
                        if (valueCC != null && valueCC.Coding != null && valueCC.Coding.Count() > 0)
                        {
                            icd10code = valueCC.Coding[0].Code.Trim();
                        }
                        Observation.ComponentComponent ecodeComp = ob.Component.Where(c => c.Code.Coding[0].Code == "eCodeIndicator").FirstOrDefault();
                        if (ecodeComp != null && ecodeComp.Value != null)
                        {
                            ecode = (bool)((FhirBoolean)ecodeComp.Value).Value;
                        }
                        if (lineNumber != null && position != null && icd10code != null)
                        {
                            eac.Add((LineNumber: (int)lineNumber, Position: (int)position, Code: icd10code, ECode: ecode));
                        }
                    }
                }
                return eac.OrderBy(element => element.LineNumber).ThenBy(element => element.Position);
            }
            set
            {
                // clear all existing eac
                Bundle.Entry.RemoveAll(entry => entry.Resource is Observation && (((Observation)entry.Resource).Code.Coding.First().Code == "80356-9"));
                if (EntityAxisCauseOfDeathObsList != null)
                {
                    EntityAxisCauseOfDeathObsList.Clear();
                }
                else
                {
                    EntityAxisCauseOfDeathObsList = new List<Observation>();
                }
                // Rebuild the list of observations
                foreach ((int LineNumber, int Position, string Code, bool ECode) eac in value)
                {
                    if(!String.IsNullOrEmpty(eac.Code))
                    {
                        Observation ob = new Observation();
                        ob.Id = Guid.NewGuid().ToString();
                        ob.Meta = new Meta();
                        string[] entityAxis_profile = { ProfileURL.EntityAxisCauseOfDeath };
                        ob.Meta.Profile = entityAxis_profile;
                        ob.Status = ObservationStatus.Final;
                        ob.Code = new CodeableConcept(CodeSystems.LOINC, "80356-9", "Cause of death entity axis code [Automated]", null);
                        ob.Subject = new ResourceReference("urn:uuid:" + Decedent.Id);
                        AddReferenceToComposition(ob.Id, "CodedContent");

                        ob.Effective = new FhirDateTime();
                        ob.Value = new CodeableConcept(CodeSystems.ICD10, eac.Code, null, null);
                        Observation.ComponentComponent lineNumComp = new Observation.ComponentComponent();
                        lineNumComp.Value = new Integer(eac.LineNumber);
                        lineNumComp.Code = new CodeableConcept(CodeSystems.Component, "lineNumber", "lineNumber", null);
                        ob.Component.Add(lineNumComp);

                        Observation.ComponentComponent positionComp = new Observation.ComponentComponent();
                        positionComp.Value = new Integer(eac.Position);
                        positionComp.Code = new CodeableConcept(CodeSystems.Component, "position", "Position", null);
                        ob.Component.Add(positionComp);

                        Observation.ComponentComponent eCodeComp = new Observation.ComponentComponent();
                        eCodeComp.Value = new FhirBoolean(eac.ECode);
                        eCodeComp.Code = new CodeableConcept(CodeSystems.Component, "eCodeIndicator", "eCodeIndicator", null);
                        ob.Component.Add(eCodeComp);

                        Bundle.AddResourceEntry(ob, "urn:uuid:" + ob.Id);
                        EntityAxisCauseOfDeathObsList.Add(ob);
                    }
                }
            }
        }

        /// <summary>Record Axis Cause Of Death</summary>
        /// <value>record-axis codes</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Tuple&lt;string, string, string&gt;[] eac = new Tuple&lt;string, string, string&gt;{Tuple.Create("position", "code", "pregnancy")}</para>
        /// <para>ExampleDeathRecord.RecordAxisCauseOfDeath = new [] { (Position: 1, Code: "T27.3", Pregnancy: true) };</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"First Record Axis Code: {ExampleDeathRecord.RecordAxisCauseOfDeath.ElememtAt(0).Code}");</para>
        /// </example>
        [Property("Record Axis Cause Of Death", Property.Types.Tuple4Arr, "Coded Content", "", true, ProfileURL.RecordAxisCauseOfDeath, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=80357-7)", "")]
        public IEnumerable<(int Position, string Code, bool Pregnancy)> RecordAxisCauseOfDeath
        {
            get
            {
                List<(int Position, string Code, bool Pregnancy)> rac = new List<(int Position, string Code, bool Pregnancy)>();
                if (RecordAxisCauseOfDeathObsList != null)
                {
                    foreach (Observation ob in RecordAxisCauseOfDeathObsList)
                    {
                        int? position = null;
                        string icd10code = null;
                        bool pregnancy = false;
                        Observation.ComponentComponent positionComp = ob.Component.Where(c => c.Code.Coding[0].Code == "position").FirstOrDefault();
                        if (positionComp != null && positionComp.Value != null)
                        {
                            position = ((Integer)positionComp.Value).Value;
                        }
                        CodeableConcept valueCC = (CodeableConcept)ob.Value;
                        if (valueCC != null && valueCC.Coding != null && valueCC.Coding.Count() > 0)
                        {
                            icd10code = valueCC.Coding[0].Code;
                        }
                        Observation.ComponentComponent pregComp = ob.Component.Where(c => c.Code.Coding[0].Code == "wouldBeUnderlyingCauseOfDeathWithoutPregnancy").FirstOrDefault();
                        if (pregComp != null && pregComp.Value != null)
                        {
                            pregnancy = (bool)((FhirBoolean)pregComp.Value).Value;
                        }
                        if (position != null && icd10code != null)
                        {
                            rac.Add((Position: (int)position, Code: icd10code, Pregnancy: pregnancy));
                        }
                    }
                }
                return rac.OrderBy(entry => entry.Position);
            }
            set
            {
                // clear all existing eac
                Bundle.Entry.RemoveAll(entry => entry.Resource is Observation && (((Observation)entry.Resource).Code.Coding.First().Code == "80357-7"));
                if (RecordAxisCauseOfDeathObsList != null)
                {
                    RecordAxisCauseOfDeathObsList.Clear();
                }
                else
                {
                    RecordAxisCauseOfDeathObsList = new List<Observation>();
                }
                // Rebuild the list of observations
                foreach ((int Position, string Code, bool Pregnancy) rac in value)
                {
                    if(!String.IsNullOrEmpty(rac.Code))
                    {
                        Observation ob = new Observation();
                        ob.Id = Guid.NewGuid().ToString();
                        ob.Meta = new Meta();
                        string[] recordAxis_profile = { ProfileURL.RecordAxisCauseOfDeath };
                        ob.Meta.Profile = recordAxis_profile;
                        ob.Status = ObservationStatus.Final;
                        ob.Code = new CodeableConcept(CodeSystems.LOINC, "80357-7", "Cause of death record axis code [Automated]", null);
                        ob.Subject = new ResourceReference("urn:uuid:" + Decedent.Id);
                        AddReferenceToComposition(ob.Id, "CodedContent");
                        ob.Effective = new FhirDateTime();
                        ob.Value = new CodeableConcept(CodeSystems.ICD10, rac.Code, null, null);
                        Observation.ComponentComponent positionComp = new Observation.ComponentComponent();
                        positionComp.Value = new Integer(rac.Position);
                        positionComp.Code = new CodeableConcept(CodeSystems.Component, "position", "Position", null);
                        ob.Component.Add(positionComp);

                        // Record axis codes have an unusual and obscure handling of a Pregnancy flag, for more information see
                        // http://build.fhir.org/ig/HL7/vrdr/branches/master/StructureDefinition-vrdr-record-axis-cause-of-death.html#usage
                        if (rac.Pregnancy)
                        {
                            Observation.ComponentComponent pregComp = new Observation.ComponentComponent();
                            pregComp.Value = new FhirBoolean(true);
                            pregComp.Code = new CodeableConcept(CodeSystems.Component, "wouldBeUnderlyingCauseOfDeathWithoutPregnancy", "Would be underlying cause of death without pregnancy, if true");
                            ob.Component.Add(pregComp);
                        }

                        Bundle.AddResourceEntry(ob, "urn:uuid:" + ob.Id);
                        RecordAxisCauseOfDeathObsList.Add(ob);
                    }
                }
            }
        }


        /// <summary>The year NCHS received the death record.</summary>
        /// <value>year, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.ReceiptYear = 2022 </para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Receipt Year: {ExampleDeathRecord.ReceiptYear}");</para>
        /// </example>
        [Property("ReceiptYear", Property.Types.Int32, "Coded Content", "Coding Status", true, ProfileURL.CodingStatusValues, true)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public int? ReceiptYear
        {
            get
            {
                Date date = CodingStatusValues?.GetSingleValue<Date>("receiptDate");
                return GetDateFragmentOrPartialDate(date, VR.ExtensionURL.PartialDateYearVR);
            }
            set
            {
                if (CodingStatusValues == null)
                {
                    CreateCodingStatusValues();
                }
                Date date = CodingStatusValues?.GetSingleValue<Date>("receiptDate");
                SetPartialDate(date.Extension.Find(ext => ext.Url == VR.ExtensionURL.PartialDate), VR.ExtensionURL.PartialDateYearVR, value);
            }
        }

        /// <summary>
        /// The month NCHS received the death record.
        /// </summary>
        /// <summary>The month NCHS received the death record.</summary>
        /// <value>month, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.ReceiptMonth = 11 </para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Receipt Month: {ExampleDeathRecord.ReceiptMonth}");</para>
        /// </example>
        [Property("ReceiptMonth", Property.Types.Int32, "Coded Content", "Coding Status", true, ProfileURL.CodingStatusValues, true)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public int? ReceiptMonth
        {
            get
            {
                Date date = CodingStatusValues?.GetSingleValue<Date>("receiptDate");
                return GetDateFragmentOrPartialDate(date, VR.ExtensionURL.PartialDateMonthVR);
            }
            set
            {
                if (CodingStatusValues == null)
                {
                    CreateCodingStatusValues();
                }
                Date date = CodingStatusValues?.GetSingleValue<Date>("receiptDate");
                SetPartialDate(date.Extension.Find(ext => ext.Url == VR.ExtensionURL.PartialDate), VR.ExtensionURL.PartialDateMonthVR, value);
            }
        }

        /// <summary>The day NCHS received the death record.</summary>
        /// <value>day, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.ReceiptDay = 13 </para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Receipt Day: {ExampleDeathRecord.ReceiptDay}");</para>
        /// </example>
        [Property("ReceiptDay", Property.Types.Int32, "Coded Content", "Coding Status", true, ProfileURL.CodingStatusValues, true)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public int? ReceiptDay
        {
            get
            {
                Date date = CodingStatusValues?.GetSingleValue<Date>("receiptDate");
                return GetDateFragmentOrPartialDate(date, VR.ExtensionURL.PartialDateDayVR);
            }
            set
            {
                if (CodingStatusValues == null)
                {
                    CreateCodingStatusValues();
                }
                Date date = CodingStatusValues?.GetSingleValue<Date>("receiptDate");
                SetPartialDate(date.Extension.Find(ext => ext.Url == VR.ExtensionURL.PartialDate), VR.ExtensionURL.PartialDateDayVR, value);
            }
        }

        /// <summary>Receipt Date.</summary>
        /// <value>receipt date</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.ReceiptDate = "2018-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Receipt Date: {ExampleDeathRecord.ReceiptDate}");</para>
        /// </example>
        [Property("Receipt Date", Property.Types.StringDateTime, "Coded Content", "Receipt Date.", true, ProfileURL.CodingStatusValues, true, 25)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public string ReceiptDate
        {
            get
            {
                // We support this legacy-style API entrypoint via the new partial date and time entrypoints
                if (ReceiptYear != null && ReceiptYear != -1 && ReceiptMonth != null && ReceiptMonth != -1 && ReceiptDay != null && ReceiptDay != -1)
                {
                    Date result = new Date((int)ReceiptYear, (int)ReceiptMonth, (int)ReceiptDay);
                    return result.ToString();
                }
                return null;
            }
            set
            {
                // We support this legacy-style API entrypoint via the new partial date and time entrypoints
                DateTimeOffset parsedDate;
                if (DateTimeOffset.TryParse(value, out parsedDate))
                {
                    ReceiptYear = parsedDate.Year;
                    ReceiptMonth = parsedDate.Month;
                    ReceiptDay = parsedDate.Day;
                }
            }
        }

        /// <summary>
        /// Coder Status; TRX field with no IJE mapping
        /// </summary>
        /// <summary>Coder Status; TRX field with no IJE mapping</summary>
        /// <value>integer code</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.CoderStatus = 3;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Coder STatus {ExampleDeathRecord.CoderStatus}");</para>
        /// </example>
        [Property("CoderStatus", Property.Types.Int32, "Coded Content", "Coding Status", true, ProfileURL.CodingStatusValues, false)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public int? CoderStatus
        {
            get
            {
                return this.CodingStatusValues?.GetSingleValue<Integer>("coderStatus")?.Value;
            }
            set
            {
                if (CodingStatusValues == null)
                {
                    CreateCodingStatusValues();
                }
                CodingStatusValues.Remove("coderStatus");
                if (value != null)
                {
                    CodingStatusValues.Add("coderStatus", new Integer(value));
                }
            }
        }

        /// <summary>
        /// Shipment Number; TRX field with no IJE mapping
        /// </summary>
        /// <summary>Coder Status; TRX field with no IJE mapping</summary>
        /// <value>string</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.ShipmentNumber = "abc123"";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Shipment Number{ExampleDeathRecord.ShipmentNumber}");</para>
        /// </example>
        [Property("ShipmentNumber", Property.Types.String, "Coded Content", "Coding Status", true, ProfileURL.CodingStatusValues, false)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public string ShipmentNumber
        {
            get
            {
                return this.CodingStatusValues?.GetSingleValue<FhirString>("shipmentNumber")?.Value;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                if (CodingStatusValues == null)
                {
                    CreateCodingStatusValues();
                }
                CodingStatusValues.Remove("shipmentNumber");
                CodingStatusValues.Add("shipmentNumber", new FhirString(value));
            }
        }
        /// <summary>
        /// Intentional Reject
        /// </summary>
        /// <summary>Intentional Reject</summary>
        /// <value>string</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; reject = new Dictionary&lt;string, string&gt;();</para>
        /// <para>format.Add("code", ValueSets.FilingFormat.electronic);</para>
        /// <para>format.Add("system", CodeSystems.IntentionalReject);</para>
        /// <para>format.Add("display", "Reject1");</para>
        /// <para>ExampleDeathRecord.IntentionalReject = "reject";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Intentional Reject {ExampleDeathRecord.IntentionalReject}");</para>
        /// </example>
        [Property("IntentionalReject", Property.Types.Dictionary, "Coded Content", "Coding Status", true, ProfileURL.CodingStatusValues, true)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public Dictionary<string, string> IntentionalReject
        {
            get
            {
                CodeableConcept intentionalReject = this.CodingStatusValues?.GetSingleValue<CodeableConcept>("intentionalReject");
                if (intentionalReject != null)
                {
                    return CodeableConceptToDict(intentionalReject);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (CodingStatusValues == null)
                {
                    CreateCodingStatusValues();
                }
                CodingStatusValues.Remove("intentionalReject");
                if (value != null)
                {
                    CodingStatusValues.Add("intentionalReject", DictToCodeableConcept(value));
                }
            }
        }

        /// <summary>Intentional Reject Helper.</summary>
        /// <value>Intentional Reject
        /// <para>"code" - the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.IntentionalRejectHelper = ValueSets.IntentionalReject.Not_Rejected;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Intentional Reject Code: {ExampleDeathRecord.IntentionalRejectHelper}");</para>
        /// </example>
        [Property("IntentionalRejectHelper", Property.Types.String, "Intentional Reject Codes", "IntentionalRejectCodes.", false, ProfileURL.CodingStatusValues, true, 4)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public string IntentionalRejectHelper
        {
            get
            {
                if (IntentionalReject.ContainsKey("code") && !String.IsNullOrWhiteSpace(IntentionalReject["code"]))
                {
                    return IntentionalReject["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("IntentionalReject", value, VRDR.ValueSets.IntentionalReject.Codes);
                }
            }
        }

        /// <summary>Acme System Reject.</summary>
        /// <value>
        /// <para>"code" - the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; reject = new Dictionary&lt;string, string&gt;();</para>
        /// <para>format.Add("code", ValueSets.FilingFormat.electronic);</para>
        /// <para>format.Add("system", CodeSystems.SystemReject);</para>
        /// <para>format.Add("display", "3");</para>
        /// <para>ExampleDeathRecord.AcmeSystemReject = reject;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Acme System Reject Code: {ExampleDeathRecord.AcmeSystemReject}");</para>
        /// </example>

        [Property("AcmeSystemReject", Property.Types.Dictionary, "Coded Content", "Coding Status", true, ProfileURL.CodingStatusValues, true)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public Dictionary<string, string> AcmeSystemReject
        {
            get
            {
                CodeableConcept acmeSystemReject = this.CodingStatusValues?.GetSingleValue<CodeableConcept>("acmeSystemReject");
                if (acmeSystemReject != null)
                {
                    return CodeableConceptToDict(acmeSystemReject);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (CodingStatusValues == null)
                {
                    CreateCodingStatusValues();
                }
                CodingStatusValues.Remove("acmeSystemReject");
                if (value != null)
                {
                    CodingStatusValues.Add("acmeSystemReject", DictToCodeableConcept(value));
                }
            }
        }

        /// <summary>Acme System Reject.</summary>
        /// <value>
        /// <para>"code" - the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.AcmeSystemRejectHelper = "3";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Acme System Reject Code: {ExampleDeathRecord.AcmeSystemReject}");</para>
        /// </example>
        [Property("AcmeSystemRejectHelper", Property.Types.String, "Acme System Reject Codes", "AcmeSystemRejectCodes.", false, ProfileURL.CodingStatusValues, true, 4)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public string AcmeSystemRejectHelper
        {
            get
            {
                if (AcmeSystemReject.ContainsKey("code") && !String.IsNullOrWhiteSpace(AcmeSystemReject["code"]))
                {
                    return AcmeSystemReject["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("AcmeSystemReject", value, VRDR.ValueSets.SystemReject.Codes);
                }
            }
        }


        /// <summary>Transax Conversion Flag</summary>
        /// <value>
        /// <para>"code" - the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; tcf = new Dictionary&lt;string, string&gt;();</para>
        /// <para>tcf.Add("code", "3");</para>
        /// <para>tcf.Add("system", CodeSystems.TransaxConversion);</para>
        /// <para>tcf.Add("display", "Conversion using non-ambivalent table entries");</para>
        /// <para>ExampleDeathRecord.TransaxConversion = tcf;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Transax Conversion Code: {ExampleDeathRecord.TransaxConversion}");</para>
        /// </example>
        [Property("TransaxConversion", Property.Types.Dictionary, "Coded Content", "Coding Status", true, ProfileURL.CodingStatusValues, true)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public Dictionary<string, string> TransaxConversion
        {
            get
            {
                CodeableConcept transaxConversion = this.CodingStatusValues?.GetSingleValue<CodeableConcept>("transaxConversion");
                if (transaxConversion != null)
                {
                    return CodeableConceptToDict(transaxConversion);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (CodingStatusValues == null)
                {
                    CreateCodingStatusValues();
                }
                CodingStatusValues.Remove("transaxConversion");
                if (value != null)
                {
                    CodingStatusValues.Add("transaxConversion", DictToCodeableConcept(value));
                }
            }
        }

        /// <summary>TransaxConversion Helper.</summary>
        /// <value>transax conversion code
        /// <para>"code" - the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleDeathRecord.TransaxConversionHelper = ValueSets.TransaxConversion.Conversion_Using_Non_Ambivalent_Table_Entries;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Filing Format: {ExampleDeathRecord.TransaxConversionHelper}");</para>
        /// </example>
        [Property("TransaxConversionFlag Helper", Property.Types.String, "Transax Conversion", "TransaxConversion Flag.", false, ProfileURL.CodingStatusValues, true, 4)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code=codingstatus)", "")]
        public string TransaxConversionHelper
        {
            get
            {
                if (TransaxConversion.ContainsKey("code") && !String.IsNullOrWhiteSpace(TransaxConversion["code"]))
                {
                    return TransaxConversion["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("TransaxConversion", value, VRDR.ValueSets.TransaxConversion.Codes);
                }
            }
        }

    }
}
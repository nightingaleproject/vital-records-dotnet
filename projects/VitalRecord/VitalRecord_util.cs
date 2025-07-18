// VitalRecord_util.cs
//    Contains utility methods used across the VitalRecords class.

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.FhirPath;
using Hl7.Fhir.Utility;
using Newtonsoft.Json;
using System.Reflection;
using Hl7.Fhir.Serialization;
using System.Xml.Linq;
using System.Globalization;
using static Hl7.Fhir.Model.Encounter;

namespace VR
{
    /// <summary>Class <c>VitalRecord</c> is a base class for FHIR Vital Records
    /// including Death Reporting (VRDR) and Birth and Fetal Death Reporting (BFDR)
    /// and is designed to help produce and consume records.</summary>
    public abstract partial class VitalRecord
    {

        /// <summary>Useful for navigating around the FHIR Bundle using FHIRPaths.</summary>
        protected ITypedElement Navigator;


        /// <summary>Get the first thing in the bundle that matches the supplied FHIR path.</summary>
        /// <param name="fhirPath">an instance of the FHIRPath attribute containing the FHIR path</param>
        /// <returns>the first matching resource or element</returns>
        private ITypedElement GetFirstEntryFor(FHIRPath fhirPath)
        {
            IEnumerable<ITypedElement> matches = Bundle.ToTypedElement().Select(fhirPath.Path);
            if (matches.Count() == 0)
            {
                return null;
            }
            return matches.First();
        }

        /// <summary>Get the FHIRPath attribute for the supplied property name.</summary>
        /// <param name="propertyName">the name of the property, assumed to be in the current class</param>
        /// <returns>the attribute or null if not found</returns>
        protected FHIRPath GetFHIRPathAttribute([CallerMemberName] string propertyName = null)
        {
            return this.GetType().GetProperty(propertyName).GetCustomAttributes(false).OfType<FHIRPath>().First();
        }

        /// <summary>Check whether the bundle contains a matching entry based on the information in the FHIRPath attribute of the calling property.</summary>
        /// <param name="propertyName">the name of the C# property, must have a FHIRPath annotation, will default to the name of the calling property</param>
        /// <returns>true if the bundle contains a matching entry, false otherwise</returns>
        protected bool EntryExists([CallerMemberName] string propertyName = null)
        {
            FHIRPath fhirPath = GetFHIRPathAttribute(propertyName);
            return GetFirstEntryFor(fhirPath) != null;
        }

        /// <summary>SNOMED code for none-of-the-above observation values</summary>
        protected const string NONE_OF_THE_ABOVE = "260413007";

        /// <summary>SNOMED code for unknown observation values</summary>
        protected const string UNKNOWN = "261665006";

        /// <summary>Update the bundle entry that matches the information in the FHIRPath attribute of the calling property.</summary>
        /// <param name="shouldExist">if true the entry will be created unless it already exists, if false the entry will be removed if it exists</param>
        /// <param name="propertyName">the name of the C# property, must have a FHIRPath annotation, will default to the name of the calling property</param>
        protected void UpdateEntry(bool shouldExist, [CallerMemberName] string propertyName = null)
        {
            FHIRPath fhirPath = GetFHIRPathAttribute(propertyName);
            if (fhirPath.FHIRType == null)
            {
                throw new ArgumentException("Invalid FHIRPath attribute, fhirType attribute must be specified to use UpdateEntry");
            }
            if (GetFirstEntryFor(fhirPath) != null)
            {
                if (shouldExist)
                {
                    return; // Entry already exists
                }
                else
                {
                    RemoveEntry(fhirPath);
                }
            }
            else
            {
                if (shouldExist)
                {
                    _ = CreateEntry(fhirPath, SubjectId(propertyName));
                }
            }
        }

        /// <summary>
        /// Test if there are any entries in the same category as the specified attribute. This can include "none of the above" entries.
        /// </summary>
        /// <param name="propertyName">The name of the FHIR attribute whose category will be checked</param>
        /// <returns>true if there is at least one entry in the checked category, false if there are no entries in the checked category</returns>
        /// <exception cref="ArgumentException">thrown if propertyName does not specify a property with a FHIRPath attribute</exception>
        public bool IsCategoryEmpty(string propertyName)
        {
            FHIRPath fhirPath = GetFHIRPathAttribute(propertyName);
            if (fhirPath.FHIRType == null)
            {
                throw new ArgumentException("Invalid FHIRPath attribute, fhirType attribute must be specified to use UpdateEntry");
            }
            Func<Bundle.EntryComponent, bool> conditionCriteria = e =>
                e.Resource.TypeName == "Condition" &&
                ((Condition)e.Resource).Category.Any(c => c.Coding[0].Code == fhirPath.CategoryCode);
            Func<Bundle.EntryComponent, bool> observationCriteria = e =>
                e.Resource.TypeName == "Observation" &&
                ((Observation)e.Resource).Code.Coding[0].Code == fhirPath.CategoryCode &&
                ((Observation)e.Resource).Value as CodeableConcept != null;
            Func<Bundle.EntryComponent, bool> procedureCriteria = e =>
                e.Resource.TypeName == "Procedure" &&
                ((Procedure)e.Resource).Category.Coding[0].Code == fhirPath.CategoryCode;
            Func<Bundle.EntryComponent, bool>[] all = { conditionCriteria, observationCriteria, procedureCriteria };
            foreach (var criteria in all)
            {
                if (Bundle.Entry.Any(criteria))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>Helper to support vital record property getter helper methods for values stored in Observations.</summary>
        /// <param name="code">the code to identify the type of Observation</param>
        protected Observation GetObservation(string code)
        {
            var entry = Bundle.Entry.Where(e => e.Resource is Observation obs && CodeableConceptToDict(obs.Code)["code"] == code).FirstOrDefault();
            if (entry != null)
            {
                Observation observation = (Observation)entry.Resource;
                return observation;
            }
            return null;
        }

        /// <summary>
        /// Helper to support retrieval of matching Observations in cases where there can be multiple matches.
        /// </summary>
        /// <param name="code">The code to identify the type of Observation.</param>
        /// <returns>A list of matching Observations, or an empty list if no matches are found.</returns>
        protected List<Observation> GetObservations(string code)
        {
            return Bundle.Entry.Where(e => e.Resource is Observation obs && CodeableConceptToDict(obs.Code)["code"] == code)
                               .Select(e => (Observation)e.Resource)
                               .ToList();
        }

        /// <summary>Helper to support vital record property getter helper methods for removing Observations.</summary>
        /// <param name="code">the code to identify the type of Observation</param>
        protected void RemoveObservation(string code)
        {
            Bundle.Entry.RemoveAll(e => e.Resource is Observation obs && CodeableConceptToDict(obs.Code)["code"] == code);
        }

        /// <summary>Helper to support vital record property getter helper methods for values stored in Conditions.</summary>
        /// <param name="propertyName">the name of the C# property, must have a FHIRPath annotation, will default to the name of the calling property</param>
        protected Condition GetCondition([CallerMemberName] string propertyName = null)
        {
            FHIRPath fhirPath = GetFHIRPathAttribute(propertyName);

            // Find all conditions with this code
            var entries = Bundle.Entry.Where(e => e.Resource is Condition obs && CodeableConceptToDict(obs.Code)["code"] == fhirPath.Code);

            // Some conditions have the same code and require a category to differentiate
            if (entries != null && fhirPath.CategoryCode != null && fhirPath.CategoryCode.Length > 0)
            {
                // TODO categories have a required default category, there should be 2 in the list and we can't use 0 index by default
                var entry = entries.Where(e => e.Resource is Condition cond && cond.Category.Any(c => c.Coding[0].Code == fhirPath.CategoryCode)).FirstOrDefault();
                if (entry != null)
                {
                    Condition cond = (Condition)entry.Resource;
                    return cond;
                }
            }
            else if (entries != null)
            {
                var entry = entries.FirstOrDefault();
                if (entry != null)
                {
                    Condition cond = (Condition)entry.Resource;
                    return cond;
                }
            }

            return null;

        }

        /// <summary>Helper to support vital record property setter helper methods for values stored in Observations.</summary>
        /// <param name="code">the code to specify the type of Observation</param>
        /// <param name="codeSystem">the code system of the code specifying the type of Observation</param>
        /// <param name="text">the text for the code specifying the type of Observation</param>
        /// <param name="profileURL">the profile URL to include in the meta of the Observation</param>
        /// <param name="section">the section of the composition the Observation should be added to</param>
        /// <param name="focusId">the reference id of the focus of the Observation</param>
        /// <param name="propertyName">the name of the C# property, used to determine the subject ID</param>
        protected Observation GetOrCreateObservation(string code, string codeSystem, string text, string profileURL, string section, string focusId = null, [CallerMemberName] string propertyName = null)
        {
            if (!profileURL.Contains("http"))
            {
                throw new ArgumentException("Given profile url " + profileURL + " that does not match a url format.");
            }
            var entry = Bundle.Entry.Where(e => e.Resource is Observation obs && CodeableConceptToDict(obs.Code)["code"] == code).FirstOrDefault();
            Observation observation;
            // If the observation is there we use it, otherwise create it
            if (entry != null)
            {
                observation = (Observation)entry.Resource;
            }
            else
            {
                observation = new Observation();
                observation.Id = Guid.NewGuid().ToString();
                observation.Meta = new Meta();
                string[] profile = { profileURL };
                observation.Meta.Profile = profile;
                observation.Code = new CodeableConcept(codeSystem, code, text, null);
                if (SubjectId(propertyName) != null)
                {
                    observation.Subject = new ResourceReference($"urn:uuid:{SubjectId(propertyName)}");
                }
                // Note: The IG supports observations that can have non-final status, but they are not used
                // for NCHS data submission at this time
                observation.Status = ObservationStatus.Final;
                if (focusId != null)
                {
                    observation.Focus.Add(new ResourceReference($"urn:uuid:{focusId}"));
                }
                AddReferenceToComposition(observation.Id, section);
                Bundle.AddResourceEntry(observation, "urn:uuid:" + observation.Id);
            }
            return observation;
        }

        // TODO: How can we make this flexible to support more types?
        /// <summary>Helper to support vital record property getter helper methods for values stored in Observations.</summary>
        /// <param name="code">the code to identify the type of Observation</param>
        /// <param name="extensionURL">if present, specifies that the value should be get from an extension with the provided URL instead</param>
        protected Dictionary<string, string> GetObservationValue(string code, string extensionURL = null)
        {
            Observation observation = GetObservation(code);
            if (observation != null)
            {
                if (extensionURL != null)
                {
                    Extension extension = observation?.Value?.Extension.FirstOrDefault(ext => ext.Url == extensionURL);
                    if (extension != null && extension.Value != null && extension.Value.GetType() == typeof(CodeableConcept))
                    {
                        return CodeableConceptToDict((CodeableConcept)extension.Value);
                    }
                }
                else
                {
                    return CodeableConceptToDict((CodeableConcept)observation.Value);
                }
            }
            return EmptyCodeableDict();
        }

        /// <summary>Helper to support vital record property setter helper methods for values stored in Observations.</summary>
        /// <param name="value">the coded value to set as the value of the property</param>
        /// <param name="code">the code to specify the type of Observation</param>
        /// <param name="codeSystem">the code system of the code specifying the type of Observation</param>
        /// <param name="text">the text for the code specifying the type of Observation</param>
        /// <param name="profileURL">the profile URL to include in the meta of the Observation</param>
        /// <param name="section">the section of the composition the Observation should be added to</param>
        /// <param name="extensionURL">if present, specifies that the value should be set on an extension with the provided URL instead</param>
        /// <param name="focusId">the reference id of the focus of the Observation</param>
        /// <param name="propertyName">the name of the C# property, used to determine the subject ID</param>
        protected void SetObservationValue(Dictionary<string, string> value, string code, string codeSystem, string text, string profileURL, string section, string extensionURL = null, string focusId = null, [CallerMemberName] string propertyName = null)
        {
            Observation observation = GetOrCreateObservation(code, codeSystem, text, profileURL, section, focusId, propertyName);

            // Set the value or the extension, depending on what's desired
            if (extensionURL != null)
            {
                // If there's a value clear this extension in case it's previously set, otherwise set an empty value
                if (observation.Value == null)
                {
                    observation.Value = new CodeableConcept();
                }
                else
                {
                    observation.Value.Extension.RemoveAll(ext => ext.Url == extensionURL);
                }
                Extension extension = new Extension(extensionURL, DictToCodeableConcept(value));
                observation.Value.Extension.Add(extension);
            }
            else
            {
                // Need to keep any existing extension that could be there
                List<Extension> extensions = observation.Value?.Extension?.FindAll(e => true);
                observation.Value = DictToCodeableConcept(value);
                if (extensions != null)
                {
                    observation.Value.Extension.AddRange(extensions);
                }
            }
        }

        /// <summary>Helper to support vital record property getter helper methods for getting and setting coded values.</summary>
        /// <param name="propertyName">the name of the C# helper property, used to determine which underlying property to call</param>
        /// <example>
        /// <para>// Given a property named MotherEducationLevel, the getter for MotherEducationLevelHelper can be defined using:</para>
        /// <para>public string MotherEducationLevelHelper</para>
        /// <para>{</para>
        /// <para>    get => GetObservationValueHelper();</para>
        /// <para>}</para>
        /// </example>
        protected string GetObservationValueHelper([CallerMemberName] string propertyName = null)
        {
            // Find the base property name by stripping off the "Helper" from the calling property
            if (!propertyName.EndsWith("Helper"))
            {
                throw new ArgumentException("GetObservationValueHelper called with a non-helper property");
            }
            string basePropertyName = propertyName.Replace("Helper", "");
            Dictionary<string, string> value = (Dictionary<string, string>)this.GetType().GetProperty(basePropertyName).GetValue(this);
            if (value.ContainsKey("code") && !string.IsNullOrWhiteSpace(value["code"]))
            {
                return value["code"];
            }
            return null;
        }

        /// <summary>Helper to support vital record property setter helper methods for getting and setting coded values.</summary>
        /// <param name="value">the coded value to set as the code in the underlying property value</param>
        /// <param name="codes">the list of allowed codes</param>
        /// <param name="propertyName">the name of the C# helper property, used to determine which underlying property to call</param>
        /// <example>
        /// <para>// Given a property named MotherEducationLevel, the setter for MotherEducationLevelHelper can be defined using:</para>
        /// <para>public string MotherEducationLevelHelper</para>
        /// <para>{</para>
        /// <para>    set => SetObservationValueHelper(value, VR.ValueSets.EducationLevel.Codes);</para>
        /// <para>}</para>
        /// </example>
        protected void SetObservationValueHelper(string value, string[,] codes, [CallerMemberName] string propertyName = null)
        {
            // Find the base property name by stripping off the "Helper" from the calling property
            // Find the base property name by stripping off the "Helper" from the calling property
            if (!propertyName.EndsWith("Helper"))
            {
                throw new ArgumentException("GetObservationValueHelper called with a non-helper property");
            }
            if (!string.IsNullOrWhiteSpace(value))
            {
                string basePropertyName = propertyName.Replace("Helper", "");
                SetCodeValue(basePropertyName, value, codes);
            }
        }

        /// <summary>Remove all of the entries for the supplied category</summary>
        /// <param name="fhirPath">the FHIRPath of a none-of-the-above entry</param>
        protected void RemoveAllEntries(FHIRPath fhirPath)
        {
            Func<Bundle.EntryComponent, bool> conditionCriteria = e =>
                e.Resource.TypeName == "Condition" &&
                ((Condition)e.Resource).Category.Any(c => c.Coding[0].Code == fhirPath.CategoryCode);
            Func<Bundle.EntryComponent, bool> observationCriteria = e =>
                e.Resource.TypeName == "Observation" &&
                ((Observation)e.Resource).Code.Coding[0].Code == fhirPath.CategoryCode &&
                ((Observation)e.Resource).Value as CodeableConcept != null &&
                (((Observation)e.Resource).Value as CodeableConcept).Coding[0].Code != NONE_OF_THE_ABOVE &&
                (((Observation)e.Resource).Value as CodeableConcept).Coding[0].Code != UNKNOWN;
            Func<Bundle.EntryComponent, bool> procedureCriteria = e =>
                e.Resource.TypeName == "Procedure" &&
                ((Procedure)e.Resource).Category.Coding[0].Code == fhirPath.CategoryCode;
            Func<Bundle.EntryComponent, bool>[] all = { conditionCriteria, observationCriteria, procedureCriteria };
            foreach (var criteria in all)
            {
                foreach (var entry in Bundle.Entry.Where(criteria))
                {
                    RemoveReferenceFromComposition(entry.FullUrl, fhirPath.Section);
                }
                Bundle.Entry.RemoveAll(new Predicate<Bundle.EntryComponent>(criteria));
            }
        }

        private void RemoveEntry(FHIRPath fhirPath)
        {
            Func<Bundle.EntryComponent, bool> func;
            switch (fhirPath.FHIRType)
            {
                case FHIRPath.FhirType.Condition:
                    func = e => e.Resource.TypeName == fhirPath.FHIRType.ToString() && ((Condition)e.Resource).Code.Coding[0].Code == fhirPath.Code;
                    break;
                case FHIRPath.FhirType.Observation:
                    func = e => e.Resource.TypeName == fhirPath.FHIRType.ToString() && (((Observation)e.Resource).Value as CodeableConcept != null)
                                                                                    && (((Observation)e.Resource).Value as CodeableConcept).Coding.Count > 0
                                                                                    && (((Observation)e.Resource).Value as CodeableConcept).Coding[0].Code == fhirPath.Code
                                                                                    && (((Observation)e.Resource).Code as CodeableConcept).Coding.Count > 0
                                                                                    && (((Observation)e.Resource).Code as CodeableConcept).Coding[0].Code == fhirPath.CategoryCode;
                    break;
                case FHIRPath.FhirType.Procedure:
                    func = e => e.Resource.TypeName == fhirPath.FHIRType.ToString() && ((Procedure)e.Resource).Code.Coding[0].Code == fhirPath.Code;
                    break;
                case FHIRPath.FhirType.Coverage:
                    func = e => e.Resource.TypeName == fhirPath.FHIRType.ToString() && ((Coverage)e.Resource).Type.Coding[0].Code == fhirPath.Code;
                    break;
                default:
                    throw new ArgumentException("Invalid FHIRPath attribute, fhirType attribute must be one of Condition, Observation, Procedure, or Coverage");
            }
            foreach (var entry in Bundle.Entry.Where(func))
            {
                RemoveReferenceFromComposition(entry.FullUrl, fhirPath.Section);
            }
            Bundle.Entry.RemoveAll(new Predicate<Bundle.EntryComponent>(func));
        }

        /// <summary>Create a bundle entry using the properties of the supplied FHIRPath and subject ID</summary>
        /// <param name="fhirPath"></param>
        /// <param name="subjectId"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>the newly created resource</returns>
        protected Resource CreateEntry(FHIRPath fhirPath, string subjectId)
        {
            Resource resource;
            CodeableConcept code = null;
            if (fhirPath.Code != null && fhirPath.Code.Length > 0)
            {
                string codeSystem = fhirPath.CodeSystem ?? CodeSystems.SCT;
                code = new CodeableConcept(codeSystem, fhirPath.Code, fhirPath.Display, null);
            }
            CodeableConcept category = null, usCoreCategory = null;
            if (fhirPath.CategoryCode != null && fhirPath.CategoryCode.Length > 0)
            {
                category = new CodeableConcept(CodeSystems.LOINC, fhirPath.CategoryCode);
                if (fhirPath.CategoryCode == "76061-1" || fhirPath.CategoryCode == "76060-3")
                {
                    // BFDR profiles ConditionFetalDeathInitiatingCauseOrCondition and
                    // ConditionFetalDeathOtherCauseOrCondition are based on
                    // USCoreConditionEncounterDiagnosisProfile which require an additional
                    // category of 'encounter-diagnosis' for conformance.
                    usCoreCategory = new CodeableConcept(CodeSystems.ConditionCategory, "encounter-diagnosis");
                }
                else
                {
                    // The remaining Conditions in the Vital Records IGs, all in BFDR for now, are based on
                    // USCoreConditionProblemsHealthConcernsProfile which requires an additional category of
                    // 'problem-list-item' for conformance.
                    // If additional Conditions are supported by Vital Records IGs in the future, additional cases
                    // would need to be defined here to ensure they get an appropriate USCore-defined category.
                    usCoreCategory = new CodeableConcept(CodeSystems.ConditionCategory, "problem-list-item");
                }
            }
            switch (fhirPath.FHIRType)
            {
                case FHIRPath.FhirType.Condition:
                    Condition condition = new Condition();
                    condition.Code = code;
                    condition.Category.Add(usCoreCategory);
                    condition.Category.Add(category);
                    condition.Subject = new ResourceReference($"urn:uuid:{subjectId}");
                    resource = condition;
                    break;
                case FHIRPath.FhirType.Observation:
                    Observation observation = new Observation();
                    observation.Code = category;
                    observation.Value = code;
                    observation.Subject = new ResourceReference($"urn:uuid:{subjectId}");
                    resource = observation;
                    break;
                case FHIRPath.FhirType.Procedure:
                    Procedure procedure = new Procedure();
                    procedure.Code = code;
                    procedure.Category = category;
                    procedure.Subject = new ResourceReference($"urn:uuid:{subjectId}");
                    resource = procedure;
                    break;
                case FHIRPath.FhirType.Coverage:
                    Coverage coverage = new Coverage();
                    coverage.Type = code;
                    resource = coverage;
                    break;
                default:
                    throw new ArgumentException("Invalid FHIRPath attribute, fhirType attribute must be one of Condition, Observation, Procedure, or Coverage");
            }

            resource.Id = Guid.NewGuid().ToString();
            AddReferenceToComposition(resource.Id, fhirPath.Section, subjectId);
            Bundle.AddResourceEntry(resource, "urn:uuid:" + resource.Id);

            if (fhirPath.Code == NONE_OF_THE_ABOVE || fhirPath.Code == UNKNOWN)
            {
                // remove all of the entries related to this none-of-the-above entry
                RemoveAllEntries(fhirPath);
            }
            else
            {
                // remove the corresponding none-of-the-above or unknown entry if it exists
                FHIRPath noneOfTheAbove = new FHIRPath(FHIRPath.FhirType.Observation, fhirPath.CategoryCode, NONE_OF_THE_ABOVE, fhirPath.Section);
                RemoveEntry(noneOfTheAbove);
                FHIRPath unknown = new FHIRPath(FHIRPath.FhirType.Observation, fhirPath.CategoryCode, UNKNOWN, fhirPath.Section);
                RemoveEntry(unknown);
            }

            return resource;
        }

        /// <summary>Get the FHIR Bundle entry id for the subject of the supplied property.</summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>the FHIR Bundle entry id</returns>
        protected virtual string SubjectId(string propertyName)
        {
            return null;
        }

        /// <summary>Helper method to create a HumanName from a list of strings.</summary>
        /// <param name="value">A list of strings to be converted into a name.</param>
        /// <param name="names">The current list of HumanName attributes for the person.</param>
        /// <param name="use"> The type of name, defaults to official.</param>
        public static void updateGivenHumanName(string[] value, List<HumanName> names, HumanName.NameUse use = HumanName.NameUse.Official)
        {
            // Remove any blank or null values.
            value = value.Where(v => !String.IsNullOrEmpty(v)).ToArray();
            // Set names only if there are non-blank values.
            if (value.Length < 1)
            {
                return;
            }
            HumanName name = names.SingleOrDefault(n => n.Use == use);
            if (name != null)
            {
                name.Given = value;
            }
            else
            {
                name = new HumanName();
                name.Use = use;
                name.Given = value;
                names.Add(name);
            }
        }

        /// <summary>Helper method to update last name.</summary>
        /// <param name="value">A list of strings to be converted into a name.</param>
        /// <param name="names">The current list of HumanName attributes for the person.</param>
        /// <param name="use"> The type of name, defaults to official.</param>
        /// <param name="propertyName">The name of the Natality record property</param>
        public static void updateFamilyName(string value, List<HumanName> names, HumanName.NameUse use = HumanName.NameUse.Official, [CallerMemberName] string propertyName = null)
        {
            HumanName name = names.SingleOrDefault(n => n.Use == HumanName.NameUse.Official);
            if (name != null && !String.IsNullOrEmpty(value))
            {
                name.Family = value;
            }
            else if (String.IsNullOrEmpty(value))
            {
                Extension missingValueReason = new Extension(OtherExtensionURL.DataAbsentReason, new Code(propertyName == "ChildFamilyName" ? "temp-unknown" : "unknown")); //if child, use temp-unknown, otherwise default to "unknown"
                if (name == null)
                {
                    name = new HumanName
                    {
                        Use = HumanName.NameUse.Official,
                        Family = value
                    };
                }
                else
                {
                    name.Family = value;
                    if (name.Family == null)
                    {
                        name.FamilyElement = new FhirString();
                    }
                    name.FamilyElement.Extension.Add(missingValueReason);
                }
            }
            else if (!String.IsNullOrEmpty(value))
            {
                name = new HumanName
                {
                    Use = HumanName.NameUse.Official,
                    Family = value
                };
                names.Add(name);
            }
        }

        /// <summary>Helper method to update suffix.</summary>
        /// <param name="value">A list of strings to be converted into a name.</param>
        /// <param name="names">The current list of HumanName attributes for the person.</param>
        /// <param name="use"> The type of name, defaults to official.</param>
        public static void updateSuffix(string value, List<HumanName> names, HumanName.NameUse use = HumanName.NameUse.Official)
        {
            if (String.IsNullOrEmpty(value))
            {
                return;
            }
            HumanName name = names.SingleOrDefault(n => n.Use == HumanName.NameUse.Official);
            if (name != null)
            {
                string[] suffix = { value };
                name.Suffix = suffix;
            }
            else
            {
                name = new HumanName();
                name.Use = HumanName.NameUse.Official;
                string[] suffix = { value };
                name.Suffix = suffix;
                names.Add(name);
            }
        }

        /// <summary>Function that validates all partial date extensions in a bundle. Currently unused due
        /// to reevaluation of partial date handling due to changes in the VitalRecords IG.</summary>
        /// <param name="bundle">The bundle in which to validate the PartialDate/Time extensions.</param>
        private void ValidatePartialDates(Bundle bundle)
        {
            System.Text.StringBuilder errors = new System.Text.StringBuilder();
            List<Resource> resources = bundle.Entry.Select(entry => entry.Resource).ToList();

            foreach (Hl7.Fhir.Model.Resource resource in resources)
            {
                foreach (DataType child in resource.Children.Where(child => child.GetType().IsSubclassOf(typeof(DataType))))
                {
                    // Extract PartialDates and PartialDateTimes.
                    //List<Extension> partialDateExtensions = child.Extension.Where(ext => ext.Url.Equals(VR.ExtensionURL.PartialDate) || ext.Url.Equals(ExtensionURL.PartialDateTimeVR)).ToList();
                    List<Extension> partialDateExtensions = child.Extension.Where(ext => ext.Url.Equals(ExtensionURL.PartialDate) || ext.Url.Equals(ExtensionURL.PartialDateTime)).ToList();
                    foreach (Extension partialDateExtension in partialDateExtensions)
                    {
                        // Validate that the required sub-extensions are in the PartialDate/Time component.
                        List<String> partialDateSubExtensions = partialDateExtension.Extension.Select(ext => ext.Url).ToList();
                        if (!partialDateSubExtensions.Contains(ExtensionURL.PartialDateDayVR))
                        {
                            errors.Append("Missing 'Date-Day' of [" + partialDateExtension.Url + "] for resource [" + resource.Id + "].").AppendLine();
                        }
                        if (!partialDateSubExtensions.Contains(VR.ExtensionURL.PartialDateMonthVR))
                        {
                            errors.Append("Missing 'Date-Month' of [" + partialDateExtension.Url + "] for resource [" + resource.Id + "].").AppendLine();
                        }
                        if (!partialDateSubExtensions.Contains(VR.ExtensionURL.PartialDateYearVR))
                        {
                            errors.Append("Missing 'Date-Year' of [" + partialDateExtension.Url + "] for resource [" + resource.Id + "].").AppendLine();
                        }
                        // Validate that there are no extraneous invalid sub-extensions of the PartialDate/Time component.
                        partialDateSubExtensions.Remove(VR.ExtensionURL.PartialDateDayVR);
                        partialDateSubExtensions.Remove(VR.ExtensionURL.PartialDateMonthVR);
                        partialDateSubExtensions.Remove(VR.ExtensionURL.PartialDateYearVR);
                        partialDateSubExtensions.Remove(VR.ExtensionURL.PartialDateTimeVR);
                        if (partialDateSubExtensions.Count() > 0)
                        {
                            errors.Append("[" + partialDateExtension.Url + "] component contains extra invalid fields [" + string.Join(", ", partialDateSubExtensions) + "] for resource [" + resource.Id + "].").AppendLine();
                        }
                    }
                }
            }
            if (errors.Length > 0)
            {
                throw new ArgumentException(errors.ToString());
            }
        }

        /// <summary>Getter helper for anything that uses PartialDateTime, allowing a particular date field (year, month, or day) to be read
        /// from the extension. Returns either a numeric date part, or -1 meaning explicitly unknown, or null meaning not specified.</summary>
        protected int? GetPartialDate(Extension partialDateTime, string partURL)
        {
            Extension part = partialDateTime?.Extension?.Find(ext => ext.Url == partURL);
            Extension dataAbsent = part?.Extension?.Find(ext => ext.Url == OtherExtensionURL.DataAbsentReason);
            // extension for absent date can be directly on the part as with year, month, day
            if (dataAbsent != null)
            {
                // The data absent reason is either a placeholder that a field hasen't been set yet (data absent reason of 'temp-unknown') or
                // a claim that there's no data (any other data absent reason, e.g., 'unknown'); return null for the former and "-1" for the latter
                string code = ((Code)dataAbsent.Value).Value;
                if (code == "temp-unknown")
                {
                    return null;
                }
                else
                {
                    return -1;
                }
            }
            // check if the part (e.g. "_valueUnsignedInt") has a data absent reason extension on the value
            Extension dataAbsentOnValue = part?.Value?.Extension?.Find(ext => ext.Url == OtherExtensionURL.DataAbsentReason);
            if (dataAbsentOnValue != null)
            {
                string code = ((Code)dataAbsentOnValue.Value).Value;
                if (code == "temp-unknown")
                {
                    return null;
                }
                else
                {
                    return -1;
                }
            }
            // If we have a value, return it
            if (part?.Value != null)
            {
                if (part.Value is Integer)
                {
                    return (int?)((Integer)part.Value).Value;
                }
                return (int?)((UnsignedInt)part.Value).Value; // Untangle a FHIR UnsignedInt in an extension into an int
            }
            // No data present at all, return null
            return null;
        }

        /// <summary>NewBlankPartialDateTimeExtension, Build a blank PartialDateTime extension (which means all the placeholder data absent
        /// reasons are present to note that the data is not in fact present). This method takes an optional flag to determine if this extension
        /// should include the time field, which is not always needed</summary>
        protected Extension NewBlankPartialDateTimeExtension(bool includeTime = true)
        {
            return NewDataAbsentReasonPartialDateTimeExtension("temp-unknown", includeTime);
        }

        private Extension NewDataAbsentReasonPartialDateTimeExtension(string dataAbsentReason, bool includeTime = true)
        {
            Extension partialDateTime = new Extension(includeTime ? ExtensionURL.PartialDateTime : ExtensionURL.PartialDate, null);
            Extension year = new Extension(VR.ExtensionURL.PartialDateYearVR, null);
            year.Extension.Add(new Extension(OtherExtensionURL.DataAbsentReason, new Code(dataAbsentReason)));
            partialDateTime.Extension.Add(year);
            Extension month = new Extension(ExtensionURL.PartialDateMonthVR, null);
            month.Extension.Add(new Extension(OtherExtensionURL.DataAbsentReason, new Code(dataAbsentReason)));
            partialDateTime.Extension.Add(month);
            Extension day = new Extension(ExtensionURL.PartialDateDayVR, null);
            day.Extension.Add(new Extension(OtherExtensionURL.DataAbsentReason, new Code(dataAbsentReason)));
            partialDateTime.Extension.Add(day);
            if (includeTime)
            {
                Extension time = new Extension(ExtensionURL.PartialDateTimeVR, null);
                time.Extension.Add(new Extension(OtherExtensionURL.DataAbsentReason, new Code(dataAbsentReason)));
                partialDateTime.Extension.Add(time);
            }
            return partialDateTime;
        }

        /// <summary>Setter helper for anything that uses PartialDateTime, allowing a particular date field (year, month, or day) to be
        /// set in the extension. Arguments are the extension to poplulate, the part of the URL to populate, and the value to specify.
        /// The value can be a positive number for an actual value, a -1 meaning that the value is explicitly unknown, or null meaning
        /// the data has not been specified.</summary>
        protected static void SetPartialDate(Extension partialDateTime, string partURL, int? value)
        {
            Extension part = partialDateTime.Extension.Find(ext => ext.Url == partURL);
            part.Extension.RemoveAll(ext => ext.Url == OtherExtensionURL.DataAbsentReason);
            if (value != null && value != -1)
            {
                part.Value = new UnsignedInt((int)value);
            }
            else
            {
                part.Value = new UnsignedInt();
                // Determine which data absent reason to use based on whether the value is unknown or -1
                part.Value.Extension.Add(new Extension(OtherExtensionURL.DataAbsentReason, new Code(value == -1 ? "unknown" : "temp-unknown")));
            }
        }

        /// <summary>Getter helper for anything that uses PartialDateTime, allowing the time to be read from the extension</summary>
        protected string GetPartialTime(Extension partialDateTime)
        {
            Extension part = partialDateTime?.Extension?.Find(ext => ext.Url == ExtensionURL.PartialDateTimeVR);
            Extension dataAbsent = part?.Extension?.Find(ext => ext.Url == OtherExtensionURL.DataAbsentReason);
            // extension for absent date can be directly on the part as with year, month, day
            if (dataAbsent != null)
            {
                // The data absent reason is either a placeholder that a field hasen't been set yet (data absent reason of 'temp-unknown') or
                // a claim that there's no data (any other data absent reason, e.g., 'unknown'); return null for the former and "-1" for the latter
                string code = ((Code)dataAbsent.Value).Value;
                if (code == "temp-unknown")
                {
                    return null;
                }
                else
                {
                    return "-1";
                }
            }
            // check if the part (e.g. "_valueTime") has a data absent reason extension on the value
            Extension dataAbsentOnValue = part?.Value?.Extension?.Find(ext => ext.Url == OtherExtensionURL.DataAbsentReason);
            if (dataAbsentOnValue != null)
            {
                string code = ((Code)dataAbsentOnValue.Value).Value;
                if (code == "temp-unknown")
                {
                    return null;
                }
                else
                {
                    return "-1";
                }
            }
            // If we have a value, return it
            if (part?.Value != null)
            {
                return part.Value.ToString();
            }
            // No data present at all, return null
            return null;
        }

        /// <summary>Setter helper for anything that uses PartialDateTime, allowing the time to be set in the extension</summary>
        protected void SetPartialTime(Extension partialDateTime, String value)
        {
            Extension part = partialDateTime.Extension.Find(ext => ext.Url == ExtensionURL.PartialDateTimeVR);
            part.Extension.RemoveAll(ext => ext.Url == OtherExtensionURL.DataAbsentReason);
            if (value != null && value != "-1")
            {
                // we need to force it to be 00:00:00 format to be compliant with the IG because the FHIR class doesn't
                if (value.Length < 8)
                {
                    value += ":";
                    value = value.PadRight(8, '0');
                }
                part.Value = new Time(value);
            }
            else
            {
                part.Value = new Time();
                // Determine which data absent reason to use based on whether the value is unknown or -1
                part.Value.Extension.Add(new Extension(OtherExtensionURL.DataAbsentReason, new Code(value == "-1" ? "unknown" : "temp-unknown")));
            }
        }

        /// <summary>Getter helper for anything that can have a regular FHIR date/time
        /// field (year, month, or day) to be read the value
        /// supports dates and date times but does NOT support extensions</summary>
        protected int? GetDateFragment(Element value, string partURL)
        {
            if (value == null)
            {
                return null;
            }
            // If we have a basic value as a valueDateTime use that, otherwise pull from the PartialDateTime extension
            if (value is FhirDateTime && ((FhirDateTime)value).Value != null)
            {
                // DateTimeOffset.Parse will insert fake information where missing,
                // so TryParseExact on the partial date info first
                if (partURL == VR.ExtensionURL.PartialDateYearVR)
                {
                    ParseDateElements(((FhirDateTime)value).Value, out int? year, out int? month, out int? day);
                    return year;
                }
                else if (partURL == VR.ExtensionURL.PartialDateMonthVR)
                {
                    ParseDateElements(((FhirDateTime)value).Value, out int? year, out int? month, out int? day);
                    return month;
                }
                else if (partURL == VR.ExtensionURL.PartialDateDayVR)
                {
                    ParseDateElements(((FhirDateTime)value).Value, out int? year, out int? month, out int? day);
                    return day;
                }
                else
                {
                    throw new ArgumentException("GetDateFragment called with unsupported PartialDateTime segment");
                }
            }
            else if (value is Date && ((Date)value).Value != null)
            {
                // Note: We can't just call ToDateTimeOffset() on the Date because want the date in whatever local time zone was provided
                // if (DateTimeOffset.TryParse(((Date)value).Value, out DateTimeOffset parsedDate))
                ParseDateElements(((Date)value).Value, out int? year, out int? month, out int? day);
                if (partURL == VR.ExtensionURL.PartialDateYearVR)
                {
                    return year;
                }
                else if (partURL == VR.ExtensionURL.PartialDateMonthVR)
                {
                    return month;
                }
                else if (partURL == VR.ExtensionURL.PartialDateDayVR)
                {
                    return day;
                }
                else
                {
                    throw new ArgumentException("GetDateFragment called with unsupported PartialDateTime segment when trying to parse individual date elements");
                }
            }
            return null;
        }

        /// <summary>Parses out the output year/month/day from the given string. Returns false if no valid FHIR date objects were extracted.
        /// A valid FHIR date object can be either: "yyyy", "yyyy-MM", or "yyyy-MM-dd </summary>
        public static bool ParseDateElements(string date, out int? year, out int? month, out int? day)
        {
            // Temporarily remove time data so we can focus on just the date elements.
            date = date?.Split('T') is string[] parts ? parts[0] : date;
            if (date != null && date.Length >= 4)
            {
                if (DateTimeOffset.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset parsedDateDay))
                {
                    year = parsedDateDay.Year;
                    month = parsedDateDay.Month;
                    day = parsedDateDay.Day;
                    return true;
                }
                else if (DateTimeOffset.TryParseExact(date, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset parsedDateMonth))
                {
                    year = parsedDateMonth.Year;
                    month = parsedDateMonth.Month;
                    day = null;
                    return true;
                }
                else if (DateTimeOffset.TryParseExact(date, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset parsedDateYear))
                {
                    year = parsedDateYear.Year;
                    month = null;
                    day = null;
                    return true;
                }
            }
            year = null;
            month = null;
            day = null;
            return false;
        }

        /// <summary>Returns a Fhir Date object parsed from the given string.</summary>
        protected static Date ConvertToDate(string date)
        {
            if (ParseDateElements(date, out int? year, out int? month, out int? day))
            {
                if (year != null && month != null && day != null)
                {
                    return new Date((int)year, (int)month, (int)day);
                }
                else if (year != null && month != null)
                {
                    return new Date((int)year, (int)month);
                }
                else if (year != null)
                {
                    return new Date((int)year);
                }
            }
            return null;
        }

        /// <summary>Returns a Fhir DateTime object parsed from the given string.</summary>
        protected static FhirDateTime ConvertToDateTime(string date)
        {
            if (ParseDateElements(date, out int? year, out int? month, out int? day))
            {
                if (year != null && month != null && day != null)
                {
                    return new FhirDateTime((int)year, (int)month, (int)day);
                }
                else if (year != null && month != null)
                {
                    return new FhirDateTime((int)year, (int)month);
                }
                else if (year != null)
                {
                    return new FhirDateTime((int)year);
                }
            }
            return null;
        }

        /// <summary>Gets the specified date element based on the ExtensionURL.PartialDate from the given
        /// FhirDate, checking in the value and PartialDate extension, and assuming there
        /// is no time data to consider.</summary>
        protected int? GetDateElement(Date birthDateElement, string partialDateURL)
        {
            if (birthDateElement == null)
            {
                return null;
            }
            string birthDate = birthDateElement.Value;
            // First check for a birth day in the birthDate string.
            if (birthDate != null && ParseDateElements(birthDate, out int? year, out int? month, out int? day))
            {
                switch (partialDateURL)
                {
                    case VR.ExtensionURL.PartialDateYearVR:
                        if (year != null) return year;
                        break;
                    case VR.ExtensionURL.PartialDateMonthVR:
                        if (month != null) return month;
                        break;
                    case VR.ExtensionURL.PartialDateDayVR:
                        if (day != null) return day;
                        break;
                    default:
                        throw new ArgumentException("Invalid PartialDateTime URL given: '" + ExtensionURL.PartialDate + "'.");
                }
            }
            // If it's not there, check for a PartialDateTime.
            return GetDateFragmentOrPartialDate(birthDateElement, partialDateURL);
        }

        /// <summary>
        /// Updates a FhirDateTime's date element based on the specified URL.
        /// </summary>
        /// <param name="dateElement"></param>
        /// <param name="value"></param>
        /// <param name="partialDateUrl"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected FhirDateTime UpdateFhirDateTimeDateElement(FhirDateTime dateElement, int? value, string partialDateUrl)
        {
            if (value == null)
            {
                return null;
            }
            ExtractBestDateElements(dateElement, out int? year, out int? month, out int? day, out _);
            // Set whichever date element we're updating to the given value.
            switch (partialDateUrl)
            {
                case VR.ExtensionURL.PartialDateYearVR:
                    year = value;
                    break;
                case VR.ExtensionURL.PartialDateMonthVR:
                    month = value;
                    break;
                case VR.ExtensionURL.PartialDateDayVR:
                    day = value;
                    break;
                default:
                    throw new Exception("Invalid partial date time URL");
            }
            return UpdateFhirDateTime(year, month, day);
        }

        private void ExtractBestDateElements(PrimitiveType date, out int? year, out int? month, out int? day, out string time)
        {
            // Get the most valid date elements, giving priority to the parsed date elements. If the partial date is used, it will include any -1 values. If there are no valid date elements in any of the possible places, it will be null.
            ParseDateElements(((IValue<string>)date).Value, out int? parsedYear, out int? parsedMonth, out int? parsedDay);
            Extension pdtExt = date.GetExtension(VR.ExtensionURL.PartialDateTime);
            year = parsedYear ?? GetPartialDate(pdtExt, VR.ExtensionURL.PartialDateYearVR);
            month = parsedMonth ?? GetPartialDate(pdtExt, VR.ExtensionURL.PartialDateMonthVR);
            day = parsedDay ?? GetPartialDate(pdtExt, VR.ExtensionURL.PartialDateDayVR);
            time = GetTimeFragment((FhirDateTime)date.GetExtension(VR.ExtensionURL.PatientBirthTime)?.Value)
                    ?? ((Time)pdtExt?.GetExtension(VR.ExtensionURL.PartialDateTimeVR)?.Value)?.Value
                    ?? GetPartialTime(date.GetExtension(VR.ExtensionURL.PartialDateTime));
            if (time == "unknown")
            {
                time = "-1";
            }
            else if (time == "temp-unknown")
            {
                time = null;
            }
        }

        /// <summary>
        /// Creates a FhirDate formatted based on PartialDateTime rules.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        private FhirDateTime UpdateFhirDateTime(int? year, int? month, int? day)
        {
            // If all the date elements are valid and known, build a complete FhirDateTime in the format yyyy-mm-dd.
            if (year != -1 && year != null && month != -1 && month != null && day != -1 && day != null)
            {
                return new FhirDateTime((int)year, (int)month, (int)day);
            }

            // If just the year and month date elements are valid and known, build a FhirDateTime in the format yyyy-mm.
            if (year != -1 && year != null && month != -1 && month != null)
            {
                FhirDateTime date = new FhirDateTime((int)year, (int)month);
                if (day == -1)
                {
                    date = SetPartialDateExtensions(date, year, month, day);
                }
                return date;
            }

            // If just the year date element is valid and known, build a FhirDateTime in the format yyyy.
            if (year != -1 && year != null)
            {
                FhirDateTime date = new FhirDateTime((int)year);
                if (day == -1 || month == -1)
                {
                    date = SetPartialDateExtensions(date, year, month, day);
                }
                return date;
            }

            // If the year is not valid or is unknown, build an empty FhirDateTime and store all date data in the partial date time extensions.
            FhirDateTime emptyDate = new FhirDateTime();
            emptyDate = SetPartialDateExtensions(emptyDate, year, month, day);
            return emptyDate;
        }

        private FhirDateTime SetPartialDateExtensions(FhirDateTime dateElement, int? yearValue, int? monthValue, int? dayValue)
        {
            dateElement.SetExtension(VR.ExtensionURL.PartialDateTime, new Extension());
            List<(int? val, string url)> dateElements = new List<(int? val, string url)>
            {
                (dayValue, VR.ExtensionURL.PartialDateDayVR),
                (monthValue, VR.ExtensionURL.PartialDateMonthVR),
                (yearValue, VR.ExtensionURL.PartialDateYearVR)
            };
            foreach ((int? val, string url) in dateElements)
            {
                switch (val)
                {
                    case -1:
                        dateElement.GetExtension(VR.ExtensionURL.PartialDateTime).Extension.Add(BuildUnknownPartialDateTime(url));
                        break;
                    case null:
                        dateElement.GetExtension(VR.ExtensionURL.PartialDateTime).Extension.Add(BuildTempUnknownPartialDateTime(url));
                        break;
                    default:
                        dateElement.GetExtension(VR.ExtensionURL.PartialDateTime).SetExtension(url, new Integer(val));
                        break;
                }
            }
            return dateElement;
        }

        private static Extension BuildTempUnknownPartialDateTime(string partialDateURL)
        {
            Extension ext = new Extension(partialDateURL, null);
            ext.SetExtension(OtherExtensionURL.DataAbsentReason, new Code("temp-unknown"));
            return ext;
        }

        private static Extension BuildUnknownPartialDateTime(string partialDateURL)
        {
            Extension ext = new Extension(partialDateURL, null);
            ext.SetExtension(OtherExtensionURL.DataAbsentReason, new Code("unknown"));
            return ext;
        }

        /// <summary>Getter helper for anything that can have a regular FHIR date/time or a PartialDateTime extension, allowing a particular date
        /// field (year, month, or day) to be read from either the value or the extension</summary>
        protected int? GetDateFragmentOrPartialDate(Element value, string partURL)
        {
            if (value == null)
            {
                return null;
            }
            var dateFragment = GetDateFragment(value, partURL);
            if (dateFragment != null)
            {
                return dateFragment;
            }
            // Look for either PartialDate or PartialDateTime
            Extension extension = value.Extension.Find(ext => ext.Url == ExtensionURL.PartialDateTime);
            if (extension == null)
            {
                extension = value.Extension.Find(ext => ext.Url == ExtensionURL.PartialDate);
            }
            return GetPartialDate(extension, partURL);
        }

        /// <summary>Extract the hour.</summary>
        protected int FhirTimeHour(Time value)
        {
            return int.Parse(value.ToString().Substring(0, 2));
        }

        /// <summary>Extract the minute.</summary>
        protected int FhirTimeMin(Time value)
        {
            return int.Parse(value.ToString().Substring(3, 2));
        }

        /// <summary>Extract the second.</summary>
        protected int FhirTimeSec(Time value)
        {
            return int.Parse(value.ToString().Substring(6, 2));
        }

        /// <summary>Convert a time stamp to a datetime stamp using the earliest allowed date.</summary>
        protected FhirDateTime ConvertFhirTimeToFhirDateTime(Time value)
        {
            return new FhirDateTime(DateTimeOffset.MinValue.Year, DateTimeOffset.MinValue.Month, DateTimeOffset.MinValue.Day,
                FhirTimeHour(value), FhirTimeMin(value), FhirTimeSec(value), TimeSpan.Zero);
        }

        /// <summary>Getter helper for anything that can have a regular FHIR date/time, allowing the time to be read from the value</summary>
        protected string GetTimeFragment(Element value)
        {
            if (value is FhirDateTime && ((FhirDateTime)value).Value != null)
            {
                string dtStr = ((FhirDateTime)value).Value;
                if (ParseDateTime(dtStr, out DateTime dateTime))
                {
                    TimeSpan timeSpan = new TimeSpan(0, dateTime.Hour, dateTime.Minute, dateTime.Second);
                    return timeSpan.ToString(@"hh\:mm\:ss");
                }
                if (ParseDateTimeOffset(dtStr, out DateTimeOffset dateTimeOffset))
                {
                    TimeSpan timeSpan = new TimeSpan(0, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second);
                    return timeSpan.ToString(@"hh\:mm\:ss");
                }
            }
            return null;
        }

        private bool ParseDateTime(string value, out DateTime dateTime)
        {
            // Using FhirDateTime's ToDateTimeOffset doesn't keep the time in the original time zone, so we parse the string representation, first using the appropriate segment of
            // the Regex defined at http://hl7.org/fhir/R4/datatypes.html#dateTime to pull out everything except the time zone
            string dateRegex = "([0-9]([0-9]([0-9][1-9]|[1-9]0)|[1-9]00)|[1-9]000)(-(0[1-9]|1[0-2])(-(0[1-9]|[1-2][0-9]|3[0-1])(T([01][0-9]|2[0-3]):[0-5][0-9]:([0-5][0-9]|60)?)?)?)?";
            Match dateStringMatch = Regex.Match(value, dateRegex);
            if (dateStringMatch != null && DateTime.TryParse(dateStringMatch.ToString(), out dateTime))
            {
                return true;
            }
            dateTime = new DateTime();
            return false;
        }

        private bool ParseDateTimeOffset(string value, out DateTimeOffset dateTimeOffset)
        {
            return DateTimeOffset.TryParse(value, out dateTimeOffset);
        }

        /// <summary>Getter helper for anything that can have a regular FHIR date/time or a PartialDateTime extension, allowing the time to be read
        /// from either the value or the extension</summary>
        protected string GetTimeFragmentOrPartialTime(Element value)
        {
            // If we have a basic value as a valueDateTime use that, otherwise pull from the PartialDateTime extension
            string time = GetTimeFragment(value);
            if (time != null)
            {
                return time;
            }
            return GetPartialTime(value.Extension.Find(ext => ext.Url == ExtensionURL.PartialDateTime));
        }

        /// <summary>Helper function to set a codeable value based on a code and the set of allowed codes.</summary>
        // <param name="field">the field name to set.</param>
        // <param name="code">the code to set the field to.</param>
        // <param name="options">the list of valid options and related display strings and code systems</param>
        protected void SetCodeValue(string field, string code, string[,] options)
        {
            Dictionary<string, string> dict = CreateCode(code, options);

            if (dict != null)
            {
                this.GetType().GetProperty(field).SetValue(this, dict);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="code"></param>
        /// <param name="options"></param>
        /// <exception cref="System.ArgumentException"></exception>
        protected Dictionary<string, string> CreateCode(string code, string[,] options)
        {
            // If string is empty don't bother to set the value
            if (code == null || code == "")
            {
                return null;
            }
            // Iterate over the allowed options and see if the code supplies is one of them
            for (int i = 0; i < options.GetLength(0); i += 1)
            {
                if (options[i, 0] == code)
                {
                    // Found it, so call the supplied setter with the appropriate dictionary built based on the code
                    // using the supplied options and return
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("code", code);
                    dict.Add("display", options[i, 1]);
                    dict.Add("system", options[i, 2]);
                    return dict;
                }
            }
            // If we got here we didn't find the code, so it's not a valid option
            throw new System.ArgumentException($"Code '{code}' is not an allowed value for options {options}");
        }

        /// <summary>Helper function to determine whether a value appears in the the set of allowed codes.</summary>
        // <param name="code">the code to check.</param>
        // <param name="options">the list of valid options and related display strings and code systems</param>
        protected bool CodeExistsInValueSet(string code, string[,] options, [CallerMemberName] string propertyName = null)
        {
            // Iterate over the allowed options and see if the code supplied is one of them
            for (int i = 0; i < options.GetLength(0); i += 1)
            {
                if (options[i, 0] == code)
                {
                    return true;
                }
            }
            throw new System.ArgumentException($"Code '{code}' is not an allowed value for the valueset used in {propertyName}");
        }

        /// <summary>Convert a "code" dictionary to a FHIR Coding.</summary>
        /// <param name="dict">represents a code.</param>
        /// <returns>the corresponding Coding representation of the code.</returns>
        protected Coding DictToCoding(Dictionary<string, string> dict)
        {
            Coding coding = new Coding();
            if (dict != null)
            {
                if (dict.ContainsKey("code") && !String.IsNullOrEmpty(dict["code"]))
                {
                    coding.Code = dict["code"];
                }
                if (dict.ContainsKey("system") && !String.IsNullOrEmpty(dict["system"]))
                {
                    coding.System = dict["system"];
                }
                if (dict.ContainsKey("display") && !String.IsNullOrEmpty(dict["display"]))
                {
                    coding.Display = dict["display"];
                }
                return coding;
            }
            return null;
        }

        /// <summary>Convert a "code" dictionary to a FHIR CodableConcept.</summary>
        /// <param name="dict">represents a code.</param>
        /// <returns>the corresponding CodeableConcept representation of the code.</returns>
        protected CodeableConcept DictToCodeableConcept(Dictionary<string, string> dict)
        {
            CodeableConcept codeableConcept = new CodeableConcept();
            Coding coding = DictToCoding(dict);
            codeableConcept.Coding.Add(coding);
            if (dict != null && dict.ContainsKey("text") && !String.IsNullOrEmpty(dict["text"]))
            {
                codeableConcept.Text = dict["text"];
            }
            return codeableConcept;
        }

        /// <summary>Check if a dictionary is empty or a default empty dictionary (all values are null or empty strings)</summary>
        /// <param name="dict">represents a code.</param>
        /// <returns>A boolean identifying whether the provided dictionary is empty or default.</returns>
        protected bool IsDictEmptyOrDefault(Dictionary<string, string> dict)
        {
            return dict.Count == 0 || dict.Values.All(v => v == null || v == "");
        }

        /// <summary>Convert a FHIR Coding to a "code" Dictionary</summary>
        /// <param name="coding">a FHIR Coding.</param>
        /// <returns>the corresponding Dictionary representation of the code.</returns>
        public static Dictionary<string, string> CodingToDict(Coding coding)
        {
            Dictionary<string, string> dictionary = EmptyCodeDict();
            if (coding != null)
            {
                if (!String.IsNullOrEmpty(coding.Code))
                {
                    dictionary["code"] = coding.Code;
                }
                if (!String.IsNullOrEmpty(coding.System))
                {
                    dictionary["system"] = coding.System;
                }
                if (!String.IsNullOrEmpty(coding.Display))
                {
                    dictionary["display"] = coding.Display;
                }
            }
            return dictionary;
        }

        /// <summary>Convert a FHIR CodableConcept to a "code" Dictionary</summary>
        /// <param name="codeableConcept">a FHIR CodeableConcept.</param>
        /// <returns>the corresponding Dictionary representation of the code.</returns>
        public static Dictionary<string, string> CodeableConceptToDict(CodeableConcept codeableConcept)
        {
            if (codeableConcept != null && codeableConcept.Coding != null)
            {
                Coding coding = codeableConcept.Coding.FirstOrDefault();
                var codeDict = CodingToDict(coding);
                if (codeableConcept != null && !String.IsNullOrEmpty(codeableConcept.Text))
                {
                    codeDict["text"] = codeableConcept.Text;
                }
                return codeDict;
            }
            else
            {
                return EmptyCodeableDict();
            }
        }

        /// <summary>Convert an "address" dictionary to a FHIR Address.</summary>
        /// <param name="dict">represents an address.</param>
        /// <returns>the corresponding FHIR Address representation of the address.</returns>
        protected Address DictToAddress(Dictionary<string, string> dict)
        {
            Address address = new Address();

            if (dict != null)
            {
                List<string> lines = new List<string>();
                if (dict.ContainsKey("addressLine1") && !String.IsNullOrEmpty(dict["addressLine1"]))
                {
                    lines.Add(dict["addressLine1"]);
                }
                if (dict.ContainsKey("addressLine2") && !String.IsNullOrEmpty(dict["addressLine2"]))
                {
                    lines.Add(dict["addressLine2"]);
                }
                if (lines.Count() > 0)
                {
                    address.Line = lines.ToArray();
                }
                if (dict.ContainsKey("addressCityC") && !String.IsNullOrEmpty(dict["addressCityC"]))
                {
                    Extension cityCode = new Extension();
                    cityCode.Url = VR.ExtensionURL.CityCode;
                    cityCode.Value = new PositiveInt(Int32.Parse(dict["addressCityC"]));
                    address.CityElement = new FhirString();
                    address.CityElement.Extension.Add(cityCode);
                }
                if (dict.ContainsKey("addressCity") && !String.IsNullOrEmpty(dict["addressCity"]))
                {
                    if (address.CityElement != null)
                    {
                        address.CityElement.Value = dict["addressCity"];
                    }
                    else
                    {
                        address.City = dict["addressCity"];
                    }

                }
                if (dict.ContainsKey("addressCountyC") && !String.IsNullOrEmpty(dict["addressCountyC"]))
                {
                    Extension countyCode = new Extension();
                    countyCode.Url = VR.ExtensionURL.DistrictCode;
                    countyCode.Value = new PositiveInt(Int32.Parse(dict["addressCountyC"]));
                    address.DistrictElement = new FhirString();
                    address.DistrictElement.Extension.Add(countyCode);
                }
                if (dict.ContainsKey("addressCounty") && !String.IsNullOrEmpty(dict["addressCounty"]))
                {
                    if (address.DistrictElement != null)
                    {
                        address.DistrictElement.Value = dict["addressCounty"];
                    }
                    else
                    {
                        address.District = dict["addressCounty"];
                    }
                }
                if (dict.ContainsKey("addressState") && !String.IsNullOrEmpty(dict["addressState"]))
                {
                    address.State = dict["addressState"];
                }
                // Special address field to support the jurisdiction extension custom to VRDR to support YC (New York City)
                // as used in the DeathLocationLoc
                if (dict.ContainsKey("addressJurisdiction") && !String.IsNullOrEmpty(dict["addressJurisdiction"]))
                {
                    if (address.StateElement == null)
                    {
                        address.StateElement = new FhirString();
                    }
                    address.StateElement.Extension.RemoveAll(ext => ext.Url == ExtensionURL.LocationJurisdictionId);
                    Extension extension = new Extension(ExtensionURL.LocationJurisdictionId, new FhirString(dict["addressJurisdiction"]));
                    address.StateElement.Extension.Add(extension);
                }
                if (dict.ContainsKey("addressZip") && !String.IsNullOrEmpty(dict["addressZip"]))
                {
                    address.PostalCode = dict["addressZip"];
                }
                if (dict.ContainsKey("addressCountry") && !String.IsNullOrEmpty(dict["addressCountry"]))
                {
                    address.Country = dict["addressCountry"];
                }
                if (dict.ContainsKey("addressStnum") && !String.IsNullOrEmpty(dict["addressStnum"]))
                {
                    Extension stnum = new Extension();
                    stnum.Url = VR.ExtensionURL.StreetNumber;
                    stnum.Value = new FhirString(dict["addressStnum"]);
                    address.Extension.Add(stnum);
                }
                if (dict.ContainsKey("addressPredir") && !String.IsNullOrEmpty(dict["addressPredir"]))
                {
                    Extension predir = new Extension();
                    predir.Url = VR.ExtensionURL.PreDirectional;
                    predir.Value = new FhirString(dict["addressPredir"]);
                    address.Extension.Add(predir);
                }
                if (dict.ContainsKey("addressStname") && !String.IsNullOrEmpty(dict["addressStname"]))
                {
                    Extension stname = new Extension();
                    stname.Url = VR.ExtensionURL.StreetName;
                    stname.Value = new FhirString(dict["addressStname"]);
                    address.Extension.Add(stname);
                }
                if (dict.ContainsKey("addressStdesig") && !String.IsNullOrEmpty(dict["addressStdesig"]))
                {
                    Extension stdesig = new Extension();
                    stdesig.Url = VR.ExtensionURL.StreetDesignator;
                    stdesig.Value = new FhirString(dict["addressStdesig"]);
                    address.Extension.Add(stdesig);
                }
                if (dict.ContainsKey("addressPostdir") && !String.IsNullOrEmpty(dict["addressPostdir"]))
                {
                    Extension postdir = new Extension();
                    postdir.Url = VR.ExtensionURL.PostDirectional;
                    postdir.Value = new FhirString(dict["addressPostdir"]);
                    address.Extension.Add(postdir);
                }
                if (dict.ContainsKey("addressUnitnum") && !String.IsNullOrEmpty(dict["addressUnitnum"]))
                {
                    Extension unitnum = new Extension();
                    unitnum.Url = VR.ExtensionURL.UnitOrAptNumber;
                    unitnum.Value = new FhirString(dict["addressUnitnum"]);
                    address.Extension.Add(unitnum);
                }

            }
            return address;
        }

        /// <summary>Convert a FHIR Address to an "address" Dictionary.</summary>
        /// <param name="addr">a FHIR Address.</param>
        /// <returns>the corresponding Dictionary representation of the FHIR Address.</returns>
        protected Dictionary<string, string> AddressToDict(Address addr)
        {
            Dictionary<string, string> dictionary = EmptyAddrDict();
            if (addr != null)
            {
                if (addr.Line != null && addr.Line.Count() > 0)
                {
                    dictionary["addressLine1"] = addr.Line.First();
                }

                if (addr.Line != null && addr.Line.Count() > 1)
                {
                    dictionary["addressLine2"] = addr.Line.Last();
                }

                if (addr.CityElement != null)
                {
                    Extension cityCode = addr.CityElement.Extension.Where(ext => ext.Url == VR.ExtensionURL.CityCode).FirstOrDefault();
                    if (cityCode != null)
                    {
                        dictionary["addressCityC"] = cityCode.Value.ToString();
                    }
                }

                if (addr.DistrictElement != null)
                {
                    Extension districtCode = addr.DistrictElement.Extension.Where(ext => ext.Url == VR.ExtensionURL.DistrictCode).FirstOrDefault();
                    if (districtCode != null)
                    {
                        dictionary["addressCountyC"] = districtCode.Value.ToString();
                    }
                }

                Extension stnum = addr.Extension.Where(ext => ext.Url == VR.ExtensionURL.StreetNumber).FirstOrDefault();
                if (stnum != null)
                {
                    dictionary["addressStnum"] = stnum.Value.ToString();
                }

                Extension predir = addr.Extension.Where(ext => ext.Url == VR.ExtensionURL.PreDirectional).FirstOrDefault();
                if (predir != null)
                {
                    dictionary["addressPredir"] = predir.Value.ToString();
                }

                Extension stname = addr.Extension.Where(ext => ext.Url == VR.ExtensionURL.StreetName).FirstOrDefault();
                if (stname != null)
                {
                    dictionary["addressStname"] = stname.Value.ToString();
                }

                Extension stdesig = addr.Extension.Where(ext => ext.Url == VR.ExtensionURL.StreetDesignator).FirstOrDefault();
                if (stdesig != null)
                {
                    dictionary["addressStdesig"] = stdesig.Value.ToString();
                }

                Extension postdir = addr.Extension.Where(ext => ext.Url == VR.ExtensionURL.PostDirectional).FirstOrDefault();
                if (postdir != null)
                {
                    dictionary["addressPostdir"] = postdir.Value.ToString();
                }

                Extension unitnum = addr.Extension.Where(ext => ext.Url == VR.ExtensionURL.UnitOrAptNumber).FirstOrDefault();
                if (unitnum != null)
                {
                    dictionary["addressUnitnum"] = unitnum.Value.ToString();
                }


                if (addr.State != null)
                {
                    dictionary["addressState"] = addr.State;
                }
                if (addr.StateElement != null)
                {
                    dictionary["addressJurisdiction"] = addr.State; // by default.  If extension present, override
                    Extension stateExt = addr.StateElement.Extension.Where(ext => ext.Url == ExtensionURL.LocationJurisdictionId).FirstOrDefault();
                    if (stateExt != null)
                    {
                        dictionary["addressJurisdiction"] = stateExt.Value.ToString();
                    }
                }
                if (addr.City != null)
                {
                    dictionary["addressCity"] = addr.City;
                }
                if (addr.District != null)
                {
                    dictionary["addressCounty"] = addr.District;
                }
                if (addr.PostalCode != null)
                {
                    dictionary["addressZip"] = addr.PostalCode;
                }
                if (addr.Country != null)
                {
                    dictionary["addressCountry"] = addr.Country;
                }
            }
            return dictionary;
        }

        /// <summary>Returns an empty "address" Dictionary.</summary>
        /// <returns>an empty "address" Dictionary.</returns>
        protected Dictionary<string, string> EmptyAddrDict()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("addressLine1", "");
            dictionary.Add("addressLine2", "");
            dictionary.Add("addressCity", "");
            dictionary.Add("addressCityC", "");
            dictionary.Add("addressCounty", "");
            dictionary.Add("addressCountyC", "");
            dictionary.Add("addressState", "");
            dictionary.Add("addressJurisdiction", "");
            dictionary.Add("addressZip", "");
            dictionary.Add("addressCountry", "");
            dictionary.Add("addressStnum", "");
            dictionary.Add("addressPredir", "");
            dictionary.Add("addressStname", "");
            dictionary.Add("addressStdesig", "");
            dictionary.Add("addressPostdir", "");
            dictionary.Add("addressUnitnum", "");
            return dictionary;
        }

        /// <summary>Returns an empty "code" Dictionary.</summary>
        /// <returns>an empty "code" Dictionary.</returns>
        public static Dictionary<string, string> EmptyCodeDict()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", "");
            dictionary.Add("system", "");
            dictionary.Add("display", "");
            return dictionary;
        }

        /// <summary>Returns an empty "codeable" Dictionary.</summary>
        /// <returns>an empty "codeable" Dictionary.</returns>
        public static Dictionary<string, string> EmptyCodeableDict()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", "");
            dictionary.Add("system", "");
            dictionary.Add("display", "");
            dictionary.Add("text", "");
            return dictionary;
        }

        /// <summary>Given a FHIR path, return the elements that match the given path;
        /// returns an empty array if no matches are found.</summary>
        /// <param name="path">represents a FHIR path.</param>
        /// <returns>all elements that match the given path, or an empty array if no matches are found.</returns>
        public object[] GetAll(string path)
        {
            var matches = Navigator.Select(path);
            ArrayList list = new ArrayList();
            foreach (var match in matches)
            {
                list.Add(match.Value);
            }
            return list.ToArray();
        }

        /// <summary>Given a FHIR path, return the first element that matches the given path.</summary>
        /// <param name="path">represents a FHIR path.</param>
        /// <returns>the first element that matches the given path, or null if no match is found.</returns>
        public object GetFirst(string path)
        {
            var matches = Navigator.Select(path);
            if (matches.Count() > 0)
            {
                return matches.First().Value;
            }
            else
            {
                return null; // Nothing found
            }
        }

        /// <summary>Given a FHIR path, return the elements that match the given path as a string;
        /// returns an empty array if no matches are found.</summary>
        /// <param name="path">represents a FHIR path.</param>
        /// <returns>all elements that match the given path as a string, or an empty array if no matches are found.</returns>
        protected string[] GetAllString(string path)
        {
            ArrayList list = new ArrayList();
            foreach (var match in GetAll(path))
            {
                list.Add(Convert.ToString(match));
            }
            return list.ToArray(typeof(string)) as string[];
        }

        /// <summary>Given a FHIR path, return the first element that matches the given path as a string;
        /// returns null if no match is found.</summary>
        /// <param name="path">represents a FHIR path.</param>
        /// <returns>the first element that matches the given path as a string, or null if no match is found.</returns>
        protected string GetFirstString(string path)
        {
            var first = GetFirst(path);
            if (first != null)
            {
                return Convert.ToString(first);
            }
            else
            {
                return null; // Nothing found
            }
        }

        /// <summary>Get a value from a Dictionary, but return null if the key doesn't exist or the value is an empty string.</summary>
        protected static string GetValue(Dictionary<string, string> dict, string key)
        {
            if (dict != null && dict.ContainsKey(key) && !String.IsNullOrWhiteSpace(dict[key]))
            {
                return dict[key];
            }
            return null;
        }

        /// <summary>Helper method to return a JSON string representation of this Vital Record.</summary>
        /// <param name="contents">string that represents </param>
        /// <returns>a new VitalRecord that corresponds to the given descriptive format</returns>
        public static RecordType FromDescription<RecordType>(string contents) where RecordType : VitalRecord, new()
        {
            RecordType record = new RecordType();
            Dictionary<string, Dictionary<string, dynamic>> description =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, dynamic>>>(contents,
                    new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
            // Loop over each category
            foreach (KeyValuePair<string, Dictionary<string, dynamic>> category in description)
            {
                // Loop over each property
                foreach (KeyValuePair<string, dynamic> property in category.Value)
                {
                    if (!property.Value.ContainsKey("Value") || property.Value["Value"] == null)
                    {
                        continue;
                    }
                    // Set the property on the new VitalRecord based on its type
                    string propertyName = property.Key;
                    Object value = null;
                    if (property.Value["Type"] == Property.Types.String || property.Value["Type"] == Property.Types.StringDateTime)
                    {
                        value = property.Value["Value"].ToString();
                        if (String.IsNullOrWhiteSpace((string)value))
                        {
                            value = null;
                        }
                    }
                    else if (property.Value["Type"] == Property.Types.StringArr)
                    {
                        value = property.Value["Value"].ToObject<String[]>();
                    }
                    else if (property.Value["Type"] == Property.Types.Int32)
                    {
                        value = property.Value["Value"].ToObject<int?>();
                    }
                    else if (property.Value["Type"] == Property.Types.Bool)
                    {
                        value = property.Value["Value"].ToObject<bool>();
                    }
                    else if (property.Value["Type"] == Property.Types.TupleArr)
                    {
                        value = property.Value["Value"].ToObject<Tuple<string, string>[]>();
                    }
                    else if (property.Value["Type"] == Property.Types.TupleCOD)
                    {
                        value = property.Value["Value"].ToObject<Tuple<string, string /*, Dictionary<string, string>*/>[]>();
                    }
                    else if (property.Value["Type"] == Property.Types.Dictionary)
                    {
                        Dictionary<string, Dictionary<string, string>> moreInfo =
                            property.Value["Value"].ToObject<Dictionary<string, Dictionary<string, string>>>();
                        Dictionary<string, string> result = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, Dictionary<string, string>> entry in moreInfo)
                        {
                            result[entry.Key] = entry.Value["Value"];
                        }
                        value = result;
                    }
                    if (value != null)
                    {
                        typeof(RecordType).GetProperty(propertyName).SetValue(record, value);
                    }
                }
            }
            return record;
        }

        /// <summary>Returns a JSON encoded structure that maps to the various property
        /// annotations found in the Vital Record. This is useful for scenarios
        /// where you may want to display the data in user interfaces.</summary>
        /// <returns>a string representation of this Vital Record in a descriptive format.</returns>
        public string ToDescription()
        {
            Dictionary<string, Dictionary<string, dynamic>> description = new Dictionary<string, Dictionary<string, dynamic>>();
            // the priority values should order the categories as: Decedent Demographics, Decedent Disposition, Death Investigation, Death Certification
            foreach (PropertyInfo property in this.GetType().GetProperties().OrderBy(p => p.GetCustomAttribute<Property>()?.Priority))
            {
                // Grab property annotation for this property
                Property info = property.GetCustomAttribute<Property>();

                // Skip properties that shouldn't be serialized.
                if (info == null || !info.Serialize)
                {
                    continue;
                }

                // Add category if it doesn't yet exist
                if (!description.ContainsKey(info.Category))
                {
                    description.Add(info.Category, new Dictionary<string, dynamic>());
                }

                // Add the new property to the category
                Dictionary<string, dynamic> category = description[info.Category];
                category[property.Name] = new Dictionary<string, dynamic>();

                // Add the attributes of the property
                category[property.Name]["Name"] = info.Name;
                category[property.Name]["Type"] = info.Type.ToString();
                category[property.Name]["Description"] = info.Description;
                category[property.Name]["IGUrl"] = info.IGUrl;
                category[property.Name]["CapturedInIJE"] = info.CapturedInIJE;

                // Add snippets
                FHIRPath path = property.GetCustomAttribute<FHIRPath>();
                category[property.Name]["CheckboxType"] = path.Section != null;
                category[property.Name]["Section"] = path.Section;
                category[property.Name]["Code"] = path.Code;
                category[property.Name]["CategoryCode"] = path.CategoryCode;
                var matches = Navigator.Select(path.Path);
                if (matches.Count() > 0)
                {
                    if (info.Type == Property.Types.TupleCOD || info.Type == Property.Types.TupleArr || info.Type == Property.Types.Tuple4Arr)
                    {
                        // Make sure to grab all of the Conditions for COD
                        string xml = "";
                        string json = "";
                        foreach (ITypedElement match in matches)
                        {
                            xml += match.ToXml();
                            json += match.ToJson() + ",";
                        }
                        category[property.Name]["SnippetXML"] = xml;
                        category[property.Name]["SnippetJSON"] = "[" + json + "]";
                    }
                    else if (!String.IsNullOrWhiteSpace(path.Element))
                    {
                        // Since there is an "Element" for this path, we need to be more
                        // specific about what is included in the snippets.
                        XElement root = XElement.Parse(matches.First().ToXml());
                        XElement node = root.DescendantsAndSelf("{http://hl7.org/fhir}" + path.Element).FirstOrDefault();
                        if (node != null)
                        {
                            node.Name = node.Name.LocalName;
                            category[property.Name]["SnippetXML"] = node.ToString();
                        }
                        else
                        {
                            category[property.Name]["SnippetXML"] = "";
                        }
                        Dictionary<string, dynamic> jsonRoot =
                           JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(matches.First().ToJson(),
                               new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
                        if (jsonRoot != null && jsonRoot.Keys.Contains(path.Element))
                        {
                            category[property.Name]["SnippetJSON"] = "{" + $"\"{path.Element}\": \"{jsonRoot[path.Element]}\"" + "}";
                        }
                        else
                        {
                            category[property.Name]["SnippetJSON"] = "";
                        }
                    }
                    else
                    {
                        category[property.Name]["SnippetXML"] = matches.First().ToXml();
                        category[property.Name]["SnippetJSON"] = matches.First().ToJson();
                    }

                }
                else
                {
                    category[property.Name]["SnippetXML"] = "";
                    category[property.Name]["SnippetJSON"] = "";
                }

                // Add the current value of the property
                if (info.Type == Property.Types.Dictionary)
                {
                    // Special case for Dictionary; we want to be able to describe what each key means
                    Dictionary<string, string> value = (Dictionary<string, string>)property.GetValue(this);
                    if (value == null)
                    {
                        continue;
                    }
                    Dictionary<string, Dictionary<string, string>> moreInfo = new Dictionary<string, Dictionary<string, string>>();
                    foreach (PropertyParam propParameter in property.GetCustomAttributes<PropertyParam>())
                    {
                        moreInfo[propParameter.Key] = new Dictionary<string, string>();
                        moreInfo[propParameter.Key]["Description"] = propParameter.Description;
                        if (value.ContainsKey(propParameter.Key))
                        {
                            moreInfo[propParameter.Key]["Value"] = value[propParameter.Key];
                        }
                        else
                        {
                            moreInfo[propParameter.Key]["Value"] = null;
                        }
                    }
                    category[property.Name]["Value"] = moreInfo;
                }
                else
                {
                    category[property.Name]["Value"] = property.GetValue(this);
                }
            }
            return JsonConvert.SerializeObject(description);
        }

        /// <summary>Gets the given place of birth dictionary address from the given patient.</summary>
        protected Dictionary<string, string> GetPlaceOfBirth(Patient patient)
        {
            if (patient != null)
            {
                Extension addressExt = patient.Extension.FirstOrDefault(extension => extension.Url == OtherExtensionURL.PatientBirthPlace);
                if (addressExt != null)
                {
                    Address address = (Address)addressExt.Value;
                    if (address != null)
                    {
                        return AddressToDict((Address)address);
                    }
                    return EmptyAddrDict();
                }
            }
            return EmptyAddrDict();
        }

        /// <summary>Gets the given place of birth dictionary address from the given related person.</summary>
        protected Dictionary<string, string> GetPlaceOfBirth(RelatedPerson person)
        {
            if (person != null)
            {
                Extension addressExt = person.Extension.FirstOrDefault(extension => extension.Url == VR.ExtensionURL.RelatedpersonBirthplace);
                if (addressExt != null)
                {
                    Address address = (Address)addressExt.Value;
                    if (address != null)
                    {
                        return AddressToDict((Address)address);
                    }
                    return EmptyAddrDict();
                }
            }
            return EmptyAddrDict();
        }

        /// <summary>Sets the given place of birth dictionary on the given patient.</summary>
        protected void SetPlaceOfBirth(Patient patient, Dictionary<string, string> value)
        {
            patient.Extension.RemoveAll(ext => ext.Url == OtherExtensionURL.PatientBirthPlace);
            if (!IsDictEmptyOrDefault(value))
            {
                Extension placeOfBirthExt = new Extension
                {
                    Url = OtherExtensionURL.PatientBirthPlace,
                    Value = DictToAddress(value)
                };
                patient.Extension.Add(placeOfBirthExt);
            }
        }

        /// <summary>Add a BirthDateElement to the given Patient resource.</summary>
        protected void AddBirthDateToPatient(Patient patient, bool includeTime)
        {
            patient.BirthDateElement = new Date();
            patient.BirthDateElement.Extension.Add(NewBlankPartialDateTimeExtension(includeTime));
        }


        /// <summary>Sets the given place of birth dictionary on the given patient.</summary>
        protected void SetPlaceOfBirth(RelatedPerson person, Dictionary<string, string> value)
        {
            person.Extension.RemoveAll(ext => ext.Url == VR.ExtensionURL.RelatedpersonBirthplace);
            if (!IsDictEmptyOrDefault(value))
            {
                Extension placeOfBirthExt = new Extension
                {
                    Url = VR.ExtensionURL.RelatedpersonBirthplace,
                    Value = DictToAddress(value)
                };
                person.Extension.Add(placeOfBirthExt);
            }
        }

        /// <summary>
        /// Creates an encounter with the given profile url.
        /// </summary>
        /// <param name="profileURL"></param>
        /// <returns></returns>
        protected Encounter CreateEncounter(string profileURL)
        {
            Encounter encounter = new Encounter()
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta()
            };
            encounter.Meta.Profile = new List<string>()
            {
                profileURL
            };
            return encounter;
        }

        /// <summary>
        /// Gets the physical location from the given encounter.
        /// </summary>
        /// <param name="encounter"></param>
        /// <returns>A dictionary in the form of a codeable concept.</returns>
        protected Dictionary<string, string> GetPhysicalLocation(Encounter encounter)
        {
            if (encounter == null)
            {
                return EmptyCodeableDict();
            }
            return CodeableConceptToDict(encounter.Location.Select(loc => loc.PhysicalType).FirstOrDefault());
        }

        /// <summary>
        /// Sets the physical location of the given encounter to the given dictionary, which should be in codeablconcept format.
        /// </summary>
        /// <param name="encounter"></param>
        /// <param name="value"></param>
        protected void SetPhysicalLocation(Encounter encounter, Dictionary<string, string> value)
        {
            encounter.Location = new List<Hl7.Fhir.Model.Encounter.LocationComponent>();
            LocationComponent location = new LocationComponent
            {
                PhysicalType = DictToCodeableConcept(value)
            };
            encounter.Location.Add(location);
        }

        /// <summary>
        /// Gets the physical location from the given encounter.
        /// </summary>
        /// <param name="encounter"></param>
        /// <returns></returns>
        protected string GetPhysicalLocationHelper(Encounter encounter)
        {
            Dictionary<string, string> locationDict = GetPhysicalLocation(encounter);
            if (locationDict.ContainsKey("code"))
            {
                string code = locationDict["code"];
                if (code == "OTH")
                {
                    if (locationDict.ContainsKey("text") && !String.IsNullOrWhiteSpace(locationDict["text"]))
                    {
                        return locationDict["text"];
                    }
                    return "Other";
                }
                else if (!String.IsNullOrWhiteSpace(code))
                {
                    return code;
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the given physical location string in the given encounter, mapping the string based on the given mapping and value set.
        /// </summary>
        /// <param name="encounter"></param>
        /// <param name="value"></param>
        /// <param name="fHIRToIJEMapping"></param>
        /// <param name="valueSetCodes"></param>
        protected void SetPhysicalLocationHelper(Encounter encounter, string value, Dictionary<string, string> fHIRToIJEMapping, string[,] valueSetCodes)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                // do nothing
                return;
            }
            if (!fHIRToIJEMapping.ContainsKey(value))
            {
                // other
                SetPhysicalLocation(encounter, CodeableConceptToDict(new CodeableConcept(CodeSystems.NullFlavor_HL7_V3, "OTH", "Other", value)));
            }
            else
            {
                // normal path
                SetPhysicalLocation(encounter, CreateCode(value, valueSetCodes));
            }
        }
    }

    /// <summary>Property attribute used to describe a VitalRecord property parameter,
    /// specifically if the property is a dictionary that has keys.</summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
    public class PropertyParam : System.Attribute
    {
        /// <summary>If the related property is a Dictionary, the key name.</summary>
        public string Key;

        /// <summary>Description of this parameter.</summary>
        public string Description;

        /// <summary>Constructor.</summary>
        public PropertyParam(string key, string description)
        {
            this.Key = key;
            this.Description = description;
        }
    }

    /// <summary>Describes a FHIR path that can be used to get to the element.</summary>
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class FHIRPath : System.Attribute
    {
        /// <summary>FHIR resource types supported for "presence only" entries implements via EntryExists and UpdateEntry</summary>
        public enum FhirType
        {
            /// <summary>FHIR Condition</summary>
            Condition,
            /// <summary>FHIR Procedure</summary>
            Procedure,
            /// <summary>FHIR Observation</summary>
            Observation,
            /// <summary>FHIR Coverage</summary>
            Coverage
        }

        /// <summary>The relevant FHIR path.</summary>
        public string Path;

        /// <summary>The relevant element.</summary>
        public string Element;

        /// <summary>The type of FHIR resource.</summary>
        public FhirType? FHIRType;

        /// <summary>The LOINC category code when the targeted FHIR resource has a category element. E.g. Condition.category.</summary>
        public string CategoryCode;

        /// <summary>The code when the targeted FHIR resource has a code element, e.g. Condition.code.</summary>
        public string Code;

        /// <summary>The code system when the targeted FHIR resource has a code element, e.g. Condition.code. Defaults to SNOMED if not specified.</summary>
        public string CodeSystem;

        /// <summary>The display when the targeted FHIR resource has a code element</summary>
        public string Display;

        /// <summary>The composition section code. Required if the resource needs to be referenced from the composition.</summary>
        public string Section;

        /// <summary>Constructor.</summary>
        public FHIRPath(string path, string element)
        {
            this.Path = path;
            this.Element = element;
        }

        /// <summary>Constructor.</summary>
        public FHIRPath(FhirType fhirType, string categoryCode = "", string code = null, string section = null, string codeSystem = null)
        {
            if (fhirType == FhirType.Observation)
            {
                this.Path = $"Bundle.entry.resource.where($this is {fhirType}).where((value as CodeableConcept).coding.code = '{code}')";
                if (categoryCode != null && categoryCode.Length > 0)
                {
                    this.Path += $".where((code as CodeableConcept).coding.code = '{categoryCode}')";
                }
            }
            else
            {
                this.Path = $"Bundle.entry.resource.where($this is {fhirType}).where(code.coding.code = '{code}')";
                if (categoryCode != null && categoryCode.Length > 0)
                {
                    this.Path += $".where(category.coding.any(code = '{categoryCode}'))";
                }
            }
            this.Element = "";
            this.FHIRType = fhirType;
            this.CategoryCode = categoryCode;
            this.Code = code;
            this.CodeSystem = codeSystem;
            this.Section = section;
        }
    }

    /// <summary>Property attribute used to describe a BirthRecord property.</summary>
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class Property : System.Attribute
    {
        /// <summary>Enum for describing the property type.</summary>
        public enum Types
        {
            /// <summary>Parameter is a string.</summary>
            String,
            /// <summary>Parameter is an array of strings.</summary>
            StringArr,
            /// <summary>Parameter is like a string, but should be treated as a date and time.</summary>
            StringDateTime,
            /// <summary>Parameter is a bool.</summary>
            Bool,
            /// <summary>Parameter is a Dictionary.</summary>
            Dictionary,
            /// <summary>Parameter is an array of Tuples.</summary>
            TupleArr,
            /// <summary>Parameter is an array of Tuples, specifically for CausesOfDeath. Specific to VRDR</summary>
            TupleCOD,
            /// <summary>Parameter is an unsigned integer.</summary>
            Int32,
            /// <summary>Parameter is an array of 4-Tuples, specifically for entity axis codes.</summary>
            Tuple4Arr
        };

        /// <summary>Name of this property.</summary>
        public string Name;

        /// <summary>The property type (e.g. string, bool, Dictionary).</summary>
        public Types Type;

        /// <summary>Category of this property.</summary>
        public string Category;

        /// <summary>Description of this property.</summary>
        public string Description;

        /// <summary>If this field should be kept when serialzing.</summary>
        public bool Serialize;

        /// <summary>URL that links to the IG description for this property.</summary>
        public string IGUrl;

        /// <summary>If this field has an equivalent in IJE.</summary>
        public bool CapturedInIJE;

        /// <summary>Priority that this should show up in generated lists. Lower numbers come first.</summary>
        public int Priority;

        /// <summary>Constructor.</summary>
        public Property(string name, Types type, string category, string description, bool serialize, string igurl, bool capturedInIJE, int priority = 4)
        {
            this.Name = name;
            this.Type = type;
            this.Category = category;
            this.Description = description;
            this.Serialize = serialize;
            this.IGUrl = igurl;
            this.CapturedInIJE = capturedInIJE;
            this.Priority = priority;
        }
    }
}
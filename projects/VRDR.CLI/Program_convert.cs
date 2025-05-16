using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace VRDR.CLI
{
    partial class Program
    {
        // VRDR STU2.2 and STU3 are not compatible.  Content that transitioned from VRDR to VRCL as part of the harmonization changed identifiers.
        // There will be jurisdictions using these different versions for a while, so converting between the versions is likely required.

        // The following two code system URLs are copied from vital-records-dotnet/VitalRecord/CodeSystems.cs and vrdr-dotnet/VRDR/CodeSystems.cs.
        // respectively. They are duplicated here so that this source file can be used in projects that include either version of the VRDR library.
        private static readonly string LocalObservationCodes = "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-local-observation-codes-vr";
        private static readonly string HL7_identifier_type = "http://terminology.hl7.org/CodeSystem/v2-0203";

        // CreateSTU2toSTU3Mapping:  Reverse the sense of the STU3 to STU2 mapping.
        static Dictionary<string, string> CreateSTU2toSTU3Mapping(Dictionary<string, string> urisSTU3toSTU2)
        {
            var revUrisSTU3toSTU2 = new Dictionary<string, string>();
            foreach (var kvp in urisSTU3toSTU2)
            {
                revUrisSTU3toSTU2[kvp.Value] = kvp.Key;
            }
            return revUrisSTU3toSTU2;
        }

        // ConvertVersionJSON:  The boolean STU3toSTU2 should be true when used in this library that supports STU3.
        // The same code could be used in the vrdr-dotnet library that supports VRDR STU2.2, with STU3toSTU2 set to false.
        static void ConvertVersion(string pOutputFile, string pInputFile, bool STU3toSTU2, bool jsonConversion)
        {
            var uris = UrisSTU3toSTU2;
            var dateTimeUris = dateTimeComponentsSTU3toSTU2;
            Bundle bundle;
            string newContent;

            if (!STU3toSTU2)
            { // The mapping is bidirectional.  Depending on which direction, we flip the map.
                uris = CreateSTU2toSTU3Mapping(UrisSTU3toSTU2);
                dateTimeUris = CreateSTU2toSTU3Mapping(dateTimeComponentsSTU3toSTU2);
            }
            string content = File.ReadAllText(pInputFile);
            // Iterate through the mapped codesystem strings, and replace them one by one
            foreach (var kvp in uris)
            {
                content = content.Replace(kvp.Key, kvp.Value);
            }
            // Fix an observation's code and CodeSystem.  This can't be done using string replace.
            if (jsonConversion)
            { // JSON Conversion
                ParserSettings parserSettings = new ParserSettings
                {
                    AcceptUnknownMembers = true,
                    AllowUnrecognizedEnums = true,
                    PermissiveParsing = true
                };
                FhirJsonParser parser = new FhirJsonParser(parserSettings);
                bundle = parser.Parse<Bundle>(content);
            }
            else
            {
                //XML Conversion
                // Parse the FHIR XML into a Resource object
                var parser = new FhirXmlParser();
                bundle = parser.Parse<Bundle>(content);
            }
            // Scan through all Observations to make sure they all have codes!
            foreach (var entry in bundle.Entry)
            {
                if (entry.Resource is Observation)
                {
                    Observation obs = (Observation)entry.Resource;
                    if (obs.Code == null || obs.Code.Coding == null || obs.Code.Coding.FirstOrDefault() == null || obs.Code.Coding.First().Code == null)
                    {
                        continue;
                    }
                    if (!STU3toSTU2)
                    {
                        switch (obs.Code.Coding.First().Code)
                        {
                            case "BR":
                                obs.Code = new CodeableConcept(LocalObservationCodes, "childbirthrecordidentifier", "Birth Record Identifier of Child", null);
                                break;
                        }
                    }
                    else
                    {
                        switch (obs.Code.Coding.First().Code)
                        {
                            case "childbirthrecordidentifier":
                                obs.Code = new CodeableConcept(HL7_identifier_type, "BR", "Birth registry number", null);
                                break;
                        }
                    }
                }
            }

            // Recursively update all extensions in the entire bundle
            UpdateExtensionsRecursively(bundle, dateTimeUris);


            if (jsonConversion)
            { // Serialize the bundle as JSON
                newContent = bundle.ToJson(new FhirJsonSerializationSettings { Pretty = true, AppendNewLine = true });
            }
            else
            {
                var serializer = new FhirXmlSerializer();
                newContent = serializer.SerializeToString(bundle);
            }
            File.WriteAllText(pOutputFile, newContent);
        }

        // UpdateExtensionsRecursively:
        // Utility routine to fill the gap in Firely FHIRPath capabilities.
        static void UpdateExtensionsRecursively(Base fhirElement, Dictionary<string, string> replacements)
        {
            if (fhirElement == null) return;

            // Check if this element has extensions (any FHIR element can!)
            if (fhirElement is IExtendable extendable)
            {
                if (extendable.Extension != null)
                {
                    foreach (var ext in extendable.Extension)
                    {
                        // If the URL matches, update it
                        if (replacements.ContainsKey(ext.Url))
                        {
                            ext.Url = replacements[ext.Url];
                        }

                        // Recursively process nested extensions
                        if (ext.Extension != null && ext.Extension.Any())
                        {
                            UpdateExtensionsRecursively(ext, replacements);
                        }
                    }
                }
            }

            // Recursively process all properties of the current resource
            foreach (var property in fhirElement.GetType().GetProperties())
            {
                if (typeof(Base).IsAssignableFrom(property.PropertyType))
                {
                    // Single FHIR element (e.g., Patient.name)
                    var value = property.GetValue(fhirElement) as Base;
                    if (value != null) UpdateExtensionsRecursively(value, replacements);
                }
                else if (typeof(IEnumerable<Base>).IsAssignableFrom(property.PropertyType))
                {
                    // List of FHIR elements (e.g., Bundle.entry, Patient.name)
                    var values = property.GetValue(fhirElement) as IEnumerable<Base>;
                    if (values != null)
                    {
                        foreach (var item in values)
                        {
                            UpdateExtensionsRecursively(item, replacements);
                        }
                    }
                }
            }
        }
    }
}
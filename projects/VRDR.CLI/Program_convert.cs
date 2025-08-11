using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        enum ConversionDirection
        {
            STU3toSTU2,
            STU2toSTU3
        }

        enum DataFormat
        {
            JSON,
            XML
        }

        /// <summary>Convert between implementation guide versions</summary>
        /// <param name="pOutputFile">path to the output file</param>
        /// <param name="pInputFile">path to the input file</param>
        /// <param name="direction">whether to convert from STU2 to STU3 or vice versa</param>
        /// <param name="format">whether to read/write JSON or XML</param>
        static void ConvertVersion(string pOutputFile, string pInputFile, ConversionDirection direction, DataFormat format)
        {
            var uris = UrisSTU3toSTU2;
            var dateTimeUris = dateTimeComponentsSTU3toSTU2;
            Bundle bundle;
            string newContent;

            if (direction == ConversionDirection.STU2toSTU3)
            { // The mapping is bidirectional.  Depending on which direction, we flip the map.
                uris = uris.ToDictionary(x => x.Value, x => x.Key);
                dateTimeUris = dateTimeUris.ToDictionary(x => x.Value, x => x.Key);
            }
            string content = File.ReadAllText(pInputFile);
            // Iterate through the mapped codesystem strings, and replace them one by one
            foreach (var kvp in uris)
            {
                content = content.Replace(kvp.Key, kvp.Value);
            }
            // Fix an observation's code and CodeSystem.  This can't be done using string replace.
            if (format == DataFormat.JSON)
            {
                // JSON Conversion
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
                    if (direction == ConversionDirection.STU2toSTU3)
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


            if (format == DataFormat.JSON)
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

        private static string Ije2JsonConversion(string ijeFilepath, string jsonFilePath)
        {
            string ijeRawRecord = File.ReadAllText(ijeFilepath);
            IJEMortality ije = new IJEMortality(ijeRawRecord);
            DeathRecord d = ije.ToRecord();
            StreamWriter sw = new StreamWriter(jsonFilePath);
            sw.WriteLine(d.ToJSON());
            sw.Flush();
            return sw.ToString();
        }
        private static string Json2Ijeconsversion(string jsonFilePath, string destFilePath)
        {
            DeathRecord d = new DeathRecord(File.ReadAllText(jsonFilePath));
            IJEMortality ije1 = new IJEMortality(d, false);
            File.WriteAllText(destFilePath, ije1.ToString());
            return ije1.ToString();
        }

        private static List<(string Input, string Output)> BuildConversionFileList(string srcPath, string destPath, 
            string searchFileExtension,string destFileExtension)
        {
            var ioFilePathMappingPathList = new List<(string, string)>();

            if (string.IsNullOrWhiteSpace(srcPath) || (!File.Exists(srcPath) && !Directory.Exists(srcPath)))
            {
                throw new FileNotFoundException($"Provided path '{srcPath}' not found.");
            }

            FileAttributes attr = File.GetAttributes(srcPath);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                foreach (var file in Directory.GetFiles(srcPath, searchFileExtension,SearchOption.TopDirectoryOnly))
                {
                    var outputPath = !string.IsNullOrWhiteSpace(destPath) && Directory.Exists(destPath)
                        ? Path.Combine(destPath, Path.ChangeExtension(Path.GetFileName(file),destFileExtension))
                        : Path.ChangeExtension(file,destFileExtension);

                    ioFilePathMappingPathList.Add((file, outputPath));
                }
            }
            else
            {
                var outputPath = !string.IsNullOrWhiteSpace(destPath) && Directory.Exists(destPath)
                    ? Path.Combine(destPath, Path.ChangeExtension(Path.GetFileName(srcPath), destFileExtension))
                        : Path.ChangeExtension(srcPath, destFileExtension);

                ioFilePathMappingPathList.Add((srcPath, outputPath));
            }

            return ioFilePathMappingPathList;
        }

        public static bool EnsureDirectoryPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            // If path exists, check type
            if (Directory.Exists(path))
                return true;
            if (File.Exists(path))
                return false;

            // Treat as directory if no extension or ends with slash
            bool looksLikeDirectory = path.EndsWith(Path.DirectorySeparatorChar.ToString()) ||
                                      path.EndsWith(Path.AltDirectorySeparatorChar.ToString()) ||
                                      string.IsNullOrEmpty(Path.GetExtension(path));

            if (looksLikeDirectory)
            {
                try
                {
                    Directory.CreateDirectory(path);
                    return true;
                }
                catch
                {
                    Console.WriteLine("Provided destination folder does not exist and could not be created. Files will be generated in the input folder instead.");
                    return false;
                }
            }

            return false;
        }

    }
}
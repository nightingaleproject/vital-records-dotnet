using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using System.Net.Http;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.ElementModel;
using Hl7.FhirPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BFDR;
using VR;

namespace BFDR.CLI
{
    class Program
    {
        static string commands =
@"* BFDR Command Line Interface - commands
  - help:  prints this help message  (no arguments)
  - fakerecord: prints a fake JSON birth record (no arguments)
  - description: prints a verbose JSON description of the record (in the format used to drive Canary) (1 argument: the path to the birth record)
  - 2ije: Read in the FHIR XML or JSON birth record and print out as IJE (1 argument: path to birth record in JSON or XML format)
  - 2ijecontent: Read in the FHIR XML or JSON birth record and dump content  in key/value IJE format (1 argument: path to birth record in JSON or XML format)
  - ije2json: Read in the IJE birth record and print out as JSON (1 argument: path to birth record in IJE format)
  - ije2json: Read in several IJE birth records and print out as JSON files to same directory they were imported from (2 or more arguments: list of paths to birth records in IJE format)
  - ije2xml: Read in the IJE birth record and print out as XML (1 argument: path to birth record in IJE format)
  - extract2ijecontent: Dump content of a submission message in key/value IJE format (1 argument: submission message)
  - submit: Create a submission FHIR message wrapping a FHIR birth record (1 argument: FHIR birth record)
  - resubmit: Create a submission update FHIR message wrapping a FHIR birth record (1 argument: FHIR birth record)
  - void: Creates a Void message for a Birth Record (1 argument: FHIR birth record; one optional argument: number of records to void)
  - ack: Create an acknowledgement FHIR message for a submission FHIR message (1 argument: submission FHIR message; many arguments: output directory and FHIR messages)
  - ije2json: Creates an IJE birth record and prints out as JSON
  - json2xml: Read in the FHIR JSON birth record, completely disassemble then reassemble, and print as FHIR XML (1 argument: FHIR JSON Birth Record)
  - checkXml: Read in the given FHIR xml (being permissive) and print out the same; useful for doing validation diffs (1 argument: FHIR XML file)
  - checkJson: Read in the given FHIR json (being permissive) and print out the same; useful for doing validation diffs (1 argument: FHIR JSon file)
  - xml2json: Read in the IJE birth record and print out as JSON (1 argument: path to death record in XML format)
  - xml2xml: Read in the IJE birth record and print out as XML (1 argument: path to death record in XML format)
  - json2json: Read in the FHIR JSON birth record, completely disassemble then reassemble, and print as FHIR JSON (1 argument: FHIR JSON Birth Record)
  - roundtrip-ije: Convert a record to IJE and back and check field by field to identify any conversion issues (1 argument: FHIR Birth Record)
  - roundtrip-all: Convert a record to JSON and back and check field by field to identify any conversion issues (1 argument: FHIR Birth Record)
  - ije: Read in and parse an IJE death record and print out the values for every (supported) field (1 argument: path to death record in IJE format)
  - ijebuilder: Create json birth record using IJE (natality) mapped fields
  - compare: Compare an IJE record with a FHIR record by each IJE field (2 arguments:  IJE record, FHIR Record)
  - extract: Extract a FHIR record from a FHIR message (1 argument: FHIR message)
    ";

        static int Main(string[] args)
        {
            if ((args.Length == 0) || ((args.Length == 1) && (args[0] == "help")))
            {
                Console.WriteLine(commands);
                //return (0);
            }
            else if (args.Length == 1 && args[0] == "fakerecord")
            {
                // 0. Set up a BirthRecord object
                BirthRecord birthRecord = new BirthRecord();
                birthRecord.BirthYear = 2023;
                birthRecord.BirthMonth = 1;
                birthRecord.BirthDay = 1;
                birthRecord.CertificateNumber = "100";
                birthRecord.StateLocalIdentifier1 = "123";
                birthRecord.DateOfBirth = "2023-01-01";
                birthRecord.BirthSexHelper = "M";

                string[] childNames = { "Alexander", "Arlo" };
                birthRecord.ChildGivenNames = childNames;
                string[] motherName = { "Xenia" };
                birthRecord.MotherGivenNames = motherName;
                string lastName = "Adkins";
                birthRecord.ChildFamilyName = lastName;
                birthRecord.MotherFamilyName = lastName;

                birthRecord.BirthLocationJurisdiction = "MA";
                Dictionary<string, string> birthAddress = new Dictionary<string, string>();
                birthAddress.Add("addressLine1", "123 Fake Street");
                birthAddress.Add("addressCity", "Springfield");
                birthAddress.Add("addressCounty", "Hampden");
                birthAddress.Add("addressState", "MA");
                birthAddress.Add("addressZip", "01101");
                birthAddress.Add("addressCountry", "US");
                birthRecord.PlaceOfBirth = birthAddress;

                birthRecord.InfantMedicalRecordNumber = "7134703";
                birthRecord.MotherMedicalRecordNumber = "2286144";
                birthRecord.MotherSocialSecurityNumber = "133756482";

                birthRecord.SetOrder = null;
                birthRecord.Plurality = null;
                birthRecord.NoCongenitalAnomaliesOfTheNewborn = true;
                birthRecord.EpiduralOrSpinalAnesthesia = true;
                birthRecord.AugmentationOfLabor = true;
                birthRecord.NoSpecifiedAbnormalConditionsOfNewborn = true;
                birthRecord.NoInfectionsPresentDuringPregnancy = true;
                birthRecord.GestationalHypertension = true;

                Dictionary<string, string> route = new Dictionary<string, string>();
                route.Add("code", "700000006");
                route.Add("system", "http://snomed.info/sct");
                route.Add("display", "Vaginal delivery of fetus (procedure)");
                birthRecord.FinalRouteAndMethodOfDelivery = route;

                birthRecord.NoObstetricProcedures = true;

                birthRecord.MotherBirthDay = 12;
                birthRecord.MotherBirthMonth = 1;
                birthRecord.MotherBirthYear = 1992;
                birthRecord.MotherDateOfBirth = "1992-01-12";
                birthRecord.FatherBirthDay = 21;
                birthRecord.FatherBirthMonth = 9;
                birthRecord.FatherBirthYear = 1990;
                birthRecord.FatherDateOfBirth = "1990-09-21";

                // TODO: add these back once correct codesystems are used for the component 
                // Ethnicity
                // birthRecord.MotherEthnicity3Helper = VR.ValueSets.HispanicNoUnknown.Yes;
                // // Race
                // Tuple<string, string>[] motherRace = { Tuple.Create(NvssRace.BlackOrAfricanAmerican, "Y")};
                // birthRecord.MotherRace = motherRace;
                // Tuple<string, string>[] fatherRace = { Tuple.Create(NvssRace.White, "Y")};
                // birthRecord.FatherRace = fatherRace;

                // 1. Write out the Record
                Console.WriteLine(birthRecord.ToJSON());

                return 0;
            }
            else if (args.Length == 2 && args[0] == "description") 
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[1]));
                Console.WriteLine(b.ToDescription());
                return 0;
            }
            else if (args.Length == 2 && args[0] == "2ije")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[1]));
                IJENatality ije1 = new IJENatality(b, false);
                Console.WriteLine(ije1.ToString());
                return 0;
            }
            else if (args.Length == 2 && args[0] == "2ijecontent")
            { // dumps content of a birth record in key/value IJE format
                BirthRecord b = new BirthRecord(File.ReadAllText(args[1]));
                IJENatality ije1 = new IJENatality(b, false);
                // Loop over every property (these are the fields); Order by priority
                List<PropertyInfo> properties = typeof(IJENatality).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Location).ToList();
                foreach (PropertyInfo property in properties)
                {
                    // Grab the field attributes
                    IJEField info = property.GetCustomAttribute<IJEField>();
                    // Grab the field value
                    string field = Convert.ToString(property.GetValue(ije1, null));
                    // Print the key/value pair to console
                    Console.WriteLine($"{info.Name}: {field.Trim()}");
                }
                return 0;
            }
            else if (args.Length == 2 && args[0] == "ije2json")
            {
                IJENatality ije1 = new IJENatality(File.ReadAllText(args[1]));
                BirthRecord b = ije1.ToRecord();
                Console.WriteLine(b.ToJSON());
                return 0;
            }
            else if (args.Length > 2 && args[0] == "ije2json")
            {
              // This command will export the files to the same directory they were imported from.
              for (int i = 1; i < args.Length; i++)
              {
                  string ijeFile = args[i];
                  string ijeRawRecord = File.ReadAllText(ijeFile);
                  IJENatality ije = new IJENatality(ijeRawRecord);
                  BirthRecord b = ije.ToRecord();
                  string outputFilename = ijeFile.Replace(".ije", ".json");
                  StreamWriter sw = new StreamWriter(outputFilename);
                  sw.WriteLine(b.ToJSON());
                  sw.Flush();
              }
              return 0;
            }
            else if (args.Length == 2 && args[0] == "ije2xml")
            {
                IJENatality ije1 = new IJENatality(File.ReadAllText(args[1]));
                BirthRecord b = ije1.ToRecord();
                Console.WriteLine(XDocument.Parse(b.ToXML()).ToString());
                return 0;
            }
            else if (args.Length == 2 && args[0] == "extract2ijecontent")
            {  // dumps content of a submission message in key/value IJE format
                BirthRecordBaseMessage message = BirthRecordBaseMessage.Parse(File.ReadAllText(args[1]), true);
                switch (message)
                {
                    case BirthRecordSubmissionMessage submission:
                        var b = submission.BirthRecord;
                        IJENatality ije1 = new IJENatality(b, false);
                        // Loop over every property (these are the fields); Order by priority
                        List<PropertyInfo> properties = typeof(IJENatality).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Location).ToList();
                        foreach (PropertyInfo property in properties)
                        {
                            // Grab the field attributes
                            IJEField info = property.GetCustomAttribute<IJEField>();
                            // Grab the field value
                            string field = Convert.ToString(property.GetValue(ije1, null));
                            // Print the key/value pair to console
                            Console.WriteLine($"{info.Name}: {field.Trim()}");
                        }
                        break;
                }
                return 0;
            }
            else if (args.Length == 2 && args[0] == "submit")
            {
                BirthRecord record = new BirthRecord(File.ReadAllText(args[1]));
                BirthRecordSubmissionMessage message = new BirthRecordSubmissionMessage(record);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length > 2 && args[0] == "submit")
            {
                string outputDirectory = args[1];
                if (!Directory.Exists(outputDirectory))
                {
                    Console.WriteLine("Must supply a valid output directory");
                    return (1);
                }
                for (int i = 2; i < args.Length; i++)
                {
                    string outputFilename = args[i].Replace(".json", "_submission.json");
                    BirthRecord record = new BirthRecord(File.ReadAllText(args[i]));
                    BirthRecordSubmissionMessage message = new BirthRecordSubmissionMessage(record);
                    message.MessageSource = "http://mitre.org/bfdr";
                    Console.WriteLine($"Writing record to {outputFilename}");
                    StreamWriter sw = new StreamWriter(outputFilename);
                    sw.WriteLine(message.ToJSON(true));
                    sw.Flush();
                }
                return 0;
            }

            else if (args.Length == 2 && args[0] == "resubmit")
            {
                BirthRecord record = new BirthRecord(File.ReadAllText(args[1]));
                BirthRecordUpdateMessage message = new BirthRecordUpdateMessage(record);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length == 2 && args[0] == "void")
            {
                BirthRecord record = new BirthRecord(File.ReadAllText(args[1]));
                BirthRecordVoidMessage message = new BirthRecordVoidMessage(record);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length == 3 && args[0] == "void")
            {
                BirthRecord record = new BirthRecord(File.ReadAllText(args[1]));
                BirthRecordVoidMessage message = new BirthRecordVoidMessage(record);
                message.BlockCount = UInt32.Parse(args[2]);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length == 2 && args[0] == "ack")
            {
                BirthRecordBaseMessage message = BirthRecordBaseMessage.Parse(File.ReadAllText(args[1]));
                BirthRecordAcknowledgementMessage ackMessage = new BirthRecordAcknowledgementMessage(message);
                Console.WriteLine(ackMessage.ToJSON(true));
                return 0;
            }
            else if (args.Length > 2 && args[0] == "ack")
            {
                string outputDirectory = args[1];
                if (!Directory.Exists(outputDirectory))
                {
                    Console.WriteLine("Must supply a valid output directory");
                    return (1);
                }
                for (int i = 2; i < args.Length; i++)
                {
                    string outputFilename = args[i].Replace(".json", "_acknowledgement.json");
                    BirthRecordBaseMessage message = BirthRecordBaseMessage.Parse(File.ReadAllText(args[i]));
                    BirthRecordAcknowledgementMessage ackMessage = new BirthRecordAcknowledgementMessage(message);
                    Console.WriteLine($"Writing acknowledgement to {outputFilename}");
                    StreamWriter sw = new StreamWriter(outputFilename);
                    sw.WriteLine(ackMessage.ToJSON(true));
                    sw.Flush();
                }
                return 0;
            }
          //  else if (args.Length >= 3 && args[0] == "batch")
          //  {
          //      string url = args[1];
          //      List<BirthRecordBaseMessage> messages = new List<BirthRecordBaseMessage>();
          //      for (int i = 2; i < args.Length; i++)
          //      {
          //          messages.Add(BirthRecordBaseMessage.Parse(File.ReadAllText(args[i])));
          //      }
          //      string payload = Client.CreateBulkUploadPayload(messages, url, true);
          //      Console.WriteLine(payload);
          //      return 0;
          //  }
           // else if (args.Length == 3 && args[0] == "filter")
           // {
           //     Console.WriteLine($"Filtering file {args[1]}");

           //     BirthRecordBaseMessage baseMessage = BirthRecordBaseMessage.Parse(File.ReadAllText(args[1]));
                
           //     FilterService FilterService = new FilterService("./BFDR.Filter/NCHSIJEFilter.json", "./BFDR.Filter/IJEToFHIRMapping.json");

           //     var filteredFile = FilterService.filterMessage(baseMessage).ToJson();
           //     BirthRecordBaseMessage.Parse(filteredFile);
           //     Console.WriteLine($"File successfully filtered and saved to {args[2]}");
                    
           //     File.WriteAllText(args[2], filteredFile);
                
           //     return 0;
           //  }
            else if (args.Length == 2 && args[0] == "ije")
            {
                string ijeString = File.ReadAllText(args[1]);
                List<PropertyInfo> properties = typeof(IJENatality).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();

                foreach (PropertyInfo property in properties)
                {
                    IJEField info = property.GetCustomAttribute<IJEField>();
                    string field = ijeString.Substring(info.Location - 1, info.Length);
                    Console.WriteLine($"{info.Field,-5} {info.Name,-15} {Truncate(info.Contents, 75),-75}: \"{field + "\"",-80}");
                }
            }
            else if (args[0] == "ijebuilder")
            {
                IJENatality ije = new IJENatality();
                foreach (string arg in args)
                {
                    string[] keyAndValue = arg.Split('=');
                    if (keyAndValue.Length == 2)
                    {
                        typeof(IJENatality).GetProperty(keyAndValue[0]).SetValue(ije, keyAndValue[1]);
                    }
                }
                BirthRecord b = ije.ToRecord();
                Console.WriteLine(b.ToJson());
            }
            else if (args.Length == 3 && args[0] == "compare")
            {
                string ijeString1 = File.ReadAllText(args[1]);

                BirthRecord record2 = new BirthRecord(File.ReadAllText(args[2]));
                IJENatality ije2 = new IJENatality(record2);
                string ijeString2 = ije2.ToString();

                List<PropertyInfo> properties = typeof(IJENatality).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();

                int differences = 0;

                foreach (PropertyInfo property in properties)
                {
                    IJEField info = property.GetCustomAttribute<IJEField>();
                    string field1 = ijeString1.Substring(info.Location - 1, info.Length);
                    string field2 = ijeString2.Substring(info.Location - 1, info.Length);
                    if (field1 != field2)
                    {
                        differences += 1;
                        Console.WriteLine($" IJE: {info.Field,-5} {info.Name,-15} {Truncate(info.Contents, 75),-75}: \"{field1 + "\"",-80}");
                        Console.WriteLine($"FHIR: {info.Field,-5} {info.Name,-15} {Truncate(info.Contents, 75),-75}: \"{field2 + "\"",-80}");
                        Console.WriteLine();
                    }
                }
                Console.WriteLine($"Differences detected: {differences}");
                return differences;
            }
            else if (args.Length == 2 && args[0] == "extract")
            {
                BirthRecordBaseMessage message = BirthRecordBaseMessage.Parse(File.ReadAllText(args[1]));
                BirthRecord record;
                switch (message)
                {
                    case BirthRecordSubmissionMessage submission:
                        record = submission.BirthRecord;
                        Console.WriteLine(record.ToJSON());
                        break;
                    case BirthRecordDemographicsCodingMessage coding:
                        record = coding.BirthRecord;
                        Console.WriteLine(record.ToJSON());
                        break;
                }
                return 0;
            }
            else if (args.Length == 2 && args[0] == "json2xml")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[1]));
                Console.WriteLine(XDocument.Parse(b.ToXML()).ToString());
                return 0;
            }
            else if (args.Length == 2 && args[0] == "checkXml")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[1]), true);
                Console.WriteLine(XDocument.Parse(b.ToXML()).ToString());
                return 0;
            }
            else if (args.Length == 2 && args[0] == "checkJson")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[1]), true);
                Console.WriteLine(b.ToJSON());
                return 0;
            }
            else if (args.Length == 2 && args[0] == "xml2json")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[1]));
                Console.WriteLine(b.ToJSON());
                return 0;
            }
            else if (args.Length == 2 && args[0] == "xml2xml")
            {
                // Forces record through getters and then setters, prints as xml
                BirthRecord indr = new BirthRecord(File.ReadAllText(args[1]));
                BirthRecord outdr = new BirthRecord();
                List<PropertyInfo> properties = typeof(BirthRecord).GetProperties().ToList();
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetCustomAttribute<Property>() != null)
                    {
                        property.SetValue(outdr, property.GetValue(indr));
                    }
                }
                Console.WriteLine(XDocument.Parse(outdr.ToXML()).ToString());
                return 0;
            }
            else if (args.Length == 2 && args[0] == "json2json")
            {
                // Forces record through getters and then setters, prints as JSON
                BirthRecord indr = new BirthRecord(File.ReadAllText(args[1]));
                BirthRecord outdr = new BirthRecord();
                List<PropertyInfo> properties = typeof(BirthRecord).GetProperties().ToList();
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetCustomAttribute<Property>() != null)
                    {
                        property.SetValue(outdr, property.GetValue(indr));
                    }
                }
                Console.WriteLine(outdr.ToJSON());
                return 0;
            }
            else if (args.Length == 2 && args[0] == "roundtrip-ije")
            {
                // Console.WriteLine("Converting FHIR to IJE...\n");
                BirthRecord b = new BirthRecord(File.ReadAllText(args[1]));
                IJENatality ije1, ije2, ije3;
                try
                {
                    ije1 = new IJENatality(b);
                    ije2 = new IJENatality(ije1.ToString());
                    ije3 = new IJENatality(new BirthRecord(ije2.ToRecord().ToXML()));
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    return (1);
                }

                int issues = 0;
                int total = 0;
                foreach (PropertyInfo property in typeof(IJENatality).GetProperties())
                {
                    string val1 = Convert.ToString(property.GetValue(ije1, null));
                    string val2 = Convert.ToString(property.GetValue(ije2, null));
                    string val3 = Convert.ToString(property.GetValue(ije3, null));

                    IJEField info = property.GetCustomAttribute<IJEField>();

                    if (val1.ToUpper() != val2.ToUpper() || val1.ToUpper() != val3.ToUpper() || val2.ToUpper() != val3.ToUpper())
                    {
                        issues++;
                        Console.WriteLine($"[***** MISMATCH *****]\t{info.Name}: {info.Contents} \t\t\"{val1}\" != \"{val2}\" != \"{val3}\"");
                    }
                    total++;
                }
                Console.WriteLine($"\n{issues} issues out of {total} total fields.");
                return issues;
            }
            else if (args.Length == 2 && args[0] == "roundtrip-all")
            {
                BirthRecord b1 = new BirthRecord(File.ReadAllText(args[1]));
                BirthRecord b2 = new BirthRecord(b1.ToJSON());
                BirthRecord b3 = new BirthRecord();
                List<PropertyInfo> properties = typeof(BirthRecord).GetProperties().ToList();
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetCustomAttribute<Property>() != null)
                    {
                        property.SetValue(b3, property.GetValue(b2));
                    }
                }

                int good = 0;
                int bad = 0;

                foreach (PropertyInfo property in properties)
                {
                    // Console.WriteLine($"Property: Name: {property.Name.ToString()} Type: {property.PropertyType.ToString()}");
                    string one;
                    string two;
                    string three;
                    if (property.PropertyType.ToString() == "System.Collections.Generic.Dictionary`2[System.String,System.String]")
                    {
                        Dictionary<string, string> oneDict = (Dictionary<string, string>)property.GetValue(b1);
                        Dictionary<string, string> twoDict = (Dictionary<string, string>)property.GetValue(b2);
                        Dictionary<string, string> threeDict = (Dictionary<string, string>)property.GetValue(b3);
                        // Ignore empty entries in the dictionary so they don't throw off comparisons.
                        one = String.Join(", ", oneDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                        two = String.Join(", ", twoDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                        three = String.Join(", ", threeDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                    }
                    else if (property.PropertyType.ToString() == "System.String[]")
                    {
                        one = String.Join(", ", (string[])property.GetValue(b1));
                        two = String.Join(", ", (string[])property.GetValue(b2));
                        three = String.Join(", ", (string[])property.GetValue(b3));
                    }
                    else
                    {
                        one = Convert.ToString(property.GetValue(b1));
                        two = Convert.ToString(property.GetValue(b2));
                        three = Convert.ToString(property.GetValue(b3));
                    }
                    if (one.ToLower() != three.ToLower())
                    {
                        Console.WriteLine("[***** MISMATCH *****]\t" + $"\"{one}\" (property: {property.Name}) does not equal \"{three}\"" + $"      1:\"{one}\" 2:\"{two}\" 3:\"{three}\"");
                        bad++;
                    }
                    else
                    {
                        // We don't actually need to see all the matches and it makes it hard to see the mismatches
                        // Console.WriteLine("[MATCH]\t" + $"\"{one}\" (property: {property.Name}) equals \"{three}\"" + $"      1:\"{one}\" 2:\"{two}\" 3:\"{three}\"");
                        good++;
                    }
                }
                Console.WriteLine($"\n{bad} mismatches out of {good + bad} total properties checked.");
                if (bad > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (args.Length == 2 && args[0] == "ije")
            {
                string ijeString = File.ReadAllText(args[1]);
                List<PropertyInfo> properties = typeof(IJENatality).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();

                foreach (PropertyInfo property in properties)
                {
                    IJEField info = property.GetCustomAttribute<IJEField>();
                    string field = ijeString.Substring(info.Location - 1, info.Length);
                    Console.WriteLine($"{info.Field,-5} {info.Name,-15} {Truncate(info.Contents, 75),-75}: \"{field + "\"",-80}");
                }
            }
            else if (args[0] == "ijebuilder")
            {
                IJENatality ije = new IJENatality();
                foreach (string arg in args)
                {
                    string[] keyAndValue = arg.Split('=');
                    if (keyAndValue.Length == 2)
                    {
                        typeof(IJENatality).GetProperty(keyAndValue[0]).SetValue(ije, keyAndValue[1]);
                    }
                }
                BirthRecord b = ije.ToRecord();
                Console.WriteLine(b.ToJson());
            }
            else if (args.Length == 3 && args[0] == "compare")
            {
                string ijeString1 = File.ReadAllText(args[1]);

                BirthRecord record2 = new BirthRecord(File.ReadAllText(args[2]));
                IJENatality ije2 = new IJENatality(record2);
                string ijeString2 = ije2.ToString();

                List<PropertyInfo> properties = typeof(IJENatality).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();

                int differences = 0;

                foreach (PropertyInfo property in properties)
                {
                    IJEField info = property.GetCustomAttribute<IJEField>();
                    string field1 = ijeString1.Substring(info.Location - 1, info.Length);
                    string field2 = ijeString2.Substring(info.Location - 1, info.Length);
                    if (field1 != field2)
                    {
                        differences += 1;
                        Console.WriteLine($" IJE: {info.Field,-5} {info.Name,-15} {Truncate(info.Contents, 75),-75}: \"{field1 + "\"",-80}");
                        Console.WriteLine($"FHIR: {info.Field,-5} {info.Name,-15} {Truncate(info.Contents, 75),-75}: \"{field2 + "\"",-80}");
                        Console.WriteLine();
                    }
                }
                Console.WriteLine($"Differences detected: {differences}");
                return differences;
            }
            else if (args.Length == 2 && args[0] == "extract")
            {
                BirthRecordBaseMessage message = BirthRecordBaseMessage.Parse(File.ReadAllText(args[1]));
                BirthRecord record;
                switch (message)
                {
                    case BirthRecordSubmissionMessage submission:
                        record = submission.BirthRecord;
                        Console.WriteLine(record.ToJSON());
                        break;
                    case BirthRecordDemographicsCodingMessage coding:
                        record = coding.BirthRecord;
                        Console.WriteLine(record.ToJSON());
                        break;
                }
                return 0;
            }
            else
            {
                Console.WriteLine($"**** No such command {args[0]} with the number of arguments supplied");
            }
            return 0;
        }

         private static string Truncate(string value, int length)
        {
            if (String.IsNullOrWhiteSpace(value) || value.Length <= length)
            {
                return value;
            }
            else
            {
                return value.Substring(0, length);
            }
        }
    }
}
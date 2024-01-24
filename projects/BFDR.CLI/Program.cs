using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using BFDR;

namespace BFDR.CLI
{
    class Program
    {
        static string commands =
@"* BFDR Command Line Interface - commands
  - help:  prints this help message  (no arguments)
  - fakerecord: prints a fake JSON birth record (no arguments)
  - submit: Create a submission FHIR message wrapping a FHIR birth record (1 argument: FHIR birth record)
  - resubmit: Create a submission update FHIR message wrapping a FHIR birth record (1 argument: FHIR birth record)
  - void: Creates a Void message for a Birth Record (1 argument: FHIR birth record; one optional argument: number of records to void)
  - ack: Create an acknowledgement FHIR message for a submission FHIR message (1 argument: submission FHIR message; many arguments: output directory and FHIR messages)
  - ije2json: creates an IJE birth record and prints out as JSON
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
                birthRecord.Identifier = "100";
                birthRecord.StateLocalIdentifier1 = "123";

                // 1. Write out the Record
                Console.WriteLine(birthRecord.ToJSON());

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
            else if (args.Length == 1 && args[0] == "ije2json")
            {
                IJENatality ije = new IJENatality();
                ije.FILENO = "111111";
                ije.ISEX = "M";
                BirthRecord b = ije.ToRecord();
                Console.WriteLine(b.ToJSON());
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
                BirthRecord b2 = new BirthRecord(d1.ToJSON());
                BirthRecord b3 = new BirthRecord();
                List<PropertyInfo> properties = typeof(BirthRecord).GetProperties().ToList();
                // HashSet<string> skipPropertyNames = new HashSet<string>() { "CausesOfDeath", "AgeAtDeathYears", "AgeAtDeathMonths", "AgeAtDeathDays", "AgeAtDeathHours", "AgeAtDeathMinutes" };
                foreach (PropertyInfo property in properties)
                {
                    // if (skipPropertyNames.Contains(property.Name))
                    // {
                    //     continue;
                    // }
                    if (property.GetCustomAttribute<Property>() != null)
                    {
                        property.SetValue(d3, property.GetValue(d2));
                    }
                }

                int good = 0;
                int bad = 0;

                foreach (PropertyInfo property in properties)
                {
                    if (skipPropertyNames.Contains(property.Name))
                    {
                        continue;
                    }
                    // Console.WriteLine($"Property: Name: {property.Name.ToString()} Type: {property.PropertyType.ToString()}");
                    string one;
                    string two;
                    string three;
                    if (property.PropertyType.ToString() == "System.Collections.Generic.Dictionary`2[System.String,System.String]")
                    {
                        Dictionary<string, string> oneDict = (Dictionary<string, string>)property.GetValue(d1);
                        Dictionary<string, string> twoDict = (Dictionary<string, string>)property.GetValue(d2);
                        Dictionary<string, string> threeDict = (Dictionary<string, string>)property.GetValue(d3);
                        // Ignore empty entries in the dictionary so they don't throw off comparisons.
                        one = String.Join(", ", oneDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                        two = String.Join(", ", twoDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                        three = String.Join(", ", threeDict.Select(x => (x.Value != "") ? (x.Key + "=" + x.Value) : ("")).ToArray()).Replace(" ,", "");
                    }
                    else if (property.PropertyType.ToString() == "System.String[]")
                    {
                        one = String.Join(", ", (string[])property.GetValue(d1));
                        two = String.Join(", ", (string[])property.GetValue(d2));
                        three = String.Join(", ", (string[])property.GetValue(d3));
                    }
                    else
                    {
                        one = Convert.ToString(property.GetValue(d1));
                        two = Convert.ToString(property.GetValue(d2));
                        three = Convert.ToString(property.GetValue(d3));
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
                List<PropertyInfo> properties = typeof(IJEMortality).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();

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

                List<PropertyInfo> properties = typeof(IJEMortality).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();

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
                BaseMessage message = BaseMessage.Parse(File.ReadAllText(args[1]));
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
            return 0;
        }
    }
}
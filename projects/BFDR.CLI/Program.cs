using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using Hl7.FhirPath;
using VR;

namespace BFDR.CLI
{
    class Program
    {
        static string commands =
@"* BFDR Command Line Interface - commands
  - help:  prints this help message  (no arguments)
  - fakerecord: prints a fake JSON record (1 argument: 'birth' or 'fetaldeath')
  - description: prints a verbose JSON description of the record (in the format used to drive Canary) (2 arguments: 'birth' or 'fetaldeath', the path to the record)
  - 2ije: Read in the FHIR XML or JSON record and print out as IJE (2 arguments: 'birth' or 'fetaldeath', path to record in JSON or XML format)
  - 2ijecontent: Read in the FHIR XML or JSON record and dump content in key/value IJE format (2 arguments: 'birth' or 'fetaldeath', path to record in JSON or XML format)
  - ije2json: Read in one or more IJE record(s) and print out as JSON (2 arguments: 'birth' or 'fetaldeath', path to record in IJE format)
  - ije2xml: Read in the IJE record and print out as XML (2 arguments: 'birth' or 'fetaldeath', path to record in IJE format)
  - extract2ijecontent: Dump content of a submission message in key/value IJE format (2 arguments: 'birth' or 'fetaldeath', submission message)
  - submit: Create a submission FHIR message wrapping a FHIR record (2 arguments: 'birth' or 'fetaldeath', FHIR record)
  - resubmit: Create a submission update FHIR message wrapping a FHIR record (2 arguments: 'birth' or 'fetaldeath', FHIR record)
  - void: Creates a Void message for a record (2 arguments: FHIR record, 'birth' or 'fetaldeath'; 1 optional argument: number of records to void)
  - ack: Create an acknowledgement FHIR message for a submission FHIR message (2 or more arguments: 'birth' or 'fetaldeath', submission FHIR message OR output directory, FHIR messages)
  - json2xml: Read in the FHIR JSON record, completely disassemble then reassemble, and print as FHIR XML (2 arguments: 'birth' or 'fetaldeath', FHIR JSON Record)
  - checkXml: Read in the given FHIR xml (being permissive) and print out the same; useful for doing validation diffs (2 arguments: 'birth' or 'fetaldeath', FHIR XML file)
  - checkJson: Read in the given FHIR json (being permissive) and print out the same; useful for doing validation diffs (2 arguments: 'birth' or 'fetaldeath', FHIR JSON file)
  - xml2json: Read in the IJE record and print out as JSON (2 arguments: 'birth' or 'fetaldeath', path to record in XML format)
  - xml2xml: Read in the IJE record and print out as XML (2 arguments: 'birth' or 'fetaldeath', path to record in XML format)
  - json2json: Read in the FHIR JSON record, completely disassemble then reassemble, and print as FHIR JSON (2 arguments: 'birth' or 'fetaldeath', FHIR JSON Record)
  - roundtrip-ije: Convert a record to IJE and back and check field by field to identify any conversion issues (2 arguments: 'birth' or 'fetaldeath', FHIR Record)
  - roundtrip-all: Convert a record to JSON and back and check field by field to identify any conversion issues (2 arguments: 'birth' or 'fetaldeath', FHIR Record)
  - ije: Read in and parse an IJE record and print out the values for every (supported) field (2 arguments: 'birth' or 'fetaldeath', path to record in IJE format)
  - ijebuilder: Create json record using IJE (natality) mapped fields (1 argument: 'birth' or 'fetaldeath')
  - compare: Compare an IJE record with a FHIR record by each IJE field (3 arguments: 'birth' or 'fetaldeath', IJE record, FHIR Record)
  - extract: Extract a FHIR record from a FHIR message (1 argument: FHIR message)
  - nre2json: Creates a Demographic Coding Bundle from a NRE Message (1 argument: TRX file)
    ";

        static int Main(string[] args)
        {
            if ((args.Length == 0) || ((args.Length == 1) && (args[0] == "help")))
            {
                Console.WriteLine(commands);
                return (0);
            }
            else if (args.Length == 2 && args[0] == "fakerecord" && args[1] == "birth")
            {
                // 0. Set up a BirthRecord object
                BirthRecord birthRecord = new BirthRecord();
                birthRecord.DateOfBirth = "2023-01-02";
                birthRecord.CertificateNumber = "100";
                birthRecord.StateLocalIdentifier1 = "123";
                birthRecord.BirthSex = "M";

                string[] childNames = { "Alexander", "Arlo" };
                birthRecord.ChildGivenNames = childNames;
                string[] motherName = { "Xenia" };
                birthRecord.MotherGivenNames = motherName;
                string lastName = "Adkins";
                birthRecord.ChildFamilyName = lastName;
                birthRecord.MotherFamilyName = lastName;

                birthRecord.CertifierName = "Janet Seito";
                birthRecord.CertifierNPI = "223347044";
                birthRecord.CertifierTitleHelper = "76231001";
                birthRecord.AttendantName = "Avery Jones";
                birthRecord.AttendantNPI = "762310012345";
                birthRecord.AttendantTitleHelper = "76231001";


                birthRecord.EventLocationJurisdiction = "MA";
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
                birthRecord.CyanoticCongenitalHeartDisease = true;
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

                // Mother Residence
                Dictionary<string, string> motherResidence = new Dictionary<string, string>();
                motherResidence.Add("addressLine1", "7 Blue Street");
                motherResidence.Add("addressCity", "Milford");
                motherResidence.Add("addressCounty", "New Haven");
                motherResidence.Add("addressState", "CT");
                motherResidence.Add("addressZip", "06460");
                motherResidence.Add("addressCountry", "US");
                birthRecord.MotherResidence = motherResidence;

                // Mother and Father birthdate
                birthRecord.MotherDateOfBirth = "1992-01-12";
                birthRecord.FatherDateOfBirth = "1990-09-21";

                // Mother and Father education
                birthRecord.MotherEducationLevelHelper = VR.ValueSets.EducationLevel.Doctoral_Or_Post_Graduate_Education;
                birthRecord.MotherEducationLevelEditFlagHelper = VR.ValueSets.EditBypass01234.Edit_Passed;
                birthRecord.FatherEducationLevelHelper = VR.ValueSets.EducationLevel.Bachelors_Degree;
                birthRecord.FatherEducationLevelEditFlagHelper = VR.ValueSets.EditBypass01234.Edit_Failed_Data_Queried_But_Not_Verified;

                // Mother and Father Ethnicity
                birthRecord.MotherEthnicityLiteral = "Colombian";
                birthRecord.MotherEthnicity3Helper = VR.ValueSets.YesNoUnknown.Yes;
                // Mother and Father Race
                Tuple<string, string>[] motherRace = { Tuple.Create(NvssRace.BlackOrAfricanAmerican, "Y") };
                birthRecord.MotherRace = motherRace;
                Tuple<string, string>[] fatherRace = { Tuple.Create(NvssRace.White, "Y") };
                birthRecord.FatherRace = fatherRace;


                // Write out the Record
                Console.WriteLine(birthRecord.ToJSON());

                return 0;
            }
            else if (args.Length == 2 && args[0] == "fakerecord" && args[1] == "fetaldeath")
            {
                // 0. Set up a FetalDeathRecord object
                FetalDeathRecord fetaldeathRecord = new FetalDeathRecord();
                fetaldeathRecord.DateOfDelivery = "2023-01-01";
                fetaldeathRecord.CertificateNumber = "100";
                fetaldeathRecord.StateLocalIdentifier1 = "123";
                fetaldeathRecord.DateOfDelivery = "2023-01-01";
                fetaldeathRecord.FetalDeathSex = "M";

                string[] fetusNames = { "Alexander", "Arlo" };
                fetaldeathRecord.FetusGivenNames = fetusNames;
                string[] motherName = { "Xenia" };
                fetaldeathRecord.MotherGivenNames = motherName;
                string lastName = "Adkins";
                fetaldeathRecord.FetusFamilyName = lastName;
                fetaldeathRecord.MotherFamilyName = lastName;

                fetaldeathRecord.CertifierName = "Janet Seito";
                fetaldeathRecord.CertifierNPI = "223347044";
                fetaldeathRecord.CertifierTitleHelper = "76231001";
                fetaldeathRecord.AttendantName = "Avery Jones";
                fetaldeathRecord.AttendantNPI = "762310012345";
                fetaldeathRecord.AttendantTitleHelper = "76231001";


                fetaldeathRecord.EventLocationJurisdiction = "MA";
                Dictionary<string, string> deliveryAddress = new Dictionary<string, string>();
                deliveryAddress.Add("addressLine1", "123 Fake Street");
                deliveryAddress.Add("addressCity", "Springfield");
                deliveryAddress.Add("addressCounty", "Hampden");
                deliveryAddress.Add("addressState", "MA");
                deliveryAddress.Add("addressZip", "01101");
                deliveryAddress.Add("addressCountry", "US");
                fetaldeathRecord.PlaceOfDelivery = deliveryAddress;

                fetaldeathRecord.InfantMedicalRecordNumber = "7134703";
                fetaldeathRecord.MotherMedicalRecordNumber = "2286144";
                fetaldeathRecord.MotherSocialSecurityNumber = "133756482";

                fetaldeathRecord.FetalDeathSetOrder = null;
                fetaldeathRecord.FetalDeathPlurality = null;
                fetaldeathRecord.EpiduralOrSpinalAnesthesia = true;
                fetaldeathRecord.AugmentationOfLabor = true;
                fetaldeathRecord.NoSpecifiedAbnormalConditionsOfNewborn = true;
                fetaldeathRecord.NoInfectionsPresentDuringPregnancy = true;
                fetaldeathRecord.GestationalHypertension = true;

                // Initiating Cause or Condition
                fetaldeathRecord.PrematureRuptureOfMembranes = true;

                Dictionary<string, string> route = new Dictionary<string, string>();
                route.Add("code", "700000006");
                route.Add("system", "http://snomed.info/sct");
                route.Add("display", "Vaginal delivery of fetus (procedure)");
                fetaldeathRecord.FinalRouteAndMethodOfDelivery = route;
                fetaldeathRecord.NoObstetricProcedures = true;

                // Mother Residence
                Dictionary<string, string> motherResidence = new Dictionary<string, string>();
                motherResidence.Add("addressLine1", "7 Blue Street");
                motherResidence.Add("addressCity", "Milford");
                motherResidence.Add("addressCounty", "New Haven");
                motherResidence.Add("addressState", "CT");
                motherResidence.Add("addressZip", "06460");
                motherResidence.Add("addressCountry", "US");
                fetaldeathRecord.MotherResidence = motherResidence;

                // Mother and Father birthdates
                fetaldeathRecord.MotherDateOfBirth = "1992-01-12";
                fetaldeathRecord.FatherDateOfBirth = "1990-09-21";

                // Mother and Father Education
                fetaldeathRecord.MotherEducationLevelHelper = VR.ValueSets.EducationLevel.Doctoral_Or_Post_Graduate_Education;
                fetaldeathRecord.MotherEducationLevelEditFlagHelper = VR.ValueSets.EditBypass01234.Edit_Passed;
                fetaldeathRecord.FatherEducationLevelHelper = VR.ValueSets.EducationLevel.Bachelors_Degree;
                fetaldeathRecord.FatherEducationLevelEditFlagHelper = VR.ValueSets.EditBypass01234.Edit_Failed_Data_Queried_But_Not_Verified;

                // Mother and Father Ethnicity
                fetaldeathRecord.MotherEthnicityLiteral = "Colombian";
                fetaldeathRecord.MotherEthnicity3Helper = VR.ValueSets.YesNoUnknown.Yes;
                // Mother and Father Race
                Tuple<string, string>[] motherRace = { Tuple.Create(NvssRace.BlackOrAfricanAmerican, "Y") };
                fetaldeathRecord.MotherRace = motherRace;
                Tuple<string, string>[] fatherRace = { Tuple.Create(NvssRace.White, "Y") };
                fetaldeathRecord.FatherRace = fatherRace;


                // Write out the Record
                Console.WriteLine(fetaldeathRecord.ToJSON());

                return 0;
            }
            else if (args.Length == 3 && args[0] == "description" && args[1] == "birth")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[2]));
                Console.WriteLine(b.ToDescription());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "description" && args[1] == "fetaldeath")
            {
                FetalDeathRecord d = new FetalDeathRecord(File.ReadAllText(args[2]));
                Console.WriteLine(d.ToDescription());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "2ije" && args[1] == "birth")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[2]));
                IJEBirth ije1 = new IJEBirth(b, false);
                Console.WriteLine(ije1.ToString());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "2ije" && args[1] == "fetaldeath")
            {
                FetalDeathRecord d = new FetalDeathRecord(File.ReadAllText(args[2]));
                IJEFetalDeath ije1 = new IJEFetalDeath(d, false);
                Console.WriteLine(ije1.ToString());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "2ijecontent" && args[1] == "birth")
            { // dumps content of a birth record in key/value IJE format
                BirthRecord b = new BirthRecord(File.ReadAllText(args[2]));
                IJEBirth ije1 = new IJEBirth(b, false);
                // Loop over every property (these are the fields); Order by priority
                List<PropertyInfo> properties = typeof(IJEBirth).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Location).ToList();
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
            else if (args.Length == 3 && args[0] == "2ijecontent" && args[1] == "fetaldeath")
            { // dumps content of a fetaldeath record in key/value IJE format
                FetalDeathRecord d = new FetalDeathRecord(File.ReadAllText(args[2]));
                IJEFetalDeath ije1 = new IJEFetalDeath(d, false);
                // Loop over every property (these are the fields); Order by priority
                List<PropertyInfo> properties = typeof(IJEFetalDeath).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Location).ToList();
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
            else if (args.Length == 3 && args[0] == "ije2json" && args[1] == "birth")
            {
                IJEBirth ije1 = new IJEBirth(File.ReadAllText(args[2]));
                BirthRecord b = ije1.ToRecord();
                Console.WriteLine(b.ToJSON());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "ije2json" && args[1] == "fetaldeath")
            {
                IJEFetalDeath ije1 = new IJEFetalDeath(File.ReadAllText(args[2]));
                FetalDeathRecord b = ije1.ToRecord();
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
                    IJEBirth ije = new IJEBirth(ijeRawRecord);
                    BirthRecord b = ije.ToRecord();
                    string outputFilename = ijeFile.Replace(".ije", ".json");
                    StreamWriter sw = new StreamWriter(outputFilename);
                    sw.WriteLine(b.ToJSON());
                    sw.Flush();
                }
                return 0;
            }
            else if (args.Length == 3 && args[0] == "ije2xml" && args[1] == "birth")
            {
                IJEBirth ije1 = new IJEBirth(File.ReadAllText(args[2]));
                BirthRecord b = ije1.ToRecord();
                Console.WriteLine(XDocument.Parse(b.ToXML()).ToString());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "ije2xml" && args[1] == "fetaldeath")
            {
                IJEFetalDeath ije1 = new IJEFetalDeath(File.ReadAllText(args[2]));
                FetalDeathRecord d = ije1.ToRecord();
                Console.WriteLine(XDocument.Parse(d.ToXML()).ToString());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "extract2ijecontent" && args[1] == "birth")
            {  // dumps content of a submission message in key/value IJE format
                BFDRBaseMessage message = BFDRBaseMessage.Parse(File.ReadAllText(args[2]), true);
                switch (message)
                {
                    case BirthRecordSubmissionMessage submission:
                        var b = submission.BirthRecord;
                        IJEBirth ije1 = new IJEBirth(b, false);
                        // Loop over every property (these are the fields); Order by priority
                        List<PropertyInfo> properties = typeof(IJEBirth).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Location).ToList();
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
            else if (args.Length == 3 && args[0] == "extract2ijecontent" && args[1] == "fetaldeath")
            {  // dumps content of a submission message in key/value IJE format
                BFDRBaseMessage message = BFDRBaseMessage.Parse(File.ReadAllText(args[2]), true);
                switch (message)
                {
                    case FetalDeathRecordSubmissionMessage submission:
                        var b = submission.FetalDeathRecord;
                        IJEFetalDeath ije1 = new IJEFetalDeath(b, false);
                        // Loop over every property (these are the fields); Order by priority
                        List<PropertyInfo> properties = typeof(IJEFetalDeath).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Location).ToList();
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
            else if (args.Length == 3 && args[0] == "submit" && args[1] == "birth")
            {
                BirthRecord record = new BirthRecord(File.ReadAllText(args[2]));
                BirthRecordSubmissionMessage message = new BirthRecordSubmissionMessage(record);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length == 3 && args[0] == "submit" && args[1] == "fetaldeath")
            {
                FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(args[2]));
                FetalDeathRecordSubmissionMessage message = new FetalDeathRecordSubmissionMessage(record);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length > 3 && args[0] == "submit" && args[1] == "birth")
            {
                string outputDirectory = args[2];
                if (!Directory.Exists(outputDirectory))
                {
                    Console.WriteLine("Must supply a valid output directory");
                    return (1);
                }
                for (int i = 3; i < args.Length; i++)
                {
                    string outputFilename = Path.GetFileName(args[i]).Replace(".json", "_submission.json");
                    string outputPath = Path.Join(outputDirectory, outputFilename);
                    BirthRecord record = new BirthRecord(File.ReadAllText(args[i]));
                    BirthRecordSubmissionMessage message = new BirthRecordSubmissionMessage(record);
                    message.MessageSource = "http://mitre.org/bfdr";
                    Console.WriteLine($"Writing record to {outputPath}");
                    StreamWriter sw = new StreamWriter(outputPath);
                    sw.WriteLine(message.ToJSON(true));
                    sw.Flush();
                }
                return 0;
            }
            else if (args.Length > 3 && args[0] == "submit" && args[1] == "fetaldeath")
            {
                string outputDirectory = args[2];
                if (!Directory.Exists(outputDirectory))
                {
                    Console.WriteLine("Must supply a valid output directory");
                    return (1);
                }
                for (int i = 3; i < args.Length; i++)
                {
                    string outputFilename = Path.GetFileName(args[i]).Replace(".json", "_submission.json");
                    string outputPath = Path.Join(outputDirectory, outputFilename);
                    FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(args[i]));
                    FetalDeathRecordSubmissionMessage message = new FetalDeathRecordSubmissionMessage(record);
                    message.MessageSource = "http://mitre.org/bfdr";
                    Console.WriteLine($"Writing record to {outputPath}");
                    StreamWriter sw = new StreamWriter(outputPath);
                    sw.WriteLine(message.ToJSON(true));
                    sw.Flush();
                }
                return 0;
            }
            else if (args.Length == 3 && args[0] == "resubmit" && args[1] == "birth")
            {
                BirthRecord record = new BirthRecord(File.ReadAllText(args[2]));
                BirthRecordUpdateMessage message = new BirthRecordUpdateMessage(record);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length == 3 && args[0] == "resubmit" && args[1] == "fetaldeath")
            {
                FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(args[2]));
                FetalDeathRecordUpdateMessage message = new FetalDeathRecordUpdateMessage(record);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length == 3 && args[0] == "void" && args[1] == "birth")
            {
                BirthRecord record = new BirthRecord(File.ReadAllText(args[2]));
                BirthRecordVoidMessage message = new BirthRecordVoidMessage(record);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length == 4 && args[0] == "void" && args[1] == "birth")
            {
                BirthRecord record = new BirthRecord(File.ReadAllText(args[2]));
                BirthRecordVoidMessage message = new BirthRecordVoidMessage(record);
                message.BlockCount = UInt32.Parse(args[3]);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length == 3 && args[0] == "void" && args[1] == "fetaldeath")
            {
                FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(args[2]));
                FetalDeathRecordVoidMessage message = new FetalDeathRecordVoidMessage(record);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length == 4 && args[0] == "void" && args[1] == "fetaldeath")
            {
                FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(args[2]));
                FetalDeathRecordVoidMessage message = new FetalDeathRecordVoidMessage(record);
                message.BlockCount = UInt32.Parse(args[3]);
                message.MessageSource = "http://mitre.org/bfdr";
                Console.WriteLine(message.ToJSON(true));
                return 0;
            }
            else if (args.Length == 3 && args[0] == "ack" && args[1] == "birth")
            {
                BFDRBaseMessage message = BFDRBaseMessage.Parse(File.ReadAllText(args[2]));
                BirthRecordAcknowledgementMessage ackMessage = new BirthRecordAcknowledgementMessage(message);
                Console.WriteLine(ackMessage.ToJSON(true));
                return 0;
            }
            else if (args.Length > 3 && args[0] == "ack" && args[1] == "birth")
            {
                string outputDirectory = args[2];
                if (!Directory.Exists(outputDirectory))
                {
                    Console.WriteLine("Must supply a valid output directory");
                    return (1);
                }
                for (int i = 3; i < args.Length; i++)
                {
                    string outputFilename = Path.GetFileName(args[i]).Replace(".json", "_acknowledgement.json");
                    string outputPath = Path.Join(outputDirectory, outputFilename);
                    BFDRBaseMessage message = BFDRBaseMessage.Parse(File.ReadAllText(args[i]));
                    BirthRecordAcknowledgementMessage ackMessage = new BirthRecordAcknowledgementMessage(message);
                    Console.WriteLine($"Writing acknowledgement to {outputPath}");
                    StreamWriter sw = new StreamWriter(outputPath);
                    sw.WriteLine(ackMessage.ToJSON(true));
                    sw.Flush();
                }
                return 0;
            }
            else if (args.Length == 3 && args[0] == "ack" && args[1] == "fetaldeath")
            {
                BFDRBaseMessage message = BFDRBaseMessage.Parse(File.ReadAllText(args[2]));
                FetalDeathRecordAcknowledgementMessage ackMessage = new FetalDeathRecordAcknowledgementMessage(message);
                Console.WriteLine(ackMessage.ToJSON(true));
                return 0;
            }
            else if (args.Length > 3 && args[0] == "ack" && args[1] == "fetaldeath")
            {
                string outputDirectory = args[2];
                if (!Directory.Exists(outputDirectory))
                {
                    Console.WriteLine("Must supply a valid output directory");
                    return (1);
                }
                for (int i = 3; i < args.Length; i++)
                {
                    string outputFilename = Path.GetFileName(args[i]).Replace(".json", "_acknowledgement.json");
                    string outputPath = Path.Join(outputDirectory, outputFilename);
                    BFDRBaseMessage message = BFDRBaseMessage.Parse(File.ReadAllText(args[i]));
                    FetalDeathRecordAcknowledgementMessage ackMessage = new FetalDeathRecordAcknowledgementMessage(message);
                    Console.WriteLine($"Writing acknowledgement to {outputPath}");
                    StreamWriter sw = new StreamWriter(outputPath);
                    sw.WriteLine(ackMessage.ToJSON(true));
                    sw.Flush();
                }
                return 0;
            }

            //  else if (args.Length >= 3 && args[0] == "batch")
            //  {
            //      string url = args[1];
            //      List<BFDRBaseMessage> messages = new List<BFDRBaseMessage>();
            //      for (int i = 2; i < args.Length; i++)
            //      {
            //          messages.Add(BFDRBaseMessage.Parse(File.ReadAllText(args[i])));
            //      }
            //      string payload = Client.CreateBulkUploadPayload(messages, url, true);
            //      Console.WriteLine(payload);
            //      return 0;
            //  }
            // else if (args.Length == 3 && args[0] == "filter")
            // {
            //     Console.WriteLine($"Filtering file {args[1]}");

            //     BFDRBaseMessage baseMessage = BFDRBaseMessage.Parse(File.ReadAllText(args[1]));

            //     FilterService FilterService = new FilterService("./BFDR.Filter/NCHSIJEFilter.json", "./BFDR.Filter/IJEToFHIRMapping.json");

            //     var filteredFile = FilterService.filterMessage(baseMessage).ToJson();
            //     BFDRBaseMessage.Parse(filteredFile);
            //     Console.WriteLine($"File successfully filtered and saved to {args[2]}");

            //     File.WriteAllText(args[2], filteredFile);

            //     return 0;
            //  }
            else if (args.Length == 3 && args[0] == "ije" && args[1] == "birth")
            {

                IJEBirth ije1 = new IJEBirth(File.ReadAllText(args[2]));

                // Loop over every property (these are the fields); Order by priority
                List<PropertyInfo> properties = typeof(IJEBirth).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();
                foreach (PropertyInfo property in properties)
                {
                    // Grab the field attributes
                    IJEField info = property.GetCustomAttribute<IJEField>();
                    // Grab the field value
                    string field = Convert.ToString(property.GetValue(ije1, null));
                    // Print the key/value pair to console
                    Console.WriteLine($"{info.Field,-5} {info.Name,-15} {VR.IJE.Truncate(info.Contents, 75),-75}: \"{field + "\"",-80}");
                }
                return 0;
            }
            else if (args.Length == 3 && args[0] == "ije" && args[1] == "fetaldeath")
            {
                IJEFetalDeath ije1 = new IJEFetalDeath(File.ReadAllText(args[2]));

                // Loop over every property (these are the fields); Order by priority
                List<PropertyInfo> properties = typeof(IJEFetalDeath).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();
                foreach (PropertyInfo property in properties)
                {
                    // Grab the field attributes
                    IJEField info = property.GetCustomAttribute<IJEField>();
                    // Grab the field value
                    string field = Convert.ToString(property.GetValue(ije1, null));
                    // Print the key/value pair to console
                    Console.WriteLine($"{info.Field,-5} {info.Name,-15} {VR.IJE.Truncate(info.Contents, 75),-75}: \"{field + "\"",-80}");
                }
                return 0;
            }
            else if (args[0] == "ijebuilder" && args[1] == "birth")
            {
                IJEBirth ije = new IJEBirth();
                foreach (string arg in args)
                {
                    string[] keyAndValue = arg.Split('=');
                    if (keyAndValue.Length == 2)
                    {
                        typeof(IJEBirth).GetProperty(keyAndValue[0]).SetValue(ije, keyAndValue[1]);
                    }
                }
                BirthRecord b = ije.ToRecord();
                Console.WriteLine(b.ToJson());
            }
            else if (args[0] == "ijebuilder" && args[1] == "fetaldeath")
            {
                IJEFetalDeath ije = new IJEFetalDeath();
                foreach (string arg in args)
                {
                    string[] keyAndValue = arg.Split('=');
                    if (keyAndValue.Length == 2)
                    {
                        typeof(IJEFetalDeath).GetProperty(keyAndValue[0]).SetValue(ije, keyAndValue[1]);
                    }
                }
                FetalDeathRecord d = ije.ToRecord();
                Console.WriteLine(d.ToJson());
            }
            else if (args.Length == 4 && args[0] == "compare" && args[1] == "birth")
            {
                IJEBirth ije1 = new IJEBirth(File.ReadAllText(args[2]));
                IJEBirth ije2 = new IJEBirth(File.ReadAllText(args[3]));

                // Loop over every property (these are the fields); Order by priority
                List<PropertyInfo> properties = typeof(IJEBirth).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();
                int differences = 0;

                foreach (PropertyInfo property in properties)
                {
                    // Grab the field attributes
                    IJEField info = property.GetCustomAttribute<IJEField>();
                    // Grab the field value
                    string field1 = Convert.ToString(property.GetValue(ije1, null));
                    string field2 = Convert.ToString(property.GetValue(ije2, null));

                    if (field1 != field2)
                    {
                        differences += 1;
                        Console.WriteLine($" IJE: {info.Field,-5} {info.Name,-15} {VR.IJE.Truncate(info.Contents, 75),-75}: \"{field1 + "\"",-80}");
                        Console.WriteLine($"FHIR: {info.Field,-5} {info.Name,-15} {VR.IJE.Truncate(info.Contents, 75),-75}: \"{field2 + "\"",-80}");
                        Console.WriteLine();
                    }
                }
                Console.WriteLine($"Differences detected: {differences}");
                return differences;
            }
            else if (args.Length == 4 && args[0] == "compare" && args[1] == "fetaldeath")
            {
                IJEFetalDeath ije1 = new IJEFetalDeath(File.ReadAllText(args[2]));
                IJEFetalDeath ije2 = new IJEFetalDeath(File.ReadAllText(args[3]));

                // Loop over every property (these are the fields); Order by priority
                List<PropertyInfo> properties = typeof(IJEFetalDeath).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();
                int differences = 0;

                foreach (PropertyInfo property in properties)
                {
                    // Grab the field attributes
                    IJEField info = property.GetCustomAttribute<IJEField>();
                    // Grab the field value
                    string field1 = Convert.ToString(property.GetValue(ije1, null));
                    string field2 = Convert.ToString(property.GetValue(ije2, null));

                    if (field1 != field2)
                    {
                        differences += 1;
                        Console.WriteLine($" IJE: {info.Field,-5} {info.Name,-15} {VR.IJE.Truncate(info.Contents, 75),-75}: \"{field1 + "\"",-80}");
                        Console.WriteLine($"FHIR: {info.Field,-5} {info.Name,-15} {VR.IJE.Truncate(info.Contents, 75),-75}: \"{field2 + "\"",-80}");
                        Console.WriteLine();
                    }
                }
                Console.WriteLine($"Differences detected: {differences}");
                return differences;
            }
            else if (args.Length == 2 && args[0] == "extract")
            {
                BFDRBaseMessage message = BFDRBaseMessage.Parse(File.ReadAllText(args[1]));
                NatalityRecord record;
                switch (message)
                {
                    case BirthRecordSubmissionMessage submission:
                        record = submission.BirthRecord;
                        Console.WriteLine(record.ToJSON());
                        break;
                    case FetalDeathRecordSubmissionMessage submission:
                        record = submission.FetalDeathRecord;
                        Console.WriteLine(record.ToJSON());
                        break;
                    case CodedCauseOfFetalDeathMessage codmessage:
                        record = codmessage.FetalDeathRecord;
                        Console.WriteLine(record.ToJSON());
                        break;
                    case BirthRecordParentalDemographicsCodingMessage coding:
                        record = coding.NatalityRecord;
                        Console.WriteLine(record.ToJSON());
                        break;
                }
                return 0;
            }
            else if (args.Length == 3 && args[0] == "json2xml" && args[1] == "birth")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[2]));
                Console.WriteLine(XDocument.Parse(b.ToXML()).ToString());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "json2xml" && args[1] == "fetaldeath")
            {
                FetalDeathRecord b = new FetalDeathRecord(File.ReadAllText(args[2]));
                Console.WriteLine(XDocument.Parse(b.ToXML()).ToString());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "checkXml" && args[1] == "birth")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[2]), true);
                Console.WriteLine(XDocument.Parse(b.ToXML()).ToString());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "checkXml" && args[1] == "fetaldeath")
            {
                FetalDeathRecord b = new FetalDeathRecord(File.ReadAllText(args[2]), true);
                Console.WriteLine(XDocument.Parse(b.ToXML()).ToString());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "checkJson" && args[1] == "birth")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[2]), true);
                Console.WriteLine(b.ToJSON());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "checkJson" && args[1] == "fetaldeath")
            {
                FetalDeathRecord b = new FetalDeathRecord(File.ReadAllText(args[2]), true);
                Console.WriteLine(b.ToJSON());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "xml2json" && args[1] == "birth")
            {
                BirthRecord b = new BirthRecord(File.ReadAllText(args[2]));
                Console.WriteLine(b.ToJSON());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "xml2json" && args[1] == "fetaldeath")
            {
                FetalDeathRecord b = new FetalDeathRecord(File.ReadAllText(args[2]));
                Console.WriteLine(b.ToJSON());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "xml2xml" && args[1] == "birth")
            {
                // Forces record through getters and then setters, prints as xml
                BirthRecord indr = new BirthRecord(File.ReadAllText(args[2]));
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
            else if (args.Length == 3 && args[0] == "xml2xml" && args[1] == "fetaldeath")
            {
                // Forces record through getters and then setters, prints as xml
                FetalDeathRecord indr = new FetalDeathRecord(File.ReadAllText(args[2]));
                FetalDeathRecord outdr = new FetalDeathRecord();
                List<PropertyInfo> properties = typeof(FetalDeathRecord).GetProperties().ToList();
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
            else if (args.Length == 3 && args[0] == "json2json" && args[1] == "birth")
            {
                // Forces record through getters and then setters, prints as JSON
                BirthRecord inbr = new BirthRecord(File.ReadAllText(args[2]));
                BirthRecord outbr = new BirthRecord();
                List<PropertyInfo> properties = typeof(BirthRecord).GetProperties().ToList();
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetCustomAttribute<Property>() != null)
                    {
                        property.SetValue(outbr, property.GetValue(inbr));
                    }
                }
                Console.WriteLine(outbr.ToJSON());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "json2json" && args[1] == "fetaldeath")
            {
                // Forces record through getters and then setters, prints as JSON
                FetalDeathRecord inbr = new FetalDeathRecord(File.ReadAllText(args[2]));
                FetalDeathRecord outbr = new FetalDeathRecord();
                List<PropertyInfo> properties = typeof(FetalDeathRecord).GetProperties().ToList();
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetCustomAttribute<Property>() != null)
                    {
                        property.SetValue(outbr, property.GetValue(inbr));
                    }
                }
                Console.WriteLine(outbr.ToJSON());
                return 0;
            }
            else if (args.Length == 3 && args[0] == "roundtrip-ije" && args[1] == "birth")
            {
                // Console.WriteLine("Converting FHIR to IJE...\n");
                BirthRecord b = new BirthRecord(File.ReadAllText(args[2]));
                IJEBirth ije1, ije2, ije3;
                try
                {
                    ije1 = new IJEBirth(b);
                    ije2 = new IJEBirth(ije1.ToString());
                    ije3 = new IJEBirth(new BirthRecord(ije2.ToRecord().ToXML()));
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    return (1);
                }

                int issues = 0;
                int total = 0;
                foreach (PropertyInfo property in typeof(IJEBirth).GetProperties())
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
            else if (args.Length == 3 && args[0] == "roundtrip-ije" && args[1] == "fetaldeath")
            {
                // Console.WriteLine("Converting FHIR to IJE...\n");

                FetalDeathRecord b = new FetalDeathRecord(File.ReadAllText(args[2]));
                IJEFetalDeath ije1, ije2, ije3;
                try
                {
                    ije1 = new IJEFetalDeath(b);
                    ije2 = new IJEFetalDeath(ije1.ToString());
                    ije3 = new IJEFetalDeath(new FetalDeathRecord(ije2.ToRecord().ToXML()));
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    return (1);
                }

                int issues = 0;
                int total = 0;
                foreach (PropertyInfo property in typeof(IJEFetalDeath).GetProperties())
                {
                    string val1 = Convert.ToString(property.GetValue(ije1, null));
                    string val2 = Convert.ToString(property.GetValue(ije2, null));
                    string val3 = Convert.ToString(property.GetValue(ije3, null));

                    IJEField info = property.GetCustomAttribute<IJEField>();

                    if (val1.ToUpper() != val2.ToUpper() || val1.ToUpper() != val3.ToUpper() || val2.ToUpper() != val3.ToUpper())
                    {
                        issues++;
                        Console.Error.WriteLine($"[***** MISMATCH *****]\t{info.Name}: {info.Contents} \t\t\"{val1}\" != \"{val2}\" != \"{val3}\"");
                    }
                    total++;
                }
                Console.Error.WriteLine($"\n{issues} issues out of {total} total fields.");
                return issues;
            }
            else if (args.Length == 3 && args[0] == "roundtrip-all" && args[1] == "birth")
            {
                BirthRecord b1 = new BirthRecord(File.ReadAllText(args[2]));
                BirthRecord b2 = new BirthRecord(b1.ToJSON());
                BirthRecord b3 = new BirthRecord();
                List<PropertyInfo> properties = typeof(BirthRecord).GetProperties().ToList();
                // Fields the cannot roundtrip
                // CertificationDate: cannot roundtrip time data, IJE does not have a field for time of certification
                // Most fields with a 0..1 text property will not roundtrip
                HashSet<string> skipPropertyNames = new HashSet<string>() { };
                foreach (PropertyInfo property in properties)
                {
                    if (skipPropertyNames.Contains(property.Name))
                    {
                        continue;
                    }
                    if (property.GetCustomAttribute<Property>() != null)
                    {
                        property.SetValue(b3, property.GetValue(b2));
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
            else if (args.Length == 3 && args[0] == "roundtrip-all" && args[1] == "fetaldeath")
            {
                FetalDeathRecord b1 = new FetalDeathRecord(File.ReadAllText(args[2]));
                FetalDeathRecord b2 = new FetalDeathRecord(b1.ToJSON());
                FetalDeathRecord b3 = new FetalDeathRecord();
                List<PropertyInfo> properties = typeof(FetalDeathRecord).GetProperties().ToList();
                // Fields the cannot roundtrip
                // CertificationDate: cannot roundtrip time data, IJE does not have a field for time of certification
                // Most fields with a 0..1 text property will not roundtrip
                HashSet<string> skipPropertyNames = new HashSet<string>() { };
                foreach (PropertyInfo property in properties)
                {
                    if (skipPropertyNames.Contains(property.Name))
                    {
                        continue;
                    }
                    if (property.GetCustomAttribute<Property>() != null)
                    {
                        property.SetValue(b3, property.GetValue(b2));
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
            else if (args.Length == 1 && args[0] == "nre2json")
            {
                IJEBirth ijeb = new IJEBirth();
                ijeb.MRACE1E = "199";
                ijeb.MRACE2E = "";
                ijeb.MRACE3E = "";
                ijeb.MRACE4E = "";
                ijeb.MRACE5E = "";
                ijeb.MRACE6E = "";
                ijeb.MRACE7E = "";
                ijeb.MRACE8E = "";

                ijeb.METHNIC1 = "N";
                ijeb.METHNIC2 = "N";
                ijeb.METHNIC3 = "N";
                ijeb.METHNIC4 = "N";
                ijeb.METHNIC5 = "";
                ijeb.METHNICE = "100";
                ijeb.METHNIC5C = "";

                BirthRecord br = ijeb.ToRecord();
                BirthRecordParentalDemographicsCodingMessage msg = new BirthRecordParentalDemographicsCodingMessage(br);
                Console.WriteLine(msg.ToJson());
                // there's something wrong with the ToJson call when converting the message to json to insert in the db
                return 0;
            }
            else
            {
                Console.WriteLine($"**** No such command {args[0]} with the number of arguments supplied");
            }
            return 0;
        }
    }
}
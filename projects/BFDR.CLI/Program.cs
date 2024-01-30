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
";
  //- batch: Read in IJE messages and create a batch submission bundle (2+ arguments: submission URL (for inside bundle) and one or more messages)
  //- filter: Read in the FHIR birth record and filter based on filter array (1 argument: path to birth record to filter)


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
            else
            {
                Console.WriteLine($"**** No such command {args[0]} with the number of arguments supplied");
            }
            return 0;
        }
    }
}
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
            return 0;
        }
    }
}
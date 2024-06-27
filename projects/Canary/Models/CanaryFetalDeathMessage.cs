using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using BFDR;
using VR;
using System.Reflection;

namespace canary.Models
{
    public class CanaryFetalDeathMessage : Message
    {

        private static Dictionary<string, string> messageDescription = new Dictionary<string, string>()
        {
            { "MessageTimestamp", "The Timestamp of the Message" },
            { "MessageId", "The Message Identifier" },
            { "MessageType", "The NCHS Message Type" },
            { "MessageSource", "The Jurisdiction Message Source" },
            { "MessageDestination", "The NCHS Message Endpoint" },
            { "MessageDestinations", "The NCHS Message Endpoints" },
            { "CertNo", "Birth Certificate Number (BirthRecord Identifier)" },
            { "StateAuxiliaryId", "Auxiliary State File Number (BirthRecord BundleIdentifier)" },
            { "NCHSIdentifier", "The NCHS compound identifier for the supplied BirthRecord" },
            { "JurisdictionId", "Two character identifier of the jurisdiction in which the birth occurred" }
        };

        public CanaryFetalDeathMessage() { }

        public CanaryFetalDeathMessage(string message)
        {
            // TODO: BFDR does not support messaging yet.
            // this.message = xxxBirthRecordBaseMessage.Parse(message, false);
        }

        public CanaryFetalDeathMessage(CommonMessage message) : base(message) {}

        public CanaryFetalDeathMessage(Record record, String type)
        {
            FetalDeathRecord fdr = (FetalDeathRecord) record.GetRecord();
            // TODO - Fetal Death Messaging
            // switch (type)
            // {
            //     case "Submission":
            //     case "http://nchs.cdc.gov/bfdr_submission":
            //         message = new BirthRecordSubmissionMessage(br);
            //         break;
            //     case "Update":
            //     case "http://nchs.cdc.gov/bfdr_submission_update":
            //         message = new BirthRecordUpdateMessage(br);
            //         break;
            //     case "Void":
            //     case "http://nchs.cdc.gov/bfdr_submission_void":
            //         message = new BirthRecordVoidMessage(br);
            //         break;
            //     default:
            //         throw new ArgumentException($"The given message type {type} is not valid.", "type");
            // }
            // message.MessageSource = "https://example.com/jurisdiction/message/endpoint";
        }

        public static string GetDescriptionFor(string entry)
        {
            PropertyInfo myPropInfo = typeof(BirthRecord).GetProperty(entry);
            return myPropInfo != null ? myPropInfo.Name : messageDescription.GetValueOrDefault(entry, "Unknown Property");
        }

        public override Dictionary<string, Message> GetResponsesFor(String type)
        {
            Dictionary<string, Message> result = new Dictionary<string, Message>();
            // Create acknowledgement
            BirthRecordAcknowledgementMessage ack = new BirthRecordAcknowledgementMessage((BirthRecordBaseMessage) message);
            Message ackMsg = new CanaryBirthMessage(ack);
            result.Add("ACK", ackMsg);

            // Create the extraction error
            BirthRecordErrorMessage err = new BirthRecordErrorMessage((BirthRecordBaseMessage) message);
            // Add the issues found during processing
            var issues = new List<BFDR.Issue>();
            var issue = new BFDR.Issue(OperationOutcome.IssueSeverity.Fatal, OperationOutcome.IssueType.Invalid, "This is a fake message");
            issues.Add(issue);
            err.Issues = issues;
            Message errMsg = new CanaryBirthMessage(err);
            result.Add("Error", errMsg);

            // Handle type of message.

            return result;
        }
    }
}

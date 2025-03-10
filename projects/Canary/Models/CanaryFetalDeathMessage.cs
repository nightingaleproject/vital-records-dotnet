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
            { "JurisdictionId", "Two character identifier of the jurisdiction in which the birth occurred" },
            { "PayloadVersionId", "Identifier of the payload FHIR IG version" },
            { "EventYear", "The year in which the birth occurred" }
        };

        public CanaryFetalDeathMessage() { }

        public CanaryFetalDeathMessage(string message)
        {
            this.message = BFDRBaseMessage.Parse(message, false);
        }

        public CanaryFetalDeathMessage(CommonMessage message) : base(message) {}

        public CanaryFetalDeathMessage(Record record, String type)
        {
            FetalDeathRecord fdr = (FetalDeathRecord) record.GetRecord();
            switch (type)
            {
                case "Submission":
                case "http://nchs.cdc.gov/fd_submission":
                    message = new FetalDeathRecordSubmissionMessage(fdr);
                    break;
                case "Update":
                case "http://nchs.cdc.gov/fd_submission_update":
                    message = new FetalDeathRecordUpdateMessage(fdr);
                    break;
                case "Void":
                case "http://nchs.cdc.gov/fd_submission_void":
                    message = new FetalDeathRecordVoidMessage(fdr);
                    break;
                default:
                    throw new ArgumentException($"The given message type {type} is not valid.", "type");
            }
            message.MessageSource = "https://example.com/jurisdiction/message/endpoint";
        }

        public static string GetDescriptionFor(string entry)
        {
            PropertyInfo myPropInfo = typeof(FetalDeathRecord).GetProperty(entry);
            return myPropInfo != null ? myPropInfo.Name : messageDescription.GetValueOrDefault(entry, "Unknown Property");
        }

        public override Dictionary<string, Message> GetResponsesFor(string type)
        {
            Dictionary<string, Message> result = new Dictionary<string, Message>();
            // Create acknowledgement
            FetalDeathRecordAcknowledgementMessage ack = new FetalDeathRecordAcknowledgementMessage((BFDRBaseMessage) message);
            Message ackMsg = new CanaryFetalDeathMessage(ack);
            result.Add("ACK", ackMsg);

            // Create the extraction error
            FetalDeathRecordErrorMessage err = new FetalDeathRecordErrorMessage((BFDRBaseMessage) message);
            // Add the issues found during processing
            var issues = new List<BFDR.Issue>();
            var issue = new BFDR.Issue(OperationOutcome.IssueSeverity.Fatal, OperationOutcome.IssueType.Invalid, "This is a fake message");
            issues.Add(issue);
            err.Issues = issues;
            Message errMsg = new CanaryFetalDeathMessage(err);
            result.Add("Error", errMsg);

            // Handle type of message.

            return result;
        }
    }
}

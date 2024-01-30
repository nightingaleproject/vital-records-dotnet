using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using BFDR;
using VR;

namespace canary.Models
{
    public class CanaryBirthMessage : Message
    {

        private static Dictionary<string, string> messageDescription = new Dictionary<string, string>()
        {
            { "TODO", "TODO" },
        };

        public CanaryBirthMessage() { }

        public CanaryBirthMessage(string message)
        {
            this.message = BirthRecordBaseMessage.Parse(message, false);
        }

        public CanaryBirthMessage(CommonMessage message) : base(message) {}

        public CanaryBirthMessage(Record record, String type)
        {
            BirthRecord dr = (BirthRecord) record.GetRecord();
            // Determine message type.
        }

        public static string GetDescriptionFor(string entry)
        {
            return messageDescription.GetValueOrDefault(entry, "Unknown Property");
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

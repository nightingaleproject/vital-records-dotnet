using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>Class <c>BirthRecordErrorMessage</c> is used to communicate that initial processing of a BFDR message failed.</summary>
    public class BirthRecordErrorMessage : BFDRErrorMessage
    {
        /// <summary>
        /// The Event URI for BirthRecordErrorMessage
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/birth_extraction_error";

        /// <summary>Default constructor that creates a new, empty BirthRecordErrorMessage.</summary>
        public BirthRecordErrorMessage(BFDRBaseMessage message) : base(message)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct an BirthRecordErrorMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BirthRecordErrorMessage</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        internal BirthRecordErrorMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle, baseMessage)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates an extraction error message for the specified message.</summary>
        /// <param name="messageId">the id of the message to create an extraction error for.</param>
        /// <param name="destination">the endpoint identifier that the extraction error message will be sent to.</param>
        /// <param name="source">the endpoint identifier that the extraction error message will be sent from.</param>
        public BirthRecordErrorMessage(string messageId, string destination, string source = "http://nchs.cdc.gov/bfdr_submission") : base(messageId, destination, source)
        {
            MessageType = MESSAGE_TYPE;
        }
    }
    /// <summary>Class <c>FetalDeathRecordErrorMessage</c> is used to communicate that initial processing of a BFDR message failed.</summary>
    public class FetalDeathRecordErrorMessage : BFDRErrorMessage
    {
        /// <summary>
        /// The Event URI for FetalDeathRecordErrorMessage
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/fd_extraction_error";
        /// <summary>Default constructor that creates a new, empty FetalDeathRecordErrorMessage.</summary>
        public FetalDeathRecordErrorMessage(BFDRBaseMessage message) : base(message)
        {
            MessageType = MESSAGE_TYPE;
        }
        /// <summary>
        /// Construct an FetalDeathRecordErrorMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the FetalDeathRecordErrorMessage</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        internal FetalDeathRecordErrorMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle, baseMessage)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates an extraction error message for the specified message.</summary>
        /// <param name="messageId">the id of the message to create an extraction error for.</param>
        /// <param name="destination">the endpoint identifier that the extraction error message will be sent to.</param>
        /// <param name="source">the endpoint identifier that the extraction error message will be sent from.</param>
        public FetalDeathRecordErrorMessage(string messageId, string destination, string source = "http://nchs.cdc.gov/bfdr_submission") : base(messageId, destination, source)
        {
            MessageType = MESSAGE_TYPE;
        }
    }

    /// <summary>Class <c>BFDRErrorMessage</c> is used to communicate that initial processing of a BFDR message failed.</summary>
    public abstract class BFDRErrorMessage : BFDRBaseMessage
    {
        private OperationOutcome details;

        /// <summary>Constructor that creates an extraction error for the specified message.</summary>
        /// <param name="sourceMessage">the message that could not be processed.</param>
        public BFDRErrorMessage(BFDRBaseMessage sourceMessage) : this(sourceMessage?.MessageId, sourceMessage?.MessageSource, sourceMessage?.MessageDestination)
        {
            this.CertNo = sourceMessage?.CertNo;
            this.StateAuxiliaryId = sourceMessage?.StateAuxiliaryId;
            this.JurisdictionId = sourceMessage?.JurisdictionId;
            this.EventYear = sourceMessage?.GetYear(); // use GetYear for backward compatibility for VRDR death_year
            this.PayloadVersionId = $"{GeneratedCustomProperty.Value}";
        }

        /// <summary>
        /// Construct an BFDRErrorMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BFDRErrorMessage</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        internal BFDRErrorMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle)
        {
            try
            {
                details = findEntry<OperationOutcome>();
            }
            catch (System.ArgumentException ex)
            {
                throw new MessageParseException($"Error processing OperationOutcome entry in the message: {ex.Message}", baseMessage);
            }
        }

        /// <summary>Constructor that creates an extraction error message for the specified message.</summary>
        /// <param name="messageId">the id of the message to create an extraction error for.</param>
        /// <param name="destination">the endpoint identifier that the extraction error message will be sent to.</param>
        /// <param name="source">the endpoint identifier that the extraction error message will be sent from.</param>
        public BFDRErrorMessage(string messageId, string destination, string source = "http://nchs.cdc.gov/bfdr_submission") : base(MESSAGE_TYPE)
        {
            Header.Source.Endpoint = source;
            this.MessageDestination = destination;
            MessageHeader.ResponseComponent resp = new MessageHeader.ResponseComponent();
            resp.Identifier = messageId;
            resp.Code = MessageHeader.ResponseType.FatalError;
            Header.Response = resp;

            this.details = new OperationOutcome();
            this.details.Id = Guid.NewGuid().ToString();
            MessageBundle.AddResourceEntry(this.details, "urn:uuid:" + this.details.Id);
            Header.Response.Details = new ResourceReference("urn:uuid:" + this.details.Id);
        }

        /// <summary>The id of the message that could not be extracted</summary>
        /// <value>the message id.</value>
        public string FailedMessageId
        {
            get
            {
                return Header?.Response?.Identifier;
            }
            set
            {
                if (Header.Response == null)
                {
                    Header.Response = new MessageHeader.ResponseComponent();
                    Header.Response.Code = MessageHeader.ResponseType.FatalError;
                }
                Header.Response.Identifier = value;
            }
        }

        /// <summary>
        /// List of issues found when attenpting to extract the message
        /// </summary>
        /// <value>list of issues</value>
        public List<Issue> Issues
        {
            get
            {
                var issues = new List<Issue>();
                foreach (var detailEntry in details.Issue)
                {
                    var issue = new Issue(detailEntry.Severity, detailEntry.Code, detailEntry.Diagnostics);
                    issues.Add(issue);
                }
                return issues;
            }
            set
            {
                details.Issue.Clear();
                foreach (var issue in value)
                {
                    var entry = new OperationOutcome.IssueComponent();
                    entry.Severity = issue.Severity;
                    entry.Code = issue.Type;
                    entry.Diagnostics = issue.Description;
                    details.Issue.Add(entry);
                }
            }
        }
    }
}
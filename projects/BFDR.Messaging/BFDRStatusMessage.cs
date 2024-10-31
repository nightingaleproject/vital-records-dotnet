using System;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>Class <c>BirthRecordStatusMessage</c> provides a status update to a jurisdiction about a previously submitted message.</summary>
    public class BirthRecordStatusMessage : BFDRStatusMessage
    {
        /// <summary>
        /// The Event URI for BirthRecordStatusMessage
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/birth_status";

        /// <summary>Default constructor that creates a new, empty BFDRStatusMessage.</summary>
        public BirthRecordStatusMessage() : base()
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Default constructor that creates a new, empty BirthRecordStatusMessage.</summary>
        public BirthRecordStatusMessage(BFDRBaseMessage message, string status) : base(message, status)
        {
            MessageType = MESSAGE_TYPE;
        }
        /// <summary>
        /// Construct a BirthRecordStatusMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BirthRecordStatusMessage</param>
        /// <returns></returns>
        internal BirthRecordStatusMessage(Bundle messageBundle) : base(messageBundle)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates a status message for the specified message.</summary>
        /// <param name="messageId">the id of the message to create status message for.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>
        public BirthRecordStatusMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(messageId, destination, status, source)
        {
            MessageType = MESSAGE_TYPE;
        }
    }
    /// <summary>Class <c>FetalDeathRecordStatusMessage</c> provides a status update to a jurisdiction about a previously submitted message.</summary>
    public class FetalDeathRecordStatusMessage : BFDRStatusMessage
    {
        /// <summary>
        /// The Event URI for FetalDeathRecordStatusMessage
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/fd_status";
       
        /// <summary>Default constructor that creates a new, empty BFDRStatusMessage.</summary>
        public FetalDeathRecordStatusMessage() : base()
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Default constructor that creates a new, empty FetalDeathRecordStatusMessage.</summary>
        public FetalDeathRecordStatusMessage(BFDRBaseMessage message, string status) : base(message, status)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a FetalDeathRecordStatusMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the FetalDeathRecordStatusMessage</param>
        /// <returns></returns>
        internal FetalDeathRecordStatusMessage(Bundle messageBundle) : base(messageBundle)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates a status message for the specified message.</summary>
        /// <param name="messageId">the id of the message to create status message for.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public FetalDeathRecordStatusMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(messageId, destination, status, source)
        {
            MessageType = MESSAGE_TYPE;
        }
    }

    /// <summary>Class <c>BFDRStatusMessage</c> provides a status update to a jurisdiction about a previously submitted message.</summary>
    public abstract class BFDRStatusMessage : BFDRBaseMessage
    {
        /// <summary>
        /// The Event URI for BFDRStatusMessage
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_status";

        /// <summary>Default constructor that creates a new, empty BFDRStatusMessage.</summary>
        public BFDRStatusMessage() : base(MESSAGE_TYPE)
        {
        }

        /// <summary>
        /// Construct a BFDRStatusMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BFDRStatusMessage</param>
        /// <returns></returns>
        internal BFDRStatusMessage(Bundle messageBundle) : base(messageBundle)
        {
        }
        /// <summary>Constructor that creates a status message for the specified message.</summary>
        /// <param name="messageToStatus">the message to create a status for.</param>
        /// <param name="status"> status value </param>
        public BFDRStatusMessage(BFDRBaseMessage messageToStatus, string status) : this(messageToStatus?.MessageId, messageToStatus?.MessageSource, status, messageToStatus?.MessageDestination)
        {
            this.CertNo = messageToStatus?.CertNo;
            this.StateAuxiliaryId = messageToStatus?.StateAuxiliaryId;
            this.JurisdictionId = messageToStatus?.JurisdictionId;
            this.EventYear = messageToStatus?.GetYear();
            this.PayloadVersionId = $"{GeneratedCustomProperty.Value}";
        }

        // TODO: The allowed status values will be different for birth
        /// <summary>Constructor that creates a status message for the specified message.</summary>
        /// <param name="messageId">the id of the message to create status message for.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public BFDRStatusMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(MESSAGE_TYPE)
        {
            Header.Source.Endpoint = source;
            this.MessageDestination = destination;
            MessageHeader.ResponseComponent resp = new MessageHeader.ResponseComponent();
            resp.Identifier = messageId;
            resp.Code = MessageHeader.ResponseType.Ok;
            Header.Response = resp;
            // TODO: The allowed values will be different for birth
            Status = status; // This should be a value from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html
        }

        /// <summary>The id of the message whose status is being reported by this message</summary>
        /// <value>the message id.</value>
        public string StatusedMessageId
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
                    Header.Response.Code = MessageHeader.ResponseType.Ok;
                }
                Header.Response.Identifier = value;
            }
        }
        /// <summary>ProcessingStatus</summary>
        public string Status
        {
            get
            {
                return Record?.GetSingleValue<FhirString>("status")?.Value;
            }
            set
            {
                SetSingleStringValue("status", value);
            }
        }
    }
}

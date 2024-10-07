using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>
    /// A <c>BirthRecordDemographicsCodingMessage</c> that conveys the coded demographics information of a decedent.
    /// </summary>
    public class BirthRecordDemographicsCodingMessage : BirthRecordBaseMessage
    {
        /// <summary>
        /// The event URI for BirthRecordDemographicsCodingMessage.
        /// </summary>
        public const String MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_demographics_coding";

        /// <summary>Bundle that contains the message payload.</summary>
        private BirthRecord birthRecord;

        /// <summary>
        /// Construct a BirthRecordDemographicsCodingMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the BirthRecordDemographicsCodingMessage</param>
        /// <returns></returns>
        public BirthRecordDemographicsCodingMessage(BirthRecord record) : base(MESSAGE_TYPE)
        {
            this.BirthRecord = record;
            ExtractBusinessIdentifiers(record);
        }

        /// <summary>
        /// Construct a BirthRecordDemographicsCodingMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BirthRecordDemographicsCodingMessage</param>
        /// <param name="baseMessage">the BirthRecordBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        /// <returns></returns>
        internal BirthRecordDemographicsCodingMessage(Bundle messageBundle, BirthRecordBaseMessage baseMessage) : base(messageBundle)
        {
            try
            {
                BirthRecord = new BirthRecord(findEntry<Bundle>());
            }
            catch (System.ArgumentException ex)
            {
                throw new MessageParseException($"Error processing BirthRecord entry in the message: {ex.Message}", baseMessage);
            }
        }
        /// <summary>Constructor that creates an BirthRecordDemographicsCodingMessage for the specified submitted birth record message.</summary>
        /// <param name="messageToCode">the message to create coding response for.</param>
        public BirthRecordDemographicsCodingMessage(BirthRecordBaseMessage messageToCode) : this(messageToCode?.MessageId, messageToCode?.MessageSource, messageToCode?.MessageDestination)
        {
            this.CertNo = messageToCode?.CertNo;
            this.StateAuxiliaryId = messageToCode?.StateAuxiliaryId;
            this.JurisdictionId = messageToCode?.JurisdictionId;
            this.BirthYear = messageToCode?.BirthYear;
            this.PayloadVersionId = $"{GeneratedCustomProperty.Value}";
        }

        /// <summary>Constructor that creates a BirthRecordDemographicsCodingMessage for the specified message.</summary>
        /// <param name="messageId">the id of the message to code.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public BirthRecordDemographicsCodingMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(MESSAGE_TYPE)
        {
            Header.Source.Endpoint = source;
            this.MessageDestination = destination;
            MessageHeader.ResponseComponent resp = new MessageHeader.ResponseComponent();
            resp.Identifier = messageId;
            resp.Code = MessageHeader.ResponseType.Ok;
            Header.Response = resp;
        }

        /// <summary>The BirthRecord conveyed by this message</summary>
        /// <value>the BirthRecord</value>
        public BirthRecord BirthRecord
        {
            get
            {
                return birthRecord;
            }
            set
            {
                birthRecord = value;
                UpdateMessageBundleRecord();
            }
        }

        /// <summary>The record bundle that should go into the message bundle for this message</summary>
        /// <value>the MessageBundleRecord</value>
        protected override Bundle MessageBundleRecord
        {
            get
            {
                return birthRecord?.GetBundle();
            }
        }
        /// <summary>The id of the birth record submission/update message that was coded to produce the content of this message</summary>
        /// <value>the message id.</value>
        public string CodedMessageId
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
    }

    /// <summary>Class <c>BirthRecordDemographicsCodingUpdateMessage</c> conveys an updated coded demographics of a decedent.</summary>
    public class BirthRecordDemographicsCodingUpdateMessage : BirthRecordDemographicsCodingMessage
    {
        /// <summary>
        /// The event URI for BirthRecordDemographicsCodingUpdateMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_demographics_coding_update";

        /// <summary>
        /// Construct a BirthRecordDemographicsCodingUpdateMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the BirthRecordDemographicsCodingUpdateMessage</param>
        /// <returns></returns>
        public BirthRecordDemographicsCodingUpdateMessage(BirthRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a BirthRecordDemographicsCodingUpdateMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BirthRecordDemographicsCodingUpdateMessage</param>
        /// <param name="baseMessage">the BirthRecordBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        /// <returns></returns>
        internal BirthRecordDemographicsCodingUpdateMessage(Bundle messageBundle, BirthRecordBaseMessage baseMessage) : base(messageBundle, baseMessage) { }
    }
}

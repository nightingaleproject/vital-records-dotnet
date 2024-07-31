using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>
    /// A <c>FetalDeathRecordDemographicsCodingMessage</c> that conveys the coded demographics information of a decedent.
    /// </summary>
    public class FetalDeathRecordDemographicsCodingMessage : FetalDeathRecordBaseMessage
    {
        /// <summary>
        /// The event URI for FetalDeathRecordDemographicsCodingMessage.
        /// </summary>
        public const String MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_demographics_coding";

        /// <summary>Bundle that contains the message payload.</summary>
        private FetalDeathRecord fetalDeathRecord;

        /// <summary>
        /// Construct a FetalDeathRecordDemographicsCodingMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the FetalDeathRecordDemographicsCodingMessage</param>
        /// <returns></returns>
        public FetalDeathRecordDemographicsCodingMessage(FetalDeathRecord record) : base(MESSAGE_TYPE)
        {
            this.FetalDeathRecord = record;
            ExtractBusinessIdentifiers(record);
        }

        /// <summary>
        /// Construct a FetalDeathRecordDemographicsCodingMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the FetalDeathRecordDemographicsCodingMessage</param>
        /// <param name="baseMessage">the FetalDeathRecordBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        /// <returns></returns>
        internal FetalDeathRecordDemographicsCodingMessage(Bundle messageBundle, FetalDeathRecordBaseMessage baseMessage) : base(messageBundle)
        {
            try
            {
                FetalDeathRecord = new FetalDeathRecord(findEntry<Bundle>());
            }
            catch (System.ArgumentException ex)
            {
                throw new MessageParseException($"Error processing FetalDeathRecord entry in the message: {ex.Message}", baseMessage);
            }
        }
        /// <summary>Constructor that creates an FetalDeathRecordDemographicsCodingMessage for the specified submitted fetalDeath record message.</summary>
        /// <param name="messageToCode">the message to create coding response for.</param>
        public FetalDeathRecordDemographicsCodingMessage(FetalDeathRecordBaseMessage messageToCode) : this(messageToCode?.MessageId, messageToCode?.MessageSource, messageToCode?.MessageDestination)
        {
            this.CertNo = messageToCode?.CertNo;
            this.StateAuxiliaryId = messageToCode?.StateAuxiliaryId;
            this.JurisdictionId = messageToCode?.JurisdictionId;
            this.BirthYear = messageToCode?.BirthYear;
        }

        /// <summary>Constructor that creates a FetalDeathRecordDemographicsCodingMessage for the specified message.</summary>
        /// <param name="messageId">the id of the message to code.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public FetalDeathRecordDemographicsCodingMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(MESSAGE_TYPE)
        {
            Header.Source.Endpoint = source;
            this.MessageDestination = destination;
            MessageHeader.ResponseComponent resp = new MessageHeader.ResponseComponent();
            resp.Identifier = messageId;
            resp.Code = MessageHeader.ResponseType.Ok;
            Header.Response = resp;
        }

        /// <summary>The FetalDeathRecord conveyed by this message</summary>
        /// <value>the FetalDeathRecord</value>
        public FetalDeathRecord FetalDeathRecord
        {
            get
            {
                return fetalDeathRecord;
            }
            set
            {
                fetalDeathRecord = value;
                UpdateMessageBundleRecord();
            }
        }

        /// <summary>The record bundle that should go into the message bundle for this message</summary>
        /// <value>the MessageBundleRecord</value>
        protected override Bundle MessageBundleRecord
        {
            get
            {
                return fetalDeathRecord?.GetDemographicCodedContentBundle();
            }
        }
        /// <summary>The id of the fetalDeath record submission/update message that was coded to produce the content of this message</summary>
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

    /// <summary>Class <c>FetalDeathRecordDemographicsCodingUpdateMessage</c> conveys an updated coded demographics of a decedent.</summary>
    public class FetalDeathRecordDemographicsCodingUpdateMessage : FetalDeathRecordDemographicsCodingMessage
    {
        /// <summary>
        /// The event URI for FetalDeathRecordDemographicsCodingUpdateMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_demographics_coding_update";

        /// <summary>
        /// Construct a FetalDeathRecordDemographicsCodingUpdateMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the FetalDeathRecordDemographicsCodingUpdateMessage</param>
        /// <returns></returns>
        public FetalDeathRecordDemographicsCodingUpdateMessage(FetalDeathRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a FetalDeathRecordDemographicsCodingUpdateMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the FetalDeathRecordDemographicsCodingUpdateMessage</param>
        /// <param name="baseMessage">the FetalDeathRecordBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        /// <returns></returns>
        internal FetalDeathRecordDemographicsCodingUpdateMessage(Bundle messageBundle, FetalDeathRecordBaseMessage baseMessage) : base(messageBundle, baseMessage) { }
    }
}

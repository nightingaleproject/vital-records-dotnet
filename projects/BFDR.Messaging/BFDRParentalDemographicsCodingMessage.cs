using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>
    /// A <c>BFDRParentalDemographicsCodingMessage</c> that conveys the coded demographics information of a decedent.
    /// </summary>
    public class BFDRParentalDemographicsCodingMessage : BFDRBaseMessage
    {
        /// <summary>
        /// The event URI for BFDRDemographicsCodingMessage.
        /// </summary>
        public const String MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_demographics_coding";

        /// <summary>Bundle that contains the message payload.</summary>
        private NatalityRecord natalityRecord;

        /// <summary>
        /// Construct a BFDRParentalDemographicsCodingMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the BFDRParentalDemographicsCodingMessage</param>
        /// <returns></returns>
        public BFDRParentalDemographicsCodingMessage(BirthRecord record) : base(MESSAGE_TYPE)
        {
            this.NatalityRecord = record;
            ExtractBusinessIdentifiers(record);
        }

        /// <summary>
        /// Construct a BFDRParentalDemographicsCodingMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BFDRParentalDemographicsCodingMessage</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        /// <returns></returns>
        internal BFDRParentalDemographicsCodingMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle)
        {
            try
            {
                BirthRecord b = new BirthRecord(findEntry<Bundle>());
            }
            catch (System.ArgumentException ex)
            {
                throw new MessageParseException($"Error processing BirthRecord entry in the message: {ex.Message}", baseMessage);
            }
        }
        /// <summary>Constructor that creates an BFDRParentalDemographicsCodingMessage for the specified submitted birth record message.</summary>
        /// <param name="messageToCode">the message to create coding response for.</param>
        public BFDRParentalDemographicsCodingMessage(BFDRBaseMessage messageToCode) : this(messageToCode?.MessageId, messageToCode?.MessageSource, messageToCode?.MessageDestination)
        {
            this.CertNo = messageToCode?.CertNo;
            this.StateAuxiliaryId = messageToCode?.StateAuxiliaryId;
            this.JurisdictionId = messageToCode?.JurisdictionId;
            this.EventYear = messageToCode?.GetYear();
        }

        /// <summary>Constructor that creates a BFDRParentalDemographicsCodingMessage for the specified message.</summary>
        /// <param name="messageId">the id of the message to code.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public BFDRParentalDemographicsCodingMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(MESSAGE_TYPE)
        {
            Header.Source.Endpoint = source;
            this.MessageDestination = destination;
            MessageHeader.ResponseComponent resp = new MessageHeader.ResponseComponent();
            resp.Identifier = messageId;
            resp.Code = MessageHeader.ResponseType.Ok;
            Header.Response = resp;
        }

        /// <summary>The NatalityRecord conveyed by this message</summary>
        /// <value>the NatalityRecord</value>
        public NatalityRecord NatalityRecord
        {
            get
            {
                return natalityRecord;
            }
            set
            {
                natalityRecord = value;
                UpdateMessageBundleRecord();
            }
        }

        /// <summary>The record bundle that should go into the message bundle for this message</summary>
        /// <value>the MessageBundleRecord</value>
        protected override Bundle MessageBundleRecord
        {
            get
            {
                return natalityRecord?.GetDemographicCodedContentBundle();
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

    /// <summary>Class <c>BFDRParentalDemographicsCodingUpdateMessage</c> conveys an updated coded demographics of a decedent.</summary>
    public class BFDRParentalDemographicsCodingUpdateMessage : BFDRParentalDemographicsCodingMessage
    {
        /// <summary>
        /// The event URI for BFDRParentalDemographicsCodingUpdateMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_demographics_coding_update";

        /// <summary>
        /// Construct a BFDRParentalDemographicsCodingUpdateMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the BFDRParentalDemographicsCodingUpdateMessage</param>
        /// <returns></returns>
        public BFDRParentalDemographicsCodingUpdateMessage(BirthRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a BFDRParentalDemographicsCodingUpdateMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BFDRParentalDemographicsCodingUpdateMessage</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        /// <returns></returns>
        internal BFDRParentalDemographicsCodingUpdateMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle, baseMessage) { }
    }
}

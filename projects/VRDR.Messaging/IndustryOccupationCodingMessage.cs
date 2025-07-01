using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace VRDR
{
    /// <summary>
    /// A <c>DemographicsCodingMessage</c> that conveys the coded demographics information of a decedent.
    /// </summary>
    public class IndustryOccupationCodingMessage : BaseMessage
    {
        /// <summary>
        /// The event URI for IndustryOccupationCodingMessage.
        /// </summary>
        public const String MESSAGE_TYPE = "http://nchs.cdc.gov/vrdr_industryoccupation_coding";

        /// <summary>Bundle that contains the message payload.</summary>
        private DeathRecord deathRecord;

        /// <summary>
        /// Construct a DemographicsCodingMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the DemographicsCodingMessage</param>
        /// <returns></returns>
        public IndustryOccupationCodingMessage(DeathRecord record) : base(MESSAGE_TYPE)
        {
            this.DeathRecord = record;
            ExtractBusinessIdentifiers(record);
        }

        /// <summary>
        /// Construct a DemographicsCodingMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the DemographicsCodingMessage</param>
        /// <param name="baseMessage">the BaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        /// <returns></returns>
        internal IndustryOccupationCodingMessage(Bundle messageBundle, BaseMessage baseMessage) : base(messageBundle)
        {
            try
            {
                DeathRecord = new DeathRecord(findEntry<Bundle>());
            }
            catch (System.ArgumentException ex)
            {
                throw new MessageParseException($"Error processing DeathRecord entry in the message: {ex.Message}", baseMessage);
            }
        }
        /// <summary>Constructor that creates an IndustryOccupationCodingMessage for the specified submitted death record message.</summary>
        /// <param name="messageToCode">the message to create coding response for.</param>
        public IndustryOccupationCodingMessage(BaseMessage messageToCode) : this(messageToCode?.MessageId, messageToCode?.MessageSource, messageToCode?.MessageDestination)
        {
            this.CertNo = messageToCode?.CertNo;
            this.StateAuxiliaryId = messageToCode?.StateAuxiliaryId;
            this.JurisdictionId = messageToCode?.JurisdictionId;
            this.DeathYear = messageToCode?.DeathYear;
            this.PayloadVersionId = $"{GeneratedCustomProperty.Value}";
        }

        /// <summary>Constructor that creates a IndustryOccupationCodingMessage for the specified message.</summary>
        /// <param name="messageId">the id of the message to code.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public IndustryOccupationCodingMessage(string messageId, string destination, string source = "http://nchs.cdc.gov/vrdr_submission") : base(MESSAGE_TYPE)
        {
            Header.Source.Endpoint = source;
            this.MessageDestination = destination;
            MessageHeader.ResponseComponent resp = new MessageHeader.ResponseComponent();
            resp.Identifier = messageId;
            resp.Code = MessageHeader.ResponseType.Ok;
            Header.Response = resp;
        }

        /// <summary>The DeathRecord conveyed by this message</summary>
        /// <value>the DeathRecord</value>
        public DeathRecord DeathRecord
        {
            get
            {
                return deathRecord;
            }
            set
            {
                deathRecord = value;
                UpdateMessageBundleRecord();
            }
        }

        /// <summary>The record bundle that should go into the message bundle for this message</summary>
        /// <value>the MessageBundleRecord</value>
        protected override Bundle MessageBundleRecord
        {
            get
            {
                return deathRecord?.GetIndustryOccupationCodedContentBundle();
            }
        }
        /// <summary>The id of the death record submission/update message that was coded to produce the content of this message</summary>
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

    /// <summary>Class <c>DemographicsCodingUpdateMessage</c> conveys an updated coded demographics of a decedent.</summary>
    public class IndustryOccupationCodingUpdateMessage : IndustryOccupationCodingMessage
    {
        /// <summary>
        /// The event URI for DemographicsCodingUpdateMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/vrdr_industryoccupation_coding_update";

        /// <summary>
        /// Construct a DemographicsCodingUpdateMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the DemographicsCodingUpdateMessage</param>
        /// <returns></returns>
        public IndustryOccupationCodingUpdateMessage(DeathRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a DemographicsCodingUpdateMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the IndustryOccupationCodingUpdateMessage</param>
        /// <param name="baseMessage">the BaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        /// <returns></returns>
        internal IndustryOccupationCodingUpdateMessage(Bundle messageBundle, BaseMessage baseMessage) : base(messageBundle, baseMessage) { }
    }
}

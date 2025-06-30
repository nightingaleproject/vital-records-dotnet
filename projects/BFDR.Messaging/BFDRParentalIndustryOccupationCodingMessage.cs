using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>
    /// Class <c>BirthRecordParentalIndustryOccupationCodingMessage</c> class that conveys the coded industry and occupation information of the parents of a child.
    /// </summary>
    public class BirthRecordParentalIndustryOccupationCodingMessage : BFDRParentalIndustryOccupationCodingMessage
    {
        /// <summary>
        /// The event URI for BirthRecordParentalIndustryOccupationCodingMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/birth_industryoccupation_coding";

        /// <summary>Default constructor that creates a new, empty BirthRecordParentalIndustryOccupationCodingMessage.</summary>
        public BirthRecordParentalIndustryOccupationCodingMessage(Bundle messageBundle, BFDRBaseMessage message) : base(messageBundle, message)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates an BirthRecordParentalIndustryOccupationCodingMessage for the specified submitted birth record message.</summary>
        /// <param name="messageToCode">the message to create coding response for.</param>
        public BirthRecordParentalIndustryOccupationCodingMessage(BFDRBaseMessage messageToCode) : base(messageToCode)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a BirthRecordParentalIndustryOccupationCodingMessage from a record containing industry and occupation coded content.
        /// </summary>
        /// <param name="record">a record containing industry and occupation coded content for initializing the BirthRecordParentalIndustryOccupationCodingMessage</param>
        /// <returns></returns>
        public BirthRecordParentalIndustryOccupationCodingMessage(NatalityRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates a BirthRecordParentalIndustryOccupationCodingMessage for the specified message.</summary>
        /// <param name="messageId">the id of the message to code.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public BirthRecordParentalIndustryOccupationCodingMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(messageId, destination, status, source)
        {
            MessageType = MESSAGE_TYPE;
        }
    }

    /// <summary>
    /// Class <c>FetalDeathRecordParentalIndustryOccupationCodingMessage</c> class that conveys the coded industry and occupation information of the parents for a fetal death.
    /// </summary>
    public class FetalDeathRecordParentalIndustryOccupationCodingMessage : BFDRParentalIndustryOccupationCodingMessage
    {
        /// <summary>
        /// The event URI for FetalDeathRecordParentalIndustryOccupationCodingMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/fd_industryoccupation_coding";
        /// <summary>Default constructor that creates a new, empty FetalDeathRecordParentalIndustryOccupationCodingMessage.</summary>
        public FetalDeathRecordParentalIndustryOccupationCodingMessage(Bundle messageBundle, BFDRBaseMessage message) : base(messageBundle, message)
        {
            MessageType = MESSAGE_TYPE;
        }
        /// <summary>Constructor that creates an FetalDeathRecordParentalIndustryOccupationCodingMessage for the specified submitted birth record message.</summary>
        /// <param name="messageToCode">the message to create coding response for.</param>
        public FetalDeathRecordParentalIndustryOccupationCodingMessage(BFDRBaseMessage messageToCode) : base(messageToCode)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a FetalDeathRecordParentalIndustryOccupationCodingMessage from a record containing industry and occupation coded content.
        /// </summary>
        /// <param name="record">a record containing industry and occupation coded content for initializing the FetalDeathRecordParentalIndustryOccupationCodingMessage</param>
        /// <returns></returns>
        public FetalDeathRecordParentalIndustryOccupationCodingMessage(NatalityRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates a FetalDeathRecordParentalIndustryOccupationCodingMessage for the specified message.</summary>
        /// <param name="messageId">the id of the message to code.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public FetalDeathRecordParentalIndustryOccupationCodingMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(messageId, destination, status, source)
        {
            MessageType = MESSAGE_TYPE;
        }
    }
    
    /// <summary>
    /// Class <c>BirthRecordParentalIndustryOccupationCodingUpdateMessage</c> class that conveys the coded industry and occupation information of the parents of a child.
    /// </summary>
    public class BirthRecordParentalIndustryOccupationCodingUpdateMessage : BFDRParentalIndustryOccupationCodingUpdateMessage
    {
        /// <summary>
        /// The event URI for BirthRecordParentalIndustryOccupationCodingUpdateMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/birth_industryoccupation_coding_update";

        /// <summary>Default constructor that creates a new, empty BirthRecordParentalIndustryOccupationCodingMessage.</summary>
        public BirthRecordParentalIndustryOccupationCodingUpdateMessage(Bundle messageBundle, BFDRBaseMessage message) : base(messageBundle, message)
        {
            MessageType = MESSAGE_TYPE;
        }
        
        /// <summary>
        /// Construct a BirthRecordParentalIndustryOccupationCodingUpdateMessage from a record containing industry and occupation coded content.
        /// </summary>
        /// <param name="record">a record containing industry and occupation coded content for initializing the BirthRecordParentalIndustryOccupationCodingUpdateMessage</param>
        /// <returns></returns>
        public BirthRecordParentalIndustryOccupationCodingUpdateMessage(NatalityRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }
    }

    /// <summary>
    /// Class <c>FetalDeathRecordParentalIndustryOccupationCodingUpdateMessage</c> class that conveys the coded industry and occupation information of the parents for a fetal death.
    /// </summary>
    public class FetalDeathRecordParentalIndustryOccupationCodingUpdateMessage : BFDRParentalIndustryOccupationCodingUpdateMessage
    {
        /// <summary>
        /// The event URI for FetalDeathRecordParentalIndustryOccupationCodingUpdateMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/fd_industryoccupation_coding_update";
        /// <summary>Default constructor that creates a new, empty FetalDeathRecordParentalIndustryOccupationCodingMessage.</summary>
        public FetalDeathRecordParentalIndustryOccupationCodingUpdateMessage(Bundle messageBundle, BFDRBaseMessage message) : base(messageBundle, message)
        {
            MessageType = MESSAGE_TYPE;
        }
        
        /// <summary>
        /// Construct a FetalDeathRecordParentalIndustryOccupationCodingUpdateMessage from a record containing industry and occupation coded content.
        /// </summary>
        /// <param name="record">a record containing industry and occupation coded content for initializing the FetalDeathRecordParentalIndustryOccupationCodingUpdateMessage</param>
        /// <returns></returns>
        public FetalDeathRecordParentalIndustryOccupationCodingUpdateMessage(NatalityRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }
    }

    /// <summary>
    /// A <c>BFDRParentalIndustryOccupationCodingMessage</c> base class that conveys the coded industry and occupation information of the parents for a fetal death.
    /// </summary>
    public abstract class BFDRParentalIndustryOccupationCodingMessage : BFDRBaseMessage
    {

        /// <summary>Bundle that contains the message payload.</summary>
        private NatalityRecord natalityRecord; // this is natality record to be inclusive of birth or fetal death response codes

        /// <summary>
        /// Construct a BFDRParentalIndustryOccupationCodingMessage from a record containing industry and occupation coded content.
        /// </summary>
        /// <param name="record">a record containing industry and occupation coded content for initializing the BFDRParentalIndustryOccupationCodingMessage</param>
        /// <returns></returns>
        public BFDRParentalIndustryOccupationCodingMessage(NatalityRecord record) : base(MESSAGE_TYPE)
        {
            this.NatalityRecord = record;
            ExtractBusinessIdentifiers(record);
        }

        /// <summary>
        /// Construct a BFDRParentalIndustryOccupationCodingMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BFDRParentalIndustryOccupationCodingMessage</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        /// <returns></returns>
        internal BFDRParentalIndustryOccupationCodingMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle)
        {
            try
            {
                // first try parsing as a birth record
                NatalityRecord = new BirthRecord(findEntry<Bundle>());
            }
            catch (Exception ex)
            {
                // if birth record failed, try fetal death record
                try
                {
                    NatalityRecord = new FetalDeathRecord(findEntry<Bundle>());
                }
                catch (Exception ex2)
                {
                    throw new MessageParseException($"Error processing entry as BirthRecord or FetalDeathRecord in the message: {ex.Message}, {ex2.Message}", baseMessage);
                }
            }
        }
        /// <summary>Constructor that creates an BFDRParentalIndustryOccupationCodingMessage for the specified submitted birth record message.</summary>
        /// <param name="messageToCode">the message to create coding response for.</param>
        public BFDRParentalIndustryOccupationCodingMessage(BFDRBaseMessage messageToCode) : this(messageToCode?.MessageId, messageToCode?.MessageSource, messageToCode?.MessageDestination)
        {
            this.CertNo = messageToCode?.CertNo;
            this.StateAuxiliaryId = messageToCode?.StateAuxiliaryId;
            this.JurisdictionId = messageToCode?.JurisdictionId;
            this.EventYear = messageToCode?.GetYear();
            this.PayloadVersionId = $"{GeneratedCustomProperty.Value}";
        }

        /// <summary>Constructor that creates a BFDRParentalIndustryOccupationCodingMessage for the specified message.</summary>
        /// <param name="messageId">the id of the message to code.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public BFDRParentalIndustryOccupationCodingMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(MESSAGE_TYPE)
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
                return natalityRecord?.GetCodedIndustryAndOccupationBundle();
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

    /// <summary>Class <c>BFDRParentalIndustryOccupationCodingUpdateMessage</c> conveys an updated coded industry and occupation of the parents for a fetal death.</summary>
    public abstract class BFDRParentalIndustryOccupationCodingUpdateMessage : BFDRParentalIndustryOccupationCodingMessage
    {

        /// <summary>
        /// Construct a BFDRParentalIndustryOccupationCodingUpdateMessage from a record containing industry and occupation coded content.
        /// </summary>
        /// <param name="record">a record containing industry and occupation coded content for initializing the BFDRParentalIndustryOccupationCodingUpdateMessage</param>
        /// <returns></returns>
        public BFDRParentalIndustryOccupationCodingUpdateMessage(NatalityRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a BFDRParentalIndustryOccupationCodingUpdateMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BFDRParentalIndustryOccupationCodingUpdateMessage</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        /// <returns></returns>
        internal BFDRParentalIndustryOccupationCodingUpdateMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle, baseMessage) { }
    }
}

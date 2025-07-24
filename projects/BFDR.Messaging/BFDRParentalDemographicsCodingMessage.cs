using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>
    /// Class <c>BirthRecordParentalDemographicsCodingMessage</c> class that conveys the coded demographics information of a child's parents.
    /// </summary>
    public class BirthRecordParentalDemographicsCodingMessage : BFDRParentalDemographicsCodingMessage
    {
        /// <summary>
        /// The event URI for BirthRecordParentalDemographicsCodingMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/birth_demographics_coding";

        /// <summary>Default constructor that creates a new, empty BirthRecordParentalDemographicsCodingMessage.</summary>
        public BirthRecordParentalDemographicsCodingMessage(Bundle messageBundle, BFDRBaseMessage message) : base(messageBundle, message)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates an BirthRecordParentalDemographicsCodingMessage for the specified submitted birth record message.</summary>
        /// <param name="messageToCode">the message to create coding response for.</param>
        public BirthRecordParentalDemographicsCodingMessage(BFDRBaseMessage messageToCode) : base(messageToCode)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a BirthRecordParentalDemographicsCodingMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the BirthRecordParentalDemographicsCodingMessage</param>
        /// <returns></returns>
        public BirthRecordParentalDemographicsCodingMessage(NatalityRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates a BirthRecordParentalDemographicsCodingMessage for the specified message.</summary>
        /// <param name="messageId">the id of the message to code.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public BirthRecordParentalDemographicsCodingMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(messageId, destination, status, source)
        {
            MessageType = MESSAGE_TYPE;
        }
    }

    /// <summary>
    /// Class <c>FetalDeathRecordParentalDemographicsCodingMessage</c> class that conveys the coded demographics information of the parents for a fetal death.
    /// </summary>
    public class FetalDeathRecordParentalDemographicsCodingMessage : BFDRParentalDemographicsCodingMessage
    {
        /// <summary>
        /// The event URI for FetalDeathRecordParentalDemographicsCodingMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/fd_demographics_coding";
        /// <summary>Default constructor that creates a new, empty FetalDeathRecordParentalDemographicsCodingMessage.</summary>
        public FetalDeathRecordParentalDemographicsCodingMessage(Bundle messageBundle, BFDRBaseMessage message) : base(messageBundle, message)
        {
            MessageType = MESSAGE_TYPE;
        }
        /// <summary>Constructor that creates an FetalDeathRecordParentalDemographicsCodingMessage for the specified submitted birth record message.</summary>
        /// <param name="messageToCode">the message to create coding response for.</param>
        public FetalDeathRecordParentalDemographicsCodingMessage(BFDRBaseMessage messageToCode) : base(messageToCode)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a FetalDeathRecordParentalDemographicsCodingMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the FetalDeathRecordParentalDemographicsCodingMessage</param>
        /// <returns></returns>
        public FetalDeathRecordParentalDemographicsCodingMessage(NatalityRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates a FetalDeathRecordParentalDemographicsCodingMessage for the specified message.</summary>
        /// <param name="messageId">the id of the message to code.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="status">the status being sent, from http://build.fhir.org/ig/nightingaleproject/vital_records_fhir_messaging_ig/branches/main/ValueSet-VRM-Status-vs.html</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>

        public FetalDeathRecordParentalDemographicsCodingMessage(string messageId, string destination, string status, string source = "http://nchs.cdc.gov/bfdr_submission") : base(messageId, destination, status, source)
        {
            MessageType = MESSAGE_TYPE;
        }
    }
    
    /// <summary>
    /// Class <c>BirthRecordParentalDemographicsCodingUpdateMessage</c> class that conveys the coded demographics information of a child's parents.
    /// </summary>
    public class BirthRecordParentalDemographicsCodingUpdateMessage : BFDRParentalDemographicsCodingUpdateMessage
    {
        /// <summary>
        /// The event URI for BirthRecordParentalDemographicsCodingUpdateMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/birth_demographics_coding_update";

        /// <summary>Default constructor that creates a new, empty BirthRecordParentalDemographicsCodingMessage.</summary>
        public BirthRecordParentalDemographicsCodingUpdateMessage(Bundle messageBundle, BFDRBaseMessage message) : base(messageBundle, message)
        {
            MessageType = MESSAGE_TYPE;
        }
        
        /// <summary>
        /// Construct a BirthRecordParentalDemographicsCodingUpdateMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the BirthRecordParentalDemographicsCodingUpdateMessage</param>
        /// <returns></returns>
        public BirthRecordParentalDemographicsCodingUpdateMessage(NatalityRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }
    }

    /// <summary>
    /// Class <c>FetalDeathRecordParentalDemographicsCodingUpdateMessage</c> class that conveys the coded demographics information of the parents for a fetal death.
    /// </summary>
    public class FetalDeathRecordParentalDemographicsCodingUpdateMessage : BFDRParentalDemographicsCodingUpdateMessage
    {
        /// <summary>
        /// The event URI for FetalDeathRecordParentalDemographicsCodingUpdateMessage.
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/fd_demographics_coding_update";
        /// <summary>Default constructor that creates a new, empty FetalDeathRecordParentalDemographicsCodingMessage.</summary>
        public FetalDeathRecordParentalDemographicsCodingUpdateMessage(Bundle messageBundle, BFDRBaseMessage message) : base(messageBundle, message)
        {
            MessageType = MESSAGE_TYPE;
        }
        
        /// <summary>
        /// Construct a FetalDeathRecordParentalDemographicsCodingUpdateMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the FetalDeathRecordParentalDemographicsCodingUpdateMessage</param>
        /// <returns></returns>
        public FetalDeathRecordParentalDemographicsCodingUpdateMessage(NatalityRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }
    }

    /// <summary>
    /// A <c>BFDRParentalDemographicsCodingMessage</c> base class that conveys the coded demographics information of the parents for a fetal death.
    /// </summary>
    public abstract class BFDRParentalDemographicsCodingMessage : BFDRBaseMessage
    {

        /// <summary>Bundle that contains the message payload.</summary>
        private NatalityRecord natalityRecord; // this is natality record to  be inclusive of birth or fetal death response codes

        /// <summary>
        /// Construct a BFDRParentalDemographicsCodingMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the BFDRParentalDemographicsCodingMessage</param>
        /// <returns></returns>
        public BFDRParentalDemographicsCodingMessage(NatalityRecord record) : base(MESSAGE_TYPE)
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
        /// <summary>Constructor that creates an BFDRParentalDemographicsCodingMessage for the specified submitted birth record message.</summary>
        /// <param name="messageToCode">the message to create coding response for.</param>
        public BFDRParentalDemographicsCodingMessage(BFDRBaseMessage messageToCode) : this(messageToCode?.MessageId, messageToCode?.MessageSource, messageToCode?.MessageDestination)
        {
            this.CertNo = messageToCode?.CertNo;
            this.StateAuxiliaryId = messageToCode?.StateAuxiliaryId;
            this.JurisdictionId = messageToCode?.JurisdictionId;
            this.EventYear = messageToCode?.GetYear();
            this.PayloadVersionId = $"{GeneratedCustomProperty.Value}";
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

    /// <summary>Class <c>BFDRParentalDemographicsCodingUpdateMessage</c> conveys an updated coded demographics of the parents for a fetal death.</summary>
    public abstract class BFDRParentalDemographicsCodingUpdateMessage : BFDRParentalDemographicsCodingMessage
    {

        /// <summary>
        /// Construct a BFDRParentalDemographicsCodingUpdateMessage from a record containing demographics coded content.
        /// </summary>
        /// <param name="record">a record containing demographics coded content for initializing the BFDRParentalDemographicsCodingUpdateMessage</param>
        /// <returns></returns>
        public BFDRParentalDemographicsCodingUpdateMessage(NatalityRecord record) : base(record)
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

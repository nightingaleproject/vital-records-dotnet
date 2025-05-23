using Hl7.Fhir.Model;
using VR;

namespace BFDR
{
    /// <summary>Class <c>BirthRecordAcknowledgementMessage</c> supports the acknowledgment of other messages.</summary>
    public class BirthRecordAcknowledgementMessage : BFDRAcknowledgementMessage
    {
        /// <summary>
        /// The Event URI for BirthRecordAcknowledgementMessage
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/birth_acknowledgement";

        /// <summary>Default constructor that creates a new, empty BirthRecordAcknowledgementMessage.</summary>
        public BirthRecordAcknowledgementMessage(CommonMessage message) : base(message)
        {
            MessageType = MESSAGE_TYPE;
        }
        /// <summary>Constructor that creates a BirthRecordAcknowledgementMessage from a bundle.</summary>
        internal BirthRecordAcknowledgementMessage(Bundle messageBundle) : base(messageBundle)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates an acknowledgement for the specified message.</summary>
        /// <param name="messageId">the id of the message to create an acknowledgement for.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>
        public BirthRecordAcknowledgementMessage(string messageId, string destination, string source = "http://nchs.cdc.gov/bfdr_submission") : base(messageId, destination, source)
        { 
            MessageType = MESSAGE_TYPE;
        }
    }
    /// <summary>Class <c>FetalDeathRecordAcknowledgementMessage</c> supports the acknowledgment of other messages.</summary>
    public class FetalDeathRecordAcknowledgementMessage : BFDRAcknowledgementMessage
    {
        /// <summary>
        /// The Event URI for FetalDeathRecordAcknowledgementMessage
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/fd_acknowledgement";
        /// <summary>Default constructor that creates a new, empty FetalDeathRecordAcknowledgementMessage.</summary>
        public FetalDeathRecordAcknowledgementMessage(CommonMessage message) : base(message)
        {
            MessageType = MESSAGE_TYPE;
        }
        /// <summary>Constructor that creates a FetalDeathRecordAcknowledgementMessage from a bundle.</summary>
        internal FetalDeathRecordAcknowledgementMessage(Bundle messageBundle) : base(messageBundle)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that creates an acknowledgement for the specified message.</summary>
        /// <param name="messageId">the id of the message to create an acknowledgement for.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>
        public FetalDeathRecordAcknowledgementMessage(string messageId, string destination, string source = "http://nchs.cdc.gov/bfdr_submission") : base(messageId, destination, source)
        { 
            MessageType = MESSAGE_TYPE;
        }
    }
    /// <summary>Class <c>BFDRAcknowledgementMessage</c> supports the acknowledgment of other messages.</summary>
    public abstract class BFDRAcknowledgementMessage : BFDRBaseMessage
    {

        /// <summary>Constructor that creates an acknowledgement for the specified message.</summary>
        /// <param name="messageToAck">the message to create an acknowledgement for.</param>
        public BFDRAcknowledgementMessage(CommonMessage messageToAck) : this(messageToAck?.MessageId, messageToAck?.MessageSource, messageToAck?.MessageDestination)
        {
            this.CertNo = messageToAck?.CertNo;
            this.StateAuxiliaryId = messageToAck?.StateAuxiliaryId;
            this.JurisdictionId = messageToAck?.JurisdictionId;
            this.EventYear = messageToAck?.GetYear();
            this.PayloadVersionId = $"{GeneratedCustomProperty.Value}";

            if(typeof(BFDRVoidMessage).IsInstanceOfType(messageToAck))
            {
                BFDRVoidMessage voidMessageToAck = (BFDRVoidMessage) messageToAck;
                this.BlockCount = voidMessageToAck?.BlockCount;
            }
        }

        /// <summary>
        /// Construct an BFDRAcknowledgementMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BFDRAcknowledgementMessage</param>
        /// <returns></returns>
        internal BFDRAcknowledgementMessage(Bundle messageBundle) : base(messageBundle)
        {
            // no payload for Ack message
        }

        /// <summary>Constructor that creates an acknowledgement for the specified message.</summary>
        /// <param name="messageId">the id of the message to create an acknowledgement for.</param>
        /// <param name="destination">the endpoint identifier that the ack message will be sent to.</param>
        /// <param name="source">the endpoint identifier that the ack message will be sent from.</param>
        public BFDRAcknowledgementMessage(string messageId, string destination, string source = "http://nchs.cdc.gov/bfdr_submission") : base(MESSAGE_TYPE)
        {
            Header.Source.Endpoint = source;
            this.MessageDestination = destination;
            MessageHeader.ResponseComponent resp = new MessageHeader.ResponseComponent();
            resp.Identifier = messageId;
            resp.Code = MessageHeader.ResponseType.Ok;
            Header.Response = resp;
        }

        /// <summary>The id of the message that is being acknowledged by this message</summary>
        /// <value>the message id.</value>
        public string AckedMessageId
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

        /// <summary>The number of records to void starting at the certificate number specified by the `CertNo` parameter</summary>
        public uint? BlockCount
        {
            get
            {
                return (uint?)Record?.GetSingleValue<PositiveInt>("block_count")?.Value;
            }
            set
            {
                Record.Remove("block_count");
                if (value != null && value > 1)
                {
                    Record.Add("block_count", new PositiveInt((int)value));
                }
            }
        }

    }
}

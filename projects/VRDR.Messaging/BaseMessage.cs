using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using VR;

namespace VRDR
{
    /// <summary>Class <c>BaseMessage</c> is the base class of all messages.</summary>
    public partial class BaseMessage : CommonMessage
    {
        /// <summary>
        /// Construct a BaseMessage from a FHIR Bundle. This constructor will also validate that the Bundle
        /// represents a FHIR message of the correct type.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BaseMessage</param>
        /// <param name="ignoreMissingEntries">if true, then missing bundle entries will not result in an exception</param>
        /// <param name="ignoreBundleType">if true, then an incorrect bundle type will not result in an exception</param>
        protected BaseMessage(Bundle messageBundle, bool ignoreMissingEntries = false, bool ignoreBundleType = false)
        {
            MessageBundle = messageBundle;

            // Validate bundle type is message
            if (messageBundle?.Type != Bundle.BundleType.Message && !ignoreBundleType)
            {
                String actualType = messageBundle?.Type == null ? "null" : messageBundle?.Type.ToString();
                throw new MessageParseException($"The FHIR Bundle must be of type message, not {actualType}", new BaseMessage(messageBundle, true, true));
            }

            // Find Header
            Header = findEntry<MessageHeader>(ignoreMissingEntries);

            // Find Parameters
            Record = findEntry<Parameters>(ignoreMissingEntries);
        }

        /// <summary>
        /// Constructor that creates a baseMessage for the specified message type.
        /// </summary>
        /// <param name="messageType">string specifying type of message</param>
        protected BaseMessage(String messageType) : base(messageType)
        {
            MessageHeader.MessageDestinationComponent dest = new MessageHeader.MessageDestinationComponent();
            dest.Endpoint = DeathRecordSubmissionMessage.MESSAGE_TYPE;
            Header.Destination.Add(dest);
            // Set payload version identifier
            this.PayloadVersionId = $"{GeneratedCustomProperty.Value}";
        }

        // TODO: Think about a common approach for extracting business identifiers across VRDR and BFDR
        /// <summary>
        /// Extract the business identifiers for the message from the supplied death record.
        /// </summary>
        /// <param name="from">the death record to extract the identifiers from</param>
        protected void ExtractBusinessIdentifiers(DeathRecord from)
        {
            uint certificateNumber;
            if (UInt32.TryParse(from?.Identifier, out certificateNumber))
            {
                this.CertNo = certificateNumber;
            }
            // take the first state local identifier if it exists
            if (from?.StateLocalIdentifier1 != null)
            {
                this.StateAuxiliaryId = from.StateLocalIdentifier1;
            }
            else
            {
                this.StateAuxiliaryId = null;
            }
            if (from?.DeathYear != null)
            {
                this.DeathYear = (uint)from.DeathYear;
            }
            this.JurisdictionId = from?.DeathLocationJurisdiction;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Message Properties
        //
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>Override GetYear method to be implemented differently by VRDR for backwards compatibility</summary>
        public override uint? GetYear()
        {
            return DeathYear;
        }

        /// <summary>The year in which the death occurred</summary>
        [FHIRPath("Bundle.entry.resource.where($this is Parameters)", "")]
        public uint? DeathYear
        {
            get
            {
                return (uint?)Record?.GetSingleValue<UnsignedInt>("death_year")?.Value;
            }
            set
            {
                Record.Remove("death_year");
                if (value != null)
                {
                    if (value < 1000 || value > 9999)
                    {
                        throw new ArgumentException("Year of death must be specified using four digits");
                    }
                    Record.Add("death_year", new UnsignedInt((int)value));
                }
            }
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// BaseMessage. The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        /// <exception cref="MessageParseException">Thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(StreamReader source, bool permissive = false) where T : BaseMessage
        {
            BaseMessage typedMessage = Parse(source, permissive);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Construct the appropriate subclass of BaseMessage based on a FHIR Bundle.
        /// The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="bundle">A FHIR Bundle</param>
        /// <returns>The message object of the appropriate message type</returns>
        /// <exception cref="MessageParseException">Thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(Bundle bundle) where T : BaseMessage
        {
            BaseMessage typedMessage = Parse(bundle);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// BaseMessage. The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>the deserialized message object</returns>
        /// <exception cref="MessageParseException">thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(string source, bool permissive = false) where T : BaseMessage
        {
            BaseMessage typedMessage = Parse(source, permissive);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// BaseMessage. Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        public static BaseMessage Parse(string source, bool permissive = false)
        {
            Bundle bundle = ParseGenericBundle(source, permissive);

            return Parse(bundle);
        }

        /// <summary>
        /// Construct the appropriate subclass of BaseMessage based on a FHIR Bundle.
        /// Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="bundle">A FHIR Bundle</param>
        /// <returns>The message object of the appropriate message type</returns>
        public static BaseMessage Parse(Bundle bundle)
        {
            BaseMessage message = new BaseMessage(bundle, true, false);
            switch (message.MessageType)
            {
                case DeathRecordSubmissionMessage.MESSAGE_TYPE:
                    message = new DeathRecordSubmissionMessage(bundle, message);
                    break;
                case DeathRecordUpdateMessage.MESSAGE_TYPE:
                    message = new DeathRecordUpdateMessage(bundle, message);
                    break;
                case AcknowledgementMessage.MESSAGE_TYPE:
                    message = new AcknowledgementMessage(bundle);
                    break;
                case DeathRecordVoidMessage.MESSAGE_TYPE:
                    message = new DeathRecordVoidMessage(bundle);
                    break;
                case DeathRecordAliasMessage.MESSAGE_TYPE:
                    message = new DeathRecordAliasMessage(bundle);
                    break;
                case CauseOfDeathCodingMessage.MESSAGE_TYPE:
                    message = new CauseOfDeathCodingMessage(bundle, message);
                    break;
                case DemographicsCodingMessage.MESSAGE_TYPE:
                    message = new DemographicsCodingMessage(bundle, message);
                    break;
                case IndustryOccupationCodingMessage.MESSAGE_TYPE:
                    message = new IndustryOccupationCodingMessage(bundle, message);
                    break;
                case CauseOfDeathCodingUpdateMessage.MESSAGE_TYPE:
                    message = new CauseOfDeathCodingUpdateMessage(bundle, message);
                    break;
                case DemographicsCodingUpdateMessage.MESSAGE_TYPE:
                    message = new DemographicsCodingUpdateMessage(bundle, message);
                    break;
                case IndustryOccupationCodingUpdateMessage.MESSAGE_TYPE:
                    message = new IndustryOccupationCodingUpdateMessage(bundle, message);
                    break;
                case ExtractionErrorMessage.MESSAGE_TYPE:
                    message = new ExtractionErrorMessage(bundle, message);
                    break;
                case StatusMessage.MESSAGE_TYPE:
                    message = new StatusMessage(bundle);
                    break;
                default:
                    string errorText;
                    if (message.Header == null)
                    {
                        errorText = "Failed to find a Bundle Entry containing a Resource of type MessageHeader";
                    }
                    else if (message.MessageType == null)
                    {
                        errorText = "Message type was missing from MessageHeader";
                    }
                    else
                    {
                        errorText = $"Unsupported message type: {message.MessageType}";
                    }
                    throw new MessageParseException(errorText, message);
            }
            return message;
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// BaseMessage. Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        public static BaseMessage Parse(StreamReader source, bool permissive = false)
        {
            string content = source.ReadToEnd();
            return Parse(content, permissive);
        }

        /// <summary>
        /// Convert message to message type and extract the death record
        /// </summary>
        /// <param name="message">base message</param>
        /// <returns>The death record inside the base message</returns>
        public static DeathRecord GetDeathRecordFromMessage(BaseMessage message)
        {

            Type messageType = message.GetType();

            DeathRecord dr = null;

            switch (messageType.Name)
            {
                case "DeathRecordSubmissionMessage":
                    {
                        var drsm = message as DeathRecordSubmissionMessage;
                        dr = drsm?.DeathRecord;
                        break;
                    }
                case "DeathRecordUpdateMessage":
                    {
                        var drsm = message as DeathRecordUpdateMessage;
                        dr = drsm?.DeathRecord;
                        break;
                    }
                case "CauseOfDeathCodingMessage":
                    {
                        var drsm = message as CauseOfDeathCodingMessage;
                        dr = drsm?.DeathRecord;
                        break;
                    }
                case "CauseOfDeathCodingUpdateMessage":
                    {
                        var drsm = message as CauseOfDeathCodingUpdateMessage;
                        dr = drsm?.DeathRecord;
                        break;
                    }
                case "DemographicsCodingMessage":
                    {
                        var drsm = message as DemographicsCodingMessage;
                        dr = drsm?.DeathRecord;
                        break;
                    }
                case "DemographicsCodingUpdateMessage":
                    {
                        var drsm = message as DemographicsCodingUpdateMessage;
                        dr = drsm?.DeathRecord;
                        break;
                    }
                case "IndustryOccupationCodingMessage":
                    {
                        var drsm = message as IndustryOccupationCodingMessage;
                        dr = drsm?.DeathRecord;
                        break;
                    }
                case "IndustryOccupationCodingUpdateMessage":
                    {
                        var drsm = message as IndustryOccupationCodingUpdateMessage;
                        dr = drsm?.DeathRecord;
                        break;
                    }
            }

            return dr;
        }
    }

    /// <summary>
    /// An exception that may be thrown during message parsing
    /// </summary>
    public class MessageParseException : System.ArgumentException
    {
        private BaseMessage sourceMessage;

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="errorMessage">A text error message describing the problem</param>
        /// <param name="sourceMessage">The message that caused the problem</param>
        public MessageParseException(string errorMessage, BaseMessage sourceMessage) : base(errorMessage)
        {
            this.sourceMessage = sourceMessage;
        }

        /// <summary>
        /// Build an ExtractionErrorMessage that conveys the issues reported in this exception.
        /// </summary>
        public ExtractionErrorMessage CreateExtractionErrorMessage()
        {
            var message = new ExtractionErrorMessage(sourceMessage);
            message.Issues.Add(new Issue(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.Exception, this.Message));
            return message;
        }
    }
}

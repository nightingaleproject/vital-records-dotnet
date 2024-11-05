using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using VR;

namespace BFDR
{
    /// <summary>Class <c>BirthRecordBaseMessage</c> is the base class of all messages.</summary>
    public partial class BirthRecordBaseMessage : CommonMessage
    {
        /// <summary>
        /// Construct a BirthRecordBaseMessage from a FHIR Bundle. This constructor will also validate that the Bundle
        /// represents a FHIR message of the correct type.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the CommonMessage</param>
        /// <param name="ignoreMissingEntries">if true, then missing bundle entries will not result in an exception</param>
        /// <param name="ignoreBundleType">if true, then an incorrect bundle type will not result in an exception</param>
        protected BirthRecordBaseMessage(Bundle messageBundle, bool ignoreMissingEntries = false, bool ignoreBundleType = false)
        {
            MessageBundle = messageBundle;

            // Validate bundle type is message
            if (messageBundle?.Type != Bundle.BundleType.Message && !ignoreBundleType)
            {
                String actualType = messageBundle?.Type == null ? "null" : messageBundle?.Type.ToString();
                throw new MessageParseException($"The FHIR Bundle must be of type message, not {actualType}", new BirthRecordBaseMessage(messageBundle, true, true));
            }

            // Find Header
            Header = findEntry<MessageHeader>(ignoreMissingEntries);

            // Find Parameters
            Record = findEntry<Parameters>(ignoreMissingEntries);
        }

        /// <summary>Constructor that creates a new, empty message for the specified message type.</summary>
        protected BirthRecordBaseMessage(String messageType) : base(messageType)
        {
            MessageHeader.MessageDestinationComponent dest = new MessageHeader.MessageDestinationComponent();
            dest.Endpoint = BirthRecordSubmissionMessage.MESSAGE_TYPE;
            Header.Destination.Add(dest);
            // Set payload version identifier
            this.PayloadVersionId = $"{GeneratedCustomProperty.Value}";
        }

        // TODO: Think about a common approach for extracting business identifiers across VRDR and BFDR
        /// <summary>
        /// Extract the business identifiers for the message from the supplied birth record.
        /// </summary>
        /// <param name="from">the birth record to extract the identifiers from</param>
        protected void ExtractBusinessIdentifiers(BirthRecord from)
        {
            uint certificateNumber;
            if (UInt32.TryParse(from?.CertificateNumber, out certificateNumber))
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
            if (from?.BirthYear != null)
            {
                this.BirthYear = (uint)from.BirthYear;
            }
            this.JurisdictionId = from?.BirthLocationJurisdiction;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Message Properties
        //
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>The year in which the birth occurred</summary>
        public uint? BirthYear
        {
            get
            {
                return (uint?)Record?.GetSingleValue<UnsignedInt>("birth_year")?.Value;
            }
            set
            {
                Record.Remove("birth_year");
                if (value != null)
                {
                    if (value < 1000 || value > 9999) {
                        throw new ArgumentException("Year of birth must be specified using four digits");
                    }
                    Record.Add("birth_year", new UnsignedInt((int)value));
                }
            }
        }

        /// <summary>NCHS identifier. Format is 4-digit year, two character jurisdiction id, six character/digit certificate id.</summary>
        public string NCHSIdentifier
        {
            get
            {
                if (BirthYear == null || JurisdictionId == null || CertNo == null)
                {
                    return null;
                }
                return BirthYear.Value.ToString("D4") + JurisdictionId + CertNo.Value.ToString("D6");
            }
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// BirthRecordBaseMessage. The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        /// <exception cref="MessageParseException">Thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(StreamReader source, bool permissive = false) where T: BirthRecordBaseMessage
        {
            BirthRecordBaseMessage typedMessage = Parse(source, permissive);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Construct the appropriate subclass of BirthRecordBaseMessage based on a FHIR Bundle.
        /// The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="bundle">A FHIR Bundle</param>
        /// <returns>The message object of the appropriate message type</returns>
        /// <exception cref="MessageParseException">Thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(Bundle bundle) where T: BirthRecordBaseMessage
        {
            BirthRecordBaseMessage typedMessage = Parse(bundle);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// BirthRecordBaseMessage. The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>the deserialized message object</returns>
        /// <exception cref="MessageParseException">thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(string source, bool permissive = false) where T: BirthRecordBaseMessage
        {
            BirthRecordBaseMessage typedMessage = Parse(source, permissive);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// BirthRecordBaseMessage. Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        public static BirthRecordBaseMessage Parse(string source, bool permissive = false)
        {
            Bundle bundle = ParseGenericBundle(source, permissive);

            return Parse(bundle);
        }

        /// <summary>
        /// Construct the appropriate subclass of BirthRecordBaseMessage based on a FHIR Bundle.
        /// Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="bundle">A FHIR Bundle</param>
        /// <returns>The message object of the appropriate message type</returns>
        public static BirthRecordBaseMessage Parse(Bundle bundle)
        {
            BirthRecordBaseMessage message = new BirthRecordBaseMessage(bundle, true, false);
            switch (message.MessageType)
            {
                case BirthRecordSubmissionMessage.MESSAGE_TYPE:
                    message = new BirthRecordSubmissionMessage(bundle, message);
                    break;
                case BirthRecordUpdateMessage.MESSAGE_TYPE:
                    message = new BirthRecordUpdateMessage(bundle, message);
                    break;
                case BirthRecordAcknowledgementMessage.MESSAGE_TYPE:
                    message = new BirthRecordAcknowledgementMessage(bundle);
                    break;
                case BirthRecordVoidMessage.MESSAGE_TYPE:
                    message = new BirthRecordVoidMessage(bundle);
                    break;
                case BirthRecordErrorMessage.MESSAGE_TYPE:
                    message = new BirthRecordErrorMessage(bundle, message);
                    break;
                case BirthRecordStatusMessage.MESSAGE_TYPE:
                    message = new BirthRecordStatusMessage(bundle);
                    break;
                case BirthRecordDemographicsCodingMessage.MESSAGE_TYPE:
                    message = new BirthRecordDemographicsCodingMessage(bundle, message);
                    break;
                case BirthRecordDemographicsCodingUpdateMessage.MESSAGE_TYPE:
                    message = new BirthRecordDemographicsCodingUpdateMessage(bundle, message);
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
        /// BirthRecordBaseMessage. Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        public static BirthRecordBaseMessage Parse(StreamReader source, bool permissive = false)
        {
            string content = source.ReadToEnd();
            return Parse(content, permissive);
        }

        /// <summary>
        /// Convert message to message type and extract the birth record
        /// </summary>
        /// <param name="message">base message</param>
        /// <returns>The birth record inside the base message</returns>
        public static BirthRecord GetBirthRecordFromMessage(BirthRecordBaseMessage message)
        {
                
            Type messageType = message.GetType();

            BirthRecord br = null;

            switch (messageType.Name)
            {
                case "BirthRecordSubmissionMessage":
                {
                    var brsm = message as BirthRecordSubmissionMessage;
                    br = brsm?.BirthRecord;
                    break;
                }
                case "BirthRecordUpdateMessage":
                {
                    var brsm = message as BirthRecordUpdateMessage;
                    br = brsm?.BirthRecord;
                    break;
                }
                case "BirthRecordDemographicsCodingMessage":
                {
                    var brsm = message as BirthRecordDemographicsCodingMessage;
                    br = brsm?.BirthRecord;
                    break;
                }
                case "BirthRecordDemographicsCodingUpdateMessage":
                {
                    var brsm = message as BirthRecordDemographicsCodingUpdateMessage;
                    br = brsm?.BirthRecord;
                    break;
                }
            }

            return br;
        }
    }

    /// <summary>
    /// An exception that may be thrown during message parsing
    /// </summary>
    public class MessageParseException : System.ArgumentException
    {
        private BirthRecordBaseMessage sourceMessage;

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="errorMessage">A text error message describing the problem</param>
        /// <param name="sourceMessage">The message that caused the problem</param>
        public MessageParseException(string errorMessage, BirthRecordBaseMessage sourceMessage) : base(errorMessage)
        {
            this.sourceMessage = sourceMessage;
        }

        /// <summary>
        /// Build an ExtractionErrorMessage that conveys the issues reported in this exception.
        /// </summary>
        public BirthRecordErrorMessage CreateExtractionErrorMessage()
        {
            var message = new BirthRecordErrorMessage(sourceMessage);
            message.Issues.Add(new Issue(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.Exception, this.Message));
            return message;
        }
    }
}

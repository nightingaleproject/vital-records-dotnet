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
    /// <summary>Class <c>BFDRBaseMessage</c> is the base class of all messages.</summary>
    public partial class BFDRBaseMessage : CommonMessage
    {
        /// <summary>
        /// The event URI for BirthRecordSubmission.
        /// </summary>
        public const String MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_submission";

        /// <summary>
        /// Construct a BFDRBaseMessage from a FHIR Bundle. This constructor will also validate that the Bundle
        /// represents a FHIR message of the correct type.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the CommonMessage</param>
        /// <param name="ignoreMissingEntries">if true, then missing bundle entries will not result in an exception</param>
        /// <param name="ignoreBundleType">if true, then an incorrect bundle type will not result in an exception</param>
        protected BFDRBaseMessage(Bundle messageBundle, bool ignoreMissingEntries = false, bool ignoreBundleType = false) : base(messageBundle)
        {
            MessageBundle = messageBundle;

            // Validate bundle type is message
            if (messageBundle?.Type != Bundle.BundleType.Message && !ignoreBundleType)
            {
                String actualType = messageBundle?.Type == null ? "null" : messageBundle?.Type.ToString();
                throw new MessageParseException($"The FHIR Bundle must be of type message, not {actualType}", new BFDRBaseMessage(messageBundle, true, true));
            }

            // Find Header
            Header = findEntry<MessageHeader>(ignoreMissingEntries);

            // Find Parameters
            Record = findEntry<Parameters>(ignoreMissingEntries);
        }

        /// <summary>Constructor that creates a new, empty message for the specified message type.</summary>
        protected BFDRBaseMessage(String messageType) : base(messageType)
        {
            MessageHeader.MessageDestinationComponent dest = new MessageHeader.MessageDestinationComponent();
            dest.Endpoint = BFDRBaseMessage.MESSAGE_TYPE;
            Header.Destination.Add(dest);
        }

        /// <summary>
        /// Extract the business identifiers for the message from the supplied birth record.
        /// </summary>
        /// <param name="from">the birth record to extract the identifiers from</param>
        protected void ExtractBusinessIdentifiers(NatalityRecord from)
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
            if (from?.GetYear() != null)
            {
                this.SetYear((uint)from.GetYear());
            }
            this.JurisdictionId = from?.BirthLocationJurisdiction;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Message Properties
        //
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// BFDRBaseMessage. The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        /// <exception cref="MessageParseException">Thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(StreamReader source, bool permissive = false) where T: BFDRBaseMessage
        {
            BFDRBaseMessage typedMessage = Parse(source, permissive);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Construct the appropriate subclass of BFDRBaseMessage based on a FHIR Bundle.
        /// The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="bundle">A FHIR Bundle</param>
        /// <returns>The message object of the appropriate message type</returns>
        /// <exception cref="MessageParseException">Thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(Bundle bundle) where T: BFDRBaseMessage
        {
            BFDRBaseMessage typedMessage = Parse(bundle);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// BFDRBaseMessage. The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>the deserialized message object</returns>
        /// <exception cref="MessageParseException">thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(string source, bool permissive = false) where T: BFDRBaseMessage
        {
            BFDRBaseMessage typedMessage = Parse(source, permissive);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// BFDRBaseMessage. Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        public static BFDRBaseMessage Parse(string source, bool permissive = false)
        {
            Bundle bundle = ParseGenericBundle(source, permissive);

            return Parse(bundle);
        }

        /// <summary>
        /// Construct the appropriate subclass of BFDRBaseMessage based on a FHIR Bundle.
        /// Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="bundle">A FHIR Bundle</param>
        /// <returns>The message object of the appropriate message type</returns>
        public static BFDRBaseMessage Parse(Bundle bundle)
        {
            BFDRBaseMessage message = new BFDRBaseMessage(bundle, true, false);
            switch (message.MessageType)
            {
                case BFDRAcknowledgementMessage.MESSAGE_TYPE:
                    message = new BFDRAcknowledgementMessage(bundle);
                    break;
                case BFDRErrorMessage.MESSAGE_TYPE:
                    message = new BFDRErrorMessage(bundle, message);
                    break;
                case BFDRStatusMessage.MESSAGE_TYPE:
                    message = new BFDRStatusMessage(bundle);
                    break;
                case BFDRVoidMessage.MESSAGE_TYPE:
                    message = new BFDRVoidMessage(bundle);
                    break;
                case BFDRParentalDemographicsCodingMessage.MESSAGE_TYPE:
                    message = new BFDRParentalDemographicsCodingMessage(bundle, message);
                    break;
                case BFDRParentalDemographicsCodingUpdateMessage.MESSAGE_TYPE:
                    message = new BFDRParentalDemographicsCodingUpdateMessage(bundle, message);
                    break;
                case FetalDeathRecordSubmissionMessage.MESSAGE_TYPE:
                    message = new FetalDeathRecordSubmissionMessage(bundle, message);
                    break;
                case FetalDeathRecordUpdateMessage.MESSAGE_TYPE:
                    message = new FetalDeathRecordUpdateMessage(bundle, message);
                    break;
                case BirthRecordSubmissionMessage.MESSAGE_TYPE:
                    message = new BirthRecordSubmissionMessage(bundle, message);
                    break;
                case BirthRecordUpdateMessage.MESSAGE_TYPE:
                    message = new BirthRecordUpdateMessage(bundle, message);
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
        /// BFDRBaseMessage. Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        public static BFDRBaseMessage Parse(StreamReader source, bool permissive = false)
        {
            string content = source.ReadToEnd();
            return Parse(content, permissive);
        }
        /// <summary>
        /// Convert message to message type and extract the birth record
        /// </summary>
        /// <param name="message">base message</param>
        /// <returns>The natality record inside the base message</returns>
        public static NatalityRecord GetNatalityRecordFromMessage(BFDRBaseMessage message)
        {
                
            Type messageType = message.GetType();

            NatalityRecord nr = null;

            switch (messageType.Name)
            {
                case "BirthRecordSubmissionMessage":
                {
                    var brsm = message as BirthRecordSubmissionMessage;
                    nr = brsm?.BirthRecord;
                    break;
                }
                case "BirthRecordUpdateMessage":
                {
                    var brsm = message as BirthRecordUpdateMessage;
                    nr = brsm?.BirthRecord;
                    break;
                }
                case "BFDRParentalDemographicsCodingMessage":
                {
                    var brsm = message as BFDRParentalDemographicsCodingMessage;
                    nr = brsm?.NatalityRecord;
                    break;
                }
                case "BFDRParentalDemographicsCodingUpdateMessage":
                {
                    var brsm = message as BFDRParentalDemographicsCodingUpdateMessage;
                    nr = brsm?.NatalityRecord;
                    break;
                }
                case "FetalDeathRecordSubmissionMessage":
                {
                    var brsm = message as FetalDeathRecordSubmissionMessage;
                    nr = brsm?.FetalDeathRecord;
                    break;
                }
                case "FetalDeathRecordUpdateMessage":
                {
                    var brsm = message as FetalDeathRecordUpdateMessage;
                    nr = brsm?.FetalDeathRecord;
                    break;
                }
            }

            return nr;
        }
    }

    /// <summary>
    /// An exception that may be thrown during message parsing
    /// </summary>
    public class MessageParseException : System.ArgumentException
    {
        private BFDRBaseMessage sourceMessage;

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="errorMessage">A text error message describing the problem</param>
        /// <param name="sourceMessage">The message that caused the problem</param>
        public MessageParseException(string errorMessage, BFDRBaseMessage sourceMessage) : base(errorMessage)
        {
            this.sourceMessage = sourceMessage;
        }

        /// <summary>
        /// Build an ExtractionErrorMessage that conveys the issues reported in this exception.
        /// </summary>
        public BFDRErrorMessage CreateBFDRExtractionErrorMessage()
        {
            var message = new BFDRErrorMessage(sourceMessage);
            message.Issues.Add(new Issue(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.Exception, this.Message));
            return message;
        }
    }
}

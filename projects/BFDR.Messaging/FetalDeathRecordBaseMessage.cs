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
    /// <summary>Class <c>FetalDeathRecordBaseMessage</c> is the base class of all messages.</summary>
    public partial class FetalDeathRecordBaseMessage : BFDRBaseMessage
    {
        /// <summary>
        /// Construct a FetalDeathRecordBaseMessage from a FHIR Bundle. This constructor will also validate that the Bundle
        /// represents a FHIR message of the correct type.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the CommonMessage</param>
        /// <param name="ignoreMissingEntries">if true, then missing bundle entries will not result in an exception</param>
        /// <param name="ignoreBundleType">if true, then an incorrect bundle type will not result in an exception</param>
        protected FetalDeathRecordBaseMessage(Bundle messageBundle, bool ignoreMissingEntries = false, bool ignoreBundleType = false) : base(messageBundle)
        {
            // MessageBundle = messageBundle;

            // // Validate bundle type is message
            // if (messageBundle?.Type != Bundle.BundleType.Message && !ignoreBundleType)
            // {
            //     String actualType = messageBundle?.Type == null ? "null" : messageBundle?.Type.ToString();
            //     throw new MessageParseException($"The FHIR Bundle must be of type message, not {actualType}", new FetalDeathRecordBaseMessage(messageBundle, true, true));
            // }

            // // Find Header
            // Header = findEntry<MessageHeader>(ignoreMissingEntries);

            // // Find Parameters
            // Record = findEntry<Parameters>(ignoreMissingEntries);
        }

        /// <summary>Constructor that creates a new, empty message for the specified message type.</summary>
        protected FetalDeathRecordBaseMessage(String messageType) : base(messageType)
        {
            // MessageHeader.MessageDestinationComponent dest = new MessageHeader.MessageDestinationComponent();
            // dest.Endpoint = FetalDeathRecordSubmissionMessage.MESSAGE_TYPE;
            // Header.Destination.Add(dest);
        }

        // TODO: Think about a common approach for extracting business identifiers across VRDR and BFDR
        /// <summary>
        /// Extract the business identifiers for the message from the supplied fetal death record.
        /// </summary>
        /// <param name="from">the fetal death record to extract the identifiers from</param>
        protected void ExtractBusinessIdentifiers(FetalDeathRecord from)
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
                this.DeathYear = (uint)from.DeathYear;
            }
            this.JurisdictionId = from?.BirthLocationJurisdiction;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Message Properties
        //
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>Override GetYear method to be implemented differently by the FetalDeathRecord and FetalDeathRecord subclasses</summary>
        public override uint? GetYear()
        {
            return this.DeathYear;
        }

        /// <summary>Override SetYear method to be implemented differently by the Birth Message and FetalDeath Message subclasses</summary>
        public override void SetYear(uint? year)
        {
            this.DeathYear = year;
        }

        /// TODO move this to an override for GetYear and SetYear in fetal death messaging
        /// <summary>The year in which the fetal death occurred</summary>
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
                    if (value < 1000 || value > 9999) {
                        throw new ArgumentException("Year of death must be specified using four digits");
                    }
                    Record.Add("death_year", new UnsignedInt((int)value));
                }
            }
        }

        /// <summary>NCHS identifier. Format is 4-digit year, two character jurisdiction id, six character/digit certificate id.</summary>
        public string NCHSIdentifier
        {
            get
            {
                if (DeathYear == null || JurisdictionId == null || CertNo == null)
                {
                    return null;
                }
                return DeathYear.Value.ToString("D4") + JurisdictionId + CertNo.Value.ToString("D6");
            }
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// FetalDeathRecordBaseMessage. The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        /// <exception cref="MessageParseException">Thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(StreamReader source, bool permissive = false) where T: FetalDeathRecordBaseMessage
        {
            FetalDeathRecordBaseMessage typedMessage = Parse(source, permissive);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Construct the appropriate subclass of FetalDeathRecordBaseMessage based on a FHIR Bundle.
        /// The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="bundle">A FHIR Bundle</param>
        /// <returns>The message object of the appropriate message type</returns>
        /// <exception cref="MessageParseException">Thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(Bundle bundle) where T: FetalDeathRecordBaseMessage
        {
            FetalDeathRecordBaseMessage typedMessage = Parse(bundle);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// FetalDeathRecordBaseMessage. The new object is checked to ensure it the same or a subtype of the type parameter.
        /// </summary>
        /// <typeparam name="T">the expected message type</typeparam>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>the deserialized message object</returns>
        /// <exception cref="MessageParseException">thrown when source does not represent the same or a subtype of the type parameter.</exception>
        public static T Parse<T>(string source, bool permissive = false) where T: FetalDeathRecordBaseMessage
        {
            FetalDeathRecordBaseMessage typedMessage = Parse(source, permissive);
            if (!typeof(T).IsInstanceOfType(typedMessage))
            {
                throw new MessageParseException($"The supplied message was of type {typedMessage.GetType()}, expected {typeof(T)} or a subclass", typedMessage);
            }
            return (T)typedMessage;
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct the appropriate subclass of
        /// FetalDeathRecordBaseMessage. Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        public static FetalDeathRecordBaseMessage Parse(string source, bool permissive = false)
        {
            Bundle bundle = ParseGenericBundle(source, permissive);

            return Parse(bundle);
        }

        /// <summary>
        /// Construct the appropriate subclass of FetalDeathRecordBaseMessage based on a FHIR Bundle.
        /// Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="bundle">A FHIR Bundle</param>
        /// <returns>The message object of the appropriate message type</returns>
        public static FetalDeathRecordBaseMessage Parse(Bundle bundle)
        {
            FetalDeathRecordBaseMessage message = new FetalDeathRecordBaseMessage(bundle, true, false);
            switch (message.MessageType)
            {
                case FetalDeathRecordSubmissionMessage.MESSAGE_TYPE:
                    message = new FetalDeathRecordSubmissionMessage(bundle, message);
                    break;
                case FetalDeathRecordUpdateMessage.MESSAGE_TYPE:
                    message = new FetalDeathRecordUpdateMessage(bundle, message);
                    break;
                case FetalDeathRecordAcknowledgementMessage.MESSAGE_TYPE:
                    message = new FetalDeathRecordAcknowledgementMessage(bundle);
                    break;
                case FetalDeathRecordVoidMessage.MESSAGE_TYPE:
                    message = new FetalDeathRecordVoidMessage(bundle);
                    break;
                case FetalDeathRecordErrorMessage.MESSAGE_TYPE:
                    message = new FetalDeathRecordErrorMessage(bundle, message);
                    break;
                case FetalDeathRecordStatusMessage.MESSAGE_TYPE:
                    message = new FetalDeathRecordStatusMessage(bundle);
                    break;
                case FetalDeathRecordDemographicsCodingMessage.MESSAGE_TYPE:
                    message = new FetalDeathRecordDemographicsCodingMessage(bundle, message);
                    break;
                case FetalDeathRecordDemographicsCodingUpdateMessage.MESSAGE_TYPE:
                    message = new FetalDeathRecordDemographicsCodingUpdateMessage(bundle, message);
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
        /// FetalDeathRecordBaseMessage. Clients can use the typeof operator to determine the type of message object returned.
        /// </summary>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized message object</returns>
        public static FetalDeathRecordBaseMessage Parse(StreamReader source, bool permissive = false)
        {
            string content = source.ReadToEnd();
            return Parse(content, permissive);
        }

        /// <summary>
        /// Convert message to message type and extract the fetal death record
        /// </summary>
        /// <param name="message">base message</param>
        /// <returns>The fetal death record inside the base message</returns>
        public static FetalDeathRecord GetFetalDeathRecordFromMessage(FetalDeathRecordBaseMessage message)
        {
                
            Type messageType = message.GetType();

            FetalDeathRecord br = null;

            switch (messageType.Name)
            {
                case "FetalDeathRecordSubmissionMessage":
                {
                    var brsm = message as FetalDeathRecordSubmissionMessage;
                    br = brsm?.FetalDeathRecord;
                    break;
                }
                case "FetalDeathRecordUpdateMessage":
                {
                    var brsm = message as FetalDeathRecordUpdateMessage;
                    br = brsm?.FetalDeathRecord;
                    break;
                }
                case "FetalDeathRecordDemographicsCodingMessage":
                {
                    var brsm = message as FetalDeathRecordDemographicsCodingMessage;
                    br = brsm?.FetalDeathRecord;
                    break;
                }
                case "FetalDeathRecordDemographicsCodingUpdateMessage":
                {
                    var brsm = message as FetalDeathRecordDemographicsCodingUpdateMessage;
                    br = brsm?.FetalDeathRecord;
                    break;
                }
            }

            return br;
        }
    }

    /// <summary>
    /// An exception that may be thrown during message parsing
    /// </summary>
    public class FetalDeathMessageParseException : System.ArgumentException
    {
        private FetalDeathRecordBaseMessage sourceMessage;

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="errorMessage">A text error message describing the problem</param>
        /// <param name="sourceMessage">The message that caused the problem</param>
        public FetalDeathMessageParseException(string errorMessage, FetalDeathRecordBaseMessage sourceMessage) : base(errorMessage)
        {
            this.sourceMessage = sourceMessage;
        }

        /// <summary>
        /// Build an ExtractionErrorMessage that conveys the issues reported in this exception.
        /// </summary>
        public FetalDeathRecordErrorMessage CreateExtractionErrorMessage()
        {
            var message = new FetalDeathRecordErrorMessage(sourceMessage);
            message.Issues.Add(new Issue(OperationOutcome.IssueSeverity.Error, OperationOutcome.IssueType.Exception, this.Message));
            return message;
        }
    }
}

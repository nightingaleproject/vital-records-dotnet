using System;
using System.Linq;
using System.Collections.Generic;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace VR
{
    /// <summary>Class <c>CommonMessage</c> is the base class of all messages.</summary>
    public class CommonMessage
    {
        /// <summary>Bundle that contains the message.</summary>
        protected Bundle MessageBundle { get; set; }

        /// <summary>
        /// A Parameters entry that contains business identifiers for all messages plus additional information for Coding messages.
        /// </summary>
        protected Parameters Record;

        /// <summary>MessageHeader that contains the message header.</summary>
        protected MessageHeader Header;

        /// <summary>Construct an empty CommonMessage</summary>
        protected CommonMessage() {}

        /// <summary>
        /// Construct a CommonMessage from a FHIR Bundle. This constructor will also validate that the Bundle
        /// represents a FHIR message of the correct type.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the CommonMessage</param>
        /// <param name="ignoreMissingEntries">if true, then missing bundle entries will not result in an exception</param>
        /// <param name="ignoreBundleType">if true, then an incorrect bundle type will not result in an exception</param>
        protected CommonMessage(Bundle messageBundle, bool ignoreMissingEntries = false, bool ignoreBundleType = false)
        {
            MessageBundle = messageBundle;
            // Validate bundle type is message
            if (messageBundle?.Type != Bundle.BundleType.Message && !ignoreBundleType)
            {
                String actualType = messageBundle?.Type == null ? "null" : messageBundle?.Type.ToString();
                throw new System.ArgumentException($"The FHIR Bundle must be of type message, not {actualType}");
            }
            // Find Header
            Header = findEntry<MessageHeader>(ignoreMissingEntries);

            // Find Parameters
            Record = findEntry<Parameters>(ignoreMissingEntries);
        }

        /// <summary>
        /// Returns the typed element for getting FhirPath data from the bundle.
        /// </summary>
        /// <returns></returns>
        public ITypedElement GetITypedElement()
        {
            return MessageBundle.ToTypedElement();
        }

        /// <summary>Constructor that creates a new, empty message for the specified message type.</summary>
        protected CommonMessage(String messageType)
        {
            // Start with a Bundle.
            MessageBundle = new Bundle();
            MessageBundle.Id = Guid.NewGuid().ToString();
            MessageBundle.Type = Bundle.BundleType.Message;
            MessageBundle.Timestamp = DateTime.Now;

            // Start with a MessageHeader.
            Header = new MessageHeader();
            Header.Id = Guid.NewGuid().ToString();
            Header.Event = new FhirUri(messageType);

            MessageHeader.MessageSourceComponent src = new MessageHeader.MessageSourceComponent();
            Header.Source = src;
            MessageBundle.AddResourceEntry(Header, "urn:uuid:" + Header.Id);

            Record = new Parameters();
            Record.Id = Guid.NewGuid().ToString();
            MessageBundle.AddResourceEntry(this.Record, "urn:uuid:" + this.Record.Id);
            Header.Focus.Add(new ResourceReference("urn:uuid:" + this.Record.Id));
        }

        /// <summary>
        /// Find the first Entry within the message Bundle that contains a Resource of the specified type and return that resource.
        /// </summary>
        /// <param name="ignoreMissingEntries">if true, then missing entries will not result in an exception</param>
        /// <typeparam name="T">the class of the FHIR resource to return, must match with specified type:</typeparam>
        /// <returns>The first matching Bundle entry</returns>
        protected T findEntry<T>(bool ignoreMissingEntries = false) where T : Resource
        {
            var typedEntry = MessageBundle.Entry.FirstOrDefault( entry => entry.Resource is T);
            if (typedEntry == null && !ignoreMissingEntries)
            {
                throw new System.ArgumentException($"Failed to find a Bundle Entry containing a Resource of type {typeof(T).FullName}");
            }
            return (T)typedEntry?.Resource;
        }

        /// <summary>
        /// Update the record bundle in this message based on the MessageBundleRecord property (for whichever subclass we're instantiated as).
        /// Important if we're managing a record that might have changed.
        /// </summary>
        protected void UpdateMessageBundleRecord()
        {
            MessageBundle.Entry.RemoveAll( entry => entry.Resource is Bundle);
            Header.Focus.Clear();
            Bundle newBundle = MessageBundleRecord;
            if (newBundle != null)
            {
                MessageBundle.AddResourceEntry(newBundle, "urn:uuid:" + newBundle.Id);
                Header.Focus.Add(new ResourceReference("urn:uuid:" + newBundle.Id));
            }
        }

        /// <summary>
        /// Allow explicit casting of a message into a bundle
        /// </summary>
        /// <param name="message">the record to extract the bundle from</param>
        public static explicit operator Bundle(CommonMessage message) => message.MessageBundle;

        /// <summary>Helper method to return a XML string representation of this Message.</summary>
        /// <param name="prettyPrint">controls whether the returned string is formatted for human readability (true) or compact (false)</param>
        /// <returns>a string representation of this Message in XML format</returns>
        public string ToXML(bool prettyPrint = false)
        {
            UpdateMessageBundleRecord(); // Update the record in the message bundle in case the Record (if present) has changed
            return MessageBundle.ToXml(new FhirXmlSerializationSettings { Pretty = prettyPrint, AppendNewLine = prettyPrint, TrimWhitespaces = prettyPrint });
        }

        /// <summary>Helper method to return a XML string representation of this Message.</summary>
        /// <param name="prettyPrint">controls whether the returned string is formatted for human readability (true) or compact (false)</param>
        /// <returns>a string representation of this Message in XML format</returns>
        public string ToXml(bool prettyPrint = false)
        {
            return ToXML(prettyPrint);
        }

        /// <summary>Helper method to return a JSON string representation of this Message.</summary>
        /// <param name="prettyPrint">controls whether the returned string is formatted for human readability (true) or compact (false)</param>
        /// <returns>a string representation of this Message in JSON format</returns>
        public string ToJSON(bool prettyPrint = false)
        {
            UpdateMessageBundleRecord(); // Update the record in the message bundle in case the Record (if present) has changed
            return MessageBundle.ToJson(new FhirJsonSerializationSettings { Pretty = prettyPrint, AppendNewLine = prettyPrint });
        }

        /// <summary>Helper method to return a JSON string representation of this Message.</summary>
        /// <param name="prettyPrint">controls whether the returned string is formatted for human readability (true) or compact (false)</param>
        /// <returns>a string representation of this Message in JSON format</returns>
        public string ToJson(bool prettyPrint = false)
        {
            return ToJSON(prettyPrint);
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Message Properties
        //
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>The record bundle that should go into the message bundle for this message</summary>
        /// <value>the MessageBundleRecord</value>
        protected virtual Bundle MessageBundleRecord
        {
            get
            {
                // The base message class and some subclasses do not have a record
                return null;
            }
        }

        /// <summary>Message timestamp</summary>
        /// <value>the message timestamp.</value>
        [FHIRPath("Bundle.entry.resource.where($this is Bundle)", "timestamp")]
        public DateTimeOffset? MessageTimestamp
        {
            get
            {
                return MessageBundle.Timestamp;
            }
            set
            {
                MessageBundle.Timestamp = value;
            }
        }

        /// <summary>Message Id</summary>
        /// <value>the message id.</value>
        [FHIRPath("Bundle.entry.resource.where($this is MessageHeader)", "id")]
        public string MessageId
        {
            get
            {
                return Header?.Id;
            }
            set
            {
                Header.Id = value;
                MessageBundle.Entry.RemoveAll( entry => entry.Resource is MessageHeader );
                MessageBundle.AddResourceEntry(Header, "urn:uuid:" + Header.Id);
            }
        }

        /// <summary>Message Type</summary>
        /// <value>the message type.</value>
        [FHIRPath("Bundle.entry.resource.where($this is MessageHeader)", "")]
        public string MessageType
        {
            get
            {
                if (Header?.Event != null && Header.Event.TypeName == "uri")
                {
                    return ((FhirUri)Header.Event).Value;
                }
                else
                {
                    return null;
                }

            }
            set
            {
                Header.Event = new FhirUri(value);
            }
        }

        /// <summary>Message Source</summary>
        /// <value>the message source.</value>
        [FHIRPath("Bundle.entry.resource.where($this is MessageHeader)", "source")]
        public string MessageSource
        {
            get
            {
                return Header?.Source?.Endpoint;
            }
            set
            {
                if (Header.Source == null)
                {
                    Header.Source = new MessageHeader.MessageSourceComponent();
                }
                Header.Source.Endpoint = value;
            }
        }

        /// <summary>Message Destination</summary>
        /// <value>the message destinations, in csv format to support multiple endpoints. Acts as a wrapper for MessageDestinations while still maintaining backwards compatibility.</value>
        [FHIRPath("Bundle.entry.resource.where($this is MessageHeader)", "destination")]
        public string MessageDestination
        {
            get
            {
                List<string> destinations = this.MessageDestinations;
                if (destinations == null || (destinations.Count() == 1 && destinations[0] == null) || destinations.Count() < 1) {
                    return null;
                }
                return String.Join(",", this.MessageDestinations);
            }
            set
            {
                this.MessageDestinations = value != null ? value.Split(',').ToList() : null;
            }
        }

        /// <summary>Message Destinations</summary>
        /// <value>the message destinations in list-based format.</value>
        [FHIRPath("Bundle.entry.resource.where($this is MessageHeader)", "destination")]
        public List<string> MessageDestinations
        {
            get
            {
                return Header?.Destination?.Select(dest => dest.Endpoint).ToList();
            }
            set
            {
                Header.Destination.Clear();
                if (value == null)
                {
                    MessageHeader.MessageDestinationComponent dest = new MessageHeader.MessageDestinationComponent();
                    dest.Endpoint = null;
                    Header.Destination.Add(dest);
                    return;
                }
                foreach (string endpoint in value) {
                    MessageHeader.MessageDestinationComponent dest = new MessageHeader.MessageDestinationComponent();
                    dest.Endpoint = endpoint;
                    Header.Destination.Add(dest);
                }
            }
        }

        /// <summary>Helper method to set a single string value on the Record portion of the Message</summary>
        protected void SetSingleStringValue(string key, string value)
        {
            Record.Remove(key);
            if (!String.IsNullOrWhiteSpace(value))
            {
                Record.Add(key, new FhirString(value));
            }
        }

        /// <summary>Jurisdiction-assigned certificate number</summary>
        [FHIRPath("Bundle.entry.resource.where($this is Parameters)", "")]
        public uint? CertNo
        {
            get
            {
                return (uint?)Record?.GetSingleValue<UnsignedInt>("cert_no")?.Value;
            }
            set
            {
                Record.Remove("cert_no");
                if (value != null)
                {
                    if (value > 999999)
                    {
                        throw new ArgumentException("Certificate number must be a maximum of six digits");
                    }
                    Record.Add("cert_no", new UnsignedInt((int)value));
                }
            }
        }

        /// <summary>Jurisdiction-assigned auxiliary identifier</summary>
        // [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate", "")] UNKNOWN PROPERTY
        public string StateAuxiliaryId
        {
            get
            {
                return Record?.GetSingleValue<FhirString>("state_auxiliary_id")?.Value;
            }
            set
            {
                SetSingleStringValue("state_auxiliary_id", value);
            }
        }

        /// <summary>Two character identifier of the jurisdiction in which the event occurred</summary>
        [FHIRPath("Bundle.entry.resource.where($this is Parameters)", "")]
        public string JurisdictionId
        {
            get
            {
                return Record?.GetSingleValue<FhirString>("jurisdiction_id")?.Value;
            }
            set
            {
                Record.Remove("jurisdiction_id");
                if (value != null)
                {
                    if (value.Length != 2)
                    {
                        throw new ArgumentException("Jurisdiction ID must be a two character string");
                    }
                    Record.Add("jurisdiction_id", new FhirString(value));
                }
            }
        }

        /// <summary>Identifier of the payload version</summary>
        [FHIRPath("Bundle.entry.resource.where($this is Parameters)", "")]
        public string PayloadVersionId
        {
            get
            {
                return Record?.GetSingleValue<FhirString>("payload_version_id")?.Value;
            }
            set
            {
                SetSingleStringValue("payload_version_id", value);
            }
        }
        /// TODO move this to an override for GetYear and SetYear in fetal death messaging
        /// <summary>The year in which the fetal death occurred</summary>
        [FHIRPath("Bundle.entry.resource.where($this is Parameters)", "")]
        public uint? EventYear
        {
            get
            {
                return (uint?)Record?.GetSingleValue<UnsignedInt>("event_year")?.Value;
            }
            set
            {
                Record.Remove("event_year");
                if (value != null)
                {
                    if (value < 1000 || value > 9999) {
                        throw new ArgumentException("Year of event must be specified using four digits");
                    }
                    Record.Add("event_year", new UnsignedInt((int)value));
                }
            }
        }

        /// <summary>Override GetYear method to be implemented differently by VRDR for backwards compatibility</summary>
        public virtual uint? GetYear()
        {
            return this.EventYear;
        }

        /// <summary>Override SetYear method to be implemented differently by VRDR for backwards compatibility</summary>
        public void SetYear(uint? year)
        {
            this.EventYear = year;
        }

        /// <summary>NCHS identifier. Format is 4-digit year, two character jurisdiction id, six character/digit certificate id.</summary>
        [FHIRPath("Bundle.entry.resource.where($this is Bundle)", "identifier")]
        public string NCHSIdentifier
        {
            get
            {
                if (GetYear() == null || JurisdictionId == null || CertNo == null)
                {
                    return null;
                }
                return GetYear().Value.ToString("D4") + JurisdictionId + CertNo.Value.ToString("D6");
            }
        }

        private static ParserSettings GetParserSettings(bool permissive)
        {
            return new ParserSettings { AcceptUnknownMembers = permissive,
                                        AllowUnrecognizedEnums = permissive,
                                        PermissiveParsing = permissive };
        }

        private static Bundle ParseXML(string content, bool permissive)
        {
            Bundle bundle = null;

            // Grab all errors found by visiting all nodes and report if not permissive
            if (!permissive)
            {
                List<string> entries = new List<string>();
                ISourceNode node = FhirXmlNode.Parse(content, new FhirXmlParsingSettings { PermissiveParsing = permissive });
                foreach (Hl7.Fhir.Utility.ExceptionNotification problem in node.VisitAndCatch())
                {
                    entries.Add(problem.Message);
                }
                if (entries.Count > 0)
                {
                    throw new System.ArgumentException(String.Join("; ", entries).TrimEnd());
                }
            }
            // Try Parse
            try
            {
                FhirXmlParser parser = new FhirXmlParser(GetParserSettings(permissive));
                bundle = parser.Parse<Bundle>(content);
            }
            catch (Exception e)
            {
                throw new System.ArgumentException(e.Message);
            }

            return bundle;
        }

        private static Bundle ParseJSON(string content, bool permissive)
        {
            Bundle bundle = null;

            // The purpose of this code is to validate that the content string is a valid JSON.
            // This address the issue of jurisdictions sending a JSON that is not a valid JSON.
            // If it is not, the code throws an ArgumentException with a message indicating the error.
            try
            {
                System.Text.Json.JsonDocument.Parse(content);
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new FormatException(e.Message);
            }
            // Grab all errors found by visiting all nodes and report if not permissive

            if (!permissive)
            {
                List<string> entries = new List<string>();
                ISourceNode node = FhirJsonNode.Parse(content, "Bundle", new FhirJsonParsingSettings { PermissiveParsing = permissive });
                foreach (Hl7.Fhir.Utility.ExceptionNotification problem in node.VisitAndCatch())
                {
                    entries.Add(problem.Message);
                }
                if (entries.Count > 0)
                {
                    throw new System.ArgumentException(String.Join("; ", entries).TrimEnd());
                }
            }
            // Try Parse
            try
            {
                FhirJsonParser parser = new FhirJsonParser(GetParserSettings(permissive));
                bundle = parser.Parse<Bundle>(content);
            }
            catch (Exception e)
            {
                throw new System.ArgumentException(e.Message);
            }

            return bundle;
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle.
        /// </summary>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized bundle object</returns>
        public static Bundle ParseGenericBundle(string source, bool permissive = false)
        {
            if (!String.IsNullOrEmpty(source) && source.TrimStart().StartsWith("<"))
            {
                return ParseXML(source, permissive);
            }
            else if (!String.IsNullOrEmpty(source) && source.TrimStart().StartsWith("{"))
            {
                return ParseJSON(source, permissive);
            }
            else
            {
                throw new System.ArgumentException("The given input does not appear to be a valid XML or JSON FHIR message.");
            }
        }

        /// <summary>
        /// Parse an XML or JSON serialization of a FHIR Bundle and construct a generic CommonMessage.
        /// </summary>
        /// <param name="source">the XML or JSON serialization of a FHIR Bundle</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <returns>The deserialized base message object</returns>
        public static CommonMessage ParseGenericMessage(string source, bool permissive = false)
        {
            Bundle bundle = ParseGenericBundle(source, permissive);
            CommonMessage message = new CommonMessage(bundle, true, false);
            return message;
        }

        /// <summary>
        /// Validate the fields in a CommonMessage header meet NCHS processing requirements. This
        /// function will be used by the STEVE API and NVSS API for validation. Library users can
        /// also use this to test their messages meet the validation requirements before submitting. The function
        /// throws an error if there are invalid fields. Nothing is returned if the message is valid.
        /// </summary>
        /// <param name="message">base message</param>
        /// <returns>void</returns>
        public static void ValidateMessageHeader(CommonMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.MessageSource))
            {
                throw new MessageRuleException("Message source endpoint cannot be null.", message);
            }
            if (string.IsNullOrWhiteSpace(message.MessageDestination))
            {
                throw new MessageRuleException("Message destination endpoint cannot be null.", message);
            }
            if (string.IsNullOrWhiteSpace(message.MessageId))
            {
                throw new MessageRuleException("Message ID cannot be null.", message);
            }
            if (string.IsNullOrWhiteSpace(message.GetType().Name))
            {
                throw new MessageRuleException("Message event type cannot be null.", message);
            }
            if (message.CertNo == null)
            {
                throw new MessageRuleException("Message certificate number cannot be null.", message);
            }
            if ((uint)message.CertNo.ToString().Length > 6)
            {
                throw new MessageRuleException("Message certificate number cannot be more than 6 digits long.", message);
            }
            if (string.IsNullOrWhiteSpace(message.JurisdictionId))
            {
                throw new MessageRuleException($"Message jurisdiction ID cannot be null.", message);
            }
            if (message.EventYear == null)
            {
                throw new MessageRuleException($"Message event year cannot be null.", message);
            }
            return;
        }
    }

    /// <summary>
    /// An exception that may be thrown during message validation. This indicates that the message failed business rule validation.
    /// </summary>
    public class MessageRuleException : ArgumentException
    {
        /// <summary>
        /// Gets the source message that caused the problem.
        /// </summary>
        public readonly CommonMessage SourceMessage;

        /// <summary>
        /// Construct a new instance.
        /// </summary>
        /// <param name="errorMessage">A text error message describing the problem</param>
        /// <param name="sourceMessage">The message that caused the problem</param>
        public MessageRuleException(string errorMessage, CommonMessage sourceMessage) : base(errorMessage)
        {
            SourceMessage = sourceMessage;
        }
    }
}

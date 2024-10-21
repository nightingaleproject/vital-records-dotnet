using System;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>Class <c>CodedCauseOfFetalDeathMessage</c> supports the submission of BFDR records.</summary>
    public class CodedCauseOfFetalDeathMessage : BFDRBaseMessage
    {
        /// <summary>
        /// The event URI for CodedCauseOfFetalDeathMessage.
        /// </summary>
        public new const String MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_causeofdeath";

        /// <summary>Bundle that contains the message payload.</summary>
        private FetalDeathRecord fetalDeathRecord;

        /// <summary>Default constructor that creates a new, empty CodedCauseOfFetalDeathMessage.</summary>
        public CodedCauseOfFetalDeathMessage() : base(MESSAGE_TYPE)
        {
        }

        /// <summary>Constructor that takes a BFDR.FetalDeathRecord and wraps it in a CodedCauseOfFetalDeathMessage.</summary>
        /// <param name="record">the BFDR.FetalDeathRecord to create a CodedCauseOfFetalDeathMessage for.</param>
        public CodedCauseOfFetalDeathMessage(FetalDeathRecord record) : this()
        {
            this.FetalDeathRecord = record;
            ExtractBusinessIdentifiers(record);
        }

        /// <summary>
        /// Construct a CodedCauseOfFetalDeathMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the CodedCauseOfFetalDeathMessage</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        internal CodedCauseOfFetalDeathMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle)
        {
            try
            {
                FetalDeathRecord = new FetalDeathRecord(findEntry<Bundle>());
            }
            catch (System.ArgumentException ex)
            {
                throw new MessageParseException($"Error processing FetalDeathRecord entry in the message: {ex.Message}", baseMessage);
            }
        }

        /// <summary>The FetalDeathRecord conveyed by this message</summary>
        /// <value>the FetalDeathRecord</value>
        public FetalDeathRecord FetalDeathRecord
        {
            get
            {
                return fetalDeathRecord;
            }
            set
            {
                fetalDeathRecord = value;
                UpdateMessageBundleRecord();
            }
        }

        /// <summary>The record bundle that should go into the message bundle for this message</summary>
        /// <value>the MessageBundleRecord</value>
        protected override Bundle MessageBundleRecord
        {
            get
            {
                return fetalDeathRecord?.GetBundle();
            }
        }
    }

    /// <summary>Class <c>CodedCauseOfFetalDeathUpdateMessage</c> supports the update of BFDR records.</summary>
    public class CodedCauseOfFetalDeathUpdateMessage : CodedCauseOfFetalDeathMessage
    {
        /// <summary>
        /// The event URI for CodedCauseOfFetalDeathUpdateMessage.
        /// </summary>
        public new const String MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_causeofdeath_update";

        /// <summary>Default constructor that creates a new, empty CodedCauseOfFetalDeathUpdateMessage.</summary>
        public CodedCauseOfFetalDeathUpdateMessage() : base()
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that takes a BFDR.FetalDeathRecord and wraps it in a CodedCauseOfFetalDeathUpdateMessage.</summary>
        /// <param name="record">the BFDR.FetalDeathRecord to create a CodedCauseOfFetalDeathUpdateMessage for.</param>
        public CodedCauseOfFetalDeathUpdateMessage(FetalDeathRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a CodedCauseOfFetalDeathUpdateMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the CodedCauseOfFetalDeathUpdateMessage</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        internal CodedCauseOfFetalDeathUpdateMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle, baseMessage)
        {
        }
    }
}

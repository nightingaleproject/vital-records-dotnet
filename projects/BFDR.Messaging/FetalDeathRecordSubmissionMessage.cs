using System;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>Class <c>FetalDeathRecordSubmission</c> supports the submission of BFDR records.</summary>
    public class FetalDeathRecordSubmissionMessage : FetalDeathRecordBaseMessage
    {
        /// <summary>
        /// The event URI for FetalDeathRecordSubmission.
        /// </summary>
        public const String MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_submission";

        /// <summary>Bundle that contains the message payload.</summary>
        private FetalDeathRecord fetalDeathRecord;

        /// <summary>Default constructor that creates a new, empty FetalDeathRecordSubmission.</summary>
        public FetalDeathRecordSubmissionMessage() : base(MESSAGE_TYPE)
        {
        }

        /// <summary>Constructor that takes a BFDR.FetalDeathRecord and wraps it in a FetalDeathRecordSubmission.</summary>
        /// <param name="record">the BFDR.FetalDeathRecord to create a FetalDeathRecordSubmission for.</param>
        public FetalDeathRecordSubmissionMessage(FetalDeathRecord record) : this()
        {
            this.FetalDeathRecord = record;
            ExtractBusinessIdentifiers(record);
        }

        /// <summary>
        /// Construct a FetalDeathRecordSubmission from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the FetalDeathRecordSubmission</param>
        /// <param name="baseMessage">the FetalDeathRecordBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        internal FetalDeathRecordSubmissionMessage(Bundle messageBundle, FetalDeathRecordBaseMessage baseMessage) : base(messageBundle)
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

    /// <summary>Class <c>FetalDeathRecordUpdateMessage</c> supports the update of BFDR records.</summary>
    public class FetalDeathRecordUpdateMessage : FetalDeathRecordSubmissionMessage
    {
        /// <summary>
        /// The event URI for FetalDeathRecordUpdateMessage.
        /// </summary>
        public new const String MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_submission_update";

        /// <summary>Default constructor that creates a new, empty FetalDeathRecordUpdateMessage.</summary>
        public FetalDeathRecordUpdateMessage() : base()
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that takes a BFDR.FetalDeathRecord and wraps it in a FetalDeathRecordUpdateMessage.</summary>
        /// <param name="record">the BFDR.FetalDeathRecord to create a FetalDeathRecordUpdateMessage for.</param>
        public FetalDeathRecordUpdateMessage(FetalDeathRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a FetalDeathRecordUpdateMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the FetalDeathRecordUpdateMessage</param>
        /// <param name="baseMessage">the FetalDeathRecordBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        internal FetalDeathRecordUpdateMessage(Bundle messageBundle, FetalDeathRecordBaseMessage baseMessage) : base(messageBundle, baseMessage)
        {
        }
    }
}

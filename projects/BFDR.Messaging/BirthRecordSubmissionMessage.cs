using System;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>Class <c>BirthRecordSubmission</c> supports the submission of BFDR records.</summary>
    public class BirthRecordSubmissionMessage : BFDRBaseMessage
    {
        /// <summary>
        /// The event URI for BirthRecordSubmission.
        /// </summary>
        public new const String MESSAGE_TYPE = "http://nchs.cdc.gov/birth_submission";

        /// <summary>Bundle that contains the message payload.</summary>
        private BirthRecord birthRecord;

        /// <summary>Default constructor that creates a new, empty BirthRecordSubmission.</summary>
        public BirthRecordSubmissionMessage() : base(MESSAGE_TYPE)
        {
        }

        /// <summary>Constructor that takes a BFDR.BirthRecord and wraps it in a BirthRecordSubmission.</summary>
        /// <param name="record">the BFDR.BirthRecord to create a BirthRecordSubmission for.</param>
        public BirthRecordSubmissionMessage(BirthRecord record) : this()
        {
            this.BirthRecord = record;
            ExtractBusinessIdentifiers(record);
        }

        /// <summary>
        /// Construct a BirthRecordSubmission from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BirthRecordSubmission</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        internal BirthRecordSubmissionMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle)
        {
            try
            {
                BirthRecord = new BirthRecord(findEntry<Bundle>());
            }
            catch (System.ArgumentException ex)
            {
                throw new MessageParseException($"Error processing BirthRecord entry in the message: {ex.Message}", baseMessage);
            }
        }

        /// <summary>The BirthRecord conveyed by this message</summary>
        /// <value>the BirthRecord</value>
        public BirthRecord BirthRecord
        {
            get
            {
                return birthRecord;
            }
            set
            {
                birthRecord = value;
                UpdateMessageBundleRecord();
            }
        }

        /// <summary>The record bundle that should go into the message bundle for this message</summary>
        /// <value>the MessageBundleRecord</value>
        protected override Bundle MessageBundleRecord
        {
            get
            {
                return birthRecord?.GetBundle();
            }
        }
    }

    /// <summary>Class <c>BirthRecordUpdateMessage</c> supports the update of BFDR records.</summary>
    public class BirthRecordUpdateMessage : BirthRecordSubmissionMessage
    {
        /// <summary>
        /// The event URI for BirthRecordUpdateMessage.
        /// </summary>
        public new const String MESSAGE_TYPE = "http://nchs.cdc.gov/birth_submission_update";

        /// <summary>Default constructor that creates a new, empty BirthRecordUpdateMessage.</summary>
        public BirthRecordUpdateMessage() : base()
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Constructor that takes a BFDR.BirthRecord and wraps it in a BirthRecordUpdateMessage.</summary>
        /// <param name="record">the BFDR.BirthRecord to create a BirthRecordUpdateMessage for.</param>
        public BirthRecordUpdateMessage(BirthRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a BirthRecordUpdateMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BirthRecordUpdateMessage</param>
        /// <param name="baseMessage">the BFDRBaseMessage instance that was constructed during parsing that can be used in a MessageParseException if needed</param>
        internal BirthRecordUpdateMessage(Bundle messageBundle, BFDRBaseMessage baseMessage) : base(messageBundle, baseMessage)
        {
        }
    }
}

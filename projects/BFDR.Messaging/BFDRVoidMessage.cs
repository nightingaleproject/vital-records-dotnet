using System;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>Class <c>BirthRecordVoidMessage</c> indicates that a previously submitted BirthRecordSubmissionMessage should be voided.</summary>
    public class BirthRecordVoidMessage : BFDRVoidMessage
    {
        /// <summary>
        /// The Event URI for BirthRecordVoidMessage
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/birth_submission_void";

        /// <summary>Default constructor that creates a new, empty BirthRecordVoidMessage.</summary>
        public BirthRecordVoidMessage() : base()
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>Default constructor that creates a new, empty BirthRecordVoidMessage.</summary>
        public BirthRecordVoidMessage(BirthRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a BirthRecordVoidMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BirthRecordVoidMessage</param>
        /// <returns></returns>
        internal BirthRecordVoidMessage(Bundle messageBundle) : base(messageBundle)
        {
            MessageType = MESSAGE_TYPE;
        }
    }
    /// <summary>Class <c>FetalDeathRecordVoidMessage</c> indicates that a previously submitted FetalDeathRecordSubmissionMessage should be voided.</summary>
    public class FetalDeathRecordVoidMessage : BFDRVoidMessage
    {
        /// <summary>Default constructor that creates a new, empty FetalDeathRecordVoidMessage.</summary>
        public FetalDeathRecordVoidMessage() : base()
        {
            MessageType = MESSAGE_TYPE;
        }
        /// <summary>
        /// The Event URI for FetalDeathRecordVoidMessage
        /// </summary>
        public new const string MESSAGE_TYPE = "http://nchs.cdc.gov/fd_submission_void";
        /// <summary>Default constructor that creates a new, empty FetalDeathRecordVoidMessage.</summary>
        public FetalDeathRecordVoidMessage(FetalDeathRecord record) : base(record)
        {
            MessageType = MESSAGE_TYPE;
        }

        /// <summary>
        /// Construct a FetalDeathRecordVoidMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the FetalDeathRecordVoidMessage</param>
        /// <returns></returns>
        internal FetalDeathRecordVoidMessage(Bundle messageBundle) : base(messageBundle)
        {
            MessageType = MESSAGE_TYPE;
        }
    }
    
    /// <summary>Class <c>BFDRVoidMessage</c> indicates that a previously submitted BirthRecordSubmissionMessage or FetalDeathRecordSubmissionMessageshould be voided.</summary>
    public abstract class BFDRVoidMessage : BFDRBaseMessage
    {

        /// <summary>Default constructor that creates a new, empty BFDRVoidMessage.</summary>
        public BFDRVoidMessage() : base(MESSAGE_TYPE)
        {
            BlockCount = 1;
        }

        /// <summary>
        /// Construct a BFDRVoidMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BFDRVoidMessage</param>
        /// <returns></returns>
        internal BFDRVoidMessage(Bundle messageBundle) : base(messageBundle)
        {
            if (BlockCount == null)
            {
                BlockCount = 1;
            }
        }

        /// <summary>Constructor that takes a BFDR.NatalityRecord and creates a message to void that record.</summary>
        /// <param name="record">the BFDR.NatalityRecord to create a BFDRVoidMessage for.</param>
        public BFDRVoidMessage(NatalityRecord record) : this()
        {
            BlockCount = 1;
            ExtractBusinessIdentifiers(record);
        }

        /// <summary>The number of records to void starting at the certificate number specified by the `CertNo` parameter</summary>
        public uint? BlockCount
        {
            get
            {
                return (uint?)Record?.GetSingleValue<UnsignedInt>("block_count")?.Value;
            }
            set
            {
                Record.Remove("block_count");
                if (value != null && value >= 0)
                {
                    Record.Add("block_count", new UnsignedInt((int)value));
                }
            }
        }
    }
}

using System;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>Class <c>BirthRecordVoidMessage</c> indicates that a previously submitted BirthRecordSubmissionMessage should be voided.</summary>
    public class BirthRecordVoidMessage : BirthRecordBaseMessage
    {
        /// <summary>
        /// The Event URI for BirthRecordVoidMessage
        /// </summary>
        public const string MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_submission_void";

        /// <summary>Default constructor that creates a new, empty BirthRecordVoidMessage.</summary>
        public BirthRecordVoidMessage() : base(MESSAGE_TYPE)
        {
        }

        /// <summary>
        /// Construct a BirthRecordVoidMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BirthRecordVoidMessage</param>
        /// <returns></returns>
        internal BirthRecordVoidMessage(Bundle messageBundle) : base(messageBundle)
        {
        }

        /// <summary>Constructor that takes a BFDR.BirthRecord and creates a message to void that record.</summary>
        /// <param name="record">the BFDR.BirthRecord to create a BirthRecordVoidMessage for.</param>
        public BirthRecordVoidMessage(BirthRecord record) : this()
        {
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

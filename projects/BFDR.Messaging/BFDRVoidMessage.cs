using System;
using Hl7.Fhir.Model;

namespace BFDR
{
    /// <summary>Class <c>BFDRVoidMessage</c> indicates that a previously submitted BirthRecordSubmissionMessage or FetalDeathRecordSubmissionMessageshould be voided.</summary>
    public class BFDRVoidMessage : BFDRBaseMessage
    {
        /// <summary>
        /// The Event URI for BFDRVoidMessage
        /// </summary>
        public const string MESSAGE_TYPE = "http://nchs.cdc.gov/bfdr_submission_void";

        /// <summary>Default constructor that creates a new, empty BFDRVoidMessage.</summary>
        public BFDRVoidMessage() : base(MESSAGE_TYPE)
        {
        }

        /// <summary>
        /// Construct a BFDRVoidMessage from a FHIR Bundle.
        /// </summary>
        /// <param name="messageBundle">a FHIR Bundle that will be used to initialize the BFDRVoidMessage</param>
        /// <returns></returns>
        internal BFDRVoidMessage(Bundle messageBundle) : base(messageBundle)
        {
        }

        /// <summary>Constructor that takes a BFDR.NatalityRecord and creates a message to void that record.</summary>
        /// <param name="record">the BFDR.NatalityRecord to create a BFDRVoidMessage for.</param>
        public BFDRVoidMessage(NatalityRecord record) : this()
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
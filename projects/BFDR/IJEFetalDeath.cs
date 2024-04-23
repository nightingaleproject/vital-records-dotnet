using System;
using System.Collections.Generic;
using System.Linq;
using VR;

namespace BFDR
{
    /// <summary>A "wrapper" class to convert between a FHIR based <c>FetalDeathRecord</c> and
    /// a record in IJE FetalDeath format. Each property of this class corresponds exactly
    /// with a field in the IJE FetalDeath format. The getters convert from the embedded
    /// FHIR based <c>FetalDeathRecord</c> to the IJE format for a specific field, and
    /// the setters convert from IJE format for a specific field and set that value
    /// on the embedded FHIR based <c>FetalDeathRecord</c>.</summary>
    public class IJEFetalDeath : IJE
    {
        private readonly FetalDeathRecord record;

        /// <summary>Constructor that takes a <c>FetalDeathRecord</c>.</summary>
        public IJEFetalDeath(FetalDeathRecord record, bool validate = true)
        {
            this.record = record;
            if (validate)
            {
                // We need to force a conversion to happen by calling ToString() if we want to validate
                ToString();
                CheckForValidationErrors();
            }
        }

        /// <summary>Constructor that takes an IJE string and builds a corresponding internal <c>FetalDeathRecord</c>.</summary>
        public IJEFetalDeath(string ije, bool validate = true) : this()
        {
            ProcessIJE(ije, validate);
        }

        /// <summary>Get the length of the IJE string.</summary>
        protected override uint IJELength
        {
            get
            {
                return 6000;
            }
        }

        /// <summary>Constructor that creates an empty record for constructing records using the IJE properties.</summary>
        public IJEFetalDeath()
        {
            this.record = new FetalDeathRecord();
        }

        /// <summary>FHIR based vital record.</summary>
        protected override VitalRecord Record
        {
            get
            {
                return this.record;
            }
        }

        /// <summary>FHIR based vital record.</summary>
        /// Redundant due to ToRecord(), but kept for compatibility with IJEMortality which has this method for backwards compatibility.
        public FetalDeathRecord ToFetalDeathRecord()
        {
            return this.record;
        }

        /// <summary>FHIR based vital record.</summary>
        /// Hides the IJE ToRecord method that returns a VitalRecord instead of a DeathRecord
        public new FetalDeathRecord ToRecord()
        {
            return this.record;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Class helper methods for getting and settings IJE fields.
        //
        /////////////////////////////////////////////////////////////////////////////////
        /// <summary>Converts the FHIR representation of presence-only fields to the IJE equivalent.</summary>
        /// <param name="fieldValue">the value of the field</param>
        /// <returns>Y (yes), N (no)</returns>
        private string YesNo_PresenceToIJE(bool fieldValue)
        {
            if (fieldValue)
            {
                return "Y";
            }
            else 
            {
                return "N";
            }
        }
        /// <summary>Converts the IJE representation of presence-only fields to the FHIR equivalent.</summary>
        /// <param name="value">Y (yes), N (no)</param>
        /// <param name="field">a function that will set a field in the FHIR record</param>
        private void YesNo_IJEToPresence(string value, Func<bool, bool> field)
        {
            if (value.Equals("Y"))
            {
                field(true);
            }
            else 
            {
                field(false);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Class Properties that provide getters and setters for each of the IJE
        // FetalDeath fields.
        //
        // Getters look at the embedded FetalRecord and convert values to IJE style.
        // Setters convert and store IJE style values to the embedded FetalDeathRecord.
        //
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>Date of Delivery (Fetus)--Year</summary>
        [IJEField(1, 1, 4, "Date of Delivery (Fetus)--Year", "FDOD_YR", 1)]
        public string FDOD_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Place of Delivery - code</summary>
        [IJEField(2, 5, 2, "State, U.S. Territory or Canadian Province of Place of Delivery - code", "DSTATE", 1)]
        public string DSTATE
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Certificate Number</summary>
        [IJEField(3, 7, 6, "Certificate Number", "FILENO", 1)]
        public string FILENO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Void flag</summary>
        [IJEField(4, 13, 1, "Void flag", "VOID", 1)]
        public string VOID
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Auxiliary State file number</summary>
        [IJEField(5, 14, 12, "Auxiliary State file number", "AUXNO", 1)]
        public string AUXNO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Time of Delivery</summary>
        [IJEField(6, 26, 4, "Time of Delivery", "TD", 1)]
        public string TD
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Sex</summary>
        [IJEField(7, 30, 1, "Sex", "FSEX", 1)]
        public string FSEX
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Delivery (Fetus)--Month</summary>
        [IJEField(8, 31, 2, "Date of Delivery (Fetus)--Month", "FDOD_MO", 1)]
        public string FDOD_MO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Delivery (Fetus)--Day</summary>
        [IJEField(9, 33, 2, "Date of Delivery (Fetus)--Day", "FDOD_DY", 1)]
        public string FDOD_DY
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>County of Delivery</summary>
        [IJEField(10, 35, 3, "County of Delivery", "CNTYO", 1)]
        public string CNTYO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place Where Delivery Occurred</summary>
        [IJEField(11, 38, 1, "Place Where Delivery Occurred", "DPLACE", 1)]
        public string DPLACE
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Facility ID (NPI) - If available</summary>
        [IJEField(12, 39, 12, "Facility ID (NPI) - If available", "FNPI", 1)]
        public string FNPI
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Facility ID (State-Assigned)</summary>
        [IJEField(13, 51, 4, "Facility ID (State-Assigned)", "SFN", 1)]
        public string SFN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Birth (Mother)--Year</summary>
        [IJEField(14, 55, 4, "Date of Birth (Mother)--Year", "MDOB_YR", 1)]
        public string MDOB_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Birth (Mother)--Month</summary>
        [IJEField(15, 59, 2, "Date of Birth (Mother)--Month", "MDOB_MO", 1)]
        public string MDOB_MO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Birth (Mother)--Day</summary>
        [IJEField(16, 61, 2, "Date of Birth (Mother)--Day", "MDOB_DY", 1)]
        public string MDOB_DY
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Birth (Mother)--Edit Flag</summary>
        [IJEField(17, 63, 1, "Date of Birth (Mother)--Edit Flag", "MAGE_BYPASS", 1)]
        public string MAGE_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Birth (Mother) - code</summary>
        [IJEField(18, 64, 2, "State, U.S. Territory or Canadian Province of Birth (Mother) - code", "BPLACEC_ST_TER", 1)]
        public string BPLACEC_ST_TER
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Birthplace--Country</summary>
        [IJEField(19, 66, 2, "Mother's Birthplace--Country", "BPLACEC_CNT", 1)]
        public string BPLACEC_CNT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Residence of Mother--City/Town</summary>
        [IJEField(20, 68, 5, "Residence of Mother--City/Town", "CITYC", 1)]
        public string CITYC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Residence of Mother--County</summary>
        [IJEField(21, 73, 3, "Residence of Mother--County", "COUNTYC", 1)]
        public string COUNTYC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Residence (Mother) - code</summary>
        [IJEField(22, 76, 2, "State, U.S. Territory or Canadian Province of Residence (Mother) - code", "STATEC", 1)]
        public string STATEC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Residence of Mother--Country</summary>
        [IJEField(23, 78, 2, "Residence of Mother--Country", "COUNTRYC", 1)]
        public string COUNTRYC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Residence of Mother--Inside City/Town Limits</summary>
        [IJEField(24, 80, 1, "Residence of Mother--Inside City/Town Limits", "LIMITS", 1)]
        public string LIMITS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Birth (Father)--Year</summary>
        [IJEField(25, 81, 4, "Date of Birth (Father)--Year", "FDOB_YR", 1)]
        public string FDOB_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Birth (Father)--Month</summary>
        [IJEField(26, 85, 2, "Date of Birth (Father)--Month", "FDOB_MO", 1)]
        public string FDOB_MO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Birth (Father)--Day</summary>
        [IJEField(27, 87, 2, "Date of Birth (Father)--Day", "FDOB_DY", 1)]
        public string FDOB_DY
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Birth (Father)--Edit Flag</summary>
        [IJEField(28, 89, 1, "Date of Birth (Father)--Edit Flag", "FAGE_BYPASS", 1)]
        public string FAGE_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother Married?--Ever(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(29, 90, 1, "Mother Married?--Ever(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "MARE", 1)]
        public string MARE
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother Married?-- At Conception, at Delivery or any Time in Between(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(30, 91, 1, "Mother Married?-- At Conception, at Delivery or any Time in Between(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "MARN", 1)]
        public string MARN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        // <summary>FILLER</summary>
        // [IJEField(31, 92, 1, "FILLER", "", 1)]
        // Note: Note implemented

        /// <summary>Mother's Education</summary>
        [IJEField(32, 93, 1, "Mother's Education", "MEDUC", 1)]
        public string MEDUC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Education--Edit Flag</summary>
        [IJEField(33, 94, 1, "Mother's Education--Edit Flag", "MEDUC_BYPASS", 1)]
        public string MEDUC_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother of Hispanic Origin?--Mexican</summary>
        [IJEField(34, 95, 1, "Mother of Hispanic Origin?--Mexican", "METHNIC1", 1)]
        public string METHNIC1
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother of Hispanic Origin?--Puerto Rican</summary>
        [IJEField(35, 96, 1, "Mother of Hispanic Origin?--Puerto Rican", "METHNIC2", 1)]
        public string METHNIC2
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother of Hispanic Origin?--Cuban</summary>
        [IJEField(36, 97, 1, "Mother of Hispanic Origin?--Cuban", "METHNIC3", 1)]
        public string METHNIC3
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother of Hispanic Origin?--Other</summary>
        [IJEField(37, 98, 1, "Mother of Hispanic Origin?--Other", "METHNIC4", 1)]
        public string METHNIC4
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother of Hispanic Origin?--Other Literal</summary>
        [IJEField(38, 99, 20, "Mother of Hispanic Origin?--Other Literal", "METHNIC5", 1)]
        public string METHNIC5
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--White</summary>
        [IJEField(39, 119, 1, "Mother's Race--White", "MRACE1", 1)]
        public string MRACE1
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Black or African American</summary>
        [IJEField(40, 120, 1, "Mother's Race--Black or African American", "MRACE2", 1)]
        public string MRACE2
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--American Indian or Alaska Native</summary>
        [IJEField(41, 121, 1, "Mother's Race--American Indian or Alaska Native", "MRACE3", 1)]
        public string MRACE3
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Asian Indian</summary>
        [IJEField(42, 122, 1, "Mother's Race--Asian Indian", "MRACE4", 1)]
        public string MRACE4
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Chinese</summary>
        [IJEField(43, 123, 1, "Mother's Race--Chinese", "MRACE5", 1)]
        public string MRACE5
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Filipino</summary>
        [IJEField(44, 124, 1, "Mother's Race--Filipino", "MRACE6", 1)]
        public string MRACE6
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Japanese</summary>
        [IJEField(45, 125, 1, "Mother's Race--Japanese", "MRACE7", 1)]
        public string MRACE7
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Korean</summary>
        [IJEField(46, 126, 1, "Mother's Race--Korean", "MRACE8", 1)]
        public string MRACE8
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Vietnamese</summary>
        [IJEField(47, 127, 1, "Mother's Race--Vietnamese", "MRACE9", 1)]
        public string MRACE9
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Other Asian</summary>
        [IJEField(48, 128, 1, "Mother's Race--Other Asian", "MRACE10", 1)]
        public string MRACE10
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Native Hawaiian</summary>
        [IJEField(49, 129, 1, "Mother's Race--Native Hawaiian", "MRACE11", 1)]
        public string MRACE11
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Guamanian or Chamorro</summary>
        [IJEField(50, 130, 1, "Mother's Race--Guamanian or Chamorro", "MRACE12", 1)]
        public string MRACE12
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Samoan</summary>
        [IJEField(51, 131, 1, "Mother's Race--Samoan", "MRACE13", 1)]
        public string MRACE13
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Other Pacific Islander</summary>
        [IJEField(52, 132, 1, "Mother's Race--Other Pacific Islander", "MRACE14", 1)]
        public string MRACE14
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Other</summary>
        [IJEField(53, 133, 1, "Mother's Race--Other", "MRACE15", 1)]
        public string MRACE15
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--First American Indian or Alaska Native Literal</summary>
        [IJEField(54, 134, 30, "Mother's Race--First American Indian or Alaska Native Literal", "MRACE16", 1)]
        public string MRACE16
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Second American Indian or Alaska Native Literal</summary>
        [IJEField(55, 164, 30, "Mother's Race--Second American Indian or Alaska Native Literal", "MRACE17", 1)]
        public string MRACE17
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--First Other Asian Literal</summary>
        [IJEField(56, 194, 30, "Mother's Race--First Other Asian Literal", "MRACE18", 1)]
        public string MRACE18
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Second Other Asian Literal</summary>
        [IJEField(57, 224, 30, "Mother's Race--Second Other Asian Literal", "MRACE19", 1)]
        public string MRACE19
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--First Other Pacific Islander Literal</summary>
        [IJEField(58, 254, 30, "Mother's Race--First Other Pacific Islander Literal", "MRACE20", 1)]
        public string MRACE20
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Second Other Pacific Islander Literal</summary>
        [IJEField(59, 284, 30, "Mother's Race--Second Other Pacific Islander Literal", "MRACE21", 1)]
        public string MRACE21
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--First Other Literal</summary>
        [IJEField(60, 314, 30, "Mother's Race--First Other Literal", "MRACE22", 1)]
        public string MRACE22
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race--Second Other Literal</summary>
        [IJEField(61, 344, 30, "Mother's Race--Second Other Literal", "MRACE23", 1)]
        public string MRACE23
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 1E</summary>
        [IJEField(62, 374, 3, "Mother's Race Tabulation Variable 1E", "MRACE1E", 1)]
        public string MRACE1E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 2E</summary>
        [IJEField(63, 377, 3, "Mother's Race Tabulation Variable 2E", "MRACE2E", 1)]
        public string MRACE2E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 3E</summary>
        [IJEField(64, 380, 3, "Mother's Race Tabulation Variable 3E", "MRACE3E", 1)]
        public string MRACE3E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 4E</summary>
        [IJEField(65, 383, 3, "Mother's Race Tabulation Variable 4E", "MRACE4E", 1)]
        public string MRACE4E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 5E</summary>
        [IJEField(66, 386, 3, "Mother's Race Tabulation Variable 5E", "MRACE5E", 1)]
        public string MRACE5E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 6E</summary>
        [IJEField(67, 389, 3, "Mother's Race Tabulation Variable 6E", "MRACE6E", 1)]
        public string MRACE6E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 7E</summary>
        [IJEField(68, 392, 3, "Mother's Race Tabulation Variable 7E", "MRACE7E", 1)]
        public string MRACE7E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 8E</summary>
        [IJEField(69, 395, 3, "Mother's Race Tabulation Variable 8E", "MRACE8E", 1)]
        public string MRACE8E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 16C</summary>
        [IJEField(70, 398, 3, "Mother's Race Tabulation Variable 16C", "MRACE16C", 1)]
        public string MRACE16C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 17C</summary>
        [IJEField(71, 401, 3, "Mother's Race Tabulation Variable 17C", "MRACE17C", 1)]
        public string MRACE17C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 18C</summary>
        [IJEField(72, 404, 3, "Mother's Race Tabulation Variable 18C", "MRACE18C", 1)]
        public string MRACE18C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 19C</summary>
        [IJEField(73, 407, 3, "Mother's Race Tabulation Variable 19C", "MRACE19C", 1)]
        public string MRACE19C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 20C</summary>
        [IJEField(74, 410, 3, "Mother's Race Tabulation Variable 20C", "MRACE20C", 1)]
        public string MRACE20C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 21C</summary>
        [IJEField(75, 413, 3, "Mother's Race Tabulation Variable 21C", "MRACE21C", 1)]
        public string MRACE21C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 22C</summary>
        [IJEField(76, 416, 3, "Mother's Race Tabulation Variable 22C", "MRACE22C", 1)]
        public string MRACE22C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race Tabulation Variable 23C</summary>
        [IJEField(77, 419, 3, "Mother's Race Tabulation Variable 23C", "MRACE23C", 1)]
        public string MRACE23C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Attendant</summary>
        [IJEField(78, 422, 1, "Attendant", "ATTEND", 1)]
        public string ATTEND
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother Transferred?(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(79, 423, 1, "Mother Transferred?(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "TRAN", 1)]
        public string TRAN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of First Prenatal Care Visit--Month</summary>
        [IJEField(80, 424, 2, "Date of First Prenatal Care Visit--Month", "DOFP_MO", 1)]
        public string DOFP_MO
        {
            get
            {
                return NumericAllowingUnknown_Get("DOFP_MO", "FirstPrenatalCareVisitMonth");
            }
            set
            {
                NumericAllowingUnknown_Set("DOFP_MO", "FirstPrenatalCareVisitMonth", value);
            }
        }

        /// <summary>Date of First Prenatal Care Visit--Day</summary>
        [IJEField(81, 426, 2, "Date of First Prenatal Care Visit--Day", "DOFP_DY", 1)]
        public string DOFP_DY
        {
            get
            {
                return NumericAllowingUnknown_Get("DOFP_DY", "FirstPrenatalCareVisitDay");
            }
            set
            {
                NumericAllowingUnknown_Set("DOFP_DY", "FirstPrenatalCareVisitDay", value);
            }
        }

        /// <summary>Date of First Prenatal Care Visit--Year</summary>
        [IJEField(82, 428, 4, "Date of First Prenatal Care Visit--Year", "DOFP_YR", 1)]
        public string DOFP_YR
        {
            get
            {
                return NumericAllowingUnknown_Get("DOFP_YR", "FirstPrenatalCareVisitYear");
            }
            set
            {
                NumericAllowingUnknown_Set("DOFP_YR", "FirstPrenatalCareVisitYear", value);
            }
        }

        /// <summary>Date of Last Prenatal Care Visit--Month(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(83, 432, 2, "Date of Last Prenatal Care Visit--Month(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "DOLP_MO", 1)]
        public string DOLP_MO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Last Prenatal Care Visit--Day(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(84, 434, 2, "Date of Last Prenatal Care Visit--Day(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "DOLP_DY", 1)]
        public string DOLP_DY
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Last Prenatal Care Visit--Year(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(85, 436, 4, "Date of Last Prenatal Care Visit--Year(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "DOLP_YR", 1)]
        public string DOLP_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Total Number of Prenatal Care Visits(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(86, 440, 2, "Total Number of Prenatal Care Visits(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "NPREV", 1)]
        public string NPREV
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Total Number of Prenatal Care Visits--Edit Flag(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(87, 442, 1, "Total Number of Prenatal Care Visits--Edit Flag(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "NPREV_BYPASS", 1)]
        public string NPREV_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Height--Feet</summary>
        [IJEField(88, 443, 1, "Mother's Height--Feet", "HFT", 1)]
        public string HFT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Height--Inches</summary>
        [IJEField(89, 444, 2, "Mother's Height--Inches", "HIN", 1)]
        public string HIN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Height--Edit Flag</summary>
        [IJEField(90, 446, 1, "Mother's Height--Edit Flag", "HGT_BYPASS", 1)]
        public string HGT_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Prepregnancy Weight</summary>
        [IJEField(91, 447, 3, "Mother's Prepregnancy Weight", "PWGT", 1)]
        public string PWGT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Prepregnancy Weight--Edit Flag</summary>
        [IJEField(92, 450, 1, "Mother's Prepregnancy Weight--Edit Flag", "PWGT_BYPASS", 1)]
        public string PWGT_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Weight at Delivery(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(93, 451, 3, "Mother's Weight at Delivery(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "DWGT", 1)]
        public string DWGT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Weight at Delivery--Edit Flag(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(94, 454, 1, "Mother's Weight at Delivery--Edit Flag(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "DWGT_BYPASS", 1)]
        public string DWGT_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Did Mother get WIC Food for Herself?</summary>
        [IJEField(95, 455, 1, "Did Mother get WIC Food for Herself?", "WIC", 1)]
        public string WIC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Previous Live Births Now Living</summary>
        [IJEField(96, 456, 2, "Previous Live Births Now Living", "PLBL", 1)]
        public string PLBL
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Previous Live Births Now Dead</summary>
        [IJEField(97, 458, 2, "Previous Live Births Now Dead", "PLBD", 1)]
        public string PLBD
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Previous Other Pregnancy Outcomes(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(98, 460, 2, "Previous Other Pregnancy Outcomes(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "POPO", 1)]
        public string POPO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Last Live Birth--Month</summary>
        [IJEField(99, 462, 2, "Date of Last Live Birth--Month", "MLLB", 1)]
        public string MLLB
        {
            get
            {
                return NumericAllowingUnknown_Get("MLLB", "DateOfLastLiveBirthMonth");
            }
            set
            {
                NumericAllowingUnknown_Set("MLLB", "DateOfLastLiveBirthMonth", value);
            }
        }

        /// <summary>Date of Last Live Birth--Year</summary>
        [IJEField(100, 464, 4, "Date of Last Live Birth--Year", "YLLB", 1)]
        public string YLLB
        {
            get
            {
                return NumericAllowingUnknown_Get("YLLB", "DateOfLastLiveBirthYear");
            }
            set
            {
                NumericAllowingUnknown_Set("YLLB", "DateOfLastLiveBirthYear", value);
            }
        }

        /// <summary>Date of Last Other Pregnancy Outcome--Month(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(101, 468, 2, "Date of Last Other Pregnancy Outcome--Month(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "MOPO", 1)]
        public string MOPO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date of Last Other Pregnancy Outcome--Year(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(102, 470, 4, "Date of Last Other Pregnancy Outcome--Year(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "YOPO", 1)]
        public string YOPO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Number of Cigarettes Smoked in 3 months prior to Pregnancy</summary>
        [IJEField(103, 474, 2, "Number of Cigarettes Smoked in 3 months prior to Pregnancy", "CIGPN", 1)]
        public string CIGPN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Number of Cigarettes Smoked in 1st 3 months</summary>
        [IJEField(104, 476, 2, "Number of Cigarettes Smoked in 1st 3 months", "CIGFN", 1)]
        public string CIGFN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Number of Cigarettes Smoked in 2nd 3 months</summary>
        [IJEField(105, 478, 2, "Number of Cigarettes Smoked in 2nd 3 months", "CIGSN", 1)]
        public string CIGSN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Number of Cigarettes Smoked in third trimester</summary>
        [IJEField(106, 480, 2, "Number of Cigarettes Smoked in third trimester", "CIGLN", 1)]
        public string CIGLN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date Last Normal Menses Began--Year</summary>
        [IJEField(107, 482, 4, "Date Last Normal Menses Began--Year", "DLMP_YR", 1)]
        public string DLMP_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date Last Normal Menses Began--Month</summary>
        [IJEField(108, 486, 2, "Date Last Normal Menses Began--Month", "DLMP_MO", 1)]
        public string DLMP_MO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date Last Normal Menses Began--Day</summary>
        [IJEField(109, 488, 2, "Date Last Normal Menses Began--Day", "DLMP_DY", 1)]
        public string DLMP_DY
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Risk Factors--Prepregnancy Diabetes  (NOTE: SEE INSERTED NOTES FOR RISK FACTOR LOCATIONS 490-501 AND 573-575 TO REFLECT 2004 CHANGES)</summary>
        [IJEField(110, 490, 1, "Risk Factors--Prepregnancy Diabetes  (NOTE: SEE INSERTED NOTES FOR RISK FACTOR LOCATIONS 490-501 AND 573-575 TO REFLECT 2004 CHANGES)", "PDIAB", 1)]
        public string PDIAB
        {
            get => PresenceToIJE(record.PrepregnancyDiabetes, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.PrepregnancyDiabetes = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Gestational Diabetes</summary>
        [IJEField(111, 491, 1, "Risk Factors--Gestational Diabetes", "GDIAB", 1)]
        public string GDIAB
        {
            get => PresenceToIJE(record.GestationalDiabetes, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.GestationalDiabetes = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Hypertension Prepregnancy</summary>
        [IJEField(112, 492, 1, "Risk Factors--Hypertension Prepregnancy", "PHYPE", 1)]
        public string PHYPE
        {
            get => PresenceToIJE(record.PrepregnancyHypertension, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.PrepregnancyHypertension = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary><html>Risk Factors--Hypertension Gestational <b> (SEE ADDITIONAL HYPERTENSION CATEGORY IN LOCATION 573)</b></html></summary>
        [IJEField(113, 493, 1, "<html>Risk Factors--Hypertension Gestational <b> (SEE ADDITIONAL HYPERTENSION CATEGORY IN LOCATION 573)</b></html>", "GHYPE", 1)]
        public string GHYPE
        {
            get => PresenceToIJE(record.GestationalHypertension, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.GestationalHypertension = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Previous Preterm Births(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(114, 494, 1, "Risk Factors--Previous Preterm Births(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "PPB", 1)]
        public string PPB
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Risk Factors--Poor Pregnancy Outcomes(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(115, 495, 1, "Risk Factors--Poor Pregnancy Outcomes(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "PPO", 1)]
        public string PPO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary><html>Risk Factors--Vaginal Bleeding <b><i> (NCHS DELETED THIS ITEM EFFECTIVE 2011)</i></b></html></summary>
        [IJEField(116, 496, 1, "<html>Risk Factors--Vaginal Bleeding <b><i> (NCHS DELETED THIS ITEM EFFECTIVE 2011)</i></b></html>", "VB", 1)]
        public string VB
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary><html>Risk Factors--Infertility Treatment  <b>(SEE ADDITIONAL SUBCATEGORIES IN LOCATIONS 574-575)</b></html></summary>
        [IJEField(117, 497, 1, "<html>Risk Factors--Infertility Treatment  <b>(SEE ADDITIONAL SUBCATEGORIES IN LOCATIONS 574-575)</b></html>", "INFT", 1)]
        public string INFT
        {
            get => PresenceToIJE(record.InfertilityTreatment, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.InfertilityTreatment = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Previous Cesarean</summary>
        [IJEField(118, 498, 1, "Risk Factors--Previous Cesarean", "PCES", 1)]
        public string PCES
        {
            get => PresenceToIJE(record.PreviousCesarean, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.PreviousCesarean = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Number Previous Cesareans</summary>
        [IJEField(119, 499, 2, "Risk Factors--Number Previous Cesareans", "NPCES", 1)]
        public string NPCES
        {
            get
            {
                return NumericAllowingUnknown_Get("NPCES", "NumberOfPreviousCesareans");
            }
            set
            {
                NumericAllowingUnknown_Set("NPCES", "NumberOfPreviousCesareans", value);
            }
        }

        /// <summary>Risk Factors--Number Previous Cesareans--Edit Flag</summary>
        [IJEField(120, 501, 1, "Risk Factors--Number Previous Cesareans--Edit Flag", "NPCES_BYPASS", 1)]
        public string NPCES_BYPASS
        {
            get
            {
                return Get_MappingFHIRToIJE(BFDR.Mappings.NumberPreviousCesareansEditFlags.FHIRToIJE, "NumberOfPreviousCesareansEditFlag", "NPCES_BYPASS");
            }
            set
            {
                Set_MappingIJEToFHIR(BFDR.Mappings.NumberPreviousCesareansEditFlags.IJEToFHIR, "NPCES_BYPASS", "NumberOfPreviousCesareansEditFlag", value);
            }
        }

        /// <summary>Infections Present--Gonorrhea(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(121, 502, 1, "Infections Present--Gonorrhea(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "GON", 1)]
        public string GON
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Infections Present--Syphilis(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(122, 503, 1, "Infections Present--Syphilis(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "SYPH", 1)]
        public string SYPH
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary><html>Infections Present--Herpes Simplex (HSV) <b><i>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</i></b></html></summary>
        [IJEField(123, 504, 1, "<html>Infections Present--Herpes Simplex (HSV) <b><i>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</i></b></html>", "HSV", 1)]
        public string HSV
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Infections Present--Chlamydia(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(124, 505, 1, "Infections Present--Chlamydia(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "CHAM", 1)]
        public string CHAM
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Infections Present--Listeria(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(125, 506, 1, "Infections Present--Listeria(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "LM", 1)]
        public string LM
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Infections Present--Group B streptococcus(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(126, 507, 1, "Infections Present--Group B streptococcus(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "GBS", 1)]
        public string GBS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Infections Present--Cytomeglovirus(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(127, 508, 1, "Infections Present--Cytomeglovirus(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "CMV", 1)]
        public string CMV
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Infections Present--Parvo virus(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(128, 509, 1, "Infections Present--Parvo virus(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "B19", 1)]
        public string B19
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Infections Present--Toxoplasmosis(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(129, 510, 1, "Infections Present--Toxoplasmosis(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "TOXO", 1)]
        public string TOXO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Infections Present--Other(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(130, 511, 1, "Infections Present--Other(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "OTHERI", 1)]
        public string OTHERI
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary><html>Method of Delivery--Attempted Forceps<b><i> (NCHS DELETED THIS ITEM EFFECTIVE 2011)</i></b></html></summary>
        [IJEField(131, 512, 1, "<html>Method of Delivery--Attempted Forceps<b><i> (NCHS DELETED THIS ITEM EFFECTIVE 2011)</i></b></html>", "ATTF", 1)]
        public string ATTF
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary><html>Method of Delivery--Attempted Vacuum <b><i>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</i></b></html></summary>
        [IJEField(132, 513, 1, "<html>Method of Delivery--Attempted Vacuum <b><i>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</i></b></html>", "ATTV", 1)]
        public string ATTV
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Method of Delivery--Fetal Presentation</summary>
        [IJEField(133, 514, 1, "Method of Delivery--Fetal Presentation", "PRES", 1)]
        public string PRES
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Method of Delivery--Route and Method of Delivery</summary>
        [IJEField(134, 515, 1, "Method of Delivery--Route and Method of Delivery", "ROUT", 1)]
        public string ROUT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Method of Delivery--Trial of Labor Attempted</summary>
        [IJEField(135, 516, 1, "Method of Delivery--Trial of Labor Attempted", "TLAB", 1)]
        public string TLAB
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Method of Delivery--Hysterotomy/Hysterectomy(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(136, 517, 1, "Method of Delivery--Hysterotomy/Hysterectomy(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "HYST", 1)]
        public string HYST
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Maternal Morbidity--Maternal Transfusion(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(137, 518, 1, "Maternal Morbidity--Maternal Transfusion(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "MTR", 1)]
        public string MTR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Maternal Morbidity--Perineal Laceration(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(138, 519, 1, "Maternal Morbidity--Perineal Laceration(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "PLAC", 1)]
        public string PLAC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Maternal Morbidity--Ruptured Uterus</summary>
        [IJEField(139, 520, 1, "Maternal Morbidity--Ruptured Uterus", "RUT", 1)]
        public string RUT
        {
            get => PresenceToIJE(record.RupturedUterus, record.NoMaternalMorbidities);
            set => IJEToPresence(value, (v) => record.RupturedUterus = v, (v) => record.NoMaternalMorbidities = v);
        }

        /// <summary>Maternal Morbidity--Unplanned Hysterectomy(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(140, 521, 1, "Maternal Morbidity--Unplanned Hysterectomy(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "UHYS", 1)]
        public string UHYS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Maternal Morbidity--Admit to Intensive Care</summary>
        [IJEField(141, 522, 1, "Maternal Morbidity--Admit to Intensive Care", "AINT", 1)]
        public string AINT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Maternal Morbidity--Unplanned Operation(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(142, 523, 1, "Maternal Morbidity--Unplanned Operation(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "UOPR", 1)]
        public string UOPR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Weight of Fetus</summary>
        [IJEField(143, 524, 4, "Weight of Fetus", "FWG", 1)]
        public string FWG
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Weight of Fetus--Edit Flag</summary>
        [IJEField(144, 528, 1, "Weight of Fetus--Edit Flag", "FW_BYPASS", 1)]
        public string FW_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Obstetric Estimation of Gestation</summary>
        [IJEField(145, 529, 2, "Obstetric Estimation of Gestation", "OWGEST", 1)]
        public string OWGEST
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Obstetric Estimation of Gestation--Edit Flag</summary>
        [IJEField(146, 531, 1, "Obstetric Estimation of Gestation--Edit Flag", "OWGEST_BYPASS", 1)]
        public string OWGEST_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Estimated time of fetal death</summary>
        [IJEField(147, 532, 1, "Estimated time of fetal death", "ETIME", 1)]
        public string ETIME
        {
            get
            {
                return Get_MappingFHIRToIJE(BFDR.Mappings.FetalDeathTimePoints.FHIRToIJE, "TimeOfFetalDeath", "ETIME");
            }
            set
            {
                Set_MappingIJEToFHIR(BFDR.Mappings.FetalDeathTimePoints.IJEToFHIR, "ETIME", "TimeOfFetalDeath", value);
            }
        }

        /// <summary>Was an Autopsy Performed?</summary>
        [IJEField(148, 533, 1, "Was an Autopsy Performed?", "AUTOP", 1)]
        public string AUTOP
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Was a Histological Placental Examination Performed?</summary>
        [IJEField(149, 534, 1, "Was a Histological Placental Examination Performed?", "HISTOP", 1)]
        public string HISTOP
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Were Autopsy or Histological Placental Examination Results Used in Determining the Cause of Fetal Death?</summary>
        [IJEField(150, 535, 1, "Were Autopsy or Histological Placental Examination Results Used in Determining the Cause of Fetal Death?", "AUTOPF", 1)]
        public string AUTOPF
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Plurality</summary>
        [IJEField(151, 536, 2, "Plurality", "PLUR", 1)]
        public string PLUR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Set Order</summary>
        [IJEField(152, 538, 2, "Set Order", "SORD", 1)]
        public string SORD
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Number of Fetal Deaths</summary>
        [IJEField(153, 540, 2, "Number of Fetal Deaths", "FDTH", 1)]
        public string FDTH
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Matching Number</summary>
        [IJEField(154, 542, 6, "Matching Number", "MATCH", 1)]
        public string MATCH
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Plurality--Edit Flag</summary>
        [IJEField(155, 548, 1, "Plurality--Edit Flag", "PLUR_BYPASS", 1)]
        public string PLUR_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Anencephaly(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(156, 549, 1, "Congenital Anomalies of the Fetus--Anencephaly(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "ANEN", 1)]
        public string ANEN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Meningomyelocele/Spina Bifida(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(157, 550, 1, "Congenital Anomalies of the Fetus--Meningomyelocele/Spina Bifida(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "MNSB", 1)]
        public string MNSB
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Cyanotic congenital heart disease(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(158, 551, 1, "Congenital Anomalies of the Fetus--Cyanotic congenital heart disease(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "CCHD", 1)]
        public string CCHD
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Congenital diaphragmatic hernia(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(159, 552, 1, "Congenital Anomalies of the Fetus--Congenital diaphragmatic hernia(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "CDH", 1)]
        public string CDH
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Omphalocele(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(160, 553, 1, "Congenital Anomalies of the Fetus--Omphalocele(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "OMPH", 1)]
        public string OMPH
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Gastroschisis(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(161, 554, 1, "Congenital Anomalies of the Fetus--Gastroschisis(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "GAST", 1)]
        public string GAST
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Limb Reduction Defect(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(162, 555, 1, "Congenital Anomalies of the Fetus--Limb Reduction Defect(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "LIMB", 1)]
        public string LIMB
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Cleft Lip with or without Cleft Palate(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(163, 556, 1, "Congenital Anomalies of the Fetus--Cleft Lip with or without Cleft Palate(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "CL", 1)]
        public string CL
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Cleft Palate Alone(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(164, 557, 1, "Congenital Anomalies of the Fetus--Cleft Palate Alone(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "CP", 1)]
        public string CP
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Down Syndrome(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(165, 558, 1, "Congenital Anomalies of the Fetus--Down Syndrome(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "DOWT", 1)]
        public string DOWT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Suspected Chromosomal disorder(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(166, 559, 1, "Congenital Anomalies of the Fetus--Suspected Chromosomal disorder(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "CDIT", 1)]
        public string CDIT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Congenital Anomalies of the Fetus--Hypospadias(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(167, 560, 1, "Congenital Anomalies of the Fetus--Hypospadias(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "HYPO", 1)]
        public string HYPO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>NCHS USE ONLY: Receipt date -- Year</summary>
        [IJEField(168, 561, 4, "NCHS USE ONLY: Receipt date -- Year", "R_YR", 1)]
        public string R_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>NCHS USE ONLY: Receipt date -- Month</summary>
        [IJEField(169, 565, 2, "NCHS USE ONLY: Receipt date -- Month", "R_MO", 1)]
        public string R_MO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>NCHS USE ONLY: Receipt date -- Day</summary>
        [IJEField(170, 567, 2, "NCHS USE ONLY: Receipt date -- Day", "R_DY", 1)]
        public string R_DY
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Reported Age</summary>
        [IJEField(171, 569, 2, "Mother's Reported Age", "MAGER", 1)]
        public string MAGER
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Reported Age</summary>
        [IJEField(172, 571, 2, "Father's Reported Age", "FAGER", 1)]
        public string FAGER
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Risk Factors--Hypertension Eclampsia (added after 2004)</summary>
        [IJEField(173, 573, 1, "Risk Factors--Hypertension Eclampsia (added after 2004)", "EHYPE", 1)]
        public string EHYPE
        {
            get => PresenceToIJE(record.EclampsiaHypertension, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.EclampsiaHypertension = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Infertility: Fertility Enhancing Drugs (added after 2004)</summary>
        [IJEField(174, 574, 1, "Risk Factors--Infertility: Fertility Enhancing Drugs (added after 2004)", "INFT_DRG", 1)]
        public string INFT_DRG
        {
            get => PresenceToIJE(record.ArtificialInsemination, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.ArtificialInsemination = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Infertility: Asst. Rep. Technology (added after 2004)</summary>
        [IJEField(175, 575, 1, "Risk Factors--Infertility: Asst. Rep. Technology (added after 2004)", "INFT_ART", 1)]
        public string INFT_ART
        {
            get => PresenceToIJE(record.AssistedFertilization, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.AssistedFertilization = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Date of Registration--Year</summary>
        [IJEField(176, 576, 4, "Date of Registration--Year", "DOR_YR", 1)]
        public string DOR_YR
        {
            get
            {
                return NumericAllowingUnknown_Get("DOR_YR", "RegistrationDateYear");
            }
            set
            {
                NumericAllowingUnknown_Set("DOR_YR", "RegistrationDateYear", value);
            }
        }

        /// <summary>Date of Registration--Month</summary>
        [IJEField(177, 580, 2, "Date of Registration--Month", "DOR_MO", 1)]
        public string DOR_MO
        {
            get
            {
                return NumericAllowingUnknown_Get("DOR_MO", "RegistrationDateMonth");
            }
            set
            {
                NumericAllowingUnknown_Set("DOR_MO", "RegistrationDateMonth", value);
            }
        }

        /// <summary>Date of Registration--Day</summary>
        [IJEField(178, 582, 2, "Date of Registration--Day", "DOR_DY", 1)]
        public string DOR_DY
        {
            get
            {
                return NumericAllowingUnknown_Get("DOR_DY", "RegistrationDateDay");
            }
            set
            {
                NumericAllowingUnknown_Set("DOR_DY", "RegistrationDateDay", value);
            }
        }

        // <summary>FILLER</summary>
        // [IJEField(179, 584, 3, "FILLER", "", 1)]
        // Note: Note implemented

        /// <summary>Initiating cause/condition - Rupture of membranes prior to onset of labor</summary>
        [IJEField(180, 587, 1, "Initiating cause/condition - Rupture of membranes prior to onset of labor", "COD18a1", 1)]
        public string COD18a1
        {
            get => YesNo_PresenceToIJE(record.PrematureRuptureOfMembranes);
            set => YesNo_IJEToPresence(value, (v) => record.PrematureRuptureOfMembranes = v);
        }

        /// <summary>Initiating cause/condition - Abruptio placenta</summary>
        [IJEField(181, 588, 1, "Initiating cause/condition - Abruptio placenta", "COD18a2", 1)]
        public string COD18a2
        {
            get => YesNo_PresenceToIJE(record.AbruptioPlacenta);
            set => YesNo_IJEToPresence(value, (v) => record.AbruptioPlacenta = v);
        }

        /// <summary>Initiating cause/condition - Placental insufficiency</summary>
        [IJEField(182, 589, 1, "Initiating cause/condition - Placental insufficiency", "COD18a3", 1)]
        public string COD18a3
        {
            get => YesNo_PresenceToIJE(record.PlacentalInsufficiency);
            set => YesNo_IJEToPresence(value, (v) => record.PlacentalInsufficiency = v);
        }

        /// <summary>Initiating cause/condition - Prolapsed cord</summary>
        [IJEField(183, 590, 1, "Initiating cause/condition - Prolapsed cord", "COD18a4", 1)]
        public string COD18a4
        {
            get => YesNo_PresenceToIJE(record.ProlapsedCord);
            set => YesNo_IJEToPresence(value, (v) => record.ProlapsedCord = v);
        }

        /// <summary>Initiating cause/condition - Chorioamnionitis</summary>
        [IJEField(184, 591, 1, "Initiating cause/condition - Chorioamnionitis", "COD18a5", 1)]
        public string COD18a5
        {
            get => YesNo_PresenceToIJE(record.ChorioamnionitisCOD);
            set => YesNo_IJEToPresence(value, (v) => record.ChorioamnionitisCOD = v);
        }

        /// <summary>Initiating cause/condition - Other complications of placenta, cord, or membranes</summary>
        [IJEField(185, 592, 1, "Initiating cause/condition - Other complications of placenta, cord, or membranes", "COD18a6", 1)]
        public string COD18a6
        {
            get => YesNo_PresenceToIJE(record.OtherComplicationsOfPlacentaCordOrMembranes);
            set => YesNo_IJEToPresence(value, (v) => record.OtherComplicationsOfPlacentaCordOrMembranes = v);
        }

        /// <summary>Initiating cause/condition - Unknown</summary>
        [IJEField(186, 593, 1, "Initiating cause/condition - Unknown", "COD18a7", 1)]
        public string COD18a7
        {
            get => YesNo_PresenceToIJE(record.InitiatingCauseOrConditionUnknown);
            set => YesNo_IJEToPresence(value, (v) => record.InitiatingCauseOrConditionUnknown = v);
        }

        /// <summary>Initiating cause/condition - Maternal conditions/diseases literal</summary>
        [IJEField(187, 594, 60, "Initiating cause/condition - Maternal conditions/diseases literal", "COD18a8", 1)]
        public string COD18a8
        {
            get => LeftJustified_Get("COD18a8", "MaternalConditionsDiseasesLiteral");
            set => LeftJustified_Set("COD18a8", "MaternalConditionsDiseasesLiteral", value);
        }

        /// <summary>Initiating cause/condition - Other complications of placenta, cord, or membranes literal</summary>
        [IJEField(188, 654, 60, "Initiating cause/condition - Other complications of placenta, cord, or membranes literal", "COD18a9", 1)]
        public string COD18a9
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Initiating cause/condition - Other obstetrical or pregnancy complications literal</summary>
        [IJEField(189, 714, 60, "Initiating cause/condition - Other obstetrical or pregnancy complications literal", "COD18a10", 1)]
        public string COD18a10
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Initiating cause/condition - Fetal anomaly literal</summary>
        [IJEField(190, 774, 60, "Initiating cause/condition - Fetal anomaly literal", "COD18a11", 1)]
        public string COD18a11
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Initiating cause/condition - Fetal injury literal</summary>
        [IJEField(191, 834, 60, "Initiating cause/condition - Fetal injury literal", "COD18a12", 1)]
        public string COD18a12
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Initiating cause/condition - Fetal infection literal</summary>
        [IJEField(192, 894, 60, "Initiating cause/condition - Fetal infection literal", "COD18a13", 1)]
        public string COD18a13
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Initiating cause/condition - Other fetal conditions/disorders literal</summary>
        [IJEField(193, 954, 60, "Initiating cause/condition - Other fetal conditions/disorders literal", "COD18a14", 1)]
        public string COD18a14
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Rupture of membranes prior to onset of labor</summary>
        [IJEField(194, 1014, 1, "Other significant causes or conditions - Rupture of membranes prior to onset of labor", "COD18b1", 1)]
        public string COD18b1
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Abruptio placenta</summary>
        [IJEField(195, 1015, 1, "Other significant causes or conditions - Abruptio placenta", "COD18b2", 1)]
        public string COD18b2
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions  - Placental insufficiency</summary>
        [IJEField(196, 1016, 1, "Other significant causes or conditions  - Placental insufficiency", "COD18b3", 1)]
        public string COD18b3
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Prolapsed cord</summary>
        [IJEField(197, 1017, 1, "Other significant causes or conditions - Prolapsed cord", "COD18b4", 1)]
        public string COD18b4
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Chorioamnionitis</summary>
        [IJEField(198, 1018, 1, "Other significant causes or conditions - Chorioamnionitis", "COD18b5", 1)]
        public string COD18b5
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Other complications of placenta, cord, or membranes</summary>
        [IJEField(199, 1019, 1, "Other significant causes or conditions - Other complications of placenta, cord, or membranes", "COD18b6", 1)]
        public string COD18b6
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Unknown</summary>
        [IJEField(200, 1020, 1, "Other significant causes or conditions - Unknown", "COD18b7", 1)]
        public string COD18b7
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Maternal conditions/diseases literal</summary>
        [IJEField(201, 1021, 240, "Other significant causes or conditions - Maternal conditions/diseases literal", "COD18b8", 1)]
        public string COD18b8
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Other complications of placenta, cord, or membranes literal</summary>
        [IJEField(202, 1261, 240, "Other significant causes or conditions - Other complications of placenta, cord, or membranes literal", "COD18b9", 1)]
        public string COD18b9
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Other obstetrical or pregnancy complications literal</summary>
        [IJEField(203, 1501, 240, "Other significant causes or conditions - Other obstetrical or pregnancy complications literal", "COD18b10", 1)]
        public string COD18b10
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Fetal anomaly literal</summary>
        [IJEField(204, 1741, 240, "Other significant causes or conditions - Fetal anomaly literal", "COD18b11", 1)]
        public string COD18b11
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Fetal injury literal</summary>
        [IJEField(205, 1981, 240, "Other significant causes or conditions - Fetal injury literal", "COD18b12", 1)]
        public string COD18b12
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Fetal infection literal</summary>
        [IJEField(206, 2221, 240, "Other significant causes or conditions - Fetal infection literal", "COD18b13", 1)]
        public string COD18b13
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Other significant causes or conditions - Other fetal conditions/disorders literal</summary>
        [IJEField(207, 2461, 240, "Other significant causes or conditions - Other fetal conditions/disorders literal", "COD18b14", 1)]
        public string COD18b14
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Coded initiating cause/condition</summary>
        [IJEField(208, 2701, 5, "Coded initiating cause/condition", "ICOD", 1)]
        public string ICOD
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Coded other significant causes or conditions- first mentioned</summary>
        [IJEField(209, 2706, 5, "Coded other significant causes or conditions- first mentioned", "OCOD1", 1)]
        public string OCOD1
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Coded other significant causes or conditions- second mentioned</summary>
        [IJEField(210, 2711, 5, "Coded other significant causes or conditions- second mentioned", "OCOD2", 1)]
        public string OCOD2
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Coded other significant causes or conditions- third mentioned</summary>
        [IJEField(211, 2716, 5, "Coded other significant causes or conditions- third mentioned", "OCOD3", 1)]
        public string OCOD3
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Coded other significant causes or conditions- fourth mentioned</summary>
        [IJEField(212, 2721, 5, "Coded other significant causes or conditions- fourth mentioned", "OCOD4", 1)]
        public string OCOD4
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Coded other significant causes or conditions- fifth mentioned</summary>
        [IJEField(213, 2726, 5, "Coded other significant causes or conditions- fifth mentioned", "OCOD5", 1)]
        public string OCOD5
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Coded other significant causes or conditions- sixth mentioned</summary>
        [IJEField(214, 2731, 5, "Coded other significant causes or conditions- sixth mentioned", "OCOD6", 1)]
        public string OCOD6
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Coded other significant causes or conditions- seventh mentioned</summary>
        [IJEField(215, 2736, 5, "Coded other significant causes or conditions- seventh mentioned", "OCOD7", 1)]
        public string OCOD7
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary><html>Infections Present--Genital Herpes <b><i>(Subcategory in position 504)</i></b></html></summary>
        [IJEField(216, 2741, 1, "<html>Infections Present--Genital Herpes <b><i>(Subcategory in position 504)</i></b></html>", "HSV1", 1)]
        public string HSV1
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Infections Present--HIV</summary>
        [IJEField(217, 2742, 1, "Infections Present--HIV", "HIV", 1)]
        public string HIV
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Alcohol Used?</summary>
        [IJEField(218, 2743, 1, "Alcohol Used?", "ALCOHOL", 1)]
        public string ALCOHOL
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Fetus First Name</summary>
        [IJEField(219, 2744, 50, "Fetus First Name", "FETFNAME", 1)]
        public string FETFNAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Fetus Middle Name</summary>
        [IJEField(220, 2794, 50, "Fetus Middle Name", "FETMNAME", 1)]
        public string FETMNAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Fetus Last Name</summary>
        [IJEField(221, 2844, 50, "Fetus Last Name", "FETLNAME", 1)]
        public string FETLNAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Fetus Surname Suffix</summary>
        [IJEField(222, 2894, 10, "Fetus Surname Suffix", "SUFFIX", 1)]
        public string SUFFIX
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Fetus Legal Name--Alias</summary>
        [IJEField(223, 2904, 1, "Fetus Legal Name--Alias", "ALIAS", 1)]
        public string ALIAS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Name of Delivery Facility</summary>
        [IJEField(224, 2905, 50, "Name of Delivery Facility", "HOSP_D", 1)]
        public string HOSP_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Street number</summary>
        [IJEField(225, 2955, 10, "Place of Delivery Street number", "STNUM_D", 1)]
        public string STNUM_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Pre Directional</summary>
        [IJEField(226, 2965, 10, "Place of Delivery Pre Directional", "PREDIR_D", 1)]
        public string PREDIR_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Street name</summary>
        [IJEField(227, 2975, 50, "Place of Delivery Street name", "STNAME_D", 1)]
        public string STNAME_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Street designator</summary>
        [IJEField(228, 3025, 10, "Place of Delivery Street designator", "STDESIG_D", 1)]
        public string STDESIG_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Post Directional</summary>
        [IJEField(229, 3035, 10, "Place of Delivery Post Directional", "POSTDIR_D", 1)]
        public string POSTDIR_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Unit or Apartment Number</summary>
        [IJEField(230, 3045, 7, "Place of Delivery Unit or Apartment Number", "APTNUMB_D", 1)]
        public string APTNUMB_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Street Address</summary>
        [IJEField(231, 3052, 50, "Place of Delivery Street Address", "ADDRESS_D", 1)]
        public string ADDRESS_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Zip code and Zip+4</summary>
        [IJEField(232, 3102, 9, "Place of Delivery Zip code and Zip+4", "ZIPCODE_D", 1)]
        public string ZIPCODE_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery County (literal)</summary>
        [IJEField(233, 3111, 28, "Place of Delivery County (literal)", "CNTY_D", 1)]
        public string CNTY_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery City/Town/Place (literal)</summary>
        [IJEField(234, 3139, 28, "Place of Delivery City/Town/Place (literal)", "CITY_D", 1)]
        public string CITY_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Place of Delivery - literal</summary>
        [IJEField(235, 3167, 28, "State, U.S. Territory or Canadian Province of Place of Delivery - literal", "STATE_D", 1)]
        public string STATE_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Country (literal)</summary>
        [IJEField(236, 3195, 28, "Place of Delivery Country (literal)", "COUNTRY_D", 1)]
        public string COUNTRY_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Longitude</summary>
        [IJEField(237, 3223, 17, "Place of Delivery Longitude", "LONG_D", 1)]
        public string LONG_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Place of Delivery Latitude</summary>
        [IJEField(238, 3240, 17, "Place of Delivery Latitude", "LAT_D", 1)]
        public string LAT_D
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Legal First Name</summary>
        [IJEField(239, 3257, 50, "Mother's Legal First Name", "MOMFNAME", 1)]
        public string MOMFNAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Legal Middle Name</summary>
        [IJEField(240, 3307, 50, "Mother's Legal Middle Name", "MOMMNAME", 1)]
        public string MOMMNAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Legal Last Name</summary>
        [IJEField(241, 3357, 50, "Mother's Legal Last Name", "MOMLNAME", 1)]
        public string MOMLNAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Legal Surname Suffix</summary>
        [IJEField(242, 3407, 10, "Mother's Legal Surname Suffix", "MOMSUFFIX", 1)]
        public string MOMSUFFIX
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's First Maiden Name</summary>
        [IJEField(243, 3417, 50, "Mother's First Maiden Name", "MOMFMNME", 1)]
        public string MOMFMNME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Middle Maiden Name</summary>
        [IJEField(244, 3467, 50, "Mother's Middle Maiden Name", "MOMMMID", 1)]
        public string MOMMMID
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Last Maiden Name</summary>
        [IJEField(245, 3517, 50, "Mother's Last Maiden Name", "MOMMAIDN", 1)]
        public string MOMMAIDN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Maiden Surname Suffix</summary>
        [IJEField(246, 3567, 10, "Mother's Maiden Surname Suffix", "MOMMSUFFIX", 1)]
        public string MOMMSUFFIX
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Street number</summary>
        [IJEField(247, 3577, 10, "Mother's Residence Street number", "STNUM", 1)]
        public string STNUM
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Pre Directional</summary>
        [IJEField(248, 3587, 10, "Mother's Residence Pre Directional", "PREDIR", 1)]
        public string PREDIR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Street name</summary>
        [IJEField(249, 3597, 50, "Mother's Residence Street name", "STNAME", 1)]
        public string STNAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Street designator</summary>
        [IJEField(250, 3647, 10, "Mother's Residence Street designator", "STDESIG", 1)]
        public string STDESIG
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Post Directional</summary>
        [IJEField(251, 3657, 10, "Mother's Residence Post Directional", "POSTDIR", 1)]
        public string POSTDIR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Unit or Apartment Number</summary>
        [IJEField(252, 3667, 7, "Mother's Residence Unit or Apartment Number", "APTNUMB", 1)]
        public string APTNUMB
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Street Address</summary>
        [IJEField(253, 3674, 50, "Mother's Residence Street Address", "ADDRESS", 1)]
        public string ADDRESS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Zip code and Zip+4</summary>
        [IJEField(254, 3724, 9, "Mother's Residence Zip code and Zip+4", "ZIPCODE", 1)]
        public string ZIPCODE
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence County (literal)</summary>
        [IJEField(255, 3733, 28, "Mother's Residence County (literal)", "COUNTYTXT", 1)]
        public string COUNTYTXT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence City/Town/Place (literal)</summary>
        [IJEField(256, 3761, 28, "Mother's Residence City/Town/Place (literal)", "CITYTXT", 1)]
        public string CITYTXT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Residence (Mother) - literal</summary>
        [IJEField(257, 3789, 28, "State, U.S. Territory or Canadian Province of Residence (Mother) - literal", "STATETXT", 1)]
        public string STATETXT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Country (literal)</summary>
        [IJEField(258, 3817, 28, "Mother's Residence Country (literal)", "CNTRYTXT", 1)]
        public string CNTRYTXT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Longitude</summary>
        [IJEField(259, 3845, 17, "Mother's Residence Longitude", "LONG", 1)]
        public string LONG
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Residence Latitude</summary>
        [IJEField(260, 3862, 17, "Mother's Residence Latitude", "LAT", 1)]
        public string LAT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Legal First Name</summary>
        [IJEField(261, 3879, 50, "Father's Legal First Name", "DADFNAME", 1)]
        public string DADFNAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Legal Middle Name</summary>
        [IJEField(262, 3929, 50, "Father's Legal Middle Name", "DADMNAME", 1)]
        public string DADMNAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Legal Last Name</summary>
        [IJEField(263, 3979, 50, "Father's Legal Last Name", "DADLNAME", 1)]
        public string DADLNAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Legal Surname Suffix</summary>
        [IJEField(264, 4029, 10, "Father's Legal Surname Suffix", "DADSUFFIX", 1)]
        public string DADSUFFIX
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Social Security Number</summary>
        [IJEField(265, 4039, 9, "Mother's Social Security Number", "MOM_SSN", 1)]
        public string MOM_SSN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Social Security Number</summary>
        [IJEField(266, 4048, 9, "Father's Social Security Number", "DAD_SSN", 1)]
        public string DAD_SSN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Age (Calculated)</summary>
        [IJEField(267, 4057, 2, "Mother's Age (Calculated)", "MAGE_CALC", 1)]
        public string MAGE_CALC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Age (Calculated)</summary>
        [IJEField(268, 4059, 2, "Father's Age (Calculated)", "FAGE_CALC", 1)]
        public string FAGE_CALC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Occupation of Mother</summary>
        [IJEField(269, 4061, 25, "Occupation of Mother", "MOM_OC_T", 1)]
        public string MOM_OC_T
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Occupation of Mother (coded)</summary>
        [IJEField(270, 4086, 3, "Occupation of Mother (coded)", "MOM_OC_C", 1)]
        public string MOM_OC_C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Occupation of Father</summary>
        [IJEField(271, 4089, 25, "Occupation of Father", "DAD_OC_T", 1)]
        public string DAD_OC_T
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Occupation of Father (coded)</summary>
        [IJEField(272, 4114, 3, "Occupation of Father (coded)", "DAD_OC_C", 1)]
        public string DAD_OC_C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Industry of Mother</summary>
        [IJEField(273, 4117, 25, "Industry of Mother", "MOM_IN_T", 1)]
        public string MOM_IN_T
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Industry of Mother (coded)</summary>
        [IJEField(274, 4142, 3, "Industry of Mother (coded)", "MOM_IN_C", 1)]
        public string MOM_IN_C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Industry of Father</summary>
        [IJEField(275, 4145, 25, "Industry of Father", "DAD_IN_T", 1)]
        public string DAD_IN_T
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Industry of Father (coded)</summary>
        [IJEField(276, 4170, 3, "Industry of Father (coded)", "DAD_IN_C", 1)]
        public string DAD_IN_C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Birth (Father) - code</summary>
        [IJEField(277, 4173, 2, "State, U.S. Territory or Canadian Province of Birth (Father) - code", "FBPLACD_ST_TER_C", 1)]
        public string FBPLACD_ST_TER_C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Country of Birth (Code)</summary>
        [IJEField(278, 4175, 2, "Father's Country of Birth (Code)", "FBPLACE_CNT_C", 1)]
        public string FBPLACE_CNT_C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Birth (Mother) - literal</summary>
        [IJEField(279, 4177, 28, "State, U.S. Territory or Canadian Province of Birth (Mother) - literal", "MBPLACE_ST_TER_TXT", 1)]
        public string MBPLACE_ST_TER_TXT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Country of Birth (Literal)</summary>
        [IJEField(280, 4205, 28, "Mother's Country of Birth (Literal)", "MBPLACE_CNTRY_TXT", 1)]
        public string MBPLACE_CNTRY_TXT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Birth (Father) - literal</summary>
        [IJEField(281, 4233, 28, "State, U.S. Territory or Canadian Province of Birth (Father) - literal", "FBPLACE_ST_TER_TXT", 1)]
        public string FBPLACE_ST_TER_TXT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Country of Birth (Literal)</summary>
        [IJEField(282, 4261, 28, "Father's Country of Birth (Literal)", "FBPLACE_CNTRY_TXT", 1)]
        public string FBPLACE_CNTRY_TXT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Education</summary>
        [IJEField(283, 4289, 1, "Father's Education", "FEDUC", 1)]
        public string FEDUC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Education--Edit Flag</summary>
        [IJEField(284, 4290, 1, "Father's Education--Edit Flag", "FEDUC_BYPASS", 1)]
        public string FEDUC_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father of Hispanic Origin?--Mexican</summary>
        [IJEField(285, 4291, 1, "Father of Hispanic Origin?--Mexican", "FETHNIC1", 1)]
        public string FETHNIC1
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father of Hispanic Origin?--Puerto Rican</summary>
        [IJEField(286, 4292, 1, "Father of Hispanic Origin?--Puerto Rican", "FETHNIC2", 1)]
        public string FETHNIC2
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father of Hispanic Origin?--Cuban</summary>
        [IJEField(287, 4293, 1, "Father of Hispanic Origin?--Cuban", "FETHNIC3", 1)]
        public string FETHNIC3
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father of Hispanic Origin?--Other</summary>
        [IJEField(288, 4294, 1, "Father of Hispanic Origin?--Other", "FETHNIC4", 1)]
        public string FETHNIC4
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father of Hispanic Origin?--Other Literal</summary>
        [IJEField(289, 4295, 20, "Father of Hispanic Origin?--Other Literal", "FETHNIC5", 1)]
        public string FETHNIC5
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--White</summary>
        [IJEField(290, 4315, 1, "Father's Race--White", "FRACE1", 1)]
        public string FRACE1
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Black or African American</summary>
        [IJEField(291, 4316, 1, "Father's Race--Black or African American", "FRACE2", 1)]
        public string FRACE2
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--American Indian or Alaska Native</summary>
        [IJEField(292, 4317, 1, "Father's Race--American Indian or Alaska Native", "FRACE3", 1)]
        public string FRACE3
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Asian Indian</summary>
        [IJEField(293, 4318, 1, "Father's Race--Asian Indian", "FRACE4", 1)]
        public string FRACE4
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Chinese</summary>
        [IJEField(294, 4319, 1, "Father's Race--Chinese", "FRACE5", 1)]
        public string FRACE5
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Filipino</summary>
        [IJEField(295, 4320, 1, "Father's Race--Filipino", "FRACE6", 1)]
        public string FRACE6
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Japanese</summary>
        [IJEField(296, 4321, 1, "Father's Race--Japanese", "FRACE7", 1)]
        public string FRACE7
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Korean</summary>
        [IJEField(297, 4322, 1, "Father's Race--Korean", "FRACE8", 1)]
        public string FRACE8
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Vietnamese</summary>
        [IJEField(298, 4323, 1, "Father's Race--Vietnamese", "FRACE9", 1)]
        public string FRACE9
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Other Asian</summary>
        [IJEField(299, 4324, 1, "Father's Race--Other Asian", "FRACE10", 1)]
        public string FRACE10
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Native Hawaiian</summary>
        [IJEField(300, 4325, 1, "Father's Race--Native Hawaiian", "FRACE11", 1)]
        public string FRACE11
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Guamanian or Chamorro</summary>
        [IJEField(301, 4326, 1, "Father's Race--Guamanian or Chamorro", "FRACE12", 1)]
        public string FRACE12
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Samoan</summary>
        [IJEField(302, 4327, 1, "Father's Race--Samoan", "FRACE13", 1)]
        public string FRACE13
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Other Pacific Islander</summary>
        [IJEField(303, 4328, 1, "Father's Race--Other Pacific Islander", "FRACE14", 1)]
        public string FRACE14
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Other</summary>
        [IJEField(304, 4329, 1, "Father's Race--Other", "FRACE15", 1)]
        public string FRACE15
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--First American Indian or Alaska Native Literal</summary>
        [IJEField(305, 4330, 30, "Father's Race--First American Indian or Alaska Native Literal", "FRACE16", 1)]
        public string FRACE16
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Second American Indian or Alaska Native Literal</summary>
        [IJEField(306, 4360, 30, "Father's Race--Second American Indian or Alaska Native Literal", "FRACE17", 1)]
        public string FRACE17
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--First Other Asian Literal</summary>
        [IJEField(307, 4390, 30, "Father's Race--First Other Asian Literal", "FRACE18", 1)]
        public string FRACE18
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Second Other Asian Literal</summary>
        [IJEField(308, 4420, 30, "Father's Race--Second Other Asian Literal", "FRACE19", 1)]
        public string FRACE19
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--First Other Pacific Islander Literal</summary>
        [IJEField(309, 4450, 30, "Father's Race--First Other Pacific Islander Literal", "FRACE20", 1)]
        public string FRACE20
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Second Other Pacific Islander Literal</summary>
        [IJEField(310, 4480, 30, "Father's Race--Second Other Pacific Islander Literal", "FRACE21", 1)]
        public string FRACE21
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--First Other Literal</summary>
        [IJEField(311, 4510, 30, "Father's Race--First Other Literal", "FRACE22", 1)]
        public string FRACE22
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race--Second Other Literal</summary>
        [IJEField(312, 4540, 30, "Father's Race--Second Other Literal", "FRACE23", 1)]
        public string FRACE23
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 1E</summary>
        [IJEField(313, 4570, 3, "Father's Race Tabulation Variable 1E", "FRACE1E", 1)]
        public string FRACE1E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 2E</summary>
        [IJEField(314, 4573, 3, "Father's Race Tabulation Variable 2E", "FRACE2E", 1)]
        public string FRACE2E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 3E</summary>
        [IJEField(315, 4576, 3, "Father's Race Tabulation Variable 3E", "FRACE3E", 1)]
        public string FRACE3E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 4E</summary>
        [IJEField(316, 4579, 3, "Father's Race Tabulation Variable 4E", "FRACE4E", 1)]
        public string FRACE4E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 5E</summary>
        [IJEField(317, 4582, 3, "Father's Race Tabulation Variable 5E", "FRACE5E", 1)]
        public string FRACE5E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 6E</summary>
        [IJEField(318, 4585, 3, "Father's Race Tabulation Variable 6E", "FRACE6E", 1)]
        public string FRACE6E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 7E</summary>
        [IJEField(319, 4588, 3, "Father's Race Tabulation Variable 7E", "FRACE7E", 1)]
        public string FRACE7E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 8E</summary>
        [IJEField(320, 4591, 3, "Father's Race Tabulation Variable 8E", "FRACE8E", 1)]
        public string FRACE8E
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 16C</summary>
        [IJEField(321, 4594, 3, "Father's Race Tabulation Variable 16C", "FRACE16C", 1)]
        public string FRACE16C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 17C</summary>
        [IJEField(322, 4597, 3, "Father's Race Tabulation Variable 17C", "FRACE17C", 1)]
        public string FRACE17C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 18C</summary>
        [IJEField(323, 4600, 3, "Father's Race Tabulation Variable 18C", "FRACE18C", 1)]
        public string FRACE18C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 19C</summary>
        [IJEField(324, 4603, 3, "Father's Race Tabulation Variable 19C", "FRACE19C", 1)]
        public string FRACE19C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 20C</summary>
        [IJEField(325, 4606, 3, "Father's Race Tabulation Variable 20C", "FRACE20C", 1)]
        public string FRACE20C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 21C</summary>
        [IJEField(326, 4609, 3, "Father's Race Tabulation Variable 21C", "FRACE21C", 1)]
        public string FRACE21C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 22C</summary>
        [IJEField(327, 4612, 3, "Father's Race Tabulation Variable 22C", "FRACE22C", 1)]
        public string FRACE22C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race Tabulation Variable 23C</summary>
        [IJEField(328, 4615, 3, "Father's Race Tabulation Variable 23C", "FRACE23C", 1)]
        public string FRACE23C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Hispanic Code for Literal</summary>
        [IJEField(329, 4618, 3, "Mother's Hispanic Code for Literal", "METHNIC5C", 1)]
        public string METHNIC5C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Edited Hispanic Origin Code</summary>
        [IJEField(330, 4621, 3, "Mother's Edited Hispanic Origin Code", "METHNICE", 1)]
        public string METHNICE
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Bridged Race - NCHS Code</summary>
        [IJEField(331, 4624, 2, "Mother's Bridged Race - NCHS Code", "MRACEBG_C", 1)]
        public string MRACEBG_C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Hispanic Code for Literal</summary>
        [IJEField(332, 4626, 3, "Father's Hispanic Code for Literal", "FETHNIC5C", 1)]
        public string FETHNIC5C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Edited Hispanic Origin Code</summary>
        [IJEField(333, 4629, 3, "Father's Edited Hispanic Origin Code", "FETHNICE", 1)]
        public string FETHNICE
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Bridged Race - NCHS Code</summary>
        [IJEField(334, 4632, 2, "Father's Bridged Race - NCHS Code", "FRACEBG_C", 1)]
        public string FRACEBG_C
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Hispanic Origin - Specify</summary>
        [IJEField(335, 4634, 15, "Mother's Hispanic Origin - Specify", "METHNIC_T", 1)]
        public string METHNIC_T
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Mother's Race - Specify</summary>
        [IJEField(336, 4649, 50, "Mother's Race - Specify", "MRACE_T", 1)]
        public string MRACE_T
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Hispanic Origin - Specify</summary>
        [IJEField(337, 4699, 15, "Father's Hispanic Origin - Specify", "FETHNIC_T", 1)]
        public string FETHNIC_T
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Father's Race - Specify</summary>
        [IJEField(338, 4714, 50, "Father's Race - Specify", "FRACE_T", 1)]
        public string FRACE_T
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Facility Mother Moved From (if transferred)</summary>
        [IJEField(339, 4764, 50, "Facility Mother Moved From (if transferred)", "HOSPFROM", 1)]
        public string HOSPFROM
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Attendant's Name</summary>
        [IJEField(340, 4814, 50, "Attendant's Name", "ATTEND_NAME", 1)]
        public string ATTEND_NAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Attendant's NPI</summary>
        [IJEField(341, 4864, 12, "Attendant's NPI", "ATTEND_NPI", 1)]
        public string ATTEND_NPI
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Attendant ("Other" specified text)</summary>
        [IJEField(342, 4876, 20, "Attendant (\"Other\" specified text)", "ATTEND_OTH_TXT", 1)]
        public string ATTEND_OTH_TXT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Informant's First Name</summary>
        [IJEField(343, 4896, 50, "Informant's First Name", "INFORMFST", 1)]
        public string INFORMFST
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Informant's Middle Name</summary>
        [IJEField(344, 4946, 50, "Informant's Middle Name", "INFORMMID", 1)]
        public string INFORMMID
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Informant's Last Name</summary>
        [IJEField(345, 4996, 50, "Informant's Last Name", "INFORMLST", 1)]
        public string INFORMLST
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Informant's Relationship to Fetus</summary>
        [IJEField(346, 5046, 1, "Informant's Relationship to Fetus", "INFORMRELATE", 1)]
        public string INFORMRELATE
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date Signed by Certifier--Year</summary>
        [IJEField(347, 5047, 4, "Date Signed by Certifier--Year", "CERTIFIED_YR", 1)]
        public string CERTIFIED_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date Signed by Certifier--Month</summary>
        [IJEField(348, 5051, 2, "Date Signed by Certifier--Month", "CERTIFIED_MO", 1)]
        public string CERTIFIED_MO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date Signed by Certifier--Day</summary>
        [IJEField(349, 5053, 2, "Date Signed by Certifier--Day", "CERTIFIED_DY", 1)]
        public string CERTIFIED_DY
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date Filed by Registrar--Year</summary>
        [IJEField(350, 5055, 4, "Date Filed by Registrar--Year", "REGISTER_YR", 1)]
        public string REGISTER_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date Filed by Registrar--Month</summary>
        [IJEField(351, 5059, 2, "Date Filed by Registrar--Month", "REGISTER_MO", 1)]
        public string REGISTER_MO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Date Filed by Registrar--Day</summary>
        [IJEField(352, 5061, 2, "Date Filed by Registrar--Day", "REGISTER_DY", 1)]
        public string REGISTER_DY
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Replacement (amended) Record</summary>
        [IJEField(353, 5063, 1, "Replacement (amended) Record", "REPLACE", 1)]
        public string REPLACE
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for One-Byte Field 1</summary>
        [IJEField(354, 5064, 1, "Blank for One-Byte Field 1", "PLACE1_1", 1)]
        public string PLACE1_1
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for One-Byte Field 2</summary>
        [IJEField(355, 5065, 1, "Blank for One-Byte Field 2", "PLACE1_2", 1)]
        public string PLACE1_2
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for One-Byte Field 3</summary>
        [IJEField(356, 5066, 1, "Blank for One-Byte Field 3", "PLACE1_3", 1)]
        public string PLACE1_3
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for One-Byte Field 4</summary>
        [IJEField(357, 5067, 1, "Blank for One-Byte Field 4", "PLACE1_4", 1)]
        public string PLACE1_4
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for One-Byte Field 5</summary>
        [IJEField(358, 5068, 1, "Blank for One-Byte Field 5", "PLACE1_5", 1)]
        public string PLACE1_5
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for One-Byte Field 6</summary>
        [IJEField(359, 5069, 1, "Blank for One-Byte Field 6", "PLACE1_6", 1)]
        public string PLACE1_6
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for Eight-Byte Field 1</summary>
        [IJEField(360, 5070, 8, "Blank for Eight-Byte Field 1", "PLACE8_1", 1)]
        public string PLACE8_1
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for Eight-Byte Field 2</summary>
        [IJEField(361, 5078, 8, "Blank for Eight-Byte Field 2", "PLACE8_2", 1)]
        public string PLACE8_2
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for Eight-Byte Field 3</summary>
        [IJEField(362, 5086, 8, "Blank for Eight-Byte Field 3", "PLACE8_3", 1)]
        public string PLACE8_3
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for Twenty-Byte Field</summary>
        [IJEField(363, 5094, 20, "Blank for Twenty-Byte Field", "PLACE20", 1)]
        public string PLACE20
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for future expansion</summary>
        [IJEField(364, 5114, 450, "Blank for future expansion", "BLANK", 1)]
        public string BLANK
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }

        /// <summary>Blank for Jurisdictional Use Only</summary>
        [IJEField(365, 5564, 437, "Blank for Jurisdictional Use Only", "BLANK2", 1)]
        public string BLANK2
        {
            get
            {
                // TODO: Implement mapping from FHIR record location: 
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location: 
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using VR;

namespace BFDR
{
        /// <summary>A "wrapper" class to convert between a FHIR based <c>BirthRecord</c> and
    /// a record in IJE Natality format. Each property of this class corresponds exactly
    /// with a field in the IJE Natality format. The getters convert from the embedded
    /// FHIR based <c>BirthRecord</c> to the IJE format for a specific field, and
    /// the setters convert from IJE format for a specific field and set that value
    /// on the embedded FHIR based <c>BirthRecord</c>.</summary>
    public class IJENatality : IJE
    {
        private readonly BirthRecord record;

        /// <summary>Constructor that takes a <c>BirthRecord</c>.</summary>
        public IJENatality(BirthRecord record, bool validate = true)
        {
            this.record = record;
            if (validate)
            {
                // We need to force a conversion to happen by calling ToString() if we want to validate
                ToString();
                CheckForValidationErrors();
            }
        }

        /// <summary>Constructor that takes an IJE string and builds a corresponding internal <c>BirthRecord</c>.</summary>
        public IJENatality(string ije, bool validate = true) : this()
        {
            ProcessIJE(ije, validate);
        }

        /// <summary>Get the length of the IJE string.</summary>
        protected override uint IJELength
        {
            get
            {
                return 4000;
            }
        }


        /// <summary>Constructor that creates an empty record for constructing records using the IJE properties.</summary>
        public IJENatality()
        {
            this.record = new BirthRecord();
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
        public BirthRecord ToBirthRecord()
        {
            return this.record;
        }

        /// <summary>FHIR based vital record.</summary>
        /// Hides the IJE ToRecord method that returns a VitalRecord instead of a DeathRecord
        public new BirthRecord ToRecord()
        {
            return this.record;
        }

        /// <summary>Converts the FHIR representation of presence-only fields to the IJE equivalent.</summary>
        /// <param name="fieldValue">the value of the field</param>
        /// <param name="noneOfTheAboveValue">the value of the corresponding none-of-the-above field</param>
        /// <returns>Y (yes), N (no), or U (unknown)</returns>
        private string PresenceToIJE(bool fieldValue, bool noneOfTheAboveValue)
        {
            if (fieldValue)
            {
                return "Y";
            }
            else if (noneOfTheAboveValue)
            {
                return "N";
            }
            else
            {
                return "U";
            }
        }

        /// <summary>Converts the IJE representation of presence-only fields to the FHIR equivalent.</summary>
        /// <param name="value">Y (yes), N (no), or U (unknown)</param>
        /// <param name="field">a function that will set a field in the FHIR record</param>
        /// <param name="noneOfTheAboveField">a function that will set the corresponding none-of-the-above field in the FHIR record</param>
        private void IJEToPresence(string value, Func<bool, bool> field, Func<bool, bool> noneOfTheAboveField)
        {
            if (value.Equals("Y"))
            {
                field(true);
            }
            else if (value.Equals("N"))
            {
                noneOfTheAboveField(true);
            }
            else
            {
                field(false);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////
        //
        // Class helper methods for getting and settings IJE fields.
        //
        /////////////////////////////////////////////////////////////////////////////////
        /// <summary>Checks if the given race exists in the record for Mother.</summary>
        private string Get_MotherRace(string name)
        {
            Tuple<string, string>[] raceStatus = record.MotherRace.ToArray();

            Tuple<string, string> raceTuple = Array.Find(raceStatus, element => element.Item1 == name);
            if (raceTuple != null)
            {
                return (raceTuple.Item2).Trim();
            }
            return "";
        }

        /// <summary>Adds the given race to the record for Mother.</summary>
        private void Set_MotherRace(string name, string value)
        {
            List<Tuple<string, string>> raceStatus = record.MotherRace.ToList();
            raceStatus.Add(Tuple.Create(name, value));
            record.MotherRace = raceStatus.Distinct().ToArray();
        }
        /// <summary>Checks if the given race exists in the record for Father.</summary>
        private string Get_FatherRace(string name)
        {
            Tuple<string, string>[] raceStatus = record.FatherRace.ToArray();

            Tuple<string, string> raceTuple = Array.Find(raceStatus, element => element.Item1 == name);
            if (raceTuple != null)
            {
                return (raceTuple.Item2).Trim();
            }
            return "";
        }

        /// <summary>Adds the given race to the record for Father.</summary>
        private void Set_FatherRace(string name, string value)
        {
            List<Tuple<string, string>> raceStatus = record.FatherRace.ToList();
            raceStatus.Add(Tuple.Create(name, value));
            record.FatherRace = raceStatus.Distinct().ToArray();
        }
        /////////////////////////////////////////////////////////////////////////////////
        //
        // Class Properties that provide getters and setters for each of the IJE
        // Natality fields.
        //
        // Getters look at the embedded DeathRecord and convert values to IJE style.
        // Setters convert and store IJE style values to the embedded DeathRecord.
        //
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>Date of Birth (Infant)--Year</summary>
        [IJEField(1, 1, 4, "Date of Birth (Infant)--Year", "IDOB_YR", 1)]
        public string IDOB_YR
        {
            get
            {
                return NumericAllowingUnknown_Get("IDOB_YR", "BirthYear");
            }
            set
            {
                NumericAllowingUnknown_Set("IDOB_YR", "BirthYear", value);
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Birth (Infant) - code</summary>
        [IJEField(2, 5, 2, "State, U.S. Territory or Canadian Province of Birth (Infant) - code", "BSTATE", 1)]
        public string BSTATE
        {
            get
            {
                return Dictionary_Geo_Get("BSTATE", "PlaceOfBirth", "address", "state", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Set("BSTATE", "PlaceOfBirth", "addressState", value);
                }
            }
        }

        /// <summary>Certificate Number</summary>
        [IJEField(3, 7, 6, "Certificate Number", "FILENO", 1)]
        public string FILENO
        {
            get
            {
                if (String.IsNullOrWhiteSpace(record?.Identifier))
                {
                    return "".PadLeft(6, '0');
                }
                string id_str = record.Identifier;
                if (id_str.Length > 6)
                {
                    id_str = id_str.Substring(id_str.Length - 6);
                }
                return id_str.PadLeft(6, '0');
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    RightJustifiedZeroed_Set("FILENO", "Identifier", value);
                }
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

        /// <summary>Time of Birth</summary>
        [IJEField(6, 26, 4, "Time of Birth", "TB", 1)]
        public string TB
        {
            get
            {
                return TimeAllowingUnknown_Get("TB", "BirthTime");
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                   TimeAllowingUnknown_Set("TB", "BirthTime", value);
                }
            }
        }

        /// <summary>Sex</summary>
        [IJEField(7, 30, 1, "Sex", "ISEX", 1)]
        public string ISEX
        {
            get
            {
                return Get_MappingFHIRToIJE(VR.Mappings.ConceptMapBirthSexChildVitalRecords.FHIRToIJE, "BirthSex", "ISEX");
            }
            set
            {
                Set_MappingIJEToFHIR(VR.Mappings.ConceptMapBirthSexChildVitalRecords.IJEToFHIR, "ISEX", "BirthSex", value);
            }
        }

        /// <summary>Date of Birth (Infant)--Month</summary>
        [IJEField(8, 31, 2, "Date of Birth (Infant)--Month", "IDOB_MO", 1)]
        public string IDOB_MO
        {
            get
            {
                return NumericAllowingUnknown_Get("IDOB_MO", "BirthMonth");
            }
            set
            {
                NumericAllowingUnknown_Set("IDOB_MO", "BirthMonth", value);
            }
        }

        /// <summary>Date of Birth (Infant)--Day</summary>
        [IJEField(9, 33, 2, "Date of Birth (Infant)--Day", "IDOB_DY", 1)]
        public string IDOB_DY
        {
            get
            {
                return NumericAllowingUnknown_Get("IDOB_DY", "BirthDay");
            }
            set
            {
                NumericAllowingUnknown_Set("IDOB_DY", "BirthDay", value);
            }
        }

        /// <summary>County of Birth</summary>
        [IJEField(10, 35, 3, "County of Birth", "CNTYO", 1)]
        public string CNTYO
        {
            get
            {
                return Dictionary_Geo_Get("CNTYO", "PlaceOfBirth", "address", "countyC", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("CNTYO", "PlaceOfBirth", "address", "countyC", true, value);
                }
            }
        }

        /// <summary>Place Where Birth Occurred (type of place or institution)</summary>
        [IJEField(11, 38, 1, "Place Where Birth Occurred (type of place or institution)", "BPLACE", 1)]
        public string BPLACE
        {
            get
            {
                return Get_MappingFHIRToIJE(BFDR.Mappings.BirthDeliveryOccurred.FHIRToIJE, "BirthPhysicalLocation", "BPLACE");
            }
            set
            {
                Set_MappingIJEToFHIR(BFDR.Mappings.BirthDeliveryOccurred.IJEToFHIR, "BPLACE", "BirthPhysicalLocation", value);
            }
        }

        /// <summary>Facility ID (NPI) - if available</summary>
        [IJEField(12, 39, 12, "Facility ID (NPI) - if available", "FNPI", 1)]
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
                return NumericAllowingUnknown_Get("MDOB_YR", "MotherBirthYear");
            }
            set
            {
                NumericAllowingUnknown_Set("MDOB_YR", "MotherBirthYear", value);
            }
        }

        /// <summary>Date of Birth (Mother)--Month</summary>
        [IJEField(15, 59, 2, "Date of Birth (Mother)--Month", "MDOB_MO", 1)]
        public string MDOB_MO
        {
            get
            {
                return NumericAllowingUnknown_Get("MDOB_MO", "MotherBirthMonth");
            }
            set
            {
                NumericAllowingUnknown_Set("MDOB_MO", "MotherBirthMonth", value);
            }
        }

        /// <summary>Date of Birth (Mother)--Day</summary>
        [IJEField(16, 61, 2, "Date of Birth (Mother)--Day", "MDOB_DY", 1)]
        public string MDOB_DY
        {
            get
            {
                return NumericAllowingUnknown_Get("MDOB_DY", "MotherBirthDay");
            }
            set
            {
                NumericAllowingUnknown_Set("MDOB_DY", "MotherBirthDay", value);
            }
        }

        /// <summary>Date of Birth (Mother)--Edit Flag</summary>
        [IJEField(17, 63, 1, "Date of Birth (Mother)--Edit Flag", "MAGE_BYPASS", 1)]
        public string MAGE_BYPASS
        {
            get
            {
                return Get_MappingFHIRToIJE(VR.Mappings.ConceptMapMothersDateOfBirthEditFlagsVitalRecords.FHIRToIJE, "MotherDateOfBirthEditFlag", "MAGE_BYPASS").PadLeft(1, ' ');
            }
            set
            {
                Set_MappingIJEToFHIR(VR.Mappings.ConceptMapMothersDateOfBirthEditFlagsVitalRecords.IJEToFHIR, "MAGE_BYPASS", "MotherDateOfBirthEditFlag", value);
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Birth (Mother) - code</summary>
        [IJEField(18, 64, 2, "State, U.S. Territory or Canadian Province of Birth (Mother) - code", "BPLACEC_ST_TER", 1)]
        public string BPLACEC_ST_TER
        {
            get
            {
                return Dictionary_Geo_Get("BPLACEC_ST_TER", "MotherPlaceOfBirth", "address", "state", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Set("BPLACEC_ST_TER", "MotherPlaceOfBirth", "addressState", value);
                }
            }
        }

        /// <summary>Birthplace of Mother--Country</summary>
        [IJEField(19, 66, 2, "Birthplace of Mother--Country", "BPLACEC_CNT", 1)]
        public string BPLACEC_CNT
        {
            get
            {
                return Dictionary_Geo_Get("BPLACEC_CNT", "MotherPlaceOfBirth", "address", "country", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Set("BPLACEC_CNT", "MotherPlaceOfBirth", "addressCountry", value);
                }
            }
        }

        /// <summary>Residence of Mother--City</summary>
        [IJEField(20, 68, 5, "Residence of Mother--City", "CITYC", 1)]
        public string CITYC
        {
            get
            {
                return Dictionary_Geo_Get("CITYC", "MotherResidence", "address", "cityC", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("CITYC", "MotherResidence", "address", "cityC", true, value);
                }
            }
        }

        /// <summary>Residence of Mother--County</summary>
        [IJEField(21, 73, 3, "Residence of Mother--County", "COUNTYC", 1)]
        public string COUNTYC
        {
            get
            {
                return Dictionary_Geo_Get("COUNTYC", "MotherResidence", "address", "countyC", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("COUNTYC", "MotherResidence", "address", "countyC", true, value);
                }
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Residence (Mother) - code</summary>
        [IJEField(22, 76, 2, "State, U.S. Territory or Canadian Province of Residence (Mother) - code", "STATEC", 1)]
        public string STATEC
        {
            get
            {
                return Dictionary_Geo_Get("STATEC", "MotherResidence", "address", "state", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("STATEC", "MotherResidence", "address", "state", true, value);
                }
            }
        }

        /// <summary>Residence of Mother--Country</summary>
        [IJEField(23, 78, 2, "Residence of Mother--Country", "COUNTRYC", 1)]
        public string COUNTRYC
        {
            get
            {
                return Dictionary_Geo_Get("COUNTRYC", "MotherResidence", "address", "country", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("COUNTRYC", "MotherResidence", "address", "country", true, value);
                }
            }
        }

        /// <summary>Residence of Mother--Inside City Limits</summary>
        [IJEField(24, 80, 1, "Residence of Mother--Inside City Limits", "LIMITS", 1)]
        public string LIMITS
        {
            get
            {
                return Get_MappingFHIRToIJE(VR.Mappings.ConceptMapYesNoUnknownVitalRecords.FHIRToIJE, "MotherResidenceWithinCityLimits", "LIMITS");
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MappingIJEToFHIR(VR.Mappings.ConceptMapYesNoUnknownVitalRecords.IJEToFHIR, "LIMITS", "MotherResidenceWithinCityLimits", value);
                }
            }
        }

        /// <summary>Date of Birth (Father)--Year</summary>
        [IJEField(25, 81, 4, "Date of Birth (Father)--Year", "FDOB_YR", 1)]
        public string FDOB_YR
        {
            get
            {
                return NumericAllowingUnknown_Get("FDOB_YR", "FatherBirthYear");
            }
            set
            {
                NumericAllowingUnknown_Set("FDOB_YR", "FatherBirthYear", value);
            }
        }

        /// <summary>Date of Birth (Father)--Month</summary>
        [IJEField(26, 85, 2, "Date of Birth (Father)--Month", "FDOB_MO", 1)]
        public string FDOB_MO
        {
            get
            {
                return NumericAllowingUnknown_Get("FDOB_MO", "FatherBirthMonth");
            }
            set
            {
                NumericAllowingUnknown_Set("FDOB_MO", "FatherBirthMonth", value);
            }
        }

        /// <summary>Date of Birth (Father)--Day</summary>
        [IJEField(27, 87, 2, "Date of Birth (Father)--Day", "FDOB_DY", 1)]
        public string FDOB_DY
        {
            get
            {
                return NumericAllowingUnknown_Get("FDOB_DY", "FatherBirthDay");
            }
            set
            {
                NumericAllowingUnknown_Set("FDOB_DY", "FatherBirthDay", value);
            }
        }

        /// <summary>Date of Birth (Father)--Edit Flag</summary>
        [IJEField(28, 89, 1, "Date of Birth (Father)--Edit Flag", "FAGE_BYPASS", 1)]
        public string FAGE_BYPASS
        {
            get
            {
                return Get_MappingFHIRToIJE(VR.Mappings.ConceptMapMothersDateOfBirthEditFlagsVitalRecords.FHIRToIJE, "FatherDateOfBirthEditFlag", "FAGE_BYPASS").PadLeft(1, ' ');
            }
            set
            {
                Set_MappingIJEToFHIR(VR.Mappings.ConceptMapMothersDateOfBirthEditFlagsVitalRecords.IJEToFHIR, "FAGE_BYPASS", "FatherDateOfBirthEditFlag", value);
            }
        }

        /// <summary>Mother Married?--Ever (NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(29, 90, 1, "Mother Married?--Ever (NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "MARE", 1)]
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

        /// <summary>Mother Married?-- At Conception, at Birth or any Time in Between</summary>
        [IJEField(30, 91, 1, "Mother Married?-- At Conception, at Birth or any Time in Between", "MARN", 1)]
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

        /// <summary>Mother Married?--Acknowledgement of Paternity Signed</summary>
        [IJEField(31, 92, 1, "Mother Married?--Acknowledgement of Paternity Signed", "ACKN", 1)]
        public string ACKN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

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
                string code = Get_MappingFHIRToIJE(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.FHIRToIJE, "MotherEthnicity1", "METHNIC1");
                if (String.IsNullOrWhiteSpace(code))
                {
                    code = "U";
                }
                return code;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MappingIJEToFHIR(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.IJEToFHIR, "METHNIC1", "MotherEthnicity1", value);
                }
            }
        }

        /// <summary>Mother of Hispanic Origin?--Puerto Rican</summary>
        [IJEField(35, 96, 1, "Mother of Hispanic Origin?--Puerto Rican", "METHNIC2", 1)]
        public string METHNIC2
        {
            get
            {
                string code = Get_MappingFHIRToIJE(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.FHIRToIJE, "MotherEthnicity2", "METHNIC2");
                if (String.IsNullOrWhiteSpace(code))
                {
                    code = "U";
                }
                return code;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MappingIJEToFHIR(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.IJEToFHIR, "METHNIC2", "MotherEthnicity2", value);
                }
            }
        }

        /// <summary>Mother of Hispanic Origin?--Cuban</summary>
        [IJEField(36, 97, 1, "Mother of Hispanic Origin?--Cuban", "METHNIC3", 1)]
        public string METHNIC3
        {
            get
            {
                string code = Get_MappingFHIRToIJE(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.FHIRToIJE, "MotherEthnicity3", "METHNIC3");
                if (String.IsNullOrWhiteSpace(code))
                {
                    code = "U";
                }
                return code;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MappingIJEToFHIR(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.IJEToFHIR, "METHNIC3", "MotherEthnicity3", value);
                }
            }
        }

        /// <summary>Mother of Hispanic Origin?--Other</summary>
        [IJEField(37, 98, 1, "Mother of Hispanic Origin?--Other", "METHNIC4", 1)]
        public string METHNIC4
        {
            get
            {
                string code = Get_MappingFHIRToIJE(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.FHIRToIJE, "MotherEthnicity4", "METHNIC4");
                if (String.IsNullOrWhiteSpace(code))
                {
                    code = "U";
                }
                return code;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MappingIJEToFHIR(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.IJEToFHIR, "METHNIC4", "MotherEthnicity4", value);
                }
            }
        }

        /// <summary>Mother of Hispanic Origin?--Other Literal</summary>
        [IJEField(38, 99, 20, "Mother of Hispanic Origin?--Other Literal", "METHNIC5", 1)]
        public string METHNIC5
        {
            get
            {
                var ethnicityLiteral = record.MotherEthnicityLiteral;
                if (!String.IsNullOrWhiteSpace(ethnicityLiteral))
                {
                    return Truncate(ethnicityLiteral, 20).Trim();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    record.MotherEthnicityLiteral = value;
                }
            }
        }

        /// <summary>Mother's Race--White</summary>
        [IJEField(39, 119, 1, "Mother's Race--White", "MRACE1", 1)]
        public string MRACE1
        {
            get
            {
                return Get_MotherRace(NvssRace.White);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.White, value);
                }
            }
        }

        /// <summary>Mother's Race--Black or African American</summary>
        [IJEField(40, 120, 1, "Mother's Race--Black or African American", "MRACE2", 1)]
        public string MRACE2
        {
            get
            {
                return Get_MotherRace(NvssRace.BlackOrAfricanAmerican);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.BlackOrAfricanAmerican, value);
                }
            }
        }

        /// <summary>Mother's Race--American Indian or Alaska Native</summary>
        [IJEField(41, 121, 1, "Mother's Race--American Indian or Alaska Native", "MRACE3", 1)]
        public string MRACE3
        {
            get
            {
                return Get_MotherRace(NvssRace.AmericanIndianOrAlaskanNative);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.AmericanIndianOrAlaskanNative, value);
                }
            }
        }

        /// <summary>Mother's Race--Asian Indian</summary>
        [IJEField(42, 122, 1, "Mother's Race--Asian Indian", "MRACE4", 1)]
        public string MRACE4
        {
            get
            {
                return Get_MotherRace(NvssRace.AsianIndian);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.AsianIndian, value);
                }
            }
        }

        /// <summary>Mother's Race--Chinese</summary>
        [IJEField(43, 123, 1, "Mother's Race--Chinese", "MRACE5", 1)]
        public string MRACE5
        {
            get
            {
                return Get_MotherRace(NvssRace.Chinese);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.Chinese, value);
                }
            }
        }

        /// <summary>Mother's Race--Filipino</summary>
        [IJEField(44, 124, 1, "Mother's Race--Filipino", "MRACE6", 1)]
        public string MRACE6
        {
            get
            {
                return Get_MotherRace(NvssRace.Filipino);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.Filipino, value);
                }
            }
        }

        /// <summary>Mother's Race--Japanese</summary>
        [IJEField(45, 125, 1, "Mother's Race--Japanese", "MRACE7", 1)]
        public string MRACE7
        {
            get
            {
                return Get_MotherRace(NvssRace.Japanese);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.Japanese, value);
                }
            }
        }

        /// <summary>Mother's Race--Korean</summary>
        [IJEField(46, 126, 1, "Mother's Race--Korean", "MRACE8", 1)]
        public string MRACE8
        {
            get
            {
                return Get_MotherRace(NvssRace.Korean);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.Korean, value);
                }
            }
        }

        /// <summary>Mother's Race--Vietnamese</summary>
        [IJEField(47, 127, 1, "Mother's Race--Vietnamese", "MRACE9", 1)]
        public string MRACE9
        {
            get
            {
                return Get_MotherRace(NvssRace.Vietnamese);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.Vietnamese, value);
                }
            }
        }

        /// <summary>Mother's Race--Other Asian</summary>
        [IJEField(48, 128, 1, "Mother's Race--Other Asian", "MRACE10", 1)]
        public string MRACE10
        {
            get
            {
                return Get_MotherRace(NvssRace.OtherAsian);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.OtherAsian, value);
                }

            }
        }

        /// <summary>Mother's Race--Native Hawaiian</summary>
        [IJEField(49, 129, 1, "Mother's Race--Native Hawaiian", "MRACE11", 1)]
        public string MRACE11
        {
            get
            {
                return Get_MotherRace(NvssRace.NativeHawaiian);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.NativeHawaiian, value);
                }
            }
        }

        /// <summary>Mother's Race--Guamanian or Chamorro</summary>
        [IJEField(50, 130, 1, "Mother's Race--Guamanian or Chamorro", "MRACE12", 1)]
        public string MRACE12
        {
            get
            {
                return Get_MotherRace(NvssRace.GuamanianOrChamorro);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.GuamanianOrChamorro, value);
                }
            }
        }

        /// <summary>Mother's Race--Samoan</summary>
        [IJEField(51, 131, 1, "Mother's Race--Samoan", "MRACE13", 1)]
        public string MRACE13
        {
            get
            {
                return Get_MotherRace(NvssRace.Samoan);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.Samoan, value);
                }
            }
        }

        /// <summary>Mother's Race--Other Pacific Islander</summary>
        [IJEField(52, 132, 1, "Mother's Race--Other Pacific Islander", "MRACE14", 1)]
        public string MRACE14
        {
            get
            {
                return Get_MotherRace(NvssRace.OtherPacificIslander);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.OtherPacificIslander, value);
                }
            }
        }

        /// <summary>Mother's Race--Other</summary>
        [IJEField(53, 133, 1, "Mother's Race--Other", "MRACE15", 1)]
        public string MRACE15
        {
            get
            {
                return Get_MotherRace(NvssRace.OtherRace);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.OtherRace, value);
                }
            }
        }

        /// <summary>Mother's Race--First American Indian or Alaska Native Literal</summary>
        [IJEField(54, 134, 30, "Mother's Race--First American Indian or Alaska Native Literal", "MRACE16", 1)]
        public string MRACE16
        {
            get
            {
                return Get_MotherRace(NvssRace.FirstAmericanIndianOrAlaskanNativeLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.FirstAmericanIndianOrAlaskanNativeLiteral, value);
                }
            }
        }

        /// <summary>Mother's Race--Second American Indian or Alaska Native Literal</summary>
        [IJEField(55, 164, 30, "Mother's Race--Second American Indian or Alaska Native Literal", "MRACE17", 1)]
        public string MRACE17
        {
            get
            {
                return Get_MotherRace(NvssRace.SecondAmericanIndianOrAlaskanNativeLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.SecondAmericanIndianOrAlaskanNativeLiteral, value);
                }
            }
        }

        /// <summary>Mother's Race--First Other Asian Literal</summary>
        [IJEField(56, 194, 30, "Mother's Race--First Other Asian Literal", "MRACE18", 1)]
        public string MRACE18
        {
            get
            {
                return Get_MotherRace(NvssRace.FirstOtherAsianLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.FirstOtherAsianLiteral, value);
                }
            }
        }

        /// <summary>Mother's Race--Second Other Asian Literal</summary>
        [IJEField(57, 224, 30, "Mother's Race--Second Other Asian Literal", "MRACE19", 1)]
        public string MRACE19
        {
            get
            {
                return Get_MotherRace(NvssRace.SecondOtherAsianLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.SecondOtherAsianLiteral, value);
                }
            }
        }

        /// <summary>Mother's Race--First Other Pacific Islander Literal</summary>
        [IJEField(58, 254, 30, "Mother's Race--First Other Pacific Islander Literal", "MRACE20", 1)]
        public string MRACE20
        {
            get
            {
                return Get_MotherRace(NvssRace.FirstOtherPacificIslanderLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.FirstOtherPacificIslanderLiteral, value);
                }
            }
        }

        /// <summary>Mother's Race--Second Other Pacific Islander Literal</summary>
        [IJEField(59, 284, 30, "Mother's Race--Second Other Pacific Islander Literal", "MRACE21", 1)]
        public string MRACE21
        {
            get
            {
                return Get_MotherRace(NvssRace.SecondOtherPacificIslanderLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.SecondOtherPacificIslanderLiteral, value);
                }
            }
        }

        /// <summary>Mother's Race--First Other Literal</summary>
        [IJEField(60, 314, 30, "Mother's Race--First Other Literal", "MRACE22", 1)]
        public string MRACE22
        {
            get
            {
                return Get_MotherRace(NvssRace.FirstOtherRaceLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.FirstOtherRaceLiteral, value);
                }
            }
        }

        /// <summary>Mother's Race--Second Other Literal</summary>
        [IJEField(61, 344, 30, "Mother's Race--Second Other Literal", "MRACE23", 1)]
        public string MRACE23
        {
            get
            {
                return Get_MotherRace(NvssRace.SecondOtherRaceLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MotherRace(NvssRace.SecondOtherRaceLiteral, value);
                }
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

        /// <summary>Father's Education</summary>
        [IJEField(78, 422, 1, "Father's Education", "FEDUC", 1)]
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
        [IJEField(79, 423, 1, "Father's Education--Edit Flag", "FEDUC_BYPASS", 1)]
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
        [IJEField(80, 424, 1, "Father of Hispanic Origin?--Mexican", "FETHNIC1", 1)]
        public string FETHNIC1
        {
            get
            {
                string code = Get_MappingFHIRToIJE(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.FHIRToIJE, "FatherEthnicity1", "FETHNIC1");
                if (String.IsNullOrWhiteSpace(code))
                {
                    code = "U";
                }
                return code;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MappingIJEToFHIR(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.IJEToFHIR, "FETHNIC1", "FatherEthnicity1", value);
                }
            }
        }

        /// <summary>Father of Hispanic Origin?--Puerto Rican</summary>
        [IJEField(81, 425, 1, "Father of Hispanic Origin?--Puerto Rican", "FETHNIC2", 1)]
        public string FETHNIC2
        {
            get
            {
                string code = Get_MappingFHIRToIJE(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.FHIRToIJE, "FatherEthnicity2", "FETHNIC2");
                if (String.IsNullOrWhiteSpace(code))
                {
                    code = "U";
                }
                return code;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MappingIJEToFHIR(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.IJEToFHIR, "FETHNIC2", "FatherEthnicity2", value);
                }
            }
        }

        /// <summary>Father of Hispanic Origin?--Cuban</summary>
        [IJEField(82, 426, 1, "Father of Hispanic Origin?--Cuban", "FETHNIC3", 1)]
        public string FETHNIC3
        {
            get
            {
                string code = Get_MappingFHIRToIJE(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.FHIRToIJE, "FatherEthnicity3", "FETHNIC3");
                if (String.IsNullOrWhiteSpace(code))
                {
                    code = "U";
                }
                return code;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MappingIJEToFHIR(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.IJEToFHIR, "FETHNIC3", "FatherEthnicity3", value);
                }
            }
        }

        /// <summary>Father of Hispanic Origin?--Other</summary>
        [IJEField(83, 427, 1, "Father of Hispanic Origin?--Other", "FETHNIC4", 1)]
        public string FETHNIC4
        {
            get
            {
                string code = Get_MappingFHIRToIJE(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.FHIRToIJE, "FatherEthnicity4", "FETHNIC4");
                if (String.IsNullOrWhiteSpace(code))
                {
                    code = "U";
                }
                return code;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_MappingIJEToFHIR(VR.Mappings.ConceptMapHispanicNoUnknownVitalRecords.IJEToFHIR, "FETHNIC4", "FatherEthnicity4", value);
                }
            }
        }

        /// <summary>Father of Hispanic Origin?--Other Literal</summary>
        [IJEField(84, 428, 20, "Father of Hispanic Origin?--Other Literal", "FETHNIC5", 1)]
        public string FETHNIC5
        {
            get
            {
                var ethnicityLiteral = record.FatherEthnicityLiteral;
                if (!String.IsNullOrWhiteSpace(ethnicityLiteral))
                {
                    return Truncate(ethnicityLiteral, 20).Trim();
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    record.FatherEthnicityLiteral = value;
                }
            }
        }

        /// <summary>Father's Race--White</summary>
        [IJEField(85, 448, 1, "Father's Race--White", "FRACE1", 1)]
        public string FRACE1
        {
            get
            {
                return Get_FatherRace(NvssRace.White);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.White, value);
                }
            }
        }

        /// <summary>Father's Race--Black or African American</summary>
        [IJEField(86, 449, 1, "Father's Race--Black or African American", "FRACE2", 1)]
        public string FRACE2
        {
            get
            {
                return Get_FatherRace(NvssRace.BlackOrAfricanAmerican);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.BlackOrAfricanAmerican, value);
                }
            }
        }

        /// <summary>Father's Race--American Indian or Alaska Native</summary>
        [IJEField(87, 450, 1, "Father's Race--American Indian or Alaska Native", "FRACE3", 1)]
        public string FRACE3
        {
            get
            {
                return Get_FatherRace(NvssRace.AmericanIndianOrAlaskanNative);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.AmericanIndianOrAlaskanNative, value);
                }
            }
        }

        /// <summary>Father's Race--Asian Indian</summary>
        [IJEField(88, 451, 1, "Father's Race--Asian Indian", "FRACE4", 1)]
        public string FRACE4
        {
            get
            {
                return Get_FatherRace(NvssRace.AsianIndian);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.AsianIndian, value);
                }
            }
        }

        /// <summary>Father's Race--Chinese</summary>
        [IJEField(89, 452, 1, "Father's Race--Chinese", "FRACE5", 1)]
        public string FRACE5
        {
            get
            {
                return Get_FatherRace(NvssRace.Chinese);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.Chinese, value);
                }
            }
        }

        /// <summary>Father's Race--Filipino</summary>
        [IJEField(90, 453, 1, "Father's Race--Filipino", "FRACE6", 1)]
        public string FRACE6
        {
            get
            {
                return Get_FatherRace(NvssRace.Filipino);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.Filipino, value);
                }
            }
        }

        /// <summary>Father's Race--Japanese</summary>
        [IJEField(91, 454, 1, "Father's Race--Japanese", "FRACE7", 1)]
        public string FRACE7
        {
            get
            {
                return Get_FatherRace(NvssRace.Japanese);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.Japanese, value);
                }
            }
        }

        /// <summary>Father's Race--Korean</summary>
        [IJEField(92, 455, 1, "Father's Race--Korean", "FRACE8", 1)]
        public string FRACE8
        {
            get
            {
                return Get_FatherRace(NvssRace.Korean);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.Korean, value);
                }
            }
        }

        /// <summary>Father's Race--Vietnamese</summary>
        [IJEField(93, 456, 1, "Father's Race--Vietnamese", "FRACE9", 1)]
        public string FRACE9
        {
            get
            {
                return Get_FatherRace(NvssRace.Vietnamese);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.Vietnamese, value);
                }
            }
        }

        /// <summary>Father's Race--Other Asian</summary>
        [IJEField(94, 457, 1, "Father's Race--Other Asian", "FRACE10", 1)]
        public string FRACE10
        {
            get
            {
                return Get_FatherRace(NvssRace.OtherAsian);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.OtherAsian, value);
                }

            }
        }

        /// <summary>Father's Race--Native Hawaiian</summary>
        [IJEField(95, 458, 1, "Father's Race--Native Hawaiian", "FRACE11", 1)]
        public string FRACE11
        {
            get
            {
                return Get_FatherRace(NvssRace.NativeHawaiian);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.NativeHawaiian, value);
                }
            }
        }

        /// <summary>Father's Race--Guamanian or Chamorro</summary>
        [IJEField(96, 459, 1, "Father's Race--Guamanian or Chamorro", "FRACE12", 1)]
        public string FRACE12
        {
            get
            {
                return Get_FatherRace(NvssRace.GuamanianOrChamorro);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.GuamanianOrChamorro, value);
                }
            }
        }

        /// <summary>Father's Race--Samoan</summary>
        [IJEField(97, 460, 1, "Father's Race--Samoan", "FRACE13", 1)]
        public string FRACE13
        {
            get
            {
                return Get_FatherRace(NvssRace.Samoan);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.Samoan, value);
                }
            }
        }

        /// <summary>Father's Race--Other Pacific Islander</summary>
        [IJEField(98, 461, 1, "Father's Race--Other Pacific Islander", "FRACE14", 1)]
        public string FRACE14
        {
             get
            {
                return Get_FatherRace(NvssRace.OtherPacificIslander);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.OtherPacificIslander, value);
                }
            }
        }

        /// <summary>Father's Race--Other</summary>
        [IJEField(99, 462, 1, "Father's Race--Other", "FRACE15", 1)]
        public string FRACE15
        {
            get
            {
                return Get_FatherRace(NvssRace.OtherRace);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.OtherRace, value);
                }
            }
        }

        /// <summary>Father's Race--First American Indian or Alaska Native Literal</summary>
        [IJEField(100, 463, 30, "Father's Race--First American Indian or Alaska Native Literal", "FRACE16", 1)]
        public string FRACE16
        {
            get
            {
                return Get_FatherRace(NvssRace.FirstAmericanIndianOrAlaskanNativeLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.FirstAmericanIndianOrAlaskanNativeLiteral, value);
                }
            }
        }

        /// <summary>Father's Race--Second American Indian or Alaska Native Literal</summary>
        [IJEField(101, 493, 30, "Father's Race--Second American Indian or Alaska Native Literal", "FRACE17", 1)]
        public string FRACE17
        {
            get
            {
                return Get_FatherRace(NvssRace.SecondAmericanIndianOrAlaskanNativeLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.SecondAmericanIndianOrAlaskanNativeLiteral, value);
                }
            }
        }

        /// <summary>Father's Race--First Other Asian Literal</summary>
        [IJEField(102, 523, 30, "Father's Race--First Other Asian Literal", "FRACE18", 1)]
        public string FRACE18
        {
            get
            {
                return Get_FatherRace(NvssRace.FirstOtherAsianLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.FirstOtherAsianLiteral, value);
                }
            }
        }

        /// <summary>Father's Race--Second Other Asian Literal</summary>
        [IJEField(103, 553, 30, "Father's Race--Second Other Asian Literal", "FRACE19", 1)]
        public string FRACE19
        {
            get
            {
                return Get_FatherRace(NvssRace.SecondOtherAsianLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.SecondOtherAsianLiteral, value);
                }
            }
        }

        /// <summary>Father's Race--First Other Pacific Islander Literal</summary>
        [IJEField(104, 583, 30, "Father's Race--First Other Pacific Islander Literal", "FRACE20", 1)]
        public string FRACE20
        {
            get
            {
                return Get_FatherRace(NvssRace.FirstOtherPacificIslanderLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.FirstOtherPacificIslanderLiteral, value);
                }
            }
        }

        /// <summary>Father's Race--Second Other Pacific Islander Literal</summary>
        [IJEField(105, 613, 30, "Father's Race--Second Other Pacific Islander Literal", "FRACE21", 1)]
        public string FRACE21
        {
            get
            {
                return Get_FatherRace(NvssRace.SecondOtherPacificIslanderLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.SecondOtherPacificIslanderLiteral, value);
                }
            }
        }

        /// <summary>Father's Race--First Other Literal</summary>
        [IJEField(106, 643, 30, "Father's Race--First Other Literal", "FRACE22", 1)]
        public string FRACE22
        {
            get
            {
                return Get_FatherRace(NvssRace.FirstOtherRaceLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.FirstOtherRaceLiteral, value);
                }
            }
        }

        /// <summary>Father's Race--Second Other Literal</summary>
        [IJEField(107, 673, 30, "Father's Race--Second Other Literal", "FRACE23", 1)]
        public string FRACE23
        {
            get
            {
                return Get_FatherRace(NvssRace.SecondOtherRaceLiteral);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Set_FatherRace(NvssRace.SecondOtherRaceLiteral, value);
                }
            }
        }

        /// <summary>Father's Race Tabulation Variable 1E</summary>
        [IJEField(108, 703, 3, "Father's Race Tabulation Variable 1E", "FRACE1E", 1)]
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
        [IJEField(109, 706, 3, "Father's Race Tabulation Variable 2E", "FRACE2E", 1)]
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
        [IJEField(110, 709, 3, "Father's Race Tabulation Variable 3E", "FRACE3E", 1)]
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
        [IJEField(111, 712, 3, "Father's Race Tabulation Variable 4E", "FRACE4E", 1)]
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
        [IJEField(112, 715, 3, "Father's Race Tabulation Variable 5E", "FRACE5E", 1)]
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
        [IJEField(113, 718, 3, "Father's Race Tabulation Variable 6E", "FRACE6E", 1)]
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
        [IJEField(114, 721, 3, "Father's Race Tabulation Variable 7E", "FRACE7E", 1)]
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
        [IJEField(115, 724, 3, "Father's Race Tabulation Variable 8E", "FRACE8E", 1)]
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
        [IJEField(116, 727, 3, "Father's Race Tabulation Variable 16C", "FRACE16C", 1)]
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
        [IJEField(117, 730, 3, "Father's Race Tabulation Variable 17C", "FRACE17C", 1)]
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
        [IJEField(118, 733, 3, "Father's Race Tabulation Variable 18C", "FRACE18C", 1)]
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
        [IJEField(119, 736, 3, "Father's Race Tabulation Variable 19C", "FRACE19C", 1)]
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
        [IJEField(120, 739, 3, "Father's Race Tabulation Variable 20C", "FRACE20C", 1)]
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
        [IJEField(121, 742, 3, "Father's Race Tabulation Variable 21C", "FRACE21C", 1)]
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
        [IJEField(122, 745, 3, "Father's Race Tabulation Variable 22C", "FRACE22C", 1)]
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
        [IJEField(123, 748, 3, "Father's Race Tabulation Variable 23C", "FRACE23C", 1)]
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

        /// <summary>Attendant Title</summary>
        [IJEField(124, 751, 1, "Attendant Title", "ATTEND", 1)]
        public string ATTEND
        {
            get
            {
                var ret = record.AttendantTitleHelper;
                if (ret != null && Mappings.BirthAttendantTitles.FHIRToIJE.ContainsKey(ret))
                {
                    return Get_MappingFHIRToIJE(Mappings.BirthAttendantTitles.FHIRToIJE, "AttendantTitle", "ATTEND");
                }
                else  // If the return value is not a code, it is just an arbitrary string, so return it.
                {
                    return ret;
                }
            }
            set
            {
                if (Mappings.BirthAttendantTitles.IJEToFHIR.ContainsKey(value.Split(' ')[0]))
                {
                    Set_MappingIJEToFHIR(Mappings.BirthAttendantTitles.IJEToFHIR, "ATTEND", "AttendantTitle", value.Trim());
                }
                else  // If the value is not a valid code, it is just an arbitrary string.  The helper will deal with it.
                {
                    record.AttendantTitleHelper = value;
                }
            }
        }

        /// <summary>Mother Transferred?</summary>
        [IJEField(125, 752, 1, "Mother Transferred?", "TRAN", 1)]
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
        [IJEField(126, 753, 2, "Date of First Prenatal Care Visit--Month", "DOFP_MO", 1)]
        public string DOFP_MO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Date of First Prenatal Care Visit--Day</summary>
        [IJEField(127, 755, 2, "Date of First Prenatal Care Visit--Day", "DOFP_DY", 1)]
        public string DOFP_DY
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Date of First Prenatal Care Visit--Year</summary>
        [IJEField(128, 757, 4, "Date of First Prenatal Care Visit--Year", "DOFP_YR", 1)]
        public string DOFP_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Date of Last Prenatal Care Visit--Month(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(129, 761, 2, "Date of Last Prenatal Care Visit--Month(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "DOLP_MO", 1)]
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
        [IJEField(130, 763, 2, "Date of Last Prenatal Care Visit--Day(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "DOLP_DY", 1)]
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
        [IJEField(131, 765, 4, "Date of Last Prenatal Care Visit--Year(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "DOLP_YR", 1)]
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

        /// <summary>Total Number of Prenatal Care Visits</summary>
        [IJEField(132, 769, 2, "Total Number of Prenatal Care Visits", "NPREV", 1)]
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

        /// <summary>Total Number of Prenatal Care Visits--Edit Flag</summary>
        [IJEField(133, 771, 1, "Total Number of Prenatal Care Visits--Edit Flag", "NPREV_BYPASS", 1)]
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
        [IJEField(134, 772, 1, "Mother's Height--Feet", "HFT", 1)]
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
        [IJEField(135, 773, 2, "Mother's Height--Inches", "HIN", 1)]
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
        [IJEField(136, 775, 1, "Mother's Height--Edit Flag", "HGT_BYPASS", 1)]
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

        /// <summary>Mother's Prepregnancy Weight (in whole pounds)</summary>
        [IJEField(137, 776, 3, "Mother's Prepregnancy Weight (in whole pounds)", "PWGT", 1)]
        public string PWGT
        {
            get
            {
                return NumericAllowingUnknown_Get("PWGT", "MotherPrepregnancyWeight");
            }
            set
            {
                NumericAllowingUnknown_Set("PWGT", "MotherPrepregnancyWeight", value);
            }
        }

        /// <summary>Mother's Prepregnancy Weight--Edit Flag</summary>
        [IJEField(138, 779, 1, "Mother's Prepregnancy Weight--Edit Flag", "PWGT_BYPASS", 1)]
        public string PWGT_BYPASS
        {
            get
            {
                return record.MotherPrepregnancyWeightEditFlagHelper;
            }
            set
            {
                record.MotherPrepregnancyWeightEditFlagHelper = value;
            }
        }

        /// <summary>Mother's Weight at Delivery (in whole pounds)</summary>
        [IJEField(139, 780, 3, "Mother's Weight at Delivery (in whole pounds)", "DWGT", 1)]
        public string DWGT
        {
            get
            {
                return NumericAllowingUnknown_Get("DWGT", "MotherWeightAtDelivery");
            }
            set
            {
                NumericAllowingUnknown_Set("DWGT", "MotherWeightAtDelivery", value);
            }
        }

        /// <summary>Mother's Weight at Delivery--Edit Flag</summary>
        [IJEField(140, 783, 1, "Mother's Weight at Delivery--Edit Flag", "DWGT_BYPASS", 1)]
        public string DWGT_BYPASS
        {
            // FHIR and IJE codes match so no need for a mapping
            get
            {
                return record.MotherWeightAtDeliveryEditFlagHelper;
            }
            set
            {
                record.MotherWeightAtDeliveryEditFlagHelper = value;
            }
        }

        /// <summary>Did Mother get WIC Food for Herself?</summary>
        [IJEField(141, 784, 1, "Did Mother get WIC Food for Herself?", "WIC", 1)]
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
        [IJEField(142, 785, 2, "Previous Live Births Now Living", "PLBL", 1)]
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
        [IJEField(143, 787, 2, "Previous Live Births Now Dead", "PLBD", 1)]
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

        /// <summary>Previous Other Pregnancy Outcomes</summary>
        [IJEField(144, 789, 2, "Previous Other Pregnancy Outcomes", "POPO", 1)]
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
        [IJEField(145, 791, 2, "Date of Last Live Birth--Month", "MLLB", 1)]
        public string MLLB
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Date of Last Live Birth--Year</summary>
        [IJEField(146, 793, 4, "Date of Last Live Birth--Year", "YLLB", 1)]
        public string YLLB
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Date of Last Other Pregnancy Outcome--Month</summary>
        [IJEField(147, 797, 2, "Date of Last Other Pregnancy Outcome--Month", "MOPO", 1)]
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

        /// <summary>Date of Last Other Pregnancy Outcome--Year</summary>
        [IJEField(148, 799, 4, "Date of Last Other Pregnancy Outcome--Year", "YOPO", 1)]
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
        [IJEField(149, 803, 2, "Number of Cigarettes Smoked in 3 months prior to Pregnancy", "CIGPN", 1)]
        public string CIGPN
        {
            get
            {
                return NumericAllowingUnknown_Get("CIGPN", "CigarettesPerDayInThreeMonthsPriorToPregancy");
            }
            set
            {
                NumericAllowingUnknown_Set("CIGPN", "CigarettesPerDayInThreeMonthsPriorToPregancy", value);
            }
        }

        /// <summary>Number of Cigarettes Smoked in 1st 3 months</summary>
        [IJEField(150, 805, 2, "Number of Cigarettes Smoked in 1st 3 months", "CIGFN", 1)]
        public string CIGFN
        {
            get
            {
                return NumericAllowingUnknown_Get("CIGFN", "CigarettesPerDayInFirstTrimester");
            }
            set
            {
                NumericAllowingUnknown_Set("CIGFN", "CigarettesPerDayInFirstTrimester", value);
            }
        }

        /// <summary>Number of Cigarettes Smoked in 2nd 3 months</summary>
        [IJEField(151, 807, 2, "Number of Cigarettes Smoked in 2nd 3 months", "CIGSN", 1)]
        public string CIGSN
        {
            get
            {
                return NumericAllowingUnknown_Get("CIGSN", "CigarettesPerDayInSecondTrimester");
            }
            set
            {
                NumericAllowingUnknown_Set("CIGSN", "CigarettesPerDayInSecondTrimester", value);
            }
        }

        /// <summary>Number of Cigarettes Smoked in third or last trimester</summary>
        [IJEField(152, 809, 2, "Number of Cigarettes Smoked in third or last trimester", "CIGLN", 1)]
        public string CIGLN
        {
            get
            {
                return NumericAllowingUnknown_Get("CIGLN", "CigarettesPerDayInLastTrimester");
            }
            set
            {
                NumericAllowingUnknown_Set("CIGLN", "CigarettesPerDayInLastTrimester", value);
            }
        }

        /// <summary>Principal source of Payment for this delivery</summary>
        [IJEField(153, 811, 1, "Principal source of Payment for this delivery", "PAY", 1)]
        public string PAY
        {
            get
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
        [IJEField(154, 812, 4, "Date Last Normal Menses Began--Year", "DLMP_YR", 1)]
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
        [IJEField(155, 816, 2, "Date Last Normal Menses Began--Month", "DLMP_MO", 1)]
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
        [IJEField(156, 818, 2, "Date Last Normal Menses Began--Day", "DLMP_DY", 1)]
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

        /// <summary>Risk Factors--Prepregnancy Diabetes</summary>
        [IJEField(157, 820, 1, "Risk Factors--Prepregnancy Diabetes", "PDIAB", 1)]
        public string PDIAB
        {
            get => PresenceToIJE(record.PrepregnancyDiabetes, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.PrepregnancyDiabetes = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Gestational Diabetes</summary>
        [IJEField(158, 821, 1, "Risk Factors--Gestational Diabetes", "GDIAB", 1)]
        public string GDIAB
        {
            get => PresenceToIJE(record.GestationalDiabetes, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.GestationalDiabetes = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Prepregnancy Hypertension</summary>
        [IJEField(159, 822, 1, "Risk Factors--Prepregnancy Hypertension", "PHYPE", 1)]
        public string PHYPE
        {
            get => PresenceToIJE(record.PrepregnancyHypertension, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.PrepregnancyHypertension = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Gestational Hypertension  (SEE ADDITIONAL HYPERTENSION CATEGORY IN LOCATION 924)</summary>
        [IJEField(160, 823, 1, "Risk Factors--Gestational Hypertension  (SEE ADDITIONAL HYPERTENSION CATEGORY IN LOCATION 924)", "GHYPE", 1)]
        public string GHYPE
        {
            get => PresenceToIJE(record.GestationalHypertension, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.GestationalHypertension = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Previous Preterm Births</summary>
        [IJEField(161, 824, 1, "Risk Factors--Previous Preterm Births", "PPB", 1)]
        public string PPB
        {
            get => PresenceToIJE(record.PreviousPretermBirth, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.PreviousPretermBirth = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Poor Pregnancy Outcomes(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(162, 825, 1, "Risk Factors--Poor Pregnancy Outcomes(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "PPO", 1)]
        public string PPO
        {
            get => "U"; // Not present in FHIR
            set {}
        }

        /// <summary><html>Risk Factors--Vaginal Bleeding  <b>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</b></html></summary>
        [IJEField(163, 826, 1, "<html>Risk Factors--Vaginal Bleeding  <b>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</b></html>", "VB", 1)]
        public string VB
        {
            get => "U"; // Not present in FHIR
            set {}
        }

        /// <summary>Risk Factors--Infertility Treatment  (SEE ADDITIONAL SUBCATEGORIES IN LOCATIONS 925-926)</summary>
        [IJEField(164, 827, 1, "Risk Factors--Infertility Treatment  (SEE ADDITIONAL SUBCATEGORIES IN LOCATIONS 925-926)", "INFT", 1)]
        public string INFT
        {
            get => PresenceToIJE(record.InfertilityTreatment, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.InfertilityTreatment = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Previous Cesarean</summary>
        [IJEField(165, 828, 1, "Risk Factors--Previous Cesarean", "PCES", 1)]
        public string PCES
        {
            get => PresenceToIJE(record.PreviousCesarean, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.PreviousCesarean = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Number Previous Cesareans</summary>
        [IJEField(166, 829, 2, "Risk Factors--Number Previous Cesareans", "NPCES", 1)]
        public string NPCES
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Risk Factors--Number Previous Cesareans--Edit Flag</summary>
        [IJEField(167, 831, 1, "Risk Factors--Number Previous Cesareans--Edit Flag", "NPCES_BYPASS", 1)]
        public string NPCES_BYPASS
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Infections Present--Gonorrhea</summary>
        [IJEField(168, 832, 1, "Infections Present--Gonorrhea", "GON", 1)]
        public string GON
        {
            get => PresenceToIJE(record.Gonorrhea, record.NoInfectionsPresentDuringPregnancy);
            set => IJEToPresence(value, (v) => record.Gonorrhea = v, (v) => record.NoInfectionsPresentDuringPregnancy = v);
        }

        /// <summary>Infections Present--Syphilis</summary>
        [IJEField(169, 833, 1, "Infections Present--Syphilis", "SYPH", 1)]
        public string SYPH
        {
            get => PresenceToIJE(record.Syphilis, record.NoInfectionsPresentDuringPregnancy);
            set => IJEToPresence(value, (v) => record.Syphilis = v, (v) => record.NoInfectionsPresentDuringPregnancy = v);
        }

        /// <summary><html>Infections Present--Herpes Simplex (HSV) <b> (NCHS DELETED THIS ITEM EFFECTIVE 2011)</b></html></summary>
        [IJEField(170, 834, 1, "<html>Infections Present--Herpes Simplex (HSV) <b> (NCHS DELETED THIS ITEM EFFECTIVE 2011)</b></html>", "HSV", 1)]
        public string HSV
        {
            get => PresenceToIJE(record.GenitalHerpesSimplex, record.NoInfectionsPresentDuringPregnancy);
            set => IJEToPresence(value, (v) => record.GenitalHerpesSimplex = v, (v) => record.NoInfectionsPresentDuringPregnancy = v);
        }

        /// <summary>Infections Present--Chlamydia</summary>
        [IJEField(171, 835, 1, "Infections Present--Chlamydia", "CHAM", 1)]
        public string CHAM
        {
            get => PresenceToIJE(record.Chlamydia, record.NoInfectionsPresentDuringPregnancy);
            set => IJEToPresence(value, (v) => record.Chlamydia = v, (v) => record.NoInfectionsPresentDuringPregnancy = v);
        }

        /// <summary>Infections Present--Hepatitis B</summary>
        [IJEField(172, 836, 1, "Infections Present--Hepatitis B", "HEPB", 1)]
        public string HEPB
        {
            get => PresenceToIJE(record.HepatitisB, record.NoInfectionsPresentDuringPregnancy);
            set => IJEToPresence(value, (v) => record.HepatitisB = v, (v) => record.NoInfectionsPresentDuringPregnancy = v);
        }

        /// <summary>Infections Present--Hepatitis C</summary>
        [IJEField(173, 837, 1, "Infections Present--Hepatitis C", "HEPC", 1)]
        public string HEPC
        {
            get => PresenceToIJE(record.HepatitisC, record.NoInfectionsPresentDuringPregnancy);
            set => IJEToPresence(value, (v) => record.HepatitisC = v, (v) => record.NoInfectionsPresentDuringPregnancy = v);
        }

        /// <summary>Obstetric Procedures--Cervical Cerclage(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(174, 838, 1, "Obstetric Procedures--Cervical Cerclage(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "CERV", 1)]
        public string CERV
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Obstetric Procedures--Tocolysis(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(175, 839, 1, "Obstetric Procedures--Tocolysis(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "TOC", 1)]
        public string TOC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Obstetric Procedures--Successful External Cephalic Version</summary>
        [IJEField(176, 840, 1, "Obstetric Procedures--Successful External Cephalic Version", "ECVS", 1)]
        public string ECVS
        {
            get => PresenceToIJE(record.SuccessfulExternalCephalicVersion, record.NoObstetricProcedures);
            set => IJEToPresence(value, (v) => record.SuccessfulExternalCephalicVersion = v, (v) => record.NoObstetricProcedures = v);
        }

        /// <summary>Obstetric Procedures--Failed External Cephalic Version</summary>
        [IJEField(177, 841, 1, "Obstetric Procedures--Failed External Cephalic Version", "ECVF", 1)]
        public string ECVF
        {
            get => PresenceToIJE(record.UnsuccessfulExternalCephalicVersion, record.NoObstetricProcedures);
            set => IJEToPresence(value, (v) => record.UnsuccessfulExternalCephalicVersion = v, (v) => record.NoObstetricProcedures = v);
        }

        /// <summary>Onset of Labor--Premature Rupture of Membranes(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(178, 842, 1, "Onset of Labor--Premature Rupture of Membranes(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "PROM", 1)]
        public string PROM
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Onset of Labor--Precipitous Labor(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(179, 843, 1, "Onset of Labor--Precipitous Labor(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "PRIC", 1)]
        public string PRIC
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Onset of Labor--Prolonged Labor(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(180, 844, 1, "Onset of Labor--Prolonged Labor(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "PROL", 1)]
        public string PROL
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Characteristics of Labor &amp; Delivery--Induction of Labor</summary>
        [IJEField(181, 845, 1, "Characteristics of Labor & Delivery--Induction of Labor", "INDL", 1)]
        public string INDL
        {
            get => PresenceToIJE(record.InductionOfLabor, record.NoCharacteristicsOfLaborAndDelivery);
            set => IJEToPresence(value, (v) => record.InductionOfLabor = v, (v) => record.NoCharacteristicsOfLaborAndDelivery = v);
        }

        /// <summary>Characteristics of Labor &amp; Delivery--Augmentation of Labor</summary>
        [IJEField(182, 846, 1, "Characteristics of Labor & Delivery--Augmentation of Labor", "AUGL", 1)]
        public string AUGL
        {
            get => PresenceToIJE(record.AugmentationOfLabor, record.NoCharacteristicsOfLaborAndDelivery);
            set => IJEToPresence(value, (v) => record.AugmentationOfLabor = v, (v) => record.NoCharacteristicsOfLaborAndDelivery = v);
        }

        /// <summary><html>Characteristics of Labor &amp; Delivery--Non-vertex Presentation <b>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</b></html></summary>
        [IJEField(183, 847, 1, "<html>Characteristics of Labor & Delivery--Non-vertex Presentation <b>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</b></html>", "NVPR", 1)]
        public string NVPR
        {
            get => "U"; // Not present in FHIR
            set {}
        }

        /// <summary>Characteristics of Labor &amp; Delivery--Steroids</summary>
        [IJEField(184, 848, 1, "Characteristics of Labor & Delivery--Steroids", "STER", 1)]
        public string STER
        {
            get => PresenceToIJE(record.AdministrationOfSteroidsForFetalLungMaturation, record.NoCharacteristicsOfLaborAndDelivery);
            set => IJEToPresence(value, (v) => record.AdministrationOfSteroidsForFetalLungMaturation = v, (v) => record.NoCharacteristicsOfLaborAndDelivery = v);
        }

        /// <summary>Characteristics of Labor &amp; Delivery--Antibiotics</summary>
        [IJEField(185, 849, 1, "Characteristics of Labor & Delivery--Antibiotics", "ANTB", 1)]
        public string ANTB
        {
            get => PresenceToIJE(record.AntibioticsAdministeredDuringLabor, record.NoCharacteristicsOfLaborAndDelivery);
            set => IJEToPresence(value, (v) => record.AntibioticsAdministeredDuringLabor = v, (v) => record.NoCharacteristicsOfLaborAndDelivery = v);
        }

        /// <summary>Characteristics of Labor &amp; Delivery--Chorioamnionitis</summary>
        [IJEField(186, 850, 1, "Characteristics of Labor & Delivery--Chorioamnionitis", "CHOR", 1)]
        public string CHOR
        {
            get => PresenceToIJE(record.Chorioamnionitis, record.NoCharacteristicsOfLaborAndDelivery);
            set => IJEToPresence(value, (v) => record.Chorioamnionitis = v, (v) => record.NoCharacteristicsOfLaborAndDelivery = v);
        }

        /// <summary>Characteristics of Labor &amp; Delivery--Meconium Staining(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(187, 851, 1, "Characteristics of Labor & Delivery--Meconium Staining(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "MECS", 1)]
        public string MECS
        {
            get => "U"; // Not present in FHIR
            set {}
        }

        /// <summary>Characteristics of Labor &amp; Delivery--Fetal Intolerance(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(188, 852, 1, "Characteristics of Labor & Delivery--Fetal Intolerance(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "FINT", 1)]
        public string FINT
        {
            get => "U"; // Not present in FHIR
            set {}
        }

        /// <summary>Characteristics of Labor &amp; Delivery--Anesthesia</summary>
        [IJEField(189, 853, 1, "Characteristics of Labor & Delivery--Anesthesia", "ESAN", 1)]
        public string ESAN
        {
            get => PresenceToIJE(record.EpiduralOrSpinalAnesthesia, record.NoCharacteristicsOfLaborAndDelivery);
            set => IJEToPresence(value, (v) => record.EpiduralOrSpinalAnesthesia = v, (v) => record.NoCharacteristicsOfLaborAndDelivery = v);
        }

        /// <summary><html>Method of Delivery--Attempted Forceps <b>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</b></html></summary>
        [IJEField(190, 854, 1, "<html>Method of Delivery--Attempted Forceps <b>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</b></html>", "ATTF", 1)]
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

        /// <summary><html>Method of Delivery--Attempted Vacuum <b>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</b></html></summary>
        [IJEField(191, 855, 1, "<html>Method of Delivery--Attempted Vacuum <b>(NCHS DELETED THIS ITEM EFFECTIVE 2011)</b></html>", "ATTV", 1)]
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
        [IJEField(192, 856, 1, "Method of Delivery--Fetal Presentation", "PRES", 1)]
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
        [IJEField(193, 857, 1, "Method of Delivery--Route and Method of Delivery", "ROUT", 1)]
        public string ROUT
        {
            get
            {
                if (record.UnknownFinalRouteAndMethodOfDelivery)
                {
                    return "9";
                }
                return Get_MappingFHIRToIJE(Mappings.DeliveryRoutes.FHIRToIJE, "FinalRouteAndMethodOfDelivery", "ROUT");
            }
            set
            {
                if (value == "9")
                {
                    record.UnknownFinalRouteAndMethodOfDelivery = true;
                }
                else if (String.IsNullOrEmpty(value))
                {
                    record.FinalRouteAndMethodOfDeliveryHelper = null;
                }
                else
                {
                    Set_MappingIJEToFHIR(Mappings.DeliveryRoutes.IJEToFHIR, "ROUT", "FinalRouteAndMethodOfDelivery", value.Trim());
                }
            }
        }

        /// <summary>Method of Delivery--Trial of Labor Attempted</summary>
        [IJEField(194, 858, 1, "Method of Delivery--Trial of Labor Attempted", "TLAB", 1)]
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

        /// <summary>Maternal Morbidity--Maternal Transfusion</summary>
        [IJEField(195, 859, 1, "Maternal Morbidity--Maternal Transfusion", "MTR", 1)]
        public string MTR
        {
            get => PresenceToIJE(record.MaternalTransfusion, record.NoMaternalMorbidities);
            set => IJEToPresence(value, (v) => record.MaternalTransfusion = v, (v) => record.NoMaternalMorbidities = v);
        }

        /// <summary>Maternal Morbidity--Perineal Laceration</summary>
        [IJEField(196, 860, 1, "Maternal Morbidity--Perineal Laceration", "PLAC", 1)]
        public string PLAC
        {
            get => PresenceToIJE(record.PerinealLaceration, record.NoMaternalMorbidities);
            set => IJEToPresence(value, (v) => record.PerinealLaceration = v, (v) => record.NoMaternalMorbidities = v);
        }

        /// <summary>Maternal Morbidity--Ruptured Uterus</summary>
        [IJEField(197, 861, 1, "Maternal Morbidity--Ruptured Uterus", "RUT", 1)]
        public string RUT
        {
            get => PresenceToIJE(record.RupturedUterus, record.NoMaternalMorbidities);
            set => IJEToPresence(value, (v) => record.RupturedUterus = v, (v) => record.NoMaternalMorbidities = v);
        }

        /// <summary>Maternal Morbidity--Unplanned Hysterectomy</summary>
        [IJEField(198, 862, 1, "Maternal Morbidity--Unplanned Hysterectomy", "UHYS", 1)]
        public string UHYS
        {
            get => PresenceToIJE(record.UnplannedHysterectomy, record.NoMaternalMorbidities);
            set => IJEToPresence(value, (v) => record.UnplannedHysterectomy = v, (v) => record.NoMaternalMorbidities = v);
        }

        /// <summary>Maternal Morbidity--Admit to Intensive Care</summary>
        [IJEField(199, 863, 1, "Maternal Morbidity--Admit to Intensive Care", "AINT", 1)]
        public string AINT
        {
            get => PresenceToIJE(record.ICUAdmission, record.NoMaternalMorbidities);
            set => IJEToPresence(value, (v) => record.ICUAdmission = v, (v) => record.NoMaternalMorbidities = v);
        }

        /// <summary>Maternal Morbidity--Unplanned Operation(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(200, 864, 1, "Maternal Morbidity--Unplanned Operation(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "UOPR", 1)]
        public string UOPR
        {
            get => "U"; // Not present in FHIR
            set {}
        }

        /// <summary>Birthweight in grams</summary>
        [IJEField(201, 865, 4, "Birthweight in grams", "BWG", 1)]
        public string BWG
        {
            get
            {
                return NumericAllowingUnknown_Get("BWG", "BirthWeight");
            }
            set
            {
                NumericAllowingUnknown_Set("BWG", "BirthWeight", value);
            }
        }

        /// <summary>Birthweight--Edit Flag</summary>
        [IJEField(202, 869, 1, "Birthweight--Edit Flag", "BW_BYPASS", 1)]
        public string BW_BYPASS
        {
            // TODO implement mapping once BFDR/Mapping.cs has been generated
            get
            {
                return record.BirthWeightEditFlagHelper;
            }
            set
            {
                record.BirthWeightEditFlagHelper = value;
            }
        }

        /// <summary>Obstetric Estimation of Gestation</summary>
        [IJEField(203, 870, 2, "Obstetric Estimation of Gestation", "OWGEST", 1)]
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
        [IJEField(204, 872, 1, "Obstetric Estimation of Gestation--Edit Flag", "OWGEST_BYPASS", 1)]
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

        /// <summary>Apgar Score at 5 Minutes</summary>
        [IJEField(205, 873, 2, "Apgar Score at 5 Minutes", "APGAR5", 1)]
        public string APGAR5
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Apgar Score at 10 Minutes</summary>
        [IJEField(206, 875, 2, "Apgar Score at 10 Minutes", "APGAR10", 1)]
        public string APGAR10
        {
            get
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
        [IJEField(207, 877, 2, "Plurality", "PLUR", 1)]
        public string PLUR
        {
            get
            {
                return NumericAllowingUnknown_Get("PLUR", "Plurality");
            }
            set
            {
                NumericAllowingUnknown_Set("PLUR", "Plurality", value);
            }
        }

        /// <summary>Set Order</summary>
        [IJEField(208, 879, 2, "SetOrder", "SORD", 1)]
        public string SORD
        {
            get
            {
                return NumericAllowingUnknown_Get("SORD", "SetOrder");
            }
            set
            {
                NumericAllowingUnknown_Set("SORD", "SetOrder", value);
            }
        }

        /// <summary>Number of Live Born</summary>
        [IJEField(209, 881, 2, "Number of Live Born", "LIVEB", 1)]
        public string LIVEB
        {
            get
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
        [IJEField(210, 883, 6, "Matching Number", "MATCH", 1)]
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
        [IJEField(211, 889, 1, "Plurality--Edit Flag", "PLUR_BYPASS", 1)]
        public string PLUR_BYPASS
        {
            get
            {
                return Get_MappingFHIRToIJE(VR.Mappings.ConceptMapPluralityEditFlagsVitalRecords.FHIRToIJE, "PluralityEditFlag", "PLUR_BYPASS").PadLeft(1, ' ');
            }
            set
            {
                Set_MappingIJEToFHIR(VR.Mappings.ConceptMapPluralityEditFlagsVitalRecords.IJEToFHIR, "PLUR_BYPASS", "PluralityEditFlag", value);
            }
        }

        /// <summary>Abnormal Conditions of the Newborn--Assisted Ventilation</summary>
        [IJEField(212, 890, 1, "Abnormal Conditions of the Newborn--Assisted Ventilation", "AVEN1", 1)]
        public string AVEN1
        {
            get => PresenceToIJE(record.AssistedVentilationFollowingDelivery, record.NoSpecifiedAbnormalConditionsOfNewborn);
            set => IJEToPresence(value, (v) => record.AssistedVentilationFollowingDelivery = v, (v) => record.NoSpecifiedAbnormalConditionsOfNewborn = v);
        }

        /// <summary>Abnormal Conditions of the Newborn--Assisted Ventilation > 6 hours</summary>
        [IJEField(213, 891, 1, "Abnormal Conditions of the Newborn--Assisted Ventilation > 6 hours", "AVEN6", 1)]
        public string AVEN6
        {
            get => PresenceToIJE(record.AssistedVentilationMoreThanSixHours, record.NoSpecifiedAbnormalConditionsOfNewborn);
            set => IJEToPresence(value, (v) => record.AssistedVentilationMoreThanSixHours = v, (v) => record.NoSpecifiedAbnormalConditionsOfNewborn = v);
        }

        /// <summary>Abnormal Conditions of the Newborn--Admission to NICU</summary>
        [IJEField(214, 892, 1, "Abnormal Conditions of the Newborn--Admission to NICU", "NICU", 1)]
        public string NICU
        {
            get => PresenceToIJE(record.NICUAdmission, record.NoSpecifiedAbnormalConditionsOfNewborn);
            set => IJEToPresence(value, (v) => record.NICUAdmission = v, (v) => record.NoSpecifiedAbnormalConditionsOfNewborn = v);
        }

        /// <summary>Abnormal Conditions of the Newborn--Surfactant Replacement</summary>
        [IJEField(215, 893, 1, "Abnormal Conditions of the Newborn--Surfactant Replacement", "SURF", 1)]
        public string SURF
        {
            get => PresenceToIJE(record.SurfactantReplacementTherapy, record.NoSpecifiedAbnormalConditionsOfNewborn);
            set => IJEToPresence(value, (v) => record.SurfactantReplacementTherapy = v, (v) => record.NoSpecifiedAbnormalConditionsOfNewborn = v);
        }

        /// <summary>Abnormal Conditions of the Newborn--Antibiotics</summary>
        [IJEField(216, 894, 1, "Abnormal Conditions of the Newborn--Antibiotics", "ANTI", 1)]
        public string ANTI
        {
            get => PresenceToIJE(record.AntibioticForSuspectedNeonatalSepsis, record.NoSpecifiedAbnormalConditionsOfNewborn);
            set => IJEToPresence(value, (v) => record.AntibioticForSuspectedNeonatalSepsis = v, (v) => record.NoSpecifiedAbnormalConditionsOfNewborn = v);
        }

        /// <summary>Abnormal Conditions of the Newborn--Seizures</summary>
        [IJEField(217, 895, 1, "Abnormal Conditions of the Newborn--Seizures", "SEIZ", 1)]
        public string SEIZ
        {
            get => PresenceToIJE(record.Seizure, record.NoSpecifiedAbnormalConditionsOfNewborn);
            set => IJEToPresence(value, (v) => record.Seizure = v, (v) => record.NoSpecifiedAbnormalConditionsOfNewborn = v);
        }

        /// <summary>Abnormal Conditions of the Newborn--Birth Injury(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)</summary>
        [IJEField(218, 896, 1, "Abnormal Conditions of the Newborn--Birth Injury(NCHS DELETED THIS ITEM EFFECTIVE 2014/2015)", "BINJ", 1)]
        public string BINJ
        {
            get => "U"; // Not present in FHIR
            set {}
        }

        /// <summary>Congenital Anomalies of the Newborn--Anencephaly</summary>
        [IJEField(219, 897, 1, "Congenital Anomalies of the Newborn--Anencephaly", "ANEN", 1)]
        public string ANEN
        {
            get => PresenceToIJE(record.Anencephaly, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.Anencephaly = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Meningomyelocele/Spina Bifida</summary>
        [IJEField(220, 898, 1, "Congenital Anomalies of the Newborn--Meningomyelocele/Spina Bifida", "MNSB", 1)]
        public string MNSB
        {
            get => PresenceToIJE(record.Meningomyelocele, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.Meningomyelocele = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Cyanotic congenital heart disease</summary>
        [IJEField(221, 899, 1, "Congenital Anomalies of the Newborn--Cyanotic congenital heart disease", "CCHD", 1)]
        public string CCHD
        {
            get => PresenceToIJE(record.CyanoticCongenitalHeartDisease, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.CyanoticCongenitalHeartDisease = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Congenital diaphragmatic hernia</summary>
        [IJEField(222, 900, 1, "Congenital Anomalies of the Newborn--Congenital diaphragmatic hernia", "CDH", 1)]
        public string CDH
        {
            get => PresenceToIJE(record.CongenitalDiaphragmaticHernia, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.CongenitalDiaphragmaticHernia = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Omphalocele</summary>
        [IJEField(223, 901, 1, "Congenital Anomalies of the Newborn--Omphalocele", "OMPH", 1)]
        public string OMPH
        {
            get => PresenceToIJE(record.Omphalocele, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.Omphalocele = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Gastroschisis</summary>
        [IJEField(224, 902, 1, "Congenital Anomalies of the Newborn--Gastroschisis", "GAST", 1)]
        public string GAST
        {
            get => PresenceToIJE(record.Gastroschisis, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.Gastroschisis = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Limb Reduction Defect</summary>
        [IJEField(225, 903, 1, "Congenital Anomalies of the Newborn--Limb Reduction Defect", "LIMB", 1)]
        public string LIMB
        {
            get => PresenceToIJE(record.LimbReductionDefect, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.LimbReductionDefect = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Cleft Lip with or without Cleft Palate</summary>
        [IJEField(226, 904, 1, "Congenital Anomalies of the Newborn--Cleft Lip with or without Cleft Palate", "CL", 1)]
        public string CL
        {
            get => PresenceToIJE(record.CleftLipWithOrWithoutCleftPalate, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.CleftLipWithOrWithoutCleftPalate = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Cleft Palate Alone</summary>
        [IJEField(227, 905, 1, "Congenital Anomalies of the Newborn--Cleft Palate Alone", "CP", 1)]
        public string CP
        {
            get => PresenceToIJE(record.CleftPalateAlone, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.CleftPalateAlone = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Down Syndrome</summary>
        [IJEField(228, 906, 1, "Congenital Anomalies of the Newborn--Down Syndrome", "DOWT", 1)]
        public string DOWT
        {
            get => PresenceToIJE(record.DownSyndrome, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.DownSyndrome = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Suspected Chromosomal disorder</summary>
        [IJEField(229, 907, 1, "Congenital Anomalies of the Newborn--Suspected Chromosomal disorder", "CDIT", 1)]
        public string CDIT
        {
            get => PresenceToIJE(record.SuspectedChromosomalDisorder, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.SuspectedChromosomalDisorder = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Congenital Anomalies of the Newborn--Hypospadias</summary>
        [IJEField(230, 908, 1, "Congenital Anomalies of the Newborn--Hypospadias", "HYPO", 1)]
        public string HYPO
        {
            get => PresenceToIJE(record.Hypospadias, record.NoCongenitalAnomaliesOfTheNewborn);
            set => IJEToPresence(value, (v) => record.Hypospadias = v, (v) => record.NoCongenitalAnomaliesOfTheNewborn = v);
        }

        /// <summary>Was Infant Transferred Within 24 Hours of Delivery?</summary>
        [IJEField(231, 909, 1, "Was Infant Transferred Within 24 Hours of Delivery?", "ITRAN", 1)]
        public string ITRAN
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Is Infant Living at Time of Report?</summary>
        [IJEField(232, 910, 1, "Is Infant Living at Time of Report?", "ILIV", 1)]
        public string ILIV
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Is Infant Being Breastfed?  (RECOMMENDED CHANGE TO "AT DISCHARGE" EFFECTIVE 2004)</summary>
        [IJEField(233, 911, 1, "Is Infant Being Breastfed?  (RECOMMENDED CHANGE TO \"AT DISCHARGE\" EFFECTIVE 2004)", "BFED", 1)]
        public string BFED
        {
            get
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
        [IJEField(234, 912, 4, "NCHS USE ONLY: Receipt date -- Year", "R_YR", 1)]
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
        [IJEField(235, 916, 2, "NCHS USE ONLY: Receipt date -- Month", "R_MO", 1)]
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
        [IJEField(236, 918, 2, "NCHS USE ONLY: Receipt date -- Day", "R_DY", 1)]
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
        [IJEField(237, 920, 2, "Mother's Reported Age", "MAGER", 1)]
        public string MAGER
        {
            get
            {
                return NumericAllowingUnknown_Get("MAGER", "MotherReportedAgeAtDelivery");
            }
            set
            {
                NumericAllowingUnknown_Set("MAGER", "MotherReportedAgeAtDelivery", value);
            }
        }

        /// <summary>Father's Reported Age</summary>
        [IJEField(238, 922, 2, "Father's Reported Age", "FAGER", 1)]
        public string FAGER
        {
            get
            {
                return NumericAllowingUnknown_Get("FAGER", "FatherReportedAgeAtDelivery");
            }
            set
            {
                NumericAllowingUnknown_Set("FAGER", "FatherReportedAgeAtDelivery", value);
            }
        }

        /// <summary>Risk Factors--Hypertension Eclampsia   (RECOMMENDED ADDITION EFFECTIVE 2004)</summary>
        [IJEField(239, 924, 1, "Risk Factors--Hypertension Eclampsia   (RECOMMENDED ADDITION EFFECTIVE 2004)", "EHYPE", 1)]
        public string EHYPE
        {
            get => PresenceToIJE(record.EclampsiaHypertension, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.EclampsiaHypertension = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Infertility: Fertility Enhancing Drugs  (RECOMMENDED ADDITION EFFECTIVE 2004)</summary>
        [IJEField(240, 925, 1, "Risk Factors--Infertility: Fertility Enhancing Drugs  (RECOMMENDED ADDITION EFFECTIVE 2004)", "INFT_DRG", 1)]
        public string INFT_DRG
        {
            get => PresenceToIJE(record.ArtificialInsemination, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.ArtificialInsemination = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>Risk Factors--Infertility: Asst. Rep. Technology  (RECOMMENDED ADDITION EFFECTIVE 2004)</summary>
        [IJEField(241, 926, 1, "Risk Factors--Infertility: Asst. Rep. Technology  (RECOMMENDED ADDITION EFFECTIVE 2004)", "INFT_ART", 1)]
        public string INFT_ART
        {
            get => PresenceToIJE(record.AssistedFertilization, record.NoPregnancyRiskFactors);
            set => IJEToPresence(value, (v) => record.AssistedFertilization = v, (v) => record.NoPregnancyRiskFactors = v);
        }

        /// <summary>FILLER 1</summary>
        [IJEField(242, 927, 17, "FILLER 1", "", 1)]
        public string FILLER1
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Date of Registration--Year</summary>
        [IJEField(243, 944, 4, "Date of Registration--Year", "DOR_YR", 1)]
        public string DOR_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Date of Registration--Month</summary>
        [IJEField(244, 948, 2, "Date of Registration--Month", "DOR_MO", 1)]
        public string DOR_MO
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Date of Registration--Day</summary>
        [IJEField(245, 950, 2, "Date of Registration--Day", "DOR_DY", 1)]
        public string DOR_DY
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Abnormal Conditions of the Newborn--Microcephaly</summary>
        [IJEField(246, 952, 49, "Abnormal Conditions of the Newborn--Microcephaly", "MCPH", 1)]
        public string MCPH
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Child's First Name</summary>
        [IJEField(247, 1001, 50, "Child's First Name", "KIDFNAME", 1)]
        public string KIDFNAME
        {
            get
            {
                string[] names = record.ChildGivenNames;
                if (names.Length > 0)
                {
                    return Truncate(names[0], 50).PadRight(50, ' ');
                }
                return new string(' ', 50);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    record.ChildGivenNames = new string[] { value.Trim() };
                }
            }
        }

        /// <summary>Child's Middle Name</summary>
        [IJEField(248, 1051, 50, "Child's Middle Name", "KIDMNAME", 1)]
        public string KIDMNAME
        {
            get
            {
                string[] names = record.ChildGivenNames;
                if (names.Length > 1)
                {
                    return Truncate(names[1], 50).PadRight(50, ' ');
                }
                return " ";
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (String.IsNullOrWhiteSpace(KIDFNAME)) throw new ArgumentException("Middle name cannot be set before first name");
                    if (!String.IsNullOrWhiteSpace(value))
                    {
                        if (record.ChildGivenNames != null)
                        {
                            List<string> names = record.ChildGivenNames.ToList();
                            if (names.Count() > 1) names[1] = value.Trim(); else names.Add(value.Trim());
                            record.ChildGivenNames = names.ToArray();
                        }
                    }
                }
            }
        }

        /// <summary>Child's Last Name</summary>
        [IJEField(249, 1101, 50, "Child's Last Name", "KIDLNAME", 1)]
        public string KIDLNAME
        {
            get
            {
                return LeftJustified_Get("KIDLNAME", "ChildFamilyName");
            }
            set
            {
                LeftJustified_Set("KIDLNAME", "ChildFamilyName", value);
            }
        }

        /// <summary>Child's Surname Suffix (moved from end)</summary>
        [IJEField(250, 1151, 7, "Child's Surname Suffix (moved from end)", "KIDSUFFX", 1)]
        public string KIDSUFFX
        {
            get
            {
                return LeftJustified_Get("KIDSUFFX", "ChildSuffix");
            }
            set
            {
                LeftJustified_Set("KIDSUFFX", "ChildSuffix", value);
            }
        }

        /// <summary>County of Birth (Literal)</summary>
        [IJEField(251, 1158, 25, "County of Birth (Literal)", "BIRTH_CO", 1)]
        public string BIRTH_CO
        {
            get
            {
                return Dictionary_Geo_Get("BIRTH_CO", "PlaceOfBirth", "address", "county", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Set("BIRTH_CO", "PlaceOfBirth", "addressCounty", value);
                }
            }
        }

        /// <summary>City/town/place of birth (Literal)</summary>
        [IJEField(252, 1183, 50, "City/town/place of birth (Literal)", "BRTHCITY", 1)]
        public string BRTHCITY
        {
            get
            {
                return Dictionary_Geo_Get("BRTHCITY", "PlaceOfBirth", "address", "city", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Set("BRTHCITY", "PlaceOfBirth", "addressCity", value);
                }
            }
        }

        /// <summary>Name of Facility of Birth</summary>
        [IJEField(253, 1233, 50, "Name of Facility of Birth", "HOSP", 1)]
        public string HOSP
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Mother's First Name</summary>
        [IJEField(254, 1283, 50, "Mother's First Name", "MOMFNAME", 1)]
        public string MOMFNAME
        {
            get
            {
                string[] names = record.MotherGivenNames;
                if (names.Length > 0)
                {
                    return Truncate(names[0], 50).PadRight(50, ' ');
                }
                return new string(' ', 50);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    record.MotherGivenNames = new string[] { value.Trim() };
                }
            }
        }

        /// <summary>Mother's Middle Name</summary>
        [IJEField(255, 1333, 50, "Mother's Middle Name", "MOMMIDDL", 1)]
        public string MOMMIDDL
        {
            get
            {
                string[] names = record.MotherGivenNames;
                if (names.Length > 1)
                {
                    return Truncate(names[1], 50).PadRight(50, ' ');
                }
                return " ";
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (String.IsNullOrWhiteSpace(MOMFNAME)) throw new ArgumentException("Middle name cannot be set before first name");
                    if (!String.IsNullOrWhiteSpace(value))
                    {
                        if (record.MotherGivenNames != null)
                        {
                            List<string> names = record.MotherGivenNames.ToList();
                            if (names.Count() > 1) names[1] = value.Trim(); else names.Add(value.Trim());
                            record.MotherGivenNames = names.ToArray();
                        }
                    }
                }
            }
        }

        /// <summary>Mother's Last Name</summary>
        [IJEField(256, 1383, 50, "Mother's Last Name", "MOMLNAME", 1)]
        public string MOMLNAME
        {
            get
            {
                return LeftJustified_Get("MOMLNAME", "MotherFamilyName");            }
            set
            {
                LeftJustified_Set("MOMLNAME", "MotherFamilyName", value);
            }
        }

        /// <summary>Mother's Surname Suffix</summary>
        [IJEField(257, 1433, 7, "Mother's Surname Suffix", "MOMSUFFX", 1)]
        public string MOMSUFFX
        {
            get
            {
                return LeftJustified_Get("MOMSUFFX", "MotherSuffix");
            }
            set
            {
                LeftJustified_Set("MOMSUFFX", "MotherSuffix", value);
            }
        }

        /// <summary>Mother's First Maiden Name</summary>
        [IJEField(258, 1440, 50, "Mother's First Maiden Name", "MOMFMNME", 1)]
        public string MOMFMNME
        {
            get
            {
                string[] names = record.MotherMaidenGivenNames;
                if (names.Length > 0)
                {
                    return Truncate(names[0], 50).PadRight(50, ' ');
                }
                return new string(' ', 50);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    record.MotherMaidenGivenNames = new string[] { value.Trim() };
                }
            }
        }

        /// <summary>Mother's Middle Maiden Name</summary>
        [IJEField(259, 1490, 50, "Mother's Middle Maiden Name", "MOMMMID", 1)]
        public string MOMMMID
        {
            get
            {
                string[] names = record.MotherMaidenGivenNames;
                if (names.Length > 1)
                {
                    return Truncate(names[1], 50).PadRight(50, ' ');
                }
                return " ";
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (String.IsNullOrWhiteSpace(MOMFMNME)) throw new ArgumentException("Middle name cannot be set before first name");
                    if (!String.IsNullOrWhiteSpace(value))
                    {
                        if (record.MotherMaidenGivenNames != null)
                        {
                            List<string> names = record.MotherMaidenGivenNames.ToList();
                            if (names.Count() > 1) names[1] = value.Trim(); else names.Add(value.Trim());
                            record.MotherMaidenGivenNames = names.ToArray();
                        }
                    }
                }
            }
        }

        /// <summary>Mother's Maiden Surname</summary>
        [IJEField(260, 1540, 50, "Mother's Maiden Surname", "MOMMAIDN", 1)]
        public string MOMMAIDN
        {
            get
            {
                return LeftJustified_Get("MOMMAIDN", "MotherMaidenFamilyName");
            }
            set
            {
                LeftJustified_Set("MOMMAIDN", "MotherMaidenFamilyName", value);
            }
        }

        /// <summary>Mother's Maiden Surname Suffix</summary>
        [IJEField(261, 1590, 7, "Mother's Maiden Surname Suffix", "MOMMSUFX", 1)]
        public string MOMMSUFX
        {
            get
            {
                return LeftJustified_Get("MOMMSUFX", "MotherMaidenSuffix");
            }
            set
            {
                LeftJustified_Set("MOMMSUFX", "MotherMaidenSuffix", value);
            }
        }

        /// <summary>Residence Street Number</summary>
        [IJEField(262, 1597, 10, "Residence Street Number", "STNUM", 1)]
        public string STNUM
        {
            get
            {
                return Dictionary_Geo_Get("STNUM", "MotherResidence", "address", "stnum", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("STNUM", "MotherResidence", "address", "stnum", false, value);
                }
            }
        }

        /// <summary>Residence Pre Directional</summary>
        [IJEField(263, 1607, 10, "Residence Pre Directional", "PREDIR", 1)]
        public string PREDIR
        {
            get
            {
                return Dictionary_Geo_Get("PREDIR", "MotherResidence", "address", "predir", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("PREDIR", "MotherResidence", "address", "predir", false, value);
                }
            }
        }

        /// <summary>Residence Street name</summary>
        [IJEField(264, 1617, 28, "Residence Street name", "STNAME", 1)]
        public string STNAME
        {
            get
            {
                return Dictionary_Geo_Get("STNAME", "MotherResidence", "address", "stname", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("STNAME", "MotherResidence", "address", "stname", false, value);
                }
            }
        }

        /// <summary>Residence Street designator</summary>
        [IJEField(265, 1645, 10, "Residence Street designator", "STDESIG", 1)]
        public string STDESIG
        {
            get
            {
                return Dictionary_Geo_Get("STDESIG", "MotherResidence", "address", "stdesig", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("STDESIG", "MotherResidence", "address", "stdesig", false, value);
                }
            }
        }

        /// <summary>Residence Post Directional</summary>
        [IJEField(266, 1655, 10, "Residence Post Directional", "POSTDIR", 1)]
        public string POSTDIR
        {
            get
            {
                return Dictionary_Geo_Get("POSTDIR", "MotherResidence", "address", "postdir", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("POSTDIR", "MotherResidence", "address", "postdir", false, value);
                }
            }
        }

        /// <summary>Residence Unit or Apartment Number</summary>
        [IJEField(267, 1665, 7, "Residence Unit or Apartment Number", "UNUM", 1)]
        public string UNUM
        {
            get
            {
                return Dictionary_Geo_Get("UNUM", "MotherResidence", "address", "unitnum", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("UNUM", "MotherResidence", "address", "unitnum", false, value);
                }
            }
        }

        /// <summary>Mother's Residence Street Address</summary>
        [IJEField(268, 1672, 50, "Mother's Residence Street Address", "ADDRESS", 1)]
        public string ADDRESS
        {
            get
            {
                return Dictionary_Geo_Get("ADDRESS", "MotherResidence", "address", "line1", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("ADDRESS", "MotherResidence", "address", "line1", false, value);
                }
            }
        }

        /// <summary>Mother's Residence Zip Code and Zip+4</summary>
        [IJEField(269, 1722, 9, "Mother's Residence Zip Code and Zip+4", "ZIPCODE", 1)]
        public string ZIPCODE
        {
            get
            {
                return Dictionary_Geo_Get("ZIPCODE", "MotherResidence", "address", "zip", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("ZIPCODE", "MotherResidence", "address", "zip", false, value);
                }
            }
        }

        /// <summary>Mother's Residence County (Literal)</summary>
        [IJEField(270, 1731, 28, "Mother's Residence County (Literal)", "COUNTYTXT", 1)]
        public string COUNTYTXT
        {
            get
            {
                return Dictionary_Geo_Get("COUNTYTXT", "MotherResidence", "address", "county", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("COUNTYTXT", "MotherResidence", "address", "county", false, value);
                }
            }
        }

        /// <summary>Mother's Residence City/Town (Literal)</summary>
        [IJEField(271, 1759, 28, "Mother's Residence City/Town (Literal)", "CITYTEXT", 1)]
        public string CITYTEXT
        {
            get
            {
                return Dictionary_Geo_Get("CITYTEXT", "MotherResidence", "address", "city", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("CITYTEXT", "MotherResidence", "address", "city", false, value);
                }
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Residence (Mother) - literal</summary>
        [IJEField(272, 1787, 28, "State, U.S. Territory or Canadian Province of Residence (Mother) - literal", "STATETXT", 1)]
        public string STATETXT
        {
            get
            {
                return IJEData.Instance.StateCodeToStateName(STATEC);
            }
            set
            {
                STATEC = IJEData.Instance.StateNameToStateCode(value);
            }
        }

        /// <summary>Mother's Residence Country (Literal)</summary>
        [IJEField(273, 1815, 28, "Mother's Residence Country (Literal)", "CNTRYTXT", 1)]
        public string CNTRYTXT
        {
            get
            {
                return IJEData.Instance.CountryCodeToCountryName(COUNTRYC);
            }
            set
            {
                COUNTRYC = IJEData.Instance.CountryCodeToCountryName(value);
            }
        }

        /// <summary>Father's First Name</summary>
        [IJEField(274, 1843, 50, "Father's First Name", "DADFNAME", 1)]
        public string DADFNAME
        {
            get
            {
                string[] names = record.FatherGivenNames;
                if (names.Length > 0)
                {
                    return Truncate(names[0], 50).PadRight(50, ' ');
                }
                return new string(' ', 50);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    record.FatherGivenNames = new string[] { value.Trim() };
                }
            }
        }

        /// <summary>Father's Middle Name</summary>
        [IJEField(275, 1893, 50, "Father's Middle Name", "DADMNAME", 1)]
        public string DADMNAME
        {
            get
            {
                string[] names = record.FatherGivenNames;
                if (names.Length > 1)
                {
                    return Truncate(names[1], 50).PadRight(50, ' ');
                }
                return " ";
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (String.IsNullOrWhiteSpace(DADFNAME)) throw new ArgumentException("Middle name cannot be set before first name");
                    if (!String.IsNullOrWhiteSpace(value))
                    {
                        if (record.FatherGivenNames != null)
                        {
                            List<string> names = record.FatherGivenNames.ToList();
                            if (names.Count() > 1) names[1] = value.Trim(); else names.Add(value.Trim());
                            record.FatherGivenNames = names.ToArray();
                        }
                    }
                }
            }
        }

        /// <summary>Father's Last Name</summary>
        [IJEField(276, 1943, 50, "Father's Last Name", "DADLNAME", 1)]
        public string DADLNAME
        {
            get
            {
                return LeftJustified_Get("DADLNAME", "FatherFamilyName");            }
            set
            {
                LeftJustified_Set("DADLNAME", "FatherFamilyName", value);
            }
        }

        /// <summary>Father's Surname Suffix</summary>
        [IJEField(277, 1993, 7, "Father's Surname Suffix", "DADSUFFX", 1)]
        public string DADSUFFX
        {
            get
            {
                return LeftJustified_Get("DADSUFFX", "FatherSuffix");
            }
            set
            {
                LeftJustified_Set("DADSUFFX", "FatherSuffix", value);
            }
        }

        /// <summary>Mother's Social Security Number</summary>
        [IJEField(278, 2000, 9, "Mother's Social Security Number", "MOM_SSN", 1)]
        public string MOM_SSN
        {
            get
            {
                return record.MotherSocialSecurityNumber;
            }
            set
            {
                record.MotherSocialSecurityNumber = value;
            }
        }

        /// <summary>Father's Social Security Number</summary>
        [IJEField(279, 2009, 9, "Father's Social Security Number", "DAD_SSN", 1)]
        public string DAD_SSN
        {
            get
            {
                return record.FatherSocialSecurityNumber;
            }
            set
            {
                record.FatherSocialSecurityNumber = value;
            }
        }

        /// <summary>Mother's Age (Calculated)</summary>
        [IJEField(280, 2018, 2, "Mother's Age (Calculated)", "MAGE_CALC", 1)]
        public string MAGE_CALC
        {
            get
            {
                // Not implemented in FHIR
                return "";
            }
            set
            {
                // Not implemented in FHIR
            }
        }

        /// <summary>Father's Age (Calculated)</summary>
        [IJEField(281, 2020, 2, "Father's Age (Calculated)", "FAGE_CALC", 1)]
        public string FAGE_CALC
        {
            get
            {
                // Not implemented in FHIR
                return "";
            }
            set
            {
                // Not implemented in FHIR
            }
        }

        /// <summary>Occupation of Mother</summary>
        [IJEField(282, 2022, 25, "Occupation of Mother", "MOM_OC_T", 1)]
        public string MOM_OC_T
        {
            get
            {
                return record.MotherOccupation;
            }
            set
            {
                record.MotherOccupation = value;
            }
        }

        /// <summary>Occupation of Mother (coded)</summary>
        [IJEField(283, 2047, 3, "Occupation of Mother (coded)", "MOM_OC_C", 1)]
        public string MOM_OC_C
        {
            get
            {
                // Not implemented in FHIR
                return "";
            }
            set
            {
                // Not implemented in FHIR
            }
        }

        /// <summary>Occupation of Father</summary>
        [IJEField(284, 2050, 25, "Occupation of Father", "DAD_OC_T", 1)]
        public string DAD_OC_T
        {
            get
            {
                return record.FatherOccupation;
            }
            set
            {
                record.FatherOccupation = value;
            }
        }

        /// <summary>Occupation of Father (coded)</summary>
        [IJEField(285, 2075, 3, "Occupation of Father (coded)", "DAD_OC_C", 1)]
        public string DAD_OC_C
        {
            get
            {
                // Not implemented in FHIR
                return "";
            }
            set
            {
                // Not implemented in FHIR
            }
        }

        /// <summary>Industry of Mother</summary>
        [IJEField(286, 2078, 25, "Industry of Mother", "MOM_IN_T", 1)]
        public string MOM_IN_T
        {
            get
            {
                return record.MotherIndustry;
            }
            set
            {
                record.MotherIndustry = value;
            }
        }

        /// <summary>Industry of Mother (coded)</summary>
        [IJEField(287, 2103, 3, "Industry of Mother (coded)", "MOM_IN_C", 1)]
        public string MOM_IN_C
        {
            get
            {
                // Not implemented in FHIR
                return "";
            }
            set
            {
                // Not implemented in FHIR
            }
        }

        /// <summary>Industry of Father</summary>
        [IJEField(288, 2106, 25, "Industry of Father", "DAD_IN_T", 1)]
        public string DAD_IN_T
        {
            get
            {
                return record.FatherIndustry;
            }
            set
            {
                record.FatherIndustry = value;
            }
        }

        /// <summary>Industry of Father (coded)</summary>
        [IJEField(289, 2131, 3, "Industry of Father (coded)", "DAD_IN_C", 1)]
        public string DAD_IN_C
        {
            get
            {
                // Not implemented in FHIR
                return "";
            }
            set
            {
                // Not implemented in FHIR
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Birth (Father) - code</summary>
        [IJEField(290, 2134, 2, "State, U.S. Territory or Canadian Province of Birth (Father) - code", "FBPLACD_ST_TER_C", 1)]
        public string FBPLACD_ST_TER_C
        {
            get
            {
                return Dictionary_Geo_Get("FBPLACD_ST_TER_C", "FatherPlaceOfBirth", "address", "state", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Set("FBPLACD_ST_TER_C", "FatherPlaceOfBirth", "addressState", value);
                }
            }
        }

        /// <summary>Father's Country of Birth (Code)</summary>
        [IJEField(291, 2136, 2, "Father's Country of Birth (Code)", "FBPLACE_CNT_C", 1)]
        public string FBPLACE_CNT_C
        {
            get
            {
                return Dictionary_Geo_Get("FBPLACE_CNT_C", "FatherPlaceOfBirth", "address", "country", true);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Set("FBPLACE_CNT_C", "FatherPlaceOfBirth", "addressCountry", value);
                }
            }
        }

        /// <summary>Mother's Hispanic Code for Literal</summary>
        [IJEField(292, 2138, 3, "Mother's Hispanic Code for Literal", "METHNIC5C", 1)]
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
        [IJEField(293, 2141, 3, "Mother's Edited Hispanic Origin Code", "METHNICE", 1)]
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
        [IJEField(294, 2144, 2, "Mother's Bridged Race - NCHS Code", "MRACEBG_C", 1)]
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
        [IJEField(295, 2146, 3, "Father's Hispanic Code for Literal", "FETHNIC5C", 1)]
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
        [IJEField(296, 2149, 3, "Father's Edited Hispanic Origin Code", "FETHNICE", 1)]
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
        [IJEField(297, 2152, 2, "Father's Bridged Race - NCHS Code", "FRACEBG_C", 1)]
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
        [IJEField(298, 2154, 15, "Mother's Hispanic Origin - Specify", "METHNIC_T", 1)]
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
        [IJEField(299, 2169, 50, "Mother's Race - Specify", "MRACE_T", 1)]
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
        [IJEField(300, 2219, 15, "Father's Hispanic Origin - Specify", "FETHNIC_T", 1)]
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
        [IJEField(301, 2234, 50, "Father's Race - Specify", "FRACE_T", 1)]
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
        [IJEField(302, 2284, 50, "Facility Mother Moved From (if transferred)", "HOSPFROM", 1)]
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

        /// <summary>Facility Infant Transferred To (if transferred w/in 24 hours)</summary>
        [IJEField(303, 2334, 50, "Facility Infant Transferred To (if transferred w/in 24 hours)", "HOSPTO", 1)]
        public string HOSPTO
        {
            get
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
        [IJEField(304, 2384, 20, "Attendant (\"Other\" specified text)", "ATTEND_OTH_TXT", 1)]
        public string ATTEND_OTH_TXT
        {
            get
            {
                if (record.AttendantOtherHelper != null)
                {
                    return LeftJustified_Get("ATTEND_OTH_TXT", "AttendantOtherHelper");
                }
                return "";
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    LeftJustified_Set("ATTEND_OTH_TXT", "AttendantOtherHelper", value);
                }
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Birth (Mother) - literal</summary>
        [IJEField(305, 2404, 28, "State, U.S. Territory or Canadian Province of Birth (Mother) - literal", "MBPLACE_ST_TER_TXT", 1)]
        public string MBPLACE_ST_TER_TXT
        {
            get
            {
                return IJEData.Instance.StateCodeToStateName(BPLACEC_ST_TER);
            }
            set
            {
                BPLACEC_ST_TER = IJEData.Instance.StateNameToStateCode(value);
            }
        }

        /// <summary>Mother's Country of Birth (Literal)</summary>
        [IJEField(306, 2432, 28, "Mother's Country of Birth (Literal)", "MBPLACE_CNTRY_TXT", 1)]
        public string MBPLACE_CNTRY_TXT
        {
            get
            {
                return IJEData.Instance.CountryCodeToCountryName(BPLACEC_CNT);
            }
            set
            {
                BPLACEC_CNT = IJEData.Instance.CountryNameToCountryCode(value);
            }
        }

        /// <summary>State, U.S. Territory or Canadian Province of Birth (Father) - literal</summary>
        [IJEField(307, 2460, 28, "State, U.S. Territory or Canadian Province of Birth (Father) - literal", "FBPLACE_ST_TER_TXT", 1)]
        public string FBPLACE_ST_TER_TXT
        {
            get
            {
                return IJEData.Instance.StateCodeToStateName(FBPLACD_ST_TER_C);
            }
            set
            {
                FBPLACD_ST_TER_C = IJEData.Instance.StateNameToStateCode(value);
            }
        }

        /// <summary>Father's Country of Birth (Literal)</summary>
        [IJEField(308, 2488, 28, "Father's Country of Birth (Literal)", "FBPLACE_CNTRY_TXT", 1)]
        public string FBPLACE_CNTRY_TXT
        {
            get
            {
                return IJEData.Instance.CountryCodeToCountryName(FBPLACE_CNT_C);
            }
            set
            {
                FBPLACE_CNT_C = IJEData.Instance.CountryNameToCountryCode(value);
            }
        }

        /// <summary>Mother's Mailing Address Street number</summary>
        [IJEField(309, 2516, 10, "Mother's Mailing Address Street number", "MAIL_STNUM", 1)]
        public string MAIL_STNUM
        {
            get
            {
                return Dictionary_Geo_Get("MAIL_STNUM", "MotherBilling", "address", "stnum", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("MAIL_STNUM", "MotherBilling", "address", "stnum", false, value);
                }
            }
        }

        /// <summary>Mother's Mailing Address Pre Directional</summary>
        [IJEField(310, 2526, 10, "Mother's Mailing Address Pre Directional", "MAIL_PREDIR", 1)]
        public string MAIL_PREDIR
        {
            get
            {
                return Dictionary_Geo_Get("MAIL_PREDIR", "MotherBilling", "address", "predir", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("MAIL_PREDIR", "MotherBilling", "address", "predir", false, value);
                }
            }
        }

        /// <summary>Mother's Mailing Address Street name</summary>
        [IJEField(311, 2536, 28, "Mother's Mailing Address Street name", "MAIL_STNAME", 1)]
        public string MAIL_STNAME
        {
            get
            {
                return Dictionary_Geo_Get("MAIL_STNAME", "MotherBilling", "address", "stname", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("MAIL_STNAME", "MotherBilling", "address", "stname", false, value);
                }
            }
        }

        /// <summary>Mother's Mailing Address Street designator</summary>
        [IJEField(312, 2564, 10, "Mother's Mailing Address Street designator", "MAIL_STDESIG", 1)]
        public string MAIL_STDESIG
        {
            get
            {
                return Dictionary_Geo_Get("MAIL_STDESIG", "MotherBilling", "address", "stdesig", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("MAIL_STDESIG", "MotherBilling", "address", "stdesig", false, value);
                }
            }
        }

        /// <summary>Mother's Mailing Address Post Directional</summary>
        [IJEField(313, 2574, 10, "Mother's Mailing Address Post Directional", "MAIL_POSTDIR", 1)]
        public string MAIL_POSTDIR
        {
            get
            {
                return Dictionary_Geo_Get("MAIL_POSTDIR", "MotherBilling", "address", "postdir", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("MAIL_POSTDIR", "MotherBilling", "address", "postdir", false, value);
                }
            }
        }

        /// <summary>Mother's Mailing Address Unit or Apartment Number</summary>
        [IJEField(314, 2584, 7, "Mother's Mailing Address Unit or Apartment Number", "MAIL_UNUM", 1)]
        public string MAIL_UNUM
        {
            get
            {
                return Dictionary_Geo_Get("MAIL_UNUM", "MotherBilling", "address", "unitnum", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("MAIL_UNUM", "MotherBilling", "address", "unitnum", false, value);
                }
            }
        }

        /// <summary>Mother's Mailing Address Street Address</summary>
        [IJEField(315, 2591, 50, "Mother's Mailing Address Street Address", "MAIL_ADDRESS", 1)]
        public string MAIL_ADDRESS
        {
            get
            {
                return Dictionary_Geo_Get("MAIL_ADDRESS", "MotherBilling", "address", "line1", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("MAIL_ADDRESS", "MotherBilling", "address", "line1", false, value);
                }
            }
        }

        /// <summary>Mother's Mailing Address Zip Code and Zip+4</summary>
        [IJEField(316, 2641, 9, "Mother's Mailing Address Zip Code and Zip+4", "MAIL_ZIPCODE", 1)]
        public string MAIL_ZIPCODE
        {
            get
            {
                return Dictionary_Geo_Get("MAIL_ZIPCODE", "MotherBilling", "address", "zip", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("MAIL_ZIPCODE", "MotherBilling", "address", "zip", false, value);
                }
            }
        }

        /// <summary>Mother's Mailing Address County (Literal)</summary>
        [IJEField(317, 2650, 28, "Mother's Mailing Address County (Literal)", "MAIL_COUNTYTXT", 1)]
        public string MAIL_COUNTYTXT
        {
            get
            {
                return Dictionary_Geo_Get("MAIL_COUNTYTXT", "MotherBilling", "address", "county", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("MAIL_COUNTYTXT", "MotherBilling", "address", "county", false, value);
                }
            }
        }

        /// <summary>Mother's Mailing Address City/Town (Literal)</summary>
        [IJEField(318, 2678, 28, "Mother's Mailing Address City/Town (Literal)", "MAIL_CITYTEXT", 1)]
        public string MAIL_CITYTEXT
        {
            get
            {
                return Dictionary_Geo_Get("MAIL_CITYTEXT", "MotherBilling", "address", "city", false);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary_Geo_Set("MAIL_CITYTEXT", "MotherBilling", "address", "city", false, value);
                }
            }
        }

        /// <summary>Mother's Mailing Address State (Literal)</summary>
        [IJEField(319, 2706, 28, "Mother's Mailing Address State (Literal)", "MAIL_STATETXT", 1)]
        public string MAIL_STATETXT
        {
            get
            {
                string stateCode = Dictionary_Geo_Get("MAIL_STATETXT", "MotherBilling", "address", "state", false);
                return IJEData.Instance.StateCodeToStateName(stateCode);
            }
            set
            {
                string stateCode = IJEData.Instance.StateNameToStateCode(value);
                if (!String.IsNullOrWhiteSpace(stateCode))
                {
                    Dictionary_Geo_Set("MAIL_STATETXT", "MotherBilling", "address", "state", false, stateCode);
                }
            }
        }

        /// <summary>Mother's Mailing Address Country (Literal)</summary>
        [IJEField(320, 2734, 28, "Mother's Mailing Address Country (Literal)", "MAIL_CNTRYTXT", 1)]
        public string MAIL_CNTRYTXT
        {
            get
            {
                string countryCode = Dictionary_Geo_Get("MAIL_CNTRYTXT", "MotherBilling", "address", "country", false);
                return IJEData.Instance.CountryCodeToCountryName(countryCode);
            }
            set
            {
                string countryCode = IJEData.Instance.CountryNameToCountryCode(value);
                if (!String.IsNullOrWhiteSpace(countryCode))
                {
                    Dictionary_Geo_Set("MAIL_CNTRYTXT", "MotherBilling", "address", "country", false, countryCode);
                }
            }
        }

        /// <summary>Social Security Number Requested for Child?</summary>
        [IJEField(321, 2762, 1, "Social Security Number Requested for Child?", "SSN_REQ", 1)]
        public string SSN_REQ
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>SSA/EAB Citizenship Code</summary>
        [IJEField(322, 2763, 1, "SSA/EAB Citizenship Code", "SSN_CITIZEN_CD", 1)]
        public string SSN_CITIZEN_CD
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>SSA/EAB Multiple Birth Code</summary>
        [IJEField(323, 2764, 1, "SSA/EAB Multiple Birth Code", "SSN_MULT_BTH_CD", 1)]
        public string SSN_MULT_BTH_CD
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>SSA/EAB Feedback Release</summary>
        [IJEField(324, 2765, 1, "SSA/EAB Feedback Release", "SSN_FEEDBACK", 1)]
        public string SSN_FEEDBACK
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>SSA/EAB Birth Certificate Number Code</summary>
        [IJEField(325, 2766, 11, "SSA/EAB Birth Certificate Number Code", "SSN_BRTH_CRT_NO", 1)]
        public string SSN_BRTH_CRT_NO
        {
            get
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
        [IJEField(326, 2777, 50, "Attendant's Name", "ATTEND_NAME", 1)]
        public string ATTEND_NAME
        {
            get
            {
                return LeftJustified_Get("ATTEND_NAME", "AttendantName");
            }
            set
            {
                LeftJustified_Set("ATTEND_NAME", "AttendantName", value);
            }
        }

        /// <summary>Attendant's NPI</summary>
        [IJEField(327, 2827, 12, "Attendant's NPI", "ATTEND_NPI", 1)]
        public string ATTEND_NPI
        {
            get
            {
                return LeftJustified_Get("ATTEND_NPI", "AttendantNPI");
            }
            set
            {
                LeftJustified_Set("ATTEND_NPI", "AttendantNPI", value);
            }
        }

        /// <summary>Certifier's Name</summary>
        [IJEField(328, 2839, 50, "Certifier's Name", "CERTIF_NAME", 1)]
        public string CERTIF_NAME
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Certifier's NPI</summary>
        [IJEField(329, 2889, 12, "Certifier's NPI", "CERTIF_NPI", 1)]
        public string CERTIF_NPI
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Certifier Title</summary>
        [IJEField(330, 2901, 1, "Certifier Title", "CERTIF", 1)]
        public string CERTIF
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Certifier ("Other" specified text)</summary>
        [IJEField(331, 2902, 20, "Certifier (\"Other\" specified text)", "CERTIF_OTH_TXT", 1)]
        public string CERTIF_OTH_TXT
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Infant's Medical Record Number</summary>
        [IJEField(332, 2922, 15, "Infant's Medical Record Number", "INF_MED_REC_NUM", 1)]
        public string INF_MED_REC_NUM
        {
            get
            {
                return record.InfantMedicalRecordNumber;
            }
            set
            {
                record.InfantMedicalRecordNumber = value;
            }
        }

        /// <summary>Mother's Medical Record Number</summary>
        [IJEField(333, 2937, 15, "Mother's Medical Record Number", "MOM_MED_REC_NUM", 1)]
        public string MOM_MED_REC_NUM
        {
            get
            {
                return record.MotherMedicalRecordNumber;
            }
            set
            {
                record.MotherMedicalRecordNumber = value;
            }
        }

        /// <summary>Date Signed by Certifier--Year</summary>
        [IJEField(334, 2952, 4, "Date Signed by Certifier--Year", "CERTIFIED_YR", 1)]
        public string CERTIFIED_YR
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                // return NumericAllowingUnknown_Get("CERTIFIED_YR", "CertifiedYear");
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
                // NumericAllowingUnknown_Set("CERTIFIED_YR", "CertifiedYear", value);
            }
        }

        /// <summary>Date Signed by Certifier--Month</summary>
        [IJEField(335, 2956, 2, "Date Signed by Certifier--Month", "CERTIFIED_MO", 1)]
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
        [IJEField(336, 2958, 2, "Date Signed by Certifier--Day", "CERTIFIED_DY", 1)]
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
        [IJEField(337, 2960, 4, "Date Filed by Registrar--Year", "REGISTER_YR", 1)]
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
        [IJEField(338, 2964, 2, "Date Filed by Registrar--Month", "REGISTER_MO", 1)]
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
        [IJEField(339, 2966, 2, "Date Filed by Registrar--Day", "REGISTER_DY", 1)]
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

        /// <summary>For use of jurisdictions with domestic partnerships, other types of relationships.</summary>
        [IJEField(340, 2968, 50, "For use of jurisdictions with domestic partnerships, other\ntypes of relationships.", "MARITAL_DESCRIP", 1)]
        public string MARITAL_DESCRIP
        {
            get
            {
                // TODO: Implement mapping from FHIR record location:
                return "";
            }
            set
            {
                // TODO: Implement mapping to FHIR record location:
            }
        }

        /// <summary>Replacement (amended) record</summary>
        [IJEField(341, 3018, 1, "Replacement (amended) record", "REPLACE", 1)]
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
        [IJEField(342, 3019, 1, "Blank for One-Byte Field 1", "PLACE1_1", 1)]
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
        [IJEField(343, 3020, 1, "Blank for One-Byte Field 2", "PLACE1_2", 1)]
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
        [IJEField(344, 3021, 1, "Blank for One-Byte Field 3", "PLACE1_3", 1)]
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
        [IJEField(345, 3022, 1, "Blank for One-Byte Field 4", "PLACE1_4", 1)]
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
        [IJEField(346, 3023, 1, "Blank for One-Byte Field 5", "PLACE1_5", 1)]
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
        [IJEField(347, 3024, 1, "Blank for One-Byte Field 6", "PLACE1_6", 1)]
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
        [IJEField(348, 3025, 8, "Blank for Eight-Byte Field 1", "PLACE8_1", 1)]
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
        [IJEField(349, 3033, 8, "Blank for Eight-Byte Field 2", "PLACE8_2", 1)]
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
        [IJEField(350, 3041, 8, "Blank for Eight-Byte Field 3", "PLACE8_3", 1)]
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
        [IJEField(351, 3049, 20, "Blank for Twenty-Byte Field", "PLACE20", 1)]
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

        /// <summary>Blank for Future Expansion</summary>
        [IJEField(352, 3069, 450, "Blank for Future Expansion", "BLANK", 1)]
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
        [IJEField(353, 3519, 482, "Blank for Jurisdictional Use Only", "BLANK2", 1)]
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

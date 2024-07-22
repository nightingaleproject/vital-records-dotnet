using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Hl7.Fhir.Model;
using VR;
using Hl7.Fhir.Support;
using static Hl7.Fhir.Model.Encounter;
using Hl7.Fhir.Utility;

// NatalityRecord_submissionProperties.cs
// These fields are used primarily for submitting birth records to NCHS.

namespace BFDR
{
    /// <summary>Class <c>NatalityRecord</c> is an abstract base class models FHIR Vital Records
    /// Birth Reporting (BFDR) Birth and Fetal Death Records. This class was designed to help consume
    /// and produce natality records that follow the HL7 FHIR Vital Records Birth Reporting Implementation
    /// Guide, as described at: http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// TODO BFDR STU2 has broken up its birth record bundles, the birth bundle has birthCertificateNumber + required birth compositions,
    /// the fetal death bundle has fetalDeathReportNumber + required fetal death compositions,
    /// the demographic bundle has a fileNumber + requiredCompositionCodedRaceAndEthnicity,
    /// and the cause of death bundle has a fetalDeathReportNumber + required CompositionCodedCauseOfFetalDeath
    /// TODO BFDR STU2 supports usual work and role extension
    /// </summary>
    public partial class NatalityRecord
    {
        /////////////////////////////////////////////////////////////////////////////////
        //
        // Record Properties: Birth Certification
        //
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>Birth Certificate Number.</summary>
        /// <value>a record identification string.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.Identifier = "42";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Birth Certificate Number: {ExampleBirthRecord.Identifier}");</para>
        /// </example>
        [Property("Certificate Number", Property.Types.String, "Birth Certification", "Birth Certificate Number.", true, VR.IGURL.CertificateNumber, true, 3)]
        [FHIRPath("Bundle", "identifier")]
        public string CertificateNumber
        {
            get
            {
                if (Bundle?.Identifier?.Extension != null)
                {
                    Extension ext = Bundle.Identifier.Extension.Find(ex => ex.Url == VR.ProfileURL.CertificateNumber);
                    if (ext?.Value != null)
                    {
                        return Convert.ToString(ext.Value);
                    }
                }
                return null;
            }
            set
            {
                Bundle.Identifier.Extension.RemoveAll(ex => ex.Url == VR.ProfileURL.CertificateNumber);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Extension ext = new Extension(VR.ProfileURL.CertificateNumber, new FhirString(value));
                    Bundle.Identifier.Extension.Add(ext);
                    UpdateRecordIdentifier();
                }
            }
        }

        /// <summary>Birth Record Bundle Identifier, NCHS identifier.</summary>
        /// <value>a record bundle identification string, e.g., 2022MA000100, derived from year of birth, jurisdiction of birth, and certificate number</value>
        /// <example>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"NCHS identifier: {ExampleBirthRecord.BirthRecordIdentifier}");</para>
        /// </example>
        [Property("Birth Record Identifier", Property.Types.String, "Birth Certification", "Birth Record identifier.", true, VR.IGURL.CertificateNumber, false, 4)]
        [FHIRPath("Bundle", "identifier")]
        public string RecordIdentifier
        {
            get
            {
                if (Bundle != null && Bundle.Identifier != null)
                {
                    return Bundle.Identifier.Value;
                }
                return null;
            }
            // The setter is protected because the value is derived so should never be set directly
            protected set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                if (Bundle.Identifier == null)
                {
                    Bundle.Identifier = new Identifier();
                }
                Bundle.Identifier.Value = value;
                Bundle.Identifier.System = "http://nchs.cdc.gov/bfdr_id";
            }
        }

        // TODO: Temporary placeholder with placeholder URL values and FHIR path to support testing of message creation
        /// <summary>State Local Identifier1.</summary>
        /// <para>"value" the string representation of the local identifier</para>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.StateLocalIdentifier1 = "MA";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"State local identifier: {ExampleBirthRecord.StateLocalIdentifier1}");</para>
        /// </example>
        [Property("State Local Identifier1", Property.Types.String, "Birth Certification", "State Local Identifier.", true, VR.IGURL.AuxiliaryStateIdentifier1VitalRecords, true, 5)]
        [FHIRPath("Bundle", "identifier")]
        public string StateLocalIdentifier1
        {
            get
            {
                if (Bundle?.Identifier?.Extension != null)
                {
                    Extension ext = Bundle.Identifier.Extension.Find(ex => ex.Url == VR.ProfileURL.AuxiliaryStateIdentifier1VitalRecords);
                    if (ext?.Value != null)
                    {
                        return Convert.ToString(ext.Value);
                    }
                }
                return null;
            }
            set
            {
                Bundle.Identifier.Extension.RemoveAll(ex => ex.Url == VR.ProfileURL.AuxiliaryStateIdentifier1VitalRecords);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Extension ext = new Extension(VR.ProfileURL.AuxiliaryStateIdentifier1VitalRecords, new FhirString(value));
                    Bundle.Identifier.Extension.Add(ext);
                }
            }
        }

        /// <summary>Child's Year of Birth.</summary>
        /// <value>the child's year of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthYear = 1928;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Year of Birth: {ExampleBirthRecord.BirthYear}");</para>
        /// </example>
        [Property("BirthYear", Property.Types.Int32, "Child Demographics", "Child's Year of Birth.", false, VR.IGURL.Child, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate", "")]
        public int? BirthYear
        {
            get
            {
                return GetDateElementNoTime(Child?.BirthDateElement, VR.ExtensionURL.PartialDateTimeYearVR);
            }
            set
            {
                if (Child.BirthDateElement == null)
                {
                    AddBirthDateToPatient(Child, false);
                }
                string time = this.BirthTime;
                Date newDate = SetYear(value, Child.BirthDateElement);
                if (newDate != null)
                {
                    Child.BirthDateElement = newDate;
                }
                this.BirthTime = time;
            }
        }

        private void SetNewlyCompletedDate(DateTime completeDate)
        {
            // If the time is known, populate BOTH the birthDate field and PatientBirthTime extension. If the time is unknown, populate JUST the birthDate field.
            if (BirthTime != null && BirthTime != "temp-unknown")
            {
                Time time = new Time(BirthTime);
                Child.BirthDateElement = new Date(completeDate.Year, completeDate.Month, completeDate.Day);
                // Is the TimeSpan.Zero safe for time offset? Got this line from VR.ConvertFhirTimeToFhirDateTime().
                FhirDateTime dateTime = new FhirDateTime(completeDate.Year, completeDate.Month, completeDate.Day, FhirTimeHour(time), FhirTimeMin(time), FhirTimeSec(time), TimeSpan.Zero);
                Child.BirthDateElement.SetExtension(VR.ExtensionURL.PatientBirthTime, dateTime);
            }
            else
            {
                FhirDateTime dateTime = new FhirDateTime(completeDate.Year, completeDate.Month, completeDate.Day);
                Child.BirthDate = dateTime.ToString();
                // Make sure the PatientBirthTime is not present because we have no time data.
                Child.BirthDateElement.RemoveExtension(VR.ExtensionURL.PatientBirthTime);
            }
            // Remove the now extraneous PartialDateTime.
            Child.BirthDateElement.RemoveExtension(VRExtensionURLs.PartialDateTime);
            return;
        }

        /// <summary>Overriden method that dictates which Extension URL to use for PartialDateTime Year</summary>x
        protected override string PartialDateYearUrl => VR.ExtensionURL.PartialDateTimeYearVR;

        /// <summary>Overriden method that dictates which Extension URL to use for PartialDateTime Month</summary>
        protected override string PartialDateMonthUrl => VR.ExtensionURL.PartialDateTimeMonthVR;

        /// <summary>Overriden method that dictates which Extension URL to use for PartialDateTime Day</summary>
        protected override string PartialDateDayUrl => VR.ExtensionURL.PartialDateTimeDayVR;

        /// <summary>Overriden method that dictates which Extension URL to use for PartialDateTime Time</summary>
        protected override string PartialDateTimeTimeUrl => VR.ExtensionURL.PartialDateTimeTimeVR;

        /// <summary>Overriden method that dictates which Extension URL to use for PartialDateTime</summary>
        protected override string PartialDateTimeUrl => VRExtensionURLs.PartialDateTime;

        /// <summary>Overrideable method that dictates which Extension URL to use for PartialDate</summary>
        protected override string PartialDateUrl => VRExtensionURLs.PartialDate;

        /// <summary>Child's Month of Birth.</summary>
        /// <value>the child's month of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthMonth = 11;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Month of Birth: {ExampleBirthRecord.BirthMonth}");</para>
        /// </example>
        [Property("BirthMonth", Property.Types.Int32, "Child Demographics", "Child's Month of Birth.", false, VR.IGURL.Child, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate", "")]
        public int? BirthMonth
        {
            get
            {
                return GetDateElementNoTime(Child?.BirthDateElement, VR.ExtensionURL.PartialDateTimeMonthVR);
            }
            set
            {
                string time = this.BirthTime;
                if (Child.BirthDateElement == null)
                {
                    AddBirthDateToPatient(Child, false);
                }
                Date newDate = SetMonth(value, Child.BirthDateElement);
                if (newDate != null)
                {
                    Child.BirthDateElement = newDate;
                }
                this.BirthTime = time;
            }
        }

        /// <summary>Child's Day of Birth.</summary>
        /// <value>the child's day of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthDay = 11;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Day of Birth: {ExampleBirthRecord.BirthDay}");</para>
        /// </example>
        [Property("BirthDay", Property.Types.Int32, "Child Demographics", "Child's Day of Birth.", false, VR.IGURL.Child, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.birthDate", "")]
        public int? BirthDay
        {
            get
            {
                return GetDateElementNoTime(Child?.BirthDateElement, VR.ExtensionURL.PartialDateTimeDayVR);
            }
            set
            {
                string time = this.BirthTime;
                if (Child.BirthDateElement == null)
                {
                    AddBirthDateToPatient(Child, false);
                }
                Date newDate = SetDay(value, Child.BirthDateElement);
                if (newDate != null)
                {
                    Child.BirthDateElement = newDate;
                }
                this.BirthTime = time;
            }
        }

        /// <summary>Child's Time of Birth.</summary>
        /// <value>the child's time of birth.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthTime = 11;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Time of Birth: {ExampleBirthRecord.BirthTime}");</para>
        /// </example>
        [Property("BirthTime", Property.Types.String, "Child Demographics", "Child's Time of Birth.", true, VR.IGURL.Child, true, 14)]
        // How should FHIRPath work when the time could be in 1 of 2 different places (value in PatientBirthTime | PartialDateTime extension)
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate.extension.where(url='" + VR.ExtensionURL.PatientBirthTime + "')", "")]
        public string BirthTime
        {
            get
            {
                if (Child == null || Child.BirthDateElement == null)
                {
                    return null;
                }
                // First check for a time in the patient.birthDate PatientBirthTime extension.
                if (Child.BirthDateElement.Extension.Any(ext => ext.Url == VR.ExtensionURL.PatientBirthTime))
                {
                    FhirDateTime dateTime = (FhirDateTime)Child.BirthDateElement.Extension.Find(ext => ext.Url == VR.ExtensionURL.PatientBirthTime).Value;
                    return GetTimeFragment(dateTime);
                }
                // If it's not there, check for a PartialDateTime.
                return this.GetPartialTime(this.Child.BirthDateElement.GetExtension(PartialDateTimeUrl));
            }
            set
            {
                if (Child == null)
                {
                    return;
                }
                if (Child.BirthDateElement == null)
                {
                    AddBirthDateToPatient(Child, true);
                }
                // If the date is complete, then the birth time should be included in the patientBirthTime extension.
                if (value != "-1" && DateIsComplete(this.DateOfBirth))
                {
                    FhirDateTime dateTime = new FhirDateTime(this.DateOfBirth + "T" + value);
                    Child.BirthDateElement.SetExtension(VR.ExtensionURL.PatientBirthTime, dateTime);
                    return;
                }
                // If the date is incomplete, then the birth time should be included in the partialDateTime Time extension.
                Child.BirthDateElement.RemoveExtension(VR.ExtensionURL.PatientBirthTime);
                if (!Child.BirthDateElement.Extension.Any(ext => ext.Url == VRExtensionURLs.PartialDateTime))
                {
                    Child.BirthDateElement.SetExtension(VRExtensionURLs.PartialDateTime, new Extension());
                }
                if (!Child.BirthDateElement.Extension.Find(ext => ext.Url == VRExtensionURLs.PartialDateTime).Extension.Any(ext => ext.Url == PartialDateTimeTimeUrl))
                {
                    Child.BirthDateElement.GetExtension(VRExtensionURLs.PartialDateTime).SetExtension(PartialDateTimeTimeUrl, new Extension());
                }
                // Child.BirthDateElement.GetExtension(VRExtensionURLs.PartialDateTimeVR).SetExtension(PartialDateTimeTimeUrl, new Time(value));
                SetPartialTime(Child.BirthDateElement.GetExtension(VRExtensionURLs.PartialDateTime), value);
            }
        }

        /// <summary>
        ///  Determines whether a date is a complete date (yyyy-MM-dd).
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>Whether the given date string is a complete date</returns>
        protected bool DateIsComplete(string date)
        {
            ParseDateElements(date, out int? year, out int? month, out int? day);
            return year != null && month != null && day != null;
        }

        /// <summary>Child's Date of Birth.</summary>
        /// <value>the child's date of birth</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfBirth = "1940-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Date of Birth: {ExampleBirthRecord.DateOfBirth}");</para>
        /// </example>
        [Property("Date Of Birth", Property.Types.String, "Child Demographics", "Child's Date of Birth.", true, VR.IGURL.Child, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate", "")]
        public string DateOfBirth
        {
            get
            {
                if (this.Child == null || this.Child.BirthDateElement == null)
                {
                    return null;
                }
                return this.Child.BirthDate;
            }
            set
            {
                string time = this.BirthTime;
                this.Child.BirthDateElement = ConvertToDate(value);
                this.BirthTime = time;
            }
        }

        // TODO: waiting to figure out how to differentiate between Encounters in the record
        // /// <summary>Certified Year</summary>
        // /// <value>year of certification</value>
        // /// <example>
        // /// <para>// Getter:</para>
        // /// <para>Console.WriteLine($"Certified Year: {ExampleBirthRecord.CertifiedYear}");</para>
        // /// </example>
        // [Property("Certified Year", Property.Types.Int32, "Birth Certification", "Certified Year", true, IGURL.EncounterBirth, true, 4)]
        // [FHIRPath("Bundle.entry.resource.where($this is Encounter)", "")] // TODO we need to differentiate encounters http://build.fhir.org/ig/HL7/fhir-bfdr/StructureDefinition-Encounter-birth.html
        // public int? CertifiedYear
        // {
        //     get
        //     {
        //         if (BirthEncounter != null && BirthEncounter.Effective != null)
        //         {
        //             return GetDateFragmentOrPartialDate(BirthEncounter.Effective, VR.ExtensionURL.DateYear);
        //         }
        //         return null;
        //     }
        //     set
        //     {
        //         if (value == null && BirthEncounter == null)
        //         {
        //             return;
        //         }
        //         if (BirthEncounter == null)
        //         {
        //             CreateBirthEncounter();
        //         }
        //         SetPartialDate(BirthEncounter.Effective.Extension.Find(ext => ext.Url == VR.ExtensionURL.PartialDateTime), VR.ExtensionURL.DateYear, value);
        //     }
        // }

        /// <summary>Child's BirthSex at Birth.</summary>
        /// <value>The child's BirthSex at time of birth</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthSex = "female;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Sex at Time of Birth: {ExampleBirthRecord.BirthSex}");</para>
        /// </example>
        [Property("Sex At Birth", Property.Types.String, "Child Demographics", "Child's Sex at Birth.", true, VR.IGURL.Child, true, 12)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url='" + OtherExtensionURL.BirthSex + "')", "")]
        public string BirthSex
        {
            get
            {
                if (Child != null)
                {
                    Extension sex = Child.GetExtension(VR.OtherExtensionURL.BirthSex);
                    if (sex != null && sex.Value != null && sex.Value as Code != null)
                    {
                        return (sex.Value as Code).Value;
                    }
                }
                return null;
            }
            set
            {
                if (!CodeExistsInValueSet(value, VR.ValueSets.SexAssignedAtBirth.Codes))
                {
                    return;
                }
                Child.Extension.RemoveAll(ext => ext.Url == VR.OtherExtensionURL.BirthSex);
                Child.SetExtension(VR.OtherExtensionURL.BirthSex, new Code(value));
            }
        }

        /// <summary>Child's BirthSex at Birth. This a duplicate of BirthSex and is kept to maintain consistency for the IJE to FHIR mapper to work.</summary>
        public string BirthSexHelper
        {
            get
            {
                return BirthSex;
            }
            set
            {
                BirthSex = value;
            }
        }

        /// <summary>Child's Legal Name - Given. Middle name should be the last entry.</summary>
        /// <value>the child's name (first, etc., middle)</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>string[] names = { "Example", "Something", "Middle" };</para>
        /// <para>ExampleBirthRecord.ChildGivenNames = names;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Given Name(s): {string.Join(", ", ExampleBirthRecord.ChildGivenNames)}");</para>
        /// </example>
        [Property("Child Given Names", Property.Types.StringArr, "Child Demographics", "Child’s First Name.", true, VR.IGURL.Child, true, 0)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string[] ChildGivenNames
        {
            get
            {
                return Child?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Given?.ToArray() ?? new string[0];
            }
            set
            {
                updateGivenHumanName(value, Child.Name);
            }
        }

        /// <summary>Mother's Legal Name - Given. Middle name should be the last entry.</summary>
        /// <value>the mother's name (first, etc., middle)</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>string[] names = { "Example", "Something", "Middle" };</para>
        /// <para>ExampleBirthRecord.MotherGivenNames = names;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Given Name(s): {string.Join(", ", ExampleBirthRecord.MotherGivenNames)}");</para>
        /// </example>
        [Property("Mother Given Names", Property.Types.StringArr, "Mother Demographics", "Mother First Name.", true, VR.IGURL.Mother, true, 254)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string[] MotherGivenNames
        {
            get
            {
                return Mother?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Given?.ToArray() ?? new string[0];
            }
            set
            {
                updateGivenHumanName(value, Mother.Name);
            }
        }

        /// <summary>Natural Father's Legal Name - Given. Middle name should be the last entry.</summary>
        /// <value>the natural father's name (first, etc., middle)</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>string[] names = { "Example", "Something", "Middle" };</para>
        /// <para>ExampleBirthRecord.FatherGivenNames = names;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Given Name(s): {string.Join(", ", ExampleBirthRecord.FatherGivenNames)}");</para>
        /// </example>
        [Property("Father Given Names", Property.Types.StringArr, "Father Demographics", "Father First Name.", true, VR.IGURL.RelatedPersonFatherNatural, true, 274)]
        [FHIRPath("Bundle.entry.resource.where($this is RelatedPerson)", "name")]
        public string[] FatherGivenNames
        {
            get
            {
                return Father?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Given?.ToArray() ?? new string[0];
            }
            set
            {
                updateGivenHumanName(value, Father.Name);
            }
        }

        /// <summary>Mother's Maiden Name - Given. Middle name should be the last entry.</summary>
        /// <value>the mother's maiden name (first, etc., middle)</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>string[] names = { "Example", "Something", "Middle" };</para>
        /// <para>ExampleBirthRecord.MotherMaidenGivenNames = names;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Given Name(s): {string.Join(", ", ExampleBirthRecord.MotherMaidenGivenNames)}");</para>
        /// </example>
        [Property("Mother Maiden Given Names", Property.Types.StringArr, "Mother Demographics", "Mother Maiden First Name.", true, VR.IGURL.Mother, true, 255)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string[] MotherMaidenGivenNames
        {
            get
            {
                return Mother?.Name?.Find(name => name.Use == HumanName.NameUse.Maiden)?.Given?.ToArray() ?? new string[0];
            }
            set
            {
                updateGivenHumanName(value, Mother.Name, HumanName.NameUse.Maiden);
            }
        }

        /// <summary>Child's Legal Name - Last.</summary>
        /// <value>the child's last name</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>string lastName = "Quinn";</para>
        /// <para>ExampleBirthRecord.ChildFamilyName = lastName;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Family Name(s): {string.Join(", ", ExampleBirthRecord.ChildFamilyName)}");</para>
        /// </example>
        [Property("Child Family Name", Property.Types.String, "Child Demographics", "Child's Last Name.", true, VR.IGURL.Child, true, 0)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string ChildFamilyName
        {
            get
            {
                return Child?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Family;
            }
            set
            {
                HumanName name = Child.Name.SingleOrDefault(n => n.Use == HumanName.NameUse.Official);
                if (name != null && !String.IsNullOrEmpty(value))
                {
                    name.Family = value;
                }
                else if (!String.IsNullOrEmpty(value))
                {
                    name = new HumanName
                    {
                        Use = HumanName.NameUse.Official,
                        Family = value
                    };
                    Child.Name.Add(name);
                }
            }
        }

        /// <summary>Mother's Legal Name - Last.</summary>
        /// <value>the mother's last name</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>string lastName = "Quinn";</para>
        /// <para>ExampleBirthRecord.MotherFamilyName = lastName;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Family Name(s): {string.Join(", ", ExampleBirthRecord.MotherFamilyName)}");</para>
        /// </example>
        [Property("Mother Family Name", Property.Types.String, "Mother Demographics", "Mother's Last Name.", true, VR.IGURL.Mother, true, 256)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string MotherFamilyName
        {
            get
            {
                return Mother?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Family;
            }
            set
            {
                HumanName name = Mother.Name.SingleOrDefault(n => n.Use == HumanName.NameUse.Official);
                if (name != null && !String.IsNullOrEmpty(value))
                {
                    name.Family = value;
                }
                else if (!String.IsNullOrEmpty(value))
                {
                    name = new HumanName
                    {
                        Use = HumanName.NameUse.Official,
                        Family = value
                    };
                    Mother.Name.Add(name);
                }
            }
        }

        /// <summary>Natural Father's Legal Name - Last.</summary>
        /// <value>the natural father's last name</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>string lastName = "Quinn";</para>
        /// <para>ExampleBirthRecord.FatherFamilyName = lastName;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Family Name(s): {string.Join(", ", ExampleBirthRecord.FatherFamilyName)}");</para>
        /// </example>
        [Property("Father Family Name", Property.Types.String, "Father Demographics", "Father's Last Name.", true, VR.IGURL.RelatedPersonFatherNatural, true, 276)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string FatherFamilyName
        {
            get
            {
                return Father?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Family;
            }
            set
            {
                HumanName name = Father.Name.SingleOrDefault(n => n.Use == HumanName.NameUse.Official);
                if (name != null && !String.IsNullOrEmpty(value))
                {
                    name.Family = value;
                }
                else if (!String.IsNullOrEmpty(value))
                {
                    name = new HumanName
                    {
                        Use = HumanName.NameUse.Official,
                        Family = value
                    };
                    Father.Name.Add(name);
                }
            }
        }

        /// <summary>Mother's Maiden Name - Last.</summary>
        /// <value>the mother's maiden last name</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>string lastName = "Quinn";</para>
        /// <para>ExampleBirthRecord.MotherMaidenFamilyName = lastName;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Maiden Family Name(s): {string.Join(", ", ExampleBirthRecord.MotherMaidenFamilyName)}");</para>
        /// </example>
        [Property("Mother Maiden Family Name", Property.Types.String, "Mother Demographics", "Mother's Maiden Last Name.", true, VR.IGURL.Mother, true, 260)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string MotherMaidenFamilyName
        {
            get
            {
                return Mother?.Name?.Find(name => name.Use == HumanName.NameUse.Maiden)?.Family;
            }
            set
            {
                HumanName name = Mother.Name.SingleOrDefault(n => n.Use == HumanName.NameUse.Maiden);
                if (name != null && !String.IsNullOrEmpty(value))
                {
                    name.Family = value;
                }
                else if (!String.IsNullOrEmpty(value))
                {
                    name = new HumanName
                    {
                        Use = HumanName.NameUse.Maiden,
                        Family = value
                    };
                    Mother.Name.Add(name);
                }
            }
        }

        /// <summary>Child's Suffix.</summary>
        /// <value>the child's suffix</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.ChildSuffix = "Jr.";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Suffix: {ExampleBirthRecord.ChildSuffix}");</para>
        /// </example>
        [Property("ChildSuffix", Property.Types.String, "Child Demographics", "Child's Suffix.", true, VR.IGURL.Child, true, 6)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string ChildSuffix
        {
            get
            {
                return Child?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Suffix.FirstOrDefault();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }
                HumanName name = Child.Name.SingleOrDefault(n => n.Use == HumanName.NameUse.Official);
                if (name != null)
                {
                    string[] suffix = { value };
                    name.Suffix = suffix;
                }
                else
                {
                    name = new HumanName();
                    name.Use = HumanName.NameUse.Official;
                    string[] suffix = { value };
                    name.Suffix = suffix;
                    Child.Name.Add(name);
                }
            }
        }

        /// <summary>Mother's Suffix.</summary>
        /// <value>the mother's suffix</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherSuffix = "Jr.";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Suffix: {ExampleBirthRecord.MotherSuffix}");</para>
        /// </example>
        [Property("MotherSuffix", Property.Types.String, "Mother Demographics", "Mother's Suffix.", true, VR.IGURL.Mother, true, 257)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string MotherSuffix
        {
            get
            {
                return Mother?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Suffix.FirstOrDefault();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }
                HumanName name = Mother.Name.SingleOrDefault(n => n.Use == HumanName.NameUse.Official);
                if (name != null)
                {
                    string[] suffix = { value };
                    name.Suffix = suffix;
                }
                else
                {
                    name = new HumanName();
                    name.Use = HumanName.NameUse.Official;
                    string[] suffix = { value };
                    name.Suffix = suffix;
                    Mother.Name.Add(name);
                }
            }
        }

        /// <summary>Natural Father's Suffix.</summary>
        /// <value>the natural father's suffix</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherSuffix = "Jr.";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Suffix: {ExampleBirthRecord.FatherSuffix}");</para>
        /// </example>
        [Property("FatherSuffix", Property.Types.String, "Father Demographics", "Father's Suffix.", true, VR.IGURL.RelatedPersonFatherNatural, true, 277)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string FatherSuffix
        {
            get
            {
                return Father?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Suffix.FirstOrDefault();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }
                HumanName name = Father.Name.SingleOrDefault(n => n.Use == HumanName.NameUse.Official);
                if (name != null)
                {
                    string[] suffix = { value };
                    name.Suffix = suffix;
                }
                else
                {
                    name = new HumanName();
                    name.Use = HumanName.NameUse.Official;
                    string[] suffix = { value };
                    name.Suffix = suffix;
                    Father.Name.Add(name);
                }
            }
        }

        /// <summary>Mother's Maiden Suffix.</summary>
        /// <value>the mother's maiden suffix</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherMaidenSuffix = "Jr.";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Maiden Suffix: {ExampleBirthRecord.MotherMaidenSuffix}");</para>
        /// </example>
        [Property("MotherMaidenSuffix", Property.Types.String, "Mother Demographics", "Mother's Maiden Suffix.", true, VR.IGURL.Mother, true, 261)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "name")]
        public string MotherMaidenSuffix
        {
            get
            {
                return Mother?.Name?.Find(name => name.Use == HumanName.NameUse.Maiden)?.Suffix.FirstOrDefault();
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }
                HumanName name = Mother.Name.SingleOrDefault(n => n.Use == HumanName.NameUse.Maiden);
                if (name != null)
                {
                    string[] suffix = { value };
                    name.Suffix = suffix;
                }
                else
                {
                    name = new HumanName();
                    name.Use = HumanName.NameUse.Maiden;
                    string[] suffix = { value };
                    name.Suffix = suffix;
                    Mother.Name.Add(name);
                }
            }
        }

        /// <summary>Birth Location Jurisdiction.</summary>
        /// <value>the vital record jurisdiction identifier.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthLocationJurisdiction = "MA";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Birth Location Jurisdiction: {ExampleBirthRecord.BirthLocationJurisdiction}");</para>
        /// </example>
        [Property("Birth Location Jurisdiction", Property.Types.String, "Birth Location", "Vital Records Jurisdiction of Birth Location (two character jurisdiction code, e.g. CA).", true, VR.IGURL.Child, false, 16)]
        // TODO - Currently not sure where the birth location would be in the record via FHIRPath, it seems different in BFDR vs VRDR. Some of the property fields above also need updating. Is this not in PatientChildVitalRecords at all and I just can't find it? There seems to be no reference to a jurisdiction location in the IG table of contents.
        [FHIRPath("Bundle.entry.resource.where($this is Location).where(type.coding.code='birth')", "")]
        public string BirthLocationJurisdiction
        {
            get
            {
                // Should addressJurisdction be used in BFDR? In VRDR, DeathLocationJurisdiction has the addressJurisdiction field, but BFDR uses the US Core Place of Birth, which does not include a jurisdiction.
                // If addressJurisdiction is present use it, otherwise return the addressState
                if (PlaceOfBirth.ContainsKey("addressJurisdiction") && !String.IsNullOrWhiteSpace(PlaceOfBirth["addressJurisdiction"]))
                {
                    return PlaceOfBirth["addressJurisdiction"];
                }
                if (PlaceOfBirth.ContainsKey("addressState") && !String.IsNullOrWhiteSpace(PlaceOfBirth["addressState"]))
                {
                    return PlaceOfBirth["addressState"];
                }
                return null;
            }
            set
            {
                // If the jurisdiction is YC (New York City) set the addressJurisdiction to YC and the addressState to NY, otherwise set both to the same;
                // setting the addressJurisdiction is technically optional but the way we use DeathLocationAddress (PlaceOfBirth for BFDR) to constantly read the existing values
                // when adding new values means that having both set correctly is important for consistency
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Dictionary<string, string> currentAddress = PlaceOfBirth;
                    // TODO - determine how YC/Jurisdictional checks should be handled and update. For now, this just sets the state of the PlaceOfBirth.
                    // if (value == "YC")
                    // {
                    //     currentAddress["addressJurisdiction"] = value;
                    //     currentAddress["addressState"] = "NY";
                    // }
                    // else
                    // {
                    currentAddress["addressJurisdiction"] = value;
                    currentAddress["addressState"] = value;
                    // }
                    PlaceOfBirth = currentAddress;
                    // TODO - UpdateDeathRecordIdentifier();
                }
            }
        }

        /// <summary>Child's Place Of Birth.</summary>
        /// <value>Child's Place Of Birth. A Dictionary representing residence address, containing the following key/value pairs:
        /// <para>"addressLine1" - address, line one</para>
        /// <para>"addressLine2" - address, line two</para>
        /// <para>"addressCity" - address, city</para>
        /// <para>"addressCounty" - address, county</para>
        /// <para>"addressState" - address, state</para>
        /// <para>"addressZip" - address, zip</para>
        /// <para>"addressCountry" - address, country</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; address = new Dictionary&lt;string, string&gt;();</para>
        /// <para>address.Add("addressLine1", "123 Test Street");</para>
        /// <para>address.Add("addressLine2", "Unit 3");</para>
        /// <para>address.Add("addressCity", "Boston");</para>
        /// <para>address.Add("addressCounty", "Suffolk");</para>
        /// <para>address.Add("addressState", "MA");</para>
        /// <para>address.Add("addressZip", "12345");</para>
        /// <para>address.Add("addressCountry", "US");</para>
        /// <para>ExampleBirthRecord.PlaceOfBirth = address;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"State where child was born: {ExampleBirthRecord.PlaceOfBirth["placeOfBirthState"]}");</para>
        /// </example>
        [Property("Place Of Birth", Property.Types.Dictionary, "Child Demographics", "Child's Place Of Birth.", true, VR.IGURL.Child, true, 15)]
        [PropertyParam("addressLine1", "address, line one")]
        [PropertyParam("addressLine2", "address, line two")]
        [PropertyParam("addressCity", "address, city")]
        [PropertyParam("addressCounty", "address, county")]
        [PropertyParam("addressCountyC", "address, county code")]
        [PropertyParam("addressState", "address, state")]
        [PropertyParam("addressZip", "address, zip")]
        [PropertyParam("addressCountry", "address, country")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url='" + OtherExtensionURL.PatientBirthPlace + "')", "")]
        public Dictionary<string, string> PlaceOfBirth
        {
            get
            {
                return GetPlaceOfBirth(Child);
            }
            set
            {
                SetPlaceOfBirth(Child, value);
            }
        }

        /// <summary>Child's Place Of Birth Type.</summary>
        /// <value>Place Where Birth Occurred, type of place or institution. A Dictionary representing a codeable concept of the physical location type:
        /// <para>"code" - The code used to describe this concept.</para>
        /// <para>"system" - The relevant code system.</para>
        /// <para>"display" - The human readable version of this code.</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; locationType = new Dictionary&lt;string, string&gt;();</para>
        /// <para>locationType.Add("code", "22232009");</para>
        /// <para>locationType.Add("system", "http://snomed.info/sct");</para>
        /// <para>locationType.Add("display", "Hospital");</para>
        /// <para>ExampleBirthRecord.BirthPhysicalLocation = locationType;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"The place type the child was born: {ExampleBirthRecord.BirthPhysicalLocation["code"]}");</para>
        /// </example>
        [Property("BirthPhysicalLocation", Property.Types.Dictionary, "Birth Physical Location", "Birth Physical Location.", true, IGURL.EncounterBirth, true, 16)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter)", "")]
        public Dictionary<string, string> BirthPhysicalLocation
        {
            get
            {
                if (EncounterBirth == null)
                {
                    return EmptyCodeableDict();
                }
                return CodeableConceptToDict(EncounterBirth.Location.Select(loc => loc.PhysicalType).FirstOrDefault());
            }
            set
            {
                if (EncounterBirth == null)
                {
                    EncounterBirth = new Encounter()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Meta = new Meta()
                    };
                    EncounterBirth.Meta.Profile = new List<string>()
                    {
                        ProfileURL.EncounterBirth
                    };
                }
                EncounterBirth.Location = new List<Hl7.Fhir.Model.Encounter.LocationComponent>();
                LocationComponent location = new LocationComponent
                {
                    PhysicalType = DictToCodeableConcept(value)
                };
                EncounterBirth.Location.Add(location);
            }
        }

        /// <summary>Child's Place Of Birth Type Helper</summary>
        /// <value>Child's Place Of Birth Type Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthPhysicalLocationHelper = "Hospital";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child's Place Of Birth Type: {ExampleBirthRecord.BirthPhysicalLocationHelper}");</para>
        /// </example>
        [Property("BirthPhysicalLocationHelper", Property.Types.String, "Birth Physical Location", "Birth Physical Location Helper.", false, IGURL.EncounterBirth, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(meta.profile == " + IGURL.EncounterBirth + ")", "")]
        public string BirthPhysicalLocationHelper
        {
            get
            {
                if (BirthPhysicalLocation.ContainsKey("code"))
                {
                    string code = BirthPhysicalLocation["code"];
                    if (code == "OTH")
                    {
                        if (BirthPhysicalLocation.ContainsKey("text") && !String.IsNullOrWhiteSpace(BirthPhysicalLocation["text"]))
                        {
                            return BirthPhysicalLocation["text"];
                        }
                        return "Other";
                    }
                    else if (!String.IsNullOrWhiteSpace(code))
                    {
                        return code;
                    }
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    // do nothing
                    return;
                }
                if (!BFDR.Mappings.BirthDeliveryOccurred.FHIRToIJE.ContainsKey(value))
                {
                    // other
                    Console.WriteLine("Warning: given 'child's place of birth type' code not found in value set. Setting value to 'Other'.");
                    BirthPhysicalLocation = CodeableConceptToDict(new CodeableConcept(CodeSystems.NullFlavor_HL7_V3, "OTH", "Other", value));
                }
                else
                {
                    // normal path
                    SetCodeValue("BirthPhysicalLocation", value, BFDR.ValueSets.BirthDeliveryOccurred.Codes);
                }
            }
        }

        /// <summary>Mother's Place Of Birth.</summary>
        /// <value>Mother's Place Of Birth. A Dictionary representing residence address, containing the following key/value pairs:
        /// <para>"addressLine1" - address, line one</para>
        /// <para>"addressLine2" - address, line two</para>
        /// <para>"addressCity" - address, city</para>
        /// <para>"addressCounty" - address, county</para>
        /// <para>"addressState" - address, state</para>
        /// <para>"addressZip" - address, zip</para>
        /// <para>"addressCountry" - address, country</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; address = new Dictionary&lt;string, string&gt;();</para>
        /// <para>address.Add("addressLine1", "123 Test Street");</para>
        /// <para>address.Add("addressLine2", "Unit 3");</para>
        /// <para>address.Add("addressCity", "Boston");</para>
        /// <para>address.Add("addressCounty", "Suffolk");</para>
        /// <para>address.Add("addressState", "MA");</para>
        /// <para>address.Add("addressZip", "12345");</para>
        /// <para>address.Add("addressCountry", "US");</para>
        /// <para>ExampleBirthRecord.MotherPlaceOfBirth = address;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"State where mother was born: {ExampleBirthRecord.MotherPlaceOfBirth["placeOfBirthState"]}");</para>
        /// </example>
        [Property("Mother's Place Of Birth", Property.Types.Dictionary, "Mother Demographics", "Mother's Place Of Birth.", true, VR.IGURL.Mother, true, 305)]
        [PropertyParam("addressLine1", "address, line one")]
        [PropertyParam("addressLine2", "address, line two")]
        [PropertyParam("addressCity", "address, city")]
        [PropertyParam("addressCounty", "address, county")]
        [PropertyParam("addressState", "address, state")]
        [PropertyParam("addressZip", "address, zip")]
        [PropertyParam("addressCountry", "address, country")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url='" + OtherExtensionURL.PatientBirthPlace + "')", "")]
        public Dictionary<string, string> MotherPlaceOfBirth
        {
            get => GetPlaceOfBirth(Mother);
            set => SetPlaceOfBirth(Mother, value);
        }

        /// <summary>Father's Place Of Birth.</summary>
        /// <value>Father's Place Of Birth. A Dictionary representing residence address, containing the following key/value pairs:
        /// <para>"addressLine1" - address, line one</para>
        /// <para>"addressLine2" - address, line two</para>
        /// <para>"addressCity" - address, city</para>
        /// <para>"addressCounty" - address, county</para>
        /// <para>"addressState" - address, state</para>
        /// <para>"addressZip" - address, zip</para>
        /// <para>"addressCountry" - address, country</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; address = new Dictionary&lt;string, string&gt;();</para>
        /// <para>address.Add("addressLine1", "123 Test Street");</para>
        /// <para>address.Add("addressLine2", "Unit 3");</para>
        /// <para>address.Add("addressCity", "Boston");</para>
        /// <para>address.Add("addressCounty", "Suffolk");</para>
        /// <para>address.Add("addressState", "MA");</para>
        /// <para>address.Add("addressZip", "12345");</para>
        /// <para>address.Add("addressCountry", "US");</para>
        /// <para>ExampleBirthRecord.FatherPlaceOfBirth = address;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"State where father was born: {ExampleBirthRecord.FatherPlaceOfBirth["placeOfBirthState"]}");</para>
        /// </example>
        [Property("Father's Place Of Birth", Property.Types.Dictionary, "Father Demographics", "Father's Place Of Birth.", true, VR.IGURL.RelatedPersonFather, true, 291)]
        [PropertyParam("addressLine1", "address, line one")]
        [PropertyParam("addressLine2", "address, line two")]
        [PropertyParam("addressCity", "address, city")]
        [PropertyParam("addressCounty", "address, county")]
        [PropertyParam("addressState", "address, state")]
        [PropertyParam("addressZip", "address, zip")]
        [PropertyParam("addressCountry", "address, country")]
        [FHIRPath("Bundle.entry.resource.where($this is RelatedPerson).extension.where(url='" + OtherExtensionURL.RelatedPersonBirthPlace + "')", "")]
        public Dictionary<string, string> FatherPlaceOfBirth
        {
            get => GetPlaceOfBirth(Father);
            set => SetPlaceOfBirth(Father, value);
        }

        /// <summary>Mother's Residence.</summary>
        /// <value>Mother's Residence. A Dictionary representing residence address, containing the following key/value pairs:
        /// <para>"addressLine1" - address, line one</para>
        /// <para>"addressLine2" - address, line two</para>
        /// <para>"addressCity" - address, city</para>
        /// <para>"addressCounty" - address, county</para>
        /// <para>"addressState" - address, state</para>
        /// <para>"addressZip" - address, zip</para>
        /// <para>"addressCountry" - address, country</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; address = new Dictionary&lt;string, string&gt;();</para>
        /// <para>address.Add("addressLine1", "123 Test Street");</para>
        /// <para>address.Add("addressLine2", "Unit 3");</para>
        /// <para>address.Add("addressCity", "Boston");</para>
        /// <para>address.Add("addressCounty", "Suffolk");</para>
        /// <para>address.Add("addressState", "MA");</para>
        /// <para>address.Add("addressZip", "12345");</para>
        /// <para>address.Add("addressCountry", "US");</para>
        /// <para>ExampleBirthRecord.MotherResidence = address;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"State where mother resides: {ExampleBirthRecord.MotherResidence["addressState"]}");</para>
        /// </example>
        [Property("Mother's Residence", Property.Types.Dictionary, "Mother Demographics", "Mother's Residence.", true, VR.IGURL.Mother, true, 20)]
        [PropertyParam("addressLine1", "address, line one")]
        [PropertyParam("addressLine2", "address, line two")]
        [PropertyParam("addressCity", "address, city")]
        [PropertyParam("addressCounty", "address, county")]
        [PropertyParam("addressState", "address, state")]
        [PropertyParam("addressZip", "address, zip")]
        [PropertyParam("addressCountry", "address, country")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "address")]
        public Dictionary<string, string> MotherResidence
        {
            get => AddressToDict(Mother?.Address.Find(addr => addr.Use == Address.AddressUse.Home));
            set
            {
                Address billing = Mother.Address.Find(addr => addr.Use == Address.AddressUse.Billing);
                Mother.Address.Clear();
                Address residence = DictToAddress(value);
                residence.Use = Address.AddressUse.Home;
                Mother.Address.Add(residence);
                if (billing != null)
                {
                    Mother.Address.Add(billing);
                }
            }
        }

        /// <summary>Mother's Billing.</summary>
        /// <value>Mother's Billing. A Dictionary representing billing address, containing the following key/value pairs:
        /// <para>"addressLine1" - address, line one</para>
        /// <para>"addressLine2" - address, line two</para>
        /// <para>"addressCity" - address, city</para>
        /// <para>"addressCounty" - address, county</para>
        /// <para>"addressState" - address, state</para>
        /// <para>"addressZip" - address, zip</para>
        /// <para>"addressCountry" - address, country</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; address = new Dictionary&lt;string, string&gt;();</para>
        /// <para>address.Add("addressLine1", "123 Test Street");</para>
        /// <para>address.Add("addressLine2", "Unit 3");</para>
        /// <para>address.Add("addressCity", "Boston");</para>
        /// <para>address.Add("addressCounty", "Suffolk");</para>
        /// <para>address.Add("addressState", "MA");</para>
        /// <para>address.Add("addressZip", "12345");</para>
        /// <para>address.Add("addressCountry", "US");</para>
        /// <para>ExampleBirthRecord.MotherBilling = address;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"State where mother is billed: {ExampleBirthRecord.MotherBilling["addressState"]}");</para>
        /// </example>
        [Property("Mother's Billing Address", Property.Types.Dictionary, "Mother Demographics", "Mother's Billing Address.", true, VR.IGURL.Mother, true, 20)]
        [PropertyParam("addressLine1", "address, line one")]
        [PropertyParam("addressLine2", "address, line two")]
        [PropertyParam("addressCity", "address, city")]
        [PropertyParam("addressCounty", "address, county")]
        [PropertyParam("addressState", "address, state")]
        [PropertyParam("addressZip", "address, zip")]
        [PropertyParam("addressCountry", "address, country")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "address")]
        public Dictionary<string, string> MotherBilling
        {
            get => AddressToDict(Mother?.Address.Find(addr => addr.Use == Address.AddressUse.Billing));
            set
            {
                Address residence = Mother.Address.Find(addr => addr.Use == Address.AddressUse.Home);
                Mother.Address.Clear();
                Address billing = DictToAddress(value);
                billing.Use = Address.AddressUse.Billing;
                Mother.Address.Add(billing);
                if (residence != null)
                {
                    Mother.Address.Add(residence);
                }
            }
        }

        /// <summary>Mother's residence is/is not within city limits.</summary>
        /// <value>Mother's residence is/is not within city limits. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para></value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; within = new Dictionary&lt;string, string&gt;();</para>
        /// <para>within.Add("code", "Y");</para>
        /// <para>within.Add("system", VR.CodeSystems.YesNo_0136HL7_V2);</para>
        /// <para>within.Add("display", "Yes");</para>
        /// <para>ExampleBirthRecord.MotherResidenceWithinCityLimits = within;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Residence within city limits: {ExampleBirthRecord.MotherResidenceWithinCityLimits['display']}");</para>
        /// </example>
        [Property("Mother Residence Within City Limits", Property.Types.Dictionary, "Mother Demographics", "Mother's residence is/is not within city limits.", true, VR.IGURL.Mother, true, 24)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "address")]
        public Dictionary<string, string> MotherResidenceWithinCityLimits
        {
            get
            {
                Address residence = Mother?.Address.Find(addr => addr.Use == Address.AddressUse.Home);
                if (residence != null)
                {
                    Extension cityLimits = residence.Extension.Where(ext => ext.Url == VRExtensionURLs.WithinCityLimitsIndicator).FirstOrDefault();
                    if (cityLimits != null && cityLimits.Value != null && cityLimits.Value as Coding != null)
                    {
                        return CodingToDict((Coding)cityLimits.Value);
                    }
                }
                return EmptyCodeDict();
            }
            set
            {
                if (Mother != null)
                {
                    Address residence = Mother.Address.Find(addr => addr.Use == Address.AddressUse.Home);
                    if (residence == null)
                    {
                        residence = new Address
                        {
                            Use = Address.AddressUse.Home
                        };
                        Mother.Address.Add(residence);
                    }
                    residence.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.WithinCityLimitsIndicator);
                    Extension withinCityLimits = new Extension
                    {
                        Url = VRExtensionURLs.WithinCityLimitsIndicator,
                        Value = DictToCoding(value)
                    };
                    residence.Extension.Add(withinCityLimits);
                }
            }
        }

        /// <summary>Mother's Residence Within City Limits Helper</summary>
        /// <value>Mother Residence Within City Limits.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherResidenceWithinCityLimitsHelper = VRDR.ValueSets.YesNoUnknown.Y;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Residence within city limits: {ExampleBirthRecord.MotherResidenceWithinCityLimitsHelper}");</para>
        /// </example>
        [Property("MotherResidenceWithinCityLimits Helper", Property.Types.String, "Mother Demographics", "Mother's ResidenceWithinCityLimits.", false, VR.IGURL.Mother, false, 24)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "address")]
        public string MotherResidenceWithinCityLimitsHelper
        {
            get
            {
                if (MotherResidenceWithinCityLimits.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherResidenceWithinCityLimits["code"]))
                {
                    return MotherResidenceWithinCityLimits["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherResidenceWithinCityLimits", value, VR.ValueSets.YesNoUnknown.Codes);
                }
            }
        }

        /// <summary>Infant's Medical Record Number.</summary>
        /// <value>Infant's Medical Record Number.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.InfantMedicalRecordNumber = "aaabbbcccdddeee";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child's InfantMedicalRecordNumber: {ExampleBirthRecord.InfantMedicalRecordNumber}");</para>
        /// </example>
        [Property("Infant's Medical Record Number", Property.Types.String, "Child Demographics", "Infant's Medical Record Number", true, VR.IGURL.Child, true, 34)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).identifier.where(url=CodeSystems.HL7_identifier_type and type.coding.code='MR').value", "")]
        public string InfantMedicalRecordNumber
        {
            get
            {
                return Child?.Identifier?.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == CodeSystems.HL7_identifier_type && idCoding.Code == "MR"))?.Value;
            }
            set
            {
                if (Child.Identifier.Any(id => id.Type.Coding.Any(idCoding => idCoding.System == CodeSystems.HL7_identifier_type && idCoding.Code == "MR")))
                {
                    Child.Identifier.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == CodeSystems.HL7_identifier_type && idCoding.Code == "MR")).Value = value;
                }
                else
                {
                    Coding coding = new Coding
                    {
                        System = CodeSystems.HL7_identifier_type,
                        Code = "MR",
                        Display = "Medical Record Number"
                    };
                    CodeableConcept medicalRecordNumber = new CodeableConcept();
                    medicalRecordNumber.Coding.Add(coding);
                    Identifier identifier = new Identifier
                    {
                        Type = medicalRecordNumber,
                        Value = value
                    };
                    Child.Identifier.Add(identifier);
                }
            }
        }

        /// <summary>Mother's Medical Record Number.</summary>
        /// <value>Mother's Medical Record Number.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherMedicalRecordNumber = "aaabbbcccdddeee";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's MedicalRecordNumber: {ExampleBirthRecord.MotherMedicalRecordNumber}");</para>
        /// </example>
        [Property("Mother's Medical Record Number", Property.Types.String, "Mother Demographics", "Mother's Medical Record Number", true, VR.IGURL.Mother, true, 333)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).identifier.where(url=CodeSystems.HL7_identifier_type and type.coding.code='MR').value", "")]
        public string MotherMedicalRecordNumber
        {
            get
            {
                return Mother?.Identifier?.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == CodeSystems.HL7_identifier_type && idCoding.Code == "MR"))?.Value;
            }
            set
            {
                if (Mother.Identifier.Any(id => id.Type.Coding.Any(idCoding => idCoding.System == CodeSystems.HL7_identifier_type && idCoding.Code == "MR")))
                {
                    Mother.Identifier.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == CodeSystems.HL7_identifier_type && idCoding.Code == "MR")).Value = value;
                }
                else
                {
                    Coding coding = new Coding
                    {
                        System = CodeSystems.HL7_identifier_type,
                        Code = "MR",
                        Display = "Medical Record Number"
                    };
                    CodeableConcept medicalRecordNumber = new CodeableConcept();
                    medicalRecordNumber.Coding.Add(coding);
                    Identifier identifier = new Identifier
                    {
                        Type = medicalRecordNumber,
                        Value = value
                    };
                    Mother.Identifier.Add(identifier);
                }
            }
        }

        private static string GetSSN(List<Identifier> ids)
        {
            return ids?.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == CodeSystems.HL7_identifier_type && idCoding.Code == "SS"))?.Value;
        }

        private static void SetSSN(List<Identifier> ids, string ssn)
        {
            if (ids.Any(id => id.Type.Coding.Any(idCoding => idCoding.System == CodeSystems.HL7_identifier_type && idCoding.Code == "SS")))
            {
                if (String.IsNullOrWhiteSpace(ssn))
                {
                    ids.RemoveAll(id => id.Type.Coding.Any(idCoding => idCoding.System == CodeSystems.HL7_identifier_type && idCoding.Code == "SS"));
                }
                else 
                {
                ids.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == CodeSystems.HL7_identifier_type && idCoding.Code == "SS")).Value = ssn;
                } 
            }
            else if (String.IsNullOrWhiteSpace(ssn))
            {
                return;
            }
            else
            {
                Coding coding = new Coding
                {
                    System = CodeSystems.HL7_identifier_type,
                    Code = "SS",
                    Display = "Social Security Number"
                };
                CodeableConcept socialSecurityNumber = new CodeableConcept();
                socialSecurityNumber.Coding.Add(coding);
                Identifier identifier = new Identifier
                {
                    Type = socialSecurityNumber,
                    Value = ssn
                };
                ids.Add(identifier);
            }
        }

        /// <summary>Mother's Social Security Number.</summary>
        /// <value>Mother's Social Security Number.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherSocialSecurityNumber = "123456789";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's SocialSecurityNumber: {ExampleBirthRecord.MotherSocialSecurityNumber}");</para>
        /// </example>
        [Property("Mother's Social Security Number", Property.Types.String, "Mother Demographics", "Mother's Social Security Number", true, VR.IGURL.Mother, true, 278)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).identifier.where(url=CodeSystems.HL7_identifier_type and type.coding.code='SS').value", "")]
        public string MotherSocialSecurityNumber
        {
            get => GetSSN(Mother?.Identifier);
            set => SetSSN(Mother.Identifier, value);
        }

        /// <summary>Father's Social Security Number.</summary>
        /// <value>Father's Social Security Number.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherSocialSecurityNumber = "123456789";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's SocialSecurityNumber: {ExampleBirthRecord.FatherSocialSecurityNumber}");</para>
        /// </example>
        [Property("Father's Social Security Number", Property.Types.String, "Father Demographics", "Father's Social Security Number", true, VR.IGURL.RelatedPersonFatherNatural, true, 279)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).identifier.where(url=CodeSystems.HL7_identifier_type and type.coding.code='SS').value", "")]
        public string FatherSocialSecurityNumber
        {
            get => GetSSN(Father?.Identifier);
            set => SetSSN(Father.Identifier, value);
        }

        /// <summary>Multiple birth set order</summary>
        /// <value>The order that the child was born if a multiple birth or null if it was a single birth</value>
        /// <example>
        /// <para>ExampleBirthRecord.SetOrder = null; // single birth</para>
        /// <para>ExampleBirthRecord.SetOrder = -1; // unknow whether single or multiple birth</para>
        /// <para>ExampleBirthRecord.SetOrder = 1; // multiple birth, born first</para>
        /// </example>
        [Property("SetOrder", Property.Types.Int32, "Child Demographics", "Child Demographics, Set Order", true, VR.IGURL.Child, true, 208)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "multipleBirth")]
        public int? SetOrder
        {
            get
            {
                if (Child != null && Child.MultipleBirth != null)
                {
                    if (Child.MultipleBirth as FhirBoolean != null)
                    {
                        return null;
                    }
                    else if (Child.MultipleBirth as Hl7.Fhir.Model.Integer != null && (Child.MultipleBirth as Hl7.Fhir.Model.Integer).Value != null)
                    {
                        return (Child.MultipleBirth as Hl7.Fhir.Model.Integer).Value;
                    }
                    else if (Child.MultipleBirth.Extension.Find(ext => ext.Url == ExtensionURL.DataAbsentReason) != null)
                    {
                        return -1;
                    }
                }
                return null;
            }
            set
            {
                Dictionary<string, string> pluralityEditFlag = PluralityEditFlag;
                int? plurality = Plurality;
                if (value == null)
                {
                    Child.MultipleBirth = new FhirBoolean(false);
                }
                else if (value == -1)
                {
                    Child.MultipleBirth = new Hl7.Fhir.Model.Integer();
                    Extension missingValueReason = new Extension(ExtensionURL.DataAbsentReason, new Code("unknown"));
                    Child.MultipleBirth.Extension.Add(missingValueReason);
                }
                else
                {
                    Child.MultipleBirth = new Hl7.Fhir.Model.Integer(value);
                }
                Plurality = plurality;
                PluralityEditFlag = pluralityEditFlag;
            }
        }

        /// <summary>Multiple birth set order edit flag</summary>
        /// <value>the multiple birth set order edit flag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; route = new Dictionary&lt;string, string&gt;();</para>
        /// <para>route.Add("code", "queriedCorrect");</para>
        /// <para>route.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");</para>
        /// <para>route.Add("display", "Queried, and Correct");</para>
        /// <para>ExampleBirthRecord.PluralityEditFlag = route;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Multiple birth set order edit flag: {ExampleBirthRecord.PluralityEditFlag}");</para>
        /// </example>
        [Property("PluralityEditFlag", Property.Types.Dictionary, "Child Demographics", "Child Demographics, Plurality Edit Flag", true, VR.IGURL.Child, true, 211)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).multipleBirth.extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag')", "")]
        public Dictionary<string, string> PluralityEditFlag
        {
            get
            {
                if (Child != null && Child.MultipleBirth != null)
                {
                    Extension pluralityEditFlag = Child.MultipleBirth.Extension.Find(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                    if (pluralityEditFlag != null && pluralityEditFlag.Value != null && pluralityEditFlag.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)pluralityEditFlag.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (Child.MultipleBirth == null)
                {
                    Child.MultipleBirth = new FhirBoolean(false);
                }
                Child.MultipleBirth.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                Child.MultipleBirth.Extension.Add(new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(value)));
            }
        }

        /// <summary>Multiple birth set order edit flag helper</summary>
        /// <value>the multiple birth set order edit flag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.PluralityEditFlagHelper = "queriedCorrect";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Multiple birth set order edit flag: {ExampleBirthRecord.PluralityEditFlagHelper}");</para>
        /// </example>
        [Property("PluralityEditFlagHelper", Property.Types.String, "Child Demographics", "Child Demographics, Plurality Edit Flag", false, VR.IGURL.Child, true, 211)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).multipleBirth.extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag')", "")]
        public string PluralityEditFlagHelper
        {
            get
            {
                if (PluralityEditFlag.ContainsKey("code"))
                {
                    string code = PluralityEditFlag["code"];
                    if (!String.IsNullOrWhiteSpace(code))
                    {
                        return code;
                    }
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    SetCodeValue("PluralityEditFlag", value, VR.ValueSets.PluralityEditFlags.Codes);
                }
            }
        }


        /// <summary>Multiple birth plurality</summary>
        /// <value>Where a patient is a part of a multiple birth, this is the total number of births that occurred in this pregnancy.</value>
        /// <example>
        /// <para>ExampleBirthRecord.Plurality = null; // single birth</para>
        /// <para>ExampleBirthRecord.Plurality = -1; // unknown number of births birth</para>
        /// <para>ExampleBirthRecord.Plurality = 2; // two births for this pregnancy</para>
        /// </example>
        [Property("Plurality", Property.Types.Int32, "Child Demographics", "Child Demographics, Plurality", true, VR.IGURL.Child, true, 207)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).multipleBirth.extension.where(url = 'http://hl7.org/fhir/StructureDefinition/patient-multipleBirthTotal')", "")]
        public int? Plurality
        {
            get
            {
                if (Child != null && Child.MultipleBirth != null)
                {
                    Extension plurality = Child.MultipleBirth.Extension.Find(ext => ext.Url == ExtensionURL.Plurality);
                    if (plurality != null)
                    {
                        if (plurality.Value as PositiveInt != null && (plurality.Value as PositiveInt).Value != null)
                        {
                            return (plurality.Value as PositiveInt).Value;
                        }
                        else if (plurality.Extension.Find(ext => ext.Url == ExtensionURL.DataAbsentReason) != null)
                        {
                            return -1;
                        }
                    }
                }
                return null;
            }
            set
            {
                if (Child.MultipleBirth == null)
                {
                    Child.MultipleBirth = new FhirBoolean(false);
                }
                Child.MultipleBirth.Extension.RemoveAll(ext => ext.Url == ExtensionURL.Plurality);
                if (value == null)
                {
                    return;
                }
                else if (value == -1)
                {
                    Extension plurality = new Extension(ExtensionURL.Plurality, new PositiveInt());
                    Extension missingValueReason = new Extension(ExtensionURL.DataAbsentReason, new Code("unknown"));
                    plurality.Extension.Add(missingValueReason);
                    Child.MultipleBirth.Extension.Add(plurality);
                }
                else
                {
                    Extension plurality = new Extension(ExtensionURL.Plurality, new PositiveInt(value));
                    Child.MultipleBirth.Extension.Add(plurality);
                }
            }
        }

        //
        // Congenital Anomalies of the Newborn Section
        //

        /// <summary>No Congenital Anomalies of the Newborn.</summary>
        [Property("No Congenital Anomalies of the Newborn", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "No Congenital Anomalies of the Newborn", true, IGURL.ObservationNoneOfSpecifiedCongenitalAnomoliesOfTheNewborn, false, 219)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73780-9", code: VitalRecord.NONE_OF_THE_ABOVE, section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool NoCongenitalAnomaliesOfTheNewborn
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Anencephaly.</summary>
        [Property("Anencephaly", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Anencephaly", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 219)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "89369001", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool Anencephaly
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Cleft Lip with or without Cleft Palate.</summary>
        [Property("Cleft Lip with or without Cleft Palate", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Cleft Lip with or without Cleft Palate", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 226)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "80281008", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool CleftLipWithOrWithoutCleftPalate
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Cleft Palate Alone.</summary>
        [Property("Cleft Palate Alone", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Cleft Palate Alone", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 227)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "87979003", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool CleftPalateAlone
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Congenital Diaphragmatic Hernia.</summary>
        [Property("Congenital Diaphragmatic Hernia", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Congenital Diaphragmatic Hernia", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 222)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "17190001", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool CongenitalDiaphragmaticHernia
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Cyanotic Congenital Heart Disease.</summary>
        [Property("Cyanotic Congenital Heart Disease", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Cyanotic Congenital Heart Disease", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 221)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "12770006", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool CyanoticCongenitalHeartDisease
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Down Syndrome.</summary>
        [Property("Down Syndrome", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Down Syndrome", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 228)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "70156005", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool DownSyndrome
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Gastroschisis.</summary>
        [Property("Gastroschisis", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Gastroschisis", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 224)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "72951007", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool Gastroschisis
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Hypospadias.</summary>
        [Property("Hypospadias", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Hypospadias", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 230)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "416010008", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool Hypospadias
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Limb Reduction Defect.</summary>
        [Property("Limb Reduction Defect", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Limb Reduction Defect", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 225)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "67341007", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool LimbReductionDefect
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Meningomyelocele.</summary>
        [Property("Meningomyelocele", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Meningomyelocele", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 220)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "67531005", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool Meningomyelocele
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Omphalocele.</summary>
        [Property("Omphalocele", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Omphalocele", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 223)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "18735004", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool Omphalocele
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Congenital Anomalies of the Newborn, Suspected Chromosomal Disorder.</summary>
        [Property("Suspected Chromosomal Disorder", Property.Types.Bool, "Congenital Anomalies of the Newborn",
                  "Congenital Anomalies of the Newborn, Suspected Chromosomal Disorder", true, IGURL.ConditionCongenitalAnomalyOfNewborn, true, 229)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73780-9", code: "409709004", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool SuspectedChromosomalDisorder
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        //
        // Characteristics of Labor and Delivery Section
        //

        /// <summary>No Characteristics of Labor and Delivery.</summary>
        [Property("No Characteristics of Labor and Delivery", Property.Types.Bool, "Characteristics of Labor and Delivery",
                  "No Characteristics of Labor and Delivery", true, IGURL.ObservationNoneOfSpecifiedCharacteristicsOfLaborAndDelivery, false, 185)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73813-8", code: VitalRecord.NONE_OF_THE_ABOVE, section: MEDICAL_INFORMATION_SECTION)]
        public bool NoCharacteristicsOfLaborAndDelivery
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Characteristics of Labor and Delivery, Epidural or Spinal Anesthesia.</summary>
        [Property("Epidural or Spinal Anesthesia", Property.Types.Bool, "Characteristics of Labor and Delivery",
                  "Characteristics of Labor and Delivery, Epidural or Spinal Anesthesia", true, IGURL.ProcedureEpiduralOrSpinalAnesthesia, true, 189)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73813-8", code: "18946005", section: MEDICAL_INFORMATION_SECTION)]
        public bool EpiduralOrSpinalAnesthesia
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Characteristics of Labor and Delivery, Antibiotics Administered During Labor.</summary>
        [Property("Antibiotics Administered During Labor", Property.Types.Bool, "Characteristics of Labor and Delivery",
                  "Characteristics of Labor and Delivery, Antibiotics Administered During Labor", true, IGURL.ObservationAntibioticsAdministeredDuringLabor, true, 185)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73813-8", code: "434691000124101", section: MEDICAL_INFORMATION_SECTION)]
        public bool AntibioticsAdministeredDuringLabor
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Characteristics of Labor and Delivery, Augmentation of Labor.</summary>
        [Property("Augmentation Of Labor", Property.Types.Bool, "Characteristics of Labor and Delivery",
                  "Characteristics of Labor and Delivery, Augmentation Of Labor", true, IGURL.ProcedureAugmentationOfLabor, true, 182)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73813-8", code: "237001001", section: MEDICAL_INFORMATION_SECTION)]
        public bool AugmentationOfLabor
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Characteristics of Labor and Delivery, Chorioamnionitis.</summary>
        [Property("Chorioamnionitis", Property.Types.Bool, "Characteristics of Labor and Delivery",
                  "Characteristics of Labor and Delivery, Chorioamnionitis", true, IGURL.ConditionChorioamnionitis, true, 186)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73813-8", code: "11612004", section: MEDICAL_INFORMATION_SECTION)]
        public bool Chorioamnionitis
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Characteristics of Labor and Delivery, Induction of Labor.</summary>
        [Property("Induction Of Labor", Property.Types.Bool, "Characteristics of Labor and Delivery",
                  "Characteristics of Labor and Delivery, Induction Of Labor", true, IGURL.ProcedureInductionOfLabor, true, 181)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73813-8", code: "236958009", section: MEDICAL_INFORMATION_SECTION)]
        public bool InductionOfLabor
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Characteristics of Labor and Delivery, Administration of Steroids for Fetal Lung Maturation.</summary>
        [Property("Administration of Steroids for Fetal Lung Maturation", Property.Types.Bool, "Characteristics of Labor and Delivery",
                  "Characteristics of Labor and Delivery, Administration of Steroids for Fetal Lung Maturation", true, IGURL.ObservationSteroidsFetalLungMaturation, true, 184)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73813-8", code: "434611000124106", section: MEDICAL_INFORMATION_SECTION)]
        public bool AdministrationOfSteroidsForFetalLungMaturation
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        //
        // Abnormal Conditions of the Newborn section
        //

        /// <summary>No Specified Abnormal Conditions of Newborn.</summary>
        [Property("No Specified Abnormal Conditions of Newborn", Property.Types.Bool, "Specified Abnormal Conditions of Newborn",
                  "No Specified Abnormal Conditions of Newborn", true, IGURL.ObservationNoneOfSpecifiedAbnormalConditionsOfNewborn, false, 212)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73812-0", code: VitalRecord.NONE_OF_THE_ABOVE, section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool NoSpecifiedAbnormalConditionsOfNewborn
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Specified Abnormal Conditions of Newborn, NICU Admission.</summary>
        [Property("NICU Admission", Property.Types.Bool, "Specified Abnormal Conditions of Newborn",
                  "No Specified Abnormal Conditions of Newborn, NICU Admission", true, IGURL.ObservationNICUAdmission, true, 214)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73812-0", code: "830077005", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool NICUAdmission
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Specified Abnormal Conditions of Newborn, Antibiotic for Suspected Neonatal Sepsis.</summary>
        [Property("Antibiotic for Suspected Neonatal Sepsis", Property.Types.Bool, "Specified Abnormal Conditions of Newborn",
                  "No Specified Abnormal Conditions of Newborn, Antibiotic for Suspected Neonatal Sepsis", true, IGURL.ProcedureAntibioticSuspectedNeonatalSepsis, true, 216)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73812-0", code: "434621000124103", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool AntibioticForSuspectedNeonatalSepsis
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Specified Abnormal Conditions of Newborn, Assisted Ventilation Following Delivery.</summary>
        [Property("Assisted Ventilation Following Delivery", Property.Types.Bool, "Specified Abnormal Conditions of Newborn",
                  "No Specified Abnormal Conditions of Newborn, Assisted Ventilation Following Delivery", true, IGURL.ProcedureAssistedVentilationFollowingDelivery, true, 212)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73812-0", code: "assistedventfollowingdelivery",
                  codeSystem: CodeSystemURL.AbnormalConditionsNewborn, section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool AssistedVentilationFollowingDelivery
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Specified Abnormal Conditions of Newborn, Assisted Ventilation More Than Six Hours.</summary>
        [Property("Assisted Ventilation More Than Six Hours", Property.Types.Bool, "Specified Abnormal Conditions of Newborn",
                  "No Specified Abnormal Conditions of Newborn, Assisted Ventilation More Than Six Hours", true, IGURL.ProcedureAssistedVentilationMoreThanSixHours, true, 213)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73812-0", code: "assistedventmorethan6hrs",
                  codeSystem: CodeSystemURL.AbnormalConditionsNewborn, section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool AssistedVentilationMoreThanSixHours
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Specified Abnormal Conditions of Newborn, Seizure.</summary>
        [Property("Seizure", Property.Types.Bool, "Specified Abnormal Conditions of Newborn",
                  "No Specified Abnormal Conditions of Newborn, Seizure", true, IGURL.ConditionSeizure, true, 217)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73812-0", code: "91175000", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool Seizure
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Specified Abnormal Conditions of Newborn, Surfactant Replacement Therapy.</summary>
        [Property("Surfactant Replacement Therapy", Property.Types.Bool, "Specified Abnormal Conditions of Newborn",
                  "No Specified Abnormal Conditions of Newborn, Surfactant Replacement Therapy", true, IGURL.ProcedureSurfactantReplacementTherapy, true, 215)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73812-0", code: "434701000124101", section: NEWBORN_INFORMATION_SECTION)]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool SurfactantReplacementTherapy
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        //
        // Infections Present Section
        //

        /// <summary>No Infections Present During Pregnancy.</summary>
        [Property("No Infections Present During Pregnancy", Property.Types.Bool, "Infections Present During Pregnancy",
                  "No Infections Present During Pregnancy", true, IGURL.ObservationNoneOfSpecifiedInfectionsPresentDuringPregnancy, false, 168)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "72519-2", code: VitalRecord.NONE_OF_THE_ABOVE, section: MEDICAL_INFORMATION_SECTION)]
        public bool NoInfectionsPresentDuringPregnancy
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Infections Present During Pregnancy, Chlamydia.</summary>
        [Property("Chlamydia", Property.Types.Bool, "Infections Present During Pregnancy",
                  "Infections Present During Pregnancy, Chlamydia", true, IGURL.ConditionInfectionPresentDuringPregnancy, true, 171)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "72519-2", code: "105629000", section: MEDICAL_INFORMATION_SECTION)]
        public bool Chlamydia
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Infections Present During Pregnancy, Gonorrhea.</summary>
        [Property("Gonorrhea", Property.Types.Bool, "Infections Present During Pregnancy",
                  "Infections Present During Pregnancy, Gonorrhea", true, IGURL.ConditionInfectionPresentDuringPregnancy, true, 168)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "72519-2", code: "15628003", section: MEDICAL_INFORMATION_SECTION)]
        public bool Gonorrhea
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Infections Present During Pregnancy, Hepatitis B.</summary>
        [Property("Hepatitis B", Property.Types.Bool, "Infections Present During Pregnancy",
                  "Infections Present During Pregnancy, Hepatitis B", true, IGURL.ConditionInfectionPresentDuringPregnancy, true, 172)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "72519-2", code: "66071002", section: MEDICAL_INFORMATION_SECTION)]
        public bool HepatitisB
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Infections Present During Pregnancy, Hepatitis C.</summary>
        [Property("Hepatitis C", Property.Types.Bool, "Infections Present During Pregnancy",
                  "Infections Present During Pregnancy, Hepatitis C", true, IGURL.ConditionInfectionPresentDuringPregnancy, true, 173)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "72519-2", code: "50711007", section: MEDICAL_INFORMATION_SECTION)]
        public bool HepatitisC
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Infections Present During Pregnancy, Syphilis.</summary>
        [Property("Syphilis", Property.Types.Bool, "Infections Present During Pregnancy",
                  "Infections Present During Pregnancy, Syphilis", true, IGURL.ConditionInfectionPresentDuringPregnancy, true, 169)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "72519-2", code: "76272004", section: MEDICAL_INFORMATION_SECTION)]
        public bool Syphilis
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Infections Present During Pregnancy, Genital Herpes Simplex.</summary>
        [Property("Genital Herpes Simplex", Property.Types.Bool, "Infections Present During Pregnancy",
                  "Infections Present During Pregnancy, Genital Herpes Simplex", true, IGURL.ConditionInfectionPresentDuringPregnancy, false, 173)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "72519-2", code: "33839006", section: MEDICAL_INFORMATION_SECTION)]
        public bool GenitalHerpesSimplex
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        //
        // Maternal Morbidity Section
        //

        /// <summary>No Maternal Morbidities.</summary>
        [Property("No Maternal Morbidities", Property.Types.Bool, "Maternal Morbidities",
                  "Maternal Morbidities, None", true, IGURL.ObservationNoneOfSpecifiedMaternalMorbidities, false, 195)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73781-7", code: VitalRecord.NONE_OF_THE_ABOVE, section: MEDICAL_INFORMATION_SECTION)]
        public bool NoMaternalMorbidities
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>ICU Admission.</summary>
        [Property("ICU Admission", Property.Types.Bool, "Maternal Morbidities",
                  "Maternal Morbidities, ICU Admission", true, IGURL.ObservationICUAdmission, true, 199)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73781-7", code: "309904001", section: MEDICAL_INFORMATION_SECTION)]
        public bool ICUAdmission
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Maternal Transfusion.</summary>
        [Property("Maternal Transfusion", Property.Types.Bool, "Maternal Morbidities",
                  "Maternal Morbidities, Maternal Transfusion", true, IGURL.ProcedureBloodTransfusion, true, 195)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73781-7", code: "116859006", section: MEDICAL_INFORMATION_SECTION)]
        public bool MaternalTransfusion
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Perineal Laceration.</summary>
        [Property("Perineal Laceration", Property.Types.Bool, "Maternal Morbidities",
                  "Maternal Morbidities, Perineal Laceration", true, IGURL.ConditionPerinealLaceration, true, 196)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73781-7", code: "398019008", section: MEDICAL_INFORMATION_SECTION)]
        public bool PerinealLaceration
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Ruptured Uterus.</summary>
        [Property("Ruptured Uterus", Property.Types.Bool, "Maternal Morbidities",
                  "Maternal Morbidities, Ruptured Uterus", true, IGURL.ConditionRupturedUterus, true, 197)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73781-7", code: "34430009", section: MEDICAL_INFORMATION_SECTION)]
        public bool RupturedUterus
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Unplanned Hysterectomy.</summary>
        [Property("Unplanned Hysterectomy", Property.Types.Bool, "Maternal Morbidities",
                  "Maternal Morbidities, Unplanned Hysterectomy", true, IGURL.ProcedureUnplannedHysterectomy, true, 198)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73781-7", code: "236987005", section: MEDICAL_INFORMATION_SECTION)]
        public bool UnplannedHysterectomy
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        //
        // Risk Factors Section
        //

        /// <summary>No Pregnancy Risk Factors.</summary>
        [Property("No Pregnancy Risk Factors", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, None", true, IGURL.ObservationNoneOfSpecifiedPregnancyRiskFactors, false, 250)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73775-9", code: VitalRecord.NONE_OF_THE_ABOVE, section: MEDICAL_INFORMATION_SECTION)]
        public bool NoPregnancyRiskFactors
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Eclampsia Hypertension.</summary>
        [Property("Eclampsia Hypertension", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, Eclampsia Hypertension", true, IGURL.ConditionEclampsiaHypertension, true, 244)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73775-9", code: "15938005", section: MEDICAL_INFORMATION_SECTION)]
        public bool EclampsiaHypertension
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Gestational Diabetes.</summary>
        [Property("Gestational Diabetes", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, Gestational Diabetes", true, IGURL.ConditionGestationalDiabetes, true, 241)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73775-9", code: "11687002", section: MEDICAL_INFORMATION_SECTION)]
        public bool GestationalDiabetes
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Gestational Hypertension.</summary>
        [Property("Gestational Hypertension", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, Gestational Hypertension", true, IGURL.ConditionGestationalHypertension, true, 242)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73775-9", code: "48194001", section: MEDICAL_INFORMATION_SECTION)]
        public bool GestationalHypertension
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Prepregnancy Diabetes.</summary>
        [Property("Prepregnancy Diabetes", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, Prepregnancy Diabetes", true, IGURL.ConditionPrepregnancyDiabetes, true, 240)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73775-9", code: "73211009", section: MEDICAL_INFORMATION_SECTION)]
        public bool PrepregnancyDiabetes
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Prepregnancy Hypertension.</summary>
        [Property("Prepregnancy Hypertension", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, Prepregnancy Hypertension", true, IGURL.ConditionPrepregnancyHypertension, true, 243)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73775-9", code: "38341003", section: MEDICAL_INFORMATION_SECTION)]
        public bool PrepregnancyHypertension
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Previous Cesarean.</summary>
        [Property("Previous Cesarean", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, Previous Cesarean", true, IGURL.ObservationPreviousCesarean, true, 249)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73775-9", code: "200144004", section: MEDICAL_INFORMATION_SECTION)]
        public bool PreviousCesarean
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Previous Preterm Birth.</summary>
        [Property("Previous Preterm Birth", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, Previous Preterm Birth", true, IGURL.ObservationPreviousPretermBirth, true, 245)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73775-9", code: "161765003", section: MEDICAL_INFORMATION_SECTION)]
        public bool PreviousPretermBirth
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Artificial Insemination.</summary>
        [Property("Artificial Insemination", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, Artificial Insemination", true, IGURL.ProcedureArtificialInsemination, true, 247)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73775-9", code: "58533008", section: MEDICAL_INFORMATION_SECTION)]
        public bool ArtificialInsemination
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Assisted Fertilization.</summary>
        [Property("Assisted Fertilization", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, Assisted Fertilization", true, IGURL.ProcedureAssistedFertilization, true, 248)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73775-9", code: "63487001", section: MEDICAL_INFORMATION_SECTION)]
        public bool AssistedFertilization
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Infertility Treatment.</summary>
        [Property("Infertility Treatment", Property.Types.Bool, "Pregnancy Risk Factors",
                  "Pregnancy Risk Factors, Infertility Treatment", true, IGURL.ProcedureInfertilityTreatment, true, 246)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73775-9", code: "445151000124101", section: MEDICAL_INFORMATION_SECTION)]
        public bool InfertilityTreatment
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        //
        // Final Route and Method of Delivery Section
        //

        /// <summary>Unknown Final Route and Method of Delivery.</summary>
        [Property("Unknown Final Route and Method of Delivery", Property.Types.Bool, "Final Route and Method of Delivery",
                  "Final Route and Method of Delivery, Unknown", true, IGURL.ObservationUnknownFinalRouteMethodDelivery, false, 193)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73762-7", code: VitalRecord.UNKNOWN, section: MEDICAL_INFORMATION_SECTION)]
        public bool UnknownFinalRouteAndMethodOfDelivery
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Final Route and Method of Delivery.</summary>
        /// <value>delivery route</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; route = new Dictionary&lt;string, string&gt;();</para>
        /// <para>route.Add("code", "302383004");</para>
        /// <para>route.Add("system", "http://snomed.info/sct");</para>
        /// <para>route.Add("display", "Forceps delivery (procedure)");</para>
        /// <para>ExampleBirthRecord.FinalRouteAndMethodOfDelivery = route;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Final Route and Method of Delivery: {ExampleBirthRecord.FinalRouteAndMethodOfDelivery}");</para>
        /// </example>
        [Property("Final Route and Method of Delivery", Property.Types.Dictionary, "Final Route and Method of Delivery",
                  "Final Route and Method of Delivery", true, IGURL.ProcedureFinalRouteMethodDelivery, true, 193)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73762-7", section: MEDICAL_INFORMATION_SECTION)]
        public Dictionary<string, string> FinalRouteAndMethodOfDelivery
        {
            get
            {
                FHIRPath fhirPath = GetFHIRPathAttribute();
                bool criteria(Bundle.EntryComponent e) =>
                    e.Resource.TypeName == "Procedure" &&
                    ((Procedure)e.Resource).Category?.Coding[0].Code == fhirPath.CategoryCode;
                List<Bundle.EntryComponent> matches = Bundle.Entry.Where(criteria).ToList();
                if (matches.Count == 0)
                {
                    return EmptyCodeableDict();
                }
                Procedure procedure = (Procedure)matches.First().Resource;
                return CodeableConceptToDict(procedure.Code);
            }
            set
            {
                FHIRPath fhirPath = GetFHIRPathAttribute();
                RemoveAllEntries(fhirPath);
                if (IsDictEmptyOrDefault(value))
                {
                    return;
                }
                Coding coding = DictToCoding(value);
                fhirPath.Code = coding.Code;
                fhirPath.CodeSystem = coding.System;
                fhirPath.Display = coding.Display;
               CreateEntry(fhirPath, SubjectId());
            }
        }

        /// <summary>Final Route and Method of Delivery Helper.</summary>
        /// <value>delivery route</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FinalRouteAndMethodOfDeliveryHelper = "302383004";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Final Route and Method of Delivery: {ExampleBirthRecord.FinalRouteAndMethodOfDeliveryHelper}");</para>
        /// </example>
        [Property("Final Route and Method of Delivery Helper", Property.Types.String, "Final Route and Method of Delivery",
                  "Final Route and Method of Delivery", false, IGURL.ProcedureFinalRouteMethodDelivery, true, 193)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73762-7", section: MEDICAL_INFORMATION_SECTION)]
        public string FinalRouteAndMethodOfDeliveryHelper
        {
            get
            {
                if (FinalRouteAndMethodOfDelivery.ContainsKey("code"))
                {
                    string code = FinalRouteAndMethodOfDelivery["code"];
                    if (!String.IsNullOrWhiteSpace(code))
                    {
                        return code;
                    }
                }
                return null;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    FinalRouteAndMethodOfDelivery = EmptyCodeDict();
                }
                SetCodeValue("FinalRouteAndMethodOfDelivery", value, BFDR.ValueSets.DeliveryRoutes.Codes);
            }
        }

        /// <summary>Method of Delivery - Fetal Presentation.</summary>
        /// <value>presentation</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; route = new Dictionary&lt;string, string&gt;();</para>
        /// <para>route.Add("code", "70028003");</para>
        /// <para>route.Add("system", "http://snomed.info/sct");</para>
        /// <para>route.Add("display", "Vertex presentation");</para>
        /// <para>ExampleBirthRecord.FetalPresentation = route;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Fetal Presentation: {ExampleBirthRecord.FetalPresentation}");</para>
        /// </example>
        [Property("Fetal Presentation", Property.Types.Dictionary, "Fetal Presentation",
                  "Final Route and Method of Delivery", true, IGURL.ObservationFetalPresentation, true, 193)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73761-9", section: MEDICAL_INFORMATION_SECTION)]
        public Dictionary<string, string> FetalPresentation
        {
            get
            {
                Observation obs = GetObservation("73761-9");
                if (obs != null && obs.Value != null && (obs.Value as CodeableConcept) != null)
                {
                    return CodeableConceptToDict((CodeableConcept)obs.Value);
                }
                return EmptyCodeableDict();  
            }
            set
            {
                Observation obs = GetOrCreateObservation("73761-9", CodeSystems.LOINC, "Fetal Presentation", BFDR.ProfileURL.ObservationFetalPresentation, MEDICAL_INFORMATION_SECTION);
                obs.Value = DictToCodeableConcept(value);   
            }
        }

        /// <summary>Method of Delivery - Fetal Presentation Helper.</summary>
        /// <value>presentation</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FetalPresentationHelper = "70028003";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Fetal Presentation Helper: {ExampleBirthRecord.FetalPresentationHelper}");</para>
        /// </example>
        [Property("Fetal Presentation Helper", Property.Types.String, "Method of Delivery - Fetal Presentation",
                  "Fetal Presentation", false, IGURL.ObservationFetalPresentation, true, 193)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73761-9", section: MEDICAL_INFORMATION_SECTION)]
        public string FetalPresentationHelper
        {
            get
            {
                if (FetalPresentation.ContainsKey("code") && !String.IsNullOrWhiteSpace(FetalPresentation["code"]))
                {
                    return FetalPresentation["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FetalPresentation", value, BFDR.ValueSets.FetalPresentations.Codes);
                }
                
            }
        }

        /// <summary>Labor Trial Attempted</summary>
        /// <value>attempted</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.LaborTrialAttempted = true;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother transferred: {ExampleBirthRecord.LaborTrialAttempted}");</para>
        /// </example>
        [Property("LaborTrialAttempted", Property.Types.Bool, "LaborTrialAttempted", "LaborTrialAttempted", false, BFDR.IGURL.ObservationLaborTrialAttempted, true, 288)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73760-1", section: MEDICAL_INFORMATION_SECTION)]
        public bool? LaborTrialAttempted
        {
            get
            {
                Observation obs = GetObservation("73760-1");
                if (obs != null)
                {
                    bool? infantLiving = ((FhirBoolean)obs.Value).Value;
                    return infantLiving;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                Observation obs = GetOrCreateObservation("73760-1", CodeSystems.LOINC, "Labor Trial Attempted", BFDR.ProfileURL.ObservationLaborTrialAttempted, MEDICAL_INFORMATION_SECTION);
                obs.Value = new FhirBoolean(value);
            }
        }

        /// <summary>NumberOfPreviousCesareans.</summary>
        /// <value>NumberOfPreviousCesareans</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.NumberOfPreviousCesareans = 1;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"NumberOfPreviousCesareans: {ExampleBirthRecord.NumberOfPreviousCesareans}");</para>
        /// </example>
        [Property("Number Of Previous Cesareans", Property.Types.Int32, "Number Of Previous Cesareans", "Number Of Previous Cesareans.", true, IGURL.ObservationNumberPreviousCesareans, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68497-7')", "")]
        public int? NumberOfPreviousCesareans
        {
            get
            {
                Observation obs = GetObservation("68497-7");

                if (obs != null && obs.Value != null && obs.Value as Hl7.Fhir.Model.Integer != null)
                {
                    return (obs.Value as Hl7.Fhir.Model.Integer).Value;
                }
                
                return null;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                Observation obs = GetOrCreateObservation("68497-7", CodeSystems.LOINC, "Number of Previous Cesareans", BFDR.ProfileURL.ObservationNumberPreviousCesareans, MEDICAL_INFORMATION_SECTION, Mother.Id);
                obs.Value = new Hl7.Fhir.Model.Integer(value);
            }
        }

        /// <summary>NumberOfPreviousCesareansEditFlag.</summary>
        /// <value>NumberOfPreviousCesareansEditFlag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.NumberOfPreviousCesareansEditFlag = 4;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"NumberOfPreviousCesareansEditFlag: {ExampleBirthRecord.NumberOfPreviousCesareansEditFlag}");</para>
        /// </example>
        [Property("Number Of Previous Cesareans Edit Flag", Property.Types.Dictionary, "Number of Previous Cesareans", "Number of Previous Cesareans Edit Flag.", true, IGURL.ObservationNumberPreviousCesareans, true, 14)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68497-7')", "")]
        public Dictionary<string, string> NumberOfPreviousCesareansEditFlag
        {
            get
            {
                Observation obs = GetObservation("68497-7");
                Extension editFlag = obs?.Value?.Extension.FirstOrDefault(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                if (editFlag != null && editFlag.Value != null && editFlag.Value.GetType() == typeof(CodeableConcept))
                {
                    return CodeableConceptToDict((CodeableConcept)editFlag.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (IsDictEmptyOrDefault(value))
                {
                    return;
                }
                Observation obs = GetObservation("68497-7");
                if (obs == null)
                {
                    obs = GetOrCreateObservation("68497-7", CodeSystems.LOINC, "Number of previous cesareans", BFDR.ProfileURL.ObservationNumberPreviousCesareans, MEDICAL_INFORMATION_SECTION, Mother.Id);
                    obs.Value = new UnsignedInt();
                }
                obs.Value?.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                Extension editFlag = new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(value));
                obs.Value.Extension.Add(editFlag);
            }
        }

        /// <summary>
        /// NumberOfPreviousCesareansEditFlag Helper
        /// </summary>
        [Property("NumberOfPreviousCesareansEditFlagHelper", Property.Types.String, "Number of Previous Cesareans", "Number Of Previous Cesareans Edit Flag Helper.", false, IGURL.ObservationNumberPreviousCesareans, true, 2)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68493-6')", "")]
        public String NumberOfPreviousCesareansEditFlagHelper
        {
            get
            {
                return NumberOfPreviousCesareansEditFlag.ContainsKey("code") && !String.IsNullOrWhiteSpace(NumberOfPreviousCesareansEditFlag["code"]) ? NumberOfPreviousCesareansEditFlag["code"] : null;
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("NumberOfPreviousCesareansEditFlag", value, BFDR.ValueSets.NumberPreviousCesareansEditFlags.Codes);
                }
            }
        }

        //
        // Obstetric Procedures Section
        //

        /// <summary>No Obstetric Procedures.</summary>
        [Property("No Obstetric Procedures", Property.Types.Bool, "Obstetric Procedures",
                  "Obstetric Procedures, None", true, IGURL.ObservationNoneOfSpecifiedObstetricProcedures, false, 176)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73814-6", code: VitalRecord.NONE_OF_THE_ABOVE, section: MEDICAL_INFORMATION_SECTION)]
        public bool NoObstetricProcedures
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Successful External Cephalic Version.</summary>
        [Property("Successful External Cephalic Version", Property.Types.Bool, "Obstetric Procedures",
                  "Obstetric Procedures, Successful External Cephalic Version", true, IGURL.ProcedureObstetric, true, 176)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73814-6", code: "240278000", section: MEDICAL_INFORMATION_SECTION)]
        public bool SuccessfulExternalCephalicVersion
        {
            get
            {
                FHIRPath fhirPath = GetFHIRPathAttribute();
                bool criteria(Bundle.EntryComponent e) =>
                    e.Resource.TypeName == "Procedure" &&
                    ((Procedure)e.Resource).Category?.Coding[0].Code == fhirPath.CategoryCode &&
                    ((Procedure)e.Resource).Outcome.Coding[0].Code == SUCCESSFUL_OUTCOME;
                List<Bundle.EntryComponent> matches = Bundle.Entry.Where(criteria).ToList();
                return matches.Count > 0;
            }
            set
            {
                if (value)
                {
                    if (SuccessfulExternalCephalicVersion)
                    {
                        return; // entry exists, nothing to do
                    }
                    else
                    {
                        // create an entry
                        Procedure proc = (Procedure)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                        proc.Outcome = new CodeableConcept(CodeSystems.SCT, SUCCESSFUL_OUTCOME);
                    }
                }
                else
                {
                    if (!SuccessfulExternalCephalicVersion)
                    {
                        return; // entry doesn't exist, nothing to do
                    }
                    else
                    {
                        // remove the entry
                        FHIRPath fhirPath = GetFHIRPathAttribute();
                        bool func(Bundle.EntryComponent e) =>
                            e.Resource.TypeName == fhirPath.FHIRType.ToString() &&
                            ((Procedure)e.Resource).Code.Coding[0].Code == fhirPath.Code &&
                            ((Procedure)e.Resource).Outcome.Coding[0].Code == SUCCESSFUL_OUTCOME;
                        foreach (var entry in Bundle.Entry.Where(func))
                        {
                            RemoveReferenceFromComposition(entry.FullUrl, fhirPath.Section);
                        }
                        Bundle.Entry.RemoveAll(new Predicate<Bundle.EntryComponent>(func));
                    }
                }
            }
        }

        /// <summary>Unsuccessful External Cephalic Version.</summary>
        [Property("Unsuccessful External Cephalic Version", Property.Types.Bool, "Obstetric Procedures",
                  "Obstetric Procedures, Unsuccessful External Cephalic Version", true, IGURL.ProcedureObstetric, true, 177)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73814-6", code: "240278000", section: MEDICAL_INFORMATION_SECTION)]
        public bool UnsuccessfulExternalCephalicVersion
        {
            get
            {
                FHIRPath fhirPath = GetFHIRPathAttribute();
                bool criteria(Bundle.EntryComponent e) =>
                    e.Resource.TypeName == "Procedure" &&
                    ((Procedure)e.Resource).Category?.Coding[0].Code == fhirPath.CategoryCode &&
                    ((Procedure)e.Resource).Outcome.Coding[0].Code == UNSUCCESSFUL_OUTCOME;
                List<Bundle.EntryComponent> matches = Bundle.Entry.Where(criteria).ToList();
                return matches.Count > 0;
            }
            set
            {
                if (value)
                {
                    if (UnsuccessfulExternalCephalicVersion)
                    {
                        return; // entry exists, nothing to do
                    }
                    else
                    {
                        // create an entry
                        Procedure proc = (Procedure)CreateEntry(GetFHIRPathAttribute(), SubjectId());
                        proc.Outcome = new CodeableConcept(CodeSystems.SCT, UNSUCCESSFUL_OUTCOME);
                    }
                }
                else
                {
                    if (!UnsuccessfulExternalCephalicVersion)
                    {
                        return; // entry doesn't exist, nothing to do
                    }
                    else
                    {
                        // remove the entry
                        FHIRPath fhirPath = GetFHIRPathAttribute();
                        bool func(Bundle.EntryComponent e) =>
                            e.Resource.TypeName == fhirPath.FHIRType.ToString() &&
                            ((Procedure)e.Resource).Code.Coding[0].Code == fhirPath.Code &&
                            ((Procedure)e.Resource).Outcome.Coding[0].Code == UNSUCCESSFUL_OUTCOME;
                        foreach (var entry in Bundle.Entry.Where(func))
                        {
                            RemoveReferenceFromComposition(entry.FullUrl, fhirPath.Section);
                        }
                        Bundle.Entry.RemoveAll(new Predicate<Bundle.EntryComponent>(func));
                    }
                }
            }
        }

        /// <summary>Mother's Day of Birth.</summary>
        /// <value>the mother's day of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherBirthDay = 11;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Day of Birth: {ExampleBirthRecord.MotherBirthDay}");</para>
        /// </example>
        [Property("MotherBirthDay", Property.Types.Int32, "Mother Demographics", "Mother's Day of Birth.", true, VR.IGURL.Mother, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.birthDate", "")]// TODO
        public int? MotherBirthDay
        {
            get
            {
                return GetDateElementNoTime(Mother?.BirthDateElement, VR.ExtensionURL.PartialDateTimeDayVR);
            }
            set
            {
                if (Mother.BirthDateElement == null)
                {
                    AddBirthDateToPatient(Mother, false);
                }
                Date newDate = SetDay(value, Mother.BirthDateElement);
                if (newDate != null)
                {
                    Mother.BirthDateElement = newDate;
                }
            }
        }

        /// <summary>Mother's Month of Birth.</summary>
        /// <value>the mother's month of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherBirthMonth = 11;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Month of Birth: {ExampleBirthRecord.MotherBirthMonth}");</para>
        /// </example>
        [Property("MotherBirthMonth", Property.Types.Int32, "Mother Demographics", "Mother's Month of Birth.", true, VR.IGURL.Mother, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.birthDate", "")]
        public int? MotherBirthMonth
        {
            get
            {
                return GetDateElementNoTime(Mother?.BirthDateElement, VR.ExtensionURL.PartialDateTimeMonthVR);
            }
            set
            {
                if (Mother.BirthDateElement == null)
                {
                    AddBirthDateToPatient(Mother, false);
                }
                Date newDate = SetMonth(value, Mother.BirthDateElement);
                if (newDate != null)
                {
                    Mother.BirthDateElement = newDate;
                }
            }
        }

        /// <summary>Mother's Year of Birth.</summary>
        /// <value>the mother's year of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherBirtYear = 1987;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Month of Birth: {ExampleBirthRecord.MotherBirthYear}");</para>
        /// </example>
        [Property("MotherBirthYear", Property.Types.Int32, "Mother Demographics", "Mother's Year of Birth.", true, VR.IGURL.Mother, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.birthDate", "")]
        public int? MotherBirthYear
        {
            get
            {
                return GetDateElementNoTime(Mother?.BirthDateElement, VR.ExtensionURL.PartialDateTimeYearVR);
            }
            set
            {
                if (Mother.BirthDateElement == null)
                {
                    AddBirthDateToPatient(Mother, false);
                }
                Date newDate = SetYear(value, Mother.BirthDateElement);
                if (newDate != null)
                {
                    Mother.BirthDateElement = newDate;
                }
            }
        }

        /// <summary>Mother's Date of Birth.</summary>
        /// <value>the mother's date of birth</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfBirth = "1980-05-13";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Date of Birth: {ExampleBirthRecord.MotherDateOfBirth}");</para>
        /// </example>
        [Property("Mother Date Of Birth", Property.Types.String, "Mother Demographics", "Mother's Date of Birth.", true, VR.IGURL.Mother, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate", "")]
        public string MotherDateOfBirth
        {
            get
            {
                if (this.Mother == null || this.Mother.BirthDateElement == null)
                {
                    return null;
                }
                return this.Mother.BirthDate;
            }
            set
            {
                this.Mother.BirthDateElement = ConvertToDate(value);
            }
        }

        // Parent ages at delivery are represented as extensions on the child Patient resource as shown below
        // {
        //   "extension" : [
        //     {
        //       "url" : "reportedAge",
        //       "valueQuantity" : {
        //         "value" : 34,
        //         "system" : "http://unitsofmeasure.org",
        //         "code" : "a"
        //       }
        //     },
        //     {
        //       "url" : "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Extension-role-vr",
        //       "valueCodeableConcept" : {
        //         "coding" : [
        //           {
        //             "system" : "http://terminology.hl7.org/CodeSystem/v3-RoleCode",
        //             "code" : "MTH",
        //             "display" : "mother"
        //           }
        //         ]
        //       }
        //     }
        //   ],
        //   "url" : "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Extension-reported-parent-age-at-delivery-vr"
        // }
        private int? GetParentReportedAgeAtDelivery(string role)
        {
            if (IsDictEmptyOrDefault(GetRoleCode(role)))
            {
                throw new System.ArgumentException($"Role '{role}' is not a member of the VR Role value set");
            }
            int? age = null;

            Extension parentAge = Child?.Extension.Find(ext => IsParentAgeAtBirthExt(ext, role));
            if (parentAge != null)
            {
                Extension ageExt = parentAge.Extension.Find(ext => ext.Url.Equals("reportedAge"));
                if (ageExt != null && (ageExt.Value as Quantity) != null)
                {
                    age = (int)(ageExt.Value as Quantity).Value;
                }
            }
            return age;
        }

        private Dictionary<string, string> GetRoleCode(string role)
        {
            for (int i = 0; i < VR.ValueSets.Role.Codes.Length; i++)
            {
                if (VR.ValueSets.Role.Codes[i, 0].Equals(role))
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("code", VR.ValueSets.Role.Codes[i, 0]);
                    dict.Add("display", VR.ValueSets.Role.Codes[i, 1]);
                    dict.Add("system", VR.ValueSets.Role.Codes[i, 2]);
                    return dict;
                }
            }
            return EmptyCodeDict();
        }

        private bool IsParentAgeAtBirthExt(Extension ext, string role)
        {
            if (ext.Url.Equals(VRExtensionURLs.ReportedParentAgeAtDelivery))
            {
                if (ext.Extension.Any(
                    subExt => subExt.Url == "motherOrFather" &&
                    (subExt.Value as CodeableConcept) != null &&
                    (subExt.Value as CodeableConcept).Coding.Any(code => code.Code.Equals(role))))
                {
                    return true;
                }
            }
            return false;
        }

        private void SetParentReportedAgeAtDelivery(string role, int? value)
        {
            Dictionary<string, string> roleCode = GetRoleCode(role);
            if (IsDictEmptyOrDefault(roleCode))
            {
                throw new System.ArgumentException($"Role '{role}' is not a member of the VR Role value set");
            }

            Child.Extension.RemoveAll(ext => IsParentAgeAtBirthExt(ext, role));
            Extension parentAgeAtBirth = new Extension(VRExtensionURLs.ReportedParentAgeAtDelivery, null);
            CodeableConcept parentRole = new CodeableConcept(roleCode["system"], roleCode["code"], roleCode["display"]);
            parentAgeAtBirth.Extension.Add(new Extension("motherOrFather", parentRole));
            if (value != null)
            {
                Quantity ageInYears = new Quantity((decimal)value, "a");
                parentAgeAtBirth.Extension.Add(new Extension("reportedAge", ageInYears));
            }
            Child.Extension.Add(parentAgeAtBirth);
        }

        /// <summary>Mother's Age at Delivery</summary>
        /// <value>the mother's age at Delivery in years</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherReportedAgeAtDelivery = 29;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's age at delivery: {ExampleBirthRecord.MotherReportedAgeAtDelivery}");</para>
        /// </example>
        [Property("MotherReportedAgeAtDelivery", Property.Types.Int32, "Mother Demographics", "Mother Demographics, Reported age at Delivery", true, VR.IGURL.Mother, true, 237)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Extension-reported-parent-age-at-delivery-vr')", "")]
        public int? MotherReportedAgeAtDelivery
        {
            get => GetParentReportedAgeAtDelivery("MTH");
            set => SetParentReportedAgeAtDelivery("MTH", value);
        }

        /// <summary>Mother's Date of Birth Edit Flag</summary>
        /// <value>the mother's date of birth edit flag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; edit = new Dictionary&lt;string, string&gt;();</para>
        /// <para>edit.Add("code", "queriedCorrect");</para>
        /// <para>edit.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");</para>
        /// <para>edit.Add("display", "Queried, and Correct");</para>
        /// <para>ExampleBirthRecord.MotherDateOfBirthEditFlag = route;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's date of birth edit flag: {ExampleBirthRecord.MotherDateOfBirthEditFlag}");</para>
        /// </example>
        [Property("MotherDateOfBirthEditFlag", Property.Types.Dictionary, "Mother Demographics", "Mother Demographics, Date of Birth Edit Flag", true, VR.IGURL.Mother, true, 17)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate.extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag')", "")]
        public Dictionary<string, string> MotherDateOfBirthEditFlag
        {
            get
            {
                if (Mother != null)
                {
                    Extension editFlag = Mother.BirthDateElement?.Extension.Find(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                    if (editFlag != null && editFlag.Value != null && editFlag.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)editFlag.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                Mother.BirthDateElement?.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                if (Mother.BirthDateElement == null)
                {
                    Mother.BirthDateElement = new Date();
                }
                Mother.BirthDateElement.Extension.Add(new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(value)));
            }
        }

        /// <summary>Mother's Date of Birth Edit Flag helper</summary>
        /// <value>the mother's date of birth edit flag helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherDateOfBirthEditFlagHelper = "queriedCorrect";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's date of birth edit flag: {ExampleBirthRecord.MotherDateOfBirthEditFlagHelper}");</para>
        /// </example>
        [Property("MotherDateOfBirthEditFlagHelper", Property.Types.String, "Mother Demographics", "Mother Demographics, Date of Birth Edit Flag", false, VR.IGURL.Child, true, 17)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate.extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag')", "")]
        public string MotherDateOfBirthEditFlagHelper
        {
            get
            {
                if (MotherDateOfBirthEditFlag.ContainsKey("code"))
                {
                    string code = MotherDateOfBirthEditFlag["code"];
                    if (!String.IsNullOrWhiteSpace(code))
                    {
                        return code;
                    }
                }
                return null;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    MotherDateOfBirthEditFlag = EmptyCodeDict();
                    return;
                }
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("code", value);
                dictionary.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");
                MotherDateOfBirthEditFlag = dictionary;
            }
        }

        /// <summary>Father's Day of Birth.</summary>
        /// <value>the father's day of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherBirthDay = 11;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Day of Birth: {ExampleBirthRecord.FatherBirthDay}");</para>
        /// </example>
        [Property("FatherBirthDay", Property.Types.Int32, "Father Demographics", "Father's Day of Birth.", true, VR.IGURL.RelatedPersonFatherNatural, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is RelatedPerson).extension.birthDate", "")]// TODO
        public int? FatherBirthDay
        {
            get
            {
                return GetDateElementNoTime(Father?.BirthDateElement, VR.ExtensionURL.PartialDateTimeDayVR);
            }
            set
            {
                if (Father.BirthDateElement == null)
                {
                    Father.BirthDateElement = new Date();
                    Father.BirthDateElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                Date newDate = SetDay(value, Father.BirthDateElement);
                if (newDate != null)
                {
                    Father.BirthDateElement = newDate;
                }
            }
        }

        /// <summary>Father's Month of Birth.</summary>
        /// <value>the father's month of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherBirthMonth = 9;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Day of Birth: {ExampleBirthRecord.FatherBirthMonth}");</para>
        /// </example>
        [Property("FatherBirthMonth", Property.Types.Int32, "Father Demographics", "Father's Month of Birth.", true, VR.IGURL.RelatedPersonFatherNatural, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is RelatedPerson).extension.birthDate", "")]// TODO
        public int? FatherBirthMonth
        {
            get
            {
                return GetDateElementNoTime(Father?.BirthDateElement, VR.ExtensionURL.PartialDateTimeMonthVR);
            }
            set
            {
                if (Father.BirthDateElement == null)
                {
                    Father.BirthDateElement = new Date();
                    Father.BirthDateElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                Date newDate = SetMonth(value, Father.BirthDateElement);
                if (newDate != null)
                {
                    Father.BirthDateElement = newDate;
                }
            }
        }

        /// <summary>Father's Year of Birth.</summary>
        /// <value>the father's year of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherBirthYear = 1979;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Day of Birth: {ExampleBirthRecord.FatherBirthYear}");</para>
        /// </example>
        [Property("FatherBirthYear", Property.Types.Int32, "Father Demographics", "Father's Year of Birth.", true, VR.IGURL.RelatedPersonFatherNatural, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is RelatedPerson).extension.birthDate", "")]// TODO
        public int? FatherBirthYear
        {
            get
            {
                return GetDateElementNoTime(Father?.BirthDateElement, VR.ExtensionURL.PartialDateTimeYearVR);
            }
            set
            {
                if (Father.BirthDateElement == null)
                {
                    Father.BirthDateElement = new Date();
                    Father.BirthDateElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                Date newDate = SetYear(value, Father.BirthDateElement);
                if (newDate != null)
                {
                    Father.BirthDateElement = newDate;
                }
            }
        }

        /// <summary>Father's Date of Birth.</summary>
        /// <value>the father's date of birth</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfBirth = "1940-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Date of Birth: {ExampleBirthRecord.FatherDateOfBirth}");</para>
        /// </example>
        [Property("FatherDateOfBirth", Property.Types.String, "Father Demographics", "Father's Date of Birth.", true, VR.IGURL.RelatedPersonFatherNatural, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is RelatedPerson).birthDate", "")]// TODO
        public string FatherDateOfBirth
        {
            get
            {
                if (this.Father == null || this.Father.BirthDateElement == null)
                {
                    return null;
                }
                return this.Father.BirthDate;
            }
            set
            {
                this.Father.BirthDateElement = ConvertToDate(value);
            }
        }

        /// TODO: ethinicty/race component code still uses vrdr codesystem: http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-component-cs
        /// should be http://hl7.org/fhir/us/vr-common-library/CodeSystem/codesystem-vr-component
        /// <summary>Father's Age at Delivery</summary>
        /// <value>the father's age at Delivery in years</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherReportedAgeAtDelivery = 29;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's age at delivery: {ExampleBirthRecord.FatherReportedAgeAtDelivery}");</para>
        /// </example>
        [Property("FatherReportedAgeAtDelivery", Property.Types.Int32, "Father Demographics", "Father Demographics, Reported age at Delivery", true, VR.IGURL.RelatedPersonFatherNatural, true, 238)]
        [FHIRPath("Bundle.entry.resource.where($this is RelatedPerson).extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Extension-reported-parent-age-at-delivery-vr')", "")]
        public int? FatherReportedAgeAtDelivery
        {
            get => GetParentReportedAgeAtDelivery("FTH");
            set => SetParentReportedAgeAtDelivery("FTH", value);
        }

        /// <summary>Father's Date of Birth Edit Flag</summary>
        /// <value>the father's date of birth edit flag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; edit = new Dictionary&lt;string, string&gt;();</para>
        /// <para>edit.Add("code", "queriedCorrect");</para>
        /// <para>edit.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");</para>
        /// <para>edit.Add("display", "Queried, and Correct");</para>
        /// <para>ExampleBirthRecord.FatherDateOfBirthEditFlag = route;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's date of birth edit flag: {ExampleBirthRecord.FatherDateOfBirthEditFlag}");</para>
        /// </example>
        [Property("FatherDateOfBirthEditFlag", Property.Types.Dictionary, "Father Demographics", "Father Demographics, Date of Birth Edit Flag", true, VR.IGURL.RelatedPersonFatherNatural, true, 28)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is RelatedPerson).birthDate.extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag')", "")]
        public Dictionary<string, string> FatherDateOfBirthEditFlag
        {
            get
            {
                if (Father != null)
                {
                    Extension editFlag = Father.BirthDateElement?.Extension.Find(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                    if (editFlag != null && editFlag.Value != null && editFlag.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)editFlag.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                Father.BirthDateElement?.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                if (Father.BirthDateElement == null)
                {
                    Father.BirthDateElement = new Date();
                }
                Father.BirthDateElement.Extension.Add(new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(value)));
            }
        }

        /// <summary>Father's Date of Birth Edit Flag helper</summary>
        /// <value>the father's date of birth edit flag helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherDateOfBirthEditFlagHelper = "queriedCorrect";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's date of birth edit flag: {ExampleBirthRecord.FatherDateOfBirthEditFlagHelper}");</para>
        /// </example>
        [Property("FatherDateOfBirthEditFlagHelper", Property.Types.String, "Father Demographics", "Father Demographics, Date of Birth Edit Flag", false, VR.IGURL.Child, true, 28)]
        [FHIRPath("Bundle.entry.resource.where($this is RelatedPerson).birthDate.extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag')", "")]
        public string FatherDateOfBirthEditFlagHelper
        {
            get
            {
                if (FatherDateOfBirthEditFlag.ContainsKey("code"))
                {
                    string code = FatherDateOfBirthEditFlag["code"];
                    if (!String.IsNullOrWhiteSpace(code))
                    {
                        return code;
                    }
                }
                return null;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    FatherDateOfBirthEditFlag = EmptyCodeDict();
                    return;
                }
                SetCodeValue("FatherDateOfBirthEditFlag", value, VR.ValueSets.DateOfBirthEditFlags.Codes);
            }
        }

        /// <summary>Mother's Ethnicity Hispanic Mexican.</summary>
        /// <value>the mother's ethnicity. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "Y");</para>
        /// <para>ethnicity.Add("system", CodeSystems.YesNo);</para>
        /// <para>ethnicity.Add("display", "Yes");</para>
        /// <para>ExampleBirthRecord.MotherEthnicity1 = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Ethnicity: {ExampleBirthRecord.MotherEthnicity1["display"]}");</para>
        /// </example>
        [Property("MotherEthnicity1", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Mother's Ethnicity Hispanic Mexican.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherEthnicity1
        {
            get
            {
                Observation obs = GetOrCreateObservation("inputraceandethnicityMother", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_MOTHER);
                Observation.ComponentComponent ethnicity = obs.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Mexican);
                if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                {
                    return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (value["code"] == "") {
                    return;
                }
                Observation obs = GetOrCreateObservation("inputraceandethnicityMother", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_MOTHER);
                obs.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Mexican);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, NvssEthnicity.Mexican, NvssEthnicity.MexicanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                obs.Component.Add(component);
                obs.Subject = new ResourceReference("urn:uuid:" + Child.Id);
            }
        }

        /// <summary>Mother's Ethnicity 1 Helper</summary>
        /// <value>Mother's Ethnicity 1.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VR.ValueSets.YesNoUnknown.Yes;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Ethnicity: {ExampleBirthRecord.MotherEthnicity1Helper}");</para>
        /// </example>
        [Property("Mother Ethnicity 1 Helper", Property.Types.String, "Race and Ethnicity Profiles", "Mother's Ethnicity 1.", false, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        public string MotherEthnicity1Helper
        {
            get
            {
                if (MotherEthnicity1.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherEthnicity1["code"]))
                {
                    return MotherEthnicity1["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherEthnicity1", value, VR.ValueSets.HispanicNoUnknown.Codes);
                }
            }
        }

        /// <summary>Mother's Ethnicity Hispanic Puerto Rican.</summary>
        /// <value>the mother's ethnicity. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "Y");</para>
        /// <para>ethnicity.Add("system", CodeSystems.YesNo);</para>
        /// <para>ethnicity.Add("display", "Yes");</para>
        /// <para>ExampleBirthRecord.MotherEthnicity2 = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Ethnicity: {ExampleBirthRecord.MotherEthnicity2["display"]}");</para>
        /// </example>
        [Property("MotherEthnicity2", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Mother's Ethnicity Hispanic Puerto Rican.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherEthnicity2
        {
            get
            {
                Observation obs = GetOrCreateObservation("inputraceandethnicityMother", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_MOTHER);
                Observation.ComponentComponent ethnicity = obs.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.PuertoRican);
                if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                {
                    return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (value["code"] == "") {
                    return;
                }
                Observation obs = GetOrCreateObservation("inputraceandethnicityMother", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_MOTHER);
                obs.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.PuertoRican);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, NvssEthnicity.PuertoRican, NvssEthnicity.PuertoRicanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                obs.Component.Add(component);
                obs.Subject = new ResourceReference("urn:uuid:" + Child.Id);
            }
        }

        /// <summary>Mother's Ethnicity 2 Helper</summary>
        /// <value>Mother's Ethnicity 2.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VR.ValueSets.YesNoUnknown.Yes;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Ethnicity: {ExampleBirthRecord.MotherEthnicity2Helper}");</para>
        /// </example>
        [Property("Mother Ethnicity 2 Helper", Property.Types.String, "Race and Ethnicity Profiles", "Mother's Ethnicity 2.", false, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        public string MotherEthnicity2Helper
        {
            get
            {
                if (MotherEthnicity2.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherEthnicity2["code"]))
                {
                    return MotherEthnicity2["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherEthnicity2", value, VR.ValueSets.HispanicNoUnknown.Codes);
                }
            }
        }

        /// <summary>Mother's Ethnicity Hispanic Cuban.</summary>
        /// <value>the mother's ethnicity. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "Y");</para>
        /// <para>ethnicity.Add("system", CodeSystems.YesNo);</para>
        /// <para>ethnicity.Add("display", "Yes");</para>
        /// <para>ExampleBirthRecord.MotherEthnicity3 = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Ethnicity: {ExampleBirthRecord.MotherEthnicity3["display"]}");</para>
        /// </example>
        [Property("MotherEthnicity3", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Mother's Ethnicity Hispanic Cuban.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherEthnicity3
        {
            get
            {
                Observation obs = GetOrCreateObservation("inputraceandethnicityMother", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_MOTHER);
                Observation.ComponentComponent ethnicity = obs.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Cuban);
                if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                {
                    return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (value["code"] == "") {
                    return;
                }
                Observation obs = GetOrCreateObservation("inputraceandethnicityMother", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_MOTHER);
                obs.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Cuban);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, NvssEthnicity.Cuban, NvssEthnicity.CubanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                obs.Component.Add(component);
                obs.Subject = new ResourceReference("urn:uuid:" + Child.Id);
            }
        }

        /// <summary>Mother's Ethnicity 3 Helper</summary>
        /// <value>Mother's Ethnicity 3.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VR.ValueSets.YesNoUnknown.Yes;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Ethnicity: {ExampleBirthRecord.MotherEthnicity3Helper}");</para>
        /// </example>
        [Property("Mother Ethnicity 3 Helper", Property.Types.String, "Race and Ethnicity Profiles", "Mother's Ethnicity 3.", false, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        public string MotherEthnicity3Helper
        {
            get
            {
                if (MotherEthnicity3.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherEthnicity3["code"]))
                {
                    return MotherEthnicity3["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherEthnicity3", value, VR.ValueSets.HispanicNoUnknown.Codes);
                }
            }
        }


        /// <summary>Mother's Ethnicity Hispanic Other.</summary>
        /// <value>the mother's ethnicity. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "Y");</para>
        /// <para>ethnicity.Add("system", CodeSystems.YesNo);</para>
        /// <para>ethnicity.Add("display", "Yes");</para>
        /// <para>ExampleBirthRecord.MotherEthnicity3 = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Ethnicity: {ExampleBirthRecord.MotherEthnicity4["display"]}");</para>
        /// </example>
        [Property("MotherEthnicity4", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Mother's Ethnicity Hispanic Other.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherEthnicity4
        {
            get
            {
                Observation obs = GetOrCreateObservation("inputraceandethnicityMother", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_MOTHER);
                Observation.ComponentComponent ethnicity = obs.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Other);
                if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                {
                    return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (value["code"] == "") {
                    return;
                }
                Observation obs = GetOrCreateObservation("inputraceandethnicityMother", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_MOTHER);
                obs.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Other);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, NvssEthnicity.Other, NvssEthnicity.OtherDisplay, null);
                component.Value = DictToCodeableConcept(value);
                obs.Component.Add(component);
            }
        }

        /// <summary>Mother's Ethnicity 4 Helper</summary>
        /// <value>Mother's Ethnicity 4.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VR.ValueSets.YesNoUnknown.Yes;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Ethnicity: {ExampleBirthRecord.MotherEthnicity4Helper}");</para>
        /// </example>
        [Property("Mother Ethnicity 4 Helper", Property.Types.String, "Race and Ethnicity Profiles", "Mother's Ethnicity 4.", false, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        public string MotherEthnicity4Helper
        {
            get
            {
                if (MotherEthnicity4.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherEthnicity4["code"]))
                {
                    return MotherEthnicity4["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherEthnicity4", value, VR.ValueSets.HispanicNoUnknown.Codes);
                }
            }
        }

        /// <summary>Mother's Ethnicity Hispanic Literal.</summary>
        /// <value>the mother's ethnicity. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherEthnicityLiteral = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Ethnicity: {ExampleBirthRecord.MotherEthnicityLiteral["display"]}");</para>
        /// </example>
        [Property("MotherEthnicityLiteral", Property.Types.String, "Race and Ethnicity Profiles", "Mother's Ethnicity Literal.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("ethnicity", "The literal string to describe ethnicity.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        [FHIRSubject(FHIRSubject.Subject.Mother)]
        public string MotherEthnicityLiteral
        {
            get => GetEthnicityLiteral("inputraceandethnicityMother", RACE_ETHNICITY_PROFILE_MOTHER);
            set => SetEthnicityLiteral(value, Mother.Id, "inputraceandethnicityMother", RACE_ETHNICITY_PROFILE_MOTHER);
        }

        private string GetEthnicityLiteral(string code, string section, [CallerMemberName] string propertyName = null) {
            Observation obs = GetOrCreateObservation(code, CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, section, null, propertyName);
            Observation.ComponentComponent ethnicity = obs.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Literal);
            if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as FhirString != null)
            {
                return ethnicity.Value.ToString();
            }
            return null;
        }

        private void SetEthnicityLiteral(string value, string subjectId, string code, string section, [CallerMemberName] string propertyName = null) {
            if (String.IsNullOrWhiteSpace(value))
            {
                return;
            }
            Observation obs = GetOrCreateObservation(code, CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, section, null, propertyName);
            obs.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Literal);
            Observation.ComponentComponent component = new Observation.ComponentComponent();
            component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, NvssEthnicity.Literal, NvssEthnicity.LiteralDisplay, null);
            component.Value = new FhirString(value.TrimEnd());
            obs.Component.Add(component);
        }

        /// <summary>Mother's Race values.</summary>
        /// <value>the mother's race. A tuple, where the first value of the tuple is the display value, and the second is
        /// the IJE code Y or N.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherRace = {NvssRace.BlackOrAfricanAmerican, "Y"};</para>
        /// <para>// Getter:</para>
        /// <para>string boaa = ExampleBirthRecord.RaceBlackOfAfricanAmerican;</para>
        /// </example>
        [Property("MotherRace", Property.Types.TupleArr, "Race and Ethnicity Profiles", "Mother's Race", true, VR.IGURL.InputRaceAndEthnicity, true, 38)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Tuple<string, string>[] MotherRace
        {
            get
            {
                // filter the boolean race values
                var booleanRaceCodes = NvssRace.GetBooleanRaceCodes();
                List<string> raceCodes = booleanRaceCodes.Concat(NvssRace.GetLiteralRaceCodes()).ToList();

                var races = new List<Tuple<string, string>>() { };

                Observation obs = GetOrCreateObservation("inputraceandethnicityMother", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_MOTHER);

                if (!obs.Component.Any())
                {
                    return races.ToArray();
                }
                foreach (string raceCode in raceCodes)
                {
                    Observation.ComponentComponent component = obs.Component.Where(c => c.Code.Coding[0].Code == raceCode).FirstOrDefault();
                    if (component != null)
                    {
                        // convert boolean race codes to strings
                        if (booleanRaceCodes.Contains(raceCode))
                        {
                            if (component.Value == null)
                            {
                                // If there is no value given, set the race to blank.
                                var race = Tuple.Create(raceCode, "");
                                races.Add(race);
                                continue;
                            }

                            bool? raceBool = ((FhirBoolean)component.Value).Value;

                            if (raceBool.Value)
                            {
                                var race = Tuple.Create(raceCode, "Y");
                                races.Add(race);
                            }
                            else
                            {
                                var race = Tuple.Create(raceCode, "N");
                                races.Add(race);
                            }
                        }
                        else
                        {
                            // Ignore unless there's a value present
                            if (component.Value != null)
                            {
                                var race = Tuple.Create(raceCode, component.Value.ToString());
                                races.Add(race);
                            }
                        }

                    }
                }

                return races.ToArray();
            }
            set
            {
                if (value.FirstOrDefault() == null) {
                    return;
                }
                Observation obs = GetOrCreateObservation("inputraceandethnicityMother", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_MOTHER);
                var booleanRaceCodes = NvssRace.GetBooleanRaceCodes();
                var literalRaceCodes = NvssRace.GetLiteralRaceCodes();
                foreach (Tuple<string, string> element in value)
                {
                    obs.Component.RemoveAll(c => c.Code.Coding[0].Code == element.Item1);
                    Observation.ComponentComponent component = new Observation.ComponentComponent();
                    String displayValue = NvssRace.GetDisplayValueForCode(element.Item1);
                    component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, element.Item1, displayValue, null);
                    if (booleanRaceCodes.Contains(element.Item1))
                    {
                        if (element.Item2 == "Y")
                        {
                            component.Value = new FhirBoolean(true);
                        }
                        else
                        {
                            component.Value = new FhirBoolean(false);
                        }
                    }
                    else if (literalRaceCodes.Contains(element.Item1))
                    {
                        component.Value = new FhirString(element.Item2);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid race literal code found: " + element.Item1 + " with value: " + element.Item2);
                    }
                    obs.Component.Add(component);
                }

            }
        }

        /// <summary>Father's Ethnicity Hispanic Mexican.</summary>
        /// <value>the father's ethnicity. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "Y");</para>
        /// <para>ethnicity.Add("system", CodeSystems.YesNo);</para>
        /// <para>ethnicity.Add("display", "Yes");</para>
        /// <para>ExampleBirthRecord.FatherEthnicity1 = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Ethnicity: {ExampleBirthRecord.FatherEthnicity1["display"]}");</para>
        /// </example>
        [Property("FatherEthnicity1", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Father's Ethnicity Hispanic Mexican.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherEthnicity1
        {
            get
            {
                Observation obs = GetOrCreateObservation("inputraceandethnicityFather", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_FATHER);
                Observation.ComponentComponent ethnicity = obs.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Mexican);
                if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                {
                    return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (value["code"] == "") {
                    return;
                }
                Observation obs = GetOrCreateObservation("inputraceandethnicityFather", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_FATHER);
                obs.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Mexican);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, NvssEthnicity.Mexican, NvssEthnicity.MexicanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                obs.Component.Add(component);
                obs.Subject = new ResourceReference("urn:uuid:" + Child.Id);
            }
        }

        /// <summary>Father's Ethnicity 1 Helper</summary>
        /// <value>Father's Ethnicity 1.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VR.ValueSets.YesNoUnknown.Yes;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Ethnicity: {ExampleBirthRecord.FatherEthnicity1Helper}");</para>
        /// </example>
        [Property("Father Ethnicity 1 Helper", Property.Types.String, "Race and Ethnicity Profiles", "Father's Ethnicity 1.", false, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        public string FatherEthnicity1Helper
        {
            get
            {
                if (FatherEthnicity1.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherEthnicity1["code"]))
                {
                    return FatherEthnicity1["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherEthnicity1", value, VR.ValueSets.HispanicNoUnknown.Codes);
                }
            }
        }

        /// <summary>Father's Ethnicity Hispanic PuertoRican.</summary>
        /// <value>the father's ethnicity. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "Y");</para>
        /// <para>ethnicity.Add("system", CodeSystems.YesNo);</para>
        /// <para>ethnicity.Add("display", "Yes");</para>
        /// <para>ExampleBirthRecord.FatherEthnicity1 = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Ethnicity: {ExampleBirthRecord.FatherEthnicity2["display"]}");</para>
        /// </example>
        [Property("FatherEthnicity2", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Father's Ethnicity Hispanic PuertoRican.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherEthnicity2
        {
            get
            {
                Observation obs = GetOrCreateObservation("inputraceandethnicityFather", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_FATHER);
                Observation.ComponentComponent ethnicity = obs.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.PuertoRican);
                if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                {
                    return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (value["code"] == "") {
                    return;
                }
                Observation obs = GetOrCreateObservation("inputraceandethnicityFather", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_FATHER);
                obs.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.PuertoRican);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, NvssEthnicity.PuertoRican, NvssEthnicity.PuertoRicanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                obs.Component.Add(component);
                obs.Subject = new ResourceReference("urn:uuid:" + Child.Id);
            }
        }

        /// <summary>Father's Ethnicity 2 Helper</summary>
        /// <value>Father's Ethnicity 2.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VR.ValueSets.YesNoUnknown.Yes;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Ethnicity: {ExampleBirthRecord.FatherEthnicity2Helper}");</para>
        /// </example>
        [Property("Father Ethnicity 2 Helper", Property.Types.String, "Race and Ethnicity Profiles", "Father's Ethnicity 2.", false, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        public string FatherEthnicity2Helper
        {
            get
            {
                if (FatherEthnicity2.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherEthnicity2["code"]))
                {
                    return FatherEthnicity2["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherEthnicity2", value, VR.ValueSets.HispanicNoUnknown.Codes);
                }
            }
        }

        /// <summary>Father's Ethnicity Hispanic Cuban.</summary>
        /// <value>the father's ethnicity. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "Y");</para>
        /// <para>ethnicity.Add("system", CodeSystems.YesNo);</para>
        /// <para>ethnicity.Add("display", "Yes");</para>
        /// <para>ExampleBirthRecord.FatherEthnicity3 = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Ethnicity: {ExampleBirthRecord.FatherEthnicity3["display"]}");</para>
        /// </example>
        [Property("FatherEthnicity3", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Father's Ethnicity Hispanic Cuban.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherEthnicity3
        {
            get
            {
                Observation obs = GetOrCreateObservation("inputraceandethnicityFather", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_FATHER);
                Observation.ComponentComponent ethnicity = obs.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Cuban);
                if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                {
                    return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (value["code"] == "") {
                    return;
                }
                Observation obs = GetOrCreateObservation("inputraceandethnicityFather", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_FATHER);
                obs.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Cuban);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, NvssEthnicity.Cuban, NvssEthnicity.CubanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                obs.Component.Add(component);
                obs.Subject = new ResourceReference("urn:uuid:" + Child.Id);
            }
        }

        /// <summary>Father's Ethnicity 3 Helper</summary>
        /// <value>Father's Ethnicity 3.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VR.ValueSets.YesNoUnknown.Yes;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Ethnicity: {ExampleBirthRecord.FatherEthnicity3Helper}");</para>
        /// </example>
        [Property("Father Ethnicity 3 Helper", Property.Types.String, "Race and Ethnicity Profiles", "Father's Ethnicity 3.", false, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        public string FatherEthnicity3Helper
        {
            get
            {
                if (FatherEthnicity3.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherEthnicity3["code"]))
                {
                    return FatherEthnicity3["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherEthnicity3", value, VR.ValueSets.HispanicNoUnknown.Codes);
                }
            }
        }


        /// <summary>Father's Ethnicity Hispanic Other.</summary>
        /// <value>the father's ethnicity. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "Y");</para>
        /// <para>ethnicity.Add("system", CodeSystems.YesNo);</para>
        /// <para>ethnicity.Add("display", "Yes");</para>
        /// <para>ExampleBirthRecord.FatherEthnicity4 = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Ethnicity: {ExampleBirthRecord.FatherEthnicity4["display"]}");</para>
        /// </example>
        [Property("FatherEthnicity4", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Father's Ethnicity Hispanic Other.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherEthnicity4
        {
            get
            {
                Observation obs = GetOrCreateObservation("inputraceandethnicityFather", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_FATHER);
                Observation.ComponentComponent ethnicity = obs.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Other);
                if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                {
                    return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (value["code"] == "") {
                    return;
                }
                Observation obs = GetOrCreateObservation("inputraceandethnicityFather", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_FATHER);
                obs.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Other);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, NvssEthnicity.Other, NvssEthnicity.OtherDisplay, null);
                component.Value = DictToCodeableConcept(value);
                obs.Component.Add(component);
                obs.Subject = new ResourceReference("urn:uuid:" + Child.Id);
            }
        }

        /// <summary>Father's Ethnicity 4 Helper</summary>
        /// <value>Father's Ethnicity 4.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VR.ValueSets.YesNoUnknown.Yes;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Ethnicity: {ExampleBirthRecord.FatherEthnicity4Helper}");</para>
        /// </example>
        [Property("Father Ethnicity 4 Helper", Property.Types.String, "Race and Ethnicity Profiles", "Father's Ethnicity 4.", false, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        public string FatherEthnicity4Helper
        {
            get
            {
                if (FatherEthnicity4.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherEthnicity4["code"]))
                {
                    return FatherEthnicity4["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherEthnicity4", value, VR.ValueSets.HispanicNoUnknown.Codes);
                }
            }
        }

        /// <summary>Father's Ethnicity Hispanic Literal.</summary>
        /// <value>the father's ethnicity. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLiteral = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Ethnicity: {ExampleBirthRecord.EthnicityLiteral["display"]}");</para>
        /// </example>
        [Property("FatherEthnicityLiteral", Property.Types.String, "Race and Ethnicity Profiles", "Father's Ethnicity Literal.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("ethnicity", "The literal string to describe ethnicity.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string FatherEthnicityLiteral
        {
            get => GetEthnicityLiteral("inputraceandethnicityFather", RACE_ETHNICITY_PROFILE_FATHER);
            set => SetEthnicityLiteral(value, Father.Id, "inputraceandethnicityFather", RACE_ETHNICITY_PROFILE_FATHER);
        }

        /// <summary>Father's Race values.</summary>
        /// <value>the father's race. A tuple, where the first value of the tuple is the display value, and the second is
        /// the IJE code Y or N.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherRace = {NvssRace.BlackOrAfricanAmerican, "Y"};</para>
        /// <para>// Getter:</para>
        /// <para>string boaa = ExampleBirthRecord.RaceBlackOfAfricanAmerican;</para>
        /// </example>
        [Property("FatherRace", Property.Types.TupleArr, "Race and Ethnicity Profiles", "Father's Race", true, VR.IGURL.InputRaceAndEthnicity, true, 38)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Tuple<string, string>[] FatherRace
        {
            get
            {
                // filter the boolean race values
                var booleanRaceCodes = NvssRace.GetBooleanRaceCodes();
                List<string> raceCodes = booleanRaceCodes.Concat(NvssRace.GetLiteralRaceCodes()).ToList();

                var races = new List<Tuple<string, string>>() { };

                Observation obs = GetOrCreateObservation("inputraceandethnicityFather", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_FATHER);

                if (!obs.Component.Any())
                {
                    return races.ToArray();
                }
                foreach (string raceCode in raceCodes)
                {
                    Observation.ComponentComponent component = obs.Component.Where(c => c.Code.Coding[0].Code == raceCode).FirstOrDefault();
                    if (component != null)
                    {
                        // convert boolean race codes to strings
                        if (booleanRaceCodes.Contains(raceCode))
                        {
                            if (component.Value == null)
                            {
                                // If there is no value given, set the race to blank.
                                var race = Tuple.Create(raceCode, "");
                                races.Add(race);
                                continue;
                            }

                            bool? raceBool = ((FhirBoolean)component.Value).Value;

                            if (raceBool.Value)
                            {
                                var race = Tuple.Create(raceCode, "Y");
                                races.Add(race);
                            }
                            else
                            {
                                var race = Tuple.Create(raceCode, "N");
                                races.Add(race);
                            }
                        }
                        else
                        {
                            // Ignore unless there's a value present
                            if (component.Value != null)
                            {
                                var race = Tuple.Create(raceCode, component.Value.ToString());
                                races.Add(race);
                            }
                        }

                    }
                }

                return races.ToArray();
            }
            set
            {
                if (value.FirstOrDefault() == null) {
                    return;
                }
                Observation obs = GetOrCreateObservation("inputraceandethnicityFather", CodeSystems.InputRaceAndEthnicityPerson, "Input Race and Ethnicity Person", VR.ProfileURL.InputRaceAndEthnicity, RACE_ETHNICITY_PROFILE_FATHER);
                var booleanRaceCodes = NvssRace.GetBooleanRaceCodes();
                var literalRaceCodes = NvssRace.GetLiteralRaceCodes();
                foreach (Tuple<string, string> element in value)
                {
                    obs.Component.RemoveAll(c => c.Code.Coding[0].Code == element.Item1);
                    Observation.ComponentComponent component = new Observation.ComponentComponent();
                    String displayValue = NvssRace.GetDisplayValueForCode(element.Item1);
                    component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, element.Item1, displayValue, null);
                    if (booleanRaceCodes.Contains(element.Item1))
                    {
                        if (element.Item2 == "Y")
                        {
                            component.Value = new FhirBoolean(true);
                        }
                        else
                        {
                            component.Value = new FhirBoolean(false);
                        }
                    }
                    else if (literalRaceCodes.Contains(element.Item1))
                    {
                        component.Value = new FhirString(element.Item2);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid race literal code found: " + element.Item1 + " with value: " + element.Item2);
                    }
                    obs.Component.Add(component);
                }
            }
        }

        private Dictionary<string, string> GetCodedRaceEthnicity(string observationCode, string componentCode, string section, [CallerMemberName] string propertyName = null) {
            Observation obs = GetOrCreateObservation(observationCode, CodeSystems.LocalObservationCodes, "Coded Race and Ethnicity Person", VR.ProfileURL.CodedRaceAndEthnicity, section, null, propertyName);
            Observation.ComponentComponent raceEthnicity = obs.Component.FirstOrDefault(c => c.Code.Coding[0].Code == componentCode);
            if (raceEthnicity!= null && raceEthnicity.Value != null && raceEthnicity.Value as CodeableConcept != null)
            {
                return CodeableConceptToDict((CodeableConcept)raceEthnicity.Value);
            }
            return EmptyCodeableDict();
        }
        private void SetCodedRaceEthnicity(Dictionary<string, string>  value, string observationCode, string ComponentCode, string ComponentDisplay, string section, [CallerMemberName] string propertyName = null) {
            if (value["code"] == "") {
                return;
            }
            Observation obs = GetOrCreateObservation(observationCode, CodeSystems.LocalObservationCodes, "Coded Race and Ethnicity Person", VR.ProfileURL.CodedRaceAndEthnicity, section, propertyName: propertyName);
            obs.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.CodeForLiteral);
            Observation.ComponentComponent component = new Observation.ComponentComponent();
            component.Code = new CodeableConcept(CodeSystems.ComponentCodeVR, ComponentCode, ComponentDisplay, null);
            component.Value = DictToCodeableConcept(value);
            obs.Component.Add(component);
            obs.Subject = new ResourceReference("urn:uuid:" + Child.Id);
        }

        /// <summary>Mother Race Tabulation 1E.</summary>
        /// <value>Mother Race Tabulation 1E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.MotherRaceTabulation1E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation1E: {ExampleBirthRecord.MotherRaceTabulation1E['display']}");</para>
        /// </example>
        [Property("Mother Race Tabulation 1E", Property.Types.Dictionary, "Coded Content", "Mother Race Tabulation 1E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 62)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherRaceTabulation1E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FirstEditedCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FirstEditedCode, NvssRace.FirstEditedCodeDisplay, "MTH");
        }

        /// <summary>Mother Race Tabulation 1E Helper</summary>
        /// <value>Mother Race Tabulation 1E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation1E: {ExampleBirthRecord.MotherRaceTabulation1EHelper}");</para>
        /// </example>
        [Property("Mother Race Tabulation 1E Helper", Property.Types.String, "Coded Content", "Mother Race Tabulation 1E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 62)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherRaceTabulation1EHelper
        {
            get
            {
                if (MotherRaceTabulation1E.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherRaceTabulation1E["code"]))
                {
                    return MotherRaceTabulation1E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherRaceTabulation1E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Race Tabulation 2E.</summary>
        /// <value>Mother Race Tabulation 2E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.MotherRaceTabulation2E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation2E: {ExampleBirthRecord.MotherRaceTabulation2E['display']}");</para>
        /// </example>
        [Property("Mother Race Tabulation 2E", Property.Types.Dictionary, "Coded Content", "Mother Race Tabulation 2E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 63)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherRaceTabulation2E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SecondEditedCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SecondEditedCode, NvssRace.SecondEditedCodeDisplay, "MTH");
        }

        /// <summary>Mother Race Tabulation 2E Helper</summary>
        /// <value>Mother Race Tabulation 2E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation2E: {ExampleBirthRecord.MotherRaceTabulation2EHelper}");</para>
        /// </example>
        [Property("Mother Race Tabulation 2E Helper", Property.Types.String, "Coded Content", "Mother Race Tabulation 2E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 63)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherRaceTabulation2EHelper
        {
            get
            {
                if (MotherRaceTabulation2E.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherRaceTabulation2E["code"]))
                {
                    return MotherRaceTabulation2E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherRaceTabulation2E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }


                /// <summary>Mother Race Tabulation 3E.</summary>
        /// <value>Mother Race Tabulation 3E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.MotherRaceTabulation3E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation3E: {ExampleBirthRecord.MotherRaceTabulation3E['display']}");</para>
        /// </example>
        [Property("Mother Race Tabulation 3E", Property.Types.Dictionary, "Coded Content", "Mother Race Tabulation 3E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 64)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherRaceTabulation3E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.ThirdEditedCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.ThirdEditedCode, NvssRace.ThirdEditedCodeDisplay, "MTH");
        }

        /// <summary>Mother Race Tabulation 3E Helper</summary>
        /// <value>Mother Race Tabulation 3E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation3E: {ExampleBirthRecord.MotherRaceTabulation3EHelper}");</para>
        /// </example>
        [Property("Mother Race Tabulation 3E Helper", Property.Types.String, "Coded Content", "Mother Race Tabulation 3E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 64)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherRaceTabulation3EHelper
        {
            get
            {
                if (MotherRaceTabulation3E.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherRaceTabulation3E["code"]))
                {
                    return MotherRaceTabulation3E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherRaceTabulation3E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Race Tabulation 4E.</summary>
        /// <value>Mother Race Tabulation 4E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.MotherRaceTabulation4E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation4E: {ExampleBirthRecord.MotherRaceTabulation4E['display']}");</para>
        /// </example>
        [Property("Mother Race Tabulation 4E", Property.Types.Dictionary, "Coded Content", "Mother Race Tabulation 4E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 65)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherRaceTabulation4E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FourthEditedCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FourthEditedCode, NvssRace.FourthEditedCodeDisplay, "MTH");
        }

        /// <summary>Mother Race Tabulation 4E Helper</summary>
        /// <value>Mother Race Tabulation 4E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation4E: {ExampleBirthRecord.MotherRaceTabulation4EHelper}");</para>
        /// </example>
        [Property("Mother Race Tabulation 4E Helper", Property.Types.String, "Coded Content", "Mother Race Tabulation 4E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 65)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherRaceTabulation4EHelper
        {
            get
            {
                if (MotherRaceTabulation4E.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherRaceTabulation4E["code"]))
                {
                    return MotherRaceTabulation4E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherRaceTabulation4E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Race Tabulation 5E.</summary>
        /// <value>Mother Race Tabulation 5E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.MotherRaceTabulation5E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation5E: {ExampleBirthRecord.MotherRaceTabulation5E['display']}");</para>
        /// </example>
        [Property("Mother Race Tabulation 5E", Property.Types.Dictionary, "Coded Content", "Mother Race Tabulation 5E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 66)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherRaceTabulation5E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FifthEditedCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FifthEditedCode, NvssRace.FifthEditedCodeDisplay, "MTH");
        }

        /// <summary>Mother Race Tabulation 5E Helper</summary>
        /// <value>Mother Race Tabulation 5E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation5E: {ExampleBirthRecord.MotherRaceTabulation5EHelper}");</para>
        /// </example>
        [Property("Mother Race Tabulation 5E Helper", Property.Types.String, "Coded Content", "Mother Race Tabulation 5E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 66)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherRaceTabulation5EHelper
        {
            get
            {
                if (MotherRaceTabulation5E.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherRaceTabulation5E["code"]))
                {
                    return MotherRaceTabulation5E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherRaceTabulation5E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Race Tabulation 6E.</summary>
        /// <value>Mother Race Tabulation 6E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.MotherRaceTabulation6E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation6E: {ExampleBirthRecord.MotherRaceTabulation6E['display']}");</para>
        /// </example>
        [Property("Mother Race Tabulation 6E", Property.Types.Dictionary, "Coded Content", "Mother Race Tabulation 6E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 67)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherRaceTabulation6E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SixthEditedCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SixthEditedCode, NvssRace.SixthEditedCodeDisplay, "MTH");
        }

        /// <summary>Mother Race Tabulation 6E Helper</summary>
        /// <value>Mother Race Tabulation 6E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation6E: {ExampleBirthRecord.MotherRaceTabulation6EHelper}");</para>
        /// </example>
        [Property("Mother Race Tabulation 6E Helper", Property.Types.String, "Coded Content", "Mother Race Tabulation 6E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 67)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherRaceTabulation6EHelper
        {
            get
            {
                if (MotherRaceTabulation6E.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherRaceTabulation6E["code"]))
                {
                    return MotherRaceTabulation6E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherRaceTabulation6E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Race Tabulation 7E.</summary>
        /// <value>Mother Race Tabulation 7E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.MotherRaceTabulation7E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation7E: {ExampleBirthRecord.MotherRaceTabulation7E['display']}");</para>
        /// </example>
        [Property("Mother Race Tabulation 7E", Property.Types.Dictionary, "Coded Content", "Mother Race Tabulation 7E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 68)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherRaceTabulation7E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SeventhEditedCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SeventhEditedCode, NvssRace.SeventhEditedCodeDisplay, "MTH");
        }

        /// <summary>Mother Race Tabulation 7E Helper</summary>
        /// <value>Mother Race Tabulation 7E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation7E: {ExampleBirthRecord.MotherRaceTabulation7EHelper}");</para>
        /// </example>
        [Property("Mother Race Tabulation 7E Helper", Property.Types.String, "Coded Content", "Mother Race Tabulation 7E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 68)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherRaceTabulation7EHelper
        {
            get
            {
                if (MotherRaceTabulation7E.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherRaceTabulation7E["code"]))
                {
                    return MotherRaceTabulation7E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherRaceTabulation7E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Race Tabulation 8E.</summary>
        /// <value>Mother Race Tabulation 8E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.MotherRaceTabulation8E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherRaceTabulation8E: {ExampleBirthRecord.MotherRaceTabulation8E['display']}");</para>
        /// </example>
        [Property("Mother Race Tabulation 8E", Property.Types.Dictionary, "Coded Content", "Mother Race Tabulation 8E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 69)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherRaceTabulation8E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.EighthEditedCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.EighthEditedCode, NvssRace.EighthEditedCodeDisplay, "MTH");
        }

        /// <summary>Mother Race Tabulation 8E Helper</summary>
        /// <value>Mother Race Tabulation 8E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherRaceTabulation8EHelper = VR.ValueSets.RaceCode.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Race: {ExampleBirthRecord.MotherRaceTabulation8EHelper}");</para>
        /// </example>
        [Property("Mother Race Tabulation 8E Helper", Property.Types.String, "Coded Content", "Mother Race Tabulation 8E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 69)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherRaceTabulation8EHelper
        {
            get
            {
                if (MotherRaceTabulation8E.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherRaceTabulation8E["code"]))
                {
                    return MotherRaceTabulation8E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherRaceTabulation8E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother First American Indian Code.</summary>
        /// <value>Mother First American Indian Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Ahtna");</para>
        /// <para>ExampleDeathRecord.FirstAmericanIndianCode = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FirstAmericanIndianCode: {ExampleBirthRecord.FirstAmericanIndianCode['display']}");</para>
        /// </example>
        [Property("Mother First American Indian Code", Property.Types.Dictionary, "Coded Content", "Mother First American Indian Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 70)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherFirstAmericanIndianCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FirstAmericanIndianCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FirstAmericanIndianCode, NvssRace.FirstAmericanIndianCodeDisplay, "MTH");
        }

        /// <summary>Mother First American Indian Code Helper</summary>
        /// <value>Mother First American Indian Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherFirstAmericanIndianCodeHelper = VR.ValueSets.RaceCodeAhtna;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Race: {ExampleBirthRecord.MotherFirstAmericanIndianCodeHelper}");</para>
        /// </example>
        [Property("Mother First American Indian Code Helper", Property.Types.String, "Coded Content", "Mother First American Indian Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 70)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherFirstAmericanIndianCodeHelper
        {
            get
            {
                if (MotherFirstAmericanIndianCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherFirstAmericanIndianCode["code"]))
                {
                    return MotherFirstAmericanIndianCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherFirstAmericanIndianCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Second American Indian Code.</summary>
        /// <value>Mother Second American Indian Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Ahtna");</para>
        /// <para>ExampleDeathRecord.MotherSecondAmericanIndianCode = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherSecondAmericanIndianCode: {ExampleBirthRecord.MotherSecondAmericanIndianCode['display']}");</para>
        /// </example>
        [Property("Mother Second American Indian Code", Property.Types.Dictionary, "Coded Content", "Mother Second American Indian Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 71)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherSecondAmericanIndianCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SecondAmericanIndianCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SecondAmericanIndianCode, NvssRace.SecondAmericanIndianCodeDisplay, "MTH");
        }

        /// <summary>Mother Second American Indian Code Helper</summary>
        /// <value>Mother Second American Indian Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherSecondAmericanIndianCodeHelper = VR.ValueSets.RaceCodeAhtna;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Race: {ExampleBirthRecord.MotherSecondAmericanIndianCodeHelper}");</para>
        /// </example>
        [Property("Mother Second American Indian Code Helper", Property.Types.String, "Coded Content", "Mother Second American Indian Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 71)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherSecondAmericanIndianCodeHelper
        {
            get
            {
                if (MotherSecondAmericanIndianCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherSecondAmericanIndianCode["code"]))
                {
                    return MotherSecondAmericanIndianCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherSecondAmericanIndianCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother First Other Asian Code.</summary>
        /// <value>Mother First Other Asian Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "421");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Filipino");</para>
        /// <para>ExampleDeathRecord.MotherFirstOtherAsianCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherFirstOtherAsianCode: {ExampleBirthRecord.MotherFirstOtherAsianCode['display']}");</para>
        /// </example>
        [Property("Mother First Other Asian Code", Property.Types.Dictionary, "Coded Content", "Mother First Other Asian Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 72)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherFirstOtherAsianCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FirstOtherAsianCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FirstOtherAsianCode, NvssRace.FirstOtherAsianCodeDisplay, "MTH");
        }

        /// <summary>Mother First Other Asian Code Helper</summary>
        /// <value>Mother First Other Asian Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherFirstOtherAsianCodeHelper = VR.ValueSets.RaceCodeFilipino;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Race: {ExampleBirthRecord.MotherFirstOtherAsianCodeHelper}");</para>
        /// </example>
        [Property("Mother First Other Asian Code Helper", Property.Types.String, "Coded Content", "Mother First Other Asian Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 72)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherFirstOtherAsianCodeHelper
        {
            get
            {
                if (MotherFirstOtherAsianCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherFirstOtherAsianCode["code"]))
                {
                    return MotherFirstOtherAsianCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherFirstOtherAsianCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Second Other Asian Code.</summary>
        /// <value>Mother Second Other Asian Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "421");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Filipino");</para>
        /// <para>ExampleDeathRecord.MotherSecondOtherAsianCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherSecondOtherAsianCode: {ExampleBirthRecord.MotherSecondOtherAsianCode['display']}");</para>
        /// </example>
        [Property("Mother Second Other Asian Code", Property.Types.Dictionary, "Coded Content", "Mother Second Other Asian Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 73)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherSecondOtherAsianCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SecondOtherAsianCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SecondOtherAsianCode, NvssRace.SecondOtherAsianCodeDisplay, "MTH");
        }

        /// <summary>Mother Second Other Asian Code Helper</summary>
        /// <value>Mother Second Other Asian Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherSecondOtherAsianCodeHelper = VR.ValueSets.RaceCodeFilipino;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Race: {ExampleBirthRecord.MotherSecondOtherAsianCodeHelper}");</para>
        /// </example>
        [Property("Mother Second Other Asian Code Helper", Property.Types.String, "Coded Content", "Mother Second Other Asian Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 73)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherSecondOtherAsianCodeHelper
        {
            get
            {
                if (MotherSecondOtherAsianCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherSecondOtherAsianCode["code"]))
                {
                    return MotherSecondOtherAsianCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherSecondOtherAsianCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }
        
        /// <summary>Mother First Other Pacific Islander Code.</summary>
        /// <value>Mother First Other Pacific Islander Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "531");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Mariana Islander");</para>
        /// <para>ExampleDeathRecord.MotherFirstOtherPacificIslanderCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherFirstOtherPacificIslanderCode: {ExampleBirthRecord.MotherFirstOtherPacificIslanderCode['display']}");</para>
        /// </example>
        [Property("Mother First Other Pacific Islander Code", Property.Types.Dictionary, "Coded Content", "Mother First Other Pacific Islander Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 74)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherFirstOtherPacificIslanderCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FirstOtherPacificIslanderCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FirstOtherPacificIslanderCode, NvssRace.FirstOtherPacificIslanderCodeDisplay, "MTH");
        }

        /// <summary>Mother First Other Pacific Islander Code Helper</summary>
        /// <value>Mother First Other Pacific Islander Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherFirstOtherPacificIslanderCodeHelper = VR.ValueSets.RaceCodeMariana_Islander;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Race: {ExampleBirthRecord.MotherFirstOtherPacificIslanderCodeHelper}");</para>
        /// </example>
        [Property("Mother First Other Pacific Islander Code Helper", Property.Types.String, "Coded Content", "Mother First Other Pacific Islander Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 74)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherFirstOtherPacificIslanderCodeHelper
        {
            get
            {
                if (MotherFirstOtherPacificIslanderCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherFirstOtherPacificIslanderCode["code"]))
                {
                    return MotherFirstOtherPacificIslanderCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherFirstOtherPacificIslanderCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Second Other Pacific Islander Code.</summary>
        /// <value>Mother Second Other Pacific Islander Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "531");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Mariana Islander");</para>
        /// <para>ExampleDeathRecord.MotherSecondOtherPacificIslanderCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherSecondOtherPacificIslanderCode: {ExampleBirthRecord.MotherSecondOtherPacificIslanderCode['display']}");</para>
        /// </example>
        [Property("Mother Second Other Pacific Islander Code", Property.Types.Dictionary, "Coded Content", "Mother Second Other Pacific Islander Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 75)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherSecondOtherPacificIslanderCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SecondOtherPacificIslanderCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SecondOtherPacificIslanderCode, NvssRace.SecondOtherPacificIslanderCodeDisplay, "MTH");
        }

        /// <summary>Mother Second Other Pacific Islander Code Helper</summary>
        /// <value>Mother Second Other Pacific Islander Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherSecondOtherPacificIslanderCodeHelper = VR.ValueSets.RaceCodeMariana_Islander;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Race: {ExampleBirthRecord.MotherSecondOtherPacificIslanderCodeHelper}");</para>
        /// </example>
        [Property("Mother Second Other Pacific Islander Code Helper", Property.Types.String, "Coded Content", "Mother Second Other Pacific Islander Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 75)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherSecondOtherPacificIslanderCodeHelper
        {
            get
            {
                if (MotherSecondOtherPacificIslanderCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherSecondOtherPacificIslanderCode["code"]))
                {
                    return MotherSecondOtherPacificIslanderCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherSecondOtherPacificIslanderCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother First Other Race Code.</summary>
        /// <value>Mother First Other Race Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "531");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Mariana Islander");</para>
        /// <para>ExampleDeathRecord.MotherFirstOtherRaceCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherFirstOtherRaceCode: {ExampleBirthRecord.MotherFirstOtherRaceCode['display']}");</para>
        /// </example>
        [Property("Mother First Other Race Code", Property.Types.Dictionary, "Coded Content", "Mother First Other Race Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 76)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherFirstOtherRaceCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FirstOtherRaceCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.FirstOtherRaceCode, NvssRace.FirstOtherRaceCodeDisplay, "MTH");
        }

        /// <summary>Mother First Other Race Code Helper</summary>
        /// <value>Mother First Other Race Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherFirstOtherRaceCodeHelper = VR.ValueSets.RaceCodeMariana_Islander;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Race: {ExampleBirthRecord.MotherFirstOtherRaceCodeHelper}");</para>
        /// </example>
        [Property("Mother First Other Race Code Helper", Property.Types.String, "Coded Content", "Mother First Other Race Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 76)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherFirstOtherRaceCodeHelper
        {
            get
            {
                if (MotherFirstOtherRaceCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherFirstOtherRaceCode["code"]))
                {
                    return MotherFirstOtherRaceCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherFirstOtherRaceCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Second Other Race Code.</summary>
        /// <value>Mother Second Other Race Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "531");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Mariana Islander");</para>
        /// <para>ExampleDeathRecord.MotherSecondOtherRaceCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherSecondOtherRaceCode: {ExampleBirthRecord.MotherSecondOtherRaceCode['display']}");</para>
        /// </example>
        [Property("Mother Second Other Race Code", Property.Types.Dictionary, "Coded Content", "Mother Second Other Race Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 77)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherSecondOtherRaceCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SecondOtherRaceCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssRace.SecondOtherRaceCode, NvssRace.SecondOtherRaceCodeDisplay, "MTH");
        }

        /// <summary>Mother Second Other Race Code Helper</summary>
        /// <value>MOther Second Other Race Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherSecondOtherRaceCodeHelper = VR.ValueSets.RaceCodeMariana_Islander;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Race: {ExampleBirthRecord.MotherSecondOtherRaceCodeHelper}");</para>
        /// </example>
        [Property("Mother Second Other Race Code Helper", Property.Types.String, "Coded Content", "Mother Second Other Race Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 77)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherSecondOtherRaceCodeHelper
        {
            get
            {
                if (MotherSecondOtherRaceCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherSecondOtherRaceCode["code"]))
                {
                    return MotherSecondOtherRaceCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherSecondOtherRaceCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Ethnicity Code For Literal.</summary>
        /// <value>Mother Ethnicity Code For Literal. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.VRCLHispanicOrigin);</para>
        /// <para>ethnicity.Add("display", "Chicano");</para>
        /// <para>ExampleDeathRecord.MotherEthnicityCodeForLiteral = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Ethnicity Code For Literal: {ExampleBirthRecord.MotherEthnicityCodeForLiteral['display']}");</para>
        /// </example>
        [Property("Mother Ethnicity Code For Literal", Property.Types.Dictionary, "Coded Content", "Ethnicity Code For Literal.", true, VR.IGURL.CodedRaceAndEthnicity, false, 292)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherEthnicityCodeForLiteral
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssEthnicity.CodeForLiteral, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssEthnicity.CodeForLiteral, NvssEthnicity.CodeForLiteralDisplay, "MTH");
        }

        /// <summary>Mother Ethnicity Code For Literal Helper</summary>
        /// <value>Mother Ethnicity Code For Literal Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Andalusian;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Ethnicity: {ExampleBirthRecord.MotherEthnicityCodeForLiteralHelper}");</para>
        /// </example>
        [Property("Mother Ethnicity Code For Literal Helper", Property.Types.String, "Coded Content", "Mother Ethnicity Code For Literal Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 292)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherEthnicityCodeForLiteralHelper
        {
            get
            {
                if (MotherEthnicityCodeForLiteral.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherEthnicityCodeForLiteral["code"]))
                {
                    return MotherEthnicityCodeForLiteral["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherEthnicityCodeForLiteral", value, VR.ValueSets.HispanicOrigin.Codes);
                }
            }
        }

        /// <summary>Mother Ethnicity Edited Code.</summary>
        /// <value>Mother Ethnicity Edited Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.VRCLHispanicOrigin);</para>
        /// <para>ethnicity.Add("display", "Chicano");</para>
        /// <para>ExampleDeathRecord.MotherEthnicityEditedCode = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Ethnicity Edited Code: {ExampleBirthRecord.MotherEthnicityEditedCode['display']}");</para>
        /// </example>
        [Property("Mother Ethnicity Edited Code", Property.Types.Dictionary, "Coded Content", "Mother Ethnicity Edited Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 293)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherEthnicityEditedCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssEthnicity.EditedCode, "MTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_MOTHER, NvssEthnicity.EditedCode, NvssEthnicity.EditedCodeDisplay, "MTH");
        }

        /// <summary>Mother Ethnicity Edited Code Helper</summary>
        /// <value>Mother Ethnicity Edited Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherEthnicityCodeHelper = VR.ValueSets.HispanicOrigin.Andalusian;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Ethnicity: {ExampleBirthRecord.MotherEthnicityEditedCodeHelper}");</para>
        /// </example>
        [Property("Mother Ethnicity Edited Code Helper", Property.Types.String, "Coded Content", "Mother Ethnicity Edited Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 293)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityMother')", "")]
        public string MotherEthnicityEditedCodeHelper
        {
            get
            {
                if (MotherEthnicityEditedCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherEthnicityEditedCode["code"]))
                {
                    return MotherEthnicityEditedCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherEthnicityEditedCode", value, VR.ValueSets.HispanicOrigin.Codes);
                }
            }
        }


        /// <summary>Father Race Tabulation 1E.</summary>
        /// <value>Father Race Tabulation 1E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.FatherRaceTabulation1E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation1E: {ExampleBirthRecord.FatherRaceTabulation1E['display']}");</para>
        /// </example>
        [Property("Father Race Tabulation 1E", Property.Types.Dictionary, "Coded Content", "Father Race Tabulation 1E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 108)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherRaceTabulation1E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FirstEditedCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FirstEditedCode, NvssRace.FirstEditedCodeDisplay, "NFTH");
        }

        /// <summary>Father Race Tabulation 1E Helper</summary>
        /// <value>Father Race Tabulation 1E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation1E: {ExampleBirthRecord.FatherRaceTabulation1EHelper}");</para>
        /// </example>
        [Property("Father Race Tabulation 1E Helper", Property.Types.String, "Coded Content", "Father Race Tabulation 1E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 108)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherRaceTabulation1EHelper
        {
            get
            {
                if (FatherRaceTabulation1E.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherRaceTabulation1E["code"]))
                {
                    return FatherRaceTabulation1E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherRaceTabulation1E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father Race Tabulation 2E.</summary>
        /// <value>Father Race Tabulation 2E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.FatherRaceTabulation2E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation2E: {ExampleBirthRecord.FatherRaceTabulation2E['display']}");</para>
        /// </example>
        [Property("Father Race Tabulation 2E", Property.Types.Dictionary, "Coded Content", "Father Race Tabulation 2E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 109)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherRaceTabulation2E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SecondEditedCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SecondEditedCode, NvssRace.SecondEditedCodeDisplay, "NFTH");
        }

        /// <summary>Father Race Tabulation 2E Helper</summary>
        /// <value>Father Race Tabulation 2E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation2E: {ExampleBirthRecord.FatherRaceTabulation2EHelper}");</para>
        /// </example>
        [Property("Father Race Tabulation 2E Helper", Property.Types.String, "Coded Content", "Father Race Tabulation 2E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 109)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherRaceTabulation2EHelper
        {
            get
            {
                if (FatherRaceTabulation2E.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherRaceTabulation2E["code"]))
                {
                    return FatherRaceTabulation2E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherRaceTabulation2E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }


        /// <summary>Father Race Tabulation 3E.</summary>
        /// <value>Father Race Tabulation 3E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.FatherRaceTabulation3E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation3E: {ExampleBirthRecord.FatherRaceTabulation3E['display']}");</para>
        /// </example>
        [Property("Father Race Tabulation 3E", Property.Types.Dictionary, "Coded Content", "Father Race Tabulation 3E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 110)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherRaceTabulation3E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.ThirdEditedCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.ThirdEditedCode, NvssRace.ThirdEditedCodeDisplay, "NFTH");
        }

        /// <summary>Father Race Tabulation 3E Helper</summary>
        /// <value>Father Race Tabulation 3E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation3E: {ExampleBirthRecord.FatherRaceTabulation3EHelper}");</para>
        /// </example>
        [Property("Father Race Tabulation 3E Helper", Property.Types.String, "Coded Content", "Father Race Tabulation 3E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 110)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherRaceTabulation3EHelper
        {
            get
            {
                if (FatherRaceTabulation3E.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherRaceTabulation3E["code"]))
                {
                    return FatherRaceTabulation3E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherRaceTabulation3E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father Race Tabulation 4E.</summary>
        /// <value>Father Race Tabulation 4E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.FatherRaceTabulation4E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation4E: {ExampleBirthRecord.FatherRaceTabulation4E['display']}");</para>
        /// </example>
        [Property("Father Race Tabulation 4E", Property.Types.Dictionary, "Coded Content", "Father Race Tabulation 4E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 111)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherRaceTabulation4E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FourthEditedCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FourthEditedCode, NvssRace.FourthEditedCodeDisplay, "NFTH");
        }

        /// <summary>Father Race Tabulation 4E Helper</summary>
        /// <value>Father Race Tabulation 4E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation4E: {ExampleBirthRecord.FatherRaceTabulation4EHelper}");</para>
        /// </example>
        [Property("Father Race Tabulation 4E Helper", Property.Types.String, "Coded Content", "Father Race Tabulation 4E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 111)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherRaceTabulation4EHelper
        {
            get
            {
                if (FatherRaceTabulation4E.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherRaceTabulation4E["code"]))
                {
                    return FatherRaceTabulation4E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherRaceTabulation4E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father Race Tabulation 5E.</summary>
        /// <value>Father Race Tabulation 5E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.FatherRaceTabulation5E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation5E: {ExampleBirthRecord.FatherRaceTabulation5E['display']}");</para>
        /// </example>
        [Property("Father Race Tabulation 5E", Property.Types.Dictionary, "Coded Content", "Father Race Tabulation 5E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 112)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherRaceTabulation5E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FifthEditedCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FifthEditedCode, NvssRace.FifthEditedCodeDisplay, "NFTH");
        }

        /// <summary>Father Race Tabulation 5E Helper</summary>
        /// <value>Father Race Tabulation 5E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation5E: {ExampleBirthRecord.FatherRaceTabulation5EHelper}");</para>
        /// </example>
        [Property("Father Race Tabulation 5E Helper", Property.Types.String, "Coded Content", "Father Race Tabulation 5E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 112)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherRaceTabulation5EHelper
        {
            get
            {
                if (FatherRaceTabulation5E.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherRaceTabulation5E["code"]))
                {
                    return FatherRaceTabulation5E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherRaceTabulation5E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father Race Tabulation 6E.</summary>
        /// <value>Father Race Tabulation 6E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.FatherRaceTabulation6E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation6E: {ExampleBirthRecord.FatherRaceTabulation6E['display']}");</para>
        /// </example>
        [Property("Father Race Tabulation 6E", Property.Types.Dictionary, "Coded Content", "Father Race Tabulation 6E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 113)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherRaceTabulation6E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SixthEditedCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SixthEditedCode, NvssRace.SixthEditedCodeDisplay, "NFTH");
        }

        /// <summary>Father Race Tabulation 6E Helper</summary>
        /// <value>Father Race Tabulation 6E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation6E: {ExampleBirthRecord.FatherRaceTabulation6EHelper}");</para>
        /// </example>
        [Property("Father Race Tabulation 6E Helper", Property.Types.String, "Coded Content", "Father Race Tabulation 6E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 113)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherRaceTabulation6EHelper
        {
            get
            {
                if (FatherRaceTabulation6E.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherRaceTabulation6E["code"]))
                {
                    return FatherRaceTabulation6E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherRaceTabulation6E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father Race Tabulation 7E.</summary>
        /// <value>Father Race Tabulation 7E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.FatherRaceTabulation7E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation7E: {ExampleBirthRecord.FatherRaceTabulation7E['display']}");</para>
        /// </example>
        [Property("Father Race Tabulation 7E", Property.Types.Dictionary, "Coded Content", "Father Race Tabulation 7E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 114)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherRaceTabulation7E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SeventhEditedCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SeventhEditedCode, NvssRace.SeventhEditedCodeDisplay, "NFTH");
        }

        /// <summary>Father Race Tabulation 7E Helper</summary>
        /// <value>Father Race Tabulation 7E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotheEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation7E: {ExampleBirthRecord.FatherRaceTabulation7EHelper}");</para>
        /// </example>
        [Property("Father Race Tabulation 7E Helper", Property.Types.String, "Coded Content", "Father Race Tabulation 7E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 114)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherRaceTabulation7EHelper
        {
            get
            {
                if (FatherRaceTabulation7E.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherRaceTabulation7E["code"]))
                {
                    return FatherRaceTabulation7E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherRaceTabulation7E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father Race Tabulation 8E.</summary>
        /// <value>Father Race Tabulation 8E. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Arab");</para>
        /// <para>ExampleDeathRecord.FatherRaceTabulation8E = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherRaceTabulation8E: {ExampleBirthRecord.FatherRaceTabulation8E['display']}");</para>
        /// </example>
        [Property("Father Race Tabulation 8E", Property.Types.Dictionary, "Coded Content", "Father Race Tabulation 8E.", true, VR.IGURL.CodedRaceAndEthnicity, false, 115)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherRaceTabulation8E
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.EighthEditedCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.EighthEditedCode, NvssRace.EighthEditedCodeDisplay, "NFTH");
        }

        /// <summary>Father Race Tabulation 8E Helper</summary>
        /// <value>Father Race Tabulation 8E Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherRaceTabulation8EHelper = VR.ValueSets.RaceCode.Arab;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Race: {ExampleBirthRecord.FatherRaceTabulation8EHelper}");</para>
        /// </example>
        [Property("Father Race Tabulation 8E Helper", Property.Types.String, "Coded Content", "Father Race Tabulation 8E Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 115)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherRaceTabulation8EHelper
        {
            get
            {
                if (FatherRaceTabulation8E.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherRaceTabulation8E["code"]))
                {
                    return FatherRaceTabulation8E["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherRaceTabulation8E", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father First American Indian Code.</summary>
        /// <value>Father First American Indian Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Ahtna");</para>
        /// <para>ExampleDeathRecord.FirstAmericanIndianCode = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FirstAmericanIndianCode: {ExampleBirthRecord.FirstAmericanIndianCode['display']}");</para>
        /// </example>
        [Property("Father First American Indian Code", Property.Types.Dictionary, "Coded Content", "Father First American Indian Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 116)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherFirstAmericanIndianCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FirstAmericanIndianCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FirstAmericanIndianCode, NvssRace.FirstAmericanIndianCodeDisplay, "NFTH");
        }

        /// <summary>Father First American Indian Code Helper</summary>
        /// <value>Father First American Indian Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherFirstAmericanIndianCodeHelper = VR.ValueSets.RaceCodeAhtna;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Race: {ExampleBirthRecord.FatherFirstAmericanIndianCodeHelper}");</para>
        /// </example>
        [Property("Father First American Indian Code Helper", Property.Types.String, "Coded Content", "Father First American Indian Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 116)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherFirstAmericanIndianCodeHelper
        {
            get
            {
                if (FatherFirstAmericanIndianCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherFirstAmericanIndianCode["code"]))
                {
                    return FatherFirstAmericanIndianCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherFirstAmericanIndianCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father Second American Indian Code.</summary>
        /// <value>Father Second American Indian Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.RaceCode);</para>
        /// <para>ethnicity.Add("display", "Ahtna");</para>
        /// <para>ExampleDeathRecord.FatherSecondAmericanIndianCode = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherSecondAmericanIndianCode: {ExampleBirthRecord.FatherSecondAmericanIndianCode['display']}");</para>
        /// </example>
        [Property("Father Second American Indian Code", Property.Types.Dictionary, "Coded Content", "Father Second American Indian Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 117)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherSecondAmericanIndianCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SecondAmericanIndianCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SecondAmericanIndianCode, NvssRace.SecondAmericanIndianCodeDisplay, "NFTH");
        }

        /// <summary>Father Second American Indian Code Helper</summary>
        /// <value>Fathe Second American Indian Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherSecondAmericanIndianCodeHelper = VR.ValueSets.RaceCodeAhtna;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Race: {ExampleBirthRecord.FatherSecondAmericanIndianCodeHelper}");</para>
        /// </example>
        [Property("Father Second American Indian Code Helper", Property.Types.String, "Coded Content", "Father Second American Indian Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 117)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherSecondAmericanIndianCodeHelper
        {
            get
            {
                if (FatherSecondAmericanIndianCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherSecondAmericanIndianCode["code"]))
                {
                    return FatherSecondAmericanIndianCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherSecondAmericanIndianCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father First Other Asian Code.</summary>
        /// <value>Father First Other Asian Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "421");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Filipino");</para>
        /// <para>ExampleDeathRecord.FatherFirstOtherAsianCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherFirstOtherAsianCode: {ExampleBirthRecord.FatherFirstOtherAsianCode['display']}");</para>
        /// </example>
        [Property("Father First Other Asian Code", Property.Types.Dictionary, "Coded Content", "Father First Other Asian Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 118)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherFirstOtherAsianCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FirstOtherAsianCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FirstOtherAsianCode, NvssRace.FirstOtherAsianCodeDisplay, "NFTH");
        }

        /// <summary>Father First Other Asian Code Helper</summary>
        /// <value>Father First Other Asian Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherFirstOtherAsianCodeHelper = VR.ValueSets.RaceCodeFilipino;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Race: {ExampleBirthRecord.FatherFirstOtherAsianCodeHelper}");</para>
        /// </example>
        [Property("Father First Other Asian Code Helper", Property.Types.String, "Coded Content", "Father First Other Asian Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 118)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherFirstOtherAsianCodeHelper
        {
            get
            {
                if (FatherFirstOtherAsianCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherFirstOtherAsianCode["code"]))
                {
                    return FatherFirstOtherAsianCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherFirstOtherAsianCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father Second Other Asian Code.</summary>
        /// <value>Father Second Other Asian Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "421");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Filipino");</para>
        /// <para>ExampleDeathRecord.FatherSecondOtherAsianCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherSecondOtherAsianCode: {ExampleBirthRecord.FatherSecondOtherAsianCode['display']}");</para>
        /// </example>
        [Property("Father Second Other Asian Code", Property.Types.Dictionary, "Coded Content", "Father Second Other Asian Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 119)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherSecondOtherAsianCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SecondOtherAsianCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SecondOtherAsianCode, NvssRace.SecondOtherAsianCodeDisplay, "NFTH");
        }

        /// <summary>Father Second Other Asian Code Helper</summary>
        /// <value>Father Second Other Asian Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherSecondOtherAsianCodeHelper = VR.ValueSets.RaceCodeFilipino;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Race: {ExampleBirthRecord.FatherSecondOtherAsianCodeHelper}");</para>
        /// </example>
        [Property("Father Second Other Asian Code Helper", Property.Types.String, "Coded Content", "Father Second Other Asian Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 119)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherSecondOtherAsianCodeHelper
        {
            get
            {
                if (FatherSecondOtherAsianCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherSecondOtherAsianCode["code"]))
                {
                    return FatherSecondOtherAsianCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherSecondOtherAsianCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }
        
        /// <summary>Father First Other Pacific Islander Code.</summary>
        /// <value>Father First Other Pacific Islander Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "531");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Mariana Islander");</para>
        /// <para>ExampleDeathRecord.FatherFirstOtherPacificIslanderCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherFirstOtherPacificIslanderCode: {ExampleBirthRecord.FatherFirstOtherPacificIslanderCode['display']}");</para>
        /// </example>
        [Property("Father First Other Pacific Islander Code", Property.Types.Dictionary, "Coded Content", "Father First Other Pacific Islander Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 120)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherFirstOtherPacificIslanderCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FirstOtherPacificIslanderCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FirstOtherPacificIslanderCode, NvssRace.FirstOtherPacificIslanderCodeDisplay, "NFTH");
        }

        /// <summary>Father First Other Pacific Islander Code Helper</summary>
        /// <value>Father First Other Pacific Islander Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherFirstOtherPacificIslanderCodeHelper = VR.ValueSets.RaceCodeMariana_Islander;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Race: {ExampleBirthRecord.FatherFirstOtherPacificIslanderCodeHelper}");</para>
        /// </example>
        [Property("Father First Other Pacific Islander Code Helper", Property.Types.String, "Coded Content", "Father First Other Pacific Islander Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 120)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherFirstOtherPacificIslanderCodeHelper
        {
            get
            {
                if (FatherFirstOtherPacificIslanderCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherFirstOtherPacificIslanderCode["code"]))
                {
                    return FatherFirstOtherPacificIslanderCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherFirstOtherPacificIslanderCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Mother Second Other Pacific Islander Code.</summary>
        /// <value>Mother Second Other Pacific Islander Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "531");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Mariana Islander");</para>
        /// <para>ExampleDeathRecord.FatherSecondOtherPacificIslanderCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherSecondOtherPacificIslanderCode: {ExampleBirthRecord.FatherSecondOtherPacificIslanderCode['display']}");</para>
        /// </example>
        [Property("Mother Second Other Pacific Islander Code", Property.Types.Dictionary, "Coded Content", "Mother Second Other Pacific Islander Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 121)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherSecondOtherPacificIslanderCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SecondOtherPacificIslanderCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SecondOtherPacificIslanderCode, NvssRace.SecondOtherPacificIslanderCodeDisplay, "NFTH");
        }

        /// <summary>Mother Second Other Pacific Islander Code Helper</summary>
        /// <value>Mother Second Other Pacific Islander Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherSecondOtherPacificIslanderCodeHelper = VR.ValueSets.RaceCodeMariana_Islander;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Race: {ExampleBirthRecord.FatherSecondOtherPacificIslanderCodeHelper}");</para>
        /// </example>
        [Property("Father Second Other Pacific Islander Code Helper", Property.Types.String, "Coded Content", "Father Second Other Pacific Islander Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 121)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherSecondOtherPacificIslanderCodeHelper
        {
            get
            {
                if (FatherSecondOtherPacificIslanderCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherSecondOtherPacificIslanderCode["code"]))
                {
                    return FatherSecondOtherPacificIslanderCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherSecondOtherPacificIslanderCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father First Other Race Code.</summary>
        /// <value>Father First Other Race Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "531");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Mariana Islander");</para>
        /// <para>ExampleDeathRecord.FatherFirstOtherRaceCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherFirstOtherRaceCode: {ExampleBirthRecord.FatherFirstOtherRaceCode['display']}");</para>
        /// </example>
        [Property("Father First Other Race Code", Property.Types.Dictionary, "Coded Content", "Father First Other Race Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 122)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherFirstOtherRaceCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FirstOtherRaceCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.FirstOtherRaceCode, NvssRace.FirstOtherRaceCodeDisplay, "NFTH");
        }

        /// <summary>Father First Other Race Code Helper</summary>
        /// <value>Father First Other Race Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherFirstOtherRaceCodeHelper = VR.ValueSets.RaceCodeMariana_Islander;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Race: {ExampleBirthRecord.FatherFirstOtherRaceCodeHelper}");</para>
        /// </example>
        [Property("Father First Other Race Code Helper", Property.Types.String, "Coded Content", "Father First Other Race Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 122)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherFirstOtherRaceCodeHelper
        {
            get
            {
                if (FatherFirstOtherRaceCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherFirstOtherRaceCode["code"]))
                {
                    return FatherFirstOtherRaceCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherFirstOtherRaceCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father Second Other Race Code.</summary>
        /// <value>Father Second Other Race Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; race = new Dictionary&lt;string, string&gt;();</para>
        /// <para>race.Add("code", "531");</para>
        /// <para>race.Add("system", CodeSystems.RaceCode);</para>
        /// <para>race.Add("display", "Mariana Islander");</para>
        /// <para>ExampleDeathRecord.FatherSecondOtherRaceCode = race;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"FatherSecondOtherRaceCode: {ExampleBirthRecord.FatherSecondOtherRaceCode['display']}");</para>
        /// </example>
        [Property("Father Second Other Race Code", Property.Types.Dictionary, "Coded Content", "Father Second Other Race Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 123)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherSecondOtherRaceCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SecondOtherRaceCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssRace.SecondOtherRaceCode, NvssRace.SecondOtherRaceCodeDisplay, "NFTH");
        }

        /// <summary>Father Second Other Race Code Helper</summary>
        /// <value>Father Second Other Race Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherSecondOtherRaceCodeHelper = VR.ValueSets.RaceCodeMariana_Islander;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Race: {ExampleBirthRecord.FatherSecondOtherRaceCodeHelper}");</para>
        /// </example>
        [Property("Father Second Other Race Code Helper", Property.Types.String, "Coded Content", "Father Second Other Race Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 123)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherSecondOtherRaceCodeHelper
        {
            get
            {
                if (FatherSecondOtherRaceCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherSecondOtherRaceCode["code"]))
                {
                    return FatherSecondOtherRaceCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherSecondOtherRaceCode", value, VR.ValueSets.RaceCode.Codes);
                }
            }
        }

        /// <summary>Father Ethnicity Code For Literal.</summary>
        /// <value>Father Ethnicity Code For Literal. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.VRCLEthnicityOrigin);</para>
        /// <para>ethnicity.Add("display", "Chicano");</para>
        /// <para>ExampleDeathRecord.FatherEthnicityCodeForLiteral = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Ethnicity Code For Literal: {ExampleBirthRecord.FatherEthnicityCodeForLiteral['display']}");</para>
        /// </example>
        [Property("Father Ethnicity Code For Literal", Property.Types.Dictionary, "Coded Content", "Ethnicity Code For Literal.", true, VR.IGURL.CodedRaceAndEthnicity, false, 295)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherEthnicityCodeForLiteral
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssEthnicity.CodeForLiteral, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssEthnicity.CodeForLiteral, NvssEthnicity.CodeForLiteralDisplay, "NFTH");
        }

        /// <summary>Father Ethnicity Code For Literal Helper</summary>
        /// <value>Father Ethnicity Code For Literal Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherEthnicityCodeForLiteralHelper = VR.ValueSets.HispanicOrigin.Andalusian;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Ethnicity: {ExampleBirthRecord.FatherEthnicityCodeForLiteralHelper}");</para>
        /// </example>
        [Property("Father Ethnicity Code For Literal Helper", Property.Types.String, "Coded Content", "Father Ethnicity Code For Literal Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 295)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherEthnicityCodeForLiteralHelper
        {
            get
            {
                if (FatherEthnicityCodeForLiteral.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherEthnicityCodeForLiteral["code"]))
                {
                    return FatherEthnicityCodeForLiteral["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherEthnicityCodeForLiteral", value, VR.ValueSets.HispanicOrigin.Codes);
                }
            }
        }

        /// <summary>Father Ethnicity Edited Code.</summary>
        /// <value>Father Ethnicity Edited Code. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; ethnicity = new Dictionary&lt;string, string&gt;();</para>
        /// <para>ethnicity.Add("code", "214");</para>
        /// <para>ethnicity.Add("system", CodeSystems.VRCLHispanicOrigin);</para>
        /// <para>ethnicity.Add("display", "Chicano");</para>
        /// <para>ExampleDeathRecord.FatherEthnicityEditedCode = ethnicity;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father Ethnicity Edited Code: {ExampleBirthRecord.FatherEthnicityEditedCode['display']}");</para>
        /// </example>
        [Property("Father Ethnicity Edited Code", Property.Types.Dictionary, "Coded Content", "Father Ethnicity Edited Code.", true, VR.IGURL.CodedRaceAndEthnicity, false, 296)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicity')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> FatherEthnicityEditedCode
        {
            get => GetCodedRaceEthnicity(CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssEthnicity.EditedCode, "NFTH");
            set => SetCodedRaceEthnicity(value, CODED_RACE_ETHNICITY_PROFILE_FATHER, NvssEthnicity.EditedCode, NvssEthnicity.EditedCodeDisplay, "NFTH");
        }

        /// <summary>Father Ethnicity Edited Code Helper</summary>
        /// <value>Father Ethnicity Edited Code Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherEthnicityCodeHelper = VR.ValueSets.HispanicOrigin.Andalusian;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Ethnicity: {ExampleBirthRecord.FatherEthnicityEditedCodeHelper}");</para>
        /// </example>
        [Property("Father Ethnicity Edited CodeHelper", Property.Types.String, "Coded Content", "Father Hispanic Edited Code Helper.", false, VR.IGURL.CodedRaceAndEthnicity, false, 296)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='codedraceandethnicityFather')", "")]
        public string FatherEthnicityEditedCodeHelper
        {
            get
            {
                if (FatherEthnicityEditedCode.ContainsKey("code") && !String.IsNullOrWhiteSpace(FatherEthnicityEditedCode["code"]))
                {
                    return FatherEthnicityEditedCode["code"];
                }
                return null;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("FatherEthnicityEditedCode", value, VR.ValueSets.HispanicOrigin.Codes);
                }
            }
        }

        /// <summary>Date of last live birth day</summary>
        /// <value>the date of the Mother's last live birth day
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfLastLiveBirthDay = 4;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of Last Live Birth Month: {ExampleBirthRecord.DateOfLastLiveBirthDay}");</para>
        /// </example>
        [Property("DateOfLastLiveBirthDay", Property.Types.Int32, "Date of Last Live Birth", "Date of Mother's last live birth day.", false, IGURL.ObservationDateOfLastLiveBirth, false, 34)]
        [PropertyParam("DateOfLastLiveBirthDay", "The day of the last live birth.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68499-3')", "")]
        public int? DateOfLastLiveBirthDay
        {
            get
            {

                Observation obs = GetObservation("68499-3");
                if (obs != null)
                {
                    return GetDateFragmentOrPartialDate(obs.Value, VR.ExtensionURL.PartialDateTimeDayVR);
                }
                return null;
            }
            set
            {
                Observation obs = GetOrCreateObservation("68499-3", CodeSystems.LOINC, "Date of last live birth", BFDR.ProfileURL.ObservationDateOfLastLiveBirth, DATE_OF_LAST_LIVE_BIRTH, Mother.Id);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {   
                    obs.Value = new FhirDateTime();
                    obs.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetDay(value, obs.Value as FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Date of last live birth month</summary>
        /// <value>the date of the Mother's last live birth month
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfLastLiveBirthMonth = 4;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of Last Live Birth Month: {ExampleBirthRecord.DateOfLastLiveBirthMonth}");</para>
        /// </example>
        [Property("DateOfLastLiveBirthMonth", Property.Types.Int32, "Date of Last Live Birth", "Date of Mother's last live birth month.", false, IGURL.ObservationDateOfLastLiveBirth, false, 34)]
        [PropertyParam("dateOfLastBirthMonth", "The month of the last live birth.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68499-3')", "")]
        public int? DateOfLastLiveBirthMonth
        {
            get
            {
                Observation obs = GetObservation("68499-3");
                if (obs != null)
                {
                    return GetDateFragmentOrPartialDate(obs.Value, PartialDateMonthUrl);
                }
                return null;
            }
            set
            {
                Observation obs = GetOrCreateObservation("68499-3", CodeSystems.LOINC, "Date of last live birth", BFDR.ProfileURL.ObservationDateOfLastLiveBirth, DATE_OF_LAST_LIVE_BIRTH, Mother.Id);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {   
                    obs.Value = new FhirDateTime();
                    obs.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetMonth(value, obs.Value as FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Date of last live birth year</summary>
        /// <value>the date of the Mother's last live birth year
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfLastLiveBirthYear = 2022;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of Last Live Birth Year: {ExampleBirthRecord.DateOfLastLiveBirthYear}");</para>
        /// </example>
        [Property("DateOfLastLiveBirthYear", Property.Types.Int32, "Date of Last Live Birth", "Date of Mother's last live birth year.", false, IGURL.ObservationDateOfLastLiveBirth, false, 34)]
        [PropertyParam("dateOfLastBirthYear", "The month of the last live birth.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68499-3')", "")]
        public int? DateOfLastLiveBirthYear
        {
            get
            {
                Observation obs = GetObservation("68499-3");
                if (obs != null)
                {
                    return GetDateFragmentOrPartialDate(obs.Value, PartialDateYearUrl);
                }
                return null;
            }
            set
            {
                Observation obs = GetOrCreateObservation("68499-3", CodeSystems.LOINC, "Date of last live birth", BFDR.ProfileURL.ObservationDateOfLastLiveBirth, DATE_OF_LAST_LIVE_BIRTH, Mother.Id);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {   
                    obs.Value = new FhirDateTime();
                    obs.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetYear(value, obs.Value as FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Date of last live birth.</summary>
        /// <value>Date of last live birth</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfLastLiveBirth = "2020-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of Last Live Birth: {ExampleBirthRecord.DateOfLastLiveBirth}");</para>
        /// </example>
        [Property("Date Of Last Live Birth", Property.Types.String, "Date of Last Live Birth", "Date of mother's last live birth.", true, IGURL.ObservationDateOfLastLiveBirth, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68499-3')", "")]
        public string DateOfLastLiveBirth
        {

            get
            {
                Observation obs = GetObservation("68499-3");
                if (obs != null)
                {
                    return (obs.Value as Hl7.Fhir.Model.FhirDateTime)?.Value;
                }
                return null;
            }
            set
            {
                Observation obs = GetOrCreateObservation("68499-3", CodeSystems.LOINC, "Date of last live birth", BFDR.ProfileURL.ObservationDateOfLastLiveBirth, DATE_OF_LAST_LIVE_BIRTH, Mother.Id);
                obs.Value = ConvertToDateTime(value);
            }
        }
        
        /// <summary>Date Of Last Other Pregnancy Outcome Day</summary>
        /// <value>the date of the last other pregnancy outcome day
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfLastOtherPregnancyOutcomeDay = 4;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of Last Other Pregnancy Outcome: {ExampleBirthRecord.DateOfLastOtherPregnancyOutcomeDay}");</para>
        /// </example>
        [Property("DateOfLastOtherPregnancyOutcomeDay", Property.Types.Int32, "Date of Last Other Pregnancy Outcome", "Date of last other pregnancy outcome.", false, IGURL.ObservationDateOfLastOtherPregnancyOutcome, false, 34)]
        [PropertyParam("DateOfLastOtherPregnancyOutcomeDay", "The month of the last other pregnancy outcome.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68500-8')", "")]
        public int? DateOfLastOtherPregnancyOutcomeDay
        {
            get
            {

                Observation obs = GetObservation("68500-8");
                if (obs != null)
                {
                    return GetDateFragmentOrPartialDate(obs.Value, VR.ExtensionURL.PartialDateTimeDayVR);
                }
                return null;
            }
            set
            {
                Observation obs = GetOrCreateObservation("68500-8", CodeSystems.LOINC, "Date of last other pregnancy outcome", BFDR.ProfileURL.ObservationDateOfLastOtherPregnancyOutcome, DATE_OF_LAST_OTHER_PREGNANCY_OUTCOME, Mother.Id);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {   
                    obs.Value = new FhirDateTime();
                    obs.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetDay(value, obs.Value as FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Date Of Last Other Pregnancy Outcome Month</summary>
        /// <value>the date of the last other pregnancy outcome month
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfLastOtherPregnancyOutcomeMonth = 4;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of Last Other Pregnancy Outcome: {ExampleBirthRecord.DateOfLastOtherPregnancyOutcomeMonth}");</para>
        /// </example>
        [Property("DateOfLastOtherPregnancyOutcomeMonth", Property.Types.Int32, "Date of Last Other Pregnancy Outcome", "Date of last other pregnancy outcome.", false, IGURL.ObservationDateOfLastOtherPregnancyOutcome, false, 34)]
        [PropertyParam("dateOfLastOtherPregnancyOutcomeMonth", "The month of the last other pregnancy outcome.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68500-8')", "")]
        public int? DateOfLastOtherPregnancyOutcomeMonth
        {
            get
            {
                Observation obs = GetObservation("68500-8");
                if (obs != null)
                {
                    return GetDateFragmentOrPartialDate(obs.Value, VR.ExtensionURL.PartialDateTimeMonthVR);
                }
                return null;
            }
            set
            {
                Observation obs = GetOrCreateObservation("68500-8", CodeSystems.LOINC, "Date of last other pregnancy outcome", BFDR.ProfileURL.ObservationDateOfLastOtherPregnancyOutcome, DATE_OF_LAST_OTHER_PREGNANCY_OUTCOME, Mother.Id);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {
                    obs.Value = new FhirDateTime();
                    obs.Value.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetMonth(value, obs.Value as Hl7.Fhir.Model.FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Date Of Last Other Pregnancy Outcome year</summary>
        /// <value>the date of the Mother's last other pregnancy outcome
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfLastOtherPregnancyOutcomeYear = 2022;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of Last Other Pregnancy Outcome:: {ExampleBirthRecord.DateOfLastOtherPregnancyOutcomeYear}");</para>
        /// </example>
        [Property("DateOfLastOtherPregnancyOutcomeYear", Property.Types.Int32, "Date of Last Other Pregnancy Outcome", "Date of last other pregnancy outcome.", false, IGURL.ObservationDateOfLastOtherPregnancyOutcome, false, 34)]
        [PropertyParam("dateOfLastOtherPregnancyOutcomeYear", "The year of the last other pregnancy outcome.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68500-8')", "")]
        public int? DateOfLastOtherPregnancyOutcomeYear
        {
            get
            {
                Observation obs = GetObservation("68500-8");
                if (obs != null)
                {
                    return GetDateFragmentOrPartialDate(obs.Value, VR.ExtensionURL.PartialDateTimeYearVR);
                }
                return null;
            }
            set
            {
                Observation obs = GetOrCreateObservation("68500-8", CodeSystems.LOINC, "Date of last other pregnancy outcome", BFDR.ProfileURL.ObservationDateOfLastOtherPregnancyOutcome, DATE_OF_LAST_OTHER_PREGNANCY_OUTCOME, Mother.Id);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {
                    obs.Value = new FhirDateTime();
                    obs.Value.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetYear(value, obs.Value as Hl7.Fhir.Model.FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Date Of Last Other Pregnancy Outcome.</summary>
        /// <value>Date of Last Other Pregnancy Outcome</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.DateOfLastOtherPregnancyOutcome = "2020-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of Last Other Pregnancy Outcome: {ExampleBirthRecord.DateOfLastOtherPregnancyOutcome}");</para>
        /// </example>
        [Property("Date Of Last Other Pregnancy Outcome", Property.Types.String, "Date of Last Other Pregnancy Outcome", "Date of mother's last live birth.", true, IGURL.ObservationDateOfLastOtherPregnancyOutcome, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68500-8')", "")]
        public string DateOfLastOtherPregnancyOutcome
        {
            get
            {
                Observation obs = GetObservation("68500-8");
                if (obs != null)
                {
                    return (obs.Value as Hl7.Fhir.Model.FhirDateTime)?.Value;
                }
                return null;
            }
            set
            {
                Observation obs = GetOrCreateObservation("68500-8", CodeSystems.LOINC, "Date of last other pregnancy outcome", BFDR.ProfileURL.ObservationDateOfLastOtherPregnancyOutcome, DATE_OF_LAST_OTHER_PREGNANCY_OUTCOME, Mother.Id);
                obs.Value = ConvertToDateTime(value);
            }
        }

        /// <summary>NumberOfPrenatalVisits.</summary>
        /// <value>NumberOfPrenatalVisits</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.NumberOfPrenatalVisits = 4;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"NumberOfPrenatalVisits: {ExampleBirthRecord.NumberOfPrenatalVisits}");</para>
        /// </example>
        [Property("Number Of Prenatal Visits", Property.Types.Int32, "Number of Prenatal Visits", "Number of Prenatal Visits.", true, IGURL.ObservationNumberPrenatalVisits, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68493-6')", "")]
        public int? NumberOfPrenatalVisits
        {
            get => GetIntegerObservationValue("68493-6");
            set => SetIntegerObservationValue("68493-6", CodeSystems.LOINC, value, BFDR.ProfileURL.ObservationNumberPrenatalVisits, NUMBER_OF_PRENATAL_VISITS, Mother.Id);
        }

        /// <summary>NumberOfPrenatalVisitsEditFlag.</summary>
        /// <value>NumberOfPrenatalVisitsEditFlag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.NumberOfPrenatalVisitsEditFlag = 4;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"NumberOfPrenatalVisitsEditFlag: {ExampleBirthRecord.NumberOfPrenatalVisitsEditFlag}");</para>
        /// </example>
        [Property("Number Of Prenatal Visits Edit Flag", Property.Types.Dictionary, "Number of Prenatal Visits", "Number of Prenatal Visits Edit Flag.", true, IGURL.ObservationNumberPrenatalVisits, true, 14)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68493-6')", "")]
        public Dictionary<string, string> NumberOfPrenatalVisitsEditFlag
        {
            get
            {
                Observation obs = GetObservation("68493-6");
                Extension editFlag = obs?.Value?.Extension.FirstOrDefault(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                if (editFlag != null && editFlag.Value != null && editFlag.Value.GetType() == typeof(CodeableConcept))
                {
                    return CodeableConceptToDict((CodeableConcept)editFlag.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (IsDictEmptyOrDefault(value))
                {
                    return;
                }
                Observation obs = GetObservation("68493-6");
                if (obs == null)
                {
                    obs = GetOrCreateObservation("68493-6", CodeSystems.LOINC, "Number of prenatal visits", BFDR.ProfileURL.ObservationNumberPrenatalVisits, NUMBER_OF_PRENATAL_VISITS, Mother.Id);
                    obs.Value = new UnsignedInt();
                }
                obs.Value?.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                Extension editFlag = new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(value));
                obs.Value.Extension.Add(editFlag);
            }
        }

        /// <summary>
        /// NumberOfPrenatalVisitsEditFlag Helper
        /// </summary>
        [Property("NumberOfPrenatalVisitsEditFlagHelper", Property.Types.String, "Number of Prenatal Visits", "NumberOfPrenatalVisitsEditFlag.", false, IGURL.ObservationNumberPrenatalVisits, true, 2)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68493-6')", "")]
        public String NumberOfPrenatalVisitsEditFlagHelper
        {
            get
            {
                return NumberOfPrenatalVisitsEditFlag.ContainsKey("code") && !String.IsNullOrWhiteSpace(NumberOfPrenatalVisitsEditFlag["code"]) ? NumberOfPrenatalVisitsEditFlag["code"] : null;
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("NumberOfPrenatalVisitsEditFlag", value, BFDR.ValueSets.PregnancyReportEditFlags.Codes);
                }
            }
        }

        /// <summary>GestationalAgeAtDelivery.</summary>
        /// <value>GestationalAgeAtDelivery</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.GestationalAgeAtDelivery = 4;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"GestationalAgeAtDelivery: {ExampleBirthRecord.GestationalAgeAtDelivery}");</para>
        /// </example>
        [Property("GestationalAgeAtDelivery", Property.Types.Dictionary, "Gestational Age at Delivery", "Gestational Age at Delivery", true, IGURL.ObservationGestationalAgeAtDelivery, true, 14)]
        [PropertyParam("value", "The quantity value.")]
        [PropertyParam("code", "The unit type, from UnitsOfAge ValueSet.")]
        [PropertyParam("system", "OPTIONAL: The coding system.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11884-4')", "")]
        public Dictionary<string, string> GestationalAgeAtDelivery
        {
            get
            {
                Observation obs = GetObservation("11884-4");
                if (obs?.Value != null)
                {
                    Dictionary<string, string> age = new Dictionary<string, string>();
                    Quantity quantity = (Quantity)obs.Value;
                    age.Add("value", quantity.Value == null ? "" : Convert.ToString(quantity.Value));
                    age.Add("code", quantity.Code == null ? "" : quantity.Code);
                    age.Add("system", quantity.System == null ? "" : quantity.System);
                    return age;
                }
                return new Dictionary<string, string>() { { "value", "" }, { "code", "" }, { "system", null } };
            }
            set
            {
                string extractedValue = GetValue(value, "value");
                string extractedCode = GetValue(value, "code");
                string extractedSystem = GetValue(value, "system");
                // If string is empty don't bother to set the value
                if (String.IsNullOrEmpty(extractedCode))
                {
                    return;
                }
                if (String.IsNullOrEmpty(extractedSystem))
                {
                    string[,] options = BFDR.ValueSets.UnitsOfGestationalAge.Codes;
                    // Iterate over the allowed options and see if the code supplied is one of them
                    for (int i = 0; i < options.GetLength(0); i += 1)
                    {
                        if (options[i, 0] == extractedCode)
                        {
                            // Found it, so call the supplied setter with the appropriate dictionary built based on the code
                            // using the supplied options and return
                            extractedSystem = options[i, 2];
                        }
                    }
                }

                if (extractedValue == null && extractedCode == null && extractedSystem == null) // if there is nothing to do, do nothing.
                {
                    return;
                }
                Observation obs = GetObservation("11884-4");
                if (obs == null)
                {
                    obs = GetOrCreateObservation("11884-4", CodeSystems.LOINC, "Gestational age at delivery", BFDR.ProfileURL.ObservationGestationalAgeAtDelivery, GESTATIONAL_AGE, Mother.Id);
                    obs.Value = new Quantity();
                }
                Quantity quantity = (Quantity)obs.Value;

                if (extractedValue != null)
                {
                    quantity.Value = Convert.ToDecimal(extractedValue);
                }
                if (extractedCode != null)
                {
                    quantity.Code = extractedCode;
                }
                if (extractedSystem != null)
                {
                    quantity.System = extractedSystem;
                }
                obs.Value = (Quantity)quantity;
            }
        }

        /// <summary>GestationalAgeAtDeliveryEditFlag.</summary>
        /// <value>GestationalAgeAtDeliveryEditFlag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.GestationalAgeAtDeliveryEditFlag = 4;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"GestationalAgeAtDeliveryEditFlag: {ExampleBirthRecord.GestationalAgeAtDeliveryEditFlag}");</para>
        /// </example>
        [Property("GestationalAgeAtDelivery Edit Flag", Property.Types.Dictionary, "Gestational age at delivery", "GestationalAgeAtDeliveryEditFlag.", true, IGURL.ObservationGestationalAgeAtDelivery, true, 14)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11884-4')", "")]
        public Dictionary<string, string> GestationalAgeAtDeliveryEditFlag
        {
            get
            {
                Observation obs = GetObservation("11884-4");
                Extension editFlag = obs?.Value?.Extension.FirstOrDefault(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                if (editFlag != null && editFlag.Value != null && editFlag.Value.GetType() == typeof(CodeableConcept))
                {
                    return CodeableConceptToDict((CodeableConcept)editFlag.Value);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (IsDictEmptyOrDefault(value))
                {
                    return;
                }
                Observation obs = GetOrCreateObservation("11884-4", CodeSystems.LOINC, "Gestational age at delivery", BFDR.ProfileURL.ObservationGestationalAgeAtDelivery, GESTATIONAL_AGE, Mother.Id);
                if (obs.Value == null)
                {
                    obs.Value = new CodeableConcept();
                }
                obs.Value?.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                Extension editFlag = new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(value));
                obs.Value.Extension.Add(editFlag);
            }
        }

        /// <summary>
        /// GestationalAgeAtDeliveryEditFlag Helper
        /// </summary>
        [Property("GestationalAgeAtDeliveryEditFlagHelper", Property.Types.String, "Number of Prenatal Visits", "GestationalAgeAtDeliveryEditFlagHelper.", false, IGURL.ObservationGestationalAgeAtDelivery, true, 2)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11884-4')", "")]
        public String GestationalAgeAtDeliveryEditFlagHelper
        {
            get
            {
                return GestationalAgeAtDeliveryEditFlag.ContainsKey("code") && !String.IsNullOrWhiteSpace(GestationalAgeAtDeliveryEditFlag["code"]) ? GestationalAgeAtDeliveryEditFlag["code"] : null;
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("GestationalAgeAtDeliveryEditFlag", value, BFDR.ValueSets.EstimateOfGestationEditFlags.Codes);
                }
            }
        }

        /// <summary>NumberOfBirthsNowDead.</summary>
        /// <value>NumberOfBirthsNowDead</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.NumberOfBirthsNowDead = 1;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"NumberOfBirthsNowDead: {ExampleBirthRecord.NumberOfBirthsNowDead}");</para>
        /// </example>
        [Property("NumberOfBirthsNowDead", Property.Types.Int32, "Number Of Births Now Dead", "Number Of Births Now Dead", true, IGURL.ObservationNumberBirthsNowDead, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='68496-9')", "")]
        public int? NumberOfBirthsNowDead
        {
            get => GetIntegerObservationValue("68496-9");
            set => SetIntegerObservationValue("68496-9", CodeSystems.LOINC, value, BFDR.ProfileURL.ObservationNumberBirthsNowDead, NUMBER_OF_BIRTHS_NOW_DEAD, Mother.Id); 
        }

        /// <summary>NumberOfBirthsNowLiving.</summary>
        /// <value>NumberOfBirthsNowLiving</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.NumberOfBirthsNowLiving = 1;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"NumberOfBirthsNowLiving: {ExampleBirthRecord.NumberOfBirthsNowLiving}");</para>
        /// </example>
        [Property("NumberOfBirthsNowLiving", Property.Types.Int32, "Number Of Births Now Living", "Number Of Births Now Living", true, IGURL.ObservationNumberBirthsNowLiving, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='11638-4')", "")]
        public int? NumberOfBirthsNowLiving
        {
            get
            {
                Observation obs = GetObservation("11638-4");
                if (obs != null && obs.Value != null)
                {
                    return (obs.Value as Hl7.Fhir.Model.Integer).Value;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                Observation obs = GetOrCreateObservation("11638-4", CodeSystems.LOINC, "Number of births now living", BFDR.ProfileURL.ObservationNumberBirthsNowLiving, NUMBER_OF_BIRTHS_NOW_LIVING, Mother.Id);
                obs.Value = new Hl7.Fhir.Model.Integer(value);
            }
        }

        /// <summary>NumberOfOtherPregnancyOutcomes.</summary>
        /// <value>NumberOfOtherPregnancyOutcomes</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.NumberOfOtherPregnancyOutcomes = 1;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"NumberOfOtherPregnancyOutcomes: {ExampleBirthRecord.NumberOfOtherPregnancyOutcomes}");</para>
        /// </example>
        [Property("NumberOfOtherPregnancyOutcomes", Property.Types.Int32, "Number Of Other Pregnancy Outcomes", "Number Of Other Pregnancy Outcomes", true, IGURL.ObservationNumberOtherPregnancyOutcomes, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='69043-8')", "")]
        public int? NumberOfOtherPregnancyOutcomes
        {
            get => GetIntegerObservationValue("69043-8");
            set => SetIntegerObservationValue("69043-8", CodeSystems.LOINC, value, BFDR.ProfileURL.ObservationNumberOtherPregnancyOutcomes, NUMBER_OF_OTHER_PREGNANCY_OUTCOMES, Mother.Id);
        }

        /// <summary>MotherReceivedWICFood.</summary>
        /// <value>MotherReceivedWICFood</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherReceivedWICFood = 1;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherReceivedWICFood: {ExampleBirthRecord.MotherReceivedWICFood}");</para>
        /// </example>
        [Property("MotherReceivedWICFood", Property.Types.Dictionary, "Mother Received WIC Food", "Mother Received WIC Food", true, IGURL.ObservationMotherReceivedWICFood, true, 14)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87303-4')", "")]
        public Dictionary<string, string> MotherReceivedWICFood
        {
            get
            {
                Observation obs = GetObservation("87303-4");
                if (obs != null)
                {
                    if (obs != null && obs.Value != null && obs.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)obs.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                Observation obs = GetOrCreateObservation("87303-4", CodeSystems.LOINC, "Mother received WIC food", BFDR.ProfileURL.ObservationMotherReceivedWICFood, MOTHER_RECEIVED_WIC_FOOD, Mother.Id);
                obs.Value = DictToCodeableConcept(value);
            }
        }

        /// <summary>MotherReceivedWICFoodHelper.</summary>
        /// <value>MotherReceivedWICFoodHelper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherReceivedWICFoodHelper = 1;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"MotherReceivedWICFoodHelper: {ExampleBirthRecord.MotherReceivedWICFoodHelper}");</para>
        /// </example>
        [Property("MotherReceivedWICFoodHelper", Property.Types.String, "Mother Received WIC Food", "Mother Received WIC Food", true, IGURL.ObservationMotherReceivedWICFood, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87303-4')", "")]
        public string MotherReceivedWICFoodHelper
        {
            get
            {
                if (MotherReceivedWICFood.ContainsKey("code") && !String.IsNullOrWhiteSpace(MotherReceivedWICFood["code"]))
                {
                    return MotherReceivedWICFood["code"];
                }
                return null;
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetCodeValue("MotherReceivedWICFood", value, VR.ValueSets.YesNoUnknown.Codes);
                }
            }
        }

        /// <summary>InfantBreastfedAtDischarge.</summary>
        /// <value>InfantBreastfedAtDischarge</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.InfantBreastfedAtDischarge = true;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"InfantBreastfedAtDischarge: {ExampleBirthRecord.InfantBreastfedAtDischarge}");</para>
        /// </example>
        [Property("InfantBreastfedAtDischarge", Property.Types.Bool, "Infant Breastfed At Discharge", "Infant Breastfed At Discharge", true, IGURL.ObservationInfantBreastfedAtDischarge, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='73756-9')", "")]
        public bool? InfantBreastfedAtDischarge
        {
            get
            {
                Observation obs = GetObservation("73756-9");
                if (obs != null)
                {
                    if (obs != null && obs.Value != null && obs.Value as FhirBoolean != null)
                    {
                        return Convert.ToBoolean((obs.Value).ToString());
                    }
                }
                // blank or absent data 
                 return null;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                Observation obs = GetOrCreateObservation("73756-9", CodeSystems.LOINC, "Infant breastfed at discharge", BFDR.ProfileURL.ObservationInfantBreastfedAtDischarge, INFANT_BREASTFED_AT_DISCHARGE, Mother.Id);
                obs.Value = new FhirBoolean(value);
            }
        }

        private int? GetWeight(string code)
        {
            var entry = Bundle.Entry.Where(e => e.Resource is Observation obs && CodeableConceptToDict(obs.Code)["code"] == code).FirstOrDefault();
            if (entry != null)
            {
                Observation observation = (Observation)entry.Resource;
                return (int?)(observation?.Value as Hl7.Fhir.Model.Quantity)?.Value;
            }
            return null;
        }

        private Observation SetWeight(string code, int? value, string unit, string section, string subjectId)
        {
            var entry = Bundle.Entry.Where(e => e.Resource is Observation o && CodeableConceptToDict(o.Code)["code"] == code).FirstOrDefault();
            if (!(entry?.Resource is Observation obs))
            {
                obs = new Observation
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = new CodeableConcept(VR.CodeSystems.LOINC, code),
                    Subject = new ResourceReference($"urn:uuid:{subjectId}")
                };
                obs.Category.Add(new CodeableConcept(CodeSystems.ObservationCategory, "vital-signs"));
                AddReferenceToComposition(obs.Id, section);
                Bundle.AddResourceEntry(obs, "urn:uuid:" + obs.Id);
            }
            // Create an empty quantity if needed
            if (obs.Value == null || obs.Value as Quantity == null)
            {
                obs.Value = new Hl7.Fhir.Model.Quantity();
            }
            // Set the properties of the value individually to preserve any existing obs.Value.Extension entries
            if (value != null)
            {
                (obs.Value as Quantity).Value = (int)value;
                (obs.Value as Quantity).Unit = unit;
                (obs.Value as Quantity).Code = unit;
            }
            return obs;
        }

        /// <summary>Mother's Prepregnancy Weight.</summary>
        /// <value>the mother's prepregnancy weight in whole pounds, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherPrepregnancyWeight = 120;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Prepregancy Weight: {ExampleBirthRecord.MotherPrepregnancyWeight}");</para>
        /// </example>
        [Property("MotherPrepregnancyWeight", Property.Types.Int32, "Mother Prenatal", "Prepregnancy Weight.", false, BFDR.IGURL.ObservationMotherPrepregnancyWeight, true, 137)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='56077-1')", "")]
        public int? MotherPrepregnancyWeight
        {
            // TODO replace codes with constants once BFDR value sets are autogenerated
            get => GetWeight("56077-1");
            set => SetWeight("56077-1", value, "lb_av", MOTHER_PRENATAL_SECTION, Mother.Id);
        }

        /// <summary>Mother's Weight at Delivery.</summary>
        /// <value>the mother's weight at delivery in whole pounds, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherWeightAtDelivery = 120;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Weight at Delivery: {ExampleBirthRecord.MotherWeightAtDelivery}");</para>
        /// </example>
        [Property("MotherWeightAtDelivery", Property.Types.Int32, "Mother Prenatal", "Weight at Delivery.", false, BFDR.IGURL.ObservationMotherDeliveryWeight, true, 139)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='69461-2')", "")]
        public int? MotherWeightAtDelivery
        {
            // TODO replace codes with constants once BFDR value sets are autogenerated
            get => GetWeight("69461-2");
            set => SetWeight("69461-2", value, "lb_av", MOTHER_PRENATAL_SECTION, Mother.Id);
        }

        /// <summary>Birth Weight.</summary>
        /// <value>the birth weight in grams, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthWeight = 3200;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Birth Weight: {ExampleBirthRecord.BirthWeight}");</para>
        /// </example>
        [Property("BirthWeight", Property.Types.Int32, "Child Demographics", "Weight at Delivery.", false, BFDR.IGURL.ObservationBirthWeight, true, 201)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8339-4')", "")]
        public int? BirthWeight
        {
            // TODO replace codes with constants once BFDR value sets are autogenerated
            get => GetWeight("8339-4");
            set => SetWeight("8339-4", value, "g", NEWBORN_INFORMATION_SECTION, Child.Id);
        }

        private Dictionary<string, string> GetWeightEditFlag(string code)
        {
            var entry = Bundle.Entry.Where(e => e.Resource is Observation obs && CodeableConceptToDict(obs.Code)["code"] == code).FirstOrDefault();
            if (entry != null)
            {
                Observation observation = (Observation)entry.Resource;
                Extension extension = observation?.Value?.Extension.FirstOrDefault(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                if (extension != null && extension.Value != null && extension.Value.GetType() == typeof(CodeableConcept))
                {
                    return CodeableConceptToDict((CodeableConcept)extension.Value);
                }
            }
            return EmptyCodeableDict();
        }

        private string GetWeightEditFlagHelper(string code)
        {
            Dictionary<string, string> editFlag = GetWeightEditFlag(code);
            if (editFlag.ContainsKey("code"))
            {
                string flagCode = editFlag["code"];
                if (!String.IsNullOrWhiteSpace(flagCode))
                {
                    return flagCode;
                }
            }
            return null;
        }

        private void SetWeightEditFlag(string code, Dictionary<string, string> value, string section, string subjectId)
        {
            // TODO add validation of value once ValueSets.cs has been generated.
            var entry = Bundle.Entry.Where(e => e.Resource is Observation o && CodeableConceptToDict(o.Code)["code"] == code).FirstOrDefault();
            string editFlag = value.ContainsKey("code") ? (value["code"] ?? null) : null;
            if (String.IsNullOrWhiteSpace(editFlag))
            {
                if (!(entry?.Resource is Observation currObs))
                {
                    int? weight = GetWeight(code);
                    currObs = SetWeight(code, weight, "", section, subjectId);
                } 
                currObs.Value.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                return;
            }
            if (!(entry?.Resource is Observation obs))
            {
                obs = SetWeight(code, null, "", section, subjectId);
            }

            // If there's a value clear this extension in case it's previously set, otherwise set an empty value
            if (obs.Value == null)
            {
                obs.Value = new CodeableConcept();
            }
            else
            {
                obs.Value.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
            }
            Extension extension = new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(value));
            obs.Value.Extension.Add(extension);
        }

        private void SetWeightEditFlagHelper(string code, string editFlag, string section, string subjectId)
        {
            // TODO add validation of editFlag and automate code system extraction once ValueSets.cs is available
            if (String.IsNullOrEmpty(editFlag))
            {
                SetWeightEditFlag(code, EmptyCodeDict(), section, subjectId);
                return;
            }
            // Determine the subject
            string[,] options = null;
            if (code == "8339-4")
            {
                // child / decedent fetus
                options = BFDR.ValueSets.BirthWeightEditFlags.Codes;
            }
            else if (code == "56077-1" || code == "69461-2")
            {
                // mother
                options = BFDR.ValueSets.PregnancyReportEditFlags.Codes;
            } 
            else 
            {
                Console.WriteLine("Warning: provided LOINC code for 'weight' does not correspond to mother or child.");
            }
            // Iterate over the allowed options and see if the code supplies is one of them
            for (int i = 0; i < options.GetLength(0); i += 1)
            {
                if (options[i, 0] == editFlag)
                {
                    // Found it, so call the supplied setter with the appropriate dictionary built based on the code
                    // using the supplied options and return
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("code", editFlag);
                    dict.Add("display", options[i, 1]);
                    dict.Add("system", options[i, 2]);
                    SetWeightEditFlag(code, dict, section, subjectId);
                    return;
                }
            }
        }

        /// <summary>Mother's Prepregnancy Weight Edit Flag.</summary>
        /// <value>edit flag for the mother's prepregnancy weight</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; edit = new Dictionary&lt;string, string&gt;();</para>
        /// <para>edit.Add("code", "0");</para>
        /// <para>edit.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");</para>
        /// <para>edit.Add("display", "Edit Passed");</para>
        /// <para>ExampleBirthRecord.MotherPrepregnancyWeightEditFlag = route;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's prepregnancy weight edit flag: {ExampleBirthRecord.MotherPrepregnancyWeightEditFlag}");</para>
        /// </example>
        [Property("MotherPrepregnancyWeightEditFlag", Property.Types.Dictionary, "Mother Prenatal", "Mother Prenatal, Weight at Delivery Edit Flag", true, IGURL.ObservationMotherPrepregnancyWeight, true, 138)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='56077-1')", "")]
        public Dictionary<string, string> MotherPrepregnancyWeightEditFlag
        {
            get => GetWeightEditFlag("56077-1");
            set => SetWeightEditFlag("56077-1", value, MOTHER_PRENATAL_SECTION, Mother.Id);
        }

        /// <summary>Mother's Prepregnancy Weight Edit Flag Helper.</summary>
        /// <value>edit flag for the mother's prepregnancy weight</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherPrepregnancyWeightEditFlagHelper = "0";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's prepregnancy weight edit flag: {ExampleBirthRecord.MotherPrepregnancyWeightEditFlagHelper}");</para>
        /// </example>
        [Property("MotherWeightAtDeliveryEditFlagHelper", Property.Types.String, "Mother Prenatal", "Mother Prenatal, Weight at Delivery Edit Flag Helper", false, IGURL.ObservationMotherPrepregnancyWeight, true, 138)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='56077-1')", "")]
        public string MotherPrepregnancyWeightEditFlagHelper
        {
            get => GetWeightEditFlagHelper("56077-1");
            set => SetWeightEditFlagHelper("56077-1", value, MOTHER_PRENATAL_SECTION, Mother.Id);
        }

        /// <summary>Mother's Weight at Delivery Edit Flag.</summary>
        /// <value>edit flag for the mother's weight at delivery</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; edit = new Dictionary&lt;string, string&gt;();</para>
        /// <para>edit.Add("code", "0");</para>
        /// <para>edit.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");</para>
        /// <para>edit.Add("display", "Edit Passed");</para>
        /// <para>ExampleBirthRecord.MotherWeightAtDeliveryEditFlag = route;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's weight at delivery edit flag: {ExampleBirthRecord.MotherWeightAtDeliveryEditFlag}");</para>
        /// </example>
        [Property("MotherWeightAtDeliveryEditFlag", Property.Types.Dictionary, "Mother Prenatal", "Mother Prenatal, Weight at Delivery Edit Flag", true, IGURL.ObservationMotherDeliveryWeight, true, 140)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='69461-2')", "")]
        public Dictionary<string, string> MotherWeightAtDeliveryEditFlag
        {
            get => GetWeightEditFlag("69461-2");
            set => SetWeightEditFlag("69461-2", value, MOTHER_PRENATAL_SECTION, Mother.Id);
        }

        /// <summary>Mother's Weight at Delivery Edit Flag Helper.</summary>
        /// <value>edit flag for the mother's weight at delivery</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherWeightAtDeliveryEditFlag = "0";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's weight at delivery edit flag: {ExampleBirthRecord.MotherWeightAtDeliveryEditFlag}");</para>
        /// </example>
        [Property("MotherWeightAtDeliveryEditFlagHelper", Property.Types.String, "Mother Prenatal", "Mother Prenatal, Weight at Delivery Edit Flag Helper", false, IGURL.ObservationMotherDeliveryWeight, true, 140)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='69461-2')", "")]
        public string MotherWeightAtDeliveryEditFlagHelper
        {
            get => GetWeightEditFlagHelper("69461-2");
            set => SetWeightEditFlagHelper("69461-2", value, MOTHER_PRENATAL_SECTION, Mother.Id);
        }

        /// <summary>Birth Weight Edit Flag.</summary>
        /// <value>edit flag for birth weight</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; edit = new Dictionary&lt;string, string&gt;();</para>
        /// <para>edit.Add("code", "0");</para>
        /// <para>edit.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");</para>
        /// <para>edit.Add("display", "Edit Passed");</para>
        /// <para>ExampleBirthRecord.BirthWeightEditFlag = route;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Birth weight edit flag: {ExampleBirthRecord.BirthWeightEditFlag}");</para>
        /// </example>
        [Property("BirthWeightEditFlag", Property.Types.Dictionary, "Child Demographics", "Child Demographics, Birth Weight Edit Flag", true, IGURL.ObservationBirthWeight, true, 202)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8339-4')", "")]
        public Dictionary<string, string> BirthWeightEditFlag
        {
            get => GetWeightEditFlag("8339-4");
            set => SetWeightEditFlag("8339-4", value, NEWBORN_INFORMATION_SECTION, Child.Id);
        }

        /// <summary>Birth Weight at Delivery Edit Flag Helper.</summary>
        /// <value>edit flag for birth weight</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthWeightEditFlagHelper = "0";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Birth weight edit flag: {ExampleBirthRecord.BirthWeightEditFlagHelper}");</para>
        /// </example>
        [Property("BirthWeightEditFlagHelper", Property.Types.String, "Child Demographics", "Child Demographics, Birth Weight Edit Flag Helper", false, IGURL.ObservationBirthWeight, true, 202)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8339-4')", "")]
        public string BirthWeightEditFlagHelper
        {
            get => GetWeightEditFlagHelper("8339-4");
            set => SetWeightEditFlagHelper("8339-4", value, NEWBORN_INFORMATION_SECTION, Child.Id);
        }

        /// TODO: Required field in FHIR, needs BLANK placeholder
        /// <summary>Family name of attendant.</summary>
        /// <value>the attendant's family name (i.e. last name)</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.AttendantFamilyName = "Seito";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Attendants's Name: {ExampleBirthRecord.AttendantFamilyName}");</para>
        /// </example>
        [Property("Attendant Name", Property.Types.String, "Birth Certification", "Family name of attendant.", true, VR.IGURL.Practitioner, true, 6)]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner)", "name")]
        public string AttendantFamilyName
        {
            get
            {
                if (Attendant != null && Attendant.Name.Count() > 0)
                {
                    return Attendant.Name.First().Family;
                }
                return null;
            }
            set
            {
                if (Attendant == null)
                {
                    CreateAttendant();
                }
                HumanName name = Attendant.Name.FirstOrDefault();
                if (name != null && !String.IsNullOrEmpty(value))
                {
                    name.Family = value;
                }
                else if (!String.IsNullOrEmpty(value))
                {
                    name = new HumanName();
                    name.Use = HumanName.NameUse.Official;
                    name.Family = value;
                    Attendant.Name.Add(name);
                }
            }
        }

        /// <summary>Attendant name.</summary>
        /// <value>the attendant's name</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.AttendantName = "Janet Seito";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Attendants's Name: {ExampleBirthRecord.AttendantName}");</para>
        /// </example>
        [Property("Attendant Name", Property.Types.String, "Birth Certification", "Name of attendant.", true, VR.IGURL.Practitioner, true, 6)]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner).where(extension.value ='attendant')", "name")]
        public string AttendantName
        {
            get
            {
                if (Attendant != null && Attendant.Name != null)
                {
                    return Attendant.Name.FirstOrDefault()?.Text;
                }
                return null;
            }
            set
            {
                if (Attendant == null)
                {
                    CreateAttendant();
                }
                HumanName name = Attendant.Name.FirstOrDefault();
                if (name != null && !String.IsNullOrEmpty(value))
                {
                    name.Text = value;
                }
                else if (!String.IsNullOrEmpty(value))
                {
                    name = new HumanName();
                    name.Use = HumanName.NameUse.Official;
                    name.Text = value;
                    Attendant.Name.Add(name);
                }
            }
        }

        /// <summary>Attendants NPI</summary>
        /// <value>the attendants npi</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.AttendantNPI = "123456789011";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Attendants NPI: {ExampleBirthRecord.AttendantNPI}");</para>
        /// </example>
        [Property("Attendants NPI", Property.Types.String, "Birth Certification", "Attendant's NPI.", true, VR.IGURL.Practitioner, true, 13)]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner).where(extension.value ='attendant').identifier.where(system='http://hl7.org/fhir/sid/us-npi')", "value")]
        public string AttendantNPI
        {
            get
            {
                return Attendant?.Identifier?.Find(id => id.System == CodeSystems.US_NPI_HL7)?.Value;
            }
            set
            {
                if (Attendant == null)
                {
                    CreateAttendant();
                }
                if (Attendant.Identifier.Count > 0)
                {
                    Attendant.Identifier.Clear();
                }
                Attendant.Identifier.RemoveAll(iden => iden.System == CodeSystems.US_NPI_HL7);
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Identifier npi = new Identifier();
                npi.Type = new CodeableConcept(CodeSystems.HL7_identifier_type, "NPI", "National Provider Identifier", null);
                npi.System = CodeSystems.US_NPI_HL7;
                npi.Value = value;
                Attendant.Identifier.Add(npi);
            }
        }

        /// <summary>Attendant Title</summary>
        /// <value>the title/qualification of the person who attended the birth. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; title = new Dictionary&lt;string, string&gt;();</para>
        /// <para>title.Add("code", "112247003");</para>
        /// <para>title.Add("system", CodeSystems.SCT);</para>
        /// <para>title.Add("display", "Medical Doctor");</para>
        /// <para>ExampleBirthRecord.AttendantTitle = title;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Attendant Title: {ExampleBirthRecord.AttendantTitle['display']}");</para>
        /// </example>
        [Property("Attendants Title", Property.Types.Dictionary, "Birth Certification", "Attendant's Title.", true, VR.IGURL.Practitioner, true, 13)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner).where(extension.value ='attendant')", "qualification")]
        public Dictionary<string, string> AttendantTitle
        {
            get
            {
                if (Attendant == null)
                {
                    return EmptyCodeableDict();
                }
                Practitioner.QualificationComponent qualification = Attendant.Qualification.FirstOrDefault();
                if (Attendant != null && qualification != null)
                {
                    return CodeableConceptToDict(qualification.Code);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (Attendant == null)
                {
                    CreateAttendant();
                }
                Practitioner.QualificationComponent qualification = new Practitioner.QualificationComponent();
                qualification.Code = DictToCodeableConcept(value);
                Attendant.Qualification.Clear();
                Attendant.Qualification.Add(qualification);
            }
        }

        /// <summary>Attendant Title Helper.</summary>
        /// <value>the title/qualification of the attendant.
        /// <para>"code" - the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.AttendantTitleHelper = ValueSets.BirthAttendantsTitles.MedicalDoctor;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Attendant Title: {ExampleBirthRecord.AttendantTitleHelper}");</para>
        /// </example>
        [Property("Attendant Title Helper", Property.Types.String, "Birth Certification", "Attendant Title.", false, VR.IGURL.Practitioner, true, 4)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner).where(extension.value ='attendant')", "qualification")]
        public string AttendantTitleHelper
        {
            get
            {
                if (AttendantTitle.ContainsKey("code"))
                {
                    string code = AttendantTitle["code"];
                    if (!String.IsNullOrWhiteSpace(code))
                    {
                        return code;
                    }
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    // do nothing
                    return;
                }
                if (!VR.Mappings.BirthAttendantTitles.FHIRToIJE.ContainsKey(value))
                { //other
                    Console.WriteLine("Warning: given 'attendant title' code not found in value set. Setting value to 'Other'.");
                    AttendantTitle = CodeableConceptToDict(new CodeableConcept(CodeSystems.NullFlavor_HL7_V3, "OTH", "Other", value));
                }
                else
                { // normal path
                    SetCodeValue("AttendantTitle", value, VR.ValueSets.BirthAttendantTitles.Codes);
                }
            }
        }

        /// <summary>Attendant Other Helper.</summary>
        /// <value>the "other" title/qualification of the attendant.
        /// <para>"code" - the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.AttendantOtherHelper = "Birth Clerk";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Attendant Other: {ExampleBirthRecord.AttendantOtherHelper}");</para>
        /// </example>
        [Property("Attendant Other Helper", Property.Types.String, "Birth Certification", "Attendant Other.", false, VR.IGURL.Practitioner, true, 4)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner).where(extension.value ='attendant').qualification", "other")]
        public string AttendantOtherHelper
        {
            get
            {
                if (AttendantTitle.ContainsKey("code"))
                {
                    string code = AttendantTitle["code"];
                    if (code == "OTH")
                    {
                        if (AttendantTitle.ContainsKey("text") && !String.IsNullOrWhiteSpace(AttendantTitle["text"]))
                        {
                            return (AttendantTitle["text"]);
                        }
                    }
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    // do nothing
                    return;
                }
                else
                {
                    AttendantTitle = CodeableConceptToDict(new CodeableConcept(CodeSystems.NullFlavor_HL7_V3, "OTH", "Other", value));
                }
            }
        }

        private int? GetCigarettesSmoked(string code)
        {
            var entry = Bundle.Entry.Where(e => e.Resource is Observation obs && CodeableConceptToDict(obs.Code)["code"] == code).FirstOrDefault();
            if (entry != null)
            {
                Observation observation = (Observation)entry.Resource;
                return (observation.Value as Hl7.Fhir.Model.Integer)?.Value;
            }
            return null;
        }

        private void SetCigarettesSmoked(string code, int? value)
        {
            var entry = Bundle.Entry.Where(e => e.Resource is Observation obs && CodeableConceptToDict(obs.Code)["code"] == code).FirstOrDefault();
            if (entry == null)
            {
                Observation obs = new Observation();
                obs.Id = Guid.NewGuid().ToString();
                obs.Code = new CodeableConcept(VR.CodeSystems.LOINC, code);
                obs.Value = new Hl7.Fhir.Model.Integer(value);
                obs.Subject = new ResourceReference($"urn:uuid:{Mother.Id}");
                obs.Focus.Add(new ResourceReference($"urn:uuid:{Child.Id}"));
                AddReferenceToComposition(obs.Id, MOTHER_PRENATAL_SECTION);
                Bundle.AddResourceEntry(obs, "urn:uuid:" + obs.Id);
            }
            else
            {
                (entry.Resource as Observation).Value = new Hl7.Fhir.Model.Integer(value);
            }
        }

        /// <summary>Cigarettes Smoked in 3 months prior to Pregnancy.</summary>
        /// <value>the number of cigarettes smoked per day in 3 months prior to pregnancy, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CigarettesPerDayInThreeMonthsPriorToPregancy = 20;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cigarettes In Three Months Prior To Pregancy: {ExampleBirthRecord.CigarettesPerDayInThreeMonthsPriorToPregancy}");</para>
        /// </example>
        [Property("CigarettesPerDayInThreeMonthsPriorToPregancy", Property.Types.Int32, "Mother Prenatal", "Cigarettes Smoked In Three Months Prior To Pregancy.", false, BFDR.IGURL.ObservationCigaretteSmokingBeforeDuringPregnancy, true, 149)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='64794-1')", "")]
        public int? CigarettesPerDayInThreeMonthsPriorToPregancy
        {
            // TODO update with constants once BFDR value sets are autogenerated
            get => GetCigarettesSmoked("64794-1");
            set => SetCigarettesSmoked("64794-1", value);
        }

        /// <summary>Cigarettes Smoked in First Trimester.</summary>
        /// <value>the number of cigarettes smoked per day in first trimester, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CigarettesPerDayInFirstTrimester = 20;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cigarettes In First Trimester: {ExampleBirthRecord.CigarettesPerDayInFirstTrimester}");</para>
        /// </example>
        [Property("CigarettesPerDayInFirstTrimester", Property.Types.Int32, "Mother Prenatal", "Cigarettes Smoked In First Trimester.", true, BFDR.IGURL.ObservationCigaretteSmokingBeforeDuringPregnancy, true, 150)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87298-6')", "")]
        public int? CigarettesPerDayInFirstTrimester
        {
            // TODO update with constants once BFDR value sets are autogenerated
            get => GetCigarettesSmoked("87298-6");
            set => SetCigarettesSmoked("87298-6", value);
        }

        /// <summary>Cigarettes Smoked in Second Trimester.</summary>
        /// <value>the number of cigarettes smoked per day in second trimester, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CigarettesPerDayInSecondTrimester = 20;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cigarettes In Second Trimester: {ExampleBirthRecord.CigarettesPerDayInSecondTrimester}");</para>
        /// </example>
        [Property("CigarettesPerDayInSecondTrimester", Property.Types.Int32, "Mother Prenatal", "Cigarettes Smoked In Second Trimester.", true, BFDR.IGURL.ObservationCigaretteSmokingBeforeDuringPregnancy, true, 151)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87299-4')", "")]
        public int? CigarettesPerDayInSecondTrimester
        {
            // TODO update with constants once BFDR value sets are autogenerated
            get => GetCigarettesSmoked("87299-4");
            set => SetCigarettesSmoked("87299-4", value);
        }

        /// <summary>Cigarettes Smoked in Last Trimester.</summary>
        /// <value>the number of cigarettes smoked per day in last trimester, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CigarettesPerDayInLastTrimester = 20;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Cigarettes In Last Trimester: {ExampleBirthRecord.CigarettesPerDayInLastTrimester}");</para>
        /// </example>
        [Property("CigarettesPerDayInLastTrimester", Property.Types.Int32, "Mother Prenatal", "Cigarettes Smoked In Last Trimester.", true, BFDR.IGURL.ObservationCigaretteSmokingBeforeDuringPregnancy, true, 152)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='64795-8')", "")]
        public int? CigarettesPerDayInLastTrimester
        {
            // TODO update with constants once BFDR value sets are autogenerated
            get => GetCigarettesSmoked("64795-8");
            set => SetCigarettesSmoked("64795-8", value);
        }

        private Observation GetOccupationObservation(string role)
        {
            if (IsDictEmptyOrDefault(GetRoleCode(role)))
            {
                throw new System.ArgumentException($"Role '{role}' is not a member of the VR Role value set");
            }
            var entry = Bundle.Entry.Where(
                e => e.Resource is Observation obs &&
                CodeableConceptToDict(obs.Code)["code"] == "21843-8" &&
                obs.Extension.Find(
                    ext => ext.Url == VR.OtherExtensionURL.ParentRole &&
                    CodeableConceptToDict(ext.Value as CodeableConcept)["code"] == role
                ) != null).FirstOrDefault();

            if (entry != null)
            {
                return entry.Resource as Observation;
            }
            return null;
        }

        private string GetOccupation(string role)
        {
            Observation obs = GetOccupationObservation(role);
            if (obs != null)
            {
                return (obs.Value as CodeableConcept)?.Text;
            }
            return null;
        }

        private string GetIndustry(string role)
        {
            Observation obs = GetOccupationObservation(role);
            if (obs != null)
            {
                var comp = obs.Component.Where(c => CodeableConceptToDict(c.Code)["code"] == "21844-6").FirstOrDefault();
                if (comp != null)
                {
                    return (comp.Value as CodeableConcept)?.Text;
                }
            }
            return null;
        }

        private Observation SetOccupation(string role, string value)
        {
            Observation obs = GetOccupationObservation(role);
            if (obs == null)
            {
                obs = new Observation
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = new CodeableConcept(VR.CodeSystems.LOINC, "21843-8"),
                };
                Extension roleExt = new Extension(VR.OtherExtensionURL.ParentRole, new CodeableConcept(VR.CodeSystems.RoleCode_HL7_V3, role));
                obs.Extension.Add(roleExt);
                if (role == "MTH")
                {
                    obs.Subject = new ResourceReference($"urn:uuid:{Mother.Id}");
                    AddReferenceToComposition(obs.Id, MOTHER_INFORMATION_SECTION);
                }
                else if (role == "FTH")
                {
                    obs.Subject = new ResourceReference($"urn:uuid:{Father.Id}");
                    AddReferenceToComposition(obs.Id, FATHER_INFORMATION_SECTION);
                }
                obs.Focus.Add(new ResourceReference($"urn:uuid:{Child.Id}"));
                Bundle.AddResourceEntry(obs, "urn:uuid:" + obs.Id);
            }
            if (!String.IsNullOrWhiteSpace(value))
            {
                obs.Value = new CodeableConcept
                {
                    Text = value
                };
            }
            return obs;
        }

        private void SetIndustry(string role, string value)
        {   
            if (String.IsNullOrWhiteSpace(value)) 
            {
                return;
            }
            Observation obs = GetOccupationObservation(role) ?? SetOccupation(role, null);
            var comp = obs.Component.Where(c => CodeableConceptToDict(c.Code)["code"] == "21844-6").FirstOrDefault();
            if (comp == null)
            {
                comp = new Observation.ComponentComponent
                {
                    Code = new CodeableConcept(CodeSystems.LOINC, "21844-6")
                };
                obs.Component.Add(comp);
            }
            CodeableConcept cc = new CodeableConcept
            {
                Text = value
            };
            comp.Value = cc;
        }

        /// <summary>Occupation of Mother.</summary>
        /// <value>the occupation of the mother as text</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherOccupation = "scientist";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Occupation: {ExampleBirthRecord.MotherOccupation}");</para>
        /// </example>
        [Property("MotherOccupation", Property.Types.String, "Mother Information", "Occupation", false, VR.OtherIGURL.UsualWork, true, 282)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='21843-8')", "")]
        public string MotherOccupation
        {
            get => GetOccupation("MTH");
            set => SetOccupation("MTH", value);
        }

        /// <summary>Occupation of Father.</summary>
        /// <value>the occupation of the father as text</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherOccupation = "scientist";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Occupation: {ExampleBirthRecord.FatherOccupation}");</para>
        /// </example>
        [Property("FatherOccupation", Property.Types.String, "Father Information", "Occupation", false, VR.OtherIGURL.UsualWork, true, 284)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='21843-8')", "")]
        public string FatherOccupation
        {
            get => GetOccupation("FTH");
            set => SetOccupation("FTH", value);
        }

        /// <summary>Industry of Mother.</summary>
        /// <value>the industry of the mother as text</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherIndustry = "public health";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Industry: {ExampleBirthRecord.MotherIndustry}");</para>
        /// </example>
        [Property("MotherIndustry", Property.Types.String, "Mother Information", "Industry", false, VR.OtherIGURL.UsualWork, true, 286)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='21843-8')", "")]
        public string MotherIndustry
        {
            get => GetIndustry("MTH");
            set => SetIndustry("MTH", value);
        }

        /// <summary>Industry of Father.</summary>
        /// <value>the industry of the father as text</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherIndustry = "public health";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Industry: {ExampleBirthRecord.FatherIndustry}");</para>
        /// </example>
        [Property("FatherIndustry", Property.Types.String, "Father Information", "Industry", false, VR.OtherIGURL.UsualWork, true, 288)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='21843-8')", "")]
        public string FatherIndustry
        {
            get => GetIndustry("FTH");
            set => SetIndustry("FTH", value);
        }

        /// <summary>Mother Height.</summary>
        /// <value>the height of the mother, given in inches</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherHeight = 55;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Height: {ExampleBirthRecord.MotherHeight}");</para>
        /// </example>
        [Property("MotherHeight", Property.Types.Int32, "Mother Prenatal", "Mother's Height.", false, BFDR.IGURL.ObservationMotherHeight, true, 134)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8302-2')", "")]
        public int? MotherHeight
        {
            get
            {
              Observation observation = GetObservation("8302-2");
              return (int?)(observation?.Value as Hl7.Fhir.Model.Quantity)?.Value;
            }

            set
            {
              Observation obs = GetOrCreateObservation("8302-2", CodeSystems.LOINC, "Mother height", BFDR.ProfileURL.ObservationMotherHeight, MOTHER_PRENATAL_SECTION, Mother.Id);
              obs.Category.Add(new CodeableConcept(CodeSystems.ObservationCategory, "vital-signs"));
              string unit = "in_i";
              // Create an empty quantity if needed
              if (obs.Value == null || obs.Value as Quantity == null)
              {
                obs.Value = new Hl7.Fhir.Model.Quantity();
              }
              // Set the properties of the value individually to preserve any existing obs.Value.Extension entries
              if (value != null)
              {
                (obs.Value as Quantity).Value = (int)value;
                (obs.Value as Quantity).Unit = unit;
                (obs.Value as Quantity).Code = unit;
              }
            }
        }

        /// <summary>Mother Height Edit Flag.</summary>
        /// <value>the mother's height edit flag. A Dictionary representing a code, containing the following key/value pairs:        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; height = new Dictionary&lt;string, string&gt;();</para>
        /// <para>height.Add("code", "0");</para>
        /// <para>height.Add("system", CodeSystems.BypassEditFlag);</para>
        /// <para>height.Add("display", "Edit Passed");</para>
        /// <para>ExampleBirthRecord.MotherHeightEditFlag = height;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother Height: {ExampleBirthRecord.MotherHeightEditFlag['display']}");</para>
        /// </example>
        [Property("Mother Height Edit Flag", Property.Types.Dictionary, "Mother Prenatal", "Mother's Height Edit Flag.", true, IGURL.ObservationMotherHeight, true, 136)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8302-2')", "")]
        public Dictionary<string, string> MotherHeightEditFlag
        {
            get
            {
                Observation observation = GetObservation("8302-2");
                Extension extension = observation?.Value?.Extension.FirstOrDefault(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                if (extension != null && extension.Value != null && extension.Value.GetType() == typeof(CodeableConcept))
                {
                    return CodeableConceptToDict((CodeableConcept)extension.Value);
                }
                return EmptyCodeableDict();
            }

            set
            {
                Observation obs = GetOrCreateObservation("8302-2", CodeSystems.LOINC, "Mother Height", BFDR.ProfileURL.ObservationMotherHeight, MOTHER_PRENATAL_SECTION, Mother.Id);
                // Create an empty quantity if needed
                if (obs.Value == null || obs.Value as Quantity == null)
                {
                    obs.Value = new Hl7.Fhir.Model.Quantity();
                }
                obs.Value.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);
                obs.Value.Extension.Add(new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(value)));
            }
        }

        /// <summary>Mother Height Edit Flag Helper</summary>
        /// <value>Mother Height Edit Flag.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherHeightEditFlagHelper = "0";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Height Edit Flag: {ExampleBirthRecord.MotherHeightHelperEditFlag}");</para>
        /// </example>
        [Property("Mother's Height Edit Flag Helper", Property.Types.String, "Mother Prenatal", "Mother's Height Edit Flag Helper.", false, IGURL.ObservationMotherHeight, true, 136)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8302-2')", "")]
        public string MotherHeightEditFlagHelper
        {
            get 
            {
              Dictionary<string, string> editFlag = this.MotherHeightEditFlag;
              if (editFlag.ContainsKey("code"))
              {
                  string flagCode = editFlag["code"];
                  if (!String.IsNullOrWhiteSpace(flagCode))
                  {
                      return flagCode;
                  }
              }
              return null;
            }
            set 
            {
                Observation obs = GetOrCreateObservation("8302-2", CodeSystems.LOINC, "Mother Height", BFDR.ProfileURL.ObservationMotherHeight, MOTHER_PRENATAL_SECTION, Mother.Id);
                if (obs.Value == null)
                {
                    obs.Value = new FhirString();
                }
                obs.Value.Extension.RemoveAll(ext => ext.Url == VRExtensionURLs.BypassEditFlag);

                if (String.IsNullOrEmpty(value))
                {
                    obs.Value.Extension.Add(new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(EmptyCodeDict())));
                    return;
                }

                // Iterate over the allowed options and see if the code supplies is one of them
                string[,] options = BFDR.ValueSets.PregnancyReportEditFlags.Codes;
                for (int i = 0; i < options.GetLength(0); i += 1)
                {
                    if (options[i, 0] == value)
                    {
                        // Found it, so call the supplied setter with the appropriate dictionary built based on the code
                        // using the supplied options and return
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("code", value);
                        dict.Add("display", options[i, 1]);
                        dict.Add("system", options[i, 2]);
                        obs.Value.Extension.Add(new Extension(VRExtensionURLs.BypassEditFlag, DictToCodeableConcept(dict)));
                    }
                }
            }
        }
        /// <summary>Mother's Education Level.</summary>
        /// <value>the mother's education level. A Dictionary representing a code, containing the following key/value pairs:</value>
        /// <example>
        /// <para>Dictionary&lt;string, string&gt; elevel = new Dictionary&lt;string, string&gt;();</para>
        /// <para>elevel.Add("code", "BA");</para>
        /// <para>elevel.Add("system", VR.CodeSystems.EducationLevel);</para>
        /// <para>elevel.Add("display", "Bachelor’s Degree");</para>
        /// <para>ExampleBirthRecord.MotherEducationLevel = elevel;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Education Level: {ExampleBirthRecord.MotherEducationLevel["display"]}");</para>
        /// </example>
        [Property("Mother's Education Level", Property.Types.Dictionary, "Education Profiles", "Mother's Education Level.", true, VR.IGURL.EducationLevel, false, 32)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='57712-2').value.coding", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public Dictionary<string, string> MotherEducationLevel
        {
            get => GetObservationValue("57712-2");
            set => SetObservationValue(value, "57712-2", CodeSystems.LOINC, "Highest level of education Mother", VR.ProfileURL.EducationLevel, MOTHER_INFORMATION_SECTION);
        }

        /// <summary>Mother's Education Level Helper</summary>
        /// <value>Mother's Education Level.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherEducationLevelHelper = VR.ValueSets.EducationLevel.Bachelors_Degree;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Education Level: {ExampleBirthRecord.EducationLevelHelper}");</para>
        /// </example>
        [Property("Mother's Education Level Helper", Property.Types.String, "Education Profiles", "Mother's Education Level.", false, VR.IGURL.EducationLevel, false, 32)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='57712-2').value.coding", "")]
        public string MotherEducationLevelHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, VR.ValueSets.EducationLevel.Codes);
        }

        /// <summary>Mother's Education Level Edit Flag.</summary>
        /// <value>the mother's education level edit flag. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; elevel = new Dictionary&lt;string, string&gt;();</para>
        /// <para>elevel.Add("code", "0");</para>
        /// <para>elevel.Add("system", VR.CodeSystems.BypassEditFlag);</para>
        /// <para>elevel.Add("display", "Edit Passed");</para>
        /// <para>ExampleBirthRecord.MotherEducationLevelEditFlag = elevel;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Education Level Edit Flag: {ExampleBirthRecord.EducationLevelEditFlag["display"]}");</para>
        /// </example>
        [Property("Mother's Education Level Edit Flag", Property.Types.Dictionary, "Education Profiles", "Mother's Education Level Edit Flag.", true, VR.IGURL.EducationLevel, false, 33)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='57712-2')", "")]
        public Dictionary<string, string> MotherEducationLevelEditFlag
        {
            get => GetObservationValue("57712-2", VRExtensionURLs.BypassEditFlag);
            set => SetObservationValue(value, "57712-2", CodeSystems.LOINC, "Highest level of education Mother", VR.ProfileURL.EducationLevel, MOTHER_INFORMATION_SECTION, "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag");
        }

        /// <summary>Mother's Education Level Edit Flag Helper</summary>
        /// <value>Mother's Education Level Edit Flag.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherEducationLevelEditFlag = VRDR.ValueSets.EditBypass01234.EditPassed;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Education Level Edit Flag: {ExampleBirthRecord.EducationLevelHelperEditFlag}");</para>
        /// </example>
        [Property("Education Level Edit Flag Helper", Property.Types.String, "Decedent Demographics", "Mother's Education Level Edit Flag Helper.", false, VR.IGURL.EducationLevel, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='57712-2')", "")]
        public string MotherEducationLevelEditFlagHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, VR.ValueSets.EditBypass01234.Codes);
        }

        /// <summary>Father's Education Level.</summary>
        /// <value>the father's education level. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; elevel = new Dictionary&lt;string, string&gt;();</para>
        /// <para>elevel.Add("code", "BA");</para>
        /// <para>elevel.Add("system", VR.CodeSystems.EducationLevel);</para>
        /// <para>elevel.Add("display", "Bachelor’s Degree");</para>
        /// <para>ExampleBirthRecord.FatherEducationLevel = elevel;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Education Level: {ExampleBirthRecord.FatherEducationLevel["display"]}");</para>
        /// </example>
        [Property("Father's Education Level", Property.Types.Dictionary, "Education Profiles", "Father's Education Level.", true, VR.IGURL.EducationLevel, false, 78)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87300-0').value.coding", "")]
        public Dictionary<string, string> FatherEducationLevel
        {
            get => GetObservationValue("87300-0");
            set => SetObservationValue(value, "87300-0", CodeSystems.LOINC, "Highest level of education Father", VR.ProfileURL.EducationLevel, FATHER_INFORMATION_SECTION);
        }

        /// <summary>Father's Education Level Helper</summary>
        /// <value>Father's Education Level.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherEducationLevelHelper = VR.ValueSets.EducationLevel.Bachelors_Degree;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Education Level: {ExampleBirthRecord.EducationLevelHelper}");</para>
        /// </example>
        [Property("Father's Education Level Helper", Property.Types.String, "Education Profiles", "Father's Education Level.", false, VR.IGURL.EducationLevel, false, 32)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87300-0').value.coding", "")]
        public string FatherEducationLevelHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, VR.ValueSets.EducationLevel.Codes);
        }

        /// <summary>Father's Education Level Edit Flag.</summary>
        /// <value>the father's education level edit flag. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; elevel = new Dictionary&lt;string, string&gt;();</para>
        /// <para>elevel.Add("code", "0");</para>
        /// <para>elevel.Add("system", VR.CodeSystems.BypassEditFlag);</para>
        /// <para>elevel.Add("display", "Edit Passed");</para>
        /// <para>ExampleBirthRecord.FatherEducationLevelEditFlag = elevel;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Education Level Edit Flag: {ExampleBirthRecord.EducationLevelEditFlag["display"]}");</para>
        /// </example>
        [Property("Father's Education Level Edit Flag", Property.Types.Dictionary, "Education Profiles", "Father's Education Level Edit Flag.", true, VR.IGURL.EducationLevel, false, 33)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87300-0')", "")]
        public Dictionary<string, string> FatherEducationLevelEditFlag
        {
            get => GetObservationValue("87300-0", VRExtensionURLs.BypassEditFlag);
            set => SetObservationValue(value, "87300-0", CodeSystems.LOINC, "Highest level of education Father", VR.ProfileURL.EducationLevel, FATHER_INFORMATION_SECTION, "http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag");
        }

        /// <summary>Father's Education Level Edit Flag Helper</summary>
        /// <value>Father's Education Level Edit Flag.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FatherEducationLevelEditFlag = VRDR.ValueSets.EditBypass01234.EditPassed;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Father's Education Level Edit Flag: {ExampleBirthRecord.EducationLevelHelperEditFlag}");</para>
        /// </example>
        [Property("Education Level Edit Flag Helper", Property.Types.String, "Decedent Demographics", "Father's Education Level Edit Flag Helper.", false, VR.IGURL.EducationLevel, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87300-0')", "")]
        public string FatherEducationLevelEditFlagHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, VR.ValueSets.EditBypass01234.Codes);
        }

        /// <summary>Facility ID (NPI), National Provider Identifier</summary>
        /// <value>Facility ID (NPI).</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FacilityNPI = 123456789;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Facility National Provider Identifier: {ExampleBirthRecord.FacilityNPI}");</para>
        /// </example>
        [Property("Facility ID (NPI)", Property.Types.String, "Birth Location", "Facility ID (NPI), National Provider Identifier", true, IGURL.LocationBFDR, true, 34)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87300-0')", "")]
        public string FacilityNPI
        {
            get => GetFacilityLocation(ValueSets.LocationTypes.Birth_Location)?.Identifier?.Find(identifier => identifier.System == VR.CodeSystems.US_NPI_HL7)?.Value?.ToString();
            set
            {
                Location LocationBirth = GetFacilityLocation(ValueSets.LocationTypes.Birth_Location) ?? CreateAndSetLocationBirth(ValueSets.LocationTypes.Birth_Location);
                if (LocationBirth.Identifier == null)
                {
                    LocationBirth.Identifier = new List<Identifier>();
                }
                // Check for an existing Facility NPI and if it exists, overwrite it.
                if (LocationBirth.Identifier.Any(id => id.System == VR.CodeSystems.US_NPI_HL7))
                {
                    Identifier npiIdentifier = LocationBirth.Identifier.Find(identifier => identifier.System == VR.CodeSystems.US_NPI_HL7);
                    npiIdentifier.Value = value;
                    return;
                }
                LocationBirth.Identifier.Add(new Identifier(VR.CodeSystems.US_NPI_HL7, value));
                return;
            }
        }

        /// <summary>Facility ID (JFI), Jurisdictional Facility Identifier</summary>
        /// <value>Facility ID (JFI).</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FacilityJFI = 123456789;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Jurisdictional Facility Identifier: {ExampleBirthRecord.FacilityJFI}");</para>
        /// </example>
        [Property("Facility ID (JFI)", Property.Types.String, "Birth Location", "Facility ID (JFI), Jurisdictional Facility Identifier", true, IGURL.LocationBFDR, true, 34)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87300-0')", "")]
        public string FacilityJFI
        {
            get => GetFacilityLocation(ValueSets.LocationTypes.Birth_Location)?.Identifier?.Find(identifier => identifier.Extension.Any(ext => ext.Url == BFDR.ExtensionURL.JurisdictionalFacilityIdentifier))?.GetExtension(BFDR.ExtensionURL.JurisdictionalFacilityIdentifier).Value.ToString();
            set
            {
                Location LocationBirth = GetFacilityLocation(ValueSets.LocationTypes.Birth_Location) ?? CreateAndSetLocationBirth(ValueSets.LocationTypes.Birth_Location);
                if (LocationBirth.Identifier == null)
                {
                    LocationBirth.Identifier = new List<Identifier>();
                }
                // Check for an existing Facility JFI and if it exists, overwrite it.
                if (LocationBirth.Identifier.Any(id => id.Extension.Any(ext => ext.Url == BFDR.ExtensionURL.JurisdictionalFacilityIdentifier)))
                {
                    Identifier jfiIdentifier = LocationBirth.Identifier.Find(id => id.Extension.Any(ext => ext.Url == BFDR.ExtensionURL.JurisdictionalFacilityIdentifier));
                    jfiIdentifier.SetExtension(BFDR.ExtensionURL.JurisdictionalFacilityIdentifier, new FhirString(value));
                    return;
                }
                if (LocationBirth.Identifier.Count() < 1)
                {
                    Identifier id = new Identifier();
                    id.SetExtension(BFDR.ExtensionURL.JurisdictionalFacilityIdentifier, new FhirString(value));
                    LocationBirth.Identifier.Add(id);
                    return;
                }
                LocationBirth.Identifier.First().SetExtension(BFDR.ExtensionURL.JurisdictionalFacilityIdentifier, new FhirString(value));
            }
        }

        /// <summary>Name of Facility of Birth</summary>
        /// <value>BirthFacilityName.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthFacilityName = "South Hospital";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Birth Facility Name: {ExampleBirthRecord.BirthFacilityName}");</para>
        /// </example>
        [Property("Birth Facility Name", Property.Types.String, "Birth Location", "Birth Facility Name", true, IGURL.LocationBFDR, true, 34)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87300-0')", "")]
        public string BirthFacilityName
        {
            get => GetFacilityLocation(ValueSets.LocationTypes.Birth_Location)?.Name;
            set => (GetFacilityLocation(ValueSets.LocationTypes.Birth_Location) ?? CreateAndSetLocationBirth(ValueSets.LocationTypes.Birth_Location)).Name = value;
        }

        /// <summary>Name of Facility mother moved from (if transfered)</summary>
        /// <value>FacilityMotherTransferredFrom.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FacilityMotherTransferredFrom = "South Hospital";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Facility Mother Transferred From: {ExampleBirthRecord.FacilityMotherTransferredFrom}");</para>
        /// </example>
        [Property("Facility Mother Transferred From", Property.Types.String, "Birth Location", "Facility Mother Moved From (if transferred)", true, IGURL.LocationBFDR, true, 34)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87300-0')", "")]
        public string FacilityMotherTransferredFrom
        {
            get => GetFacilityLocation(ValueSets.LocationTypes.Transfer_From_Location)?.Name;
            set => (GetFacilityLocation(ValueSets.LocationTypes.Transfer_From_Location) ?? CreateAndSetLocationBirth(ValueSets.LocationTypes.Transfer_From_Location)).Name = value;
        }

        /// <summary>Name of Facility Infant Transferred To (if transfered w/in 24 hours)</summary>
        /// <value>FacilityInfantTransferredTo.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FacilityInfantTransferredTo = "South Hospital";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Facility Mother Transferred From: {ExampleBirthRecord.FacilityInfantTransferredTo}");</para>
        /// </example>
        [Property("Facility Infant Transferred To", Property.Types.String, "Birth Location", "Facility Infant Transferred To (if transferred w/in 24 hours)", true, IGURL.LocationBFDR, true, 34)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87300-0')", "")]
        public string FacilityInfantTransferredTo
        {
            get => GetFacilityLocation(ValueSets.LocationTypes.Transfer_To_Location)?.Name;
            set => (GetFacilityLocation(ValueSets.LocationTypes.Transfer_To_Location) ?? CreateAndSetLocationBirth(ValueSets.LocationTypes.Transfer_To_Location)).Name = value;
        }

        private Location GetFacilityLocation(string code) {
            return (Location)Bundle.Entry.Where(e => e.Resource is Location loc && loc.Type.Any(type => type.Coding.Any(coding => coding.System == CodeSystems.LocalBFDRCodes && coding.Code == code))).FirstOrDefault()?.Resource;
        }

        /// <summary>Last Menstrual Period.</summary>
        /// <value>the date that the last normal menstrual period began</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.LastMenstrualPeriod = "2023-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Last Menstrual Period: {ExampleBirthRecord.LastMenstrualPeriod}");</para>
        /// </example>
        [Property("LastMenstrualPeriod", Property.Types.String, "Mother Prenatal", "Last Menstrual Period.", true, BFDR.IGURL.ObservationLastMenstrualPeriod, true, 154)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8665-2')", "")]
        public string LastMenstrualPeriod
        {
            get
            {
                Observation obs = GetObservation("8665-2");
                if (obs != null)
                {
                    return (obs.Value as Hl7.Fhir.Model.FhirDateTime)?.Value;
                }
                return null;
            }
            set
            {
                Observation obs = GetOrCreateObservation("8665-2", CodeSystems.LOINC, "Mother Prenatal", BFDR.ProfileURL.ObservationLastMenstrualPeriod, MOTHER_PRENATAL_SECTION);
                obs.Value = ConvertToDateTime(value);
            }
        }

        /// <summary>Year of Last Menstrual Period.</summary>
        /// <value>the year that the last normal menstrual period began</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.LastMenstrualPeriodYear = 2023;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Year of Last Menstrual Period: {ExampleBirthRecord.LastMenstrualPeriodYear}");</para>
        /// </example>
        [Property("LastMenstrualPeriodYear", Property.Types.Int32, "Mother Prenatal", "Year of Last Menstrual Period.", true, BFDR.IGURL.ObservationLastMenstrualPeriod, true, 154)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8665-2')", "")]
        public int? LastMenstrualPeriodYear
        {
            get
            {
                Observation obs = GetObservation("8665-2");
                return GetDateFragmentOrPartialDate(obs?.Value as Hl7.Fhir.Model.FhirDateTime, VR.ExtensionURL.PartialDateTimeYearVR);
            }
            set
            {
                Observation obs = GetOrCreateObservation("8665-2", CodeSystems.LOINC, "Mother Prenatal", BFDR.ProfileURL.ObservationLastMenstrualPeriod, MOTHER_PRENATAL_SECTION);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {
                    obs.Value = new FhirDateTime();
                    obs.Value.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetYear(value, obs.Value as Hl7.Fhir.Model.FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Month of Last Menstrual Period.</summary>
        /// <value>the month that the last normal menstrual period began</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.LastMenstrualPeriodMonth = 2;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Month of Last Menstrual Period: {ExampleBirthRecord.LastMenstrualPeriodMonth}");</para>
        /// </example>
        [Property("LastMenstrualPeriodMonth", Property.Types.Int32, "Mother Prenatal", "Month of Last Menstrual Period.", true, BFDR.IGURL.ObservationLastMenstrualPeriod, true, 155)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8665-2')", "")]
        public int? LastMenstrualPeriodMonth
        {
            get
            {
                Observation obs = GetObservation("8665-2");
                return GetDateFragmentOrPartialDate(obs?.Value as Hl7.Fhir.Model.FhirDateTime, VR.ExtensionURL.PartialDateTimeMonthVR);
            }
            set
            {
                Observation obs = GetOrCreateObservation("8665-2", CodeSystems.LOINC, "Mother Prenatal", BFDR.ProfileURL.ObservationLastMenstrualPeriod, MOTHER_PRENATAL_SECTION);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {
                    obs.Value = new FhirDateTime();
                    obs.Value.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetMonth(value, obs.Value as Hl7.Fhir.Model.FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Day of Last Menstrual Period.</summary>
        /// <value>the day that the last normal menstrual period began</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.LastMenstrualPeriodDay = 28;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Day of Last Menstrual Period: {ExampleBirthRecord.LastMenstrualPeriodDay}");</para>
        /// </example>
        [Property("LastMenstrualPeriodDay", Property.Types.Int32, "Mother Prenatal", "Day of Last Menstrual Period.", true, BFDR.IGURL.ObservationLastMenstrualPeriod, true, 156)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='8665-2')", "")]
        public int? LastMenstrualPeriodDay
        {
            get
            {
                Observation obs = GetObservation("8665-2");
                return GetDateFragmentOrPartialDate(obs?.Value as Hl7.Fhir.Model.FhirDateTime, VR.ExtensionURL.PartialDateTimeDayVR);
            }
            set
            {
                Observation obs = GetOrCreateObservation("8665-2", CodeSystems.LOINC, "Mother Prenatal", BFDR.ProfileURL.ObservationLastMenstrualPeriod, MOTHER_PRENATAL_SECTION);

                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {
                    obs.Value = new FhirDateTime();
                    obs.Value.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetDay(value, obs.Value as Hl7.Fhir.Model.FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>First Prenatal Care Visit.</summary>
        /// <value>the date of the first prenatal care visit</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FirstPrenatalCareVisit = "2023-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"First prenatal care visit on: {ExampleBirthRecord.FirstPrenatalCareVisit}");</para>
        /// </example>
        [Property("FirstPrenatalCareVisit", Property.Types.String, "Mother Prenatal", "First Prenatal Care Visit.", true, BFDR.IGURL.ObservationDateOfFirstPrenatalCareVisit, true, 126)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='69044-6')", "")]
        public string FirstPrenatalCareVisit
        {
            get
            {
                Observation obs = GetObservation("69044-6");
                if (obs != null)
                {
                    return (obs.Value as Hl7.Fhir.Model.FhirDateTime)?.Value;
                }
                    return null;
            }
            set
            {
                Observation obs = GetOrCreateObservation("69044-6", CodeSystems.LOINC, "Mother Prenatal", BFDR.ProfileURL.ObservationDateOfFirstPrenatalCareVisit, MOTHER_PRENATAL_SECTION, Child.Id);
                obs.Value = ConvertToDateTime(value);
            }
        }

        /// <summary>Marital Description</summary>
        /// <value>for use of jurisdictions with domestic partnerships, othertypes of relationships</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MaritalStatus = "single";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Marital status: {ExampleBirthRecord.MaritalStatus}");</para>
        /// </example>
        [Property("MaritalStatus", Property.Types.String, "MaritalStatus", "MaritalStatus", false, VR.IGURL.Mother, true, 288)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "maritalStatus")]
        public string MaritalStatus
        {
            get
            {
                return Mother?.MaritalStatus?.Text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                CodeableConcept cc = new CodeableConcept();
                cc.Text = value;
                Mother.MaritalStatus = cc;
            }
        }

        /// <summary>Mother Married During Pregnancy</summary>
        /// <value>Mother marrier during pregnancy</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherMarriedDuringPregnancy = yesNoUnknownCC;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother married during pregnancy: {ExampleBirthRecord.MotherMarriedDuringPregnancy}");</para>
        /// </example>
        [Property("MotherMarriedDuringPregnancy", Property.Types.Dictionary, "MotherMarriedDuringPregnancy", "MotherMarriedDuringPregnancy", false, BFDR.IGURL.ObservationMotherMarriedDuringPregnancy, true, 288)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87301-8')", "")]
        public Dictionary<string, string> MotherMarriedDuringPregnancy
        {
            get => GetObservationValue("87301-8");
            set => SetObservationValue(value, "87301-8", CodeSystems.LOINC, "Mother married during pregnancy", BFDR.ProfileURL.ObservationMotherMarriedDuringPregnancy, MOTHER_INFORMATION_SECTION);
        }

        /// <summary>Year of the First Prenatal Care Visit.</summary>
        /// <value>the year of the first prenatal care visit</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FirstPrenatalCareVisitYear = 2023;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Year of the First Prenatal Visit: {ExampleBirthRecord.FirstPrenatalCareVisitYear}");</para>
        /// </example>
        [Property("FirstPrenatalCareVisitYear", Property.Types.Int32, "Mother Prenatal", "Year of First Prenatal Care Visit.", true, BFDR.IGURL.ObservationDateOfFirstPrenatalCareVisit, true, 128)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='69044-6')", "")]
        public int? FirstPrenatalCareVisitYear
        {
            get
            {
                Observation obs = GetObservation("69044-6");
                return GetDateFragmentOrPartialDate(obs?.Value as Hl7.Fhir.Model.FhirDateTime, VR.ExtensionURL.PartialDateTimeYearVR);
            }
            set
            {
                Observation obs = GetOrCreateObservation("69044-6", CodeSystems.LOINC, "Mother Prenatal", BFDR.ProfileURL.ObservationDateOfFirstPrenatalCareVisit, MOTHER_PRENATAL_SECTION, Child.Id);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {
                    obs.Value = new FhirDateTime();
                    obs.Value.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetYear(value, obs.Value as Hl7.Fhir.Model.FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Month of First Prenatal Care Visit.</summary>
        /// <value>the month of the first prenatal care visit</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FirstPrenatalCareVisitMonth = 2;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Month of First Prenatal Visit: {ExampleBirthRecord.FirstPrenatalCareVisitMonth}");</para>
        /// </example>
        [Property("FirstPrenatalCareVisitMonth", Property.Types.Int32, "Mother Prenatal", "Month of First Prenatal Care Visit.", true, BFDR.IGURL.ObservationDateOfFirstPrenatalCareVisit, true, 126)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='69044-6')", "")]
        public int? FirstPrenatalCareVisitMonth
        {
            get
            {
                Observation obs = GetObservation("69044-6");
                return GetDateFragmentOrPartialDate(obs?.Value as Hl7.Fhir.Model.FhirDateTime, VR.ExtensionURL.PartialDateTimeMonthVR);
            }
            set
            {
                Observation obs = GetOrCreateObservation("69044-6", CodeSystems.LOINC, "Mother Prenatal", BFDR.ProfileURL.ObservationDateOfFirstPrenatalCareVisit, MOTHER_PRENATAL_SECTION, Child.Id);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {
                    obs.Value = new FhirDateTime();
                    obs.Value.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetMonth(value, obs.Value as Hl7.Fhir.Model.FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Day of First Prenatal Care Visit.</summary>
        /// <value>the day of the first prenatal care visit</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.FirstPrenatalCareVisitDay = 2;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Month of First Prenatal Visit: {ExampleBirthRecord.FirstPrenatalCareVisitDay}");</para>
        /// </example>
        [Property("FirstPrenatalCareVisitDay", Property.Types.Int32, "Mother Prenatal", "Day of First Prenatal Care Visit.", true, BFDR.IGURL.ObservationDateOfFirstPrenatalCareVisit, true, 127)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='69044-6')", "")]
        public int? FirstPrenatalCareVisitDay
        {
            get
            {
                Observation obs = GetObservation("69044-6");
                return GetDateFragmentOrPartialDate(obs?.Value as Hl7.Fhir.Model.FhirDateTime, VR.ExtensionURL.PartialDateTimeDayVR);
            }
            set
            {
                Observation obs = GetOrCreateObservation("69044-6", CodeSystems.LOINC, "Mother Prenatal", BFDR.IGURL.ObservationDateOfFirstPrenatalCareVisit, MOTHER_PRENATAL_SECTION, Child.Id);
                if (obs.Value as Hl7.Fhir.Model.FhirDateTime == null)
                {
                    obs.Value = new FhirDateTime();
                    obs.Value.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetDay(value, obs.Value as Hl7.Fhir.Model.FhirDateTime);
                if (newDate != null)
                {
                    obs.Value = newDate;
                }
            }
        }

        /// <summary>Date of Registration.</summary>
        /// <value>the date that the birth was registered</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.RegistrationDate = "2023-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of birth registration: {ExampleBirthRecord.RegistrationDate}");</para>
        /// </example>
        [Property("RegistrationDate", Property.Types.String, "Birth Certification", "Date of Registration.", true, BFDR.IGURL.CompositionProviderLiveBirthReport, true, 243)]
        [FHIRPath("Bundle.entry.resource.where($this is Composition)", "date")]
        public string RegistrationDate
        {
            get
            {
                return this.Composition?.Date;
            }
            set
            {
                this.Composition.DateElement = ConvertToDateTime(value);
            }
        }

        /// <summary>Year of Registration.</summary>
        /// <value>the year that the birth was registered</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.RegistrationDateYear = 2023;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Year of birth registration: {ExampleBirthRecord.RegistrationDateYear}");</para>
        /// </example>
        [Property("RegistrationDateYear", Property.Types.Int32, "Birth Certification", "Year of Registration.", true, BFDR.IGURL.CompositionProviderLiveBirthReport, true, 243)]
        [FHIRPath("Bundle.entry.resource.where($this is Composition)", "date")]
        public int? RegistrationDateYear
        {
            get
            {
                // First check the value string
                if (this.Composition.Date != null && ParseDateElements(this.Composition?.Date, out int? year, out int? month, out int? day))
                {
                    return year;
                }
                return GetDateFragmentOrPartialDate(this.Composition?.DateElement, VR.ExtensionURL.PartialDateTimeYearVR);
            }
            set
            {
                if (this.Composition.DateElement == null)
                {
                    this.Composition.DateElement = new FhirDateTime();
                    this.Composition.DateElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetYear(value, this.Composition.DateElement);
                if (newDate != null)
                {
                    this.Composition.DateElement = newDate;
                }
            }
        }

        /// <summary>Month of Registration.</summary>
        /// <value>the month that the birth was registered</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.RegistrationDateMonth = 10;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Month of birth registration: {ExampleBirthRecord.RegistrationDateMonth}");</para>
        /// </example>
        [Property("RegistrationDateMonth", Property.Types.Int32, "Birth Certification", "Month of Registration.", true, BFDR.IGURL.CompositionProviderLiveBirthReport, true, 244)]
        [FHIRPath("Bundle.entry.resource.where($this is Composition)", "date")]
        public int? RegistrationDateMonth
        {
            get
            {
                // First check the value string
                if (this.Composition.Date != null && ParseDateElements(this.Composition?.Date, out int? year, out int? month, out int? day))
                {
                    return month;
                }
                return GetDateFragmentOrPartialDate(this.Composition?.DateElement, VR.ExtensionURL.PartialDateTimeMonthVR);
            }
            set
            {
                if (this.Composition.DateElement == null)
                {
                    this.Composition.DateElement = new FhirDateTime();
                    this.Composition.DateElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetMonth(value, this.Composition.DateElement);
                if (newDate != null)
                {
                    this.Composition.DateElement = newDate;
                }
            }
        }

        /// <summary>Day of Registration.</summary>
        /// <value>the day that the birth was registered</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.RegistrationDateDay = 23;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Day of birth registration: {ExampleBirthRecord.RegistrationDateDay}");</para>
        /// </example>
        [Property("RegistrationDateDay", Property.Types.Int32, "Birth Certification", "Day of Registration.", true, BFDR.IGURL.CompositionProviderLiveBirthReport, true, 245)]
        [FHIRPath("Bundle.entry.resource.where($this is Composition)", "date")]
        public int? RegistrationDateDay
        {
            get
            {
                // First check the value string
                if (this.Composition.Date != null && ParseDateElements(this.Composition?.Date, out int? year, out int? month, out int? day))
                {
                    return day;
                }
                return GetDateFragmentOrPartialDate(this.Composition?.DateElement, VR.ExtensionURL.PartialDateTimeDayVR);
            }
            set
            {
                if (this.Composition.DateElement == null)
                {
                    this.Composition.DateElement = new FhirDateTime();
                    this.Composition.DateElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                }
                FhirDateTime newDate = SetDay(value, this.Composition.DateElement);
                if (newDate != null)
                {
                    this.Composition.DateElement = newDate;

                }
            }
        }

        /// <summary>Principal source of Payment for this delivery.</summary>
        /// <value>Principal source of Payment for this delivery. A Dictionary representing a codeable concept of the payor type:
        /// <para>"code" - The code used to describe this concept.</para>
        /// <para>"system" - The relevant code system.</para>
        /// <para>"display" - The human readable version of this code.</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; locationType = new Dictionary&lt;string, string&gt;();</para>
        /// <para>locationType.Add("code", "privateinsurance");</para>
        /// <para>locationType.Add("system", "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-vr-birth-and-fetal-death-financial-class");</para>
        /// <para>locationType.Add("display", "PRIVATE HEALTH INSURANCE");</para>
        /// <para>ExampleBirthRecord.PayorTypeFinancialClass = locationType;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Principal source of Payment for this delivery: {ExampleBirthRecord.PayorTypeFinancialClass["code"]}");</para>
        /// </example>
        [Property("PayorTypeFinancialClass", Property.Types.Dictionary, "Payor Type Financial Class", "Source of Payment.", true, IGURL.CoveragePrincipalPayerDelivery, true, 16)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Coverage)", "")]
        public Dictionary<string, string> PayorTypeFinancialClass
        {
            get
            {
                if (Coverage == null)
                {
                    return EmptyCodeableDict();
                }
                return CodeableConceptToDict(Coverage.Type);
            }
            set
            {
                if (Coverage == null)
                {
                    Coverage = new Coverage()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Meta = new Meta()
                    };
                    Coverage.Meta.Profile = new List<string>()
                    {
                        ProfileURL.CoveragePrincipalPayerDelivery
                    };
                }
                Coverage.Type = DictToCodeableConcept(value);
            }
        }

        /// <summary>Principal source of Payment for this delivery Helper</summary>
        /// <value>Principal source of Payment for this delivery Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.PayorTypeFinancialClassHelper = "Hospital";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child's Place Of Birth Type: {ExampleBirthRecord.PayorTypeFinancialClassHelper}");</para>
        /// </example>
        [Property("PayorTypeFinancialClassHelper", Property.Types.String, "PayorTypeFinancialClassHelper", "Principal source of Payment for this delivery Helper.", false, IGURL.CoveragePrincipalPayerDelivery, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Coverage).where(meta.profile == " + ProfileURL.CoveragePrincipalPayerDelivery + ")", "")]
        public string PayorTypeFinancialClassHelper
        {
            get
            {
                if (PayorTypeFinancialClass.ContainsKey("code"))
                {
                    string code = PayorTypeFinancialClass["code"];
                    if (code == "9999")
                    {
                        if (PayorTypeFinancialClass.ContainsKey("text") && !String.IsNullOrWhiteSpace(PayorTypeFinancialClass["text"]))
                        {
                            return PayorTypeFinancialClass["text"];
                        }
                        return "9999";
                    }
                    else if (!String.IsNullOrWhiteSpace(code))
                    {
                        return code;
                    }
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    // do nothing
                    return;
                }
                if (!BFDR.Mappings.BirthAndFetalDeathFinancialClass.FHIRToIJE.ContainsKey(value))
                {
                    // unknown
                    Console.WriteLine("Warning: given 'principal source of payment for this delivery' code not found in value set. Setting value to 'Unavailable / Unknown'.");
                    PayorTypeFinancialClass = CodeableConceptToDict(new CodeableConcept(CodeSystems.NAHDO, "9999", "Unavailable / Unknown", value));
                }
                else
                {
                    // normal path
                    SetCodeValue("PayorTypeFinancialClass", value, BFDR.ValueSets.BirthAndFetalDeathFinancialClass.Codes);
                }
            }
        }
        /// <summary>Mother Married During Pregnancy Helper</summary>
        /// <value>Mother marrier during pregnancy helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherMarriedDuringPregnancyHelper = "yes";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother married during pregnancy helper: {ExampleBirthRecord.MotherMarriedDuringPregnancyHelper}");</para>
        /// </example>
        [Property("MotherMarriedDuringPregnancyHelper", Property.Types.String, "MotherMarriedDuringPregnancyHelper", "MotherMarriedDuringPregnancyHelper", false, BFDR.IGURL.ObservationMotherMarriedDuringPregnancy, true, 288)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87301-8')", "")]
        public string MotherMarriedDuringPregnancyHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, VR.ValueSets.YesNoUnknown.Codes);
        }

        /// <summary>Paternity Acknowledgement Signed</summary>
        /// <value>Paternity acknowledgement signed</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.PaternityAcknowledgementSigned = yesNoUnknownCC;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Paternity acknowledgement signed: {ExampleBirthRecord.PaternityAcknowledgementSigned}");</para>
        /// </example>
        [Property("PaternityAcknowledgementSigned", Property.Types.Dictionary, "PaternityAcknowledgementSigned", "PaternityAcknowledgementSigned", false, BFDR.IGURL.ObservationPaternityAcknowledgementSigned, true, 288)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87302-6')", "")]
        public Dictionary<string, string> PaternityAcknowledgementSigned
        {
            get => GetObservationValue("87302-6");
            set => SetObservationValue(value, "87302-6", CodeSystems.LOINC, "Paternity acknowledgement signed", BFDR.ProfileURL.ObservationPaternityAcknowledgementSigned, FATHER_INFORMATION_SECTION);
        }

        /// <summary>Paternity Acknowledgement Signed Helper</summary>
        /// <value>Paternity acknowledgement signed helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.PaternityAcknowledgementSignedHelper = "yes";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Paternity acknowledgement signed: {ExampleBirthRecord.PaternityAcknowledgementSignedHelper}");</para>
        /// </example>
        [Property("PaternityAcknowledgementSignedHelper", Property.Types.String, "PaternityAcknowledgementSignedHelper", "PaternityAcknowledgementSignedHelper", false, BFDR.IGURL.ObservationPaternityAcknowledgementSigned, true, 288)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87302-6')", "")]
        public string PaternityAcknowledgementSignedHelper
        {
            get => GetObservationValueHelper();
            set => SetObservationValueHelper(value, VR.ValueSets.YesNoUnknownNotApplicable.Codes);
        }

        /// <summary>Mother Transferred</summary>
        /// <value>Mother transferred</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherTransferred = "hosp-trans";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother transferred: {ExampleBirthRecord.MotherTransferred}");</para>
        /// </example>
        [Property("MotherTransferred", Property.Types.Dictionary, "MotherTransferred", "MotherTransferred", false, VR.IGURL.Mother, true, 288)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "hospitalization")]
        public Dictionary<string, string> MotherTransferred
        {
            get
            {
                Dictionary<string, string> admitSource = CodeableConceptToDict(EncounterMaternity?.Hospitalization?.AdmitSource);
                return admitSource;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                // TODO define CC
                CodeableConcept admitSource = DictToCodeableConcept(value);
                HospitalizationComponent hosp = new HospitalizationComponent();
                hosp.AdmitSource = admitSource;
                EncounterMaternity.Hospitalization = hosp;
            }
        }

        /// <summary>Mother Transferred Helper</summary>
        /// <value>Mother transferred helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherTransferredHelper = "hosp-trans";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother transferred: {ExampleBirthRecord.MotherTransferredHelper}");</para>
        /// </example>
        [Property("MotherTransferred", Property.Types.String, "MotherTransferred", "MotherTransferred", false, VR.IGURL.Mother, true, 288)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "hospitalization")]
        public string MotherTransferredHelper
        {
            get
            {
                // TODO there should be a mapping for this
                if (MotherTransferred.ContainsKey("code"))
                {
                    string code = MotherTransferred["code"];
                    if (code == "hosp-trans")
                    {
                        // TODO add check for Transferred From name == "UNKNOWN" and return U
                        if (FacilityMotherTransferredFrom == "UNKNOWN")
                        {
                            return "U";
                        }
                        return "Y";
                    }
                    if (code == "other")
                    {
                        return "N";
                    }
                }
                return "";
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }
                // IJE values are Y, N, U, set to "hosp-trans" if value is Y
                // https://build.fhir.org/ig/HL7/fhir-bfdr/usage.html#mother-or-infant-transferred
                if (value == "Y")
                {
                    MotherTransferred = CodeableConceptToDict(new CodeableConcept(CodeSystems.AdmitSource, "hosp-trans", "Transferred from other hospital", "The Patient has been transferred from another hospital for this encounter."));
                }
                else if (value == "U")
                {
                    // If the value is unknown, set the code to hosp-trans, and the hospitalization.origin.name should be set to “UNKNOWN” 
                    // https://build.fhir.org/ig/HL7/fhir-bfdr/usage.html#mother-or-infant-transferred
                    MotherTransferred = CodeableConceptToDict(new CodeableConcept(CodeSystems.AdmitSource, "hosp-trans", "Transferred from other hospital", "The Patient has been transferred from another hospital for this encounter."));
                    FacilityMotherTransferredFrom = "UNKNOWN";
                }
                else 
                {
                    // all other codes should be interpretted as N with "other" as the code to express mother did not transfer
                    MotherTransferred = CodeableConceptToDict(new CodeableConcept(CodeSystems.AdmitSource, "other", "Other", "Did not transfer"));
                }
            }
        }

        /// <summary>Infant Living</summary>
        /// <value>Infant Living</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.InfantLiving = true;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother transferred: {ExampleBirthRecord.MotherTransferred}");</para>
        /// </example>
        [Property("InfantLiving", Property.Types.Bool, "InfantLiving", "InfantLiving", false, BFDR.IGURL.ObservationInfantLiving, true, 288)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='73757-7')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool? InfantLiving
        {
            get
            {
                Observation obs = GetObservation("73757-7");
                if (obs != null)
                {
                    bool? infantLiving = ((FhirBoolean)obs.Value).Value;
                    return infantLiving;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                Observation obs = GetOrCreateObservation("73757-7", CodeSystems.LOINC, "Infant living", BFDR.ProfileURL.ObservationInfantLiving, NEWBORN_INFORMATION_SECTION);
                obs.Value = new FhirBoolean(value);
            }
        }

        /// <summary>Infant Transferred</summary>
        /// <value>Infant transferred</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.InfantTransferred = "hosp-trans";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Infant transferred: {ExampleBirthRecord.InfantTransferred}");</para>
        /// </example>
        [Property("InfantTransferred", Property.Types.Dictionary, "InfantTransferred", "InfantTransferred", false, BFDR.IGURL.EncounterBirth, true, 288)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "hospitalization")]
        public Dictionary<string, string> InfantTransferred
        {
            get
            {
                Dictionary<string, string> dischargeDisposition = CodeableConceptToDict(EncounterBirth?.Hospitalization?.DischargeDisposition);
                return dischargeDisposition;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                CodeableConcept dischargeDisposition = DictToCodeableConcept(value);
                HospitalizationComponent hosp = new HospitalizationComponent();
                hosp.DischargeDisposition = dischargeDisposition;
                EncounterBirth.Hospitalization = hosp;
            }
        }

        /// <summary>Infant Transferred Helper</summary>
        /// <value>Infant transferred helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.InfantTransferredHelper = "hosp-trans";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Infant transferred helper: {ExampleBirthRecord.InfantTransferredHelper}");</para>
        /// </example>
        [Property("InfantTransferred", Property.Types.String, "InfantTransferred", "InfantTransferred", false, BFDR.IGURL.EncounterBirth, true, 288)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "hospitalization")]
        public string InfantTransferredHelper
        {
            get
            {
                if (InfantTransferred.ContainsKey("code"))
                {
                    string code = InfantTransferred["code"];
                    if (code == "other-hcf")
                    {
                        return "Y";
                    }
                    else if (FacilityInfantTransferredTo == "UNKNOWN")
                    {
                        return "U";
                    }
                    else if (code == "oth")
                    {
                        return "N";
                    }
                }

                return "";
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }
                // IJE values are Y, N, U, only set to "hosp-trans" if value is Y
                // IG guidance https://build.fhir.org/ig/HL7/fhir-bfdr/usage.html#mother-or-infant-transferred
                if (value == "Y")
                {
                    // TODO update this to point to generated code system
                    InfantTransferred = CodeableConceptToDict(new CodeableConcept(CodeSystems.DischargeDisposition, "other-hcf", "Other healthcare facility", "The patient was transferred to another healthcare facility."));
                }
                else if (value == "N")
                {
                    // TODO update this to point to generated code system
                    InfantTransferred = CodeableConceptToDict(new CodeableConcept(CodeSystems.DischargeDisposition, "oth", "Other", "Did not transfer"));
                }
                else if (value == "U")
                {
                    // TODO set destination in this observation to a reference of the location
                    FacilityInfantTransferredTo = "UNKNOWN";
                }
            }
        }

        /// <summary>Number Live Born</summary>
        /// <value>Number of live born</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.NumberLiveBorn = 1;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Number of live born: {ExampleBirthRecord.NumberLiveBorn}");</para>
        /// </example>
        [Property("NumberLiveBorn", Property.Types.Int32, "NumberLiveBorn", "NumberLiveBorn", false, BFDR.IGURL.ObservationNumberLiveBirthsThisDelivery, true, 288)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='73773-4')", "")]
        public int? NumberLiveBorn
        {
            get
            {
                Observation obs = GetObservation("73773-4");
                if (obs != null)
                {
                    return (obs.Value as Hl7.Fhir.Model.Integer).Value;
                }
                return null;
            }
            set
            {
                // The observation value for NumberLiveBorn is an int not a codeable concept so we can't use the util functions
                if (value == null)
                {
                    return;
                }
                Observation obs = GetOrCreateObservation("73773-4", CodeSystems.LOINC, "Number live born", BFDR.ProfileURL.ObservationNumberLiveBirthsThisDelivery, MOTHER_INFORMATION_SECTION);
                obs.Value = new Hl7.Fhir.Model.Integer(value);
            }
        }

        /// <summary>SSNRequested</summary>
        /// <value>SSN requested</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.SSNRequested = true;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"SSN Requested: {ExampleBirthRecord.SSNRequested}");</para>
        /// </example>
        [Property("SSNRequested", Property.Types.Bool, "SSNRequested", "SSNRequested", false, BFDR.IGURL.ObservationSSNRequestedForChild, true, 288)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='87295-2')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public bool? SSNRequested
        {
            get
            {
                Observation obs = GetObservation("87295-2");
                if (obs != null)
                {
                    bool? ssnRequested = ((FhirBoolean)obs.Value).Value;
                    return ssnRequested;
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                Observation obs = GetOrCreateObservation("87295-2", CodeSystems.LOINC, "SSN requested", BFDR.ProfileURL.ObservationSSNRequestedForChild, NEWBORN_INFORMATION_SECTION);
                obs.Value = new FhirBoolean(value);
            }
        }

        private int? GetIntegerObservationValue(string code)
        {
            Observation observation = GetObservation(code);
            if (observation != null)
            {
                return (int?)(observation?.Value as Hl7.Fhir.Model.Integer)?.Value;
            }
            return null;
        }

        private Observation SetIntegerObservationValue(string code, string codeDesc, int? value, string profileURL, string section, string subjectId, [CallerMemberName] string propertyName = null)
        {
            Observation obs = GetOrCreateObservation(code, CodeSystems.LOINC, codeDesc, profileURL, section, null, propertyName);
            if (obs.Category.FirstOrDefault(cat => cat.Coding.Any(catCode => catCode.Code == "vital-signs")) == null)
            {
                obs.Category.Add(new CodeableConcept(CodeSystems.ObservationCategory, "vital-signs"));
            }
            // Create an empty Integer if needed
            if (obs.Value == null || obs.Value as Integer == null)
            {
                obs.Value = new Hl7.Fhir.Model.Integer();
            }
            // Set the properties of the value individually to preserve any existing obs.Value.Extension entries
            if (value != null)
            {
                (obs.Value as Integer).Value = (int)value;
            }
            return obs;
        }

        /// <summary>APGAR score at 5 mins.</summary>
        /// <value>the APGAR score at 5 minutes, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.ApgarScoreFiveMinutes = 20;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"APGAR score at 5 minutes: {ExampleBirthRecord.ApgarScoreFiveMinutes}");</para>
        /// </example>
        [Property("ApgarScoreFiveMinutes", Property.Types.Int32, "Newborn Information", "APGAR score at 5 minutes.", true, BFDR.IGURL.ObservationApgarScore, true, 205)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='9274-2')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public int? ApgarScoreFiveMinutes
        {
            get => GetIntegerObservationValue("9274-2");
            set => SetIntegerObservationValue("9274-2", "5 minute Apgar Score", value, BFDR.ProfileURL.ObservationApgarScore, NEWBORN_INFORMATION_SECTION, Child.Id);
        }

        /// <summary>APGAR score at 10 mins.</summary>
        /// <value>the APGAR score at 10 minutes, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.ApgarScoreTenMinutes = 20;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"APGAR score at 10 minutes: {ExampleBirthRecord.ApgarScoreTenMinutes}");</para>
        /// </example>
        [Property("ApgarScoreTenMinutes", Property.Types.Int32, "Newborn Information", "APGAR score at 10 minutes.", true, BFDR.IGURL.ObservationApgarScore, true, 206)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='9271-8')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public int? ApgarScoreTenMinutes
        {
            get => GetIntegerObservationValue("9271-8");
            set => SetIntegerObservationValue("9271-8", "10 minute Apgar Score", value, BFDR.ProfileURL.ObservationApgarScore, NEWBORN_INFORMATION_SECTION, Child.Id);
        }


        /// <summary>Certifier name.</summary>
        /// <value>the certifier's name</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifierName = "Janet Seito";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certifier's Name: {ExampleBirthRecord.CertifierName}");</para>
        /// </example>
        [Property("Certifier Name", Property.Types.String, "Birth Certification", "Name of Certifier.", true, VR.IGURL.Practitioner, true, 6)]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner).where(extension.value ='certifier')", "name")]
        public string CertifierName
        {
            get
            {
                if (Certifier != null && Certifier.Name != null)
                {
                    return Certifier.Name.FirstOrDefault()?.Text;
                }
                return null;
            }
            set
            {
                if (Certifier == null)
                {
                    CreateCertifier();
                }
                HumanName name = Certifier.Name.FirstOrDefault();
                if (name != null && !String.IsNullOrEmpty(value))
                {
                    name.Text = value;
                }
                else if (!String.IsNullOrEmpty(value))
                {
                    name = new HumanName();
                    name.Use = HumanName.NameUse.Official;
                    name.Text = value;
                    Certifier.Name.Add(name);
                }
            }
        }

        /// <summary>Certifier NPI</summary>
        /// <value>the certifiers npi</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifierNPI = "123456789011";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certifier NPI: {ExampleBirthRecord.CertifierNPI}");</para>
        /// </example>
        [Property("Certifier NPI", Property.Types.String, "Birth Certification", "Certifier's NPI.", true, VR.IGURL.Practitioner, true, 13)]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner).where(extension.value ='certifier').identifier.where(system='http://hl7.org/fhir/sid/us-npi')", "value")]
        public string CertifierNPI
        {
            get
            {
                return Certifier?.Identifier?.Find(id => id.System == CodeSystems.US_NPI_HL7)?.Value;
            }
            set
            {
                if (Certifier == null)
                {
                    CreateCertifier();
                }
                if (Certifier.Identifier.Count > 0)
                {
                    Certifier.Identifier.Clear();
                }
                Certifier.Identifier.RemoveAll(iden => iden.System == CodeSystems.US_NPI_HL7);
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Identifier npi = new Identifier();
                npi.Type = new CodeableConcept(CodeSystems.HL7_identifier_type, "NPI", "National Provider Identifier", null);
                npi.System = CodeSystems.US_NPI_HL7;
                npi.Value = value;
                Certifier.Identifier.Add(npi);
            }
        }

        /// <summary>Certifier Title</summary>
        /// <value>the title/qualification of the person who certified the birth. A Dictionary representing a code, containing the following key/value pairs:
        /// <para>"code" - the code</para>
        /// <para>"system" - the code system this code belongs to</para>
        /// <para>"display" - a human readable meaning of the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; title = new Dictionary&lt;string, string&gt;();</para>
        /// <para>title.Add("code", "112247003");</para>
        /// <para>title.Add("system", CodeSystems.SCT);</para>
        /// <para>title.Add("display", "Medical Doctor");</para>
        /// <para>ExampleBirthRecord.CertifierTitle = title;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certifier Title: {ExampleBirthRecord.CertifierTitle['display']}");</para>
        /// </example>
        [Property("Certifiers Title", Property.Types.Dictionary, "Birth Certification", "Certifier's Title.", true, VR.IGURL.Practitioner, true, 13)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner).where(extension.value ='certifier')", "qualification")]
        public Dictionary<string, string> CertifierTitle
        {
            get
            {
                if (Certifier == null)
                {
                    return EmptyCodeableDict();
                }
                Practitioner.QualificationComponent qualification = Certifier.Qualification.FirstOrDefault();
                if (Certifier != null && qualification != null)
                {
                    return CodeableConceptToDict(qualification.Code);
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (Certifier == null)
                {
                    CreateCertifier();
                }
                Practitioner.QualificationComponent qualification = new Practitioner.QualificationComponent();
                qualification.Code = DictToCodeableConcept(value);
                Certifier.Qualification.Clear();
                Certifier.Qualification.Add(qualification);
            }
        }

        /// <summary>Certifier Title Helper.</summary>
        /// <value>the title/qualification of the Certifier.
        /// <para>"code" - the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifierTitleHelper = ValueSets.BirthAttendantsTitles.MedicalDoctor;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certifier Title: {ExampleBirthRecord.CertifierTitleHelper}");</para>
        /// </example>
        [Property("Certifier Title Helper", Property.Types.String, "Birth Certification", "Certifier Title.", false, VR.IGURL.Practitioner, true, 4)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner).where(extension.value ='certifier')", "qualification")]
        public string CertifierTitleHelper
        {
            get
            {
                if (CertifierTitle.ContainsKey("code"))
                {
                    string code = CertifierTitle["code"];
                    if (!String.IsNullOrWhiteSpace(code))
                    {
                        return code;
                    }
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    // do nothing
                    return;
                }
                if (!VR.Mappings.BirthAttendantTitles.FHIRToIJE.ContainsKey(value))
                { //other
                    Console.WriteLine("Warning: given 'certifier title' code not found in value set. Setting value to 'Other'.");
                    CertifierTitle = CodeableConceptToDict(new CodeableConcept(CodeSystems.NullFlavor_HL7_V3, "OTH", "Other", value));
                }
                else
                { // normal path
                    SetCodeValue("CertifierTitle", value, VR.ValueSets.BirthAttendantTitles.Codes);
                }
            }
        }

        /// <summary>Certifier Other Helper.</summary>
        /// <value>the "other" title/qualification of the Certifier.
        /// <para>"code" - the code</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifierOtherHelper = "Birth Clerk";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certifier Other: {ExampleBirthRecord.CertifierOtherHelper}");</para>
        /// </example>
        [Property("Certifier Other Helper", Property.Types.String, "Birth Certification", "Certifier Other.", false, VR.IGURL.Practitioner, true, 4)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [FHIRPath("Bundle.entry.resource.where($this is Practitioner).where(extension.value ='certifier').qualification", "other")]
        public string CertifierOtherHelper
        {
            get
            {
                if (CertifierTitle.ContainsKey("code"))
                {
                    string code = CertifierTitle["code"];
                    if (code == "OTH")
                    {
                        if (CertifierTitle.ContainsKey("text") && !String.IsNullOrWhiteSpace(CertifierTitle["text"]))
                        {
                            return (CertifierTitle["text"]);
                        }
                    }
                }
                return null;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    // do nothing
                    return;
                }
                else
                {
                    CertifierTitle = CodeableConceptToDict(new CodeableConcept(CodeSystems.NullFlavor_HL7_V3, "OTH", "Other", value));
                }
            }
        }

        /// <summary>Date of Certification.</summary>
        /// <value>the date of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifiedDate = "2023-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of birth certification: {ExampleBirthRecord.CertificationDate}");</para>
        /// </example>
        [Property("CertificationDate", Property.Types.String, "Birth Certification", "Date of Certification.", true, BFDR.IGURL.CompositionProviderLiveBirthReport, true, 243)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")]
        public string CertificationDate
        {
            get
            {
                Encounter.ParticipantComponent certifier = EncounterBirth?.Participant?.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                if (certifier != null && certifier.Period.Start != null)
                {
                    return certifier.Period.Start;
                }
                return null;
            }
            set
            {
                Encounter.ParticipantComponent certifier = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                if (certifier != null)
                {
                    certifier.Period.StartElement = ConvertToDateTime(value);
                }
                else
                {
                    Encounter.ParticipantComponent newCertifier = new Encounter.ParticipantComponent();
                    CodeableConcept t = new CodeableConcept(CodeSystems.LOINC, "87287-9", "Birth certifier", null);
                    newCertifier.Type.Add(t);
                    Period p = new Period();
                    p.StartElement = ConvertToDateTime(value);
                    newCertifier.Period = p;
                    EncounterBirth.Participant.Add(newCertifier);
                }
            }
        }

        /// <summary>Certified Year</summary>
        /// <value>year of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifiedYear = 2023;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certified Year: {ExampleBirthRecord.CertifiedYear}");</para>
        /// </example>
        [Property("Certified Year", Property.Types.Int32, "Birth Certification", "Certified Year", true, IGURL.EncounterBirth, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")] 
        public int? CertifiedYear
        {
            get
            {
                if (EncounterBirth == null)
                {
                    return null;
                }
                Encounter.ParticipantComponent certifier = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                // First check the value string
                if (certifier == null || certifier.Period == null || certifier.Period.StartElement == null)
                {
                    return null;
                }
                if (certifier != null && certifier.Period.StartElement != null && ParseDateElements(certifier.Period.Start, out int? year, out int? month, out int? day))
                {
                    return year;
                }
                return GetDateFragmentOrPartialDate(certifier.Period.StartElement, VR.ExtensionURL.PartialDateTimeYearVR);
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (EncounterBirth == null)
                {
                    CreateBirthEncounter();
                }
                Encounter.ParticipantComponent stateComp = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                if (stateComp == null) // make certifier participant with date
                {  
                    Encounter.ParticipantComponent certifier = new Encounter.ParticipantComponent();
                    CodeableConcept t = new CodeableConcept(CodeSystems.LOINC, "87287-9", "Birth certifier", null);
                    certifier.Type.Add(t);
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    certifier.Period = p;
                    EncounterBirth.Participant.Add(certifier);
                    stateComp = certifier;
                }  
                if (stateComp.Period == null || stateComp.Period.StartElement == null) //certifier participant exists but no period or period.start
                {
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    stateComp.Period = p;
                }
                FhirDateTime newDate = SetYear(value, stateComp.Period.StartElement);
                if (newDate != null)
                {
                    stateComp.Period.StartElement = newDate;
                }
            }
        }

        /// <summary>Certified Month</summary>
        /// <value>month of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifiedMonth = 10;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certified Month: {ExampleBirthRecord.CertifiedMonth}");</para>
        /// </example>
        [Property("Certified Month", Property.Types.Int32, "Birth Certification", "Certified Month", true, IGURL.EncounterBirth, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")] 
        public int? CertifiedMonth
        {
            get
            {
                if (EncounterBirth == null)
                {
                    return null;
                }
                Encounter.ParticipantComponent certifier = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                // First check the value string
                if (certifier == null || certifier.Period == null || certifier.Period.StartElement == null)
                {
                    return null;
                }
                if (certifier != null && certifier.Period.StartElement != null && ParseDateElements(certifier.Period.Start, out int? year, out int? month, out int? day))
                {
                    return month;
                }
                return GetDateFragmentOrPartialDate(certifier.Period.StartElement, VR.ExtensionURL.PartialDateTimeMonthVR);
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (EncounterBirth == null)
                {
                    CreateBirthEncounter();
                }
                Encounter.ParticipantComponent stateComp = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                if (stateComp == null) // make certifier participant with date
                {  
                    Encounter.ParticipantComponent certifier = new Encounter.ParticipantComponent();
                    CodeableConcept t = new CodeableConcept(CodeSystems.LOINC, "87287-9", "Birth certifier", null);
                    certifier.Type.Add(t);
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    certifier.Period = p;
                    EncounterBirth.Participant.Add(certifier);
                    stateComp = certifier;
                }  
                if (stateComp.Period == null || stateComp.Period.StartElement == null) //certifier participant exists but no period or period.start
                {
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    stateComp.Period = p;
                }
                FhirDateTime newDate = SetMonth(value, stateComp.Period.StartElement); 
                if (newDate != null)
                {
                    stateComp.Period.StartElement = newDate; 
                }
            }
        }

        /// <summary>Certified Day</summary>
        /// <value>day of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifiedDay = 23;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certified Day: {ExampleBirthRecord.CertifiedDay}");</para>
        /// </example> 
        [Property("Certified Day", Property.Types.Int32, "Birth Certification", "Certified Day", true, IGURL.EncounterBirth, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")] 
        public int? CertifiedDay
        {
            get
            {
                if (EncounterBirth == null)
                {
                    return null;
                }
                Encounter.ParticipantComponent certifier = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                // First check the value string
                if (certifier == null || certifier.Period == null || certifier.Period.StartElement == null)
                {
                    return null;
                }
                if (certifier != null && certifier.Period.StartElement != null && ParseDateElements(certifier.Period.Start, out int? year, out int? month, out int? day))
                {
                    return day;
                }
                return GetDateFragmentOrPartialDate(certifier.Period.StartElement, VR.ExtensionURL.PartialDateTimeDayVR);
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (EncounterBirth == null)
                {
                    CreateBirthEncounter();
                }
                Encounter.ParticipantComponent stateComp = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                if (stateComp == null) // make certifier participant with date
                {  
                    Encounter.ParticipantComponent certifier = new Encounter.ParticipantComponent();
                    CodeableConcept t = new CodeableConcept(CodeSystems.LOINC, "87287-9", "Birth certifier", null);
                    certifier.Type.Add(t);
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    certifier.Period = p;
                    EncounterBirth.Participant.Add(certifier);
                    stateComp = certifier;
                }  
                if (stateComp.Period == null || stateComp.Period.StartElement == null) //certifier participant exists but no period or period.start
                {
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    stateComp.Period = p;
                }
                FhirDateTime newDate = SetDay(value, stateComp.Period.StartElement); 
                if (newDate != null)
                {
                    stateComp.Period.StartElement = newDate; 
                }
            }
        }

        /// <summary>Set an emerging issue value, creating an empty EmergingIssues Observation as needed.</summary>
        private void SetEmergingIssue(string identifier, string value)
        {
            if (value == null)
            {
                return;
            }
            Observation EmergingIssues = GetOrCreateObservation(identifier, CodeSystems.ObservationCode, "NCHS-required Parameter Slots for Emerging Issues", VR.IGURL.EmergingIssues, EMERGING_ISSUES_SECTION, null, identifier);

            // Remove existing component (if it exists) and add an appropriate component.
            EmergingIssues.Component.RemoveAll(cmp => cmp.Code != null && cmp.Code.Coding != null && cmp.Code.Coding.Count() > 0 && cmp.Code.Coding.First().Code == identifier);
            Observation.ComponentComponent component = new Observation.ComponentComponent();
            component.Code = new CodeableConcept(CodeSystems.ComponentCode, identifier, null, null);
            component.Value = new FhirString(value);
            EmergingIssues.Component.Add(component);
        }

        /// <summary>Get an emerging issue value.</summary>
        private string GetEmergingIssue(string identifier)             
        {
            Observation EmergingIssues = GetObservation(identifier);
            if (EmergingIssues == null)
            {
                return null;
            }
            // Remove existing component (if it exists) and add an appropriate component.
            Observation.ComponentComponent issue = EmergingIssues.Component.FirstOrDefault(c => c.Code.Coding[0].Code == identifier);
            if (issue != null && issue.Value != null && issue.Value as FhirString != null)
            {
                return issue.Value.ToString();
            }
            return null;
        }


        /// <summary>Emerging Issue Field Length 1 Number 1</summary>
        /// <value>the emerging issue value</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EmergingIssue1_1 = "X";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Emerging Issue Value: {ExampleBirthRecord.EmergingIssue1_1}");</para>
        /// </example>
        [Property("Emerging Issue Field Length 1 Number 1", Property.Types.String, "Emerging Issues", "One-Byte Field 1", true, VR.IGURL.EmergingIssues, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='emergingissues')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string EmergingIssue1_1
        {
            get
            {
                return GetEmergingIssue("EmergingIssue1_1");
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetEmergingIssue("EmergingIssue1_1", value);
                }
            }
        }

        /// <summary>Emerging Issue Field Length 1 Number 2</summary>
        /// <value>the emerging issue value</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EmergingIssue1_2 = "X";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Emerging Issue Value: {ExampleBirthRecord.EmergingIssue1_2}");</para>
        /// </example>
        [Property("Emerging Issue Field Length 1 Number 2", Property.Types.String, "Emerging Issues", "1-Byte Field 2", true, VR.IGURL.EmergingIssues, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='emergingissues')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string EmergingIssue1_2
        {
            get
            {
                return GetEmergingIssue("EmergingIssue1_2");
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetEmergingIssue("EmergingIssue1_2", value);
                }
            }
        }

        /// <summary>Emerging Issue Field Length 1 Number 3</summary>
        /// <value>the emerging issue value</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EmergingIssue1_3 = "X";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Emerging Issue Value: {ExampleBirthRecord.EmergingIssue1_3}");</para>
        /// </example>
        [Property("Emerging Issue Field Length 1 Number 3", Property.Types.String, "Emerging Issues", "1-Byte Field 3", true, VR.IGURL.EmergingIssues, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='emergingissues')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string EmergingIssue1_3
        {
            get
            {
                return GetEmergingIssue("EmergingIssue1_3");
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetEmergingIssue("EmergingIssue1_3", value);
                }
            }
        }

        /// <summary>Emerging Issue Field Length 1 Number 4</summary>
        /// <value>the emerging issue value</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EmergingIssue1_4 = "X";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Emerging Issue Value: {ExampleBirthRecord.EmergingIssue1_4}");</para>
        /// </example>
        [Property("Emerging Issue Field Length 1 Number 4", Property.Types.String, "Emerging Issues", "1-Byte Field 4", true, VR.IGURL.EmergingIssues, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='emergingissues')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string EmergingIssue1_4
        {
            get
            {
                return GetEmergingIssue("EmergingIssue1_4");
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetEmergingIssue("EmergingIssue1_4", value);
                }
            }
        }

        /// <summary>Emerging Issue Field Length 1 Number 5</summary>
        /// <value>the emerging issue value</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EmergingIssue1_5 = "X";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Emerging Issue Value: {ExampleBirthRecord.EmergingIssue1_5}");</para>
        /// </example>
        [Property("Emerging Issue Field Length 1 Number 5", Property.Types.String, "Emerging Issues", "1-Byte Field 5", true, VR.IGURL.EmergingIssues, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='emergingissues')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string EmergingIssue1_5
        {
            get
            {
                return GetEmergingIssue("EmergingIssue1_5");
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetEmergingIssue("EmergingIssue1_5", value);
                }
            }
        }

        /// <summary>Emerging Issue Field Length 1 Number 6</summary>
        /// <value>the emerging issue value</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EmergingIssue1_6 = "X";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Emerging Issue Value: {ExampleBirthRecord.EmergingIssue1_6}");</para>
        /// </example>
        [Property("Emerging Issue Field Length 1 Number 6", Property.Types.String, "Emerging Issues", "1-Byte Field 6", true, VR.IGURL.EmergingIssues, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='emergingissues')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string EmergingIssue1_6
        {
            get
            {
                return GetEmergingIssue("EmergingIssue1_6");
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetEmergingIssue("EmergingIssue1_6", value);
                }
            }
        }

        /// <summary>Emerging Issue Field Length 8 Number 1</summary>
        /// <value>the emerging issue value</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EmergingIssue8_1 = "XXXXXXXX";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Emerging Issue Value: {ExampleBirthRecord.EmergingIssue8_1}");</para>
        /// </example>
        [Property("Emerging Issue Field Length 8 Number 1", Property.Types.String, "Emerging Issues", "8-Byte Field 1", true, VR.IGURL.EmergingIssues, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='emergingissues')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string EmergingIssue8_1
        {
            get
            {
                return GetEmergingIssue("EmergingIssue8_1");
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetEmergingIssue("EmergingIssue8_1", value);
                }
            }
        }

        /// <summary>Emerging Issue Field Length 8 Number 2</summary>
        /// <value>the emerging issue value</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EmergingIssue8_2 = "XXXXXXXX";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Emerging Issue Value: {ExampleBirthRecord.EmergingIssue8_2}");</para>
        /// </example>
        [Property("Emerging Issue Field Length 8 Number 2", Property.Types.String, "Emerging Issues", "8-Byte Field 2", true, VR.IGURL.EmergingIssues, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='emergingissues')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string EmergingIssue8_2
        {
            get
            {
                return GetEmergingIssue("EmergingIssue8_2");
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetEmergingIssue("EmergingIssue8_2", value);
                }
            }
        }

        /// <summary>Emerging Issue Field Length 8 Number 3</summary>
        /// <value>the emerging issue value</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EmergingIssue8_3 = "XXXXXXXX";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Emerging Issue Value: {ExampleBirthRecord.EmergingIssue8_3}");</para>
        /// </example>
        [Property("Emerging Issue Field Length 8 Number 3", Property.Types.String, "Emerging Issues", "8-Byte Field 3", true, VR.IGURL.EmergingIssues, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='emergingissues')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string EmergingIssue8_3
        {
            get
            {
                return GetEmergingIssue("EmergingIssue8_3");
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetEmergingIssue("EmergingIssue8_3", value);
                }
            }
        }

        /// <summary>Emerging Issue Field Length 20</summary>
        /// <value>the emerging issue value</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EmergingIssue20 = "XXXXXXXXXXXXXXXXXXXX";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Emerging Issue Value: {ExampleBirthRecord.EmergingIssue20}");</para>
        /// </example>
        [Property("Emerging Issue Field Length 20", Property.Types.String, "Emerging Issues", "20-Byte Field", true, VR.IGURL.EmergingIssues, false, 50)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='emergingissues')", "")]
        [FHIRSubject(FHIRSubject.Subject.Newborn)]
        public string EmergingIssue20
        {
            get
            {
                return GetEmergingIssue("EmergingIssue20");
            }
            set
            {
                if(!String.IsNullOrWhiteSpace(value))
                {
                    SetEmergingIssue("EmergingIssue20", value);
                }
            }
        }
    }
}

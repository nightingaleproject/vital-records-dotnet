using System;
using System.Linq;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using VR;
using Hl7.Fhir.Support;

// BirthRecord_submissionProperties.cs
// These fields are used primarily for submitting birth records to NCHS. 

namespace BFDR
{
    /// <summary>Class <c>BirthRecord</c> models a FHIR Birth and Fetal Death Reporting (BFDR) Birth
    /// Record. This class was designed to help consume and produce birth records that follow the
    /// HL7 FHIR Birth and Fetal Death Reporting Implementation Guide, as described at:
    /// TODO add link to BFDR IG
    /// </summary>
    public partial class BirthRecord
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
        [Property("Certificate Number", Property.Types.String, "Birth Certification", "Birth Certificate Number.", true, IGURL.CertificateNumber, true, 3)]
        [FHIRPath("Bundle", "identifier")]
        public string CertificateNumber
        {
            get
            {
                if (Bundle?.Identifier?.Extension != null)
                {
                    Extension ext = Bundle.Identifier.Extension.Find(ex => ex.Url == ExtensionURL.CertificateNumber);
                    if (ext?.Value != null)
                    {
                        return Convert.ToString(ext.Value);
                    }
                }
                return null;
            }
            set
            {
                Bundle.Identifier.Extension.RemoveAll(ex => ex.Url == ExtensionURL.CertificateNumber);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Extension ext = new Extension(ExtensionURL.CertificateNumber, new FhirString(value));
                    Bundle.Identifier.Extension.Add(ext);
                    UpdateBirthRecordIdentifier();
                }
            }
        }

        /// <summary>Update the bundle identifier from the component fields.</summary>
        private void UpdateBirthRecordIdentifier()
        {
            uint certificateNumber = 0;
            if (CertificateNumber != null)
            {
                UInt32.TryParse(CertificateNumber, out certificateNumber);
            }
            uint birthYear = 0;
            if (this.BirthYear != null)
            {
                birthYear = (uint)this.BirthYear;
            }
            String jurisdictionId = this.BirthLocationJurisdiction;
            if (jurisdictionId == null || jurisdictionId.Trim().Length < 2)
            {
                jurisdictionId = "XX";
            }
            else
            {
                jurisdictionId = jurisdictionId.Trim().Substring(0, 2).ToUpper();
            }
            this.BirthRecordIdentifier = $"{birthYear:D4}{jurisdictionId}{certificateNumber:D6}";

        }

        /// <summary>Birth Record Bundle Identifier, NCHS identifier.</summary>
        /// <value>a record bundle identification string, e.g., 2022MA000100, derived from year of birth, jurisdiction of birth, and certificate number</value>
        /// <example>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"NCHS identifier: {ExampleBirthRecord.BirthRecordIdentifier}");</para>
        /// </example>
        [Property("Birth Record Identifier", Property.Types.String, "Birth Certification", "Birth Record identifier.", true, IGURL.CertificateNumber, false, 4)]
        [FHIRPath("Bundle", "identifier")]
        public string BirthRecordIdentifier
        {
            get
            {
                if (Bundle != null && Bundle.Identifier != null)
                {
                    return Bundle.Identifier.Value;
                }
                return null;
            }
            // The setter is private because the value is derived so should never be set directly
            private set
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
        [Property("State Local Identifier1", Property.Types.String, "Birth Certification", "State Local Identifier.", true, ProfileURL.CompositionProviderLiveBirthReport, true, 5)]
        [FHIRPath("Bundle", "identifier")]
        public string StateLocalIdentifier1
        {
            get
            {
                if (Bundle?.Identifier?.Extension != null)
                {
                    Extension ext = Bundle.Identifier.Extension.Find(ex => ex.Url == ExtensionURL.AuxiliaryStateIdentifier1);
                    if (ext?.Value != null)
                    {
                        return Convert.ToString(ext.Value);
                    }
                }
                return null;
            }
            set
            {
                Bundle.Identifier.Extension.RemoveAll(ex => ex.Url == ExtensionURL.AuxiliaryStateIdentifier1);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Extension ext = new Extension(ExtensionURL.AuxiliaryStateIdentifier1, new FhirString(value));
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
                if (Child == null || Child.BirthDateElement == null)
                {
                    return null;
                }
                // First check for a date element in the patient.birthDate PatientBirthTime extension.
                if (Child.BirthDateElement.Extension.Any(ext => ext.Url == VR.ExtensionURL.PatientBirthTime))
                {
                    FhirDateTime fhirDateTimeValue = (FhirDateTime) Child.BirthDateElement.GetExtension(VR.ExtensionURL.PatientBirthTime).Value;
                    return DateTimeOffset.Parse(fhirDateTimeValue.Value).Year;
                }
                // If it's not there, check for a PartialDateTime.
                return GetDateFragmentOrPartialDate(Child.BirthDateElement, VR.ExtensionURL.PartialDateTimeYearVR);
            }
            set
            {
                if (value == null)
                {
                    return;
                    // Check for valid year values (0-9999)?
                }
                if (Child.BirthDateElement == null)
                {
                    AddBirthDateToChild();
                }
                // If the birthDate and PatientBirthDate elements have date data, add the day to them.
                if (Child.BirthDateElement.Extension.Any(ext => ext.Url == VR.ExtensionURL.PatientBirthTime))
                {
                    Extension patientBirthTime = Child.BirthDateElement.GetExtension(VR.ExtensionURL.PatientBirthTime);
                    FhirDateTime fhirPatientBirthTime = (FhirDateTime) patientBirthTime.Value;
                    DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(fhirPatientBirthTime.Value);
                    FhirDateTime updatedBirthDate = new FhirDateTime( (int)value, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Offset);
                    // Need to test that this does not include the time in the birthDate value string.
                    Child.BirthDate = Child.BirthDateElement.Value;
                    return;
                }
                // If adding this date element will make the date complete, then wipe the PartialDateTime and build the birthDate field.
                if (BirthDay != null && BirthMonth != null)
                {
                    DateTime date = new DateTime((int) value, (int) BirthMonth, (int) BirthDay);
                    SetNewlyCompletedDate(date);
                    return;
                }
                // If this date element will not make the date complete, then add the day to a PartialDateTime exension.
                CreateAndSetPartialDate(Child.BirthDateElement, VR.ExtensionURL.PartialDateTimeYearVR, value);
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
            Child.BirthDateElement.RemoveExtension(VRExtensionURLs.PartialDateTimeVR);
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
        protected override string PartialDateTimeUrl => VRExtensionURLs.PartialDateTimeVR;

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
                if (Child == null || Child.BirthDateElement == null)
                {
                    return null;
                }
                // First check for a time in the patient.birthDate extension.
                if (Child.BirthDateElement.Extension.Any(ext => ext.Url == VR.ExtensionURL.PatientBirthTime))
                {
                    FhirDateTime fhirDateTimeValue = (FhirDateTime) Child.BirthDateElement.GetExtension(VR.ExtensionURL.PatientBirthTime).Value;
                    return DateTimeOffset.Parse(fhirDateTimeValue.Value).Month;
                }
                // If it's not there, check for a PartialDateTime.
                return GetDateFragmentOrPartialDate(Child.BirthDateElement, VR.ExtensionURL.PartialDateTimeMonthVR);
            }
            set
            {
                if (value == null)
                {
                    return;
                    // Check for valid month values (1-12)?
                }
                if (Child.BirthDateElement == null)
                {
                    AddBirthDateToChild();
                }
                // If the birthDate and PatientBirthDate elements have date data, add the day to them.
                if (Child.BirthDateElement.Extension.Any(ext => ext.Url == VR.ExtensionURL.PatientBirthTime))
                {
                    Extension patientBirthTime = Child.BirthDateElement.GetExtension(VR.ExtensionURL.PatientBirthTime);
                    FhirDateTime fhirPatientBirthTime = (FhirDateTime) patientBirthTime.Value;
                    DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(fhirPatientBirthTime.Value);
                    FhirDateTime updatedBirthDate = new FhirDateTime(dateTimeOffset.Year, (int)value, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Offset);
                    // Need to test that this does not include the time in the birthDate value string.
                    Child.BirthDate = Child.BirthDateElement.Value;
                    return;
                }
                // If adding this date element will make the date complete, then wipe the PartialDateTime and build the birthDate field.
                if (BirthDay != null && BirthYear != null)
                {
                    DateTime date = new DateTime((int) BirthYear, (int) value, (int) BirthDay);
                    SetNewlyCompletedDate(date);
                    return;
                }
                // If not, then add the day to a PartialDateTime exension.
                CreateAndSetPartialDate(Child.BirthDateElement, VR.ExtensionURL.PartialDateTimeMonthVR, value);
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
                if (Child == null || Child.BirthDateElement == null)
                {
                    return null;
                }
                // First check for a time in the patient.birthDate extension.
                if (Child.BirthDateElement.Extension.Any(ext => ext.Url == VR.ExtensionURL.PatientBirthTime))
                {
                    FhirDateTime fhirDateTimeValue = (FhirDateTime) Child.BirthDateElement.GetExtension(VR.ExtensionURL.PatientBirthTime).Value;
                    return DateTimeOffset.Parse(fhirDateTimeValue.Value).Day;
                }
                // If it's not there, check for a PartialDateTime.
                return GetDateFragmentOrPartialDate(Child.BirthDateElement, VR.ExtensionURL.PartialDateTimeDayVR);
            }
            set
            {
                if (value == null)
                {
                    return;
                    // Check for valid day values (0-31)?
                }
                if (Child.BirthDateElement == null)
                {
                    AddBirthDateToChild();
                }
                // If the birthDate and PatientBirthDate elements have date data, add the day to them.
                if (Child.BirthDateElement.Extension.Any(ext => ext.Url == VR.ExtensionURL.PatientBirthTime))
                {
                    Extension patientBirthTime = Child.BirthDateElement.GetExtension(VR.ExtensionURL.PatientBirthTime);
                    FhirDateTime fhirPatientBirthTime = (FhirDateTime) patientBirthTime.Value;
                    DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(fhirPatientBirthTime.Value);
                    FhirDateTime updatedBirthDate = new FhirDateTime(dateTimeOffset.Year, dateTimeOffset.Month, (int)value, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Offset);
                    // Need to test that this does not include the time in the birthDate value string.
                    Child.BirthDate = Child.BirthDateElement.Value;
                    return;
                }
                // If adding this date element will make the date complete, then wipe the PartialDateTime and build the birthDate field.
                if (BirthYear != null && BirthMonth != null)
                {
                    DateTime date = new DateTime((int) BirthYear, (int) BirthMonth, (int) value);
                    SetNewlyCompletedDate(date);
                    return;
                }
                // If not, then add the day to a PartialDateTime exension.
                CreateAndSetPartialDate(Child.BirthDateElement, VR.ExtensionURL.PartialDateTimeDayVR, value);
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
                    FhirDateTime dateTime = (FhirDateTime) Child.BirthDateElement.Extension.Find(ext => ext.Url == VR.ExtensionURL.PatientBirthTime).Value;
                    return GetTimeFragment(dateTime);
                }
                // If it's not there, check for a PartialDateTime.
                if (Child.BirthDateElement.Extension.Any(ext => ext.Url == VRExtensionURLs.PartialDateTimeVR))
                {
                    Extension partialDateTime = Child.BirthDateElement.Extension.Find(ext => ext.Url == VRExtensionURLs.PartialDateTimeVR);
                    if (partialDateTime.Extension.Any(ext => ext.Url == VR.ExtensionURL.PartialDateTimeTimeVR))
                    {
                        Extension partialTime = partialDateTime.GetExtension(VR.ExtensionURL.PartialDateTimeTimeVR);
                        if (partialTime.Extension.FirstOrDefault()?.Url == OtherExtensionURL.DataAbsentReason)
                        {
                            return partialTime.Extension.FirstOrDefault().Value.ToString();
                        }
                        return partialTime.Value.ToString();
                    }
                }
                return null;
            }
            set
            {
                // If both the date and time of birth are known and complete, then the .birthDate field should include both the date (as a YYYY-MM-DD string) and PatientBirthTime (with both date and string as a Fhir dateTime).
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                if (Child.BirthDateElement == null)
                {
                    AddBirthDateToChild();
                }
                // First, check that this would constitute a complete dateTime, in which case use the PatientBirthTime extension.
                if (BirthYear != null && BirthMonth != null && BirthDay != null)
                {
                    // Parse out the date to recreate a date object with the time.
                    // FhirDateTime dateTime = new FhirDateTime(value);
                    if (value.Length < 8)
                    {
                        value += ":";
                        value = value.PadRight(8, '0');
                    }
                    Time time = new Time(value);
                    FhirDateTime dateTime = new FhirDateTime((int) BirthYear, (int) BirthMonth, (int) BirthDay, FhirTimeHour(time), FhirTimeMin(time), FhirTimeSec(time), TimeSpan.Zero);
                    Child.BirthDateElement.SetExtension(VR.ExtensionURL.PatientBirthTime, dateTime);
                    // Remove extraneous PartialDateTime extension since the dateTime is complete.
                    Child.BirthDateElement.RemoveExtension(VRExtensionURLs.PartialDateTimeVR);
                    return;
                }
                // If this will not be a complete date, then it should be added to the PartialDateTime.
                if (!Child.BirthDateElement.Extension.Any(ext => ext.Url == VRExtensionURLs.PartialDateTimeVR))
                {
                    Child.BirthDateElement.AddExtension(VRExtensionURLs.PartialDateTimeVR, new Extension());
                }
                if (!Child.BirthDateElement.Extension.Find(ext => ext.Url == VRExtensionURLs.PartialDateTimeVR).Extension.Any(ext => ext.Url == VR.ExtensionURL.PartialDateTimeTimeVR))
                {
                    Child.BirthDateElement.Extension.Find(ext => ext.Url == VRExtensionURLs.PartialDateTimeVR).AddExtension(VR.ExtensionURL.PartialDateTimeTimeVR, new Extension());
                }
                SetPartialTime(Child.BirthDateElement.GetExtension(VRExtensionURLs.PartialDateTimeVR), value);
            }
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
                // We support this legacy API entrypoint via the new partial date entrypoints
                if (BirthYear != null && BirthYear != -1 && BirthMonth != null && BirthMonth != -1 && BirthDay != null && BirthDay != -1)
                {
                    Date result = new Date((int)BirthYear, (int)BirthMonth, (int)BirthDay);
                    return result.ToString();
                }
                return null;
            }
            set
            {
                // We support this legacy API entrypoint via the new partial date entrypoints
                DateTimeOffset parsedDate;
                if (DateTimeOffset.TryParse(value, out parsedDate))
                {
                    BirthYear = parsedDate.Year;
                    BirthMonth = parsedDate.Month;
                    BirthDay = parsedDate.Day;
                }
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
                    Extension sex = Child.Extension.Find(ext => ext.Url == VR.OtherExtensionURL.BirthSex);
                    if (sex != null && sex.Value != null)
                    {
                        return sex.Value.ToString();
                    }
                }
                return "";
            }
            set
            {
                Child.Extension.RemoveAll(ext => ext.Url == VR.OtherExtensionURL.BirthSex);
                if (value == null && Child.Extension == null)
                {
                    return;
                }
                Child.AddExtension(VR.OtherExtensionURL.BirthSex, new FhirString(value));
            }
        }

        /// <summary>Child's Sex at Birth Helper.</summary>
        /// <value>The child's sex at time of birth</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthSexHelper = "female;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Sex at Time of Birth: {ExampleBirthRecord.BirthSexHelper}");</para>
        /// </example>
        [Property("Sex At Birth Helper", Property.Types.String, "Child Demographics", "Child's Sex at Birth.", true, VR.IGURL.Child, true, 12)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url='" + OtherExtensionURL.BirthSex + "')", "")]
        public string BirthSexHelper
        {
            // SexHelper is required for the IJE mapping to work since it appends "Helper" to the end of the property name.
            get
            {
                return this.BirthSex;
            }
            set
            {
                this.BirthSex = value;
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
                return Child?.Name?.Find(name => name.Use == HumanName.NameUse.Official)?.Suffix.First();
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
        [Property("Birth Location Jurisdiction", Property.Types.String, "TODO", "Vital Records Jurisdiction of Birth Location (two character jurisdiction code, e.g. CA).", true, VR.IGURL.Child, false, 16)]
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

        /// <summary>Infant's Medical Record Number.</summary>
        /// <value>Infant's Medical Record Number.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.InfantMedicalRecordNumber = "aaabbbcccdddeee";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child's InfantMedicalRecordNumber: {ExampleBirthRecord.InfantMedicalRecordNumber}");</para>
        /// </example>
        [Property("Infant's Medical Record Number", Property.Types.String, "Child Demographics", "Infant's Medical Record Number", true, VR.IGURL.Child, true, 34)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).identifier.where(url='http://terminology.hl7.org/CodeSystem/v2-0203' and type.coding.code='MR').value", "")]
        public string InfantMedicalRecordNumber
        {
            get
            {
                return Child?.Identifier?.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == "http://terminology.hl7.org/CodeSystem/v2-0203" && idCoding.Code == "MR"))?.Value;
            }
            set
            {
                if (Child.Identifier.Any(id => id.Type.Coding.Any(idCoding => idCoding.System == "http://terminology.hl7.org/CodeSystem/v2-0203" && idCoding.Code == "MR")))
                {
                    Child.Identifier.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == "http://terminology.hl7.org/CodeSystem/v2-0203" && idCoding.Code == "MR")).Value = value;
                }
                else
                {
                    Coding coding = new Coding
                    {
                        System = "http://terminology.hl7.org/CodeSystem/v2-0203",
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
        [FHIRPath("Bundle.entry.resource.where($this is Patient).identifier.where(url='http://terminology.hl7.org/CodeSystem/v2-0203' and type.coding.code='MR').value", "")]
        public string MotherMedicalRecordNumber
        {
            get
            {
                return Mother?.Identifier?.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == "http://terminology.hl7.org/CodeSystem/v2-0203" && idCoding.Code == "MR"))?.Value;
            }
            set
            {
                if (Mother.Identifier.Any(id => id.Type.Coding.Any(idCoding => idCoding.System == "http://terminology.hl7.org/CodeSystem/v2-0203" && idCoding.Code == "MR")))
                {
                    Mother.Identifier.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == "http://terminology.hl7.org/CodeSystem/v2-0203" && idCoding.Code == "MR")).Value = value;
                }
                else
                {
                    Coding coding = new Coding
                    {
                        System = "http://terminology.hl7.org/CodeSystem/v2-0203",
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

        /// <summary>Mother's Social Security Number.</summary>
        /// <value>Mother's Social Security Number.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherSocialSecurityNumber = "123456789";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's SocialSecurityNumber: {ExampleBirthRecord.MotherSocialSecurityNumber}");</para>
        /// </example>
        [Property("Mother's Social Security Number", Property.Types.String, "Mother Demographics", "Mother's Social Security Number", true, VR.IGURL.Mother, true, 278)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).identifier.where(url='http://terminology.hl7.org/CodeSystem/v2-0203' and type.coding.code='SS').value", "")]
        public string MotherSocialSecurityNumber
        {
            get
            {
                return Mother?.Identifier?.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == "http://terminology.hl7.org/CodeSystem/v2-0203" && idCoding.Code == "SS"))?.Value;
            }
            set
            {
                if (Mother.Identifier.Any(id => id.Type.Coding.Any(idCoding => idCoding.System == "http://terminology.hl7.org/CodeSystem/v2-0203" && idCoding.Code == "SS")))
                {
                    Mother.Identifier.Find(id => id.Type.Coding.Any(idCoding => idCoding.System == "http://terminology.hl7.org/CodeSystem/v2-0203" && idCoding.Code == "SS")).Value = value;
                }
                else
                {
                    Coding coding = new Coding
                    {
                        System = "http://terminology.hl7.org/CodeSystem/v2-0203",
                        Code = "SS",
                        Display = "Social Security Number"
                    };
                    CodeableConcept socialSecurityNumber = new CodeableConcept();
                    socialSecurityNumber.Coding.Add(coding);
                    Identifier identifier = new Identifier
                    {
                        Type = socialSecurityNumber,
                        Value = value
                    };
                    Mother.Identifier.Add(identifier);
                }
            }
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
                if (String.IsNullOrEmpty(value))
                {
                    PluralityEditFlag = EmptyCodeDict();
                    return;
                }
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("code", value);
                dictionary.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");
                PluralityEditFlag = dictionary;
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
        [Property("No Infections Present During Pregnancy", Property.Types.Bool, "No Infections Present During Pregnancy", 
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
                  "Pregnancy Risk Factors, None", true, IGURL.ObservationNoneOfSpecifiedPregnancyRiskFactors, false, 157)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73775-9", code: VitalRecord.NONE_OF_THE_ABOVE, section: MEDICAL_INFORMATION_SECTION)]
        public bool NoPregnancyRiskFactors
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Eclampsia Hypertension.</summary>
        [Property("Eclampsia Hypertension", Property.Types.Bool, "Pregnancy Risk Factors", 
                  "Pregnancy Risk Factors, Eclampsia Hypertension", true, IGURL.ConditionEclampsiaHypertension, true, 239)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73775-9", code: "15938005", section: MEDICAL_INFORMATION_SECTION)]
        public bool EclampsiaHypertension
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Gestational Diabetes.</summary>
        [Property("Gestational Diabetes", Property.Types.Bool, "Pregnancy Risk Factors", 
                  "Pregnancy Risk Factors, Gestational Diabetes", true, IGURL.ConditionGestationalDiabetes, true, 158)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73775-9", code: "11687002", section: MEDICAL_INFORMATION_SECTION)]
        public bool GestationalDiabetes
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Gestational Hypertension.</summary>
        [Property("Gestational Hypertension", Property.Types.Bool, "Pregnancy Risk Factors", 
                  "Pregnancy Risk Factors, Gestational Hypertension", true, IGURL.ConditionGestationalHypertension, true, 160)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73775-9", code: "48194001", section: MEDICAL_INFORMATION_SECTION)]
        public bool GestationalHypertension
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Prepregnancy Diabetes.</summary>
        [Property("Prepregnancy Diabetes", Property.Types.Bool, "Pregnancy Risk Factors", 
                  "Pregnancy Risk Factors, Prepregnancy Diabetes", true, IGURL.ConditionPrepregnancyDiabetes, true, 157)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73775-9", code: "73211009", section: MEDICAL_INFORMATION_SECTION)]
        public bool PrepregnancyDiabetes
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Prepregnancy Hypertension.</summary>
        [Property("Prepregnancy Hypertension", Property.Types.Bool, "Pregnancy Risk Factors", 
                  "Pregnancy Risk Factors, Prepregnancy Hypertension", true, IGURL.ConditionPrepregnancyHypertension, true, 159)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Condition, categoryCode: "73775-9", code: "38341003", section: MEDICAL_INFORMATION_SECTION)]
        public bool PrepregnancyHypertension
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Previous Cesarean.</summary>
        [Property("Previous Cesarean", Property.Types.Bool, "Pregnancy Risk Factors", 
                  "Pregnancy Risk Factors, Previous Cesarean", true, IGURL.ObservationPreviousCesarean, true, 165)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73775-9", code: "200144004", section: MEDICAL_INFORMATION_SECTION)]
        public bool PreviousCesarean
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Previous Preterm Birth.</summary>
        [Property("Previous Preterm Birth", Property.Types.Bool, "Pregnancy Risk Factors", 
                  "Pregnancy Risk Factors, Previous Preterm Birth", true, IGURL.ObservationPreviousPretermBirth, true, 161)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Observation, categoryCode: "73775-9", code: "161765003", section: MEDICAL_INFORMATION_SECTION)]
        public bool PreviousPretermBirth
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Artificial Insemination.</summary>
        [Property("Artificial Insemination", Property.Types.Bool, "Pregnancy Risk Factors", 
                  "Pregnancy Risk Factors, Artificial Insemination", true, IGURL.ProcedureArtificialInsemination, true, 240)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73775-9", code: "58533008", section: MEDICAL_INFORMATION_SECTION)]
        public bool ArtificialInsemination
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Assisted Fertilization.</summary>
        [Property("Assisted Fertilization", Property.Types.Bool, "Pregnancy Risk Factors", 
                  "Pregnancy Risk Factors, Assisted Fertilization", true, IGURL.ProcedureAssistedFertilization, true, 241)]
        [FHIRPath(fhirType: FHIRPath.FhirType.Procedure, categoryCode: "73775-9", code: "63487001", section: MEDICAL_INFORMATION_SECTION)]
        public bool AssistedFertilization
        {
            get => EntryExists();
            set => UpdateEntry(value);
        }

        /// <summary>Infertility Treatment.</summary>
        [Property("Infertility Treatment", Property.Types.Bool, "Pregnancy Risk Factors", 
                  "Pregnancy Risk Factors, Infertility Treatment", true, IGURL.ProcedureInfertilityTreatment, true, 164)]
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
                    ((Procedure)e.Resource).Category.Coding[0].Code == fhirPath.CategoryCode;
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
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("code", value);
                dictionary.Add("system", CodeSystems.SCT);
                FinalRouteAndMethodOfDelivery = dictionary;
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
                    ((Procedure)e.Resource).Category.Coding[0].Code == fhirPath.CategoryCode &&
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
                    ((Procedure)e.Resource).Category.Coding[0].Code == fhirPath.CategoryCode &&
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
                Date newDate = SetDay(value, Mother.BirthDateElement, MotherBirthYear, MotherBirthMonth);
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
                Date newDate = SetMonth(value, Mother.BirthDateElement, MotherBirthYear, MotherBirthDay);
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
                Date newDate = SetYear(value, Mother.BirthDateElement, MotherBirthMonth, MotherBirthDay);
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
                return this.Mother.BirthDate;
            }
            set
            {
                this.Mother.BirthDateElement = ConvertToDate(value);
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
                Date newDate = SetDay(value, Father.BirthDateElement, FatherBirthYear, FatherBirthMonth);
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
                Date newDate = SetMonth(value, Father.BirthDateElement, FatherBirthYear, FatherBirthDay);
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
                Date newDate = SetYear(value, Father.BirthDateElement, FatherBirthMonth, FatherBirthDay);
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
        [FHIRPath("Bundle.entry.resource.where($this is RelatedPerson).extension.birthDate", "")]// TODO
        public string FatherDateOfBirth
        {
            get
            {
                return this.Father.BirthDate;
            }
            set
            {
                this.Father.BirthDateElement = ConvertToDate(value);
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
        /// <para>Console.WriteLine($"Mother Ethnicity: {ExampleBirthRecord.MotherEthnicity1['display']}");</para>
        /// </example>
        [Property("MotherEthnicity1", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Mother's Ethnicity Hispanic Mexican.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        public Dictionary<string, string> MotherEthnicity1
        {
            get
            {
                if (InputRaceAndEthnicityObsMother != null)
                {
                    Observation.ComponentComponent ethnicity = InputRaceAndEthnicityObsMother.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Mexican);
                    if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (InputRaceAndEthnicityObsMother == null)
                {
                    CreateInputRaceEthnicityObsMother();
                }
                InputRaceAndEthnicityObsMother.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Mexican);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCode, NvssEthnicity.Mexican, NvssEthnicity.MexicanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                InputRaceAndEthnicityObsMother.Component.Add(component);
            }
        }

        /// <summary>Mother's Ethnicity 1 Helper</summary>
        /// <value>Mother's Ethnicity 1.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VRDR.ValueSets.YesNoUnknown.Yes;</para>
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
                if(!String.IsNullOrWhiteSpace(value))
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
        /// <para>Console.WriteLine($"Mother Ethnicity: {ExampleBirthRecord.MotherEthnicity2['display']}");</para>
        /// </example>
        [Property("MotherEthnicity2", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Mother's Ethnicity Hispanic Puerto Rican.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        public Dictionary<string, string> MotherEthnicity2
        {
            get
            {
                if (InputRaceAndEthnicityObsMother != null)
                {
                    Observation.ComponentComponent ethnicity = InputRaceAndEthnicityObsMother.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.PuertoRican);
                    if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (InputRaceAndEthnicityObsMother == null)
                {
                    CreateInputRaceEthnicityObsMother();
                }
                InputRaceAndEthnicityObsMother.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.PuertoRican);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCode, NvssEthnicity.PuertoRican, NvssEthnicity.PuertoRicanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                InputRaceAndEthnicityObsMother.Component.Add(component);
            }
        }

        /// <summary>Mother's Ethnicity 2 Helper</summary>
        /// <value>Mother's Ethnicity 2.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VRDR.ValueSets.YesNoUnknown.Yes;</para>
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
                if(!String.IsNullOrWhiteSpace(value))
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
        /// <para>Console.WriteLine($"Mother Ethnicity: {ExampleBirthRecord.MotherEthnicity3['display']}");</para>
        /// </example>
        [Property("MotherEthnicity3", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Mother's Ethnicity Hispanic Cuban.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        public Dictionary<string, string> MotherEthnicity3
        {
            get
            {
                if (InputRaceAndEthnicityObsMother != null)
                {
                    Observation.ComponentComponent ethnicity = InputRaceAndEthnicityObsMother.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Cuban);
                    if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (InputRaceAndEthnicityObsMother == null)
                {
                    CreateInputRaceEthnicityObsMother();
                }
                InputRaceAndEthnicityObsMother.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Cuban);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCode, NvssEthnicity.Cuban, NvssEthnicity.CubanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                InputRaceAndEthnicityObsMother.Component.Add(component);
            }
        }

        /// <summary>Mother's Ethnicity 3 Helper</summary>
        /// <value>Mother's Ethnicity 3.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VRDR.ValueSets.YesNoUnknown.Yes;</para>
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
                if(!String.IsNullOrWhiteSpace(value))
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
        /// <para>Console.WriteLine($"Mother Ethnicity: {ExampleBirthRecord.MotherEthnicity4['display']}");</para>
        /// </example>
        [Property("MotherEthnicity4", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Mother's Ethnicity Hispanic Other.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        public Dictionary<string, string> MotherEthnicity4
        {
            get
            {
                if (InputRaceAndEthnicityObsMother != null)
                {
                    Observation.ComponentComponent ethnicity = InputRaceAndEthnicityObsMother.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Other);
                    if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (InputRaceAndEthnicityObsMother == null)
                {
                    CreateInputRaceEthnicityObsMother();
                }
                InputRaceAndEthnicityObsMother.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Other);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCode, NvssEthnicity.Other, NvssEthnicity.OtherDisplay, null);
                component.Value = DictToCodeableConcept(value);
                InputRaceAndEthnicityObsMother.Component.Add(component);
            }
        }

        /// <summary>Mother's Ethnicity 4 Helper</summary>
        /// <value>Mother's Ethnicity 4.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VRDR.ValueSets.YesNoUnknown.Yes;</para>
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
                if(!String.IsNullOrWhiteSpace(value))
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
        /// <para>Console.WriteLine($"Ethnicity: {ExampleBirthRecord.MotherEthnicityLiteral['display']}");</para>
        /// </example>
        [Property("MotherEthnicityLiteral", Property.Types.String, "Race and Ethnicity Profiles", "Mother's Ethnicity Literal.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("ethnicity", "The literal string to describe ethnicity.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityMother')", "")]
        public string MotherEthnicityLiteral
        {
            get
            {
                if (InputRaceAndEthnicityObsMother != null)
                {
                    Observation.ComponentComponent ethnicity = InputRaceAndEthnicityObsMother.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Literal);
                    if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as FhirString != null)
                    {
                        return ethnicity.Value.ToString();
                    }
                }
                return null;
            }
            set
            {
                if (InputRaceAndEthnicityObsMother == null)
                {
                    CreateInputRaceEthnicityObsMother();
                }
                InputRaceAndEthnicityObsMother.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Literal);
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCode, NvssEthnicity.Literal, NvssEthnicity.LiteralDisplay, null);
                component.Value = new FhirString(value);
                InputRaceAndEthnicityObsMother.Component.Add(component);
            }
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
        public Tuple<string, string>[] MotherRace
        {
            get
            {
                // filter the boolean race values
                var booleanRaceCodes = NvssRace.GetBooleanRaceCodes();
                List<string> raceCodes = booleanRaceCodes.Concat(NvssRace.GetLiteralRaceCodes()).ToList();

                var races = new List<Tuple<string, string>>() { };

                if (InputRaceAndEthnicityObsMother == null)
                {
                    return races.ToArray();
                }
                foreach (string raceCode in raceCodes)
                {
                    Observation.ComponentComponent component = InputRaceAndEthnicityObsMother.Component.Where(c => c.Code.Coding[0].Code == raceCode).FirstOrDefault();
                    if (component != null)
                    {
                        // convert boolean race codes to strings
                        if (booleanRaceCodes.Contains(raceCode))
                        {
                            if (component.Value == null) {
                              // If there is no value given, set the race to blank.
                              var race = Tuple.Create(raceCode, "");
                              races.Add(race);
                              continue;
                            }

                            // Todo Find conversion from FhirBoolean to bool
                            string raceBool = ((FhirBoolean)component.Value).ToString();

                            if (Convert.ToBoolean(raceBool))
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
                if (InputRaceAndEthnicityObsMother == null)
                {
                    CreateInputRaceEthnicityObsMother();
                }
                var booleanRaceCodes = NvssRace.GetBooleanRaceCodes();
                var literalRaceCodes = NvssRace.GetLiteralRaceCodes();
                foreach (Tuple<string, string> element in value)
                {
                    InputRaceAndEthnicityObsMother.Component.RemoveAll(c => c.Code.Coding[0].Code == element.Item1);
                    Observation.ComponentComponent component = new Observation.ComponentComponent();
                    String displayValue = NvssRace.GetDisplayValueForCode(element.Item1);
                    component.Code = new CodeableConcept(CodeSystems.ComponentCode, element.Item1, displayValue, null);
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
                    InputRaceAndEthnicityObsMother.Component.Add(component);
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
        /// <para>Console.WriteLine($"Father Ethnicity: {ExampleBirthRecord.FatherEthnicity1['display']}");</para>
        /// </example>
        [Property("FatherEthnicity1", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Father's Ethnicity Hispanic Mexican.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        public Dictionary<string, string> FatherEthnicity1
        {
            get
            {
                if (InputRaceAndEthnicityObsFather != null)
                {
                    Observation.ComponentComponent ethnicity = InputRaceAndEthnicityObsFather.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Mexican);
                    if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (InputRaceAndEthnicityObsFather == null)
                {
                    CreateInputRaceEthnicityObsFather();
                }
                InputRaceAndEthnicityObsFather.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Mexican);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCode, NvssEthnicity.Mexican, NvssEthnicity.MexicanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                InputRaceAndEthnicityObsFather.Component.Add(component);
            }
        }

        /// <summary>Father's Ethnicity 1 Helper</summary>
        /// <value>Father's Ethnicity 1.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VRDR.ValueSets.YesNoUnknown.Yes;</para>
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
                if(!String.IsNullOrWhiteSpace(value))
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
        /// <para>Console.WriteLine($"Father Ethnicity: {ExampleBirthRecord.FatherEthnicity2['display']}");</para>
        /// </example>
        [Property("FatherEthnicity2", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Father's Ethnicity Hispanic PuertoRican.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        public Dictionary<string, string> FatherEthnicity2
        {
            get
            {
                if (InputRaceAndEthnicityObsFather != null)
                {
                    Observation.ComponentComponent ethnicity = InputRaceAndEthnicityObsFather.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.PuertoRican);
                    if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (InputRaceAndEthnicityObsFather == null)
                {
                    CreateInputRaceEthnicityObsFather();
                }
                InputRaceAndEthnicityObsFather.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.PuertoRican);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCode, NvssEthnicity.PuertoRican, NvssEthnicity.PuertoRicanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                InputRaceAndEthnicityObsFather.Component.Add(component);
            }
        }

        /// <summary>Father's Ethnicity 2 Helper</summary>
        /// <value>Father's Ethnicity 2.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VRDR.ValueSets.YesNoUnknown.Yes;</para>
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
                if(!String.IsNullOrWhiteSpace(value))
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
        /// <para>Console.WriteLine($"Father Ethnicity: {ExampleBirthRecord.FatherEthnicity3['display']}");</para>
        /// </example>
        [Property("FatherEthnicity3", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Father's Ethnicity Hispanic Cuban.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        public Dictionary<string, string> FatherEthnicity3
        {
            get
            {
                if (InputRaceAndEthnicityObsFather != null)
                {
                    Observation.ComponentComponent ethnicity = InputRaceAndEthnicityObsFather.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Cuban);
                    if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (InputRaceAndEthnicityObsFather == null)
                {
                    CreateInputRaceEthnicityObsFather();
                }
                InputRaceAndEthnicityObsFather.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Cuban);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCode, NvssEthnicity.Cuban, NvssEthnicity.CubanDisplay, null);
                component.Value = DictToCodeableConcept(value);
                InputRaceAndEthnicityObsFather.Component.Add(component);
            }
        }

        /// <summary>Father's Ethnicity 3 Helper</summary>
        /// <value>Father's Ethnicity 3.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VRDR.ValueSets.YesNoUnknown.Yes;</para>
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
                if(!String.IsNullOrWhiteSpace(value))
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
        /// <para>Console.WriteLine($"Father Ethnicity: {ExampleBirthRecord.FatherEthnicity4['display']}");</para>
        /// </example>
        [Property("FatherEthnicity4", Property.Types.Dictionary, "Race and Ethnicity Profiles", "Father's Ethnicity Hispanic Other.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        public Dictionary<string, string> FatherEthnicity4
        {
            get
            {
                if (InputRaceAndEthnicityObsFather != null)
                {
                    Observation.ComponentComponent ethnicity = InputRaceAndEthnicityObsFather.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Other);
                    if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as CodeableConcept != null)
                    {
                        return CodeableConceptToDict((CodeableConcept)ethnicity.Value);
                    }
                }
                return EmptyCodeableDict();
            }
            set
            {
                if (InputRaceAndEthnicityObsFather == null)
                {
                    CreateInputRaceEthnicityObsFather();
                }
                InputRaceAndEthnicityObsFather.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Other);
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCode, NvssEthnicity.Other, NvssEthnicity.OtherDisplay, null);
                component.Value = DictToCodeableConcept(value);
                InputRaceAndEthnicityObsFather.Component.Add(component);
            }
        }

        /// <summary>Father's Ethnicity 4 Helper</summary>
        /// <value>Father's Ethnicity 4.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.EthnicityLevel = VRDR.ValueSets.YesNoUnknown.Yes;</para>
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
                if(!String.IsNullOrWhiteSpace(value))
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
        /// <para>Console.WriteLine($"Ethnicity: {ExampleBirthRecord.EthnicityLiteral['display']}");</para>
        /// </example>
        [Property("FatherEthnicityLiteral", Property.Types.String, "Race and Ethnicity Profiles", "Father's Ethnicity Literal.", true, VR.IGURL.InputRaceAndEthnicity, false, 34)]
        [PropertyParam("ethnicity", "The literal string to describe ethnicity.")]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='inputraceandethnicityFather')", "")]
        public string FatherEthnicityLiteral
        {
            get
            {
                if (InputRaceAndEthnicityObsFather != null)
                {
                    Observation.ComponentComponent ethnicity = InputRaceAndEthnicityObsFather.Component.FirstOrDefault(c => c.Code.Coding[0].Code == NvssEthnicity.Literal);
                    if (ethnicity != null && ethnicity.Value != null && ethnicity.Value as FhirString != null)
                    {
                        return ethnicity.Value.ToString();
                    }
                }
                return null;
            }
            set
            {
                if (InputRaceAndEthnicityObsFather == null)
                {
                    CreateInputRaceEthnicityObsFather();
                }
                InputRaceAndEthnicityObsFather.Component.RemoveAll(c => c.Code.Coding[0].Code == NvssEthnicity.Literal);
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                Observation.ComponentComponent component = new Observation.ComponentComponent();
                component.Code = new CodeableConcept(CodeSystems.ComponentCode, NvssEthnicity.Literal, NvssEthnicity.LiteralDisplay, null);
                component.Value = new FhirString(value);
                InputRaceAndEthnicityObsFather.Component.Add(component);
            }
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
        public Tuple<string, string>[] FatherRace
        {
            get
            {
                // filter the boolean race values
                var booleanRaceCodes = NvssRace.GetBooleanRaceCodes();
                List<string> raceCodes = booleanRaceCodes.Concat(NvssRace.GetLiteralRaceCodes()).ToList();

                var races = new List<Tuple<string, string>>() { };

                if (InputRaceAndEthnicityObsFather == null)
                {
                    return races.ToArray();
                }
                foreach (string raceCode in raceCodes)
                {
                    Observation.ComponentComponent component = InputRaceAndEthnicityObsFather.Component.Where(c => c.Code.Coding[0].Code == raceCode).FirstOrDefault();
                    if (component != null)
                    {
                        // convert boolean race codes to strings
                        if (booleanRaceCodes.Contains(raceCode))
                        {
                            if (component.Value == null) {
                              // If there is no value given, set the race to blank.
                              var race = Tuple.Create(raceCode, "");
                              races.Add(race);
                              continue;
                            }

                            // Todo Find conversion from FhirBoolean to bool
                            string raceBool = ((FhirBoolean)component.Value).ToString();

                            if (Convert.ToBoolean(raceBool))
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
                if (InputRaceAndEthnicityObsFather == null)
                {
                    CreateInputRaceEthnicityObsFather();
                }
                var booleanRaceCodes = NvssRace.GetBooleanRaceCodes();
                var literalRaceCodes = NvssRace.GetLiteralRaceCodes();
                foreach (Tuple<string, string> element in value)
                {
                    InputRaceAndEthnicityObsFather.Component.RemoveAll(c => c.Code.Coding[0].Code == element.Item1);
                    Observation.ComponentComponent component = new Observation.ComponentComponent();
                    String displayValue = NvssRace.GetDisplayValueForCode(element.Item1);
                    component.Code = new CodeableConcept(CodeSystems.ComponentCode, element.Item1, displayValue, null);
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
                    InputRaceAndEthnicityObsFather.Component.Add(component);
                }

            }
        }
    }
}
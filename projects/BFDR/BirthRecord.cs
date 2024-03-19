using System;
using System.Linq;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using VR;


namespace BFDR
{
    public partial class BirthRecord : BirthOrFetalDeathRecord
    {
        public BirthRecord() : base() {}
        public BirthRecord(string record, bool permissive = false) : base(record, permissive) {}
        public BirthRecord(Bundle bundle) : base(bundle) {}

        protected override uint? GetYear()
        {
            return (uint?)this.BirthYear;
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
                Date newDate = SetYear(value, Child.BirthDateElement, BirthMonth, BirthDay);
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
                Date newDate = SetMonth(value, Child.BirthDateElement, BirthYear, BirthDay);
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
                Date newDate = SetDay(value, Child.BirthDateElement, BirthYear, BirthMonth);
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
                return this.Child.BirthDate;
            }
            set
            {
                string time = this.BirthTime;
                this.Child.BirthDateElement = ConvertToDate(value);
                this.BirthTime = time;
            }
        }
    }
}

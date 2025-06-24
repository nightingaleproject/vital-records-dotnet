using System;
using System.Collections.Generic;
using System.Linq;
using VR;

namespace BFDR
{
    /// <summary>Base class for all natality IJE converter classes</summary>
    public abstract class IJENatality : IJE
    {
        /// <summary>Default constructor</summary>
        public IJENatality() {}

        /// <summary>Constructor that takes a <c>NatalityRecord</c>.</summary>
        public IJENatality(NatalityRecord record, bool validate = true) {}

        /// <summary>Constructor that takes an IJE string</summary>
        public IJENatality(string ije, bool validate = true) {}

        /// <summary>FHIR based natality record.</summary>
        protected abstract NatalityRecord NatalityRecord
        {
            get;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Class helper methods for getting and settings IJE fields.
        //
        /////////////////////////////////////////////////////////////////////////////////
        

        ///
        /// <summary>Sets FirstName.</summary>
        /// 
        protected string[] Update_FirstName(string value, string[] givenNames)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                givenNames = new string[] { value.Trim() };
            }
            return givenNames;
        }
        ///
        /// <summary>Sets Middle Name.</summary>
        /// 
        protected string[] Update_MiddleName(string value, string[] givenNames, string ijeFirstName)
        {
          if (!String.IsNullOrWhiteSpace(value))
          {
              if (String.IsNullOrWhiteSpace(ijeFirstName)) throw new ArgumentException("Middle name cannot be set before first name");
                  if (givenNames != null)
                  {
                      List<string> names = givenNames.ToList();
                      if (names.Count() > 1) names[1] = value.Trim(); else names.Add(value.Trim());
                      return names.ToArray();
                  }
              }
          return givenNames;
        }

        ///
        /// <summary>Checks if the given race exists in the record for Mother.</summary>
        /// 
        protected string Get_MotherRace(string name)
        {
            Tuple<string, string>[] raceStatus = NatalityRecord.MotherRace.ToArray();

            Tuple<string, string> raceTuple = Array.Find(raceStatus, element => element.Item1 == name);
            if (raceTuple != null)
            {
                return (raceTuple.Item2).Trim();
            }
            return "";
        }

        /// <summary>Adds the given race to the record for Mother.</summary>
        protected void Set_MotherRace(string name, string value)
        {
            value = value.Trim();
            List<Tuple<string, string>> raceStatus = NatalityRecord.MotherRace.ToList();
            raceStatus.Add(Tuple.Create(name, value));
            NatalityRecord.MotherRace = raceStatus.Distinct().ToArray();
        }

        /// <summary>Checks if the given race exists in the record for Father.</summary>
        protected string Get_FatherRace(string name)
        {
            Tuple<string, string>[] raceStatus = NatalityRecord.FatherRace.ToArray();

            Tuple<string, string> raceTuple = Array.Find(raceStatus, element => element.Item1 == name);
            if (raceTuple != null)
            {
                return (raceTuple.Item2).Trim();
            }
            return "";
        }

        /// <summary>Adds the given race to the record for Father.</summary>
        protected void Set_FatherRace(string name, string value)
        {
            value = value.Trim();
            List<Tuple<string, string>> raceStatus = NatalityRecord.FatherRace.ToList();
            raceStatus.Add(Tuple.Create(name, value));
            NatalityRecord.FatherRace = raceStatus.Distinct().ToArray();
        }

        /// <summary>
        /// Creates and returns a new date string based on the given string and year value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        protected static string AddYear(string value, string date)
        {
            // When setting the date elements via IJE, year MUST be set first, then month, then day to maintain a consistenly valid date.
            // Otherwise, an error will throw.
            VitalRecord.ParseDateElements(date, out _, out int? month, out int? day);
            if (String.IsNullOrWhiteSpace(value))
            {
                // Silently skips empty or white space IJE values.
                return "";
            }
            if (value == "9999")
            {
                throw new System.FormatException("Birthdate IJE unknowns for this field in the format '9999' are not supported by this library.");
            }
            if (value.Length != 4)
            {
                throw new ArgumentException($"Setting IJE Year must be exactly 4 digits. Given: {value}");
            }
            if (month != null && day != null)
            {
                return new Hl7.Fhir.Model.Date(int.Parse(value), (int)month, (int)day).ToString();
            }
            if (month != null)
            {
                return new Hl7.Fhir.Model.Date(int.Parse(value), (int)month).ToString();
            }
            try
            {
                return new Hl7.Fhir.Model.Date(int.Parse(value)).ToString();
            }
            catch (System.FormatException e)
            {
                throw new FormatException($"The given year value is not in a format supported by FHIR or is outside of the supported range. Given '{value}'. {e.StackTrace}");
            }
        }

        /// <summary>
        /// Creates and returns a new date string based on the given string and month value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected static string AddMonth(string value, string date)
        {
            VitalRecord.ParseDateElements(date, out int? year, out _, out int? day);
            if (String.IsNullOrWhiteSpace(value)) {
                // Silently skips empty or white space IJE values.
                return null;
            }
            if (value == "99")
            {
                throw new System.FormatException("Birthdate IJE unknowns for this field in the format '99' are not supported by this library.");
            }
            if (value.Length != 2)
            {
                throw new ArgumentException($"Setting IJE Month must be exactly 2 digits. Given: {value}");
            }
            if (year == null)
            {
                throw new ArgumentException($"The year must be set before month data can be set. Currently set year: '{year}', day: '{day}'");
            }
            if (day != null)
            {
                return new Hl7.Fhir.Model.Date((int)year, int.Parse(value), (int)day).ToString();
            }
            return new Hl7.Fhir.Model.Date((int)year, int.Parse(value)).ToString();
        }

        /// <summary>
        /// Creates and returns a new date string based on the given string and day value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected static string AddDay(string value, string date)
        {
            VitalRecord.ParseDateElements(date, out int? year, out int? month, out _);
            if (String.IsNullOrWhiteSpace(value))
            {
                // Silently skips empty or white space IJE values.
                return "";
            }
            if (value.Length != 2)
            {
                throw new ArgumentException($"Setting IJE Day must be exactly 2 digits. Given: {value}");
            }
            if (value == "99")
            {
                throw new System.FormatException("Birthdate IJE unknowns for this field in the format '99' are not supported by this library.");
            }
            if (year == null || month == null)
            {
                throw new ArgumentException($"The year and month must be set before day data can be set. Currently set year: '{year}', month: '{month}'");
            }
            return new Hl7.Fhir.Model.Date((int)year, (int)month, int.Parse(value)).ToString();
        }

        protected static string GetYearIJEFormatted(string date)
        {
            VitalRecord.ParseDateElements(date, out int? year, out _, out _);
            return year == null || year <= 0 ? "".PadLeft(4, ' ') : year.ToString().PadLeft(4, '0');
        }

        protected static string GetMonthIJEFormatted(string date)
        {
            VitalRecord.ParseDateElements(date, out _, out int? month, out _);
            return month == null || month <= 0 ? "".PadLeft(2, ' ') : month.ToString().PadLeft(2, '0');
        }

        protected static string GetDayIJEFormatted(string date)
        {
            VitalRecord.ParseDateElements(date, out _, out _, out int? day);
            return day == null || day <= 0 ? "".PadLeft(2, ' ') : day.ToString().PadLeft(2, '0');
        }
        
        /// <summary>
        /// Creates and returns a new datetime string based on the given string time value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected static string AddTime(string value, string date)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            VitalRecord.ParseDateElements(date, out int? year, out int? month, out int? day);
            if (year == null || month == null || day == null)
            {
                throw new ArgumentException($"A complete date (year, month, and day) must be set before time data can be set. Currently set year: '{year}', month: '{month}', day: '{day}'");
            }
            if (value.Length != 4)
            {
                throw new ArgumentException($"Setting IJE Times must be exactly 4 digits in the format HHMM. Given: {value}");
            }
            int hours = int.Parse(value.Substring(0, 2));
            int minutes = int.Parse(value.Substring(2));
            if (hours == 99 || minutes == 99)
            {
                return null;
            }
            TimeSpan timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(new DateTime((int)year, (int)month, (int)day));
            return new Hl7.Fhir.Model.FhirDateTime((int)year, (int)month, (int)day, hours, minutes, 0, timeZoneOffset).ToString();
        }

        /// <summary>
        /// Returns the time in IJE HHMM format from the given datetime string.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        protected static string GetTimeIJEFormatted(string dateTime)
        {
            if (String.IsNullOrEmpty(dateTime))
            {
                return null;
            }
            string timeStr = dateTime?.Split('T') is string[] parts && parts.Length > 1 ? parts[1] : null;
            if (timeStr == null)
            {
                return null;
            }
            if (Hl7.Fhir.ElementModel.Types.Time.TryParse(timeStr, out Hl7.Fhir.ElementModel.Types.Time parsedTime))
                {
                    if (parsedTime.Hours == null || parsedTime.Minutes == null)
                    {
                        return null;
                    }
                    TimeSpan timeSpan = new TimeSpan((int)parsedTime.Hours, (int)parsedTime.Minutes, 0);
                    return timeSpan.ToString(@"hhmm");
                }
            return null;
        }
    }
}

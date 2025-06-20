using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using Hl7.Fhir.Model;
namespace VR
{
    /// <summary>Property attribute used to describe a field in the IJE format.</summary>
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class IJEField : System.Attribute
    {
        /// <summary>Field number.</summary>
        public int Field;

        /// <summary>Beginning location.</summary>
        public int Location;

        /// <summary>Field length.</summary>
        public int Length;

        /// <summary>Description of what the field contains.</summary>
        public string Contents;

        /// <summary>Field name.</summary>
        public string Name;

        /// <summary>Priority - lower will be "GET" and "SET" earlier.</summary>
        public int Priority;

        /// <summary>Constructor.</summary>
        public IJEField(int field, int location, int length, string contents, string name, int priority)
        {
            this.Field = field;
            this.Location = location;
            this.Length = length;
            this.Contents = contents;
            this.Name = name;
            this.Priority = priority;
        }
    }

    /// <summary>Base class for all type-specific IJE converter classes</summary>
    public abstract class IJE
    {
        /// <summary>Length of the IJE file</summary>
        protected abstract uint IJELength
        {
            get;
        }

        /// <summary>Validation errors encountered while converting a record</summary>
        protected List<string> validationErrors = new List<string>();

        /// <summary>IJE data lookup helper. Thread-safe singleton!</summary>
        protected IJEData dataLookup = IJEData.Instance;

        /// <summary>Field _void.</summary>
        private string _void;

        /// <summary>FHIR based vital record.</summary>
        protected abstract VitalRecord Record
        {
            get;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Class helper methods for getting and settings IJE fields.
        //
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>Get the type of record.</summary>
        public string GetTypeOfRecord()
        {
            return Record.GetType().ToString();
        }

        /// <summary>Get the length of the IJE string.</summary>
        public uint GetIJELength()
        {
            return IJELength;
        }

        /// <summary>Truncates the given string to the given length.</summary>
        public static string Truncate(string value, int length)
        {
            if (String.IsNullOrWhiteSpace(value) || value.Length <= length)
            {
                return value;
            }
            else
            {
                return value.Substring(0, length);
            }
        }

        /// <summary>Grabs the IJEInfo for a specific IJE field name.</summary>
        protected IJEField FieldInfo(string ijeFieldName)
        {
            return this.GetType().GetProperty(ijeFieldName).GetCustomAttribute<IJEField>();
        }

        /// <summary>Get a value on the VitalRecord that is a numeric string with the option of being set to all 9s on the IJE side and -1 on the
        /// FHIR side to represent'unknown' and blank on the IJE side and null on the FHIR side to represent unspecified</summary>
        protected string NumericAllowingUnknown_Get(string ijeFieldName, string fhirFieldName)
        {
            IJEField info = FieldInfo(ijeFieldName);
            int? value = (int?)Record.GetType().GetProperty(fhirFieldName).GetValue(Record);
            if (value == null) return new String(' ', info.Length); // No value specified
            if (value == -1) return new String('9', info.Length); // Explicitly set to unknown
            string valueString = Convert.ToString(value);
            if (valueString.Length > info.Length)
            {
                validationErrors.Add($"Error: FHIR field {fhirFieldName} contains string '{valueString}' that's not the expected length for IJE field {ijeFieldName} of length {info.Length}");
            }
            return Truncate(valueString, info.Length).PadLeft(info.Length, '0');
        }

        /// <summary>Get a value on the VitalRecord that is a numeric string with the option of being set to all 8s on the IJE side and absent on the
        /// FHIR side to represent'unknown'</summary>
        protected string NumericAllowingUnknownAndAbsence_Get(string ijeFieldName, string fhirFieldName, string obsCodeToRemove)
        {
            IJEField info = FieldInfo(ijeFieldName);
            if (!Record.GetBundle().Entry.Any(e => e.Resource is Observation obs && obs.Code.Coding.Any(coding => coding.Code == obsCodeToRemove)))
            {
                return new String('8', info.Length); // Set to explicitly absent.
            }
            return NumericAllowingUnknown_Get(ijeFieldName, fhirFieldName);
        }

        /// <summary>Set a value on the VitalRecord that is a numeric string with the option of being set to all 9s on the IJE side and -1 on the
        /// FHIR side to represent'unknown' and blank on the IJE side and null on the FHIR side to represent unspecified</summary>
        protected void NumericAllowingUnknown_Set(string ijeFieldName, string fhirFieldName, string value)
        {
            IJEField info = FieldInfo(ijeFieldName);
            if (value == new string(' ', info.Length))
            {
                Record.GetType().GetProperty(fhirFieldName).SetValue(Record, null);
            }
            else if (value == new string('9', info.Length))
            {
                Record.GetType().GetProperty(fhirFieldName).SetValue(Record, -1);
            }
            else
            {
                Record.GetType().GetProperty(fhirFieldName).SetValue(Record, Convert.ToInt32(value));
            }
        }

        /// <summary>Set a value on the VitalRecord that is a numeric string with the option of being set to all 8s on the IJE side and absent on the
        /// FHIR side to represent'unknown'</summary>
        protected void NumericAllowingUnknownAndAbsence_Set(string ijeFieldName, string fhirFieldName, string value, string obsCodeToRemove)
        {
            IJEField info = FieldInfo(ijeFieldName);
            if (value == new string('8', info.Length))
            {
                Record.GetBundle().Entry.RemoveAll(e => e.Resource is Observation obs && obs.Code.Coding.Any(coding => coding.Code == obsCodeToRemove));
                return;
            }
            NumericAllowingUnknown_Set(ijeFieldName, fhirFieldName, value);
        }

        /// <summary>Get the value of the supplied array at position pos, correctly padded to the supplied IJE field length</summary>
        protected string LeftJustifiedValue(string ijeFieldName, string[] values, int pos = 0)
        {
            IJEField info = FieldInfo(ijeFieldName);
            if (values == null || values.Length <= pos)
            {
                return new string(' ', info.Length);
            }
            return Truncate(values[pos], info.Length).PadRight(info.Length, ' ');
        }

        /// <summary>Get a value on the VitalRecord whose IJE type is a left justified string.</summary>
        protected string LeftJustified_Get(string ijeFieldName, string fhirFieldName)
        {
            IJEField info = FieldInfo(ijeFieldName);
            string current = Convert.ToString(Record.GetType().GetProperty(fhirFieldName).GetValue(Record));
            if (current != null)
            {
                if (current.Length > info.Length)
                {
                    validationErrors.Add($"Error: FHIR field {fhirFieldName} contains string '{current}' too long for IJE field {ijeFieldName} of length {info.Length}");
                }
                return Truncate(current, info.Length).PadRight(info.Length, ' ');
            }
            else
            {
                return new String(' ', info.Length);
            }
        }

        /// <summary>Set a value on the VitalRecord whose IJE type is a left justified string.</summary>
        protected void LeftJustified_Set(string ijeFieldName, string fhirFieldName, string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                IJEField info = FieldInfo(ijeFieldName);
                Record.GetType().GetProperty(fhirFieldName).SetValue(Record, value.Trim());
            }
        }

        /// <summary>Get a bool value on the DeathRecord whose IJE type is Y, N, U.</summary>
        protected string Boolean_Get(string ijeFieldName, string fhirFieldName)
        {
            IJEField info = FieldInfo(ijeFieldName);
            object value = Record.GetType().GetProperty(fhirFieldName).GetValue(Record);
            if (value != null)
            {
                if (Convert.ToBoolean(value))
                {
                    return "Y";
                }
                else
                {
                    return "N";
                }
            }
            else
            {
                // if there was no data in the field, return U
                return "U";
            }
        }

        /// <summary>Set a bool value on the DeathRecord whose IJE type is Y, N, U.</summary>
        protected void Boolean_Set(string ijeFieldName, string fhirFieldName, string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                IJEField info = FieldInfo(ijeFieldName);
                if (value.Trim() == "Y")
                {
                    Record.GetType().GetProperty(fhirFieldName).SetValue(Record, true);
                }
                else if (value.Trim() == "N")
                {
                    Record.GetType().GetProperty(fhirFieldName).SetValue(Record, false);
                }
                // U or blank results in no data in FHIR
            }
        }

        /// <summary>Get a value on the DeathRecord that is a time with the option of being set to all 9s on the IJE side and null on the FHIR side to represent null</summary>
        protected string TimeAllowingUnknown_Get(string ijeFieldName, string fhirFieldName)
        {
            IJEField info = FieldInfo(ijeFieldName);
            string timeString = (string)this.Record.GetType().GetProperty(fhirFieldName).GetValue(this.Record);
            if (timeString == null) return new String(' ', info.Length); // No value specified
            if (timeString == "-1") return new String('9', info.Length); // Explicitly set to unknown
            DateTimeOffset parsedTime;
            if (DateTimeOffset.TryParse(timeString, out parsedTime))
            {
                TimeSpan timeSpan = new TimeSpan(0, parsedTime.Hour, parsedTime.Minute, parsedTime.Second);
                return timeSpan.ToString(@"hhmm");
            }
            // No valid date found
            validationErrors.Add($"Error: FHIR field {fhirFieldName} contains value '{timeString}' that cannot be parsed into a time for IJE field {ijeFieldName}");
            return new String(' ', info.Length);
        }

        /// <summary>Set a value on the DeathRecord that is a time with the option of being set to all 9s on the IJE side and null on the FHIR side to represent null</summary>
        protected void TimeAllowingUnknown_Set(string ijeFieldName, string fhirFieldName, string value)
        {
            IJEField info = FieldInfo(ijeFieldName);
            if (value == new string(' ', info.Length))
            {
                this.Record.GetType().GetProperty(fhirFieldName).SetValue(this.Record, null);
            }
            else if (value == new string('9', info.Length))
            {
                this.Record.GetType().GetProperty(fhirFieldName).SetValue(this.Record, "-1");
            }
            else
            {
                if (DateTimeOffset.TryParseExact(value, "HHmm", null, DateTimeStyles.None, out DateTimeOffset parsedTime))
                {
                    TimeSpan timeSpan = new TimeSpan(0, parsedTime.Hour, parsedTime.Minute, 0);
                    this.Record.GetType().GetProperty(fhirFieldName).SetValue(this.Record, timeSpan.ToString(@"hh\:mm\:ss"));
                }
                else
                {
                    validationErrors.Add($"Error: FHIR field {fhirFieldName} value of '{value}' is invalid for IJE field {ijeFieldName}");
                }
            }
        }

        /// <summary>Get a value from the VitalRecord whose IJE type is a right justified, zero filled string.</summary>
        protected string RightJustifiedZeroed_Get(string ijeFieldName, string fhirFieldName)
        {
            IJEField info = FieldInfo(ijeFieldName);
            string current = Convert.ToString(Record.GetType().GetProperty(fhirFieldName).GetValue(Record));
            if (current != null)
            {
                if (current.Length > info.Length)
                {
                    validationErrors.Add($"Error: FHIR field {fhirFieldName} contains string '{current}' too long for IJE field {ijeFieldName} of length {info.Length}");
                    current = current.Substring(current.Length - info.Length);
                }
                return current.PadLeft(info.Length, '0');
            }
            return "".PadLeft(info.Length, '0');
        }

        /// <summary>Set a value on the VitalRecord whose IJE type is a right justified, zero filled string.</summary>
        protected void RightJustifiedZeroed_Set(string ijeFieldName, string fhirFieldName, string value)
        {
            Record.GetType().GetProperty(fhirFieldName).SetValue(Record, value.TrimStart('0'));
        }

        /// <summary>Gets the Void IJE status.</summary>
        protected string Get_Void()
        {
            if (_void == null)
            {
                return "0";
            }
            else
            {
                return _void;
            }
        }

        /// <summary>Sets the Void IJE status.</summary>
        protected void Set_Void(string value)
        {
            if (value.Trim() == "1")
            {
                _void = "1";
            }
            else
            {
                _void = "0";
            }
        }

        /// <summary>Helper that takes an IJE string and builds a corresponding internal <c>VitalRecord</c>.</summary>
        protected void ProcessIJE(string ije, bool validate)
        {
            if (ije == null)
            {
                throw new ArgumentException("IJE string cannot be null.");
            }
            if (ije.Length < IJELength)
            {
                ije = ije.PadRight((int)IJELength, ' ');
            }

            // Loop over every property (these are the fields); Order by priority
            List<PropertyInfo> properties = this.GetType().GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Priority).ToList();
            foreach (PropertyInfo property in properties)
            {
                // Grab the field attributes
                IJEField info = property.GetCustomAttribute<IJEField>();
                // Grab the field value
                string field = ije.Substring(info.Location - 1, info.Length);
                // Set the value on this IJEMortality (and the embedded record)
                property.SetValue(this, field);
            }

            if (validate) CheckForValidationErrors();

        }

        /// <summary>Returns the corresponding <c>VitalRecord</c> for this IJE string.</summary>
        public VitalRecord ToRecord()
        {
            return Record;
        }

        /// <summary>Converts the internal <c>VitalRecord</c> into an IJE string.</summary>
        public override string ToString()
        {
            // Start with empty IJE record
            StringBuilder ije = new StringBuilder(new String(' ', (int)IJELength), (int)IJELength);

            var propertyList = this.GetType().GetProperties();
            // Loop over every property (these are the fields)
            foreach (PropertyInfo property in propertyList)
            {
                // Grab the field value
                string field = Convert.ToString(property.GetValue(this, null));
                // Grab the field attributes
                IJEField info = property.GetCustomAttribute<IJEField>();
                // Be mindful about lengths
                if (field.Length > info.Length)
                {
                    field = field.Substring(0, info.Length);
                }
                // Insert the field value into the record
                ije.Remove(info.Location - 1, field.Length);
                ije.Insert(info.Location - 1, field);
            }
            return ije.ToString();
        }

        /// <summary>Check if there were validation errors.</summary>
        /// <exception cref="ArgumentOutOfRangeException">if there are validation errors</exception>
        protected void CheckForValidationErrors()
        {
            if (validationErrors.Count > 0)
            {
                string errorString = $"Found {validationErrors.Count} validation errors:\n{String.Join("\n", validationErrors)}";
                throw new ArgumentOutOfRangeException(errorString);
            }
        }

        /// <summary>Given a Dictionary mapping FHIR codes to IJE strings and the relevant FHIR and IJE fields pull the value
        /// from the FHIR record object and provide the appropriate IJE string</summary>
        /// <param name="mapping">Dictionary for mapping the desired concept from FHIR to IJE; these dictionaries are defined in Mappings.cs</param>
        /// <param name="fhirField">Name of the FHIR field to get from the record; must have a related Helper property, e.g., EducationLevel must have EducationLevelHelper</param>
        /// <param name="ijeField">Name of the IJE field that the FHIR field content is being placed into</param>
        /// <returns>The IJE value of the field translated from the FHIR value on the record</returns>
        protected string Get_MappingFHIRToIJE(Dictionary<string, string> mapping, string fhirField, string ijeField)
        {
            PropertyInfo helperProperty = Record.GetType().GetProperty($"{fhirField}Helper");
            if (helperProperty == null)
            {
                throw new NullReferenceException($"No helper method found called '{fhirField}Helper'");
            }
            string fhirCode = (string)helperProperty.GetValue(Record);
            if (String.IsNullOrWhiteSpace(fhirCode))
            {
                return "";
            }
            try
            {
                return mapping[fhirCode];
            }
            catch (KeyNotFoundException)
            {
                switch (ijeField)
                {
                    case "COD":
                        ijeField = "County of Death";
                        break;
                    case "COD1A":
                        ijeField = "Cause of Death-1A";
                        break;
                    case "COD1B":
                        ijeField = "Cause of Death-1B";
                        break;
                    case "COD1C":
                        ijeField = "Cause of Death-1C";
                        break;
                    case "COD1D":
                        ijeField = "Cause of Death-1D";
                        break;
                    default:
                        break;
                }
                validationErrors.Add($"Error: Unable to find IJE {ijeField} mapping for FHIR {fhirField} field value '{fhirCode}'");
                return "";
            }

        }

        /// <summary>Given a Dictionary mapping IJE codes to FHIR strings and the relevant IJE and FHIR fields translate the IJE
        /// string to the appropriate FHIR code and set the value on the FHIR record object</summary>
        /// <param name="mapping">Dictionary for mapping the desired concept from IJE to FHIR; these dictionaries are defined in Mappings.cs</param>
        /// <param name="ijeField">Name of the IJE field that the FHIR field content is being set from</param>
        /// <param name="fhirField">Name of the FHIR field to set on the record; must have a related Helper property, e.g., EducationLevel must have EducationLevelHelper</param>
        /// <param name="value">The value to translate from IJE to FHIR and set on the record</param>
        protected void Set_MappingIJEToFHIR(Dictionary<string, string> mapping, string ijeField, string fhirField, string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                try
                {
                    PropertyInfo helperProperty = Record.GetType().GetProperty($"{fhirField}Helper");
                    if (helperProperty == null)
                    {
                        throw new NullReferenceException($"No helper method found called '{fhirField}Helper'");
                    }
                    helperProperty.SetValue(Record, mapping[value]);
                }
                catch (KeyNotFoundException)
                {
                    validationErrors.Add($"Error: Unable to find FHIR {fhirField} mapping for IJE {ijeField} field value '{value}'");
                }
            }
        }

        /// <summary>Set a value on the VitalRecord whose property is a Dictionary type.</summary>
        protected void Dictionary_Set(string ijeFieldName, string fhirFieldName, string key, string value)
        {
            IJEField info = FieldInfo(ijeFieldName);
            Dictionary<string, string> dictionary = (Dictionary<string, string>)this.Record.GetType().GetProperty(fhirFieldName).GetValue(this.Record);
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, string>();
            }
            if (!String.IsNullOrWhiteSpace(value))
            {
                dictionary[key] = value.Trim();
            }

            this.Record.GetType().GetProperty(fhirFieldName).SetValue(this.Record, dictionary);
        }

        /// <summary>Get a value on the VitalRecord whose property is a geographic type (and is contained in a dictionary).</summary>
        protected string Dictionary_Geo_Get(string ijeFieldName, string fhirFieldName, string keyPrefix, string geoType, bool isCoded)
        {
            IJEField info = FieldInfo(ijeFieldName);
            Dictionary<string, string> dictionary = this.Record == null ? null : (Dictionary<string, string>)this.Record.GetType().GetProperty(fhirFieldName).GetValue(this.Record);
            string key = keyPrefix + char.ToUpper(geoType[0]) + geoType.Substring(1);
            if (dictionary == null || !dictionary.ContainsKey(key))
            {
                return new String(' ', info.Length);
            }
            string current = Convert.ToString(dictionary[key]);
            if (isCoded)
            {
                if (geoType == "insideCityLimits")
                {
                    if (String.IsNullOrWhiteSpace(current))
                    {
                        current = "U";
                    }
                    else if (current == "true" || current == "True")
                    {
                        current = "Y";
                    }
                    else if (current == "false" || current == "False")
                    {
                        current = "N";
                    }
                }
                else if (geoType == "countyC" || geoType == "cityC")
                {
                    current = Truncate(current, info.Length).PadLeft(info.Length, '0');
                }
            }

            if (geoType == "zip")
            {  // Remove "-" for zip
                current.Replace("-", string.Empty);
            }
            if (!String.IsNullOrWhiteSpace(current))
            {
                return Truncate(current, info.Length).PadRight(info.Length, ' ');
            }
            else
            {
                return new String(' ', info.Length);
            }
        }

        /// <summary>Set a value on the VitalRecord whose property is a geographic type (and is contained in a dictionary).</summary>
        protected void Dictionary_Geo_Set(string ijeFieldName, string fhirFieldName, string keyPrefix, string geoType, bool isCoded, string value)
        {
            IJEField info = FieldInfo(ijeFieldName);
            Dictionary<string, string> dictionary = (Dictionary<string, string>)this.Record.GetType().GetProperty(fhirFieldName).GetValue(this.Record);
            string key = keyPrefix + char.ToUpper(geoType[0]) + geoType.Substring(1);

            // if the value is null, and the dictionary does not exist, return
            if (dictionary == null && String.IsNullOrWhiteSpace(value))
            {
                return;
            }
            // initialize the dictionary if it does not exist
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, string>();
            }

            if (!dictionary.ContainsKey(key) || String.IsNullOrWhiteSpace(dictionary[key]))
            {
                if (isCoded)
                {
                    if (geoType == "insideCityLimits")
                    {
                        if (!String.IsNullOrWhiteSpace(value) && value == "N")
                        {
                            dictionary[key] = "False";
                        }
                    }
                    else
                    {
                        dictionary[key] = value.Trim();
                    }
                }
                else
                {
                    dictionary[key] = value.Trim();
                }
            }
            else
            {
                dictionary[key] = value.Trim();
            }
            this.Record.GetType().GetProperty(fhirFieldName).SetValue(this.Record, dictionary);
        }

        /// <summary>Converts the FHIR representation of presence-only fields to the IJE equivalent.</summary>
        /// <param name="fieldValue">the value of the field</param>
        /// <param name="noneOfTheAboveValue">the value of the corresponding none-of-the-above field</param>
        /// <param name="fhirFieldName">name of the FHIR field being checked</param>
        /// <returns>Y (yes), N (no), or U (unknown)</returns>
        /// <remarks>U will only be returned if the category for the FHIR field is completely empty. Therefore, if a U was provided
        /// in the original IJE along with Y or N in the same category, that U value will not be maintained during a round-trip conversion.</remarks>
        protected string PresenceToIJE(bool fieldValue, bool noneOfTheAboveValue, string fhirFieldName)
        {
            if (fieldValue)
            {
                return "Y";
            }
            else if (noneOfTheAboveValue)
            {
                return "N";
            }
            else if (this.Record.IsCategoryEmpty(fhirFieldName))
            {
                return "U";
            }
            else
            {
                return "N";
            }
        }

        /// <summary>Converts the IJE representation of presence-only fields to the FHIR equivalent.</summary>
        /// <param name="value">Y (yes), N (no), or U (unknown)</param>
        /// <param name="fhirFieldName">name of the FHIR field that represents this IJE field</param>
        /// <param name="noneOfTheAboveFieldName">name of the FHIR field that represents "none of the above" for the FHIR field's category</param>
        /// <remarks>A value of N causes the FHIR field to be set to false and, if the field's category is then empty,
        /// cause the none of the above field to be set to true. U or other non-Y values cause the FHIR field to be set to false, but
        /// will not cause the none of the above field to be set to true.</remarks>
        protected void IJEToPresence(string value, string fhirFieldName, string noneOfTheAboveFieldName)
        {
            if (value.Equals("Y"))
            {
                this.Record.GetType().GetProperty(fhirFieldName).SetValue(this.Record, true);
            }
            else if (value.Equals("N"))
            {
                this.Record.GetType().GetProperty(fhirFieldName).SetValue(this.Record, false);
                if (this.Record.IsCategoryEmpty(fhirFieldName))
                {
                    this.Record.GetType().GetProperty(noneOfTheAboveFieldName).SetValue(this.Record, true);
                }
            }
            else
            {
                this.Record.GetType().GetProperty(fhirFieldName).SetValue(this.Record, false);
            }
        }
    }
}
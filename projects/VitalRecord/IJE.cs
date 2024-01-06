using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
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
        protected static string Truncate(string value, int length)
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


        /// <summary>Set a value on the VitalRecord whose IJE type is a right justified, zero filled string.</summary>
        protected void RightJustifiedZeroed_Set(string ijeFieldName, string fhirFieldName, string value)
        {
            IJEField info = FieldInfo(ijeFieldName);
            Record.GetType().GetProperty(fhirFieldName).SetValue(Record, value.TrimStart('0'));
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
        public VitalRecord ToRecord() {
            return Record;
        }

        /// <summary>Converts the internal <c>VitalRecord</c> into an IJE string.</summary>
        public override string ToString()
        {
            // Start with empty IJE record
            StringBuilder ije = new StringBuilder(new String(' ', (int)IJELength), (int)IJELength);

            // Loop over every property (these are the fields)
            foreach (PropertyInfo property in this.GetType().GetProperties())
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
    }
}
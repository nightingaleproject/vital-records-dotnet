using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BFDR;
using VR;

namespace canary.Models
{
    public class CanaryBirthRecord : Record
    {

        public CanaryBirthRecord() : base() { }

        public CanaryBirthRecord(VitalRecord record) : base(record) { }

        public CanaryBirthRecord(string record) : base(record) { }

        /// <summary>Check the given FHIR record string and return a list of issues. Also returned
        /// the parsed record if parsing was successful.</summary>
        public static Record CheckGet(string record, bool permissive, out List<Dictionary<string, string>> issues)
        {
            CanaryBirthRecord recordToSerialize = new CanaryBirthRecord(new BirthRecord(record, permissive));
            Record result = CheckGet(recordToSerialize, out issues);
            try
            {
                recordToSerialize.CreateIJEFromRecord(recordToSerialize.record, true);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                issues.AddRange(DecorateErrors(ex));
            }
            return result;
        }

        protected override VitalRecord CreateEmptyRecord()
        {
            return new BirthRecord();
        }

        protected override VR.IJE CreateIJEFromRecord(VitalRecord record, bool validate = true)
        {
            return new IJEBirth((BirthRecord)record, validate);
        }

        protected override VR.IJE CreateIJEFromString(string ije, bool validate = true)
        {
            return new IJEBirth(ije, validate);
        }

        protected override VitalRecord CreateRecordFromDescription(string value)
        {
            return VitalRecord.FromDescription<BirthRecord>(value);
        }

        protected override VitalRecord CreateRecordFromFHIR(string fhirString, bool permissive = true)
        {
            return new BirthRecord(fhirString);
        }

        protected override VitalRecord GenerateFakeRecord(string state, string type, string sex)
        {
            return new BirthRecordFaker(state, sex).Generate(true);
        }

        protected override List<PropertyInfo> GetIJEProperties()
        {
            return typeof(IJEBirth).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BFDR;
using VR;

namespace canary.Models
{
    public class CanaryFetalDeathRecord : Record
    {

        public CanaryFetalDeathRecord() : base() { }

        public CanaryFetalDeathRecord(VitalRecord record) : base(record) { }

        public CanaryFetalDeathRecord(string record) : base(record) { }

        /// <summary>Check the given FHIR record string and return a list of issues. Also returned
        /// the parsed record if parsing was successful.</summary>
        public static Record CheckGet(string record, bool permissive, out List<Dictionary<string, string>> issues)
        {
            CanaryFetalDeathRecord recordToSerialize = new CanaryFetalDeathRecord(new FetalDeathRecord(record, permissive));
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
            return new FetalDeathRecord();
        }

        protected override VR.IJE CreateIJEFromRecord(VitalRecord record, bool permissive = true)
        {
            return new IJEFetalDeath((FetalDeathRecord)record, permissive);
        }

        protected override VR.IJE CreateIJEFromString(string ije, bool permissive = true)
        {
            return new IJEFetalDeath(ije, permissive);
        }

        protected override VitalRecord CreateRecordFromDescription(string value)
        {
            return VitalRecord.FromDescription<FetalDeathRecord>(value);
        }

        protected override VitalRecord CreateRecordFromFHIR(string fhirString, bool permissive = true)
        {
            return new FetalDeathRecord(fhirString);
        }

        protected override VitalRecord GenerateFakeRecord(string state, string type, string sex)
        {
            return new FetalDeathFaker(state, sex).Generate(true);
        }

        protected override List<PropertyInfo> GetIJEProperties()
        {
            return typeof(IJEFetalDeath).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();
        }
    }
}

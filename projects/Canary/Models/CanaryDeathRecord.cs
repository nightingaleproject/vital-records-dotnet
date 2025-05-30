using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VR;
using VRDR;

namespace canary.Models
{

    public class CanaryDeathRecord : Record
    {
        public CanaryDeathRecord() : base() { }

        public CanaryDeathRecord(VitalRecord record) : base(record) { }

        public CanaryDeathRecord(string record) : base(record) { }

        public CanaryDeathRecord(string record, bool permissive) : base(record, permissive) { }

        public static Record CheckGet(string record, bool permissive, out List<Dictionary<string, string>> issues, bool retrieveFsh = false)
        {
            CanaryDeathRecord recordToSerialize = new CanaryDeathRecord(new DeathRecord(record, permissive));
            Record result = CheckGet(recordToSerialize, out issues, retrieveFsh);
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
            return new DeathRecord();
        }

        protected override VR.IJE CreateIJEFromRecord(VitalRecord record, bool validate = true)
        {
            return new IJEMortality((DeathRecord)record, validate);
        }

        protected override VR.IJE CreateIJEFromString(string ije, bool validate = true)
        {
            return new IJEMortality(ije, validate);
        }

        protected override VitalRecord CreateRecordFromDescription(string value)
        {
            return VitalRecord.FromDescription<DeathRecord>(value);
        }

        protected override VitalRecord CreateRecordFromFHIR(string fhirString, bool permissive = true)
        {
            return new DeathRecord(fhirString);
        }

        protected override VitalRecord GenerateFakeRecord(string state, string type, string sex)
        {
            return new DeathRecordFaker(state, type, sex).Generate(true);
        }

        protected override List<PropertyInfo> GetIJEProperties()
        {
            return typeof(IJEMortality).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();
        }


    }
}

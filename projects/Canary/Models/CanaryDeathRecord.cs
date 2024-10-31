using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using VR;
using VRDR;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;
using RestSharp.Authenticators;

namespace canary.Models
{

    public class CanaryDeathRecord : Record
    {
        public CanaryDeathRecord() : base() {}

        public CanaryDeathRecord(VitalRecord record) : base(record) {}

        public CanaryDeathRecord(string record) : base(record) {}

        public CanaryDeathRecord(string record, bool permissive) : base(record, permissive) {}

        public static Record CheckGet(string record, bool permissive, out List<Dictionary<string, string>> issues, bool retrieveFsh = false)
        {
            CanaryDeathRecord recordToSerialize = new CanaryDeathRecord(new DeathRecord(record, permissive));
            return Record.CheckGet(recordToSerialize, out issues, retrieveFsh);
        }

        protected override VitalRecord CreateEmptyRecord()
        {
            return new DeathRecord();
        }

        protected override VR.IJE CreateIJEFromRecord(VitalRecord record, bool permissive = true)
        {
            return new IJEMortality((DeathRecord) record, permissive);
        }

        protected override VR.IJE CreateIJEFromString(string ije, bool permissive = true)
        {
            return new IJEMortality(ije, permissive);
        }

        protected override VitalRecord CreateRecordFromDescription(string value) {
          return VitalRecord.FromDescription<DeathRecord>(value); 
        }

        protected override VitalRecord CreateRecordFromFHIR(string fhirString, bool permissive = true)
        {
            return new DeathRecord(fhirString);
        }

        protected override VitalRecord GenerateFakeRecord(string state, string type, string sex) {
          return new DeathRecordFaker(state, type, sex).Generate(true);
        }

        protected override List<PropertyInfo> GetIJEProperties()
        {
            return typeof(IJEMortality).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Field).ToList();
        }


    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VRDR;
using VR;
using Newtonsoft.Json;
using System.IO;
using System.Text.Json.Nodes;
using BFDR;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Text.RegularExpressions;
using System.Linq;

namespace canary.Models
{
    public class RecordContext : DbContext
    {

        public DbSet<CanaryDeathRecord> DeathRecords { get; set; }
        public DbSet<CanaryBirthRecord> BirthRecords { get; set; }

        public DbSet<Endpoint> Endpoints { get; set; }

        public DbSet<DeathTest> DeathTests { get; set; }
        public DbSet<BirthTest> BirthTests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=canary.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Endpoints
            modelBuilder.Entity<Endpoint>().Property(b => b.Issues).HasConversion(v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(v));

            modelBuilder.Entity<Endpoint>().Property(b => b.Record).HasConversion(v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Record>(v));

            // Common Record Serializer
            modelBuilder.Entity<Record>().Property(b => b.IjeInfo).HasConversion(v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(v));

            // VRDR
            modelBuilder.Entity<DeathTest>().Property(t => t.ReferenceRecord).HasConversion(t => t.GetRecord().ToXML(),
                t => new CanaryDeathRecord(t));

            modelBuilder.Entity<DeathTest>().Property(t => t.TestRecord).HasConversion(t => t.GetRecord().ToXML(),
                t => new CanaryDeathRecord(t));

            modelBuilder.Entity<CanaryDeathMessage>();

            // BFDR
            modelBuilder.Entity<BirthTest>().Property(t => t.ReferenceRecord).HasConversion(t => t.GetRecord().ToXML(),
                t => new CanaryBirthRecord(t));

            modelBuilder.Entity<BirthTest>().Property(t => t.TestRecord).HasConversion(t => t.GetRecord().ToXML(),
                t => new CanaryBirthRecord(t));

            modelBuilder.Entity<CanaryBirthMessage>();
        }
    }

    public abstract class Record
    {
        protected VitalRecord record { get; set; }

        private string ije { get; set; }

        private string fsh { get; set; }


        public int RecordId { get; set; }

        public string Xml
        {
            get
            {
                if (record == null)
                {
                    return null;
                }
                return record.ToXML();
            }
            set
            {
                record = this.CreateRecordFromFHIR(value);
                ije = this.CreateIJEFromRecord(this.record).ToString();
            }
        }

        public string Json
        {
            get
            {
                if (record == null)
                {
                    return null;
                }
                return record.ToJSON();
            }
            set
            {
                record = this.CreateRecordFromFHIR(value);
                ije = this.CreateIJEFromRecord(this.record).ToString();
            }
        }

        public string Ije
        {
            get
            {
                if (record == null)
                {
                    return null;
                }
                return ije;
            }
            set
            {
                this.record = this.CreateIJEFromString(value).ToRecord();
                ije = value;
            }
        }

        public string Fsh
        {
            get
            {
                if (record == null)
                {
                    return null;
                }
                return fsh;
            }

            set { fsh = value; }

        }

        public VitalRecord GetRecord()
        {
            return record;
        }

        public List<Dictionary<string, string>> IjeInfo
        {
            get
            {
                string ijeString = ije;
                List<PropertyInfo> properties = this.GetIJEProperties();
                List<Dictionary<string, string>> propList = new List<Dictionary<string, string>>();
                foreach (PropertyInfo property in properties)
                {
                    IJEField info = property.GetCustomAttribute<IJEField>();
                    string field = ijeString.Substring(info.Location - 1, info.Length);
                    Dictionary<string, string> propInfo = new Dictionary<string, string>();
                    propInfo.Add("number", info.Field.ToString());
                    propInfo.Add("location", info.Location.ToString());
                    propInfo.Add("length", info.Length.ToString());
                    propInfo.Add("contents", info.Contents);
                    propInfo.Add("name", info.Name);
                    propInfo.Add("value", field.Trim());
                    propList.Add(propInfo);
                }
                return propList;
            }
            set
            {
                // NOOP
            }
        }

        public string FhirInfo
        {
            get
            {
                if (record == null)
                {
                    return null;
                }
                return record.ToDescription();
            }
            set
            {
                this.record = this.CreateRecordFromDescription(value);
                this.ije = this.CreateIJEFromRecord(this.record).ToString();
            }
        }

        public Record()
        {
            record = this.CreateEmptyRecord();
            this.ije = this.CreateIJEFromRecord(this.record, false).ToString();
        }

        public Record(VitalRecord record)
        {
            this.record = record;
            this.ije = this.CreateIJEFromRecord(record, false).ToString();
        }

        public Record(string record)
        {
            this.record = this.CreateRecordFromFHIR(record);
            this.ije = this.CreateIJEFromRecord(this.record, false).ToString();
        }

        public Record(string record, bool permissive)
        {
            this.record = this.CreateRecordFromFHIR(record, permissive);
            this.ije = this.CreateIJEFromRecord(this.record).ToString();
        }

        protected abstract VitalRecord CreateEmptyRecord();

        protected abstract VitalRecord CreateRecordFromDescription(string value);

        protected abstract VitalRecord CreateRecordFromFHIR(string fhir, bool permissive = true);

        protected abstract VitalRecord GenerateFakeRecord(string state, string type, string sex);

        protected abstract IJE CreateIJEFromRecord(VitalRecord record, bool permissive = true);

        protected abstract IJE CreateIJEFromString(string value, bool permissive = true);

        protected abstract List<PropertyInfo> GetIJEProperties();

        /// <summary>Check the given FHIR record string and return a list of issues. Also returned
        /// the parsed record if parsing was successful.</summary>
        protected static Record CheckGet(Record recordToSerialize, out List<Dictionary<string, string>> issues)
        {
            Record newRecord = null;
            List<Dictionary<string, string>> entries = new List<Dictionary<string, string>>();
            try
            {
                // If the object fails to serialize then it will not be possible for canary to return it to the user
                // (since canary has to serialize it to JSON in order to do so). This is why serialization is tested
                // here and if it passes then the record is considered "safe" to return.
                JsonConvert.SerializeObject(recordToSerialize);
                newRecord = recordToSerialize;
                validateRecordType(newRecord);
            }
            catch (Exception e)
            {
                entries = DecorateErrors(e);
            }
            issues = entries;
            return newRecord;
        }

        /// <summary>Recursively call InnerException and add all errors to the list until we reach the BaseException.</summary>
        public static List<Dictionary<string, string>> DecorateErrors(Exception e)
        {
            List<Dictionary<string, string>> entries = new List<Dictionary<string, string>>();

            return DecorateErrorsRecursive(e, entries);
        }

        public static string ValidateFshSushi(string fshInput)
        {
            string resultData = string.Empty;

            System.Threading.Tasks.Task<string> task =
                System.Threading.Tasks.Task.Run<string>(async () => await GetSushResults(fshInput));

            resultData = task.Result;
            return resultData;
        }

        public static List<Dictionary<string, string>> ParseSushiErrorsAndWarnings(string sushiResults)
        {
            var issueList = new List<Dictionary<string, string>>();

            if (String.IsNullOrWhiteSpace(sushiResults))
            {
                return issueList;
            }

            JToken jToken = JToken.Parse(sushiResults);

            var errorList = jToken["errors"].Select(z => z).ToList();

            foreach (var errorData in errorList)
            {
                StringBuilder messageInfo = new StringBuilder();
                messageInfo.Append((string)errorData["message"]);
                if (errorData["location"] != null)
                {
                    messageInfo.Append(" Location: ");
                    messageInfo.Append(" start col: ");
                    messageInfo.Append((string)errorData["location"]["startColumn"]);
                    messageInfo.Append(" end col: ");
                    messageInfo.Append((string)errorData["location"]["endColumn"]);
                    messageInfo.Append(" start line: ");
                    messageInfo.Append((string)errorData["location"]["startLine"]);
                    messageInfo.Append(" end line: ");
                    messageInfo.Append((string)errorData["location"]["endLine"]);
                }

                issueList.Add(
                    new Dictionary<string, string> { { "severity", "error" },
                        { "message", messageInfo.ToString() } }
                );
            }

            var warningList = jToken["warnings"].Select(z => z).ToList();

            foreach (var warningData in warningList)
            {
                StringBuilder messageInfo = new StringBuilder();
                messageInfo.Append((string)warningData["message"]);
                if (warningData["location"] != null)
                {
                    messageInfo.Append(" Location: ");
                    messageInfo.Append(" start col: ");
                    messageInfo.Append((string)warningData["location"]["startColumn"]);
                    messageInfo.Append(" end col: ");
                    messageInfo.Append((string)warningData["location"]["endColumn"]);
                    messageInfo.Append(" start line: ");
                    messageInfo.Append((string)warningData["location"]["startLine"]);
                    messageInfo.Append(" end line: ");
                    messageInfo.Append((string)warningData["location"]["endLine"]);
                }

                issueList.Add(
                    new Dictionary<string, string> { { "severity", "warning" },
                        { "message", messageInfo.ToString() } }
                );
            }

            return issueList;
        }

        public async static Task<string> GetFshInspection(string fshData)
        {
            string ret = string.Empty;

            try
            {
                string url = Environment.GetEnvironmentVariable("DATA_CONVERSION_HOST") ?? "cte-nvss-canary-a213fdc38384.azurewebsites.net";

                JsonObject fshJson = new JsonObject();
                fshJson.Add("fsh", fshData);
                string convertedFshData = fshJson.ToString();

                var options = new RestClientOptions("https://" + url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/FshToFhir", Method.Post);
                request.AddHeader("Host", url);
                request.AddJsonBody(fshJson);
                RestResponse response = await client.ExecuteAsync(request);
                ret = response.Content;

            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        public async static Task<string> GetSushResults(string fshInput)
        {
            string ret = string.Empty;

            ret = await GetFshInspection(fshInput);

            return ret;
        }

        public static void validateRecordType(Record record)
        {
            List<string> acceptedPayloadTypes = new List<string>() { "document" };  //Add to this with "document", "newtype", "etc"

            if (String.IsNullOrWhiteSpace(record.Json))
            {
                return;
            }

            var jsonData = JsonObject.Parse(record.Json);
            if(jsonData["type"] == null)
            {
                throw new Exception("No type key in JSON data");
            }
            string payloadType = jsonData["type"].ToString();
            if (!acceptedPayloadTypes.Contains(payloadType))
            {
                throw new Exception("JSON input type not equal to " + string.Join(",", acceptedPayloadTypes.ToArray()));
            }
        }

        public static List<Dictionary<string, string>> DecorateErrorsRecursive(Exception exception, List<Dictionary<string, string>> entries)
        {
            if (exception != null && exception.Message != null)
            {
                foreach (string er in exception.Message.Split(";"))
                {
                    Dictionary<string, string> entry = new Dictionary<string, string>();
                    // targetSite contains the information required to show the function class and function that
                    // the error occurred in
                    var targetSite = exception.TargetSite;
                    string erString = er.Trim();
                    // Ensure the original error string always ends in a period.
                    if (!erString.EndsWith('.')) erString += '.';
                    string errorWithLocation = $"{erString} Error occurred";
                    if (targetSite.ReflectedType != null) errorWithLocation += $" at {targetSite.ReflectedType}";
                    if (targetSite.Name != null) errorWithLocation += $" in function {targetSite.Name}";
                    errorWithLocation += ".";
                    entry.Add("message", errorWithLocation.Replace("Parser:", "").Trim());
                    entry.Add("severity", "error");
                    entries.Add(entry);
                }
                return DecorateErrorsRecursive(exception.InnerException, entries);
            }
            return entries;
        }

        /// <summary>Populate this record with synthetic data.</summary>
        public void Populate(string state = "MA", string type = "Natural", string sex = "Male")
        {
            this.record = this.GenerateFakeRecord(state, type, sex);
            this.Ije = this.CreateIJEFromRecord(this.record, true).ToString();
        }

        public async static Task<string> GetFshData(string fhirMessage)
        {
            string ret = string.Empty;

            string rawFsh = await GetRawFshData(fhirMessage);

            ret = await ConvertFshDataProfileNames(rawFsh);

            return ret;

        }

        public async static Task<string> GetRawFshData(string fhirMessage)
        {
            string ret = string.Empty;

            try
            {
                string url = Environment.GetEnvironmentVariable("DATA_CONVERSION_HOST") ?? "cte-nvss-canary-a213fdc38384.azurewebsites.net";

                byte[] bytes = Encoding.ASCII.GetBytes(fhirMessage);

                var fhrContent = Regex.Replace(fhirMessage, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1");

                var options = new RestClientOptions("https://" + url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/FhirToFsh", Method.Post);
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Host", url);
                request.AddJsonBody(fhirMessage);
                RestResponse response = await client.ExecuteAsync(request);
                ret = response.Content;

            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        public async static Task<string> ConvertFshDataProfileNames(string rawFshData)
        {
            string ret = string.Empty;

            try
            {
                string url = Environment.GetEnvironmentVariable("DATA_CONVERSION_HOST") ?? "cte-nvss-canary-a213fdc38384.azurewebsites.net";

                byte[] bytes = Encoding.ASCII.GetBytes(rawFshData);

                var fhrContent = Regex.Replace(rawFshData, @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1");

                var options = new RestClientOptions("https://" + url)
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/ConvertInstanceOf", Method.Post);
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Host", url);
                request.AddJsonBody(rawFshData);
                RestResponse response = await client.ExecuteAsync(request);
                ret = response.Content;

            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

    }

}

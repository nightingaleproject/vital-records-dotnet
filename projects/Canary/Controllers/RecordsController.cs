using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VRDR;
using VR;
using canary.Models;
using BFDR;

namespace canary.Controllers
{
    [ApiController]
    public class RecordsController : ControllerBase
    {
        /// <summary>
        /// Returns all records.
        /// GET /api/records
        /// </summary>
        [HttpGet("Records")]
        [HttpGet("Records/Index")]
        public Record[] VRDRIndex()
        {
            // Find the record in the database and return it
            using (var db = new RecordContext())
            {
                return db.DeathRecords.ToArray();
            }
        }

        /// <summary>
        /// Given an id, returns the corresponding death record.
        /// GET /api/records/{id}
        /// </summary>
        [HttpGet("Records/vrdr/{id:int}")]
        [HttpGet("Records/vrdr/Get/{id:int}")]
        public Record GetVRDR(int id)
        {
            // Find the record in the database and return it
            using (var db = new RecordContext())
            {
                return db.DeathRecords.Where(record => record.RecordId == id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Given an id, returns the corresponding birth record.
        /// GET /api/records/{id}
        /// </summary>
        [HttpGet("Records/bfdr/{id:int}")]
        [HttpGet("Records/bfdr/Get/{id:int}")]
        public Record GetBFDR(int id)
        {
            // Find the record in the database and return it
            using (var db = new RecordContext())
            {
                return db.BirthRecords.Where(record => record.RecordId == id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Given an id, returns the corresponding (death) record as JSON.
        /// GET /api/records/json/{id}
        /// </summary>
        [HttpGet("Records/JSON/{id:int}")]
        public string GetJson(int id)
        {
            return Connectathon.FromId(id).ToJSON();
        }

        /// <summary>
        /// Given an id, returns the corresponding (death) record as XML.
        /// GET /api/records/xml/{id}
        /// </summary>
        [HttpGet("Records/XML/{id:int}")]
        public string Getxml(int id)
        {
            return Connectathon.FromId(id).ToXML();
        }

        /// <summary>
        /// Creates a new synthetic death record, and returns it. Does not save it to the database.
        /// GET /api/records/new
        /// </summary>
        [HttpGet("Records/vrdr/New")]
        public Record NewVRDRRecordGet(string state, string type, string sex)
        {
            // Create new record from scratch
            CanaryDeathRecord record = new();

            // Populate the record with fake data
            record.Populate(state, type, sex);

            // Return the record
            return record;
        }

        [HttpGet("Records/bfdr/New")]
        public Record NewBFDRRecordGet(string state, string sex)
        {
            // Create new record from scratch
            CanaryBirthRecord record = new();

            // Populate the record with fake data
            record.Populate(state: state, sex: sex);

            // Return the record
            return record;
        }

        /// <summary>
        /// Creates a new death record using the contents provided. Returns the record and any validation issues.
        /// POST /api/records/vrdr/new
        /// </summary>
        [HttpPost("Records/vrdr/New")]
        public async Task<(Record record, List<Dictionary<string, string>> issues)> NewVRDRPost()
        {
            return await NewPost(true);
        }

        /// <summary>
        /// Creates a new birth record using the contents provided. Returns the record and any validation issues.
        /// POST /api/records/bfdr/new
        /// </summary>
        [HttpPost("Records/bfdr/New")]
        public async Task<(Record record, List<Dictionary<string, string>> issues)> NewBFDRPost()
        {
            return await NewPost(false);
        }

        private async Task<(Record record, List<Dictionary<string, string>> issues)> NewPost(bool isVRDR)
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            if (!String.IsNullOrEmpty(input))
            {
                if (input.Trim().StartsWith("<") || input.Trim().StartsWith("{")) // XML or JSON?
                {
                    List<Dictionary<string, string>> issues;
                    Record record = isVRDR ? CanaryDeathRecord.CheckGet(input, false, out issues) : CanaryBirthRecord.CheckGet(input, false, out issues);
                    return (record, issues);
                }
                else
                {
                    try // IJE?
                    {
                        // If input.Length != 5000, truncate/pad according to force it to 5000.
                        if (input.Length > 5000)
                        {
                            input = input.Substring(0, 5000);
                        }
                        else if (input.Length < 5000)
                        {
                            input = input.PadRight(5000, ' ');
                        }
                        if (isVRDR)
                        {
                            IJEMortality ije = new IJEMortality(input);
                            DeathRecord deathRecord = ije.ToRecord();
                            return (new CanaryDeathRecord(deathRecord), new List<Dictionary<string, string>> {} );
                        }
                        else
                        {
                            IJENatality ije = new IJENatality(input);
                            BirthRecord br = ije.ToRecord();
                            return (new CanaryBirthRecord(br), new List<Dictionary<string, string>> {} );
                        }

                    }
                    catch (Exception e)
                    {
                        String message = e.Message;
                        while (e.InnerException != null)
                        {
                            e = e.InnerException;
                            message += "; Inner Exception = [ " + e.Message + " ]";
                        }
                        return (null, new List<Dictionary<string, string>> { new Dictionary<string, string> { { "severity", "error" }, { "message", message } } });
                    }
                }
            }
            else
            {
                return (null, new List<Dictionary<string, string>> { new Dictionary<string, string> { { "severity", "error" }, { "message", "The given input appears to be empty." } } });
            }
        }

        /// <summary>
        /// Creates a new death record using the "description" contents provided. Returns the record.
        /// POST /api/records/vrdr/description/new
        /// </summary>
        [HttpPost("Records/vrdr/Description/New")]
        public async Task<Record> NewVRDRDescriptionPost()
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            if (!String.IsNullOrEmpty(input))
            {
                DeathRecord record = VitalRecord.FromDescription<DeathRecord>(input);
                return new CanaryDeathRecord(record);
            }
            return null;
        }

        /// <summary>
        /// Creates a new death record using the "description" contents provided. Returns the record.
        /// POST /api/records/bfdr/description/new
        /// </summary>
        [HttpPost("Records/bfdr/Description/New")]
        public async Task<Record> NewBFDRDescriptionPost()
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            if (!String.IsNullOrEmpty(input))
            {
                BirthRecord record = VitalRecord.FromDescription<BirthRecord>(input);
                return new CanaryBirthRecord(record);
            }
            return null;
        }

        /// <summary>
        /// Get's the DeathRecord "Description" structure.
        /// GET /api/records/description
        /// </summary>
        [HttpGet("Records/vrdr/Description")]
        public string GetVRDRDescription()
        {
            return GetDescription(new DeathRecord(), typeof(DeathRecord).GetProperties());
        }

        [HttpGet("Records/bfdr/Description")]
        public string GetBFDRDescription()
        {
            return GetDescription(new BirthRecord(), typeof(BirthRecord).GetProperties());
        }

        private static string GetDescription(VitalRecord record, PropertyInfo[] properties)
        {
            Dictionary<string, Dictionary<string, dynamic>> description = new Dictionary<string, Dictionary<string, dynamic>>();
            foreach(PropertyInfo property in properties.OrderBy(p => p.GetCustomAttribute<Property>().Priority))
            {
                // Grab property annotation for this property
                Property info = property.GetCustomAttribute<Property>();

                // Skip properties that shouldn't be serialized.
                if (!info.Serialize)
                {
                    continue;
                }

                // Add category if it doesn't yet exist
                if (!description.ContainsKey(info.Category))
                {
                    description.Add(info.Category, new Dictionary<string, dynamic>());
                }

                // Add the new property to the category
                Dictionary<string, dynamic> category = description[info.Category];
                category[property.Name] = new Dictionary<string, dynamic>();

                // Add the attributes of the property
                category[property.Name]["Name"] = info.Name;
                category[property.Name]["Type"] = info.Type.ToString();
                category[property.Name]["Description"] = info.Description;

                // Add the current value of the property
                if (info.Type == Property.Types.Dictionary)
                {
                    // Special case for Dictionary; we want to be able to describe what each key means
                    Dictionary<string, string> value = (Dictionary<string, string>)property.GetValue(record);
                    if (value == null)
                    {
                        continue;
                    }
                    Dictionary<string, Dictionary<string, string>> moreInfo = new Dictionary<string, Dictionary<string, string>>();
                    foreach (PropertyParam parameter in property.GetCustomAttributes<PropertyParam>())
                    {
                        moreInfo[parameter.Key] = new Dictionary<string, string>();
                        moreInfo[parameter.Key]["Description"] = parameter.Description;
                        moreInfo[parameter.Key]["Value"] = null;
                        if (value.ContainsKey(parameter.Key))
                        {
                            moreInfo[parameter.Key]["Value"] = value[parameter.Key];
                        }
                        else
                        {
                            moreInfo[parameter.Key]["Value"] = null;
                        }

                    }
                    category[property.Name]["Value"] = moreInfo;
                }
                else
                {
                    category[property.Name]["Value"] = property.GetValue(record);
                }
            }
            return JsonConvert.SerializeObject(description);
        }

        /// <summary>
        /// Creates a new synthetic death record, saves it to the database, and returns it.
        /// GET /api/records/new
        /// </summary>
        [HttpGet("Records/vrdr/Create")]
        public Record VRDRCreate(string state, string type)
        {
            using (var db = new RecordContext())
            {
                // Create new record from scratch
                CanaryDeathRecord record = new();

                // Populate the record with fake data
                record.Populate();

                // Save the record to the database
                db.DeathRecords.Add(record);
                db.SaveChanges();

                // Return the record
                return record;
            }
        }

        [HttpGet("Records/bfdr/Create")]
        public Record BFDRCreate(string state, string type)
        {
            using (var db = new RecordContext())
            {
                // Create new record from scratch
                CanaryBirthRecord record = new();

                // Populate the record with fake data
                record.Populate();

                // Save the record to the database
                db.BirthRecords.Add(record);
                db.SaveChanges();

                // Return the record
                return record;
            }
        }
    }
}

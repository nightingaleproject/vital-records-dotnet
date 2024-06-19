using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VR;
using canary.Models;

// TODO - Rather than using BFDR, use the phrasing "BFDR - Birth" and "BFDR - Fetal Death"

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
            return ControllerMappers.dbRecords["vrdr"]().ToArray();
        }

        /// <summary>
        /// Given an id, returns the corresponding record based on the given record type.
        /// GET /api/records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/{id}
        /// </summary>
        [HttpGet("Records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/{id:int}")]
        [HttpGet("Records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/Get/{id:int}")]
        public Record Get(string recordType, int id)
        {
            // Find the record in the database and return it
            return ControllerMappers.dbRecords[recordType]().Where(record => record.RecordId == id).FirstOrDefault();
        }

        /// <summary>
        /// Given an id, returns the corresponding record as JSON.
        /// GET /api/records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/json/{id}
        /// </summary>
        [HttpGet("Records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/JSON/{id:int}")]
        public string GetJSON(string recordType, int id)
        {
            return ControllerMappers.connectathonRecords[recordType](id).ToJSON();
        }

        /// <summary>
        /// Given an id, returns the corresponding record as XML.
        /// GET /api/records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/xml/{id}
        /// </summary>
        [HttpGet("Records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/XML/{id:int}")]
        public string GetXML(string recordType, int id)
        {
            return ControllerMappers.connectathonRecords[recordType](id).ToXML();
        }

        /// <summary>
        /// Creates a new synthetic record based on the given type, and returns it. Does not save it to the database.
        /// GET /api/records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/New
        /// </summary>
        [HttpGet("Records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/New")]
        public Record NewRecordGet(string recordType, string state, string type, string sex)
        {
            // Create new record from scratch
            Record record = ControllerMappers.createEmptyCanaryRecord[recordType];

            // Populate the record with fake data
            record.Populate(state: state, sex: sex, type: type);

            // Return the record
            return record;
        }

        // TODO - figure out why these explicit routes are needed only in this case. It should just use NewPost(), but can't due to a strange error.
        [HttpPost("Records/vrdr/New")]
        public async Task<(Record record, List<Dictionary<string, string>> issues)> NewVRDRPost()
        {
            return await NewPost("vrdr");
        }

        [HttpPost("Records/bfdr/New")]
        public async Task<(Record record, List<Dictionary<string, string>> issues)> NewBFDRPost()
        {
            return await NewPost("bfdr");
        }

        /// <summary>
        /// Creates a new death record using the contents provided. Returns the record and any validation issues.
        /// POST /api/records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/new
        /// </summary>
        [HttpPost("Records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/New")]
        public async Task<(Record record, List<Dictionary<string, string>> issues)> NewPost(string recordType)
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            if (!String.IsNullOrEmpty(input))
            {
                if (input.Trim().StartsWith("<") || input.Trim().StartsWith("{")) // XML or JSON?
                {
                    return ControllerMappers.checkGetRecord[recordType](input);
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
                        return ControllerMappers.createRecordFromIJE[recordType](input);
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
        /// Creates a new record using the "description" contents provided. Returns the record.
        /// POST /api/records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/description/new
        /// </summary>
        [HttpPost("Records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/Description/New")]
        public async Task<Record> NewDescriptionPost(string recordType)
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            if (!String.IsNullOrEmpty(input))
            {
                return ControllerMappers.createCanaryRecordFromString[recordType](input);
            }
            return null;
        }

        /// <summary>
        /// Get's the VitalRecord "Description" structure of the given record type.
        /// GET /api/records/description
        /// </summary>
        [HttpGet("Records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/Description")]
        public string GetDescription(string recordType)
        {
            VitalRecord record = ControllerMappers.createEmptyRecord[recordType];
            PropertyInfo[] properties = ControllerMappers.getRecordProperties[recordType];

            Dictionary<string, Dictionary<string, dynamic>> description = new Dictionary<string, Dictionary<string, dynamic>>();
            foreach (PropertyInfo property in properties.OrderBy(p => p.GetCustomAttribute<Property>().Priority))
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

                // Add snippets
                FHIRPath path = property.GetCustomAttribute<FHIRPath>();
                category[property.Name]["CheckboxType"] = path.Section != null;
                category[property.Name]["Section"] = path.Section;
                category[property.Name]["Code"] = path.Code;
                category[property.Name]["CategoryCode"] = path.CategoryCode;

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
        /// Creates a new synthetic record, saves it to the database, and returns it.
        /// GET /api/records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/create
        /// </summary>
        [HttpGet("Records/{recordType:regex(^(vrdr|bfdr-birth|bfdr-fetaldeath)$)}/Create")]
        public Record Create(string recordType, string state, string type)
        {
            // Create new record from scratch
            Record record = ControllerMappers.createEmptyCanaryRecord[recordType];

            // Populate the record with fake data
            record.Populate();

            // Save the record to the database
            ControllerMappers.addDbRecord[recordType](record);

            // Return the record
            return record;
        }
    }
}
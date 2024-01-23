using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VRDR;
using canary.Models;
using BFDR;

namespace canary.Controllers
{
    [ApiController]
    public class EndpointsController : ControllerBase
    {
        /// <summary>
        /// Creates a new endpoint. Returns its id.
        /// GET /api/endpoints/new
        /// </summary>
        [HttpGet("Endpoints/New")]
        [HttpGet("Endpoints/vrdr/New")]
        [HttpGet("Endpoints/bfdr/New")]
        public int New()
        {
            // Find the record in the database and return it
            using (var db = new RecordContext())
            {
                if (db.Endpoints.Count() > 100)
                {
                    // Too many endpoints in existance, delete the oldest to prevent someone from abusing this.
                    // TODO: Probably a smoother way to accomplish this. Investigate.
                    db.Endpoints.Remove(db.Endpoints.FirstOrDefault());
                }
                Endpoint endpoint = new();
                db.Endpoints.Add(endpoint);
                db.SaveChanges();
                return endpoint.EndpointId;
            }
        }

        /// <summary>
        /// Given an id, returns the corresponding endpoint.
        /// GET /api/records/{id}
        /// </summary>
        [HttpGet("Endpoints/{id:int}")]
        [HttpGet("Endpoints/vrdr/{id:int}")]
        [HttpGet("Endpoints/bfdr/{id:int}")]
        [HttpGet("Endpoints/Get/{id:int}")]
        [HttpGet("Endpoints/Get/vrdr/{id:int}")]
        [HttpGet("Endpoints/Get/bfdr/{id:int}")]
        public Endpoint Get(int id)
        {
            // Find the record in the database and return it
            using (var db = new RecordContext())
            {
                return db.Endpoints.Where(e => e.EndpointId == id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Lets you post a raw record to Canary, which is processed and added to the Endpoint.
        /// POST /api/endpoints/record/{id:int}
        /// </summary>
        [HttpPost("Endpoints/vrdr/Record/{id:int}")]
        public async Task<int> VRDRRecordPost(int id)
        {
            Record record = null;
            List<Dictionary<string, string>> issues = null;
            string input;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                input = await reader.ReadToEndAsync();
            }
            if (!String.IsNullOrEmpty(input))
            {
                if (input.Trim().StartsWith("<") || input.Trim().StartsWith("{")) // XML or JSON?
                {
                    record = CanaryDeathRecord.CheckGet(input, false, out issues);
                }
                else
                {
                    try // IJE?
                    {
                        if (input.Length != 5000)
                        {
                            (record, issues) = (null, new List<Dictionary<string, string>> { new Dictionary<string, string> { { "severity", "error" }, { "message", "The given input does not appear to be a valid record." } } });
                        }
                        IJEMortality ije = new IJEMortality(input);
                        DeathRecord deathRecord = ije.ToRecord();
                        (record, issues) = (new CanaryDeathRecord(deathRecord), new List<Dictionary<string, string>> {} );
                    }
                    catch (Exception e)
                    {
                        (record, issues) = (null, new List<Dictionary<string, string>> { new Dictionary<string, string> { { "severity", "error" }, { "message", e.Message } } });
                    }
                }
                if (record != null)
                {
                    using (var db = new RecordContext())
                    {
                        Endpoint endpoint = db.Endpoints.Where(e => e.EndpointId == id).FirstOrDefault();
                        endpoint.Record = record;
                        endpoint.Issues = issues;
                        endpoint.Finished = true;
                        db.SaveChanges();
                        return endpoint.EndpointId;
                    }
                }
            }
            else
            {
                issues = new List<Dictionary<string, string>> { new Dictionary<string, string> { { "severity", "error" }, { "message", "The given input appears to be empty." } } };
            }

            if (record == null && issues != null)
            {
                using (var db = new RecordContext())
                {
                    Endpoint endpoint = db.Endpoints.Where(e => e.EndpointId == id).FirstOrDefault();
                    endpoint.Issues = issues;
                    endpoint.Finished = true;
                    db.SaveChanges();
                    return endpoint.EndpointId;
                }
            }

            return 0;
        }

    }
}

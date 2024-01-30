using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VRDR;
using canary.Models;
using VR;
using BFDR;

namespace canary.Controllers
{
    [ApiController]
    public class TestsController : ControllerBase
    {
        /// <summary>
        /// Returns all tests.
        /// GET /api/tests
        /// </summary>
        [HttpGet("Tests")]
        [HttpGet("Tests/Index")]
        public Dictionary<string, string>[] IndexVRDR()
        {
            using (var db = new RecordContext())
            {
                List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                foreach(Test test in db.DeathTests.Take(20))
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("testId", test.TestId.ToString());
                    result.Add("created", test.Created.ToString());
                    result.Add("type", test.Type != null ? test.Type.ToString() : "");
                    result.Add("completedDateTime", test.CompletedDateTime.ToString());
                    result.Add("correct", test.Correct.ToString());
                    result.Add("total", test.Total.ToString());
                    results.Add(result);
                }
                return results.ToArray();
            }
        }

        /// <summary>
        /// Gets a death test by id.
        /// GET /api/tests/vrdr/1
        /// </summary>
        [HttpGet("Tests/vrdr/{id:int}")]
        public Test GetVRDRTest(int id)
        {
            using (var db = new RecordContext())
            {
                return db.DeathTests.Where(t => t.TestId == id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a birth test by id.
        /// GET /api/tests/bfdr/1
        /// </summary>
        [HttpGet("Tests/bfdr/{id:int}")]
        public Test GetBFDRTest(int id)
        {
            using (var db = new RecordContext())
            {
                return db.BirthTests.Where(t => t.TestId == id).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a pre-defined connectathon test by id with a certificate
        /// number and an optional state parameter which sets the placeOfDeath
        /// to the provided state.
        /// GET /api/tests/connectathon/1 or /api/tests/connectathon/1/AK
        /// </summary>
        [HttpGet("Tests/vrdr/Connectathon/{id:int}/{certificateNumber:int}/{state?}")]
        public Test GetTestVRDRConnectathon(int id, int certificateNumber, string state)
        {
            using (var db = new RecordContext())
            {
                DeathTest test = new DeathTest(Connectathon.FromId(id, certificateNumber, state));
                db.DeathTests.Add(test);
                db.SaveChanges();
                return test;
            }
        }

        /// <summary>
        /// Gets a pre-defined connectathon test by id with a certificate
        /// number and an optional state parameter which sets the placeOfDeath
        /// to the provided state.
        /// GET /api/tests/connectathon/1 or /api/tests/connectathon/1/AK
        /// </summary>
        [HttpGet("Tests/bfdr/Connectathon/{id:int}/{certificateNumber:int}/{state?}")]
        public Test GetTestBFDRConnectathon(int id, int certificateNumber, string state)
        {
            using (var db = new RecordContext())
            {
                BirthTest test = new BirthTest(Connectathon.FromId(id, certificateNumber, state));
                db.BirthTests.Add(test);
                db.SaveChanges();
                return test;
            }
        }

        [HttpPost("Tests/vrdr/Validator")]
        public async Task<Test> GetTestVRDRIJEValidator(int id)
        {
            using (var db = new RecordContext())
            {
                string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
                if (String.IsNullOrEmpty(input))
                {
                    return null;
                }
                DeathRecord record = VitalRecord.FromDescription<DeathRecord>(input);
                DeathTest test = new DeathTest(record);
                db.DeathTests.Add(test);
                db.SaveChanges();
                return test;
            }
        }

        [HttpPost("Tests/bfdr/Validator")]
        public async Task<Test> GetTestBFDRIJEValidator(int id)
        {
            using (var db = new RecordContext())
            {
                string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
                if (String.IsNullOrEmpty(input))
                {
                    return null;
                }
                BirthRecord record = VitalRecord.FromDescription<BirthRecord>(input);
                BirthTest test = new BirthTest(record);
                db.BirthTests.Add(test);
                db.SaveChanges();
                return test;
            }
        }

        /// <summary>
        /// Starts a new VRDR test.
        /// GET /api/tests/new
        /// </summary>
        [HttpGet("Tests/vrdr/New")]
        public Test NewVRDRTest()
        {
            using (var db = new RecordContext())
            {
                DeathTest test = new();
                db.DeathTests.Add(test);
                db.SaveChanges();
                return test;
            }
        }

        /// <summary>
        /// Starts a new VRDR test.
        /// GET /api/tests/new
        /// </summary>
        [HttpGet("Tests/bfdr/New")]
        public Test NewBFDRTest()
        {
            using (var db = new RecordContext())
            {
                BirthTest test = new();
                db.BirthTests.Add(test);
                db.SaveChanges();
                return test;
            }
        }

        /// <summary>
        /// Calculates test results.
        /// POST /api/tests/<type>/run/<id>
        /// </summary>
        [HttpPost("Tests/vrdr/{type}/Run/{id:int}")]
        public async Task<Test> RunVRDRTest(int id, string type)
        {
            using (var db = new RecordContext())
            {
                DeathTest test = db.DeathTests.Where(t => t.TestId == id).FirstOrDefault();
                string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
                if (!String.IsNullOrEmpty(input))
                {
                    test.Type = type;
                    test.Run<DeathRecord>(input);
                }
                db.DeathTests.Remove(test);
                db.SaveChanges();
                return test;
            }
        }

        /// <summary>
        /// Calculates test results.
        /// POST /api/tests/<type>/run/<id>
        /// </summary>
        [HttpPost("Tests/bfdr/{type}/Run/{id:int}")]
        public async Task<Test> RunBFDRTest(int id, string type)
        {
            using (var db = new RecordContext())
            {
                BirthTest test = db.BirthTests.Where(t => t.TestId == id).FirstOrDefault();
                string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
                if (!String.IsNullOrEmpty(input))
                {
                    test.Type = type;
                    test.Run<BirthRecord>(input);
                }
                db.BirthTests.Remove(test);
                db.SaveChanges();
                return test;
            }
        }

        [HttpPost("Tests/vrdr/{type}/Response")]
        public async Task<Dictionary<string, Message>> GetVRDRTestResponse(int id, string type)
        {
            return await GetTestResponse(id, type, (input) => new CanaryDeathMessage(input));
        }

        [HttpPost("Tests/bfdr/{type}/Response")]
        public async Task<Dictionary<string, Message>> GetBFDRTestResponse(int id, string type)
        {
            return await GetTestResponse(id, type, (input) => new CanaryBirthMessage(input));
        }

        private async Task<Dictionary<string, Message>> GetTestResponse(int id, string type, Func<string, Message> createMessage)
        {
            using (var db = new RecordContext())
            {
                string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
                if (String.IsNullOrEmpty(input))
                {
                    return null;
                }

                // get the responses for the submitted message
                Message msg = createMessage(input);
                Dictionary<string, Message> result = msg.GetResponsesFor(type);
                
                return result;
            }
        }
    }
}

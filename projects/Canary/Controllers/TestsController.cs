using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using canary.Models;
using VR;

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
        /// Gets a test by id.
        /// GET /api/tests/vrdr/1
        /// </summary>
        [HttpGet("Tests/{recordType:regex(^(" + ControllerMappers.VRDR + "|" + ControllerMappers.BFDR_BIRTH + "|" + ControllerMappers.BFDR_FETALDEATH + ")$)}/{id:int}")]
        public Test GetTest(string recordType, int id)
        {
            using RecordContext db = new();
            return ControllerMappers.dbTests[recordType](db).Where(t => t.TestId == id).FirstOrDefault();
        }

        /// <summary>
        /// Gets a pre-defined connectathon test by id with a certificate
        /// number and an optional state parameter
        /// GET /api/tests/vrdr/connectathon/1/1/AK
        /// </summary>
        [HttpGet("Tests/{recordType:regex(^(" + ControllerMappers.VRDR + "|" + ControllerMappers.BFDR_BIRTH + "|" + ControllerMappers.BFDR_FETALDEATH + ")$)}/Connectathon/{id:int}/{certificateNumber:int}/{state?}")]
        public Test GetTestConnectathon(string recordType, int id, int certificateNumber, string state)
        {
            Test test = ControllerMappers.createTestFromRecord[recordType](ControllerMappers.connectathonRecordsParams[recordType](id, certificateNumber, state));
            ControllerMappers.addDbTest[recordType](test);
            return test;
        }

        [HttpPost("Tests/{recordType:regex(^(" + ControllerMappers.VRDR + "|" + ControllerMappers.BFDR_BIRTH + "|" + ControllerMappers.BFDR_FETALDEATH + ")$)}/Validator")]
        public async Task<Test> GetTestIJEValidator(string recordType)
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }
            VitalRecord record = ControllerMappers.createRecordFromDescription[recordType](input);
            Test test = ControllerMappers.createTestFromRecord[recordType](record);
            ControllerMappers.addDbTest[recordType](test);
            return test;
        }

        /// <summary>
        /// Starts a new VRDR test.
        /// GET /api/tests/new
        /// </summary>
        [HttpGet("Tests/{recordType:regex(^(" + ControllerMappers.VRDR + "|" + ControllerMappers.BFDR_BIRTH + "|" + ControllerMappers.BFDR_FETALDEATH + ")$)}/New")]
        public Test NewTest(string recordType)
        {
            Test test = ControllerMappers.createEmptyTest[recordType]();
            ControllerMappers.addDbTest[recordType](test);
            return test;
        }

        /// <summary>
        /// Calculates test results.
        /// POST /api/tests/<type>/run/<id>
        /// </summary>
        [HttpPost("Tests/{recordType:regex(^(" + ControllerMappers.VRDR + "|" + ControllerMappers.BFDR_BIRTH + "|" + ControllerMappers.BFDR_FETALDEATH + ")$)}/{type}/Run/{id:int}")]
        public async Task<Test> RunTest(string recordType, string type, int id)
        {
            Console.WriteLine("LL" + recordType);
            Test test = GetTest(recordType, id);
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
            if (!String.IsNullOrEmpty(input))
            {
                test.Type = type;
                ControllerMappers.runTest[recordType](test, input);
            }
            ControllerMappers.removeDbTest[recordType](test);
            return test;
        }

        [HttpPost("Tests/{recordType:regex(^(" + ControllerMappers.VRDR + "|" + ControllerMappers.BFDR_BIRTH + "|" + ControllerMappers.BFDR_FETALDEATH + ")$)}/{type}/Response")]
        public async Task<Dictionary<string, Message>> GetTestResponse(string recordType, string type)
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }

            // get the responses for the submitted message
            Message msg = ControllerMappers.createMessageFromString[recordType](type);
            Dictionary<string, Message> result = msg.GetResponsesFor(type);
            
            return result;
        }
    }
}

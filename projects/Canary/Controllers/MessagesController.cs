using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VRDR;
using canary.Models;
using System.Reflection;
using BFDR;
using VR;
using VRDR.Interfaces;
using VRDR;

namespace canary.Controllers
{
    [ApiController]
    public class MessagesController : ControllerBase
    {
        /// <summary>
        /// Inspects a death message using the contents provided. Returns the message + record and any validation issues.
        /// POST Messages/vrdr/Inspect
        /// </summary>
        [HttpPost("Messages/vrdr/Inspect")]
        public async Task<(Record record, List<Dictionary<string, string>> issues)> NewVRDRPost()
        {
            return await NewPost<DeathRecord>((input) => BaseMessage.Parse(input, false));
        }

        /// <summary>
        /// Inspects a birth message using the contents provided. Returns the message + record and any validation issues.
        /// POST Messages/bfdr/Inspect
        /// </summary>
        [HttpPost("Messages/bfdr/Inspect")]
        public async Task<(Record record, List<Dictionary<string, string>> issues)> NewBFDRPost()
        {
            return await NewPost<BirthRecord>((input) => BirthRecordBaseMessage.Parse(input, false));
        }

        private async Task<(Record record, List<Dictionary<string, string>> issues)> NewPost<RecordType>(Func<string, CommonMessage> parser) where RecordType : VitalRecord, new()
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            if (!String.IsNullOrEmpty(input))
            {
                if (input.Trim().StartsWith("<") || input.Trim().StartsWith("{")) // XML or JSON?
                {
                    CommonMessage message = parser(input);
                    RecordType extracted = new();
                    foreach (PropertyInfo property in message.GetType().GetProperties())
                    {
                        if (property.PropertyType == typeof(RecordType))
                        {
                            extracted = (RecordType)property.GetValue(message);
                        }
                    }
                    string recordString = extracted.ToJSON();
                    List<Dictionary<string, string>> issues;
                    var messageInspectResults = typeof(RecordType) == typeof(DeathRecord) ? CanaryDeathRecord.CheckGet(recordString, false, out issues) : CanaryBirthRecord.CheckGet(recordString, false, out issues);

                    return (messageInspectResults, issues);
                }
                else
                {
                    return (null, new List<Dictionary<string, string>> 
                        { new Dictionary<string, string> { { "severity", "error" }, 
                            { "message", "The given input is not JSON or XML." } } }
                    );
                }
            }
            else
            {
                return (null, new List<Dictionary<string, string>> { new Dictionary<string, string> { { "severity", "error" }, { "message", "The given input appears to be empty." } } });
            }
        }

        /// <summary>
        /// Creates a new message using the contents provided. Returns the message and any validation issues.
        /// POST /api/messages/vrdr/new
        /// </summary>
        [HttpPost("Messages/vrdr/New")]
        public async Task<(Message message, List<Dictionary<string, string>> issues)> NewVRDRMessagePost()
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            try {

                BaseMessage message = BaseMessage.Parse(input, false);
                // If we were to return the Message here, the controller would automatically
                // serialize the message into a JSON object. Since that would happen outside of this
                // try/catch block, this would mean any errors would return a 500 and not display
                // them nicely to the user. By doing the serialization here and returning a string
                // we can nicely display any deserialization errors to the user.
                // One such error can be caused by removing the `<source>` endpoint from a Submission
                // message and then trying to validate it.
                JsonConvert.SerializeObject(message);
                //ICanaryDeathMessage canaryDeathMessage = 
                return (message: new CanaryDeathMessage(message), issues: new List<Dictionary<string, string>>());
            }
            catch (Exception e)
            {
                return (message: null, issues: Record.DecorateErrors(e));
            }
        }

        /// <summary>
        /// Creates a new message using the contents provided. Returns the message and any validation issues.
        /// POST /api/messages/bfdr/new
        /// </summary>
        [HttpPost("Messages/bfdr/New")]
        public async Task<(Message message, List<Dictionary<string, string>> issues)> NewBFDRMessagePost()
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            try {
                BirthRecordBaseMessage message = BirthRecordBaseMessage.Parse(input, false);
                // If we were to return the Message here, the controller would automatically
                // serialize the message into a JSON object. Since that would happen outside of this
                // try/catch block, this would mean any errors would return a 500 and not display
                // them nicely to the user. By doing the serialization here and returning a string
                // we can nicely display any deserialization errors to the user.
                // One such error can be caused by removing the `<source>` endpoint from a Submission
                // message and then trying to validate it.
                JsonConvert.SerializeObject(message);
                return (message: new CanaryBirthMessage(message), issues: new List<Dictionary<string, string>>());
            }
            catch (Exception e)
            {
                return (message: null, issues: Record.DecorateErrors(e));
            }
        }

        /// <summary>
        /// Creates a new VRDR message of the provided type using record provided
        /// POST /api/messages/vrdr/create?type={type}
        /// </summary>
        [HttpPost("Messages/vrdr/Create")]
        public async Task<(Message message, List<Dictionary<string, string>> issues)> NewVRDRMessageRecordPost(String type)
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            Record record = CanaryDeathRecord.CheckGet(input, false, out _);
            try {
                return (new CanaryDeathMessage(record, type), null);
            }
            catch (ArgumentException e) {
                return (null, new List<Dictionary<string, string>> { new Dictionary<string, string> { { "severity", "error" }, { "message", e.Message } } });
            }
        }

        /// <summary>
        /// Creates a new BFDR message of the provided type using record provided
        /// POST /api/messages/bfdr/create?type={type}
        /// </summary>
        [HttpPost("Messages/bfdr/Create")]
        public async Task<(Message message, List<Dictionary<string, string>> issues)> NewBFDRMessageRecordPost(String type)
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            Record record = CanaryBirthRecord.CheckGet(input, false, out _);
            try {
                return (new CanaryBirthMessage(record, type), null);
            }
            catch (ArgumentException e) {
                return (null, new List<Dictionary<string, string>> { new Dictionary<string, string> { { "severity", "error" }, { "message", e.Message } } });
            }
        }
  }
}

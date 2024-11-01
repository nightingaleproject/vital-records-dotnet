using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using canary.Models;
using System.Reflection;
using VR;

namespace canary.Controllers
{
    [ApiController]
    [Route("Messages")]
    public class MessagesController : ControllerBase
    {
        /// <summary>
        /// Inspects FSH message
        /// </summary>
        /// <returns></returns>
        [HttpPost("Messages/sushi/Inspect")]
        public async Task<(string results, List<Dictionary<string, string>> issues)> NewFshPost()
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            if (!String.IsNullOrEmpty(input))
            {
                //Verify input is FSH, check for Alias?
                if (input.Trim().StartsWith("Alias:")) // FSH?
                {
                    var messageInspectResults = Record.ValidateFshSushi(input);

                    var issueList = Record.ParseSushiErrorsAndWarnings(messageInspectResults);

                    return (results: messageInspectResults, issues: issueList);

                }
                else
                {
                    return (null, new List<Dictionary<string, string>>
                        { new Dictionary<string, string> { { "severity", "error" },
                            { "message", "The given input is not FSH." } } }
                    );
                }
            }
            else
            {
                return (null, new List<Dictionary<string, string>> { new Dictionary<string, string> { { "severity", "error" }, { "message", "The given input appears to be empty." } } });
            }
        }

        [HttpPost("Messages/vrdr/InspectWithFsh")]
        public async Task<(Record record, List<Dictionary<string, string>> issues)> NewVRDRFshPost()
        {
            return await NewPost("vrdr", true);
        }


        /// <summary>
        /// Inspects a deathrecord birthrecord or fetaldeathrecord message using the contents provided. Returns the message + record and any validation issues.
        /// POST Messages/vrdr/Inspect
        /// </summary>
        [HttpPost("{recordType:regex(^(" + ControllerMappers.VRDR + "|" + ControllerMappers.BFDR_BIRTH + "|" + ControllerMappers.BFDR_FETALDEATH + ")$)}/Inspect")]
        public async Task<(Record record, List<Dictionary<string, string>> issues)> NewPost(string recordType, bool getFsh = false)
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            if (!String.IsNullOrEmpty(input))
            {
                if (input.Trim().StartsWith("<") || input.Trim().StartsWith("{")) // XML or JSON?
                {
                    CommonMessage message = ControllerMappers.parseMessage[recordType](input);
                    string extractedRecordString = ControllerMappers.createEmptyRecord[recordType]().ToJSON();
                    foreach (PropertyInfo property in message.GetType().GetProperties())
                    {
                        if (property.PropertyType == typeof(VitalRecord))
                        {
                            extractedRecordString = ((VitalRecord)property.GetValue(message)).ToJSON();
                        }
                    }
                    return ControllerMappers.checkGetRecord[recordType](extractedRecordString, getFsh);
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
        /// POST /api/messages/{recordType:regex(^(" + ControllerMappers.VRDR + "|" + ControllerMappers.BFDR_BIRTH + "|" + ControllerMappers.BFDR_FETALDEATH + ")$)}/new
        /// </summary>
        [HttpPost("{recordType:regex(^(" + ControllerMappers.VRDR + "|" + ControllerMappers.BFDR_BIRTH + "|" + ControllerMappers.BFDR_FETALDEATH + ")$)}/New")]
        [Consumes("application/json")]
        public async Task<(Message message, List<Dictionary<string, string>> issues)> NewMessagePost(string recordType)
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            try {
                CommonMessage message = ControllerMappers.parseMessage[recordType](input);
                // If we were to return the Message here, the controller would automatically
                // serialize the message into a JSON object. Since that would happen outside of this
                // try/catch block, this would mean any errors would return a 500 and not display
                // them nicely to the user. By doing the serialization here and returning a string
                // we can nicely display any deserialization errors to the user.
                // One such error can be caused by removing the `<source>` endpoint from a Submission
                // message and then trying to validate it.
                JsonConvert.SerializeObject(message);
                return (message: ControllerMappers.createMessageFromMessage[recordType](message), issues: new List<Dictionary<string, string>>());
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
        [HttpPost("{recordType:regex(^(" + ControllerMappers.VRDR + "|" + ControllerMappers.BFDR_BIRTH + "|" + ControllerMappers.BFDR_FETALDEATH + ")$)}/Create")]
        public async Task<(Message message, List<Dictionary<string, string>> issues)> NewMessageRecordPost(string recordType, String type)
        {
            string input = await new StreamReader(Request.Body, Encoding.UTF8).ReadToEndAsync();

            (Record record, List<Dictionary<string, string>> _) = ControllerMappers.checkGetRecord[recordType](input, false);
            try {
                return (ControllerMappers.createMessageFromType[recordType](record, type), null);
            }
            catch (ArgumentException e) {
                return (null, new List<Dictionary<string, string>> { new Dictionary<string, string> { { "severity", "error" }, { "message", e.Message } } });
            }
        }
    }
}

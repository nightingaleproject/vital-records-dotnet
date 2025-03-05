using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using canary.Controllers;
using System.IO;
using Microsoft.AspNetCore.Http;
using BFDR;
using Newtonsoft.Json;

namespace canary.tests
{
    public class RecordTests
    {

        private RecordsController _recController;        
        string documentTypePayload = "";
        string messageTypePayload = "";
        string emptyTypePayload = "";
        string romeroIje = "";
        string romeroJson = "";

        public RecordTests()
        {
            documentTypePayload = File.ReadAllText(FixturePath("fixtures/json/DocumentTypePayload.json"));
            messageTypePayload = File.ReadAllText(FixturePath("fixtures/json/MessageTypePayload.json"));
            emptyTypePayload = File.ReadAllText(FixturePath("fixtures/json/EmptyTypePayload.json"));
            romeroIje = File.ReadAllText(FixturePath("fixtures/ije/romeroIje.ije"));
            romeroJson = File.ReadAllText(FixturePath("fixtures/json/BirthRecordR.json"));
            _recController = new RecordsController();
        }

        [Fact]
        public async void TestConverter()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(romeroIje));
            var httpContext = new DefaultHttpContext()
            {
                Request = { Body = stream, ContentLength = stream.Length }
            };
            _recController.ControllerContext.HttpContext = httpContext;
            var response = await _recController.NewPostAsync("bfdr-birth");
            ((BirthRecord) response.Item1.GetRecord()).EventLocationJurisdiction = "AZ";
            ((BirthRecord) response.Item1.GetRecord()).CertificateNumber = "99991";
            BirthRecord br = new BirthRecord(romeroJson);
            br.EventLocationJurisdiction = "AZ";
            br.CertificateNumber = "99991";

            Assert.Equal(JsonConvert.SerializeObject(br), JsonConvert.SerializeObject(new BirthRecord(response.Item1.Json)));
        }


        [Fact]
        public void TestDocumentTypePayload()
        {
            List<Dictionary<string, string>> issues;
            var resultData = canary.Models.CanaryDeathRecord.CheckGet(documentTypePayload, true, out issues);

            StringBuilder issueList = new StringBuilder();
            foreach(var issue in issues)
            {
                issueList.Append(string.Join("\n", issue.Select(p => "K=" + p.Key + ",L=" + p.Value)));
            }

            Assert.DoesNotContain("error", issueList.ToString());
        }

        [Fact]
        public void TestNonDocumentTypePayload()
        {
            List<Dictionary<string, string>> issues;
            Assert.Throws<BundleTypeException>(() => canary.Models.CanaryDeathRecord.CheckGet(messageTypePayload, true, out issues));
        }

        [Fact]
        public void TestMissingTypePayload()
        {
            List<Dictionary<string, string>> issues;
            var resultData = canary.Models.CanaryDeathRecord.CheckGet(emptyTypePayload, true, out issues);

            StringBuilder issueList = new StringBuilder();
            foreach (var issue in issues)
            {
                issueList.Append(string.Join("\n", issue.Select(p => "K=" + p.Key + ",L=" + p.Value)));
            }

            Assert.Contains("error", issueList.ToString()); 
        }

        public static string FixturePath(string filePath)
        {
            if (Path.IsPathRooted(filePath))
            {
                return filePath;
            }
            else
            {
                return Path.GetRelativePath(Directory.GetCurrentDirectory(), filePath);
            }
        }

    }

}

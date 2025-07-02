using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using canary.Controllers;
using System.IO;
using Microsoft.AspNetCore.Http;
using BFDR;
using Newtonsoft.Json;
using canary.Models;
using System;

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
        string invalidLocationBirthJson = "";
        string invalidLocationFetalDeathJson = "";
        string invalidLocationDeathJson = "";

        public RecordTests()
        {
            documentTypePayload = File.ReadAllText(FixturePath("fixtures/json/DocumentTypePayload.json"));
            messageTypePayload = File.ReadAllText(FixturePath("fixtures/json/MessageTypePayload.json"));
            emptyTypePayload = File.ReadAllText(FixturePath("fixtures/json/EmptyTypePayload.json"));
            romeroIje = File.ReadAllText(FixturePath("fixtures/ije/romeroIje.ije"));
            romeroJson = File.ReadAllText(FixturePath("fixtures/json/BirthRecordR.json"));
            invalidLocationBirthJson = File.ReadAllText(FixturePath("fixtures/json/BirthRecordInvalidLocation.json"));
            invalidLocationFetalDeathJson = File.ReadAllText(FixturePath("fixtures/json/FetalDeathRecordInvalidLocation.json"));
            invalidLocationDeathJson = File.ReadAllText(FixturePath("fixtures/json/DeathRecordInvalidLocation.json"));
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
            ((BirthRecord)response.Item1.GetRecord()).EventLocationJurisdiction = "AZ";
            ((BirthRecord)response.Item1.GetRecord()).CertificateNumber = "99991";
            BirthRecord br = new BirthRecord(romeroJson);
            br.EventLocationJurisdiction = "AZ";
            br.CertificateNumber = "99991";
            // The timezone has to be manually set here because the connectathon records are generated based on the local time zone. However, our test FHIR records have hard coded time zone data which must be updated to match the local time zone.
            string timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(new DateTime(2000, 1, 1)).ToString()[..6];
            if (timeZoneOffset == "00:00:")
            {
                timeZoneOffset = "+00:00";
            }
            Assert.Equal(JsonConvert.SerializeObject(br).Replace("-05:00", timeZoneOffset), JsonConvert.SerializeObject(new BirthRecord(response.Item1.Json)));
        }

        [Fact]
        public void IJEIssueBirthRecordCheck()
        {
            CanaryBirthRecord.CheckGet(invalidLocationBirthJson, false, out List<Dictionary<string, string>> issues);
            Assert.Single(issues);
            Assert.Contains("Unable to find IJE BPLACE mapping", issues[0]["message"]);
        }

        [Fact]
        public void IJEIssueFetalDeathRecordCheck()
        {
            CanaryFetalDeathRecord.CheckGet(invalidLocationFetalDeathJson, false, out List<Dictionary<string, string>> issues);
            Assert.Single(issues);
            Assert.Contains("Unable to find IJE DPLACE mapping", issues[0]["message"]);
        }

        [Fact]
        public void IJEIssueDeathRecordCheck()
        {
            CanaryDeathRecord.CheckGet(invalidLocationDeathJson, false, out List<Dictionary<string, string>> issues);
            Assert.Single(issues);
            Assert.Contains("Unable to find IJE DPLACE mapping", issues[0]["message"]);
        }

        [Fact]
        public void TestDocumentTypePayload()
        {
            List<Dictionary<string, string>> issues;
            var resultData = canary.Models.CanaryDeathRecord.CheckGet(documentTypePayload, true, out issues);

            StringBuilder issueList = new StringBuilder();
            foreach (var issue in issues)
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

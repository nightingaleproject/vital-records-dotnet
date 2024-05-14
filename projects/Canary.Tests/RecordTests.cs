using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using canary.Controllers;
using System.IO;
using Microsoft.AspNetCore.Http;
using VRDR;

using canary.Models;

namespace canary.tests
{
    public class RecordTests
    {
        string documentTypePayload = "";
        string messageTypePayload = "";
        string emptyTypePayload = "";

        public RecordTests()
        {
            documentTypePayload = File.ReadAllText(FixturePath("fixtures/json/DocumentTypePayload.json"));
            messageTypePayload = File.ReadAllText(FixturePath("fixtures/json/MessageTypePayload.json"));
            emptyTypePayload = File.ReadAllText(FixturePath("fixtures/json/EmptyTypePayload.json"));
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
            var resultData = canary.Models.CanaryDeathRecord.CheckGet(messageTypePayload, true, out issues);

            StringBuilder issueList = new StringBuilder();
            foreach (var issue in issues)
            {
                issueList.Append(string.Join("\n", issue.Select(p => "K=" + p.Key + ",L=" + p.Value)));
            }

            Assert.Contains("error", issueList.ToString());

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

        private string FixturePath(string filePath)
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

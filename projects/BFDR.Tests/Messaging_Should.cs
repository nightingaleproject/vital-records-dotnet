using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using Xunit;
using Hl7.Fhir.Model;
using VR;

namespace BFDR.Tests
{
    public class Messaging_Should
    {
        private BirthRecord record;

        public Messaging_Should()
        {
            // TODO: Should we also test with an XML record? And a JSON record without identifiers?
            record = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
        }

        [Fact]
        public void CreateEmptySubmission()
        {
            BirthRecordSubmissionMessage submission = new BirthRecordSubmissionMessage();
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", submission.MessageType);
            Assert.Null(submission.BirthRecord);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", submission.MessageDestination);
            Assert.NotNull(submission.MessageTimestamp);
            Assert.Null(submission.MessageSource);
            Assert.NotNull(submission.MessageId);
            Assert.Null(submission.CertNo);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Null(submission.NCHSIdentifier);
        }

        [Fact]
        public void CreateSubmissionFromBirthRecord()
        {
            // Test with fixture record
            BirthRecordSubmissionMessage submission = new BirthRecordSubmissionMessage(record);
            Assert.NotNull(submission.BirthRecord);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", submission.MessageType);
            Assert.Equal((uint)48858, submission.CertNo);
            Assert.Equal((uint)2019, submission.BirthYear);
            Assert.Equal("000000000042", submission.StateAuxiliaryId);
            Assert.Equal("2019UT048858", submission.NCHSIdentifier);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);
            Assert.Equal("48858", submission.BirthRecord.CertificateNumber);

            // Test with null record
            submission = new BirthRecordSubmissionMessage(null);
            Assert.Null(submission.BirthRecord);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", submission.MessageType);
            Assert.Null(submission.CertNo);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Null(submission.NCHSIdentifier);
        }

        [Fact]
        public void LoadSubmissionFromJSON()
        {
            BirthRecordSubmissionMessage submission = BirthRecordBaseMessage.Parse<BirthRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", submission.MessageType);
            Assert.Equal("2019UT048858", submission.NCHSIdentifier);
            Assert.Equal((uint)48858, submission.CertNo);
            Assert.Equal((uint)2019, submission.BirthYear);
            Assert.Equal("000000000042", submission.StateAuxiliaryId);
            Assert.Equal(submission.JurisdictionId, submission.BirthRecord.BirthLocationJurisdiction);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);
        }

        [Fact]
        public void CreateEmptyUpdate()
        {
            BirthRecordUpdateMessage submission = new BirthRecordUpdateMessage();
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission_update", submission.MessageType);
            Assert.Null(submission.BirthRecord);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", submission.MessageDestination);
            Assert.NotNull(submission.MessageTimestamp);
            Assert.Null(submission.MessageSource);
            Assert.NotNull(submission.MessageId);
            Assert.Null(submission.CertNo);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Null(submission.NCHSIdentifier);
        }

        [Fact]
        public void CreateUpdateFromBirthRecord()
        {
            // Test with fixture record
            BirthRecordUpdateMessage submission = new BirthRecordUpdateMessage(record);
            Assert.NotNull(submission.BirthRecord);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission_update", submission.MessageType);
            Assert.Equal((uint)48858, submission.CertNo);
            Assert.Equal((uint)2019, submission.BirthYear);
            Assert.Equal("000000000042", submission.StateAuxiliaryId);
            Assert.Equal("2019UT048858", submission.NCHSIdentifier);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);

            // Test with null record
            submission = new BirthRecordUpdateMessage(null);
            Assert.Null(submission.BirthRecord);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission_update", submission.MessageType);
            Assert.Null(submission.CertNo);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Null(submission.NCHSIdentifier);
        }

        [Fact]
        public void LoadUpdateFromJSON()
        {
            BirthRecordUpdateMessage submission = BirthRecordBaseMessage.Parse<BirthRecordUpdateMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordUpdateMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission_update", submission.MessageType);
            Assert.Equal("2019UT048858", submission.NCHSIdentifier);
            Assert.Equal((uint)48858, submission.CertNo);
            Assert.Equal((uint)2019, submission.BirthYear);
            Assert.Equal("000000000042", submission.StateAuxiliaryId);
            Assert.Equal(submission.JurisdictionId, submission.BirthRecord.BirthLocationJurisdiction);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);
            Assert.Equal("48858", submission.BirthRecord.CertificateNumber);
        }

        [Fact]
        public void CreateMultipleDestinationsMessage()
        {
            BirthRecordSubmissionMessage submission = new BirthRecordSubmissionMessage();
            string[] destinationsArray1 = new string[] { "dest1", "dest2" };
            string destinationsString1 = "dest1,dest2";
            string[] destinationsArray2 = new string[] { "dest3", "dest4" };
            string destinationsString2 = "dest3,dest4";
            submission.MessageDestinations = destinationsArray1.ToList();
            Assert.Equal(destinationsArray1, submission.MessageDestinations);
            Assert.Equal(destinationsString1, submission.MessageDestination);
            submission.MessageDestination = destinationsString2;
            Assert.Equal(destinationsArray2, submission.MessageDestinations);
            Assert.Equal(destinationsString2, submission.MessageDestination);
        }

       [Fact]
        public void CreateAckForMessage()
        {
            BirthRecordUpdateMessage submission = new BirthRecordUpdateMessage(record);
            BirthRecordAcknowledgementMessage ack = new BirthRecordAcknowledgementMessage(submission);
            Assert.Equal("http://nchs.cdc.gov/bfdr_acknowledgement", ack.MessageType);
            Assert.Equal(submission.MessageId, ack.AckedMessageId);
            Assert.Equal(submission.MessageSource, ack.MessageDestination);
            Assert.Equal(submission.MessageDestination, ack.MessageSource);
            Assert.Equal(submission.StateAuxiliaryId, ack.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, ack.CertNo);
            Assert.Equal(submission.NCHSIdentifier, ack.NCHSIdentifier);
        }

        [Fact]
        public void LoadAckFromJSON()
        {
            BirthRecordAcknowledgementMessage ack = BirthRecordBaseMessage.Parse<BirthRecordAcknowledgementMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordAcknowledgementMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/bfdr_acknowledgement", ack.MessageType);
            Assert.Equal("0df23820-f4ad-4a29-8862-a5effb85f1c5", ack.AckedMessageId);
            Assert.Equal("http://mitre.org/bfdr", ack.MessageDestination);
            Assert.Equal("2019UT048858", ack.NCHSIdentifier);
            Assert.Equal((uint)48858, ack.CertNo);
            Assert.Equal((uint)2019, ack.BirthYear);
            Assert.Equal("000000000042", ack.StateAuxiliaryId);
        }

        [Fact]
        public void CreateBirthRecordVoidMessage()
        {
            BirthRecordVoidMessage message = new BirthRecordVoidMessage();
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission_void", message.MessageType);
            Assert.Null(message.CertNo);
            message.CertNo = 11;
            Assert.Equal((uint)11, message.CertNo);
            Assert.Null(message.BirthYear);
            message.BirthYear = 2021;
            Assert.Equal((uint)2021, message.BirthYear);
            Assert.Null(message.JurisdictionId);
            message.JurisdictionId = "MA";
            Assert.Equal("MA", message.JurisdictionId);
            Assert.Equal("2021MA000011", message.NCHSIdentifier);
            Assert.Null(message.StateAuxiliaryId);
            message.StateAuxiliaryId = "0000123";
            Assert.Equal("0000123", message.StateAuxiliaryId);
            Assert.Null(message.BlockCount);
            message.BlockCount = 100;
            Assert.Equal((uint)100, message.BlockCount);
            message.BlockCount = 0;
            Assert.Equal((uint)0, message.BlockCount);
        }

        [Fact]
        public void LoadBirthRecordVoidMessageFromJson()
        {
            BirthRecordVoidMessage message = BirthRecordBaseMessage.Parse<BirthRecordVoidMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordVoidMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission_void", message.MessageType);
            Assert.Equal((uint)48858, message.CertNo);
            Assert.Equal((uint)10, message.BlockCount);
            Assert.Equal("000000000042", message.StateAuxiliaryId);
            Assert.Equal("2019UT048858", message.NCHSIdentifier);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
            Assert.Equal("http://mitre.org/bfdr", message.MessageSource);
        }

        [Fact]
        public void CreateAckForBirthRecordVoidMessage()
        {
            BirthRecordVoidMessage voidMessage = BirthRecordBaseMessage.Parse<BirthRecordVoidMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordVoidMessage.json"));
            BirthRecordAcknowledgementMessage ack = new BirthRecordAcknowledgementMessage(voidMessage);
            Assert.Equal("http://nchs.cdc.gov/bfdr_acknowledgement", ack.MessageType);
            Assert.Equal(voidMessage.MessageId, ack.AckedMessageId);
            Assert.Equal(voidMessage.MessageSource, ack.MessageDestination);
            Assert.Equal(voidMessage.MessageDestination, ack.MessageSource);
            Assert.Equal(voidMessage.StateAuxiliaryId, ack.StateAuxiliaryId);
            Assert.Equal(voidMessage.CertNo, ack.CertNo);
            Assert.Equal(voidMessage.NCHSIdentifier, ack.NCHSIdentifier);
            Assert.Equal(voidMessage.BlockCount, ack.BlockCount);
        }

        [Fact]
        public void CreateStatusMessage()
        {
            BirthRecordSubmissionMessage submission = new BirthRecordSubmissionMessage(record);
            BirthRecordStatusMessage status = new BirthRecordStatusMessage(submission, "manualDemographicCoding");
            Assert.Equal("http://nchs.cdc.gov/bfdr_status", status.MessageType);
            Assert.Equal("manualDemographicCoding", status.Status);
            Assert.Equal(submission.MessageId, status.StatusedMessageId);
            Assert.Equal(submission.MessageSource, status.MessageDestination);
            Assert.Equal(submission.MessageDestination, status.MessageSource);
            Assert.Equal(submission.StateAuxiliaryId, status.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, status.CertNo);
            Assert.Equal(submission.NCHSIdentifier, status.NCHSIdentifier);
        }

        [Fact]
        public void CreateAckForStatusMessage()
        {
            BirthRecordStatusMessage statusMessage = BirthRecordBaseMessage.Parse<BirthRecordStatusMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordStatusMessage.json"));
            BirthRecordAcknowledgementMessage ack = new BirthRecordAcknowledgementMessage(statusMessage);
            Assert.Equal("http://nchs.cdc.gov/bfdr_acknowledgement", ack.MessageType);
            Assert.Equal(statusMessage.MessageId, ack.AckedMessageId);
            Assert.Equal(statusMessage.MessageSource, ack.MessageDestination);
            Assert.Equal(statusMessage.MessageDestination, ack.MessageSource);
            Assert.Equal(statusMessage.StateAuxiliaryId, ack.StateAuxiliaryId);
            Assert.Equal(statusMessage.CertNo, ack.CertNo);
            Assert.Equal(statusMessage.NCHSIdentifier, ack.NCHSIdentifier);
        }

        [Fact]
        public void CreateExtractionErrorForMessage()
        {
            BirthRecordSubmissionMessage submission = BirthRecordBaseMessage.Parse<BirthRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessage.json"));
            BirthRecordErrorMessage err = new BirthRecordErrorMessage(submission);
            Assert.Equal("http://nchs.cdc.gov/bfdr_extraction_error", err.MessageType);
            Assert.Equal(submission.MessageId, err.FailedMessageId);
            Assert.Equal(submission.MessageSource, err.MessageDestination);
            Assert.Equal(submission.StateAuxiliaryId, err.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, err.CertNo);
            Assert.Equal(submission.NCHSIdentifier, err.NCHSIdentifier);
            Assert.Empty(err.Issues);
            var issues = new List<Issue>();
            var issue = new Issue(OperationOutcome.IssueSeverity.Fatal, OperationOutcome.IssueType.Invalid, "The message was invalid");
            issues.Add(issue);
            issue = new Issue(OperationOutcome.IssueSeverity.Warning, OperationOutcome.IssueType.Expired, "The message was very old");
            issues.Add(issue);
            err.Issues = issues;
            issues = err.Issues;
            Assert.Equal(2, (int)issues.Count);
            Assert.Equal(OperationOutcome.IssueSeverity.Fatal, issues[0].Severity);
            Assert.Equal(OperationOutcome.IssueType.Invalid, issues[0].Type);
            Assert.Equal("The message was invalid", issues[0].Description);
            Assert.Equal(OperationOutcome.IssueSeverity.Warning, issues[1].Severity);
            Assert.Equal(OperationOutcome.IssueType.Expired, issues[1].Type);
            Assert.Equal("The message was very old", issues[1].Description);
        }

        [Fact]
        public void LoadExtractionErrorFromJson()
        {
            BirthRecordErrorMessage err = BirthRecordBaseMessage.Parse<BirthRecordErrorMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordErrorMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/bfdr_extraction_error", err.MessageType);
            Assert.Equal((uint)1, err.CertNo);
            Assert.Equal("42", err.StateAuxiliaryId);
            Assert.Equal("2018MA000001", err.NCHSIdentifier);
            var issues = err.Issues;
            Assert.Equal(2, (int)issues.Count);
            Assert.Equal(OperationOutcome.IssueSeverity.Fatal, issues[0].Severity);
            Assert.Equal(OperationOutcome.IssueType.Invalid, issues[0].Type);
            Assert.Equal("The message was invalid", issues[0].Description);
            Assert.Equal(OperationOutcome.IssueSeverity.Warning, issues[1].Severity);
            Assert.Equal(OperationOutcome.IssueType.Expired, issues[1].Type);
            Assert.Equal("The message was very old", issues[1].Description);
        }

        // TODO: Once we have a real implementation of a demographic coding message implement real tests as well
        [Fact]
        public void CreateDemographicForMessage()
        {
            BirthRecordSubmissionMessage submission = BirthRecordBaseMessage.Parse<BirthRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessage.json"));
            BirthRecordDemographicsCodingMessage coding = new BirthRecordDemographicsCodingMessage(submission);
            Assert.Equal("http://nchs.cdc.gov/bfdr_demographics_coding", coding.MessageType);
            Assert.Equal(submission.MessageId, coding.CodedMessageId);
            Assert.Equal(submission.MessageSource, coding.MessageDestination);
            Assert.Equal(submission.MessageDestination, coding.MessageSource);
            Assert.Equal(submission.StateAuxiliaryId, coding.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, coding.CertNo);
            Assert.Equal(submission.NCHSIdentifier, coding.NCHSIdentifier);
        }

        [Fact]
        public void CreateDemographicCodingResponseFromJSON()
        {
            BirthRecordDemographicsCodingMessage message = BirthRecordBaseMessage.Parse<BirthRecordDemographicsCodingMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordDemographicsCodingMessage.json"));
            Assert.Equal(BirthRecordDemographicsCodingMessage.MESSAGE_TYPE, message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
            Assert.Equal((uint)100, message.CertNo);
            Assert.Equal((uint)2023, message.BirthYear);
            Assert.Equal("123", message.StateAuxiliaryId);
            Assert.Equal("2023YC000100", message.NCHSIdentifier);
            // TODO: Check demographic coding fields once implemented
        }

        [Fact]
        public void CreateDemographicsCodingUpdateFromJSON()
        {
            BirthRecordDemographicsCodingUpdateMessage message = BirthRecordBaseMessage.Parse<BirthRecordDemographicsCodingUpdateMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordDemographicsCodingUpdateMessage.json"));
            Assert.Equal(BirthRecordDemographicsCodingUpdateMessage.MESSAGE_TYPE, message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
            Assert.Equal((uint)100, message.CertNo);
            Assert.Equal((uint)2023, message.BirthYear);
            Assert.Equal("123", message.StateAuxiliaryId);
            Assert.Equal("2023YC000100", message.NCHSIdentifier);
            // TODO: Check demographic coding fields once implemented
        }

       [Fact]
        public void CreateDemographicCodingResponse()
        {
            // This test creates a response using the approach NCHS will use via IJE setters
            IJEBirth ije = new IJEBirth();
            ije.IDOB_YR = "2022";
            ije.BSTATE = "YC";
            ije.FILENO = "123";
            // TODO: Set the IJE fields that support demographic data
            BirthRecordDemographicsCodingMessage message = new BirthRecordDemographicsCodingMessage(ije.ToRecord());
            message.MessageSource = "http://nchs.cdc.gov/bfdr_submission";
            message.MessageDestination = "https://example.org/jurisdiction/endpoint";
            Assert.Equal(BirthRecordDemographicsCodingMessage.MESSAGE_TYPE, message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageSource);
            Assert.Equal("https://example.org/jurisdiction/endpoint", message.MessageDestination);
            Assert.Equal((uint)123, message.CertNo);
            Assert.Equal((uint)2022, message.BirthYear);
            Assert.Equal("2022YC000123", message.NCHSIdentifier);
            // TODO: Check demographic coding fields once implemented
        }

        [Fact]
        public void CreateDemographicCodingUpdate()
        {
            // This test creates a response using the approach NCHS will use via IJE setters
            IJEBirth ije = new IJEBirth();
            ije.IDOB_YR = "2022";
            ije.BSTATE = "YC";
            ije.FILENO = "123";
            // TODO: Set the IJE fields that support demographic data
            BirthRecordDemographicsCodingUpdateMessage message = new BirthRecordDemographicsCodingUpdateMessage(ije.ToRecord());
            message.MessageSource = "http://nchs.cdc.gov/bfdr_submission";
            message.MessageDestination = "https://example.org/jurisdiction/endpoint";
            Assert.Equal(BirthRecordDemographicsCodingUpdateMessage.MESSAGE_TYPE, message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageSource);
            Assert.Equal("https://example.org/jurisdiction/endpoint", message.MessageDestination);
            Assert.Equal((uint)123, message.CertNo);
            Assert.Equal((uint)2022, message.BirthYear);
            Assert.Equal("2022YC000123", message.NCHSIdentifier);
            // TODO: Check demographic coding fields once implemented
        }
    }
}

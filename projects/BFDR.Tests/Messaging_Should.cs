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
        private FetalDeathRecord fetalDeathRecord;

        public Messaging_Should()
        {
            // TODO: Should we also test with an XML record? And a JSON record without identifiers?
            record = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
            fetalDeathRecord = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicFetalDeathRecord.json")));
        }

        [Fact]
        public void CreateEmptyBirthSubmission()
        {
            BirthRecordSubmissionMessage submission = new BirthRecordSubmissionMessage();
            Assert.Equal("http://nchs.cdc.gov/birth_submission", submission.MessageType);
            Assert.Null(submission.BirthRecord);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", submission.MessageDestination);
            Assert.NotNull(submission.MessageTimestamp);
            Assert.Null(submission.MessageSource);
            Assert.NotNull(submission.MessageId);
            Assert.Null(submission.CertNo);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Null(submission.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
        }

        [Fact]
        public void CreateEmptyFetalDeathSubmission()
        {
            FetalDeathRecordSubmissionMessage submission = new FetalDeathRecordSubmissionMessage();
            Assert.Equal("http://nchs.cdc.gov/fd_submission", submission.MessageType);
            Assert.Null(submission.FetalDeathRecord);
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
            Assert.Equal("http://nchs.cdc.gov/birth_submission", submission.MessageType);
            Assert.Equal((uint)15075, submission.CertNo);
            Assert.Equal((uint)2019, submission.EventYear);
            Assert.Equal("444455555", submission.StateAuxiliaryId);
            Assert.Equal("2019UT015075", submission.NCHSIdentifier);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
            Assert.Equal("15075", submission.BirthRecord.CertificateNumber);

            // Test with null record
            submission = new BirthRecordSubmissionMessage(null);
            Assert.Null(submission.BirthRecord);
            Assert.Equal("http://nchs.cdc.gov/birth_submission", submission.MessageType);
            Assert.Null(submission.CertNo);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Null(submission.NCHSIdentifier);
        }

        [Fact]
        public void CreateSubmissionFromFetalDeathRecord()
        {
            // Test with fixture record
            FetalDeathRecordSubmissionMessage submission = new FetalDeathRecordSubmissionMessage(fetalDeathRecord);
            Assert.NotNull(submission.FetalDeathRecord);
            Assert.Equal("http://nchs.cdc.gov/fd_submission", submission.MessageType);
            Assert.Equal((uint)87366, submission.CertNo);
            Assert.Equal((uint)2020, submission.EventYear);
            Assert.Equal("444455555", submission.StateAuxiliaryId);
            Assert.Equal("NY", submission.JurisdictionId);
            Assert.Equal("2020NY087366", submission.NCHSIdentifier);
            Assert.Equal((uint)2020, submission.FetalDeathRecord.GetYear());
            Assert.Equal("87366", submission.FetalDeathRecord.CertificateNumber);

            // Test with null record
            submission = new FetalDeathRecordSubmissionMessage(null);
            Assert.Null(submission.FetalDeathRecord);
            Assert.Equal("http://nchs.cdc.gov/fd_submission", submission.MessageType);
            Assert.Null(submission.CertNo);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Null(submission.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
        }

        [Fact]
        public void LoadBirthSubmissionFromJSON()
        {
            BirthRecordSubmissionMessage submission = BFDRBaseMessage.Parse<BirthRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/birth_submission", submission.MessageType);
            Assert.Equal("2019UT048858", submission.NCHSIdentifier);
            Assert.Equal((uint)48858, submission.CertNo);
            Assert.Equal((uint)2019, submission.EventYear);
            Assert.Equal("000000000042", submission.StateAuxiliaryId);
            Assert.Equal(submission.JurisdictionId, submission.BirthRecord.BirthLocationJurisdiction);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);
            Assert.Null(submission.PayloadVersionId);
        }

        // TODO need example test message
        // [Fact]
        // public void LoadFetalDeathSubmissionFromJSON()
        // {
        //     FetalDeathRecordSubmissionMessage submission = BFDRBaseMessage.Parse<FetalDeathRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessage.json"));
        //     Assert.Equal("http://nchs.cdc.gov/fd_submission", submission.MessageType);
        //     Assert.Equal("2019UT048858", submission.NCHSIdentifier);
        //     Assert.Equal((uint)48858, submission.CertNo);
        //     Assert.Equal((uint)2019, submission.EventYear);
        //     Assert.Equal("000000000042", submission.StateAuxiliaryId);
        //     Assert.Equal(submission.JurisdictionId, submission.FetalDeathRecord.GetLocationJurisdiction());
        //     Assert.Equal(2019, submission.FetalDeathRecord.GetYear());
        // }

        [Fact]
        public void CreateEmptyUpdate()
        {
            BirthRecordUpdateMessage submission = new BirthRecordUpdateMessage();
            Assert.Equal("http://nchs.cdc.gov/birth_submission_update", submission.MessageType);
            Assert.Null(submission.BirthRecord);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", submission.MessageDestination);
            Assert.NotNull(submission.MessageTimestamp);
            Assert.Null(submission.MessageSource);
            Assert.NotNull(submission.MessageId);
            Assert.Null(submission.CertNo);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Null(submission.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
        }

        [Fact]
        public void CreateEmptyFetalDeathUpdate()
        {
            FetalDeathRecordUpdateMessage submission = new FetalDeathRecordUpdateMessage();
            Assert.Equal("http://nchs.cdc.gov/fd_submission_update", submission.MessageType);
            Assert.Null(submission.FetalDeathRecord);
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
            Assert.Equal("http://nchs.cdc.gov/birth_submission_update", submission.MessageType);
            Assert.Equal((uint)15075, submission.CertNo);
            Assert.Equal((uint)2019, submission.EventYear);
            Assert.Equal("444455555", submission.StateAuxiliaryId);
            Assert.Equal("2019UT015075", submission.NCHSIdentifier);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);

            // Test with null record
            submission = new BirthRecordUpdateMessage(null);
            Assert.Null(submission.BirthRecord);
            Assert.Equal("http://nchs.cdc.gov/birth_submission_update", submission.MessageType);
            Assert.Null(submission.CertNo);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Null(submission.NCHSIdentifier);
        }

        [Fact]
        public void CreateUpdateFromFetalDeathRecord()
        {
            // Test with fixture record
            FetalDeathRecordUpdateMessage submission = new FetalDeathRecordUpdateMessage(fetalDeathRecord);
            Assert.NotNull(submission.FetalDeathRecord);
            Assert.Equal("http://nchs.cdc.gov/fd_submission_update", submission.MessageType);
            Assert.Equal((uint)87366, submission.CertNo);
            Assert.Equal((uint)2020, submission.EventYear);
            Assert.Equal("444455555", submission.StateAuxiliaryId);
            Assert.Equal("NY", submission.JurisdictionId);
            Assert.Equal("2020NY087366", submission.NCHSIdentifier);
            Assert.Equal((uint)2020, submission.FetalDeathRecord.GetYear());

            // Test with null record
            submission = new FetalDeathRecordUpdateMessage(null);
            Assert.Null(submission.FetalDeathRecord);
            Assert.Equal("http://nchs.cdc.gov/fd_submission_update", submission.MessageType);
            Assert.Null(submission.CertNo);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Null(submission.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
        }

        [Fact]
        public void LoadUpdateFromJSON()
        {
            BirthRecordUpdateMessage submission = BFDRBaseMessage.Parse<BirthRecordUpdateMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordUpdateMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/birth_submission_update", submission.MessageType);
            Assert.Equal("2019UT048858", submission.NCHSIdentifier);
            Assert.Equal((uint)48858, submission.CertNo);
            Assert.Equal((uint)2019, submission.EventYear);
            Assert.Equal("000000000042", submission.StateAuxiliaryId);
            Assert.Equal(submission.JurisdictionId, submission.BirthRecord.BirthLocationJurisdiction);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
            Assert.Equal("48858", submission.BirthRecord.CertificateNumber);
        }

        // TODO need test example
        // [Fact]
        // public void LoadUpdateFetalDeathFromJSON()
        // {
        //     FetalDeathRecordUpdateMessage submission = BFDRBaseMessage.Parse<FetalDeathRecordUpdateMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordUpdateMessage.json"));
        //     Assert.Equal("http://nchs.cdc.gov/birth_submission_update", submission.MessageType);
        //     Assert.Equal("2019UT048858", submission.NCHSIdentifier);
        //     Assert.Equal((uint)48858, submission.CertNo);
        //     Assert.Equal((uint)2019, submission.EventYear);
        //     Assert.Equal("000000000042", submission.StateAuxiliaryId);
        //     Assert.Equal(submission.JurisdictionId, submission.FetalDeathRecord.BirthLocationJurisdiction);
        //     Assert.Equal(2019, submission.FetalDeathRecord.BirthYear);
        //     Assert.Equal("48858", submission.FetalDeathRecord.CertificateNumber);
        // }

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
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
        }

       [Fact]
        public void CreateAckForMessage()
        {
            BirthRecordUpdateMessage submission = new BirthRecordUpdateMessage(record);
            BirthRecordAcknowledgementMessage ack = new BirthRecordAcknowledgementMessage(submission);
            Assert.Equal("http://nchs.cdc.gov/birth_acknowledgement", ack.MessageType);
            Assert.Equal(submission.MessageId, ack.AckedMessageId);
            Assert.Equal(submission.MessageSource, ack.MessageDestination);
            Assert.Equal(submission.MessageDestination, ack.MessageSource);
            Assert.Equal(submission.StateAuxiliaryId, ack.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, ack.CertNo);
            Assert.Equal(submission.NCHSIdentifier, ack.NCHSIdentifier);
        }

        [Fact]
        public void CreateAckForFetalDeathMessage()
        {
            FetalDeathRecordUpdateMessage submission = new FetalDeathRecordUpdateMessage(fetalDeathRecord);
            FetalDeathRecordAcknowledgementMessage ack = new FetalDeathRecordAcknowledgementMessage(submission);
            Assert.Equal("http://nchs.cdc.gov/fd_acknowledgement", ack.MessageType);
            Assert.Equal(submission.MessageId, ack.AckedMessageId);
            Assert.Equal(submission.MessageSource, ack.MessageDestination);
            Assert.Equal(submission.MessageDestination, ack.MessageSource);
            Assert.Equal(submission.StateAuxiliaryId, ack.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, ack.CertNo);
            Assert.Equal(submission.NCHSIdentifier, ack.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
        }

        [Fact]
        public void LoadAckFromJSON()
        {
            BirthRecordAcknowledgementMessage ack = BFDRBaseMessage.Parse<BirthRecordAcknowledgementMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordAcknowledgementMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/birth_acknowledgement", ack.MessageType);
            Assert.Equal("0df23820-f4ad-4a29-8862-a5effb85f1c5", ack.AckedMessageId);
            Assert.Equal("http://mitre.org/bfdr", ack.MessageDestination);
            Assert.Equal("2019UT048858", ack.NCHSIdentifier);
            Assert.Equal((uint)48858, ack.CertNo);
            Assert.Equal((uint)2019, ack.EventYear);
            Assert.Equal("000000000042", ack.StateAuxiliaryId);
            Assert.Null(ack.PayloadVersionId);
        }

        [Fact]
        public void CreateBFDRVoidMessage()
        {
            BirthRecordVoidMessage message = new BirthRecordVoidMessage();
            Assert.Equal("http://nchs.cdc.gov/birth_submission_void", message.MessageType);
            Assert.Null(message.CertNo);
            message.CertNo = 11;
            Assert.Equal((uint)11, message.CertNo);
            Assert.Null(message.EventYear);
            message.EventYear = 2021;
            Assert.Equal((uint)2021, message.EventYear);
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
            Assert.Equal("BFDR_STU2_0", message.PayloadVersionId);
        }

        // TODO confirm if Voids exist in BFDR
        // [Fact]
        // public void LoadBFDRVoidMessageFromJson()
        // {
        //     BFDRVoidMessage message = BFDRBaseMessage.Parse<BFDRVoidMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordVoidMessage.json"));
        //     Assert.Equal("http://nchs.cdc.gov/bfdr_submission_void", message.MessageType);
        //     Assert.Equal((uint)48858, message.CertNo);
        //     Assert.Equal((uint)10, message.BlockCount);
        //     Assert.Equal("000000000042", message.StateAuxiliaryId);
        //     Assert.Equal("2019UT048858", message.NCHSIdentifier);
        //     Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
        //     Assert.Equal("http://mitre.org/bfdr", message.MessageSource);
        // }

        // [Fact]
        // public void CreateAckForBFDRVoidMessage()
        // {
        //     BFDRVoidMessage voidMessage = BFDRBaseMessage.Parse<BFDRVoidMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordVoidMessage.json"));
        //     BFDRAcknowledgementMessage ack = new BFDRAcknowledgementMessage(voidMessage);
        //     Assert.Equal("http://nchs.cdc.gov/birth_acknowledgement", ack.MessageType);
        //     Assert.Equal(voidMessage.MessageId, ack.AckedMessageId);
        //     Assert.Equal(voidMessage.MessageSource, ack.MessageDestination);
        //     Assert.Equal(voidMessage.MessageDestination, ack.MessageSource);
        //     Assert.Equal(voidMessage.StateAuxiliaryId, ack.StateAuxiliaryId);
        //     Assert.Equal(voidMessage.CertNo, ack.CertNo);
        //     Assert.Equal(voidMessage.NCHSIdentifier, ack.NCHSIdentifier);
        //     Assert.Equal(voidMessage.BlockCount, ack.BlockCount);
        // }

        [Fact]
        public void CreateStatusMessage()
        {
            BirthRecordSubmissionMessage submission = new BirthRecordSubmissionMessage(record);
            BirthRecordStatusMessage status = new BirthRecordStatusMessage(submission, "manualDemographicCoding");
            Assert.Equal("http://nchs.cdc.gov/birth_status", status.MessageType);
            Assert.Equal("manualDemographicCoding", status.Status);
            Assert.Equal(submission.MessageId, status.StatusedMessageId);
            Assert.Equal(submission.MessageSource, status.MessageDestination);
            Assert.Equal(submission.MessageDestination, status.MessageSource);
            Assert.Equal(submission.StateAuxiliaryId, status.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, status.CertNo);
            Assert.Equal(submission.NCHSIdentifier, status.NCHSIdentifier);
            Assert.Equal(submission.PayloadVersionId, status.PayloadVersionId);
            Assert.Equal("BFDR_STU2_0", status.PayloadVersionId);
        }

        [Fact]
        public void CreateAckForStatusMessage()
        {
            BirthRecordStatusMessage statusMessage = BFDRBaseMessage.Parse<BirthRecordStatusMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordStatusMessage.json"));
            BirthRecordAcknowledgementMessage ack = new BirthRecordAcknowledgementMessage(statusMessage);
            Assert.Equal("http://nchs.cdc.gov/birth_acknowledgement", ack.MessageType);
            Assert.Equal(statusMessage.MessageId, ack.AckedMessageId);
            Assert.Equal(statusMessage.MessageSource, ack.MessageDestination);
            Assert.Equal(statusMessage.MessageDestination, ack.MessageSource);
            Assert.Equal(statusMessage.StateAuxiliaryId, ack.StateAuxiliaryId);
            Assert.Equal(statusMessage.CertNo, ack.CertNo);
            Assert.Equal(statusMessage.NCHSIdentifier, ack.NCHSIdentifier);
            Assert.Null(statusMessage.PayloadVersionId);
            Assert.Equal("BFDR_STU2_0", ack.PayloadVersionId);
        }

        [Fact]
        public void CreateExtractionErrorForMessage()
        {
            BirthRecordSubmissionMessage submission = BFDRBaseMessage.Parse<BirthRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessage.json"));
            BirthRecordErrorMessage err = new BirthRecordErrorMessage(submission);
            Assert.Equal("http://nchs.cdc.gov/birth_extraction_error", err.MessageType);
            Assert.Equal(submission.MessageId, err.FailedMessageId);
            Assert.Equal(submission.MessageSource, err.MessageDestination);
            Assert.Equal(submission.StateAuxiliaryId, err.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, err.CertNo);
            Assert.Equal(submission.NCHSIdentifier, err.NCHSIdentifier);
            Assert.Null(submission.PayloadVersionId);
            Assert.Equal("BFDR_STU2_0", err.PayloadVersionId);
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
            BirthRecordErrorMessage err = BFDRBaseMessage.Parse<BirthRecordErrorMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordErrorMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/birth_extraction_error", err.MessageType);
            Assert.Equal((uint)1, err.CertNo);
            Assert.Equal("42", err.StateAuxiliaryId);
            Assert.Equal("2018MA000001", err.NCHSIdentifier);
            Assert.Null(err.PayloadVersionId);
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
            BirthRecordSubmissionMessage submission = BFDRBaseMessage.Parse<BirthRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessage.json"));
            BirthRecordParentalDemographicsCodingMessage coding = new BirthRecordParentalDemographicsCodingMessage(submission);
            Assert.Equal("http://nchs.cdc.gov/birth_demographics_coding", coding.MessageType);
            Assert.Equal(submission.MessageId, coding.CodedMessageId);
            Assert.Equal(submission.MessageSource, coding.MessageDestination);
            Assert.Equal(submission.MessageDestination, coding.MessageSource);
            Assert.Equal(submission.StateAuxiliaryId, coding.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, coding.CertNo);
            Assert.Equal(submission.NCHSIdentifier, coding.NCHSIdentifier);
            Assert.Null(submission.PayloadVersionId);
            Assert.Equal("BFDR_STU2_0", coding.PayloadVersionId);
        }

        [Fact]
        public void CreateDemographicCodingResponseFromJSON()
        {
            BirthRecordParentalDemographicsCodingMessage message = BFDRBaseMessage.Parse<BirthRecordParentalDemographicsCodingMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordDemographicsCodingMessage.json"));
            Assert.Equal(BirthRecordParentalDemographicsCodingMessage.MESSAGE_TYPE, message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
            Assert.Equal((uint)100, message.CertNo);
            Assert.Equal((uint)2023, message.EventYear);
            Assert.Equal("123", message.StateAuxiliaryId);
            Assert.Equal("2023YC000100", message.NCHSIdentifier);
            Assert.Null(message.PayloadVersionId);
            // TODO: Check demographic coding fields once implemented
        }

        [Fact]
        public void CreateDemographicsCodingUpdateFromJSON()
        {
            BirthRecordParentalDemographicsCodingUpdateMessage message = BFDRBaseMessage.Parse<BirthRecordParentalDemographicsCodingUpdateMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordDemographicsCodingUpdateMessage.json"));
            Assert.Equal(BirthRecordParentalDemographicsCodingUpdateMessage.MESSAGE_TYPE, message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
            Assert.Equal((uint)100, message.CertNo);
            Assert.Equal((uint)2023, message.EventYear);
            Assert.Equal("123", message.StateAuxiliaryId);
            Assert.Equal("2023YC000100", message.NCHSIdentifier);
            Assert.Null(message.PayloadVersionId);
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
            BirthRecordParentalDemographicsCodingMessage message = new BirthRecordParentalDemographicsCodingMessage(ije.ToRecord());
            message.MessageSource = "http://nchs.cdc.gov/bfdr_submission";
            message.MessageDestination = "https://example.org/jurisdiction/endpoint";
            Assert.Equal(BirthRecordParentalDemographicsCodingMessage.MESSAGE_TYPE, message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageSource);
            Assert.Equal("https://example.org/jurisdiction/endpoint", message.MessageDestination);
            Assert.Equal((uint)123, message.CertNo);
            Assert.Equal((uint)2022, message.EventYear);
            Assert.Equal("2022YC000123", message.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", message.PayloadVersionId);
            // TODO: Check demographic coding fields once implemented
        }

        [Fact]
        public void CreateDemographicCodingResponseForBirthJson()
        {
                // create an IJE birth message, convert it to FHIR, round trip the FHIR to json and back to make sure the bundles are all added to the json correctly
                IJEBirth ijeb = new IJEBirth();
                ijeb.MRACE1E = "199";
                ijeb.MRACE2E = "";
                ijeb.MRACE3E = "";
                ijeb.MRACE4E = "";
                ijeb.MRACE5E = "";
                ijeb.MRACE6E = "";
                ijeb.MRACE7E = "";
                ijeb.MRACE8E = "";

                ijeb.METHNIC1 = "N";
                ijeb.METHNIC2 = "N";                
                ijeb.METHNIC3 = "N";
                ijeb.METHNIC4 = "N";
                ijeb.METHNIC5 = "";
                ijeb.METHNICE = "100";
                ijeb.METHNIC5C = "";

                BirthRecord br = ijeb.ToRecord();
                BirthRecordParentalDemographicsCodingMessage msg = new BirthRecordParentalDemographicsCodingMessage(br);
                String msgJson = msg.ToJson();
                // parse the json and make sure the bundles are present
                BirthRecordParentalDemographicsCodingMessage message = BFDRBaseMessage.Parse<BirthRecordParentalDemographicsCodingMessage>(msgJson);
                NatalityRecord br2 = message.NatalityRecord;
                Assert.Equal("100", br2.MotherEthnicityEditedCodeHelper);
                Assert.Equal("199", br2.MotherRaceTabulation1EHelper);
        }

        [Fact]
        public void CreateDemographicCodingResponseForFetalDeathJson()
        {
                // create an IJE birth message, convert it to FHIR, round trip the FHIR to json and back to make sure the bundles are all added to the json correctly
                IJEFetalDeath ijefd = new IJEFetalDeath();
                ijefd.MRACE1E = "199";
                ijefd.MRACE2E = "";
                ijefd.MRACE3E = "";
                ijefd.MRACE4E = "";
                ijefd.MRACE5E = "";
                ijefd.MRACE6E = "";
                ijefd.MRACE7E = "";
                ijefd.MRACE8E = "";

                ijefd.METHNIC1 = "N";
                ijefd.METHNIC2 = "N";                
                ijefd.METHNIC3 = "N";
                ijefd.METHNIC4 = "N";
                ijefd.METHNIC5 = "";
                ijefd.METHNICE = "100";
                ijefd.METHNIC5C = "";

                FetalDeathRecord fdr = ijefd.ToRecord();
                FetalDeathRecordParentalDemographicsCodingMessage msg = new FetalDeathRecordParentalDemographicsCodingMessage(fdr);
                String msgJson = msg.ToJson();
                // parse the json and make sure the bundles are present
                FetalDeathRecordParentalDemographicsCodingMessage message = BFDRBaseMessage.Parse<FetalDeathRecordParentalDemographicsCodingMessage>(msgJson);
                NatalityRecord fdr2 = message.NatalityRecord;
                Assert.Equal("100", fdr2.MotherEthnicityEditedCodeHelper);
                Assert.Equal("199", fdr2.MotherRaceTabulation1EHelper);
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
            BirthRecordParentalDemographicsCodingUpdateMessage message = new BirthRecordParentalDemographicsCodingUpdateMessage(ije.ToRecord());
            message.MessageSource = "http://nchs.cdc.gov/bfdr_demographics_coding_update";
            message.MessageDestination = "https://example.org/jurisdiction/endpoint";
            Assert.Equal(BirthRecordParentalDemographicsCodingUpdateMessage.MESSAGE_TYPE, message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_demographics_coding_update", message.MessageSource);
            Assert.Equal("https://example.org/jurisdiction/endpoint", message.MessageDestination);
            Assert.Equal((uint)123, message.CertNo);
            Assert.Equal((uint)2022, message.EventYear);
            Assert.Equal("2022YC000123", message.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", message.PayloadVersionId);
            // TODO: Check demographic coding fields once implemented
        }

        [Fact]
        public void CreateCodedCauseOfFetalDeathMessageJson()
        {
                // create an IJE birth message, convert it to FHIR, round trip the FHIR to json and back to make sure the bundles are all added to the json correctly
                IJEFetalDeath ijefd = new IJEFetalDeath();
                ijefd.ICOD = "P011";

                FetalDeathRecord fdr = ijefd.ToRecord();
                CodedCauseOfFetalDeathMessage msg = new CodedCauseOfFetalDeathMessage(fdr);
                String msgJson = msg.ToJson();
                // parse the json and make sure the bundles are present
                CodedCauseOfFetalDeathMessage message = BFDRBaseMessage.Parse<CodedCauseOfFetalDeathMessage>(msgJson);
                FetalDeathRecord fdr2 = message.FetalDeathRecord;
                Assert.Equal("P01.1", fdr2.CodedInitiatingFetalCOD);
        }

        /// <summary>
        /// Tests the validation of the message header.
        /// </summary>
        /// <remarks>
        /// This test parses a message from a JSON fixture and validates its header.
        /// It ensures that the message is of type <see cref="BirthRecordSubmissionMessage"/>.
        /// It also verifies that a <see cref="MessageRuleException"/> is thrown when the message header is invalid,
        /// and checks that the exception message and source message type are correct.
        /// </remarks>
        /// <exception cref="MessageRuleException">
        /// Thrown when the message header validation fails due to the certificate number being more than 6 digits long.
        /// </exception>
        [Fact]
        public void ValidateMessageHeader()
        {
            var msg = BFDRBaseMessage.Parse(TestHelpers.FixtureStream("fixtures/json/MessageHeaderValidation.json"));
            Assert.IsType<BirthRecordSubmissionMessage>(msg);
            MessageRuleException ex = Assert.Throws<MessageRuleException>(() => CommonMessage.ValidateMessageHeader(msg));
            Assert.Equal("Message certificate number cannot be more than 6 digits long.", ex.Message);
            Assert.IsType<BirthRecordSubmissionMessage>(ex.SourceMessage);
        }

        /// <summary>
        /// Tests the validation of the message header.
        /// </summary>
        /// <remarks>
        /// This test parses a message from a JSON fixture and validates its header.
        /// It ensures that the message is of type <see cref="BirthRecordSubmissionMessage"/>.
        /// It also verifies that a <see cref="MessageRuleException"/> is thrown when the message header is invalid,
        /// and checks that the exception message and source message type are correct.
        /// </remarks>
        /// <exception cref="MessageRuleException">
        /// Thrown when the message header validation fails due to a missing event year.
        /// </exception>
        [Fact]
        public void ValidateMessageHeaderEventYear()
        {
            var msg = BFDRBaseMessage.Parse(TestHelpers.FixtureStream("fixtures/json/MessageHeaderValidationMissingYear.json"));
            Assert.IsType<BirthRecordSubmissionMessage>(msg);
            MessageRuleException ex = Assert.Throws<MessageRuleException>(() => CommonMessage.ValidateMessageHeader(msg));
            Assert.Equal("Message event year cannot be null.", ex.Message);
            Assert.IsType<BirthRecordSubmissionMessage>(ex.SourceMessage);
        }
    }
}

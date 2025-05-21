using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using Xunit;
using Hl7.Fhir.Model;
using VR;
using Hl7.Fhir.Utility;
using System.Collections.Concurrent;

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
            Assert.Equal(submission.JurisdictionId, submission.BirthRecord.EventLocationJurisdiction);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);
            Assert.Null(submission.PayloadVersionId);
        }

        [Fact]
        public void LoadFetalDeathSubmissionFromJSON()
        {
            FetalDeathRecordSubmissionMessage submission = BFDRBaseMessage.Parse<FetalDeathRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BasicFetalDeathRecordSubmissionMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/fd_submission", submission.MessageType);
            Assert.Equal("2025UT453723", submission.NCHSIdentifier);
            Assert.Equal((uint)453723, submission.CertNo);
            Assert.Equal((uint)2025, submission.EventYear);
            Assert.Null(submission.StateAuxiliaryId);
            Assert.Equal("MA", submission.FetalDeathRecord.EventLocationJurisdiction);
            Assert.Equal(2025, submission.FetalDeathRecord.DeliveryYear);
        }

        [Fact]
        public void BirthRecordSubmissionMissingBundle()
        {
            MessageParseException ex = Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse<BirthRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessageNoBundle.json"));
            });
            Assert.Contains("Error processing BirthRecord", ex.Message);
        }

        [Fact]
        public void FetalDeathSubmissionMissingBundle()
        {
            MessageParseException ex = Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse<FetalDeathRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BasicFetalDeathRecordSubmissionMessageNoBundle.json"));
            });
            Assert.Contains("Error processing FetalDeathRecord", ex.Message);
        }

        [Fact]
        public void CreateTypedMessageFromBundle()
        {
            Bundle bundle = CommonMessage.ParseGenericBundle(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BirthRecordSubmissionMessage.json")));
            BirthRecordSubmissionMessage submission = BFDRBaseMessage.Parse<BirthRecordSubmissionMessage>(bundle);
            Assert.Equal("http://nchs.cdc.gov/birth_submission", submission.MessageType);
            Assert.Equal("2019UT048858", submission.NCHSIdentifier);
            Assert.Equal((uint)48858, submission.CertNo);
            Assert.Equal((uint)2019, submission.EventYear);
            Assert.Equal("000000000042", submission.StateAuxiliaryId);
            Assert.Equal(submission.JurisdictionId, submission.BirthRecord.EventLocationJurisdiction);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);
            Assert.Null(submission.PayloadVersionId);
        }

        [Fact]
        public void WrongTypedMessageFromBundle()
        {
            Bundle bundle = CommonMessage.ParseGenericBundle(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BirthRecordSubmissionMessage.json")));
            Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse<FetalDeathRecordSubmissionMessage>(bundle);
            });
        }

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
            Assert.Equal(submission.JurisdictionId, submission.BirthRecord.EventLocationJurisdiction);
            Assert.Equal(2019, submission.BirthRecord.BirthYear);
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
            Assert.Equal("48858", submission.BirthRecord.CertificateNumber);
        }


        [Fact]
        public void LoadUpdateFetalDeathFromJSON()
        {
            FetalDeathRecordUpdateMessage submission = BFDRBaseMessage.Parse<FetalDeathRecordUpdateMessage>(TestHelpers.FixtureStream("fixtures/json/FetalDeathUpdateMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/fd_submission_update", submission.MessageType);
            Assert.Equal("2020NY087366", submission.NCHSIdentifier);
            Assert.Equal((uint)87366, submission.CertNo);
            Assert.Equal((uint)2020, submission.EventYear);
            Assert.Equal("444455555", submission.StateAuxiliaryId);
            Assert.Equal(submission.JurisdictionId, submission.FetalDeathRecord.EventLocationJurisdiction);
            Assert.Equal(2020, submission.FetalDeathRecord.DeliveryYear);
            Assert.Equal("87366", submission.FetalDeathRecord.CertificateNumber);
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
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
        }

        [Fact]
        public void CreateAckForBirthMessage()
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
        public void LoadBirthAckFromJSON()
        {
            BirthRecordAcknowledgementMessage ack = BFDRBaseMessage.Parse<BirthRecordAcknowledgementMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordAcknowledgementMessage.json"), true);
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
        public void LoadFetalDeathAckFromJSON()
        {
            FetalDeathRecordAcknowledgementMessage ack = BFDRBaseMessage.Parse<FetalDeathRecordAcknowledgementMessage>(TestHelpers.FixtureStream("fixtures/json/FetalDeathAcknowledgementMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/fd_acknowledgement", ack.MessageType);
            Assert.Equal("cd9fa913-55f2-4dfd-940d-11e77344362a", ack.AckedMessageId);
            Assert.Equal("http://nchs.cdc.gov/fd_submission", ack.MessageSource);
            Assert.Equal("http://mitre.org/bfdr", ack.MessageDestination);
            Assert.Equal("2022MA874232", ack.NCHSIdentifier);
            Assert.Equal((uint)874232, ack.CertNo);
            Assert.Equal((uint)2022, ack.EventYear);
            Assert.Equal("abcdef20", ack.StateAuxiliaryId);
            Assert.Equal("VRDRSTU2.2", ack.PayloadVersionId);
        }

        [Fact]
        public void CreateBirthAckFromParams()
        {
            string messageId = Guid.NewGuid().ToString();
            BirthRecordAcknowledgementMessage ack = new BirthRecordAcknowledgementMessage(messageId, "http://some.url.com/destination", "http://different.site.com/source");
            Assert.Equal("http://nchs.cdc.gov/birth_acknowledgement", ack.MessageType);
            Assert.Equal(messageId, ack.AckedMessageId);
            Assert.Equal("http://different.site.com/source", ack.MessageSource);
            Assert.Equal("http://some.url.com/destination", ack.MessageDestination);
            Assert.Null(ack.StateAuxiliaryId);
            Assert.Null(ack.CertNo);
            Assert.Null(ack.NCHSIdentifier);
        }

        [Fact]
        public void CreateFetalDeathAckFromParams()
        {
            string messageId = Guid.NewGuid().ToString();
            FetalDeathRecordAcknowledgementMessage ack = new FetalDeathRecordAcknowledgementMessage(messageId, "http://some.url.com/destination", "http://different.site.com/source");
            Assert.Equal("http://nchs.cdc.gov/fd_acknowledgement", ack.MessageType);
            Assert.Equal(messageId, ack.AckedMessageId);
            Assert.Equal("http://different.site.com/source", ack.MessageSource);
            Assert.Equal("http://some.url.com/destination", ack.MessageDestination);
            Assert.Null(ack.StateAuxiliaryId);
            Assert.Null(ack.CertNo);
            Assert.Null(ack.NCHSIdentifier);
        }

        [Fact]
        public void SetAcknowledgementMessageFields()
        {
            string messageId = Guid.NewGuid().ToString();
            BirthRecordAcknowledgementMessage ack = new BirthRecordAcknowledgementMessage(null);
            Assert.Null(ack.AckedMessageId);
            Assert.Null(ack.BlockCount);
            ack.BlockCount = 0;
            Assert.Null(ack.BlockCount);
            ack.AckedMessageId = messageId;
            ack.BlockCount = 4;
            Assert.Equal(messageId, ack.AckedMessageId);
            Assert.Equal((uint)4, ack.BlockCount);
            ack.BlockCount = 0;
            Assert.Null(ack.BlockCount);
            ack.BlockCount = 3;
            ack.BlockCount = null;
            Assert.Null(ack.BlockCount);
        }

        [Fact]
        public void CreateEmptyBirthVoidMessage()
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
            Assert.Equal((uint)1, message.BlockCount);
            message.BlockCount = 100;
            Assert.Equal((uint)100, message.BlockCount);
            message.BlockCount = 0;
            Assert.Equal((uint)0, message.BlockCount);
            Assert.Equal("BFDR_STU2_0", message.PayloadVersionId);
        }

        [Fact]
        public void CreateEmptyFetalDeathVoidMessage()
        {
            FetalDeathRecordVoidMessage message = new FetalDeathRecordVoidMessage();
            Assert.Equal("http://nchs.cdc.gov/fd_submission_void", message.MessageType);
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
            Assert.Equal((uint)1, message.BlockCount);
            message.BlockCount = 100;
            Assert.Equal((uint)100, message.BlockCount);
            message.BlockCount = 0;
            Assert.Equal((uint)0, message.BlockCount);
            Assert.Equal("BFDR_STU2_0", message.PayloadVersionId);
        }

        [Fact]
        public void CreateVoidFromBirthRecord()
        {
            // Test with fixture record
            BirthRecordVoidMessage message = new BirthRecordVoidMessage(record);
            Assert.Equal("http://nchs.cdc.gov/birth_submission_void", message.MessageType);
            Assert.Equal((uint)15075, message.CertNo);
            Assert.Equal((uint)2019, message.EventYear);
        }

        [Fact]
        public void CreateVoidFromFetalDeathRecord()
        {
            FetalDeathRecordVoidMessage message = new FetalDeathRecordVoidMessage(fetalDeathRecord);
            Assert.Equal("http://nchs.cdc.gov/fd_submission_void", message.MessageType);
            Assert.Equal((uint)87366, message.CertNo);
            Assert.Equal((uint)2020, message.EventYear);
        }

        [Fact]
        public void LoadBFDRVoidMessageFromJson()
        {
            BFDRVoidMessage message = BFDRBaseMessage.Parse<BFDRVoidMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordVoidMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/birth_submission_void", message.MessageType);
            Assert.Equal((uint)48858, message.CertNo);
            Assert.Equal((uint)10, message.BlockCount);
            Assert.Equal("000000000042", message.StateAuxiliaryId);
            Assert.Equal("2019UT048858", message.NCHSIdentifier);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
            Assert.Equal("http://mitre.org/bfdr", message.MessageSource);
        }

        [Fact]
        public void LoadFetalDeathVoidMessageFromJson()
        {
            BFDRVoidMessage message = BFDRBaseMessage.Parse<BFDRVoidMessage>(TestHelpers.FixtureStream("fixtures/json/FetalDeathVoidMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/fd_submission_void", message.MessageType);
            Assert.Equal((uint)63214, message.CertNo);
            Assert.Equal((uint)8, message.BlockCount);
            Assert.Equal("abc00020", message.StateAuxiliaryId);
            Assert.Equal("2022MA063214", message.NCHSIdentifier);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
            Assert.Equal("http://mitre.org/bfdr", message.MessageSource);
        }

        [Fact]
        public void LoadBFDRVoidMessageWithoutBlockCount()
        {
            BFDRVoidMessage message = BFDRBaseMessage.Parse<BFDRVoidMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordVoidMessageWithoutBlockCount.json"));
            Assert.Equal("http://nchs.cdc.gov/birth_submission_void", message.MessageType);
            Assert.Equal((uint)48858, message.CertNo);
            Assert.Equal((uint)1, message.BlockCount);
            Assert.Equal("000000000042", message.StateAuxiliaryId);
            Assert.Equal("2019UT048858", message.NCHSIdentifier);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
            Assert.Equal("http://mitre.org/bfdr", message.MessageSource);
        }

        [Fact]
        public void CreateAckForBFDRVoidMessage()
        {
            BFDRVoidMessage voidMessage = BFDRBaseMessage.Parse<BFDRVoidMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordVoidMessage.json"));
            BirthRecordAcknowledgementMessage ack = new BirthRecordAcknowledgementMessage(voidMessage);
            Assert.Equal("http://nchs.cdc.gov/birth_acknowledgement", ack.MessageType);
            Assert.Equal(voidMessage.MessageId, ack.AckedMessageId);
            Assert.Equal(voidMessage.MessageSource, ack.MessageDestination);
            Assert.Equal(voidMessage.MessageDestination, ack.MessageSource);
            Assert.Equal(voidMessage.StateAuxiliaryId, ack.StateAuxiliaryId);
            Assert.Equal(voidMessage.CertNo, ack.CertNo);
            Assert.Equal(voidMessage.NCHSIdentifier, ack.NCHSIdentifier);
            Assert.Equal(voidMessage.BlockCount, ack.BlockCount);
        }

        [Fact]
        public void CreateEmptyBirthRecordStatusMessage()
        {
            BirthRecordStatusMessage status = new BirthRecordStatusMessage();
            Assert.Equal("http://nchs.cdc.gov/birth_status", status.MessageType);
            Assert.Null(status.Status);
            Assert.Null(status.StatusedMessageId);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", status.MessageDestination); // http://nchs.cdc.gov/bfdr_submission
            Assert.Null(status.MessageSource);
            Assert.Null(status.StateAuxiliaryId);
            Assert.Null(status.CertNo);
            Assert.Null(status.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", status.PayloadVersionId);
            string differentMessageId = Guid.NewGuid().ToString();
            status.StatusedMessageId = differentMessageId;
            Assert.Equal(differentMessageId, status.StatusedMessageId);

        }

        [Fact]
        public void CreateBirthRecordStatusMessage()
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
        public void CreateBirthRecordStatusMessageFromParams()
        {
            string messageId = Guid.NewGuid().ToString();
            BirthRecordStatusMessage status = new BirthRecordStatusMessage(messageId, "http://some.url.com/destination", "manualDemographicCoding", "http://different.website.com/source");
            Assert.Equal("http://nchs.cdc.gov/birth_status", status.MessageType);
            Assert.Equal("manualDemographicCoding", status.Status);
            Assert.Equal(messageId, status.StatusedMessageId);
            Assert.Equal("http://some.url.com/destination", status.MessageDestination);
            Assert.Equal("http://different.website.com/source", status.MessageSource);
            Assert.Null(status.StateAuxiliaryId);
            Assert.Null(status.CertNo);
            Assert.Null(status.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", status.PayloadVersionId);
        }

        [Fact]
        public void CreateAckForBirthRecordStatusMessage()
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
        public void CreateBirthExtractionErrorForMessage()
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
            string differentMessageId = Guid.NewGuid().ToString();
            err.FailedMessageId = differentMessageId;
            Assert.Equal(differentMessageId, err.FailedMessageId);
        }

        [Fact]
        public void CreateFetalDeathExtractionErrorForMessage()
        {
            FetalDeathRecordSubmissionMessage submission = BFDRBaseMessage.Parse<FetalDeathRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BasicFetalDeathRecordSubmissionMessage.json"));
            FetalDeathRecordErrorMessage err = new FetalDeathRecordErrorMessage(submission);
            Assert.Equal("http://nchs.cdc.gov/fd_extraction_error", err.MessageType);
            Assert.Equal(submission.MessageId, err.FailedMessageId);
            Assert.Equal(submission.MessageSource, err.MessageDestination);
            Assert.Equal(submission.StateAuxiliaryId, err.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, err.CertNo);
            Assert.Equal(submission.NCHSIdentifier, err.NCHSIdentifier);
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
            string differentMessageId = Guid.NewGuid().ToString();
            err.FailedMessageId = differentMessageId;
            Assert.Equal(differentMessageId, err.FailedMessageId);
        }

        [Fact]
        public void LoadBirthExtractionErrorFromJson()
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

        [Fact]
        public void LoadFetalDeathExtractionErrorFromJson()
        {
            FetalDeathRecordErrorMessage err = BFDRBaseMessage.Parse<FetalDeathRecordErrorMessage>(TestHelpers.FixtureStream("fixtures/json/FetalDeathErrorMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/fd_extraction_error", err.MessageType);
            Assert.Equal((uint)874343, err.CertNo);
            Assert.Equal("abcdef20", err.StateAuxiliaryId);
            Assert.Equal("2022MA874343", err.NCHSIdentifier);
            Assert.Equal("VRDRSTU2.2", err.PayloadVersionId);
            var issues = err.Issues;
            Assert.Single(issues);
            Assert.Equal(OperationOutcome.IssueSeverity.Error, issues[0].Severity);
            Assert.Equal(OperationOutcome.IssueType.Required, issues[0].Type);
            Assert.Equal("something was not correct.", issues[0].Description);
        }

        [Fact]
        public void BirthExtractionErrorMissingOperationOutcome()
        {
            MessageParseException ex = Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse<BirthRecordErrorMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordErrorMessageNoOperationOutcome.json"));
            });
            Assert.Contains("Error processing OperationOutcome", ex.Message);
        }

        [Fact]
        public void CreateBirthExtractionErrorFromParams()
        {
            string messageId = Guid.NewGuid().ToString();
            BirthRecordErrorMessage err = new BirthRecordErrorMessage(messageId, "http://different.site.com/source", "http://different.site.com/source");
            Assert.Equal("http://nchs.cdc.gov/birth_extraction_error", err.MessageType);
            Assert.Equal(messageId, err.FailedMessageId);
            Assert.Null(err.CertNo);
            Assert.Null(err.StateAuxiliaryId);
            Assert.Null(err.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", err.PayloadVersionId);
            Assert.Empty(err.Issues);
        }

        [Fact]
        public void CreateFetalDeathExtractionErrorFromParams()
        {
            string messageId = Guid.NewGuid().ToString();
            FetalDeathRecordErrorMessage err = new FetalDeathRecordErrorMessage(messageId, "http://different.site.com/source", "http://different.site.com/source");
            Assert.Equal("http://nchs.cdc.gov/fd_extraction_error", err.MessageType);
            Assert.Equal(messageId, err.FailedMessageId);
            Assert.Null(err.CertNo);
            Assert.Null(err.StateAuxiliaryId);
            Assert.Null(err.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", err.PayloadVersionId);
            Assert.Empty(err.Issues);
        }

        [Fact]
        public void CreateEmptyFetalDeathStatusMessage()
        {
            FetalDeathRecordStatusMessage status = new FetalDeathRecordStatusMessage();
            Assert.Equal("http://nchs.cdc.gov/fd_status", status.MessageType);
            Assert.Null(status.Status);
            Assert.Null(status.StatusedMessageId);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", status.MessageDestination); // http://nchs.cdc.gov/bfdr_submission
            Assert.Null(status.MessageSource);
            Assert.Null(status.StateAuxiliaryId);
            Assert.Null(status.CertNo);
            Assert.Null(status.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", status.PayloadVersionId);
        }

        [Fact]
        public void CreateFetalDeathStatusMessage()
        {
            FetalDeathRecordSubmissionMessage submission = new FetalDeathRecordSubmissionMessage(fetalDeathRecord);
            FetalDeathRecordStatusMessage status = new FetalDeathRecordStatusMessage(submission, "manualCauseOfDeathCoding");
            Assert.Equal("http://nchs.cdc.gov/fd_status", status.MessageType);
            Assert.Equal("manualCauseOfDeathCoding", status.Status);
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
        public void CreateFetalDeathStatusMessageFromParams()
        {
            string messageId = Guid.NewGuid().ToString();
            FetalDeathRecordStatusMessage status = new FetalDeathRecordStatusMessage(messageId, "http://some.url.com/destination", "manualCauseOfDeathCoding", "http://different.website.com/source");
            Assert.Equal("http://nchs.cdc.gov/fd_status", status.MessageType);
            Assert.Equal("manualCauseOfDeathCoding", status.Status);
            Assert.Equal(messageId, status.StatusedMessageId);
            Assert.Equal("http://some.url.com/destination", status.MessageDestination);
            Assert.Equal("http://different.website.com/source", status.MessageSource);
            Assert.Null(status.StateAuxiliaryId);
            Assert.Null(status.CertNo);
            Assert.Null(status.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", status.PayloadVersionId);
        }

        [Fact]
        public void CreateAckForFetalDeathStatusMessage()
        {
            FetalDeathRecordStatusMessage statusMessage = BFDRBaseMessage.Parse<FetalDeathRecordStatusMessage>(TestHelpers.FixtureStream("fixtures/json/FetalDeathRecordStatusMessage.json"));
            FetalDeathRecordAcknowledgementMessage ack = new FetalDeathRecordAcknowledgementMessage(statusMessage);
            Assert.Equal("http://nchs.cdc.gov/fd_acknowledgement", ack.MessageType);
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
        public void CreateBirthRecordParentalDemographicFromSubmissionMessage()
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
        public void ModifyCodedMessageIdForBirthRecordParentalDemographicsCodingMessage()
        {
            BirthRecordSubmissionMessage submission = BFDRBaseMessage.Parse<BirthRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessage.json"));
            BirthRecordParentalDemographicsCodingMessage coding = new BirthRecordParentalDemographicsCodingMessage(submission);
            Assert.Equal(submission.MessageId, coding.CodedMessageId);
            string differentMessageId = Guid.NewGuid().ToString();
            coding.CodedMessageId = differentMessageId;
            Assert.Equal(differentMessageId, coding.CodedMessageId);
        }

        [Fact]
        public void CreateFetalDeathParentalDemographicFromSubmissionMessage()
        {
            FetalDeathRecordSubmissionMessage submission = BFDRBaseMessage.Parse<FetalDeathRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BasicFetalDeathRecordSubmissionMessage.json"));
            FetalDeathRecordParentalDemographicsCodingMessage coding = new FetalDeathRecordParentalDemographicsCodingMessage(submission);
            Assert.Equal("http://nchs.cdc.gov/fd_demographics_coding", coding.MessageType);
            Assert.Equal(submission.MessageId, coding.CodedMessageId);
            Assert.Equal(submission.MessageSource, coding.MessageDestination);
            Assert.Equal(submission.MessageDestination, coding.MessageSource);
            Assert.Equal(submission.StateAuxiliaryId, coding.StateAuxiliaryId);
            Assert.Equal(submission.CertNo, coding.CertNo);
            Assert.Equal(submission.NCHSIdentifier, coding.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", submission.PayloadVersionId);
            Assert.Equal("BFDR_STU2_0", coding.PayloadVersionId);
        }

        [Fact]
        public void CreateParentalDemographicCodingResponseFromJSON()
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
        public void CreateBirthRecordParentalDemographicsCodingUpdateFromJSON()
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
        public void CreateFetalDeathParentalDemographicsCodingUpdateFromJSON()
        {
            FetalDeathRecordParentalDemographicsCodingUpdateMessage message = BFDRBaseMessage.Parse<FetalDeathRecordParentalDemographicsCodingUpdateMessage>(TestHelpers.FixtureStream("fixtures/json/FetalDeathDemographicsCodingUpdateMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/fd_demographics_coding_update", message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageSource);
            Assert.Equal("https://example.com/jurisdiction/message/endpoint", message.MessageDestination);
            Assert.Equal((uint)55123, message.CertNo);
            Assert.Equal((uint)2022, message.EventYear);
            Assert.Equal("abcdef20", message.StateAuxiliaryId);
            Assert.Equal("2022MA055123", message.NCHSIdentifier);
            Assert.Equal("VRDRSTU2.2", message.PayloadVersionId);
        }

        [Fact]
        public void CreateParentalDemographicCodingResponse()
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
        public void CreateParentalDemographicCodingResponseForBirthJson()
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
        public void CreateParentalDemographicCodingResponseForFetalDeathJson()
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
        public void ParentalDemographicsCodingMessageWithoutNatalityRecord()
        {
            MessageParseException ex = Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse<BirthRecordParentalDemographicsCodingMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordDemographicsCodingMessageNoNatality.json"));
            });
            Assert.Contains("Error processing entry as BirthRecord or FetalDeathRecord", ex.Message);
        }

        [Fact]
        public void CreateBirthRecordParentalDemographicMessageFromParams()
        {
            string messageId = Guid.NewGuid().ToString();
            BirthRecordParentalDemographicsCodingMessage message = new BirthRecordParentalDemographicsCodingMessage(messageId, "https://example.org/jurisdiction/endpoint", "manualDemographicCoding", "http://nchs.cdc.gov/bfdr_submission");
            Assert.Equal("http://nchs.cdc.gov/birth_demographics_coding", message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageSource);
            Assert.Equal("https://example.org/jurisdiction/endpoint", message.MessageDestination);
            Assert.Equal(messageId, message.CodedMessageId);
        }

        [Fact]
        public void CreateFetalDeathParentalDemographicMessageFromParams()
        {
            string messageId = Guid.NewGuid().ToString();
            FetalDeathRecordParentalDemographicsCodingMessage message = new FetalDeathRecordParentalDemographicsCodingMessage(messageId, "https://example.org/jurisdiction/endpoint", "manualDemographicCoding", "http://nchs.cdc.gov/bfdr_submission");
            Assert.Equal("http://nchs.cdc.gov/fd_demographics_coding", message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageSource);
            Assert.Equal("https://example.org/jurisdiction/endpoint", message.MessageDestination);
            Assert.Equal(messageId, message.CodedMessageId);
        }

        [Fact]
        public void CreateBirthRecordDemographicCodingUpdate()
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
        public void CreateFetalDeathDemographicCodingUpdate()
        {
            IJEFetalDeath ije = new IJEFetalDeath();
            ije.FDOD_YR = "2022";
            ije.DSTATE = "TX";
            ije.FILENO = "234";
            FetalDeathRecordParentalDemographicsCodingUpdateMessage message = new FetalDeathRecordParentalDemographicsCodingUpdateMessage(ije.ToRecord());
            message.MessageSource = "http://nchs.cdc.gov/bfdr_demographics_coding_update";
            message.MessageDestination = "https://example.org/jurisdiction/endpoint";
            Assert.Equal(FetalDeathRecordParentalDemographicsCodingUpdateMessage.MESSAGE_TYPE, message.MessageType);
            Assert.Equal("http://nchs.cdc.gov/bfdr_demographics_coding_update", message.MessageSource);
            Assert.Equal("https://example.org/jurisdiction/endpoint", message.MessageDestination);
            Assert.Equal((uint)234, message.CertNo);
            Assert.Equal((uint)2022, message.EventYear);
            Assert.Equal("2022TX000234", message.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", message.PayloadVersionId);
        }

        [Fact]
        public void CreateEmptyCodedCauseOfFetalDeathMessage()
        {
            CodedCauseOfFetalDeathMessage message = new CodedCauseOfFetalDeathMessage();
            Assert.Equal("http://nchs.cdc.gov/fd_causeofdeath_coding", message.MessageType);
            Assert.Null(message.FetalDeathRecord);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
            Assert.Null(message.MessageSource);
            Assert.NotNull(message.MessageTimestamp);
            Assert.NotNull(message.MessageId);
            Assert.Null(message.CertNo);
            Assert.Null(message.StateAuxiliaryId);
            Assert.Null(message.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", message.PayloadVersionId);
        }

        [Fact]
        public void CreateCodedCauseOfFetalDeathMessageFromFetalDeathRecord()
        {
            CodedCauseOfFetalDeathMessage message = new CodedCauseOfFetalDeathMessage(fetalDeathRecord);
            Assert.NotNull(message.FetalDeathRecord);
            Assert.Equal("http://nchs.cdc.gov/fd_causeofdeath_coding", message.MessageType);
            Assert.Equal((uint)87366, message.CertNo);
            Assert.Equal((uint)2020, message.EventYear);
            Assert.Equal("444455555", message.StateAuxiliaryId);
            Assert.Equal("NY", message.JurisdictionId);
            Assert.Equal("2020NY087366", message.NCHSIdentifier);
            Assert.Equal((uint)2020, message.FetalDeathRecord.GetYear());
            Assert.Equal("87366", message.FetalDeathRecord.CertificateNumber);
        }

        [Fact]
        public void LoadCodedCauseOfFetalDeathMessageFromJson()
        {
            CodedCauseOfFetalDeathMessage message = BFDRBaseMessage.Parse<CodedCauseOfFetalDeathMessage>(TestHelpers.FixtureStream("fixtures/json/FetalDeathCodedCauseMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/fd_causeofdeath_coding", message.MessageType);
            Assert.Equal((uint)874232, message.CertNo);
            Assert.Equal("abcdef20", message.StateAuxiliaryId);
            Assert.Equal("2022MA874232", message.NCHSIdentifier);
            Assert.Equal("https://example.com/jurisdiction/message/endpoint", message.MessageDestination);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageSource);
            FetalDeathRecord fdr = message.FetalDeathRecord;
            Assert.NotNull(fdr);
        }

        [Fact]
        public void CodedCauseOfFetalDeathMessageMissingFetalDeathRecord()
        {
            MessageParseException ex = Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse<BirthRecordErrorMessage>(TestHelpers.FixtureStream("fixtures/json/FetalDeathCodedCauseMessageNoBundle.json"));
            });
            Assert.Contains("Error processing FetalDeathRecord", ex.Message);
        }

        [Fact]
        public void CreateEmptyCodedCauseOfDeathUpdateMessage()
        {
            CodedCauseOfFetalDeathUpdateMessage message = new CodedCauseOfFetalDeathUpdateMessage();
            Assert.Equal("http://nchs.cdc.gov/fd_causeofdeath_coding_update", message.MessageType);
            Assert.Null(message.FetalDeathRecord);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageDestination);
            Assert.Null(message.MessageSource);
            Assert.NotNull(message.MessageTimestamp);
            Assert.NotNull(message.MessageId);
            Assert.Null(message.CertNo);
            Assert.Null(message.StateAuxiliaryId);
            Assert.Null(message.NCHSIdentifier);
            Assert.Equal("BFDR_STU2_0", message.PayloadVersionId);
        }

        [Fact]
        public void CreateCodedCauseOfDeathUpdateMessageFromFetalDeathRecord()
        {
            CodedCauseOfFetalDeathUpdateMessage message = new CodedCauseOfFetalDeathUpdateMessage(fetalDeathRecord);
            Assert.NotNull(message.FetalDeathRecord);
            Assert.Equal("http://nchs.cdc.gov/fd_causeofdeath_coding_update", message.MessageType);
            Assert.Equal((uint)87366, message.CertNo);
            Assert.Equal((uint)2020, message.EventYear);
            Assert.Equal("444455555", message.StateAuxiliaryId);
            Assert.Equal("NY", message.JurisdictionId);
            Assert.Equal("2020NY087366", message.NCHSIdentifier);
            Assert.Equal((uint)2020, message.FetalDeathRecord.GetYear());
            Assert.Equal("87366", message.FetalDeathRecord.CertificateNumber);
        }

        [Fact]
        public void LoadCodedCauseOfDeathUpdateMessageFromJson()
        {
            CodedCauseOfFetalDeathUpdateMessage message = BFDRBaseMessage.Parse<CodedCauseOfFetalDeathUpdateMessage>(TestHelpers.FixtureStream("fixtures/json/FetalDeathCodedCauseUpdateMessage.json"));
            Assert.Equal("http://nchs.cdc.gov/fd_causeofdeath_coding_update", message.MessageType);
            Assert.Equal((uint)874232, message.CertNo);
            Assert.Equal("abcdef20", message.StateAuxiliaryId);
            Assert.Equal("2022MA874232", message.NCHSIdentifier);
            Assert.Equal("https://example.com/jurisdiction/message/endpoint", message.MessageDestination);
            Assert.Equal("http://nchs.cdc.gov/bfdr_submission", message.MessageSource);
            FetalDeathRecord fdr = message.FetalDeathRecord;
            Assert.NotNull(fdr);
        }

        [Fact]
        public void WrongMessageTypeWhenParsingJson()
        {
            Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse<FetalDeathRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessage.json"));
            });
        }

        [Fact(Skip = "Throws an ArgumentException, but maybe it shouldn't?")]
        public void HeadlessMessageCausesException()
        {
            Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse(TestHelpers.FixtureStream("fixtures/json/HeadlessMessage.json"));
            });
        }

        [Fact]
        public void MissingMessageTypeCausesException()
        {
            Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse(TestHelpers.FixtureStream("fixtures/json/MissingMessage.json"));
            });
        }

        [Fact]
        public void InvalidMessageTypeCausesException()
        {
            Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse(TestHelpers.FixtureStream("fixtures/json/InvalidMessage.json"));
            });
        }

        [Fact]
        public void CreateBirthRecordErrorMessageFromException()
        {
            MessageParseException ex = Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse(TestHelpers.FixtureStream("fixtures/json/InvalidMessage.json"));
            });
            BirthRecordErrorMessage message = ex.CreateBirthRecordExtractionErrorMessage();
            Assert.Single(message.Issues);
            Assert.Equal(OperationOutcome.IssueSeverity.Error, message.Issues[0].Severity);
            Assert.Equal(OperationOutcome.IssueType.Exception, message.Issues[0].Type);
            Assert.Equal(ex.Message, message.Issues[0].Description);
        }

        [Fact]
        public void CreateFetalDeathErrorMessageFromException()
        {
            MessageParseException ex = Assert.Throws<MessageParseException>(() =>
            {
                BFDRBaseMessage.Parse(TestHelpers.FixtureStream("fixtures/json/InvalidMessage.json"));
            });
            FetalDeathRecordErrorMessage message = ex.CreateFetalDeathRecordExtractionErrorMessage();
            Assert.Single(message.Issues);
            Assert.Equal(OperationOutcome.IssueSeverity.Error, message.Issues[0].Severity);
            Assert.Equal(OperationOutcome.IssueType.Exception, message.Issues[0].Type);
            Assert.Equal(ex.Message, message.Issues[0].Description);
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

        [Fact]
        public void GetNatalityRecordFromBirthRecordSubmissionMessage()
        {
            BirthRecordSubmissionMessage submission = new BirthRecordSubmissionMessage(record);
            NatalityRecord natality = BFDRBaseMessage.GetNatalityRecordFromMessage(submission);
            Assert.Equal(natality, record);
        }

        [Fact]
        public void GetNatalityRecordFromBirthRecordUpdateMessage()
        {
            BirthRecordUpdateMessage submission = new BirthRecordUpdateMessage(record);
            NatalityRecord natality = BFDRBaseMessage.GetNatalityRecordFromMessage(submission);
            Assert.Equal(natality, record);
        }

        [Fact(Skip = "GetNatalityRecordFromMessage needs fixing")]
        public void GetNatalityRecordFromBirthRecordParentalDemographicsCodingMessage()
        {
            BirthRecordSubmissionMessage submission = BFDRBaseMessage.Parse<BirthRecordSubmissionMessage>(TestHelpers.FixtureStream("fixtures/json/BirthRecordSubmissionMessage.json"));
            BirthRecordParentalDemographicsCodingMessage coding = new BirthRecordParentalDemographicsCodingMessage(submission);
            NatalityRecord natality = BFDRBaseMessage.GetNatalityRecordFromMessage(coding);
            Assert.Equal(natality, submission.BirthRecord);
        }

        [Fact]
        public void GetNatalityRecordFromFetalDeathRecordSubmissionMessage()
        {
            FetalDeathRecordSubmissionMessage submission = new FetalDeathRecordSubmissionMessage(fetalDeathRecord);
            NatalityRecord natality = BFDRBaseMessage.GetNatalityRecordFromMessage(submission);
            Assert.Equal(natality, fetalDeathRecord);
        }

        [Fact]
        public void GetNatalityRecordFromFetalDeathRecordUpdateMessage()
        {
            FetalDeathRecordUpdateMessage submission = new FetalDeathRecordUpdateMessage(fetalDeathRecord);
            NatalityRecord natality = BFDRBaseMessage.GetNatalityRecordFromMessage(submission);
            Assert.Equal(natality, fetalDeathRecord);
        }
    }
}

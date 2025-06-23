using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BFDR;

namespace BFDR.Tests
{
    public class Connectathon_Should
    {
        [Fact]
        public void Should_Return_BirthRecord_When_FromId_Called_With_Valid_Id()
        {
            var result1 = Connectathon.FromId(1);
            var result2 = Connectathon.FromId(2);
            var result3 = Connectathon.FromId(3);

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.IsType<BirthRecord>(result1);
            Assert.IsType<BirthRecord>(result2);
            Assert.IsType<BirthRecord>(result3);
        }

        [Fact]
        public void Should_Return_Null_When_FromId_Called_With_Invalid_Id()
        {
            var result = Connectathon.FromId(999);

            Assert.Null(result);
        }

        [Fact]
        public void Should_Set_CertificateNumber_When_FromId_Called_With_CertificateNumber()
        {
            var certificateNumber = 123456;

            var result = Connectathon.FromId(1, certificateNumber);

            Assert.NotNull(result);
            Assert.Equal(certificateNumber.ToString(), result.CertificateNumber);
        }

        [Fact]
        public void Should_Set_EventLocationJurisdiction_When_FromId_Called_With_State()
        {
            var state = "CA";

            var result = Connectathon.FromId(1, state: state);

            Assert.NotNull(result);
            Assert.Equal(state, result.EventLocationJurisdiction);
        }

        [Fact]
        public void Should_Set_BirthYear_When_FromId_Called_With_Year()
        {
            var year = 2023;

            var result = Connectathon.FromId(1, year: year);

            Assert.NotNull(result);
            Assert.Equal(year, result.BirthYear);
        }

        [Fact]
        public void Should_Set_All_Parameters_When_FromId_Called_With_All_Parameters()
        {
            var certificateNumber = 789012;
            var state = "TX";
            var year = 2022;

            var result = Connectathon.FromId(2, certificateNumber, state, year);

            Assert.NotNull(result);
            Assert.Equal(certificateNumber.ToString(), result.CertificateNumber);
            Assert.Equal(state, result.EventLocationJurisdiction);
            Assert.Equal(year, result.BirthYear);
        }

        [Fact]
        public void Should_Return_YytrfCardenasRomero_Record_When_YytrfCardenasRomero_Called()
        {
            var result = Connectathon.YytrfCardenasRomero();

            Assert.NotNull(result);
            Assert.IsType<BirthRecord>(result);
            // Verify some key properties from the IJE string
            Assert.Equal("YYTRF", result.ChildGivenNames[0]);
            Assert.Equal("CARDENAS ROMERO", result.ChildFamilyName);
            Assert.Equal("ALEJANDRA", result.MotherGivenNames[0]);
            Assert.Equal("ROMERO LEON", result.MotherFamilyName);
            Assert.Equal("RAMON", result.FatherGivenNames[0]);
            Assert.Equal("CARDENAS OTERO", result.FatherFamilyName);
        }

        [Fact]
        public void Should_Return_XyugbnxZalbanaiz_Record_When_XyugbnxZalbanaiz_Called()
        {
            var result = Connectathon.XyugbnxZalbanaiz();

            Assert.NotNull(result);
            Assert.IsType<BirthRecord>(result);
            // Verify some key properties from the IJE string
            Assert.Equal("XYUGBNX", result.ChildGivenNames[0]);
            Assert.Equal("ZALBANAIZ", result.ChildFamilyName);
            Assert.Equal("REEM", result.MotherGivenNames[0]);
            Assert.Equal("ALHAMADI", result.MotherFamilyName);
            Assert.Equal("OMAR", result.FatherGivenNames[0]);
            Assert.Equal("ALBANAI", result.FatherFamilyName);
        }

        [Fact]
        public void Should_Return_NullMonroe_Record_When_NullMonroe_Called()
        {
            var result = Connectathon.NullMonroe();

            Assert.NotNull(result);
            Assert.IsType<BirthRecord>(result);
            // Verify this is the null/empty data record
            Assert.Equal("Monroe", result.ChildFamilyName);
        }

        [Fact]
        public void Should_Return_Array_Of_BirthRecords_When_BirthRecords_Property_Accessed()
        {
            var result = Connectathon.BirthRecords;

            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
            Assert.All(result, record => Assert.IsType<BirthRecord>(record));
        }

        [Fact]
        public void Should_Return_FetalDeathRecord_When_FetalDeathRecordFromId_Called_With_Valid_Id()
        {
            var result = Connectathon.FetalDeathRecordFromId(1);

            Assert.NotNull(result);
            Assert.IsType<FetalDeathRecord>(result);
        }

        [Fact]
        public void Should_Return_Null_When_FetalDeathRecordFromId_Called_With_Invalid_Id()
        {
            var result = Connectathon.FetalDeathRecordFromId(999);

            Assert.Null(result);
        }

        [Fact]
        public void Should_Set_CertificateNumber_When_FetalDeathRecordFromId_Called_With_CertificateNumber()
        {
            var certificateNumber = 654321;

            var result = Connectathon.FetalDeathRecordFromId(1, certificateNumber);

            Assert.NotNull(result);
            Assert.Equal(certificateNumber.ToString(), result.CertificateNumber);
        }

        [Fact]
        public void Should_Set_EventLocationJurisdiction_When_FetalDeathRecordFromId_Called_With_State()
        {
            var state = "FL";

            var result = Connectathon.FetalDeathRecordFromId(1, state: state);

            Assert.NotNull(result);
            Assert.Equal(state, result.EventLocationJurisdiction);
        }

        [Fact]
        public void Should_Set_DeliveryYear_When_FetalDeathRecordFromId_Called_With_Year()
        {
            var year = 2021;

            var result = Connectathon.FetalDeathRecordFromId(1, year: year);

            Assert.NotNull(result);
            Assert.Equal(year, result.DeliveryYear);
        }

        [Fact]
        public void Should_Set_All_Parameters_When_FetalDeathRecordFromId_Called_With_All_Parameters()
        {
            var certificateNumber = 111222;
            var state = "WA";
            var year = 2020;

            var result = Connectathon.FetalDeathRecordFromId(1, certificateNumber, state, year);

            Assert.NotNull(result);
            Assert.Equal(certificateNumber.ToString(), result.CertificateNumber);
            Assert.Equal(state, result.EventLocationJurisdiction);
            Assert.Equal(year, result.DeliveryYear);
        }

        [Fact]
        public void Should_Return_Test1_Record_When_Test1_Called()
        {
            var result = Connectathon.Test1();

            Assert.NotNull(result);
            Assert.IsType<FetalDeathRecord>(result);
            // Verify some key properties from the IJE string
            Assert.Equal("Zeke", result.FetusGivenNames[0]);
            Assert.Equal("Anwar", result.FetusFamilyName);
        }

        [Fact]
        public void Should_Return_Array_Of_FetalDeathRecords_When_FetalDeathRecords_Property_Accessed()
        {
            var result = Connectathon.FetalDeathRecords;

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.All(result, record => Assert.IsType<FetalDeathRecord>(record));
        }

        [Fact]
        public void Should_Write_XML_File_When_WriteRecordAsXml_Called()
        {
            var record = Connectathon.FromId(1);
            var filename = "test_record.xml";

            var result = Connectathon.WriteRecordAsXml(record, filename);

            Assert.NotNull(result);
            Assert.Contains("<Bundle", result);
        }

        [Fact]
        public void Should_Return_Same_Record_When_FromId_Called_Multiple_Times_With_Same_Id()
        {
            var result1 = Connectathon.FromId(1);
            var result2 = Connectathon.FromId(1);

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            // Compare key properties to ensure they're the same record type
            Assert.Equal(result1.ChildFamilyName, result2.ChildFamilyName);
            Assert.Equal(result1.MotherFamilyName, result2.MotherFamilyName);
            Assert.Equal(result1.FatherFamilyName, result2.FatherFamilyName);
        }

        [Fact]
        public void Should_Not_Modify_Original_When_FromId_Called_With_Parameters()
        {
            var originalRecord = Connectathon.FromId(1);
            var originalCertNumber = originalRecord.CertificateNumber;
            var originalState = originalRecord.EventLocationJurisdiction;
            var originalYear = originalRecord.BirthYear;

            var modifiedRecord = Connectathon.FromId(1, 999999, "ZZ", 1999);

            // Verify original record wasn't modified
            var checkOriginal = Connectathon.FromId(1);
            Assert.Equal(originalCertNumber, checkOriginal.CertificateNumber);
            Assert.Equal(originalState, checkOriginal.EventLocationJurisdiction);
            Assert.Equal(originalYear, checkOriginal.BirthYear);

            // Verify modified record has new values
            Assert.Equal("999999", modifiedRecord.CertificateNumber);
            Assert.Equal("ZZ", modifiedRecord.EventLocationJurisdiction);
            Assert.Equal(1999, modifiedRecord.BirthYear);
        }

        [Fact]
        public void Should_Handle_Null_Parameters_Gracefully_When_FromId_Called()
        {
            var result = Connectathon.FromId(1, null, null, null);

            Assert.NotNull(result);
            // Should return the base record without modifications
            var baseRecord = Connectathon.FromId(1);
            Assert.Equal(baseRecord.CertificateNumber, result.CertificateNumber);
            Assert.Equal(baseRecord.EventLocationJurisdiction, result.EventLocationJurisdiction);
            Assert.Equal(baseRecord.BirthYear, result.BirthYear);
        }

        [Fact]
        public void Should_Handle_Null_Parameters_Gracefully_When_FetalDeathRecordFromId_Called()
        {
            var result = Connectathon.FetalDeathRecordFromId(1, null, null, null);

            Assert.NotNull(result);
            // Should return the base record without modifications
            var baseRecord = Connectathon.FetalDeathRecordFromId(1);
            Assert.Equal(baseRecord.CertificateNumber, result.CertificateNumber);
            Assert.Equal(baseRecord.EventLocationJurisdiction, result.EventLocationJurisdiction);
            Assert.Equal(baseRecord.DeliveryYear, result.DeliveryYear);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Should_Return_Valid_BirthRecord_For_All_Valid_Ids(int id)
        {
            var result = Connectathon.FromId(id);

            Assert.NotNull(result);
            Assert.IsType<BirthRecord>(result);
            Assert.NotNull(result.ChildFamilyName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        [InlineData(-1)]
        [InlineData(100)]
        public void Should_Return_Null_For_Invalid_BirthRecord_Ids(int id)
        {
            var result = Connectathon.FromId(id);

            Assert.Null(result);
        }

        [Theory]
        [InlineData(1)]
        public void Should_Return_Valid_FetalDeathRecord_For_Valid_Id(int id)
        {
            var result = Connectathon.FetalDeathRecordFromId(id);

            Assert.NotNull(result);
            Assert.IsType<FetalDeathRecord>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(-1)]
        [InlineData(100)]
        public void Should_Return_Null_For_Invalid_FetalDeathRecord_Ids(int id)
        {
            var result = Connectathon.FetalDeathRecordFromId(id);

            Assert.Null(result);
        }

        [Fact]
        public void Should_Generate_Records_With_Different_Properties()
        {
            var record1 = Connectathon.FromId(1);
            var record2 = Connectathon.FromId(2);
            var record3 = Connectathon.FromId(3);

            Assert.NotEqual(record1.ChildFamilyName, record2.ChildFamilyName);
            Assert.NotEqual(record2.ChildFamilyName, record3.ChildFamilyName);
            Assert.NotEqual(record1.ChildFamilyName, record3.ChildFamilyName);
        }

        [Fact]
        public void Should_Preserve_Record_Integrity_When_Parameters_Modified()
        {
            var originalRecord = Connectathon.FromId(1);

            var modifiedRecord = Connectathon.FromId(1, 12345, "NY", 2023);

            // Core record data should be preserved
            Assert.Equal(originalRecord.ChildFamilyName, modifiedRecord.ChildFamilyName);
            Assert.Equal(originalRecord.MotherFamilyName, modifiedRecord.MotherFamilyName);
            Assert.Equal(originalRecord.FatherFamilyName, modifiedRecord.FatherFamilyName);

            // Only specified parameters should be different
            Assert.NotEqual(originalRecord.CertificateNumber, modifiedRecord.CertificateNumber);
            Assert.NotEqual(originalRecord.EventLocationJurisdiction, modifiedRecord.EventLocationJurisdiction);
            Assert.NotEqual(originalRecord.BirthYear, modifiedRecord.BirthYear);
        }
    }
}

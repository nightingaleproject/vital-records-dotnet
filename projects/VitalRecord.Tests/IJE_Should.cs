using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using VR;
using Hl7.Fhir.Model;

namespace VitalRecord.Tests
{
    public class IJE_Should
    {
        // Mock VitalRecord subclass for testing
        private class MockVitalRecord : VR.VitalRecord
        {
            public MockVitalRecord() : base()
            {
                Bundle = new Bundle();
                Bundle.Id = Guid.NewGuid().ToString();
                Bundle.Type = Bundle.BundleType.Document;
                Bundle.Timestamp = DateTimeOffset.Now;
            }

            protected override void RestoreReferences()
            {
                // Mock implementation - no-op for testing
            }

            protected override string GetSectionFocusId(string section)
            {
                return "mock-focus-id";
            }

            protected override string[] CompositionSections
            {
                get => new string[] { };
            }

            public string TestStringValue { get; set; } = "";

            public string TestStringValueHelper { get; set; } = "mockFHIRCode";

            public bool? TestBoolValue { get; set; } = null;

            public int? TestIntValue { get; set; } = null;

            public Dictionary<string, string> TestDictionaryValue { get; set; } = null;
        }

        // Mock subclass for testing the abstract IJE base class
        private class MockIJE : IJE
        {
            public const uint LENGTH = 40;

            public MockVitalRecord _record;

            protected override uint IJELength => LENGTH;

            protected override VR.VitalRecord Record => _record;

            [IJEField(1, 1, 5, "Test Field 1", "TESTFLD1", 1)]
            public string TestField1 { get; set; } = "";

            [IJEField(2, 6, 10, "Test Field 2", "TESTFLD2", 2)]
            public string TestField2 { get; set; } = "";

            [IJEField(3, 16, 3, "Test Numeric Field", "TESTNUMF", 3)]
            public string TestNumericField { get; set; } = "";

            [IJEField(4, 19, 1, "Test Boolean Field", "TESTBOOL", 4)]
            public string TestBooleanField { get; set; } = "";

            [IJEField(5, 20, 4, "Test Time Field", "TESTTIME", 5)]
            public string TestTimeField { get; set; } = "";

            [IJEField(6, 24, 5, "Test Right Justified Field", "TESTRJST", 6)]
            public string TestRightJustifiedField { get; set; } = "";

            [IJEField(7, 29, 10, "Test Dictionary Field", "TESTDICT", 7)]
            public string TestDictionaryField { get; set; } = "";

            [IJEField(8, 39, 1, "Test Void Field", "TESTVOID", 8)]
            public string TestVoidField
            {
                get => Get_Void();
                set => Set_Void(value);
            }

            public MockIJE() : base()
            {
                _record = new MockVitalRecord();
            }

            public MockIJE(string ijeString) : base()
            {
                _record = new MockVitalRecord();
                ProcessIJE(ijeString, false);
            }

            // Expose protected methods for testing
            public new string LeftJustified_Get(string ijeFieldName, string fhirFieldName) => base.LeftJustified_Get(ijeFieldName, fhirFieldName);
            public new string Boolean_Get(string ijeFieldName, string fhirFieldName) => base.Boolean_Get(ijeFieldName, fhirFieldName);
            public new string NumericAllowingUnknown_Get(string ijeFieldName, string fhirFieldName) => base.NumericAllowingUnknown_Get(ijeFieldName, fhirFieldName);
            public new string RightJustifiedZeroed_Get(string ijeFieldName, string fhirFieldName) => base.RightJustifiedZeroed_Get(ijeFieldName, fhirFieldName);
            public new string LeftJustifiedValue(string ijeFieldName, string[] values, int pos = 0) => base.LeftJustifiedValue(ijeFieldName, values, pos);
            public new void Dictionary_Set(string ijeFieldName, string fhirFieldName, string key, string value) => base.Dictionary_Set(ijeFieldName, fhirFieldName, key, value);
            public new string Dictionary_Geo_Get(string ijeFieldName, string fhirFieldName, string keyPrefix, string geoType, bool isCoded) => base.Dictionary_Geo_Get(ijeFieldName, fhirFieldName, keyPrefix, geoType, isCoded);
            public new void Dictionary_Geo_Set(string ijeFieldName, string fhirFieldName, string keyPrefix, string geoType, bool isCoded, string value) => base.Dictionary_Geo_Set(ijeFieldName, fhirFieldName, keyPrefix, geoType, isCoded, value);
            public new string Get_MappingFHIRToIJE(Dictionary<string, string> mapping, string fhirField, string ijeField) => base.Get_MappingFHIRToIJE(mapping, fhirField, ijeField);
            public new void Set_MappingIJEToFHIR(Dictionary<string, string> mapping, string fhirField, string ijeField, string value) => base.Set_MappingIJEToFHIR(mapping, fhirField, ijeField, value);
            public List<string> GetValidationErrors() => validationErrors;
        }

        [Fact]
        public void IJELength_ShouldBeCorrect()
        {
            var mockIJE = new MockIJE();
            Assert.Equal(MockIJE.LENGTH, mockIJE.GetIJELength());
        }

        [Fact]
        public void RecordType_ShouldBeCorrect()
        {
            var mockIJE = new MockIJE();
            Assert.Contains("MockVitalRecord", mockIJE.GetTypeOfRecord());
        }

        [Fact]
        public void Constructor_WithEmptyString_ShouldInitializeAllFieldsToEmpty()
        {
            var mockIJE = new MockIJE();

            Assert.Equal("", mockIJE.TestField1);
            Assert.Equal("", mockIJE.TestField2);
            Assert.Equal("", mockIJE.TestNumericField);
            Assert.Equal("", mockIJE.TestBooleanField);
            Assert.Equal("", mockIJE.TestTimeField);
            Assert.Equal("", mockIJE.TestRightJustifiedField);
            Assert.Equal("", mockIJE.TestDictionaryField);
        }

        [Fact]
        public void Constructor_WithIJEString_ShouldParseFieldsCorrectly()
        {
            string ijeString = "ABCDE1234567890123T56789012345678901234567890";
            var mockIJE = new MockIJE(ijeString);

            Assert.Equal("ABCDE", mockIJE.TestField1);
            Assert.Equal("1234567890", mockIJE.TestField2);
            Assert.Equal("123", mockIJE.TestNumericField);
            Assert.Equal("T", mockIJE.TestBooleanField);
            Assert.Equal("5678", mockIJE.TestTimeField);
            Assert.Equal("90123", mockIJE.TestRightJustifiedField);
            Assert.Equal("4567890123", mockIJE.TestDictionaryField);
        }

        [Fact]
        public void Constructor_WithShortIJEString_ShouldPadWithSpaces()
        {
            string ijeString = "ABCDE";
            var mockIJE = new MockIJE(ijeString);

            Assert.Equal("ABCDE", mockIJE.TestField1);
            Assert.Equal("          ", mockIJE.TestField2); // Padded with spaces
            Assert.Equal("   ", mockIJE.TestNumericField); // Padded with spaces
        }

        [Fact]
        public void Constructor_WithNullString_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MockIJE(null));
        }

        [Fact]
        public void LeftJustified_Get_ShouldHandleNullAndLongValues()
        {
            var mockIJE = new MockIJE();
            mockIJE._record.TestStringValue = null;
            Assert.Equal("     ", mockIJE.LeftJustified_Get("TestField1", "TestStringValue"));
            mockIJE._record.TestStringValue = "1234567890";
            Assert.Equal("12345", mockIJE.LeftJustified_Get("TestField1", "TestStringValue"));
        }

        [Fact]
        public void LeftJustifiedValue_ShouldHandleNullAndIndexOutOfBounds()
        {
            var mockIJE = new MockIJE();
            Assert.Equal("     ", mockIJE.LeftJustifiedValue("TestField1", null, 0));
            Assert.Equal("     ", mockIJE.LeftJustifiedValue("TestField1", new string[1], 2));
        }

        [Fact]
        public void Boolean_Get_ShouldHandleNull()
        {
            var mockIJE = new MockIJE();
            Assert.Equal("U", mockIJE.Boolean_Get("TestBooleanField", "TestBoolValue"));
        }

        [Fact]
        public void NumericAllowingUnknown_Get_ShouldHandleLongValues()
        {
            var mockIJE = new MockIJE();
            mockIJE._record.TestIntValue = 1234567890;
            Assert.Equal("12345", mockIJE.NumericAllowingUnknown_Get("TestField1", "TestIntValue"));
            Assert.Contains("not the expected length for IJE field", mockIJE.GetValidationErrors()[0]);
        }

        [Fact]
        public void RightJustifiedZeroed_Get_ShouldHandleNullAndLongValues()
        {
            var mockIJE = new MockIJE();
            mockIJE._record.TestStringValue = null;
            Assert.Equal("00000", mockIJE.RightJustifiedZeroed_Get("TestField1", "TestStringValue"));
            mockIJE._record.TestStringValue = "1234567890";
            Assert.Equal("67890", mockIJE.RightJustifiedZeroed_Get("TestField1", "TestStringValue"));
        }

        [Fact]
        public void ToRecord_ShouldReturnTheRecord()
        {
            var mockIJE = new MockIJE();
            Assert.Same(mockIJE._record, mockIJE.ToRecord());
        }

        [Fact]
        public void Get_MappingFHIRToIJE_ShouldHandleMissingHelpersAndMappings()
        {
            var mockIJE = new MockIJE();
            var mapping = new Dictionary<string, string>();
            Assert.Throws<NullReferenceException>(() => mockIJE.Get_MappingFHIRToIJE(mapping, "TestStringValueMissing", "TestField1"));
            Assert.Equal("", mockIJE.Get_MappingFHIRToIJE(mapping, "TestStringValue", "COD"));
            Assert.Contains("Error: Unable to find IJE County of Death", mockIJE.GetValidationErrors()[0]);
            Assert.Equal("", mockIJE.Get_MappingFHIRToIJE(mapping, "TestStringValue", "COD1A"));
            Assert.Contains("Error: Unable to find IJE Cause of Death-1A", mockIJE.GetValidationErrors()[1]);
            Assert.Equal("", mockIJE.Get_MappingFHIRToIJE(mapping, "TestStringValue", "COD1B"));
            Assert.Contains("Error: Unable to find IJE Cause of Death-1B", mockIJE.GetValidationErrors()[2]);
            Assert.Equal("", mockIJE.Get_MappingFHIRToIJE(mapping, "TestStringValue", "COD1C"));
            Assert.Contains("Error: Unable to find IJE Cause of Death-1C", mockIJE.GetValidationErrors()[3]);
            Assert.Equal("", mockIJE.Get_MappingFHIRToIJE(mapping, "TestStringValue", "COD1D"));
            Assert.Contains("Error: Unable to find IJE Cause of Death-1D", mockIJE.GetValidationErrors()[4]);
        }

        [Fact]
        public void Set_MappingIJEToFHIR_ShouldHandleMissingHelpersAndMappings()
        {
            var mockIJE = new MockIJE();
            var mapping = new Dictionary<string, string>();
            Assert.Throws<NullReferenceException>(() => mockIJE.Set_MappingIJEToFHIR(mapping, "TestField1", "TestStringValueMissing", "value"));
            mockIJE.Set_MappingIJEToFHIR(mapping, "TestField1", "TestStringValue", "value");
            Assert.Contains("Error: Unable to find FHIR TestStringValue mapping", mockIJE.GetValidationErrors()[0]);
        }

        [Fact]
        public void Dictionary_Set_ShouldCreateDictionaryIfNull()
        {
            var mockIJE = new MockIJE();
            Assert.Null(mockIJE._record.TestDictionaryValue);
            mockIJE.Dictionary_Set("TestDictionaryField", "TestDictionaryValue", "key", "value");
            Assert.NotNull(mockIJE._record.TestDictionaryValue);
            Assert.True(mockIJE._record.TestDictionaryValue.ContainsKey("key"));
        }

        [Fact]
        public void Dictionary_Geo_Get_ShouldHandleNullAndInsideCityLimits()
        {
            var mockIJE = new MockIJE();
            var value = mockIJE.Dictionary_Geo_Get("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", true);
            Assert.Equal("          ", value);
            // Dictionary_Set ignores empty values
            mockIJE.Dictionary_Set("TestDictionaryField", "TestDictionaryValue", "InsideCityLimits", "");
            value = mockIJE.Dictionary_Geo_Get("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", true);
            Assert.Equal("          ", value);
            // Bypass Dictionary_Set to add an empty value
            mockIJE._record.TestDictionaryValue["InsideCityLimits"] = "";
            value = mockIJE.Dictionary_Geo_Get("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", true);
            Assert.Equal("U         ", value);
            mockIJE.Dictionary_Set("TestDictionaryField", "TestDictionaryValue", "InsideCityLimits", "true");
            value = mockIJE.Dictionary_Geo_Get("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", true);
            Assert.Equal("Y         ", value);
            mockIJE.Dictionary_Set("TestDictionaryField", "TestDictionaryValue", "InsideCityLimits", "false");
            value = mockIJE.Dictionary_Geo_Get("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", true);
            Assert.Equal("N         ", value);
            // Bypass Dictionary_Set to add an empty value
            mockIJE._record.TestDictionaryValue["InsideCityLimits"] = "";
            value = mockIJE.Dictionary_Geo_Get("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", false);
            Assert.Equal("          ", value);
        }

        [Fact]
        public void Dictionary_Geo_Set_ShouldHandleNullAndInsideCityLimits()
        {
            var mockIJE = new MockIJE();
            Assert.Null(mockIJE._record.TestDictionaryValue);
            mockIJE.Dictionary_Geo_Set("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", true, null);
            Assert.Null(mockIJE._record.TestDictionaryValue);
            mockIJE.Dictionary_Geo_Set("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", true, "N");
            Assert.NotNull(mockIJE._record.TestDictionaryValue);
            Assert.Equal("False", mockIJE._record.TestDictionaryValue["InsideCityLimits"]);
            var value = mockIJE.Dictionary_Geo_Get("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", true);
            Assert.Equal("N         ", value);
            mockIJE.Dictionary_Geo_Set("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", true, "Foo");
            Assert.NotNull(mockIJE._record.TestDictionaryValue);
            Assert.Equal("Foo", mockIJE._record.TestDictionaryValue["InsideCityLimits"]);
            value = mockIJE.Dictionary_Geo_Get("TestDictionaryField", "TestDictionaryValue", "", "insideCityLimits", true);
            Assert.Equal("Foo       ", value);
        }
    }
}

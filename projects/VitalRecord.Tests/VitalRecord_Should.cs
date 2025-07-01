using Hl7.Fhir.Model;
using Xunit;

namespace VitalRecord.Tests
{
    // The IJE base class is well tested by the BFDR.Tests and VRDR.Tests projects.
    // The tests in this file only check various edge cases not tested elsewhere.
    public class VitalRecord_Should
    {
        // Mock VitalRecord subclass for testing
        private class MockVitalRecord : VR.VitalRecord
        {
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

            // Expose protected methods for testing
            public new static FhirDateTime ConvertDateToFhirDateTime(Date value) => VR.VitalRecord.ConvertDateToFhirDateTime(value);
            public new static FhirDateTime ConvertToDateTime(string date) => VR.VitalRecord.ConvertToDateTime(date);
        }

        [Fact]
        public void ConvertDateToFhirDateTime_ShouldPreserveTheDate()
        {
            var now = Date.Today();
            var fhirDate = MockVitalRecord.ConvertDateToFhirDateTime(now);
            Assert.Equal(now.Value, fhirDate.Value);
        }

        [Fact]
        public void ConvertToDateTime_ShouldSupportJustYear()
        {
            var fhirDate = MockVitalRecord.ConvertToDateTime("2025");
            Assert.Equal("2025", fhirDate.Value);
        }
    }
}
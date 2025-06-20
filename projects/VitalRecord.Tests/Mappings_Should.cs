using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using VR;

namespace VitalRecord.Tests
{
    public class Mappings_Should
    {
        [Fact]
        public void BirthAttendantTitles_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.BirthAttendantTitles.IJEToFHIR;
            var fhirToIje = Mappings.BirthAttendantTitles.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("309343006", ijeToFhir["1"]);
            Assert.Equal("76231001", ijeToFhir["2"]);
            Assert.Equal("UNK", ijeToFhir["9"]);

            Assert.Equal("1", fhirToIje["309343006"]);
            Assert.Equal("2", fhirToIje["76231001"]);
            Assert.Equal("9", fhirToIje["UNK"]);
        }

        [Fact]
        public void BirthAttendantTitles_ShouldHaveReversibleMappings()
        {
            var ijeToFhir = Mappings.BirthAttendantTitles.IJEToFHIR;
            var fhirToIje = Mappings.BirthAttendantTitles.FHIRToIJE;

            foreach (var kvp in ijeToFhir)
            {
                Assert.True(fhirToIje.ContainsKey(kvp.Value), $"FHIR to IJE mapping missing for FHIR value: {kvp.Value}");
                Assert.Equal(kvp.Key, fhirToIje[kvp.Value]);
            }
        }

        [Fact]
        public void BirthSexChild_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.BirthSexChild.IJEToFHIR;
            var fhirToIje = Mappings.BirthSexChild.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("M", ijeToFhir["M"]);
            Assert.Equal("F", ijeToFhir["F"]);
            Assert.Equal("UNK", ijeToFhir["N"]);

            Assert.Equal("M", fhirToIje["M"]);
            Assert.Equal("F", fhirToIje["F"]);
            Assert.Equal("N", fhirToIje["UNK"]);
        }

        [Fact]
        public void BirthSexFetus_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.BirthSexFetus.IJEToFHIR;
            var fhirToIje = Mappings.BirthSexFetus.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("M", ijeToFhir["M"]);
            Assert.Equal("F", ijeToFhir["F"]);
            Assert.Equal("UNK", ijeToFhir["U"]);

            Assert.Equal("M", fhirToIje["M"]);
            Assert.Equal("F", fhirToIje["F"]);
            Assert.Equal("U", fhirToIje["UNK"]);
        }

        [Fact]
        public void EducationLevel_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.EducationLevel.IJEToFHIR;
            var fhirToIje = Mappings.EducationLevel.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("ELEM", ijeToFhir["1"]);
            Assert.Equal("HS", ijeToFhir["3"]);
            Assert.Equal("BA", ijeToFhir["6"]);
            Assert.Equal("UNK", ijeToFhir["9"]);

            Assert.Equal("1", fhirToIje["ELEM"]);
            Assert.Equal("3", fhirToIje["HS"]);
            Assert.Equal("6", fhirToIje["BA"]);
            Assert.Equal("9", fhirToIje["UNK"]);
        }

        [Fact]
        public void MaritalStatus_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.MaritalStatus.IJEToFHIR;
            var fhirToIje = Mappings.MaritalStatus.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("D", ijeToFhir["D"]);
            Assert.Equal("L", ijeToFhir["A"]);
            Assert.Equal("M", ijeToFhir["M"]);
            Assert.Equal("S", ijeToFhir["S"]);
            Assert.Equal("W", ijeToFhir["W"]);
            Assert.Equal("UNK", ijeToFhir["U"]);

            Assert.Equal("D", fhirToIje["D"]);
            Assert.Equal("A", fhirToIje["L"]);
            Assert.Equal("M", fhirToIje["M"]);
            Assert.Equal("S", fhirToIje["S"]);
            Assert.Equal("W", fhirToIje["W"]);
            Assert.Equal("U", fhirToIje["UNK"]);
        }

        [Fact]
        public void MaritalStatus_ShouldHandleSpecialCase_T()
        {
            var fhirToIje = Mappings.MaritalStatus.FHIRToIJE;

            Assert.Equal("U", fhirToIje["T"]);
        }

        [Fact]
        public void HispanicNoUnknown_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.HispanicNoUnknown.IJEToFHIR;
            var fhirToIje = Mappings.HispanicNoUnknown.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("Y", ijeToFhir["H"]);
            Assert.Equal("N", ijeToFhir["N"]);
            Assert.Equal("UNK", ijeToFhir["U"]);

            Assert.Equal("H", fhirToIje["Y"]);
            Assert.Equal("N", fhirToIje["N"]);
            Assert.Equal("U", fhirToIje["UNK"]);
        }

        [Fact]
        public void YesNoUnknown_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.YesNoUnknown.IJEToFHIR;
            var fhirToIje = Mappings.YesNoUnknown.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("N", ijeToFhir["N"]);
            Assert.Equal("Y", ijeToFhir["Y"]);
            Assert.Equal("UNK", ijeToFhir["U"]);

            Assert.Equal("N", fhirToIje["N"]);
            Assert.Equal("Y", fhirToIje["Y"]);
            Assert.Equal("U", fhirToIje["UNK"]);
        }

        [Fact]
        public void YesNoNotApplicable_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.YesNoNotApplicable.IJEToFHIR;
            var fhirToIje = Mappings.YesNoNotApplicable.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("N", ijeToFhir["N"]);
            Assert.Equal("Y", ijeToFhir["Y"]);
            Assert.Equal("NA", ijeToFhir["X"]);

            Assert.Equal("N", fhirToIje["N"]);
            Assert.Equal("Y", fhirToIje["Y"]);
            Assert.Equal("X", fhirToIje["NA"]);
        }

        [Fact]
        public void YesNoUnknownNotApplicable_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.YesNoUnknownNotApplicable.IJEToFHIR;
            var fhirToIje = Mappings.YesNoUnknownNotApplicable.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("N", ijeToFhir["N"]);
            Assert.Equal("Y", ijeToFhir["Y"]);
            Assert.Equal("NA", ijeToFhir["X"]);
            Assert.Equal("UNK", ijeToFhir["U"]);

            Assert.Equal("N", fhirToIje["N"]);
            Assert.Equal("Y", fhirToIje["Y"]);
            Assert.Equal("X", fhirToIje["NA"]);
            Assert.Equal("U", fhirToIje["UNK"]);
        }

        [Fact]
        public void UnitsOfAge_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.UnitsOfAge.IJEToFHIR;
            var fhirToIje = Mappings.UnitsOfAge.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("a", ijeToFhir["1"]);
            Assert.Equal("mo", ijeToFhir["2"]);
            Assert.Equal("d", ijeToFhir["4"]);
            Assert.Equal("h", ijeToFhir["5"]);
            Assert.Equal("min", ijeToFhir["6"]);
            Assert.Equal("UNK", ijeToFhir["9"]);

            Assert.Equal("1", fhirToIje["a"]);
            Assert.Equal("2", fhirToIje["mo"]);
            Assert.Equal("4", fhirToIje["d"]);
            Assert.Equal("5", fhirToIje["h"]);
            Assert.Equal("6", fhirToIje["min"]);
            Assert.Equal("9", fhirToIje["UNK"]);
        }

        [Fact]
        public void RaceMissingValueReason_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.RaceMissingValueReason.IJEToFHIR;
            var fhirToIje = Mappings.RaceMissingValueReason.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("ASKU", ijeToFhir["S"]);
            Assert.Equal("UNK", ijeToFhir["C"]);
            Assert.Equal("PREFUS", ijeToFhir["R"]);

            Assert.Equal("S", fhirToIje["ASKU"]);
            Assert.Equal("C", fhirToIje["UNK"]);
            Assert.Equal("R", fhirToIje["PREFUS"]);
        }

        [Fact]
        public void EditBypass01234_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.EditBypass01234.IJEToFHIR;
            var fhirToIje = Mappings.EditBypass01234.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test that all values 0-4 are mapped
            for (int i = 0; i <= 4; i++)
            {
                string key = i.ToString();
                Assert.True(ijeToFhir.ContainsKey(key), $"IJE to FHIR mapping missing for key: {key}");
                Assert.True(fhirToIje.ContainsKey(key), $"FHIR to IJE mapping missing for key: {key}");
                Assert.Equal(key, ijeToFhir[key]);
                Assert.Equal(key, fhirToIje[key]);
            }
        }

        [Fact]
        public void HispanicOrigin_ShouldHaveExtensiveMappings()
        {
            var ijeToFhir = Mappings.HispanicOrigin.IJEToFHIR;
            var fhirToIje = Mappings.HispanicOrigin.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test that mappings are extensive (should have many entries)
            Assert.True(ijeToFhir.Count > 50, "HispanicOrigin should have many mappings");
            Assert.Equal(ijeToFhir.Count, fhirToIje.Count);

            // Test some specific mappings
            Assert.Equal("100", ijeToFhir["100"]);
            Assert.Equal("200", ijeToFhir["200"]);
            Assert.Equal("999", ijeToFhir["999"]);

            Assert.Equal("100", fhirToIje["100"]);
            Assert.Equal("200", fhirToIje["200"]);
            Assert.Equal("999", fhirToIje["999"]);
        }

        [Fact]
        public void RaceCode_ShouldHaveExtensiveMappings()
        {
            var ijeToFhir = Mappings.RaceCode.IJEToFHIR;
            var fhirToIje = Mappings.RaceCode.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test that mappings are extensive (should have many entries)
            Assert.True(ijeToFhir.Count > 100, "RaceCode should have many mappings");
            Assert.Equal(ijeToFhir.Count, fhirToIje.Count);

            // Test some specific mappings
            Assert.Equal("100", ijeToFhir["100"]);
            Assert.Equal("999", ijeToFhir["999"]);
            Assert.Equal("A01", ijeToFhir["A01"]);
            Assert.Equal("R96", ijeToFhir["R96"]);

            Assert.Equal("100", fhirToIje["100"]);
            Assert.Equal("999", fhirToIje["999"]);
            Assert.Equal("A01", fhirToIje["A01"]);
            Assert.Equal("R96", fhirToIje["R96"]);
        }

        [Fact]
        public void RaceRecode40_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.RaceRecode40.IJEToFHIR;
            var fhirToIje = Mappings.RaceRecode40.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test that we have mappings for 01-40 and 99
            Assert.Equal(41, ijeToFhir.Count); // 01-40 plus 99
            Assert.Equal(41, fhirToIje.Count);

            // Test some specific mappings
            Assert.Equal("01", ijeToFhir["01"]);
            Assert.Equal("40", ijeToFhir["40"]);
            Assert.Equal("99", ijeToFhir["99"]);

            Assert.Equal("01", fhirToIje["01"]);
            Assert.Equal("40", fhirToIje["40"]);
            Assert.Equal("99", fhirToIje["99"]);
        }

        [Fact]
        public void DateOfBirthEditFlags_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.DateOfBirthEditFlags.IJEToFHIR;
            var fhirToIje = Mappings.DateOfBirthEditFlags.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("0", ijeToFhir["0"]);
            Assert.Equal("1dataQueried", ijeToFhir["1"]);

            Assert.Equal("0", fhirToIje["0"]);
            Assert.Equal("1", fhirToIje["1dataQueried"]);
        }

        [Fact]
        public void PluralityEditFlags_ShouldHaveValidMappings()
        {
            var ijeToFhir = Mappings.PluralityEditFlags.IJEToFHIR;
            var fhirToIje = Mappings.PluralityEditFlags.FHIRToIJE;

            Assert.NotNull(ijeToFhir);
            Assert.NotNull(fhirToIje);
            Assert.NotEmpty(ijeToFhir);
            Assert.NotEmpty(fhirToIje);

            // Test specific mappings
            Assert.Equal("0off", ijeToFhir["0"]);
            Assert.Equal("1queriedCorrect", ijeToFhir["1"]);
            Assert.Equal("2pluralityQueriedInconsistent", ijeToFhir["2"]);

            Assert.Equal("0", fhirToIje["0off"]);
            Assert.Equal("1", fhirToIje["1queriedCorrect"]);
            Assert.Equal("2", fhirToIje["2pluralityQueriedInconsistent"]);
        }

        [Theory]
        [InlineData("BirthAttendantTitles")]
        [InlineData("BirthSexChild")]
        [InlineData("BirthSexFetus")]
        [InlineData("EducationLevel")]
        [InlineData("HispanicNoUnknown")]
        [InlineData("YesNoUnknown")]
        [InlineData("YesNoNotApplicable")]
        [InlineData("YesNoUnknownNotApplicable")]
        [InlineData("UnitsOfAge")]
        [InlineData("RaceMissingValueReason")]
        [InlineData("EditBypass01234")]
        [InlineData("DateOfBirthEditFlags")]
        [InlineData("PluralityEditFlags")]
        public void AllMappingClasses_ShouldHaveBothDirections(string mappingClassName)
        {
            var mappingType = typeof(Mappings).GetNestedType(mappingClassName);
            Assert.NotNull(mappingType);

            var ijeToFhirField = mappingType.GetField("IJEToFHIR");
            var fhirToIjeField = mappingType.GetField("FHIRToIJE");

            Assert.NotNull(ijeToFhirField);
            Assert.NotNull(fhirToIjeField);

            var ijeToFhirDict = ijeToFhirField.GetValue(null) as Dictionary<string, string>;
            var fhirToIjeDict = fhirToIjeField.GetValue(null) as Dictionary<string, string>;

            Assert.NotNull(ijeToFhirDict);
            Assert.NotNull(fhirToIjeDict);
            Assert.NotEmpty(ijeToFhirDict);
            Assert.NotEmpty(fhirToIjeDict);
        }

        [Fact]
        public void AllMappingDictionaries_ShouldBeReadonly()
        {
            // Test that dictionaries are readonly by checking the field attributes
            var mappingType = typeof(Mappings.BirthAttendantTitles);
            var ijeToFhirField = mappingType.GetField("IJEToFHIR");
            var fhirToIjeField = mappingType.GetField("FHIRToIJE");

            Assert.True(ijeToFhirField.IsInitOnly, "IJEToFHIR should be readonly");
            Assert.True(fhirToIjeField.IsInitOnly, "FHIRToIJE should be readonly");
        }

        [Fact]
        public void MappingsClass_ShouldBeStatic()
        {
            Assert.True(typeof(Mappings).IsAbstract && typeof(Mappings).IsSealed, "Mappings class should be static");
        }

        [Fact]
        public void AllNestedClasses_ShouldBeStatic()
        {
            var nestedTypes = typeof(Mappings).GetNestedTypes();

            foreach (var nestedType in nestedTypes)
            {
                Assert.True(nestedType.IsAbstract && nestedType.IsSealed,
                    $"Nested class {nestedType.Name} should be static");
            }
        }

        [Fact]
        public void Mappings_ShouldHaveExpectedNumberOfNestedClasses()
        {
            var nestedTypes = typeof(Mappings).GetNestedTypes();

            var expectedClasses = new[]
            {
                "BirthAttendantTitles",
                "BirthSexChild",
                "BirthSexFetus",
                "DateOfBirthEditFlags",
                "EditBypass01234",
                "EducationLevel",
                "HispanicNoUnknown",
                "HispanicOrigin",
                "MaritalStatus",
                "PluralityEditFlags",
                "RaceCode",
                "RaceMissingValueReason",
                "RaceRecode40",
                "UnitsOfAge",
                "YesNoNotApplicable",
                "YesNoUnknownNotApplicable",
                "YesNoUnknown"
            };

            Assert.Equal(expectedClasses.Length, nestedTypes.Length);

            foreach (var expectedClass in expectedClasses)
            {
                Assert.Contains(nestedTypes, t => t.Name == expectedClass);
            }
        }

        [Fact]
        public void AllMappings_ShouldNotContainNullOrEmptyKeys()
        {
            // Test a few representative mapping classes
            var testMappings = new[]
            {
                Mappings.BirthAttendantTitles.IJEToFHIR,
                Mappings.BirthAttendantTitles.FHIRToIJE,
                Mappings.EducationLevel.IJEToFHIR,
                Mappings.EducationLevel.FHIRToIJE,
                Mappings.YesNoUnknown.IJEToFHIR,
                Mappings.YesNoUnknown.FHIRToIJE
            };

            foreach (var mapping in testMappings)
            {
                foreach (var kvp in mapping)
                {
                    Assert.False(string.IsNullOrEmpty(kvp.Key), "Mapping keys should not be null or empty");
                    Assert.False(string.IsNullOrEmpty(kvp.Value), "Mapping values should not be null or empty");
                }
            }
        }

        [Fact]
        public void AllMappings_ShouldNotContainDuplicateKeys()
        {
            // Test a few representative mapping classes
            var testMappings = new Dictionary<string, Dictionary<string, string>>
            {
                { "BirthAttendantTitles.IJEToFHIR", Mappings.BirthAttendantTitles.IJEToFHIR },
                { "BirthAttendantTitles.FHIRToIJE", Mappings.BirthAttendantTitles.FHIRToIJE },
                { "EducationLevel.IJEToFHIR", Mappings.EducationLevel.IJEToFHIR },
                { "EducationLevel.FHIRToIJE", Mappings.EducationLevel.FHIRToIJE }
            };

            foreach (var testMapping in testMappings)
            {
                var keys = testMapping.Value.Keys.ToList();
                var distinctKeys = keys.Distinct().ToList();

                Assert.True(keys.Count == distinctKeys.Count,
                    $"Mapping {testMapping.Key} should not contain duplicate keys");
            }
        }
    }
}

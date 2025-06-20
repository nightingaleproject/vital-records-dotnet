using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using VR;

namespace VitalRecord.Tests
{
    public class ValueSets_Should
    {
        [Fact]
        public void ValueSetsClass_ShouldBeStatic()
        {
            Assert.True(typeof(ValueSets).IsAbstract && typeof(ValueSets).IsSealed, "ValueSets class should be static");
        }

        [Fact]
        public void AllNestedClasses_ShouldBeStatic()
        {
            var nestedTypes = typeof(ValueSets).GetNestedTypes();

            foreach (var nestedType in nestedTypes)
            {
                Assert.True(nestedType.IsAbstract && nestedType.IsSealed,
                    $"Nested class {nestedType.Name} should be static");
            }
        }

        [Fact]
        public void ValueSets_ShouldHaveExpectedNestedClasses()
        {
            var nestedTypes = typeof(ValueSets).GetNestedTypes();
            var nestedTypeNames = nestedTypes.Select(t => t.Name).ToList();

            var expectedClasses = new[]
            {
                "BirthAttendantTitles",
                "CodedRaceAndEthnicityPerson",
                "DateOfBirthEditFlags",
                "EducationLevelPerson",
                "EducationLevel",
                "FatherRelationship",
                "HispanicOrigin",
                "InputRaceAndEthnicityPerson",
                "Jurisdiction",
                "MaritalStatus",
                "MotherRelationship",
                "PartialDateDataAbsentReason",
                "PluralityEditFlags",
                "RaceCode",
                "RaceMissingValueReason",
                "RaceRecode40",
                "ResidenceCountry",
                "Role",
                "SexAssignedAtBirth",
                "StatesTerritoriesProvinces",
                "UnitsOfAge",
                "UsstatesTerritories",
                "Usstates",
                "Usterritories",
                "YesNoNotApplicable",
                "YesNoUnknownNotApplicable",
                "YesNoUnknown",
                "EditBypass01234"
            };

            Assert.True(nestedTypes.Length >= expectedClasses.Length,
                $"Expected at least {expectedClasses.Length} nested classes, but found {nestedTypes.Length}");

            foreach (var expectedClass in expectedClasses)
            {
                Assert.Contains(expectedClass, nestedTypeNames);
            }
        }

        [Theory]
        [InlineData("BirthAttendantTitles")]
        [InlineData("EducationLevel")]
        [InlineData("HispanicOrigin")]
        [InlineData("MaritalStatus")]
        [InlineData("RaceCode")]
        [InlineData("SexAssignedAtBirth")]
        [InlineData("YesNoUnknown")]
        public void ValueSetClasses_ShouldHaveCodesArray(string valueSetClassName)
        {
            var valueSetType = typeof(ValueSets).GetNestedType(valueSetClassName);
            Assert.NotNull(valueSetType);

            var codesField = valueSetType.GetField("Codes");
            Assert.NotNull(codesField);
            Assert.True(codesField.IsStatic, "Codes field should be static");
            Assert.True(codesField.IsPublic, "Codes field should be public");
            Assert.Equal(typeof(string[,]), codesField.FieldType);

            var codesArray = codesField.GetValue(null) as string[,];
            Assert.NotNull(codesArray);
            Assert.True(codesArray.GetLength(0) > 0, "Codes array should have at least one row");
            Assert.Equal(3, codesArray.GetLength(1)); // Should have 3 columns: code, display, system
        }

        [Fact]
        public void BirthAttendantTitles_ShouldHaveValidCodes()
        {
            var codes = ValueSets.BirthAttendantTitles.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 0);
            Assert.Equal(3, codes.GetLength(1));

            // Test specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("309343006", "Medical Doctor", VR.CodeSystems.SCT), codesList);
            Assert.Contains(("76231001", "Osteopath", VR.CodeSystems.SCT), codesList);
            Assert.Contains(("UNK", "Unknown", VR.CodeSystems.NullFlavor_HL7_V3), codesList);
        }

        [Fact]
        public void BirthAttendantTitles_ShouldHaveValidConstants()
        {
            Assert.Equal("309343006", ValueSets.BirthAttendantTitles.Medical_Doctor);
            Assert.Equal("76231001", ValueSets.BirthAttendantTitles.Osteopath);
            Assert.Equal("445521000124102", ValueSets.BirthAttendantTitles.Advanced_Practice_Midwife);
            Assert.Equal("445531000124104", ValueSets.BirthAttendantTitles.Lay_Midwife);
            Assert.Equal("OTH", ValueSets.BirthAttendantTitles.Other);
            Assert.Equal("UNK", ValueSets.BirthAttendantTitles.Unknown);
        }

        [Fact]
        public void EducationLevel_ShouldHaveValidCodes()
        {
            var codes = ValueSets.EducationLevel.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 0);
            Assert.Equal(3, codes.GetLength(1));

            // Test specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("ELEM", "Elementary School", VR.CodeSystems.EducationLevel), codesList);
            Assert.Contains(("HS", "High School or secondary school degree complete", VR.CodeSystems.EducationLevel), codesList);
            Assert.Contains(("BA", "Bachelor's degree", VR.CodeSystems.DegreeLicenceAndCertificate), codesList);
            Assert.Contains(("UNK", "unknown", VR.CodeSystems.NullFlavor_HL7_V3), codesList);
        }

        [Fact]
        public void EducationLevel_ShouldHaveValidConstants()
        {
            Assert.Equal("ELEM", ValueSets.EducationLevel.Elementary_School);
            Assert.Equal("SEC", ValueSets.EducationLevel.Some_Secondary_Or_High_School_Education);
            Assert.Equal("HS", ValueSets.EducationLevel.High_School_Or_Secondary_School_Degree_Complete);
            Assert.Equal("SCOL", ValueSets.EducationLevel.Some_College_Education);
            Assert.Equal("POSTG", ValueSets.EducationLevel.Doctoral_Or_Post_Graduate_Education);
            Assert.Equal("AA", ValueSets.EducationLevel.Associates_Or_Technical_Degree_Complete);
            Assert.Equal("BA", ValueSets.EducationLevel.Bachelors_Degree);
            Assert.Equal("MA", ValueSets.EducationLevel.Masters_Degree);
            Assert.Equal("UNK", ValueSets.EducationLevel.Unknown);
        }

        [Fact]
        public void MaritalStatus_ShouldHaveValidCodes()
        {
            var codes = ValueSets.MaritalStatus.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 0);
            Assert.Equal(3, codes.GetLength(1));

            // Test specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("D", "Divorced", VR.CodeSystems.PH_MaritalStatus_HL7_2x), codesList);
            Assert.Contains(("M", "Married", VR.CodeSystems.PH_MaritalStatus_HL7_2x), codesList);
            Assert.Contains(("S", "Never Married", VR.CodeSystems.PH_MaritalStatus_HL7_2x), codesList);
            Assert.Contains(("UNK", "unknown", VR.CodeSystems.NullFlavor_HL7_V3), codesList);
        }

        [Fact]
        public void MaritalStatus_ShouldHaveValidConstants()
        {
            Assert.Equal("D", ValueSets.MaritalStatus.Divorced);
            Assert.Equal("L", ValueSets.MaritalStatus.Legally_Separated);
            Assert.Equal("M", ValueSets.MaritalStatus.Married);
            Assert.Equal("S", ValueSets.MaritalStatus.Never_Married);
            Assert.Equal("W", ValueSets.MaritalStatus.Widowed);
            Assert.Equal("T", ValueSets.MaritalStatus.Domestic_Partner);
            Assert.Equal("UNK", ValueSets.MaritalStatus.Unknown);
        }

        [Fact]
        public void SexAssignedAtBirth_ShouldHaveValidCodes()
        {
            var codes = ValueSets.SexAssignedAtBirth.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 0);
            Assert.Equal(3, codes.GetLength(1));

            // Test specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("F", "Female", VR.CodeSystems.VRCLAdministrativeGender), codesList);
            Assert.Contains(("M", "Male", VR.CodeSystems.VRCLAdministrativeGender), codesList);
            Assert.Contains(("UNK", "Unknown", VR.CodeSystems.NullFlavor_HL7_V3), codesList);
        }

        [Fact]
        public void SexAssignedAtBirth_ShouldHaveValidConstants()
        {
            Assert.Equal("F", ValueSets.SexAssignedAtBirth.Female);
            Assert.Equal("M", ValueSets.SexAssignedAtBirth.Male);
            Assert.Equal("UNK", ValueSets.SexAssignedAtBirth.Unknown);
        }

        [Fact]
        public void YesNoUnknown_ShouldHaveValidCodes()
        {
            var codes = ValueSets.YesNoUnknown.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 0);
            Assert.Equal(3, codes.GetLength(1));

            // Test specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("N", "No", VR.CodeSystems.YesNo), codesList);
            Assert.Contains(("Y", "Yes", VR.CodeSystems.YesNo), codesList);
            Assert.Contains(("UNK", "unknown", VR.CodeSystems.NullFlavor_HL7_V3), codesList);
        }

        [Fact]
        public void YesNoUnknown_ShouldHaveValidConstants()
        {
            Assert.Equal("N", ValueSets.YesNoUnknown.No);
            Assert.Equal("Y", ValueSets.YesNoUnknown.Yes);
            Assert.Equal("UNK", ValueSets.YesNoUnknown.Unknown);
        }

        [Fact]
        public void YesNoNotApplicable_ShouldHaveValidCodes()
        {
            var codes = ValueSets.YesNoNotApplicable.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 0);
            Assert.Equal(3, codes.GetLength(1));

            // Test specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("N", "No", VR.CodeSystems.YesNo), codesList);
            Assert.Contains(("Y", "Yes", VR.CodeSystems.YesNo), codesList);
            Assert.Contains(("NA", "not applicable", VR.CodeSystems.NullFlavor_HL7_V3), codesList);
        }

        [Fact]
        public void YesNoNotApplicable_ShouldHaveValidConstants()
        {
            Assert.Equal("N", ValueSets.YesNoNotApplicable.No);
            Assert.Equal("Y", ValueSets.YesNoNotApplicable.Yes);
            Assert.Equal("NA", ValueSets.YesNoNotApplicable.Not_Applicable);
        }

        [Fact]
        public void YesNoUnknownNotApplicable_ShouldHaveValidCodes()
        {
            var codes = ValueSets.YesNoUnknownNotApplicable.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 0);
            Assert.Equal(3, codes.GetLength(1));

            // Test specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("Y", "Yes", VR.CodeSystems.YesNo), codesList);
            Assert.Contains(("N", "No", VR.CodeSystems.YesNo), codesList);
            Assert.Contains(("UNK", "unknown", VR.CodeSystems.NullFlavor_HL7_V3), codesList);
            Assert.Contains(("NA", "not applicable", VR.CodeSystems.NullFlavor_HL7_V3), codesList);
        }

        [Fact]
        public void YesNoUnknownNotApplicable_ShouldHaveValidConstants()
        {
            Assert.Equal("Y", ValueSets.YesNoUnknownNotApplicable.Yes);
            Assert.Equal("N", ValueSets.YesNoUnknownNotApplicable.No);
            Assert.Equal("UNK", ValueSets.YesNoUnknownNotApplicable.Unknown);
            Assert.Equal("NA", ValueSets.YesNoUnknownNotApplicable.Not_Applicable);
        }

        [Fact]
        public void UnitsOfAge_ShouldHaveValidCodes()
        {
            var codes = ValueSets.UnitsOfAge.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 0);
            Assert.Equal(3, codes.GetLength(1));

            // Test specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("min", "Minutes", VR.CodeSystems.UnitsOfMeasure), codesList);
            Assert.Contains(("d", "Days", VR.CodeSystems.UnitsOfMeasure), codesList);
            Assert.Contains(("h", "Hours", VR.CodeSystems.UnitsOfMeasure), codesList);
            Assert.Contains(("mo", "Months", VR.CodeSystems.UnitsOfMeasure), codesList);
            Assert.Contains(("a", "Years", VR.CodeSystems.UnitsOfMeasure), codesList);
            Assert.Contains(("UNK", "unknown", VR.CodeSystems.NullFlavor_HL7_V3), codesList);
        }

        [Fact]
        public void UnitsOfAge_ShouldHaveValidConstants()
        {
            Assert.Equal("min", ValueSets.UnitsOfAge.Minutes);
            Assert.Equal("d", ValueSets.UnitsOfAge.Days);
            Assert.Equal("h", ValueSets.UnitsOfAge.Hours);
            Assert.Equal("mo", ValueSets.UnitsOfAge.Months);
            Assert.Equal("a", ValueSets.UnitsOfAge.Years);
            Assert.Equal("UNK", ValueSets.UnitsOfAge.Unknown);
        }

        [Fact]
        public void HispanicOrigin_ShouldHaveExtensiveCodes()
        {
            var codes = ValueSets.HispanicOrigin.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 50, "HispanicOrigin should have many codes");
            Assert.Equal(3, codes.GetLength(1));

            // Test some specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("100", "Non-Hispanic", VR.CodeSystems.VRCLHispanicOrigin), codesList);
            Assert.Contains(("200", "Spaniard", VR.CodeSystems.VRCLHispanicOrigin), codesList);
            Assert.Contains(("211", "Mexican", VR.CodeSystems.VRCLHispanicOrigin), codesList);
            Assert.Contains(("261", "Puerto Rican", VR.CodeSystems.VRCLHispanicOrigin), codesList);
            Assert.Contains(("271", "Cuban", VR.CodeSystems.VRCLHispanicOrigin), codesList);
            Assert.Contains(("999", "First Pass Reject", VR.CodeSystems.VRCLHispanicOrigin), codesList);
        }

        [Fact]
        public void HispanicOrigin_ShouldHaveValidConstants()
        {
            Assert.Equal("100", ValueSets.HispanicOrigin.Non_Hispanic);
            Assert.Equal("200", ValueSets.HispanicOrigin.Spaniard);
            Assert.Equal("211", ValueSets.HispanicOrigin.Mexican);
            Assert.Equal("261", ValueSets.HispanicOrigin.Puerto_Rican);
            Assert.Equal("271", ValueSets.HispanicOrigin.Cuban);
            Assert.Equal("999", ValueSets.HispanicOrigin.First_Pass_Reject);
        }

        [Fact]
        public void RaceCode_ShouldHaveExtensiveCodes()
        {
            var codes = ValueSets.RaceCode.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 100, "RaceCode should have many codes");
            Assert.Equal(3, codes.GetLength(1));

            // Test some specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("100", "White Checkbox", VR.CodeSystems.VRCLRaceCode), codesList);
            Assert.Contains(("101", "White", VR.CodeSystems.VRCLRaceCode), codesList);
            Assert.Contains(("200", "Black Checkbox", VR.CodeSystems.VRCLRaceCode), codesList);
            Assert.Contains(("201", "Black", VR.CodeSystems.VRCLRaceCode), codesList);
            Assert.Contains(("300", "American Indian Checkbox", VR.CodeSystems.VRCLRaceCode), codesList);
            Assert.Contains(("999", "First Pass Reject", VR.CodeSystems.VRCLRaceCode), codesList);
        }

        [Fact]
        public void RaceCode_ShouldHaveValidConstants()
        {
            Assert.Equal("100", ValueSets.RaceCode.White_Checkbox);
            Assert.Equal("101", ValueSets.RaceCode.White);
            Assert.Equal("200", ValueSets.RaceCode.Black_Checkbox);
            Assert.Equal("201", ValueSets.RaceCode.Black);
            Assert.Equal("300", ValueSets.RaceCode.American_Indian_Checkbox);
            Assert.Equal("999", ValueSets.RaceCode.First_Pass_Reject);
        }

        [Fact]
        public void RaceRecode40_ShouldHaveValidCodes()
        {
            var codes = ValueSets.RaceRecode40.Codes;

            Assert.NotNull(codes);
            Assert.Equal(41, codes.GetLength(0)); // Should have 41 codes (01-40 plus 99)
            Assert.Equal(3, codes.GetLength(1));

            // Test some specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("01", "White", VR.CodeSystems.VRCLRaceRecode40), codesList);
            Assert.Contains(("02", "Black", VR.CodeSystems.VRCLRaceRecode40), codesList);
            Assert.Contains(("40", "Black, AIAN, Asian, NHOPI and White", VR.CodeSystems.VRCLRaceRecode40), codesList);
            Assert.Contains(("99", "Unknown and Other Race", VR.CodeSystems.VRCLRaceRecode40), codesList);
        }

        [Fact]
        public void RaceRecode40_ShouldHaveValidConstants()
        {
            Assert.Equal("01", ValueSets.RaceRecode40.White);
            Assert.Equal("02", ValueSets.RaceRecode40.Black);
            Assert.Equal("40", ValueSets.RaceRecode40.Black_Aian_Asian_Nhopi_And_White);
            Assert.Equal("99", ValueSets.RaceRecode40.Unknown_And_Other_Race);
        }

        [Fact]
        public void Jurisdiction_ShouldHaveValidCodes()
        {
            var codes = ValueSets.Jurisdiction.Codes;

            Assert.NotNull(codes);
            Assert.True(codes.GetLength(0) > 50, "Jurisdiction should have many codes");
            Assert.Equal(3, codes.GetLength(1));

            // Test some specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("AL", "Alabama", VR.CodeSystems.VRCLUSStatesTerritories), codesList);
            Assert.Contains(("CA", "California", VR.CodeSystems.VRCLUSStatesTerritories), codesList);
            Assert.Contains(("NY", "New York", VR.CodeSystems.VRCLUSStatesTerritories), codesList);
            Assert.Contains(("YC", "New York City", VR.CodeSystems.VRCLJurisdictions), codesList);
            Assert.Contains(("ZZ", "Unknown Jurisdiction", VR.CodeSystems.VRCLJurisdictions), codesList);
        }

        [Fact]
        public void Jurisdiction_ShouldHaveValidConstants()
        {
            Assert.Equal("AL", ValueSets.Jurisdiction.Alabama);
            Assert.Equal("CA", ValueSets.Jurisdiction.California);
            Assert.Equal("NY", ValueSets.Jurisdiction.New_York);
            Assert.Equal("YC", ValueSets.Jurisdiction.New_York_City);
            Assert.Equal("ZZ", ValueSets.Jurisdiction.Unknown_Jurisdiction);
        }

        [Fact]
        public void EditBypass01234_ShouldHaveValidCodes()
        {
            var codes = ValueSets.EditBypass01234.Codes;

            Assert.NotNull(codes);
            Assert.Equal(5, codes.GetLength(0)); // Should have 5 codes (0-4)
            Assert.Equal(3, codes.GetLength(1));

            // Test specific known values
            var codesList = new List<(string code, string display, string system)>();
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                codesList.Add((codes[i, 0], codes[i, 1], codes[i, 2]));
            }

            Assert.Contains(("0", "Edit Passed", VR.CodeSystems.VRCLEditFlags), codesList);
            Assert.Contains(("1", "Edit Failed, Data Queried, and Verified", VR.CodeSystems.VRCLEditFlags), codesList);
            Assert.Contains(("2", "Edit Failed, Data Queried, but not Verified", VR.CodeSystems.VRCLEditFlags), codesList);
            Assert.Contains(("3", "Edit Failed, Review Needed", VR.CodeSystems.VRCLEditFlags), codesList);
            Assert.Contains(("4", "Edit Failed, Query Needed", VR.CodeSystems.VRCLEditFlags), codesList);
        }

        [Fact]
        public void EditBypass01234_ShouldHaveValidConstants()
        {
            Assert.Equal("0", ValueSets.EditBypass01234.Edit_Passed);
            Assert.Equal("1", ValueSets.EditBypass01234.Edit_Failed_Data_Queried_And_Verified);
            Assert.Equal("2", ValueSets.EditBypass01234.Edit_Failed_Data_Queried_But_Not_Verified);
            Assert.Equal("3", ValueSets.EditBypass01234.Edit_Failed_Review_Needed);
            Assert.Equal("4", ValueSets.EditBypass01234.Edit_Failed_Query_Needed);
        }

        [Theory]
        [InlineData("BirthAttendantTitles")]
        [InlineData("EducationLevel")]
        [InlineData("HispanicOrigin")]
        [InlineData("MaritalStatus")]
        [InlineData("RaceCode")]
        [InlineData("SexAssignedAtBirth")]
        [InlineData("YesNoUnknown")]
        [InlineData("YesNoNotApplicable")]
        [InlineData("YesNoUnknownNotApplicable")]
        [InlineData("UnitsOfAge")]
        [InlineData("EditBypass01234")]
        public void AllValueSetCodes_ShouldNotContainNullOrEmptyValues(string valueSetClassName)
        {
            var valueSetType = typeof(ValueSets).GetNestedType(valueSetClassName);
            Assert.NotNull(valueSetType);

            var codesField = valueSetType.GetField("Codes");
            Assert.NotNull(codesField);

            var codesArray = codesField.GetValue(null) as string[,];
            Assert.NotNull(codesArray);

            for (int i = 0; i < codesArray.GetLength(0); i++)
            {
                for (int j = 0; j < codesArray.GetLength(1); j++)
                {
                    Assert.False(string.IsNullOrEmpty(codesArray[i, j]),
                        $"Code array in {valueSetClassName} should not contain null or empty values at position [{i},{j}]");
                }
            }
        }

        [Theory]
        [InlineData("BirthAttendantTitles")]
        [InlineData("EducationLevel")]
        [InlineData("MaritalStatus")]
        [InlineData("SexAssignedAtBirth")]
        [InlineData("YesNoUnknown")]
        [InlineData("UnitsOfAge")]
        public void AllValueSetCodes_ShouldNotContainDuplicateCodes(string valueSetClassName)
        {
            var valueSetType = typeof(ValueSets).GetNestedType(valueSetClassName);
            Assert.NotNull(valueSetType);

            var codesField = valueSetType.GetField("Codes");
            Assert.NotNull(codesField);

            var codesArray = codesField.GetValue(null) as string[,];
            Assert.NotNull(codesArray);

            var codes = new List<string>();
            for (int i = 0; i < codesArray.GetLength(0); i++)
            {
                codes.Add(codesArray[i, 0]); // First column contains the codes
            }

            var distinctCodes = codes.Distinct().ToList();
            Assert.Equal(codes.Count, distinctCodes.Count);
        }

        [Fact]
        public void AllValueSetConstants_ShouldBePublicAndStatic()
        {
            var nestedTypes = typeof(ValueSets).GetNestedTypes();

            foreach (var nestedType in nestedTypes)
            {
                var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.Static);

                foreach (var field in fields)
                {
                    if (field.Name != "Codes") // Skip the Codes array field
                    {
                        Assert.True(field.IsPublic);
                        Assert.True(field.IsStatic);
                        Assert.Equal(typeof(string), field.FieldType);
                    }
                }
            }
        }

        [Fact]
        public void AllValueSetConstants_ShouldHaveValidValues()
        {
            // Test a few representative value sets to ensure constants match codes in arrays
            var testCases = new[]
            {
                (typeof(ValueSets.BirthAttendantTitles), "Medical_Doctor", "309343006"),
                (typeof(ValueSets.EducationLevel), "Elementary_School", "ELEM"),
                (typeof(ValueSets.MaritalStatus), "Married", "M"),
                (typeof(ValueSets.SexAssignedAtBirth), "Female", "F"),
                (typeof(ValueSets.YesNoUnknown), "Yes", "Y"),
                (typeof(ValueSets.UnitsOfAge), "Years", "a")
            };

            foreach (var (type, constantName, expectedValue) in testCases)
            {
                var field = type.GetField(constantName);
                Assert.NotNull(field);

                var actualValue = field.GetValue(null) as string;
                Assert.Equal(expectedValue, actualValue);
            }
        }

        [Fact]
        public void ValueSets_ShouldNotHaveInstanceMembers()
        {
            var instanceFields = typeof(ValueSets).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var instanceMethods = typeof(ValueSets).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            var instanceProperties = typeof(ValueSets).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Assert.Empty(instanceFields);
            Assert.Empty(instanceMethods);
            Assert.Empty(instanceProperties);
        }

        [Theory]
        [InlineData("Role")]
        [InlineData("FatherRelationship")]
        [InlineData("MotherRelationship")]
        [InlineData("RaceMissingValueReason")]
        [InlineData("PartialDateDataAbsentReason")]
        public void AdditionalValueSets_ShouldHaveValidStructure(string valueSetClassName)
        {
            var valueSetType = typeof(ValueSets).GetNestedType(valueSetClassName);
            Assert.NotNull(valueSetType);

            var codesField = valueSetType.GetField("Codes");
            Assert.NotNull(codesField);
            Assert.True(codesField.IsStatic, "Codes field should be static");
            Assert.True(codesField.IsPublic, "Codes field should be public");
            Assert.Equal(typeof(string[,]), codesField.FieldType);

            var codesArray = codesField.GetValue(null) as string[,];
            Assert.NotNull(codesArray);
            Assert.True(codesArray.GetLength(0) > 0, "Codes array should have at least one row");
            Assert.Equal(3, codesArray.GetLength(1)); // Should have 3 columns: code, display, system
        }

        [Fact]
        public void Role_ShouldHaveValidConstants()
        {
            Assert.Equal("FTH", ValueSets.Role.Father);
            Assert.Equal("MTH", ValueSets.Role.Mother);
        }

        [Fact]
        public void FatherRelationship_ShouldHaveValidConstants()
        {
            Assert.Equal("ADOPTF", ValueSets.FatherRelationship.Adoptive_Father);
            Assert.Equal("FTH", ValueSets.FatherRelationship.Father);
            Assert.Equal("NFTH", ValueSets.FatherRelationship.Natural_Father);
        }

        [Fact]
        public void MotherRelationship_ShouldHaveValidConstants()
        {
            Assert.Equal("ADOPTM", ValueSets.MotherRelationship.Adoptive_Mother);
            Assert.Equal("MTH", ValueSets.MotherRelationship.Mother);
            Assert.Equal("NMTH", ValueSets.MotherRelationship.Natural_Mother);
        }

        [Fact]
        public void RaceMissingValueReason_ShouldHaveValidConstants()
        {
            Assert.Equal("PREFUS", ValueSets.RaceMissingValueReason.Patient_Refuse);
            Assert.Equal("ASKU", ValueSets.RaceMissingValueReason.Asked_But_Unknown);
            Assert.Equal("UNK", ValueSets.RaceMissingValueReason.Unknown);
        }
    }
}

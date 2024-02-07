using System;
using System.Collections.Generic;
using System.IO;
using VR;
using Xunit;

namespace BFDR.Tests
{
  public class BirthRecord_Should
  {
    private BirthRecord SetterBirthRecord;

    public BirthRecord_Should()
    {
      SetterBirthRecord = new BirthRecord();
    }

    [Fact]
    public void NoCongentialAbnormalities()
    {
      Assert.False(SetterBirthRecord.NoCongenitalAnomaliesOfTheNewborn);
      SetterBirthRecord.NoCongenitalAnomaliesOfTheNewborn = true;
      Assert.True(SetterBirthRecord.NoCongenitalAnomaliesOfTheNewborn);
      SetterBirthRecord.NoCongenitalAnomaliesOfTheNewborn = false;
      Assert.False(SetterBirthRecord.NoCongenitalAnomaliesOfTheNewborn);
      Assert.False(SetterBirthRecord.Anencephaly);
      SetterBirthRecord.Anencephaly = true;
      Assert.True(SetterBirthRecord.Anencephaly);
      SetterBirthRecord.NoCongenitalAnomaliesOfTheNewborn = true;
      Assert.False(SetterBirthRecord.Anencephaly);
      Assert.True(SetterBirthRecord.NoCongenitalAnomaliesOfTheNewborn);
      SetterBirthRecord.Anencephaly = true;
      Assert.True(SetterBirthRecord.Anencephaly);
      Assert.False(SetterBirthRecord.NoCongenitalAnomaliesOfTheNewborn);
    }

    [Fact]
    public void Set_Anencephaly()
    {
      Assert.False(SetterBirthRecord.Anencephaly);
      String json = SetterBirthRecord.ToJSON();
      Assert.DoesNotContain("89369001", json); // code
      Assert.DoesNotContain("73780-9", json); // category code
      Assert.DoesNotContain("57075-4", json); // composition section code
      // Check nothing changes if we set a missing entry to false
      SetterBirthRecord.Anencephaly = false;
      json = SetterBirthRecord.ToJSON();
      Assert.DoesNotContain("89369001", json); // code
      Assert.DoesNotContain("73780-9", json); // category code
      Assert.DoesNotContain("57075-4", json); // composition section code
      SetterBirthRecord.Anencephaly = true;
      Assert.True(SetterBirthRecord.Anencephaly);
      json = SetterBirthRecord.ToJSON();
      Assert.Contains("89369001", json); // code
      Assert.Contains("73780-9", json); // category code
      Assert.Contains("57075-4", json); // composition section code
      // Check nothing changes if we set an existing entry to true
      SetterBirthRecord.Anencephaly = true;
      json = SetterBirthRecord.ToJSON();
      Assert.Contains("89369001", json); // code
      Assert.Contains("73780-9", json); // category code
      Assert.Contains("57075-4", json); // composition section code
      SetterBirthRecord.Anencephaly = false;
      Assert.False(SetterBirthRecord.Anencephaly);
      json = SetterBirthRecord.ToJSON();
      Assert.DoesNotContain("89369001", json); // code
      // composition section is retained even when section entries are removed
    }

    [Fact]
    public void Set_AntibioticsAdministeredDuringLabor()
    {
      Assert.False(SetterBirthRecord.AntibioticsAdministeredDuringLabor);
      String json = SetterBirthRecord.ToJSON();
      Assert.DoesNotContain("434691000124101", json); // code
      Assert.DoesNotContain("73813-8", json); // category code
      Assert.DoesNotContain("55752-0", json); // composition section code
      // Check nothing changes if we set a missing entry to false
      SetterBirthRecord.AntibioticsAdministeredDuringLabor = false;
      json = SetterBirthRecord.ToJSON();
      Assert.DoesNotContain("434691000124101", json); // code
      Assert.DoesNotContain("73813-8", json); // category code
      Assert.DoesNotContain("55752-0", json); // composition section code
      SetterBirthRecord.AntibioticsAdministeredDuringLabor = true;
      Assert.True(SetterBirthRecord.AntibioticsAdministeredDuringLabor);
      json = SetterBirthRecord.ToJSON();
      Assert.Contains("434691000124101", json); // code
      Assert.Contains("73813-8", json); // category code
      Assert.Contains("55752-0", json); // composition section code
      // Check nothing changes if we set an existing entry to true
      SetterBirthRecord.AntibioticsAdministeredDuringLabor = true;
      json = SetterBirthRecord.ToJSON();
      Assert.Contains("434691000124101", json); // code
      Assert.Contains("73813-8", json); // category code
      Assert.Contains("55752-0", json); // composition section code
      SetterBirthRecord.AntibioticsAdministeredDuringLabor = false;
      Assert.False(SetterBirthRecord.AntibioticsAdministeredDuringLabor);
      json = SetterBirthRecord.ToJSON();
      Assert.DoesNotContain("434691000124101", json); // code
      // note that composition section is retained even when section entries are removed
    }

    [Fact]
    public void Set_InductionOfLabor()
    {
      Assert.False(SetterBirthRecord.InductionOfLabor);
      String json = SetterBirthRecord.ToJSON();
      Assert.DoesNotContain("236958009", json); // code
      Assert.DoesNotContain("55752-0", json); // composition section code
      SetterBirthRecord.InductionOfLabor = true;
      Assert.True(SetterBirthRecord.InductionOfLabor);
      json = SetterBirthRecord.ToJSON();
      Assert.Contains("236958009", json); // code
      Assert.Contains("55752-0", json); // composition section code
      SetterBirthRecord.InductionOfLabor = false;
      Assert.False(SetterBirthRecord.InductionOfLabor);
      json = SetterBirthRecord.ToJSON();
      Assert.DoesNotContain("236958009", json); // code
      // composition section is retained even when section entries are removed
    }

    [Fact]
    public void Set_AssistedVentilationFollowingDelivery()
    {
      // Test custom code system URL
      Assert.False(SetterBirthRecord.AssistedVentilationFollowingDelivery);
      String json = SetterBirthRecord.ToJSON();
      Assert.DoesNotContain(CodeSystemURL.AbnormalConditionsNewborn, json);
      SetterBirthRecord.AssistedVentilationFollowingDelivery = true;
      Assert.True(SetterBirthRecord.AssistedVentilationFollowingDelivery);
      json = SetterBirthRecord.ToJSON();
      Assert.Contains(CodeSystemURL.AbnormalConditionsNewborn, json);
    }

    [Fact]
    public void Set_FinalRouteAndMethodOfDelivery()
    {
      // Check nothing present in fresh record
      Assert.False(SetterBirthRecord.UnknownFinalRouteAndMethodOfDelivery);
      var coding = SetterBirthRecord.FinalRouteAndMethodOfDelivery;
      Assert.Equal("", coding["code"]);
      // Test setting the final route
      coding.Clear();
      coding.Add("code", "302383004");
      coding.Add("system", "http://snomed.info/sct");
      coding.Add("display", "Forceps delivery (procedure)");
      SetterBirthRecord.FinalRouteAndMethodOfDelivery = coding;
      String json = SetterBirthRecord.ToJSON();
      Assert.Contains("302383004", json); // code
      Assert.Contains("73762-7", json); // category
      coding = SetterBirthRecord.FinalRouteAndMethodOfDelivery;
      Assert.Equal("302383004", coding["code"]);
      // Test that setting unknown removes the previously set route
      SetterBirthRecord.UnknownFinalRouteAndMethodOfDelivery = true;
      Assert.True(SetterBirthRecord.UnknownFinalRouteAndMethodOfDelivery);
      coding = SetterBirthRecord.FinalRouteAndMethodOfDelivery;
      Assert.Equal("", coding["code"]);
      // Test that setting a route removes the unknown observation
      coding.Clear();
      coding.Add("code", "302383004");
      coding.Add("system", "http://snomed.info/sct");
      coding.Add("display", "Forceps delivery (procedure)");
      SetterBirthRecord.FinalRouteAndMethodOfDelivery = coding;
      Assert.False(SetterBirthRecord.UnknownFinalRouteAndMethodOfDelivery);
    }

    [Fact]
    public void Set_ObstetricProcedures()
    {
      Assert.False(SetterBirthRecord.NoObstetricProcedures);
      Assert.False(SetterBirthRecord.SuccessfulExternalCephalicVersion);
      Assert.False(SetterBirthRecord.UnsuccessfulExternalCephalicVersion);
      SetterBirthRecord.SuccessfulExternalCephalicVersion = true;
      Assert.False(SetterBirthRecord.NoObstetricProcedures);
      Assert.True(SetterBirthRecord.SuccessfulExternalCephalicVersion);
      Assert.False(SetterBirthRecord.UnsuccessfulExternalCephalicVersion);
      String json = SetterBirthRecord.ToJSON();
      Assert.Contains("385669000", json); // successful outcome
      SetterBirthRecord.UnsuccessfulExternalCephalicVersion = true;
      Assert.False(SetterBirthRecord.NoObstetricProcedures);
      Assert.True(SetterBirthRecord.SuccessfulExternalCephalicVersion);
      Assert.True(SetterBirthRecord.UnsuccessfulExternalCephalicVersion);
      json = SetterBirthRecord.ToJSON();
      Assert.Contains("385671000", json); // unsuccessful outcome
      SetterBirthRecord.UnsuccessfulExternalCephalicVersion = false;
      Assert.False(SetterBirthRecord.NoObstetricProcedures);
      Assert.True(SetterBirthRecord.SuccessfulExternalCephalicVersion);
      Assert.False(SetterBirthRecord.UnsuccessfulExternalCephalicVersion);
      SetterBirthRecord.NoObstetricProcedures = true;
      Assert.True(SetterBirthRecord.NoObstetricProcedures);
      Assert.False(SetterBirthRecord.SuccessfulExternalCephalicVersion);
      Assert.False(SetterBirthRecord.UnsuccessfulExternalCephalicVersion);
      json = SetterBirthRecord.ToJSON();
      Assert.DoesNotContain("385669000", json); // successful outcome
      Assert.DoesNotContain("385671000", json); // successful outcome
      SetterBirthRecord.SuccessfulExternalCephalicVersion = true;
      Assert.False(SetterBirthRecord.NoObstetricProcedures);
      Assert.True(SetterBirthRecord.SuccessfulExternalCephalicVersion);
      Assert.False(SetterBirthRecord.UnsuccessfulExternalCephalicVersion);
    }

    // Oddities about the example Baby G. Quinn PatientChildVitalRecord JSON in the IG:
    // The name use is set to 'usual' but 'official' is required.
    // The birthdate is in a 'birthDate' field, not in a 'partialDate' extension.
    // Why is multiplebirth[x] listed in the example as multipleBirthInteger?

    [Fact]
    public void Set_Plurality()
    {
      Assert.Null(SetterBirthRecord.SetOrder);
      Assert.Null(SetterBirthRecord.Plurality);
      Assert.Equal("", SetterBirthRecord.PluralityEditFlag["code"]);
      SetterBirthRecord.SetOrder = null;
      Assert.Null(SetterBirthRecord.SetOrder);
      SetterBirthRecord.SetOrder = 3;
      Assert.Equal(3, SetterBirthRecord.SetOrder);
      Assert.Null(SetterBirthRecord.Plurality);
      Assert.Equal("", SetterBirthRecord.PluralityEditFlag["code"]);
      SetterBirthRecord.Plurality = 4;
      Assert.Equal(3, SetterBirthRecord.SetOrder);
      Assert.Equal(4, SetterBirthRecord.Plurality);
      Assert.Equal("", SetterBirthRecord.PluralityEditFlag["code"]);
      SetterBirthRecord.SetOrder = -1;
      Assert.Equal(-1, SetterBirthRecord.SetOrder);
      Assert.Equal(4, SetterBirthRecord.Plurality);
      Assert.Equal("", SetterBirthRecord.PluralityEditFlag["code"]);
      SetterBirthRecord.SetOrder = null;
      Assert.Null(SetterBirthRecord.SetOrder);
      Assert.Equal(4, SetterBirthRecord.Plurality);
      Assert.Equal("", SetterBirthRecord.PluralityEditFlag["code"]);
      var coding = new Dictionary<string, string>();
      coding.Add("code", "queriedCorrect");
      coding.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");
      SetterBirthRecord.PluralityEditFlag = coding;
      Assert.Null(SetterBirthRecord.SetOrder);
      Assert.Equal(4, SetterBirthRecord.Plurality);
      Assert.Equal("queriedCorrect", SetterBirthRecord.PluralityEditFlag["code"]);
      SetterBirthRecord.Plurality = 2;
      SetterBirthRecord.SetOrder = 1;
      Assert.Equal(1, SetterBirthRecord.SetOrder);
      Assert.Equal(2, SetterBirthRecord.Plurality);
      Assert.Equal("queriedCorrect", SetterBirthRecord.PluralityEditFlag["code"]);
    }

    [Fact]
    public void Set_MotherEthnicity1()
    {
      // default Ethnicity should be null
      Assert.Null(SetterBirthRecord.MotherEthnicity1Helper);

      SetterBirthRecord.MotherEthnicity1Helper = "N";
      Dictionary<string, string> CodeN = new Dictionary<string, string>();
      CodeN.Add("code", VR.ValueSets.HispanicNoUnknown.Codes[0, 0]);
      CodeN.Add("display", VR.ValueSets.HispanicNoUnknown.Codes[0, 1]);
      CodeN.Add("system", VR.ValueSets.HispanicNoUnknown.Codes[0, 2]);
      Assert.Equal("N", SetterBirthRecord.MotherEthnicity1Helper);
      Assert.Equal(CodeN, SetterBirthRecord.MotherEthnicity1);

      SetterBirthRecord.MotherEthnicity1Helper = "Y";
      Dictionary<string, string> CodeY = new Dictionary<string, string>();
      CodeY.Add("code", VR.ValueSets.HispanicNoUnknown.Codes[1, 0]);
      CodeY.Add("display", VR.ValueSets.HispanicNoUnknown.Codes[1, 1]);
      CodeY.Add("system", VR.ValueSets.HispanicNoUnknown.Codes[1, 2]);
      Assert.Equal("Y", SetterBirthRecord.MotherEthnicity1Helper);
      Assert.Equal(CodeY, SetterBirthRecord.MotherEthnicity1);

      SetterBirthRecord.MotherEthnicity1Helper = "UNK";
      Dictionary<string, string> CodeU = new Dictionary<string, string>();
      CodeU.Add("code", VR.ValueSets.HispanicNoUnknown.Codes[2, 0]);
      CodeU.Add("display", VR.ValueSets.HispanicNoUnknown.Codes[2, 1]);
      CodeU.Add("system", VR.ValueSets.HispanicNoUnknown.Codes[2, 2]);
      Assert.Equal("UNK", SetterBirthRecord.MotherEthnicity1Helper);
      Assert.Equal(CodeU, SetterBirthRecord.MotherEthnicity1);
    }

    [Fact]
    public void Set_FatherEthnicity1()
    {
      // default Ethnicity should be null
      Assert.Null(SetterBirthRecord.FatherEthnicity1Helper);

      SetterBirthRecord.FatherEthnicity1Helper = "N";
      Dictionary<string, string> CodeN = new Dictionary<string, string>();
      CodeN.Add("code", VR.ValueSets.HispanicNoUnknown.Codes[0, 0]);
      CodeN.Add("display", VR.ValueSets.HispanicNoUnknown.Codes[0, 1]);
      CodeN.Add("system", VR.ValueSets.HispanicNoUnknown.Codes[0, 2]);
      Assert.Equal("N", SetterBirthRecord.FatherEthnicity1Helper);
      Assert.Equal(CodeN, SetterBirthRecord.FatherEthnicity1);

      SetterBirthRecord.FatherEthnicity1Helper = "Y";
      Dictionary<string, string> CodeY = new Dictionary<string, string>();
      CodeY.Add("code", VR.ValueSets.HispanicNoUnknown.Codes[1, 0]);
      CodeY.Add("display", VR.ValueSets.HispanicNoUnknown.Codes[1, 1]);
      CodeY.Add("system", VR.ValueSets.HispanicNoUnknown.Codes[1, 2]);
      Assert.Equal("Y", SetterBirthRecord.FatherEthnicity1Helper);
      Assert.Equal(CodeY, SetterBirthRecord.FatherEthnicity1);

      SetterBirthRecord.FatherEthnicity1Helper = "UNK";
      Dictionary<string, string> CodeU = new Dictionary<string, string>();
      CodeU.Add("code", VR.ValueSets.HispanicNoUnknown.Codes[2, 0]);
      CodeU.Add("display", VR.ValueSets.HispanicNoUnknown.Codes[2, 1]);
      CodeU.Add("system", VR.ValueSets.HispanicNoUnknown.Codes[2, 2]);
      Assert.Equal("UNK", SetterBirthRecord.FatherEthnicity1Helper);
      Assert.Equal(CodeU, SetterBirthRecord.FatherEthnicity1);
    }

    [Fact]
    public void Set_MotherEthnicityLiteral()
    {
      // default Ethnicity should be null
      Assert.Null(SetterBirthRecord.MotherEthnicityLiteral);
      SetterBirthRecord.MotherEthnicityLiteral = "Guatemalan";
      Assert.Equal("Guatemalan", SetterBirthRecord.MotherEthnicityLiteral);
    }

    [Fact]
    public void Set_FatherEthnicityLiteral()
    {
      // default Ethnicity should be null
      Assert.Null(SetterBirthRecord.FatherEthnicityLiteral);
      SetterBirthRecord.FatherEthnicityLiteral = "Guatemalan";
      Assert.Equal("Guatemalan", SetterBirthRecord.FatherEthnicityLiteral);
    }

    [Fact]
    public void TestPatientChildVitalRecordProperties()
    {
      // Test FHIR record import.
      BirthRecord firstRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      string firstDescription = firstRecord.ToDescription();
      // Test conversion via FromDescription.
      BirthRecord secondRecord = VitalRecord.FromDescription<BirthRecord>(firstDescription);

      // Record Identifier
      Assert.Equal("48858", firstRecord.Identifier);
      Assert.Equal(firstRecord.BirthRecordIdentifier, secondRecord.BirthRecordIdentifier);
      Assert.Equal(firstRecord.Identifier, secondRecord.Identifier);
      // Date of Birth - Year
      Assert.Equal(2019, firstRecord.BirthYear);
      Assert.Equal(firstRecord.BirthYear, secondRecord.BirthYear);
      // Date of Birth - Month
      Assert.Equal(2, firstRecord.BirthMonth);
      Assert.Equal(firstRecord.BirthMonth, secondRecord.BirthMonth);
      // Date of Birth - Day
      Assert.Equal(25, firstRecord.BirthDay);
      Assert.Equal(firstRecord.BirthDay, secondRecord.BirthDay);
      // Complete Date of Birth.
      Assert.Equal("2019-02-25", firstRecord.DateOfBirth);
      Assert.Equal(firstRecord.DateOfBirth, secondRecord.DateOfBirth);
      // State of Birth
      Assert.Equal("UT", firstRecord.PlaceOfBirth["addressState"]);
      Assert.Equal(firstRecord.PlaceOfBirth["addressState"], secondRecord.PlaceOfBirth["addressState"]);
      Assert.Equal("UT", firstRecord.BirthLocationJurisdiction); // TODO - Birth Location Jurisdiction still needs to be finalized.
      // Time of Birth
      Assert.Equal("13:00:00", firstRecord.BirthTime);
      Assert.Equal(firstRecord.BirthTime, secondRecord.BirthTime);
      // Sex
      Assert.Equal("F", firstRecord.BirthSex["code"]);
      Assert.Equal(firstRecord.BirthSex, secondRecord.BirthSex);
      Assert.Equal("F", firstRecord.BirthSexHelper);
      Assert.Equal(firstRecord.BirthSex["code"], secondRecord.BirthSexHelper);
      // Plurality
      // TODO ---
      // Set Order
      // TODO ---
      Assert.Equal(1, firstRecord.SetOrder);
      Assert.Equal(firstRecord.SetOrder, secondRecord.SetOrder);
      // Mother's Age
      // TODO ---
      // Father's Age
      // TODO ---
      // Child's First Name
      Assert.Equal("Baby", firstRecord.ChildGivenNames[0]);
      Assert.Equal(firstRecord.ChildGivenNames[0], secondRecord.ChildGivenNames[0]);
      // Child's Middle Name
      Assert.Equal("G", firstRecord.ChildGivenNames[1]);
      Assert.Equal(firstRecord.ChildGivenNames[1], secondRecord.ChildGivenNames[1]);
      // Child's Last Name
      Assert.Equal("Quinn", firstRecord.ChildFamilyName);
      Assert.Equal(firstRecord.ChildFamilyName, secondRecord.ChildFamilyName);
      // Child's Surname Suffix
      Assert.Equal("III", firstRecord.ChildSuffix);
      Assert.Equal(firstRecord.ChildSuffix, secondRecord.ChildSuffix);
      // County of Birth (Literal)
      Assert.Equal("Salt Lake", firstRecord.PlaceOfBirth["addressCounty"]);
      Assert.Equal(firstRecord.PlaceOfBirth["addressCounty"], secondRecord.PlaceOfBirth["addressCounty"]);
      // City/town/place of birth (Literal)
      Assert.Equal("Salt Lake City", firstRecord.PlaceOfBirth["addressCity"]);
      Assert.Equal(firstRecord.PlaceOfBirth["addressCity"], secondRecord.PlaceOfBirth["addressCity"]);
      // Infant Medical Record Number
      Assert.Equal("9932702", firstRecord.InfantMedicalRecordNumber);
      Assert.Equal(firstRecord.InfantMedicalRecordNumber, secondRecord.InfantMedicalRecordNumber);
      // Mother Medical Record Number
      // TODO ---
      // Mother Social Security Number
      // TODO ---
    }

    [Fact]
    public void TestPatientMotherVitalRecordProperties()
    {
      // Test FHIR record import.
      BirthRecord firstRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      string firstDescription = firstRecord.ToDescription();
      // Test conversion via FromDescription.
      BirthRecord secondRecord = VitalRecord.FromDescription<BirthRecord>(firstDescription);

      // Mother Date of Birth - Year
      Assert.Equal(1985, firstRecord.MotherBirthYear);
      Assert.Equal(firstRecord.MotherBirthYear, secondRecord.MotherBirthYear);
      // Mother Date of Birth - Month
      Assert.Equal(1, firstRecord.MotherBirthMonth);
      Assert.Equal(firstRecord.MotherBirthMonth, secondRecord.MotherBirthMonth);
      // Mother Date of Birth - Day
      Assert.Equal(15, firstRecord.MotherBirthDay);
      Assert.Equal(firstRecord.MotherBirthDay, secondRecord.MotherBirthDay);
      // Mother Complete Date of Birth.
      Assert.Equal("1985-01-15", firstRecord.MotherDateOfBirth);
      Assert.Equal(firstRecord.MotherDateOfBirth, secondRecord.MotherDateOfBirth);
    }

    [Fact]
    public void TestRelatedPersonFatherProperties()
    {
      // Test FHIR record import.
      BirthRecord firstRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      string firstDescription = firstRecord.ToDescription();
      // Test conversion via FromDescription.
      BirthRecord secondRecord = VitalRecord.FromDescription<BirthRecord>(firstDescription);

      // Mother Date of Birth - Year
      Assert.Equal(1972, firstRecord.FatherBirthYear);
      Assert.Equal(firstRecord.FatherBirthYear, secondRecord.FatherBirthYear);
      // Mother Date of Birth - Month
      Assert.Equal(11, firstRecord.FatherBirthMonth);
      Assert.Equal(firstRecord.FatherBirthMonth, secondRecord.FatherBirthMonth);
      // Mother Date of Birth - Day
      Assert.Equal(24, firstRecord.FatherBirthDay);
      Assert.Equal(firstRecord.FatherBirthDay, secondRecord.FatherBirthDay);
      // Mother Complete Date of Birth.
      Assert.Equal("1972-11-24", firstRecord.FatherDateOfBirth);
      Assert.Equal(firstRecord.FatherDateOfBirth, secondRecord.FatherDateOfBirth);
    }

    [Fact]
    public void TestChildBirthDateSetters()
    {
      BirthRecord record1 = new BirthRecord();
      // Date of Birth - Year
      record1.BirthYear = 2021;
      Assert.Equal(2021, record1.BirthYear);
      // Date of Birth - Month
      record1.BirthMonth = 3;
      Assert.Equal(3, record1.BirthMonth);
      // Date of Birth - Day
      record1.BirthDay = 9;
      Assert.Equal(9, record1.BirthDay);
      // Complete Date of Birth.
      Assert.Equal("2021-03-09", record1.DateOfBirth);
      // TODO - there should be a test that double checks that the output FHIR JSON has the _birthDate and birthDate.
      // Time of Birth
      record1.BirthTime = "13:00:00";
      Assert.Equal("13:00:00", record1.BirthTime);

      // Test in a different order.
      BirthRecord record2 = new BirthRecord();
      // Time of Birth
      record2.BirthTime = "07:30:00";
      Assert.Equal("07:30:00", record2.BirthTime);
      // Date of Birth - Day
      record2.BirthDay = 29;
      Assert.Equal(29, record2.BirthDay);
      // Date of Birth - Month
      record2.BirthMonth = 11;
      Assert.Equal(11, record2.BirthMonth);
      // Date of Birth - Year
      record2.BirthYear = 2022;
      Assert.Equal(2022, record2.BirthYear);
      // Complete Date of Birth.
      Assert.Equal("2022-11-29", record2.DateOfBirth);
      record2.DateOfBirth = "2022-10-28";
      Assert.Equal("2022-10-28", record2.DateOfBirth);
      Assert.Equal(28, record2.BirthDay);
      Assert.Equal(10, record2.BirthMonth);
      Assert.Equal(2022, record2.BirthYear);
      Assert.Equal("07:30:00", record2.BirthTime);

      // Test updating a completed date.
      BirthRecord record3 = new BirthRecord();
      record3.DateOfBirth = "1990-10-08";
      record3.BirthMonth = 8;
      record3.BirthTime = "12:36:00";
      Assert.Equal("1990-08-08", record3.DateOfBirth);
      Assert.Equal("12:36:00", record3.BirthTime);
      
      // Test partial dates and times.
      BirthRecord record4 = new BirthRecord();
      // Birth Time
      record4.BirthTime = "12:36:00";
      Assert.Equal("12:36:00", record4.BirthTime);
      // Date of Birth - Year
      record4.BirthYear = 1992;
      Assert.Equal(1992, record4.BirthYear);
      Assert.Equal("1992", record4.DateOfBirth);
      Assert.Equal("12:36:00", record4.BirthTime);
      // Date of Birth - Month
      record4.BirthMonth = 7;
      Assert.Equal(7, record4.BirthMonth);
      Assert.Equal("1992-07", record4.DateOfBirth);
      Assert.Equal("12:36:00", record4.BirthTime);
      // Date of Birth - Day
      record4.BirthDay = 3;
      Assert.Equal(3, record4.BirthDay);
      Assert.Equal("12:36:00", record4.BirthTime);
      // Complete Date of Birth.
      Assert.Equal("1992-07-03", record4.DateOfBirth);
      record4.DateOfBirth = "1993-06";
      Assert.Equal("1993-06", record4.DateOfBirth);
      Assert.Null(record4.BirthDay);
      Assert.Equal(6, record4.BirthMonth);
      Assert.Equal(1993, record4.BirthYear);
      Assert.Equal("12:36:00", record4.BirthTime);
      record4.DateOfBirth = "1994";
      Assert.Equal("1994", record4.DateOfBirth);
      Assert.Null(record4.BirthDay);
      Assert.Null(record4.BirthMonth);
      Assert.Equal(1994, record4.BirthYear);
      Assert.Equal("12:36:00", record4.BirthTime);
      // Birth time again
      record4.BirthTime = "09:45:28";
      Assert.Equal("1994", record4.DateOfBirth);
      Assert.Null(record4.BirthDay);
      Assert.Null(record4.BirthMonth);
      Assert.Equal(1994, record4.BirthYear);
      Assert.Equal("09:45:28", record4.BirthTime);

      // Test partial dates and times in weird orders.
      BirthRecord record5 = new BirthRecord();
      // Date of Birth - Day
      record5.BirthDay = 5;
      Assert.Equal(5, record5.BirthDay);
      Assert.Null(record5.DateOfBirth);
      Assert.Null(record5.BirthTime);
      // Birth Time
      record5.BirthTime = "01:05:04";
      Assert.Equal(5, record5.BirthDay);
      Assert.Null(record5.DateOfBirth);
      Assert.Equal("01:05:04", record5.BirthTime);
      // Date of Birth - Month
      record5.BirthMonth = 9;
      Assert.Equal(5, record5.BirthDay);
      Assert.Equal(9, record5.BirthMonth);
      Assert.Null(record5.DateOfBirth);
      Assert.Equal("01:05:04", record5.BirthTime);
      // Date of Birth - Year
      record5.BirthYear = 1988;
      Assert.Equal(5, record5.BirthDay);
      Assert.Equal(9, record5.BirthMonth);
      Assert.Equal(1988, record5.BirthYear);
      Assert.Equal("1988-09-05", record5.DateOfBirth);
      Assert.Equal("01:05:04", record5.BirthTime);
    }

    [Fact]
    public void TestChildBirthDateTimeUnknowns()
    {
      BirthRecord record1 = new BirthRecord();
      record1.DateOfBirth = "1990-08-29";
      record1.BirthTime = "11:22:33";
      Assert.Equal("1990-08-29", record1.DateOfBirth);
      Assert.Equal("11:22:33", record1.BirthTime);
      record1.BirthYear = -1;
      Assert.Equal(-1, record1.BirthYear);
      Assert.Equal(8, record1.BirthMonth);
      Assert.Equal(29, record1.BirthDay);
      Assert.Null(record1.DateOfBirth);
      Assert.Equal("11:22:33", record1.BirthTime);
      record1.BirthYear = 2000;
      Assert.Equal(2000, record1.BirthYear);
      Assert.Equal(8, record1.BirthMonth);
      Assert.Equal(29, record1.BirthDay);
      Assert.Equal("11:22:33", record1.BirthTime);
      record1.BirthTime = "-1";
      Assert.Equal(2000, record1.BirthYear);
      Assert.Equal(8, record1.BirthMonth);
      Assert.Equal(29, record1.BirthDay);
      Assert.Equal("-1", record1.BirthTime);
      record1.BirthMonth = -1;
      Assert.Equal("2000", record1.DateOfBirth);
      Assert.Equal(-1, record1.BirthMonth);
      Assert.Equal(29, record1.BirthDay);
      Assert.Equal("-1", record1.BirthTime);
      record1.BirthMonth = 3;
      Assert.Equal("2000-03-29", record1.DateOfBirth);
      Assert.Equal("-1", record1.BirthTime);
      record1.BirthDay = -1;
      Assert.Equal(-1, record1.BirthDay);
      Assert.Equal("2000-03", record1.DateOfBirth);
      Assert.Equal("-1", record1.BirthTime);
      record1.BirthTime = "07:52:43";
      Assert.Equal(-1, record1.BirthDay);
      Assert.Equal("2000-03", record1.DateOfBirth);
      Assert.Equal("07:52:43", record1.BirthTime);
    }

    [Fact]
    public void TestChildBirthPlaceSetters()
    {
      BirthRecord record = new BirthRecord();
      // State of Birth
      Dictionary<string, string> placeOfBirth = new Dictionary<string, string>();
      placeOfBirth["addressState"] = "UT";
      placeOfBirth["addressCounty"] = "Salt Lake";
      placeOfBirth["addressCity"] = "Salt Lake City";
      record.PlaceOfBirth = placeOfBirth;
      Assert.Equal("UT", record.PlaceOfBirth["addressState"]);
      Assert.Equal("UT", record.BirthLocationJurisdiction); // TODO - Birth Location Jurisdiction still needs to be finalized.
      // County of Birth (Literal)
      Assert.Equal("Salt Lake", record.PlaceOfBirth["addressCounty"]);
      // City/town/place of birth (Literal)
      Assert.Equal("Salt Lake City", record.PlaceOfBirth["addressCity"]);
    }

    [Fact]
    public void TestChildNameSetters()
    {
      BirthRecord record = new BirthRecord();
      Assert.Empty(record.ChildGivenNames);
      Assert.Null(record.ChildFamilyName);
      Assert.Null(record.ChildSuffix);
      // Child's First Name
      string[] names = {"Baby", "G"};
      record.ChildGivenNames = names;
      Assert.Equal("Baby", record.ChildGivenNames[0]);
      // Child's Middle Name
      Assert.Equal("G", record.ChildGivenNames[1]);
      // Child's Last Name
      record.ChildFamilyName = "Quinn";
      Assert.Equal("Quinn", record.ChildFamilyName);
      // Child's Surname Suffix
      record.ChildSuffix = "III";
      Assert.Equal("III", record.ChildSuffix);
    }

    [Fact]
    public void TestMotherNameSetters()
    {
      BirthRecord record = new BirthRecord();
      Assert.Empty(record.MotherGivenNames);
      Assert.Null(record.MotherFamilyName);
      Assert.Null(record.MotherSuffix);
      // Mother's First Name
      string[] names = {"Mommy", "D"};
      record.MotherGivenNames = names;
      Assert.Equal("Mommy", record.MotherGivenNames[0]);
      // Mother's Middle Name
      Assert.Equal("D", record.MotherGivenNames[1]);
      // Mother's Last Name
      record.MotherFamilyName = "Quin";
      Assert.Equal("Quin", record.MotherFamilyName);
      // Mother's Surname Suffix
      record.MotherSuffix = "II";
      Assert.Equal("II", record.MotherSuffix);
    }

    [Fact]
    public void TestMotherMaidenNameSetters()
    {
      BirthRecord record = new BirthRecord();
      Assert.Empty(record.MotherMaidenGivenNames);
      Assert.Null(record.MotherMaidenFamilyName);
      Assert.Null(record.MotherMaidenSuffix);
      // Mother's Maiden First Name
      string[] names = {"Maiden", "A"};
      record.MotherMaidenGivenNames = names;
      Assert.Equal("Maiden", record.MotherMaidenGivenNames[0]);
      // Mother's Maiden Middle Name
      Assert.Equal("A", record.MotherMaidenGivenNames[1]);
      // Mother's Maiden Last Name
      record.MotherMaidenFamilyName = "Quince";
      Assert.Equal("Quince", record.MotherMaidenFamilyName);
      // Mother's Surname Suffix
      record.MotherMaidenSuffix = "IV";
      Assert.Equal("IV", record.MotherMaidenSuffix);
    }

    [Fact]
    public void TestChildSexSetters()
    {
      BirthRecord record = new BirthRecord();
      record.BirthSexHelper = "F";
      Assert.Equal("F", record.BirthSex["code"]);
      Assert.Equal("F", record.BirthSexHelper);
      record.BirthSexHelper = "M";
      Assert.Equal("M", record.BirthSex["code"]);
      Assert.Equal("M", record.BirthSexHelper);
    }

    [Fact]
    public void TestChildIdentifierSetters()
    {
      BirthRecord record = new BirthRecord();
      // Record Identifier
      record.Identifier = "87366";
      // Infant Medical Record Number
      record.InfantMedicalRecordNumber = "9932734";
      Assert.Equal("9932734", record.InfantMedicalRecordNumber);
    }

    [Fact]
    public void TestMotherIdentifierSetters()
    {
      BirthRecord record = new BirthRecord();
      // Mother Medical Record Number
      record.MotherMedicalRecordNumber = "9932733";
      Assert.Equal("9932733", record.MotherMedicalRecordNumber);
      // Mother SSN
      record.MotherSocialSecurityNumber = "1234567890";
      Assert.Equal("1234567890", record.MotherSocialSecurityNumber);
    }

    [Fact]
    public void EmptyRecordToDescription()
    {
      BirthRecord birthRecord = new();
      string description = birthRecord.ToDescription();
      Assert.NotNull(description);
    }

    [Fact]
    public void EmptyRecordToIJE()
    {
      BirthRecord birthRecord = new();
      string ije = new IJENatality(birthRecord, false).ToString(); // Don't validate since empty record
      Assert.NotNull(ije);
    }

    [Fact]
    public void TestMotherBirthDateSetters()
    {
      BirthRecord record1 = new BirthRecord();
      // Date of Birth - Year
      record1.MotherBirthYear = 1990;
      Assert.Equal(1990, record1.MotherBirthYear);
      // Date of Birth - Month
      record1.MotherBirthMonth = 8;
      Assert.Equal(8, record1.MotherBirthMonth);
      // Date of Birth - Day
      record1.MotherBirthDay = 29;
      Assert.Equal(29, record1.MotherBirthDay);
      // Complete Date of Birth.
      Assert.Equal("1990-08-29", record1.MotherDateOfBirth);

      // Test in a different order.
      BirthRecord record2 = new BirthRecord();
      // Date of Birth - Day
      record2.MotherBirthDay = 1;
      Assert.Equal(1, record2.MotherBirthDay);
      // Date of Birth - Month
      record2.MotherBirthMonth = 5;
      Assert.Equal(5, record2.MotherBirthMonth);
      // Date of Birth - Year
      record2.MotherBirthYear = 1992;
      Assert.Equal(1992, record2.MotherBirthYear);
      // Complete Date of Birth.
      Assert.Equal("1992-05-01", record2.MotherDateOfBirth);
      record2.MotherDateOfBirth = "1993-06-02";
      Assert.Equal("1993-06-02", record2.MotherDateOfBirth);
      Assert.Equal(2, record2.MotherBirthDay);
      Assert.Equal(6, record2.MotherBirthMonth);
      Assert.Equal(1993, record2.MotherBirthYear);

      // Test updating a completed date.
      BirthRecord record3 = new BirthRecord();
      record3.MotherDateOfBirth = "1990-10-08";
      record3.MotherBirthMonth = 8;
      Assert.Equal("1990-08-08", record3.MotherDateOfBirth);

      // Test partial dates.
      BirthRecord record4 = new BirthRecord();
      // Date of Birth - Year
      record4.MotherBirthYear = 1992;
      Assert.Equal(1992, record4.MotherBirthYear);
      Assert.Equal("1992", record4.MotherDateOfBirth);
      // Date of Birth - Month
      record4.MotherBirthMonth = 7;
      Assert.Equal(7, record4.MotherBirthMonth);
      Assert.Equal("1992-07", record4.MotherDateOfBirth);
      // Date of Birth - Day
      record4.MotherBirthDay = 3;
      Assert.Equal(3, record4.MotherBirthDay);
      // Complete Date of Birth.
      Assert.Equal("1992-07-03", record4.MotherDateOfBirth);
      record4.MotherDateOfBirth = "1993-06";
      Assert.Equal("1993-06", record4.MotherDateOfBirth);
      Assert.Null(record4.MotherBirthDay);
      Assert.Equal(6, record4.MotherBirthMonth);
      Assert.Equal(1993, record4.MotherBirthYear);
      record4.MotherDateOfBirth = "1994";
      Assert.Equal("1994", record4.MotherDateOfBirth);
      Assert.Null(record4.MotherBirthDay);
      Assert.Null(record4.MotherBirthMonth);
      Assert.Equal(1994, record4.MotherBirthYear);

      // Test partial dates in weird orders.
      BirthRecord record5 = new BirthRecord();
      // Date of Birth - Day
      record5.MotherBirthDay = 5;
      Assert.Equal(5, record5.MotherBirthDay);
      Assert.Null(record5.MotherDateOfBirth);
      // Date of Birth - Month
      record5.MotherBirthMonth = 9;
      Assert.Equal(9, record5.MotherBirthMonth);
      Assert.Null(record5.MotherDateOfBirth);
      // Date of Birth - Year
      record5.MotherBirthYear = 1988;
      Assert.Equal(1988, record5.MotherBirthYear);
      Assert.Equal("1988-09-05", record5.MotherDateOfBirth);
    }

    [Fact]
    public void TestMotherBirthDateUnknowns()
    {
      BirthRecord record1 = new BirthRecord();
      record1.MotherDateOfBirth = "1990-08-29";
      Assert.Equal("1990-08-29", record1.MotherDateOfBirth);
      record1.MotherBirthYear = -1;
      Assert.Equal(-1, record1.MotherBirthYear);
      Assert.Equal(8, record1.MotherBirthMonth);
      Assert.Equal(29, record1.MotherBirthDay);
      Assert.Null(record1.MotherDateOfBirth);
      record1.MotherBirthYear = 2000;
      Assert.Equal(2000, record1.MotherBirthYear);
      Assert.Equal(8, record1.MotherBirthMonth);
      Assert.Equal(29, record1.MotherBirthDay);
      record1.MotherBirthMonth = -1;
      Assert.Equal("2000", record1.MotherDateOfBirth);
      Assert.Equal(-1, record1.MotherBirthMonth);
      Assert.Equal(29, record1.MotherBirthDay);
      record1.MotherBirthMonth = 3;
      Assert.Equal("2000-03-29", record1.MotherDateOfBirth);
      record1.MotherBirthDay = -1;
      Assert.Equal(-1, record1.MotherBirthDay);
      Assert.Equal("2000-03", record1.MotherDateOfBirth);
    }

    [Fact]
    public void TestFatherBirthDateUnknowns()
    {
      BirthRecord record1 = new BirthRecord();
      record1.FatherDateOfBirth = "1990-08-29";
      Assert.Equal("1990-08-29", record1.FatherDateOfBirth);
      record1.FatherBirthYear = -1;
      Assert.Equal(-1, record1.FatherBirthYear);
      Assert.Equal(8, record1.FatherBirthMonth);
      Assert.Equal(29, record1.FatherBirthDay);
      Assert.Null(record1.FatherDateOfBirth);
      record1.FatherBirthYear = 2000;
      Assert.Equal(2000, record1.FatherBirthYear);
      Assert.Equal(8, record1.FatherBirthMonth);
      Assert.Equal(29, record1.FatherBirthDay);
      record1.FatherBirthMonth = -1;
      Assert.Equal("2000", record1.FatherDateOfBirth);
      Assert.Equal(-1, record1.FatherBirthMonth);
      Assert.Equal(29, record1.FatherBirthDay);
      record1.FatherBirthMonth = 3;
      Assert.Equal("2000-03-29", record1.FatherDateOfBirth);
      record1.FatherBirthDay = -1;
      Assert.Equal(-1, record1.FatherBirthDay);
      Assert.Equal("2000-03", record1.FatherDateOfBirth);
    }

    [Fact]
    public void TestFatherBirthDateSetters()
    {
      BirthRecord record1 = new BirthRecord();
      // Date of Birth - Year
      record1.FatherBirthYear = 1990;
      Assert.Equal(1990, record1.FatherBirthYear);
      // Date of Birth - Month
      record1.FatherBirthMonth = 8;
      Assert.Equal(8, record1.FatherBirthMonth);
      // Date of Birth - Day
      record1.FatherBirthDay = 29;
      Assert.Equal(29, record1.FatherBirthDay);
      // Complete Date of Birth.
      Assert.Equal("1990-08-29", record1.FatherDateOfBirth);

      // Test in a different order.
      BirthRecord record2 = new BirthRecord();
      // Date of Birth - Day
      record2.FatherBirthDay = 1;
      Assert.Equal(1, record2.FatherBirthDay);
      // Date of Birth - Month
      record2.FatherBirthMonth = 5;
      Assert.Equal(5, record2.FatherBirthMonth);
      // Date of Birth - Year
      record2.FatherBirthYear = 1992;
      Assert.Equal(1992, record2.FatherBirthYear);
      // Complete Date of Birth.
      Assert.Equal("1992-05-01", record2.FatherDateOfBirth);
      record2.FatherDateOfBirth = "1993-06-02";
      Assert.Equal("1993-06-02", record2.FatherDateOfBirth);
      Assert.Equal(2, record2.FatherBirthDay);
      Assert.Equal(6, record2.FatherBirthMonth);
      Assert.Equal(1993, record2.FatherBirthYear);

      // Test partial dates.
      BirthRecord record3 = new BirthRecord();
      // Date of Birth - Year
      record3.FatherBirthYear = 1992;
      Assert.Equal(1992, record3.FatherBirthYear);
      Assert.Equal("1992", record3.FatherDateOfBirth);
      // Date of Birth - Month
      record3.FatherBirthMonth = 7;
      Assert.Equal(7, record3.FatherBirthMonth);
      Assert.Equal("1992-07", record3.FatherDateOfBirth);
      // Date of Birth - Day
      record3.FatherBirthDay = 3;
      Assert.Equal(3, record3.FatherBirthDay);
      // Complete Date of Birth.
      Assert.Equal("1992-07-03", record3.FatherDateOfBirth);
      record3.FatherDateOfBirth = "1993-06";
      Assert.Equal("1993-06", record3.FatherDateOfBirth);
      Assert.Null(record3.FatherBirthDay);
      Assert.Equal(6, record3.FatherBirthMonth);
      Assert.Equal(1993, record3.FatherBirthYear);
      record3.FatherDateOfBirth = "1994";
      Assert.Equal("1994", record3.FatherDateOfBirth);
      Assert.Null(record3.FatherBirthDay);
      Assert.Null(record3.FatherBirthMonth);
      Assert.Equal(1994, record3.FatherBirthYear);

      // Test partial dates in weird orders.
      BirthRecord record4 = new BirthRecord();
      // Date of Birth - Day
      record4.FatherBirthDay = 5;
      Assert.Equal(5, record4.FatherBirthDay);
      Assert.Null(record4.FatherDateOfBirth);
      // Date of Birth - Month
      record4.FatherBirthMonth = 9;
      Assert.Equal(9, record4.FatherBirthMonth);
      Assert.Null(record4.FatherDateOfBirth);
      // Date of Birth - Year
      record4.FatherBirthYear = 1988;
      Assert.Equal(1988, record4.FatherBirthYear);
      Assert.Equal("1988-09-05", record4.FatherDateOfBirth);
    }

    [Fact]
    public void ParseMotherRaceEthnicityJsonToIJE()
    {
        // Hispanic or Latino
        BirthRecord b = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/RaceEthnicityCaseRecord.json")));
        IJENatality ije1 = new IJENatality(b);
        Assert.Equal("H", ije1.METHNIC1);
        Assert.Equal("H", ije1.METHNIC2);
        Assert.Equal("H", ije1.METHNIC3);
        Assert.Equal("H", ije1.METHNIC4);
        Assert.Equal("Y", ije1.MRACE1);
        Assert.Equal("Y", ije1.MRACE2);
        Assert.Equal("Y", ije1.MRACE3);
        Assert.Equal("N", ije1.MRACE4);
        Assert.Equal("N", ije1.MRACE5);
        Assert.Equal("N", ije1.MRACE6);
        Assert.Equal("N", ije1.MRACE7);
        Assert.Equal("N", ije1.MRACE8);
        Assert.Equal("N", ije1.MRACE9);
        Assert.Equal("N", ije1.MRACE10);
        Assert.Equal("N", ije1.MRACE11);
        Assert.Equal("N", ije1.MRACE12);
        Assert.Equal("N", ije1.MRACE13);
        Assert.Equal("N", ije1.MRACE14);
        Assert.Equal("N", ije1.MRACE15);

        // Non Hispanic or Latino
        BirthRecord b2 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/RaceEthnicityCaseRecord2.json")));
        IJENatality ije2 = new IJENatality(b2);
        Assert.Equal("N", ije2.METHNIC1);
        Assert.Equal("N", ije2.METHNIC2);
        Assert.Equal("N", ije2.METHNIC3);
        Assert.Equal("N", ije2.METHNIC4);
        Assert.Equal("Y", ije2.MRACE10);
        Assert.Equal("Malaysian", ije2.MRACE18);
        Assert.Equal("Y", ije2.MRACE3);

        BirthRecord b3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
        IJENatality ije3 = new IJENatality(b3);
        Assert.Equal("H", ije3.METHNIC1);
        Assert.Equal("U", ije3.METHNIC2);
        Assert.Equal("U", ije3.METHNIC3);
        Assert.Equal("U", ije3.METHNIC4);
        Assert.Equal("", ije3.MRACE18);
        Assert.Equal("Y", ije3.MRACE1);
        Assert.Equal("Y", ije3.MRACE2);
        Assert.Equal("Y", ije3.MRACE3);
        Assert.Equal("N", ije3.MRACE4);
        Assert.Equal("N", ije3.MRACE5);
        Assert.Equal("N", ije3.MRACE6);
        Assert.Equal("N", ije3.MRACE7);
        Assert.Equal("N", ije3.MRACE8);
        Assert.Equal("N", ije3.MRACE9);
        Assert.Equal("N", ije3.MRACE10);
        Assert.Equal("N", ije3.MRACE11);
        Assert.Equal("N", ije3.MRACE12);
        Assert.Equal("N", ije3.MRACE13);
        Assert.Equal("N", ije3.MRACE14);
        Assert.Equal("N", ije3.MRACE15);
    }

    [Fact]
    public void ParseMotherRaceEthnicityIJEtoJson()
    {
        BirthRecord b = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
        IJENatality ije1 = new IJENatality(b);
        IJENatality ije2 = new IJENatality(ije1.ToString(), true);
        BirthRecord b2 = ije2.ToRecord();

        // Ethnicity tuple
        Assert.Equal("Y", b2.MotherEthnicity1Helper);

        // Race tuple
        foreach (var pair in b2.MotherRace)
        {
            switch (pair.Item1)
            {
                case NvssRace.White:
                    Assert.Equal("Y", pair.Item2);
                    break;
                case NvssRace.AmericanIndianOrAlaskanNative:
                    Assert.Equal("Y", pair.Item2);
                    break;
                default:
                    break;
            }
        }
        Assert.Equal(15, b2.MotherRace.Length);
    }


    [Fact]
    public void ParseFatherRaceEthnicityIJEtoJson()
    {
        BirthRecord b = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
        IJENatality ije1 = new IJENatality(b);
        IJENatality ije2 = new IJENatality(ije1.ToString(), true);
        BirthRecord b2 = ije2.ToRecord();

        // Ethnicity tuple
        Assert.Equal("Y", b2.FatherEthnicity1Helper);

        // Race tuple
        foreach (var pair in b2.FatherRace)
        {
            switch (pair.Item1)
            {
                case NvssRace.White:
                    Assert.Equal("N", pair.Item2);
                    break;
                case NvssRace.AmericanIndianOrAlaskanNative:
                    Assert.Equal("Y", pair.Item2);
                    break;
                default:
                    break;
            }
        }
        Assert.Equal(15, b2.FatherRace.Length);
    }

    [Fact]
    public void TestImportMotherBirthplace()
    {
      // Test FHIR record import.
      BirthRecord firstRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      string firstDescription = firstRecord.ToDescription();
      // Test conversion via FromDescription.
      BirthRecord secondRecord = VitalRecord.FromDescription<BirthRecord>(firstDescription);
      // Test IJE Conversion.
      IJENatality ije = new(secondRecord);

      // Country
      Assert.Equal("US", firstRecord.MotherBirthCountry);
      Assert.Equal(firstRecord.MotherBirthCountry, secondRecord.MotherBirthCountry);
      Assert.Equal(firstRecord.MotherBirthCountry, ije.BPLACEC_CNT);
      // State
      Assert.Equal("UT", firstRecord.MotherBirthState);
      Assert.Equal(firstRecord.MotherBirthState, secondRecord.MotherBirthState);
      Assert.Equal(firstRecord.MotherBirthState, ije.BPLACEC_ST_TER);
    }

    [Fact]
    public void TestSetMotherBirthplace()
    {
      // Manually set birth record values.
      BirthRecord br = new()
      {
          MotherBirthCountry = "US",
          MotherBirthState = "UT"
      };
      // Test IJE conversion from BirthRecord.
      IJENatality ije = new(br);

      Assert.Equal("US", br.MotherBirthCountry);
      Assert.Equal(ije.BPLACEC_CNT, br.MotherBirthCountry);
      Assert.Equal(ije.BPLACEC_ST_TER, br.MotherBirthState);
      br.MotherBirthCountry = "AA";
      Assert.Equal("AA", br.MotherBirthCountry);
      br.MotherBirthState = "MA";
      Assert.Equal("MA", br.MotherBirthState);
    }

    [Fact]
    public void TestSetPhysicalBirthPlace()
    {
      // Manually set birth record values.
      BirthRecord br1 = new()
      {
          BirthPhysicalLocationHelper = "22232009",
      };
      // Test IJE conversion from BirthRecord.
      IJENatality ije1 = new(br1);

      Assert.Equal("22232009", br1.BirthPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", br1.BirthPhysicalLocation["system"]);
      Assert.Equal("Hospital", br1.BirthPhysicalLocation["display"]);
      Assert.Equal(br1.BirthPhysicalLocationHelper, br1.BirthPhysicalLocation["code"]);

      Dictionary<string, string> birthPlaceCode = new()
      {
          ["code"] = "22232009",
          ["system"] = "http://snomed.info/sct",
          ["display"] = "Hospital"
      };
      BirthRecord br2 = new()
      {
          BirthPhysicalLocation = birthPlaceCode
      };
      IJENatality ije2 = new(br2);

      Assert.Equal("22232009", br2.BirthPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", br2.BirthPhysicalLocation["system"]);
      Assert.Equal("Hospital", br2.BirthPhysicalLocation["display"]);
      Assert.Equal(br2.BirthPhysicalLocationHelper, br2.BirthPhysicalLocation["code"]);
    }
  }
}
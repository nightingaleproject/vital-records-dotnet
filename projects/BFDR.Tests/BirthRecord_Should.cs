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
    private BirthRecord FakeBirthRecord;

    public BirthRecord_Should()
    {
      SetterBirthRecord = new BirthRecord();
      FakeBirthRecord = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BirthRecordFakeWithRace.json")));
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

      // Record Certificate Number
      Assert.Equal("48858", firstRecord.CertificateNumber);
      Assert.Equal(firstRecord.CertificateNumber, secondRecord.CertificateNumber);
      // Record Birth Record Identifier
      Assert.Equal("2019UT48858", firstRecord.BirthRecordIdentifier);
      Assert.Equal(firstRecord.BirthRecordIdentifier, secondRecord.BirthRecordIdentifier);
      // Record State Local Identifier 1
      Assert.Equal("000000000042", firstRecord.StateLocalIdentifier1);
      Assert.Equal(firstRecord.StateLocalIdentifier1, secondRecord.StateLocalIdentifier1);
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
      // County of Birth (Code)
      Assert.Equal("035", firstRecord.PlaceOfBirth["addressCountyC"]);
      Assert.Equal(firstRecord.PlaceOfBirth["addressCountyC"], secondRecord.PlaceOfBirth["addressCountyC"].PadLeft(3, '0'));
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

      // Test setting unknown date components
      record1.BirthMonth = -1;
      Assert.Equal("2021", record1.DateOfBirth);
      record1.BirthMonth = 7;
      Assert.Equal("2021-07-09", record1.DateOfBirth);
      record1.BirthYear = -1;
      Assert.Null(record1.DateOfBirth);
      record1.BirthYear = 2020;
      Assert.Equal("2020-07-09", record1.DateOfBirth);


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
    public void TestBirthPlaceSetters()
    {
      BirthRecord record = new BirthRecord();
      // State of Birth
      record.PlaceOfBirth = new Dictionary<string, string>
      {
        ["addressState"] = "UT",
        ["addressCounty"] = "Salt Lake",
        ["addressCity"] = "Salt Lake City",
        ["addressCountyC"] = "035"
      };
      record.MotherPlaceOfBirth = new Dictionary<string, string>
      {
        ["addressState"] = "MA",
        ["addressCounty"] = "Middlesex",
        ["addressCity"] = "Bedford",
        ["addressCountry"] = "US"
      };
      record.FatherPlaceOfBirth = new Dictionary<string, string>
      {
        ["addressState"] = "NH",
        ["addressCounty"] = "Hillsboro",
        ["addressCity"] = "Nashua"
      };

      IJENatality ije = new(record);

      Assert.Equal("UT", record.PlaceOfBirth["addressState"]);
      Assert.Equal("UT", record.BirthLocationJurisdiction); // TODO - Birth Location Jurisdiction still needs to be finalized.
      // County of Birth (Literal)
      Assert.Equal("Salt Lake", record.PlaceOfBirth["addressCounty"]);
      // City/town/place of birth (Literal)
      Assert.Equal("Salt Lake City", record.PlaceOfBirth["addressCity"]);
      // County of Birth (Code)
      Assert.Equal("035", record.PlaceOfBirth["addressCountyC"].PadLeft(3, '0'));
      Assert.Equal("MA", record.MotherPlaceOfBirth["addressState"]);
      Assert.Equal("Middlesex", record.MotherPlaceOfBirth["addressCounty"]);
      Assert.Equal("Bedford", record.MotherPlaceOfBirth["addressCity"]);
      Assert.Equal("US", record.MotherPlaceOfBirth["addressCountry"]);
      Assert.Equal("NH", record.FatherPlaceOfBirth["addressState"]);
      Assert.Equal("Hillsboro", record.FatherPlaceOfBirth["addressCounty"]);
      Assert.Equal("Nashua", record.FatherPlaceOfBirth["addressCity"]);
      Assert.Equal(ije.BPLACEC_CNT, record.MotherPlaceOfBirth["addressCountry"]);
      Assert.Equal(ije.BPLACEC_ST_TER, record.MotherPlaceOfBirth["addressState"]);
    }

    [Fact]
    public void TestMotherAddressSetters()
    {
      BirthRecord record = new BirthRecord();

      Dictionary<string, string> addr = new Dictionary<string, string>();
      addr["addressState"] = "UT";
      addr["addressCounty"] = "Salt Lake";
      addr["addressCity"] = "Salt Lake City";
      record.MotherResidence = addr;

      addr["addressState"] = "MA";
      addr["addressCounty"] = "Middlesex";
      addr["addressCity"] = "Bedford";
      record.MotherBilling = addr;

      Assert.Equal("UT", record.MotherResidence["addressState"]);
      Assert.Equal("Salt Lake", record.MotherResidence["addressCounty"]);
      Assert.Equal("Salt Lake City", record.MotherResidence["addressCity"]);

      Assert.Equal("MA", record.MotherBilling["addressState"]);
      Assert.Equal("Middlesex", record.MotherBilling["addressCounty"]);
      Assert.Equal("Bedford", record.MotherBilling["addressCity"]);
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
    public void TestFatherNameSetters()
    {
      BirthRecord record = new BirthRecord();
      Assert.Empty(record.FatherGivenNames);
      Assert.Null(record.FatherFamilyName);
      Assert.Null(record.FatherSuffix);
      // Father's First Name
      string[] names = {"Pappy", "C"};
      record.FatherGivenNames = names;
      Assert.Equal("Pappy", record.FatherGivenNames[0]);
      // Father's Middle Name
      Assert.Equal("C", record.FatherGivenNames[1]);
      // Father's Last Name
      record.FatherFamilyName = "Pipp";
      Assert.Equal("Pipp", record.FatherFamilyName);
      // Father's Surname Suffix
      record.FatherSuffix = "III";
      Assert.Equal("III", record.FatherSuffix);
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
      // Record Identifiers
      record.CertificateNumber = "87366";
      Assert.Equal("87366", record.CertificateNumber);
      Assert.Equal("0000XX087366", record.BirthRecordIdentifier);
      Assert.Null(record.StateLocalIdentifier1);
      record.StateLocalIdentifier1 = "0000033";
      Assert.Equal("0000033", record.StateLocalIdentifier1);
      record.BirthYear = 2020;
      record.CertificateNumber = "767676";
      Assert.Equal("2020XX767676", record.BirthRecordIdentifier);
      record.BirthLocationJurisdiction = "WY";
      record.CertificateNumber = "898989";
      Assert.Equal("2020WY898989", record.BirthRecordIdentifier);
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
    public void TestFatherIdentifierSetters()
    {
      BirthRecord record = new BirthRecord();
      // Father SSN
      record.FatherSocialSecurityNumber = "1231231234";
      Assert.Equal("1231231234", record.FatherSocialSecurityNumber);
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
      Assert.Null(record1.MotherReportedAgeAtDelivery);
      record1.MotherReportedAgeAtDelivery = 27;
      Assert.Equal(27, record1.MotherReportedAgeAtDelivery);

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
      Assert.Null(record1.FatherReportedAgeAtDelivery);
      record1.FatherReportedAgeAtDelivery = 28;
      Assert.Equal(28, record1.FatherReportedAgeAtDelivery);

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
    public void IdentifiersPresent()
    {
      Assert.Equal("100", FakeBirthRecord.CertificateNumber);
      Assert.Equal("123", FakeBirthRecord.StateLocalIdentifier1);
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

      Assert.Equal(firstRecord.MotherPlaceOfBirth, secondRecord.MotherPlaceOfBirth);
      // Country
      Assert.Equal("US", firstRecord.MotherPlaceOfBirth["addressCountry"]);
      Assert.Equal(firstRecord.MotherPlaceOfBirth["addressCountry"], ije.BPLACEC_CNT);
      // State
      Assert.Equal("UT", firstRecord.MotherPlaceOfBirth["addressState"]);
      Assert.Equal(firstRecord.MotherPlaceOfBirth["addressState"], ije.BPLACEC_ST_TER);
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
      IJENatality ije = new(br1);

      Assert.Equal("22232009", br1.BirthPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", br1.BirthPhysicalLocation["system"]);
      Assert.Equal("Hospital", br1.BirthPhysicalLocation["display"]);
      Assert.Equal(br1.BirthPhysicalLocationHelper, br1.BirthPhysicalLocation["code"]);
      Assert.Equal("1", ije.BPLACE);

      Dictionary<string, string> birthPlaceCode = new()
      {
          ["code"] = "22232009",
          ["system"] = "http://snomed.info/sct",
          ["display"] = "Hospital"
      };
      br1.BirthPhysicalLocation = birthPlaceCode;
      ije = new(br1);
      Assert.Equal("22232009", br1.BirthPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", br1.BirthPhysicalLocation["system"]);
      Assert.Equal("Hospital", br1.BirthPhysicalLocation["display"]);
      Assert.Equal(br1.BirthPhysicalLocationHelper, br1.BirthPhysicalLocation["code"]);
      Assert.Equal("1", ije.BPLACE);

      br1.BirthPhysicalLocationHelper = "67190003";
      ije = new(br1);
      Assert.Equal("67190003", br1.BirthPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", br1.BirthPhysicalLocation["system"]);
      Assert.Equal("Free-standing clinic", br1.BirthPhysicalLocation["display"]);
      Assert.Equal(br1.BirthPhysicalLocationHelper, br1.BirthPhysicalLocation["code"]);
      Assert.Equal("6", ije.BPLACE);
    }

    [Fact]
    public void TestImportPhysicalBirthPlace()
    {
      // Test FHIR record import.
      BirthRecord firstRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      // Test conversion via FromDescription.
      BirthRecord secondRecord = VitalRecord.FromDescription<BirthRecord>(firstRecord.ToDescription());

      Assert.Equal("22232009", firstRecord.BirthPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", firstRecord.BirthPhysicalLocation["system"]);
      Assert.Equal("Hospital", firstRecord.BirthPhysicalLocation["display"]);
      Assert.Equal(firstRecord.BirthPhysicalLocationHelper, firstRecord.BirthPhysicalLocation["code"]);
      Assert.Equal("22232009", secondRecord.BirthPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", secondRecord.BirthPhysicalLocation["system"]);
      Assert.Equal("Hospital", secondRecord.BirthPhysicalLocation["display"]);
      Assert.Equal(secondRecord.BirthPhysicalLocationHelper, secondRecord.BirthPhysicalLocation["code"]);
    }

    [Fact]
    public void TestAttendantPropertiesSetter()
    {
        // Attendant's name
        Assert.Null(SetterBirthRecord.AttendantName);
        SetterBirthRecord.AttendantName = "Janet Seito";
        Assert.Equal("Janet Seito", SetterBirthRecord.AttendantName);
        // Attendant's NPI
        Assert.Null(SetterBirthRecord.AttendantNPI);
        SetterBirthRecord.AttendantNPI = "123456789011";
        Assert.Equal("123456789011", SetterBirthRecord.AttendantNPI);
        // Attendant's Title
        Dictionary<string, string> AttendantTitle = new Dictionary<string, string>();
        AttendantTitle.Add("code", "112247003");
        AttendantTitle.Add("system", CodeSystems.SCT);
        AttendantTitle.Add("display", "Medical Doctor");
        SetterBirthRecord.AttendantTitle = AttendantTitle;
        Assert.Equal("112247003", SetterBirthRecord.AttendantTitle["code"]);
        Assert.Equal(CodeSystems.SCT, SetterBirthRecord.AttendantTitle["system"]);
        Assert.Equal("Medical Doctor", SetterBirthRecord.AttendantTitle["display"]);
        // test setting other Attendant Title
        BirthRecord record2 = new BirthRecord();
        record2.AttendantName = "Jessica Leung";
        Assert.Equal("Jessica Leung", record2.AttendantName);
        record2.AttendantTitleHelper = "Birth Clerk"; //set using Title helper
        Assert.Equal("OTH", record2.AttendantTitle["code"]);
        Assert.Equal(CodeSystems.NullFlavor_HL7_V3, record2.AttendantTitle["system"]);
        Assert.Equal("Other", record2.AttendantTitle["display"]);
        Assert.Equal("Birth Clerk", record2.AttendantTitle["text"]);
        record2.AttendantOtherHelper = "Birth Clerk"; //set using Other helper
        Assert.Equal("OTH", record2.AttendantTitle["code"]);
        Assert.Equal(CodeSystems.NullFlavor_HL7_V3, record2.AttendantTitle["system"]);
        Assert.Equal("Other", record2.AttendantTitle["display"]);
        Assert.Equal("Birth Clerk", record2.AttendantTitle["text"]);
        Assert.Equal("Birth Clerk", record2.AttendantOtherHelper);
        // test IJE translations
        IJENatality ije1 = new IJENatality(SetterBirthRecord);
        Assert.Equal("Janet Seito", ije1.ATTEND_NAME.Trim());
        Assert.Equal("123456789011", ije1.ATTEND_NPI);
        Assert.Equal("1", ije1.ATTEND);
        IJENatality ije2 = new IJENatality(record2);
        Assert.Equal("Jessica Leung", ije2.ATTEND_NAME.Trim());
        Assert.Equal("            ", ije2.ATTEND_NPI);
        Assert.Equal("5", ije2.ATTEND);
        Assert.Equal("Birth Clerk", ije2.ATTEND_OTH_TXT.Trim());
    }

    [Fact]
    public void TestCertifierPropertiesSetter()
    {
        // Certifier's name
        Assert.Null(SetterBirthRecord.CertifierName);
        SetterBirthRecord.CertifierName = "Avery Jones";
        Assert.Equal("Avery Jones", SetterBirthRecord.CertifierName);
        // Certifier's NPI
        Assert.Null(SetterBirthRecord.CertifierNPI);
        SetterBirthRecord.CertifierNPI = "123456789011";
        Assert.Equal("123456789011", SetterBirthRecord.CertifierNPI);
        // Certifier's Title
        Dictionary<string, string> CertifierTitle = new Dictionary<string, string>();
        CertifierTitle.Add("code", "76231001");
        CertifierTitle.Add("system", CodeSystems.SCT);
        CertifierTitle.Add("display", "Osteopath");
        SetterBirthRecord.CertifierTitle = CertifierTitle;
        Assert.Equal("76231001", SetterBirthRecord.CertifierTitle["code"]);
        Assert.Equal(CodeSystems.SCT, SetterBirthRecord.CertifierTitle["system"]);
        Assert.Equal("Osteopath", SetterBirthRecord.CertifierTitle["display"]);
        // test setting other Certifier Title
        BirthRecord record2 = new BirthRecord();
        record2.CertifierName = "Jessica Leung";
        Assert.Equal("Jessica Leung", record2.CertifierName);
        record2.CertifierTitleHelper = "Birth Clerk"; //set using Title helper
        Assert.Equal("OTH", record2.CertifierTitle["code"]);
        Assert.Equal(CodeSystems.NullFlavor_HL7_V3, record2.CertifierTitle["system"]);
        Assert.Equal("Other", record2.CertifierTitle["display"]);
        Assert.Equal("Birth Clerk", record2.CertifierTitle["text"]);
        record2.CertifierOtherHelper = "Birth Clerk"; //set using Other helper
        Assert.Equal("OTH", record2.CertifierTitle["code"]);
        Assert.Equal(CodeSystems.NullFlavor_HL7_V3, record2.CertifierTitle["system"]);
        Assert.Equal("Other", record2.CertifierTitle["display"]);
        Assert.Equal("Birth Clerk", record2.CertifierTitle["text"]);
        Assert.Equal("Birth Clerk", record2.CertifierOtherHelper);
        // test IJE translations
        IJENatality ije1 = new IJENatality(SetterBirthRecord);
        Assert.Equal("Avery Jones", ije1.CERTIF_NAME.Trim());
        Assert.Equal("123456789011", ije1.CERTIF_NPI);
        Assert.Equal("2", ije1.CERTIF);
        IJENatality ije2 = new IJENatality(record2);
        Assert.Equal("Jessica Leung", ije2.CERTIF_NAME.Trim());
        Assert.Equal("            ", ije2.CERTIF_NPI);
        Assert.Equal("5", ije2.CERTIF);
        Assert.Equal("Birth Clerk", ije2.CERTIF_OTH_TXT.Trim());
    }

    [Fact]
    public void PatientBirthDatePresent()
    {
      Assert.Equal(2023, FakeBirthRecord.BirthYear);
      Assert.Equal(1, FakeBirthRecord.BirthMonth);
      Assert.Equal(1, FakeBirthRecord.BirthDay);
      Assert.Equal("2023-01-01", FakeBirthRecord.DateOfBirth);
    }
    [Fact]
    public void ParentBirthDatesPresent()
    {
      Assert.Equal(1992, FakeBirthRecord.MotherBirthYear);
      Assert.Equal(1, FakeBirthRecord.MotherBirthMonth);
      Assert.Equal(12, FakeBirthRecord.MotherBirthDay);
      Assert.Equal("1992-01-12", FakeBirthRecord.MotherDateOfBirth);
      Assert.Equal(1990, FakeBirthRecord.FatherBirthYear );
      Assert.Equal(9, FakeBirthRecord.FatherBirthMonth);
      Assert.Equal(21, FakeBirthRecord.FatherBirthDay);
      Assert.Equal("1990-09-21", FakeBirthRecord.FatherDateOfBirth);
    }
    [Fact]
    public void ChildNamesPresent()
    {
      Assert.Equal("Alexander", FakeBirthRecord.ChildGivenNames[0]);
      Assert.Equal("Arlo", FakeBirthRecord.ChildGivenNames[1]);
      Assert.Equal("Adkins", FakeBirthRecord.ChildFamilyName);
    }
    [Fact]
    public void MotherNamesPresent()
    {
      Assert.Equal("Xenia", FakeBirthRecord.MotherGivenNames[0]);
      Assert.Equal("Adkins", FakeBirthRecord.MotherFamilyName);
    }
    [Fact]
    public void BirthLocationPresent()
    {
      Assert.Equal("MA", FakeBirthRecord.BirthLocationJurisdiction);
      Assert.Equal("123 Fake Street", FakeBirthRecord.PlaceOfBirth["addressLine1"]);
      Assert.Equal("MA", FakeBirthRecord.PlaceOfBirth["addressState"]);
      Assert.Equal("01101", FakeBirthRecord.PlaceOfBirth["addressZip"]);
    }
    [Fact]
    public void PersonalIDsPresent()
    {
      Assert.Equal("7134703", FakeBirthRecord.InfantMedicalRecordNumber);
      Assert.Equal("2286144", FakeBirthRecord.MotherMedicalRecordNumber);
      Assert.Equal("133756482", FakeBirthRecord.MotherSocialSecurityNumber);
    }
    [Fact]
    public void BirthDataPresent()
    {
      Assert.Null(FakeBirthRecord.SetOrder);
      Assert.Null(FakeBirthRecord.Plurality);
      Assert.True(FakeBirthRecord.NoCongenitalAnomaliesOfTheNewborn);
      Assert.True(FakeBirthRecord.EpiduralOrSpinalAnesthesia );
      Assert.True(FakeBirthRecord.AugmentationOfLabor);
      Assert.True(FakeBirthRecord.NoSpecifiedAbnormalConditionsOfNewborn);
      Assert.True(FakeBirthRecord.NoInfectionsPresentDuringPregnancy);
      Assert.True(FakeBirthRecord.GestationalHypertension);
      Assert.Equal("700000006", FakeBirthRecord.FinalRouteAndMethodOfDelivery["code"]);
      Assert.True(FakeBirthRecord.NoObstetricProcedures);
      // some negative cases
      Assert.False(FakeBirthRecord.GestationalDiabetes);
      Assert.False(FakeBirthRecord.ArtificialInsemination);
    }

    [Fact]
    public void TestPropertiesWithHelpers()
    {
        // Test all properties that have helpers
        // TODO: Move some existing property tests here
        TestCodedPropertyWithHelper("MotherEducationLevel", VR.ValueSets.EducationLevel.Codes);
        TestCodedPropertyWithHelper("MotherEducationLevelEditFlag", VR.ValueSets.EditBypass01234.Codes);
        TestCodedPropertyWithHelper("FatherEducationLevel", VR.ValueSets.EducationLevel.Codes);
        TestCodedPropertyWithHelper("FatherEducationLevelEditFlag", VR.ValueSets.EditBypass01234.Codes);
    }

    // Given a property name that takes a code and has a helper property, and a list of codes to test,
    // test setting and getting all the valid codes
    private void TestCodedPropertyWithHelper(string propertyName, string[,] codes)
    {
        // Helper name is just an extension of the property name
        string helperName = propertyName + "Helper";
        BirthRecord record = new BirthRecord();
        // Default should be null
        Assert.Equal("", ((Dictionary<string, string>)record.GetType().GetProperty(propertyName).GetValue(record))["code"]);
        Assert.Null(record.GetType().GetProperty(helperName).GetValue(record));
        for (int i = 0; i < codes.GetLength(0); i++)
        {
            // Set to the code via the helper and make sure the get returns the same code for both the helper and the base
            // property along with the appropriate system and display values
            record.GetType().GetProperty(helperName).SetValue(record, codes[i, 0]);
            Assert.Equal(codes[i, 0], record.GetType().GetProperty(helperName).GetValue(record));
            Assert.Equal(codes[i, 0], ((Dictionary<string, string>)record.GetType().GetProperty(propertyName).GetValue(record))["code"]);
            Assert.Equal(codes[i, 1], ((Dictionary<string, string>)record.GetType().GetProperty(propertyName).GetValue(record))["display"]);
            Assert.Equal(codes[i, 2], ((Dictionary<string, string>)record.GetType().GetProperty(propertyName).GetValue(record))["system"]);
            // Reset it and then set the value via the base property coded value and make sure the correct values are present
            record.GetType().GetProperty(propertyName).SetValue(record, null);
            Assert.Null(record.GetType().GetProperty(helperName).GetValue(record));
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("code", codes[i, 0]);
            record.GetType().GetProperty(propertyName).SetValue(record, dict);
            Assert.Equal(codes[i, 0], record.GetType().GetProperty(helperName).GetValue(record));
            Assert.Equal(codes[i, 0], ((Dictionary<string, string>)record.GetType().GetProperty(propertyName).GetValue(record))["code"]);
        }
    }

    [Fact]
    public void SetAttendantAfterParse()
    {
        //name
        Assert.Equal("Avery Jones", FakeBirthRecord.AttendantName);
        FakeBirthRecord.AttendantName = "Janet Seito";
        Assert.Equal("Janet Seito", FakeBirthRecord.AttendantName);
        //NPI
        Assert.Equal("762310012345", FakeBirthRecord.AttendantNPI);
        FakeBirthRecord.AttendantNPI = "762310012000";
        Assert.Equal("762310012000", FakeBirthRecord.AttendantNPI);
        //title
        Assert.Equal("76231001", FakeBirthRecord.AttendantTitle["code"]);
        Assert.Equal(CodeSystems.SCT, FakeBirthRecord.AttendantTitle["system"]);
        Assert.Equal("Osteopath", FakeBirthRecord.AttendantTitle["display"]);
        Dictionary<string, string> AttendantTitle = new Dictionary<string, string>();
        AttendantTitle.Add("code", "112247003");
        AttendantTitle.Add("system", CodeSystems.SCT);
        AttendantTitle.Add("display", "Medical Doctor");
        FakeBirthRecord.AttendantTitle = AttendantTitle;
        //Other
        FakeBirthRecord.AttendantOtherHelper = "Nurse";
        Assert.Equal("OTH", FakeBirthRecord.AttendantTitle["code"]);
        Assert.Equal(CodeSystems.NullFlavor_HL7_V3, FakeBirthRecord.AttendantTitle["system"]);
        Assert.Equal("Other", FakeBirthRecord.AttendantTitle["display"]);
        Assert.Equal("Nurse", FakeBirthRecord.AttendantTitle["text"]);
        Assert.Equal("Nurse", FakeBirthRecord.AttendantOtherHelper);
    }

    [Fact]
    public void SetCertifierAfterParse()
    {
        //name
        Assert.Equal("Janet Seito", FakeBirthRecord.CertifierName);
        FakeBirthRecord.CertifierName = "Janet Seto";
        Assert.Equal("Janet Seto", FakeBirthRecord.CertifierName);
        //NPI
        Assert.Equal("223347044", FakeBirthRecord.CertifierNPI);
        FakeBirthRecord.CertifierNPI = "762310012000";
        Assert.Equal("762310012000", FakeBirthRecord.CertifierNPI);
        //title
        Assert.Equal("76231001", FakeBirthRecord.CertifierTitle["code"]);
        Assert.Equal(CodeSystems.SCT, FakeBirthRecord.CertifierTitle["system"]);
        Assert.Equal("Osteopath", FakeBirthRecord.CertifierTitle["display"]);
        Dictionary<string, string> CertifierTitle = new Dictionary<string, string>();
        CertifierTitle.Add("code", "112247003");
        CertifierTitle.Add("system", CodeSystems.SCT);
        CertifierTitle.Add("display", "Medical Doctor");
        FakeBirthRecord.CertifierTitle = CertifierTitle;
        //Other
        FakeBirthRecord.CertifierOtherHelper = "Nurse";
        Assert.Equal("OTH", FakeBirthRecord.CertifierTitle["code"]);
        Assert.Equal(CodeSystems.NullFlavor_HL7_V3, FakeBirthRecord.CertifierTitle["system"]);
        Assert.Equal("Other", FakeBirthRecord.CertifierTitle["display"]);
        Assert.Equal("Nurse", FakeBirthRecord.CertifierTitle["text"]);
        Assert.Equal("Nurse", FakeBirthRecord.CertifierOtherHelper);
    }
    
    [Fact]
    public void TestImportLocation()
    {
      BirthRecord br = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal("116441967701", br.FacilityNPI);
      Assert.Equal("UT12", br.FacilityJFI);
      Assert.Equal("South Hospital", br.BirthFacilityName);
      Assert.Equal("Transfer From Hospital", br.FacilityMotherTransferredFrom);
      Assert.Equal("Transfer To Hospital", br.FacilityInfantTransferredTo);
    }

    [Fact]
    public void TestSetLocation()
    {
      BirthRecord br = new()
      {
          FacilityNPI = "4815162342",
          FacilityJFI = "636",
          BirthFacilityName = "Lahey Hospital",
          FacilityMotherTransferredFrom = "Sunnyvale Medical",
          FacilityInfantTransferredTo = "LD Care"
      };
      Assert.Equal("4815162342", br.FacilityNPI);
      Assert.Equal("636", br.FacilityJFI);
      Assert.Equal("Lahey Hospital", br.BirthFacilityName);
      Assert.Equal("Sunnyvale Medical", br.FacilityMotherTransferredFrom);
      Assert.Equal("LD Care", br.FacilityInfantTransferredTo);
      br.FacilityNPI = "999";
      Assert.Equal("999", br.FacilityNPI);
      Assert.Equal("636", br.FacilityJFI);
      Assert.Equal("Lahey Hospital", br.BirthFacilityName);
      br.FacilityJFI = "0909";
      Assert.Equal("999", br.FacilityNPI);
      Assert.Equal("0909", br.FacilityJFI);
      Assert.Equal("Lahey Hospital", br.BirthFacilityName);
      br.BirthFacilityName = "Bob's Medical Center";
      Assert.Equal("999", br.FacilityNPI);
      Assert.Equal("0909", br.FacilityJFI);
      Assert.Equal("Bob's Medical Center", br.BirthFacilityName);
      br.FacilityMotherTransferredFrom = "Abignale Hospital";
      br.FacilityInfantTransferredTo = "Pittsfield Medical Facility";
      Assert.Equal("999", br.FacilityNPI);
      Assert.Equal("0909", br.FacilityJFI);
      Assert.Equal("Bob's Medical Center", br.BirthFacilityName);
      Assert.Equal("Abignale Hospital", br.FacilityMotherTransferredFrom);
      Assert.Equal("Pittsfield Medical Facility", br.FacilityInfantTransferredTo);
    }

    [Fact]
    public void SetLastMenstrualPeriod()
    {
      Assert.Null(SetterBirthRecord.LastMenstrualPeriod);
      Assert.Null(SetterBirthRecord.LastMenstrualPeriodYear);
      Assert.Null(SetterBirthRecord.LastMenstrualPeriodMonth);
      Assert.Null(SetterBirthRecord.LastMenstrualPeriodDay);
      SetterBirthRecord.LastMenstrualPeriod = "2023-02";
      Assert.Equal("2023-02", SetterBirthRecord.LastMenstrualPeriod);
      Assert.Equal(2023, SetterBirthRecord.LastMenstrualPeriodYear);
      Assert.Equal(2, SetterBirthRecord.LastMenstrualPeriodMonth);
      Assert.Null(SetterBirthRecord.LastMenstrualPeriodDay);
      SetterBirthRecord.LastMenstrualPeriod = null;
      Assert.Null(SetterBirthRecord.LastMenstrualPeriod);
      Assert.Null(SetterBirthRecord.LastMenstrualPeriodYear);
      Assert.Null(SetterBirthRecord.LastMenstrualPeriodMonth);
      Assert.Null(SetterBirthRecord.LastMenstrualPeriodDay);
      SetterBirthRecord.LastMenstrualPeriodMonth = 4;
      Assert.Null(SetterBirthRecord.LastMenstrualPeriod);
      Assert.Null(SetterBirthRecord.LastMenstrualPeriodYear);
      Assert.Equal(4, SetterBirthRecord.LastMenstrualPeriodMonth);
      Assert.Null(SetterBirthRecord.LastMenstrualPeriodDay);
      SetterBirthRecord.LastMenstrualPeriodYear = 2024;
      Assert.Equal(2024, SetterBirthRecord.LastMenstrualPeriodYear);
      Assert.Equal("2024-04", SetterBirthRecord.LastMenstrualPeriod);

    }

    [Fact]
    public void TestMotherHeightPropertiesSetter()
    {
        BirthRecord record = new BirthRecord();
        // Height
        Assert.Null(record.MotherHeight);
        record.MotherHeight = 67;
        Assert.Equal(67, record.MotherHeight);
        // Edit Flag
        Assert.Equal("", record.MotherHeightEditFlag["code"]);
        record.MotherHeightEditFlagHelper = VR.ValueSets.EditBypass01234.Edit_Passed;
        Assert.Equal(VR.ValueSets.EditBypass01234.Edit_Passed, record.MotherHeightEditFlag["code"]);
        // IJE translations
        IJENatality ije1 = new IJENatality(record);
        Assert.Equal("5", ije1.HFT);
        Assert.Equal("7", ije1.HIN);  
        Assert.Equal("0", ije1.HGT_BYPASS);
    }  

    [Fact]
    public void SetFirstPrenatalCareVisit()
    {
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisit);
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisitYear);
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisitMonth);
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisitDay);
      SetterBirthRecord.FirstPrenatalCareVisit = "2023-02";
      Assert.Equal("2023-02", SetterBirthRecord.FirstPrenatalCareVisit);
      Assert.Equal(2023, SetterBirthRecord.FirstPrenatalCareVisitYear);
      Assert.Equal(2, SetterBirthRecord.FirstPrenatalCareVisitMonth);
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisitDay);
      SetterBirthRecord.FirstPrenatalCareVisit = null;
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisit);
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisitYear);
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisitMonth);
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisitDay);
      SetterBirthRecord.FirstPrenatalCareVisitMonth = 4;
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisit);
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisitYear);
      Assert.Equal(4, SetterBirthRecord.FirstPrenatalCareVisitMonth);
      Assert.Null(SetterBirthRecord.FirstPrenatalCareVisitDay);
    }

    [Fact]
    public void SetRegistrationDate()
    {
      Assert.Null(SetterBirthRecord.RegistrationDate);
      Assert.Null(SetterBirthRecord.RegistrationDateYear);
      Assert.Null(SetterBirthRecord.RegistrationDateMonth);
      Assert.Null(SetterBirthRecord.RegistrationDateDay);
      SetterBirthRecord.RegistrationDate = "2023-02";
      Assert.Equal("2023-02", SetterBirthRecord.RegistrationDate);
      Assert.Equal(2023, SetterBirthRecord.RegistrationDateYear);
      Assert.Equal(2, SetterBirthRecord.RegistrationDateMonth);
      SetterBirthRecord.RegistrationDateYear = 2022;
      Assert.Equal("2022-02", SetterBirthRecord.RegistrationDate);
      Assert.Equal(2022, SetterBirthRecord.RegistrationDateYear);
      Assert.Equal(2, SetterBirthRecord.RegistrationDateMonth);
      SetterBirthRecord.RegistrationDateDay = 3;
      Assert.Equal("2022-02-03", SetterBirthRecord.RegistrationDate);
      Assert.Equal(2022, SetterBirthRecord.RegistrationDateYear);
      Assert.Equal(2, SetterBirthRecord.RegistrationDateMonth);
      Assert.Equal(3, SetterBirthRecord.RegistrationDateDay);
      SetterBirthRecord.RegistrationDate = null;
      Assert.Null(SetterBirthRecord.RegistrationDate);
      Assert.Null(SetterBirthRecord.RegistrationDateYear);
      Assert.Null(SetterBirthRecord.RegistrationDateMonth);
      Assert.Null(SetterBirthRecord.RegistrationDateDay);
      SetterBirthRecord.RegistrationDateMonth = 4;
      Assert.Null(SetterBirthRecord.RegistrationDate);
      Assert.Null(SetterBirthRecord.RegistrationDateYear);
      Assert.Equal(4, SetterBirthRecord.RegistrationDateMonth);
      Assert.Null(SetterBirthRecord.RegistrationDateDay);
    }

    [Fact]
    public void TestImportPayorFinancialClass()
    {
      // Test FHIR record import.
      BirthRecord firstRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      string firstDescription = firstRecord.ToDescription();
      // Test conversion via FromDescription.
      BirthRecord secondRecord = VitalRecord.FromDescription<BirthRecord>(firstDescription);

      Assert.Equal("5", firstRecord.PayorTypeFinancialClass["code"]);
      Assert.Equal(VR.CodeSystems.NAHDO, firstRecord.PayorTypeFinancialClass["system"]);
      Assert.Equal("PRIVATE HEALTH INSURANCE", firstRecord.PayorTypeFinancialClass["display"]);
      Assert.Equal(firstRecord.PayorTypeFinancialClass["code"], firstRecord.PayorTypeFinancialClassHelper);

      Assert.Equal("5", secondRecord.PayorTypeFinancialClass["code"]);
      Assert.Equal(VR.CodeSystems.NAHDO, secondRecord.PayorTypeFinancialClass["system"]);
      Assert.Equal("PRIVATE HEALTH INSURANCE", secondRecord.PayorTypeFinancialClass["display"]);
      Assert.Equal(secondRecord.PayorTypeFinancialClass["code"], secondRecord.PayorTypeFinancialClassHelper);
    }

    [Fact]
    public void TestSetPayorFinancialClass()
    {
      BirthRecord br = new()
      {
          PayorTypeFinancialClassHelper = "311"
      };

      Assert.Equal("311", br.PayorTypeFinancialClass["code"]);
      Assert.Equal(VR.CodeSystems.NAHDO, br.PayorTypeFinancialClass["system"]);
      Assert.Equal("TRICARE (CHAMPUS)", br.PayorTypeFinancialClass["display"]);
      Assert.Equal(br.PayorTypeFinancialClass["code"], br.PayorTypeFinancialClassHelper);

      Dictionary<string, string> payorType = new()
      {
          ["code"] = "2",
          ["system"] = VR.CodeSystems.NAHDO,
          ["display"] = "MEDICAID"
      };
      br.PayorTypeFinancialClass = payorType;
      Assert.Equal("2", br.PayorTypeFinancialClass["code"]);
      Assert.Equal(VR.CodeSystems.NAHDO, br.PayorTypeFinancialClass["system"]);
      Assert.Equal("MEDICAID", br.PayorTypeFinancialClass["display"]);
      Assert.Equal(br.PayorTypeFinancialClass["code"], br.PayorTypeFinancialClassHelper);

      br.PayorTypeFinancialClassHelper = "33";
      Assert.Equal("33", br.PayorTypeFinancialClass["code"]);
      Assert.Equal(VR.CodeSystems.NAHDO, br.PayorTypeFinancialClass["system"]);
      Assert.Equal("Indian Health Service or Tribe", br.PayorTypeFinancialClass["display"]);
      Assert.Equal(br.PayorTypeFinancialClass["code"], br.PayorTypeFinancialClassHelper);

      br.PayorTypeFinancialClassHelper = null;
      Assert.Equal("33", br.PayorTypeFinancialClass["code"]);
      Assert.Equal(VR.CodeSystems.NAHDO, br.PayorTypeFinancialClass["system"]);
      Assert.Equal("Indian Health Service or Tribe", br.PayorTypeFinancialClass["display"]);
      Assert.Equal(br.PayorTypeFinancialClass["code"], br.PayorTypeFinancialClassHelper);

      br.PayorTypeFinancialClassHelper = "xxx";
      Assert.Equal("9999", br.PayorTypeFinancialClass["code"]);
      Assert.Equal(VR.CodeSystems.NAHDO, br.PayorTypeFinancialClass["system"]);
      Assert.Equal("Unavailable / Unknown", br.PayorTypeFinancialClass["display"]);
      Assert.Equal("xxx", br.PayorTypeFinancialClass["text"]);
      Assert.Equal(br.PayorTypeFinancialClass["text"], br.PayorTypeFinancialClassHelper);

      br.PayorTypeFinancialClassHelper = "33";
      Assert.Equal("33", br.PayorTypeFinancialClass["code"]);
      Assert.Equal(VR.CodeSystems.NAHDO, br.PayorTypeFinancialClass["system"]);
      Assert.Equal("Indian Health Service or Tribe", br.PayorTypeFinancialClass["display"]);
      Assert.Equal(br.PayorTypeFinancialClass["code"], br.PayorTypeFinancialClassHelper);
    }
    
    [Fact]
    public void SetMaritalStatus()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.MaritalStatus = "Single";
      Assert.Equal("Single", birthRecord.MaritalStatus);

      IJENatality ije = new IJENatality();
      ije.MARITAL_DESCRIP = "Married";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal("Married", birthRecord2.MaritalStatus);

      BirthRecord birthRecord3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal("Divorced", birthRecord3.MaritalStatus);
    }

    [Fact]
    public void SetMotherMarriedDuringPregnancy()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.MotherMarriedDuringPregnancyHelper = "Y";
      Assert.Equal("Y", birthRecord.MotherMarriedDuringPregnancyHelper);
      Dictionary<string, string> cc = new Dictionary<string, string>();
      cc.Add("code", VR.ValueSets.YesNoUnknown.Codes[1, 0]);
      cc.Add("system", VR.ValueSets.YesNoUnknown.Codes[1, 2]);
      cc.Add("display", VR.ValueSets.YesNoUnknown.Codes[1, 1]);
      Assert.Equal(cc, birthRecord.MotherMarriedDuringPregnancy);

      IJENatality ije = new IJENatality();
      ije.MARN = "N";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal("N", birthRecord2.MotherMarriedDuringPregnancyHelper);

      BirthRecord birthRecord3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal("Y", birthRecord3.MotherMarriedDuringPregnancyHelper);
      Assert.Equal(cc, birthRecord3.MotherMarriedDuringPregnancy);
    }

    [Fact]
    public void SetPaternityAcknowledgementSigned()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.PaternityAcknowledgementSignedHelper = "Y";
      Assert.Equal("Y", birthRecord.PaternityAcknowledgementSignedHelper);
      Dictionary<string, string> cc = new Dictionary<string, string>();
      cc.Add("code", VR.ValueSets.YesNoNotApplicable.Codes[1, 0]);
      cc.Add("system", VR.ValueSets.YesNoNotApplicable.Codes[1, 2]);
      cc.Add("display", VR.ValueSets.YesNoNotApplicable.Codes[1, 1]);
      Assert.Equal(cc, birthRecord.PaternityAcknowledgementSigned);

      IJENatality ije = new IJENatality();
      ije.ACKN = "X";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal("NA", birthRecord2.PaternityAcknowledgementSignedHelper);

      BirthRecord birthRecord3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal("Y", birthRecord3.PaternityAcknowledgementSignedHelper);
      Assert.Equal(cc, birthRecord3.PaternityAcknowledgementSigned);
    }

    [Fact]
    public void SetMotherTransferred()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.MotherTransferredHelper = "Y";
      Assert.Equal("Y", birthRecord.MotherTransferredHelper);
      Dictionary<string, string> cc = new Dictionary<string, string>();
      cc.Add("code", "hosp-trans");
      cc.Add("system", "http://terminology.hl7.org/CodeSystem/admit-source");
      cc.Add("display", "Transferred from other hospital");
      cc.Add("text", "The Patient has been transferred from another hospital for this encounter.");
      Assert.Equal(cc, birthRecord.MotherTransferred);

      IJENatality ije = new IJENatality();
      ije.TRAN = "N";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal("N", birthRecord2.MotherTransferredHelper);

      ije.TRAN = "Y";
      BirthRecord birthRecord3 = ije.ToBirthRecord();
      Assert.Equal("Y", birthRecord3.MotherTransferredHelper);

      ije.TRAN = "U";
      BirthRecord birthRecord4 = ije.ToBirthRecord();
      Assert.Equal("U", birthRecord3.MotherTransferredHelper);
      Assert.Equal("UNKNOWN", birthRecord4.FacilityMotherTransferredFrom);

      BirthRecord birthRecord5 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal("Y", birthRecord5.MotherTransferredHelper);
      Assert.Equal(cc, birthRecord5.MotherTransferred);
    }

    [Fact]
    public void SetInfantLiving()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.InfantLiving = true;
      Assert.True(birthRecord.InfantLiving);

      IJENatality ije = new IJENatality();
      ije.ILIV = "N";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.False(birthRecord2.InfantLiving);

      BirthRecord birthRecord3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.True(birthRecord3.InfantLiving);
    }

    [Fact]
    public void SetInfantTransferred()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.InfantTransferredHelper = "Y";
      Assert.Equal("Y", birthRecord.InfantTransferredHelper);
      Dictionary<string, string> cc = new Dictionary<string, string>();
      cc.Add("code", "other-hcf");
      cc.Add("system", "http://terminology.hl7.org/CodeSystem/discharge-disposition");
      cc.Add("display", "Other healthcare facility");
      cc.Add("text", "The patient was transferred to another healthcare facility.");
      Assert.Equal(cc, birthRecord.InfantTransferred);

      IJENatality ije = new IJENatality();
      ije.ITRAN = "N";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal("N", birthRecord2.InfantTransferredHelper);

      IJENatality ije2 = new IJENatality();
      ije2.ITRAN = "U";
      BirthRecord birthRecord3 = ije2.ToBirthRecord();
      Assert.Equal("UNKNOWN", birthRecord3.FacilityInfantTransferredTo);
      Assert.Equal("U", birthRecord3.InfantTransferredHelper);


      BirthRecord birthRecord4 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal("Y", birthRecord4.InfantTransferredHelper);
      Assert.Equal(cc, birthRecord4.InfantTransferred);
    }

    [Fact]
    public void SetNumberLiveBorn()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.NumberLiveBorn = 2;
      Assert.Equal(2, birthRecord.NumberLiveBorn);

      IJENatality ije = new IJENatality();
      ije.LIVEB = "1";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal(1, birthRecord2.NumberLiveBorn);

      BirthRecord birthRecord3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal(3, birthRecord3.NumberLiveBorn);
    }

    [Fact]
    public void SetSSNRequested()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.SSNRequested = true;
      Assert.True(birthRecord.SSNRequested);

      IJENatality ije = new IJENatality();
      ije.SSN_REQ = "N";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.False(birthRecord2.SSNRequested);

      BirthRecord birthRecord3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.True(birthRecord3.SSNRequested);
    }
    
    [Fact]
    public void Test_EmergingIssues()
    {
        BirthRecord record1 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
        record1.EmergingIssue1_1 = "A";
        record1.EmergingIssue1_2 = "B";
        record1.EmergingIssue1_3 = "C";
        record1.EmergingIssue1_4 = "D";
        record1.EmergingIssue1_5 = "E";
        record1.EmergingIssue1_6 = "F";
        record1.EmergingIssue8_1 = "AAAAAAAA";
        record1.EmergingIssue8_2 = "BBBBBBBB";
        record1.EmergingIssue8_3 = "CCCCCCCC";
        record1.EmergingIssue20 = "AAAAAAAAAAAAAAAAAAAA";
        Assert.Equal("A", record1.EmergingIssue1_1);
        Assert.Equal("B", record1.EmergingIssue1_2);
        Assert.Equal("C", record1.EmergingIssue1_3);
        Assert.Equal("D", record1.EmergingIssue1_4);
        Assert.Equal("E", record1.EmergingIssue1_5);
        Assert.Equal("F", record1.EmergingIssue1_6);
        Assert.Equal("AAAAAAAA", record1.EmergingIssue8_1);
        Assert.Equal("BBBBBBBB", record1.EmergingIssue8_2);
        Assert.Equal("CCCCCCCC", record1.EmergingIssue8_3);
        Assert.Equal("AAAAAAAAAAAAAAAAAAAA", record1.EmergingIssue20);
        IJENatality ije = new IJENatality(record1, false); // Don't validate since we don't care about most fields
        Assert.Equal("A", ije.PLACE1_1);
        Assert.Equal("B", ije.PLACE1_2);
        Assert.Equal("C", ije.PLACE1_3);
        Assert.Equal("D", ije.PLACE1_4);
        Assert.Equal("E", ije.PLACE1_5);
        Assert.Equal("F", ije.PLACE1_6);
        Assert.Equal("AAAAAAAA", ije.PLACE8_1);
        Assert.Equal("BBBBBBBB", ije.PLACE8_2);
        Assert.Equal("CCCCCCCC", ije.PLACE8_3);
        Assert.Equal("AAAAAAAAAAAAAAAAAAAA", ije.PLACE20);
    }
    
    [Fact]
    public void ParseCertificationDate()
    { 
      BirthRecord firstRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal("2019-02-12T13:30:00-07:00", firstRecord.CertificationDate);
      Assert.Equal(2019, firstRecord.CertifiedYear);
      Assert.Equal(2, firstRecord.CertifiedMonth);
      Assert.Equal(12, firstRecord.CertifiedDay);
    }

    [Fact]
    public void SetCertificationDate()
    {
      Assert.Null(SetterBirthRecord.CertificationDate);
      Assert.Null(SetterBirthRecord.CertifiedYear);
      Assert.Null(SetterBirthRecord.CertifiedMonth);
      Assert.Null(SetterBirthRecord.CertifiedDay);
      SetterBirthRecord.CertificationDate = "2023-02";
      Assert.Equal("2023-02", SetterBirthRecord.CertificationDate);
      Assert.Equal(2023, SetterBirthRecord.CertifiedYear);
      Assert.Equal(2, SetterBirthRecord.CertifiedMonth);
      SetterBirthRecord.CertifiedYear = 2022;
      Assert.Equal("2022-02", SetterBirthRecord.CertificationDate);
      Assert.Equal(2022, SetterBirthRecord.CertifiedYear);
      Assert.Equal(2, SetterBirthRecord.CertifiedMonth);
      SetterBirthRecord.CertifiedDay = 3;
      Assert.Equal("2022-02-03", SetterBirthRecord.CertificationDate);
      Assert.Equal(2022, SetterBirthRecord.CertifiedYear);
      Assert.Equal(2, SetterBirthRecord.CertifiedMonth);
      Assert.Equal(3, SetterBirthRecord.CertifiedDay);
      SetterBirthRecord.CertificationDate = null;
      Assert.Null(SetterBirthRecord.RegistrationDate);
      Assert.Null(SetterBirthRecord.CertifiedYear);
      Assert.Null(SetterBirthRecord.CertifiedMonth);
      Assert.Null(SetterBirthRecord.CertifiedDay);
      SetterBirthRecord.CertifiedMonth = 4;
      Assert.Null(SetterBirthRecord.CertificationDate);
      Assert.Null(SetterBirthRecord.CertifiedYear);
      Assert.Equal(4, SetterBirthRecord.CertifiedMonth);
      Assert.Null(SetterBirthRecord.CertifiedDay);
      // test IJE translations
      SetterBirthRecord.CertificationDate = "2023-02-19";
      IJENatality ije1 = new IJENatality(SetterBirthRecord);
      Assert.Equal("2023", ije1.CERTIFIED_YR);
      Assert.Equal("02", ije1.CERTIFIED_MO);
      Assert.Equal("19", ije1.CERTIFIED_DY);
      BirthRecord br2 = ije1.ToRecord();
      Assert.Equal("2023-02-19", br2.CertificationDate);
      Assert.Equal(2023, (int)br2.CertifiedYear);
      Assert.Equal(02, (int)br2.CertifiedMonth);
      Assert.Equal(19, (int)br2.CertifiedDay);
    }

    [Fact]  
    public void SetPartialDateOfLastLiveBirthFields()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.DateOfLastLiveBirthMonth = 5;
      birthRecord.DateOfLastLiveBirthYear = 2023;
      IJENatality ije = new(birthRecord);
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal(5, birthRecord2.DateOfLastLiveBirthMonth);
      Assert.Equal(2023, birthRecord2.DateOfLastLiveBirthYear);
      Assert.Equal("2023-05", birthRecord2.DateOfLastLiveBirth);

      //birthRecord2.DateOfLastLiveBirthMonth = null;
      birthRecord2.DateOfLastLiveBirthMonth = -1;
      Assert.Equal("2023", birthRecord2.DateOfLastLiveBirth);
      birthRecord2.DateOfLastLiveBirthDay = 15;
      Assert.Equal("2023", birthRecord2.DateOfLastLiveBirth);
      Assert.Equal(15, birthRecord2.DateOfLastLiveBirthDay);
      birthRecord2.DateOfLastLiveBirthMonth = 2;
      Assert.Equal("2023-02-15", birthRecord2.DateOfLastLiveBirth);

      BirthRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal(2014, parsedRecord.DateOfLastLiveBirthYear);
      Assert.Equal(11, parsedRecord.DateOfLastLiveBirthMonth);
      Assert.Equal(20, parsedRecord.DateOfLastLiveBirthDay);
      Assert.Equal("2014-11-20", parsedRecord.DateOfLastLiveBirth);

    }

    [Fact]
    public void SetUnknownDateOfLastLiveBirthFields()
    {
      IJENatality ije = new IJENatality();
      ije.MLLB = "99";
      ije.YLLB = "9999";
      BirthRecord birthRecord = ije.ToBirthRecord();
      Assert.Equal(-1, birthRecord.DateOfLastLiveBirthMonth);
      Assert.Equal(-1, birthRecord.DateOfLastLiveBirthYear);
    }

    [Fact]
    public void SetFullDateOfLastLiveBirthFields()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.DateOfLastLiveBirth = "2020-01-01";
      IJENatality ije = new(birthRecord);
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal("2020-01-01", birthRecord2.DateOfLastLiveBirth);
    }

    [Fact]
    public void SetPartialDateOfLastOtherPregnancyOutcome()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.DateOfLastOtherPregnancyOutcomeMonth = 11;
      birthRecord.DateOfLastOtherPregnancyOutcomeYear = 2022;
      IJENatality ije = new(birthRecord);
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal(11, birthRecord.DateOfLastOtherPregnancyOutcomeMonth);
      Assert.Equal(2022, birthRecord.DateOfLastOtherPregnancyOutcomeYear);
      // // TODO check roundtrip from IJE
      Assert.Equal(11, birthRecord2.DateOfLastOtherPregnancyOutcomeMonth);
      Assert.Equal(2022, birthRecord2.DateOfLastOtherPregnancyOutcomeYear);
      Assert.Equal("2022-11", birthRecord2.DateOfLastOtherPregnancyOutcome);
      birthRecord2.DateOfLastOtherPregnancyOutcomeMonth = -1;
      Assert.Equal("2022", birthRecord2.DateOfLastOtherPregnancyOutcome);
      birthRecord2.DateOfLastOtherPregnancyOutcomeDay = 24;
      Assert.Equal("2022", birthRecord2.DateOfLastOtherPregnancyOutcome);
      Assert.Equal(24, birthRecord2.DateOfLastOtherPregnancyOutcomeDay);
      birthRecord2.DateOfLastOtherPregnancyOutcomeMonth = 4;
      Assert.Equal("2022-04-24", birthRecord2.DateOfLastOtherPregnancyOutcome);

      BirthRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal(2015, parsedRecord.DateOfLastOtherPregnancyOutcomeYear);
      Assert.Equal(5, parsedRecord.DateOfLastOtherPregnancyOutcomeMonth);
      Assert.Equal(10, parsedRecord.DateOfLastOtherPregnancyOutcomeDay);
      Assert.Equal("2015-05-10", parsedRecord.DateOfLastOtherPregnancyOutcome);
    }

    [Fact]
    public void SetUnknownDateOfLastOtherPregnancyOutcome()
    {
      IJENatality ije = new IJENatality();
      ije.MOPO = "99";
      ije.YOPO = "9999";
      BirthRecord birthRecord = ije.ToBirthRecord();
      Assert.Equal(-1, birthRecord.DateOfLastOtherPregnancyOutcomeMonth);
      Assert.Equal(-1, birthRecord.DateOfLastOtherPregnancyOutcomeYear);
    }

    [Fact]
    public void SetFullDateOfLastOtherPregnancyOutcome()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.DateOfLastOtherPregnancyOutcome = "2022-10-09";
      IJENatality ije = new(birthRecord);
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal("2022-10-09", birthRecord2.DateOfLastOtherPregnancyOutcome);
    }

    [Fact]
    public void SetNumberOfPrenatalVisits()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.NumberOfPrenatalVisits = 5;
      IJENatality ije = new(birthRecord);
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal(5, birthRecord2.NumberOfPrenatalVisits);

      BirthRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal(8, parsedRecord.NumberOfPrenatalVisits);
    }

    [Fact]
    public void SetNumberOfPrenatalVisitsEditBypass()
    {
      IJENatality ije = new IJENatality();
      ije.NPREV_BYPASS = "1";
      BirthRecord birthRecord = ije.ToBirthRecord();
      Dictionary<string, string> editBypass = new Dictionary<string, string>();
      editBypass.Add("code", "1");
      editBypass.Add("system", VR.CodeSystems.VRCLEditFlags);
      editBypass.Add("display", "Edit Failed, Data Queried, and Verified");
      Assert.Equal(editBypass, birthRecord.NumberOfPrenatalVisitsEditFlag);

      BirthRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Dictionary<string, string> editBypass0 = new Dictionary<string, string>();
      editBypass0.Add("code", "0");
      editBypass0.Add("system", VR.CodeSystems.VRCLEditFlags);
      editBypass0.Add("display", "Edit Passed");
      Assert.Equal(editBypass0, parsedRecord.NumberOfPrenatalVisitsEditFlag);
    }

    [Fact]
    public void SetObstetricEstimateOfGestation()
    {
      BirthRecord birthRecord1 = new BirthRecord();
      Dictionary<string, string> dict = new Dictionary<string, string>();
      dict.Add("value", "10");
      dict.Add("code", "wk");
      birthRecord1.GestationalAgeAtDelivery = dict;
      Assert.Equal(dict["value"], birthRecord1.GestationalAgeAtDelivery["value"]);
      Assert.Equal("wk", birthRecord1.GestationalAgeAtDelivery["code"]);
      Assert.Equal("http://unitsofmeasure.org", birthRecord1.GestationalAgeAtDelivery["system"]);

      IJENatality ije = new IJENatality();
      ije.OWGEST = "38";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal("38", birthRecord2.GestationalAgeAtDelivery["value"]);
      Assert.Equal("wk", birthRecord2.GestationalAgeAtDelivery["code"]);
      Assert.Equal("http://unitsofmeasure.org", birthRecord2.GestationalAgeAtDelivery["system"]);
      
      BirthRecord birthRecord3 = new BirthRecord();
      Dictionary<string, string> dict2 = new Dictionary<string, string>();
      dict2.Add("value", "48");
      dict2.Add("code", "d");
      birthRecord3.GestationalAgeAtDelivery = dict2;
      Assert.Equal(dict2["value"], birthRecord3.GestationalAgeAtDelivery["value"]);
      Assert.Equal("d", birthRecord3.GestationalAgeAtDelivery["code"]);
      Assert.Equal("http://unitsofmeasure.org", birthRecord3.GestationalAgeAtDelivery["system"]);
      // IJE should divide days by 7 and round down
      IJENatality ije2 = new(birthRecord3);
      ije2.OWGEST = "06";

      BirthRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal("36", parsedRecord.GestationalAgeAtDelivery["value"]);
      Assert.Equal("wk", parsedRecord.GestationalAgeAtDelivery["code"]);
      Assert.Equal("http://unitsofmeasure.org", parsedRecord.GestationalAgeAtDelivery["system"]);
    }

    [Fact]
    public void SetGestationalAgeAtDeliveryEditFlag()
    {
      IJENatality ije = new IJENatality();
      ije.OWGEST_BYPASS = "0";
      BirthRecord birthRecord = ije.ToBirthRecord();
      Dictionary<string, string> editBypass = new Dictionary<string, string>();
      editBypass.Add("code", "0off");
      editBypass.Add("system", VR.CodeSystems.VRCLEditFlags);
      editBypass.Add("display", "Off");
      Assert.Equal(editBypass, birthRecord.GestationalAgeAtDeliveryEditFlag);
    }

    [Fact]
    public void SetNumberOfBirthsNowDead()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.NumberOfBirthsNowDead = 2;
      IJENatality ije = new(birthRecord);
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal(2, birthRecord2.NumberOfBirthsNowDead);

      BirthRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal(1, parsedRecord.NumberOfBirthsNowDead);
    }

    [Fact]
    public void SetNumberOfBirthsNowLiving()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.NumberOfBirthsNowLiving = 3;
      IJENatality ije = new(birthRecord);
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal(3, birthRecord2.NumberOfBirthsNowLiving);

      BirthRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal(2, parsedRecord.NumberOfBirthsNowLiving);
    }

    [Fact]
    public void SetNumberOfOtherPregnancyOutcomes()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.NumberOfOtherPregnancyOutcomes = 1;
      IJENatality ije = new(birthRecord);
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal(1, birthRecord2.NumberOfOtherPregnancyOutcomes);

      BirthRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal(3, parsedRecord.NumberOfOtherPregnancyOutcomes);
    }

    [Fact]
    public void SetMotherReceivedWICFood()
    {
      IJENatality ije = new IJENatality();
      ije.WIC = "Y";
      BirthRecord birthRecord = ije.ToBirthRecord();
      Assert.Equal("Y", birthRecord.MotherReceivedWICFoodHelper);

      BirthRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal("N", parsedRecord.MotherReceivedWICFoodHelper);
    }

    [Fact]
    public void SetInfantBreastfedAtDischarge()
    {
      IJENatality ije = new IJENatality();
      ije.BFED = "Y";
      BirthRecord birthRecord = ije.ToBirthRecord();
      Assert.True(birthRecord.InfantBreastfedAtDischarge);

      BirthRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.True(parsedRecord.InfantBreastfedAtDischarge);
    }

    [Fact]
    public void SetFetalPresentation()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.FetalPresentationHelper = "6096002";
      Dictionary<string, string> cc = new Dictionary<string, string>();
      cc.Add("code", "6096002");
      cc.Add("system", "http://snomed.info/sct");
      cc.Add("display", "Breech presentation (finding)");
      Assert.Equal(cc, birthRecord.FetalPresentation);

      IJENatality ije = new IJENatality();
      ije.PRES = "1";
      Dictionary<string, string> cc2 = new Dictionary<string, string>();
      cc2.Add("code", "70028003");
      cc2.Add("system", "http://snomed.info/sct");
      cc2.Add("display", "Vertex presentation (finding)");
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal(cc2, birthRecord2.FetalPresentation);
      Assert.Equal("70028003", birthRecord2.FetalPresentationHelper);

      BirthRecord birthRecord3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal(cc, birthRecord3.FetalPresentation);
      Assert.Equal("6096002", birthRecord3.FetalPresentationHelper);
    }

    [Fact]
    public void SetLaborTrialAttempted()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.LaborTrialAttempted = true;
      Assert.True(birthRecord.LaborTrialAttempted);

      IJENatality ije = new IJENatality();
      ije.TLAB = "Y";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.True(birthRecord2.LaborTrialAttempted);

      BirthRecord birthRecord3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.True(birthRecord3.LaborTrialAttempted);
    }

    [Fact]
    public void SetNumberOfPreviousCesareans()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.NumberOfPreviousCesareans = 2;
      Assert.Equal(2, birthRecord.NumberOfPreviousCesareans);

      IJENatality ije = new IJENatality();
      ije.NPCES = "1";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal(1, birthRecord2.NumberOfPreviousCesareans);

      BirthRecord birthRecord3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Assert.Equal(1, birthRecord3.NumberOfPreviousCesareans);
    }
    
    [Fact]
    public void SetNumberOfPreviousCesareansEditFlag()
    {
      BirthRecord birthRecord = new BirthRecord();
      birthRecord.NumberOfPreviousCesareansEditFlagHelper = "1failedVerified";
      Assert.Equal("1failedVerified", birthRecord.NumberOfPreviousCesareansEditFlagHelper);
      Dictionary<string, string> cc = new Dictionary<string, string>();
      cc.Add("code", "1failedVerified");
      cc.Add("system", "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-edit-flags");
      cc.Add("display", "Edit Failed, Verified");
      Assert.Equal(cc, birthRecord.NumberOfPreviousCesareansEditFlag);

      IJENatality ije = new IJENatality();
      ije.NPCES_BYPASS = "0";
      BirthRecord birthRecord2 = ije.ToBirthRecord();
      Assert.Equal("0", birthRecord2.NumberOfPreviousCesareansEditFlagHelper);

      BirthRecord birthRecord3 = new BirthRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicBirthRecord.json")));
      Dictionary<string, string> cc2 = new Dictionary<string, string>();
      cc2.Add("code", "0");
      cc2.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");
      cc2.Add("display", "Edit Passed");
      Assert.Equal("0", birthRecord3.NumberOfPreviousCesareansEditFlagHelper);
      Assert.Equal(cc2, birthRecord3.NumberOfPreviousCesareansEditFlag);
    }

  }
 
}

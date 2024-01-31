using System;
using System.IO;
using System.Collections.Generic;
using VR;
using Xunit;

namespace BFDR.Tests
{
  public class NatalityData_Should
  {
    [Fact]
    public void TestPatientChildVitalRecordProperties()
    {
      // Test IJE import.
      IJENatality ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ijeImported.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJENatality ijeConverted = new(br);

      // Certificate Number
      Assert.Equal("048858", ijeImported.FILENO);
      Assert.Equal(ijeImported.FILENO, ijeConverted.FILENO);
      // Date of Birth (Infant)--Year
      Assert.Equal("2023", ijeImported.IDOB_YR);
      Assert.Equal(ijeImported.IDOB_YR, ijeConverted.IDOB_YR);
      Assert.Equal(2023, br.BirthYear);
      // State, U.S. Territory or Canadian Province of Birth (Infant) - code
      Assert.Equal("MA".PadRight(2), ijeImported.BSTATE);
      Assert.Equal(ijeImported.BSTATE, ijeConverted.BSTATE);
      Assert.Equal("MA", br.PlaceOfBirth["addressState"]);
      Assert.Equal("MA", br.BirthLocationJurisdiction); // TODO - Birth Location Jurisdiction still needs to be finalized.
      // Time of Birth
      Assert.Equal("1230".PadRight(4), ijeImported.TB);
      Assert.Equal(ijeImported.TB, ijeConverted.TB);
      Assert.Equal("12:30:00", br.BirthTime);
      // Sex
      Assert.Equal("M", ijeImported.ISEX);
      Assert.Equal(ijeImported.ISEX, ijeConverted.ISEX);
      Assert.Equal("M", br.BirthSex["code"]);
      // Date of Birth (Infant)--Month
      Assert.Equal("11", ijeImported.IDOB_MO);
      Assert.Equal(ijeImported.IDOB_MO, ijeConverted.IDOB_MO);
      Assert.Equal(11, br.BirthMonth);
      // Date of Birth (Infant)--Day
      Assert.Equal("25", ijeImported.IDOB_DY);
      Assert.Equal(ijeImported.IDOB_DY, ijeConverted.IDOB_DY);
      Assert.Equal(25, br.BirthDay);
      Assert.Equal(ijeImported.IDOB_YR + "-" + ijeImported.IDOB_MO + "-" + ijeImported.IDOB_DY, br.DateOfBirth);
      Assert.Equal("2023-11-25", br.DateOfBirth);
      // County of Birth | (CountyCodes) (CNTYO)
      // TODO --- 
      // Plurality
      // TODO --- 
      // Set Order
      // TODO ---
      Assert.Equal("06", ijeImported.SORD);
      Assert.Equal(ijeImported.SORD, ijeConverted.SORD);
      Assert.Equal(6, br.SetOrder);
      // Plurality--Edit Flag
      // TODO --- 
      // Mother's Reported Age
      // TODO --- 
      // Father's Reported Age
      // TODO --- 
      // Child's First Name
      Assert.Equal("TestFirst".PadRight(50), ijeImported.KIDFNAME);
      Assert.Equal(ijeImported.KIDFNAME, ijeConverted.KIDFNAME);
      Assert.Equal("TestFirst", br.ChildGivenNames[0]);
      // Child's Middle Name
      Assert.Equal("TestMiddle".PadRight(50), ijeImported.KIDMNAME);
      Assert.Equal(ijeImported.KIDMNAME, ijeConverted.KIDMNAME);
      Assert.Equal("TestMiddle", br.ChildGivenNames[1]);
      // Child's Last Name
      Assert.Equal("TestLast".PadRight(50), ijeImported.KIDLNAME);
      Assert.Equal(ijeImported.KIDLNAME, ijeConverted.KIDLNAME);
      Assert.Equal("TestLast", br.ChildFamilyName);
      // Child's Surname Suffix (moved from end)
      Assert.Equal("Jr.".PadRight(7), ijeImported.KIDSUFFX);
      Assert.Equal(ijeImported.KIDSUFFX, ijeConverted.KIDSUFFX);
      Assert.Equal("Jr.", br.ChildSuffix);
      // County of Birth (Literal) | (BIRTH_CO)
      Assert.Equal("TestCounty".PadRight(25), ijeImported.BIRTH_CO);
      Assert.Equal(ijeImported.BIRTH_CO, ijeConverted.BIRTH_CO);
      Assert.Equal("TestCounty", br.PlaceOfBirth["addressCounty"]);
      // City/town/place of birth (Literal)
      Assert.Equal("TestCity".PadRight(50), ijeImported.BRTHCITY);
      Assert.Equal(ijeImported.BRTHCITY, ijeConverted.BRTHCITY);
      Assert.Equal("TestCity", br.PlaceOfBirth["addressCity"]);
      // Infant's Medical Record Number
      Assert.Equal("aaabbbcccdddeee".PadRight(15), ijeImported.INF_MED_REC_NUM);
      Assert.Equal(ijeImported.INF_MED_REC_NUM, ijeConverted.INF_MED_REC_NUM);
      Assert.Equal("aaabbbcccdddeee", br.InfantMedicalRecordNumber);
    }

    [Fact]
    public void TestBirthSexSetters()
    {
      IJENatality ije = new IJENatality();
      ije.ISEX = "M";
      Assert.Equal("M", ije.ISEX);
      Assert.Equal("M", ije.ToRecord().BirthSex["code"]);
      Assert.Equal("M", ije.ToRecord().BirthSexHelper);
      ije.ISEX = "F";
      Assert.Equal("F", ije.ISEX);
      Assert.Equal("F", ije.ToRecord().BirthSex["code"]);
      Assert.Equal("F", ije.ToRecord().BirthSexHelper);
      ije.ToRecord().BirthSexHelper = "M";
      Assert.Equal("M", ije.ISEX);
      Assert.Equal("M", ije.ToRecord().BirthSex["code"]);
      Assert.Equal("M", ije.ToRecord().BirthSexHelper);
    }

    // Test Patient Mother Vital Properties
    [Fact]
    public void TestPatientMotherVitalRecordProperties()
    {
      // Test IJE import.
      IJENatality ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ijeImported.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJENatality ijeConverted = new(br);

      // Date of Birth (Mother)--Year
      Assert.Equal("1994", ijeImported.MDOB_YR);
      Assert.Equal(ijeImported.MDOB_YR, ijeConverted.MDOB_YR);
      Assert.Equal(1994, br.MotherBirthYear);
      // Date of Birth (Mother)--Month
      Assert.Equal("11", ijeImported.MDOB_MO);
      Assert.Equal(ijeImported.MDOB_MO, ijeConverted.MDOB_MO);
      Assert.Equal(11, br.MotherBirthMonth);
      // Date of Birth (Mother)--Day
      Assert.Equal("29", ijeImported.MDOB_DY);
      Assert.Equal(ijeImported.MDOB_DY, ijeConverted.MDOB_DY);
      Assert.Equal(29, br.MotherBirthDay);
      Assert.Equal(ijeImported.MDOB_YR + "-" + ijeImported.MDOB_MO + "-" + ijeImported.MDOB_DY, br.MotherDateOfBirth);
      Assert.Equal("1994-11-29", br.MotherDateOfBirth);
    }

    // Test Related Person Father Properties
    [Fact]
    public void TestRelatedPersonFatherProperties()
    {
      // Test IJE import.
      IJENatality ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ijeImported.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJENatality ijeConverted = new(br);

      // Date of Birth (Father)--Year
      Assert.Equal("1992", ijeImported.FDOB_YR);
      Assert.Equal(ijeImported.FDOB_YR, ijeConverted.FDOB_YR);
      Assert.Equal(1992, br.FatherBirthYear);
      // Date of Birth (Father)--Month
      Assert.Equal("10", ijeImported.FDOB_MO);
      Assert.Equal(ijeImported.FDOB_MO, ijeConverted.FDOB_MO);
      Assert.Equal(10, br.FatherBirthMonth);
      // Date of Birth (Father)--Day
      Assert.Equal("05", ijeImported.FDOB_DY);
      Assert.Equal(ijeImported.FDOB_DY, ijeConverted.FDOB_DY);
      Assert.Equal(5, br.FatherBirthDay);
      Assert.Equal(ijeImported.FDOB_YR + "-" + ijeImported.FDOB_MO + "-" + ijeImported.FDOB_DY, br.FatherDateOfBirth);
      Assert.Equal("1992-10-05", br.FatherDateOfBirth);
    }

    // Test unknown date values for Mother and Father.
    [Fact]
    public void TestParentBirthDateUnknowns()
    {
      // Patient Mother.
      IJENatality ije = new();
      ije.MDOB_YR = "2000";
      ije.MDOB_MO = "09";
      ije.MDOB_DY = "27";
      Assert.Equal("2000", ije.MDOB_YR);
      Assert.Equal("09", ije.MDOB_MO);
      Assert.Equal("27", ije.MDOB_DY);
      ije.MDOB_DY = "99";
      Assert.Equal("99", ije.MDOB_DY);
      BirthRecord converted = ije.ToRecord();
      Assert.Equal(-1, converted.MotherBirthDay);
      Assert.Equal("2000-09", converted.MotherDateOfBirth);
      ije.MDOB_DY = "30";
      converted = ije.ToRecord();
      Assert.Equal(30, converted.MotherBirthDay);
      Assert.Equal("2000-09-30", converted.MotherDateOfBirth);
      ije.MDOB_MO = "99";
      converted = ije.ToRecord();
      Assert.Equal(-1, converted.MotherBirthMonth);
      Assert.Equal(30, converted.MotherBirthDay);
      Assert.Equal("2000", converted.MotherDateOfBirth);
      ije.MDOB_MO = "05";
      converted = ije.ToRecord();
      Assert.Equal(5, converted.MotherBirthMonth);
      Assert.Equal(30, converted.MotherBirthDay);
      Assert.Equal("2000-05-30", converted.MotherDateOfBirth);
      ije.MDOB_YR = "9999";
      Assert.Equal(-1, converted.MotherBirthYear);
      Assert.Equal(5, converted.MotherBirthMonth);
      Assert.Equal(30, converted.MotherBirthDay);
      Assert.Null(converted.MotherDateOfBirth);
      // Related Person Father.
      ije.FDOB_YR = "1999";
      ije.FDOB_MO = "11";
      ije.FDOB_DY = "27";
      Assert.Equal("1999", ije.FDOB_YR);
      Assert.Equal("11", ije.FDOB_MO);
      Assert.Equal("27", ije.FDOB_DY);
      ije.FDOB_DY = "99";
      Assert.Equal("99", ije.FDOB_DY);
      converted = ije.ToRecord();
      Assert.Equal(-1, converted.FatherBirthDay);
      Assert.Equal("1999-11", converted.FatherDateOfBirth);
      ije.FDOB_DY = "22";
      converted = ije.ToRecord();
      Assert.Equal(22, converted.FatherBirthDay);
      Assert.Equal("1999-11-22", converted.FatherDateOfBirth);
      ije.FDOB_MO = "99";
      converted = ije.ToRecord();
      Assert.Equal(-1, converted.FatherBirthMonth);
      Assert.Equal(22, converted.FatherBirthDay);
      Assert.Equal("1999", converted.FatherDateOfBirth);
      ije.FDOB_MO = "05";
      converted = ije.ToRecord();
      Assert.Equal(5, converted.FatherBirthMonth);
      Assert.Equal(22, converted.FatherBirthDay);
      Assert.Equal("1999-05-22", converted.FatherDateOfBirth);
      ije.FDOB_YR = "9999";
      Assert.Equal(-1, converted.FatherBirthYear);
      Assert.Equal(5, converted.FatherBirthMonth);
      Assert.Equal(22, converted.FatherBirthDay);
      Assert.Null(converted.FatherDateOfBirth);
    }

    // Test Unknown Parent Birthdate imports from IJE.
    [Fact]
    public void TestUnknownParentBirthdateImports()
    {
      // Test IJE import.
      IJENatality ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/UnknownParentBirthDates.ije")), true);
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ijeImported.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJENatality ijeConverted = new(br);

      // Date of Birth (Mother)--Year
      Assert.Equal("9999", ijeImported.MDOB_YR);
      Assert.Equal(ijeImported.MDOB_YR, ijeConverted.MDOB_YR);
      Assert.Equal(-1, br.MotherBirthYear);
      // Date of Birth (Mother)--Month
      Assert.Equal("99", ijeImported.MDOB_MO);
      Assert.Equal(ijeImported.MDOB_MO, ijeConverted.MDOB_MO);
      Assert.Equal(-1, br.MotherBirthMonth);
      // Date of Birth (Mother)--Day
      Assert.Equal("99", ijeImported.MDOB_DY);
      Assert.Equal(ijeImported.MDOB_DY, ijeConverted.MDOB_DY);
      Assert.Equal(-1, br.MotherBirthDay);
      Assert.Null(br.MotherDateOfBirth);
      // Date of Birth (Father)--Year
      Assert.Equal("9999", ijeImported.FDOB_YR);
      Assert.Equal(ijeImported.FDOB_YR, ijeConverted.FDOB_YR);
      Assert.Equal(-1, br.FatherBirthYear);
      // Date of Birth (Father)--Month
      Assert.Equal("99", ijeImported.FDOB_MO);
      Assert.Equal(ijeImported.FDOB_MO, ijeConverted.FDOB_MO);
      Assert.Equal(-1, br.FatherBirthMonth);
      // Date of Birth (Father)--Day
      Assert.Equal("99", ijeImported.FDOB_DY);
      Assert.Equal(ijeImported.FDOB_DY, ijeConverted.FDOB_DY);
      Assert.Equal(-1, br.FatherBirthDay);
      Assert.Null(br.FatherDateOfBirth);
    }

    [Fact]
    public void TestPlurality()
    {
      BirthRecord fhir = new BirthRecord();
      IJENatality ije = new IJENatality(fhir);
      Assert.Equal("  ", ije.SORD);
      Assert.Equal("  ", ije.PLUR);
      Assert.Equal(" ", ije.PLUR_BYPASS);
      ije.SORD = "02";
      ije.PLUR = "03";
      ije.PLUR_BYPASS = "1";
      Assert.Equal("02", ije.SORD);
      Assert.Equal("03", ije.PLUR);
      Assert.Equal("1", ije.PLUR_BYPASS);
      ije.PLUR = "99";
      Assert.Equal("99", ije.PLUR);
      ije.PLUR = "  ";
      Assert.Equal("  ", ije.PLUR);
    }
    
    [Fact]
    public void TestCongenitalAnomaliesOfTheNewborn()
    {
      BirthRecord fhir = new BirthRecord();
      IJENatality ije = new IJENatality(fhir);
      Assert.False(fhir.Anencephaly);
      Assert.False(fhir.NoCongenitalAnomaliesOfTheNewborn);
      Assert.Equal("U", ije.ANEN);
      ije.ANEN = "Y";
      Assert.Equal("Y", ije.ANEN);
      Assert.True(fhir.Anencephaly);
      Assert.False(fhir.NoCongenitalAnomaliesOfTheNewborn);
      ije.ANEN = "U";
      Assert.Equal("U", ije.ANEN);
      Assert.False(fhir.Anencephaly);
      Assert.False(fhir.NoCongenitalAnomaliesOfTheNewborn);
      ije.ANEN = "N";
      Assert.Equal("N", ije.ANEN);
      Assert.False(fhir.Anencephaly);
      Assert.True(fhir.NoCongenitalAnomaliesOfTheNewborn);
      ije.ANEN = "U";
      // FHIR uses false for either U or N, the two values are differentiated by the value of the corresponding none-of-the-above field
      // Setting an existing false attribute to false doesn't change the corresponding none-of-the-above field so we get N instead of U
      Assert.Equal("N", ije.ANEN);
      Assert.False(fhir.Anencephaly);
      Assert.True(fhir.NoCongenitalAnomaliesOfTheNewborn);
      ije.ANEN = "Y";
      Assert.Equal("Y", ije.ANEN);
      Assert.True(fhir.Anencephaly);
      Assert.False(fhir.NoCongenitalAnomaliesOfTheNewborn);
      ije.HYPO = "N"; // Setting any field to N sets all fields in the same group to N
      Assert.Equal("N", ije.HYPO);
      Assert.Equal("N", ije.ANEN);
      Assert.False(fhir.Hypospadias);
      Assert.False(fhir.Anencephaly);
      Assert.True(fhir.NoCongenitalAnomaliesOfTheNewborn);
      ije.HYPO = "Y"; // Setting any field to Y sets all fields in the same group to U unless they are also Y
      Assert.Equal("Y", ije.HYPO);
      Assert.Equal("U", ije.ANEN);
      Assert.True(fhir.Hypospadias);
      Assert.False(fhir.Anencephaly);
      Assert.False(fhir.NoCongenitalAnomaliesOfTheNewborn);
    }

    [Fact]
    public void TestMotherFatherPlaceOfBirth()
    {
      BirthRecord fhir = new BirthRecord();
      IJENatality ije = new IJENatality(fhir);

      ije.FBPLACD_ST_TER_C = "NH";
      ije.FBPLACE_CNT_C = "US";
      
      ije.BPLACEC_ST_TER = "MA";
      ije.BPLACEC_CNT = "US";
      
      Assert.Equal("NH", ije.FBPLACD_ST_TER_C);
      Assert.Equal("New Hampshire", ije.FBPLACE_ST_TER_TXT);
      Assert.Equal("US", ije.FBPLACE_CNT_C);
      Assert.Equal("United States", ije.FBPLACE_CNTRY_TXT);
      
      Assert.Equal("MA", ije.BPLACEC_ST_TER);
      Assert.Equal("Massachusetts", ije.MBPLACE_ST_TER_TXT);
      Assert.Equal("US", ije.BPLACEC_CNT);
      Assert.Equal("United States", ije.MBPLACE_CNTRY_TXT);
    }

    [Fact]
    public void TestMotherResidenceAndBilling()
    {
      BirthRecord fhir = new BirthRecord();
      IJENatality ije = new IJENatality(fhir);

      ije.STATEC = "NH";
      ije.COUNTRYC = "US";
      ije.LIMITS = "U";
      
      ije.MAIL_STATETXT = "Massachusetts";
      ije.MAIL_CNTRYTXT = "United States";
      
      Assert.Equal("NH", ije.STATEC);
      Assert.Equal("New Hampshire", ije.STATETXT);
      Assert.Equal("US", ije.COUNTRYC);
      Assert.Equal("United States", ije.CNTRYTXT);
      Assert.Equal("U", ije.LIMITS);
      Assert.Equal("UNK", fhir.MotherResidenceWithinCityLimits["code"]);
      
      // Assert.Equal("MA", fhir.MotherBilling["addressState"]);
      // Assert.Equal("Massachusetts", ije.MAIL_STATETXT);
      // Assert.Equal("US", fhir.MotherBilling["addressCountry"]);
      // Assert.Equal("United States", ije.MAIL_CNTRYTXT);
    }

    [Fact]
    public void TestMethodOfDelivery()
    {
      BirthRecord fhir = new BirthRecord();
      IJENatality ije = new IJENatality(fhir);
      Assert.Equal("", ije.ROUT);
      ije.ROUT = "9";
      Assert.Equal("9", ije.ROUT);
      Assert.True(fhir.UnknownFinalRouteAndMethodOfDelivery);
      ije.ROUT = "1";
      Assert.Equal("1", ije.ROUT);
      Assert.False(fhir.UnknownFinalRouteAndMethodOfDelivery);
      Assert.Equal("700000006", fhir.FinalRouteAndMethodOfDelivery["code"]);
      Assert.Equal(CodeSystems.SCT, fhir.FinalRouteAndMethodOfDelivery["system"]);
      ije.ROUT = "";
      Assert.Equal("", fhir.FinalRouteAndMethodOfDelivery["code"]);
      Assert.Equal("", fhir.FinalRouteAndMethodOfDelivery["system"]);
      ije.ROUT = " ";
      Assert.Equal("", fhir.FinalRouteAndMethodOfDelivery["code"]);
      Assert.Equal("", fhir.FinalRouteAndMethodOfDelivery["system"]);
      ije.ROUT = null;
      Assert.Equal("", fhir.FinalRouteAndMethodOfDelivery["code"]);
      Assert.Equal("", fhir.FinalRouteAndMethodOfDelivery["system"]);
    }

    [Fact]
    public void TestSetMotherEthnicity1()
    {
      BirthRecord fhir = new BirthRecord();
      IJENatality ije = new IJENatality(fhir);
      Assert.Equal("U", ije.METHNIC1);
      ije.METHNIC1 = "H";
      Assert.Equal("Y", fhir.MotherEthnicity1Helper);
      Assert.Equal("H", ije.METHNIC1);

      Dictionary<string, string> CodeY = new Dictionary<string, string>();
      CodeY.Add("code", VR.ValueSets.HispanicNoUnknown.Codes[1, 0]);
      CodeY.Add("display", VR.ValueSets.HispanicNoUnknown.Codes[1, 1]);
      CodeY.Add("system", VR.ValueSets.HispanicNoUnknown.Codes[1, 2]);
      Assert.Equal(CodeY, fhir.MotherEthnicity1);
    }

    [Fact]
    public void TestSetFatherEthnicity1()
    {
      BirthRecord fhir = new BirthRecord();
      IJENatality ije = new IJENatality(fhir);
      Assert.Equal("U", ije.FETHNIC1);
      ije.FETHNIC1 = "H";
      Assert.Equal("Y", fhir.FatherEthnicity1Helper);
      Assert.Equal("H", ije.FETHNIC1);

      Dictionary<string, string> CodeY = new Dictionary<string, string>();
      CodeY.Add("code", VR.ValueSets.HispanicNoUnknown.Codes[1, 0]);
      CodeY.Add("display", VR.ValueSets.HispanicNoUnknown.Codes[1, 1]);
      CodeY.Add("system", VR.ValueSets.HispanicNoUnknown.Codes[1, 2]);
      Assert.Equal(CodeY, fhir.FatherEthnicity1);
    }
  }
}
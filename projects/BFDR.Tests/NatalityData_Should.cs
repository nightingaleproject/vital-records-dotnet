using System.IO;
using System.Collections.Generic;
using VR;
using Xunit;
using System;

namespace BFDR.Tests
{
  public class NatalityData_Should
  {
    [Fact]
    public void TestImportPatientChildVitalRecordProperties()
    {
      // Test IJE import.
      IJEBirth ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ijeImported.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJEBirth ijeConverted = new(br);

      // Certificate Number
      Assert.Equal("048858", ijeImported.FILENO);
      Assert.Equal(ijeImported.FILENO, ijeConverted.FILENO);
      // Date of Birth (Infant)--Year
      Assert.Equal("2023", ijeImported.IDOB_YR);
      Assert.Equal(ijeImported.IDOB_YR, ijeConverted.IDOB_YR);
      // State, U.S. Territory or Canadian Province of Birth (Infant) - code
      Assert.Equal("MA".PadRight(2), ijeImported.BSTATE);
      Assert.Equal(ijeImported.BSTATE, ijeConverted.BSTATE);
      Assert.Equal("MA", br.PlaceOfBirth["addressState"]);
      Assert.Equal("MA", br.EventLocationJurisdiction); // TODO - Birth Location Jurisdiction still needs to be finalized.
      // Time of Birth
      Assert.Equal("1230".PadRight(4), ijeImported.TB);
      Assert.Equal(ijeImported.TB, ijeConverted.TB);
      // Sex
      Assert.Equal("M", ijeImported.ISEX);
      Assert.Equal(ijeImported.ISEX, ijeConverted.ISEX);
      Assert.Equal("M", br.BirthSex);
      // Date of Birth (Infant)--Month
      Assert.Equal("11", ijeImported.IDOB_MO);
      Assert.Equal(ijeImported.IDOB_MO, ijeConverted.IDOB_MO);
      // Date of Birth (Infant)--Day
      Assert.Equal("25", ijeImported.IDOB_DY);
      Assert.Equal(ijeImported.IDOB_DY, ijeConverted.IDOB_DY);
      // Check complete date of birth in record
      Assert.Equal(ijeImported.IDOB_YR + "-" + ijeImported.IDOB_MO + "-" + ijeImported.IDOB_DY, br.DateOfBirth);
      Assert.Equal("2023-11-25", br.DateOfBirth);
      // County of Birth | (CountyCodes) (CNTYO)
      Assert.Equal("467", ijeImported.CNTYO);
      // Plurality
      // TODO ---
      // Set Order
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
    public void TestBirthState()
    {
      IJEBirth ije = new IJEBirth();
      ije.BSTATE = "HI";
      BirthRecord br = ije.ToRecord();
      Assert.Equal("HI", br.EventLocationJurisdiction);
      ije.BSTATE = "TT";
      Assert.Equal("TT", ije.BSTATE);
      br = ije.ToRecord();
      Assert.Equal("TT", br.EventLocationJurisdiction);
      ije.BSTATE = "TS";
      Assert.Equal("TS", ije.BSTATE);
      br = ije.ToRecord();
      Assert.Equal("TS", br.EventLocationJurisdiction);
      ije.BSTATE = "ZZ";
      Assert.Equal("ZZ", ije.BSTATE);
      br = ije.ToRecord();
      Assert.Equal("ZZ", br.EventLocationJurisdiction);
    }

    [Fact]
    public void TestBirthDateTime()
    {
      string timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).ToString()[..6];
      if (timeZoneOffset == "00:00:")
      {
        timeZoneOffset = "+00:00";
      }
      BirthRecord rec = new BirthRecord();
      Assert.Null(rec.DateOfBirth);
      Assert.Null(rec.BirthDateTime);
      Assert.Equal("    ", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("  ", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("  ", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("    ", new IJEBirth(rec).TB);
      rec.DateOfBirth = "2021";
      Assert.Equal("2021", rec.DateOfBirth);
      Assert.Equal("2021", rec.BirthDateTime);
      Assert.Equal("2021", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("  ", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("  ", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("    ", new IJEBirth(rec).TB);
      rec.DateOfBirth = "2021-03";
      Assert.Equal("2021-03", rec.DateOfBirth);
      Assert.Equal("2021-03", rec.BirthDateTime);
      Assert.Equal("2021", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("03", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("  ", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("    ", new IJEBirth(rec).TB);
      rec.DateOfBirth = "2021-03-09";
      Assert.Equal("2021-03-09", rec.DateOfBirth);
      Assert.Equal("2021-03-09", rec.BirthDateTime);
      Assert.Equal("2021", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("03", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("09", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("    ", new IJEBirth(rec).TB);
      rec.BirthDateTime = "2024-08-23T13:00:00";
      Assert.Equal("2024-08-23", rec.DateOfBirth);
      Assert.Equal("2024-08-23T13:00:00" + timeZoneOffset, rec.BirthDateTime);
      Assert.Equal("2024", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("08", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("23", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("1300", new IJEBirth(rec).TB);
      rec.DateOfBirth = "1990-10-08";
      Assert.Equal("1990-10-08", rec.DateOfBirth);
      Assert.Equal("1990-10-08T13:00:00" + timeZoneOffset, rec.BirthDateTime);
      Assert.Equal("1990", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("10", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("08", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("1300", new IJEBirth(rec).TB);
      rec.DateOfBirth = "2023";
      Assert.Equal("2023", rec.DateOfBirth);
      Assert.Equal("2023", rec.BirthDateTime);
      Assert.Equal("2023", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("  ", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("  ", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("    ", new IJEBirth(rec).TB);
      Assert.Throws<System.ArgumentException>(() => rec.BirthDateTime = "2023T15:30:00");
      Assert.Equal("2023", rec.DateOfBirth);
      Assert.Equal("2023", rec.BirthDateTime);
      Assert.Throws<System.ArgumentException>(() => rec.BirthDateTime = "T15:30:00");
      Assert.Equal("2023", rec.DateOfBirth);
      Assert.Equal("2023", rec.BirthDateTime);
      Assert.Throws<System.ArgumentException>(() => rec.DateOfBirth = "20");
      Assert.Equal("2023", rec.DateOfBirth);
      Assert.Equal("2023", rec.BirthDateTime);
      rec.DateOfBirth = null;
      Assert.Equal("2023", rec.DateOfBirth);
      Assert.Equal("2023", rec.BirthDateTime);
      rec.DateOfBirth = "";
      Assert.Equal("2023", rec.DateOfBirth);
      Assert.Equal("2023", rec.BirthDateTime);
      rec.BirthDateTime = null;
      Assert.Equal("2023", rec.DateOfBirth);
      Assert.Equal("2023", rec.BirthDateTime);
      rec.BirthDateTime = "";
      Assert.Equal("2023", rec.DateOfBirth);
      Assert.Equal("2023", rec.BirthDateTime);
      Assert.Throws<System.ArgumentException>(() => rec.BirthDateTime = "2022-08-19");
      Assert.Equal("2023", rec.DateOfBirth);
      Assert.Equal("2023", rec.BirthDateTime);
      Assert.Equal("2023", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("  ", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("  ", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("    ", new IJEBirth(rec).TB);
      rec.BirthDateTime = "2024-11-29T12:24";
      Assert.Equal("2024-11-29", rec.DateOfBirth);
      Assert.Equal("2024-11-29T12:24:00-05:00", rec.BirthDateTime);
      Assert.Equal("2024", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("11", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("29", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("1224", new IJEBirth(rec).TB);
      rec.BirthDateTime = "2021-10-28T15:20:46";
      Assert.Equal("2021-10-28", rec.DateOfBirth);
      Assert.Equal("2021-10-28T15:20:46" + timeZoneOffset, rec.BirthDateTime);
      Assert.Equal("2021", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("10", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("28", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("1520", new IJEBirth(rec).TB);
      rec.BirthDateTime = "2018-03-27T10:55:33-02:00";
      Assert.Equal("2018-03-27", rec.DateOfBirth);
      Assert.Equal("2018-03-27T10:55:33-02:00", rec.BirthDateTime);
      Assert.Equal("2018", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("03", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("27", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("1055", new IJEBirth(rec).TB);
      Assert.Throws<System.FormatException>(() => rec.BirthDateTime = "2020-09-05T07");
      Assert.Equal("2018-03-27", rec.DateOfBirth);
      Assert.Equal("2018-03-27T10:55:33-02:00", rec.BirthDateTime);
      Assert.Equal("2018", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("03", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("27", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("1055", new IJEBirth(rec).TB);
      rec.BirthDateTime = "2021-08-07T08:09";
      Assert.Equal("2021-08-07", rec.DateOfBirth);
      Assert.Equal("2021-08-07T08:09:00" + timeZoneOffset, rec.BirthDateTime);
      Assert.Equal("2021", new IJEBirth(rec).IDOB_YR);
      Assert.Equal("08", new IJEBirth(rec).IDOB_MO);
      Assert.Equal("07", new IJEBirth(rec).IDOB_DY);
      Assert.Equal("0809", new IJEBirth(rec).TB);
    }

    [Fact]
    public void TestSetPatientChildVitalRecordProperties()
    {
      IJEBirth ije = new()
      {
        CNTYO = "635"
      };
      Assert.Equal("635", ije.CNTYO);
    }

    [Fact]
    public void TestBirthSexSetters()
    {
      IJEBirth ije = new IJEBirth();
      ije.ISEX = "M";
      Assert.Equal("M", ije.ISEX);
      Assert.Equal("M", ije.ToRecord().BirthSex);
      ije.ISEX = "F";
      Assert.Equal("F", ije.ISEX);
      Assert.Equal("F", ije.ToRecord().BirthSex);
      ije.ToRecord().BirthSex = "M";
      Assert.Equal("M", ije.ISEX);
      Assert.Equal("M", ije.ToRecord().BirthSex);
    }

    // Test Patient Mother Vital Properties
    [Fact]
    public void TestPatientMotherVitalRecordProperties()
    {
      // Test IJE import.
      IJEBirth ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ijeImported.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJEBirth ijeConverted = new(br);

      // Date of Birth (Mother)--Year
      Assert.Equal("1994", ijeImported.MDOB_YR);
      Assert.Equal(ijeImported.MDOB_YR, ijeConverted.MDOB_YR);
      // Date of Birth (Mother)--Month
      Assert.Equal("11", ijeImported.MDOB_MO);
      Assert.Equal(ijeImported.MDOB_MO, ijeConverted.MDOB_MO);
      // Date of Birth (Mother)--Day
      Assert.Equal("29", ijeImported.MDOB_DY);
      Assert.Equal(ijeImported.MDOB_DY, ijeConverted.MDOB_DY);
      Assert.Equal(ijeImported.MDOB_YR + "-" + ijeImported.MDOB_MO + "-" + ijeImported.MDOB_DY, br.MotherDateOfBirth);
      Assert.Equal("1994-11-29", br.MotherDateOfBirth);
    }

    // Test Related Person Father Properties
    [Fact]
    public void TestRelatedPersonFatherProperties()
    {
      // Test IJE import.
      IJEBirth ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ijeImported.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJEBirth ijeConverted = new(br);

      // Date of Birth (Father)--Year
      Assert.Equal("1992", ijeImported.FDOB_YR);
      Assert.Equal(ijeImported.FDOB_YR, ijeConverted.FDOB_YR);
      // Date of Birth (Father)--Month
      Assert.Equal("10", ijeImported.FDOB_MO);
      Assert.Equal(ijeImported.FDOB_MO, ijeConverted.FDOB_MO);
      // Date of Birth (Father)--Day
      Assert.Equal("05", ijeImported.FDOB_DY);
      Assert.Equal(ijeImported.FDOB_DY, ijeConverted.FDOB_DY);
      Assert.Equal(ijeImported.FDOB_YR + "-" + ijeImported.FDOB_MO + "-" + ijeImported.FDOB_DY, br.FatherDateOfBirth);
      Assert.Equal("1992-10-05", br.FatherDateOfBirth);
    }

    [Fact]
    public void TestBirthDateUnknownsErrors()
    {
      IJEBirth ije = new();
      Assert.Throws<System.FormatException>(() => ije.IDOB_DY = "99");
      Assert.Throws<System.FormatException>(() => ije.IDOB_MO = "99");
      Assert.Throws<System.FormatException>(() => ije.IDOB_YR = "9999");
      Assert.Null(ije.ToRecord().DateOfBirth);
      Assert.Throws<System.FormatException>(() => ije.MDOB_DY = "99");
      Assert.Throws<System.FormatException>(() => ije.MDOB_MO = "99");
      Assert.Throws<System.FormatException>(() => ije.MDOB_YR = "9999");
      Assert.Null(ije.ToRecord().MotherDateOfBirth);
      Assert.Throws<System.FormatException>(() => ije.FDOB_DY = "99");
      Assert.Throws<System.FormatException>(() => ije.FDOB_MO = "99");
      Assert.Throws<System.FormatException>(() => ije.MDOB_YR = "9999");
      Assert.Null(ije.ToRecord().FatherDateOfBirth);
      // Sample tiny IJE string with parent birthdates set to 9999.
      Assert.Throws<System.Reflection.TargetInvocationException>(() => new IJEBirth("                                                      99999999                  99999999", true));
    }

    [Fact]
    public void TestBirthDateOrderingErrors()
    {
      IJEBirth ije = new();
      // Child
      
      // Mother
      Assert.Throws<System.ArgumentException>(() => ije.MDOB_MO = "14");
      Assert.Equal("  ", ije.MDOB_MO);
      Assert.Null(ije.ToRecord().MotherDateOfBirth);
      Assert.Throws<System.ArgumentException>(() => ije.MDOB_DY = "14");
      Assert.Equal("  ", ije.MDOB_DY);
      Assert.Null(ije.ToRecord().MotherDateOfBirth);
      ije.MDOB_YR = "2000";
      Assert.Equal("2000", ije.MDOB_YR);
      Assert.Equal("2000", ije.ToRecord().MotherDateOfBirth);
      Assert.Throws<System.ArgumentException>(() => ije.MDOB_DY = "14");
      Assert.Equal("  ", ije.MDOB_DY);
      Assert.Equal("2000", ije.ToRecord().MotherDateOfBirth);
      ije.MDOB_MO = "10";
      Assert.Equal("2000", ije.MDOB_YR);
      Assert.Equal("10", ije.MDOB_MO);
      Assert.Equal("2000-10", ije.ToRecord().MotherDateOfBirth);
      ije.MDOB_DY = "21";
      Assert.Equal("2000", ije.MDOB_YR);
      Assert.Equal("10", ije.MDOB_MO);
      Assert.Equal("21", ije.MDOB_DY);
      Assert.Equal("2000-10-21", ije.ToRecord().MotherDateOfBirth);

      // Father
    }

    [Fact]
    public void TestParentBirthDateSetters()
    {
      // Patient Mother.
      IJEBirth ije = new();
      ije.MDOB_YR = "2000";
      Assert.Equal("2000", ije.ToRecord().MotherDateOfBirth);
      ije.MDOB_MO = "09";
      Assert.Equal("2000-09", ije.ToRecord().MotherDateOfBirth);
      ije.MDOB_DY = "27";
      Assert.Equal("2000-09-27", ije.ToRecord().MotherDateOfBirth);
      ije.MAGE_BYPASS = "0";
      ije.MAGER = "29";
      Assert.Equal(29, ije.ToRecord().MotherReportedAgeAtDelivery);
      Assert.Equal("2000", ije.MDOB_YR);
      Assert.Equal("09", ije.MDOB_MO);
      Assert.Equal("27", ije.MDOB_DY);
      Assert.Equal("0", ije.MAGE_BYPASS);
      Assert.Equal("29", ije.MAGER);
      Assert.Equal("2000-09-27", ije.ToRecord().MotherDateOfBirth);
      ije.MDOB_DY = "30";
      Assert.Equal("30", ije.MDOB_DY);
      Assert.Equal("2000-09-30", ije.ToRecord().MotherDateOfBirth);
      Assert.Throws<System.FormatException>(() => ije.MDOB_MO = "99");
      Assert.Equal("2000-09-30", ije.ToRecord().MotherDateOfBirth);
      ije.MDOB_MO = "05";
      Assert.Equal("05", ije.MDOB_MO);
      Assert.Equal("2000-05-30", ije.ToRecord().MotherDateOfBirth);
      Assert.Throws<System.FormatException>(() => ije.MDOB_YR = "9999");
      Assert.Equal("2000", ije.MDOB_YR);
      Assert.Equal("2000-05-30", ije.ToRecord().MotherDateOfBirth);

      // Related Person Father.
      ije.FDOB_YR = "1999";
      Assert.Equal("1999", ije.ToRecord().FatherDateOfBirth);
      ije.FDOB_MO = "11";
      Assert.Equal("1999-11", ije.ToRecord().FatherDateOfBirth);
      ije.FDOB_DY = "27";
      Assert.Equal("1999-11-27", ije.ToRecord().FatherDateOfBirth);
      ije.FAGE_BYPASS = "0";
      ije.FAGER = "31";
      Assert.Equal(31, ije.ToRecord().FatherReportedAgeAtDelivery);
      Assert.Equal("1999", ije.FDOB_YR);
      Assert.Equal("11", ije.FDOB_MO);
      Assert.Equal("27", ije.FDOB_DY);
      Assert.Equal("0", ije.FAGE_BYPASS);
      Assert.Equal("31", ije.FAGER);
      Assert.Throws<System.FormatException>(() => ije.FDOB_DY = "99");
      Assert.Equal("27", ije.FDOB_DY);
      Assert.Equal("1999-11-27", ije.ToRecord().FatherDateOfBirth);
      ije.FDOB_DY = "22";
      Assert.Equal("1999-11-22", ije.ToRecord().FatherDateOfBirth);
      Assert.Throws<System.FormatException>(() => ije.FDOB_MO = "99");
      Assert.Equal("1999-11-22", ije.ToRecord().FatherDateOfBirth);
      ije.FDOB_MO = "05";
      Assert.Equal("05", ije.FDOB_MO);
      Assert.Equal("1999-05-22", ije.ToRecord().FatherDateOfBirth);
      Assert.Throws<System.FormatException>(() => ije.FDOB_YR = "9999");
      Assert.Equal("1999-05-22", ije.ToRecord().FatherDateOfBirth);
    }

    [Fact]
    public void TestBirthRecordMotherDateOfBirthRoundtrip()
    {
      IJEBirth ije = new IJEBirth();
      ije.MDOB_YR = "1992";
      ije.MDOB_MO = "01";
      ije.MDOB_DY = "12";
      // convert IJE to FHIR
      BirthRecord br = ije.ToRecord();
      Assert.Equal("1992-01-12", br.MotherDateOfBirth);

      // then to a json string
      string asJson = br.ToJSON();
      // Create a fhir record from the json
      BirthRecord brConverted = new BirthRecord(asJson);
      Assert.Equal("1992-01-12", brConverted.MotherDateOfBirth);

      // convert back to IJE and confirm the values are the same
      IJEBirth ijeConverted = new IJEBirth(brConverted);
      Assert.Equal("1992", ijeConverted.MDOB_YR);
      Assert.Equal("01", ijeConverted.MDOB_MO);
      Assert.Equal("12", ijeConverted.MDOB_DY);
    }

    [Fact]
    public void TestPlurality()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);
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
      IJEBirth ije = new IJEBirth(fhir);
      // a new record is empty, so these values will be U in IJE and false in FHIR
      Assert.False(fhir.Anencephaly);
      Assert.False(fhir.Hypospadias);
      Assert.False(fhir.Meningomyelocele);
      Assert.False(fhir.NoCongenitalAnomaliesOfTheNewborn);
      Assert.Equal("U", ije.ANEN);
      Assert.Equal("U", ije.HYPO);
      Assert.Equal("U", ije.MNSB);
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
      // Setting N can potentially set the none-of-the-above field, but setting U can not do that.
      Assert.Equal("N", ije.ANEN);
      Assert.False(fhir.Anencephaly);
      Assert.True(fhir.NoCongenitalAnomaliesOfTheNewborn);
      ije.ANEN = "Y";
      Assert.Equal("Y", ije.ANEN);
      Assert.True(fhir.Anencephaly);
      Assert.False(fhir.NoCongenitalAnomaliesOfTheNewborn);
      ije.HYPO = "N"; // Setting any field to N sets none-of-the-above only if there are no Y in the same group
      Assert.Equal("N", ije.HYPO);
      Assert.Equal("Y", ije.ANEN);
      Assert.False(fhir.Hypospadias);
      Assert.True(fhir.Anencephaly);
      Assert.False(fhir.NoCongenitalAnomaliesOfTheNewborn);
      ije.HYPO = "Y"; // Setting any field to Y sets all fields in the same group to N unless they are also Y
      Assert.Equal("Y", ije.HYPO);
      Assert.Equal("N", ije.MNSB);
      Assert.True(fhir.Hypospadias);
      Assert.False(fhir.Meningomyelocele);
      Assert.False(fhir.NoCongenitalAnomaliesOfTheNewborn);
    }

    [Fact]
    public void TestMotherFatherPlaceOfBirth()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);

      ije.FBPLACD_ST_TER_C = "NH";
      ije.FBPLACE_CNT_C = "US";

      ije.BPLACEC_ST_TER = "MA";
      ije.BPLACEC_CNT = "US";

      Assert.Equal("NH", ije.FBPLACD_ST_TER_C);
      Assert.Equal("New Hampshire", ije.FBPLACE_ST_TER_TXT.Trim());
      Assert.Equal("US", ije.FBPLACE_CNT_C);
      Assert.Equal("United States", ije.FBPLACE_CNTRY_TXT.Trim());

      Assert.Equal("MA", ije.BPLACEC_ST_TER);
      Assert.Equal("Massachusetts", ije.MBPLACE_ST_TER_TXT.Trim());
      Assert.Equal("US", ije.BPLACEC_CNT.Trim());
      Assert.Equal("United States", ije.MBPLACE_CNTRY_TXT.Trim());
    }

    [Fact]
    public void TestMotherResidenceAndBilling()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);

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
      IJEBirth ije = new IJEBirth(fhir);
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
      IJEBirth ije = new IJEBirth(fhir);
      Assert.Equal("U", ije.METHNIC1);
      ije.METHNIC1 = "H";
      Assert.Equal("Y", fhir.MotherEthnicity1Helper);
      Assert.Equal("H", ije.METHNIC1);

      Dictionary<string, string> CodeY = new Dictionary<string, string>();
      CodeY.Add("code", VR.ValueSets.YesNoUnknown.Codes[1, 0]);
      CodeY.Add("display", VR.ValueSets.YesNoUnknown.Codes[1, 1]);
      CodeY.Add("system", VR.ValueSets.YesNoUnknown.Codes[1, 2]);
      Assert.Equal(CodeY, fhir.MotherEthnicity1);
    }

    [Fact]
    public void TestSetFatherEthnicity1()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);
      Assert.Equal("U", ije.FETHNIC1);
      ije.FETHNIC1 = "H";
      Assert.Equal("Y", fhir.FatherEthnicity1Helper);
      Assert.Equal("H", ije.FETHNIC1);

      Dictionary<string, string> CodeY = new Dictionary<string, string>();
      CodeY.Add("code", VR.ValueSets.YesNoUnknown.Codes[1, 0]);
      CodeY.Add("display", VR.ValueSets.YesNoUnknown.Codes[1, 1]);
      CodeY.Add("system", VR.ValueSets.YesNoUnknown.Codes[1, 2]);
      Assert.Equal(CodeY, fhir.FatherEthnicity1);
    }

    [Fact]
    public void TestSetEthnicityLiteral()
    {
      IJEBirth ije = new();
      Assert.Equal("", ije.METHNIC5.Trim());
      Assert.Equal("", ije.FETHNIC5.Trim());
      ije.METHNIC5 = "Bolivian";
      ije.FETHNIC5 = "Columbian";
      Assert.Equal("Bolivian", ije.METHNIC5.Trim());
      Assert.Equal("Columbian", ije.FETHNIC5.Trim());
      Assert.Equal("Bolivian", ije.ToRecord().MotherEthnicityLiteral.Trim());
      Assert.Equal("Columbian", ije.ToRecord().FatherEthnicityLiteral.Trim());
    }

    [Fact]
    public void TestImportEthnicityLiteral()
    {
      IJEBirth ije = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      Assert.Equal("Chilean", ije.METHNIC5.Trim());
      Assert.Equal("Panamanian", ije.FETHNIC5.Trim());
      Assert.Equal("Chilean", ije.ToRecord().MotherEthnicityLiteral.Trim());
      Assert.Equal("Panamanian", ije.ToRecord().FatherEthnicityLiteral.Trim());
    }

    [Fact]
    public void TestSetCodedEthnicityLiteral()
    {
      IJEBirth ije = new();
      Assert.Equal("", ije.METHNIC5C.Trim());
      Assert.Equal("", ije.FETHNIC5C.Trim());
      ije.METHNIC5C = VR.ValueSets.HispanicOrigin.Bolivian;
      ije.FETHNIC5C = VR.ValueSets.HispanicOrigin.Chicano;
      Assert.Equal("232", ije.METHNIC5C);
      Assert.Equal("214", ije.FETHNIC5C);
      Assert.Equal("232", ije.ToRecord().MotherEthnicityCodeForLiteralHelper);
      Assert.Equal("214", ije.ToRecord().FatherEthnicityCodeForLiteralHelper);
    }

    [Fact]
    public void TestSetCodedEthnicity()
    {
      IJEBirth ije = new();
      Assert.Equal("", ije.METHNICE.Trim());
      Assert.Equal("", ije.FETHNICE.Trim());
      Assert.Equal("", ije.METHNIC5C.Trim());
      Assert.Equal("", ije.FETHNIC5C.Trim());
      ije.FETHNICE = VR.ValueSets.HispanicOrigin.Chicano;
      ije.METHNICE = VR.ValueSets.HispanicOrigin.Bolivian;
      ije.FETHNIC5C = VR.ValueSets.HispanicOrigin.Dominican;
      ije.METHNIC5C = VR.ValueSets.HispanicOrigin.Mestizo;
      Assert.Equal("232", ije.METHNICE);
      Assert.Equal("214", ije.FETHNICE);
      Assert.Equal("289", ije.METHNIC5C);
      Assert.Equal("275", ije.FETHNIC5C);
      Assert.Equal("214", ije.ToRecord().FatherEthnicityEditedCodeHelper);
      Assert.Equal("232", ije.ToRecord().MotherEthnicityEditedCodeHelper);
      Assert.Equal("275", ije.ToRecord().FatherEthnicityCodeForLiteralHelper);
      Assert.Equal("289", ije.ToRecord().MotherEthnicityCodeForLiteralHelper);
    }

    [Fact]
    public void TestSetFatherRaceTabulation()
    {
      IJEBirth ije = new();
      Assert.Equal("", ije.FRACE1E.Trim());
      Assert.Equal("", ije.FRACE2E.Trim());
      Assert.Equal("", ije.FRACE3E.Trim());
      Assert.Equal("", ije.FRACE4E.Trim());
      Assert.Equal("", ije.FRACE6E.Trim());
      Assert.Equal("", ije.FRACE7E.Trim());
      Assert.Equal("", ije.FRACE8E.Trim());
      Assert.Equal("", ije.FRACE16C.Trim());
      Assert.Equal("", ije.FRACE17C.Trim());
      Assert.Equal("", ije.FRACE18C.Trim());
      Assert.Equal("", ije.FRACE19C.Trim());
      Assert.Equal("", ije.FRACE20C.Trim());
      Assert.Equal("", ije.FRACE21C.Trim());
      Assert.Equal("", ije.FRACE22C.Trim());
      Assert.Equal("", ije.FRACE23C.Trim());
      ije.FRACE1E = VR.ValueSets.RaceCode.Arab;
      Assert.Equal("102", ije.FRACE1E);
      Assert.Equal("102", ije.ToRecord().FatherRaceTabulation1EHelper);
      ije.FRACE2E = VR.ValueSets.RaceCode.Yapese;
      Assert.Equal("541", ije.FRACE2E);
      Assert.Equal("541", ije.ToRecord().FatherRaceTabulation2EHelper);
      ije.FRACE3E = VR.ValueSets.RaceCode.Moor;
      Assert.Equal("667", ije.FRACE3E);
      Assert.Equal("667", ije.ToRecord().FatherRaceTabulation3EHelper);
      ije.FRACE4E = VR.ValueSets.RaceCode.Arab;
      Assert.Equal("102", ije.FRACE4E);
      Assert.Equal("102", ije.ToRecord().FatherRaceTabulation4EHelper);
      ije.FRACE5E = VR.ValueSets.RaceCode.Apache;
      Assert.Equal("A09", ije.FRACE5E);
      Assert.Equal("A09", ije.ToRecord().FatherRaceTabulation5EHelper);
      ije.FRACE6E = VR.ValueSets.RaceCode.Yapese;
      Assert.Equal("541", ije.FRACE6E);
      Assert.Equal("541", ije.ToRecord().FatherRaceTabulation6EHelper);
      ije.FRACE7E = VR.ValueSets.RaceCode.Hmong;
      Assert.Equal("422", ije.FRACE7E);
      Assert.Equal("422", ije.ToRecord().FatherRaceTabulation7EHelper);
      ije.FRACE8E = VR.ValueSets.RaceCode.Moor;
      Assert.Equal("667", ije.FRACE8E);
      Assert.Equal("667", ije.ToRecord().FatherRaceTabulation8EHelper);

      ije.FRACE16C = VR.ValueSets.RaceCode.Apache;
      Assert.Equal("A09", ije.FRACE16C);
      Assert.Equal("A09", ije.ToRecord().FatherFirstAmericanIndianCodeHelper);
      ije.FRACE17C = VR.ValueSets.RaceCode.Algonquian;
      Assert.Equal("A05", ije.FRACE17C);
      Assert.Equal("A05", ije.ToRecord().FatherSecondAmericanIndianCodeHelper);
      ije.FRACE18C = VR.ValueSets.RaceCode.Hmong;
      Assert.Equal("422", ije.FRACE18C);
      Assert.Equal("422", ije.ToRecord().FatherFirstOtherAsianCodeHelper);
      ije.FRACE19C = VR.ValueSets.RaceCode.Filipino;
      Assert.Equal("421", ije.FRACE19C);
      Assert.Equal("421", ije.ToRecord().FatherSecondOtherAsianCodeHelper);
      ije.FRACE20C = VR.ValueSets.RaceCode.Yapese;
      Assert.Equal("541", ije.FRACE20C);
      Assert.Equal("541", ije.ToRecord().FatherFirstOtherPacificIslanderCodeHelper);
      ije.FRACE21C = VR.ValueSets.RaceCode.Fijian;
      Assert.Equal("542", ije.FRACE21C);
      Assert.Equal("542", ije.ToRecord().FatherSecondOtherPacificIslanderCodeHelper);
      ije.FRACE22C = VR.ValueSets.RaceCode.Fijian;
      Assert.Equal("542", ije.FRACE22C);
      Assert.Equal("542", ije.ToRecord().FatherFirstOtherRaceCodeHelper);
      ije.FRACE23C = VR.ValueSets.RaceCode.Moor;
      Assert.Equal("667", ije.FRACE23C);
      Assert.Equal("667", ije.ToRecord().FatherSecondOtherRaceCodeHelper);
    }

    [Fact]
    public void TestSetMotherRaceTabulation()
    {
      IJEBirth ije = new();
      Assert.Equal("", ije.MRACE1E.Trim());
      Assert.Equal("", ije.MRACE2E.Trim());
      Assert.Equal("", ije.MRACE3E.Trim());
      Assert.Equal("", ije.MRACE4E.Trim());
      Assert.Equal("", ije.MRACE6E.Trim());
      Assert.Equal("", ije.MRACE7E.Trim());
      Assert.Equal("", ije.MRACE8E.Trim());
      Assert.Equal("", ije.MRACE16C.Trim());
      Assert.Equal("", ije.MRACE17C.Trim());
      Assert.Equal("", ije.MRACE18C.Trim());
      Assert.Equal("", ije.MRACE19C.Trim());
      Assert.Equal("", ije.MRACE20C.Trim());
      Assert.Equal("", ije.MRACE21C.Trim());
      Assert.Equal("", ije.MRACE22C.Trim());
      Assert.Equal("", ije.MRACE23C.Trim());
      ije.MRACE1E = VR.ValueSets.RaceCode.Arab;
      Assert.Equal("102", ije.MRACE1E);
      Assert.Equal("102", ije.ToRecord().MotherRaceTabulation1EHelper);
      ije.MRACE2E = VR.ValueSets.RaceCode.Yapese;
      Assert.Equal("541", ije.MRACE2E);
      Assert.Equal("541", ije.ToRecord().MotherRaceTabulation2EHelper);
      ije.MRACE3E = VR.ValueSets.RaceCode.Moor;
      Assert.Equal("667", ije.MRACE3E);
      Assert.Equal("667", ije.ToRecord().MotherRaceTabulation3EHelper);
      ije.MRACE4E = VR.ValueSets.RaceCode.Arab;
      Assert.Equal("102", ije.MRACE4E);
      Assert.Equal("102", ije.ToRecord().MotherRaceTabulation4EHelper);
      ije.MRACE5E = VR.ValueSets.RaceCode.Apache;
      Assert.Equal("A09", ije.MRACE5E);
      Assert.Equal("A09", ije.ToRecord().MotherRaceTabulation5EHelper);
      ije.MRACE6E = VR.ValueSets.RaceCode.Yapese;
      Assert.Equal("541", ije.MRACE6E);
      Assert.Equal("541", ije.ToRecord().MotherRaceTabulation6EHelper);
      ije.MRACE7E = VR.ValueSets.RaceCode.Hmong;
      Assert.Equal("422", ije.MRACE7E);
      Assert.Equal("422", ije.ToRecord().MotherRaceTabulation7EHelper);
      ije.MRACE8E = VR.ValueSets.RaceCode.Moor;
      Assert.Equal("667", ije.MRACE8E);
      Assert.Equal("667", ije.ToRecord().MotherRaceTabulation8EHelper);

      ije.MRACE16C = VR.ValueSets.RaceCode.Apache;
      Assert.Equal("A09", ije.MRACE16C);
      Assert.Equal("A09", ije.ToRecord().MotherFirstAmericanIndianCodeHelper);
      ije.MRACE17C = VR.ValueSets.RaceCode.Algonquian;
      Assert.Equal("A05", ije.MRACE17C);
      Assert.Equal("A05", ije.ToRecord().MotherSecondAmericanIndianCodeHelper);
      ije.MRACE18C = VR.ValueSets.RaceCode.Hmong;
      Assert.Equal("422", ije.MRACE18C);
      Assert.Equal("422", ije.ToRecord().MotherFirstOtherAsianCodeHelper);
      ije.MRACE19C = VR.ValueSets.RaceCode.Filipino;
      Assert.Equal("421", ije.MRACE19C);
      Assert.Equal("421", ije.ToRecord().MotherSecondOtherAsianCodeHelper);
      ije.MRACE20C = VR.ValueSets.RaceCode.Yapese;
      Assert.Equal("541", ije.MRACE20C);
      Assert.Equal("541", ije.ToRecord().MotherFirstOtherPacificIslanderCodeHelper);
      ije.MRACE21C = VR.ValueSets.RaceCode.Fijian;
      Assert.Equal("542", ije.MRACE21C);
      Assert.Equal("542", ije.ToRecord().MotherSecondOtherPacificIslanderCodeHelper);
      ije.MRACE22C = VR.ValueSets.RaceCode.Fijian;
      Assert.Equal("542", ije.MRACE22C);
      Assert.Equal("542", ije.ToRecord().MotherFirstOtherRaceCodeHelper);
      ije.MRACE23C = VR.ValueSets.RaceCode.Moor;
      Assert.Equal("667", ije.MRACE23C);
      Assert.Equal("667", ije.ToRecord().MotherSecondOtherRaceCodeHelper);
    }

    [Fact]
    public void TestImportMotherBirthplace()
    {
      // Test IJE import.
      IJEBirth ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ijeImported.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJEBirth ijeConverted = new(br);

      // Country
      Assert.Equal("US", ijeImported.BPLACEC_CNT);
      Assert.Equal(ijeImported.BPLACEC_CNT, br.MotherPlaceOfBirth["addressCountry"]);
      Assert.Equal(ijeImported.BPLACEC_CNT, ijeConverted.BPLACEC_CNT);
      // State
      Assert.Equal("CA", ijeImported.BPLACEC_ST_TER);
      Assert.Equal(ijeImported.BPLACEC_ST_TER, br.MotherPlaceOfBirth["addressState"]);
      Assert.Equal(ijeImported.BPLACEC_ST_TER, ijeConverted.BPLACEC_ST_TER);
    }

    [Fact]
    public void TestSetMotherBirthplace()
    {
      // Manually set ije values.
      IJEBirth ije = new()
      {
        BPLACEC_CNT = "US",
        BPLACEC_ST_TER = "FL"
      };
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ije.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJEBirth ijeConverted = new(br);

      // Country
      Assert.Equal("US", ije.BPLACEC_CNT);
      Assert.Equal(ije.BPLACEC_CNT, br.MotherPlaceOfBirth["addressCountry"]);
      Assert.Equal(ije.BPLACEC_CNT, ijeConverted.BPLACEC_CNT);
      ije.BPLACEC_CNT = "AE";
      Assert.Equal("AE", ije.BPLACEC_CNT);
      // State
      Assert.Equal("FL", ije.BPLACEC_ST_TER);
      Assert.Equal(ije.BPLACEC_ST_TER, br.MotherPlaceOfBirth["addressState"]);
      Assert.Equal(ije.BPLACEC_ST_TER, ijeConverted.BPLACEC_ST_TER);
      ije.BPLACEC_ST_TER = "AL";
      Assert.Equal("AL", ije.BPLACEC_ST_TER);
    }

    [Fact]
    public void TestSetBirthPlaceType()
    {
      // Manually set ije values.
      IJEBirth ije = new()
      {
        BPLACE = "1"
      };
      Assert.Equal("1", ije.BPLACE);
      Assert.Equal("22232009", ije.ToRecord().BirthPhysicalLocation["code"]);
      Assert.Equal("Hospital", ije.ToRecord().BirthPhysicalLocation["display"]);
      Assert.Equal("http://snomed.info/sct", ije.ToRecord().BirthPhysicalLocation["system"]);

      ije.BPLACE = "3";
      Assert.Equal("3", ije.BPLACE);
      Assert.Equal("408839006", ije.ToRecord().BirthPhysicalLocation["code"]);
      Assert.Equal("Planned home birth", ije.ToRecord().BirthPhysicalLocation["display"]);
      Assert.Equal("http://snomed.info/sct", ije.ToRecord().BirthPhysicalLocation["system"]);
    }

    [Fact]
    public void TestImportBirthPlaceType()
    {
      // Test IJE import.
      IJEBirth ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ijeImported.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJEBirth ijeConverted = new(br);

      Assert.Equal("2", ijeImported.BPLACE);
      Assert.Equal(ijeImported.BPLACE, ijeConverted.BPLACE);
      Assert.Equal("91154008", ijeImported.ToRecord().BirthPhysicalLocation["code"]);
      Assert.Equal("Free-standing birthing center", ijeImported.ToRecord().BirthPhysicalLocation["display"]);
      Assert.Equal("http://snomed.info/sct", ijeImported.ToRecord().BirthPhysicalLocation["system"]);
    }

    [Fact]
    public void TestImportIdentifiers()
    {
      IJEBirth ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      BirthRecord br = ijeImported.ToRecord();
      IJEBirth ijeConverted = new(br);
      // Certificate Number - FILENO
      Assert.Equal("48858".PadLeft(6, '0'), ijeImported.FILENO);
      Assert.Equal(ijeImported.FILENO, ijeConverted.FILENO);
      Assert.Equal(ijeImported.FILENO, br.CertificateNumber.PadLeft(6, '0'));
      // Auxiliary State file number - AUXNO
      Assert.Equal("87".PadRight(12, ' '), ijeImported.AUXNO);
      Assert.Equal(ijeImported.AUXNO, ijeConverted.AUXNO);
      Assert.Equal(ijeImported.AUXNO, br.StateLocalIdentifier1.PadRight(12, ' '));
      Assert.Contains("2023", br.DateOfBirth);
      Assert.Equal(ijeImported.IDOB_YR + ijeImported.BSTATE + ijeImported.FILENO, br.RecordIdentifier);
      Assert.Equal("2023MA048858", br.RecordIdentifier);
    }

    [Fact]
    public void TestSetIdentifiers()
    {
      IJEBirth ije = new IJEBirth();
      Assert.Equal("".PadLeft(12, ' '), ije.AUXNO);
      ije.FILENO = "765765";
      Assert.Equal("765765".PadLeft(6, '0'), ije.FILENO);
      ije.AUXNO = "32";
      Assert.Equal("32".PadRight(12, ' '), ije.AUXNO);
      ije.IDOB_YR = "2010";
      ije.BSTATE = "HI";
      ije.FILENO = "897897";
      Assert.Equal("897897".PadLeft(6, '0'), ije.FILENO);
      BirthRecord br = ije.ToRecord();
      Assert.Equal(ije.FILENO, br.CertificateNumber.PadLeft(6, '0'));
      Assert.Equal(ije.AUXNO, br.StateLocalIdentifier1.PadRight(12, ' '));
      Assert.Equal(ije.IDOB_YR + ije.BSTATE + ije.FILENO, br.RecordIdentifier);
      Assert.Equal("2010HI897897", br.RecordIdentifier);

      br = new BirthRecord();
      ije = new IJEBirth(br);
      Assert.Equal("".PadLeft(12, ' '), ije.AUXNO);

    }

    [Fact]
    public void TestSetSmoking()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);
      Assert.Equal("  ", ije.CIGPN);
      Assert.Null(fhir.CigarettesPerDayInThreeMonthsPriorToPregancy);
      ije.CIGPN = "99";
      Assert.Equal("99", ije.CIGPN);
      Assert.Equal(-1, fhir.CigarettesPerDayInThreeMonthsPriorToPregancy);
      ije.CIGPN = "20";
      Assert.Equal("20", ije.CIGPN);
      Assert.Equal(20, fhir.CigarettesPerDayInThreeMonthsPriorToPregancy);
      ije.CIGPN = "  ";
      Assert.Equal("  ", ije.CIGPN);
      Assert.Null(fhir.CigarettesPerDayInThreeMonthsPriorToPregancy);

      Assert.Equal("  ", ije.CIGFN);
      Assert.Null(fhir.CigarettesPerDayInFirstTrimester);
      ije.CIGFN = "99";
      Assert.Equal("99", ije.CIGFN);
      Assert.Equal(-1, fhir.CigarettesPerDayInFirstTrimester);
      ije.CIGFN = "22";
      Assert.Equal("22", ije.CIGFN);
      Assert.Equal(22, fhir.CigarettesPerDayInFirstTrimester);
      ije.CIGFN = "  ";
      Assert.Equal("  ", ije.CIGFN);
      Assert.Null(fhir.CigarettesPerDayInFirstTrimester);

      Assert.Equal("  ", ije.CIGSN);
      Assert.Null(fhir.CigarettesPerDayInSecondTrimester);
      ije.CIGSN = "99";
      Assert.Equal("99", ije.CIGSN);
      Assert.Equal(-1, fhir.CigarettesPerDayInSecondTrimester);
      ije.CIGSN = "18";
      Assert.Equal("18", ije.CIGSN);
      Assert.Equal(18, fhir.CigarettesPerDayInSecondTrimester);
      ije.CIGSN = "  ";
      Assert.Equal("  ", ije.CIGSN);
      Assert.Null(fhir.CigarettesPerDayInSecondTrimester);

      Assert.Equal("  ", ije.CIGLN);
      Assert.Null(fhir.CigarettesPerDayInLastTrimester);
      ije.CIGLN = "99";
      Assert.Equal("99", ije.CIGLN);
      Assert.Equal(-1, fhir.CigarettesPerDayInLastTrimester);
      ije.CIGLN = "21";
      Assert.Equal("21", ije.CIGLN);
      Assert.Equal(21, fhir.CigarettesPerDayInLastTrimester);
      ije.CIGLN = "  ";
      Assert.Equal("  ", ije.CIGLN);
      Assert.Null(fhir.CigarettesPerDayInLastTrimester);
    }

    [Fact]
    public void TestSetWeight()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);
      Assert.Equal("   ", ije.PWGT);
      Assert.Null(fhir.MotherPrepregnancyWeight);
      Assert.Null(fhir.MotherPrepregnancyWeightEditFlagHelper);
      Assert.Equal("   ", ije.DWGT);
      Assert.Null(fhir.MotherWeightAtDelivery);
      Assert.Null(fhir.MotherWeightAtDeliveryEditFlagHelper);
      Assert.Equal("    ", ije.BWG);
      Assert.Null(fhir.BirthWeight);
      Assert.Null(fhir.BirthWeightEditFlagHelper);
      ije.PWGT = "095";
      ije.PWGT_BYPASS = "1";
      ije.DWGT_BYPASS = "0";
      ije.DWGT = "120";
      ije.BWG = "3200";
      ije.BW_BYPASS = "2";
      Assert.Equal("1", ije.PWGT_BYPASS);
      Assert.Equal("095", ije.PWGT);
      Assert.Equal("0", ije.DWGT_BYPASS);
      Assert.Equal("120", ije.DWGT);
      Assert.Equal("3200", ije.BWG);
      Assert.Equal("2", ije.BW_BYPASS);
      ije.PWGT = "999";
      Assert.Equal("999", ije.PWGT);
      Assert.Equal(-1, fhir.MotherPrepregnancyWeight);
    }

    [Fact]
    public void TestSetOccupationAndIndustry()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);
      Assert.Null(fhir.MotherOccupation);
      Assert.Equal("", ije.MOM_OC_T.Trim());
      Assert.Null(fhir.FatherOccupation);
      Assert.Equal("", ije.DAD_OC_T.Trim());
      Assert.Null(fhir.MotherIndustry);
      Assert.Equal("", ije.MOM_IN_T.Trim());
      Assert.Null(fhir.FatherIndustry);
      Assert.Equal("", ije.DAD_IN_T.Trim());
      ije.MOM_OC_T = "scientist";
      Assert.Equal("scientist", ije.MOM_OC_T.Trim());
      Assert.Equal("scientist", fhir.MotherOccupation);
      Assert.Null(fhir.MotherIndustry);
      Assert.Equal("", ije.MOM_IN_T.Trim());
      Assert.Null(fhir.FatherOccupation);
      Assert.Equal("", ije.DAD_OC_T.Trim());
      Assert.Null(fhir.FatherIndustry);
      Assert.Equal("", ije.DAD_IN_T.Trim());
      ije.MOM_IN_T = "public health";
      Assert.Equal("scientist", ije.MOM_OC_T.Trim());
      Assert.Equal("scientist", fhir.MotherOccupation);
      Assert.Equal("public health", ije.MOM_IN_T.Trim());
      Assert.Equal("public health", fhir.MotherIndustry);
      Assert.Null(fhir.FatherOccupation);
      Assert.Equal("", ije.DAD_OC_T.Trim());
      Assert.Null(fhir.FatherIndustry);
      Assert.Equal("", ije.DAD_IN_T.Trim());
      ije.DAD_IN_T = "real estate";
      Assert.Equal("scientist", ije.MOM_OC_T.Trim());
      Assert.Equal("scientist", fhir.MotherOccupation);
      Assert.Equal("public health", ije.MOM_IN_T.Trim());
      Assert.Equal("public health", fhir.MotherIndustry);
      Assert.Null(fhir.FatherOccupation);
      Assert.Equal("", ije.DAD_OC_T.Trim());
      Assert.Equal("real estate", ije.DAD_IN_T.Trim());
      Assert.Equal("real estate", fhir.FatherIndustry);
      ije.DAD_OC_T = "realtor";
      Assert.Equal("realtor", ije.DAD_OC_T.Trim());
      Assert.Equal("realtor", fhir.FatherOccupation);
      Assert.Equal("real estate", ije.DAD_IN_T.Trim());
      Assert.Equal("real estate", fhir.FatherIndustry);
      Assert.Equal("scientist", ije.MOM_OC_T.Trim());
      Assert.Equal("scientist", fhir.MotherOccupation);
      Assert.Equal("public health", ije.MOM_IN_T.Trim());
      Assert.Equal("public health", fhir.MotherIndustry);
    }

    [Fact]
    public void TestImportLocation()
    {
      IJEBirth ije = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")));
      BirthRecord br = ije.ToRecord();
      Assert.Equal("878787878787", ije.FNPI);
      Assert.Equal("6678", ije.SFN);
      Assert.Equal("Bryant Medical".PadRight(50), ije.HOSP);
      Assert.Equal("Lennon Medical".PadRight(50), ije.HOSPFROM);
      Assert.Equal("Starr Hospital".PadRight(50), ije.HOSPTO);
      Assert.Equal("878787878787", br.FacilityNPI);
      Assert.Equal("6678", br.FacilityJFI);
      Assert.Equal("Bryant Medical", br.BirthFacilityName);
      Assert.Equal("Lennon Medical", br.FacilityMotherTransferredFrom);
      Assert.Equal("Starr Hospital", br.FacilityInfantTransferredTo);
    }

    [Fact]
    public void TestSetLocation()
    {
      IJEBirth ije = new()
      {
        FNPI = "25789",
        SFN = "1111",
        HOSP = "Griffin Hospital",
        HOSPFROM = "Taylor Hospital",
        HOSPTO = "Oswald Medical"
      };
      BirthRecord br = ije.ToRecord();
      Assert.Equal("25789".PadRight(12), ije.FNPI);
      Assert.Equal("1111", ije.SFN);
      Assert.Equal("Griffin Hospital".PadRight(50), ije.HOSP);
      Assert.Equal("Taylor Hospital".PadRight(50), ije.HOSPFROM);
      Assert.Equal("Oswald Medical".PadRight(50), ije.HOSPTO);
      Assert.Equal("25789", br.FacilityNPI);
      Assert.Equal("1111", br.FacilityJFI);
      Assert.Equal("Griffin Hospital", br.BirthFacilityName);
      Assert.Equal("Taylor Hospital", br.FacilityMotherTransferredFrom);
      Assert.Equal("Oswald Medical", br.FacilityInfantTransferredTo);
      ije.SFN = "55";
      ije.FNPI = "09870987";
      ije.HOSP = "Simpson Medical";
      ije.HOSPFROM = "Swanson Facility";
      ije.HOSPTO = "Jones Hospital";
      Assert.Equal("09870987".PadRight(12), ije.FNPI);
      Assert.Equal("55".PadRight(4), ije.SFN);
      Assert.Equal("Simpson Medical".PadRight(50), ije.HOSP);
      Assert.Equal("Swanson Facility".PadRight(50), ije.HOSPFROM);
      Assert.Equal("Jones Hospital".PadRight(50), ije.HOSPTO);
      Assert.Equal("09870987", br.FacilityNPI);
      Assert.Equal("55", br.FacilityJFI);
      Assert.Equal("Simpson Medical", br.BirthFacilityName);
      Assert.Equal("Swanson Facility", br.FacilityMotherTransferredFrom);
      Assert.Equal("Jones Hospital", br.FacilityInfantTransferredTo);
    }

    [Fact]
    public void TestLastMenses()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);
      Assert.Equal("    ", ije.DLMP_YR);
      Assert.Equal("  ", ije.DLMP_MO);
      Assert.Equal("  ", ije.DLMP_DY);
      ije.DLMP_DY = "24";
      Assert.Equal("    ", ije.DLMP_YR);
      Assert.Equal("  ", ije.DLMP_MO);
      Assert.Equal("24", ije.DLMP_DY);
      ije.DLMP_MO = "02";
      Assert.Equal("    ", ije.DLMP_YR);
      Assert.Equal("02", ije.DLMP_MO);
      Assert.Equal("24", ije.DLMP_DY);
      ije.DLMP_YR = "2023";
      Assert.Equal("2023", ije.DLMP_YR);
      Assert.Equal("02", ije.DLMP_MO);
      Assert.Equal("24", ije.DLMP_DY);
    }

    [Fact]
    public void TestMotherHeightPropertiesSetter()
    {
      BirthRecord record = new BirthRecord();
      IJEBirth ije1 = new IJEBirth(record);
      // Height
      Assert.Equal("99", ije1.HIN);
      Assert.Equal("9", ije1.HFT);
      ije1.HFT = "5";
      ije1.HIN = "7";
      Assert.Equal("07", ije1.HIN);
      Assert.Equal("5", ije1.HFT);
      ije1.HFT = "5";
      ije1.HIN = "3";
      Assert.Equal("5", ije1.HFT);
      Assert.Equal("03", ije1.HIN);
      // Edit Flag
      Assert.Equal("", ije1.HGT_BYPASS);
      ije1.HGT_BYPASS = "1";
      Assert.Equal("1", ije1.HGT_BYPASS);
      // FHIR translations
      BirthRecord record1 = new BirthRecord(ije1.ToRecord().ToXML());
      Assert.Equal(63, record1.MotherHeight);
      Assert.Equal(VR.ValueSets.EditBypass01234.Edit_Failed_Data_Queried_And_Verified, record1.MotherHeightEditFlag["code"]);
    }

    [Fact]
    public void TestFirstPrenatalCare()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);
      Assert.Equal("8888", ije.DOFP_YR);
      Assert.Equal("88", ije.DOFP_MO);
      Assert.Equal("88", ije.DOFP_DY);
      ije.DOFP_DY = "24";
      Assert.Equal("    ", ije.DOFP_YR);
      Assert.Equal("  ", ije.DOFP_MO);
      Assert.Equal("24", ije.DOFP_DY);
      ije.DOFP_MO = "02";
      Assert.Equal("    ", ije.DOFP_YR);
      Assert.Equal("02", ije.DOFP_MO);
      Assert.Equal("24", ije.DOFP_DY);
      ije.DOFP_YR = "2023";
      Assert.Equal("2023", ije.DOFP_YR);
      Assert.Equal("02", ije.DOFP_MO);
      Assert.Equal("24", ije.DOFP_DY);
    }

    [Fact]
    public void TestRegistrationDate()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);
      Assert.Equal("    ", ije.DOR_YR);
      Assert.Equal("  ", ije.DOR_MO);
      Assert.Equal("  ", ije.DOR_DY);
      ije.DOR_DY = "24";
      Assert.Equal("    ", ije.DOR_YR);
      Assert.Equal("  ", ije.DOR_MO);
      Assert.Equal("24", ije.DOR_DY);
      ije.DOR_MO = "02";
      Assert.Equal("    ", ije.DOR_YR);
      Assert.Equal("02", ije.DOR_MO);
      Assert.Equal("24", ije.DOR_DY);
      ije.DOR_YR = "2023";
      Assert.Equal("2023", ije.DOR_YR);
      Assert.Equal("02", ije.DOR_MO);
      Assert.Equal("24", ije.DOR_DY);
    }

    [Fact]
    public void TestSetPayorType()
    {
      // Manually set ije values.
      IJEBirth ije = new()
      {
        PAY = "1"
      };
      Assert.Equal("1", ije.PAY);
      Assert.Equal("2", ije.ToRecord().PayorTypeFinancialClass["code"]);
      Assert.Equal("MEDICAID", ije.ToRecord().PayorTypeFinancialClass["display"]);
      Assert.Equal(VR.CodeSystems.NAHDO, ije.ToRecord().PayorTypeFinancialClass["system"]);

      ije.PAY = "3";
      Assert.Equal("3", ije.PAY);
      Assert.Equal("81", ije.ToRecord().PayorTypeFinancialClass["code"]);
      Assert.Equal("Self-pay", ije.ToRecord().PayorTypeFinancialClass["display"]);
      Assert.Equal(VR.CodeSystems.NAHDO, ije.ToRecord().PayorTypeFinancialClass["system"]);
    }

    [Fact]
    public void TestImportPayorType()
    {
      // Test IJE import.
      IJEBirth ijeImported = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/BasicBirthRecord.ije")), true);
      // Test IJE conversion to BirthRecord.
      BirthRecord br = ijeImported.ToRecord();
      // Test IJE conversion from BirthRecord.
      IJEBirth ijeConverted = new(br);

      Assert.Equal("6", ijeImported.PAY);
      Assert.Equal(ijeImported.PAY, ijeConverted.PAY);
      Assert.Equal("38", ijeImported.ToRecord().PayorTypeFinancialClass["code"]);
      Assert.Equal("Other Government (Federal, State, Local not specified)", ijeImported.ToRecord().PayorTypeFinancialClass["display"]);
      Assert.Equal(VR.CodeSystems.NAHDO, ijeImported.ToRecord().PayorTypeFinancialClass["system"]);
    }

    [Fact]
    public void TestApgarScores()
    {
      BirthRecord fhir = new BirthRecord();
      IJEBirth ije = new IJEBirth(fhir);
      Assert.Equal("  ", ije.APGAR5);
      Assert.Equal("88", ije.APGAR10);
      ije.APGAR5 = "99";
      ije.APGAR10 = "15";
      Assert.Equal("99", ije.APGAR5);
      Assert.Equal(-1, fhir.ApgarScoreFiveMinutes);
      Assert.Equal("15", ije.APGAR10);
      Assert.Equal(15, fhir.ApgarScoreTenMinutes);
    }

    [Fact]
    public void TestIJERoundTrip()
    {
      BirthRecord fhir = new BirthRecord(File.ReadAllText("fixtures/json/BirthRecordFakeWithRace.json"));
      Assert.False(fhir.NoPregnancyRiskFactors); // if present, will cause IJE values to flip to N
      Assert.False(fhir.GestationalDiabetes); // should map to N
      Assert.True(fhir.GestationalHypertension); // should map to Y
      Assert.True(fhir.NoCongenitalAnomaliesOfTheNewborn); // should map to N
      Assert.False(fhir.Anencephaly); // would normally map to U, but NoCongenitalAnomaliesOfTheNewborn should flip IJE to N
      IJEBirth ije = new IJEBirth(fhir);
      Assert.Equal("N", ije.GDIAB);
      Assert.Equal("Y", ije.GHYPE);
      Assert.Equal("9", ije.HFT);
      Assert.Equal("99", ije.HIN);
      Assert.Equal("N", ije.ECVS);
      Assert.Equal("N", ije.ECVF);
      Assert.Equal("N", ije.ANEN);
      Assert.Equal("7134703", ije.INF_MED_REC_NUM.Trim());
      Assert.Equal("2286144", ije.MOM_MED_REC_NUM.Trim());
      IJEBirth ije2 = new IJEBirth(ije.ToString());
      Assert.Equal("N", ije2.GDIAB);
      Assert.Equal("Y", ije2.GHYPE);
      Assert.Equal("9", ije2.HFT);
      Assert.Equal("99", ije2.HIN);
      Assert.Equal("N", ije2.ECVS);
      Assert.Equal("N", ije2.ECVF);
      Assert.Equal("N", ije2.ANEN);
      Assert.Equal("7134703", ije2.INF_MED_REC_NUM.Trim());
      Assert.Equal("2286144", ije2.MOM_MED_REC_NUM.Trim());
      IJEBirth ije3 = new IJEBirth(new BirthRecord(ije2.ToRecord().ToXML()));
      Assert.Equal("N", ije3.GDIAB);
      Assert.Equal("Y", ije3.GHYPE);
      Assert.Equal("9", ije3.HFT);
      Assert.Equal("99", ije3.HIN);
      Assert.Equal("N", ije3.ECVS);
      Assert.Equal("N", ije3.ECVF);
      Assert.Equal("N", ije3.ANEN);
      Assert.Equal("7134703", ije3.INF_MED_REC_NUM.Trim());
      Assert.Equal("2286144", ije3.MOM_MED_REC_NUM.Trim());
    }

    [Fact]
    public void BlankEights()
    {
      IJEBirth ije = new()
      {
        YOPO = "2020",
        MOPO = "04",
        DOFP_DY = "05",
        DOFP_MO = "07",
        DOFP_YR = "2021",
        APGAR10 = "09",
        MLLB = "08",
        YLLB = "2017"
      };

      Assert.Equal("2020", ije.YOPO);
      Assert.Equal("04", ije.MOPO);
      Assert.Equal("09", ije.APGAR10);
      Assert.Equal("05", ije.DOFP_DY);
      Assert.Equal("07", ije.DOFP_MO);
      Assert.Equal("2021", ije.DOFP_YR);
      Assert.Equal("08", ije.MLLB);
      Assert.Equal("2017", ije.YLLB);

      ije.YOPO = "8888";
      ije.DOFP_DY = "88";
      ije.APGAR10 = "88";
      ije.MLLB = "88";
      Assert.Equal("8888", ije.YOPO);
      Assert.Null(ije.ToRecord().DateOfLastOtherPregnancyOutcomeYear);
      Assert.Equal("88", ije.MOPO);
      Assert.Null(ije.ToRecord().DateOfLastOtherPregnancyOutcomeMonth);
      Assert.Equal("88", ije.APGAR10);
      Assert.Null(ije.ToRecord().ApgarScoreTenMinutes);
      Assert.Equal("88", ije.DOFP_DY);
      Assert.Null(ije.ToRecord().FirstPrenatalCareVisitDay);
      Assert.Equal("88", ije.DOFP_MO);
      Assert.Null(ije.ToRecord().FirstPrenatalCareVisitMonth);
      Assert.Equal("8888", ije.DOFP_YR);
      Assert.Null(ije.ToRecord().FirstPrenatalCareVisitYear);
      Assert.Equal("88", ije.MLLB);
      Assert.Null(ije.ToRecord().DateOfLastLiveBirthMonth);
      Assert.Equal("8888", ije.YLLB);
      Assert.Null(ije.ToRecord().DateOfLastLiveBirthYear);
    }

    [Fact]
    public void SetVOID()
    {
      IJEBirth ije = new IJEBirth();
      Assert.Equal("0", ije.VOID);
      ije.VOID = "123";
      Assert.Equal("0", ije.VOID);
      ije.VOID = " ";
      Assert.Equal("0", ije.VOID);
      ije.VOID = "abc #$@";
      Assert.Equal("0", ije.VOID);
      ije.VOID = " 0 ";
      Assert.Equal("0", ije.VOID);
      ije.VOID = "0";
      Assert.Equal("0", ije.VOID);
      ije.VOID = " 1 ";
      Assert.Equal("1", ije.VOID);
      ije.VOID = "1";
      Assert.Equal("1", ije.VOID);
      ije.VOID = "2";
      Assert.Equal("0", ije.VOID);
    }
  }
}
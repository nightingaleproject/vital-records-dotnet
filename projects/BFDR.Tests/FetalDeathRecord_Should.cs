using System;
using System.Collections.Generic;
using System.IO;
using VR;
using Xunit;

namespace BFDR.Tests
{
  public class FetalDeathRecord_Should
  {
    private FetalDeathRecord SetterFetalDeathRecord;

    public FetalDeathRecord_Should()
    {
      SetterFetalDeathRecord = new FetalDeathRecord();
    }

    [Fact]
    public void TestFetalDeathIdentifierSetters()
    {
      // Record Identifiers
      SetterFetalDeathRecord.CertificateNumber = "87366";
      Assert.Equal("87366", SetterFetalDeathRecord.CertificateNumber);
      Assert.Equal("0000XX087366", SetterFetalDeathRecord.RecordIdentifier);
      Assert.Null(SetterFetalDeathRecord.StateLocalIdentifier1);
      SetterFetalDeathRecord.StateLocalIdentifier1 = "0000033";
      Assert.Equal("0000033", SetterFetalDeathRecord.StateLocalIdentifier1);
      // TODO: Delivery year is not implemented yet
      // SetterFetalDeathRecord.DeliveryYear = 2020;
      // SetterFetalDeathRecord.CertificateNumber = "767676";
      // Assert.Equal("2020XX767676", SetterFetalDeathRecord.RecordIdentifier);
      SetterFetalDeathRecord.BirthLocationJurisdiction = "WY";
      SetterFetalDeathRecord.CertificateNumber = "898989";
      Assert.Equal("0000WY898989", SetterFetalDeathRecord.RecordIdentifier);
    }

    [Fact]
    public void ParseRegistrationDate()
    { 
      FetalDeathRecord record = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("2019-01-09", record.RegistrationDate);
      Assert.Equal(2019, record.RegistrationDateYear);
      Assert.Equal(1, record.RegistrationDateMonth);
      Assert.Equal(9, record.RegistrationDateDay);

      IJEFetalDeath ije = new(record);
      Assert.Equal("2019", ije.DOR_YR);
      Assert.Equal("01", ije.DOR_MO);
      Assert.Equal("09", ije.DOR_DY);

      //set after parse
      record.FirstPrenatalCareVisit = "2024-05"; 
      Assert.Equal("2024-05", record.FirstPrenatalCareVisit);
      Assert.Equal(2024, record.FirstPrenatalCareVisitYear);
      Assert.Equal(5, record.FirstPrenatalCareVisitMonth);
      Assert.Null(record.FirstPrenatalCareVisitDay);
    }

    [Fact]
    public void BirthDataPresent()
    {
      FetalDeathRecord record = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      //maternal morbidity
      Assert.False(record.NoMaternalMorbidities);
      Assert.False(record.RupturedUterus); 
      Assert.False(record.ICUAdmission);
      //risk factors
      Assert.False(record.NoPregnancyRiskFactors);
      Assert.False(record.EclampsiaHypertension); 
      Assert.False(record.GestationalDiabetes); 
      Assert.False(record.GestationalHypertension); 
      Assert.False(record.PrepregnancyDiabetes); 
      Assert.False(record.PrepregnancyHypertension); 
      Assert.False(record.PreviousCesarean); 
      Assert.False(record.ArtificialInsemination); 
      Assert.False(record.AssistedFertilization); 
      Assert.False(record.InfertilityTreatment); 
      //set after parse
      record.NoMaternalMorbidities = true;
      Assert.True(record.NoMaternalMorbidities);
      record.EclampsiaHypertension = true;
      Assert.True(record.EclampsiaHypertension);
      record.ArtificialInsemination = true;
      Assert.True(record.ArtificialInsemination);
    }

    [Fact]
    public void SetNumberOfPreviousCesareans()
    {
      FetalDeathRecord deathRecord = new FetalDeathRecord();
      deathRecord.NumberOfPreviousCesareans = 2;
      Assert.Equal(2, deathRecord.NumberOfPreviousCesareans);

      IJEFetalDeath ije = new IJEFetalDeath();
      ije.NPCES = "1";
      FetalDeathRecord deathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal(1, deathRecord2.NumberOfPreviousCesareans);

      FetalDeathRecord deathRecord3 = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal(1, deathRecord3.NumberOfPreviousCesareans);
    }

    [Fact]
    public void SetNumberOfPreviousCesareansEditFlag()
    {
      FetalDeathRecord deathRecord = new FetalDeathRecord();
      deathRecord.NumberOfPreviousCesareansEditFlagHelper = "1failedVerified";
      Assert.Equal("1failedVerified", deathRecord.NumberOfPreviousCesareansEditFlagHelper);
      Dictionary<string, string> cc = new Dictionary<string, string>();
      cc.Add("code", "1failedVerified");
      cc.Add("system", "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-edit-flags");
      cc.Add("display", "Edit Failed, Verified");
      Assert.Equal(cc, deathRecord.NumberOfPreviousCesareansEditFlag);

      IJEFetalDeath ije = new IJEFetalDeath();
      ije.NPCES_BYPASS = "0";
      FetalDeathRecord deathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal("0", deathRecord2.NumberOfPreviousCesareansEditFlagHelper);

      FetalDeathRecord deathRecord3 = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Dictionary<string, string> cc2 = new Dictionary<string, string>();
      cc2.Add("code", "0");
      cc2.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");
      cc2.Add("display", "Edit Passed");
      Assert.Equal("0", deathRecord3.NumberOfPreviousCesareansEditFlagHelper);
      Assert.Equal(cc2, deathRecord3.NumberOfPreviousCesareansEditFlag);
    }
  }
}

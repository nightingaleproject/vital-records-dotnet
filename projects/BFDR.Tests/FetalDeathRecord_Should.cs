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
    public void SetAutopsyorHistologicalExamResultsUsed()
    {
      FetalDeathRecord fetalDeathRecord = new FetalDeathRecord();
      fetalDeathRecord.AutopsyorHistologicalExamResultsUsedHelper = "Y";
      Assert.Equal("Y", fetalDeathRecord.AutopsyorHistologicalExamResultsUsedHelper);
      Dictionary<string, string> cc = new Dictionary<string, string>();
      cc.Add("code", VR.ValueSets.YesNoNotApplicable.Codes[1, 0]);
      cc.Add("system", VR.ValueSets.YesNoNotApplicable.Codes[1, 2]);
      cc.Add("display", VR.ValueSets.YesNoNotApplicable.Codes[1, 1]);
      Assert.Equal(cc, fetalDeathRecord.AutopsyorHistologicalExamResultsUsed);

      IJEFetalDeath ije = new IJEFetalDeath();
      ije.AUTOPF = "N";
      FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal("N", fetalDeathRecord2.AutopsyorHistologicalExamResultsUsedHelper);

      FetalDeathRecord fetalDeathRecord3 = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicFetalDeathRecord.json")));
      Assert.Equal("Y", fetalDeathRecord3.AutopsyorHistologicalExamResultsUsedHelper);
      Assert.Equal(cc, fetalDeathRecord3.AutopsyorHistologicalExamResultsUsed);
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
    public void SetPhysicalDeliveryPlace()
    {
      SetterFetalDeathRecord.DeliveryPhysicalLocationHelper =  "22232009";

      IJEFetalDeath ije = new(SetterFetalDeathRecord);
      Assert.Equal("22232009", SetterFetalDeathRecord.DeliveryPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", SetterFetalDeathRecord.DeliveryPhysicalLocation["system"]);
      Assert.Equal("Hospital", SetterFetalDeathRecord.DeliveryPhysicalLocation["display"]);
      Assert.Equal(SetterFetalDeathRecord.DeliveryPhysicalLocationHelper, SetterFetalDeathRecord.DeliveryPhysicalLocation["code"]);
      Assert.Equal("1", ije.DPLACE);

      Dictionary<string, string> deliveryPlaceCode = new()
      {
          ["code"] = "22232009",
          ["system"] = "http://snomed.info/sct",
          ["display"] = "Hospital"
      };
      SetterFetalDeathRecord.DeliveryPhysicalLocation = deliveryPlaceCode;
      ije = new(SetterFetalDeathRecord);
      Assert.Equal("22232009", SetterFetalDeathRecord.DeliveryPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", SetterFetalDeathRecord.DeliveryPhysicalLocation["system"]);
      Assert.Equal("Hospital", SetterFetalDeathRecord.DeliveryPhysicalLocation["display"]);
      Assert.Equal(SetterFetalDeathRecord.DeliveryPhysicalLocationHelper, SetterFetalDeathRecord.DeliveryPhysicalLocation["code"]);
      Assert.Equal("1", ije.DPLACE);

      SetterFetalDeathRecord.DeliveryPhysicalLocationHelper = "67190003";
      ije = new(SetterFetalDeathRecord);
      Assert.Equal("67190003", SetterFetalDeathRecord.DeliveryPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", SetterFetalDeathRecord.DeliveryPhysicalLocation["system"]);
      Assert.Equal("Free-standing clinic", SetterFetalDeathRecord.DeliveryPhysicalLocation["display"]);
      Assert.Equal(SetterFetalDeathRecord.DeliveryPhysicalLocationHelper, SetterFetalDeathRecord.DeliveryPhysicalLocation["code"]);
      Assert.Equal("6", ije.DPLACE);
    }

    [Fact]
    public void ParsePhysicalDeliveryPlace()
    {
      // Test FHIR record import.
      FetalDeathRecord firstRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      // Test conversion via FromDescription.
      FetalDeathRecord secondRecord = VitalRecord.FromDescription<FetalDeathRecord>(firstRecord.ToDescription());

      Assert.Equal("22232009", firstRecord.DeliveryPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", firstRecord.DeliveryPhysicalLocation["system"]);
      Assert.Equal("Hospital", firstRecord.DeliveryPhysicalLocation["display"]);
      Assert.Equal(firstRecord.DeliveryPhysicalLocationHelper, firstRecord.DeliveryPhysicalLocation["code"]);
      Assert.Equal("22232009", secondRecord.DeliveryPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", secondRecord.DeliveryPhysicalLocation["system"]);
      Assert.Equal("Hospital", secondRecord.DeliveryPhysicalLocation["display"]);
      Assert.Equal(secondRecord.DeliveryPhysicalLocationHelper, secondRecord.DeliveryPhysicalLocation["code"]);
      //set after parse
      firstRecord.DeliveryPhysicalLocationHelper = "91154008";
      Assert.Equal("91154008", firstRecord.DeliveryPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", firstRecord.DeliveryPhysicalLocation["system"]);
      Assert.Equal("Free-standing birthing center", firstRecord.DeliveryPhysicalLocation["display"]);
    }

    [Fact]
    public void SetCertificationDate()
    {
      Assert.Null(SetterFetalDeathRecord.CertificationDate);
      Assert.Null(SetterFetalDeathRecord.CertifiedYear);
      Assert.Null(SetterFetalDeathRecord.CertifiedMonth);
      Assert.Null(SetterFetalDeathRecord.CertifiedDay);
      SetterFetalDeathRecord.CertificationDate = "2023-02";
      Assert.Equal("2023-02", SetterFetalDeathRecord.CertificationDate);
      Assert.Equal(2023, SetterFetalDeathRecord.CertifiedYear);
      Assert.Equal(2, SetterFetalDeathRecord.CertifiedMonth);
      SetterFetalDeathRecord.CertifiedYear = 2022;
      Assert.Equal("2022-02", SetterFetalDeathRecord.CertificationDate);
      Assert.Equal(2022, SetterFetalDeathRecord.CertifiedYear);
      Assert.Equal(2, SetterFetalDeathRecord.CertifiedMonth);
      SetterFetalDeathRecord.CertifiedDay = 3;
      Assert.Equal("2022-02-03", SetterFetalDeathRecord.CertificationDate);
      Assert.Equal(2022, SetterFetalDeathRecord.CertifiedYear);
      Assert.Equal(2, SetterFetalDeathRecord.CertifiedMonth);
      Assert.Equal(3, SetterFetalDeathRecord.CertifiedDay);
      SetterFetalDeathRecord.CertificationDate = null;
      Assert.Null(SetterFetalDeathRecord.CertificationDate);
      Assert.Null(SetterFetalDeathRecord.CertifiedYear);
      Assert.Null(SetterFetalDeathRecord.CertifiedMonth);
      Assert.Null(SetterFetalDeathRecord.CertifiedDay);
      SetterFetalDeathRecord.CertifiedMonth = 4;
      Assert.Null(SetterFetalDeathRecord.CertificationDate);
      Assert.Null(SetterFetalDeathRecord.CertifiedYear);
      Assert.Equal(4, SetterFetalDeathRecord.CertifiedMonth);
      Assert.Null(SetterFetalDeathRecord.CertifiedDay);
      // test IJE translations
      SetterFetalDeathRecord.CertificationDate = "2023-02-19";
      IJEFetalDeath ije1 = new(SetterFetalDeathRecord);
      Assert.Equal("2023", ije1.CERTIFIED_YR);
      Assert.Equal("02", ije1.CERTIFIED_MO);
      Assert.Equal("19", ije1.CERTIFIED_DY);
      FetalDeathRecord br2 = ije1.ToRecord();
      Assert.Equal("2023-02-19", br2.CertificationDate);
      Assert.Equal(2023, (int)br2.CertifiedYear);
      Assert.Equal(02, (int)br2.CertifiedMonth);
      Assert.Equal(19, (int)br2.CertifiedDay);
    }
      
    [Fact]
    public void SetEstimatedTimeOfFetalDeath()
    {
      SetterFetalDeathRecord.TimeOfFetalDeathHelper = "434671000124102";
      Assert.Equal("434671000124102", SetterFetalDeathRecord.TimeOfFetalDeathHelper);
      Assert.Equal("434671000124102", SetterFetalDeathRecord.TimeOfFetalDeath["code"]);
      Assert.Equal("http://snomed.info/sct", SetterFetalDeathRecord.TimeOfFetalDeath["system"]);
      Assert.Equal("L", new IJEFetalDeath(SetterFetalDeathRecord).ETIME);
      Dictionary<string, string> timeOfFetalDeath = new()
      {
          { "code", "434681000124104" },
          { "system", "http://snomed.info/sct" }
      };
      SetterFetalDeathRecord.TimeOfFetalDeath = timeOfFetalDeath;
      Assert.Equal("434681000124104", SetterFetalDeathRecord.TimeOfFetalDeathHelper);
      Assert.Equal("434681000124104", SetterFetalDeathRecord.TimeOfFetalDeath["code"]);
      Assert.Equal("http://snomed.info/sct", SetterFetalDeathRecord.TimeOfFetalDeath["system"]);
      Assert.Equal("N", new IJEFetalDeath(SetterFetalDeathRecord).ETIME);
      SetterFetalDeathRecord.TimeOfFetalDeathHelper = "UNK";
      Assert.Equal("UNK", SetterFetalDeathRecord.TimeOfFetalDeathHelper);
      Assert.Equal("UNK", SetterFetalDeathRecord.TimeOfFetalDeath["code"]);
      Assert.Equal("http://terminology.hl7.org/CodeSystem/v3-NullFlavor", SetterFetalDeathRecord.TimeOfFetalDeath["system"]);
      Assert.Equal("U", new IJEFetalDeath(SetterFetalDeathRecord).ETIME);
    }

    [Fact]
    public void ParseEstimatedTimeOfFetalDeath()
    {
      FetalDeathRecord record = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("434631000124100", record.TimeOfFetalDeathHelper);
      Assert.Equal("434631000124100", record.TimeOfFetalDeath["code"]);
      Assert.Equal("http://snomed.info/sct", record.TimeOfFetalDeath["system"]);
      Assert.Equal("A", new IJEFetalDeath(record).ETIME);
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

    [Fact]
    public void ParseFetalDeathCauseOrCondition()
    { 
      FetalDeathRecord record = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.True(record.PrematureRuptureOfMembranes);
      Assert.False(record.AbruptioPlacenta);
      Assert.False(record.PlacentalInsufficiency);
      Assert.False(record.ProlapsedCord);
      Assert.False(record.ChorioamnionitisCOD);
      Assert.False(record.OtherComplicationsOfPlacentaCordOrMembranes);
      Assert.False(record.InitiatingCauseOrConditionUnknown);
      Assert.Null(record.MaternalConditionsDiseasesLiteral);
      Assert.Null(record.OtherComplicationsOfPlacentaCordMembranesLiteral);
      Assert.Null(record.OtherObstetricalOrPregnancyComplicationsLiteral);
      Assert.Null(record.FetalAnomalyLiteral);
      Assert.Null(record.FetalInjuryLiteral);
      Assert.Null(record.FetalInfectionLiteral);
      Assert.Null(record.OtherFetalConditionsDisordersLiteral);

      //set after parse
      record.PrematureRuptureOfMembranes = false; 
      Assert.False(record.PrematureRuptureOfMembranes);
      record.ProlapsedCord = true; 
      Assert.True(record.ProlapsedCord);
      record.MaternalConditionsDiseasesLiteral = "Complication of Placenta Cord";
      Assert.Equal("Complication of Placenta Cord", record.MaternalConditionsDiseasesLiteral);
      record.OtherObstetricalOrPregnancyComplicationsLiteral = "Other Obstetrical Complication";
      Assert.Equal("Other Obstetrical Complication", record.OtherObstetricalOrPregnancyComplicationsLiteral);
      record.FetalInfectionLiteral = "Some Fetal Infection";
      Assert.Equal("Some Fetal Infection", record.FetalInfectionLiteral);

      IJEFetalDeath ije = new(record);
      Assert.Equal("N", ije.COD18a1);
      Assert.Equal("Y", ije.COD18a4);
      Assert.Equal("Complication of Placenta Cord", ije.COD18a8.Trim());
      Assert.Equal("Other Obstetrical Complication", ije.COD18a10.Trim());
      Assert.Equal("Some Fetal Infection", ije.COD18a13.Trim());
    }

    [Fact]
    public void Set_FetalDeathCauseOrCondition()
    {
      Assert.False(SetterFetalDeathRecord.PrematureRuptureOfMembranes);
      SetterFetalDeathRecord.PrematureRuptureOfMembranes = true;
      Assert.True(SetterFetalDeathRecord.PrematureRuptureOfMembranes);

      SetterFetalDeathRecord.AbruptioPlacenta = false;
      Assert.False(SetterFetalDeathRecord.AbruptioPlacenta);

      Assert.False(SetterFetalDeathRecord.PlacentalInsufficiency);
      SetterFetalDeathRecord.PlacentalInsufficiency = true;
      Assert.True(SetterFetalDeathRecord.PlacentalInsufficiency);

      SetterFetalDeathRecord.ProlapsedCord = true;
      Assert.True(SetterFetalDeathRecord.ProlapsedCord);

      SetterFetalDeathRecord.ChorioamnionitisCOD = true;
      Assert.True(SetterFetalDeathRecord.ChorioamnionitisCOD);

      Assert.False(SetterFetalDeathRecord.OtherComplicationsOfPlacentaCordOrMembranes);
      SetterFetalDeathRecord.OtherComplicationsOfPlacentaCordOrMembranes = true;
      Assert.True(SetterFetalDeathRecord.OtherComplicationsOfPlacentaCordOrMembranes);

      Assert.False(SetterFetalDeathRecord.InitiatingCauseOrConditionUnknown);
      SetterFetalDeathRecord.InitiatingCauseOrConditionUnknown = true;
      Assert.True(SetterFetalDeathRecord.InitiatingCauseOrConditionUnknown);

      Assert.Null(SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral);
      SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral = "Complication of Placenta Cord";
      Assert.Equal("Complication of Placenta Cord", SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral);

      Assert.Null(SetterFetalDeathRecord.OtherComplicationsOfPlacentaCordMembranesLiteral);
      SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral = "Other Complication of Placenta Cord";
      Assert.Equal("Other Complication of Placenta Cord", SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral);

      Assert.Null(SetterFetalDeathRecord.OtherObstetricalOrPregnancyComplicationsLiteral);
      SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral = "Some Obstetrical Complication";
      Assert.Equal("Some Obstetrical Complication", SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral);

      Assert.Null(SetterFetalDeathRecord.FetalAnomalyLiteral);
      SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral = "Some Fetal Anomaly";
      Assert.Equal("Some Fetal Anomaly", SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral);

      Assert.Null(SetterFetalDeathRecord.FetalInjuryLiteral);
      SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral = "Some Fetal Injury";
      Assert.Equal("Some Fetal Injury", SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral);

      Assert.Null(SetterFetalDeathRecord.FetalInfectionLiteral);
      SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral = "Some Fetal Infection";
      Assert.Equal("Some Fetal Infection", SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral); 

      Assert.Null(SetterFetalDeathRecord.OtherFetalConditionsDisordersLiteral);
      SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral = "Other Fetal Condition or Disorder";
      Assert.Equal("Other Fetal Condition or Disorder", SetterFetalDeathRecord.MaternalConditionsDiseasesLiteral);
    }
  }
}

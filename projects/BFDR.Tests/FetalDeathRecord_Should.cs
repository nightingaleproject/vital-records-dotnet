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
    private FetalDeathRecord BasicFetalDeathRecord;

    public FetalDeathRecord_Should()
    {
      SetterFetalDeathRecord = new FetalDeathRecord();
      BasicFetalDeathRecord = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicFetalDeathRecord.json")));
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
        SetterFetalDeathRecord.AutopsyorHistologicalExamResultsUsedHelper = "Y";
        Assert.Equal("Y", SetterFetalDeathRecord.AutopsyorHistologicalExamResultsUsedHelper);
        Dictionary<string, string> cc = new Dictionary<string, string>();
        cc.Add("code", VR.ValueSets.YesNoNotApplicable.Codes[1, 0]);
        cc.Add("system", VR.ValueSets.YesNoNotApplicable.Codes[1, 2]);
        cc.Add("display", VR.ValueSets.YesNoNotApplicable.Codes[1, 1]);
        Assert.Equal(cc, SetterFetalDeathRecord.AutopsyorHistologicalExamResultsUsed);

        IJEFetalDeath ije = new IJEFetalDeath();
        ije.AUTOPF = "N";
        FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
        Assert.Equal("N", fetalDeathRecord2.AutopsyorHistologicalExamResultsUsedHelper);
    }

    [Fact]
    public void Get_AutopsyorHistologicalExamResultsUsed()
    {
        Dictionary<string, string> cc = new Dictionary<string, string>();
        cc.Add("code", VR.ValueSets.YesNoNotApplicable.Codes[1, 0]);
        cc.Add("system", VR.ValueSets.YesNoNotApplicable.Codes[1, 2]);
        cc.Add("display", VR.ValueSets.YesNoNotApplicable.Codes[1, 1]);
        Assert.Equal("Y", BasicFetalDeathRecord.AutopsyorHistologicalExamResultsUsedHelper);
        Assert.Equal(cc, BasicFetalDeathRecord.AutopsyorHistologicalExamResultsUsed);
        Assert.Equal(VR.CodeSystems.YesNo, BasicFetalDeathRecord.AutopsyorHistologicalExamResultsUsed["system"]);
        Assert.Equal("Yes", BasicFetalDeathRecord.AutopsyorHistologicalExamResultsUsed["display"]);
    }

    [Fact]
    public void Set_AutopsyPerformedIndicator()
    {
        Dictionary<string, string> cc = new Dictionary<string, string>();
        cc.Add("code", VR.ValueSets.YesNoUnknown.Codes[1, 0]);
        cc.Add("system", VR.ValueSets.YesNoUnknown.Codes[1, 2]);
        cc.Add("display", VR.ValueSets.YesNoUnknown.Codes[1, 1]);
        SetterFetalDeathRecord.AutopsyPerformedIndicator = cc;
        Assert.Equal("Y", SetterFetalDeathRecord.AutopsyPerformedIndicator["code"]);
        Assert.Equal(VR.CodeSystems.YesNo, SetterFetalDeathRecord.AutopsyPerformedIndicator["system"]);
        Assert.Equal("Yes", SetterFetalDeathRecord.AutopsyPerformedIndicator["display"]);
        Assert.Equal("Y", SetterFetalDeathRecord.AutopsyPerformedIndicatorHelper);
        SetterFetalDeathRecord.AutopsyPerformedIndicatorHelper = "N";
        Assert.Equal("N", SetterFetalDeathRecord.AutopsyPerformedIndicator["code"]);
        Assert.Equal(VR.CodeSystems.YesNo, SetterFetalDeathRecord.AutopsyPerformedIndicator["system"]);
        Assert.Equal("No", SetterFetalDeathRecord.AutopsyPerformedIndicator["display"]);
        Assert.Equal("N", SetterFetalDeathRecord.AutopsyPerformedIndicatorHelper);
        SetterFetalDeathRecord.AutopsyPerformedIndicatorHelper = "UNK";
        Assert.Equal("UNK", SetterFetalDeathRecord.AutopsyPerformedIndicator["code"]);
        Assert.Equal(VR.CodeSystems.NullFlavor_HL7_V3, SetterFetalDeathRecord.AutopsyPerformedIndicator["system"]);
        Assert.Equal("unknown", SetterFetalDeathRecord.AutopsyPerformedIndicator["display"]);
        Assert.Equal("UNK", SetterFetalDeathRecord.AutopsyPerformedIndicatorHelper);

        IJEFetalDeath ije = new IJEFetalDeath();
        ije.AUTOP = "N";
        FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
        Assert.Equal("N", fetalDeathRecord2.AutopsyPerformedIndicatorHelper);
    }

    [Fact]
    public void Get_AutopsyPerformedIndicator()
    {
        Assert.Equal(VR.ValueSets.YesNoUnknown.Yes, BasicFetalDeathRecord.AutopsyPerformedIndicatorHelper);
        Assert.Equal("Y", BasicFetalDeathRecord.AutopsyPerformedIndicator["code"]);
        Assert.Equal(VR.CodeSystems.YesNo, BasicFetalDeathRecord.AutopsyPerformedIndicator["system"]);
        Assert.Equal("Yes", BasicFetalDeathRecord.AutopsyPerformedIndicator["display"]);
    }

    [Fact]
    public void Set_BirthWeight()
    {
        // Birth Weight
        Assert.Null(SetterFetalDeathRecord.BirthWeight);
        SetterFetalDeathRecord.BirthWeight = 3333;
        Assert.Equal(3333, SetterFetalDeathRecord.BirthWeight);
        // Edit Flag
        Assert.Null(SetterFetalDeathRecord.MotherDateOfBirthEditFlagHelper);
        Assert.Equal("", SetterFetalDeathRecord.BirthWeightEditFlag["code"]);
        SetterFetalDeathRecord.BirthWeightEditFlagHelper = BFDR.ValueSets.BirthWeightEditFlags.Off;
        Assert.Equal(BFDR.ValueSets.BirthWeightEditFlags.Off, SetterFetalDeathRecord.BirthWeightEditFlag["code"]);
        // IJE translations
        IJEFetalDeath ije = new IJEFetalDeath(SetterFetalDeathRecord);
        Assert.Equal("3333", ije.FWG);
        Assert.Equal(BFDR.ValueSets.BirthWeightEditFlags.Off, ije.FW_BYPASS);
        ije.FW_BYPASS = "2failedBirthWeightGestationEdit";
        Assert.Equal("2failedBirthWeightGestationEdit", ije.FW_BYPASS); 
    }  

    [Fact]
    public void Get_BirthWeight()
    {
        Assert.Equal(3333, BasicFetalDeathRecord.BirthWeight);
        Assert.Equal(BFDR.ValueSets.BirthWeightEditFlags.Off, BasicFetalDeathRecord.BirthWeightEditFlagHelper);
        Assert.Equal(BFDR.ValueSets.BirthWeightEditFlags.Off, BasicFetalDeathRecord.BirthWeightEditFlag["code"]);
        Assert.Equal(VR.CodeSystems.VRCLEditFlags, BasicFetalDeathRecord.BirthWeightEditFlag["system"]);
        Assert.Equal("Off", BasicFetalDeathRecord.BirthWeightEditFlag["display"]); 
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
    public void TestCigarettesSmoked()
    {
      FetalDeathRecord fetalDeathRecord = new FetalDeathRecord();
      fetalDeathRecord.CigarettesPerDayInThreeMonthsPriorToPregancy = 22;
      Assert.Equal(22, fetalDeathRecord.CigarettesPerDayInThreeMonthsPriorToPregancy);
      fetalDeathRecord.CigarettesPerDayInFirstTrimester = 4;
      Assert.Equal(4, fetalDeathRecord.CigarettesPerDayInFirstTrimester);
      fetalDeathRecord.CigarettesPerDayInSecondTrimester = 2;
      Assert.Equal(2, fetalDeathRecord.CigarettesPerDayInSecondTrimester);
      fetalDeathRecord.CigarettesPerDayInLastTrimester = 1;
      Assert.Equal(1, fetalDeathRecord.CigarettesPerDayInLastTrimester);

      //ije translations
      IJEFetalDeath ije = new IJEFetalDeath(fetalDeathRecord);
      Assert.Equal("22", ije.CIGPN);
      Assert.Equal("04", ije.CIGFN);
      Assert.Equal("02", ije.CIGSN);
      Assert.Equal("01", ije.CIGLN);
      FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal(22, fetalDeathRecord2.CigarettesPerDayInThreeMonthsPriorToPregancy);
      Assert.Equal(4, fetalDeathRecord2.CigarettesPerDayInFirstTrimester);
      Assert.Equal(2, fetalDeathRecord2.CigarettesPerDayInSecondTrimester);
      Assert.Equal(1, fetalDeathRecord2.CigarettesPerDayInLastTrimester);

      //parse
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal(0, record.CigarettesPerDayInThreeMonthsPriorToPregancy);
      Assert.Equal(0, record.CigarettesPerDayInFirstTrimester);
      Assert.Equal(1, record.CigarettesPerDayInSecondTrimester);
      Assert.Equal(0, record.CigarettesPerDayInLastTrimester);
      //set after parse
      record.CigarettesPerDayInThreeMonthsPriorToPregancy = 21;
      record.CigarettesPerDayInFirstTrimester = 4;
      Assert.Equal(21, record.CigarettesPerDayInThreeMonthsPriorToPregancy);
      Assert.Equal(4, record.CigarettesPerDayInFirstTrimester);
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
      SetterFetalDeathRecord.OtherComplicationsOfPlacentaCordMembranesLiteral = "Other Complication of Placenta Cord";
      Assert.Equal("Other Complication of Placenta Cord", SetterFetalDeathRecord.OtherComplicationsOfPlacentaCordMembranesLiteral);

      Assert.Null(SetterFetalDeathRecord.OtherObstetricalOrPregnancyComplicationsLiteral);
      SetterFetalDeathRecord.OtherObstetricalOrPregnancyComplicationsLiteral = "Some Obstetrical Complication";
      Assert.Equal("Some Obstetrical Complication", SetterFetalDeathRecord.OtherObstetricalOrPregnancyComplicationsLiteral);

      Assert.Null(SetterFetalDeathRecord.FetalAnomalyLiteral);
      SetterFetalDeathRecord.FetalAnomalyLiteral = "Some Fetal Anomaly";
      Assert.Equal("Some Fetal Anomaly", SetterFetalDeathRecord.FetalAnomalyLiteral);

      Assert.Null(SetterFetalDeathRecord.FetalInjuryLiteral);
      SetterFetalDeathRecord.FetalInjuryLiteral = "Some Fetal Injury";
      Assert.Equal("Some Fetal Injury", SetterFetalDeathRecord.FetalInjuryLiteral);

      Assert.Null(SetterFetalDeathRecord.FetalInfectionLiteral);
      SetterFetalDeathRecord.FetalInfectionLiteral = "Some Fetal Infection";
      Assert.Equal("Some Fetal Infection", SetterFetalDeathRecord.FetalInfectionLiteral); 

      Assert.Null(SetterFetalDeathRecord.OtherFetalConditionsDisordersLiteral);
      SetterFetalDeathRecord.OtherFetalConditionsDisordersLiteral = "Other Fetal Condition or Disorder";
      Assert.Equal("Other Fetal Condition or Disorder", SetterFetalDeathRecord.OtherFetalConditionsDisordersLiteral);
    }

    [Fact]
    public void ParseOtherFetalDeathCauseOrCondition()
    { 
      FetalDeathRecord record = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.False(record.OtherCOD_PrematureRuptureOfMembranes);
      Assert.False(record.OtherCOD_AbruptioPlacenta);
      Assert.True(record.OtherCOD_PlacentalInsufficiency);
      Assert.False(record.OtherCOD_ProlapsedCord);
      Assert.False(record.OtherCOD_ChorioamnionitisCOD);
      Assert.False(record.OtherCOD_OtherComplicationsOfPlacentaCordOrMembranes);
      Assert.False(record.OtherCOD_OtherCauseOrConditionUnknown);
      Assert.Null(record.OtherCOD_MaternalConditionsDiseasesLiteral);
      Assert.Null(record.OtherCOD_OtherComplicationsOfPlacentaCordMembranesLiteral);
      Assert.Null(record.OtherCOD_OtherObstetricalOrPregnancyComplicationsLiteral);
      Assert.Null(record.OtherCOD_FetalAnomalyLiteral);
      Assert.Null(record.OtherCOD_FetalInjuryLiteral);
      Assert.Null(record.OtherCOD_FetalInfectionLiteral);
      Assert.Null(record.OtherCOD_OtherFetalConditionsDisordersLiteral);

      //set after parse
      record.OtherCOD_PlacentalInsufficiency = false; 
      Assert.False(record.OtherCOD_PlacentalInsufficiency);
      record.OtherCOD_ProlapsedCord = true; 
      Assert.True(record.OtherCOD_ProlapsedCord);
      record.OtherCOD_OtherFetalConditionsDisordersLiteral = "Some other fetal condition disorder";
      Assert.Equal("Some other fetal condition disorder", record.OtherCOD_OtherFetalConditionsDisordersLiteral);
      record.OtherCOD_OtherComplicationsOfPlacentaCordMembranesLiteral = "Other Placenta Cord Membranes Complication";
      Assert.Equal("Other Placenta Cord Membranes Complication", record.OtherCOD_OtherComplicationsOfPlacentaCordMembranesLiteral);
      record.OtherCOD_FetalInjuryLiteral = "Some Fetal Injury";
      Assert.Equal("Some Fetal Injury", record.OtherCOD_FetalInjuryLiteral);

      IJEFetalDeath ije = new(record);
      Assert.Equal("N", ije.COD18b3);
      Assert.Equal("Y", ije.COD18b4);
      Assert.Equal("Some other fetal condition disorder", ije.COD18b14.Trim());
      Assert.Equal("Other Placenta Cord Membranes Complication", ije.COD18b9.Trim());
      Assert.Equal("Some Fetal Injury", ije.COD18b12.Trim());
    }

    [Fact]
    public void Set_OtherFetalDeathCauseOrCondition()
    {
      Assert.False(SetterFetalDeathRecord.OtherCOD_PrematureRuptureOfMembranes);
      SetterFetalDeathRecord.OtherCOD_PrematureRuptureOfMembranes = true;
      Assert.True(SetterFetalDeathRecord.OtherCOD_PrematureRuptureOfMembranes);

      SetterFetalDeathRecord.OtherCOD_AbruptioPlacenta = false;
      Assert.False(SetterFetalDeathRecord.OtherCOD_AbruptioPlacenta);

      Assert.False(SetterFetalDeathRecord.OtherCOD_PlacentalInsufficiency);
      SetterFetalDeathRecord.OtherCOD_PlacentalInsufficiency = true;
      Assert.True(SetterFetalDeathRecord.OtherCOD_PlacentalInsufficiency);

      SetterFetalDeathRecord.OtherCOD_ProlapsedCord = true;
      Assert.True(SetterFetalDeathRecord.OtherCOD_ProlapsedCord);

      SetterFetalDeathRecord.OtherCOD_ChorioamnionitisCOD = true;
      Assert.True(SetterFetalDeathRecord.OtherCOD_ChorioamnionitisCOD);

      Assert.False(SetterFetalDeathRecord.OtherCOD_OtherComplicationsOfPlacentaCordOrMembranes);
      SetterFetalDeathRecord.OtherCOD_OtherComplicationsOfPlacentaCordOrMembranes = true;
      Assert.True(SetterFetalDeathRecord.OtherCOD_OtherComplicationsOfPlacentaCordOrMembranes);

      Assert.False(SetterFetalDeathRecord.OtherCOD_OtherCauseOrConditionUnknown);
      SetterFetalDeathRecord.OtherCOD_OtherCauseOrConditionUnknown = true;
      Assert.True(SetterFetalDeathRecord.OtherCOD_OtherCauseOrConditionUnknown);

      Assert.Null(SetterFetalDeathRecord.OtherCOD_MaternalConditionsDiseasesLiteral);
      SetterFetalDeathRecord.OtherCOD_MaternalConditionsDiseasesLiteral = "Complication of Placenta Cord";
      Assert.Equal("Complication of Placenta Cord", SetterFetalDeathRecord.OtherCOD_MaternalConditionsDiseasesLiteral);

      Assert.Null(SetterFetalDeathRecord.OtherCOD_OtherComplicationsOfPlacentaCordMembranesLiteral);
      SetterFetalDeathRecord.OtherCOD_OtherComplicationsOfPlacentaCordMembranesLiteral = "Other Complication of Placenta Cord";
      Assert.Equal("Other Complication of Placenta Cord", SetterFetalDeathRecord.OtherCOD_OtherComplicationsOfPlacentaCordMembranesLiteral);

      Assert.Null(SetterFetalDeathRecord.OtherCOD_OtherObstetricalOrPregnancyComplicationsLiteral);
      SetterFetalDeathRecord.OtherCOD_OtherObstetricalOrPregnancyComplicationsLiteral = "Some Obstetrical Complication";
      Assert.Equal("Some Obstetrical Complication", SetterFetalDeathRecord.OtherCOD_OtherObstetricalOrPregnancyComplicationsLiteral);

      Assert.Null(SetterFetalDeathRecord.OtherCOD_FetalAnomalyLiteral);
      SetterFetalDeathRecord.OtherCOD_FetalAnomalyLiteral = "Some Fetal Anomaly";
      Assert.Equal("Some Fetal Anomaly", SetterFetalDeathRecord.OtherCOD_FetalAnomalyLiteral);

      Assert.Null(SetterFetalDeathRecord.OtherCOD_FetalInjuryLiteral);
      SetterFetalDeathRecord.OtherCOD_FetalInjuryLiteral = "Some Fetal Injury";
      Assert.Equal("Some Fetal Injury", SetterFetalDeathRecord.OtherCOD_FetalInjuryLiteral);

      Assert.Null(SetterFetalDeathRecord.OtherCOD_FetalInfectionLiteral);
      SetterFetalDeathRecord.OtherCOD_FetalInfectionLiteral = "Some Fetal Infection";
      Assert.Equal("Some Fetal Infection", SetterFetalDeathRecord.OtherCOD_FetalInfectionLiteral); 

      Assert.Null(SetterFetalDeathRecord.OtherCOD_OtherFetalConditionsDisordersLiteral);
      SetterFetalDeathRecord.OtherCOD_OtherFetalConditionsDisordersLiteral = "Other Fetal Condition or Disorder";
      Assert.Equal("Other Fetal Condition or Disorder", SetterFetalDeathRecord.OtherCOD_OtherFetalConditionsDisordersLiteral);
    }

    [Fact]
    public void TestImportDeliveryLocation()
    {
      FetalDeathRecord record = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("116441967701", record.FacilityNPI);
      Assert.Equal("UT12", record.FacilityJFI);
      Assert.Equal("South Hospital", record.BirthFacilityName);
      Assert.Null(record.FacilityMotherTransferredFrom);
      Assert.Equal("", record.PlaceOfDelivery["addressStnum"]);
      Assert.Equal("", record.PlaceOfDelivery["addressPredir"]);
      Assert.Equal("", record.PlaceOfDelivery["addressStname"]);
      Assert.Equal("", record.PlaceOfDelivery["addressStdesig"]);
      Assert.Equal("", record.PlaceOfDelivery["addressPostdir"]);
      Assert.Equal("", record.PlaceOfDelivery["addressUnitnum"]);
      Assert.Equal("2100 North Ave", record.PlaceOfDelivery["addressLine1"]);
      Assert.Equal("84116", record.PlaceOfDelivery["addressZip"]);
      Assert.Equal("Made Up", record.PlaceOfDelivery["addressCounty"]);
      Assert.Equal("Salt Lake City", record.PlaceOfDelivery["addressCity"]);
      Assert.Equal("UT", record.PlaceOfDelivery["addressState"]);
      Assert.Equal("US", record.PlaceOfDelivery["addressCountry"]);

      IJEFetalDeath ije = new(record);
      Assert.Equal("2100 North Ave", ije.ADDRESS_D.Trim());
      Assert.Equal("84116", ije.ZIPCODE_D.Trim());
      Assert.Equal("Made Up", ije.CNTY_D.Trim());
      Assert.Equal("Salt Lake City", ije.CITY_D.Trim());
      Assert.Equal("Utah", ije.STATE_D);
      Assert.Equal("United States", ije.COUNTRY_D);

      //set after parse
      Dictionary<string, string> addr = new Dictionary<string, string>();
      addr["addressState"] = "MA";
      addr["addressCounty"] = "Middlesex";
      addr["addressCity"] = "Bedford";
      record.PlaceOfDelivery = addr;
      Assert.Equal("MA", record.PlaceOfDelivery["addressState"]);
      Assert.Equal("Middlesex", record.PlaceOfDelivery["addressCounty"]);
      Assert.Equal("Bedford", record.PlaceOfDelivery["addressCity"]);
    }

    [Fact]
    public void TestSetDeliveryLocation()
    {
      BirthRecord br = new()
      {
          FacilityNPI = "4815162342",
          FacilityJFI = "636",
          BirthFacilityName = "Lahey Hospital",
          FacilityMotherTransferredFrom = "Sunnyvale Medical",
      };
      Assert.Equal("4815162342", br.FacilityNPI);
      Assert.Equal("636", br.FacilityJFI);
      Assert.Equal("Lahey Hospital", br.BirthFacilityName);
      Assert.Equal("Sunnyvale Medical", br.FacilityMotherTransferredFrom);
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
      Assert.Equal("999", br.FacilityNPI);
      Assert.Equal("0909", br.FacilityJFI);
      Assert.Equal("Bob's Medical Center", br.BirthFacilityName);
      Assert.Equal("Abignale Hospital", br.FacilityMotherTransferredFrom);
    }

    [Fact]
    public void ParseMotherRaceEthnicityJsonToIJE()
    {
        FetalDeathRecord b3 = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
        IJEFetalDeath ije3 = new IJEFetalDeath(b3);
        Assert.Equal("H", ije3.METHNIC1);
        Assert.Equal("H", ije3.METHNIC2);
        Assert.Equal("H", ije3.METHNIC3);
        Assert.Equal("N", ije3.METHNIC4);
        Assert.Equal("Malaysian", ije3.MRACE18);
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
        FetalDeathRecord b = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
        IJEFetalDeath ije1 = new IJEFetalDeath(b);
        IJEFetalDeath ije2 = new IJEFetalDeath(ije1.ToString(), true);
        FetalDeathRecord b2 = ije2.ToRecord();

        // Ethnicity tuple
        Assert.Equal("Y", b2.MotherEthnicity1Helper);
        Assert.Equal("Y", b2.MotherEthnicity2Helper);
        Assert.Equal("Y", b2.MotherEthnicity3Helper);
        Assert.Equal("N", b2.MotherEthnicity4Helper);
        Assert.Null(b2.MotherEthnicityLiteral);

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
        Assert.Equal(17, b2.MotherRace.Length);
    }

    [Fact]
    public void ParseFatherRaceEthnicityJsonToIJE()
    {
        FetalDeathRecord b3 = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
        IJEFetalDeath ije3 = new IJEFetalDeath(b3);
        Assert.Equal("H", ije3.FETHNIC1);
        Assert.Equal("H", ije3.FETHNIC2);
        Assert.Equal("H", ije3.FETHNIC3);
        Assert.Equal("N", ije3.FETHNIC4);
        Assert.Equal("Malaysian", ije3.FRACE18);
        Assert.Equal("Y", ije3.FRACE1);
        Assert.Equal("N", ije3.FRACE2);
        Assert.Equal("Y", ije3.FRACE3);
        Assert.Equal("N", ije3.FRACE4);
        Assert.Equal("N", ije3.FRACE5);
        Assert.Equal("N", ije3.FRACE6);
        Assert.Equal("N", ije3.FRACE7);
        Assert.Equal("N", ije3.FRACE8);
        Assert.Equal("N", ije3.FRACE9);
        Assert.Equal("Y", ije3.FRACE10);
        Assert.Equal("N", ije3.FRACE11);
        Assert.Equal("N", ije3.FRACE12);
        Assert.Equal("N", ije3.FRACE13);
        Assert.Equal("N", ije3.FRACE14);
        Assert.Equal("N", ije3.FRACE15);
    }

    [Fact]
    public void ParseFatherRaceEthnicityIJEtoJson()
    {
        FetalDeathRecord b = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
        IJEFetalDeath ije1 = new IJEFetalDeath(b);
        IJEFetalDeath ije2 = new IJEFetalDeath(ije1.ToString(), true);
        FetalDeathRecord b2 = ije2.ToRecord();

        // Ethnicity tuple
        Assert.Equal("Y", b2.FatherEthnicity1Helper);
        Assert.Equal("Y", b2.FatherEthnicity2Helper);
        Assert.Equal("Y", b2.FatherEthnicity3Helper);
        Assert.Equal("N", b2.FatherEthnicity4Helper);
        Assert.Null(b2.FatherEthnicityLiteral);

        // Race tuple
        foreach (var pair in b2.FatherRace)
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
        Assert.Equal(17, b2.FatherRace.Length);
    }
  }
}

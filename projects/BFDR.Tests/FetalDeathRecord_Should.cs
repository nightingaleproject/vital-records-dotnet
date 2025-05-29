using System;
using System.Collections.Generic;
using System.IO;
using VR;
using Xunit;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

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
        SetterFetalDeathRecord.DateOfDelivery = "2020";
        // SetterFetalDeathRecord.CertificateNumber = "767676";
        // Assert.Equal("2020XX767676", SetterFetalDeathRecord.RecordIdentifier);
        // Is the field name EventLocationJurisdiction for Fetal Death?
        SetterFetalDeathRecord.EventLocationJurisdiction = "WY";
        SetterFetalDeathRecord.CertificateNumber = "898989";
        Assert.Equal("2020WY898989", SetterFetalDeathRecord.RecordIdentifier);
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
    public void GetAutopsyorHistologicalExamResultsUsed()
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
    public void SetAutopsyPerformedIndicator()
    {
        Dictionary<string, string> cc = new Dictionary<string, string>();
        cc.Add("code", BFDR.ValueSets.PerformedNotPerformedPlanned.Codes[1, 0]);
        cc.Add("system", BFDR.ValueSets.PerformedNotPerformedPlanned.Codes[1, 2]);
        cc.Add("display", BFDR.ValueSets.PerformedNotPerformedPlanned.Codes[1, 1]);
        SetterFetalDeathRecord.AutopsyPerformedIndicator = cc;
        Assert.Equal("262008008", SetterFetalDeathRecord.AutopsyPerformedIndicator["code"]);
        Assert.Equal(VR.CodeSystems.SCT, SetterFetalDeathRecord.AutopsyPerformedIndicator["system"]);
        Assert.Equal("Not Performed", SetterFetalDeathRecord.AutopsyPerformedIndicator["display"]);
        Assert.Equal("262008008", SetterFetalDeathRecord.AutopsyPerformedIndicatorHelper);
        SetterFetalDeathRecord.AutopsyPerformedIndicatorHelper = "398166005";
        Assert.Equal("398166005", SetterFetalDeathRecord.AutopsyPerformedIndicator["code"]);
        Assert.Equal(VR.CodeSystems.SCT, SetterFetalDeathRecord.AutopsyPerformedIndicator["system"]);
        Assert.Equal("Performed", SetterFetalDeathRecord.AutopsyPerformedIndicator["display"]);
        Assert.Equal("398166005", SetterFetalDeathRecord.AutopsyPerformedIndicatorHelper);
        SetterFetalDeathRecord.AutopsyPerformedIndicatorHelper = "397943006";
        Assert.Equal("397943006", SetterFetalDeathRecord.AutopsyPerformedIndicator["code"]);
        Assert.Equal(VR.CodeSystems.SCT, SetterFetalDeathRecord.AutopsyPerformedIndicator["system"]);
        Assert.Equal("Planned", SetterFetalDeathRecord.AutopsyPerformedIndicator["display"]);
        Assert.Equal("397943006", SetterFetalDeathRecord.AutopsyPerformedIndicatorHelper);

        IJEFetalDeath ije = new IJEFetalDeath();
        ije.AUTOP = "N";
        FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
        Assert.Equal("262008008", fetalDeathRecord2.AutopsyPerformedIndicatorHelper);
    }

    [Fact]
    public void GetAutopsyPerformedIndicator()
    {
        Assert.Equal(BFDR.ValueSets.PerformedNotPerformedPlanned.Performed, BasicFetalDeathRecord.AutopsyPerformedIndicatorHelper);
        Assert.Equal("398166005", BasicFetalDeathRecord.AutopsyPerformedIndicator["code"]);
        Assert.Equal(VR.CodeSystems.SCT, BasicFetalDeathRecord.AutopsyPerformedIndicator["system"]);
        Assert.Equal("Performed", BasicFetalDeathRecord.AutopsyPerformedIndicator["display"]);
    }

    [Fact]
    public void Set_HistologicalPlacentalExamination()
    {
        SetterFetalDeathRecord.HistologicalPlacentalExaminationPerformedHelper = "398166005";
        Assert.Equal("Performed", SetterFetalDeathRecord.HistologicalPlacentalExaminationPerformed["display"]);
        Dictionary<string, string> cc = new Dictionary<string, string>();
        cc.Add("code", BFDR.ValueSets.PerformedNotPerformedPlanned.Codes[0, 0]);
        cc.Add("system", BFDR.ValueSets.PerformedNotPerformedPlanned.Codes[0, 2]);
        cc.Add("display", BFDR.ValueSets.PerformedNotPerformedPlanned.Codes[0, 1]);
        Assert.Equal(cc, SetterFetalDeathRecord.HistologicalPlacentalExaminationPerformed);

        IJEFetalDeath ije = new IJEFetalDeath();
        ije.HISTOP = "N";
        Assert.Equal("N", ije.HISTOP);
        FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
        Assert.Equal("262008008", fetalDeathRecord2.HistologicalPlacentalExaminationPerformedHelper);
        Assert.Equal("Not Performed", fetalDeathRecord2.HistologicalPlacentalExaminationPerformed["display"]);
    }

    [Fact]
    public void Get_HistologicalPlacentalExamination()
    {
        FetalDeathRecord record = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
        Dictionary<string, string> cc = new Dictionary<string, string>();
        cc.Add("code", BFDR.ValueSets.PerformedNotPerformedPlanned.Codes[0, 0]);
        cc.Add("system", BFDR.ValueSets.PerformedNotPerformedPlanned.Codes[0, 2]);
        cc.Add("display", BFDR.ValueSets.PerformedNotPerformedPlanned.Codes[0, 1]);
        Assert.Equal("398166005", record.HistologicalPlacentalExaminationPerformedHelper);
        Assert.Equal(cc, record.HistologicalPlacentalExaminationPerformed);
        Assert.Equal(VR.CodeSystems.SCT, record.HistologicalPlacentalExaminationPerformed["system"]);
        Assert.Equal("Performed", record.HistologicalPlacentalExaminationPerformed["display"]);
    }

    [Fact]
    public void Set_FetalRemainsDispositionMethod()
    {
        SetterFetalDeathRecord.FetalRemainsDispositionMethodHelper = "449971000124106";
        Assert.Equal("Burial", SetterFetalDeathRecord.FetalRemainsDispositionMethod["display"]);
        Dictionary<string, string> cc = new Dictionary<string, string>();
        cc.Add("code", BFDR.ValueSets.FetalRemainsDispositionMethod.Codes[0, 0]);
        cc.Add("system", BFDR.ValueSets.FetalRemainsDispositionMethod.Codes[0, 2]);
        cc.Add("display", BFDR.ValueSets.FetalRemainsDispositionMethod.Codes[0, 1]);
        Assert.Equal(cc, SetterFetalDeathRecord.FetalRemainsDispositionMethod);
    }

    [Fact]
    public void Get_FetalRemainsDispositionMethod()
    {
        FetalDeathRecord record = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
        Dictionary<string, string> cc = new Dictionary<string, string>();
        cc.Add("code", BFDR.ValueSets.FetalRemainsDispositionMethod.Codes[2, 0]);
        cc.Add("system", BFDR.ValueSets.FetalRemainsDispositionMethod.Codes[2, 2]);
        cc.Add("display", BFDR.ValueSets.FetalRemainsDispositionMethod.Codes[2, 1]);
        Assert.Null(record.FetalRemainsDispositionMethodHelper);
        record.FetalRemainsDispositionMethodHelper = "449951000124101";
        Assert.Equal(cc, record.FetalRemainsDispositionMethod);
        Assert.Equal(VR.CodeSystems.SCT, record.FetalRemainsDispositionMethod["system"]);
        Assert.Equal("Donation", record.FetalRemainsDispositionMethod["display"]);
    }

    [Fact]
    public void SetBirthWeight()
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
        Assert.Equal("0", ije.FW_BYPASS);
        ije.FW_BYPASS = "2";
        Assert.Equal("2", ije.FW_BYPASS);
    }

    [Fact]
    public void GetBirthWeight()
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
      Assert.False(record.FertilityEnhancingDrugTherapyArtificialIntrauterineInsemination);
      Assert.False(record.AssistedReproductiveTechnology);
      Assert.False(record.InfertilityTreatment);
      //set after parse
      record.NoMaternalMorbidities = true;
      Assert.True(record.NoMaternalMorbidities);
      record.EclampsiaHypertension = true;
      Assert.True(record.EclampsiaHypertension);
      record.FertilityEnhancingDrugTherapyArtificialIntrauterineInsemination = true;
      Assert.True(record.FertilityEnhancingDrugTherapyArtificialIntrauterineInsemination);
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
    public void SetFetalDeathCauseOrCondition()
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
    public void SetOtherFetalDeathCauseOrCondition()
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
    public void SetFetalPresentation()
    {
      FetalDeathRecord fetalDeathRecord = new FetalDeathRecord();
      fetalDeathRecord.FetalPresentationHelper = "6096002";
      Dictionary<string, string> cc = new Dictionary<string, string>();
      cc.Add("code", "6096002");
      cc.Add("system", "http://snomed.info/sct");
      cc.Add("display", "Breech presentation (finding)");
      Assert.Equal(cc, fetalDeathRecord.FetalPresentation);

      IJEFetalDeath ije = new IJEFetalDeath();
      ije.PRES = "1";
      Dictionary<string, string> cc2 = new Dictionary<string, string>();
      cc2.Add("code", "70028003");
      cc2.Add("system", "http://snomed.info/sct");
      cc2.Add("display", "Vertex presentation (finding)");
      FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal(cc2, fetalDeathRecord2.FetalPresentation);
      Assert.Equal("70028003", fetalDeathRecord2.FetalPresentationHelper);

      FetalDeathRecord fetalDeathRecord3 = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal(cc, fetalDeathRecord3.FetalPresentation);
      Assert.Equal("6096002", fetalDeathRecord3.FetalPresentationHelper);
      fetalDeathRecord3.FetalPresentationHelper = "70028003";
      Assert.Equal(cc2, fetalDeathRecord3.FetalPresentation);
    }

    [Fact]
    public void SetObstetricEstimateOfGestation()
    {
      FetalDeathRecord fetalDeathRecord1 = new FetalDeathRecord();
      Dictionary<string, string> dict = new Dictionary<string, string>();
      dict.Add("value", "10");
      dict.Add("code", "wk");
      fetalDeathRecord1.GestationalAgeAtDelivery = dict;
      Assert.Equal(dict["value"], fetalDeathRecord1.GestationalAgeAtDelivery["value"]);
      Assert.Equal("wk", fetalDeathRecord1.GestationalAgeAtDelivery["code"]);
      Assert.Equal("http://unitsofmeasure.org", fetalDeathRecord1.GestationalAgeAtDelivery["system"]);

      IJEFetalDeath ije = new IJEFetalDeath();
      ije.OWGEST = "38";
      FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal("38", fetalDeathRecord2.GestationalAgeAtDelivery["value"]);
      Assert.Equal("wk", fetalDeathRecord2.GestationalAgeAtDelivery["code"]);
      Assert.Equal("http://unitsofmeasure.org", fetalDeathRecord2.GestationalAgeAtDelivery["system"]);

      FetalDeathRecord fetalDeathRecord3 = new FetalDeathRecord();
      Dictionary<string, string> dict2 = new Dictionary<string, string>();
      dict2.Add("value", "48");
      dict2.Add("code", "d");
      fetalDeathRecord3.GestationalAgeAtDelivery = dict2;
      Assert.Equal(dict2["value"], fetalDeathRecord3.GestationalAgeAtDelivery["value"]);
      Assert.Equal("d", fetalDeathRecord3.GestationalAgeAtDelivery["code"]);
      Assert.Equal("http://unitsofmeasure.org", fetalDeathRecord3.GestationalAgeAtDelivery["system"]);
      // IJE should divide days by 7 and round down
      IJEFetalDeath ije2 = new(fetalDeathRecord3);
      ije2.OWGEST = "06";
      Assert.Equal("06", ije2.OWGEST);

      FetalDeathRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("20", parsedRecord.GestationalAgeAtDelivery["value"]);
      Assert.Equal("wk", parsedRecord.GestationalAgeAtDelivery["code"]);
      Assert.Equal("http://unitsofmeasure.org", parsedRecord.GestationalAgeAtDelivery["system"]);
      parsedRecord.GestationalAgeAtDelivery = dict2;
      Assert.Equal("48", parsedRecord.GestationalAgeAtDelivery["value"]);
      Assert.Equal("d", parsedRecord.GestationalAgeAtDelivery["code"]);
    }

    [Fact]
    public void SetGestationalAgeAtDeliveryEditFlag()
    {
      IJEFetalDeath ije = new IJEFetalDeath();
      ije.OWGEST_BYPASS = "0";
      FetalDeathRecord fetalDeathRecord = ije.ToFetalDeathRecord();
      Dictionary<string, string> editBypass = new Dictionary<string, string>();
      editBypass.Add("code", "0off");
      editBypass.Add("system", VR.CodeSystems.VRCLEditFlags);
      editBypass.Add("display", "Off");
      Assert.Equal(editBypass, fetalDeathRecord.GestationalAgeAtDeliveryEditFlag);
    }

    [Fact]
    public void MaternalMorbidities()
    {
      FetalDeathRecord fetalDeathRecord = new FetalDeathRecord();
      fetalDeathRecord.NoMaternalMorbidities = true;
      Assert.True(fetalDeathRecord.NoMaternalMorbidities);
      Assert.False(fetalDeathRecord.ICUAdmission);
      Assert.False(fetalDeathRecord.MaternalTransfusion);
      Assert.False(fetalDeathRecord.PerinealLaceration);
      Assert.False(fetalDeathRecord.RupturedUterus);
      Assert.False(fetalDeathRecord.UnplannedHysterectomy);

      IJEFetalDeath ije = new IJEFetalDeath();
      Assert.Equal("U", ije.AINT);
      ije.AINT = "Y";
      Assert.Equal("Y", ije.AINT);

      FetalDeathRecord parsedRecord = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.False(parsedRecord.ICUAdmission);
      parsedRecord.ICUAdmission = true;
      Assert.True(parsedRecord.ICUAdmission);

      IJEFetalDeath ije2 = new(parsedRecord);
      Assert.Equal("Y", ije2.AINT);
    }

    [Fact]
    public void SetLaborTrialAttempted()
    {
      FetalDeathRecord fetalDeathRecord = new FetalDeathRecord();
      fetalDeathRecord.LaborTrialAttempted = true;
      Assert.True(fetalDeathRecord.LaborTrialAttempted);

      IJEFetalDeath ije = new IJEFetalDeath();
      ije.TLAB = "Y";
      FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
      Assert.True(fetalDeathRecord2.LaborTrialAttempted);

      FetalDeathRecord fetalDeathRecord3 = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Null(fetalDeathRecord3.LaborTrialAttempted);
      fetalDeathRecord3.LaborTrialAttempted = true;
      Assert.True(fetalDeathRecord3.LaborTrialAttempted);

    }

    [Fact]
    public void FinalRouteAndMethodOfDelivery()
    {
      // Check nothing present in fresh record
      Assert.False(SetterFetalDeathRecord.UnknownFinalRouteAndMethodOfDelivery);
      var coding = SetterFetalDeathRecord.FinalRouteAndMethodOfDelivery;
      Assert.Equal("", coding["code"]);
      // Test setting the final route
      coding.Clear();
      coding.Add("code", "302383004");
      coding.Add("system", "http://snomed.info/sct");
      coding.Add("display", "Forceps delivery (procedure)");
      SetterFetalDeathRecord.FinalRouteAndMethodOfDelivery = coding;
      String json = SetterFetalDeathRecord.ToJSON();
      Assert.Contains("302383004", json); // code
      Assert.Contains("73762-7", json); // category
      Assert.Equal(coding, SetterFetalDeathRecord.FinalRouteAndMethodOfDelivery);
      coding = SetterFetalDeathRecord.FinalRouteAndMethodOfDelivery;
      Assert.Equal("302383004", coding["code"]);
      // Test that setting unknown removes the previously set route
      SetterFetalDeathRecord.UnknownFinalRouteAndMethodOfDelivery = true;
      Assert.True(SetterFetalDeathRecord.UnknownFinalRouteAndMethodOfDelivery);
      coding = SetterFetalDeathRecord.FinalRouteAndMethodOfDelivery;
      Assert.Equal("", coding["code"]);
      // Test that setting a route removes the unknown observation
      coding.Clear();
      coding.Add("code", "302383004");
      coding.Add("system", "http://snomed.info/sct");
      coding.Add("display", "Forceps delivery (procedure)");
      SetterFetalDeathRecord.FinalRouteAndMethodOfDelivery = coding;
      Assert.False(SetterFetalDeathRecord.UnknownFinalRouteAndMethodOfDelivery);
      // Parse record
      FetalDeathRecord fetalDeathRecord = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      var altCoding = SetterFetalDeathRecord.FinalRouteAndMethodOfDelivery;
      altCoding.Clear();
      altCoding.Add("code", "700000006");
      altCoding.Add("system", "http://snomed.info/sct");
      altCoding.Add("display", "Vaginal delivery of fetus (procedure)");
      altCoding.Add("text", "Spontaneous vaginal delivery");
      Assert.Equal(altCoding, fetalDeathRecord.FinalRouteAndMethodOfDelivery);
      fetalDeathRecord.FinalRouteAndMethodOfDelivery = coding;
      Assert.Equal(coding, fetalDeathRecord.FinalRouteAndMethodOfDelivery);
      // IJE record
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
      Assert.Equal("Utah", ije.STATE_D.Trim());
      Assert.Equal("United States", ije.COUNTRY_D.Trim());

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
      FetalDeathRecord br = new()
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
    public void SetOccupationIndustry()
    {
      FetalDeathRecord fetalDeathRecord = new FetalDeathRecord();
      fetalDeathRecord.MotherOccupation = "Carpenter";
      Assert.Equal("Carpenter", fetalDeathRecord.MotherOccupation);
      fetalDeathRecord.MotherIndustry = "Construction";
      Assert.Equal("Construction", fetalDeathRecord.MotherIndustry);
      fetalDeathRecord.FatherOccupation = "Lawyer";
      Assert.Equal("Lawyer", fetalDeathRecord.FatherOccupation);
      fetalDeathRecord.FatherIndustry = "Legal Services";
      Assert.Equal("Legal Services", fetalDeathRecord.FatherIndustry);

      //ije translations
      IJEFetalDeath ije = new IJEFetalDeath(fetalDeathRecord);
      Assert.Equal("Carpenter", ije.MOM_OC_T.Trim());
      Assert.Equal("Construction", ije.MOM_IN_T.Trim());
      Assert.Equal("Lawyer", ije.DAD_OC_T.Trim());
      Assert.Equal("Legal Services", ije.DAD_IN_T.Trim());
      FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal("Carpenter", fetalDeathRecord2.MotherOccupation);
      Assert.Equal("Construction", fetalDeathRecord2.MotherIndustry);
      Assert.Equal("Lawyer", fetalDeathRecord2.FatherOccupation);
      Assert.Equal("Legal Services", fetalDeathRecord2.FatherIndustry);
    }

    [Fact]
    public void GetOccupationIndustry()
    {
       //parse
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("Secretary", record.MotherOccupation);
      Assert.Equal("State Agency", record.MotherIndustry);
      Assert.Equal("Teaching Assistant", record.FatherOccupation);
      Assert.Equal("Elementary Schools", record.FatherIndustry);
      //set after parse
      record.MotherOccupation = "Carpenter";
      record.MotherIndustry = "Construction";
      Assert.Equal("Carpenter", record.MotherOccupation);
      Assert.Equal("Construction", record.MotherIndustry);
      // convert to IJE
      IJEFetalDeath ije = new(record);
      Assert.Equal("Carpenter", ije.MOM_OC_T.Trim());
      Assert.Equal("Construction", ije.MOM_IN_T.Trim());
      Assert.Equal("Teaching Assistant", ije.DAD_OC_T.Trim());
      Assert.Equal("Elementary Schools", ije.DAD_IN_T.Trim());
    }


    [Fact]
    public void Set_MotherReceivedWICFood()
    {
      IJEFetalDeath ije = new IJEFetalDeath();
      ije.WIC = "Y";
      Assert.Equal("Y", ije.WIC);
      FetalDeathRecord fetalDeathRecord = ije.ToFetalDeathRecord();
      Assert.Equal("Y", fetalDeathRecord.MotherReceivedWICFoodHelper);
      fetalDeathRecord.MotherReceivedWICFoodHelper = "N";
      Assert.Equal("N", fetalDeathRecord.MotherReceivedWICFood["code"]);
      Assert.Equal("http://terminology.hl7.org/CodeSystem/v2-0136", fetalDeathRecord.MotherReceivedWICFood["system"]);
      IJEFetalDeath ije2 = new(fetalDeathRecord);
      Assert.Equal("N", ije.WIC);


    }

    [Fact]
    public void Get_MotherReceivedWICFood()
    {
      FetalDeathRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("N", parsedRecord.MotherReceivedWICFoodHelper);
      parsedRecord.MotherReceivedWICFoodHelper = "Y";
      Assert.Equal("Y", parsedRecord.MotherReceivedWICFoodHelper);
    }

    [Fact]
    public void Set_PreviousBirths()
    {
      FetalDeathRecord fetalDeathRecord = new FetalDeathRecord();
      fetalDeathRecord.NumberOfBirthsNowDead = 2;
      fetalDeathRecord.NumberOfBirthsNowLiving = 3;
      Assert.Equal(2, fetalDeathRecord.NumberOfBirthsNowDead);
      Assert.Equal(3, fetalDeathRecord.NumberOfBirthsNowLiving);
      IJEFetalDeath ije = new(fetalDeathRecord);
      FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal(2, fetalDeathRecord2.NumberOfBirthsNowDead);
      Assert.Equal(3, fetalDeathRecord2.NumberOfBirthsNowLiving);
    }

    [Fact]
    public void Get_PreviousBirths()
    {
      FetalDeathRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal(1, parsedRecord.NumberOfBirthsNowLiving);
      Assert.Equal(0, parsedRecord.NumberOfBirthsNowDead);
      parsedRecord.NumberOfBirthsNowLiving = 3;
      parsedRecord.NumberOfBirthsNowDead = 2;
      Assert.Equal(3, parsedRecord.NumberOfBirthsNowLiving);
      Assert.Equal(2, parsedRecord.NumberOfBirthsNowDead);
    }

    [Fact]
    public void Set_FetalDeathsThisDelivery()
    {
      FetalDeathRecord fetalDeathRecord = new FetalDeathRecord();
      fetalDeathRecord.NumberOfFetalDeathsThisDelivery = 2;
      Assert.Equal(2, fetalDeathRecord.NumberOfFetalDeathsThisDelivery);
      IJEFetalDeath ije = new(fetalDeathRecord);
      FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal(2, fetalDeathRecord2.NumberOfFetalDeathsThisDelivery);
    }

    [Fact]
    public void Get_FetalDeathsThisDelivery()
    {
      FetalDeathRecord parsedRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal(1, parsedRecord.NumberOfFetalDeathsThisDelivery);
      parsedRecord.NumberOfFetalDeathsThisDelivery = 3;
      Assert.Equal(3, parsedRecord.NumberOfFetalDeathsThisDelivery);
    }

    [Fact]
    public void TestImportLastMenstrualPeriod()
    {
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("2018-04-18", record.LastMenstrualPeriod);
      Assert.Equal(2018, record.LastMenstrualPeriodYear);
      Assert.Equal(4, record.LastMenstrualPeriodMonth);
      Assert.Equal(18, record.LastMenstrualPeriodDay);

      IJEFetalDeath ije = new(record);
      Assert.Equal("2018", ije.DLMP_YR);
      Assert.Equal("04", ije.DLMP_MO);
      Assert.Equal("18", ije.DLMP_DY);

      // set after parse
      record.LastMenstrualPeriod = "2023-02";
      Assert.Equal("2023-02", record.LastMenstrualPeriod);
      Assert.Equal(2023, record.LastMenstrualPeriodYear);
      Assert.Equal(2, record.LastMenstrualPeriodMonth);
      Assert.Null(record.LastMenstrualPeriodDay);
    }

    [Fact]
    public void SetLastMenstrualPeriod()
    {
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriod);
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriodYear);
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriodMonth);
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriodDay);
      SetterFetalDeathRecord.LastMenstrualPeriod = "2023-02";
      Assert.Equal("2023-02", SetterFetalDeathRecord.LastMenstrualPeriod);
      Assert.Equal(2023, SetterFetalDeathRecord.LastMenstrualPeriodYear);
      Assert.Equal(2, SetterFetalDeathRecord.LastMenstrualPeriodMonth);
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriodDay);
      SetterFetalDeathRecord.LastMenstrualPeriod = null;
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriod);
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriodYear);
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriodMonth);
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriodDay);
      SetterFetalDeathRecord.LastMenstrualPeriodMonth = 4;
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriod);
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriodYear);
      Assert.Equal(4, SetterFetalDeathRecord.LastMenstrualPeriodMonth);
      Assert.Null(SetterFetalDeathRecord.LastMenstrualPeriodDay);
      SetterFetalDeathRecord.LastMenstrualPeriodYear = 2024;
      Assert.Equal(2024, SetterFetalDeathRecord.LastMenstrualPeriodYear);
      Assert.Equal("2024-04", SetterFetalDeathRecord.LastMenstrualPeriod);
    }

    [Fact]
    public void TestMotherHeightPropertiesSetter()
    {
        FetalDeathRecord record = new FetalDeathRecord();
        // Height
        Assert.Null(record.MotherHeight);
        record.MotherHeight = 67;
        Assert.Equal(67, record.MotherHeight);
        // Edit Flag
        Assert.Equal("", record.MotherHeightEditFlag["code"]);
        record.MotherHeightEditFlagHelper = VR.ValueSets.EditBypass01234.Edit_Passed;
        Assert.Equal(VR.ValueSets.EditBypass01234.Edit_Passed, record.MotherHeightEditFlag["code"]);
        // IJE translations
        IJEFetalDeath ije1 = new IJEFetalDeath(record);
        Assert.Equal("5", ije1.HFT);
        Assert.Equal("07", ije1.HIN);
        Assert.Equal("0", ije1.HGT_BYPASS);
    }

    [Fact]
    public void TestImportMotherHeightProperties()
    {
        FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
        Assert.Equal(56, record.MotherHeight);

        //set after parse
        record.MotherHeight = 68;
        Assert.Equal(68, record.MotherHeight);
    }

    [Fact]
    public void TestWeightPropertiesSetter()
    {
        FetalDeathRecord record = new FetalDeathRecord();
        // Prepregnancy Weight
        Assert.Null(record.MotherPrepregnancyWeight);
        record.MotherPrepregnancyWeight = 145;
        Assert.Equal(145, record.MotherPrepregnancyWeight);
        // Birth Weight
        Assert.Null(record.BirthWeight);
        record.BirthWeight = 2500;
        Assert.Equal(2500, record.BirthWeight);
        // Edit Flags
        Assert.Equal("", record.MotherPrepregnancyWeightEditFlag["code"]);
        record.MotherPrepregnancyWeightEditFlagHelper = BFDR.ValueSets.PregnancyReportEditFlags.Edit_Passed;
        Assert.Equal(BFDR.ValueSets.PregnancyReportEditFlags.Edit_Passed, record.MotherPrepregnancyWeightEditFlag["code"]);
        Assert.Equal("", record.BirthWeightEditFlag["code"]);
        record.BirthWeightEditFlagHelper = BFDR.ValueSets.BirthWeightEditFlags.Off;
        Assert.Equal(BFDR.ValueSets.BirthWeightEditFlags.Off, record.BirthWeightEditFlag["code"]);
        // IJE translations
        IJEFetalDeath ije1 = new IJEFetalDeath(record);
        Assert.Equal("145", ije1.PWGT);
        Assert.Equal("0", ije1.PWGT_BYPASS);
        // TODO: add these when fetal weight is added
        // Assert.Equal("2500", ije1.FWG);
        // Assert.Equal("0", ije1.FW_BYPASS);
    }

    [Fact]
    public void TestImportWeightProperties()
    {
        FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
        // Prepregnancy Weight
        Assert.Equal(180, record.MotherPrepregnancyWeight);
        // TODO: add these when fetal weight is added
        // Birth Weight
        // Assert.Equal(1530, record.BirthWeight);

        // set after parse
        record.MotherPrepregnancyWeight = 146;
        // record.BirthWeight = 2502;
        Assert.Equal(146, record.MotherPrepregnancyWeight);
        // Assert.Equal(2502, record.BirthWeight);
    }

    [Fact]
    public void ParseMotherRaceEthnicityJsonToIJE()
    {
        FetalDeathRecord b3 = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
        IJEFetalDeath ije3 = new IJEFetalDeath(b3);
        Assert.Equal("Y", ije3.METHNIC1);
        Assert.Equal("Y", ije3.METHNIC2);
        Assert.Equal("Y", ije3.METHNIC3);
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
        Assert.Equal("Y", ije3.FETHNIC1);
        Assert.Equal("Y", ije3.FETHNIC2);
        Assert.Equal("Y", ije3.FETHNIC3);
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

    [Fact]
    public void TestImportRace()
    {
      FetalDeathRecord record = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/Bundle-bundle-coded-race-and-ethnicity-baby-g-quinn.json")), true);
      Assert.Equal("101", record.FatherRaceTabulation1EHelper);
      Assert.Equal("101", record.MotherRaceTabulation1EHelper);
      Assert.Equal("233", record.FatherEthnicityEditedCodeHelper);
      Assert.Equal("233", record.MotherEthnicityEditedCodeHelper);
    }

    [Fact]
    public void Set_FatherRace()
    {
      // default Ethnicity should be null
      Assert.Null(SetterFetalDeathRecord.FatherRaceTabulation1EHelper);
      SetterFetalDeathRecord.FatherRaceTabulation1EHelper = "222";
      Assert.Equal("222", SetterFetalDeathRecord.FatherRaceTabulation1EHelper);
      Assert.Equal("Tobago", SetterFetalDeathRecord.FatherRaceTabulation1E["display"]);
    }

    [Fact]
    public void Set_MotherRace()
    {
      // default Ethnicity should be null
      Assert.Null(SetterFetalDeathRecord.FatherRaceTabulation1EHelper);
      SetterFetalDeathRecord.FatherRaceTabulation1EHelper = "222";
      Assert.Equal("222", SetterFetalDeathRecord.FatherRaceTabulation1EHelper);
      Assert.Equal("Tobago", SetterFetalDeathRecord.FatherRaceTabulation1E["display"]);
    }

    [Fact]
    public void Set_FatherEthnicityLiteral()
    {
      // default Ethnicity should be null
      Assert.Null(SetterFetalDeathRecord.FatherEthnicityLiteral);
      SetterFetalDeathRecord.FatherEthnicityLiteral = "Guatemalan";
      Assert.Equal("Guatemalan", SetterFetalDeathRecord.FatherEthnicityLiteral);
    }

    [Fact]
    public void Set_MotherCodedEthnicityLiteral()
    {
      Assert.Equal("", SetterFetalDeathRecord.MotherEthnicityCodeForLiteral["display"]);
      Dictionary<string, string> CodedEthnicity = new Dictionary<string, string>();
      CodedEthnicity.Add("code", VR.ValueSets.HispanicOrigin.Codes[2, 0]);
      CodedEthnicity.Add("display", VR.ValueSets.HispanicOrigin.Codes[2, 1]);
      CodedEthnicity.Add("system", VR.ValueSets.HispanicOrigin.Codes[2, 2]);
      SetterFetalDeathRecord.MotherEthnicityCodeForLiteral = CodedEthnicity;
      Assert.Equal("Andalusian", SetterFetalDeathRecord.MotherEthnicityCodeForLiteral["display"]);
    }

    [Fact]
    public void Set_MotherCodedEthnicityEdited()
    {
      Assert.Equal("", SetterFetalDeathRecord.MotherEthnicityEditedCode["display"]);
      Dictionary<string, string> CodedEthnicity = new Dictionary<string, string>();
      CodedEthnicity.Add("code", VR.ValueSets.HispanicOrigin.Codes[2, 0]);
      CodedEthnicity.Add("display", VR.ValueSets.HispanicOrigin.Codes[2, 1]);
      CodedEthnicity.Add("system", VR.ValueSets.HispanicOrigin.Codes[2, 2]);
      SetterFetalDeathRecord.MotherEthnicityEditedCode = CodedEthnicity;
      Assert.Equal("Andalusian", SetterFetalDeathRecord.MotherEthnicityEditedCode["display"]);
    }

    [Fact]
    public void Set_FatherCodedEthnicityEdited()
    {
      Assert.Equal("", SetterFetalDeathRecord.FatherEthnicityEditedCode["display"]);
      Dictionary<string, string> CodedEthnicity = new Dictionary<string, string>();
      CodedEthnicity.Add("code", VR.ValueSets.HispanicOrigin.Codes[2, 0]);
      CodedEthnicity.Add("display", VR.ValueSets.HispanicOrigin.Codes[2, 1]);
      CodedEthnicity.Add("system", VR.ValueSets.HispanicOrigin.Codes[2, 2]);
      SetterFetalDeathRecord.FatherEthnicityEditedCode = CodedEthnicity;
      Assert.Equal("Andalusian", SetterFetalDeathRecord.FatherEthnicityEditedCode["display"]);
    }

    [Fact]
    public void Set_FatherCodedEthnicityLiteral()
    {
      Assert.Equal("", SetterFetalDeathRecord.FatherEthnicityCodeForLiteral["display"]);
      Dictionary<string, string> CodedEthnicity = new Dictionary<string, string>();
      CodedEthnicity.Add("code", VR.ValueSets.HispanicOrigin.Codes[2, 0]);
      CodedEthnicity.Add("display", VR.ValueSets.HispanicOrigin.Codes[2, 1]);
      CodedEthnicity.Add("system", VR.ValueSets.HispanicOrigin.Codes[2, 2]);
      SetterFetalDeathRecord.FatherEthnicityCodeForLiteral = CodedEthnicity;
      Assert.Equal("Andalusian", SetterFetalDeathRecord.FatherEthnicityCodeForLiteral["display"]);
    }

    [Fact]
    public void TestImportEducation()
    {
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("SEC", record.MotherEducationLevelHelper);
      Assert.Null(record.FatherEducationLevelHelper);
      Dictionary<string, string> cc = new Dictionary<string, string>();
      cc.Add("code", "SEC");
      cc.Add("system", "http://terminology.hl7.org/CodeSystem/v3-EducationLevel");
      cc.Add("display", "Some secondary or high school education");
      cc.Add("text", "9th through 12th grade; no diploma");
      Assert.Equal(cc, record.MotherEducationLevel);
      // set after parse
      record.FatherEducationLevel = cc;
      Assert.Equal(cc, record.FatherEducationLevel);
      record.MotherEducationLevelHelper = "SCOL";
      Assert.Equal("SCOL", record.MotherEducationLevelHelper);
      Dictionary<string, string> cc2 = new Dictionary<string, string>();
      cc2.Add("code", "SCOL");
      cc2.Add("system", "http://terminology.hl7.org/CodeSystem/v3-EducationLevel");
      cc2.Add("display", "Some College education");
      Assert.Equal(cc2, record.MotherEducationLevel);

      var coding = new Dictionary<string, string>();
      coding.Add("code", "1");
      coding.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");
      record.MotherEducationLevelEditFlag = coding;
      Assert.Equal("1", record.MotherEducationLevelEditFlagHelper);
      Assert.Equal("1", record.MotherEducationLevelEditFlag["code"]);
      record.FatherEducationLevelEditFlag = coding;
      Assert.Equal("1", record.FatherEducationLevelEditFlagHelper);
      Assert.Equal("1", record.FatherEducationLevelEditFlag["code"]);

      IJEFetalDeath ije = new(record);
      Assert.Equal("2", ije.FEDUC);
      Assert.Equal("4", ije.MEDUC);
      Assert.Equal("1", ije.FEDUC_BYPASS);
      Assert.Equal("1", ije.MEDUC_BYPASS);
    }

    [Fact]
    public void TestParentReportedAge()
    {

      // set
      SetterFetalDeathRecord.MotherReportedAgeAtDelivery = 26;
      SetterFetalDeathRecord.FatherReportedAgeAtDelivery = 27;
      Assert.Equal(26, SetterFetalDeathRecord.MotherReportedAgeAtDelivery);
      Assert.Equal(27, SetterFetalDeathRecord.FatherReportedAgeAtDelivery);

      // parsed record
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicFetalDeathRecord.json")));
      Assert.Equal(33, record.MotherReportedAgeAtDelivery);
      Assert.Equal(31, record.FatherReportedAgeAtDelivery);

      // to IJE
      IJEFetalDeath ije = new(record);
      Assert.Equal("33", ije.MAGER.Trim(' '));
      Assert.Equal("31", ije.FAGER.Trim(' '));
    }


    [Fact]
    public void TestChildName()
    {
      // set
      FetalDeathRecord record = new FetalDeathRecord();
      Assert.Empty(record.FetusGivenNames);
      Assert.Null(record.FetusFamilyName);
      Assert.Null(record.FetusSuffix);
      // Child's First Name
      string[] names = {"Baby", "G"};
      record.FetusGivenNames = names;
      Assert.Equal("Baby", record.FetusGivenNames[0]);
      // Child's Middle Name
      Assert.Equal("G", record.FetusGivenNames[1]);
      // Child's Last Name
      record.FetusFamilyName = "Quinn";
      Assert.Equal("Quinn", record.FetusFamilyName);
      // Child's Surname Suffix
      record.FetusSuffix = "III";
      Assert.Equal("III", record.FetusSuffix);

      // parsed record
      FetalDeathRecord parsedRecord = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicFetalDeathRecord.json")));
      Assert.Equal("Baby", parsedRecord.FetusGivenNames[0]);
      Assert.Equal("G", parsedRecord.FetusGivenNames[1]);
      Assert.Equal("Quinn", parsedRecord.FetusFamilyName);
      // update
      string[] parsedNames = {"Jim", "Jam"};
      parsedRecord.FetusGivenNames = parsedNames;
      parsedRecord.FetusFamilyName = "Jones";
      parsedRecord.FetusSuffix = "Junior";
      Assert.Equal("Jim", parsedRecord.FetusGivenNames[0]);
      Assert.Equal("Jam", parsedRecord.FetusGivenNames[1]);
      Assert.Equal("Jones", parsedRecord.FetusFamilyName);
      Assert.Equal("Junior", parsedRecord.FetusSuffix);

      // to IJE
      IJEFetalDeath ije = new(parsedRecord);
      Assert.Equal("Jim", ije.FETFNAME.Trim(' '));
      Assert.Equal("Jam", ije.FETMNAME.Trim(' '));
      Assert.Equal("Jones", ije.FETLNAME.Trim(' '));
      Assert.Equal("Junior", ije.SUFFIX.Trim(' '));
      // update
      ije.FETFNAME = "A";
      Assert.Equal("A", ije.FETFNAME.Trim(' '));
      ije.FETMNAME = "B";
      Assert.Equal("B", ije.FETMNAME.Trim(' '));
      ije.FETLNAME = "C";
      Assert.Equal("C", ije.FETLNAME.Trim(' '));
      ije.SUFFIX = "D";
      Assert.Equal("D", ije.SUFFIX.Trim(' '));

      // test missing family name
      Assert.Equal("Quinn", record.FetusFamilyName);
      Assert.Null(record.GetFamilyNameAbsentDataReason());
      record.FetusFamilyName = ""; //set family name to empty
      Assert.Equal("", record.FetusFamilyName);
      Assert.Equal("unknown", record.GetFamilyNameAbsentDataReason());
      record.FetusFamilyName = null; //set family name to null
      Assert.Null(record.FetusFamilyName);
      Assert.Equal("unknown", record.GetFamilyNameAbsentDataReason());
      IJEFetalDeath ije2 = new(record);
      Assert.Equal("", ije2.FETLNAME.Trim(' '));
    }

    // FHIR manages names in a way that there is a fundamental incompatibility with IJE: the "middle name" is the second element in
    // an array of given names. That means that it's not possible to set a middle name without first having a first name. The library
    // handles this by 1) raising an exception if a middle name is set before a first name and 2) resetting the middle name if the first
    // name is set again. If a user sets the first name and then the middle name then no problems will occur.

    [Fact]
    public void SettingMiddleNameFirstRaisesException()
    {
        IJEFetalDeath ije = new IJEFetalDeath();
        Exception ex = Assert.Throws<System.ArgumentException>(() => ije.FETMNAME = "M");
        Assert.Equal("Middle name cannot be set before first name", ex.Message);
        // ex = Assert.Throws<System.ArgumentException>(() => ije.MOMMNAME = "M");
        // Assert.Equal("Middle name cannot be set before first name", ex.Message);
        // ex = Assert.Throws<System.ArgumentException>(() => ije.DADMNAME = "M");
        // Assert.Equal("Middle name cannot be set before first name", ex.Message);
        }

    [Fact]
    public void TestPatientDecedentFetusVitalRecordProperties()
    {
      // Test FHIR record import.
      FetalDeathRecord firstRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      string firstDescription = firstRecord.ToDescription();
      // Test conversion via FromDescription.
      FetalDeathRecord secondRecord = VitalRecord.FromDescription<FetalDeathRecord>(firstDescription);

      // Record Certificate Number
      Assert.Equal("9876", firstRecord.CertificateNumber);
      Assert.Equal(firstRecord.CertificateNumber, secondRecord.CertificateNumber);
      // Record Birth Record Identifier
      Assert.Equal("2019MI009876", firstRecord.RecordIdentifier);
      Assert.Equal(firstRecord.RecordIdentifier, secondRecord.RecordIdentifier);
      // Record State Local Identifier 1
      Assert.Equal("11111-11111", firstRecord.StateLocalIdentifier1);
      Assert.Equal(firstRecord.StateLocalIdentifier1, secondRecord.StateLocalIdentifier1);
      // Complete Date of Delivery.
      Assert.Equal("2019-01-09", firstRecord.DateOfDelivery);
      Assert.Equal(firstRecord.DateOfDelivery, secondRecord.DateOfDelivery);
      // State of Delivery
      Assert.Equal("UT", firstRecord.PlaceOfDelivery["addressState"]);
      Assert.Equal(firstRecord.PlaceOfDelivery["addressState"], secondRecord.PlaceOfDelivery["addressState"]);
      // Time of Delivery
      Assert.Contains("18:23:00", firstRecord.DateTimeOfDelivery);
      Assert.Equal(firstRecord.DateTimeOfDelivery, secondRecord.DateTimeOfDelivery);
      // Sex
      Assert.Equal("F", firstRecord.FetalDeathSex);
      Assert.Equal(firstRecord.FetalDeathSex, secondRecord.FetalDeathSex);
      // Assert.Equal("F", firstRecord.FetalDeathSexHelper);
      // Assert.Equal(firstRecord.FetalDeathSex["code"], secondRecord.FetalDeathSexHelper);
      // Plurality
      Assert.Equal(4, firstRecord.FetalDeathPlurality);
      // Set Order
      Assert.Equal(3, firstRecord.FetalDeathSetOrder);
      Assert.Equal(firstRecord.FetalDeathSetOrder, secondRecord.FetalDeathSetOrder);
      // // Mother's Age
      //  Assert.Equal(34, firstRecord.MotherReportedAgeAtDelivery);
      // // Father's Age
      //  Assert.Equal(35, firstRecord.FatherReportedAgeAtDelivery);
      // // Child's First Name
      // Assert.Equal("Baby", firstRecord.ChildGivenNames[0]);
      // Assert.Equal(firstRecord.ChildGivenNames[0], secondRecord.ChildGivenNames[0]);
      // // Child's Middle Name
      // Assert.Equal("G", firstRecord.ChildGivenNames[1]);
      // Assert.Equal(firstRecord.ChildGivenNames[1], secondRecord.ChildGivenNames[1]);
      // // Child's Last Name
      // Assert.Equal("Quinn", firstRecord.ChildFamilyName);
      // Assert.Equal(firstRecord.ChildFamilyName, secondRecord.ChildFamilyName);
      // // Child's Surname Suffix
      // Assert.Equal("III", firstRecord.ChildSuffix);
      // Assert.Equal(firstRecord.ChildSuffix, secondRecord.ChildSuffix);
      // County of Delivery (Literal)
      Assert.Equal("Made Up", firstRecord.PlaceOfDelivery["addressCounty"]);
      Assert.Equal(firstRecord.PlaceOfDelivery["addressCounty"], secondRecord.PlaceOfDelivery["addressCounty"]);
      // County of Delivery (Code)
      Assert.Equal("000", firstRecord.PlaceOfDelivery["addressCountyC"]);
      Assert.Equal(firstRecord.PlaceOfDelivery["addressCountyC"], secondRecord.PlaceOfDelivery["addressCountyC"].PadLeft(3, '0'));
      // City/town/place of Delivery (Literal)
      Assert.Equal("Salt Lake City", firstRecord.PlaceOfDelivery["addressCity"]);
      Assert.Equal(firstRecord.PlaceOfDelivery["addressCity"], secondRecord.PlaceOfDelivery["addressCity"]);
      // Mother Social Security Number
      Assert.Equal("132224986", firstRecord.MotherSocialSecurityNumber);
      Assert.Equal(firstRecord.MotherSocialSecurityNumber, secondRecord.MotherSocialSecurityNumber);
      // Father Social Security Number
      Assert.Null(firstRecord.FatherSocialSecurityNumber);
      Assert.Equal(firstRecord.FatherSocialSecurityNumber, secondRecord.FatherSocialSecurityNumber);
    }

    [Fact]
    public void TestMotherBirthDateSetter()
    {
      BirthRecord_Should.TestMotherBirthDateHelper(new FetalDeathRecord());
    }

    [Fact]
    public void TestMotherBirthDateUnknowns()
    {
      BirthRecord_Should.TestMotherBirthDateUnknownsHelper(new FetalDeathRecord());
    }

    [Fact]
    public void TestFatherBirthDateSetters()
    {
      BirthRecord_Should.TestFatherBirthDateSetterHelper(new FetalDeathRecord());
    }

    [Fact]
    public void ParentBirthDatesPresent()
    {
      FetalDeathRecord fd = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("1990-03-11", fd.MotherDateOfBirth);
      Assert.Equal("1991-06-05", fd.FatherDateOfBirth);
    }

    [Fact]
    public void TestImportFatherBirthplace()
    {
      // Test FHIR record import.
      FetalDeathRecord firstRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      string firstDescription = firstRecord.ToDescription();
      // Test conversion via FromDescription.
      FetalDeathRecord secondRecord = VitalRecord.FromDescription<FetalDeathRecord>(firstDescription);
      // Test IJE Conversion.
      IJEFetalDeath ije = new(secondRecord);

      Assert.Equal(firstRecord.FatherPlaceOfBirth, secondRecord.FatherPlaceOfBirth);
      // Country
      Assert.Equal("US", firstRecord.FatherPlaceOfBirth["addressCountry"]);
      Assert.Equal(firstRecord.FatherPlaceOfBirth["addressCountry"], ije.FBPLACE_CNT_C);
      // State
      Assert.Equal("MA", firstRecord.FatherPlaceOfBirth["addressState"]);
      Assert.Equal(firstRecord.FatherPlaceOfBirth["addressState"], ije.FBPLACD_ST_TER_C);
      // set after parse
      firstRecord.FatherPlaceOfBirth = new Dictionary<string, string>
      {
        ["addressState"] = "NY",
        ["addressCounty"] = "Queens",
        ["addressCity"] = "New York",
        ["addressCountry"] = "US"
      };
      Assert.Equal("NY", firstRecord.FatherPlaceOfBirth["addressState"]);
      Assert.Equal("Queens", firstRecord.FatherPlaceOfBirth["addressCounty"]);
      Assert.Equal("New York", firstRecord.FatherPlaceOfBirth["addressCity"]);
      Assert.Equal("US", firstRecord.FatherPlaceOfBirth["addressCountry"]);
    }

    [Fact]
    public void Test_EmergingIssues()
    {
        FetalDeathRecord record1 = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
        Assert.Null(record1.EmergingIssue1_1);
        Assert.Null(record1.EmergingIssue1_2);
        Assert.Null(record1.EmergingIssue1_3);
        Assert.Null(record1.EmergingIssue1_4);
        Assert.Null(record1.EmergingIssue1_5);
        Assert.Null(record1.EmergingIssue1_6);
        Assert.Null(record1.EmergingIssue8_1);
        Assert.Null(record1.EmergingIssue8_2);
        Assert.Null(record1.EmergingIssue8_3);
        Assert.Null(record1.EmergingIssue20);
        //set after parse
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

        IJEFetalDeath ije = new IJEFetalDeath(record1, false); // Don't validate since we don't care about most fields
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
    public void TestMotherPlaceOfBirth()
    {
      FetalDeathRecord fhir = new FetalDeathRecord();
      IJEFetalDeath ije = new IJEFetalDeath(fhir);

      ije.BPLACEC_ST_TER = "MA";
      ije.BPLACEC_CNT = "US";

      Assert.Equal("MA", ije.BPLACEC_ST_TER);
      Assert.Equal("Massachusetts", ije.MBPLACE_ST_TER_TXT.Trim());
      Assert.Equal("US", ije.BPLACEC_CNT.Trim());
      Assert.Equal("United States", ije.MBPLACE_CNTRY_TXT.Trim());
    }

    [Fact]
    public void TestMotherResidence()
    {
      FetalDeathRecord fhirRecord = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/BasicFetalDeathRecord.json")));
      IJEFetalDeath ije = new IJEFetalDeath(fhirRecord);

      // set fields in parsed record
      ije.STATEC = "NH";
      ije.COUNTRYC = "US";
      ije.COUNTYC = "007";
      ije.COUNTYTXT = "Coos";
      ije.CITYTXT = "Berlin";
      ije.ZIPCODE= "03570";
      ije.STNUM = "5";
      ije.APTNUMB = "1";
      ije.PREDIR = "E";
      ije.POSTDIR = "W";
      ije.STNAME = "Main";
      ije.STDESIG = "St";
      ije.ADDRESS = "5 E Main St Berlin, NH";
      ije.LIMITS = "U";

      Assert.Equal("NH", ije.STATEC);
      Assert.Equal("New Hampshire", ije.STATETXT.Trim());
      Assert.Equal("US", ije.COUNTRYC);
      Assert.Equal("United States", ije.CNTRYTXT);
      Assert.Equal("U", ije.LIMITS);
      Assert.Equal("UNK", fhirRecord.MotherResidenceWithinCityLimits["code"]);
      Assert.Equal("007", ije.COUNTYC);
      Assert.Equal("Coos", ije.COUNTYTXT.Trim());
      Assert.Equal("Berlin", ije.CITYTXT.Trim());
      Assert.Equal("03570", ije.ZIPCODE.Trim());
      Assert.Equal("5", ije.STNUM.Trim());
      Assert.Equal("1", ije.APTNUMB.Trim());
      Assert.Equal("E", ije.PREDIR.Trim());
      Assert.Equal("W", ije.POSTDIR.Trim());
      Assert.Equal("Main", ije.STNAME.Trim());
      Assert.Equal("St", ije.STDESIG.Trim());
      Assert.Equal("5 E Main St Berlin, NH", ije.ADDRESS.Trim());
    }

    [Fact]
    public void Set_Plurality()
    {
      Assert.Null(SetterFetalDeathRecord.FetalDeathSetOrder);
      Assert.Null(SetterFetalDeathRecord.FetalDeathPlurality);
      Assert.Equal("", SetterFetalDeathRecord.FetalDeathPluralityEditFlag["code"]);
      SetterFetalDeathRecord.FetalDeathSetOrder = null;
      Assert.Null(SetterFetalDeathRecord.FetalDeathSetOrder);
      SetterFetalDeathRecord.FetalDeathSetOrder = 3;
      Assert.Equal(3, SetterFetalDeathRecord.FetalDeathSetOrder);
      Assert.Null(SetterFetalDeathRecord.FetalDeathPlurality);
      Assert.Equal("", SetterFetalDeathRecord.FetalDeathPluralityEditFlag["code"]);
      SetterFetalDeathRecord.FetalDeathPlurality = 4;
      Assert.Equal(3, SetterFetalDeathRecord.FetalDeathSetOrder);
      Assert.Equal(4, SetterFetalDeathRecord.FetalDeathPlurality);
      Assert.Equal("", SetterFetalDeathRecord.FetalDeathPluralityEditFlag["code"]);
      SetterFetalDeathRecord.FetalDeathSetOrder = -1;
      Assert.Equal(-1, SetterFetalDeathRecord.FetalDeathSetOrder);
      Assert.Equal(4, SetterFetalDeathRecord.FetalDeathPlurality);
      Assert.Equal("", SetterFetalDeathRecord.FetalDeathPluralityEditFlag["code"]);
      SetterFetalDeathRecord.FetalDeathSetOrder = null;
      Assert.Null(SetterFetalDeathRecord.FetalDeathSetOrder);
      Assert.Equal(4, SetterFetalDeathRecord.FetalDeathPlurality);
      Assert.Equal("", SetterFetalDeathRecord.FetalDeathPluralityEditFlag["code"]);
      var coding = new Dictionary<string, string>();
      coding.Add("code", "queriedCorrect");
      coding.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");
      SetterFetalDeathRecord.FetalDeathPluralityEditFlag = coding;
      Assert.Null(SetterFetalDeathRecord.FetalDeathSetOrder);
      Assert.Equal(4, SetterFetalDeathRecord.FetalDeathPlurality);
      Assert.Equal("queriedCorrect", SetterFetalDeathRecord.FetalDeathPluralityEditFlag["code"]);
      SetterFetalDeathRecord.FetalDeathPlurality = 2;
      SetterFetalDeathRecord.FetalDeathSetOrder = 1;
      Assert.Equal(1, SetterFetalDeathRecord.FetalDeathSetOrder);
      Assert.Equal(2, SetterFetalDeathRecord.FetalDeathPlurality);
      Assert.Equal("queriedCorrect", SetterFetalDeathRecord.FetalDeathPluralityEditFlag["code"]);
    }

    [Fact]
    public void TestMotherFatherSSN()
    {
      //parse
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("132224986", record.MotherSocialSecurityNumber);
      Assert.Null(record.FatherSocialSecurityNumber);

      //set after parse
      record.MotherSocialSecurityNumber = "123456789";
      Assert.Equal("123456789", record.MotherSocialSecurityNumber);
      record.FatherSocialSecurityNumber = "123123123";
      Assert.Equal("123123123", record.FatherSocialSecurityNumber);

      IJEFetalDeath ije = new(record);
      Assert.Equal("123456789", ije.MOM_SSN);
      Assert.Equal("123123123", ije.DAD_SSN);
    }

    [Fact]
    public void MotherNamesPresent()
    {
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("Carmen", record.MotherGivenNames[0]);
      Assert.Equal("Teresa", record.MotherGivenNames[1]);
      Assert.Equal("Lee", record.MotherFamilyName);
      Assert.Null(record.MotherSuffix);
      //set after parse
      string[] names = {"Mommy", "D"};
      record.MotherGivenNames = names;
      Assert.Equal("Mommy", record.MotherGivenNames[0]);
      // Mother's Middle Name
      Assert.Equal("D", record.MotherGivenNames[1]);
      // Mother's Last Name
      record.MotherFamilyName = "Le";
      Assert.Equal("Le", record.MotherFamilyName);

      IJEFetalDeath ije = new(record);
      Assert.Equal("Mommy", ije.MOMFNAME.Trim());
      Assert.Equal("D", ije.MOMMNAME.Trim());
      Assert.Equal("Le", ije.MOMLNAME.Trim());
    }

    [Fact]
    public void FatherNamesPresent()
    {
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.Equal("Tom", record.FatherGivenNames[0]);
      Assert.Equal("Yan", record.FatherGivenNames[1]);
      Assert.Equal("Lee", record.FatherFamilyName);
      Assert.Null(record.FatherSuffix);
      //set after parse
      string[] names = {"Daddy", "D"};
      record.FatherGivenNames = names;
      Assert.Equal("Daddy", record.FatherGivenNames[0]);
      // Father's Middle Name
      Assert.Equal("D", record.FatherGivenNames[1]);
      // Father's Last Name
      record.FatherFamilyName = "Le";
      Assert.Equal("Le", record.FatherFamilyName);
      // Father's suffix
      record.FatherSuffix = "Jr";
      Assert.Equal("Jr", record.FatherSuffix);

      IJEFetalDeath ije = new(record);
      Assert.Equal("Daddy", ije.DADFNAME.Trim());
      Assert.Equal("D", ije.DADMNAME.Trim());
      Assert.Equal("Le", ije.DADLNAME.Trim());
      Assert.Equal("Jr", ije.DADSUFFIX.Trim());
    }

    [Fact]
    public void TestMotherNameSetters()
    {
      FetalDeathRecord record = new FetalDeathRecord();
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
      FetalDeathRecord record = new FetalDeathRecord();
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
      FetalDeathRecord record = new FetalDeathRecord();
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
    public void TestPatientFetalDeath() {
      Assert.True(SetterFetalDeathRecord.PatientFetalDeath); //default for DecedentFetus is True
      //parse
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathReport.json")));
      Assert.True(record.PatientFetalDeath);

      SetterFetalDeathRecord.PatientFetalDeath = false; //DecedentFetus is required to have PatientFetalDeath extension = true. --> handle in business rules.
      Assert.Null(SetterFetalDeathRecord.PatientFetalDeath); //Fetal death should only be indicated if Patient is deceased (value = true).
      SetterFetalDeathRecord.PatientFetalDeath = true;
      Assert.True(SetterFetalDeathRecord.PatientFetalDeath);
    }
    [Fact]
    public void TestCodedInitiatingFetalCOD(){
      Assert.Null(SetterFetalDeathRecord.CodedInitiatingFetalCOD);
      SetterFetalDeathRecord.CodedInitiatingFetalCOD = "R836";
      Assert.Equal("R836", SetterFetalDeathRecord.CodedInitiatingFetalCOD);
      SetterFetalDeathRecord.CodedInitiatingFetalCOD = "R83.6";
      Assert.Equal("R83.6", SetterFetalDeathRecord.CodedInitiatingFetalCOD);
      SetterFetalDeathRecord.CodedInitiatingFetalCOD = "R83.";
      Assert.Equal("R83.", SetterFetalDeathRecord.CodedInitiatingFetalCOD);
      SetterFetalDeathRecord.CodedInitiatingFetalCOD = "R83";
      Assert.Equal("R83", SetterFetalDeathRecord.CodedInitiatingFetalCOD);
      //parse
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathCauseOrConditionCodedContent.json")));
      Assert.Equal("P01.1", record.CodedInitiatingFetalCOD);
      //set after parse
      record.CodedInitiatingFetalCOD = "R836";
      Assert.Equal("R836", record.CodedInitiatingFetalCOD);

      IJEFetalDeath ije = new IJEFetalDeath();
      ije.ICOD = "R836";
      FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal("R83.6", fetalDeathRecord2.CodedInitiatingFetalCOD);
    }

    [Fact]
    public void TestCodedOtherFetalCOD(){
      Assert.Null(SetterFetalDeathRecord.OCOD1);
      SetterFetalDeathRecord.OCOD1 = "R836";
      Assert.Equal("R836", SetterFetalDeathRecord.OCOD1);
      SetterFetalDeathRecord.OCOD1 = "R83.6";
      Assert.Equal("R83.6", SetterFetalDeathRecord.OCOD1);
      SetterFetalDeathRecord.OCOD1 = "R83.";
      Assert.Equal("R83.", SetterFetalDeathRecord.OCOD1);
      SetterFetalDeathRecord.OCOD1 = "R83";
      Assert.Equal("R83", SetterFetalDeathRecord.OCOD1);
      //parse
      FetalDeathRecord record = new FetalDeathRecord(File.ReadAllText(TestHelpers.FixturePath("fixtures/json/FetalDeathCauseOrConditionCodedContent.json")));
      Assert.Equal("P02.1", record.OCOD1);
      //set after parse
      record.OCOD1 = "R836";
      Assert.Equal("R836", record.OCOD1);

      IJEFetalDeath ije = new IJEFetalDeath();
      ije.OCOD1 = "R836";
      FetalDeathRecord fetalDeathRecord2 = ije.ToFetalDeathRecord();
      Assert.Equal("R83.6", fetalDeathRecord2.OCOD1);
    }

    [Fact]
    public void Test_GetCodedCauseOfFetalDeathBundle()
    {
      // Test with current single coded cause of fetal death record
      string[] recordFiles = { "fixtures/json/FetalDeathCauseOrConditionCodedContent.json" };
      foreach (var recordFile in recordFiles)
      {
        // Load the record
        FetalDeathRecord record = new(File.ReadAllText(TestHelpers.FixturePath(recordFile)), true);
        // Use it to generate a new record based on a new industry and occupation bundle and on the JSON output of that bundle
        Bundle newBundle = record.GetCodedCauseOfFetalDeathBundle();
        FetalDeathRecord newRecord = new(newBundle);
        FetalDeathRecord newRecordFromJSON = new(newBundle.ToJson());
        // Confirm that each new record contains the appropriate contents from the old record
        List<FetalDeathRecord> recordsToTest = new List<FetalDeathRecord> { newRecord, newRecordFromJSON };
        foreach (var testRecord in recordsToTest)
        {
          // Confirm identifier match
          Assert.Equal(record.RecordIdentifier, testRecord.RecordIdentifier);
          // Confirm relevant record information matches
          Assert.Equal(record.CodedInitiatingFetalCOD, testRecord.CodedInitiatingFetalCOD);
          Assert.Equal(record.OCOD1, testRecord.OCOD1);
          Assert.Equal(record.OCOD2, testRecord.OCOD2);
          Assert.Equal(record.OCOD3, testRecord.OCOD3);
          Assert.Equal(record.OCOD4, testRecord.OCOD4);
          Assert.Equal(record.OCOD5, testRecord.OCOD5);
          Assert.Equal(record.OCOD6, testRecord.OCOD6);
          Assert.Equal(record.OCOD7, testRecord.OCOD7);
        }
      }
    }

  }
}
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
    public void Set_HistologicalPlacentalExamination()
    {
        SetterFetalDeathRecord.HistologicalPlacentalExaminationPerformedHelper = "398166005";
        Assert.Equal("Performed", SetterFetalDeathRecord.HistologicalPlacentalExaminationPerformed["display"]);
        Dictionary<string, string> cc = new Dictionary<string, string>();
        cc.Add("code", BFDR.ValueSets.HistologicalPlacentalExamination.Codes[0, 0]);
        cc.Add("system", BFDR.ValueSets.HistologicalPlacentalExamination.Codes[0, 2]);
        cc.Add("display", BFDR.ValueSets.HistologicalPlacentalExamination.Codes[0, 1]);
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
        cc.Add("code", BFDR.ValueSets.HistologicalPlacentalExamination.Codes[0, 0]);
        cc.Add("system", BFDR.ValueSets.HistologicalPlacentalExamination.Codes[0, 2]);
        cc.Add("display", BFDR.ValueSets.HistologicalPlacentalExamination.Codes[0, 1]);
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
  }
}

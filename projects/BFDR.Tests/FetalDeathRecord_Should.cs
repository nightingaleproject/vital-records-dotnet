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
        // Height
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
  }
}

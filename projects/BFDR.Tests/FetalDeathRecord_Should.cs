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
  }
}

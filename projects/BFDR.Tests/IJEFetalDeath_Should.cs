using System.IO;
using Xunit;

namespace BFDR.Tests
{
  public class IJEFetalDeath_Should
  {

    [Fact]
    public void SetRegistrationDate()
    {
      FetalDeathRecord fhir = new FetalDeathRecord();
      IJEFetalDeath ije = new IJEFetalDeath(fhir);
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
    public void SetFirstPrenatalCare()
    {
      IJEFetalDeath ije = new();
      Assert.Equal("    ", ije.DOFP_YR);
      Assert.Equal("  ", ije.DOFP_MO);
      Assert.Equal("  ", ije.DOFP_DY);
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
    public void ParseFirstPrenatalCare()
    {
      IJEFetalDeath ije = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/FetalDeathRecord.ije")));
      Assert.Equal("2024", ije.DOFP_YR);
      Assert.Equal("03", ije.DOFP_MO);
      Assert.Equal("11", ije.DOFP_DY);
    }

    [Fact]
    public void SetDateLastLiveBirth()
    {
      IJEFetalDeath ije = new();
      Assert.Equal("  ", ije.MLLB);
      Assert.Equal("    ", ije.YLLB);
      ije.MLLB = "10";
      Assert.Equal("10", ije.MLLB);
      Assert.Equal("    ", ije.YLLB);
      ije.YLLB = "2016";
      Assert.Equal("10", ije.MLLB);
      Assert.Equal("2016", ije.YLLB);
    }

    [Fact]
    public void ParseDateLastLiveBirth()
    {
      IJEFetalDeath ije = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/FetalDeathRecord.ije")));
      Assert.Equal("05", ije.MLLB);
      Assert.Equal("2014", ije.YLLB);
    }

    [Fact]
    public void SetEstimatedTimeOfFetalDeath()
    {
      IJEFetalDeath ije = new();
      Assert.Equal("", ije.ETIME);
      ije.ETIME = "L";
      FetalDeathRecord record = ije.ToRecord();
      Assert.Equal("434671000124102", record.TimeOfFetalDeath["code"]);
      Assert.Equal("http://snomed.info/sct", record.TimeOfFetalDeath["system"]);
      Assert.Equal("L", new IJEFetalDeath(record).ETIME);
      ije.ETIME = "U";
      record = ije.ToRecord();
      Assert.Equal("UNK", record.TimeOfFetalDeathHelper);
      Assert.Equal("UNK", record.TimeOfFetalDeath["code"]);
      Assert.Equal("http://terminology.hl7.org/CodeSystem/v3-NullFlavor", record.TimeOfFetalDeath["system"]);
      Assert.Equal("U", new IJEFetalDeath(record).ETIME);
    }

    [Fact]
    public void ParseEstimatedTimeOfFetalDeath()
    {
      IJEFetalDeath ije = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/FetalDeathRecord.ije")));
      FetalDeathRecord record = ije.ToRecord();
      Assert.Equal("434631000124100", record.TimeOfFetalDeathHelper);
      Assert.Equal("434631000124100", record.TimeOfFetalDeath["code"]);
      Assert.Equal("http://snomed.info/sct", record.TimeOfFetalDeath["system"]);
      Assert.Equal("A", new IJEFetalDeath(record).ETIME);
    }

    [Fact]
    public void SetAndParseDeliveryPlace()
    {
      IJEFetalDeath ije = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/FetalDeathRecord.ije")));
      FetalDeathRecord record = ije.ToRecord();
      Assert.Equal("6", ije.DPLACE);
      Assert.Equal("67190003", record.DeliveryPhysicalLocationHelper);
      Assert.Equal("67190003", record.DeliveryPhysicalLocation["code"]);
      Assert.Equal("http://snomed.info/sct", record.DeliveryPhysicalLocation["system"]);
      ije.DPLACE = "4";
      Assert.Equal("408838003", ije.ToRecord().DeliveryPhysicalLocationHelper);
      Assert.Equal("4", ije.DPLACE);
    }
  }
}

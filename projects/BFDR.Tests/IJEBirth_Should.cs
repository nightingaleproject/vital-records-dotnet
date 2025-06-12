using System.IO;
using Xunit;

namespace BFDR.Tests
{
  public class IJEBirth_Should
  {
    [Fact]
    public void SetCategoryAllUnknown()
    {
      IJEBirth ije = new IJEBirth();
      ije.INDL = "U";
      ije.AUGL = "U";
      ije.STER = "U";
      ije.ANTB = "U";
      ije.CHOR = "U";
      ije.ESAN = "U";
      Assert.Equal("U", ije.INDL);
      Assert.Equal("U", ije.AUGL);
      Assert.Equal("U", ije.STER);
      Assert.Equal("U", ije.ANTB);
      Assert.Equal("U", ije.CHOR);
      Assert.Equal("U", ije.ESAN);
      BirthRecord record = ije.ToRecord();
      Assert.False(record.InductionOfLabor);
      Assert.False(record.AugmentationOfLabor);
      Assert.False(record.AdministrationOfSteroidsForFetalLungMaturation);
      Assert.False(record.AntibioticsAdministeredDuringLabor);
      Assert.False(record.Chorioamnionitis);
      Assert.False(record.EpiduralOrSpinalAnesthesia);
      Assert.False(record.NoCharacteristicsOfLaborAndDelivery);
    }

    [Fact]
    public void SetCategoryAllYes()
    {
      IJEBirth ije = new IJEBirth();
      ije.INDL = "Y";
      ije.AUGL = "Y";
      ije.STER = "Y";
      ije.ANTB = "Y";
      ije.CHOR = "Y";
      ije.ESAN = "Y";
      Assert.Equal("Y", ije.INDL);
      Assert.Equal("Y", ije.AUGL);
      Assert.Equal("Y", ije.STER);
      Assert.Equal("Y", ije.ANTB);
      Assert.Equal("Y", ije.CHOR);
      Assert.Equal("Y", ije.ESAN);
      BirthRecord record = ije.ToRecord();
      Assert.True(record.InductionOfLabor);
      Assert.True(record.AugmentationOfLabor);
      Assert.True(record.AdministrationOfSteroidsForFetalLungMaturation);
      Assert.True(record.AntibioticsAdministeredDuringLabor);
      Assert.True(record.Chorioamnionitis);
      Assert.True(record.EpiduralOrSpinalAnesthesia);
      Assert.False(record.NoCharacteristicsOfLaborAndDelivery);
    }

    [Fact]
    public void SetCategoryAllNo()
    {
      IJEBirth ije = new IJEBirth();
      ije.INDL = "N";
      ije.AUGL = "N";
      ije.STER = "N";
      ije.ANTB = "N";
      ije.CHOR = "N";
      ije.ESAN = "N";
      Assert.Equal("N", ije.INDL);
      Assert.Equal("N", ije.AUGL);
      Assert.Equal("N", ije.STER);
      Assert.Equal("N", ije.ANTB);
      Assert.Equal("N", ije.CHOR);
      Assert.Equal("N", ije.ESAN);
      BirthRecord record = ije.ToRecord();
      Assert.False(record.InductionOfLabor);
      Assert.False(record.AugmentationOfLabor);
      Assert.False(record.AdministrationOfSteroidsForFetalLungMaturation);
      Assert.False(record.AntibioticsAdministeredDuringLabor);
      Assert.False(record.Chorioamnionitis);
      Assert.False(record.EpiduralOrSpinalAnesthesia);
      Assert.True(record.NoCharacteristicsOfLaborAndDelivery);
    }

    [Fact]
    public void SetCategoryYesNo()
    {
      IJEBirth ije = new IJEBirth();
      ije.INDL = "Y";
      ije.AUGL = "N";
      ije.STER = "Y";
      ije.ANTB = "N";
      ije.CHOR = "Y";
      ije.ESAN = "N";
      Assert.Equal("Y", ije.INDL);
      Assert.Equal("N", ije.AUGL);
      Assert.Equal("Y", ije.STER);
      Assert.Equal("N", ije.ANTB);
      Assert.Equal("Y", ije.CHOR);
      Assert.Equal("N", ije.ESAN);
      BirthRecord record = ije.ToRecord();
      Assert.True(record.InductionOfLabor);
      Assert.False(record.AugmentationOfLabor);
      Assert.True(record.AdministrationOfSteroidsForFetalLungMaturation);
      Assert.False(record.AntibioticsAdministeredDuringLabor);
      Assert.True(record.Chorioamnionitis);
      Assert.False(record.EpiduralOrSpinalAnesthesia);
      Assert.False(record.NoCharacteristicsOfLaborAndDelivery);
    }

    [Fact]
    public void SetCategoryYesUnknown()
    {
      IJEBirth ije = new IJEBirth();
      ije.INDL = "Y";
      ije.AUGL = "U";
      ije.STER = "Y";
      ije.ANTB = "U";
      ije.CHOR = "Y";
      ije.ESAN = "U";
      Assert.Equal("Y", ije.INDL);
      Assert.Equal("N", ije.AUGL);
      Assert.Equal("Y", ije.STER);
      Assert.Equal("N", ije.ANTB);
      Assert.Equal("Y", ije.CHOR);
      Assert.Equal("N", ije.ESAN);
      BirthRecord record = ije.ToRecord();
      Assert.True(record.InductionOfLabor);
      Assert.False(record.AugmentationOfLabor);
      Assert.True(record.AdministrationOfSteroidsForFetalLungMaturation);
      Assert.False(record.AntibioticsAdministeredDuringLabor);
      Assert.True(record.Chorioamnionitis);
      Assert.False(record.EpiduralOrSpinalAnesthesia);
      Assert.False(record.NoCharacteristicsOfLaborAndDelivery);
    }

    [Fact]
    public void SetCategoryNoUnknown()
    {
      IJEBirth ije = new IJEBirth();
      ije.INDL = "N";
      ije.AUGL = "U";
      ije.STER = "N";
      ije.ANTB = "U";
      ije.CHOR = "N";
      ije.ESAN = "U";
      Assert.Equal("N", ije.INDL);
      Assert.Equal("N", ije.AUGL);
      Assert.Equal("N", ije.STER);
      Assert.Equal("N", ije.ANTB);
      Assert.Equal("N", ije.CHOR);
      Assert.Equal("N", ije.ESAN);
      BirthRecord record = ije.ToRecord();
      Assert.False(record.InductionOfLabor);
      Assert.False(record.AugmentationOfLabor);
      Assert.False(record.AdministrationOfSteroidsForFetalLungMaturation);
      Assert.False(record.AntibioticsAdministeredDuringLabor);
      Assert.False(record.Chorioamnionitis);
      Assert.False(record.EpiduralOrSpinalAnesthesia);
      Assert.True(record.NoCharacteristicsOfLaborAndDelivery);
    }

    [Fact]
    public void SetCategoryYesNoUnknown()
    {
      IJEBirth ije = new IJEBirth();
      ije.INDL = "Y";
      ije.AUGL = "N";
      ije.STER = "U";
      ije.ANTB = "Y";
      ije.CHOR = "N";
      ije.ESAN = "U";
      Assert.Equal("Y", ije.INDL);
      Assert.Equal("N", ije.AUGL);
      Assert.Equal("N", ije.STER);
      Assert.Equal("Y", ije.ANTB);
      Assert.Equal("N", ije.CHOR);
      Assert.Equal("N", ije.ESAN);
      BirthRecord record = ije.ToRecord();
      Assert.True(record.InductionOfLabor);
      Assert.False(record.AugmentationOfLabor);
      Assert.False(record.AdministrationOfSteroidsForFetalLungMaturation);
      Assert.True(record.AntibioticsAdministeredDuringLabor);
      Assert.False(record.Chorioamnionitis);
      Assert.False(record.EpiduralOrSpinalAnesthesia);
      Assert.False(record.NoCharacteristicsOfLaborAndDelivery);
    }
  }
}
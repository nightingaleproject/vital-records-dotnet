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

    [Fact]
    public void ParseIJEConnectathonTestData()
    {
      string rawIJE = new(File.ReadAllText(TestHelpers.FixturePath("fixtures/ije/ConnectathonFetalDeathRecord.ije")));
      IJEFetalDeath ije = new IJEFetalDeath(rawIJE);

      // Confirm the ije can be converted to fhir
      FetalDeathRecord record = ije.ToRecord();
      Assert.Equal(2024, record.DeliveryYear);
      Assert.Equal("Anwar", record.FetusFamilyName);

      // Confirm the ije fields are what we expect from the connectathon test record 1
      // A number of issues were found for some fields, see commented out lines below, some will not be implemented, others can be investigated further
      Assert.Equal("2024", ije.FDOD_YR);
      Assert.Equal("MA", ije.DSTATE);
      Assert.Equal("000008", ije.FILENO);
      Assert.Equal("0", ije.VOID);
      Assert.Equal("            ", ije.AUXNO);
      Assert.Equal("1830", ije.TD);
      Assert.Equal("M", ije.FSEX);
      Assert.Equal("12", ije.FDOD_MO);
      Assert.Equal("13", ije.FDOD_DY);
      Assert.Equal("209", ije.CNTYO);
      Assert.Equal("1", ije.DPLACE);
      Assert.Equal("", ije.FNPI.Trim());
      Assert.Equal("2040", ije.SFN);
      Assert.Equal("1998", ije.MDOB_YR);
      Assert.Equal("01", ije.MDOB_MO);
      Assert.Equal("14", ije.MDOB_DY);
      Assert.Equal("0", ije.MAGE_BYPASS);
      Assert.Equal("PA", ije.BPLACEC_ST_TER.Trim());
      Assert.Equal("FM", ije.BPLACEC_CNT);
      Assert.Equal("36000", ije.CITYC);
      Assert.Equal("209", ije.COUNTYC);
      Assert.Equal("DE", ije.STATEC);
      Assert.Equal("US", ije.COUNTRYC);
      Assert.Equal("Y", ije.LIMITS);
      Assert.Equal("2000", ije.FDOB_YR);
      Assert.Equal("10", ije.FDOB_MO);
      Assert.Equal("20", ije.FDOB_DY);
      Assert.Equal("0", ije.FAGE_BYPASS);
      Assert.Equal("", ije.MARE.Trim());
      Assert.Equal("", ije.MARN.Trim());
      Assert.Equal("9", ije.MEDUC);
      Assert.Equal("0", ije.MEDUC_BYPASS);
      Assert.Equal("U", ije.METHNIC1);
      Assert.Equal("U", ije.METHNIC2);
      Assert.Equal("U", ije.METHNIC3);
      Assert.Equal("U", ije.METHNIC4);
      Assert.Equal("", ije.METHNIC5.Trim());
      Assert.Equal("N", ije.MRACE1);
      Assert.Equal("N", ije.MRACE2);
      Assert.Equal("N", ije.MRACE3);
      Assert.Equal("N", ije.MRACE4);
      Assert.Equal("N", ije.MRACE5);
      Assert.Equal("N", ije.MRACE6);
      Assert.Equal("N", ije.MRACE7);
      Assert.Equal("N", ije.MRACE8);
      Assert.Equal("N", ije.MRACE9);
      Assert.Equal("N", ije.MRACE10);
      Assert.Equal("N", ije.MRACE11);
      Assert.Equal("N", ije.MRACE12);
      Assert.Equal("N", ije.MRACE13);
      Assert.Equal("Y", ije.MRACE14);
      Assert.Equal("N", ije.MRACE15);
      Assert.Equal("", ije.MRACE16.Trim());
      Assert.Equal("", ije.MRACE17.Trim());
      Assert.Equal("", ije.MRACE18.Trim());
      Assert.Equal("", ije.MRACE19.Trim());
      Assert.Equal("CHUUK", ije.MRACE20);
      Assert.Equal("", ije.MRACE21.Trim());
      Assert.Equal("", ije.MRACE22.Trim());
      Assert.Equal("", ije.MRACE23.Trim());
      Assert.Equal("", ije.MRACE1E.Trim());
      Assert.Equal("", ije.MRACE2E.Trim());
      Assert.Equal("", ije.MRACE3E.Trim());
      Assert.Equal("", ije.MRACE4E.Trim());
      Assert.Equal("", ije.MRACE5E.Trim());
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
      Assert.Equal("2", ije.ATTEND);
      Assert.Equal("", ije.TRAN.Trim());
      Assert.Equal("99", ije.DOFP_MO);
      Assert.Equal("99", ije.DOFP_DY);
      Assert.Equal("9999", ije.DOFP_YR);
      Assert.Equal("", ije.DOLP_MO.Trim());
      Assert.Equal("", ije.DOLP_DY.Trim());
      Assert.Equal("", ije.DOLP_YR.Trim());
      Assert.Equal("", ije.NPREV.Trim());
      Assert.Equal("", ije.NPREV_BYPASS.Trim());
      Assert.Equal("5", ije.HFT);
      Assert.Equal("03", ije.HIN);
      Assert.Equal("0", ije.HGT_BYPASS);
      Assert.Equal("182", ije.PWGT);
      Assert.Equal("0", ije.PWGT_BYPASS);
      Assert.Equal("U", ije.WIC);
      Assert.Equal("02", ije.PLBL);
      Assert.Equal("00", ije.PLBD);
      Assert.Equal("", ije.POPO.Trim());
      Assert.Equal("99", ije.MLLB);
      Assert.Equal("2022", ije.YLLB);
      Assert.Equal("", ije.MOPO.Trim());
      Assert.Equal("", ije.YOPO.Trim());
      Assert.Equal("00", ije.CIGPN);
      Assert.Equal("00", ije.CIGFN);
      Assert.Equal("00", ije.CIGSN);
      Assert.Equal("00", ije.CIGLN);
      Assert.Equal("9999", ije.DLMP_YR);
      Assert.Equal("99", ije.DLMP_MO);
      Assert.Equal("99", ije.DLMP_DY);
      Assert.Equal("N", ije.PDIAB);
      Assert.Equal("N", ije.GDIAB);
      Assert.Equal("N", ije.PHYPE);
      Assert.Equal("N", ije.GHYPE);
      Assert.Equal("", ije.PPB.Trim());
      Assert.Equal("", ije.PPO.Trim());
      // Assert.Equal("N", ije.VB); // VB is not implemented
      Assert.Equal("N", ije.INFT);
      Assert.Equal("N", ije.PCES);
      Assert.Equal("00", ije.NPCES);
      Assert.Equal("0", ije.NPCES_BYPASS);
      Assert.Equal("", ije.GON.Trim());
      Assert.Equal("", ije.SYPH.Trim());
      // Assert.Equal("N", ije.HSV); // HSV is not implemented
      Assert.Equal("", ije.CHAM.Trim());
      Assert.Equal("", ije.LM.Trim());
      Assert.Equal("", ije.GBS.Trim());
      Assert.Equal("", ije.CMV.Trim());
      Assert.Equal("", ije.B19.Trim());
      Assert.Equal("", ije.TOXO.Trim());
      Assert.Equal("", ije.OTHERI.Trim());
      // Assert.Equal("N", ije.ATTF); // ATTF is not implemented
      // Assert.Equal("N", ije.ATTV); // ATTV is not implemented
      Assert.Equal("3", ije.PRES);
      Assert.Equal("1", ije.ROUT);
      Assert.Equal("U", ije.TLAB); // X can't roundtrip, TODO consider changing this to N or Y so the record can be roundtripped
      Assert.Equal("", ije.HYST.Trim());
      Assert.Equal("", ije.MTR.Trim());
      Assert.Equal("", ije.PLAC.Trim());
      Assert.Equal("N", ije.RUT);
      Assert.Equal("", ije.UHYS.Trim());
      Assert.Equal("N", ije.AINT);
      Assert.Equal("", ije.UOPR.Trim());
      Assert.Equal("0999", ije.FWG);
      Assert.Equal("0", ije.FW_BYPASS);
      Assert.Equal("32", ije.OWGEST);
      Assert.Equal("0", ije.OWGEST_BYPASS);
      Assert.Equal("N", ije.ETIME);
      Assert.Equal("N", ije.AUTOP);
      Assert.Equal("Y", ije.HISTOP);
      Assert.Equal("N", ije.AUTOPF);
      Assert.Equal("01", ije.PLUR);
      Assert.Equal("99", ije.SORD);
      Assert.Equal("99", ije.FDTH);
      // Assert.Equal("999999", ije.MATCH); // not implemented in FHIR
      Assert.Equal("0", ije.PLUR_BYPASS);
      Assert.Equal("", ije.ANEN.Trim());
      Assert.Equal("", ije.MNSB.Trim());
      Assert.Equal("", ije.CCHD.Trim());
      Assert.Equal("", ije.CDH.Trim());
      Assert.Equal("", ije.OMPH.Trim());
      Assert.Equal("", ije.GAST.Trim());
      Assert.Equal("", ije.LIMB.Trim());
      Assert.Equal("", ije.CL.Trim());
      Assert.Equal("", ije.CP.Trim());
      Assert.Equal("", ije.DOWT.Trim());
      Assert.Equal("", ije.CDIT.Trim());
      Assert.Equal("", ije.HYPO.Trim());
      Assert.Equal("", ije.R_YR.Trim());
      Assert.Equal("", ije.R_MO.Trim());
      Assert.Equal("", ije.R_DY.Trim());
      Assert.Equal("26", ije.MAGER);
      Assert.Equal("23", ije.FAGER);
      Assert.Equal("N", ije.EHYPE);
      // Assert.Equal("X", ije.INFT_DRG); // should this be X? we can't round trip X or U for these fields, consider changing to N for testing purposes and determine if this is an issue
      // Assert.Equal("X", ije.INFT_ART); // should this be X?
      Assert.Equal("2024", ije.DOR_YR);
      Assert.Equal("12", ije.DOR_MO);
      Assert.Equal("13", ije.DOR_DY);
      Assert.Equal("N", ije.COD18a1);
      Assert.Equal("N", ije.COD18a2);
      Assert.Equal("N", ije.COD18a3);
      Assert.Equal("N", ije.COD18a4);
      Assert.Equal("N", ije.COD18a5);
      Assert.Equal("N", ije.COD18a6);
      Assert.Equal("N", ije.COD18a7);
      Assert.Equal("NO PRENATAL CARE", ije.COD18a8.Trim());
      Assert.Equal("", ije.COD18a9.Trim());
      Assert.Equal("", ije.COD18a10.Trim());
      Assert.Equal("", ije.COD18a11.Trim());
      Assert.Equal("99          0", ije.COD18a12.Trim());
      Assert.Equal("", ije.COD18a13.Trim());
      Assert.Equal("", ije.COD18a13.Trim());
      Assert.Equal("", ije.COD18a14.Trim());
      Assert.Equal("N", ije.COD18b1);
      Assert.Equal("N", ije.COD18b2);
      Assert.Equal("N", ije.COD18b3);
      Assert.Equal("N", ije.COD18b4);
      Assert.Equal("N", ije.COD18b5);
      Assert.Equal("N", ije.COD18b6);
      Assert.Equal("N", ije.COD18b7);
      Assert.Equal("RECENT XXXXXXXXXXX", ije.COD18b8.Trim());
      Assert.Equal("", ije.COD18b9.Trim());
      Assert.Equal("", ije.COD18b10.Trim());
      Assert.Equal("NONE IDENTIFIED", ije.COD18b11.Trim());
      Assert.Equal("", ije.COD18b12.Trim());
      Assert.Equal("", ije.COD18b13.Trim());
      Assert.Equal("INTRAUTERINE FETAL DEMISE", ije.COD18b14.Trim());
      Assert.Equal("", ije.ICOD.Trim());
      Assert.Equal("", ije.OCOD1.Trim());
      Assert.Equal("", ije.OCOD2.Trim());
      Assert.Equal("", ije.OCOD3.Trim());
      Assert.Equal("", ije.OCOD4.Trim());
      Assert.Equal("", ije.OCOD5.Trim());
      Assert.Equal("", ije.OCOD6.Trim());
      Assert.Equal("", ije.OCOD7.Trim());
      Assert.Equal("", ije.HSV1.Trim());
      Assert.Equal("", ije.HIV.Trim());
      Assert.Equal("", ije.ALCOHOL.Trim());
      Assert.Equal("Zeke", ije.FETFNAME.Trim());
      Assert.Equal("", ije.FETMNAME.Trim());
      Assert.Equal("Anwar", ije.FETLNAME.Trim());
      Assert.Equal("", ije.SUFFIX.Trim());
      Assert.Equal("", ije.ALIAS.Trim());
      Assert.Equal("", ije.HOSP_D.Trim());
      Assert.Equal("", ije.STNUM_D.Trim());
      Assert.Equal("", ije.PREDIR_D.Trim());
      Assert.Equal("", ije.STNAME_D.Trim());
      Assert.Equal("", ije.STDESIG_D.Trim());
      Assert.Equal("", ije.POSTDIR_D.Trim());
      Assert.Equal("", ije.APTNUMB_D.Trim());
      Assert.Equal("", ije.ADDRESS_D.Trim());
      Assert.Equal("", ije.ZIPCODE_D.Trim());
      Assert.Equal("", ije.CNTY_D.Trim());
      Assert.Equal("", ije.CITY_D.Trim());
      Assert.Equal("", ije.STATE_D.Trim());
      Assert.Equal("", ije.COUNTRY_D.Trim());
      Assert.Equal("", ije.LONG_D.Trim());
      Assert.Equal("", ije.LAT_D.Trim());
      Assert.Equal("", ije.MOMFNAME.Trim());
      Assert.Equal("", ije.MOMMNAME.Trim());
      Assert.Equal("", ije.MOMLNAME.Trim());
      Assert.Equal("", ije.MOMSUFFIX.Trim());
      Assert.Equal("", ije.MOMFMNME.Trim());
      Assert.Equal("", ije.MOMMMID.Trim());
      Assert.Equal("", ije.MOMMAIDN.Trim());
      Assert.Equal("", ije.MOMMSUFFIX.Trim());
      Assert.Equal("XXXX", ije.STNUM.Trim());
      Assert.Equal("S", ije.PREDIR.Trim());
      Assert.Equal("14 TH", ije.STNAME.Trim());
      Assert.Equal("ST", ije.STDESIG.Trim());
      Assert.Equal("", ije.POSTDIR.Trim());
      Assert.Equal("", ije.APTNUMB.Trim());
      Assert.Equal("XXXX S 14 TH ST", ije.ADDRESS.Trim());
      Assert.Equal("XXXXX", ije.ZIPCODE.Trim());
      Assert.Equal("XXXXXXXXX", ije.COUNTYTXT.Trim());
      Assert.Equal("xxxxxx xxxx", ije.CITYTXT.Trim());
      Assert.Equal("Delaware", ije.STATETXT.Trim());
      Assert.Equal("United States", ije.CNTRYTXT.Trim());
      Assert.Equal("", ije.LONG.Trim());
      Assert.Equal("", ije.LAT.Trim());
      Assert.Equal("", ije.DADFNAME.Trim());
      Assert.Equal("", ije.DADMNAME.Trim());
      Assert.Equal("", ije.DADLNAME.Trim());
      Assert.Equal("", ije.DADSUFFIX.Trim());
      Assert.Equal("", ije.MOM_SSN.Trim());
      Assert.Equal("", ije.DAD_SSN.Trim());
      Assert.Equal("", ije.MAGE_CALC.Trim());
      Assert.Equal("", ije.FAGE_CALC.Trim());
      Assert.Equal("", ije.MOM_OC_T.Trim());
      Assert.Equal("", ije.MOM_OC_C.Trim());
      Assert.Equal("", ije.DAD_OC_T.Trim());
      Assert.Equal("", ije.DAD_OC_C.Trim());
      Assert.Equal("", ije.MOM_IN_T.Trim());
      Assert.Equal("", ije.MOM_IN_C.Trim());
      Assert.Equal("", ije.DAD_IN_T.Trim());
      Assert.Equal("", ije.DAD_IN_C.Trim());
      Assert.Equal("", ije.FBPLACD_ST_TER_C.Trim());
      Assert.Equal("", ije.FBPLACE_CNT_C.Trim());
      // Assert.Equal("", ije.MBPLACE_ST_TER_TXT.Trim()); // roundtrip pulls from the code to text translation for the BLPACE_ST_TER field so this isn't rountripping
      // Assert.Equal("", ije.MBPLACE_CNTRY_TXT.Trim()); // null error resolved with padding, but pulls from the code to text translation so there is a mis match
      Assert.Equal("", ije.FBPLACE_ST_TER_TXT.Trim());
      Assert.Equal("", ije.FBPLACE_CNTRY_TXT.Trim());
      Assert.Equal("", ije.FEDUC.Trim());
      Assert.Equal("", ije.FEDUC_BYPASS.Trim());
      Assert.Equal("U", ije.FETHNIC1.Trim()); // blanks become u when round tripped, is this correct?
      Assert.Equal("U", ije.FETHNIC2.Trim()); // blanks become u when round tripped, is this correct?
      Assert.Equal("U", ije.FETHNIC3.Trim()); // blanks become u when round tripped, is this correct?
      Assert.Equal("U", ije.FETHNIC4.Trim()); // blanks become u when round tripped, is this correct?
      Assert.Equal("", ije.FETHNIC5.Trim());
      Assert.Equal("", ije.FRACE1.Trim());
      Assert.Equal("", ije.FRACE2.Trim());
      Assert.Equal("", ije.FRACE3.Trim());
      Assert.Equal("", ije.FRACE4.Trim());
      Assert.Equal("", ije.FRACE5.Trim());
      Assert.Equal("", ije.FRACE6.Trim());
      Assert.Equal("", ije.FRACE7.Trim());
      Assert.Equal("", ije.FRACE8.Trim());
      Assert.Equal("", ije.FRACE9.Trim());
      Assert.Equal("", ije.FRACE10.Trim());
      Assert.Equal("", ije.FRACE11.Trim());
      Assert.Equal("", ije.FRACE12.Trim());
      Assert.Equal("", ije.FRACE13.Trim());
      Assert.Equal("", ije.FRACE14.Trim());
      Assert.Equal("", ije.FRACE15.Trim());
      Assert.Equal("", ije.FRACE16.Trim());
      Assert.Equal("", ije.FRACE17.Trim());
      Assert.Equal("", ije.FRACE18.Trim());
      Assert.Equal("", ije.FRACE19.Trim());
      Assert.Equal("", ije.FRACE20.Trim());
      Assert.Equal("", ije.FRACE21.Trim());
      Assert.Equal("", ije.FRACE22.Trim());
      Assert.Equal("", ije.FRACE23.Trim());
      Assert.Equal("", ije.FRACE1E.Trim());
      Assert.Equal("", ije.FRACE2E.Trim());
      Assert.Equal("", ije.FRACE3E.Trim());
      Assert.Equal("", ije.FRACE4E.Trim());
      Assert.Equal("", ije.FRACE5E.Trim());
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
      Assert.Equal("", ije.METHNIC5C.Trim());
      Assert.Equal("", ije.METHNICE.Trim());
      Assert.Equal("", ije.MRACEBG_C.Trim());
      Assert.Equal("", ije.FETHNIC5C.Trim());
      Assert.Equal("", ije.FETHNICE.Trim());
      Assert.Equal("", ije.FRACEBG_C.Trim());
      Assert.Equal("", ije.METHNIC_T.Trim());
      Assert.Equal("", ije.MRACE_T.Trim());
      Assert.Equal("", ije.FETHNIC_T.Trim());
      Assert.Equal("", ije.FRACE_T.Trim());
      Assert.Equal("", ije.HOSPFROM.Trim());
      Assert.Equal("", ije.ATTEND_NAME.Trim());
      Assert.Equal("", ije.ATTEND_NPI.Trim());
      Assert.Equal("", ije.ATTEND_OTH_TXT.Trim());
      Assert.Equal("", ije.INFORMFST.Trim());
      Assert.Equal("", ije.INFORMMID.Trim());
      Assert.Equal("", ije.INFORMLST.Trim());
      Assert.Equal("", ije.INFORMRELATE.Trim());
      Assert.Equal("", ije.CERTIFIED_YR.Trim());
      Assert.Equal("", ije.CERTIFIED_MO.Trim());
      Assert.Equal("", ije.CERTIFIED_DY.Trim());
      Assert.Equal("", ije.REGISTER_YR.Trim());
      Assert.Equal("", ije.REGISTER_MO.Trim());
      Assert.Equal("", ije.REGISTER_DY.Trim());
      // Assert.Equal("0", ije.REPLACE); // Not implemented
      Assert.Equal("", ije.PLACE1_1.Trim());
      Assert.Equal("", ije.PLACE1_2.Trim());
      Assert.Equal("", ije.PLACE1_3.Trim());
      Assert.Equal("", ije.PLACE1_4.Trim());
      Assert.Equal("", ije.PLACE1_5.Trim());
      Assert.Equal("", ije.PLACE1_6.Trim());
      Assert.Equal("", ije.PLACE8_1.Trim());
      Assert.Equal("", ije.PLACE8_2.Trim());
      Assert.Equal("", ije.PLACE8_3.Trim());
      Assert.Equal("", ije.PLACE20.Trim());
      Assert.Equal("", ije.BLANK.Trim());
      Assert.Equal("", ije.BLANK2.Trim());
    }

    [Fact]
    public void TestMotherDateOfBirthRoundtrip()
    {
      IJEFetalDeath ije = new IJEFetalDeath();
      ije.MDOB_YR = "1992";
      ije.MDOB_MO = "01";
      ije.MDOB_DY = "12";
      // convert IJE to FHIR
      FetalDeathRecord fd = ije.ToRecord();
      Assert.Equal(1992, fd.MotherBirthYear);
      Assert.Equal(1, fd.MotherBirthMonth);
      Assert.Equal(12, fd.MotherBirthDay);

      // then to a json string
      string asJson = fd.ToJSON();
      // Create a fhir record from the json
      FetalDeathRecord fdRecord = new FetalDeathRecord(asJson);
      Assert.Equal(1992, fdRecord.MotherBirthYear);
      Assert.Equal(1, fdRecord.MotherBirthMonth);
      Assert.Equal(12, fdRecord.MotherBirthDay);

      // convert back to IJE and confirm the values are the same
      IJEFetalDeath ije2 = new IJEFetalDeath(fdRecord);
      Assert.Equal("1992", ije2.MDOB_YR);
      Assert.Equal("01", ije2.MDOB_MO);
      Assert.Equal("12", ije2.MDOB_DY);
    }


  [Fact]
    public void TestDeathState()
    {
      IJEFetalDeath ije = new IJEFetalDeath();
      ije.DSTATE = "HI";
      FetalDeathRecord dr = ije.ToRecord();
      Assert.Equal("HI", dr.EventLocationJurisdiction);
      ije.DSTATE = "TT";
      Assert.Equal("TT", ije.DSTATE);
      dr = ije.ToRecord();
      Assert.Equal("TT", dr.EventLocationJurisdiction);
      ije.DSTATE = "TS";
      Assert.Equal("TS", ije.DSTATE);
      dr = ije.ToRecord();
      Assert.Equal("TS", dr.EventLocationJurisdiction);
      ije.DSTATE = "ZZ";
      Assert.Equal("ZZ", ije.DSTATE);
      dr = ije.ToRecord();
      Assert.Equal("ZZ", dr.EventLocationJurisdiction);
    }
}
}
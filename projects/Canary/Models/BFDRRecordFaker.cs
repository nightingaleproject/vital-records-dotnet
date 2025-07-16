using System;
using System.Collections.Generic;
using Bogus.Extensions.UnitedStates;
using BFDR;
using System.Linq;

namespace canary.Models
{
  /// <summary>Class <c>BirthRecordFaker</c> can be used to generate synthetic <c>BirthRecord</c>s. Various
  /// options are available to tailoring the records generated to specific use case by the class.
  /// </summary>
  public class BirthRecordFaker : RecordFaker<BirthRecord>
  {
    public BirthRecordFaker(string state = "XX", string sex = "Male") : base(state, sex) { }

    /// <summary>Return a new record populated with fake data.</summary>
    public override BirthRecord Generate(bool simple = false)
    {
      BirthRecord record = new();
      Bogus.Faker faker = new("en");
      BFDRRecordFaker.GenerateBFDRAttributes(record, state, simple, faker);
      Bogus.DataSets.Name.Gender gender = sex == "Male" ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female;
      record.ChildGivenNames = new string[] { faker.Name.FirstName(gender), faker.Name.FirstName(gender) };
      record.ChildFamilyName = faker.Name.LastName(gender);
      record.ChildSuffix = faker.Name.Suffix();
      record.BirthSex = gender.ToString().Substring(0, 1);
      if (faker.Random.Bool())
      {
        record.MotherPrepregnancyWeight = faker.Random.Number(100, 200);
        record.MotherWeightAtDelivery = record.MotherPrepregnancyWeight + faker.Random.Number(7, 12);
      }
      // set more demographic and record keeping info
      DateTime birth = faker.Date.Recent();
      DateTimeOffset birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
      record.DateOfBirth = birthUtc.ToString("yyyy-MM-dd");
      birth = faker.Date.Past(123, DateTime.Today.AddYears(-18));
      birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
      record.FatherDateOfBirth = birthUtc.ToString("yyyy-MM-dd");
      birth = faker.Date.Past(123, DateTime.Today.AddYears(-18));
      birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
      record.FatherDateOfBirth = birthUtc.ToString("yyyy-MM-dd");

      return record;
    }
  }

  /// <summary>Class <c>FetalDeathFaker</c> can be used to generate synthetic <c>BirthRecord</c>s. Various
  /// options are available to tailoring the records generated to specific use case by the class.
  /// </summary>
  public class FetalDeathFaker : RecordFaker<FetalDeathRecord>
  {
    public FetalDeathFaker(string state = "XX", string sex = "Male") : base(state, sex) { }

    /// <summary>Return a new record populated with fake data.</summary>
    public override FetalDeathRecord Generate(bool simple = false)
    {
      FetalDeathRecord record = new();
      Bogus.Faker faker = new("en");
      BFDRRecordFaker.GenerateBFDRAttributes(record, state, simple, faker);
      // Grab Gender enum value
      Bogus.DataSets.Name.Gender gender = sex == "Male" ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female;
      record.FetusGivenNames = new string[] { faker.Name.FirstName(gender), faker.Name.FirstName(gender) };
      record.FetusFamilyName = faker.Name.LastName(gender);
      record.FetusSuffix = faker.Name.Suffix();
      record.FetalDeathSex = gender.ToString().Substring(0, 1);
      DateTime birth = faker.Date.Recent();
      DateTimeOffset birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
      record.DateOfDelivery = birthUtc.ToString("yyyy-MM-dd");
      birth = faker.Date.Past(123, DateTime.Today.AddYears(-18));
      birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
      record.FatherDateOfBirth = birthUtc.ToString("yyyy-MM-dd");
      birth = faker.Date.Past(123, DateTime.Today.AddYears(-18));
      birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
      record.FatherDateOfBirth = birthUtc.ToString("yyyy-MM-dd");
      int initiatingCause = faker.Random.Number(0, 7);
      Console.WriteLine(initiatingCause);
      switch (initiatingCause)
      {
        case 0:
          record.PrematureRuptureOfMembranes = true;
          break;
        case 1:
          record.AbruptioPlacenta = true;
          break;
        case 3:
          record.PlacentalInsufficiency = true;
          break;
        case 4:
          record.ProlapsedCord = true;
          break;
        case 5:
          record.ChorioamnionitisCOD = true;
          break;
        case 6:
          record.OtherComplicationsOfPlacentaCordOrMembranes = true;
          break;
        case 7:
          record.InitiatingCauseOrConditionUnknown = true;
          break;
      }
      return record;
    }
  }

  static class BFDRRecordFaker
  {
    public static void GenerateBFDRAttributes(NatalityRecord record, string state, bool simple, Bogus.Faker faker)
    {
      record.EventLocationJurisdiction = state;
      record.CertificateNumber = Convert.ToString(faker.Random.Number(100000, 999999));
      record.InfantMedicalRecordNumber = Convert.ToString(faker.Random.Number(10000, 99999));
      record.MotherMedicalRecordNumber = Convert.ToString(faker.Random.Number(10000, 99999));
      record.MotherSocialSecurityNumber = faker.Person.Ssn();
      record.FatherSocialSecurityNumber = faker.Person.Ssn();
      record.MotherPrepregnancyWeightEditFlagHelper = "1";
      record.MotherWeightAtDeliveryEditFlagHelper = "0";
      record.BirthWeightEditFlagHelper = "2";
      record.MotherIndustry = "mother";
      record.FatherIndustry = "father";
      record.MotherOccupation = "CEO";
      record.FatherOccupation = "COO";
      record.PlaceOfBirth = new Dictionary<string, string>
          {
            { "addressLine1", $"{faker.Random.Number(999) + 1} Main Street" },
            { "addressCity", "Springfield" },
            // birthAddress.Add("addressCounty", "Hampden");
            { "addressState", state },
            // birthAddress.Add("addressZip", "01101");
            { "addressCountry", "US" }
          };
      // set some boolean fields
      if (faker.Random.Bool())
      {
        record.NoSpecifiedAbnormalConditionsOfNewborn = faker.Random.Bool();
        if (!record.NoSpecifiedAbnormalConditionsOfNewborn)
        {
          record.Seizure = faker.Random.Bool();
        }
      }
      if (faker.Random.Bool())
      {
        record.NoCharacteristicsOfLaborAndDelivery = faker.Random.Bool();
        if (!record.NoCharacteristicsOfLaborAndDelivery)
        {
          record.Chorioamnionitis = faker.Random.Bool();
          record.EpiduralOrSpinalAnesthesia = faker.Random.Bool();
        }
      }
      if (faker.Random.Bool())
      {
        record.NoMaternalMorbidities = faker.Random.Bool();
        if (!record.NoMaternalMorbidities)
        {
          record.MaternalTransfusion = faker.Random.Bool();
          record.PerinealLaceration = faker.Random.Bool();
        }
      }
      if (faker.Random.Bool())
      {
        record.NoInfectionsPresentDuringPregnancy = faker.Random.Bool();
        if (!record.NoInfectionsPresentDuringPregnancy)
        {
          if (record is BirthRecord birthRecord)
          {
            birthRecord.Gonorrhea = faker.Random.Bool();
            birthRecord.Syphilis = faker.Random.Bool();
          }
          else if (record is FetalDeathRecord fetalDeathRecord)
          {
            fetalDeathRecord.HepatitisB = faker.Random.Bool();
          }
        }
      }
      if (faker.Random.Bool())
      {
        if (record is BirthRecord birthRecord)
        {
          birthRecord.NoCongenitalAnomaliesOfTheNewborn = faker.Random.Bool();
          if (!birthRecord.NoCongenitalAnomaliesOfTheNewborn)
          {
            birthRecord.CleftLipWithOrWithoutCleftPalate = faker.Random.Bool();
            birthRecord.Omphalocele = faker.Random.Bool();
          }
        }

      }
      if (faker.Random.Bool())
      {
        record.NoPregnancyRiskFactors = faker.Random.Bool();
        if (!record.NoPregnancyRiskFactors)
        {
          record.GestationalDiabetes = faker.Random.Bool();
          record.PreviousCesarean = faker.Random.Bool();
        }
      }
      // set some numerical fields
      if (faker.Random.Bool())
      {
        record.ApgarScoreFiveMinutes = faker.Random.Number(0, 10);
        if (record.ApgarScoreFiveMinutes < 6)
        {
          record.ApgarScoreTenMinutes = faker.Random.Number(0, 10);
        }
      }
      if (faker.Random.Bool())
      {
        record.BirthWeight = faker.Random.Number(2000, 4000);
      }
      if (faker.Random.Bool())
      {
        record.MotherHeight = faker.Random.Number(60, 72);
      }
      if (faker.Random.Bool())
      {
        record.CigarettesPerDayInThreeMonthsPriorToPregancy = faker.Random.Number(0, 5);
        record.CigarettesPerDayInFirstTrimester = faker.Random.Number(0, 3);
        record.CigarettesPerDayInSecondTrimester = faker.Random.Number(0, 2);
        record.CigarettesPerDayInLastTrimester = faker.Random.Number(0, 1);
      }
      if (faker.Random.Bool())
      {
        if (faker.Random.Bool())
        {
          record.MotherReceivedWICFoodHelper = "Y";
        }
        else
        {
          record.MotherReceivedWICFoodHelper = "N";
        }
      }
      // Occupation / Industry
      Tuple<string, string>[] occupationIndustries =
      {
              Tuple.Create("Secretary", "State agency"),
              Tuple.Create("Carpenter", "Construction"),
              Tuple.Create("Programmer", "Health Insurance"),
              Tuple.Create("Lawyer", "Legal Services"),
              Tuple.Create("Scientist", "Public Health"),
              Tuple.Create("Baker", "Production"),
              Tuple.Create("Flight Attendant", "Transportation"),
              Tuple.Create("Barber", "Personal Care and Service"),
              Tuple.Create("Firefighter", "Protective Service"),
          };
      Tuple<string, string> occIndFather = faker.Random.ArrayElement<Tuple<string, string>>(occupationIndustries);
      Tuple<string, string> occIndMother = faker.Random.ArrayElement<Tuple<string, string>>(occupationIndustries);
      record.MotherIndustry = occIndMother.Item2;
      record.FatherIndustry = occIndFather.Item2;
      record.MotherOccupation = occIndMother.Item1;
      record.FatherOccupation = occIndFather.Item1;
      record.MotherEducationLevelHelper = VR.ValueSets.EducationLevel.Codes[faker.Random.Number(VR.ValueSets.EducationLevel.Codes.GetLength(0) - 1), 0];
      record.FatherEducationLevelHelper = VR.ValueSets.EducationLevel.Codes[faker.Random.Number(VR.ValueSets.EducationLevel.Codes.GetLength(0) - 1), 0];

      // Ethnicity
      if (faker.Random.Bool() && !simple)
      {
        record.MotherEthnicity1Helper = VR.ValueSets.YesNoUnknown.No;
        record.MotherEthnicity2Helper = VR.ValueSets.YesNoUnknown.No;
        record.MotherEthnicity3Helper = VR.ValueSets.YesNoUnknown.Yes;
        record.MotherEthnicity4Helper = VR.ValueSets.YesNoUnknown.No;
        record.FatherEthnicity1Helper = VR.ValueSets.YesNoUnknown.No;
        record.FatherEthnicity2Helper = VR.ValueSets.YesNoUnknown.Yes;
        record.FatherEthnicity3Helper = VR.ValueSets.YesNoUnknown.No;
        record.FatherEthnicity4Helper = VR.ValueSets.YesNoUnknown.No;
        record.MotherEthnicityLiteral = "";
        record.FatherEthnicityLiteral = "";
      }
      else
      {
        record.MotherEthnicity1Helper = VR.ValueSets.YesNoUnknown.No;
        record.MotherEthnicity2Helper = VR.ValueSets.YesNoUnknown.No;
        record.FatherEthnicity1Helper = VR.ValueSets.YesNoUnknown.No;
        record.FatherEthnicity2Helper = VR.ValueSets.YesNoUnknown.No;
      }
      // Race
      string[] nvssRaces = VR.NvssRace.GetBooleanRaceCodes().Concat(VR.NvssRace.GetLiteralRaceCodes()).ToArray();
      if (!simple)
      {
        string race1 = faker.Random.ArrayElement(nvssRaces);
        string race2 = faker.Random.ArrayElement(nvssRaces);
        string race3 = faker.Random.ArrayElement(nvssRaces);
        string race4 = faker.Random.ArrayElement(nvssRaces);
        record.MotherRace = nvssRaces.Select(raceCode =>
        {
          if (raceCode == race1 || raceCode == race2)
          {
            return Tuple.Create(raceCode, "Y");
          }
          return Tuple.Create(raceCode, "N");
        }).ToArray();
        record.FatherRace = nvssRaces.Select(raceCode =>
        {
          if (raceCode == race3 || raceCode == race4)
          {
            return Tuple.Create(raceCode, "Y");
          }
          return Tuple.Create(raceCode, "N");
        }).ToArray();
      }
      else
      {
        string race1 = faker.Random.ArrayElement(nvssRaces);
        string race2 = faker.Random.ArrayElement(nvssRaces);
        record.MotherRace = nvssRaces.Select(raceCode =>
        {
          if (raceCode == race1)
          {
            return Tuple.Create(raceCode, "Y");
          }
          return Tuple.Create(raceCode, "N");
        }).ToArray();
        record.FatherRace = nvssRaces.Select(raceCode =>
        {
          if (raceCode == race2)
          {
            return Tuple.Create(raceCode, "Y");
          }
          return Tuple.Create(raceCode, "N");
        }).ToArray();
      }
    }
  }
}

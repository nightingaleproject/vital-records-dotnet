using System.Collections.Generic;
using Xunit;
using BFDR;
using canary.Models;
using VR;
using System.Linq;
using System;

namespace canary.tests
{
  public class BFDRRecordFakerTests
  {
    readonly List<string> raceCodes = NvssRace.GetBooleanRaceCodes().Concat(NvssRace.GetLiteralRaceCodes()).ToList();

    [Fact]
    public void GenerateRaceAttributes()
    {
      BirthRecordFaker faker = new();
      BirthRecord record = faker.Generate();
      // all race codes should be present
      foreach (string raceCode in raceCodes)
      {
        Assert.Contains(record.FatherRace, element => element.Item1 == raceCode);
        Assert.Contains(record.MotherRace, element => element.Item1 == raceCode);
      }
      // one or two values should be Y
      List<Tuple<string, string>> FatherY = record.FatherRace.Where(element => element.Item2 == "Y").ToList();
      Assert.InRange(FatherY.Count, 1, 2);
      List<Tuple<string, string>> MotherY = record.MotherRace.Where(element => element.Item2 == "Y").ToList();
      Assert.InRange(MotherY.Count, 1, 2);
    }

    [Fact]
    public void GenerateSimpleRaceAttributes()
    {
      BirthRecordFaker faker = new();
      BirthRecord record = faker.Generate(true);
      // all race codes should be present
      foreach (string raceCode in raceCodes)
      {
        Assert.Contains(record.FatherRace, element => element.Item1 == raceCode);
        Assert.Contains(record.MotherRace, element => element.Item1 == raceCode);
      }
      // one value should be Y
      List<Tuple<string, string>> FatherY = record.FatherRace.Where(element => element.Item2 == "Y").ToList();
      Assert.Single(FatherY);
      List<Tuple<string, string>> MotherY = record.MotherRace.Where(element => element.Item2 == "Y").ToList();
      Assert.Single(MotherY);
    }
  }
}
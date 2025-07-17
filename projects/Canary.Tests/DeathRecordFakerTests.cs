using System.Collections.Generic;
using System.Linq;
using Xunit;
using VRDR;
using canary.Models;
using System;

namespace canary.tests
{
  public class DeathRecordFakerTests
  {
    readonly List<string> raceCodes = NvssRace.GetBooleanRaceCodes();

    [Fact]
    public void GenerateRaceAttributes()
    {
      DeathRecordFaker faker = new();
      DeathRecord record = faker.Generate();
      // all boolean race codes should be present
      foreach (string raceCode in raceCodes)
      {
        Assert.Contains(record.Race, element => element.Item1 == raceCode);
      }
      // one to three values should be Y
      List<Tuple<string, string>> decedentRace = record.Race.Where(element => element.Item2 == "Y").ToList();
      Assert.InRange(decedentRace.Count, 1, 3);
    }

    [Fact]
    public void GenerateSimpleRaceAttributes()
    {
      DeathRecordFaker faker = new();
      DeathRecord record = faker.Generate(true);
      // all boolean race codes should be present
      foreach (string raceCode in raceCodes)
      {
        Assert.Contains(record.Race, element => element.Item1 == raceCode);
      }
      // one value should be Y
      List<Tuple<string, string>> decedentRace = record.Race.Where(element => element.Item2 == "Y").ToList();
      Assert.Single(decedentRace);
    }
  }
}
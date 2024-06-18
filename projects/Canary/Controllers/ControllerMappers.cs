using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BFDR;
using canary.Models;
using VR;
using VRDR;

namespace canary.Controllers
{
  public static class ControllerMappers
  {
    public static readonly Dictionary<string, Func<IQueryable<Record>>> dbRecords = new()
        {
            {"vrdr", () => {
                    using RecordContext db = new();
                    return db.DeathRecords;
                }
            },
            {"bfdr", () => {
                    using RecordContext db = new();
                    return db.BirthRecords;
                }
            }
        };

    public static readonly Dictionary<string, Func<IQueryable<Test>>> dbTests = new()
        {
            {"vrdr", () => {
                    using RecordContext db = new();
                    return db.DeathTests;
                }
            },
            {"bfdr", () => {
                    using RecordContext db = new();
                    return db.BirthTests;
                }
            }
        };

    public static readonly Dictionary<string, Action<Record>> addDbRecord = new()
        {
            {"vrdr", (Record record) => {
                    using RecordContext db = new();
                    db.DeathRecords.Add((CanaryDeathRecord)record);
                    db.SaveChanges();
                }
            },
            {"bfdr", (Record record) => {
                    using RecordContext db = new();
                    db.BirthRecords.Add((CanaryBirthRecord)record);
                    db.SaveChanges();
                }
            },
        };

    public static readonly Dictionary<string, Record> createEmptyCanaryRecord = new()
        {
            {"vrdr", new CanaryDeathRecord()},
            {"bfdr", new CanaryBirthRecord()},
        };

    public static readonly Dictionary<string, VitalRecord> createEmptyRecord = new()
        {
            {"vrdr", new DeathRecord()},
            {"bfdr", new BirthRecord()},
        };

    public static readonly Dictionary<string, PropertyInfo[]> getRecordProperties = new()
        {
            {"vrdr", typeof(DeathRecord).GetProperties()},
            {"bfdr", typeof(BirthRecord).GetProperties()},
        };

    public static readonly Dictionary<string, Func<string, Record>> createRecordFromString = new()
        {
            {"vrdr", (string record) => new CanaryDeathRecord(record)},
            {"bfdr", (string record) => new CanaryBirthRecord(record)},
        };

    public static readonly Dictionary<string, Func<string, Record>> createCanaryRecordFromString = new()
        {

            {"vrdr", (string input) => new CanaryDeathRecord(VitalRecord.FromDescription<DeathRecord>(input))},
            {"bfdr", (string input) => new CanaryBirthRecord(VitalRecord.FromDescription<BirthRecord>(input))},
        };

    public static readonly Dictionary<string, Func<int, VitalRecord>> connectathonRecords = new()
        {
            {"vrdr", (int id) => VRDR.Connectathon.FromId(id)},
            {"bfdr", (int id) => BFDR.Connectathon.FromId(id)}
        };

    public static readonly Dictionary<string, Func<string, (Record, List<Dictionary<string, string>>)>> createRecordFromIJE = new()
        {
            {"vrdr", static (string input) => {
                    DeathRecord dr = new IJEMortality(input).ToRecord();
                    return (new CanaryDeathRecord(dr), new List<Dictionary<string, string>> {} );
                }
            },
            {"bfdr", static (string input) => {
                    BirthRecord br = new IJEBirth(input).ToRecord();
                    return (new CanaryBirthRecord(br), new List<Dictionary<string, string>> {} );
                }
            }
        };

    public static readonly Dictionary<string, Func<string, (Record, List<Dictionary<string, string>>)>> checkGetRecord = new() {
            {"vrdr", static (string input) => {
                    Record record = CanaryDeathRecord.CheckGet(input, false, out List<Dictionary<string, string>> issues);
                    return (record, issues);
                }
            },
            {"bfdr", static (string input) => {
                    Record record = CanaryBirthRecord.CheckGet(input, false, out List<Dictionary<string, string>> issues);
                    return (record, issues);
                }
            }
        };
  }
}
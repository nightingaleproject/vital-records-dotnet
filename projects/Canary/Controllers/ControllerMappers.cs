using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BFDR;
using canary.Models;
using VR;
using VRDR;
using VRDRConnectathon = VRDR.Connectathon;

namespace canary.Controllers
{
  public static class ControllerMappers
  {
    public static readonly string VRDR = "vrdr";
    public static readonly string BFDR_BIRTH = "bfdr-birth";
    public static readonly string BFDR_FETALDEATH = "bfdr-fetaldeath";

    public static readonly Dictionary<string, Func<IQueryable<Record>>> dbRecords = new()
        {
            {VRDR, () => {
                    using RecordContext db = new();
                    return db.DeathRecords;
                }
            },
            {BFDR_BIRTH, () => {
                    using RecordContext db = new();
                    return db.BirthRecords;
                }
            }
        };

    public static readonly Dictionary<string, Func<IQueryable<Test>>> dbTests = new()
        {
            {VRDR, () => {
                    using RecordContext db = new();
                    return db.DeathTests;
                }
            },
            {BFDR_BIRTH, () => {
                    using RecordContext db = new();
                    return db.BirthTests;
                }
            }
        };

    public static readonly Dictionary<string, Action<Record>> addDbRecord = new()
        {
            {VRDR, (Record record) => {
                    using RecordContext db = new();
                    db.DeathRecords.Add((CanaryDeathRecord)record);
                    db.SaveChanges();
                }
            },
            {BFDR_BIRTH, (Record record) => {
                    using RecordContext db = new();
                    db.BirthRecords.Add((CanaryBirthRecord)record);
                    db.SaveChanges();
                }
            },
        };

    public static readonly Dictionary<string, Action<Test>> addDbTest = new()
        {
            {VRDR, (Test test) => {
                    using RecordContext db = new();
                    db.DeathTests.Add((DeathTest)test);
                    db.SaveChanges();
                }
            },
            {BFDR_BIRTH, (Test test) => {
                    using RecordContext db = new();
                    db.BirthTests.Add((BirthTest)test);
                    db.SaveChanges();
                }
            },
        };

    public static readonly Dictionary<string, Action<Test>> removeDbTest = new()
        {
            {VRDR, (Test test) => {
                    using RecordContext db = new();
                    db.DeathTests.Remove((DeathTest)test);
                    db.SaveChanges();
                }
            },
            {BFDR_BIRTH, (Test test) => {
                    using RecordContext db = new();
                    db.BirthTests.Remove((BirthTest)test);
                    db.SaveChanges();
                }
            },
        };

    public static readonly Dictionary<string, Action<Test, String>> runTest = new()
        {
            {VRDR, (Test test, string input) => {
                    test.Run<DeathRecord>(input);
                }
            },
            {BFDR_BIRTH, (Test test, string input) => {
                    test.Run<BirthRecord>(input);
                }
            },
        };

    public static readonly Dictionary<string, Record> createEmptyCanaryRecord = new()
        {
            {VRDR, new CanaryDeathRecord()},
            {BFDR_BIRTH, new CanaryBirthRecord()},
        };

    public static readonly Dictionary<string, VitalRecord> createEmptyRecord = new()
        {
            {VRDR, new DeathRecord()},
            {BFDR_BIRTH, new BirthRecord()},
        };

    public static readonly Dictionary<string, Test> createEmptyTest = new()
        {
            {VRDR, new DeathTest()},
            {BFDR_BIRTH, new BirthTest()},
        };

    public static readonly Dictionary<string, Func<VitalRecord, Test>> createTestFromRecord = new()
        {
            {VRDR, (VitalRecord record) => new DeathTest((DeathRecord)record)},
            {BFDR_BIRTH, (VitalRecord record) => new BirthTest((BirthRecord)record)},
        };

    public static readonly Dictionary<string, PropertyInfo[]> getRecordProperties = new()
        {
            {VRDR, typeof(DeathRecord).GetProperties()},
            {BFDR_BIRTH, typeof(BirthRecord).GetProperties()},
        };

    public static readonly Dictionary<string, Func<string, Record>> createRecordFromString = new()
        {
            {VRDR, (string record) => new CanaryDeathRecord(record)},
            {BFDR_BIRTH, (string record) => new CanaryBirthRecord(record)},
        };

    public static readonly Dictionary<string, Func<string, Message>> creatMessageFromString = new()
        {
            {VRDR, (string input) => new CanaryDeathMessage(input)},
            {BFDR_BIRTH, (string input) => new CanaryBirthMessage(input)},
        };

    public static readonly Dictionary<string, Func<string, VitalRecord>> createRecordFromDescription = new()
        {

            {VRDR, (string input) => VitalRecord.FromDescription<DeathRecord>(input)},
            {BFDR_BIRTH, (string input) => VitalRecord.FromDescription<BirthRecord>(input)},
        };

    public static readonly Dictionary<string, Func<string, Record>> createCanaryRecordFromString = new()
        {

            {VRDR, (string input) => new CanaryDeathRecord(createRecordFromDescription[VRDR](input))},
            {BFDR_BIRTH, (string input) => new CanaryBirthRecord(createRecordFromDescription[BFDR_BIRTH](input))},
        };

    public static readonly Dictionary<string, Func<int, VitalRecord>> connectathonRecords = new()
        {
            {VRDR, (int id) => VRDRConnectathon.FromId(id)},
            {BFDR_BIRTH, (int id) => BFDR.Connectathon.FromId(id)}
        };

    public static readonly Dictionary<string, Func<int, int, string, VitalRecord>> connectathonRecordsParams = new()
        {
            {VRDR, (int id, int certificateNumber, string state) => VRDRConnectathon.FromId(id, certificateNumber, state)},
            {BFDR_BIRTH, (int id, int certificateNumber, string state) => BFDR.Connectathon.FromId(id, certificateNumber, state)}
        };

    public static readonly Dictionary<string, Func<string, (Record, List<Dictionary<string, string>>)>> createRecordFromIJE = new()
        {
            {VRDR, static (string input) => {
                    DeathRecord dr = new IJEMortality(input).ToRecord();
                    return (new CanaryDeathRecord(dr), new List<Dictionary<string, string>> {} );
                }
            },
            {BFDR_BIRTH, static (string input) => {
                    BirthRecord br = new IJEBirth(input).ToRecord();
                    return (new CanaryBirthRecord(br), new List<Dictionary<string, string>> {} );
                }
            }
        };

    public static readonly Dictionary<string, Func<string, (Record, List<Dictionary<string, string>>)>> checkGetRecord = new() {
            {VRDR, static (string input) => {
                    Record record = CanaryDeathRecord.CheckGet(input, false, out List<Dictionary<string, string>> issues);
                    return (record, issues);
                }
            },
            {BFDR_BIRTH, static (string input) => {
                    Record record = CanaryBirthRecord.CheckGet(input, false, out List<Dictionary<string, string>> issues);
                    return (record, issues);
                }
            }
        };
  }
}
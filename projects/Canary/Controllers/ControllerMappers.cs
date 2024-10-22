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
        public const string VRDR = "vrdr";
        public const string BFDR_BIRTH = "bfdr-birth";
        public const string BFDR_FETALDEATH = "bfdr-fetaldeath";

        public static readonly Dictionary<string, Func<RecordContext, IQueryable<Record>>> dbRecords = new()
        {
            {VRDR, (RecordContext db) => {
                    return db.DeathRecords;
                }
            },
            {BFDR_BIRTH, (RecordContext db) => {
                    return db.BirthRecords;
                }
            },
            {BFDR_FETALDEATH, (RecordContext db) => {
                    return db.FetalDeathRecords;
                }
            }
        };

        public static readonly Dictionary<string, Func<RecordContext, IQueryable<Test>>> dbTests = new()
        {
            {VRDR, (RecordContext db) => {
                    return db.DeathTests;
                }
            },
            {BFDR_BIRTH, (RecordContext db) => {
                    return db.BirthTests;
                }
            },
            {BFDR_FETALDEATH, (RecordContext db) => {
                    return db.FetalDeathTests;
                }
            }
        };

        public static readonly Dictionary<string, Action<Record, RecordContext>> addDbRecord = new()
        {
            {VRDR, (Record record, RecordContext db) => {
                    db.DeathRecords.Add((CanaryDeathRecord)record);
                    db.SaveChanges();
                }
            },
            {BFDR_BIRTH, (Record record, RecordContext db) => {
                    db.BirthRecords.Add((CanaryBirthRecord)record);
                    db.SaveChanges();
                }
            },
            {BFDR_FETALDEATH, (Record record, RecordContext db) => {
                    db.FetalDeathRecords.Add((CanaryFetalDeathRecord)record);
                    db.SaveChanges();
                }
            }
        };

        public static readonly Dictionary<string, Action<Test, RecordContext>> addDbTest = new()
        {
            {VRDR, (Test test, RecordContext db) => {
                    db.DeathTests.Add((DeathTest)test);
                    db.SaveChanges();
                }
            },
            {BFDR_BIRTH, (Test test, RecordContext db) => {
                    db.BirthTests.Add((BirthTest)test);
                    db.SaveChanges();
                }
            },
            {BFDR_FETALDEATH, (Test test, RecordContext db) => {
                    db.FetalDeathTests.Add((FetalDeathTest)test);
                    db.SaveChanges();
                }
            }
        };

        public static readonly Dictionary<string, Action<Test, RecordContext>> removeDbTest = new()
        {
            {VRDR, (Test test, RecordContext db) => {
                    db.DeathTests.Remove((DeathTest)test);
                    db.SaveChanges();
                }
            },
            {BFDR_BIRTH, (Test test, RecordContext db) => {
                    db.BirthTests.Remove((BirthTest)test);
                    db.SaveChanges();
                }
            },
            {BFDR_FETALDEATH, (Test test, RecordContext db) => {
                    db.FetalDeathTests.Remove((FetalDeathTest)test);
                    db.SaveChanges();
                }
            },
        };

        public static readonly Dictionary<string, Action<Test, string>> runTest = new()
        {
            {VRDR, (Test test, string input) => {
                    test.Run<DeathRecord>(input);
                }
            },
            {BFDR_BIRTH, (Test test, string input) => {
                    test.Run<BirthRecord>(input);
                }
            },
            {BFDR_FETALDEATH, (Test test, string input) => {
                    test.Run<FetalDeathRecord>(input);
                }
            }
        };

        public static readonly Dictionary<string, Func<Record>> createEmptyCanaryRecord = new()
        {
            {VRDR, () => new CanaryDeathRecord()},
            {BFDR_BIRTH, () => new CanaryBirthRecord()},
            {BFDR_FETALDEATH, () => new CanaryFetalDeathRecord()}
        };

        public static readonly Dictionary<string, Func<VitalRecord>> createEmptyRecord = new()
        {
            {VRDR, () => new DeathRecord()},
            {BFDR_BIRTH, () => new BirthRecord()},
            {BFDR_FETALDEATH, () => new FetalDeathRecord()}
        };

        public static readonly Dictionary<string, Func<Test>> createEmptyTest = new()
        {
            {VRDR, () => new DeathTest()},
            {BFDR_BIRTH, () => new BirthTest()},
            {BFDR_FETALDEATH, () => new FetalDeathTest()}
        };

        public static readonly Dictionary<string, Func<VitalRecord, Test>> createTestFromRecord = new()
        {
            {VRDR, (VitalRecord record) => new DeathTest((DeathRecord)record)},
            {BFDR_BIRTH, (VitalRecord record) => new BirthTest((BirthRecord)record)},
            {BFDR_FETALDEATH, (VitalRecord record) => new FetalDeathTest((FetalDeathRecord)record)}
        };

        public static readonly Dictionary<string, PropertyInfo[]> getRecordProperties = new()
        {
            {VRDR, typeof(DeathRecord).GetProperties()},
            {BFDR_BIRTH, typeof(BirthRecord).GetProperties()},
            {BFDR_FETALDEATH, typeof(FetalDeathRecord).GetProperties()}
        };

        public static readonly Dictionary<string, Func<string, Record>> createRecordFromString = new()
        {
            {VRDR, (string record) => new CanaryDeathRecord(record)},
            {BFDR_BIRTH, (string record) => new CanaryBirthRecord(record)},
            {BFDR_FETALDEATH, (string record) => new CanaryFetalDeathRecord(record)}
        };

        public static readonly Dictionary<string, Func<string, Message>> createMessageFromString = new()
        {
            {VRDR, (string input) => new CanaryDeathMessage(input)},
            {BFDR_BIRTH, (string input) => new CanaryBirthMessage(input)},
            {BFDR_FETALDEATH, (string input) => new CanaryFetalDeathMessage(input)}
        };

        public static readonly Dictionary<string, Func<Record, string, Message>> createMessageFromType = new()
        {
            {VRDR, (Record record, string type) => new CanaryDeathMessage(record, type)},
            {BFDR_BIRTH, (Record record, string type) => new CanaryBirthMessage(record, type)},
            {BFDR_FETALDEATH, (Record record, string type) => new CanaryFetalDeathMessage(record, type)}
        };

        public static readonly Dictionary<string, Func<CommonMessage, Message>> createMessageFromMessage = new()
        {
            {VRDR, (CommonMessage message) => new CanaryDeathMessage(message)},
            {BFDR_BIRTH, (CommonMessage message) => new CanaryBirthMessage(message)},
            {BFDR_FETALDEATH, (CommonMessage message) => new CanaryFetalDeathMessage(message)}
        };

        public static readonly Dictionary<string, Func<string, CommonMessage>> parseMessage = new()
        {
            {VRDR, (string input) => BaseMessage.Parse(input, false)},
            {BFDR_BIRTH, (string input) => BFDRBaseMessage.Parse(input, false)},
            {BFDR_FETALDEATH, (string input) => BFDRBaseMessage.Parse(input, false)} // TODO - Fetal Death Messaging
        };

        public static readonly Dictionary<string, Func<string, VitalRecord>> createRecordFromDescription = new()
        {

            {VRDR, (string input) => VitalRecord.FromDescription<DeathRecord>(input)},
            {BFDR_BIRTH, (string input) => VitalRecord.FromDescription<BirthRecord>(input)},
            {BFDR_FETALDEATH, (string input) => VitalRecord.FromDescription<FetalDeathRecord>(input)}
        };

        public static readonly Dictionary<string, Func<string, Record>> createCanaryRecordFromString = new()
        {

            {VRDR, (string input) => new CanaryDeathRecord(createRecordFromDescription[VRDR](input))},
            {BFDR_BIRTH, (string input) => new CanaryBirthRecord(createRecordFromDescription[BFDR_BIRTH](input))},
            {BFDR_FETALDEATH, (string input) => new CanaryFetalDeathRecord(createRecordFromDescription[BFDR_FETALDEATH](input))}
        };

        public static readonly Dictionary<string, Func<int, VitalRecord>> connectathonRecords = new()
        {
            {VRDR, (int id) => VRDRConnectathon.FromId(id)},
            {BFDR_BIRTH, (int id) => BFDR.Connectathon.FromId(id)},
            // {BFDR_FETALDEATH, (int id) => BFDR.Connectathon.FromId(id)} // TODO - Fetal Death Connecthon Records.
        };

        public static readonly Dictionary<string, Func<int, int, string, VitalRecord>> connectathonRecordsParams = new()
        {
            {VRDR, (int id, int certificateNumber, string state) => VRDRConnectathon.FromId(id, certificateNumber, state)},
            {BFDR_BIRTH, (int id, int certificateNumber, string state) => BFDR.Connectathon.FromId(id, certificateNumber, state)}
            // TODO - Fetal Death Connectathon Records.
        };

        public static readonly Dictionary<string, Func<string, (Record, List<Dictionary<string, string>>)>> createRecordFromIJE = new()
        {
            {VRDR, static (string input) => (new CanaryDeathRecord(new IJEMortality(input).ToRecord()), new List<Dictionary<string, string>> {} )},
            {BFDR_BIRTH, static (string input) => (new CanaryBirthRecord(new IJEBirth(input).ToRecord()), new List<Dictionary<string, string>> {} )},
            {BFDR_FETALDEATH, static (string input) => (new CanaryFetalDeathRecord(new IJEFetalDeath(input).ToRecord()), new List<Dictionary<string, string>> {} )}
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
            },
            {BFDR_FETALDEATH, static (string input) => {
                    Record record = CanaryFetalDeathRecord.CheckGet(input, false, out List<Dictionary<string, string>> issues);
                    return (record, issues);
                }
            }
        };
    }
}
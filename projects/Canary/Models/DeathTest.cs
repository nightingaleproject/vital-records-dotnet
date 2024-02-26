using VR;
using VRDR;

namespace canary.Models
{

    public class DeathTest : Test
    {
        public DeathTest() : base()
        {
        }

        public DeathTest(VitalRecord record) : base(record)
        {
        }

        protected override Record CreateEmptyRecord()
        {
            return new CanaryDeathRecord();
        }

        protected override Message CreateMessage(Record referenceRecord, string messageType)
        {
            return new CanaryDeathMessage(referenceRecord, messageType);
        }

        protected override Message CreateMessage(string description)
        {
            return new CanaryDeathMessage(description);
        }

        protected override Record CreateRecordFromFHIR(string description)
        {
            return new CanaryDeathRecord(VitalRecord.FromDescription<DeathRecord>(description));
        }

        protected override Record CreateRecordFromVitalRecord(VitalRecord record)
        {
            return new CanaryDeathRecord(record);
        }

        protected override string GetMessageDescriptionFor(string propertyName)
        {
            return CanaryDeathMessage.GetDescriptionFor(propertyName);
        }
    }
}

using BFDR;
using VR;

namespace canary.Models
{

    public class FetalDeathTest : Test
    {
        public FetalDeathTest() : base()
        {
        }
        
        public FetalDeathTest(VitalRecord record) : base(record)
        {
        }

        protected override Record CreateEmptyRecord()
        {
            return new CanaryFetalDeathRecord();
        }

        protected override Message CreateMessage(Record referenceRecord, string messageType)
        {
            return new CanaryFetalDeathMessage(referenceRecord, messageType);
        }

        protected override Message CreateMessage(string description)
        {
            return new CanaryFetalDeathMessage(description);
        }

        protected override Record CreateRecordFromFHIR(string description)
        {
            return new CanaryFetalDeathRecord(VitalRecord.FromDescription<BirthRecord>(description));
        }

        protected override Record CreateRecordFromVitalRecord(VitalRecord record)
        {
            return new CanaryFetalDeathRecord(record);
        }

        protected override string GetMessageDescriptionFor(string propertyName)
        {
            return CanaryFetalDeathMessage.GetDescriptionFor(propertyName);
        }
    }
}

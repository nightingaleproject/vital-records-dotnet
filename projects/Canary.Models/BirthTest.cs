using BFDR;
using VR;

namespace canary.Models
{

    public class BirthTest : Test
    {
        public BirthTest() : base()
        {
        }
        
        public BirthTest(VitalRecord record) : base(record)
        {
        }

        protected override Record CreateEmptyRecord()
        {
            return new CanaryBirthRecord();
        }

        protected override Message CreateMessage(Record referenceRecord, string messageType)
        {
            return new CanaryBirthMessage(referenceRecord, messageType);
        }

        protected override Message CreateMessage(string description)
        {
            return new CanaryBirthMessage(description);
        }

        protected override Record CreateRecordFromFHIR(string description)
        {
            return new CanaryBirthRecord(VitalRecord.FromDescription<BirthRecord>(description));
        }

        protected override Record CreateRecordFromVitalRecord(VitalRecord record)
        {
            return new CanaryBirthRecord(record);
        }

        protected override string GetMessageDescriptionFor(string propertyName)
        {
            return CanaryBirthMessage.GetDescriptionFor(propertyName);
        }
    }
}

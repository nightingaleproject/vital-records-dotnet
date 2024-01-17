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

        protected override Record CreateRecordFromFHIR(string description)
        {
            return new CanaryDeathRecord(VitalRecord.FromDescription<DeathRecord>(description));
        }

        protected override Record CreateRecordFromVitalRecord(VitalRecord record)
        {
            return new CanaryDeathRecord(record);
        }
    }
}

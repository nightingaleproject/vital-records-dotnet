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

        protected override Record CreateRecordFromFHIR(string description)
        {
            return new CanaryBirthRecord(VitalRecord.FromDescription<BirthRecord>(description));
        }

        protected override Record CreateRecordFromVitalRecord(VitalRecord record)
        {
            return new CanaryBirthRecord(record);
        }
    }
}

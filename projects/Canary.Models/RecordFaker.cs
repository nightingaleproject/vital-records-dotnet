using System;
using VR;
using BFDR;

namespace canary.Models
{
    /// <summary>Class <c>RecordFaker</c> can be used to generate synthetic <c>VitalRecord</c>s. Various
    /// options are available to tailoring the records generated to specific use case by the class.
    /// </summary>
    public abstract class RecordFaker<RecordType> where RecordType : VitalRecord
    {

        /// <summary>State to use when generating records.</summary>
        protected string state;

        /// <summary>Decedent Sex to use.</summary>
        protected string sex;

        /// <summary>Constructor with optional arguments to customize the records generated.</summary>
        public RecordFaker(string state = "MA", string sex = "Male")
        {
            this.state = state;
            this.sex = sex;
        }

        /// <summary>Return a new record populated with fake data.</summary>
        public abstract RecordType Generate(bool simple = false);
    }
}

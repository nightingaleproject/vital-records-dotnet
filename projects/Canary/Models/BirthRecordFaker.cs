using System;
using System.Collections.Generic;
using Bogus.Extensions.UnitedStates;
using VRDR;
using VR;
using BFDR;

namespace canary.Models
{
    /// <summary>Class <c>Faker</c> can be used to generate synthetic <c>BirthRecord</c>s. Various
    /// options are available to tailoring the records generated to specific use case by the class.
    /// </summary>
    public class BirthRecordFaker : RecordFaker<BirthRecord> 
    {
        public BirthRecordFaker(string state = "MA", string sex = "Male") : base(state, sex) {}

        /// <summary>Return a new record populated with fake data.</summary>
        public override BirthRecord Generate(bool simple = false)
        {
            BirthRecord record = new BirthRecord();
            Bogus.Faker faker = new Bogus.Faker("en");
            // Grab Gender enum value
            string gender = sex == "Male" ? "M" : "F";
            record.Identifier = Convert.ToString(faker.Random.Number(999999));
            // record.BirthSexHelper = gender.ToString().ToLower();
            record.BirthSexHelper = gender;
            return record;
        }
    }
}

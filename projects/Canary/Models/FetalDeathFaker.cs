using System;
using Bogus.Extensions.UnitedStates;
using BFDR;

namespace canary.Models
{
    /// <summary>Class <c>Faker</c> can be used to generate synthetic <c>BirthRecord</c>s. Various
    /// options are available to tailoring the records generated to specific use case by the class.
    /// </summary>
    public class FetalDeathFaker : RecordFaker<FetalDeathRecord> 
    {
        public FetalDeathFaker(string state = "MA", string sex = "Male") : base(state, sex) {}

        /// <summary>Return a new record populated with fake data.</summary>
        public override FetalDeathRecord Generate(bool simple = false)
        {
            FetalDeathRecord record = new();
            Bogus.Faker faker = new Bogus.Faker("en");
            // Grab Gender enum value
            Bogus.DataSets.Name.Gender gender = sex == "Male" ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female;
            record.CertificateNumber = Convert.ToString(faker.Random.Number(999999));
            record.FetusGivenNames = new string[] { faker.Name.FirstName(gender), faker.Name.FirstName(gender) };
            record.FetusFamilyName = faker.Name.LastName(gender);
            record.FetusSuffix = faker.Name.Suffix();
            record.InfantMedicalRecordNumber = "912912";
            record.MotherMedicalRecordNumber = "876876";
            record.MotherSocialSecurityNumber = faker.Person.Ssn();
            record.FatherSocialSecurityNumber = faker.Person.Ssn();
            record.MotherPrepregnancyWeightEditFlagHelper = "1";
            record.MotherWeightAtDeliveryEditFlagHelper = "0";
            record.BirthWeightEditFlagHelper = "2";
            record.MotherIndustry = "mother";
            record.FatherIndustry = "father";
            record.MotherOccupation = "CEO";
            record.FatherOccupation = "COO";
            DateTime birth = faker.Date.Recent();
            DateTimeOffset birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
            record.DateOfDelivery = birthUtc.ToString("yyyy-MM-dd");
            birth = faker.Date.Past(123, DateTime.Today.AddYears(-18));
            birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
            record.FatherDateOfBirth = birthUtc.ToString("yyyy-MM-dd");
            birth = faker.Date.Past(123, DateTime.Today.AddYears(-18));
            birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
            record.FatherDateOfBirth = birthUtc.ToString("yyyy-MM-dd");
            return record;
        }
    }
}

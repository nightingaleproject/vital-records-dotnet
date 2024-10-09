using System;
using System.Collections.Generic;
using Bogus.Extensions.UnitedStates;
using VRDR;
using VR;
using BFDR;

namespace Canary.Models
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
            // set some boolean fields
            if (faker.Random.Bool()) {
              record.NoSpecifiedAbnormalConditionsOfNewborn = faker.Random.Bool();
              if (!record.NoSpecifiedAbnormalConditionsOfNewborn) {
                record.Seizure = faker.Random.Bool(); 
              }
            }
            if (faker.Random.Bool()) {
              record.NoCharacteristicsOfLaborAndDelivery= faker.Random.Bool();
              if (!record.NoCharacteristicsOfLaborAndDelivery) {
                record.Chorioamnionitis = faker.Random.Bool();
                record.EpiduralOrSpinalAnesthesia = faker.Random.Bool();
              }
            }
            if (faker.Random.Bool()) {
              record.NoMaternalMorbidities= faker.Random.Bool();
              if (!record.NoMaternalMorbidities) {
                record.MaternalTransfusion = faker.Random.Bool();
                record.PerinealLaceration = faker.Random.Bool();
              }
            }
            if (faker.Random.Bool()) {
              record.NoInfectionsPresentDuringPregnancy = faker.Random.Bool();
              if (!record.NoInfectionsPresentDuringPregnancy) {
                record.Gonorrhea= faker.Random.Bool();
                record.Syphilis = faker.Random.Bool();
              }
            }
            if (faker.Random.Bool()) {
              record.NoCongenitalAnomaliesOfTheNewborn= faker.Random.Bool();
              if (!record.NoCongenitalAnomaliesOfTheNewborn) {
                record.CleftLipWithOrWithoutCleftPalate = faker.Random.Bool();
                record.Omphalocele = faker.Random.Bool();
              }
            }
            if (faker.Random.Bool()) {
              record.NoPregnancyRiskFactors = faker.Random.Bool();
              if (!record.NoPregnancyRiskFactors) {
                record.GestationalDiabetes = faker.Random.Bool();
                record.PreviousCesarean = faker.Random.Bool();
              }
            }
            // set some numerical fields
            if (faker.Random.Bool()) {
              record.ApgarScoreFiveMinutes = faker.Random.Number(0, 10);
              if (record.ApgarScoreFiveMinutes < 6) {
                record.ApgarScoreTenMinutes = faker.Random.Number(0, 10);
              }
            }
            if (faker.Random.Bool()) {
              record.BirthWeight = faker.Random.Number(2000, 4000);
            }
            if (faker.Random.Bool()) {
              record.MotherPrepregnancyWeight = faker.Random.Number(100, 200);
              record.MotherWeightAtDelivery = record.MotherPrepregnancyWeight + faker.Random.Number(7, 12);
            }
            if (faker.Random.Bool()) {
              record.MotherHeight = faker.Random.Number(60,72);
            }
            if (faker.Random.Bool()) {
              record.CigarettesPerDayInThreeMonthsPriorToPregancy = faker.Random.Number(0,5);
              record.CigarettesPerDayInFirstTrimester = faker.Random.Number(0,3);
              record.CigarettesPerDayInSecondTrimester = faker.Random.Number(0,2);
              record.CigarettesPerDayInLastTrimester = faker.Random.Number(0,1);
            }
            if (faker.Random.Bool()) {
              if (faker.Random.Bool()){
                record.MotherReceivedWICFoodHelper = "Y";
              } else {
                record.MotherReceivedWICFoodHelper = "N";
              }
            }
            // Grab Gender enum value
            Bogus.DataSets.Name.Gender gender = sex == "Male" ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female;
            record.CertificateNumber = Convert.ToString(faker.Random.Number(100000, 999999));
            record.ChildGivenNames = new string[] { faker.Name.FirstName(gender), faker.Name.FirstName(gender) };
            record.ChildFamilyName = faker.Name.LastName(gender);
            record.ChildSuffix = faker.Name.Suffix();
            record.BirthSex = gender.ToString().Substring(0, 1);
            record.InfantMedicalRecordNumber = Convert.ToString(faker.Random.Number(10000, 99999));
            record.MotherMedicalRecordNumber = Convert.ToString(faker.Random.Number(10000, 99999));
            record.MotherSocialSecurityNumber = faker.Person.Ssn();
            record.FatherSocialSecurityNumber = faker.Person.Ssn();
            record.MotherPrepregnancyWeightEditFlagHelper = "1";
            record.MotherWeightAtDeliveryEditFlagHelper = "0";
            record.BirthWeightEditFlagHelper = "2";
            // Occupation / Industry
            Tuple<string, string>[] occupationIndustries =
            {
                Tuple.Create("Secretary", "State agency"),
                Tuple.Create("Carpenter", "Construction"),
                Tuple.Create("Programmer", "Health Insurance"),
                Tuple.Create("Lawyer", "Legal Services"),
                Tuple.Create("Scientist", "Public Health"),
                Tuple.Create("Baker", "Production"),
                Tuple.Create("Flight Attendant", "Transportation"),
                Tuple.Create("Barber", "Personal Care and Service"),
                Tuple.Create("Firefighter", "Protective Service"),
            };
            Tuple<string, string> occIndFather = faker.Random.ArrayElement<Tuple<string, string>>(occupationIndustries);
            Tuple<string, string> occIndMother = faker.Random.ArrayElement<Tuple<string, string>>(occupationIndustries);
            record.MotherIndustry = occIndMother.Item2;
            record.FatherIndustry = occIndFather.Item2;
            record.MotherOccupation = occIndMother.Item1;
            record.FatherOccupation = occIndFather.Item1;
            record.MotherEducationLevelHelper = VR.ValueSets.EducationLevel.Codes[faker.Random.Number(VRDR.ValueSets.EducationLevel.Codes.GetLength(0) - 1), 0];
            record.FatherEducationLevelHelper = VR.ValueSets.EducationLevel.Codes[faker.Random.Number(VRDR.ValueSets.EducationLevel.Codes.GetLength(0) - 1), 0];
            // set more demographic and record keeping info
            DateTime birth = faker.Date.Recent();
            DateTimeOffset birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
            record.DateOfBirth = birthUtc.ToString("yyyy-MM-dd");
            birth = faker.Date.Past(123, DateTime.Today.AddYears(-18));
            birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
            record.FatherDateOfBirth = birthUtc.ToString("yyyy-MM-dd");
            birth = faker.Date.Past(123, DateTime.Today.AddYears(-18));
            birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
            record.FatherDateOfBirth = birthUtc.ToString("yyyy-MM-dd");
            // Ethnicity
            if (faker.Random.Bool() && !simple)
            {
                record.MotherEthnicity1Helper = VRDR.ValueSets.YesNoUnknown.No;
                record.MotherEthnicity2Helper = VRDR.ValueSets.YesNoUnknown.No;
                record.MotherEthnicity3Helper = VRDR.ValueSets.YesNoUnknown.Yes;
                record.MotherEthnicity4Helper = VRDR.ValueSets.YesNoUnknown.No;
                record.FatherEthnicity1Helper = VRDR.ValueSets.YesNoUnknown.No;
                record.FatherEthnicity2Helper = VRDR.ValueSets.YesNoUnknown.Yes;
                record.FatherEthnicity3Helper = VRDR.ValueSets.YesNoUnknown.No;
                record.FatherEthnicity4Helper = VRDR.ValueSets.YesNoUnknown.No;
                record.MotherEthnicityLiteral = "";
                record.FatherEthnicityLiteral = "";
            }
            else
            {
                record.MotherEthnicity1Helper = VRDR.ValueSets.YesNoUnknown.No;
                record.MotherEthnicity2Helper = VRDR.ValueSets.YesNoUnknown.No;
                record.FatherEthnicity1Helper = VRDR.ValueSets.YesNoUnknown.No;
                record.FatherEthnicity2Helper = VRDR.ValueSets.YesNoUnknown.No;
            }
            // Race
            Tuple<string, string>[] nvssRaces =
            {
                Tuple.Create(VR.NvssRace.AmericanIndianOrAlaskanNative, "Y"),
                Tuple.Create(VR.NvssRace.AsianIndian, "Y"),
                Tuple.Create(VR.NvssRace.BlackOrAfricanAmerican, "Y"),
                Tuple.Create(VR.NvssRace.Chinese, "Y"),
                Tuple.Create(VR.NvssRace.Filipino, "Y"),
                Tuple.Create(VR.NvssRace.GuamanianOrChamorro, "Y"),
                Tuple.Create(VR.NvssRace.Japanese, "Y"),
                Tuple.Create(VR.NvssRace.Korean, "Y"),
                Tuple.Create(VR.NvssRace.NativeHawaiian, "Y"),
                Tuple.Create(VR.NvssRace.OtherAsian, "Y"),
                Tuple.Create(VR.NvssRace.OtherPacificIslander, "Y"),
                Tuple.Create(VR.NvssRace.OtherRace, "Y"),
                Tuple.Create(VR.NvssRace.Samoan, "Y"),
                Tuple.Create(VR.NvssRace.Vietnamese, "Y"),
                Tuple.Create(VR.NvssRace.White, "Y"),
            };
            if (!simple)
            {
                Tuple<string, string> race1 = faker.Random.ArrayElement<Tuple<string, string>>(nvssRaces);
                Tuple<string, string> race2 = faker.Random.ArrayElement<Tuple<string, string>>(nvssRaces);
                Tuple<string, string> race3 = faker.Random.ArrayElement<Tuple<string, string>>(nvssRaces);
                Tuple<string, string> race4 = faker.Random.ArrayElement<Tuple<string, string>>(nvssRaces);
                Tuple<string, string>[] FatherRaces = { race1, race2 };
                Tuple<string, string>[] MotherRaces = { race3, race4 };
                record.FatherRace = FatherRaces;
                record.MotherRace = MotherRaces;
            }
            else
            {
                Tuple<string, string> race1 = faker.Random.ArrayElement<Tuple<string, string>>(nvssRaces);
                Tuple<string, string> race2 = faker.Random.ArrayElement<Tuple<string, string>>(nvssRaces);
                record.FatherRace = new Tuple<string, string>[] { race1 };
                record.MotherRace = new Tuple<string, string>[] { race2 };
            }
            record.BirthLocationJurisdiction = state;
            Dictionary<string, string> birthAddress = new Dictionary<string, string>();
            birthAddress.Add("addressLine1", $"{faker.Random.Number(999) + 1} Main Street");
            birthAddress.Add("addressCity", "Springfield");
            // birthAddress.Add("addressCounty", "Hampden");
            birthAddress.Add("addressState", state);
            // birthAddress.Add("addressZip", "01101");
            birthAddress.Add("addressCountry", "US");
            record.PlaceOfBirth = birthAddress;
            Tuple<string, string>[] risks_anomalies_characteristics =
            {
                Tuple.Create(VR.NvssRace.AmericanIndianOrAlaskanNative, "Y"),
                Tuple.Create(VR.NvssRace.AsianIndian, "Y"),
                Tuple.Create(VR.NvssRace.BlackOrAfricanAmerican, "Y"),
                Tuple.Create(VR.NvssRace.Chinese, "Y"),
                Tuple.Create(VR.NvssRace.Filipino, "Y"),
                Tuple.Create(VR.NvssRace.GuamanianOrChamorro, "Y"),
                Tuple.Create(VR.NvssRace.Japanese, "Y"),
                Tuple.Create(VR.NvssRace.Korean, "Y"),
                Tuple.Create(VR.NvssRace.NativeHawaiian, "Y"),
                Tuple.Create(VR.NvssRace.OtherAsian, "Y"),
                Tuple.Create(VR.NvssRace.OtherPacificIslander, "Y"),
                Tuple.Create(VR.NvssRace.OtherRace, "Y"),
                Tuple.Create(VR.NvssRace.Samoan, "Y"),
                Tuple.Create(VR.NvssRace.Vietnamese, "Y"),
                Tuple.Create(VR.NvssRace.White, "Y"),
            };

            return record;
        }
    }
}

using System;
using System.Collections.Generic;
using Bogus.Extensions.UnitedStates;
using VRDR;
using VR;
using System.Linq;

namespace canary.Models
{
    /// <summary>Class <c>Faker</c> can be used to generate synthetic <c>DeathRecord</c>s. Various
    /// options are available to tailoring the records generated to specific use case by the class.
    /// </summary>
    public class DeathRecordFaker : RecordFaker<DeathRecord>
    {

        /// <summary>Type of Cause of Death; Natural or Injury.</summary>
        private string type;

        /// <summary>Constructor with optional arguments to customize the records generated.</summary>
        public DeathRecordFaker(string state = "MA", string type = "Natural", string sex = "Male") : base(state, sex)
        {
            this.type = type;
        }

        /// <summary>Return a new record populated with fake data.</summary>
        public override DeathRecord Generate(bool simple = false)
        {
            DeathRecord record = new DeathRecord();

            Random random = new Random();
            Bogus.Faker faker = new Bogus.Faker("en");

            // Grab Gender enum value
            Bogus.DataSets.Name.Gender gender = sex == "Male" ? Bogus.DataSets.Name.Gender.Male : Bogus.DataSets.Name.Gender.Female;

            // Was married?
            bool wasMarried = faker.Random.Bool();

            record.Identifier = Convert.ToString(faker.Random.Number(999999));
            // record.BundleIdentifier = Convert.ToString(faker.Random.Number(999999));
            DateTime date = faker.Date.Recent();
            record.CertifiedTime = date.ToString("s");
            record.RegisteredTime = new DateTimeOffset(date.AddDays(-1).Year, date.AddDays(-1).Month, date.AddDays(-1).Day, 0, 0, 0, TimeSpan.Zero).ToString("s");
            record.StateLocalIdentifier1 = Convert.ToString(faker.Random.Number(999999));

            // Basic Decedent information

            record.GivenNames = new string[] { faker.Name.FirstName(gender), faker.Name.FirstName(gender) };
            record.FamilyName = faker.Name.LastName(gender);
            record.Suffix = faker.Name.Suffix();
            if (gender == Bogus.DataSets.Name.Gender.Female && wasMarried)
            {
                record.MaidenName = faker.Name.LastName(gender);
            }

            record.FatherFamilyName = record.FamilyName;
            record.FatherGivenNames = new string[] { faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male), faker.Name.FirstName(Bogus.DataSets.Name.Gender.Male) };
            record.FatherSuffix = faker.Name.Suffix();

            record.MotherGivenNames = new string[] { faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female), faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female) };
            record.MotherMaidenName = faker.Name.LastName(Bogus.DataSets.Name.Gender.Female);
            record.MotherSuffix = faker.Name.Suffix();

            record.SpouseGivenNames = new string[] { faker.Name.FirstName(), faker.Name.FirstName() };
            record.SpouseFamilyName = record.FamilyName;
            record.SpouseSuffix = faker.Name.Suffix();

            record.BirthRecordId = Convert.ToString(faker.Random.Number(999999));


            record.SexAtDeathHelper = gender.ToString().ToLower();
            record.SSN = faker.Person.Ssn().Replace("-", string.Empty);
            DateTime birth = faker.Date.Past(123, DateTime.Today.AddYears(-18));
            DateTime death = faker.Date.Recent();
            DateTimeOffset birthUtc = new DateTimeOffset(birth.Year, birth.Month, birth.Day, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset deathUtc = new DateTimeOffset(death.Year, death.Month, death.Day, 0, 0, 0, TimeSpan.Zero);
            record.DateOfBirth = birthUtc.ToString("yyyy-MM-dd");
            record.DateOfDeath = deathUtc.ToString("yyyy-MM-dd");
            int age = death.Year - birth.Year;
            if (birthUtc > deathUtc.AddYears(-age)) age--;
            record.AgeAtDeath = new Dictionary<string, string>() { { "value", age.ToString() }, { "code", "a" } };

            // Place of residence

            Dictionary<string, string> residence = new Dictionary<string, string>();
            residence.Add("addressLine1", $"{faker.Random.Number(999) + 1} {faker.Address.StreetName()}");
            residence.Add("addressCity", "Bedford");
            residence.Add("addressCounty", "Middlesex");
            residence.Add("addressState", "MA");
            residence.Add("addressZip", "01730");
            residence.Add("addressCountry", "US");
            record.Residence = residence;

            // Residence Within City Limits
            record.ResidenceWithinCityLimitsHelper = VR.ValueSets.YesNoUnknown.Yes;

            // Place of birth

            Dictionary<string, string> placeOfBirth = new Dictionary<string, string>();
            placeOfBirth.Add("addressCity", "Bedford");
            placeOfBirth.Add("addressCounty", "Middlesex");
            placeOfBirth.Add("addressState", "MA");
            placeOfBirth.Add("addressZip", "01730");
            placeOfBirth.Add("addressCountry", "US");
            record.PlaceOfBirth = placeOfBirth;
            record.BirthRecordState = state;

            // Place of death

            record.DeathLocationName = "Bedford Hospital";

            record.DeathLocationTypeHelper = VRDR.ValueSets.PlaceOfDeath.Death_In_Hospital_Based_Emergency_Department_Or_Outpatient_Department;

            Dictionary<string, string> placeOfDeath = new Dictionary<string, string>();
            placeOfDeath.Add("addressLine1", $"{faker.Random.Number(999) + 1} {faker.Address.StreetName()}");
            placeOfDeath.Add("addressCity", "Bedford");
            placeOfDeath.Add("addressCounty", "Middlesex");
            placeOfDeath.Add("addressState", "MA");
            placeOfDeath.Add("addressZip", "01730");
            placeOfDeath.Add("addressCountry", "US");
            record.DeathLocationAddress = placeOfDeath;
            record.DeathLocationJurisdiction = state;

            record.DeathLocationJurisdiction = state;

            // Marital status

            record.MaritalStatusHelper = VR.ValueSets.MaritalStatus.Codes[faker.Random.Number(VR.ValueSets.MaritalStatus.Codes.GetLength(0) - 1), 0];

            // Ethnicity
            if (faker.Random.Bool() && !simple)
            {
                record.Ethnicity1Helper = VR.ValueSets.YesNoUnknown.No;
                record.Ethnicity2Helper = VR.ValueSets.YesNoUnknown.No;
                record.Ethnicity3Helper = VR.ValueSets.YesNoUnknown.No;
                record.Ethnicity4Helper = VR.ValueSets.YesNoUnknown.Yes;
                record.EthnicityLiteral = "";
            }
            else
            {
                record.Ethnicity1Helper = VR.ValueSets.YesNoUnknown.No;
                record.Ethnicity2Helper = VR.ValueSets.YesNoUnknown.No;
                record.Ethnicity3Helper = VR.ValueSets.YesNoUnknown.No;
                record.Ethnicity4Helper = VR.ValueSets.YesNoUnknown.No;
            }

            // Race
            string[] nvssRaces = VR.NvssRace.GetBooleanRaceCodes().ToArray();
            if (!simple)
            {
                string race1 = faker.Random.ArrayElement(nvssRaces);
                string race2 = faker.Random.ArrayElement(nvssRaces);
                string race3 = faker.Random.ArrayElement(nvssRaces);
                record.Race = nvssRaces.Select(raceCode =>
                {
                    if (raceCode == race1 || raceCode == race2 || raceCode == race3)
                    {
                        return Tuple.Create(raceCode, "Y");
                    }
                    return Tuple.Create(raceCode, "N");
                }).ToArray();
            }
            else
            {
                string race1 = faker.Random.ArrayElement(nvssRaces);
                record.Race = nvssRaces.Select(raceCode =>
                {
                    if (raceCode == race1)
                    {
                        return Tuple.Create(raceCode, "Y");
                    }
                    return Tuple.Create(raceCode, "N");
                }).ToArray();
            }

            // Education level

            record.EducationLevelHelper = VR.ValueSets.EducationLevel.Codes[faker.Random.Number(VR.ValueSets.EducationLevel.Codes.GetLength(0) - 1), 0];

            Tuple<string, string>[] occupationIndustries =
            {
                Tuple.Create("secretary", "State agency"),
                Tuple.Create("carpenter", "construction"),
                Tuple.Create("Programmer", "Health Insurance"),
            };
            Tuple<string, string> occInd = faker.Random.ArrayElement<Tuple<string, string>>(occupationIndustries);

            // Occupation

            record.UsualOccupation = occInd.Item1;

            // Industry

            record.UsualIndustry = occInd.Item2;

            // Military Service

            record.MilitaryServiceHelper = VR.ValueSets.YesNoUnknown.Codes[faker.Random.Number(VR.ValueSets.YesNoUnknown.Codes.GetLength(0) - 1), 0];

            // Funeral Home Name

            record.FuneralHomeName = faker.Name.LastName() + " Funeral Home";

            // Funeral Home Address

            Dictionary<string, string> fha = new Dictionary<string, string>();
            fha.Add("addressLine1", $"{faker.Random.Number(999) + 1} {faker.Address.StreetName()}");
            fha.Add("addressCity", "Bedford");
            fha.Add("addressCounty", "Middlesex");
            fha.Add("addressState", "MA");
            fha.Add("addressZip", "01730");
            fha.Add("addressCountry", "US");
            record.FuneralHomeAddress = fha;

            // Disposition Location Name

            record.DispositionLocationName = faker.Name.LastName() + " Cemetery";

            // Disposition Location Address

            Dictionary<string, string> dispLoc = new Dictionary<string, string>();
            dispLoc.Add("addressCity", "Bedford");
            dispLoc.Add("addressCounty", "Middlesex");
            dispLoc.Add("addressState", "MA");
            dispLoc.Add("addressZip", "01730");
            dispLoc.Add("addressCountry", "US");
            record.DispositionLocationAddress = dispLoc;

            // Disposition Method

            record.DecedentDispositionMethodHelper = VRDR.ValueSets.MethodOfDisposition.Codes[faker.Random.Number(VRDR.ValueSets.MethodOfDisposition.Codes.GetLength(0) - 1), 0];

            // Basic Certifier information

            Dictionary<string, string> certifierIdentifier = new Dictionary<string, string>();
            certifierIdentifier.Add("system", "http://hl7.org/fhir/sid/us-npi");
            certifierIdentifier.Add("value", Convert.ToString(faker.Random.Number(999999)));
            record.CertifierIdentifier = certifierIdentifier;

            record.CertifierFamilyName = faker.Name.LastName();
            record.CertifierGivenNames = new string[] { faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female), faker.Name.FirstName(Bogus.DataSets.Name.Gender.Female) };
            record.CertifierSuffix = "MD";

            Dictionary<string, string> certifierAddress = new Dictionary<string, string>();
            certifierAddress.Add("addressLine1", $"{faker.Random.Number(999) + 1} {faker.Address.StreetName()}");
            certifierAddress.Add("addressCity", "Bedford");
            certifierAddress.Add("addressCounty", "Middlesex");
            certifierAddress.Add("addressState", "MA");
            certifierAddress.Add("addressZip", "01730");
            certifierAddress.Add("addressCountry", "US");
            record.CertifierAddress = certifierAddress;

            // Certifier type

            record.CertificationRoleHelper = VRDR.ValueSets.CertifierTypes.Death_Certification_And_Verification_By_Physician_Procedure;

            // TODO Add these fields
            Dictionary<string, string> relationship = new Dictionary<string, string>();
            relationship["text"] = "Spouse";
            record.ContactRelationship = relationship;

            if (type == "Natural")
            {
                record.MannerOfDeathTypeHelper = VRDR.ValueSets.MannerOfDeath.Natural_Death;

                record.DateOfDeath = deathUtc.ToString("o");
                record.DateOfDeathPronouncement = deathUtc.AddHours(1).ToString("o");

                // Randomly pick one of four possible natural causes
                int choice = faker.Random.Number(3);
                if (choice == 0)
                {
                    Tuple<string, string>[] causes =
                    {
                        Tuple.Create("Pulmonary embolism", "30 minutes"),
                        Tuple.Create("Deep venuous thrombosis in left thigh", "3 days"),
                        Tuple.Create("Acute hepatic failure", "3 days"),
                        Tuple.Create("Moderately differentiated hepatocellular carcinoma", "over 3 months")
                    };
                    record.CausesOfDeath = causes;

                    record.AutopsyPerformedIndicatorHelper = VR.ValueSets.YesNoUnknown.No;
                    record.AutopsyResultsAvailableHelper = VR.ValueSets.YesNoUnknown.No;
                    record.ExaminerContactedHelper = VR.ValueSets.YesNoUnknown.No;

                    record.TobaccoUseHelper = VRDR.ValueSets.ContributoryTobaccoUse.No;
                }
                else if (choice == 1)
                {
                    Tuple<string, string>[] causes =
                    {
                        Tuple.Create("Acute myocardial infarction", "2 days"),
                        Tuple.Create("Arteriosclerotic heart disease", "10 years")
                    };
                    record.CausesOfDeath = causes;

                    record.ContributingConditions = "Carcinoma of cecum, Congestive heart failure";

                    record.AutopsyPerformedIndicatorHelper = VR.ValueSets.YesNoUnknown.No;
                    record.AutopsyResultsAvailableHelper = VR.ValueSets.YesNoUnknown.No;
                    record.ExaminerContactedHelper = VR.ValueSets.YesNoUnknown.No;

                    record.TobaccoUseHelper = VRDR.ValueSets.ContributoryTobaccoUse.No;
                }
                else if (choice == 2)
                {
                    Tuple<string, string>[] causes =
                    {
                        Tuple.Create("Pulmonary embolism", "1 hour"),
                        Tuple.Create("Acute myocardial infarction", "7 days"),
                        Tuple.Create("Chronic ischemic heart disease", "8 years")
                    };
                    record.CausesOfDeath = causes;

                    record.ContributingConditions = "Non-insulin-dependent diabetes mellitus, Obesity, Hypertension, Congestive heart failure";

                    record.AutopsyPerformedIndicatorHelper = VR.ValueSets.YesNoUnknown.No;
                    record.AutopsyResultsAvailableHelper = VR.ValueSets.YesNoUnknown.No;
                    record.ExaminerContactedHelper = VR.ValueSets.YesNoUnknown.No;

                    record.TobaccoUseHelper = VRDR.ValueSets.ContributoryTobaccoUse.Yes;
                }
                else if (choice == 3)
                {
                    Tuple<string, string>[] causes =
                    {
                        Tuple.Create("Rupture of left ventricle", "Minutes"),
                        Tuple.Create("Myocardial infarction", "2 Days"),
                        Tuple.Create("Coronary atherosclerosis", "2 Years")
                    };
                    record.CausesOfDeath = causes;

                    record.ContributingConditions = "Non-insulin-dependent diabetes mellitus, Cigarette smoking, Hypertension, Hypercholesterolemia, Coronary bypass surgery";

                    record.AutopsyPerformedIndicatorHelper = VR.ValueSets.YesNoUnknown.Yes;
                    record.AutopsyResultsAvailableHelper = VR.ValueSets.YesNoUnknown.Yes;
                    record.ExaminerContactedHelper = VR.ValueSets.YesNoUnknown.Yes;

                    record.TobaccoUseHelper = VRDR.ValueSets.ContributoryTobaccoUse.Yes;
                }
            }
            if (type == "Injury")
            {
                // Randomly pick one of three possible injury causes
                int choice = faker.Random.Number(2);
                if (choice == 0)
                {
                    Tuple<string, string>[] causes =
                    {
                        Tuple.Create("Carbon monoxide poisoning", "Unkown"),
                        Tuple.Create("Inhalation of automobile exhaust fumes", "Unkown")
                    };
                    record.CausesOfDeath = causes;

                    record.ContributingConditions = "Terminal gastric adenocarcinoma, depression";

                    record.AutopsyPerformedIndicatorHelper = VR.ValueSets.YesNoUnknown.Yes;
                    record.AutopsyResultsAvailableHelper = VR.ValueSets.YesNoUnknown.Yes;
                    record.ExaminerContactedHelper = VR.ValueSets.YesNoUnknown.Yes;

                    record.TobaccoUseHelper = VRDR.ValueSets.ContributoryTobaccoUse.Unknown;

                    record.MannerOfDeathTypeHelper = VRDR.ValueSets.MannerOfDeath.Suicide;

                    Dictionary<string, string> detailsOfInjury = new Dictionary<string, string>();
                    record.InjuryLocationName = "Own home garage";
                    record.InjuryDate = new DateTimeOffset(deathUtc.Year, deathUtc.Month, deathUtc.Day, 0, 0, 0, TimeSpan.Zero).ToString("s");
                    record.InjuryLocationLatitude = "70.4";
                    record.InjuryLocationLongitude = "-30";

                    Dictionary<string, string> detailsOfInjuryAddr = new Dictionary<string, string>();
                    detailsOfInjuryAddr.Add("addressLine1", residence["addressLine1"]);
                    detailsOfInjuryAddr.Add("addressCity", residence["addressCity"]);
                    detailsOfInjuryAddr.Add("addressCounty", residence["addressCounty"]);
                    detailsOfInjuryAddr.Add("addressState", state);
                    detailsOfInjuryAddr.Add("addressCountry", "US");
                    record.InjuryLocationAddress = detailsOfInjuryAddr;

                    record.InjuryPlaceDescription = "Home";

                    record.DateOfDeath = new DateTimeOffset(deathUtc.Year, deathUtc.Month, deathUtc.Day, 0, 0, 0, TimeSpan.Zero).ToString("s");
                }
                else if (choice == 1)
                {
                    Tuple<string, string>[] causes =
                    {
                        Tuple.Create("Cardiac tamponade", "15 minutes"),
                        Tuple.Create("Perforation of heart", "20 minutes"),
                        Tuple.Create("Gunshot wound to thorax", "20 minutes")
                    };
                    record.CausesOfDeath = causes;

                    record.AutopsyPerformedIndicatorHelper = VR.ValueSets.YesNoUnknown.Yes;
                    record.AutopsyResultsAvailableHelper = VR.ValueSets.YesNoUnknown.Yes;
                    record.ExaminerContactedHelper = VR.ValueSets.YesNoUnknown.No;

                    record.TobaccoUseHelper = VRDR.ValueSets.ContributoryTobaccoUse.No;

                    record.MannerOfDeathTypeHelper = VRDR.ValueSets.MannerOfDeath.Homicide;

                    Dictionary<string, string> detailsOfInjury = new Dictionary<string, string>();
                    record.InjuryLocationName = "restaurant";
                    record.InjuryDate = deathUtc.AddMinutes(-20).ToString("s");
                    record.InjuryLocationLatitude = "70.4";
                    record.InjuryLocationLongitude = "-30.5";

                    Dictionary<string, string> detailsOfInjuryAddr = new Dictionary<string, string>();
                    detailsOfInjuryAddr.Add("addressLine1", residence["addressLine1"]);
                    detailsOfInjuryAddr.Add("addressCity", "Bedford");
                    detailsOfInjuryAddr.Add("addressCounty", "Middlesex");
                    detailsOfInjuryAddr.Add("addressState", "MA");
                    detailsOfInjuryAddr.Add("addressCountry", "US");
                    record.InjuryLocationAddress = detailsOfInjuryAddr;

                    record.InjuryPlaceDescription = "Trade and Service Area";
                }
                else if (choice == 2)
                {
                    Tuple<string, string>[] causes =
                    {
                        Tuple.Create("Cerebral contusion", "minutes"),
                        Tuple.Create("Fractured skull", "minutes"),
                        Tuple.Create("Blunt impact to head", "minutes")
                    };
                    record.CausesOfDeath = causes;

                    record.AutopsyPerformedIndicatorHelper = VR.ValueSets.YesNoUnknown.No;
                    record.AutopsyResultsAvailableHelper = VR.ValueSets.YesNoUnknown.No;
                    record.ExaminerContactedHelper = VR.ValueSets.YesNoUnknown.Yes;

                    record.TobaccoUseHelper = VRDR.ValueSets.ContributoryTobaccoUse.No;

                    record.MannerOfDeathTypeHelper = VRDR.ValueSets.MannerOfDeath.Accidental_Death;

                    Dictionary<string, string> detailsOfInjury = new Dictionary<string, string>();
                    record.InjuryLocationName = "Highway";
                    record.InjuryDate = deathUtc.ToString("s");
                    record.InjuryLocationLatitude = "70.4";
                    record.InjuryLocationLongitude = "-30.5";

                    Dictionary<string, string> detailsOfInjuryAddr = new Dictionary<string, string>();
                    detailsOfInjuryAddr.Add("addressLine1", residence["addressLine1"]);
                    detailsOfInjuryAddr.Add("addressCity", "Bedford");
                    detailsOfInjuryAddr.Add("addressCounty", "Middlesex");
                    detailsOfInjuryAddr.Add("addressState", "MA");
                    detailsOfInjuryAddr.Add("addressCountry", "US");
                    record.InjuryLocationAddress = detailsOfInjuryAddr;

                    record.InjuryPlaceDescription = "Street/Highway";

                    // TransportationRole
                    record.TransportationRoleHelper = VRDR.ValueSets.TransportationIncidentRole.Passenger;
                }
            }

            if (gender == Bogus.DataSets.Name.Gender.Female)
            {
                record.PregnancyStatusHelper = VRDR.ValueSets.DeathPregnancyStatus.Not_Pregnant_Within_Past_Year;
            }
            else
            {
                record.PregnancyStatusHelper = VRDR.ValueSets.DeathPregnancyStatus.Not_Applicable;
            }

            return record;
        }
    }
}

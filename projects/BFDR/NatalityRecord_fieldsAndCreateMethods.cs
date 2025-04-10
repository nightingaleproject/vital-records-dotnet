using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Hl7.Fhir.Model;
using VR;


// NatalityRecord_fieldsAndCreateMethods.cs
//     Contains field definitions and associated createXXXX methods used to construct a field

namespace BFDR
{
    /// <summary>Class <c>NatalityRecord</c> is an abstract base class models FHIR Vital Records
    /// Birth Reporting (BFDR) Birth and Fetal Death Records. This class was designed to help consume
    /// and produce natality records that follow the HL7 FHIR Vital Records Birth Reporting Implementation
    /// Guide, as described at: http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// </summary>
    public partial class NatalityRecord
    {
        /// <summary>The Natality Subject - eithr a Child or a DecedentFetus.</summary>
        protected Patient Subject;

        /// <summary>The Mother.</summary>
        protected Patient Mother;

        /// <summary>The Father.</summary>
        protected RelatedPerson Father;

        /// <summary>The Attendant.</summary>
        protected Practitioner Attendant;

        /// <summary>The Certifier.</summary>
        protected Practitioner Certifier;

        /// <summary>The coverage associated with the birth.</summary>
        protected Coverage Coverage;
        /// <summary>The maternity encounter.</summary>
        protected Encounter EncounterMaternity;

        private const string RACE_ETHNICITY_PROFILE_MOTHER = "inputraceandethnicityMother";
        private const string RACE_ETHNICITY_PROFILE_FATHER = "inputraceandethnicityFather";
        private const string CODED_RACE_ETHNICITY_PROFILE_FATHER = "codedraceandethnicityFather";
        private const string CODED_RACE_ETHNICITY_PROFILE_MOTHER = "codedraceandethnicityMother";

        /// <summary>Composition Type Constants</summary>
        private const string COMPOSITION_PROVIDER_FETAL_DEATH_REPORT = "69045-3";
        private const string COMPOSITION_PROVIDER_LIVE_BIRTH_REPORT = "68998-4";
        private const string COMPOSITION_JURISDICTION_FETAL_DEATH_REPORT = "92010-8";
        private const string COMPOSITION_JURISDICTION_LIVE_BIRTH_REPORT  ="92011-6";
        private const string COMPOSITION_CODED_CAUSE_OF_FETAL_DEATH = "86804-2";
        private const string COMPOSITION_CODED_RACE_AND_ETHNICITY = "86805-9";
        private const string COMPOSITION_CODED_INDUSTRY_AND_OCCUPATION = "industry_occupation_document";

        /// <summary>Composition Section Constants</summary>
        protected const string MOTHER_PRENATAL_SECTION = "57073-9";
        private const string MEDICAL_INFORMATION_SECTION = "55752-0";
        private const string NEWBORN_INFORMATION_SECTION = "57075-4";
        private const string MOTHER_INFORMATION_SECTION = "92014-0";
        private const string FATHER_INFORMATION_SECTION = "92013-2";
        private const string PATIENT_QUESTIONAIRRE_RESPONSE_SECTION = "74465-6";
        private const string EMERGING_ISSUES_SECTION = "emergingIssues";
        private const string SUCCESSFUL_OUTCOME = "385669000";
        private const string UNSUCCESSFUL_OUTCOME = "385671000";
        private const string DATE_OF_LAST_LIVE_BIRTH = "68499-3";
        private const string DATE_OF_LAST_OTHER_PREGNANCY_OUTCOME = "68500-8";
        private const string NUMBER_OF_PRENATAL_VISITS = "68493-6";
        private const string GESTATIONAL_AGE = "11884-4";
        private const string NUMBER_OF_BIRTHS_NOW_DEAD = "68496-9"; 
        private const string NUMBER_OF_BIRTHS_NOW_LIVING = "11638-4";
        private const string NUMBER_OF_OTHER_PREGNANCY_OUTCOMES = "69043-8";
        private const string MOTHER_RECEIVED_WIC_FOOD = "87303-4";
        private const string INFANT_BREASTFED_AT_DISCHARGE = "73756-9";

        /// <summary> Fetus Section Constant </summary>
        protected const string FETUS_SECTION = "76400-1";

        /// <summary> Coded Cause of Fetal Death Section Constant </summary>
        protected const string CODEDCAUSEOFFETALDEATH_SECTION = "86804-2";

        /// <summary>DemographicComposition Section Constants</summary>
        private const string RACE_ETHNICITY_MOTHER = "MTH";
        private const string RACE_ETHNICITY_FATHER = "NFTH";

        /// <summary>CompositionSections that define the codes that represent the different sections in the composition</summary>
        protected override string[] CompositionSections
        {
            get
            {
                return new string[] {
                    MOTHER_PRENATAL_SECTION, MEDICAL_INFORMATION_SECTION, NEWBORN_INFORMATION_SECTION, MOTHER_INFORMATION_SECTION,
                    FATHER_INFORMATION_SECTION, PATIENT_QUESTIONAIRRE_RESPONSE_SECTION, EMERGING_ISSUES_SECTION, RACE_ETHNICITY_MOTHER, 
                    RACE_ETHNICITY_FATHER, DATE_OF_LAST_LIVE_BIRTH, DATE_OF_LAST_OTHER_PREGNANCY_OUTCOME, NUMBER_OF_PRENATAL_VISITS,
                    GESTATIONAL_AGE, NUMBER_OF_BIRTHS_NOW_DEAD, NUMBER_OF_BIRTHS_NOW_LIVING, NUMBER_OF_OTHER_PREGNANCY_OUTCOMES, MOTHER_RECEIVED_WIC_FOOD, 
                    INFANT_BREASTFED_AT_DISCHARGE
                };
            } 
        }

        /// <summary>Birth record composition sections use LOINC codes</summary>
        protected override string CompositionSectionCodeSystem { get => VR.CodeSystems.LOINC; }
        
        /// <summary>Use annotation on the property to determine whether the subject is
        /// the mother or newborn, then extract the corresponding resource id.</summary>
        /// <param name="propertyName">name of the NatalityRecord property</param>
        /// <returns>the subject's resource id</returns>
        protected override string SubjectId([CallerMemberName] string propertyName = null)
        {
            IEnumerable<FHIRSubject> subjects = this.GetType().GetProperty(propertyName).GetCustomAttributes(false).OfType<FHIRSubject>();
            if ((subjects == null) || subjects.Count() == 0)
            {
                return Mother.Id;
            }
            return subjects.First().subject == FHIRSubject.Subject.Newborn ? Subject.Id : Mother.Id;
        }

        /// <summary>Create Attendant/Practitioner.</summary>
        protected void CreateAttendant()
        {
            Attendant = new Practitioner();
            Attendant.Id = Guid.NewGuid().ToString();
            Attendant.Meta = new Meta();
            string[] attendant_profile = { BFDR.ExtensionURL.PractitionerBirthAttendant };
            Attendant.Meta.Profile = attendant_profile;
            Extension roleExt = new Extension(BFDR.ExtensionURL.ExtensionRole, new Code("attendant"));
            Attendant.Extension.Add(roleExt);
            // Not linked to Composition or inserted in bundle, since this is run before the composition exists.
        }
    
            /// <summary>Create Certifier/Practitioner.</summary>
        protected void CreateCertifier()
        {
            Certifier = new Practitioner();
            Certifier .Id = Guid.NewGuid().ToString();
            Certifier .Meta = new Meta();
            string[] certifier_profile = { BFDR.ExtensionURL.PractitionerBirthCertifier };
            Certifier .Meta.Profile = certifier_profile;
            Extension roleExt = new Extension(BFDR.ExtensionURL.ExtensionRole, new Code("certifier"));
            Certifier.Extension.Add(roleExt);
            // Not linked to Composition or inserted in bundle, since this is run before the composition exists.
        }

        /// <summary>Create and set Birth Location.</summary>
        protected Location CreateAndSetLocationBirth(string code)
        {
            Location locationBirth = new Location
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta()
                {
                    Profile = new List<string>()
                    {
                        ExtensionURL.LocationBFDR
                    }
                },
                Type = new List<CodeableConcept>()
                {
                    new CodeableConcept(CodeSystems.LocalBFDRCodes, code)
                }
            };
            Bundle.AddResourceEntry(locationBirth, "urn:uuid:" + locationBirth.Id);
            return locationBirth;
        }
        

        /// <summary>Create Maternity Encounter.</summary>
        protected Encounter CreateMaternityEncounter()
        {
            EncounterMaternity = CreateEncounter(ProfileURL.EncounterMaternity);
            Extension roleExt = new Extension(BFDR.ExtensionURL.ExtensionRole, new CodeableConcept(CodeSystems.RoleCode_HL7_V3, "MTH"));
            EncounterMaternity.Extension.Add(roleExt);
            return EncounterMaternity;
        }
    }

    /// <summary>Describes the subject of a birth record field</summary>
    public class FHIRSubject : System.Attribute
    {
        /// <summary>People in a birth record</summary>
        public enum Subject
        {
            /// <summary>The mother</summary>
            Mother,
            /// <summary>The newborn</summary>
            Newborn,
            /// <summary>The decedent fetus</summary>
            DecedentFetus
        }
        /// <summary>The subject of the field</summary>
        public Subject subject;

        /// <summary>Constructor.</summary>
        public FHIRSubject(Subject subject)
        {
            this.subject = subject;
        }
    }
}

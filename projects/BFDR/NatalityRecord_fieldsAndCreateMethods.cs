using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.FhirPath;
using Newtonsoft.Json;
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
        /// <summary>The Child.</summary>
        protected Patient Child;

        /// <summary>The Mother.</summary>
        protected Patient Mother;

        /// <summary>The Father.</summary>
        protected RelatedPerson Father;

        /// <summary>The Attendant.</summary>
        protected Practitioner Attendant;

        /// <summary>The Certifier.</summary>
        protected Practitioner Certifier;

        /// <summary>The encounter of the birth.</summary>
        protected Encounter EncounterBirth;

        /// <summary>The coverage associated with the birth.</summary>
        protected Coverage Coverage;
        /// <summary>The maternity encounter.</summary>
        protected Encounter EncounterMaternity;

        /// <summary>Composition Section Constants</summary>
        private const string RACE_ETHNICITY_PROFILE_MOTHER = "inputraceandethnicityMother";
        private const string RACE_ETHNICITY_PROFILE_FATHER = "inputraceandethnicityFather";
        private const string MOTHER_PRENATAL_SECTION = "57073-9";
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

        /// <summary>CompositionSections that define the codes that represent the different sections in the composition</summary>
        protected override string[] CompositionSections
        {
            get
            {
                return new string[] {
                    MOTHER_PRENATAL_SECTION, MEDICAL_INFORMATION_SECTION, NEWBORN_INFORMATION_SECTION, MOTHER_INFORMATION_SECTION,
                    FATHER_INFORMATION_SECTION, PATIENT_QUESTIONAIRRE_RESPONSE_SECTION, EMERGING_ISSUES_SECTION, RACE_ETHNICITY_PROFILE_MOTHER, 
                    RACE_ETHNICITY_PROFILE_FATHER, DATE_OF_LAST_LIVE_BIRTH, DATE_OF_LAST_OTHER_PREGNANCY_OUTCOME, NUMBER_OF_PRENATAL_VISITS,
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
            return subjects.First().subject == FHIRSubject.Subject.Newborn ? Child.Id : Mother.Id;
        }

        /// <summary>Create Attendant/Practitioner.</summary>
        protected void CreateAttendant()
        {
            Attendant = new Practitioner();
            Attendant.Id = Guid.NewGuid().ToString();
            Attendant.Meta = new Meta();
            string[] attendant_profile = { VR.ProfileURL.Practitioner };
            Attendant.Meta.Profile = attendant_profile;
            Extension roleExt = new Extension(VRExtensionURLs.Role, new Code("attendant"));
            Attendant.Extension.Add(roleExt);
            // Not linked to Composition or inserted in bundle, since this is run before the composition exists.
        }
    
            /// <summary>Create Certifier/Practitioner.</summary>
        protected void CreateCertifier()
        {
            Certifier = new Practitioner();
            Certifier .Id = Guid.NewGuid().ToString();
            Certifier .Meta = new Meta();
            string[] certifier_profile = { VR.ProfileURL.Practitioner };
            Certifier .Meta.Profile = certifier_profile;
            Extension roleExt = new Extension(VRExtensionURLs.Role, new Code("certifier"));
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
        
        /// <summary>Create Birth Encounter.</summary>
        protected void CreateBirthEncounter()
        {
            EncounterBirth = new Encounter();
            EncounterBirth .Id = Guid.NewGuid().ToString();
            EncounterBirth .Meta = new Meta();
            string[] encounterBirth_profile = { ProfileURL.EncounterBirth };
            EncounterBirth .Meta.Profile = encounterBirth_profile;
            Extension roleExt = new Extension(VRExtensionURLs.Role, new CodeableConcept(CodeSystems.RoleCode_HL7_V3, "CHILD"));
            EncounterBirth.Extension.Add(roleExt);
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
            Newborn
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

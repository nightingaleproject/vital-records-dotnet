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


// BirthRecord_fieldsAndCreateMethods.cs
//     Contains field definitions and associated createXXXX methods used to construct a field

namespace BFDR
{
    /// <summary>Class <c>BirthRecord</c> models a FHIR Vital Records Birth Reporting (BFDR) Birth
    /// Record. This class was designed to help consume and produce birth records that follow the
    /// HL7 FHIR Vital Records Birth Reporting Implementation Guide, as described at:
    /// http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// </summary>
    public partial class BirthRecord
    {
        /// <summary>The Child.</summary>
        private Patient Child;

        /// <summary>The Mother.</summary>
        private Patient Mother;

        /// <summary>The Father.</summary>
        private RelatedPerson Father;

        /// <summary>The Attendant.</summary>
        private Practitioner Attendant;

        /// <summary>The Mother's Race and Ethnicity provided by Jurisdiction.</summary>
        private Observation InputRaceAndEthnicityObsMother;

        /// <summary>The Father's Race and Ethnicity provided by Jurisdiction.</summary>
        private Observation InputRaceAndEthnicityObsFather;

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
        

        /// <summary>CompositionSections that define the codes that represent the different sections in the composition</summary>
        protected override string[] CompositionSections
        {
            get
            {
                return new string[] {
                    MOTHER_PRENATAL_SECTION, MEDICAL_INFORMATION_SECTION, NEWBORN_INFORMATION_SECTION, MOTHER_INFORMATION_SECTION,
                    FATHER_INFORMATION_SECTION, PATIENT_QUESTIONAIRRE_RESPONSE_SECTION, EMERGING_ISSUES_SECTION, RACE_ETHNICITY_PROFILE_MOTHER, RACE_ETHNICITY_PROFILE_FATHER
                };
            } 
        }

        /// <summary>Birth record composition sections use LOINC codes</summary>
        protected override string CompositionSectionCodeSystem { get => VR.CodeSystems.LOINC; }
        
        /// <summary>Use annotation on the property to determine whether the subject is
        /// the mother or newborn, then extract the corresponding resource id.</summary>
        /// <param name="propertyName">name of the BirthRecord property</param>
        /// <returns>the subject's resource id</returns>
        protected override string SubjectId([CallerMemberName] string propertyName = null)
        {
            IEnumerable<FHIRSubject> subjects = this.GetType().GetProperty(propertyName).GetCustomAttributes(false).OfType<FHIRSubject>();
            if (subjects == null || subjects.Count() == 0)
            {
                return Mother.Id;
            }
            return subjects.First().subject == FHIRSubject.Subject.Newborn ? Child.Id : Mother.Id;
        }

        /// <summary> Create Mother Input Race and Ethnicity </summary>
        private void CreateInputRaceEthnicityObsMother()
        {
            InputRaceAndEthnicityObsMother = new Observation();
            InputRaceAndEthnicityObsMother.Id = Guid.NewGuid().ToString();
            InputRaceAndEthnicityObsMother.Meta = new Meta();
            string[] raceethnicity_profile = { VR.ProfileURL.InputRaceAndEthnicity };
            InputRaceAndEthnicityObsMother.Meta.Profile = raceethnicity_profile;
            InputRaceAndEthnicityObsMother.Status = ObservationStatus.Final;
            InputRaceAndEthnicityObsMother.Code = new CodeableConcept(CodeSystems.InputRaceAndEthnicityPerson, "inputraceandethnicityMother", "Input Race and Ethnicity Person", null);
            InputRaceAndEthnicityObsMother.Subject = new ResourceReference("urn:uuid:" + Child.Id);
            InputRaceAndEthnicityObsMother.Focus.Add(new ResourceReference("urn:uuid:" + Mother.Id));
            AddReferenceToComposition(InputRaceAndEthnicityObsMother.Id, RACE_ETHNICITY_PROFILE_MOTHER);
            Bundle.AddResourceEntry(InputRaceAndEthnicityObsMother, "urn:uuid:" + InputRaceAndEthnicityObsMother.Id);
        }

        /// <summary> Create Father Input Race and Ethnicity </summary>
        private void CreateInputRaceEthnicityObsFather()
        {
            InputRaceAndEthnicityObsFather = new Observation();
            InputRaceAndEthnicityObsFather.Id = Guid.NewGuid().ToString();
            InputRaceAndEthnicityObsFather.Meta = new Meta();
            string[] raceethnicity_profile = { VR.ProfileURL.InputRaceAndEthnicity };
            InputRaceAndEthnicityObsFather.Meta.Profile = raceethnicity_profile;
            InputRaceAndEthnicityObsFather.Status = ObservationStatus.Final;
            InputRaceAndEthnicityObsFather.Code = new CodeableConcept(CodeSystems.InputRaceAndEthnicityPerson, "inputraceandethnicityFather", "Input Race and Ethnicity Person", null);
            InputRaceAndEthnicityObsFather.Subject = new ResourceReference("urn:uuid:" + Child.Id);
            AddReferenceToComposition(InputRaceAndEthnicityObsFather.Id, RACE_ETHNICITY_PROFILE_FATHER);
            Bundle.AddResourceEntry(InputRaceAndEthnicityObsFather, "urn:uuid:" + InputRaceAndEthnicityObsFather.Id);
        }

        /// <summary>Create Practitioner.</summary>
        private void CreatePractitioner()
        {
            Attendant = new Practitioner();
            Attendant.Id = Guid.NewGuid().ToString();
            Attendant.Meta = new Meta();
            string[] attendant_profile = { VR.ProfileURL.Practitioner };
            Attendant.Meta.Profile = attendant_profile;
            // Not linked to Composition or inserted in bundle, since this is run before the composition exists.
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

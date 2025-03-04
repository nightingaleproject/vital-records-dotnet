using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using VR;

// NatalityRecord_constructors.cs
//     Contains constructors and associated methods for the NatalityRecords class
namespace BFDR
{
    /// <summary>Class <c>NatalityRecord</c> is an abstract base class models FHIR Vital Records
    /// Birth Reporting (BFDR) Birth and Fetal Death Records. This class was designed to help consume
    /// and produce natality records that follow the HL7 FHIR Vital Records Birth Reporting Implementation
    /// Guide, as described at: http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// </summary>
    public abstract partial class NatalityRecord : VitalRecord
    {
        // Only some natality records are required to have a subject present
        // Note: At time of development Coded Cause of Fetal Death (86804-2) requires a subject but should not, so it's not included here
        private static readonly string[] COMPOSITIONS_REQUIRING_SUBJECT = {
            COMPOSITION_PROVIDER_FETAL_DEATH_REPORT, COMPOSITION_PROVIDER_LIVE_BIRTH_REPORT,
            COMPOSITION_JURISDICTION_FETAL_DEATH_REPORT, COMPOSITION_JURISDICTION_LIVE_BIRTH_REPORT
        };

        // Within a composition some sections have a focus that references the mother
        private static readonly string[] COMPOSITION_MOTHER_FOCUS_SECTIONS = {
            MOTHER_PRENATAL_SECTION, MEDICAL_INFORMATION_SECTION, MOTHER_INFORMATION_SECTION
        };

        /// <summary>Default constructor that creates a new, empty NatalityRecord.</summary>
        protected NatalityRecord(string bundleProfile) : base()
        {
            InitializeCompositionAndSubject();

            // Start with an empty Bundle.
            Bundle = new Bundle();
            Bundle.Id = Guid.NewGuid().ToString();
            Bundle.Type = Bundle.BundleType.Document; // By default, Bundle type is "document".
            Bundle.Meta = new Meta();
            string[] bundle_profile = { bundleProfile };
            Bundle.Timestamp = DateTime.Now;
            Bundle.Meta.Profile = bundle_profile;

            // Start with an empty mother. Need reference in Composition.
            Mother = new Patient();
            Mother.Id = Guid.NewGuid().ToString();
            Mother.Meta = new Meta();
            string[] mother_profile = { VR.ProfileURL.Mother };
            Mother.Meta.Profile = mother_profile;

            // Start with an empty father.
            Father = new RelatedPerson
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta()
            };
            string[] father_profile = { VR.ProfileURL.RelatedPersonFatherNatural };
            Father.Meta.Profile = father_profile;
            Father.Relationship.Add(new CodeableConcept(CodeSystems.RoleCode_HL7_V3, "NFTH"));

            // Start with an empty Coverage.
            Coverage = new Coverage()
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta()
            };
            Coverage.Meta.Profile = new List<string>()
            {
                ProfileURL.CoveragePrincipalPayerDelivery
            };

            // Start with an empty EncounterMaternity.
            CreateMaternityEncounter();

            CreateCertifier();

            Encounter.ParticipantComponent p = new Encounter.ParticipantComponent();
            p.Type.Add(new CodeableConcept(VR.CodeSystems.LOINC, "87287-9"));
            p.Individual = new ResourceReference("urn:uuid:" + Certifier.Id);
            EncounterMaternity.Participant.Add(p);

            CreateAttendant();

            //TODO: Author is a required field in the composition
            // Author = new Organization();
            // AuthorOrganization.Id = Guid.NewGuid().ToString();
            // AuthorOrganization.Active = true;
            // // Organization requires a name
            // AuthorOrganization.Name = "VRO";


            // TODO: Start with an empty certification. - need reference in Composition
            // CreateBirthCertification();

            // Add Composition to bundle. As the record is filled out, new entries will be added to this element.
            // Sections will be added to the composition as needed by the VitalRecord.AddReferenceToComposition method
            Composition.Id = Guid.NewGuid().ToString();
            Composition.Status = CompositionStatus.Final;
            Composition.Subject = new ResourceReference("urn:uuid:" + Subject.Id);
            // Author for jurisdictions is an organization (VRO)
            // Composition.Author.Add(new ResourceReference("urn:uuid:" + Author.Id));
            Composition.Attester.Add(new Composition.AttesterComponent());
            //Composition.Attester.First().Party = new ResourceReference("urn:uuid:" + Certifier.Id);
            Composition.Attester.First().ModeElement = new Code<Hl7.Fhir.Model.Composition.CompositionAttestationMode>(Hl7.Fhir.Model.Composition.CompositionAttestationMode.Legal);
            Hl7.Fhir.Model.Composition.EventComponent eventComponent = new Hl7.Fhir.Model.Composition.EventComponent();
            eventComponent.Code.Add(new CodeableConcept(CodeSystems.SCT, "103693007", "Diagnostic procedure (procedure)", null));
            //eventComponent.Detail.Add(new ResourceReference("urn:uuid:" + BirthCertification.Id));
            Composition.Event.Add(eventComponent);
            Bundle.AddResourceEntry(Composition, "urn:uuid:" + Composition.Id);

            // Add entries for the child, mother, and father.
            Bundle.AddResourceEntry(Subject, "urn:uuid:" + Subject.Id);
            Bundle.AddResourceEntry(Mother, "urn:uuid:" + Mother.Id);
            Bundle.AddResourceEntry(Father, "urn:uuid:" + Father.Id);

            Bundle.AddResourceEntry(Certifier, "urn:uuid:" + Certifier.Id);
            Bundle.AddResourceEntry(Attendant, "urn:uuid:" + Attendant.Id);
            Bundle.AddResourceEntry(EncounterMaternity, "urn:uuid:" + EncounterMaternity.Id);
            Bundle.AddResourceEntry(Coverage, "urn:uuid:" + Coverage.Id);
            // AddReferenceToComposition(BirthCertification.Id, "BirthCertification");
            // Bundle.AddResourceEntry(BirthCertification, "urn:uuid:" + BirthCertification.Id);

            // Create a Navigator for this new birth record.
            Navigator = Bundle.ToTypedElement();

            UpdateRecordIdentifier();
        }

        /// <summary>Initialize Composition and Subject.</summary>
        protected abstract void InitializeCompositionAndSubject();

        /// <summary>Constructor that takes a string that represents a FHIR Natality Record in either XML or JSON format.</summary>
        /// <param name="record">represents a FHIR Natality Record in either XML or JSON format.</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <exception cref="ArgumentException">Record is neither valid XML nor JSON.</exception>
        protected NatalityRecord(string record, bool permissive = false) : base(record, permissive){}

        /// <summary>Constructor that takes a FHIR Bundle that represents a FHIR Natality Record.</summary>
        /// <param name="bundle">represents a FHIR Bundle.</param>
        /// <exception cref="ArgumentException">Record is invalid.</exception>
        protected NatalityRecord(Bundle bundle)
        {
            Bundle = bundle;
            Navigator = Bundle.ToTypedElement();
            RestoreReferences();
        }

        /// <summary>Abstract GetYear method to be implemented differently by the BirthRecord and FetalDeathRecord subclasses</summary>
        public abstract uint? GetYear();

        /// <summary>Update the bundle identifier from the component fields.</summary>
        private void UpdateRecordIdentifier()
        {
            uint certificateNumber = 0;
            if (CertificateNumber != null)
            {
                UInt32.TryParse(CertificateNumber, out certificateNumber);
            }
            uint year = 0;
            if (this.GetYear() != null)
            {
                year = (uint)this.GetYear();
            }
            
            String jurisdictionId = this.EventLocationJurisdiction;
            if (jurisdictionId == null || jurisdictionId.Trim().Length < 2)
            {
                jurisdictionId = "XX";
            }
            else
            {
                jurisdictionId = jurisdictionId.Trim().Substring(0, 2).ToUpper();
            }
            this.RecordIdentifier = $"{year:D4}{jurisdictionId}{certificateNumber:D6}";

        }

        /// <summary>Helper method to return the subset of this record that makes up a DemographicCodedContent bundle.</summary>
        /// <returns>a new FHIR Bundle</returns>
        public Bundle GetDemographicCodedContentBundle()
        {
            Bundle dccBundle = new Bundle();
            dccBundle.Id = Guid.NewGuid().ToString();
            dccBundle.Type = Bundle.BundleType.Document;
            dccBundle.Meta = new Meta();
            string[] profile = { ProfileURL.BundleDocumentDemographicCodedContent };
            dccBundle.Meta.Profile = profile;
            dccBundle.Timestamp = DateTime.Now;
            // Make sure to include the base identifiers, including certificate number and auxiliary state IDs
            dccBundle.Identifier = Bundle.Identifier;
            // Add composition; we should always create a new composition appropriate for this bundle type
            Composition composition = new Composition();
            composition.Id = Guid.NewGuid().ToString();
            composition.Status = CompositionStatus.Final;
            composition.Meta = new Meta();
            string[] composition_profile = { ProfileURL.CompositionCodedRaceAndEthnicity };
            composition.Meta.Profile = composition_profile;
            composition.Type = new CodeableConcept(CodeSystems.LOINC, "86805-9", "Race and ethnicity information Document", null);
            // Note: Subject is optional in this type of composition, and not included in the bundle, so we leave it out
            // Note: Author is a required field for the composition, for now only expected to be NCHS for this type of bundle
            Organization author = new Organization();
            author.Id = Guid.NewGuid().ToString();
            author.Active = true;
            author.Name = "National Center for Health Statistics";
            composition.Author = new List<ResourceReference> { new ResourceReference("urn:uuid:" + author.Id) };
            AddResourceToBundleIfPresent(author, dccBundle);
            composition.Title = "Demographic Coded Content";
            if (Mother != null)
            {
                Composition.SectionComponent motherSection = new Composition.SectionComponent
                {
                  Code = new CodeableConcept(CodeSystems.RoleCode_HL7_V3, "MTH")
                  //TODO: add mother, input, coded race/ethnicity reference slices
                };
                composition.Section.Add(motherSection);
            } else if (Father != null)
            {
                Composition.SectionComponent fatherSection = new Composition.SectionComponent
                {
                  Code = new CodeableConcept(CodeSystems.RoleCode_HL7_V3, "NFTH")
                  //TODO: add father, input, coded race/ethnicity reference slices
                };
                composition.Section.Add(fatherSection);

            } else 
            {
                //TODO: demographic content composition should have a relevant mother and/or father - this should be an exception
                Console.WriteLine("Warning: Failed to find a Mother or Father for Demographic Information.");
            }            
            // NOTE: If we want to put observations in the coded content bundle that don't have references we'll
            // need to move them over by grabbing them by the observation code
            return dccBundle;
        }

        /// <summary>Restores class references from a newly parsed record.</summary>
        protected override void RestoreReferences()
        {
            // Note: Unlike mortality records, where some bundles are collections so don't have a composition,
            // all natality records are documents and so should have a composition
            Composition = (Composition) Bundle.Entry.FirstOrDefault(entry => entry.Resource is Composition)?.Resource;
            if (Composition == null)
            {
                throw new System.ArgumentException("Failed to find a Composition. The first entry in the FHIR Bundle should be a Composition.");
            }

            // See if this is a type of bundle that requires a subject
            bool requiresSubject = COMPOSITIONS_REQUIRING_SUBJECT.Contains(Composition.Type?.Coding?[0]?.Code);

            // Depending on the type of bundle, some of this information may not be present, so check it in a null-safe way
            if (requiresSubject && String.IsNullOrWhiteSpace(Composition.Subject?.Reference))
            {
                throw new System.ArgumentException("The Composition is missing a subject reference to the Child resource.");
            }

            // Retrieve Subject (Child/Fetus)
            List<Patient> patients = Bundle.Entry.FindAll(entry => entry.Resource is Patient).ConvertAll(entry => (Patient) entry.Resource);
            string subjectId = Composition.Subject?.Reference;
            if (subjectId != null)
            {
                Subject = patients.Find(patient => subjectId.Contains(patient.Id));
            }
            if (requiresSubject && Subject == null)
            {
                throw new System.ArgumentException("The Bundle is missing a Patient resource for the child.");
            }

            // Retrieve the mother as well, if present in the overall record.
            // If present, it should be the focus of one of the mother-related sections in the compositon
            // Note: Some bundles (e.g. coded race and ethnicity) have a MTH section with an optional reference
            // that's not a focus; we ignore these since they are not expected to have mother patient entries
            Composition.SectionComponent motherFocusSection =
                Composition.Section.Find(section =>
                    COMPOSITION_MOTHER_FOCUS_SECTIONS.Contains(section?.Code?.Coding?[0]?.Code) &&
                    section.Focus?.Reference != null
                );
            string motherId = motherFocusSection?.Focus?.Reference;
            if (motherId != null)
            {
                // TODO: If not found, consider an error for record types where Mother is expected
                Mother = patients.Find(patient => motherId.Contains(patient.Id));
            }

            // Retrieve the Father
            Father = Bundle.Entry.FindAll(entry => entry.Resource is RelatedPerson)
                     .ConvertAll(entry => (RelatedPerson) entry.Resource)
                     .Find(resource => resource?.Relationship?[0]?.Coding?[0]?.Code == "NFTH");

            // Retrieve Coverage information; note that we expect only a single coverage entry in the record
            Coverage = (Coverage)Bundle.Entry.Find(entry => entry.Resource is Coverage)?.Resource;

            // Retrieve attendant and certifier
            List<Practitioner> practitioners = Bundle.Entry.FindAll(entry => entry.Resource is Practitioner).ConvertAll(entry => (Practitioner) entry.Resource);
            Attendant = practitioners.Find(practitioner => practitioner.Extension.Any(ext => Convert.ToString(ext.Value) == "attendant"));
            Certifier = practitioners.Find(practitioner => practitioner.Extension.Any(ext => Convert.ToString(ext.Value) == "certifier"));

            // TODO: when supporting fetal death, will have to impl. fetalDeathIdentifier
            // We want to update the record identifier for records that include a subject
            if (requiresSubject)
            {
                UpdateRecordIdentifier();
            }

            // Scan through all Observations to make sure they all have codes!
            foreach (var ob in Bundle.Entry.Where(entry => entry.Resource is Observation))
            {
                Observation obs = (Observation)ob.Resource;
                if (obs.Code == null || obs.Code.Coding == null || obs.Code.Coding.FirstOrDefault() == null || obs.Code.Coding.First().Code == null)
                {
                    throw new System.ArgumentException("Found an Observation resource that did not contain a code. All Observations must include a code to specify what the Observation is referring to.");
                }
            }

        }
    }
}
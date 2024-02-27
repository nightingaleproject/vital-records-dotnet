using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using VR;

// BirthRecord_constructors.cs
//     Contains constructors and associated methods for the BirthRecords class
namespace BFDR
{
    /// <summary>Class <c>BirthRecord</c> models a FHIR Vital Records Birth Reporting (BFDR) Birth
    /// Record. This class was designed to help consume and produce birth records that follow the
    /// HL7 FHIR Vital Records Birth Reporting Implementation Guide, as described at:
    /// http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// </summary>
    public partial class BirthRecord : VitalRecord
    {
        /// <summary>Default constructor that creates a new, empty BirthRecord.</summary>
        public BirthRecord() : base()
        {
            // Start with an empty Bundle.
            Bundle = new Bundle();
            Bundle.Id = Guid.NewGuid().ToString();
            Bundle.Type = Bundle.BundleType.Document; // By default, Bundle type is "document".
            Bundle.Meta = new Meta();
            string[] bundle_profile = { ProfileURL.BundleDocumentBFDR };
            Bundle.Timestamp = DateTime.Now;
            Bundle.Meta.Profile = bundle_profile;

            // Start with an empty child. Need reference in Composition.
            Child = new Patient();
            Child.Id = Guid.NewGuid().ToString();
            Child.Meta = new Meta();
            string[] child_profile = { VR.IGURL.Child };
            Child.Meta.Profile = child_profile;

            // Start with an empty mother. Need reference in Composition.
            Mother = new Patient();
            Mother.Id = Guid.NewGuid().ToString();
            Mother.Meta = new Meta();
            string[] mother_profile = { VR.IGURL.Mother };
            Mother.Meta.Profile = mother_profile;

            // Start with an empty father.
            Father = new RelatedPerson
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta()
            };
            string[] father_profile = { VR.IGURL.RelatedPersonFatherNatural };
            Father.Meta.Profile = father_profile;
            Father.Relationship.Add(new CodeableConcept(CodeSystems.RoleCode_HL7_V3, "NFTH"));

            // Start with an empty EncounterBirth.
            EncounterBirth = new Encounter()
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta()
            };
            EncounterBirth.Meta.Profile = new List<string>()
            {
                ProfileURL.EncounterBirth
            };

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
            EncounterMaternity = new Encounter()
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta()
            };
            EncounterMaternity.Meta.Profile = new List<string>()
            {
                ProfileURL.Encounter_Maternity
            };

            // TODO: Start with an empty certifier. - Need reference in Composition
            CreateCertifier();
            CreateAttendant();

            // TODO: Start with an empty certification. - need reference in Composition
            //CreateBirthCertification();

            // Add Composition to bundle. As the record is filled out, new entries will be added to this element.
            // Sections will be added to the composition as needed by the VitalRecord.AddReferenceToComposition method
            Composition.Id = Guid.NewGuid().ToString();
            Composition.Status = CompositionStatus.Final;
            Composition.Meta = new Meta();
            string[] composition_profile = { ProfileURL.CompositionJurisdictionLiveBirthReport };
            Composition.Meta.Profile = composition_profile;
            Composition.Type = new CodeableConcept(CodeSystems.LOINC, "71230-7", "Birth certificate", null);
            Composition.Subject = new ResourceReference("urn:uuid:" + Child.Id);
            //Composition.Author.Add(new ResourceReference("urn:uuid:" + Certifier.Id));
            Composition.Title = "Birth Certificate";
            Composition.Attester.Add(new Composition.AttesterComponent());
            //Composition.Attester.First().Party = new ResourceReference("urn:uuid:" + Certifier.Id);
            Composition.Attester.First().ModeElement = new Code<Hl7.Fhir.Model.Composition.CompositionAttestationMode>(Hl7.Fhir.Model.Composition.CompositionAttestationMode.Legal);
            Hl7.Fhir.Model.Composition.EventComponent eventComponent = new Hl7.Fhir.Model.Composition.EventComponent();
            eventComponent.Code.Add(new CodeableConcept(CodeSystems.SCT, "103693007", "Diagnostic procedure (procedure)", null));
            //eventComponent.Detail.Add(new ResourceReference("urn:uuid:" + BirthCertification.Id));
            Composition.Event.Add(eventComponent);
            Bundle.AddResourceEntry(Composition, "urn:uuid:" + Composition.Id);


            // Add entries for the child, mother, and father.
            Bundle.AddResourceEntry(Child, "urn:uuid:" + Child.Id);
            Bundle.AddResourceEntry(Mother, "urn:uuid:" + Mother.Id);
            Bundle.AddResourceEntry(Father, "urn:uuid:" + Father.Id);

            // AddReferenceToComposition(Certifier.Id, "BirthCertification");
            Bundle.AddResourceEntry(Certifier, "urn:uuid:" + Certifier.Id);
            Bundle.AddResourceEntry(Attendant, "urn:uuid:" + Attendant.Id);
            // AddReferenceToComposition(BirthCertification.Id, "BirthCertification");
            // Bundle.AddResourceEntry(BirthCertification, "urn:uuid:" + BirthCertification.Id);

            // AddReferenceToComposition(Pronouncer.Id, "OBE");
            // Bundle.AddResourceEntry(Pronouncer, "urn:uuid:" + Pronouncer.Id);
            //Bundle.AddResourceEntry(Mortician, "urn:uuid:" + Mortician.Id);
            //Bundle.AddResourceEntry(FuneralHomeDirector, "urn:uuid:" + FuneralHomeDirector.Id);

            // Create a Navigator for this new birth record.
            Navigator = Bundle.ToTypedElement();

            UpdateBirthRecordIdentifier();
        }

        /// <summary>Constructor that takes a string that represents a FHIR Birth Record in either XML or JSON format.</summary>
        /// <param name="record">represents a FHIR Birth Record in either XML or JSON format.</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <exception cref="ArgumentException">Record is neither valid XML nor JSON.</exception>
        public BirthRecord(string record, bool permissive = false) : base(record, permissive){}

        /// <summary>Constructor that takes a FHIR Bundle that represents a FHIR Birth Record.</summary>
        /// <param name="bundle">represents a FHIR Bundle.</param>
        /// <exception cref="ArgumentException">Record is invalid.</exception>
        public BirthRecord(Bundle bundle)
        {
            Bundle = bundle;
            Navigator = Bundle.ToTypedElement();
            RestoreReferences();
        }

        /// <summary>Helper method to return the subset of this record that makes up a DemographicCodedContent bundle.</summary>
        /// <returns>a new FHIR Bundle</returns>
        public Bundle GetDemographicCodedContentBundle()
        {
            Bundle dccBundle = new Bundle();
            dccBundle.Id = Guid.NewGuid().ToString();
            dccBundle.Type = Bundle.BundleType.Collection;
            dccBundle.Meta = new Meta();
            // TODO: URLs.cs has the profile URL with the extensions instead of the profiles, fix this once URLs.cs is fixed
            string[] profile = { ExtensionURL.DemographicCodedContentBundleBFDR };
            dccBundle.Meta.Profile = profile;
            dccBundle.Timestamp = DateTime.Now;
            // Make sure to include the base identifiers, including certificate number and auxiliary state IDs
            dccBundle.Identifier = Bundle.Identifier;
            // TODO: Here we'd determine what resources to add to this particular bundle, see VRDR for example
            // NOTE: If we want to put observations in the coded content bundle that don't have references we'll
            // need to move them over by grabbing them by the observation code
            return dccBundle;
        }

        /// <summary>Restores class references from a newly parsed record.</summary>
        protected override void RestoreReferences()
        {
            // Depending on the type of bundle, some of this information may not be present, so check it in a null-safe way
            string profile = Bundle.Meta?.Profile?.FirstOrDefault();
            // TODO: The BFDR composition type can be one of 4 types, described here:
            // https://build.fhir.org/ig/HL7/fhir-bfdr/StructureDefinition-Bundle-document-bfdr.html
            bool fullRecord = false;    // VRDR.ProfileURL.DeathCertificateDocument.Equals(profile);
            // Grab Composition
            var compositionEntry = Bundle.Entry.FirstOrDefault(entry => entry.Resource is Composition);
            if (compositionEntry != null)
            {
                Composition = (Composition)compositionEntry.Resource;
            }
            else if (fullRecord)
            {
                throw new System.ArgumentException("Failed to find a Composition. The first entry in the FHIR Bundle should be a Composition.");
            }
            // Grab Child and Mother.
            if (fullRecord && (Composition.Subject == null || String.IsNullOrWhiteSpace(Composition.Subject.Reference)))
            {
                throw new System.ArgumentException("The Composition is missing a subject (a reference to the Child resource).");
            }
            List<Patient> patients = Bundle.Entry.FindAll(entry => entry.Resource is Patient).ConvertAll(entry => (Patient) entry.Resource);
            Child = patients.Find(patient => patient.Meta.Profile.Any(patientProfile => patientProfile == VR.IGURL.Child));
            Mother = patients.Find(patient => patient.Meta.Profile.Any(patientProfile => patientProfile == VR.IGURL.Mother));
            // Grab Father
            Father = Bundle.Entry.FindAll(entry => entry.Resource is RelatedPerson).ConvertAll(entry => (RelatedPerson) entry.Resource).Find(resource => resource.Meta.Profile.Any(relatedPersonProfile => relatedPersonProfile == VR.IGURL.RelatedPersonFatherNatural));
            EncounterBirth = Bundle.Entry.FindAll(entry => entry.Resource is Encounter).ConvertAll(entry => (Encounter) entry.Resource).Find(resource => resource.Meta.Profile.Any(relatedPersonProfile => relatedPersonProfile == ProfileURL.EncounterBirth));
            Coverage = Bundle.Entry.FindAll(entry => entry.Resource is Coverage).ConvertAll(entry => (Coverage) entry.Resource).Find(resource => resource.Meta.Profile.Any(coverageProfile => coverageProfile == ProfileURL.CoveragePrincipalPayerDelivery));
            EncounterMaternity = Bundle.Entry.FindAll(entry => entry.Resource is Encounter).ConvertAll(entry => (Encounter) entry.Resource).Find(resource => resource.Meta.Profile.Any(relatedPersonProfile => relatedPersonProfile == ProfileURL.Encounter_Maternity));
            // Grab attendant and certifier
            List<Practitioner> practitioners = Bundle.Entry.FindAll(entry => entry.Resource is Practitioner).ConvertAll(entry => (Practitioner) entry.Resource);
            Attendant = practitioners.Find(patient => patient.Extension.Any(ext => Convert.ToString(ext.Value) == "attendant"));
            Certifier = practitioners.Find(patient => patient.Extension.Any(ext => Convert.ToString(ext.Value) == "certifier"));

            if (fullRecord && Child == null)
            {
                throw new System.ArgumentException("Failed to find a Child (Patient).");
            }
            if (fullRecord)
            {
                // TODO - this may need to have some behavior based on VRDR.RestoreReferences().
            }

            // Scan through all Observations to make sure they all have codes!
            foreach (var ob in Bundle.Entry.Where(entry => entry.Resource is Observation))
            {
                Observation obs = (Observation)ob.Resource;
                if (obs.Code == null || obs.Code.Coding == null || obs.Code.Coding.FirstOrDefault() == null || obs.Code.Coding.First().Code == null)
                {
                    throw new System.ArgumentException("Found an Observation resource that did not contain a code. All Observations must include a code to specify what the Observation is referring to.");
                }
                switch (obs.Code.Coding.First().Code)
                {
                    case "inputraceandethnicityMother":
                        InputRaceAndEthnicityObsMother = (Observation)obs;
                        break;
                    case "inputraceandethnicityFather":
                        InputRaceAndEthnicityObsFather = (Observation)obs;
                        break;
                    default:
                        // skip
                        break;
                }
            }

        }
    }
}
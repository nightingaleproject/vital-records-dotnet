using System;
using System.Linq;
using Hl7.Fhir.Model;
using VR;

namespace BFDR
{
  /// <summary>Class <c>FetalDeathRecord</c> is a class designed to help consume and produce fetal death
  /// records that follow the HL7 FHIR Vital Records FetalDeath Reporting Implementation Guide, as described
  /// at: http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
  /// </summary>
  public partial class FetalDeathRecord : NatalityRecord
  {

    /// <summary>Default constructor that creates a new, empty FetalDeathRecord.</summary>
    public FetalDeathRecord() : base(ProfileURL.BundleDocumentFetalDeathReport)
    {
      Composition.Encounter = new ResourceReference("urn:uuid:" + EncounterMaternity.Id);
      this.PatientFetalDeath = true;
    }

    /// <summary>Constructor that takes a string that represents a FHIR FetalDeath Record in either XML or JSON format.</summary>
    /// <param name="record">represents a FHIR FetalDeath Record in either XML or JSON format.</param>
    /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
    /// <exception cref="ArgumentException">Record is neither valid XML nor JSON.</exception>
    public FetalDeathRecord(string record, bool permissive = false) : base(record, permissive) { }

    /// <summary>Constructor that takes a FHIR Bundle that represents a FHIR FetalDeath Record.</summary>
    /// <param name="bundle">represents a FHIR Bundle.</param>
    /// <exception cref="ArgumentException">Record is invalid.</exception>
    public FetalDeathRecord(Bundle bundle) : base(bundle) { }

    /// <summary>Return the delivery year for this record to be used in the identifier</summary>
    public override uint? GetYear()
    {
      // TODO - make sure this parsedatelements works for Timezones
      ParseDateElements(this.DateOfDelivery, out int? year, out _, out _);
      return (uint?) year;
    }

      /// <summary>Helper method to return the subset of this record that makes up a CodedCauseOfFetalDeath bundle.</summary>
      /// <returns>a new FHIR Bundle</returns>
      public Bundle GetCodedCauseOfFetalDeathBundle()
      {
        // Create the base bundle
        Bundle ccofdBundle = BaseBundle(ProfileURL.BundleDocumentCodedCauseOfFetalDeath,
                                        ProfileURL.CompositionCodedCauseOfFetalDeath,
                                        new CodeableConcept(CodeSystems.LOINC, "86804-2", "Cause of death classification and related information Document", null),
                                        "Coded Cause of Fetal Death",
                                        "National Center for Health Statistics");
        // Add the correct observations to the bundle and composition
        AddResourceToBundleAndComposition(GetObservation("92022-3"), "86804-2", CodeSystems.LOINC, ccofdBundle);
        AddResourceToBundleAndComposition(GetObservation("92023-1"), "86804-2", CodeSystems.LOINC, ccofdBundle);
        return ccofdBundle;
    }

    /// <inheritdoc/>
    protected override void RestoreReferences()
    {
      // Do we need differentiate anything for [http://hl7.org/fhir/us/bfdr/StructureDefinition/Bundle-document-demographic-coded-content]? It's not technically a Fetal Death Report bundle... https://build.fhir.org/ig/HL7/fhir-bfdr/StructureDefinition-Bundle-document-demographic-coded-content.html
      // Restore the common references between Birth Records and Fetal Death Records.
      base.RestoreReferences();
      // Restore FetalDeath specific references.
      string maternityEncounterId = Composition?.Encounter?.Reference;
      EncounterMaternity = (Encounter)Bundle.Entry.Find(entry => entry.Resource is Encounter && maternityEncounterId.Contains(entry.Resource.Id))?.Resource;
    }

    /// <inheritdoc/>
    protected override void InitializeCompositionAndSubject()
    {
      // Initialize empty FetalDeathRecord specific objects.
      // Start with an empty decedent fetus. Need reference in Composition.
      Subject = new Patient
      {
        Id = Guid.NewGuid().ToString(),
        Meta = new Meta()
        {
          Profile = new[] { BFDR.ProfileURL.PatientDecedentFetus }
        }
      };
      Composition.Meta = new Meta
      {
        Profile = new[] { ProfileURL.CompositionJurisdictionFetalDeathReport }
      };
      Composition.Type = new CodeableConcept(CodeSystems.LOINC, "92010-8", "Jurisdiction fetal death report Document", null);
      Composition.Title = "Fetal Death Report";
    }

    /// <summary>
    /// Initialize sections creates empty sections based on the composition type
    /// These are necessary so resources like Mother and Father can be referenced from somewhere in the composition
    /// </summary>
    protected override void InitializeSections()
    {
      CreateNewSection(MOTHER_PRENATAL_SECTION);
      CreateNewSection(MEDICAL_INFORMATION_SECTION);
      CreateNewSection(FETUS_SECTION);
      CreateNewSection(MOTHER_INFORMATION_SECTION);
      CreateNewSection(FATHER_INFORMATION_SECTION);
      CreateNewSection(EMERGING_ISSUES_SECTION);
    }
  }
}
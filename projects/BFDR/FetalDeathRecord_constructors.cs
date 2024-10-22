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
    private const string FETUS_SECTION = "76400-1";

    private const string CODEDCAUSEOFFETALDEATH_SECTION = "86804-2";

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

    /// <summary>Return the birth year for this record to be used in the identifier</summary>
    public override uint? GetYear()
    {
      // TODO: Uncomment and remove null return when DeliveryYear is implemented
      return (uint?)this.DeliveryYear;
    }

    /// <inheritdoc/>
    protected override void RestoreReferences()
    {
      // Do we need differentiate anything for [http://hl7.org/fhir/us/bfdr/StructureDefinition/Bundle-document-demographic-coded-content]? It's not technically a Fetal Death Report bundle... https://build.fhir.org/ig/HL7/fhir-bfdr/StructureDefinition-Bundle-document-demographic-coded-content.html
      // Restore the common references between Birth Records and Fetal Death Records.
      this.RestoreReferences(ProfileURL.BundleDocumentFetalDeathReport, new[] { ProfileURL.CompositionJurisdictionFetalDeathReport }, ProfileURL.PatientDecedentFetus);
      // Restore FetalDeath specific references.
      string maternityEncounterId = Composition?.Encounter.Reference;
      EncounterMaternity = Bundle.Entry.FindAll(entry => entry.Resource is Encounter).ConvertAll(entry => (Encounter)entry.Resource).Find(resource => resource.Meta.Profile.Any(p => p == ProfileURL.EncounterMaternity) && maternityEncounterId.Contains(resource.Id));
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
      Composition.Type = new CodeableConcept(CodeSystems.LOINC, "71230-7", "Fetal Death Report", null);
      Composition.Title = "Fetal Death Report";
    }
  }
}
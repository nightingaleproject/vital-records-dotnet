using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using VR;
using static Hl7.Fhir.Model.Encounter;

namespace BFDR
{
    /// <summary>Class <c>BirthRecord</c> is a class designed to help consume and produce birth records
    /// that follow the HL7 FHIR Vital Records Birth Reporting Implementation Guide, as described at:
    /// http://hl7.org/fhir/us/bfdr and https://github.com/hl7/bfdr.
    /// </summary>
    public partial class BirthRecord : NatalityRecord
    {

        /// <summary>Default constructor that creates a new, empty BirthRecord.</summary>
        public BirthRecord() : base()
        {

            // Start with an empty child. Need reference in Composition.

        }

        /// <summary>Constructor that takes a string that represents a FHIR Birth Record in either XML or JSON format.</summary>
        /// <param name="record">represents a FHIR Birth Record in either XML or JSON format.</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <exception cref="ArgumentException">Record is neither valid XML nor JSON.</exception>
        public BirthRecord(string record, bool permissive = false) : base(record, permissive) {}

        /// <summary>Constructor that takes a FHIR Bundle that represents a FHIR Birth Record.</summary>
        /// <param name="bundle">represents a FHIR Bundle.</param>
        /// <exception cref="ArgumentException">Record is invalid.</exception>
        public BirthRecord(Bundle bundle) : base(bundle) {}

        /// <summary>Return the birth year for this record to be used in the identifier</summary>
        protected override uint? GetYear()
        {
            return (uint?)this.BirthYear;
        }
        
        /// <inheritdoc/>
        protected override void RestoreReferences()
        {
            // Restore BirthRecord specific references.
            List<Patient> patients = Bundle.Entry.FindAll(entry => entry.Resource is Patient).ConvertAll(entry => (Patient) entry.Resource);
            Subject = patients.Find(patient => patient.Meta.Profile.Any(patientProfile => patientProfile == VR.ProfileURL.Child));
            // Restore the common references between Birth Records and Fetal Death Records.
            base.RestoreReferences();
        }

        /// <inheritdoc/>
        protected override void InitializeCompositionAndSubject()
        {
            // Initialize empty BirthRecord specific Subject and Composition.
            Subject = new Patient
            {
                Id = Guid.NewGuid().ToString(),
                Meta = new Meta() {
                    Profile = new[] { VR.ProfileURL.Child }
                }
            };
            Composition.Meta = new Meta
            {
                Profile = new[] { ProfileURL.CompositionJurisdictionLiveBirthReport }
            };
            Composition.Type = new CodeableConcept(CodeSystems.LOINC, "71230-7", "Birth certificate", null);
            Composition.Title = "Birth Certificate";
        }

        /// <summary>Date of Certification.</summary>
        /// <value>the date of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifiedDate = "2023-02-19";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Date of birth certification: {ExampleBirthRecord.CertificationDate}");</para>
        /// </example>
        [Property("CertificationDate", Property.Types.String, "Birth Certification", "Date of Certification.", true, BFDR.IGURL.CompositionProviderLiveBirthReport, true, 243)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")]
        public string CertificationDate
        {
            get
            {
                Encounter.ParticipantComponent certifier = EncounterBirth?.Participant?.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                if (certifier != null && certifier?.Period?.Start != null)
                {
                    return certifier.Period.Start;
                }
                return null;
            }
            set
            {
                Encounter.ParticipantComponent certifier = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                if (certifier != null)
                {
                    Period p = new Period();
                    p.StartElement = ConvertToDateTime(value);
                    certifier.Period = p;
                }
                else
                {
                    Encounter.ParticipantComponent newCertifier = new Encounter.ParticipantComponent();
                    CodeableConcept t = new CodeableConcept(CodeSystems.LOINC, "87287-9", "Birth certifier", null);
                    newCertifier.Type.Add(t);
                    Period p = new Period();
                    p.StartElement = ConvertToDateTime(value);
                    newCertifier.Period = p;
                    EncounterBirth.Participant.Add(newCertifier);
                }
            }
        }

        /// <summary>Certified Year</summary>
        /// <value>year of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifiedYear = 2023;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certified Year: {ExampleBirthRecord.CertifiedYear}");</para>
        /// </example>
        [Property("Certified Year", Property.Types.Int32, "Birth Certification", "Certified Year", true, IGURL.EncounterBirth, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")] 
        public int? CertifiedYear
        {
            get
            {
                if (EncounterBirth == null)
                {
                    return null;
                }
                Encounter.ParticipantComponent certifier = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                // First check the value string
                if (certifier == null || certifier.Period == null || certifier.Period.StartElement == null)
                {
                    return null;
                }
                if (certifier != null && certifier.Period.StartElement != null && ParseDateElements(certifier.Period.Start, out int? year, out int? month, out int? day))
                {
                    return year;
                }
                return GetDateFragmentOrPartialDate(certifier.Period.StartElement, VR.ExtensionURL.PartialDateTimeYearVR);
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (EncounterBirth == null)
                {
                    CreateBirthEncounter();
                }
                Encounter.ParticipantComponent stateComp = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                if (stateComp == null) // make certifier participant with date
                {  
                    Encounter.ParticipantComponent certifier = new Encounter.ParticipantComponent();
                    CodeableConcept t = new CodeableConcept(CodeSystems.LOINC, "87287-9", "Birth certifier", null);
                    certifier.Type.Add(t);
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    certifier.Period = p;
                    EncounterBirth.Participant.Add(certifier);
                    stateComp = certifier;
                }  
                if (stateComp.Period == null || stateComp.Period.StartElement == null) //certifier participant exists but no period or period.start
                {
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    stateComp.Period = p;
                }
                FhirDateTime newDate = SetYear(value, stateComp.Period.StartElement);
                if (newDate != null)
                {
                    stateComp.Period.StartElement = newDate;
                }
            }
        }

        /// <summary>Certified Month</summary>
        /// <value>month of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifiedMonth = 10;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certified Month: {ExampleBirthRecord.CertifiedMonth}");</para>
        /// </example>
        [Property("Certified Month", Property.Types.Int32, "Birth Certification", "Certified Month", true, IGURL.EncounterBirth, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")] 
        public int? CertifiedMonth
        {
            get
            {
                if (EncounterBirth == null)
                {
                    return null;
                }
                Encounter.ParticipantComponent certifier = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                // First check the value string
                if (certifier == null || certifier.Period == null || certifier.Period.StartElement == null)
                {
                    return null;
                }
                if (certifier != null && certifier.Period.StartElement != null && ParseDateElements(certifier.Period.Start, out int? year, out int? month, out int? day))
                {
                    return month;
                }
                return GetDateFragmentOrPartialDate(certifier.Period.StartElement, VR.ExtensionURL.PartialDateTimeMonthVR);
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (EncounterBirth == null)
                {
                    CreateBirthEncounter();
                }
                Encounter.ParticipantComponent stateComp = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                if (stateComp == null) // make certifier participant with date
                {  
                    Encounter.ParticipantComponent certifier = new Encounter.ParticipantComponent();
                    CodeableConcept t = new CodeableConcept(CodeSystems.LOINC, "87287-9", "Birth certifier", null);
                    certifier.Type.Add(t);
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    certifier.Period = p;
                    EncounterBirth.Participant.Add(certifier);
                    stateComp = certifier;
                }  
                if (stateComp.Period == null || stateComp.Period.StartElement == null) //certifier participant exists but no period or period.start
                {
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    stateComp.Period = p;
                }
                FhirDateTime newDate = SetMonth(value, stateComp.Period.StartElement); 
                if (newDate != null)
                {
                    stateComp.Period.StartElement = newDate; 
                }
            }
        }

        /// <summary>Certified Day</summary>
        /// <value>day of certification</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.CertifiedDay = 23;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Certified Day: {ExampleBirthRecord.CertifiedDay}");</para>
        /// </example> 
        [Property("Certified Day", Property.Types.Int32, "Birth Certification", "Certified Day", true, IGURL.EncounterBirth, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(extension.value.coding.code='CHILD')", "")] 
        public int? CertifiedDay
        {
            get
            {
                if (EncounterBirth == null)
                {
                    return null;
                }
                Encounter.ParticipantComponent certifier = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                // First check the value string
                if (certifier == null || certifier.Period == null || certifier.Period.StartElement == null)
                {
                    return null;
                }
                if (certifier != null && certifier.Period.StartElement != null && ParseDateElements(certifier.Period.Start, out int? year, out int? month, out int? day))
                {
                    return day;
                }
                return GetDateFragmentOrPartialDate(certifier.Period.StartElement, VR.ExtensionURL.PartialDateTimeDayVR);
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                if (EncounterBirth == null)
                {
                    CreateBirthEncounter();
                }
                Encounter.ParticipantComponent stateComp = EncounterBirth.Participant.FirstOrDefault(entry => ((Encounter.ParticipantComponent)entry).Type.Any(t => t.Coding.Any(c => c.Code == "87287-9")));
                if (stateComp == null) // make certifier participant with date
                {  
                    Encounter.ParticipantComponent certifier = new Encounter.ParticipantComponent();
                    CodeableConcept t = new CodeableConcept(CodeSystems.LOINC, "87287-9", "Birth certifier", null);
                    certifier.Type.Add(t);
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    certifier.Period = p;
                    EncounterBirth.Participant.Add(certifier);
                    stateComp = certifier;
                }  
                if (stateComp.Period == null || stateComp.Period.StartElement == null) //certifier participant exists but no period or period.start
                {
                    Period p = new Period();
                    p.StartElement = new FhirDateTime();
                    p.StartElement.Extension.Add(NewBlankPartialDateTimeExtension(false));
                    stateComp.Period = p;
                }
                FhirDateTime newDate = SetDay(value, stateComp.Period.StartElement); 
                if (newDate != null)
                {
                    stateComp.Period.StartElement = newDate; 
                }
            }
        }

        /// <summary>Child's Place Of Birth Type.</summary>
        /// <value>Place Where Birth Occurred, type of place or institution. A Dictionary representing a codeable concept of the physical location type:
        /// <para>"code" - The code used to describe this concept.</para>
        /// <para>"system" - The relevant code system.</para>
        /// <para>"display" - The human readable version of this code.</para>
        /// </value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; locationType = new Dictionary&lt;string, string&gt;();</para>
        /// <para>locationType.Add("code", "22232009");</para>
        /// <para>locationType.Add("system", "http://snomed.info/sct");</para>
        /// <para>locationType.Add("display", "Hospital");</para>
        /// <para>ExampleBirthRecord.BirthPhysicalLocation = locationType;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"The place type the child was born: {ExampleBirthRecord.BirthPhysicalLocation["code"]}");</para>
        /// </example>
        [Property("BirthPhysicalLocation", Property.Types.Dictionary, "Birth Physical Location", "Birth Physical Location.", true, IGURL.EncounterBirth, true, 16)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter)", "")]
        public Dictionary<string, string> BirthPhysicalLocation
        {
            get => GetPhysicalLocation(EncounterBirth);
            set => SetPhysicalLocation(EncounterBirth ?? CreateEncounter(ProfileURL.EncounterBirth), value);
        }

        /// <summary>Child's Place Of Birth Type Helper</summary>
        /// <value>Child's Place Of Birth Type Helper</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthPhysicalLocationHelper = "Hospital";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child's Place Of Birth Type: {ExampleBirthRecord.BirthPhysicalLocationHelper}");</para>
        /// </example>
        [Property("BirthPhysicalLocationHelper", Property.Types.String, "Birth Physical Location", "Birth Physical Location Helper.", false, IGURL.EncounterBirth, true, 4)]
        [FHIRPath("Bundle.entry.resource.where($this is Encounter).where(meta.profile == " + IGURL.EncounterBirth + ")", "")]
        public string BirthPhysicalLocationHelper
        {
            get => GetPhysicalLocationHelper(EncounterBirth);
            set => SetPhysicalLocationHelper(EncounterBirth ?? CreateEncounter(ProfileURL.EncounterBirth), value, BFDR.Mappings.BirthDeliveryOccurred.FHIRToIJE, BFDR.ValueSets.BirthDeliveryOccurred.Codes);
        }

        /// <summary>Mother's Weight at Delivery.</summary>
        /// <value>the mother's weight at delivery in whole pounds, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.MotherWeightAtDelivery = 120;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Mother's Weight at Delivery: {ExampleBirthRecord.MotherWeightAtDelivery}");</para>
        /// </example>
        [Property("MotherWeightAtDelivery", Property.Types.Int32, "Mother Prenatal", "Weight at Delivery.", false, BFDR.IGURL.ObservationMotherDeliveryWeight, true, 139)]
        [FHIRPath("Bundle.entry.resource.where($this is Observation).where(code.coding.code='69461-2')", "")]
        public int? MotherWeightAtDelivery
        {
            // TODO replace codes with constants once BFDR value sets are autogenerated
            get => GetWeight("69461-2");
            set => SetWeight("69461-2", value, "lb_av", MOTHER_PRENATAL_SECTION, Mother.Id);
        }

        /// <summary>Child's Year of Birth.</summary>
        /// <value>the child's year of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthYear = 1928;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Year of Birth: {ExampleBirthRecord.BirthYear}");</para>
        /// </example>
        [Property("BirthYear", Property.Types.Int32, "Child Demographics", "Child's Year of Birth.", false, VR.IGURL.Child, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate", "")]
        public int? BirthYear
        {
            get => GetBirthYear();
            set => SetBirthYear(value);
        }

        /// <summary>Child's Month of Birth.</summary>
        /// <value>the child's month of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthMonth = 11;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Month of Birth: {ExampleBirthRecord.BirthMonth}");</para>
        /// </example>
        [Property("BirthMonth", Property.Types.Int32, "Child Demographics", "Child's Month of Birth.", false, VR.IGURL.Child, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate", "")]
        public int? BirthMonth
        {
            get => GetBirthMonth();
            set => SetBirthMonth(value);
        }

        /// <summary>Child's Day of Birth.</summary>
        /// <value>the child's day of birth, or -1 if explicitly unknown, or null if never specified</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthDay = 11;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Day of Birth: {ExampleBirthRecord.BirthDay}");</para>
        /// </example>
        [Property("BirthDay", Property.Types.Int32, "Child Demographics", "Child's Day of Birth.", false, VR.IGURL.Child, true, 14)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.birthDate", "")]
        public int? BirthDay
        {
            get => GetBirthDay();
            set => SetBirthDay(value);
        }

        /// <summary>Child's Time of Birth.</summary>
        /// <value>the child's time of birth.</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthTime = 11;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Child Time of Birth: {ExampleBirthRecord.BirthTime}");</para>
        /// </example>
        [Property("BirthTime", Property.Types.String, "Child Demographics", "Child's Time of Birth.", true, VR.IGURL.Child, true, 14)]
        // How should FHIRPath work when the time could be in 1 of 2 different places (value in PatientBirthTime | PartialDateTime extension)
        [FHIRPath("Bundle.entry.resource.where($this is Patient).birthDate.extension.where(url='" + VR.ExtensionURL.PatientBirthTime + "')", "")]
        public string BirthTime
        {
            get => GetBirthTime();
            set => SetBirthTime(value);
        }

        /// <summary>Child's BirthSex at Birth.</summary>
        /// <value>The child's BirthSex at time of birth</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthSex = "female;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Sex at Time of Birth: {ExampleBirthRecord.BirthSex}");</para>
        /// </example>
        [Property("Sex At Birth", Property.Types.String, "Child Demographics", "Child's Sex at Birth.", true, VR.IGURL.Child, true, 12)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url='" + OtherExtensionURL.BirthSex + "')", "")]
        public string BirthSex
        {
            get => GetBirthSex();
            set => SetBirthSex(value);
        }

        /// <summary>Child's Sex at Birth Helper.</summary>
        /// <value>The child's sex at time of birth</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.BirthSexHelper = "female;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Sex at Time of Birth: {ExampleBirthRecord.BirthSexHelper}");</para>
        /// </example>
        [Property("Sex At Birth Helper", Property.Types.String, "Child Demographics", "Child's Sex at Birth.", false, VR.IGURL.Child, true, 12)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).extension.where(url='" + OtherExtensionURL.BirthSex + "')", "")]
        public string BirthSexHelper
        {
            get => GetBirthSex();
            set => SetBirthSex(value);
        }

        /// <summary>Multiple birth set order</summary>
        /// <value>The order that the child was born if a multiple birth or null if it was a single birth</value>
        /// <example>
        /// <para>ExampleBirthRecord.SetOrder = null; // single birth</para>
        /// <para>ExampleBirthRecord.SetOrder = -1; // unknow whether single or multiple birth</para>
        /// <para>ExampleBirthRecord.SetOrder = 1; // multiple birth, born first</para>
        /// </example>
        [Property("SetOrder", Property.Types.Int32, "Child Demographics", "Child Demographics, Set Order", true, VR.IGURL.Child, true, 208)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient)", "multipleBirth")]
        public int? SetOrder
        {
            get => GetSetOrder();
            set => SetSetOrder(value);
        }

        /// <summary>Multiple birth set order edit flag</summary>
        /// <value>the multiple birth set order edit flag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>Dictionary&lt;string, string&gt; route = new Dictionary&lt;string, string&gt;();</para>
        /// <para>route.Add("code", "queriedCorrect");</para>
        /// <para>route.Add("system", "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags");</para>
        /// <para>route.Add("display", "Queried, and Correct");</para>
        /// <para>ExampleBirthRecord.PluralityEditFlag = route;</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Multiple birth set order edit flag: {ExampleBirthRecord.PluralityEditFlag}");</para>
        /// </example>
        [Property("PluralityEditFlag", Property.Types.Dictionary, "Child Demographics", "Child Demographics, Plurality Edit Flag", true, VR.IGURL.Child, true, 211)]
        [PropertyParam("code", "The code used to describe this concept.")]
        [PropertyParam("system", "The relevant code system.")]
        [PropertyParam("display", "The human readable version of this code.")]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).multipleBirth.extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag')", "")]
        public Dictionary<string, string> PluralityEditFlag
        {
            get => GetPluralityEditFlag();
            set => SetPluralityEditFlag(value);
        }

        /// <summary>Multiple birth set order edit flag helper</summary>
        /// <value>the multiple birth set order edit flag</value>
        /// <example>
        /// <para>// Setter:</para>
        /// <para>ExampleBirthRecord.PluralityEditFlagHelper = "queriedCorrect";</para>
        /// <para>// Getter:</para>
        /// <para>Console.WriteLine($"Multiple birth set order edit flag: {ExampleBirthRecord.PluralityEditFlagHelper}");</para>
        /// </example>
        [Property("PluralityEditFlagHelper", Property.Types.String, "Child Demographics", "Child Demographics, Plurality Edit Flag", false, VR.IGURL.Child, true, 211)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).multipleBirth.extension.where(url = 'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag')", "")]
        public string PluralityEditFlagHelper
        {
            get => GetPluralityEditFlagHelper();
            set => SetPluralityEditFlagHelper(value);
        }

        /// <summary>Multiple birth plurality</summary>
        /// <value>Where a patient is a part of a multiple birth, this is the total number of births that occurred in this pregnancy.</value>
        /// <example>
        /// <para>ExampleBirthRecord.Plurality = null; // single birth</para>
        /// <para>ExampleBirthRecord.Plurality = -1; // unknown number of births birth</para>
        /// <para>ExampleBirthRecord.Plurality = 2; // two births for this pregnancy</para>
        /// </example>
        [Property("Plurality", Property.Types.Int32, "Child Demographics", "Child Demographics, Plurality", true, VR.IGURL.Child, true, 207)]
        [FHIRPath("Bundle.entry.resource.where($this is Patient).multipleBirth.extension.where(url = 'http://hl7.org/fhir/StructureDefinition/patient-multipleBirthTotal')", "")]
        public int? Plurality
        {
            get => GetPlurality();
            set => SetPlurality(value);
        }
    }
}

using System.Linq;
using System.Collections.Generic;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;

namespace VR
{
    /// <summary>Class <c>VitalRecord</c> is a base class for FHIR Vital Records 
    /// including Death Reporting (VRDR) and Birth and Fetal Death Reporting (BFDR)
    /// and is designed to help produce and consume records.</summary>
    public abstract partial class VitalRecord
    {
        /// <summary>Default constructor that creates a new, empty Record.</summary>
        protected VitalRecord()
        {
            Composition = new Composition();
        }

        /// <summary>Constructor that takes a string that represents a FHIR Vital Record in either XML or JSON format.</summary>
        /// <param name="record">represents a FHIR Vital Record in either XML or JSON format.</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <exception cref="ArgumentException">Record is neither valid XML nor JSON.</exception>
        protected VitalRecord(string record, bool permissive = false)
        {
            ParserSettings parserSettings = new ParserSettings
            {
                AcceptUnknownMembers = permissive,
                AllowUnrecognizedEnums = permissive,
                PermissiveParsing = permissive
            };
            // XML?
            Boolean maybeXML = record.TrimStart().StartsWith("<");
            Boolean maybeJSON = record.TrimStart().StartsWith("{");
            if (!String.IsNullOrEmpty(record) && (maybeXML || maybeJSON))
            {
                // Grab all errors found by visiting all nodes and report if not permissive
                if (!permissive)
                {
                    List<string> entries = new List<string>();
                    ISourceNode node = null;
                    if (maybeXML)
                    {
                        node = FhirXmlNode.Parse(record, new FhirXmlParsingSettings { PermissiveParsing = permissive });
                    }
                    else
                    {
                        node = FhirJsonNode.Parse(record, "Bundle", new FhirJsonParsingSettings { PermissiveParsing = permissive });
                    }
                    foreach (Hl7.Fhir.Utility.ExceptionNotification problem in node.VisitAndCatch())
                    {
                        entries.Add(problem.Message);
                    }
                    if (entries.Count > 0)
                    {
                        throw new System.ArgumentException(String.Join("; ", entries).TrimEnd());
                    }
                }
                // Try Parse
                try
                {
                    if (maybeXML)
                    {
                        FhirXmlParser parser = new FhirXmlParser(parserSettings);
                        Bundle = parser.Parse<Bundle>(record);
                    }
                    else
                    {
                        try
                        {
                            System.Text.Json.JsonDocument.Parse(record);
                        }
                        catch (System.Text.Json.JsonException e)
                        {
                            throw new FormatException(e.Message);
                        }
                        FhirJsonParser parser = new FhirJsonParser(parserSettings);
                        Bundle = parser.Parse<Bundle>(record);
                    }

                    ValidatePartialDates(Bundle);
                    
                    Navigator = Bundle.ToTypedElement();
                }
                catch (Exception e)
                {
                    throw new System.ArgumentException(e.Message);
                }
            }
            // Fill out class instance references
            if (Navigator != null)
            {
                RestoreReferences();
            }
            else
            {
                throw new System.ArgumentException("The given input does not appear to be a valid XML or JSON FHIR record.");
            }
            // Validate that the given Bundle is not a message.
            if (this.Bundle.Type == Bundle.BundleType.Message)
            {
                throw new BundleTypeException("The FHIR Bundle must be of type Document, not Message.");
            }
        }
        
        /// <summary>Restores class references from a newly parsed record.</summary>
        protected abstract void RestoreReferences();

        /// <summary>Returns the focus id of a section in the composition.</summary>
        /// <returns>the string uuid of the section focus</returns>
        protected abstract string GetSectionFocusId(string section);

        /// <summary>Helper method to return a XML string representation of this Vital Record.</summary>
        /// <returns>a string representation of this Vital Record in XML format</returns>
        public string ToXML()
        {
            return Bundle.ToXml();
        }

        /// <summary>Helper method to return a XML string representation of this Vital Record.</summary>
        /// <returns>a string representation of this Vital Record in XML format</returns>
        public string ToXml()
        {
            return Bundle.ToXml();
        }

        /// <summary>Helper method to return a JSON string representation of this Vital Record.</summary>
        /// <returns>a string representation of this Vital Record in JSON format</returns>
        public string ToJSON()
        {
            return Bundle.ToJson();
        }

        /// <summary>Helper method to return a JSON string representation of this Vital Record.</summary>
        /// <returns>a string representation of this Vital Record in JSON format</returns>
        public string ToJson()
        {
            return Bundle.ToJson();
        }

        /// <summary>Helper method to return an ITypedElement of the record bundle.</summary>
        /// <returns>an ITypedElement of the record bundle</returns>
        public ITypedElement GetITypedElement()
        {
            return Navigator;
        }

        /// <summary>Helper method to return a the bundle.</summary>
        /// <returns>a FHIR Bundle</returns>
        public Bundle GetBundle()
        {
            return Bundle;
        }

        /// <summary>Adds a resource to the bundle if the resource os not null.</summary>
        /// <param name="resource">the resource to add, may be null</param>
        /// <param name="bundle">the bundle to add to, may not be null</param>
        protected void AddResourceToBundleIfPresent(Resource resource, Bundle bundle)
        {
            if (resource != null)
            {
                bundle.AddResourceEntry(resource, "urn:uuid:" + resource.Id);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Class helper methods useful for building, searching through records.
        //
        /////////////////////////////////////////////////////////////////////////////////


        /// <summary>Return a reference to the Composition object for unit testing.</summary>
        public Composition GetComposition()
        {
            return Composition;
        }

        /// <summary>Remove a reference from the Vital Record Composition.</summary>
        /// <param name="reference">a reference.</param>
        /// <param name="code">a code for the section to modify.</param>
        protected bool RemoveReferenceFromComposition(string reference, string code)
        {
            Composition.SectionComponent section = Composition.Section.Where(s => s.Code.Coding.First().Code == code).First();
            return section.Entry.RemoveAll(entry => entry.Reference == reference) > 0;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //
        // Class helper methods useful for building, searching through records.
        //
        /////////////////////////////////////////////////////////////////////////////////

        /// <summary>Add a reference to the Vital Record Composition.</summary>
        /// <param name="reference">a reference.</param>
        /// <param name="code">the code for the section to add to.</param>
        /// <param name="focusId">the identifier of the resource that is the focus of the section, ignored if null</param>
        /// The sections are defined by the child class
        protected void AddReferenceToComposition(string reference, string code, string focusId = null)
        {
            // In many of the createXXXXXX methods this gets called as a last step to add a reference to the new instance to the composition.
            // The Composition is present only in the DeathCertificateDocument, and is absent in all of the other bundles.
            // In lieu of putting conditional logic in all of the calling methods, added it here.
            if (Composition == null)
            {
                return;
            }

            //Composition.Section.First().Entry.Add(new ResourceReference("urn:uuid:" + reference));
            Composition.SectionComponent section = new Composition.SectionComponent();
            if (CompositionSections.Any(code.Contains))
            {
                // Find the right section
                foreach (var s in Composition.Section)
                {
                    if (s.Code != null && s.Code.Coding.Count > 0 && s.Code.Coding.First().Code == code)
                    {
                        section = s;
                    }
                }
                if (section.Code == null)
                {
                    Dictionary<string, string> coding = new Dictionary<string, string>();
                    // Default to VR.CodeSystems.DocumentSections but overriden by BFDR since it uses a different code system for sections
                    coding["system"] = CompositionSectionCodeSystem;
                    coding["code"] = code;
                    section.Code = DictToCodeableConcept(coding);
                    Composition.Section.Add(section);
                }
                section.Entry.Add(new ResourceReference("urn:uuid:" + reference));
                // all sections start with an "Empty Reason" by default, since we added an entry, clear the empty reason
                section.EmptyReason = null;
            }
        }

        /// <summary>Takes a string that represents a FHIR Death Record in either XML or JSON format and creates a VitalRecord of the specified type from it.</summary>
        /// <param name="fhirString">represents a FHIR Vital Record in either XML or JSON format.</param>
        /// <param name="permissive">if the parser should be permissive when parsing the given string</param>
        /// <exception cref="ArgumentException">Record is neither valid XML nor JSON.</exception>
        public static RecordType CreateRecordFromFHIR<RecordType>(string fhirString, bool permissive = false) where RecordType : VitalRecord{
            return Activator.CreateInstance(typeof(RecordType), new object[] { fhirString, permissive }) as RecordType;
        }

    }
}

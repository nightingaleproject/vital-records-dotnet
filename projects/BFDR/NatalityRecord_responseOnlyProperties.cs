using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VR;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;

namespace BFDR
{
    public partial class NatalityRecord
    {
        private Dictionary<string, string> GetCodedOccupation(string role)
        {
            Observation observation = GetOrCreateOccupationObservation(role);
            if (observation != null)
            {
                return CodeableConceptToDict((CodeableConcept)observation.Value);
            }
            return EmptyCodeableDict();
        }

        private Dictionary<string, string> GetCodedIndustry(string role)
        {
            Observation observation = GetOrCreateOccupationObservation(role);
            if (observation != null)
            {
                var component = observation.Component.Where(c => CodeableConceptToDict(c.Code)["code"] == "86188-0").FirstOrDefault();
                if (component != null)
                {
                    return CodeableConceptToDict((CodeableConcept)component.Value);
                }
            }
            return EmptyCodeableDict();
        }

        private void SetCodedOccupation(string role, Dictionary<string, string> value)
        {
            Observation observation = GetOrCreateOccupationObservation(role);
            if (observation != null)
            {
                // Preserve any existing Text in the value
                string existingText = (observation.Value as CodeableConcept).Text;
                observation.Value = DictToCodeableConcept(value);
                (observation.Value as CodeableConcept).Text = existingText;
            }
        }

        private void SetCodedIndustry(string role, Dictionary<string, string> value)
        {
            Observation observation = GetOrCreateOccupationObservation(role);
            if (observation != null)
            {
                var component = observation.Component.Where(c => CodeableConceptToDict(c.Code)["code"] == "86188-0").FirstOrDefault();
                if (component == null)
                {
                    component = new Observation.ComponentComponent
                    {
                        Code = new CodeableConcept(CodeSystems.LOINC, "86188-0")
                    };
                    observation.Component.Add(component);
                }
                // Preserve any existing Text in the value
                string existingText = (component.Value as CodeableConcept).Text;
                component.Value = DictToCodeableConcept(value);
                (component.Value as CodeableConcept).Text = existingText;
            }
        }

        // Coded getter and setter for 4 things that use the above, helpers for all for can just use GetObservationValueHelper

    }
}

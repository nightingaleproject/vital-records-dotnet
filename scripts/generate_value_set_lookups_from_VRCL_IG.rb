=begin
This ruby script reads an enumerated set of Valueset files (keys in the 'valuesets' hash) and generates a
single CSharp file in the specified location that includes a class describing each valueset (named according
to the values in the 'valuesets' hash).

Currently, the script assumes a FSH/SUSHI directory structure, with the valuesets being in the
fsh-generated/resource subdirectory. This should probably be changed to assume the content is in the flat
structure that is part of an IG download. This change will be made when the VRDR IG has been rebuilt to
include all of the necessary valuesets.

All references to codesystems use the definitions in VRDR.CodeSystems.

Inputs:   <input directory of an IG download> <output directory for file ValueSets.cs>
Output:   ValueSets.cs in the specified directory

Directions:
  1) Install sushi (https://github.com/FHIR/sushi)
  1) Download and build the IG:

     git clone https://github.com/HL7/vrdr.git
     cd vrdr
     sushi

  3) use the resulting fsh-generated directory as the first argument for this script
  4) decide where you want the output generated, and use that as the second argument for this script

Example run: ruby tools/generate_value_set_lookups_from_VRDR_IG.rb ../vrdr/fsh-generated/ VRDR/

Example of generated output:

namespace VR
{
    public static class ValueSets
    {
        public static class DeathLocationType {
            public static string[,] Codes = {
                { "63238001", "Hospital Dead on Arrival", VRDR.CodeSystems.SCT },
                { "440081000124100", "Decedent's Home", VRDR.CodeSystems.SCT },
                { "440071000124103", "Hospice", VRDR.CodeSystems.SCT },
                { "16983000", "Hospital Inpatient", VRDR.CodeSystems.SCT },
                { "450391000124102", "Death in emergency Room/Outpatient", VRDR.CodeSystems.SCT },
                { "450381000124100", "Death in nursing home/Long term care facility", VRDR.CodeSystems.SCT },
                { "OTH", "Other (Specify)", CodeSystems.PH_NullFlavor_HL7_V3 },
                { "UNK", "Unknown", CodeSystems.PH_NullFlavor_HL7_V3 }
            };
            public static string Hospital_Dead_on_Arrival = "63238001";
            public static string Decedents_Home = "440081000124100";
            ...
        }
    }
}
=end
require 'json'
require 'pry'
codesystems = {
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-canadian-provinces-vr" => "VR.CodeSystems.CanadianProvinces",
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-country-code-vr" => "VR.CodeSystems.CountryCodes",
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-jurisdictions-vr" => "VR.CodeSystems.Jurisdictions",
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags" => "VR.CodeSystems.BirthAndDeathEditFlags",
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-hispanic-origin-vr" => "VR.CodeSystems.HispanicOrigin",
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-local-observation-codes-vr" => "VR.CodeSystems.LocalObservationCodes",
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-missing-value-reason-vr" => "VR.CodeSystems.MissingValueReason",
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-race-code-vr" => "VR.CodeSystems.RaceCode",
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-race-recode-40-vr" => "VR.CodeSystems.RaceRecode40",
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/codesystem-vr-component" => "VR.CodeSystems.LocalComponentCodes",
    "http://hl7.org/fhir/us/vr-common-library/CodeSystem/codesystem-ije-vr" => "VR.CodeSystems.PlaceholderCodeSystemsForIJE",
}

valuesets = {
    "ValueSet-ValueSet-coded-race-and-ethnicity-person-vr.json" => "CodedRaceAndEthnicityPerson",
    "ValueSet-ValueSet-hispanic-no-unknown-vr.json" => "HispanicNoUnknown",
    "ValueSet-ValueSet-hispanic-origin-vr.json" => "HispanicOrigin",
    "ValueSet-ValueSet-input-race-and-ethnicity-person-vr.json" => "InputRaceAndEthnicityPerson",
    "ValueSet-ValueSet-race-code-vr.json" => "RaceCode",
    "ValueSet-ValueSet-birthplace-country-vr.json" => "BirthplaceCounty",
    "ValueSet-ValueSet-jurisdiction-vr.json" => "Jurisdictions",
    "ValueSet-ValueSet-residence-country-vr.json" => "ResidenceCountry",
    "ValueSet-ValueSet-states-territories-provinces-vr.json" => "StatesTerritoriesAndProvinces",
    "ValueSet-ValueSet-usstates-territories-vr.json" => "USStatesTerritories",
    "ValueSet-ValueSet-birth-attendant-titles-vr.json" => "BirthAttendantsTitles",
    "ValueSet-ValueSet-birth-sex-child-vr.json" => "BirthSexChild",
    "ValueSet-ValueSet-birth-sex-fetus-vr.json" => "BirthSexFetus",
    "ValueSet-ValueSet-education-level-person-vr.json" => "EducationLevelPerson",
    "ValueSet-ValueSet-education-level-vr.json" => "EducationLevel",
    "ValueSet-ValueSet-father-relationship-vr.json" => "FatherRelationship",
    "ValueSet-ValueSet-mother-relationship-vr.json" => "MotherRelationship",
    "ValueSet-ValueSet-role-vr.json" => "Role",
    "ValueSet-ValueSet-units-of-age-vr.json" => "UnitsOfAge",
    "ValueSet-ValueSet-yes-no-not-applicable-vr.json" => "YesNoNotApplicable",
    "ValueSet-ValueSet-yes-no-unknown-not-applicable-vr.json" => "YesNoUnknownNotApplicable",
    "ValueSet-ValueSet-yes-no-unknown-vr.json" => "YesNoUnknown",
    "ValueSet-valueset-edit-bypass-01234-vr.json" => "EditBypass01234",
    "ValueSet-ValueSet-mothers-date-of-birth-edit-flags-vr.json" => "MothersDateofBirthEditFlagsNCHS",
    "ValueSet-ValueSet-plurality-edit-flags-vr.json" => "PluralityEditFlags",
    "ValueSet-ValueSet-race-missing-value-reason-vr.json" => "RaceMissingValueReason",
    "ValueSet-ValueSet-race-recode-40-vr.json" => "RaceRecode40",
}
# These are special cases that we want to shorten the string produced by the general approach, for practical reasons
# special_cases =
#     {
#     "Medical_Examiner_Coroner_On_The_Basis_Of_Examination_And_Or_Investigation_In_My_Opinion_Death_Occurred_At_The_Time_Date_And_Place_And_Due_To_The_Cause_S_And_Manner_Stated" => "Medical_Examiner_Coroner",
#     "Pronouncing_Certifying_Physician_To_The_Best_Of_My_Knowledge_Death_Occurred_At_The_Time_Date_And_Place_And_Due_To_The_Cause_S_And_Manner_Stated" => "Pronouncing_Certifying_Physician",
#     "Certifying_Physician_To_The_Best_Of_My_Knowledge_Death_Occurred_Due_To_The_Cause_S_And_Manner_Stated" => "Certifying_Physician"
# }

outfilename = ARGV[1] + "/ValueSets.cs"
puts "Output in #{outfilename}"
file = file=File.open(outfilename,"w")
systems_without_constants = []

file.puts "// DO NOT EDIT MANUALLY! This file was generated by the script \"#{__FILE__}\"

namespace VR
{
    /// <summary> ValueSet Helpers </summary>
    public static class ValueSets"
file.puts "    {"
valuesets.each do |vsfile, fieldname|
        puts "Generating output for #{vsfile}"
        filename = ARGV[0] + "/resources/" + vsfile
        value_set_data = JSON.parse(File.read(filename))
        file.puts "        /// <summary> #{fieldname} </summary>
        public static class #{fieldname} {
            /// <summary> Codes </summary>
            public static string[,] Codes = {"
        groups = value_set_data["compose"]["include"]
        first = true
        groups.each { | group |
            system = group["system"]
            if codesystems[system]
                system = codesystems[system]
            else
              systems_without_constants << system
            end
            # TODO remove nil check once VS are complete
            if group["concept"].nil? != true
                for concept in group["concept"]
                    file.puts "," if first == false
                    first = false
                    # TODO update print system to differentiate between literal strings and references to VR
                    file.print "                { \"#{concept["code"]}\", \"#{concept["display"]}\", \"#{system}\" }"
                end
            else
                print ("SKIP! No concept codes defined: #{group}\n")
            end
        }
        file.puts "\n            };"
        groups.each { | group |
            system = group["system"]
            if codesystems[system]
                system = codesystems[system]
            end
            if group["concept"].nil? != true
                for concept in group["concept"]
                    # TODO: Remove check if display is present once IG is refined
                    if concept["display"].nil? != true
                        display = concept["display"].gsub("'", '').split(/[^a-z0-9]+/i).map(&:capitalize).join('_')
                        if display[0][/\d/] then display = "_" + display end
                        # TODO add back special cases
                        #if special_cases[display] != nil then display = special_cases[display] end
                        file.puts "            /// <summary> #{display} </summary>"
                        file.puts "            public static string  #{display} = \"#{concept["code"]}\";"
                    else
                        file.puts "            /// <summary> #{fieldname} </summary>"
                        file.puts "            public static string  #{fieldname}#{concept["code"]} = \"#{concept["code"]}\";"
                    end

                end
            else
                print ("SKIP! No concept codes defined: #{group}\n")
            end
        }
        file.puts "        };"

    end
file.puts "   }
}"

puts
puts "Saw the following code systems that don't have constants:"
puts
puts systems_without_constants.uniq
puts
puts "Suggestions:"
puts
systems_without_constants.uniq.each do |system|
  puts "        /// <summary> #{system} </summary>"
  puts "        public static string XYZ = \"#{system}\";"
end

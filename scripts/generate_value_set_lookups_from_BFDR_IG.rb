=begin
This ruby script reads an enumerated set of Valueset files (keys in the 'valuesets' hash) and generates a
single CSharp file in the specified location that includes a class describing each valueset (named according
to the values in the 'valuesets' hash).

Currently, the script assumes a FSH/SUSHI directory structure, with the valuesets being in the
fsh-generated/resource subdirectory. This should probably be changed to assume the content is in the flat
structure that is part of an IG download. This change will be made when the VRDR IG has been rebuilt to
include all of the necessary valuesets.

All references to codesystems use the definitions in BFDR.CodeSystems.

Inputs:   <input directory of an IG download> <output directory for file ValueSets.cs>
Output:   ValueSets.cs in the specified directory

Directions:
  1) Install sushi (https://github.com/FHIR/sushi)
  1) Download and build the IG:

     git clone https://github.com/HL7/fhir-bfdr
     cd fhir-bfdr
     sushi

  3) use the resulting fsh-generated directory as the first argument for this script
  4) decide where you want the output generated, and use that as the second argument for this script

Example run: ruby scripts/generate_value_set_lookups_from_BFDR_IG.rb ~/Documents/code/fhir-bfdr/fsh-generated/ ./projects/BFDR

Example of generated output:

namespace BFDR
{
    public static class ValueSets
    {
        public static class PlaceTypeOfBirth {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
                { "22232009", "Hospital", VR.CodeSystems.SCT },
                { "91154008", "Free-standing birthing center", VR.CodeSystems.SCT },
                { "408839006", "Planned home birth", VR.CodeSystems.SCT },
                { "408838003", "Unplanned home birth", VR.CodeSystems.SCT },
                { "67190003", "Free-standing clinic", VR.CodeSystems.SCT },
                { "unknownplannedhomebirth", "Unknown if Planned Home Birth", "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-vr-birth-delivery-occurred" },
                { "OTH", "Other", VR.CodeSystems.NullFlavor_HL7_V3 },
                { "UNK", "Unknown", VR.CodeSystems.NullFlavor_HL7_V3 }
            };
            /// <summary> Hospital </summary>
            public static string  Hospital = "22232009";
            /// <summary> Free_Standing_Birthing_Center </summary>
            ...
        }
    }
}
=end
require 'json'
require 'pry'
codesystems = {
    "http://snomed.info/sct" => "VR.CodeSystems.SCT",
    "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-vr-birth-and-fetal-death-financial-class" => "VR.CodeSystems.PayorType",
    "http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-vr-birth-delivery-occurred" => "\"http://hl7.org/fhir/us/bfdr/CodeSystem/CodeSystem-vr-birth-delivery-occurred\"",
    "http://terminology.hl7.org/CodeSystem/v3-NullFlavor" => "VR.CodeSystems.NullFlavor_HL7_V3"
}

valuesets = {
    "ValueSet-ValueSet-birth-delivery-occurred.json" => "PlaceTypeOfBirth",
    "ValueSet-ValueSet-birth-and-fetal-death-financial-class.json" => "PayorType"
}
# These are special cases that we want to shorten the string produced by the general approach, for practical reasons
special_cases =
  {
    # "Medical_Examiner_Coroner_On_The_Basis_Of_Examination_And_Or_Investigation_In_My_Opinion_Death_Occurred_At_The_Time_Date_And_Place_And_Due_To_The_Cause_S_And_Manner_Stated" => "Medical_Examiner_Coroner",
    # "Pronouncing_Certifying_Physician_To_The_Best_Of_My_Knowledge_Death_Occurred_At_The_Time_Date_And_Place_And_Due_To_The_Cause_S_And_Manner_Stated" => "Pronouncing_Certifying_Physician",
    # "Certifying_Physician_To_The_Best_Of_My_Knowledge_Death_Occurred_Due_To_The_Cause_S_And_Manner_Stated" => "Certifying_Physician"
  }

outfilename = ARGV[1] + "/ValueSets.cs"
puts "Output in #{outfilename}"
file = file=File.open(outfilename,"w")
systems_without_constants = []

file.puts "// DO NOT EDIT MANUALLY! This file was generated by the script \"#{__FILE__}\"

namespace BFDR
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
            for concept in group["concept"]
                file.puts "," if first == false
                first = false
                file.print "                { \"#{concept["code"]}\", \"#{concept["display"]}\", #{system} }"
            end
        }
        file.puts "\n            };"
        groups.each { | group |
            system = group["system"]
            if codesystems[system]
                system = codesystems[system]
            end
            for concept in group["concept"]
                display = concept["display"].gsub("'", '').split(/[^a-z0-9]+/i).map(&:capitalize).join('_')
                if display[0][/\d/] then display = "_" + display end
                if special_cases[display] != nil then display = special_cases[display] end
                file.puts "            /// <summary> #{display} </summary>"
                file.puts "            public static string  #{display} = \"#{concept["code"]}\";"
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

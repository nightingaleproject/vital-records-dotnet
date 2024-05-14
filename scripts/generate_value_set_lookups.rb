# This ruby script takes the concept map JSON files that are generated as part of the VRDR, BFDR, and VR Common IGs
# and creates output files that includes a class describing each valueset
#
# Currently, the script assumes a FSH/SUSHI directory structure, with the valuesets being in the
# fsh-generated/resource subdirectory. This should probably be changed to assume the content is in the flat
# structure that is part of an IG download. This change will be made when the VRDR IG has been rebuilt to
# include all of the necessary valuesets.
#
# All references to codesystems use the definitions in VRDR.CodeSystems.
#
# Inputs:   <input directory of an IG download> <CodeSystems.cs>
# Output:   <c# source code defining value set classes>
#
# Usage: ruby tools/generate_value_set_lookups.rb <path-to-value-sets> <path-to-code-systems> > <path-to-source-file>
#
# If you need to generate the value set JSON files, first install sushi (https://github.com/FHIR/sushi).
# Then, to generate the value set lookup file for VRDR, first checkout the VRDR IG
#
#     git clone https://github.com/HL7/vrdr.git
#     cd vrdr
#     sushi
#
# The value set JSON files will be built in vrdr/fsh-generated/resources/. To generate the value set lookup
# file, assuming the current directory is this scripts directory and the IG was checked out at the same
# directory level as this repository, run
#
#     ruby generate_value_set_lookups.rb ../../vrdr/fsh-generated ../projects/VitalRecord/CodeSystems.cs >! ../projects/VRDR/ValueSets.cs
#
# To generate the value set lookup file for BFDR, first checkout the BFDR IG
#
#     git clone https://github.com/HL7/fhir-bfdr.git
#     cd fhir-bfdr
#     sushi
#
# The value set JSON files will be built in fhir-bfdr/fsh-generated/resources/. To generate the value set
# lookup file, assuming the current directory is this scripts directory and the IG was checked out at the same
# directory level as this repository, run
#
#     ruby generate_value_set_lookups.rb ../../fhir-bfdr/fsh-generated ../projects/VitalRecord/CodeSystems.cs >! ../projects/BFDR/ValueSets.cs
#
# To generate the value set lookup file for the common VitalRecord library, first checkout the VR Common IG
#
#     git clone https://github.com/HL7/vr-common-library.git
#     cd vrdr
#     sushi
#
# The value set JSON files will be built in vr-common-library/fsh-generated/resources/. To generate the value
# set lookup file, assuming the current directory is this scripts directory and the IG was checked out at the
# same directory level as this repository, run
#
#     ruby generate_value_set_lookups.rb ../../vr-common-library/fsh-generated ../projects/VitalRecord/CodeSystems.cs >! ../projects/VitalRecord/ValueSets.cs
#
# Example of generated output:
# 
# namespace VRDR
# {
#     public static class ValueSets
#     {
#         public static class DeathLocationType {
#             public static string[,] Codes = {
#                 { "63238001", "Hospital Dead on Arrival", VRDR.CodeSystems.SCT },
#                 { "440081000124100", "Decedent's Home", VRDR.CodeSystems.SCT },
#                 { "440071000124103", "Hospice", VRDR.CodeSystems.SCT },
#                 { "16983000", "Hospital Inpatient", VRDR.CodeSystems.SCT },
#                 { "450391000124102", "Death in emergency Room/Outpatient", VRDR.CodeSystems.SCT },
#                 { "450381000124100", "Death in nursing home/Long term care facility", VRDR.CodeSystems.SCT },
#                 { "OTH", "Other (Specify)", CodeSystems.PH_NullFlavor_HL7_V3 },
#                 { "UNK", "Unknown", CodeSystems.PH_NullFlavor_HL7_V3 }
#             };
#             public static string Hospital_Dead_on_Arrival = "63238001";
#             public static string Decedents_Home = "440081000124100";
#             ...
#         }
#     }
# }

require 'json'
require 'erb'

path_to_value_sets = ARGV.shift
path_to_codesystems = ARGV.shift

# Read the codesystems source code file and generate a mapping
codesystems = {}
File.read(path_to_codesystems).split("\n").each do |line|
  if match = line.match(/public static string ([^\s]+) = "([^"]+)";/)
    codesystems[match[2]] = match[1]
  end
end

# Read all the value set definitions in the IG
value_set_files = Dir.glob("#{path_to_value_sets}/**/ValueSet*.json")
raise "No Value Sets Found" unless value_set_files.size > 0

# Map the file name to a class name
valuesets = {}
# The set of prefixes and postfixes we can remove
prefixes = ['ValueSet-ValueSet-', 'ValueSet-vrdr-', 'ValueSet-valueset-', 'ValueSet-']
postfixes = ['-vs.json', '-vr.json', '.json']
value_set_files.each do |filename|
  shortname = File.basename(filename)
  prefixes.each { |prefix| shortname.gsub!(prefix, '') }
  postfixes.each { |postfix| shortname.gsub!(postfix, '') }
  classname = shortname.split('-').map(&:capitalize).join('')
  valuesets[classname] = JSON.parse(File.read(filename))
end

# There is a special case where the SexAssignedAtBirth value set refers to VSAC, which we can't easily
# process, so we populate it here
# TODO: Reconsider this workaround if the SexAssignedAtBirth value set is updated
if valuesets['SexAssignedAtBirth']
  if valuesets['SexAssignedAtBirth']['compose']['include'].any? { |include| include&.[]('concept')&.any? { |concept| concept['code'] == 'F' } }
    raise "Workaround for SexAssignedAtBirth value set no longer needed"
  end
  valuesets['SexAssignedAtBirth']['compose']['include'] << {
    'system' => 'http://terminology.hl7.org/CodeSystem/v3-AdministrativeGender',
    'concept' => [ { 'code' => 'F', 'display' => 'Female' }, { 'code' => 'M', 'display' => 'Male' } ]
  }     
  valuesets['SexAssignedAtBirth']['compose']['include'] << {
    'system' => "http://terminology.hl7.org/CodeSystem/v3-NullFlavor",
    'concept' => [ { 'code' => 'UNK', 'display' => 'unknown' } ]
  }
end

# The PregnancyStatus value set doesn't have a display value for N/A, so we add it here
# TODO: Remove this workaround if PregnancyStatus is updated
if valuesets['PregnancyStatus']
  if valuesets.dig('PregnancyStatus', 'compose', 'include', 1, 'concept', 0, 'code') != 'NA'
    raise 'Workaround for PregnancyStatus is not longer finding the correct coding to update'
  end
  if valuesets.dig('PregnancyStatus', 'compose', 'include', 1, 'concept', 0, 'display')
    raise 'Workaround for PregnancyStatus value set no longer needed'
  end
  valuesets.dig('PregnancyStatus', 'compose', 'include', 1, 'concept', 0)['display'] = 'not applicable'
end

# Some value set definitions only have null flavor values, leave those out
valuesets.reject! do |classname, data|
  data.dig('compose', 'include').all? do |subset|
    subset['system'].nil? || subset['concept'].nil? || subset['system'].match(/NullFlavor/)
  end
end

# The JurisdictionsProvinces value set is obsolete, so leave it out
valuesets.delete('JurisdictionsProvinces')

# These are special cases that we want to shorten the string produced by the general approach, for practical reasons
special_cases =
    {
    "Medical_Examiner_Coroner_On_The_Basis_Of_Examination_And_Or_Investigation_In_My_Opinion_Death_Occurred_At_The_Time_Date_And_Place_And_Due_To_The_Cause_S_And_Manner_Stated" => "Medical_Examiner_Coroner",
    "Pronouncing_Certifying_Physician_To_The_Best_Of_My_Knowledge_Death_Occurred_At_The_Time_Date_And_Place_And_Due_To_The_Cause_S_And_Manner_Stated" => "Pronouncing_Certifying_Physician",
    "Certifying_Physician_To_The_Best_Of_My_Knowledge_Death_Occurred_Due_To_The_Cause_S_And_Manner_Stated" => "Certifying_Physician"
}

# Track any code systems we come across where we don't have constants defined
systems_without_constants = []

# The namespace depends on the IG
namespace = case path_to_value_sets
            when /vrdr/ then 'VRDR'
            when /bfdr/ then 'BFDR'
            when /common/ then 'VR'
            end

# Create a template for the output file
template = <<-EOT
// DO NOT EDIT MANUALLY! This file was generated by the script "<%= scriptname %>"

namespace <%= namespace %>
{
    /// <summary> ValueSet Helpers </summary>
    public static class ValueSets
    {
<% valuesets.each do |classname, value_set_data| -%>
        /// <summary> <%= classname %> </summary>
        public static class <%= classname %> {
            /// <summary> Codes </summary>
            public static string[,] Codes = {
<% groups = value_set_data['compose']['include'] -%>
<% groups.each do | group | -%>
<% next unless group['concept'] -%>
<% system = codesystems[group['system']] || group['system'] -%>
<% systems_without_constants << group['system'] unless codesystems[group['system']] -%>
<% group['concept'].each_with_index do |concept, index| -%>
                { "<%= concept['code'] %>", "<%= concept['display'] %>", VR.CodeSystems.<%= system %> },
<% end -%>
<% end -%>
            };

<% groups.each do | group | -%>
<% next unless group['concept'] -%>
<% system = codesystems[group['system']] || group['system'] -%>
<% group['concept'].each do |concept| -%>
<% raise 'Concept ' + concept.inspect + ' in ' + classname + ' found without a display value' unless concept['display'] -%>
<% display = concept['display'].gsub("'", '').split(/[^a-z0-9]+/i).map(&:capitalize).join('_') -%>
<% display = "_" + display if display.match(/^\\d/) -%>
<% display = special_cases[display] if special_cases[display] -%>
            /// <summary> <%= display %> </summary>
            public static string <%= display %> = "<%= concept['code'] %>";
<% end -%>
<% end -%>
        };

<% end -%>
    }
}
EOT

# Set up other variables that are used in the template above
scriptname = __FILE__

# Populate and print the template
puts ERB.new(template, trim_mode: '-').result(binding)

if systems_without_constants.length > 0
  STDERR.puts
  STDERR.puts "Saw the following code systems that don't have constants:"
  STDERR.puts
  STDERR.puts systems_without_constants.uniq
  STDERR.puts
  STDERR.puts "Suggestions:"
  STDERR.puts
  systems_without_constants.uniq.each do |system|
    system_name = case system 
                  when /CodeSystem\/CodeSystem-vr-/
                    system.match(/CodeSystem\/CodeSystem-vr-(.*)/)[1]
                  when /CodeSystem\/vrdr-/
                    system.match(/CodeSystem\/vrdr-(.*)-cs/)[1]
                  when /CodeSystem\/CodeSystem-.*-vr/
                    system.match(/CodeSystem\/CodeSystem-(.*)-vr/)[1]
                  when /CodeSystem\/CodeSystem-.*/
                    system.match(/CodeSystem\/CodeSystem-(.*)/)[1]
                  when /https?:\/\/www\..*\.com/
                    system.match(/https?:\/\/www\.(.*)\.com/)[1]
                  else
                    'unknown'
                  end
    variable_name = system_name.split('-').map(&:capitalize).join('')
    comment_name = system_name.split('-').map(&:capitalize).join(' ')
    STDERR.puts "        /// <summary> #{comment_name} </summary>"
    STDERR.puts "        public static string #{variable_name} = \"#{system}\";"
    STDERR.puts
  end
end

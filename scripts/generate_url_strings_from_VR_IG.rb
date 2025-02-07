# This script takes the JSON files that are generated as part of the VR IG and creates an output
# file with static URL strings for each StructureDefinition, Extension, and IG HTML page
#
# Usage: ruby scripts/generate_url_strings_from_VR_IG.rb <path-to-json-files> > projects/VitalRecord/URLs.cs
#
# If you need to generate the concept map JSON files, first install sushi (https://github.com/FHIR/sushi) then
#
#     git clone https://github.com/HL7/vr-common-library
#     cd vr-common-library
#     sushi
#
# The JSON files will be built in vr-common-library/fsh-generated/resources/
#
# Usage: ruby scripts/generate_url_strings_from_VR_IG.rb vr-common-library/fsh-generated/resources/

require 'json'
require 'erb'

path_to_ig = ARGV.shift
raise "Please provide a path to the IG JSON files" unless path_to_ig

# Load the Structure Definitions from the provided directory
structure_definition_files = Dir.glob("#{path_to_ig}/**/StructureDefinition*.json")
raise "No Structure Definitions Found" unless structure_definition_files.size > 0

# Load the CodeSystem Definitions from the provided directory
code_system_files = Dir.glob("#{path_to_ig}/**/CodeSystem*.json")
raise "No CodeSystem Definitions Found" unless code_system_files.size > 0

# Iterate through each file and populate a hash of name => url
structure_definition_hash = {}
name = "blank"
structure_definitions = structure_definition_files.each do |structure_definition_file|
  # Load and parse the JSON
  structure_definition = JSON.parse(File.read(structure_definition_file))
  # Grab the name and URL and populate the hash
  name = structure_definition['name']
  url = structure_definition['url']
  structure_definition_hash[name] = url
end

# Iterate through each file and populate a hash of name => url
code_system_hash = {}
code_systems = code_system_files.each do |code_system_file|
  # Load and parse the JSON
  code_system = JSON.parse(File.read(code_system_file))
  # Grab the name and URL and populate the hash
  name = code_system['name']
  url = code_system['url']
  code_system_hash[name] = url
end

# Helper method to transform a StructureDefinition or Codesystem URL into a relative extension URL
def structure_definition_url_to_ig_url(url)
  # Transform this: http://hl7.org/fhir/us/vr-common-library/StructureDefinition/vrdr-decedent-education-level
  # Into this:      https://hl7.org/fhir/us/vr-common-library/StructureDefinition-vrdr-decedent-education-level.html
  url.gsub('http://hl7.org/fhir/us/vr-common-library/StructureDefinition/', 'https://hl7.org/fhir/us/vr-common-library/StructureDefinition-') + '.html'
end
# Helper method to determine whether a definition is an Extension or a Profile
def url_type(name)
  if name.include?('Extension') then 'extension' 
  elsif name.include?('CodeSystem') then 'codesystem' 
  else 'profile' 
  end
end

# Helper method to extract a short name from an extension named according to conventions
def short_name(name)
  if /Extension(?<shortname>[a-zA-Z]+)VitalRecords/ =~ name
    shortname
  elsif /CodeSystem(?<shortname>[a-zA-Z]+)VitalRecords/ =~ name
    shortname
  elsif /(Observation|Patient|Practitioner)(?<shortname>[a-zA-Z]+)VitalRecords/ =~ name
    shortname
  elsif /(?<shortname>[a-zA-Z]+)VitalRecords/ =~ name
    shortname
  else
    name
  end
end

# Create a template for the output file

template = <<-EOT
// DO NOT EDIT MANUALLY! This file was generated by the script "<%= scriptname %>"

namespace VR
{
    /// <summary>Profile URLs</summary>
    public static class ProfileURL
    {
<% structure_definition_hash.select { |name, url| url_type(name) == 'profile' }.each do |name, url| -%>
        /// <summary>URL for <%= short_name(name) %></summary>
        public const string <%= short_name(name) %> = "<%= url %>";

<% end -%>
    }

    /// <summary>Extension URLs</summary>
    public class ExtensionURL
    {

<% structure_definition_hash.select { |name, url| url_type(name) == 'extension' }.each do |name, url| -%>
        /// <summary>URL for <%= short_name(name) %></summary>
        public const string <%= short_name(name) %> = "<%= url %>";

<% end -%>

        /// <summary>URL for PatientBirthTime as defined in the VitalRecords IG</summary>
        public const string PatientBirthTime = "http://hl7.org/fhir/StructureDefinition/patient-birthTime";  

        /// <summary>URL for PartialDate Day as defined in the VitalRecords IG</summary>
        public const string PartialDateDayVR = "day";     

        /// <summary>URL for PartialDate Month as defined in the VitalRecords IG</summary>
        public const string PartialDateMonthVR = "month";

        /// <summary>URL for PartialDateTime Year as defined in the VitalRecords IG</summary>
        public const string PartialDateYearVR = "year";

        /// <summary>URL for PartialDateTime Time as defined in the VitalRecords IG</summary>
        public const string PartialDateTimeVR = "time";

    }
            /// <summary>Extension URLs</summary>
    public class CodeSystemURL
    {

<% code_system_hash.select { |name, url| url_type(name) == 'codesystem' }.each do |name, url| -%>
        /// <summary>URL for <%= short_name(name) %></summary>
        public const string <%= short_name(name) %> = "<%= url %>";

<% end -%>
    }
    /// <summary>IG URLs</summary>
    public static class IGURL
    {
<% structure_definition_hash.each do |name, url| -%>
        /// <summary>URL for <%= short_name(name) %></summary>
        public const string <%= short_name(name) %> = "<%= structure_definition_url_to_ig_url(url) %>";

<% end -%>
    }
    /// <summary>Vital Records Custom Code System URLs</summary>
    public static class IGCodeSystemURL
    {
        /// <summary>URL for <%= short_name(name) %></summary>
<% code_system_hash.each do |name, url| -%>
        /// <summary>URL for <%= short_name(name) %></summary>
        public const string <%= short_name(name) %> = "<%= structure_definition_url_to_ig_url(url) %>";
<% end -%>
    }
}
EOT

# Set up other variables that are used in the template above
scriptname = __FILE__

# Populate and print the template
puts ERB.new(template, nil, '-').result(binding)

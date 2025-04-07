# Read a list of files, each containing one or more IJE records, and write individual FHIR records where the
# file name is the certificate identifier (year, jurisdiction, certificate number) with the JSON extension

require 'open3'

# Look for the user supplied output directory first and create it if it's not there
output_directory = ARGV.shift
if output_directory.nil?
  puts "Usage: #{__FILE__} <output-directory> <ije-files...>"
  exit
end
if !Dir.exist?(output_directory)
  puts "Creating directory \"#{output_directory}\""
  Dir.mkdir(output_directory)
end

# Relative path to the CLI used for IJE to FHIR conversion
cli_path = File.join(File.dirname(__FILE__), '..', '..', 'projects', 'VRDR.CLI')

# Convert in batches of at most 100
ARGF.each_slice(100) do |ije_lines|

  # Create individual IJE files for each record, keeping track of the file names
  puts "Creating #{ije_lines.size} individual IJE files"
  ije_individual_files = []
  ije_lines.each do |ije_line|
    file_prefix = ije_line[0,12]
    ije_individual_file = File.join(output_directory, "#{file_prefix}.ije")
    ije_individual_files << ije_individual_file
    File.write(ije_individual_file, ije_line)
  end

  # Convert the IJE files to FHIR JSON
  puts "Converting #{ije_lines.size} individual IJE files to individual FHIR JSON files"
  command = "dotnet run --project #{cli_path} ije2json #{ije_individual_files.join(' ')}"
  stdout, stderr, status = Open3.capture3(command)
  if stderr.length > 0
    puts stderr
  end
  # Delete the IJE files now that we're done with them
  puts "Cleaning up #{ije_lines.size} individual IJE files"
  File.unlink(*ije_individual_files)
end

require 'pp'
require 'json'
require 'rexml/document'
require 'rexml/formatters/pretty'

# Other Changes that are missing from this script:
# The BirthRecordIdentifier profile (http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-birth-record-identifier) uses the code vrdr-observation-cs#childbirthrecordidentifier instead of http://terminology.hl7.org/CodeSystem/v2-0203#BR.

#ruby tools/convertTestInstancesToSTU2.rb 

#create hash for mapping of links 
urisSTU3toSTU2 = {
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/BypassEditFlag' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/BypassEditFlag', # profile
'http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-vr-edit-flags' => 'http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-bypass-edit-flag-cs', #codesystem
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Extension-jurisdiction-id-vr' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/Location-Jurisdiction-Id', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/AuxiliaryStateIdentifier1' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/AuxiliaryStateIdentifier1', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/AuxiliaryStateIdentifier2' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/AuxiliaryStateIdentifier2', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/CertificateNumber' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/CertificateNumber', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/input-race-and-ethnicity-vr' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-input-race-and-ethnicity', #profile
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/coded-race-and-ethnicity-vr' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-coded-race-and-ethnicity', #profile
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Observation-emerging-issues-vr' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-emerging-issues', #profile
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/CityCode' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/CityCode', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/DistrictCode' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/DistrictCode', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/StreetDesignator'=> 'http://hl7.org/fhir/us/vrdr/StructureDefinition/StreetDesignator', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/StreetName' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/StreetName', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/StreetNumber' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/StreetNumber', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/PreDirectional' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/PreDirectional', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/PostDirectional' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/PostDirectional', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/UnitOrAptNumber' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/UnitOrAptNumber', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Extension-partial-date-time-vr' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/PartialDateTime', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Extension-partial-date-vr' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/PartialDate', #extension
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/Extension-within-city-limits-indicator-vr' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/WithinCityLimitsIndicator', #extension
'day' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/Date-Day', #extension 
'month' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/Date-Month', #extension 
'year' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/Date-Year', #extension
'time' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/Date-Time', #extension
'http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-local-observation-codes-vr' => 'http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-observations-cs' , #codesystem
'http://hl7.org/fhir/us/vr-common-library/CodeSystem/codesystem-vr-component' => 'http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-component-cs', #codesystem
'http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-race-code-vr' => 'http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-race-code-cs', #codesystem
'http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-race-recode-40-vr' => 'http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-race-recode-40-cs', #codesystem
'http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-hispanic-origin-vr' => 'http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-hispanic-origin-cs', #codesystem
'http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-country-code-vr' => 'http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-country-code-cs', #codesystem
'http://hl7.org/fhir/us/vr-common-library/CodeSystem/CodeSystem-jurisdictions-vr' => 'http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-jurisdictions-cs', #codesystem
'http://hl7.org/fhir/us/vrdr/CodeSystem/CodeSystem-death-pregnancy-status' => 'http://hl7.org/fhir/us/vrdr/CodeSystem/vrdr-pregnancy-status-cs', # unintentional change of Codesystem ID between STU2 and STU3.3
}



#this is needed because Decedent examples in VRDR use partialDate extension for birthdate. this was changed to partialDateTime extension in the our new abstract Patient profile
decedentOnlyUris = {
'http://hl7.org/fhir/us/vr-common-library/StructureDefinition/ExtensionPartialDateTimeVitalRecords' => 'http://hl7.org/fhir/us/vrdr/StructureDefinition/PartialDate'
}



def  createSTU2toSTU3Mapping(urisSTU3toSTU2)
  rev_urisSTU3toSTU2 = {}
  urisSTU3toSTU2.each{|key, value|
    rev_urisSTU3toSTU2[value] = key
  }

  return rev_urisSTU3toSTU2

end


# #this is needed to substitute the partialDateTime extension for the Decedent part of the DeathCertificateDocument and MortalityRoster bundles
# def exchangeFirstURL(pOutputFile, pInputFile, uris)
#     content = File.read(pInputFile)
#     uris.each{|key, value|
#         content=content.sub(key,value)}
#     File.open(pOutputFile, 'w') { |file| file.write(content) }
#     return pOutputFile
# end

#global substitute json
def exchangeURLsJSON(pOutputFile, pInputFile, uris)
    content = File.read(pInputFile)
    uris.each{|key, value|
        content=content.gsub(key,value)}
    parsed_content = JSON.parse(content)
    pretty_json = JSON.pretty_generate(parsed_content)
    File.open(pOutputFile, 'w') { |file| file.puts(pretty_json) }
    return pOutputFile
end





#global substitute xml
def exchangeURLsXML(pOutputFile, pInputFile, uris)
  formatter = REXML::Formatters::Pretty.new(2) # Set indentation level
  xml_content = File.read(pInputFile)
  uris.each{|key, value|
      xml_content=xml_content.gsub(key,value)}
  doc = REXML::Document.new(xml_content)
  File.open(pOutputFile, 'w') { |file| formatter.write(doc, file)  }
  return pOutputFile
end


urisSTU2toSTU3 = createSTU2toSTU3Mapping(urisSTU3toSTU2)

testInstancesBeforeConvertingFolder = 'projects/VRDR.Tests/fixtures/json/old/'
testInstancesAfterConvertingFolder = 'projects/VRDR.Tests/fixtures/json/'
Dir.foreach(Dir.pwd + '/' + testInstancesBeforeConvertingFolder) do |filename|
    next if filename == '.' or filename == '..'
    print "'" + filename + "'" + "\n"
    rpath = Dir.pwd +  '/' + testInstancesBeforeConvertingFolder + filename
    next if File.directory?(rpath)
    vOutputFile = File.open(Dir.pwd +  '/' + testInstancesAfterConvertingFolder + filename, "w")
    vInputFile = File.open(rpath)
    # if filename.include? "Patient-Decedent-Example" 
    #     vInputFile = exchangeURLs(vOutputFile, vInputFile, decedentOnlyUris) #exchange birthdate url in Decedent examples
    # end
    # if filename.include? "Bundle-DeathCertificateDocument-Example" or filename.include? "Bundle-MortalityRosterBundle-Example"
    #     vInputFile = exchangeFirstURL(vOutputFile, vInputFile, decedentOnlyUris) #exchange Decedent birthdate url in bundles
    # end
    exchangeURLsJSON(vOutputFile, vInputFile, urisSTU2toSTU3)
end

testInstancesBeforeConvertingFolder = 'projects/VRDR.Tests/fixtures/xml/old/'
testInstancesAfterConvertingFolder = 'projects/VRDR.Tests/fixtures/xml/'
Dir.foreach(Dir.pwd + '/' + testInstancesBeforeConvertingFolder) do |filename|
    next if filename == '.' or filename == '..'
    print "'" + filename + "'" + "\n"
    rpath = Dir.pwd +  '/' + testInstancesBeforeConvertingFolder + filename
    next if File.directory?(rpath)
    vOutputFile = File.open(Dir.pwd +  '/' + testInstancesAfterConvertingFolder + filename, "w")
    vInputFile = File.open(rpath)
    # if filename.include? "Patient-Decedent-Example" 
    #     vInputFile = exchangeURLs(vOutputFile, vInputFile, decedentOnlyUris) #exchange birthdate url in Decedent examples
    # end
    # if filename.include? "Bundle-DeathCertificateDocument-Example" or filename.include? "Bundle-MortalityRosterBundle-Example"
    #     vInputFile = exchangeFirstURL(vOutputFile, vInputFile, decedentOnlyUris) #exchange Decedent birthdate url in bundles
    # end
    exchangeURLsXML(vOutputFile, vInputFile, urisSTU2toSTU3)
end
# Exit if a command fails
set -e

# Run the C# test suite
# echo "* dotnet test BFDR.Tests/BFDR.Tests.csproj"
# dotnet test BFDR.Tests/BFDR.Tests.csproj

# Make sure we can read and parse a JSON file
echo "* dotnet run --project BFDR.CLI 2ije birth BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI 2ije birth BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null
echo "* dotnet run --project BFDR.CLI 2ije fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json"
dotnet run --project BFDR.CLI 2ije fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json > /dev/null

# Make sure we can read and parse an XML file
echo "* dotnet run --project BFDR.CLI 2ijecontent birth BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI 2ijecontent birth BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null
echo "* dotnet run --project BFDR.CLI 2ijecontent fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json"
dotnet run --project BFDR.CLI 2ijecontent fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json > /dev/null

# Make sure we can pull all information out of a JSON file
echo "* dotnet run --project BFDR.CLI description birth BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI description birth BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null
echo "* dotnet run --project BFDR.CLI description fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json"
dotnet run --project BFDR.CLI description fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json > /dev/null

# Make sure we can convert ije to json
echo "* dotnet run --project BFDR.CLI ije2json birth BFDR.Tests/fixtures/ije/BasicBirthRecord.ije"
dotnet run --project BFDR.CLI ije2json birth BFDR.Tests/fixtures/ije/BasicBirthRecord.ije > /dev/null
echo "* dotnet run --project BFDR.CLI ije2json fetaldeath BFDR.Tests/fixtures/ije/FetalDeathRecord.ije"
dotnet run --project BFDR.CLI ije2json fetaldeath BFDR.Tests/fixtures/ije/FetalDeathRecord.ije > /dev/null

# Make sure we can convert ije to xml format
echo "* dotnet run --project BFDR.CLI ije2xml birth BFDR.Tests/fixtures/ije/BasicBirthRecord.ije"
dotnet run --project BFDR.CLI ije2xml birth BFDR.Tests/fixtures/ije/BasicBirthRecord.ije > /dev/null
echo "* dotnet run --project BFDR.CLI ije2xml fetaldeath BFDR.Tests/fixtures/ije/FetalDeathRecord.ije"
dotnet run --project BFDR.CLI ije2xml fetaldeath BFDR.Tests/fixtures/ije/FetalDeathRecord.ije > /dev/null

# Make sure we can convert content of submission message in ije format
echo "* dotnet run --project BFDR.CLI extract2ijecontent birth BFDR.Tests/fixtures/json/BirthRecordSubmissionMessage.json"
dotnet run --project BFDR.CLI extract2ijecontent birth BFDR.Tests/fixtures/json/BirthRecordSubmissionMessage.json > /dev/null
echo "* dotnet run --project BFDR.CLI extract2ijecontent fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecordSubmissionMessage.json"
dotnet run --project BFDR.CLI extract2ijecontent fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecordSubmissionMessage.json > /dev/null

# Make sure we can void a record
echo "* dotnet run --project BFDR.CLI void birth BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI void birth BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null
echo "* dotnet run --project BFDR.CLI void birth BFDR.Tests/fixtures/json/BasicBirthRecord.json 1"
dotnet run --project BFDR.CLI void birth BFDR.Tests/fixtures/json/BasicBirthRecord.json 1 > /dev/null
echo "* dotnet run --project BFDR.CLI void fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json"
dotnet run --project BFDR.CLI void fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json > /dev/null
echo "* dotnet run --project BFDR.CLI void fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json 1"
dotnet run --project BFDR.CLI void fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json 1 > /dev/null

# Make sure we can create an acknowledgement FHIR message for a submission FHIR message
echo "* dotnet run --project BFDR.CLI ack birth BFDR.Tests/fixtures/json/BirthRecordSubmissionMessage.json"
dotnet run --project BFDR.CLI ack birth BFDR.Tests/fixtures/json/BirthRecordSubmissionMessage.json > /dev/null
echo "* dotnet run --project BFDR.CLI ack birth BFDR.Tests/fixtures/json BFDR.Tests/fixtures/json/BirthRecordSubmissionMessage.json"
dotnet run --project BFDR.CLI ack birth BFDR.Tests/fixtures/json BFDR.Tests/fixtures/json/BirthRecordSubmissionMessage.json > /dev/null
echo "* dotnet run --project BFDR.CLI ack fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecordSubmissionMessage.json"
dotnet run --project BFDR.CLI ack fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecordSubmissionMessage.json > /dev/null
echo "* dotnet run --project BFDR.CLI ack fetaldeath BFDR.Tests/fixtures/json BFDR.Tests/fixtures/json/BasicFetalDeathRecordSubmissionMessage.json"
dotnet run --project BFDR.CLI ack fetaldeath BFDR.Tests/fixtures/json BFDR.Tests/fixtures/json/BasicFetalDeathRecordSubmissionMessage.json > /dev/null

# Make sure we can read in and parse an ije record
echo "* dotnet run --project BFDR.CLI ije birth BFDR.Tests/fixtures/ije/BasicBirthRecord.ije"
dotnet run --project BFDR.CLI ije birth BFDR.Tests/fixtures/ije/BasicBirthRecord.ije > /dev/null
echo "* dotnet run --project BFDR.CLI ije fetaldeath BFDR.Tests/fixtures/ije/FetalDeathRecord2.ije"
dotnet run --project BFDR.CLI ije fetaldeath BFDR.Tests/fixtures/ije/FetalDeathRecord2.ije > /dev/null

# Make sure we can create a json record using IJE mapped fields
echo "* dotnet run --project BFDR.CLI ijebuilder birth"
dotnet run --project BFDR.CLI ijebuilder birth > /dev/null
echo "* dotnet run --project BFDR.CLI ijebuilder fetaldeath"
dotnet run --project BFDR.CLI ijebuilder fetaldeath > /dev/null

# Make sure we can read in the FHIR JSON
echo "* dotnet run --project BFDR.CLI json2xml birth BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI json2xml birth BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null
echo "* dotnet run --project BFDR.CLI json2xml fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json"
dotnet run --project BFDR.CLI json2xml fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json > /dev/null

# Make sure we can read in the FHIR xml
echo "* dotnet run --project BFDR.CLI checkXml birth BFDR.Tests/fixtures/xml/BasicBirthRecord.xml"
dotnet run --project BFDR.CLI checkXml birth BFDR.Tests/fixtures/xml/BasicBirthRecord.xml > /dev/null
echo "* dotnet run --project BFDR.CLI checkXml fetaldeath BFDR.Tests/fixtures/xml/BasicFetalDeathRecord.xml"
dotnet run --project BFDR.CLI checkXml fetaldeath BFDR.Tests/fixtures/xml/BasicFetalDeathRecord.xml > /dev/null

# Make sure we can read in the FHIR JSON
echo "* dotnet run --project BFDR.CLI checkJson birth BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI checkJson birth BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null
echo "* dotnet run --project BFDR.CLI checkJson fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json"
dotnet run --project BFDR.CLI checkJson fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json > /dev/null

# Make sure we can convert xml to FHIR JSON
echo "* dotnet run --project BFDR.CLI xml2json birth BFDR.Tests/fixtures/xml/BasicBirthRecord.xml"
dotnet run --project BFDR.CLI xml2json birth BFDR.Tests/fixtures/xml/BasicBirthRecord.xml > /dev/null
echo "* dotnet run --project BFDR.CLI xml2json fetaldeath BFDR.Tests/fixtures/xml/BasicFetalDeathRecord.xml"
dotnet run --project BFDR.CLI xml2json fetaldeath BFDR.Tests/fixtures/xml/BasicFetalDeathRecord.xml > /dev/null

# Make sure we can convert xml to xml
echo "* dotnet run --project BFDR.CLI xml2xml birth BFDR.Tests/fixtures/xml/BasicBirthRecord.xml"
dotnet run --project BFDR.CLI xml2xml birth BFDR.Tests/fixtures/xml/BasicBirthRecord.xml > /dev/null
echo "* dotnet run --project BFDR.CLI xml2xml fetaldeath BFDR.Tests/fixtures/xml/BasicFetalDeathRecord.xml"
dotnet run --project BFDR.CLI xml2xml fetaldeath BFDR.Tests/fixtures/xml/BasicFetalDeathRecord.xml > /dev/null

# Make sure we can convert FHIR JSON to FHIR JSON
echo "* dotnet run --project BFDR.CLI json2json birth BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI json2json birth BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null
echo "* dotnet run --project BFDR.CLI json2json fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json"
dotnet run --project BFDR.CLI json2json fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json > /dev/null

# Convert JSON files to IJE and back and make sure there's no data loss
echo "* dotnet run --project BFDR.CLI roundtrip-ije birth BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI roundtrip-ije birth BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null
echo "* dotnet run --project BFDR.CLI roundtrip-ije fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json"
dotnet run --project BFDR.CLI roundtrip-ije fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord.json > /dev/null
# echo "* dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1_wJurisdiction.json"
# dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1_wJurisdiction.json

# Convert XML files to IJE and back and make sure there's no data loss
# echo "* dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1.xml"
# dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1.xml
# echo "* dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1_wJurisdiction.xml"
# dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1_wJurisdiction.xml

# Convert JSON files to IJE and back and check every field individually
echo "* dotnet run --project BFDR.CLI roundtrip-all birth BFDR.Tests/fixtures/json/BirthRecordBabyGQuinn.json"
dotnet run --project BFDR.CLI roundtrip-all birth BFDR.Tests/fixtures/json/BirthRecordBabyGQuinn.json 
echo "* dotnet run --project BFDR.CLI roundtrip-all birth BFDR.Tests/fixtures/json/BirthRecordBabyGQuinnJurisdiction.json"
dotnet run --project BFDR.CLI roundtrip-all birth BFDR.Tests/fixtures/json/BirthRecordBabyGQuinnJurisdiction.json 
echo "* dotnet run --project BFDR.CLI roundtrip-all birth BFDR.Tests/fixtures/json/BirthRecordFakeWithRace.json"
dotnet run --project BFDR.CLI roundtrip-all birth BFDR.Tests/fixtures/json/BirthRecordFakeWithRace.json

echo "* dotnet run --project BFDR.CLI roundtrip-all fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord2.json"
dotnet run --project BFDR.CLI roundtrip-all fetaldeath BFDR.Tests/fixtures/json/BasicFetalDeathRecord2.json

# Convert an XML file to IJE and back and check every field individually
# echo "* dotnet run --project BFDR.CLI roundtrip-all BFDR.Tests/fixtures/json/BasicBirthRecord.json"
# dotnet run --project BFDR.CLI roundtrip-all BFDR.Tests/fixtures/json/BasicBirthRecord.json

# Convert a JSON file to MRE and compare the results
# echo "* dotnet run --project BFDR.CLI json2mre BFDR.Tests/fixtures/json/Bundle-DemographicCodedContentBundle-Example1.json > example.mre"
# dotnet run --project BFDR.CLI json2mre BFDR.Tests/fixtures/json/Bundle-DemographicCodedContentBundle-Example1.json > example.mre
# echo "* dotnet run --project BFDR.CLI compareMREtoJSON example.mre BFDR.Tests/fixtures/json/Bundle-DemographicCodedContentBundle-Example1.json"
# dotnet run --project BFDR.CLI compareMREtoJSON example.mre BFDR.Tests/fixtures/json/Bundle-DemographicCodedContentBundle-Example1.json
# rm example.mre

# Convert a JSON file to TRX and compare the results
# echo "* dotnet run --project BFDR.CLI json2trx BFDR.Tests/fixtures/json/Bundle-CauseOfDeathCodedContentBundle-Example1.json > example.trx"
# dotnet run --project BFDR.CLI json2trx BFDR.Tests/fixtures/json/Bundle-CauseOfDeathCodedContentBundle-Example1.json > example.trx
# echo "* dotnet run --project BFDR.CLI compareTRXtoJSON example.trx BFDR.Tests/fixtures/json/Bundle-CauseOfDeathCodedContentBundle-Example1.json"
# dotnet run --project BFDR.CLI compareTRXtoJSON example.trx BFDR.Tests/fixtures/json/Bundle-CauseOfDeathCodedContentBundle-Example1.json
# rm example.trx

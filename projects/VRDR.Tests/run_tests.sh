# Exit if a command fails
set -e

# Run the C# test suite
echo "* dotnet test VRDR.Tests/VRDR.Tests.csproj"
dotnet test VRDR.Tests/VRDR.Tests.csproj

# Make sure we can read and parse a JSON file
echo "* dotnet run --project VRDR.CLI json2json VRDR.CLI/1_wJurisdiction.json"
dotnet run --project VRDR.CLI json2json VRDR.CLI/1_wJurisdiction.json > /dev/null

# Make sure we can read and parse an XML file
echo "* dotnet run --project VRDR.CLI xml2xml VRDR.CLI/1_wJurisdiction.xml"
dotnet run --project VRDR.CLI xml2xml VRDR.CLI/1_wJurisdiction.xml > /dev/null

# Make sure we can pull all information out of a JSON file
echo "* dotnet run --project VRDR.CLI description VRDR.CLI/1_wJurisdiction.json"
dotnet run --project VRDR.CLI description VRDR.CLI/1_wJurisdiction.json > /dev/null

# Make sure we can pull all information out of a XML file
echo "* dotnet run --project VRDR.CLI description VRDR.CLI/1_wJurisdiction.xml"
dotnet run --project VRDR.CLI description VRDR.CLI/1_wJurisdiction.xml > /dev/null

# Convert JSON files to IJE and back and make sure there's no data loss
echo "* dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI/1.json"
dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI/1.json
echo "* dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI/1_wJurisdiction.json"
dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI/1_wJurisdiction.json

# Convert XML files to IJE and back and make sure there's no data loss
echo "* dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI/1.xml"
dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI/1.xml
echo "* dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI/1_wJurisdiction.xml"
dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI/1_wJurisdiction.xml

# Convert JSON files to IJE and back and check every field individually
echo "* dotnet run --project VRDR.CLI roundtrip-all VRDR.Tests/fixtures/json/Bundle-DeathCertificateDocument-Example2.json"
dotnet run --project VRDR.CLI roundtrip-all VRDR.Tests/fixtures/json/Bundle-DeathCertificateDocument-Example2.json
echo "* dotnet run --project VRDR.CLI roundtrip-all VRDR.CLI/1_wJurisdiction.json"
dotnet run --project VRDR.CLI roundtrip-all VRDR.CLI/1_wJurisdiction.json

# Convert an XML file to IJE and back and check every field individually
echo "* dotnet run --project VRDR.CLI roundtrip-all VRDR.CLI/1_wJurisdiction.xml"
dotnet run --project VRDR.CLI roundtrip-all VRDR.CLI/1_wJurisdiction.xml

# Convert a JSON file to MRE and compare the results
echo "* dotnet run --project VRDR.CLI json2mre VRDR.Tests/fixtures/json/Bundle-DemographicCodedContentBundle-Example1.json > example.mre"
dotnet run --project VRDR.CLI json2mre VRDR.Tests/fixtures/json/Bundle-DemographicCodedContentBundle-Example1.json > example.mre
echo "* dotnet run --project VRDR.CLI compareMREtoJSON example.mre VRDR.Tests/fixtures/json/Bundle-DemographicCodedContentBundle-Example1.json"
dotnet run --project VRDR.CLI compareMREtoJSON example.mre VRDR.Tests/fixtures/json/Bundle-DemographicCodedContentBundle-Example1.json
rm example.mre

# Convert a JSON file to TRX and compare the results
echo "* dotnet run --project VRDR.CLI json2trx VRDR.Tests/fixtures/json/Bundle-CauseOfDeathCodedContentBundle-Example1.json > example.trx"
dotnet run --project VRDR.CLI json2trx VRDR.Tests/fixtures/json/Bundle-CauseOfDeathCodedContentBundle-Example1.json > example.trx
echo "* dotnet run --project VRDR.CLI compareTRXtoJSON example.trx VRDR.Tests/fixtures/json/Bundle-CauseOfDeathCodedContentBundle-Example1.json"
dotnet run --project VRDR.CLI compareTRXtoJSON example.trx VRDR.Tests/fixtures/json/Bundle-CauseOfDeathCodedContentBundle-Example1.json
rm example.trx

# Convert an STU3 JSON file (with only features found in STU2) to STU2 and Back and Compare the STU2 JSON against the STU2 fixture.
echo "* dotnet run --project VRDR.CLI rdtripstu3-to-stu2 VRDR.Tests/fixtures/json/DeathRecord1_STU3.json "   
dotnet run --project VRDR.CLI rdtripstu3-to-stu2 VRDR.Tests/fixtures/json/DeathRecord1_STU3.json    
echo "* dotnet run --project VRDR.CLI json-diff tempSTU2.json VRDR.Tests/fixtures/json/DeathRecord1_STU2.json"
dotnet run --project VRDR.CLI json-diff tempSTU2.json VRDR.Tests/fixtures/json/DeathRecord1_STU2.json

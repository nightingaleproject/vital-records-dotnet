# Exit if a command fails
set -e

# Run the C# test suite
# echo "* dotnet test BFDR.Tests/BFDR.Tests.csproj"
# dotnet test BFDR.Tests/BFDR.Tests.csproj

# Make sure we can read and parse a JSON file
echo "* dotnet run --project BFDR.CLI 2ije BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI 2ije BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null

# Make sure we can read and parse an XML file
echo "* dotnet run --project BFDR.CLI 2ijecontent BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI 2ijecontent BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null

# Make sure we can pull all information out of a JSON file
echo "* dotnet run --project BFDR.CLI description BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI description BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null

# Make sure we can pull all information out of a XML file
echo "* dotnet run --project BFDR.CLI description BFDR.Tests/fixtures/json/BasicBirthRecord.json"
dotnet run --project BFDR.CLI description BFDR.Tests/fixtures/json/BasicBirthRecord.json > /dev/null

# Convert JSON files to IJE and back and make sure there's no data loss
# echo "* dotnet run --project BFDR.CLI roundtrip-ije BFDR.Tests/fixtures/json/BasicBirthRecord.json"
# dotnet run --project BFDR.CLI roundtrip-ije BFDR.Tests/fixtures/json/BasicBirthRecord.json
# echo "* dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1_wJurisdiction.json"
# dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1_wJurisdiction.json

# Convert XML files to IJE and back and make sure there's no data loss
# echo "* dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1.xml"
# dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1.xml
# echo "* dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1_wJurisdiction.xml"
# dotnet run --project BFDR.CLI roundtrip-ije BFDR.CLI/1_wJurisdiction.xml

# Convert JSON files to IJE and back and check every field individually
echo "* dotnet run --project BFDR.CLI roundtrip-all BFDR.Tests/fixtures/json/BirthRecordBabyGQuinn.json"
dotnet run --project BFDR.CLI roundtrip-all BFDR.Tests/fixtures/json/BirthRecordBabyGQuinn.json 
echo "* dotnet run --project BFDR.CLI roundtrip-all BFDR.Tests/fixtures/json/BirthRecordBabyGQuinnJurisdiction.json"
dotnet run --project BFDR.CLI roundtrip-all BFDR.Tests/fixtures/json/BirthRecordBabyGQuinnJurisdiction.json 
echo "* dotnet run --project BFDR.CLI roundtrip-all BFDR.Tests/fixtures/json/BirthRecordFakeWithRace.json"
dotnet run --project BFDR.CLI roundtrip-all BFDR.Tests/fixtures/json/BirthRecordFakeWithRace.json

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

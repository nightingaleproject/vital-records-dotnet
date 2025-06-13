@REM Run the C# test suite
dotnet test VRDR.Tests\VRDR.Tests.csproj || exit /b 127

@REM Make sure we can read and parse a JSON file
dotnet run --project VRDR.CLI json2json VRDR.CLI\1_wJurisdiction.json > NUL || exit /b 127

@REM Make sure we can read and parse an XML file
dotnet run --project VRDR.CLI xml2xml VRDR.CLI\1_wJurisdiction.xml > NUL || exit /b 127

@REM Make sure we can pull all information out of a JSON file
dotnet run --project VRDR.CLI description VRDR.CLI\1_wJurisdiction.json > NUL || exit /b 127

@REM Make sure we can pull all information out of a XML file
dotnet run --project VRDR.CLI description VRDR.CLI\1_wJurisdiction.xml > NUL || exit /b 127

@REM Convert JSON files to IJE and back and make sure there's no data loss
dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI\1.json || exit /b 127
dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI\1_wJurisdiction.json || exit /b 127

@REM Convert XML files to IJE and back and make sure there's no data loss
dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI\1.xml || exit /b 127
dotnet run --project VRDR.CLI roundtrip-ije VRDR.CLI\1_wJurisdiction.xml || exit /b 127

@REM Convert JSON files to IJE and back and check every field individually
dotnet run --project VRDR.CLI roundtrip-all VRDR.Tests\fixtures\json\Bundle-DeathCertificateDocument-Example2.json || exit /b 127
dotnet run --project VRDR.CLI roundtrip-all VRDR.CLI\1_wJurisdiction.json || exit /b 127

@REM Convert an XML file to IJE and back and check every field individually
dotnet run --project VRDR.CLI roundtrip-all VRDR.CLI\1_wJurisdiction.xml || exit /b 127

@REM Convert a JSON file to MRE and compare the results
dotnet run --project VRDR.CLI json2mre VRDR.Tests\fixtures\json\Bundle-DemographicCodedContentBundle-Example1.json > example.mre || exit /b 127
dotnet run --project VRDR.CLI compareMREtoJSON example.mre VRDR.Tests\fixtures\json\Bundle-DemographicCodedContentBundle-Example1.json || exit /b 127
del example.mre

@REM Convert a JSON file to TRX and compare the results
dotnet run --project VRDR.CLI json2trx VRDR.Tests\fixtures\json\Bundle-CauseOfDeathCodedContentBundle-Example1.json > example.trx || exit /b 127
dotnet run --project VRDR.CLI compareTRXtoJSON example.trx VRDR.Tests\fixtures\json\Bundle-CauseOfDeathCodedContentBundle-Example1.json || exit /b 127
del example.trx

@REM Convert an STU3 JSON file (with only features found in STU2) to STU2 and Back and Compare the STU2 JSON against the STU2 fixture.
dotnet run --project VRDR.CLI rdtripstu3-to-stu2 VRDR.Tests\fixtures\json\DeathRecord1_STU3.json || exit /b 127
dotnet run --project VRDR.CLI json-diff tempSTU2.json VRDR.Tests\fixtures\json\DeathRecord1_STU2.json || exit /b 127

@REM Make sure we can read and parse a JSON file
dotnet run --project BFDR.CLI 2ije birth BFDR.Tests\fixtures\json\BasicBirthRecord.json > NUL || exit /b 127
dotnet run --project BFDR.CLI 2ije fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecord.json > NUL || exit /b 127

@REM Make sure we can read and parse an XML file
dotnet run --project BFDR.CLI 2ijecontent birth BFDR.Tests\fixtures\json\BasicBirthRecord.json > NUL || exit /b 127
dotnet run --project BFDR.CLI 2ijecontent fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecord.json > NUL || exit /b 127

@REM Make sure we can pull all information out of a JSON file
dotnet run --project BFDR.CLI description birth BFDR.Tests\fixtures\json\BasicBirthRecord.json > NUL || exit /b 127
dotnet run --project BFDR.CLI description fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecord.json > NUL || exit /b 127

@REM Make sure we can convert ije to json
dotnet run --project BFDR.CLI ije2json birth BFDR.Tests\fixtures\ije\BasicBirthRecord.ije > NUL || exit /b 127
dotnet run --project BFDR.CLI ije2json fetaldeath BFDR.Tests\fixtures\ije\FetalDeathRecord.ije > NUL || exit /b 127

@REM Make sure we can convert ije to xml format
dotnet run --project BFDR.CLI ije2xml birth BFDR.Tests\fixtures\ije\BasicBirthRecord.ije > NUL || exit /b 127
dotnet run --project BFDR.CLI ije2xml fetaldeath BFDR.Tests\fixtures\ije\FetalDeathRecord.ije > NUL || exit /b 127

@REM Make sure we can convert content of submission message in ije format
dotnet run --project BFDR.CLI extract2ijecontent birth BFDR.Tests\fixtures\json\BirthRecordSubmissionMessage.json > NUL || exit /b 127
dotnet run --project BFDR.CLI extract2ijecontent fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecordSubmissionMessage.json > NUL || exit /b 127

@REM Make sure we can void a record
dotnet run --project BFDR.CLI void birth BFDR.Tests\fixtures\json\BasicBirthRecord.json > NUL || exit /b 127
dotnet run --project BFDR.CLI void birth BFDR.Tests\fixtures\json\BasicBirthRecord.json 1 > NUL || exit /b 127
dotnet run --project BFDR.CLI void fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecord.json > NUL || exit /b 127
dotnet run --project BFDR.CLI void fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecord.json 1 > NUL || exit /b 127

@REM Make sure we can create an acknowledgement FHIR message for a submission FHIR message
dotnet run --project BFDR.CLI ack birth BFDR.Tests\fixtures\json\BirthRecordSubmissionMessage.json > NUL || exit /b 127
dotnet run --project BFDR.CLI ack birth BFDR.Tests\fixtures\json BFDR.Tests\fixtures\json\BirthRecordSubmissionMessage.json > NUL || exit /b 127
dotnet run --project BFDR.CLI ack fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecordSubmissionMessage.json > NUL || exit /b 127
dotnet run --project BFDR.CLI ack fetaldeath BFDR.Tests\fixtures\json BFDR.Tests\fixtures\json\BasicFetalDeathRecordSubmissionMessage.json > NUL || exit /b 127

@REM Make sure we can read in and parse an ije record
dotnet run --project BFDR.CLI ije birth BFDR.Tests\fixtures\ije\BasicBirthRecord.ije > NUL || exit /b 127
dotnet run --project BFDR.CLI ije fetaldeath BFDR.Tests\fixtures\ije\FetalDeathRecord2.ije > NUL || exit /b 127

@REM Make sure we can create a json record using IJE mapped fields
dotnet run --project BFDR.CLI ijebuilder birth > NUL || exit /b 127
dotnet run --project BFDR.CLI ijebuilder fetaldeath > NUL || exit /b 127

@REM Make sure we can read in the FHIR JSON
dotnet run --project BFDR.CLI json2xml birth BFDR.Tests\fixtures\json\BasicBirthRecord.json > NUL || exit /b 127
dotnet run --project BFDR.CLI json2xml fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecord.json > NUL || exit /b 127

@REM Make sure we can read in the FHIR xml
dotnet run --project BFDR.CLI checkXml birth BFDR.Tests\fixtures\xml\BasicBirthRecord.xml > NUL || exit /b 127
dotnet run --project BFDR.CLI checkXml fetaldeath BFDR.Tests\fixtures\xml\BasicFetalDeathRecord.xml > NUL || exit /b 127

@REM Make sure we can read in the FHIR JSON
dotnet run --project BFDR.CLI checkJson birth BFDR.Tests\fixtures\json\BasicBirthRecord.json > NUL || exit /b 127
dotnet run --project BFDR.CLI checkJson fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecord.json > NUL || exit /b 127

@REM Make sure we can convert xml to FHIR JSON
dotnet run --project BFDR.CLI xml2json birth BFDR.Tests\fixtures\xml\BasicBirthRecord.xml > NUL || exit /b 127
dotnet run --project BFDR.CLI xml2json fetaldeath BFDR.Tests\fixtures\xml\BasicFetalDeathRecord.xml > NUL || exit /b 127

@REM Make sure we can convert xml to xml
dotnet run --project BFDR.CLI xml2xml birth BFDR.Tests\fixtures\xml\BasicBirthRecord.xml > NUL || exit /b 127
dotnet run --project BFDR.CLI xml2xml fetaldeath BFDR.Tests\fixtures\xml\BasicFetalDeathRecord.xml > NUL || exit /b 127

@REM Make sure we can convert FHIR JSON to FHIR JSON
dotnet run --project BFDR.CLI json2json birth BFDR.Tests\fixtures\json\BasicBirthRecord.json > NUL || exit /b 127
dotnet run --project BFDR.CLI json2json fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecord.json > NUL || exit /b 127

@REM Convert JSON files to IJE and back and make sure there's no data loss
dotnet run --project BFDR.CLI roundtrip-ije birth BFDR.Tests\fixtures\json\BasicBirthRecord.json > NUL || exit /b 127
dotnet run --project BFDR.CLI roundtrip-ije birth BFDR.Tests\fixtures\json\BasicBirthRecord2.json > NUL || exit /b 127
dotnet run --project BFDR.CLI roundtrip-ije fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecord.json > NUL || exit /b 127

@REM Convert JSON files to IJE and back and check every field individually
dotnet run --project BFDR.CLI roundtrip-all birth BFDR.Tests\fixtures\json\BirthRecordZ.json || exit /b 127

dotnet run --project BFDR.CLI roundtrip-all birth BFDR.Tests\fixtures\json\BirthRecordBabyGQuinn.json || exit /b 127
dotnet run --project BFDR.CLI roundtrip-all birth BFDR.Tests\fixtures\json\BirthRecordBabyGQuinnJurisdiction.json || exit /b 127
dotnet run --project BFDR.CLI roundtrip-all birth BFDR.Tests\fixtures\json\BirthRecordFakeWithRace.json || exit /b 127

dotnet run --project BFDR.CLI roundtrip-all fetaldeath BFDR.Tests\fixtures\json\BasicFetalDeathRecord2.json || exit /b 127

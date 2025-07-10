# bfdr-cli

This project contains a small .NET (C#) command-line interface program that demonstrates usage of the BFDR project.

## Usage

These sample commands are run from the `projects` directory. The `[birth|fetaldeath]` argument should be substituted with one of `birth` or `fetaldeath`, indicating which record type should be used for the operation.

```bash
# Prints CLI Help
dotnet run --project BFDR.CLI help

# Builds a fake birth/fetal death record and print out the record as FHIR XML and JSON
dotnet run --project BFDR.CLI fakerecord [birth|fetaldeath]

# Read in the FHIR XML or JSON birth/fetal death record and print out as IJE
dotnet run --project BFDR.CLI 2ije [birth|fetaldeath] BFDR.CLI/1.xml

# Read in the IJE birth/fetal death record and print out as FHIR XML
dotnet run --project BFDR.CLI ije2xml [birth|fetaldeath] BFDR.CLI/1.MOR

# Read in the IJE birth/fetal death record and print out as FHIR JSON
dotnet run --project BFDR.CLI ije2json [birth|fetaldeath] BFDR.CLI/1.MOR

# Read in the FHIR XML birth/fetal death record and print out as FHIR JSON
dotnet run --project BFDR.CLI xml2json [birth|fetaldeath] BFDR.CLI/1.xml

# Read in the FHIR JSON birth/fetal death record and print out as FHIR XML
dotnet run --project BFDR.CLI json2xml [birth|fetaldeath] BFDR.CLI/1.json

# Read in the FHIR JSON birth/fetal death record, completely disassemble then reassemble, and print as FHIR JSON
dotnet run --project BFDR.CLI json2json [birth|fetaldeath] BFDR.CLI/1.json

# Read in the FHIR XML birth/fetal death record, completely disassemble then reassemble, and print as FHIR XML
dotnet run --project BFDR.CLI xml2xml [birth|fetaldeath] BFDR.CLI/1.xml

# Read in the given FHIR xml (being permissive) and print out the same; useful for doing validation diffs
dotnet run --project BFDR.CLI checkXml [birth|fetaldeath] BFDR.CLI/1.xml

# Read in the given FHIR json (being permissive) and print out the same; useful for doing validation diffs
dotnet run --project BFDR.CLI checkJson [birth|fetaldeath] BFDR.CLI/1.json

# Read in and parse an IJE birth/fetal death record and print out the values for every (supported) field
dotnet run --project BFDR.CLI ije [birth|fetaldeath] BFDR.CLI/1.MOR

# Generate a verbose JSON description of the record (in the format used to drive Canary)
dotnet run --project BFDR.CLI description [birth|fetaldeath] BFDR.CLI/1.json

# Dump content of a birth/fetal death record in key/value IJE format
dotnet run --project BFDR.CLI 2ijecontent [birth|fetaldeath] BFDR.CLI/1.json

# Convert a record to IJE and back to identify any conversion issues
dotnet run --project BFDR.CLI roundtrip-ije [birth|fetaldeath] BFDR.CLI/1.json

# Convert a record to JSON and back to identify any conversion issues
dotnet run --project BFDR.CLI roundtrip-all [birth|fetaldeath] BFDR.CLI/1.json

# Create a record using the provided IJE field name and value pairs
dotnet run --project BFDR.CLI ijebuilder [birth|fetaldeath] GNAME=Lazarus AGE=990

# Compare an IJE record with a FHIR record by each IJE field
dotnet run --project BFDR.CLI compare [birth|fetaldeath] BFDR.CLI/1.MOR BFDR.CLI/1.json

# Extract a FHIR record from a FHIR message
dotnet run --project BFDR.CLI extract BFDR.CLI/1submit.json

# Dump content of a submission message in key/value IJE format
dotnet run --project BFDR.CLI extract2ijecontent [birth|fetaldeath] BFDR.CLI/1submit.json

# Create an acknowledgement FHIR message for a submission FHIR message
dotnet run --project BFDR.CLI ack [birth|fetaldeath] BFDR.CLI/1submit.json

# Creates a void message for a birth Record (1 argument: FHIR birth record; one optional argument: number of records to void)
dotnet run --project BFDR.CLI void birth BFDR.Tests/fixtures/json/BirthRecord1.json

# Creates a void message for a fetal death Record (1 argument: FHIR fetal death record; one optional argument: number of records to void)
dotnet run --project BFDR.CLI void fetaldeath BFDR.Tests/fixtures/json/FetalDeathRecord1.json
```

# bfdr-cli

This project contains a small .NET (C#) command-line interface program that demonstrates usage of the BFDR project.

## Usage: Birth Examples

```bash
# Prints CLI Help
dotnet run --project BFDR.CLI help

# Builds a fake birth record and print out the record as FHIR XML and JSON
dotnet run --project BFDR.CLI fakerecord birth

# Read in the FHIR XML or JSON birth record and print out as IJE
dotnet run --project BFDR.CLI 2ije birth BFDR.CLI/1.xml

# Read in the IJE birth record and print out as FHIR XML
dotnet run --project BFDR.CLI ije2xml birth BFDR.CLI/1.MOR

# Read in the IJE birth record and print out as FHIR JSON
dotnet run --project BFDR.CLI ije2json birth BFDR.CLI/1.MOR

# Read in the FHIR XML birth record and print out as FHIR JSON
dotnet run --project BFDR.CLI xml2json birth BFDR.CLI/1.xml

# Read in the FHIR JSON birth record and print out as FHIR XML
dotnet run --project BFDR.CLI json2xml birth BFDR.CLI/1.json

# Read in the FHIR JSON birth record, completely disassemble then reassemble, and print as FHIR JSON
dotnet run --project BFDR.CLI json2json birth BFDR.CLI/1.json

# Read in the FHIR XML birth record, completely disassemble then reassemble, and print as FHIR XML
dotnet run --project BFDR.CLI xml2xml birth BFDR.CLI/1.xml

# Read in the given FHIR xml (being permissive) and print out the same; useful for doing validation diffs
dotnet run --project BFDR.CLI checkXml birth BFDR.CLI/1.xml

# Read in the given FHIR json (being permissive) and print out the same; useful for doing validation diffs
dotnet run --project BFDR.CLI checkJson birth BFDR.CLI/1.json

# Read in and parse an IJE birth record and print out the values for every (supported) field
dotnet run --project BFDR.CLI ije birth BFDR.CLI/1.MOR

# Generate a verbose JSON description of the record (in the format used to drive Canary)
dotnet run --project BFDR.CLI description birth BFDR.CLI/1.json

# Dump content of a birth record in key/value IJE format
dotnet run --project BFDR.CLI 2ijecontent birth BFDR.CLI/1.json

# Convert a record to IJE and back to identify any conversion issues
dotnet run --project BFDR.CLI roundtrip-ije birth BFDR.CLI/1.json

# Convert a record to JSON and back to identify any conversion issues
dotnet run --project BFDR.CLI roundtrip-all birth BFDR.CLI/1.json

# Create a record using the provided IJE field name and value pairs
dotnet run --project BFDR.CLI ijebuilder birth GNAME=Lazarus AGE=990

# Compare an IJE record with a FHIR record by each IJE field
dotnet run --project BFDR.CLI compare birth BFDR.CLI/1.MOR BFDR.CLI/1.json

# Extract a FHIR record from a FHIR message
dotnet run --project BFDR.CLI extract BFDR.CLI/1submit.json

# Dump content of a submission message in key/value IJE format
dotnet run --project BFDR.CLI extract2ijecontent birth BFDR.CLI/1submit.json

# Create an acknowledgement FHIR message for a submission FHIR message
dotnet run --project BFDR.CLI ack birth BFDR.CLI/1submit.json

# Creates a void message for a Birth Record (1 argument: FHIR birth record; one optional argument: number of records to void)
dotnet run --project BFDR.CLI void birth BFDR.Tests/fixtures/json/BirthRecord1.json
```

## Usage: Fetal Death Examples

```bash
# Prints CLI Help
dotnet run --project BFDR.CLI help

# Builds a fake fetaldeath record and print out the record as FHIR XML and JSON
dotnet run --project BFDR.CLI fakerecord fetaldeath

# Read in the FHIR XML or JSON fetaldeath record and print out as IJE
dotnet run --project BFDR.CLI 2ije fetaldeath BFDR.CLI/1.xml

# Read in the IJE fetaldeath record and print out as FHIR XML
dotnet run --project BFDR.CLI ije2xml fetaldeath BFDR.CLI/1.MOR

# Read in the IJE fetaldeath record and print out as FHIR JSON
dotnet run --project BFDR.CLI ije2json fetaldeath BFDR.CLI/1.MOR

# Read in the FHIR XML fetaldeath record and print out as FHIR JSON
dotnet run --project BFDR.CLI xml2json fetaldeath BFDR.CLI/1.xml

# Read in the FHIR JSON fetaldeath record and print out as FHIR XML
dotnet run --project BFDR.CLI json2xml fetaldeath BFDR.CLI/1.json

# Read in the FHIR JSON fetaldeath record, completely disassemble then reassemble, and print as FHIR JSON
dotnet run --project BFDR.CLI json2json fetaldeath BFDR.CLI/1.json

# Read in the FHIR XML fetaldeath record, completely disassemble then reassemble, and print as FHIR XML
dotnet run --project BFDR.CLI xml2xml fetaldeath BFDR.CLI/1.xml

# Read in the given FHIR xml (being permissive) and print out the same; useful for doing validation diffs
dotnet run --project BFDR.CLI checkXml fetaldeath BFDR.CLI/1.xml

# Read in the given FHIR json (being permissive) and print out the same; useful for doing validation diffs
dotnet run --project BFDR.CLI checkJson fetaldeath BFDR.CLI/1.json

# Read in and parse an IJE fetaldeath record and print out the values for every (supported) field
dotnet run --project BFDR.CLI ije fetaldeath BFDR.CLI/1.MOR

# Generate a verbose JSON description of the record (in the format used to drive Canary)
dotnet run --project BFDR.CLI description fetaldeath BFDR.CLI/1.json

# Dump content of a fetaldeath record in key/value IJE format
dotnet run --project BFDR.CLI 2ijecontent fetaldeath BFDR.CLI/1.json

# Convert a record to IJE and back to identify any conversion issues
dotnet run --project BFDR.CLI roundtrip-ije fetaldeath BFDR.CLI/1.json

# Convert a record to JSON and back to identify any conversion issues
dotnet run --project BFDR.CLI roundtrip-all fetaldeath BFDR.CLI/1.json

# Create a record using the provided IJE field name and value pairs
dotnet run --project BFDR.CLI ijebuilder fetaldeath GNAME=Lazarus AGE=990

# Compare an IJE record with a FHIR record by each IJE field
dotnet run --project BFDR.CLI compare fetaldeath BFDR.CLI/1.MOR BFDR.CLI/1.json

# Extract a FHIR record from a FHIR message
dotnet run --project BFDR.CLI extract BFDR.CLI/1submit.json

# Dump content of a submission message in key/value IJE format
dotnet run --project BFDR.CLI extract2ijecontent fetaldeath BFDR.CLI/1submit.json

# Create an acknowledgement FHIR message for a submission FHIR message
dotnet run --project BFDR.CLI ack fetaldeath BFDR.CLI/1submit.json

# Creates a void message for a fetaldeath Record (1 argument: FHIR fetaldeath record; one optional argument: number of records to void)
dotnet run --project BFDR.CLI void fetaldeath BFDR.Tests/fixtures/json/FetalDeathRecord1.json
```

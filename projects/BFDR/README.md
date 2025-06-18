# bfdr-dotnet

This repository includes .NET (C#) code for

- Producing and consuming the Vital Records Birth and Fetal Birth Reporting (BFDR) Health Level 7 (HL7) Fast Healthcare Interoperability Resources (FHIR) standard. [Click here to view the FHIR Implementation Guide STU2.0](https://build.fhir.org/ig/HL7/fhir-bfdr/branches/master/index.html).
- Producing and consuming FHIR messages for the exchange of BFDR documents.
- Support for converting BFDR FHIR records to and from the Inter-Jurisdictional Exchange (IJE) Natality format, as well as companion microservice for performing conversions.

## Documentation

## Versions
Interactions with NCHS are governed by the CI build version of the BFDR and Birth and Fetal Birth Records Messaging IGs, and should use the latest, supported releases of the BFDR .NET software and Canary.

<table class="versionTable" border="3">
<tbody>
<tr>
<td style="text-align: center;"><strong>BFDR IG</strong></td>
<td style="text-align: center;"><strong>Messaging IG</strong></td>
<td style="text-align: center;"><strong>FHIR</strong></td>
<td style="text-align: center;"><strong>Version</strong></td>
<td style="text-align: center;"><strong>BFDR</strong></td>
<td style="text-align: center;"><strong>BFDR.Messaging</strong></td>
</tr>
</tr>
<tr>
<td style="text-align: center;"><a href="https://build.fhir.org/ig/HL7/fhir-bfdr/branches/v2.0.0-preview3/ije_mapping_natality.html">STU2.0 v2.0.0-preview3</a></td>
<td style="text-align: center;"><a href="https://nightingaleproject.github.io/vital_records_fhir_messaging_ig/v2.0.0-preview1/index.html">v2.0.0-preview1</a></td>
<td style="text-align: center;">R4</td>
<td style="text-align: center;">V4.0.3</td>
<td style="text-align: center;"><a href="">nuget</a> <a href=""> github</a></td>
<td style="text-align: center;"><a href="">nuget</a> <a href=""> github</a></td>
</tr>


</tbody>
</table>

## Requirements

### Development & CLI Requirements
- This repository is built using .NET Core 6.0, download [here](https://dotnet.microsoft.com/download)
### Library Usage
- The BFDR and BFDR.Messaging libraries target .NET Standard 2.0
- To check whether your .NET version supports a release, refer to [the .NET matrix](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support).
  - Note whether you are using .NET Core or .NET Framework - see [here](https://docs.microsoft.com/en-us/archive/msdn-magazine/2017/september/net-standard-demystifying-net-core-and-net-standard) for distinctions between the .NET implementation options.
  - Once you’ve determined your .NET implementation type and version, for example you are using .NET Framework 4.6.1, refer to the matrix to verify whether your .NET implementation supports the targeted .NET Standard version.
    - Ex. If you are using .NET Framework 4.6.1, you can look at the matrix and see the .NET Framework 4.6.1 supports .NET Standard 2.0 so the tool would be supported.

## Project Organization

### BFDR
This directory contains a FHIR Birth and Fetal Birth Record library for consuming and producing BFDR FHIR. This library also includes support for converting to and from the Inter-Jurisdictional Exchange (IJE) Natality format.

#### Usage

This package will be published on NuGet, so including it is as easy as:
```xml
<ItemGroup>
  ...
  <PackageReference Include="BFDR" Version="1.0.0-preview.13" />
  ...
</ItemGroup>
```

You can also include a locally downloaded copy of the library instead of the NuGet version by referencing `BFDR.csproj` in your project configuration, for example (taken from BFDR.CLI):
```xml
<Project Sdk="Microsoft.NET.Sdk">
  ...
  <ItemGroup>
    <ProjectReference Include="..\BFDR\BFDR.csproj" />
    ...
  </ItemGroup>
</Project>
```

#### Producing Example
The following snippet is a quick example of producing a from-scratch FHIR BFDR record using this library, and then printing it out as a JSON string. 
```cs
using BFDR;

BirthRecord birthRecord = new BirthRecord();

// Set birth date in parts
BirthRecord birthRecord = new BirthRecord();
birthRecord.BirthYear = 2023;
birthRecord.BirthMonth = 1;
birthRecord.BirthDay = 1;
// Set birth date with one date
birthRecord.DateOfBirth = "2023-01-01";

// Set certificate number
birthRecord.CertificateNumber = "100";

// Set state identifier
birthRecord.StateLocalIdentifier1 = "123";

// Print record as a JSON string
Console.WriteLine(birthRecord.ToJSON());
```

#### Consuming Example
An example of consuming a BFDR FHIR document (in XML format) using this library, and printing some details from it:
```cs
using BFDR;

// Read in FHIR Birth Record XML file as a string
string xml = File.ReadAllText("./example_bfdr_fhir_record.xml");

// Construct a new BirthRecord object from the FHIR BFDR XML string
BirthRecord birthRecord = new BirthRecord(xml);

// Print out some details from the record
Console.WriteLine($"Child's Last Name: {birthRecord.ChildFamilyName}");
```

#### Specifying that a date or time is explicitly unknown

When specifying a date or time it is important to be able to differentiate between "we explicitly
don't know the date, and we're telling you that we don't know it" and just not setting a date
property at all. For this reason the date and time properties on the BirthRecord class support the
special value of -1 (for properties that expect an integer) or "-1" (for properties that expect a
string) in order to specify that the data is explicitly unknown. This is equivalent to using a value
of "9999" in IJE.

Example:

```
BirthRecord birthRecord = new BirthRecord();
birthRecord.BirthYear = 2022;
birthRecord.BirthMonth = 2;
birthRecord.BirthDay = -1;
birthRecord.BirthTime = "-1";
```

#### Names in FHIR

FHIR manages names in a way that there is a fundamental incompatibility with IJE: in FHIR the
"middle name" is stored as the second element in an array of given names. That means that it's not
possible to set a middle name without first setting a first name. The library handles this by

1. Requiring the entire given name (first name and any middle names) to be set all at once when
using the BirthRecord class
1. Raising an exception if a middle name is set before a first name when using the IJEBirth
class
1. Resetting the middle name if the first name is set again when using the IJEBirth class;
setting the first name and then the middle name ensures no issues will occur.

For the child's last name, if the family name, denoted as ChildFamilyName in FHIR, is missing or unknown,
its corresponding KIDLNAME in IJE has value of "UNKNOWN". Vice versa, if its KIDLNAME in IJE is "UNKNOWN",
its corresponding ChildFamilyName in FHIR has value of NULL. All other values have 1-to-1 mappings between
FHIR's ChildFamilyName and IJE's KIDLNAME.

#### FHIR BFDR record to/from IJE Natality format

An example of converting a BFDR FHIR Birth Record to an IJE string:
```cs
using BFDR;

// Read in FHIR Birth Record XML file as a string
string xml = File.ReadAllText("./example_bfdr_fhir_record.xml");

// Construct a new BirthRecord object from the string
BirthRecord birthRecord = new BirthRecord(xml);

// Create an IJEBirth instance from the BirthRecord
IJEBirth ije = new IJEBirth(birthRecord);

// Print out the corresponding IJE version of the BirthRecord
string ijeString = ije.ToString(); // Converts BirthRecord to IJE
Console.WriteLine(ijeString);
```

An example of converting an IJE string to a BFDR FHIR Birth Record:
```cs
using BFDR;

// Construct a new IJEBirth instance from an IJE string
IJEBirth ije = new IJEBirth("..."); // This will convert the IJE string to a BirthRecord

// Grab the corresponding FHIR BirthRecord
BirthRecord birthRecord = ije.ToRecord();

// Print out the converted FHIR BirthRecord as a JSON string
Console.WriteLine(birthRecord.ToJSON());
```

### BFDR.Messaging

This directory contains classes to create and parse FHIR messages used for Birth and Fetal Birth Reporting.

#### Usage

This package is published on NuGet, so including it is as easy as:
```xml
<ItemGroup>
  ...
  <PackageReference Include="BFDR.Messaging" Version="1.0.0-preview.13" />
  ...
</ItemGroup>
```

Note that the BFDR.Messaging package automatically includes the BFDR package, a project file should not reference both.

You can also include a locally downloaded copy of the library instead of the NuGet version by referencing `BFDRMessaging.csproj` in your project configuration, for example:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  ...
  <ItemGroup>
    <ProjectReference Include="..\BFDR.Messaging\BFDRMessaging.csproj" />
    ...
  </ItemGroup>
</Project>
```

#### Return Coding Examples

When birth and fetal death records are submitted to NCHS, NCHS responds with various coded content
messages. This section shows examples of creating the different types of response messages.

##### Demographics Coding Messages

The following four message types can be created:

* BirthRecordParentalDemographicsCodingMessage
* BirthRecordParentalDemographicsCodingUpdateMessage
* FetalDeathRecordParentalDemographicsCodingMessage
* FetalDeathRecordParentalDemographicsCodingUpdateMessage

These messages can be created following this example, substituting the appropriate message type
listed above as needed:

```csharp
// Create and populate the IJE record
// Note: This example also includes input fields, which can be included in the return message but may not be required
IJEBirth ije = new IJEBirth();
ije.IDOB_YR = "2022";
ije.BSTATE = "YC";
ije.FILENO = "123";
ije.IDOB_YR = "2025";
ije.BSTATE = "NJ";
ije.FILENO = "000001";
ije.AUXNO = "123456781234";
ije.MRACE1E = "100";
ije.MRACE2E = "101";
ije.MRACE3E = "102";
ije.MRACE4E = "103";
ije.MRACE5E = "104";
ije.MRACE6E = "105";
ije.MRACE7E = "106";
ije.MRACE8E = "107";
ije.MRACE16C = "108";
ije.MRACE17C = "109";
ije.MRACE18C = "110";
ije.MRACE19C = "111";
ije.MRACE20C = "112";
ije.MRACE21C = "113";
ije.MRACE22C = "114";
ije.MRACE23C = "115";
ije.FRACE1E = "116";
ije.FRACE2E = "117";
ije.FRACE3E = "118";
ije.FRACE4E = "119";
ije.FRACE5E = "120";
ije.FRACE6E = "121";
ije.FRACE7E = "122";
ije.FRACE8E = "123";
ije.FRACE16C = "124";
ije.FRACE17C = "125";
ije.FRACE18C = "126";
ije.FRACE19C = "127";
ije.FRACE20C = "128";
ije.FRACE21C = "129";
ije.FRACE22C = "130";
ije.FRACE23C = "131";
ije.METHNIC5C = "200";
ije.METHNICE = "201";
ije.FETHNIC5C = "202";
ije.FETHNICE = "203";
ije.METHNIC1 = "N";
ije.METHNIC2 = "H";
ije.METHNIC3 = "U";
ije.METHNIC4 = "N";
ije.METHNIC5 = "literal1";
ije.MRACE1 = "Y";
ije.MRACE2 = "N";
ije.MRACE3 = "Y";
ije.MRACE4 = "N";
ije.MRACE5 = "Y";
ije.MRACE6 = "N";
ije.MRACE7 = "Y";
ije.MRACE8 = "N";
ije.MRACE9 = "Y";
ije.MRACE10 = "N";
ije.MRACE11 = "Y";
ije.MRACE12 = "N";
ije.MRACE13 = "Y";
ije.MRACE14 = "N";
ije.MRACE15 = "Y";
ije.MRACE16 = "literal2";
ije.MRACE17 = "literal3";
ije.MRACE18 = "literal4";
ije.MRACE19 = "literal5";
ije.MRACE20 = "literal6";
ije.MRACE21 = "literal7";
ije.MRACE22 = "literal8";
ije.MRACE23 = "literal9";
ije.FETHNIC1 = "H";
ije.FETHNIC2 = "U";
ije.FETHNIC3 = "N";
ije.FETHNIC4 = "H";
ije.FETHNIC5 = "literal10";
ije.FRACE1 = "N";
ije.FRACE2 = "Y";
ije.FRACE3 = "N";
ije.FRACE4 = "Y";
ije.FRACE5 = "N";
ije.FRACE6 = "Y";
ije.FRACE7 = "N";
ije.FRACE8 = "Y";
ije.FRACE9 = "N";
ije.FRACE10 = "Y";
ije.FRACE11 = "N";
ije.FRACE12 = "Y";
ije.FRACE13 = "N";
ije.FRACE14 = "Y";
ije.FRACE15 = "N";
ije.FRACE16 = "literal11";
ije.FRACE17 = "literal12";
ije.FRACE18 = "literal13";
ije.FRACE19 = "literal14";
ije.FRACE20 = "literal15";
ije.FRACE21 = "literal16";
ije.FRACE22 = "literal17";
ije.FRACE23 = "literal18";
// Create the message from the IJE content and set message fields as needed
BirthRecordParentalDemographicsCodingMessage message = new BirthRecordParentalDemographicsCodingMessage(ije.ToRecord());
message.MessageSource = "http://nchs.cdc.gov/bfdr_submission";
message.MessageDestination = "https://example.org/jurisdiction/endpoint";
// The message can be converted to JSON when complete
message.ToJson()
```

##### Coded Cause of Fetal Death Message

The following two message types can be created:

* CodedCauseOfFetalDeathMessage
* CodedCauseOfFetalDeathUpdateMessage

These messages can be created following this example, substituting the appropriate message type
listed above as needed:

```csharp
// Create and populate the IJE record
IJEFetalDeath ije = new IJEFetalDeath();
ije.FDOD_YR = "2025";
ije.DSTATE = "NJ";
ije.FILENO = "000001";
ije.AUXNO = "123456781234";
ije.ICOD = "P011";
ije.OCOD1 = "P021";
ije.OCOD2 = "P022";
ije.OCOD3 = "P023";
ije.OCOD4 = "P024";
ije.OCOD5 = "P025";
ije.OCOD6 = "P026";
ije.OCOD7 = "P027";
// Create the message from the IJE content and set message fields as needed
CodedCauseOfFetalDeathMessage message = new CodedCauseOfFetalDeathMessage(ije.ToRecord());
message.MessageSource = "http://nchs.cdc.gov/bfdr_submission";
message.MessageDestination = "https://example.org/jurisdiction/endpoint";
// The message can be converted to JSON when complete
message.ToJson()
```

##### Parental Industry and Occupation Coding Message

The following four message types can be created:

* BirthRecordParentalIndustryOccupationCodingMessage
* FetalDeathRecordParentalIndustryOccupationCodingMessage
* BirthRecordParentalIndustryOccupationCodingUpdateMessage
* FetalDeathRecordParentalIndustryOccupationCodingUpdateMessage

These messages can be created following this example, substituting the appropriate message type
listed above as needed:

```csharp
// Create and populate the IJE record
IJEBirth ije = new IJEBirth();
ije.IDOB_YR = "2025";
ije.BSTATE = "NJ";
ije.FILENO = "000001";
ije.MOM_OC_T = "Mother occupation";
ije.MOM_IN_T = "Mother industry";
ije.DAD_OC_T = "Father occupation";
ije.DAD_IN_T = "Father industry";
// Some fields do not exist in IJE, so we set those after converting to a FHIR record
BirthRecord record = ije.ToBirthRecord();
record.MotherCodedOccupationHelper = "13-2011";
record.MotherCodedIndustryHelper = "54121";
record.FatherCodedOccupationHelper = "27-2011";
record.FatherCodedIndustryHelper = "5223";
// Create the message from the IJE content and set message fields as needed
BirthRecordParentalIndustryOccupationCodingMessage message = new BirthRecordParentalIndustryOccupationCodingMessage(ije.ToRecord());
message.MessageSource = "http://nchs.cdc.gov/bfdr_submission";
message.MessageDestination = "https://example.org/jurisdiction/endpoint";
// The message can be converted to JSON when complete
message.ToJson()
```

### BFDR.Tests

This directory contains unit and functional tests for the BFDR library as well as scripts for testing via the CLI and translation microservice.

#### Usage

The tests are automatically run by the repository GitHub workflows config, but can be run locally by executing the following command in the root project directory:

```bash
./BFDR.Tests/run_tests.sh
```

The C# tests can be run separately from the other tests by executing the following command:

```bash
dotnet test
```

### BFDR.CLI
This directory contains a sample command line interface app that uses the BFDR library to do a few different things.

#### Birth CLI Examples 

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

#### FetalDeath CLI Examples 

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

#### For Library Developers

Attributes (equivalent to Annotations in Java) are used in .NET to promote loose coupling via  “declarative” programming, and add Metadata to the target program entity, namely .NET assembly, module, for global scope, and class, interface, struct, enum, constructor, delegate, field, property, method, parameter, return value, and event, for non-global scope. They can be either built-in or custom, and denoted by pair or pair(s), for mutltiple attributes, of square brackets [...] surrounding the target entity. As shown in [BFDR/NatalityRecord_submissionProperties.cs](../master/BFDR/NatalityRecord_submissionProperties.cs), the custom attributes [Property(...)] and [FHIRPath(...)] for each of the BirthRecord's properties, and [PropertyParam(...)] for many of its properties, add relevant sets of Metadata to their targets, based on their definitions and orders of formal parameters given in [BFDR/BirthRecord.cs](../master/BFDR/BirthRecord.cs), where custom attribute [Property(...)], as in
```
        [Property("Birth Record Identifier", Property.Types.String, "Birth Certification", "Birth Record identifier.", true, IGURL.BirthCertificate, true, 4)]
        [FHIRPath("Bundle", "identifier")]
        public string BirthRecordIdentifier
        {
            get
            {
                if (Bundle != null && Bundle.Identifier != null)
                {
                    return Bundle.Identifier.Value;
                }
                return null;
            }
            // The setter is private because the value is derived so should never be set directly
            private set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    return;
                }
                if (Bundle.Identifier == null)
                {
                    Bundle.Identifier = new Identifier();
                }
                Bundle.Identifier.Value = value;
                Bundle.Identifier.System = "http://nchs.cdc.gov/bfdr_id";
            }
        }
```
for example, can be seen mapped to the following custom attribute class with the same name:

`public class Property : System.Attribute`

and with the following constructor:

`public Property(string name, Types type, string category, string description, bool serialize, string igurl, bool capturedInIJE, int priority = 4)`

Similarly, custom attribute [FHIRPath( ... )] is mapped to custom attribute class FHIRPath with public constructor `FHIRPath(string path, string element)`

and custom attribute [PropertyParam( ... )] is mapped to custom attribute class PropertyParam with public constructor `PropertyParam(string key, string description)`

Custom attribute classes are typically derived, either directly or indirectly, from built-in abstract class System.Attribute, just as illustrated here.

The property values of these Metadata/attributes for BirthRecord are set and retrieved via setters and getters, respectively, based on individual sets of rules also as shown in [BFDR/NatalityRecord_submissionProperties.cs](../master/BFDR/NatalityRecord_submissionProperties.cs)

Snippet from [BFDR.CLI/Program.cs](../master/projects/BFDR.CLI/Program.cs#L479-L489) gives an example of how these custom attributes can be used:
```
BirthRecord d = new BirthRecord(File.ReadAllText(args[1]));
IJEBirth ije1 = new IJEBirth(d, false);
// Loop over every property (these are the fields); Order by priority
List<PropertyInfo> properties = typeof(IJEBirth).GetProperties().ToList().OrderBy(p => p.GetCustomAttribute<IJEField>().Location).ToList();
foreach (PropertyInfo property in properties)
{
    // Grab the field attributes
    IJEField info = property.GetCustomAttribute<IJEField>();
    // Grab the field value
    string field = Convert.ToString(property.GetValue(ije1, null));
}   
```
Custom attributes are also used extensively in IJEField's properties, one of which is shown below as an example.
```
        [IJEField(1, 1, 4, "Date of Birth--Year", "DOB_YR", 1)]
        public string DOB_YR
        {
            get
            {
                return NumericAllowingUnknown_Get("DOB_YR", "BirthYear");
            }
            set
            {
                NumericAllowingUnknown_Set("DOB_YR", "BirthYear", value);
            }
        }
```
which is mapped to the following custom attribute class with the same name:

`public class IJEField : System.Attribute`

and with the following constructor:

`public IJEField(int field, int location, int length, string contents, string name, int priority)` in the same file [BFDR/IJEBirth.cs](../master/BFDR/IJEBirth.cs)


Official resources:<br/>
https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/reflection-and-attributes/attribute-tutorial<br/>
https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/attributes<br/>
https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/reflection-and-attributes/creating-custom-attributes<br/>
https://learn.microsoft.com/en-us/dotnet/standard/attributes/writing-custom-attributes<br/><br/>

## Contributing
This repository follows [Semantic Versioning](https://semver.org/). Bug fixes and feature enhancements should be merged into the `master` branch via Pull Requests (PR), instead of directly committing to the `master` branch. Once a PR is merged, a new version of the library will be automatically published to NuGet.

```
git pull origin master
git checkout -b <your-working-branch-name>
<commit-your-changes>
<test-with-changes-from-master>
git push -u origin <your-working-branch-name>
```
Once the working branch is pushed to the respository, follow these steps:

1. Create a new PR with the working branch as base, and master as head.
1. The PR title and description should follow the [Conventional Commit](https://www.conventionalcommits.org/en/v1.0.0/) format. The PR title should be structured as  `<type>[optional scope]: <short-message>`, where a type can be:
  - **feat:** introduces a new feature to the codebase (correlates with MINOR in Semantic Versioning).
  - **fix:** patches a bug in the codebase (correlates with PATCH in Semantic Versioning).
  - Other types such as `build`, `chore`, `ci`, `docs`, `style`, `refactor`, `perf`, `test`, and others are allowed as well (correlates with PATCH in Semantic Versioning).
  
  (The PR title needs to be concise and conform to the style guide for change tracking purposes. The PR description can include additional details about the changes associated with this PR.)
1. Assign one or more reviewers to review your changes. At least one approved review is required before the PR can be merged.
1. If the PR addresses an existing Issue, link the PR with the Issue to resolve it through the PR.
1. Once the PR is approved by a reviewer, with all discussions resolved and all checks passed, click the Squash and Merge button. Avoid using "Create a merge commit." The PR title and description will automatically fill in the commit message boxes.

#### Release Pull Request

With each commit to the default branch, a release pull request will be automatically created/updated. This PR increments the package version and updates the CHANGELOG using a special branch named `actions/release`. You don't need to wait for this special PR to be merged into the default branch before merging additional pull requests. It consolidates subsequent commits to the default branch; only one instance of release pull request will be active.


#### Publishing a Version

To create a new release of BFDR on NuGet:

1. Bump the version of the libraries listed in the [Directory.Build.props](Directory.Build.props) file. Whenever a commit is merged into the master branch that changes the Directory.Build.props file, [Github Actions](.github/workflows/publish.yml) will automatically build and publish a new version of the package based on the value specified.
1. Update the PayloadVersionID in BFDR.Messaging.csproj to match the IG version implemented by the library. See [Payload Version Identifier Values](https://nightingaleproject.github.io/vital_records_fhir_messaging_ig/v2.0.0-preview1/ValueSet-Payload-version-vs.html)
1. Update the version numbers listed in this README
1. Update the CHANGELOG.md file with information on what is changing in the release
1. Merge the above changes to master, causing the GitHub publishing workflow to fire
1. Create a GitHub release
    1. Go to the [Releases page](https://github.com/nightingaleproject/bfdr-dotnet/releases)
    1. Click on "Draft a new release"
    1. Enter the release version on the tag and release; this should be the same as in the Directory.Build.props file (e.g., v3.2.0-preview3)
    1. Copy the information from the CHANGELOG.md file from this version into the release description
    1. Do not check the "pre-release" button, even for preview releases, since those don't show up on the main GitHub page

## License

Copyright 2024 The MITRE Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

```
http://www.apache.org/licenses/LICENSE-2.0
```

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

### Contact Information

For questions or comments about bfdr-dotnet, please send email to

    nvssmodernization@cdc.gov

import { faMailBulk } from '@fortawesome/free-solid-svg-icons';

export function RecordTesting(recordType) {
  return [
    {
      icon: "upload",
      title: `Producing FHIR ${recordType} Records`,
      description: `Test a data provider system's ability to produce a valid FHIR ${recordType} Record document.`,
      route: `test-fhir-producing`
    },
    {
      icon: "download",
      title: `Consuming FHIR ${recordType} Records`,
      description: "Test a data provider system's ability to consume a valid FHIR ${recordType} Record document.",
      route: `test-fhir-consuming`
    },
    {
      icon: "sync",
      title: `${recordType} Record Roundtrip (Consuming)`,
      description: "Test a data provider system's ability to handle converting between internal data structures and external data formats. This test involves consuming FHIR, and then producing equivalent IJE.",
      route: `test-edrs-roundtrip-consuming`
    },
    {
      icon: "sync",
      title: `${recordType} Record Roundtrip (Producing)`,
      description: "Test a data provider system's ability to handle converting between internal data structures and external data formats. This test involves consuming IJE, and then producing equivalent FHIR.",
      route: `test-edrs-roundtrip-producing`
    },
    {
      icon: "tasks",
      title: `Connectathon FHIR ${recordType} Records (Producing)`,
      description: "Test a data provider system's ability to produce pre-defined records as tested at Connectathons.",
      route: `test-connectathon-dash/records`
    },
    {
      icon: "tasks",
      title: `Validate FHIR ${recordType} Records with IJE (Producing)`,
      description: "Test a data provider system's ability to produce records by validating against an IJE file.",
      route: `test-fhir-ije-validator-producing`
    }
  ]
}

export function MessageTesting(recordType) {
  return [
    {
      icon: "cloud upload",
      title: `Producing FHIR ${recordType} Messages`,
      description: `Test a data provider system's ability to produce a valid FHIR Message for a generated FHIR ${recordType} Record document.`,
      route: "test-fhir-message-producing"
    },
    {
      faIcon: faMailBulk,
      title: `Connectathon FHIR ${recordType} Messages (Producing)`,
      description: `Test a data provider system's ability to produce pre-defined FHIR ${recordType} Messages as tested at Connectathons.`,
      route: "test-connectathon-dash/message"
    }
  ]
}

export function RecordTools(recordType, ijeType) {
  return [
    {
      icon: "clipboard list",
      title: `Generate Synthetic ${recordType} Records`,
      description: `Generate synthetic ${recordType} records in FHIR (XML or JSON) and IJE ${ijeType} format. These generated records can be downloaded locally, copied to the clipboard, or POSTed to an endpoint.`,
      route: "tool-record-generator"
    },
    {
      icon: "clipboard check",
      title: `FHIR ${recordType} Record Syntax Checker`,
      description: `Check a given FHIR ${recordType} Record for syntax/structural issues.`,
      route: "tool-fhir-syntax-checker"
    },
    {
      icon: "random",
      title: `${recordType} Record Format Converter`,
      description: "Test a data provider system's ability to handle converting between internal data structures and external data formats. This test involves consuming FHIR, and then producing equivalent IJE.",
      route: "tool-record-converter"
    },
    {
      icon: "find",
      title: `FHIR ${recordType} Record Inspector`,
      description: `Inspect a FHIR ${recordType} Record and show details about the record and what it contains.`,
      route: "tool-fhir-inspector"
    },
    {
      icon: "magic",
      title: `FHIR ${recordType} Record Creator`,
      description: `Create a new record from scratch by filling out a web form, and generate FHIR from it.`,
      route: "tool-fhir-creator"
    },
    {
      icon: "search",
      title: `IJE ${ijeType} Record Inspector`,
      description: `Inspect an IJE ${ijeType} file and show details about the record and what it contains.`,
      route: "tool-ije-inspector"
    }
  ]
}

export function MessageTools(recordType) {
  return [
    {
      icon: "envelope",
      title: `FHIR ${recordType} Message Syntax Checker`,
      description: `Check a given FHIR ${recordType} Message for syntax/structural issues.`,
      route: "tool-fhir-message-syntax-checker"
    },
    {
      icon: "cloud download",
      title: `Creating FHIR ${recordType} Messages`,
      description: `Create a valid FHIR Message for a user provided FHIR ${recordType} Record document.`,
      route: "test-fhir-message-creation"
    },
    {
      icon: "find",
      title: `FHIR ${recordType} Message Inspector`,
      description: `Inspect a FHIR ${recordType} Message file and show details about what it contains.`,
      route: "tool-message-inspector"
    }
  ]
}
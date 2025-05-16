# VRDR.STU2.Convert

This project illustrates use of functionality included in the VRDR.CLI project to convert JSON FHIR bundles between VRDR STU2 and STU3.

## Build

```
dotnet build
```

## Convert a FHIR JSON Bundle

```
dotnet run jsonstu2-to-stu3 path/to/input/stu2file.json path/to/output/stu3file.json
```

## Roundtrip STU2 to STU3 to STU2

Convert STU2 file to STU3 and then back to STU2, compare the original and final STU2 files for equality using the VRDR library.

```
dotnet run rdtripstu2-to-stu3 path/to/input/stu2file.json
```

# VitalRecord.Tests

The classes in the VitalRecord project are tested extensively by the BFDR.Tests and VRDR.Tests projects. This project contains additional tests for the classes in the VitalRecord project to improve code coverage.

## Running

Either:

```sh
cd projects/VitalRecord.Tests
dotnet test
```

or

```sh
cd projects
dotnet test vr-dotnet.sln
```

## AI-Generated Tests

Portions of `ValueSets_Should.cs` and `Mappings_Should.cs` were created using an AI assistant due to the large amount of tests needed to cover the auto-generated value set and code mapping files. The tests were validated by human developers.
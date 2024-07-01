# Vital Records .NET Monorepo

## Projects

- [VitalRecord](projects/VitalRecord): A shared library for producing and consuming Vital Record FHIR records.

- [VRDR](projects/VRDR): FHIR Death Record library for consuming and producing VRDR FHIR.
- [Canary](projects/Canary): Canary is an open source testing framework that supports development of systems that perform standards based exchange of mortality data. Canary provides tests and tools to aid developers in implementing the Vital Records Death Reporting (VRDR) FHIR death record format.


| Package         | Latest Version |
|----------------|-----|
| VitalRecord           | [![VitalRecord on NuGet](https://img.shields.io/nuget/v/VitalRecord?label=)](https://www.nuget.org/packages/VitalRecord/) |
| VitalRecord.Messaging | [![VitalRecord.Messaging on NuGet](https://img.shields.io/nuget/v/VitalRecord.Messaging?label=)](https://www.nuget.org/packages/VitalRecord.Messaging/) |
| VRDR                  | [![VRDR on NuGet](https://img.shields.io/nuget/v/VRDR?label=)](https://www.nuget.org/packages/VRDR/)    |
| VRDR.Messaging        | [![VRDR.Messaging on NuGet](https://img.shields.io/nuget/v/VRDR.Messaging?label=)](https://www.nuget.org/packages/VRDR.Messaging/)    |
| VRDR.Client           | [![VRDR.Client on NuGet](https://img.shields.io/nuget/v/VRDR.Client?label=)](https://www.nuget.org/packages/VRDR.Client/)    |

## <a name="commit"></a> Commit Message Format

We have very precise rules over how our Git commit messages must be formatted.
This format leads to **easier to read commit history**.

Each commit message consists of a **header**, a **body**, and a **footer**.


```
<header>
<BLANK LINE>
<body>
<BLANK LINE>
<footer>
```

The `header` is mandatory and must conform to the [Commit Message Header](#commit-header) format.

The `body` is optional but is encouraged to conform to [Commit Message Body](#commit-body) format.

The `footer` is optional. The [Commit Message Footer](#commit-footer) format describes what the footer is used for and the structure it must have.


#### <a name="commit-header"></a>Commit Message Header

```
<type>(<scope>): <short summary>
  │       │             │
  │       │             └─⫸ Summary in present tense. Not capitalized. No period at the end.
  │       │
  │       └─⫸ Commit Scope: vr|vrdr|bfdr|canary
  │
  └─⫸ Commit Type: build|ci|docs|feat|fix|perf|refactor|test
```

The `<type>` and `<summary>` fields are mandatory, the `(<scope>)` field is optional.


##### Type

Must be one of the following:

* **build**: Changes that affect the build system or external dependencies (example scopes: gulp, broccoli, npm)
* **chore**: Changes outside of othter categories
* **ci**: Changes to our CI configuration files and scripts (examples: CircleCi, SauceLabs)
* **docs**: Documentation only changes
* **feat**: A new feature
* **fix**: A bug fix
* **perf**: A code change that improves performance
* **refactor**: A code change that neither fixes a bug nor adds a feature
* **test**: Adding missing tests or correcting existing tests

##### Scope
The scope should be the name of the project package affected (as perceived by the person reading the changelog generated from commit messages).

The following is the list of supported scopes:

* `vr`
* `vrdr`
* `bfdr`
* `canary`
* none/empty string: useful for `test` and `refactor` changes that are done across all packages (e.g. `test: add missing unit tests`) and for docs changes that are not related to a specific package (e.g. `docs: fix typo in tutorial`).


##### Summary

Use the summary field to provide a succinct description of the change:

* use the imperative, present tense: "change" not "changed" nor "changes"
* don't capitalize the first letter
* no dot (.) at the end


#### <a name="commit-body"></a>Commit Message Body

Just as in the summary, use the imperative, present tense: "fix" not "fixed" nor "fixes".

Explain the motivation for the change in the commit message body. This commit message should explain _why_ you are making the change.
You can include a comparison of the previous behavior with the new behavior in order to illustrate the impact of the change.


#### <a name="commit-footer"></a>Commit Message Footer

The footer can contain information about breaking changes and deprecations and is also the place to reference GitHub issues, Jira tickets, and other PRs that this commit closes or is related to.
For example:

```
BREAKING CHANGE: <breaking change summary>
<BLANK LINE>
<breaking change description + migration instructions>
<BLANK LINE>
<BLANK LINE>
Fixes #<issue number>
```

or

```
DEPRECATED: <what is deprecated>
<BLANK LINE>
<deprecation description + recommended update path>
<BLANK LINE>
<BLANK LINE>
Closes #<pr number>
```

Breaking Change section should start with the phrase "BREAKING CHANGE: " followed by a summary of the breaking change, a blank line, and a detailed description of the breaking change that also includes migration instructions.

Similarly, a Deprecation section should start with "DEPRECATED: " followed by a short description of what is deprecated, a blank line, and a detailed description of the deprecation that also mentions the recommended update path.


### Revert commits

If the commit reverts a previous commit, it should begin with `revert: `, followed by the header of the reverted commit.

The content of the commit message body should contain:

- information about the SHA of the commit being reverted in the following format: `This reverts commit <SHA>`,
- a clear description of the reason for reverting the commit message.


## Migration notes

- default branch is `main`
- manual project version advancements (in *.csproj) for any/all applicable projects with each commit
- publish to NuGet automatically following version bump
- [Task](https://taskfile.dev/) is enabled during devcontainer build

## Code Coverage Report
Pull requests will automatically include code coverage checks. To get a full report locally, install the [coverlet.collector](https://github.com/coverlet-coverage/coverlet) to generate a code coverage xml file (you may additionally install coverlet.msbuild as well). To generate a readable report, install [ReportGenerator](https://github.com/danielpalme/ReportGenerator):

- `dotnet tool install -g dotnet-reportgenerator-globaltool` to install report generator global tool (accessible across projects)
- `dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools` for installing the same package in a "tools" directory within a project
- `dotnet new tool-manifest` to create a tool manifest file within a project
- `dotnet tool install dotnet-reportgenerator-globaltool` for installing without `-g` global flag, configuring it within a project

To generate the report, coverlet must first evaluate code coverage (`dotnet test /p:CollectCoverage=true`) and use XPlat coverage tool to track execution (`dotnet test --collect:"XPlat Code Coverage"`). The generated coverage.cobertura.xml file can then be used to create a readable html report using ReportGenerator (`dotnet reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html` - `-reports:coverage.cobertura.xml` indicates the path to the xml coverage information and `-targetdir:coveragereport` indicates where the generated report will appear).
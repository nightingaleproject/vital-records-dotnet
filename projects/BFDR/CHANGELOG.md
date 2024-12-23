# Changelog

All notable changes to this project will be documented in this file. See [commit-and-tag-version](https://github.com/absolute-version/commit-and-tag-version) for commit guidelines.

<a name="1.0.0-preview.10"></a>
## [1.0.0-preview.10](https://github.com/nightingaleproject/vital-records-dotnet/commit/e5d39978b91a26b4460d68f423a12d5159f515d0) (2024-12-23)

### Features
* Add message header validation

<a name="1.0.0-preview.9"></a>
## [1.0.0-preview.9](https://github.com/nightingaleproject/vital-records-dotnet/commit/ba4cd43e081ede28959f0e9d3070b59644943953) (2024-11-05)

### Features
* Add FetalDeathRecord to the BFDR library
* IJE to FHIR and FHIR to IJE translation for fetal death records
* FHIR messaging support for fetal death messaging types
* Alignment with BFDR STU 2.0 IG

<a name="1.0.0-preview.8"></a>
## [1.0.0-preview.8](https://github.com/nightingaleproject/vital-records-dotnet/commit/73693cc824b0a8e5b666743204978d42b3de4bd9) (2024-10-08)

### Features
* Add IG version to FHIR message header 

### Fixes
* Fix demographic coding message fhir to json conversion to include the record bundle
* Fix the birth year in the connectathon test records
* Fix partial date implementation to align with IG

<a name="1.0.0-preview.7"></a>
## [1.0.0-preview.7](https://github.com/nightingaleproject/vital-records-dotnet/commit/6884361c8d9b3df8b22f334c6d04f1e39e566b82) (2024-08-08)

### Features
* Update Connectathon test cases to use valid year of birth

### Fixes
* Update PaternityAcknowledgementSigned to use correct mapping

<a name="1.0.0-preview.6"></a>
## [1.0.0-preview.6](https://github.com/nightingaleproject/vital-records-dotnet/commit/6884361c8d9b3df8b22f334c6d04f1e39e566b82) (2024-08-06)

### Features
* Add support for null connectathon test case in Canary

<a name="1.0.0-preview.5"></a>
## [1.0.0-preview.5](https://github.com/nightingaleproject/vital-records-dotnet/commit/db5765b2710016d4b3fd9c80e9e27227503376e3) (2024-07-30)

### Features
* Add code coverage to git workflows

### Fixes
* Fix handling of parent ethnicity literls
* Add coded race and ethnicity for parent

<a name="1.0.0-preview.4"></a>
## [1.0.0-preview.4](https://github.com/nightingaleproject/vital-records-dotnet/commit/2380dac3c4a2988fd254a30e02806d6420f1a1d4) (2024-07-11)

### Features
* Add support for Explicit Absence "8" inputs for IJE fields YOPO, MOPO, MLLB, YLLB, APGAR10, DOFP_XX from VitalRecord 1.0.0-preview.2

### Fixes
* Fix tests for basic birth record
* Update README with refactored class names and bfdr package info

<a name="1.0.0-preview.3"></a>
## [1.0.0-preview.3](https://github.com/nightingaleproject/vital-records-dotnet/commit/6dd85fad045cadf988fdb8645bb33b2a905f109a) (2024-05-24)

### Fixes
* Removes extraneous elements from PartialDateTimes.
* Adds support for 'temp-unknown' elements in PartialDateTimes when building a date extension.
* Removes validation of PartialDateTimes since, in VR, elements now have a [0..1] cardinality instead of [1..1].
* Fixes mixed-up Profile URLs and Section Codes for certain fields in BFDR.


<a name="1.0.0-preview.2"></a>
## [1.0.0-preview.2](https://github.com/nightingaleproject/vital-records-dotnet/commit/725088c0632eff716e0e865b07014af595b99ca3) (2024-05-03)


### Fixes

* add versin file ([58b8c1a](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/58b8c1ab7ea4fd1260ccf1d608a336d7c43a1ee3))
* correction to weight editflag codes ([40691e4](https://github.com/nightingaleproject/vital-records-dotnet/commit/40691e4631e436e3a2a20c5d0ed1d1a74ec94c13))
* renaming BFDR.messaging file ([725088c](https://github.com/nightingaleproject/vital-records-dotnet/commit/725088c0632eff716e0e865b07014af595b99ca33))

# Changelog

All notable changes to this project will be documented in this file. See [commit-and-tag-version](https://github.com/absolute-version/commit-and-tag-version) for commit guidelines.

<a name="1.0.0-preview.3"></a>


### Features
* Adds support for Explicit Absence "8" inputs for IJE fields YOPO, MOPO, MLLB, YLLB, APGAR10, DOFP_XX

### Fixes
* Removes extraneous elements from PartialDateTimes.
* Adds support for 'temp-unknown' elements in PartialDateTimes when building a date extension.
* Removes validation of PartialDateTimes since, in VR, elements now have a [0..1] cardinality instead of [1..1].
* Fixes mixed-up Profile URLs and Section Codes for certain fields in BFDR.


<a name="1.0.0-preview.2"></a>
## [1.0.0-preview.2](https://github.com/nightingaleproject/vital-records-dotnet/commit/725088c0632eff716e0e865b07014af595b99ca3) (2024-05-03)


### Bug Fixes

* add versin file ([58b8c1a](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/58b8c1ab7ea4fd1260ccf1d608a336d7c43a1ee3))
* correction to weight editflag codes ([40691e4](https://github.com/nightingaleproject/vital-records-dotnet/commit/40691e4631e436e3a2a20c5d0ed1d1a74ec94c13))
* renaming BFDR.messaging file ([725088c](https://github.com/nightingaleproject/vital-records-dotnet/commit/725088c0632eff716e0e865b07014af595b99ca33))
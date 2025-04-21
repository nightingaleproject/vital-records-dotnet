# Changelog

All notable changes to this project will be documented in this file. See [commit-and-tag-version](https://github.com/absolute-version/commit-and-tag-version) for commit guidelines.

<a name="1.0.0-preview.7"></a>
## [1.0.0-preview.7](https://github.com/nightingaleproject/vital-records-dotnet/commit/c8c1bdab07c4fbe50cdec91e4a83569ed0adc1b6) (2025-04-18)

### Features
* Add void flag for birth and fetal death void messages

### Fixes
* Fix handling of XX, TS, TT, and ZZ jurisdictions in test examples
* Fix how sections are intialized and populated in the composition

<a name="1.0.0-preview.6"></a>
## [1.0.0-preview.6](https://github.com/nightingaleproject/vital-records-dotnet/commit/34c63730c52913170ad01d0d25782387190e4d1e) (2025-01-30)

### Features
* Update util functions to use latest urls
* Regenerate code systems, mappings, and urls to support VRDR STU 3.0 and BFDR 2.0

### Fixes
* Fix time component handling for partial date times

<a name="1.0.0-preview.5"></a>
## [1.0.0-preview.5](https://github.com/nightingaleproject/vital-records-dotnet/commit/e5d39978b91a26b4460d68f423a12d5159f515d0) (2024-12-23)

### Features
* Add message header validation

<a name="1.0.0-preview.4"></a>
## [1.0.0-preview.4](https://github.com/nightingaleproject/vital-records-dotnet/commit/ba4cd43e081ede28959f0e9d3070b59644943953#diff-7b854a1e3a0d54f7ed17b2542764e52a452d596f0827084fdf95084a40fde76d) (2024-11-05)

### Features
* Update VRCL code systems, mappings, value sets, and urls to align with VRCL STU 2.0
* Add support for presence to IJE fields
* Add helper functions for FHIR date time fields
* Add helper functions to get and set physical location fields
* Add helper functions to get and set encounters

<a name="1.0.0-preview.3"></a>
## [1.0.0-preview.3](https://github.com/nightingaleproject/vital-records-dotnet/commit/db5765b2710016d4b3fd9c80e9e27227503376e3) (2024-07-30)

### Features
* Add test for state text
* Update race and ethnicity

<a name="1.0.0-preview.2"></a>
## [1.0.0-preview.2](https://github.com/nightingaleproject/vital-records-dotnet/commit/c4a19c02a13452d4d498b4aa3354ae7264a2cad8) (2024-07-09)

### Features
* add helper function for bfdr fields where all 8s represents unknown ([6dd85fa](https://github.com/nightingaleproject/vital-records-dotnet/commit/6dd85fad045cadf988fdb8645bb33b2a905f109a))
* remove extraneous FHIR elements in partial dates and add temp-unknowns where necessary ([6dd85fa](https://github.com/nightingaleproject/vital-records-dotnet/commit/6dd85fad045cadf988fdb8645bb33b2a905f109a))

<a name="1.0.1-preview"></a>
## [1.0.1-preview](https://github.com/nightingaleproject/vital-records-dotnet/commit/76944eb9ba1fcb010e96bcab6313ede7ad78e8f3) (2024-05-12)

### Feautures
* add support for natality bfdr-library and canary integration ([c54b85a](https://github.com/nightingaleproject/vital-records-dotnet/commit/c54b85a4a29c51e363adc092d4d8b2ed5d764ec4))

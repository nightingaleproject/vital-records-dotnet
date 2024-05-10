# Changelog

All notable changes to this project will be documented in this file. See [commit-and-tag-version](https://github.com/absolute-version/commit-and-tag-version) for commit guidelines.

## [1.0.0-preview.3](https://github.com/nightingaleproject/vital-record-dotnet-demo/compare/e3fb04b330e97b909cc034d9f42e11c48d489fd1...23.4.0) (2024-05-10)


### Features
* BFDR Test Connectathon records are now present and supported for record and message testing in Canary.
* Adds support for Explicit Absence "8" inputs for IJE fields YOPO, MOPO, MLLB, YLLB, APGAR10, DOFP_XX

### Fixes
* Removes extraneous elements from PartialDateTimes.
* Adds support for 'temp-unknown' elements in PartialDateTimes when building a date extension.
* Fixes mixed-up Profile URLs and Section Codes for certain fields in BFDR.


## [23.4.0](https://github.com/nightingaleproject/vital-record-dotnet-demo/compare/e3fb04b330e97b909cc034d9f42e11c48d489fd1...23.4.0) (2023-11-06)


### Features

* add a ([8650848](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/8650848bb9e5e03f93b94f47dc0a9618e125c4da))
* add setter/getter for Decedents gender ([#482](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/482)) ([68c8e03](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/68c8e033d690de05be162091bc6d2af0c22abcee))
* allow retrieval of all Connectathon records ([a5d5f07](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/a5d5f07a601cf33fe31872e16a2d250e5e727a76))
* sync pregnancy status vealue set with IG ([61d84c6](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/61d84c6361b1760d847bcf97d263e5fdba6c0771))


### Bug Fixes

* add version file ([58b8c1a](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/58b8c1ab7ea4fd1260ccf1d608a336d7c43a1ee3))
* adding missing place of injury and description ([#524](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/524)) ([6c90377](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/6c90377cc7a942211dda20079e53321c51e29c97))
* alias flag on fhir message to 0 (or 1) from blank or null ([#513](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/513)) ([0407051](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/040705121b126b945c1e5659928e9c4174a135b5))
* another letter ([e0420eb](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/e0420eb81f7e61f9bd247696e10fb9b49496993a))
* change BaseMessage.MessageBundle accessibility to public get, protected set ([72ae122](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/72ae122430a6465699fd59006475b2306ddf978c))
* commit message ([f0b11ec](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/f0b11ece197f70bff5c20e04141110b4d4e55fed))
* correct typo ([9b4ad75](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/9b4ad75c469719c60f330732991b9320274e964e))
* correct-position-error-between-jurisidiction-and-output-directory… ([#473](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/473)) ([ccbc98d](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/ccbc98d118233e1dfe2de61efe962e73810b188e))
* default decedent gender to unknown ([#529](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/529)) ([172d2b5](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/172d2b5017c40c7443d8b29c4570bbcb3f934275))
* ensure correct FHIR date of death and injury handling ([#484](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/484)) ([991ae30](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/991ae30a9e852b23bbfd85f8b04f8d0f4f5f740e))
* expected age at time of death for test record [#3](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/3) ([e3fb04b](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/e3fb04b330e97b909cc034d9f42e11c48d489fd1))
* handling of missing decedents last name LNAME ([#494](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/494)) ([b5d0c30](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/b5d0c309e159fc3b76b00e4e2c4d22fb4839eb04))
* make update ([335dfb4](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/335dfb4e8d11b76da32463cc03fc2a73386302b8))
* partial date time handling of day, month and year ([#496](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/496)) ([3e5b1d3](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/3e5b1d3fb8fc9bbafd406e7bf6fd4dc63a9af31a))
* partialDate and partialDateTime validation ([#492](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/492)) ([5e7e932](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/5e7e9326cd97d0c5c4beab751ba0ea3030e1c8e9))
* sync VOID message block_count property with messaging IG ([731db5b](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/731db5b004c8091031e979cda1d23e834e499f2d))
* void flag on fhir message to 0 (or 1) from blank or null ([#508](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/508)) ([33f3efb](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/33f3efbbcdb59d76ad90fd8baa34c4b47d421fcd))
* **VRDR:** minor edit ([f3f1d55](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/f3f1d55199407a0cc4934ed4a553c56bcfd065e3))
* **VRDR:** update something ([1feef6b](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/1feef6b663d94a3fe6c145f7b3596765256254e1))
* working directory ([273eb34](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/273eb34ecab2d2686add18356e4b1fa5dcd913ad))
* yes-no value set url ([#456](https://github.com/nightingaleproject/vital-record-dotnet-demo/issues/456)) ([bf042dc](https://github.com/nightingaleproject/vital-record-dotnet-demo/commit/bf042dcedfa3b883582741e8bb71c8b93155a0b9))

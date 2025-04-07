# This script generates synthetic IJE mortality records, intended to be realistic enough to use in a test
# environment but not realistic enough to use for all purposes (e.g., an attempt is made to have reasonable
# ages and cause of death literals but unrealistic records will likely be generated)

# Usage: ruby generate_synthetic_ije_records.rb <year> <number-of-records> <optional-jurisdiction>

# We use faker for generation of synthetic values for common data types
require 'faker'

# We use our own fake values for some fields
require './libraries/ije_faker'

# We use the common IJE format definition
require './libraries/ije_format'

year = ARGV.shift.to_i
if !year.to_s.match(/^[0-9]{4}$/)
  raise "First argument should be a valid 4 digit year"
end

count = ARGV.shift
if !count || !count.match(/^[0-9]+$/)
  raise "Second argument should be a valid number of records to generate"
end
count = count.to_i

jurisdiction = ARGV.shift
if jurisdiction && !jurisdiction.to_s.match(/^[A-Z]{2}$/)
  raise "Third (optional) argument must be a jurisdiction idenfifier if present"
end

# Utility method used to select reasonable race values, set up to handle the sequential approach of setting
# individual race fields; note: race distribution does not match US race distribution
def race_value_selector(ije, race_field_number)
  # Given the field to be set, retrieve all the previous field values
  previous_field_values = (1...race_field_number).to_a.map { |n| ije.send("RACE#{n}") }
  # We care about the number that are set to some value and the number that are set to yes
  number_set = previous_field_values.length
  number_yes = previous_field_values.select { |v| v == 'Y' }.length
  # We know there are 15 fields, and we want there to be a 0.1% chance that 0 are picked, a 5% chance that 2
  # are picked, a 1% chance that 3 are picked, and otherwise 1 is picked
  case number_yes
  when 0
    # Cumulative 99.9% chance that (at least) one of the remaining fields should be set
    return rand < (0.999 / (15 - number_set)) ? 'Y' : 'N'
  when 1
    # Cumulative 6% chance that (at least) one of the remaining fields should be set
    return rand < (0.06 / (15 - number_set)) ? 'Y' : 'N'
  when 2
    # Cumulative 1% chance that (at least) one of the remaining fields should be set
    return rand < (0.01 / (15 - number_set)) ? 'Y' : 'N'
  else
    # Don't set more than 2 fields
    return 'N'
  end
end

# For each IJE field we define a generator that generates valid values for that field, which may depend on
# values that other fields are set to

ije_generator = {
  # Date of Death--Year
  DOD_YR: -> (ije) { raise "This field should be preset and not generated" },
  # State, U.S. Territory or Canadian Province of Death - code
  DSTATE: -> (ije) { IJEFaker.jurisdiction },
  # Certificate Number
  FILENO: -> (ije) { IJEFaker.certificate_number(ije.DSTATE, ije.DOD_YR) }, # Based on jurisdiction and year
  # Void flag
  VOID: -> (ije) { '0' }, # Valid record
  # Auxiliary State file number
  AUXNO: -> (ije) { '' }, # Intentionally left blank
  # Source flag: paper/electronic
  MFILED: -> (ije) { '0' }, # All set to "electronic"
  # Decedent's Legal Name--Given
  GNAME: -> (ije) { IJEFaker.given_name(ije.SEX) }, # Name generated based on sex
  # Decedent's Legal Name--Middle
  MNAME: -> (ije) { ije.DMIDDLE[0] }, # Take first character of middle name
  # Decedent's Legal Name--Last
  LNAME: -> (ije) { IJEFaker.last_name },
  # Decedent's Legal Name--Suffix
  SUFF: -> (ije) { '' }, # No suffix
  # Decedent's Legal Name--Alias
  ALIAS: -> (ije) { '' }, # No alias
  # Father's Surname
  FLNAME: -> (ije) {
    # Set based on sex and marital status; this makes an over-simplistic assumption about name changes
    if ije.SEX == 'F' && ['M', 'A', 'W'].include?(ije.MARITAL)
      IJEFaker.last_name
    else
      ije.LNAME
    end
  },
  # Sex
  SEX: -> (ije) {
    # Generate the sex based on the COD sequence so that it's reasonable
    sequence = [:COD1A, :COD1B, :COD1C, :COD1D, :OTHERCONDITION].map { |f| ije.send(f) }
    IJEFaker.value_for_sequence(:sex, sequence)
  },
  # Sex--Edit Flag
  SEX_BYPASS: -> (ije) { '0' }, # Edit passed
  # Social Security Number
  SSN: -> (ije) { Faker::IdNumber.unique.valid.gsub('-', '') }, # NOTE: Gets slow with many records
  # Decedent's Age--Type
  AGETYPE: -> (ije) { '1' }, # Age type is always years
  # Decedent's Age--Units
  AGE: -> (ije) {
    # Generate the age based on the COD sequence so that it's reasonable
    sequence = [:COD1A, :COD1B, :COD1C, :COD1D, :OTHERCONDITION].map { |f| ije.send(f) }
    IJEFaker.value_for_sequence(:age, sequence)
  },
  # Decedent's Age--Edit Flag
  AGE_BYPASS: -> (ije) { '0' }, # Edit passed
  # Date of Birth--Year
  DOB_YR: -> (ije) { ije.DOD_YR.to_i - ije.AGE.to_i }, # Just based on age
  # Date of Birth--Month
  DOB_MO: -> (ije) { ije.DOD_MO }, # Keep simple by making same as month of death
  # Date of Birth--Day
  DOB_DY: -> (ije) { ije.DOD_DY }, # Keep simple by making same as day of death
  # Birthplace--Country
  BPLACE_CNT: -> (ije) { 'US' }, # Default to US
  # State, U.S. Territory or Canadian Province of Birth - code
  BPLACE_ST: -> (ije) {
    # Somewhat likely that state of birth is the same as state of residence, and cannot be YC
    default = ije.STATEC
    IJEFaker.jurisdiction_no_yc(rand < 0.5 ? default : IJEFaker.jurisdiction)
  },
  # Decedent's Residence--City
  CITYC: -> (ije) { IJEFaker.place_code(ije.STATEC, ije.CITYTEXT_R) }, # Look up code
  # Decedent's Residence--County
  COUNTYC: -> (ije) { IJEFaker.county_code(ije.STATEC, ije.CITYTEXT_R) }, # Look up code
  # State, U.S. Territory or Canadian Province of Decedent's residence - code
  STATEC: -> (ije) {
    # More likely that state of death and residence is the same than not, and cannot be YC
    default = ije.DSTATE
    IJEFaker.jurisdiction_no_yc(rand < 0.95 ? default : IJEFaker.jurisdiction)
  },
  # Decedent's Residence--Country
  COUNTRYC: -> (ije) { 'US' }, # Default to US
  # Decedent's Residence--Inside City Limits
  LIMITS: -> (ije) { (['Y'] * 4 + ['N', 'U']).sample },
  # Marital Status
  MARITAL: -> (ije) { IJEFaker.marital_status(ije.AGE) },
  # Marital Status--Edit Flag
  MARITAL_BYPASS: -> (ije) { '0' }, # Edit passed
  # Place of Death
  DPLACE: -> (ije) { ['1', '2', '3', '4', '5', '6', '7', '9'].sample },
  # County of Death Occurrence
  COD: -> (ije) { ije.COUNTYC }, # Make county of death same as county of residence
  # Method of Disposition
  DISP: -> (ije) { ['B', 'C', 'D', 'E', 'R', 'O', 'U'].sample },
  # Date of Death--Month
  DOD_MO: -> (ije) { raise "This field should be preset and not generated" },
  # Date of Death--Day
  DOD_DY: -> (ije) { raise "This field should be preset and not generated" },
  # Time of Death
  TOD: -> (ije) { '%02d%02d' % [rand(0..23), rand(0..59)] }, # Random time in military format
  # Decedent's Education
  DEDUC: -> (ije) {
    case ije.AGE.to_i
    when 0..14 then '1' # 8th grade or less
    when 15..18 then ['1', '2'].sample # through 12th grade; no diploma
    when 19..22 then ['1', '2', '3', '4'].sample # High school graduate or some college
    else ['1', '2', '3', '4', '5', '6', '7', '8', '9'].sample # Anything, including unknown
    end
  },
  # Decedent's Education--Edit Flag
  DEDUC_BYPASS: -> (ije) { '0' }, # Edit passed
  # Decedent of Hispanic Origin?--Mexican
  DETHNIC1: -> (ije) { ['N', 'N', 'N', 'N', 'H', 'U'].sample },
  # Decedent of Hispanic Origin?--Puerto Rican
  DETHNIC2: -> (ije) { ['N', 'N', 'N', 'N', 'H', 'U'].sample },
  # Decedent of Hispanic Origin?--Cuban
  DETHNIC3: -> (ije) { ['N', 'N', 'N', 'N', 'H', 'U'].sample },
  # Decedent of Hispanic Origin?--Other
  DETHNIC4: -> (ije) { ['N', 'N', 'N', 'N', 'H', 'U'].sample },
  # Decedent of Hispanic Origin?--Other, Literal
  DETHNIC5: -> (ije) { '' }, # Blank
  # Decedent's Race--White
  RACE1: -> (ije) { race_value_selector(ije, 1) },
  # Decedent's Race--Black or African American
  RACE2: -> (ije) { race_value_selector(ije, 2) },
  # Decedent's Race--American Indian or Alaska Native
  RACE3: -> (ije) { race_value_selector(ije, 3) },
  # Decedent's Race--Asian Indian
  RACE4: -> (ije) { race_value_selector(ije, 4) },
  # Decedent's Race--Chinese
  RACE5: -> (ije) { race_value_selector(ije, 5) },
  # Decedent's Race--Filipino
  RACE6: -> (ije) { race_value_selector(ije, 6) },
  # Decedent's Race--Japanese
  RACE7: -> (ije) { race_value_selector(ije, 7) },
  # Decedent's Race--Korean
  RACE8: -> (ije) { race_value_selector(ije, 8) },
  # Decedent's Race--Vietnamese
  RACE9: -> (ije) { race_value_selector(ije, 9) },
  # Decedent's Race--Other Asian
  RACE10: -> (ije) { race_value_selector(ije, 10) },
  # Decedent's Race--Native Hawaiian
  RACE11: -> (ije) { race_value_selector(ije, 11) },
  # Decedent's Race--Guamanian or Chamorro
  RACE12: -> (ije) { race_value_selector(ije, 12) },
  # Decedent's Race--Samoan
  RACE13: -> (ije) { race_value_selector(ije, 13) },
  # Decedent's Race--Other Pacific Islander
  RACE14: -> (ije) { race_value_selector(ije, 14) },
  # Decedent's Race--Other
  RACE15: -> (ije) { race_value_selector(ije, 15) },
  # Decedent's Race--First American Indian or Alaska Native Literal
  RACE16: -> (ije) { '' }, # Blank
  # Decedent's Race--Second American Indian or Alaska Native Literal
  RACE17: -> (ije) { '' }, # Blank
  # Decedent's Race--First Other Asian Literal
  RACE18: -> (ije) { '' }, # Blank
  # Decedent's Race--Second Other Asian Literal
  RACE19: -> (ije) { '' }, # Blank
  # Decedent's Race--First Other Pacific Islander Literal
  RACE20: -> (ije) { '' }, # Blank
  # Decedent's Race--Second Other Pacific Islander Literal
  RACE21: -> (ije) { '' }, # Blank
  # Decedent's Race--First Other Literal
  RACE22: -> (ije) { '' }, # Blank
  # Decedent's Race--Second Other Literal
  RACE23: -> (ije) { '' }, # Blank
  # Decedent's Race--Missing
  RACE_MVR: -> (ije) {
    # See if we answered yes for any
    if (1..14).any? { |v| ije.send("RACE#{v}") == 'Y' }
      ''  # Blank, since race provided
    else
      ['R', 'S', 'C'].sample # One of the allowed reasons
    end
  },
  # Occupation -- Literal (REQUIRED)
  OCCUP: -> (ije) {
    if ije.AGE >= 18
      Faker::Job.title[0, 40]
    else
      'Child'
    end
  },
  # Occupation -- Code (OPTIONAL)
  OCCUPC: -> (ije) { '' }, # Blank
  # Industry -- Literal (REQUIRED)
  INDUST: -> (ije) {
    if ije.AGE > 18
      Faker::Job.field[0, 40]
    else
      'Never Worked'
    end
  },
  # Industry -- Code (OPTIONAL)
  INDUSTC: -> (ije) { '' }, # Blank
  # Infant Death/Birth Linking - birth certificate number
  BCNO: -> (ije) { '' }, # Blank
  # Infant Death/Birth Linking - year of birth
  IDOB_YR: -> (ije) { ije.DOB_YR },
  # Infant Death/Birth Linking - State, U.S. Territory or Canadian Province of Birth - code
  BSTATE: -> (ije) { ije.BPLACE_ST },
  # Occupation -- 4 digit Code (OPTIONAL)
  OCCUPC4: -> (ije) { '' }, # Blank
  # Industry -- 4 digit Code (OPTIONAL)
  INDUSTC4: -> (ije) { '' }, # Blank
  # Date of Registration--Year
  DOR_YR: -> (ije) { ije.DOD_YR }, # Prompt registration
  # Date of Registration--Month
  DOR_MO: -> (ije) { ije.DOD_MO }, # Prompt registration
  # Date of Registration--Day
  DOR_DY: -> (ije) { ije.DOD_DY }, # Prompt registration
  # FILLER 2 for expansion
  FILLER2: -> (ije) { '' }, # Blank
  # Manner of Death
  MANNER: -> (ije) {
    # Generate the manner based on the COD sequence so that it's reasonable
    sequence = [:COD1A, :COD1B, :COD1C, :COD1D, :OTHERCONDITION].map { |f| ije.send(f) }
    IJEFaker.value_for_sequence(:manner, sequence)
  },
  # Intentional Reject
  INT_REJ: -> (ije) { '' }, # Blank
  # Acme System Reject Codes
  SYS_REJ: -> (ije) { '' }, # Blank
  # Place of Injury (computer generated)
  INJPL: -> (ije) { '' }, # Blank
  # Was Autopsy performed
  AUTOP: -> (ije) { },
  # Were Autopsy Findings Available to Complete the Cause of Death?
  AUTOPF: -> (ije) { },
  # Did Tobacco Use Contribute to Death?
  TOBAC: -> (ije) {
    # Generate tobacco use based on the COD sequence so that it's reasonable
    sequence = [:COD1A, :COD1B, :COD1C, :COD1D, :OTHERCONDITION].map { |f| ije.send(f) }
    IJEFaker.value_for_sequence(:tobacco, sequence)
  },
  # Pregnancy
  PREG: -> (ije) {
    # Generate pregnancy status based on sex of decedent, age, and COD sequence so that it's reasonable
    if (ije.SEX == 'F' && ije.AGE >= 18)
      sequence = [:COD1A, :COD1B, :COD1C, :COD1D, :OTHERCONDITION].map { |f| ije.send(f) }
      IJEFaker.value_for_sequence(:pregnancy, sequence)
    else
      8 # Not applicable
    end
  },
  # If Female--Edit Flag: From EDR only
  PREG_BYPASS: -> (ije) { '0' }, # Edit passed
  # Date of injury--month
  DOI_MO: -> (ije) { ije.MANNER == 'A' ? ije.DOD_MO : '' }, # Use DOD if it's an injury
  # Date of injury--day
  DOI_DY: -> (ije) { ije.MANNER == 'A' ? ije.DOD_DY : '' }, # Use DOD if it's an injury
  # Date of injury--year
  DOI_YR: -> (ije) { ije.MANNER == 'A' ? ije.DOD_YR : '' }, # Use DOD if it's an injury
  # Time of injury
  TOI_HR: -> (ije) { ije.MANNER == 'A' ? ije.TOD : '' }, # Use TOD if it's an injury
  # Injury at work
  WORKINJ: -> (ije) { '' }, # Blank
  # Title of Certifier
  CERTL: -> (ije) {
    # Generate the certifier type based on the COD sequence so that it's reasonable
    sequence = [:COD1A, :COD1B, :COD1C, :COD1D, :OTHERCONDITION].map { |f| ije.send(f) }
    IJEFaker.value_for_sequence(:certifier_type, sequence)
  },
  # Activity at time of death (computer generated)
  INACT: -> (ije) { '' }, # Blank
  # Auxiliary State file number
  AUXNO2: -> (ije) { '' }, # Blank
  # State Specific Data
  STATESP: -> (ije) { '' }, # Blank
  # Surgery Date--month
  SUR_MO: -> (ije) { '' }, # Blank
  # Surgery Date--day
  SUR_DY: -> (ije) { '' }, # Blank
  # Surgery Date--year
  SUR_YR: -> (ije) { '' }, # Blank
  # Time of Injury Unit
  TOI_UNIT: -> (ije) { ije.MANNER == 'A' ? 'M' : '' }, # Military time if it's an injury
  # For possible future change in transax
  BLANK1: -> (ije) { '' }, # Blank
  # Decedent ever served in Armed Forces?
  ARMEDF: -> (ije) {
    # Generate a plausible value based on age
    if ije.AGE >= 18
      (['Y'] * 1 + ['N'] * 15 + ['U'] * 4).sample
    else
      'N'
    end
  },
  # Death Institution name
  DINSTI: -> (ije) { '' }, # Blank
  # Long String address for place of death
  ADDRESS_D: -> (ije) { '' }, # Blank
  # Place of death. Street number
  STNUM_D: -> (ije) { '' }, # Blank
  # Place of death. Pre Directional
  PREDIR_D: -> (ije) { '' }, # Blank
  # Place of death. Street name
  STNAME_D: -> (ije) { '' }, # Blank
  # Place of death. Street designator
  STDESIG_D: -> (ije) { '' }, # Blank
  # Place of death. Post Directional
  POSTDIR_D: -> (ije) { '' }, # Blank
  # Place of death. City or Town name
  CITYTEXT_D: -> (ije) { '' }, # Blank
  # Place of death. State name literal
  STATETEXT_D: -> (ije) { '' }, # Blank
  # Place of death. Zip code
  ZIP9_D: -> (ije) { '' }, # Blank
  # Place of death. County of Death
  COUNTYTEXT_D: -> (ije) { '' }, # Blank
  # Place of death. City FIPS code
  CITYCODE_D: -> (ije) { '' }, # Blank
  # Place of death. Longitude
  LONG_D: -> (ije) { '' }, # Blank
  # Place of Death. Latitude
  LAT_D: -> (ije) { '' }, # Blank
  # Decedent's spouse living at decedent's DOD?
  SPOUSELV: -> (ije) {
    # Check on marital status
    if ije.MARITAL == 'M' || ije.MARITAL == 'A'
      [1, 2].sample # Yes or no
    else
      8 # Unmarried
    end
  },
  # Spouse's First Name
  SPOUSEF: -> (ije) {
    if ije.MARITAL == 'M' || ije.MARITAL == 'A'
      # Does not try to cover all possibilities
      IJEFaker.given_name(ije.SEX == 'F' ? 'M' : 'F')
    else
      ''
    end
  },
  # Husband's Surname/Wife's Maiden Last Name
  SPOUSEL: -> (ije) {
    if ije.MARITAL == 'M' || ije.MARITAL == 'A'
      IJEFaker.last_name
    else
      ''
    end
  },
  # Decedent's Residence - Street number
  STNUM_R: -> (ije) { rand(1..999).to_s },
  # Decedent's Residence - Pre Directional
  PREDIR_R: -> (ije) { '' }, # Blank
  # Decedent's Residence - Street name
  STNAME_R: -> (ije) { Faker::Address.street_name[0, 28] },
  # Decedent's Residence - Street designator
  STDESIG_R: -> (ije) { '' }, # Blank
  # Decedent's Residence - Post Directional
  POSTDIR_R: -> (ije) { '' }, # Blank
  # Decedent's Residence - Unit or apt number
  UNITNUM_R: -> (ije) { '' }, # Blank
  # Decedent's Residence - City or Town name
  CITYTEXT_R: -> (ije) { IJEFaker.place_name(ije.STATEC)[0, 28] }, # Pick a random "place" in the state
  # Decedent's Residence - ZIP code
  ZIP9_R: -> (ije) { '' }, # Blank; could be done with an extensive lookup
  # Decedent's Residence - County
  COUNTYTEXT_R: -> (ije) { IJEFaker.county_name(ije.STATEC, ije.CITYTEXT_R) }, # Look up name
  # Decedent's Residence - State name
  STATETEXT_R: -> (ije) { IJEFaker.jurisdiction_state_name(ije.STATEC) }, # Look up name
  # Decedent's Residence - COUNTRY name
  COUNTRYTEXT_R: -> (ije) { 'United States' },
  # Long string address for decedent's place of residence same as above but allows states to choose the way they capture information.
  ADDRESS_R: -> (ije) { "#{ije.STNUM_R} #{ije.STNAME_R}" },
  # Old NCHS residence state code
  RESSTATE: -> (ije) { '' }, # Blank
  # Old NCHS residence city/county combo code
  RESCON: -> (ije) { '' }, # Blank
  # Hispanic -
  DETHNICE: -> (ije) { '' }, # Blank
  # Race -
  NCHSBRIDGE: -> (ije) { '' }, # Blank
  # Hispanic - old NCHS single ethnicity codes
  HISPOLDC: -> (ije) { '' }, # Blank
  # Race - old NCHS single race codes
  RACEOLDC: -> (ije) { '' }, # Blank
  # Hispanic Origin - Specify
  HISPSTSP: -> (ije) { '' }, # Blank
  # Race - Specify
  RACESTSP: -> (ije) { '' }, # Blank
  # Middle Name of Decedent
  DMIDDLE: -> (ije) { IJEFaker.middle_name }, # This is the full middle name
  # Father's First Name
  DDADF: -> (ije) { IJEFaker.given_name('M') },
  # Father's Middle Name
  DDADMID: -> (ije) { IJEFaker.middle_name },
  # Mother's First Name
  DMOMF: -> (ije) { IJEFaker.given_name('F') },
  # Mother's Middle Name
  DMOMMID: -> (ije) { IJEFaker.middle_name },
  # Mother's Maiden Surname
  DMOMMDN: -> (ije) { IJEFaker.last_name },
  # Was case Referred to Medical Examiner/Coroner?
  REFERRED: -> (ije) { ije.CERTL == 'M' ? 'Y' : 'N' }, # Based on cerfifier type
  # Place of Injury- literal
  POILITRL: -> (ije) { '' }, # Blank
  # Describe How Injury Occurred
  HOWINJ: -> (ije) { '' }, # Blank
  # If Transportation Accident, Specify
  TRANSPRT: -> (ije) { '' }, # Blank; could be added for certain cause of death types
  # County of Injury - literal
  COUNTYTEXT_I: -> (ije) { ije.MANNER == 'A' ? IJEFaker.county_name(ije.STATECODE_I, ije.CITYTEXT_I) : '' },
  # County of Injury code
  COUNTYCODE_I: -> (ije) { ije.MANNER == 'A' ? IJEFaker.county_code(ije.STATECODE_I, ije.CITYTEXT_I) : '' },
  # Town/city of Injury - literal
  CITYTEXT_I: -> (ije) { ije.MANNER == 'A' ? IJEFaker.place_name(ije.STATECODE_I)[0, 28] : '' },
  # Town/city of Injury code
  CITYCODE_I: -> (ije) { ije.MANNER == 'A' ? IJEFaker.place_code(ije.STATECODE_I, ije.CITYTEXT_I) : '' },
  # State, U.S. Territory or Canadian Province of Injury - code
  STATECODE_I: -> (ije) { ije.MANNER == 'A' ? IJEFaker.jurisdiction_no_yc(ije.DSTATE) : '' }, # State of death, not YC
  # Place of injury. Longitude
  LONG_I: -> (ije) { '' }, # Blank
  # Place of injury. Latitude
  LAT_I: -> (ije) { '' }, # Blank
  # Old NCHS education code if collected - receiving state will recode as they prefer
  OLDEDUC: -> (ije) { '' }, # Blank
  # Replacement Record -- suggested codes
  REPLACE: -> (ije) { '' }, # Blank
  # Cause of Death Part I Line a
  COD1A: -> (ije) { raise "This field should be preset and not generated" },
  # Cause of Death Part I Interval, Line a
  INTERVAL1A: -> (ije) { raise "This field should be preset and not generated" },
  # Cause of Death Part I Line b
  COD1B: -> (ije) { raise "This field should be preset and not generated" },
  # Cause of Death Part I Interval, Line b
  INTERVAL1B: -> (ije) { raise "This field should be preset and not generated" },
  # Cause of Death Part I Line c
  COD1C: -> (ije) { raise "This field should be preset and not generated" },
  # Cause of Death Part I Interval, Line c
  INTERVAL1C: -> (ije) { raise "This field should be preset and not generated" },
  # Cause of Death Part I Line d
  COD1D: -> (ije) { raise "This field should be preset and not generated" },
  # Cause of Death Part I Interval, Line d
  INTERVAL1D: -> (ije) { raise "This field should be preset and not generated" },
  # Cause of Death Part II
  OTHERCONDITION: -> (ije) { raise "This field should be preset and not generated" },
  # Decedent's Maiden Name
  DMAIDEN: -> (ije) { ije.FLNAME }, # Set to father's surname
  # Decedent's Birth Place City - Code
  DBPLACECITYCODE: -> (ije) { '' }, # Blank
  # Decedent's Birth Place City - Literal
  DBPLACECITY: -> (ije) { '' }, # Blank
  # Spouse's Middle Name
  SPOUSEMIDNAME: -> (ije) {
    if ije.MARITAL == 'M' || ije.MARITAL == 'A'
      IJEFaker.middle_name
    else
      ''
    end
  },
  # Spouse's Suffix
  SPOUSESUFFIX: -> (ije) { '' }, # Blank
  # Father's Suffix
  FATHERSUFFIX: -> (ije) { '' }, # Blank
  # Mother's Suffix
  MOTHERSSUFFIX: -> (ije) { '' }, # Blank
  # Informant's Relationship
  INFORMRELATE: -> (ije) { '' }, # Blank
  # State, U.S. Territory or Canadian Province of Disposition - code
  DISPSTATECD: -> (ije) { '' }, # Blank
  # Disposition State or Territory - Literal
  DISPSTATE: -> (ije) { '' }, # Blank
  # Disposition City - Code
  DISPCITYCODE: -> (ije) { '' }, # Blank
  # Disposition City - Literal
  DISPCITY: -> (ije) { '' }, # Blank
  # Funeral Facility Name
  FUNFACNAME: -> (ije) { '' }, # Blank
  # Funeral Facility - Street number
  FUNFACSTNUM: -> (ije) { '' }, # Blank
  # Funeral Facility - Pre Directional
  FUNFACPREDIR: -> (ije) { '' }, # Blank
  # Funeral Facility - Street name
  FUNFACSTRNAME: -> (ije) { '' }, # Blank
  # Funeral Facility - Street designator
  FUNFACSTRDESIG: -> (ije) { '' }, # Blank
  # Funeral Facility - Post Directional
  FUNPOSTDIR: -> (ije) { '' }, # Blank
  # Funeral Facility - Unit or apt number
  FUNUNITNUM: -> (ije) { '' }, # Blank
  # Long string address for Funeral Facility same as above but allows states to choose the way they capture information.
  FUNFACADDRESS: -> (ije) { '' }, # Blank
  # Funeral Facility - City or Town name
  FUNCITYTEXT: -> (ije) { '' }, # Blank
  # State, U.S. Territory or Canadian Province of Funeral Facility - code
  FUNSTATECD: -> (ije) { '' }, # Blank
  # State, U.S. Territory or Canadian Province of Funeral Facility - literal
  FUNSTATE: -> (ije) { '' }, # Blank
  # Funeral Facility - ZIP
  FUNZIP: -> (ije) { '' }, # Blank
  # Person Pronouncing Date Signed
  PPDATESIGNED: -> (ije) { "#{ije.DOD_MO}#{ije.DOD_DY}#{ije.DOD_YR}" }, # Death date for simplicity
  # Person Pronouncing Time Pronounced
  PPTIME: -> (ije) { ije.TOD }, # Death time for simplicity
  # Certifier's First Name
  # Note: Depending on how the data is used we may want a common set of certifiers per jurisdiction
  CERTFIRST: -> (ije) { IJEFaker.given_name(['M', 'F'].sample) },
  # Certifier's Middle Name
  CERTMIDDLE: -> (ije) { IJEFaker.middle_name },
  # Certifier's Last Name
  CERTLAST: -> (ije) { IJEFaker.last_name },
  # Certifier's Suffix Name
  CERTSUFFIX: -> (ije) { '' }, # Blank
  # Certifier - Street number
  CERTSTNUM: -> (ije) { '' }, # Blank
  # Certifier - Pre Directional
  CERTPREDIR: -> (ije) { '' }, # Blank
  # Certifier - Street name
  CERTSTRNAME: -> (ije) { '' }, # Blank
  # Certifier - Street designator
  CERTSTRDESIG: -> (ije) { '' }, # Blank
  # Certifier - Post Directional
  CERTPOSTDIR: -> (ije) { '' }, # Blank
  # Certifier - Unit or apt number
  CERTUNITNUM: -> (ije) { '' }, # Blank
  # Long string address for Certifier same as above but allows states to choose the way they capture information.
  CERTADDRESS: -> (ije) { '' }, # Blank
  # Certifier - City or Town name
  CERTCITYTEXT: -> (ije) { '' }, # Blank
  # State, U.S. Territory or Canadian Province of Certifier - code
  CERTSTATECD: -> (ije) { '' }, # Blank
  # State, U.S. Territory or Canadian Province of Certifier - literal
  CERTSTATE: -> (ije) { '' }, # Blank
  # Certifier - Zip
  CERTZIP: -> (ije) { '' }, # Blank
  # Certifier Date Signed
  CERTDATE: -> (ije) { "#{ije.DOD_MO}#{ije.DOD_DY}#{ije.DOD_YR}" }, # Death date for simplicity
  # Date Filed
  FILEDATE: -> (ije) { "#{ije.DOD_MO}#{ije.DOD_DY}#{ije.DOD_YR}" }, # Death date for simplicity
  # State, U.S. Territory or Canadian Province of Injury - literal
  STINJURY: -> (ije) { '' }, # Blank
  # State, U.S. Territory or Canadian Province of Birth - literal
  STATEBTH: -> (ije) { '' }, # Blank
  # Country of Death - Code
  DTHCOUNTRYCD: -> (ije) { '' }, # Blank
  # Country of Death - Literal
  DTHCOUNTRY: -> (ije) { '' }, # Blank
  # SSA State Source of Death
  SSADTHCODE: -> (ije) { '' }, # Blank
  # SSA Foreign Country Indicator
  SSAFOREIGN: -> (ije) { '' }, # Blank
  # SSA EDR Verify Code
  SSAVERIFY: -> (ije) { '' }, # Blank
  # SSA Date of SSN Verification
  SSADATEVER: -> (ije) { '' }, # Blank
  # SSA Date of State Transmission
  SSADATETRANS: -> (ije) { '' }, # Blank
  # Marital Descriptor
  MARITAL_DESCRIP: -> (ije) { '' }, # Blank
  # Hispanic Code for Literal
  DETHNIC5C: -> (ije) { '' }, # Blank
  # Blank for One-Byte Field 1
  PLACE1_1: -> (ije) { '' }, # Blank
  # Blank for One-Byte Field 2
  PLACE1_2: -> (ije) { '' }, # Blank
  # Blank for One-Byte Field 3
  PLACE1_3: -> (ije) { '' }, # Blank
  # Blank for One-Byte Field 4
  PLACE1_4: -> (ije) { '' }, # Blank
  # Blank for One-Byte Field 5
  PLACE1_5: -> (ije) { '' }, # Blank
  # Blank for One-Byte Field 6
  PLACE1_6: -> (ije) { '' }, # Blank
  # Blank for Eight-Byte Field 1
  PLACE8_1: -> (ije) { '' }, # Blank
  # Blank for Eight-Byte Field 2
  PLACE8_2: -> (ije) { '' }, # Blank
  # Blank for Eight-Byte Field 3
  PLACE8_3: -> (ije) { '' }, # Blank
  # Blank for Twenty-Byte Field
  PLACE20: -> (ije) { '' }, # Blank
  # Blank for future expansion
  BLANK2: -> (ije) { '' }, # Blank
  # Blank for Jurisdictional Use Only
  BLANK3: -> (ije) { '' }, # Blank
}

# Generate an IJE record by iterating through all the generator fields and setting an IJE record with the
# value generated for each field; we automatically detect dependencies and loop until all dependencies are met

# Utility class to track what methods are called to track dependencies
class DependencyChecker
  attr_reader :methods_called
  def initialize(wrapped_object)
    @wrapped_object = wrapped_object
    @methods_called = []
  end
  def method_missing(method, *args)
    @methods_called |= [method]
    @wrapped_object.send(method, *args)
  end
  def self.check(lambda, wrapped_object)
    dependency_checker = self.new(wrapped_object)
    lambda.call(dependency_checker) rescue nil # Ignore exceptions for this call
    return dependency_checker.methods_called
  end
end

# Generate a number of records based on the count provided
count.times do |record_number|

  # Create the record we'll populate
  ije_record = IJEFormat.new

  # Preset some fields based on input parameters
  ije_record.DOD_YR = year
  ije_record.DSTATE = jurisdiction if jurisdiction

  # Assume that we want to spread all of the desired records evenly across the year specified, though only up
  # to the current day if it's the current year
  start_date = Date.parse("#{year}-01-01")
  end_date = year.to_i < Date.today.year ? Date.parse("#{year}-12-31") : Date.today
  days_in_year = (end_date - start_date).to_i
  raise "Year cannot be in future" if days_in_year < 1
  current_day = (record_number * days_in_year) / count
  record_date = start_date + current_day
  ije_record.DOD_MO = '%02d' % record_date.month
  ije_record.DOD_DY = '%02d' % record_date.day

  # We set the COD info here since we get all the fields at once since they're related
  sequence = IJEFaker.literal_sequence
  ije_record.COD1A = sequence[0]
  ije_record.COD1B = sequence[1]
  ije_record.COD1C = sequence[2]
  ije_record.COD1D = sequence[3]
  ije_record.OTHERCONDITION = sequence[4]
  intervals = IJEFaker.value_for_sequence(:intervals, sequence)
  ije_record.INTERVAL1A = intervals[0].to_s
  ije_record.INTERVAL1B = intervals[1].to_s
  ije_record.INTERVAL1C = intervals[2].to_s
  ije_record.INTERVAL1D = intervals[3].to_s
  autopsy_information = IJEFaker.value_for_sequence(:autopsy_and_autopsy_available, sequence)
  if autopsy_information && autopsy_information.length > 0
    ije_record.AUTOP = autopsy_information.split('|').first
    ije_record.AUTOPF = autopsy_information.split('|').last
  end

  # Track which fields we've successfully set for tracking dependencies, starting with preset fields above
  fields_set = [:DOD_YR, :DOD_MO, :DOD_DY,
                :COD1A, :COD1B, :COD1C, :COD1D, :OTHERCONDITION,
                :INTERVAL1A, :INTERVAL1B, :INTERVAL1C, :INTERVAL1D]
  fields_set += [:AUTOP, :AUTOPF] if autopsy_information && autopsy_information.length > 0
  fields_set << :DSTATE if jurisdiction

  # We keep track of whether we've made progress so we don't get stuck
  fields_set_last_pass = fields_set.length

  # Iterate until we've set all the fields using the generator
  while fields_set.length < ije_generator.length

    # Set whatever we can on this pass
    ije_generator.each do |field, generator|

      # Skip if we've already set this field
      next if fields_set.include?(field)

      # Call the generator with the dependency checker to determine fields it depends on, except for the
      # FILENO field which we don't want to call twice (not particularly elegant)
      dependencies = if field == :FILENO
                       [:DOD_YR, :DSTATE]
                     else
                       DependencyChecker.check(generator, ije_record)
                     end

      # Skip if there are dependencies that have not been set
      next if (dependencies - fields_set).length > 0

      # Generate the field value, set the record, and note that we've set this field
      value = generator.call(ije_record)
      ije_record.send("#{field}=", value)
      fields_set << field
    end

    # Make sure we're making progress
    if !(fields_set.length > fields_set_last_pass)
      raise "Unable to make progress building record; unset fields: #{(ije_generator.keys - fields_set).join(' ')}"
    end
    fields_set_last_pass = fields_set.length

  end

  puts IJEFormat.write([ije_record])

end

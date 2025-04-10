require 'ije'

class IJEFormat < IJE::FixedLengthRecordFormat
  range 0..3, :DOD_YR # Date of Death--Year
  range 4..5, :DSTATE # State, U.S. Territory or Canadian Province of Death - code
  range 6..11, :FILENO, type: :rjust_zeroes # Certificate Number
  range 12..12, :VOID # Void flag
  range 13..24, :AUXNO # Auxiliary State file number
  range 25..25, :MFILED # Source flag: paper/electronic
  range 26..75, :GNAME # Decedent's Legal Name--Given 
  range 76..76, :MNAME # Decedent's Legal Name--Middle
  range 77..126, :LNAME # Decedent's Legal Name--Last
  range 127..136, :SUFF # Decedent's Legal Name--Suffix
  range 137..137, :ALIAS # Decedent's Legal Name--Alias
  range 138..187, :FLNAME # Father's Surname
  range 188..188, :SEX # Sex
  range 189..189, :SEX_BYPASS # Sex--Edit Flag
  range 190..198, :SSN # Social Security Number
  range 199..199, :AGETYPE # Decedent's Age--Type
  range 200..202, :AGE  # Decedent's Age--Units
  range 203..203, :AGE_BYPASS # Decedent's Age--Edit Flag
  range 204..207, :DOB_YR # Date of Birth--Year
  range 208..209, :DOB_MO # Date of Birth--Month
  range 210..211, :DOB_DY # Date of Birth--Day
  range 212..213, :BPLACE_CNT # Birthplace--Country
  range 214..215, :BPLACE_ST # State, U.S. Territory or Canadian Province of Birth - code
  range 216..220, :CITYC # Decedent's Residence--City
  range 221..223, :COUNTYC # Decedent's Residence--County
  range 224..225, :STATEC # State, U.S. Territory or Canadian Province of Decedent's residence - code
  range 226..227, :COUNTRYC # Decedent's Residence--Country
  range 228..228, :LIMITS # Decedent's Residence--Inside City Limits
  range 229..229, :MARITAL # Marital Status
  range 230..230, :MARITAL_BYPASS # Marital Status--Edit Flag
  range 231..231, :DPLACE # Place of Death
  range 232..234, :COD # County of Death Occurrence
  range 235..235, :DISP # Method of Disposition
  range 236..237, :DOD_MO # Date of Death--Month
  range 238..239, :DOD_DY # Date of Death--Day
  range 240..243, :TOD # Time of Death
  range 244..244, :DEDUC # Decedent's Education
  range 245..245, :DEDUC_BYPASS # Decedent's Education--Edit Flag
  range 246..246, :DETHNIC1 # Decedent of Hispanic Origin?--Mexican
  range 247..247, :DETHNIC2 # Decedent of Hispanic Origin?--Puerto Rican
  range 248..248, :DETHNIC3 # Decedent of Hispanic Origin?--Cuban
  range 249..249, :DETHNIC4 # Decedent of Hispanic Origin?--Other
  range 250..269, :DETHNIC5 # Decedent of Hispanic Origin?--Other, Literal
  range 270..270, :RACE1 # Decedent's Race--White
  range 271..271, :RACE2 # Decedent's Race--Black or African American
  range 272..272, :RACE3 # Decedent's Race--American Indian or Alaska Native
  range 273..273, :RACE4 # Decedent's Race--Asian Indian
  range 274..274, :RACE5 # Decedent's Race--Chinese
  range 275..275, :RACE6 # Decedent's Race--Filipino
  range 276..276, :RACE7 # Decedent's Race--Japanese
  range 277..277, :RACE8 # Decedent's Race--Korean
  range 278..278, :RACE9 # Decedent's Race--Vietnamese
  range 279..279, :RACE10 # Decedent's Race--Other Asian
  range 280..280, :RACE11 # Decedent's Race--Native Hawaiian
  range 281..281, :RACE12 # Decedent's Race--Guamanian or Chamorro
  range 282..282, :RACE13 # Decedent's Race--Samoan
  range 283..283, :RACE14 # Decedent's Race--Other Pacific Islander
  range 284..284, :RACE15 # Decedent's Race--Other
  range 285..314, :RACE16 # Decedent's Race--First American Indian or Alaska Native Literal
  range 315..344, :RACE17 # Decedent's Race--Second American Indian or Alaska Native Literal
  range 345..374, :RACE18 # Decedent's Race--First Other Asian Literal
  range 375..404, :RACE19 # Decedent's Race--Second Other Asian Literal
  range 405..434, :RACE20 # Decedent's Race--First Other Pacific Islander Literal
  range 435..464, :RACE21 # Decedent's Race--Second Other Pacific Islander Literal
  range 465..494, :RACE22 # Decedent's Race--First Other Literal
  range 495..524, :RACE23 # Decedent's Race--Second Other Literal
  range 525..527, :RACE1E # Race Tabulation Variables
  range 528..530, :RACE2E # 
  range 531..533, :RACE3E # 
  range 534..536, :RACE4E # 
  range 537..539, :RACE5E # 
  range 540..542, :RACE6E # 
  range 543..545, :RACE7E # 
  range 546..548, :RACE8E # 
  range 549..551, :RACE16C # 
  range 552..554, :RACE17C # 
  range 555..557, :RACE18C # 
  range 558..560, :RACE19C # 
  range 561..563, :RACE20C # 
  range 564..566, :RACE21C # 
  range 567..569, :RACE22C # 
  range 570..572, :RACE23C # 
  range 573..573, :RACE_MVR # Decedent's Race--Missing
  range 574..613, :OCCUP # Occupation -- Literal (REQUIRED)
  range 614..616, :OCCUPC # Occupation -- Code (OPTIONAL)
  range 617..656, :INDUST # Industry -- Literal (REQUIRED)
  range 657..659, :INDUSTC # Industry -- Code (OPTIONAL)
  range 660..665, :BCNO # Infant Death/Birth Linking - birth certificate number
  range 666..669, :IDOB_YR # Infant Death/Birth Linking - year of birth
  range 670..671, :BSTATE # Infant Death/Birth Linking - State, U.S. Territory or Canadian Province of Birth - code
  range 672..675, :R_YR # NCHS USE ONLY: Receipt date -- Year
  range 676..677, :R_MO # NCHS USE ONLY: Receipt date -- Month
  range 678..679, :R_DY # NCHS USE ONLY: Receipt date -- Day
  range 680..683, :OCCUPC4 # Occupation -- 4 digit Code (OPTIONAL)
  range 684..687, :INDUSTC4 # Industry -- 4 digit Code (OPTIONAL)
  range 688..691, :DOR_YR # Date of Registration--Year
  range 692..693, :DOR_MO # Date of Registration--Month
  range 694..695, :DOR_DY # Date of Registration--Day
  range 696..699, :FILLER2 # FILLER 2 for expansion
  range 700..700, :MANNER # Manner of Death
  range 701..701, :INT_REJ # Intentional Reject 
  range 702..702, :SYS_REJ # Acme System Reject Codes
  range 703..703, :INJPL # Place of Injury (computer generated)
  range 704..708, :MAN_UC # Manual Underlying Cause 
  range 709..713, :ACME_UC # ACME Underlying Cause
  range 714..873, :EAC # Entity-axis codes
  range 874..874, :TRX_FLG # Transax conversion flag: Computer Generated
  range 875..974, :RAC # Record-axis codes
  range 975..975, :AUTOP # Was Autopsy performed
  range 976..976, :AUTOPF # Were Autopsy Findings Available to Complete the Cause of Death?
  range 977..977, :TOBAC # Did Tobacco Use Contribute to Death?
  range 978..978, :PREG # Pregnancy
  range 979..979, :PREG_BYPASS # If Female--Edit Flag: From EDR only
  range 980..981, :DOI_MO # Date of injury--month
  range 982..983, :DOI_DY # Date of injury--day
  range 984..987, :DOI_YR # Date of injury--year
  range 988..991, :TOI_HR # Time of injury
  range 992..992, :WORKINJ # Injury at work
  range 993..1022, :CERTL # Title of Certifier
  range 1023..1023, :INACT # Activity at time of death (computer generated)
  range 1024..1035, :AUXNO2 # Auxiliary State file number
  range 1036..1065, :STATESP # State Specific Data 
  range 1066..1067, :SUR_MO # Surgery Date--month
  range 1068..1069, :SUR_DY # Surgery Date--day
  range 1070..1073, :SUR_YR # Surgery Date--year
  range 1074..1074, :TOI_UNIT # Time of Injury Unit
  range 1075..1079, :BLANK1 # For possible future change in transax
  range 1080..1080, :ARMEDF # Decedent ever served in Armed Forces?
  range 1081..1110, :DINSTI # Death Institution name
  range 1111..1160, :ADDRESS_D # Long String address for place of death
  range 1161..1170, :STNUM_D # Place of death. Street number
  range 1171..1180, :PREDIR_D # Place of death. Pre Directional
  range 1181..1230, :STNAME_D # Place of death. Street name
  range 1231..1240, :STDESIG_D # Place of death. Street designator
  range 1241..1250, :POSTDIR_D # Place of death. Post Directional
  range 1251..1278, :CITYTEXT_D # Place of death. City or Town name
  range 1279..1306, :STATETEXT_D # Place of death. State name literal
  range 1307..1315, :ZIP9_D # Place of death. Zip code
  range 1316..1343, :COUNTYTEXT_D # Place of death. County of Death
  range 1344..1348, :CITYCODE_D # Place of death. City FIPS code
  range 1349..1365, :LONG_D # Place of death. Longitude
  range 1366..1382, :LAT_D # Place of Death. Latitude
  range 1383..1383, :SPOUSELV # Decedent's spouse living at decedent's DOD?
  range 1384..1433, :SPOUSEF # Spouse's First Name
  range 1434..1483, :SPOUSEL  # Husband's Surname/Wife's Maiden Last Name
  range 1484..1493, :STNUM_R # Decedent's Residence - Street number
  range 1494..1503, :PREDIR_R # Decedent's Residence - Pre Directional
  range 1504..1531, :STNAME_R # Decedent's Residence - Street name
  range 1532..1541, :STDESIG_R # Decedent's Residence - Street designator
  range 1542..1551, :POSTDIR_R # Decedent's Residence - Post Directional
  range 1552..1558, :UNITNUM_R # Decedent's Residence - Unit or apt number
  range 1559..1586, :CITYTEXT_R # Decedent's Residence - City or Town name
  range 1587..1595, :ZIP9_R # Decedent's Residence - ZIP code
  range 1596..1623, :COUNTYTEXT_R # Decedent's Residence - County
  range 1624..1651, :STATETEXT_R  # Decedent's Residence - State name
  range 1652..1679, :COUNTRYTEXT_R # Decedent's Residence - COUNTRY name
  range 1680..1729, :ADDRESS_R # Long string address for decedent's place of residence same as above but allows states to choose the way they capture information.
  range 1730..1731, :RESSTATE # Old NCHS residence state code
  range 1732..1734, :RESCON # Old NCHS residence city/county combo code
  range 1735..1737, :DETHNICE  # Hispanic -
  range 1738..1739, :NCHSBRIDGE # Race -
  range 1740..1740, :HISPOLDC # Hispanic - old NCHS single ethnicity codes
  range 1741..1741, :RACEOLDC # Race - old NCHS single race codes
  range 1742..1756, :HISPSTSP # Hispanic Origin - Specify 
  range 1757..1806, :RACESTSP # Race - Specify
  range 1807..1856, :DMIDDLE # Middle Name of Decedent 
  range 1857..1906, :DDADF # Father's First Name
  range 1907..1956, :DDADMID # Father's Middle Name
  range 1957..2006, :DMOMF # Mother's First Name
  range 2007..2056, :DMOMMID # Mother's Middle Name
  range 2057..2106, :DMOMMDN # Mother's Maiden Surname
  range 2107..2107, :REFERRED # Was case Referred to Medical Examiner/Coroner?
  range 2108..2157, :POILITRL # Place of Injury- literal
  range 2158..2407, :HOWINJ # Describe How Injury Occurred
  range 2408..2437, :TRANSPRT # If Transportation Accident, Specify
  range 2438..2465, :COUNTYTEXT_I # County of Injury - literal
  range 2466..2468, :COUNTYCODE_I # County of Injury code
  range 2469..2496, :CITYTEXT_I # Town/city of Injury - literal
  range 2497..2501, :CITYCODE_I # Town/city of Injury code
  range 2502..2503, :STATECODE_I # State, U.S. Territory or Canadian Province of Injury - code
  range 2504..2520, :LONG_I # Place of injury. Longitude
  range 2521..2537, :LAT_I # Place of injury. Latitude
  range 2538..2539, :OLDEDUC # Old NCHS education code if collected - receiving state will recode as they prefer
  range 2540..2540, :REPLACE # Replacement Record -- suggested codes
  range 2541..2660, :COD1A # Cause of Death Part I Line a
  range 2661..2680, :INTERVAL1A # Cause of Death Part I Interval, Line a
  range 2681..2800, :COD1B # Cause of Death Part I Line b
  range 2801..2820, :INTERVAL1B # Cause of Death Part I Interval, Line b
  range 2821..2940, :COD1C # Cause of Death Part I Line c
  range 2941..2960, :INTERVAL1C # Cause of Death Part I Interval, Line c
  range 2961..3080, :COD1D # Cause of Death Part I Line d
  range 3081..3100, :INTERVAL1D # Cause of Death Part I Interval, Line d
  range 3101..3340, :OTHERCONDITION # Cause of Death Part II
  range 3341..3390, :DMAIDEN # Decedent's Maiden Name
  range 3391..3395, :DBPLACECITYCODE # Decedent's Birth Place City - Code
  range 3396..3423, :DBPLACECITY # Decedent's Birth Place City - Literal
  range 3424..3473, :SPOUSEMIDNAME # Spouse's Middle Name
  range 3474..3483, :SPOUSESUFFIX # Spouse's Suffix
  range 3484..3493, :FATHERSUFFIX # Father's Suffix
  range 3494..3503, :MOTHERSSUFFIX # Mother's Suffix
  range 3504..3533, :INFORMRELATE # Informant's Relationship
  range 3534..3535, :DISPSTATECD # State, U.S. Territory or Canadian Province of Disposition - code
  range 3536..3563, :DISPSTATE # Disposition State or Territory - Literal
  range 3564..3568, :DISPCITYCODE # Disposition City - Code
  range 3569..3596, :DISPCITY # Disposition City - Literal
  range 3597..3696, :FUNFACNAME # Funeral Facility Name
  range 3697..3706, :FUNFACSTNUM # Funeral Facility - Street number
  range 3707..3716, :FUNFACPREDIR # Funeral Facility - Pre Directional
  range 3717..3744, :FUNFACSTRNAME # Funeral Facility - Street name
  range 3745..3754, :FUNFACSTRDESIG # Funeral Facility - Street designator
  range 3755..3764, :FUNPOSTDIR # Funeral Facility - Post Directional
  range 3765..3771, :FUNUNITNUM # Funeral Facility - Unit or apt number
  range 3772..3821, :FUNFACADDRESS # Long string address for Funeral Facility same as above but allows states to choose the way they capture information.
  range 3822..3849, :FUNCITYTEXT # Funeral Facility - City or Town name
  range 3850..3851, :FUNSTATECD # State, U.S. Territory or Canadian Province of Funeral Facility - code
  range 3852..3879, :FUNSTATE # State, U.S. Territory or Canadian Province of Funeral Facility - literal
  range 3880..3888, :FUNZIP # Funeral Facility - ZIP
  range 3889..3896, :PPDATESIGNED # Person Pronouncing Date Signed
  range 3897..3900, :PPTIME # Person Pronouncing Time Pronounced
  range 3901..3950, :CERTFIRST # Certifier's First Name
  range 3951..4000, :CERTMIDDLE # Certifier's Middle Name
  range 4001..4050, :CERTLAST # Certifier's Last Name
  range 4051..4060, :CERTSUFFIX # Certifier's Suffix Name
  range 4061..4070, :CERTSTNUM # Certifier - Street number
  range 4071..4080, :CERTPREDIR # Certifier - Pre Directional
  range 4081..4108, :CERTSTRNAME # Certifier - Street name
  range 4109..4118, :CERTSTRDESIG # Certifier - Street designator
  range 4119..4128, :CERTPOSTDIR # Certifier - Post Directional
  range 4129..4135, :CERTUNITNUM # Certifier - Unit or apt number
  range 4136..4185, :CERTADDRESS # Long string address for Certifier same as above but allows states to choose the way they capture information.
  range 4186..4213, :CERTCITYTEXT # Certifier - City or Town name
  range 4214..4215, :CERTSTATECD # State, U.S. Territory or Canadian Province of Certifier - code
  range 4216..4243, :CERTSTATE # State, U.S. Territory or Canadian Province of Certifier - literal
  range 4244..4252, :CERTZIP # Certifier - Zip
  range 4253..4260, :CERTDATE # Certifier Date Signed
  range 4261..4268, :FILEDATE # Date Filed
  range 4269..4296, :STINJURY # State, U.S. Territory or Canadian Province of Injury - literal
  range 4297..4324, :STATEBTH # State, U.S. Territory or Canadian Province of Birth - literal
  range 4325..4326, :DTHCOUNTRYCD # Country of Death - Code
  range 4327..4354, :DTHCOUNTRY # Country of Death - Literal
  range 4355..4357, :SSADTHCODE # SSA State Source of Death
  range 4358..4358, :SSAFOREIGN # SSA Foreign Country Indicator
  range 4359..4359, :SSAVERIFY # SSA EDR Verify Code
  range 4360..4367, :SSADATEVER # SSA Date of SSN Verification
  range 4368..4375, :SSADATETRANS # SSA Date of State Transmission
  range 4376..4425, :MARITAL_DESCRIP # Marital Descriptor
  range 4426..4428, :DETHNIC5C  # Hispanic Code for Literal
  range 4429..4429, :PLACE1_1 # Blank for One-Byte Field 1
  range 4430..4430, :PLACE1_2 # Blank for One-Byte Field 2
  range 4431..4431, :PLACE1_3 # Blank for One-Byte Field 3
  range 4432..4432, :PLACE1_4 # Blank for One-Byte Field 4
  range 4433..4433, :PLACE1_5 # Blank for One-Byte Field 5
  range 4434..4434, :PLACE1_6 # Blank for One-Byte Field 6
  range 4435..4442, :PLACE8_1 # Blank for Eight-Byte Field 1
  range 4443..4450, :PLACE8_2 # Blank for Eight-Byte Field 2
  range 4451..4458, :PLACE8_3 # Blank for Eight-Byte Field 3
  range 4459..4478, :PLACE20 # Blank for Twenty-Byte Field
  range 4479..4728, :BLANK2 # Blank for future expansion
  range 4729..4999, :BLANK3 # Blank for Jurisdictional Use Only

  # Support some specific field types
  def rjust_zeroes_input(field_name, field_value, field_length)
    "%0#{field_length}d" % field_value
  end
  def rjust_zeroes_output(field_name, field_value, field_length)
    # Remove all zeroes from left
    field_value.to_s.gsub(/^0+/, '')
  end

end

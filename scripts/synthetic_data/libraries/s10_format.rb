require 'ije'

class S10Format < IJE::FixedLengthRecordFormat
  range 0..3, :DOD_YR
  range 4..5, :ST_OCC
  range 6..11, :CERT_NO
  range 12..12, :CS
  range 13..16, :LOT
  range 17..17, :SECT
  range 18..20, :SHIP
  range 21..22, :REC_MO
  range 23..24, :REC_DY
  range 25..28, :REC_YR
  range 29..32, :VER_SM
  range 33..34, :DOD_MO
  range 35..36, :DOD_DY
  range 37..37, :SEX
  range 38..38, :AGETYPE
  range 39..41, :AGE
  range 42..161, :CODIa
  range 162..181, :INTIa
  range 182..301, :CODIb
  range 302..321, :INTIb
  range 322..441, :CODIc
  range 442..461, :INTIc
  range 462..581, :CODId
  range 582..601, :INTId
  range 602..841, :CODII
  range 842..842, :TOBAC
  range 843..843, :PREG
  range 844..844, :PREG_BYPASS
  range 845..845, :MANNER
  range 846..847, :DOI_MO
  range 848..849, :DOI_DY
  range 850..853, :DOI_YR
  range 854..857, :TOI_HR
  range 858..858, :UNITS_OF_TIME  # https://www.cdc.gov/nchs/data/dvs/2003s10.pdf does not specify beyond "Units of Time"
  range 859..908, :INJPLL
  range 909..909, :WORKINJ
  range 910..1159, :LINJURY
  range 1160..1189, :TRANSPL
  range 1190..1190, :AUTOP
  range 1191..1191, :AUTOPF
  range 1192..1221, :CERTL
  range 1222..1223, :SUR_MO
  range 1224..1225, :SUR_DY
  range 1226..1229, :SUR_YR
  range 1230..1230, :INC_DT
  range 1231..1231, :DUE2Ib
  range 1232..1232, :DUE2Ic
  range 1233..1233, :DUE2Id
  range 1234..1234, :DUE2II
  range 1235..1235, :INACT
  range 1236..1236, :INJPL
  range 1237..1248, :AUXNO
  range 1249..1278, :STATESP
end

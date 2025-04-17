# Read a list of IJE files and write out derived S10 files

require 'date'
require './libraries/ije_format'
require './libraries/s10_format'

# Make sure we can map all the fields, allowing some to map automatically if the names match

ije_format_fields = IJEFormat.instance_variable_get('@ranges').keys
s10_format_fields = S10Format.instance_variable_get('@ranges').keys

# Find the field names that overlap and build our initial mapping from that
overlap = ije_format_fields & s10_format_fields
mapping = overlap.each_with_object({}) { |field, hash| hash[field] = field }

# Fill in the mapping with some manual mapping where fields line up but names have changed
mapping[:CERT_NO] = :FILENO
mapping[:ST_OCC] = :DSTATE
mapping[:CODIa] = :COD1A
mapping[:INTIa] = :INTERVAL1A
mapping[:CODIb] = :COD1B
mapping[:INTIb] = :INTERVAL1B
mapping[:CODIc] = :COD1C
mapping[:INTIc] = :INTERVAL1C
mapping[:CODId] = :COD1D
mapping[:INTId] = :INTERVAL1D
mapping[:CODII] = :OTHERCONDITION
mapping[:INJPLL] = :POILITRL
mapping[:LINJURY] = :HOWINJ
mapping[:TRANSPL] = :TRANSPRT

# Some data doesn't come from IJE, so populate these with static values

mapping[:CS] = '0' # Coder status
mapping[:LOT] = '0001' # Lot
mapping[:SECT] = '0' # Section number
mapping[:SHIP] = '001' # Shipment number
mapping[:REC_MO] = '%02d' % Date.today.month # NCHS receipt date -- Month
mapping[:REC_DY] = '%02d' % Date.today.day # NCHS receipt date -- Day
mapping[:REC_YR] = Date.today.year.to_s # NCHS receipt date -- Year
mapping[:VER_SM] = '0001' # Version number of SuperMICAR

# There are some fields that don't seem to be collected anymore or we don't know how they get populated
ignore = [:UNITS_OF_TIME, :DUE2Ib, :DUE2Ic, :DUE2Id, :DUE2II, :INC_DT]

# Sanity check that we're not missing any mapping
missing = s10_format_fields - mapping.keys - ignore
raise "Cannot map fields #{missing.inspect}" if missing.length > 0

ARGV.each do |ije_file|
  ije_data = File.read(ije_file)
  ije_data.split(/\n/).each do |ije_line|
    IJEFormat.read(ije_line) do |ije|
      s10_input = {}
      mapping.each do |s10_key, ije_key|
        s10_input[s10_key] = ije_key.is_a?(Symbol) ? ije.send(ije_key) : ije_key
      end
      puts S10Format.write([S10Format.new(s10_input)])
    end
  end
end

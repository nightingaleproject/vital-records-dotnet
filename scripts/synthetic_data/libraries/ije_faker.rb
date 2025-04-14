# Generate fake data for some IJE fields

require 'json'
require 'csv'
require 'faker'
require 'weighted_distribution'

class IJEFaker

  # Generate a jurisdiction based on distribution across actual populations at some point in time
  def self.jurisdiction

    # Just set up the weighted distribution once
    if @jurisdiction_distribution.nil?
      jurisdiction_populations = {
        CA: 39538223, TX: 29145505, FL: 21538187, NY: 11943214, PA: 13002700, IL: 12812508,
        OH: 11799448, GA: 10711908, NC: 10439388, MI: 10077331, NJ: 9288994,  VA: 8631393,
        WA: 7705281,  AZ: 7151502,  TN: 6910840,  MA: 7029917,  IN: 6785528,  MO: 6154913,
        MD: 6177224,  WI: 5893718,  CO: 5773714,  MN: 5706494,  SC: 5118425,  AL: 5024279,
        LA: 4657757,  KY: 4505836,  OR: 4237256,  OK: 3959353,  CT: 3605944,  UT: 3271616,
        IA: 3190369,  PR: 3285874,  NV: 3104614,  AR: 3011524,  KS: 2937880,  MS: 2961279,
        NM: 2117522,  NE: 1961504,  ID: 1839106,  WV: 1793716,  HI: 1455271,  NH: 1377529,
        ME: 1362359,  MT: 1084225,  RI: 1097379,  DE: 989948,   SD: 886667,   ND: 779094,
        AK: 733391,   DC: 689545,   VT: 643077,   WY: 576851,   GU: 153836,   VI: 87146,
        AS: 49710,    MP: 47329,    YC: 8258035
      }
      @jurisdiction_distribution = WeightedDistribution.new(jurisdiction_populations)
    end

    @jurisdiction_distribution.sample.to_s

  end

  # For some contexts (e.g., state of residence) we want YC to map to NY
  def self.jurisdiction_no_yc(jurisdiction)
    jurisdiction == 'YC' ? 'NY' : jurisdiction
  end

  # Given the jurisdiction code look up the name of the state
  def self.jurisdiction_state_name(jurisdiction)
    # Includes YC -> New York (state)
    @jurisdiction_lookup ||= {
      "AL" => "Alabama", "KY" => "Kentucky", "ND" => "North Dakota", "AK" => "Alaska", "LA" => "Louisiana",
      "OH" => "Ohio", "AZ" => "Arizona", "ME" => "Maine", "OK" => "Oklahoma", "AR" => "Arkansas", "MD" => "Maryland",
      "OR" => "Oregon", "CA" => "California", "MA" => "Massachusetts", "PA" => "Pennsylvania", "CO" => "Colorado",
      "MI" => "Michigan", "RI" => "Rhode Island", "CT" => "Connecticut", "MN" => "Minnesota", "SC" => "South Carolina",
      "DC" => "District of Columbia", "MS" => "Mississippi", "SD" => "South Dakota", "DE" => "Delaware", "MO" => "Missouri",
      "TN" => "Tennessee", "FL" => "Florida", "MT" => "Montana", "TX" => "Texas", "GA" => "Georgia", "NE" => "Nebraska",
      "UT" => "Utah", "HI" => "Hawaii", "NV" => "Nevada", "VT" => "Vermont", "ID" => "Idaho", "NH" => "New Hampshire",
      "VA" => "Virginia", "IL" => "Illinois", "NJ" => "New Jersey", "WA" => "Washington", "IN" => "Indiana",
      "NM" => "New Mexico", "WV" => "West Virginia", "IA" => "Iowa", "NY" => "New York", "YC" => "New York",
      "WI" => "Wisconsin", "KS" => "Kansas", "NC" => "North Carolina", "WY" => "Wyoming", "AS" => "American Samoa",
      "GU" => "Guam", "MP" => "Northern Marianas", "PR" => "Puerto Rico", "VI" => "Virgin Islands"
    }
    @jurisdiction_lookup[jurisdiction]
  end

  # Generate a certificate number for a jurisdiction and a year; we just start at 1 and increment
  def self.certificate_number(jurisdiction, year)
    @certificate_numbers ||= Hash.new { |h, k| h[k] = Hash.new(0) }
    number = @certificate_numbers[jurisdiction][year] += 1
  end

  # Generate a marital status based on the age using a distribution from one jurisdiction
  def self.marital_status(age)
    # Just set up the weighted distribution once
    if @marital_distribution.nil?
      marital_counts = {
        'M' => 20645,
        'A' => 228,
        'W' => 18058,
        'D' => 10334,
        'S' => 6286,
        'U' => 395
      }
      @marital_distribution = WeightedDistribution.new(marital_counts)
    end
    # Always single if under a somewhat arbitrary age cut off
    age.to_i > 17 ? @marital_distribution.sample : 'S'
  end

  # Generate first, middle, and last names adding a suffix to make it clearer that the data is synthetic
  def self.given_name(sex)
    name = sex == 'F' ? Faker::Name.female_first_name : Faker::Name.male_first_name
    "#{name}#{'%02d' % rand(0..99)}"
  end
  def self.middle_name
    name = Faker::Name.middle_name
    "#{name}#{'%02d' % rand(0..99)}"
  end
  def self.last_name
    name = Faker::Name.last_name
    "#{name}#{'%02d' % rand(0..99)}"
  end

  # Load up information on common literal cause of death sequences from a JSON file; this file was generated
  # using sequences that appear commonly in a set of de-identified jurisdiction data to ensure it's not
  # connected to any individual decedent so is limited to a relatively small set of possible causes
  def self.sequence_data
    @sequence_data ||= JSON.parse(File.read(File.join(File.dirname(__FILE__), '..', 'datafiles', 'common_literal_sequences.json')))
  end

  # Select a random literal sequence
  # Note: this could be faster but performance isn't critical here at the moment
  def self.literal_sequence
    # Pick a random number and step through the literals adding up the probabilities until we hit our number
    r = rand
    sum = 0
    sequence_data.each do |ls|
      sum += ls['likelihood']
      return ls['literals'] if r < sum
    end
    return sequence_data.last['literals']
  end

  # Given a type of value and a sequence from the set of sequences generate and return an appropriate value
  def self.value_for_sequence(value_type, sequence)
    # Find the matching sequence
    match = sequence_data.detect { |ls| ls['literals'] == sequence }
    raise 'Previous sequence not found' unless match
    # Find the value type information
    type_info = match[value_type.to_s]
    raise "Could not find value type #{value_type} information" unless type_info
    # If it's a two element array it's a range, if it's a hash we pick a random value at the provided
    # probabilities, otherwise we just return the value
    # NOTE: This results in data that leans towards being plausible but can still result in odd results
    # since different value types can interact with each other and not just with the cause of death
    if type_info.is_a?(Array) && type_info.length == 2
      rand(type_info.first..type_info.last) # Random value in range
    elsif type_info.is_a?(Hash)
      r = rand
      sum = 0
      type_info.each do |type, likelihood|
        sum += likelihood
        return type if r < sum
      end
      return type_info.keys.last
    else
      return type_info
    end
  end

  # Load up information on jurisdiction county and city codes
  def self.location_code_data
    # Organize by state code, caching
    if !@location_code_data
      @location_code_data = Hash.new { |h, k| h[k] = [] }
      # CSV file has a header that we drop and state as the first element
      CSV.read(File.join(File.dirname(__FILE__), '..', 'datafiles', 'PLACE_CODES.txt')).drop(1).each do |line|
        @location_code_data[line[0]] << line
      end
    end
    @location_code_data
  end

  # Given a jurisdiction pick a random place name in the jurisdiction from the location data
  def self.place_name(jurisdiction)
    # If jurisdiction can't be found we return '' meaning unknown; place name is in 4th space
    location_code_data[jurisdiction]&.sample&.[](3) || ''
  end

  # Given a jurisdiction and a place name in that jurisdiction return the place code
  def self.place_code(jurisdiction, place_name)
    # If it can't be found we return '99999' meaning unknown; place code is in 6th slot
    location_code_data[jurisdiction]&.detect { |l| l[3] == place_name }&.[](5) || '99999'
  end

  # Given a jurisdiction and a place name in that jurisdiction return the county name
  def self.county_name(jurisdiction, place_name)
    # If it can't be found we return '' meaning unknown; county name is in 2nd slot
    location_code_data[jurisdiction]&.detect { |l| l[3] == place_name }&.[](1) || ''
  end

  # Given a jurisdiction and a place name in that jurisdiction return the county code
  def self.county_code(jurisdiction, place_name)
    # If it can't be found we return '999' meaning unknown; county code is in 3rd slot
    location_code_data[jurisdiction]&.detect { |l| l[3] == place_name }&.[](2) || '999'
  end

end

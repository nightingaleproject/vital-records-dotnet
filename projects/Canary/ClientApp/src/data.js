export const stateOptions = [
  { key: 'AL', text: 'Alabama', value: 'AL' },
  { key: 'AK', text: 'Alaska', value: 'AK' },
  { key: 'AZ', text: 'Arizona', value: 'AZ' },
  { key: 'AR', text: 'Arkansas', value: 'AR' },
  { key: 'AS', text: 'American Samoa', value: 'AS' },
  { key: 'CA', text: 'California', value: 'CA' },
  { key: 'CO', text: 'Colorado', value: 'CO' },
  { key: 'MP', text: 'Commonwealth of Northern Mariana Islands', value: 'MP' },
  { key: 'CT', text: 'Connecticut', value: 'CT' },
  { key: 'DE', text: 'Delaware', value: 'DE' },
  { key: 'DC', text: 'District of Columbia', value: 'DC' },
  { key: 'FL', text: 'Florida', value: 'FL' },
  { key: 'GA', text: 'Georgia', value: 'GA' },
  { key: 'GU', text: 'Guam', value: 'GU' },
  { key: 'HI', text: 'Hawaii', value: 'HI' },
  { key: 'ID', text: 'Idaho', value: 'ID' },
  { key: 'IL', text: 'Illinois', value: 'IL' },
  { key: 'IN', text: 'Indiana', value: 'IN' },
  { key: 'IA', text: 'Iowa', value: 'IA' },
  { key: 'KS', text: 'Kansas', value: 'KS' },
  { key: 'KY', text: 'Kentucky', value: 'KY' },
  { key: 'LA', text: 'Louisiana', value: 'LA' },
  { key: 'ME', text: 'Maine', value: 'ME' },
  { key: 'MD', text: 'Maryland', value: 'MD' },
  { key: 'MA', text: 'Massachusetts', value: 'MA' },
  { key: 'MI', text: 'Michigan', value: 'MI' },
  { key: 'MN', text: 'Minnesota', value: 'MN' },
  { key: 'MS', text: 'Mississippi', value: 'MS' },
  { key: 'MO', text: 'Missouri', value: 'MO' },
  { key: 'MT', text: 'Montana', value: 'MT' },
  { key: 'NE', text: 'Nebraska', value: 'NE' },
  { key: 'NV', text: 'Nevada', value: 'NV' },
  { key: 'NH', text: 'New Hampshire', value: 'NH' },
  { key: 'NJ', text: 'New Jersey', value: 'NJ' },
  { key: 'NM', text: 'New Mexico', value: 'NM' },
  { key: 'NY', text: 'New York', value: 'NY' },
  { key: 'YC', text: 'New York City', value: 'YC' },
  { key: 'NC', text: 'North Carolina', value: 'NC' },
  { key: 'ND', text: 'North Dakota', value: 'ND' },
  { key: 'OH', text: 'Ohio', value: 'OH' },
  { key: 'OK', text: 'Oklahoma', value: 'OK' },
  { key: 'OR', text: 'Oregon', value: 'OR' },
  { key: 'PA', text: 'Pennsylvania', value: 'PA' },
  { key: 'PR', text: 'Puerto Rico', value: 'PR' },
  { key: 'RI', text: 'Rhode Island', value: 'RI' },
  { key: 'SC', text: 'South Carolina', value: 'SC' },
  { key: 'SD', text: 'South Dakota', value: 'SD' },
  { key: 'TN', text: 'Tennessee', value: 'TN' },
  { key: 'TX', text: 'Texas', value: 'TX' },
  { key: 'UT', text: 'Utah', value: 'UT' },
  { key: 'VI', text: 'U.S. Virgin Islands', value: 'VI' },
  { key: 'VT', text: 'Vermont', value: 'VT' },
  { key: 'VA', text: 'Virginia', value: 'VA' },
  { key: 'WA', text: 'Washington', value: 'WA' },
  { key: 'WV', text: 'West Virginia', value: 'WV' },
  { key: 'WI', text: 'Wisconsin', value: 'WI' },
  { key: 'WY', text: 'Wyoming', value: 'WY' },
];

export const messageTypesVRDR = {
  "http://nchs.cdc.gov/vrdr_submission": "Submission",
  "http://nchs.cdc.gov/vrdr_submission_update": "Update",
  "http://nchs.cdc.gov/vrdr_acknowledgement": "Acknowledgement",
  "http://nchs.cdc.gov/vrdr_alias": "Alias",
  "http://nchs.cdc.gov/vrdr_submission_void": "Void",
  "http://nchs.cdc.gov/vrdr_coding": "Coding",
  "http://nchs.cdc.gov/vrdr_coding_update": "Coding Update",
  "http://nchs.cdc.gov/vrdr_extraction_error": "Extraction Error"
}

export const messageTypesBirth = {
  "http://nchs.cdc.gov/birth_acknowledgement": "Acknowledgement",
  "http://nchs.cdc.gov/birth_demographics_coding": "Demographics Coding",
  "http://nchs.cdc.gov/birth_extraction_error": "Error",
  "http://nchs.cdc.gov/birth_status": "Status",
  "http://nchs.cdc.gov/birth_submission": "Submission",
  "http://nchs.cdc.gov/birth_submission_void": "Void",
  "http://nchs.cdc.gov/birth_submission_update": "Update"
}

export const messageTypesFetalDeath = {
  "http://nchs.cdc.gov/fd_acknowledgement": "Acknowledgement",
  "http://nchs.cdc.gov/fd_demographics_coding": "Demographics Coding",
  "http://nchs.cdc.gov/fd_extraction_error": "Error",
  "http://nchs.cdc.gov/fd_status": "Status",
  "http://nchs.cdc.gov/fd_submission": "Submission",
  "http://nchs.cdc.gov/fd_submission_void": "Void",
  "http://nchs.cdc.gov/fd_submission_update": "Update"
}

export const messageTypeIconsVRDR = [
  { key: 'submission', name: 'Submission', icon: 'paper plane' },
  // { key: 'acknowledgement', name: 'Acknowledgement', icon: 'thumbs up' },
  { key: 'update', name: 'Update', icon: 'redo' },
  { key: 'alias', name: 'Alias', icon: 'at' },
  { key: 'void', name: 'Void', icon: 'ban' },
];

export const messageTypeIconsBFDR = [
  { key: 'submission', name: 'Submission', icon: 'paper plane' },
  // { key: 'acknowledgement', name: 'Acknowledgement', icon: 'thumbs up' },
  { key: 'update', name: 'Update', icon: 'redo' },
  { key: 'void', name: 'Void', icon: 'ban' },
];

export const responseMessageTypeIconsVRDR = [
  { key: 'ack', name: 'ACK', icon: 'thumbs up' },
  { key: 'trx', name: 'TRX', icon: 'exchange' },
  { key: 'mre', name: 'MRE', icon: 'exchange' },
  { key: 'error', name: 'Error', icon: 'ban' },
];

export const responseMessageTypeIconsBFDR = [
  { key: 'ack', name: 'ACK', icon: 'thumbs up' },
  { key: 'error', name: 'Error', icon: 'ban' },
];
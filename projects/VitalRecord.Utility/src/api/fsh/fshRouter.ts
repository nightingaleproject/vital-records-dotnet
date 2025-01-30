import { OpenAPIRegistry } from '@asteasolutions/zod-to-openapi';
import express, { type Request, type Response, type Router } from 'express';
import { z } from 'zod';
import { processor, utils, gofshExport, gofshClient } from 'gofsh';
import { sushiClient } from 'fsh-sushi';

import { createApiResponse } from '@/api-docs/openAPIResponseBuilders';
import { escapeRegExp } from '@/common/utils/escapeRegExp';

export const fshRegistry = new OpenAPIRegistry();
export const fshRouter: Router = express.Router();

fshRegistry.registerPath({
  method: 'get',
  path: '/health-check',
  tags: ['Health Check'],
  responses: createApiResponse(z.null(), 'Success'),
});

fshRouter.post('/fhir-to-fsh', async (_req: Request, res: Response) => {
  const body = _req.body;
  const fsh = await gofshClient.fhirToFsh([body]);

  return res.status(200).send(fsh);
});

fshRouter.post('/fsh-to-fhir', async (_req: Request, res: Response) => {
  const body = _req.body;

  const fhir = await sushiClient.fshToFhir(body["fsh"], {
    dependencies: [
      {
        packageId: 'hl7.fhir.us.vrdr',
        version: '2.2.0',
      },
      {
        packageId: 'hl7.fhir.us.core',
        version: 'current',
      },
    ],
  });

  return res.status(200).send(fhir);
});

export const PROFILES = {
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-activity-at-time-of-death':
    'ActivityAtTimeOfDeath',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-automated-underlying-cause-of-death':
    'AutomatedUnderlyingCauseOfDeath',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-autopsy-performed-indicator':
    'AutopsyPerformedIndicator',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-birth-record-identifier':
    'BirthRecordIdentifier',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-cause-of-death-coded-bundle':
    'CauseOfDeathCodedContentBundle',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-cause-of-death-part1':
    'CauseOfDeathPart1',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-cause-of-death-part2':
    'CauseOfDeathPart2',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-certifier': 'Certifier',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-coded-race-and-ethnicity':
    'CodedRaceAndEthnicity',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-coding-status-values':
    'CodingStatusValues',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-death-certificate':
    'DeathCertificate',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-death-certificate-document':
    'DeathCertificateDocument',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-death-certification':
    'DeathCertification',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-death-date':
    'DeathDate',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-death-location':
    'DeathLocation',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-decedent': 'Decedent',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-decedent-age':
    'DecedentAge',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-decedent-disposition-method':
    'DecedentDispositionMethod',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-decedent-education-level':
    'DecedentEducationLevel',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-decedent-father':
    'DecedentFather',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-military-service':
    'DecedentMilitaryService',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-decedent-mother':
    'DecedentMother',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-decedent-pregnancy':
    'DecedentPregnancyStatus',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-decedent-spouse':
    'DecedentSpouse',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-decedent-usual-work':
    'DecedentUsualWork',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-demographic-coded-bundle':
    'DemographicCodedContentBundle',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-disposition-location':
    'DispositionLocation',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-entity-axis-cause-of-death':
    'EntityAxisCauseOfDeath',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-examiner-contacted':
    'ExaminerContacted',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-funeral-home':
    'FuneralHome',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-injury-incident':
    'InjuryIncident',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-injury-location':
    'InjuryLocation',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-input-race-and-ethnicity':
    'InputRaceAndEthnicity',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-manner-of-death':
    'MannerOfDeath',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-manual-underlying-cause-of-death':
    'ManualUnderlyingCauseOfDeath',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-mortician': 'Mortician',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-place-of-injury':
    'PlaceOfInjury',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-record-axis-cause-of-death':
    'RecordAxisCauseOfDeath',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-surgery-date':
    'SurgeryDate',
  'http://hl7.org/fhir/us/vrdr/StructureDefinition/vrdr-tobacco-use-contributed-to-death':
    'TobaccoUseContributedToDeath',
};

export const convertInstanceOfToProfileName = (text: string): string => {
  for (const [key, value] of Object.entries(PROFILES)) {
    const s = String.raw`InstanceOf: \w+\n(Usage: .*\n(\* meta\..*\n)*?\* meta\.profile = "${escapeRegExp(
      key,
    )}")`;
    const regex = new RegExp(s, 'gm');
    text = text.replace(regex, `InstanceOf: ${value}\n$1`);
  }

  return text;
};

fshRouter.post('/convert-instance-of', async (_req: Request, res: Response) => {
  const body = _req.body;

  body['fsh'] = convertInstanceOfToProfileName(body['fsh']);
  return res.status(200).send(body);
});

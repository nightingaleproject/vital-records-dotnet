import express, { Request, Response } from 'express';
import { gofshClient } from 'gofsh';
import { sushiClient } from 'fsh-sushi';

const app = express();
const port = 443;

app.use(express.json()); // Parses JSON request bodies
app.use(express.text());
app.use(express.urlencoded({ extended: true })); // Parses URL-encoded request bodies

app.get('/', (req: Request, res: Response) => {
  res.send('ok');
});

app.listen(port, () => {
  console.log(`Server is listening on port ${port}`);
});

app.post('/fhirToFsh', async (req: Request, res: Response) => {
  const vrdr = req.query.vrdr || '2.2.0';
  const bfdr = req.query.bfdr || '2.0.0';

  const fsh = await gofshClient.fhirToFsh(
    [
      {
        resourceType: 'StructureDefinition',
        name: 'Foo',
        baseDefinition: 'http://hl7.org/fhir/StructureDefinition/Patient',
        kind: 'resource',
        derivation: 'constraint',
        fhirVersion: '4.0.1',
      },
      req.body,
    ],
    {
      style: 'string',
      dependencies: [`hl7.fhir.us.vrdr#${vrdr}`, `hl7.fhir.us.bfdr#${bfdr}`],
    },
  );

  res.send(fsh);
});

app.post('/fshToFhir', async (req: Request, res: Response) => {
  const vrdr = req.query.vrdr as string || '2.2.0';
  const bfdr = req.query.bfdr as string || '2.0.0';

  const fhir = await sushiClient.fshToFhir(req.body as string, {
    fhirVersion: '4.0.1',
    dependencies: [
      {
        packageId: 'hl7.fhir.us.vrdr',
        version: vrdr,
      },
      {
        packageId: 'hl7.fhir.us.bfdr',
        version: bfdr,
      },
    ],
  });

  res.send(fhir);
});
"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const gofsh_1 = require("gofsh");
const fsh_sushi_1 = require("fsh-sushi");
const app = (0, express_1.default)();
const port = 443;
app.use(express_1.default.json()); // Parses JSON request bodies
app.use(express_1.default.text());
app.use(express_1.default.urlencoded({ extended: true })); // Parses URL-encoded request bodies
app.get('/', (req, res) => {
    res.send('ok');
});
app.listen(port, () => {
    console.log(`Server is listening on port ${port}`);
});
app.post('/fhirToFsh', async (req, res) => {
    const vrdr = req.query.vrdr || '2.2.0';
    const bfdr = req.query.bfdr || '2.0.0';
    const fsh = await gofsh_1.gofshClient.fhirToFsh([
        {
            resourceType: 'StructureDefinition',
            name: 'Foo',
            baseDefinition: 'http://hl7.org/fhir/StructureDefinition/Patient',
            kind: 'resource',
            derivation: 'constraint',
            fhirVersion: '4.0.1',
        },
        req.body,
    ], {
        style: 'string',
        dependencies: [`hl7.fhir.us.vrdr#${vrdr}`, `hl7.fhir.us.bfdr#${bfdr}`],
        logLevel: 'info',
        indent: true,
    });
    res.send(fsh);
});
app.post('/fshToFhir', async (req, res) => {
    const vrdr = req.query.vrdr || '2.2.0';
    const bfdr = req.query.bfdr || '2.0.0';
    const fhir = await fsh_sushi_1.sushiClient.fshToFhir(req.body, {
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

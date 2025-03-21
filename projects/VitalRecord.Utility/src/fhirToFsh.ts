import { app, HttpRequest, HttpResponse, HttpResponseInit, InvocationContext } from "@azure/functions";
// import { utils } from 'fsh-sushi';
// import {
//   ensureOutputDir,
//   FHIRDefinitions,
//   getInputDir,
//   getAliasFile,
//   getFhirProcessor,
//   getResources,
//   writeFSH,
//   logger,
//   stats,
//   fshingTrip,
//   getRandomPun,
//   ProcessingOptions,
//   loadExternalDependencies
// } from './utils';
// import { Package, AliasProcessor , ExportableAlias } from 'gofsh';
// import { ExportableAlias } from './exportable';

import { processor, utils, gofshExport, gofshClient } from "gofsh/dist/index";




// pattern = re.compile(f"(InstanceOf: \w*)\n(Usage: .+\n(\* meta\..*\n)*?)\* meta\.profile = \"{url}\"", re.RegexFlag.MULTILINE)
// content = pattern.sub(rf'InstanceOf: {profile_name}\n\2* meta.profile = "{url}"', content)

const Handler = async (request: HttpRequest, context: InvocationContext): Promise<HttpResponseInit> => {
    context.log(`Http function processed request for url "${request.url}"`);

    const fhir = !!request.query.get('demo') ? q : await request.text();

    // const fsh = await
    const fsh = await gofshClient.fhirToFsh([fhir])




    return {
        jsonBody: fsh
    }



    // return fsh;
};

app.http('FhirToFsh', {
    methods: ['POST'],
    authLevel: 'anonymous',
    handler: Handler
});
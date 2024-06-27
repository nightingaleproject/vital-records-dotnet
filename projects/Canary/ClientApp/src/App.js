import React from 'react';
import { Routes, Route, useParams, matchPath, useLocation } from 'react-router-dom';
import { ConnectathonDashboard } from './components/dashboard/ConnectathonDashboard';
import { Dashboard } from './components/dashboard/Dashboard';
import { Layout } from './components/Layout';
import { Connectathon } from './components/tests/Connectathon';
import { EDRSRoundtripConsuming } from './components/tests/EDRSRoundtripConsuming';
import { EDRSRoundtripProducing } from './components/tests/EDRSRoundtripProducing';
import { FHIRIJEValidatorProducing } from './components/tests/FHIRIJEValidatorProducing';
import { FHIRConsuming } from './components/tests/FHIRConsuming';
import { FHIRMessageProducing } from './components/tests/FHIRMessageProducing';
import { FHIRProducing } from './components/tests/FHIRProducing';
import { RecentTests } from './components/tests/RecentTests';
import { FHIRCreator } from './components/tools/FHIRCreator';
import { FHIRInspector } from './components/tools/FHIRInspector';
import { MessageInspector } from './components/tools/MessageInspector';
import { FHIRMessageCreator } from './components/tools/FHIRMessageCreator';
import { FHIRMessageSyntaxChecker } from './components/tools/FHIRMessageSyntaxChecker';
import { FHIRSyntaxChecker } from './components/tools/FHIRSyntaxChecker';
import { IJEInspector } from './components/tools/IJEInspector';
import { MessageConnectathonProducing } from './components/tests/MessageConnectathonProducing';
import { RecordConverter } from './components/tools/RecordConverter';
import { RecordGenerator } from './components/tools/RecordGenerator';
import { HomeScreen } from './components/HomeScreen';
import { Button } from 'semantic-ui-react';
import { Link } from 'react-router-dom';

export default function App() {

  // Get record type param.
  const { pathname } = useLocation()
  const match = matchPath(
    {
      path: "/:recordType/*",
    },
    pathname
  );
  const recordType = match ? match.params.recordType : '';
  if (recordType !== 'vrdr' && recordType !== 'bfdr-birth' && recordType !== 'bfdr-fetaldeath' && recordType !== '') {
    console.error(`Invalid route '${recordType}' due to invalid record type.`)
    return (
      <div>
        <div>
          Invalid record type '{recordType}'. Must be either 'VRDR', 'BFDR-Birth', or 'BFDR-FetalDeath'.
        </div>
        <Button as={Link} to={"/"}>
          Return to Canary Home.
        </Button>
      </div>
    );
  }
  const recordTypeReadable = (() => {
    if (recordType.toLowerCase() === 'vrdr') {
      return 'VRDR'
    } else if (recordType.toLowerCase() === 'bfdr-birth') {
      return 'BFDR Birth'
    } else if (recordType.toLowerCase() === 'bfdr-fetaldeath') {
      return 'BFDR Fetal Death'
    };
  })();

  /**
   * Adds parameters to the given component class type.
   * @param {*} WrappedComponent The component type to add prarams to.
   * @returns A new version of the component type with parameters.
   */
    const addParams = WrappedComponent => () => {
      const params = useParams();
      params['recordType'] = recordType;
      return (
        <WrappedComponent
          params={params}
        />
      );
    };

  // TODO - change these from React classes to functional components to prevent the need for param wrapping.
  const ConnectathonDashboardParams = addParams(ConnectathonDashboard)
  const ConnectathonParams = addParams(Connectathon)
  const MessageConnectathonProducingParams = addParams(MessageConnectathonProducing)
  const EDRSRoundtripConsumingParams = addParams(EDRSRoundtripConsuming)
  const EDRSRoundtripProducingParams = addParams(EDRSRoundtripProducing)

  // conditional routes based on record type (if vfdr, then do the connnectathon specific routes, etc.)

  return (
    <Layout recordType={recordType} recordTypeReadable={recordTypeReadable} >
      <Routes>
        <Route path="/" element={<HomeScreen />} />
        <Route path=":recordType/*">
          <Route index element={<Dashboard recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          <Route path="recent-tests" element={<RecentTests />} />
          <Route path="test-fhir-consuming">
            <Route index element={<FHIRConsuming recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
            <Route path=":id" element={<FHIRConsuming recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          </Route>
          <Route path="test-fhir-producing">
            <Route index element={<FHIRProducing recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
            <Route path=":id" element={<FHIRProducing recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          </Route>
          <Route path="test-fhir-message-producing">
            <Route index element={<FHIRMessageProducing recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
            <Route path=":id" element={<FHIRMessageProducing recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          </Route>
          <Route path="test-fhir-message-creation">
            <Route index element={<FHIRMessageCreator recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
            <Route path=":id" element={<FHIRMessageCreator recordType={recordType} />} recordTypeReadable={recordTypeReadable} />
          </Route>
          <Route path="test-edrs-roundtrip-consuming">
            <Route index element={<EDRSRoundtripConsumingParams />} />
            <Route path=":id" element={<EDRSRoundtripConsumingParams />} />
          </Route>
          <Route path="test-edrs-roundtrip-producing">
            <Route index element={<EDRSRoundtripProducingParams />} />
            <Route path=":id" element={<EDRSRoundtripProducingParams />} />
          </Route>
          <Route path="test-fhir-ije-validator-producing" element={<FHIRIJEValidatorProducing recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          <Route path="test-connectathon-dash/:type" element={<ConnectathonDashboardParams />} />
          <Route path="test-connectathon/:id" element={<ConnectathonParams />} />
          <Route path="test-connectathon-messaging/:id" element={<MessageConnectathonProducingParams />} />
          <Route path="tool-fhir-inspector" element={<FHIRInspector recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          <Route path="tool-fhir-creator" element={<FHIRCreator recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          <Route path="tool-fhir-syntax-checker" element={<FHIRSyntaxChecker recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          <Route path="tool-fhir-message-syntax-checker" element={<FHIRMessageSyntaxChecker recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          <Route path="tool-ije-inspector" element={<IJEInspector recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          <Route path="tool-record-converter" element={<RecordConverter recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          <Route path="tool-record-generator" element={<RecordGenerator recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
          <Route path="tool-message-inspector" element={<MessageInspector recordType={recordType} recordTypeReadable={recordTypeReadable} />} />
        </Route>
      </Routes>
    </Layout>
  );
}
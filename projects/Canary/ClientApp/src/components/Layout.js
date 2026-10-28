import React from 'react';
import { SemanticToastContainer } from 'react-semantic-toasts';
import { Container, Grid } from 'semantic-ui-react';
import { Navigation } from './Navigation';

export function Layout(props) {

  return (
    <React.Fragment>
      <SemanticToastContainer />
      <div className="canary-app-shell">
        <header className="canary-app-header">
          <Navigation recordType={props.recordType} recordTypeReadable={props.recordTypeReadable} ijeType={props.ijeType} 
            showFHIRProducingActions={props.showFHIRProducingActions}                           // Item 1.1
            showFHIRConsumingActions={props.showFHIRConsumingActions}                           // Item 1.2
            showEDRSConsumingActions={props.showEDRSConsumingActions}                           // Item 1.3
            showEDRSProducingActions={props.showEDRSProducingActions}                           // Item 1.4
            showConnectathonFHIRProducingActions={props.showConnectathonFHIRProducingActions}   // Item 1.5
            showValidateFHIRWithIJEActions={props.showValidateFHIRWithIJEActions}               // Item 1.6
            showProdFHIRWithMsg={props.showProdFHIRWithMsg}                                     // Item 2.1 
            showTestConnectathonMsgPage={props.showTestConnectathonMsgPage}                     // Item 2.2  
            showToolFhirCreator={props.showToolFhirCreator}                                     // Item 3.5
            showMessageCreatorActions={props.showMessageCreatorActions}                         // Item 4.2
            />
        </header>
        <main className="canary-app-content">
          <Container>
            <Grid>{props.children}</Grid>
          </Container>
        </main>
      </div>
    </React.Fragment>
  );
}

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
          <Navigation recordType={props.recordType} recordTypeReadable={props.recordTypeReadable} ijeType={props.ijeType}/>
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

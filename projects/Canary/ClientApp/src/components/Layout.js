import React from 'react';
import { SemanticToastContainer } from 'react-semantic-toasts';
import { Container, Grid } from 'semantic-ui-react';
import { Navigation } from './Navigation';

export function Layout(props) {

  return (
    <React.Fragment>
      <SemanticToastContainer />
      <Navigation recordType={props.recordType} recordTypeReadable={props.recordTypeReadable} ijeType={props.ijeType}/>
      <Container>
        <Grid>{props.children}</Grid>
      </Container>
    </React.Fragment>
  );
}

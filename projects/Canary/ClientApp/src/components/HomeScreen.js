import React, { Component } from 'react';
import { Card } from 'semantic-ui-react';
import { Container, Divider, Grid, Header, Icon, Item, Segment } from 'semantic-ui-react';
import { Link } from 'react-router-dom';

const CanaryCard = (props) => (
  <Card as={Link} to={props.link} centered>
    <Card.Content>
      <Card.Header>{props.title}</Card.Header>
      <Card.Description>
        {props.subtitle}
      </Card.Description>
    </Card.Content>
  </Card>
)

export class HomeScreen extends Component {
  displayName = HomeScreen.name;

  render() {
    return (
      <React.Fragment>
        <Container className="p-b-50">
          <Divider horizontal className="p-t-30">
            <Header as="h2">
              <Icon name="clipboard list" />
              Select a Record Type to Test
            </Header>
          </Divider>
          <div className="p-t-30" />
          <CanaryCard
            title='Vital Records Death Reporting'
            subtitle='VRDR STU2'
            link='/vrdr'
          />
          <CanaryCard 
            title='BFDR Birth Reporting'
            subtitle={`BFDR ${window.BFDR_VERSION}`}
            link='/bfdr-birth'
          />
          <CanaryCard 
            title='BFDR Fetal Death Reporting'
            subtitle={`BFDR ${window.BFDR_VERSION}`}
            link='/bfdr-fetaldeath'
          />
        </Container>
      </React.Fragment>
    );
  }
}
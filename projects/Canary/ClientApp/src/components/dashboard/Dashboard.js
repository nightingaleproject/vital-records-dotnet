import { faFeatherAlt } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React from 'react';
import { Container, Divider, Grid, Header, Icon, Item, Segment } from 'semantic-ui-react';
import { DashboardItem } from './DashboardItem';
import * as NavigationOptions from '../NavigationOptions';

export function Dashboard(props) {

  const recordType = props.recordType.toUpperCase();
  const ijeDataType = (() => {
    if (recordType === 'VRDR') {
      return 'Mortality'
    } else if (recordType === 'BFDR-BIRTH') {
      return 'Natality'
    } else if (recordType === 'BFDR-FETALDEATH') {
      return 'Fetal Death'
    };
  })();
  const recordTypeStr = (() => {
    if (recordType === 'VRDR') {
      return 'VRDR'
    } else if (recordType === 'BFDR-BIRTH') {
      return 'BFDR Birth'
    } else if (recordType === 'BFDR-FETALDEATH') {
      return 'BFDR Fetal Death'
    };
  })();

  return (
    <React.Fragment>
      <Grid centered columns={1}>
        <Grid.Column width={15}>
          <Container className="p-b-50">
            <Divider horizontal>
              <Header as="h2">
                <FontAwesomeIcon icon={faFeatherAlt} size="lg" fixedWidth className="p-r-5" />
                Open Source {ijeDataType} Data Standards Testing
              </Header>
            </Divider>
            <Segment size="large" basic>
              <Container>
                <p>
                  Canary is a testing framework that supports development of systems that perform standards based exchange of {ijeDataType.toLowerCase()} data, providing tests
                  and tools to aid developers in implementing the <a href="https://hl7.org/fhir/us/vrdr/">FHIR Vital Records Death Record</a> format. TODO, this should change based on BFDR/VRDR.
                </p>
              </Container>
            </Segment>
            <Divider horizontal className="p-t-30">
              <Header as="h2">
                <Icon name="clipboard list" />
                Record Testing
              </Header>
            </Divider>
            <Item.Group className="m-h-30">
              {NavigationOptions.RecordTesting(recordTypeStr).map((navigationOption) => {
                return (
                  <DashboardItem
                    key={navigationOption.title}
                    icon={navigationOption.icon}
                    title={navigationOption.title}
                    description={navigationOption.description}
                    route={navigationOption.route}
                  />
                )
              })}
            </Item.Group>
            <Divider horizontal className="p-t-30">
              <Header as="h2">
                <Icon name="inbox" />
                Message Testing
              </Header>
            </Divider>
            <Item.Group className="m-h-30">
              {NavigationOptions.MessageTesting(recordTypeStr).map((navigationOption) => {
                return (
                  <DashboardItem
                    key={navigationOption.title}
                    icon={navigationOption.icon}
                    faIcon={navigationOption.faIcon}
                    title={navigationOption.title}
                    description={navigationOption.description}
                    route={navigationOption.route}
                  />
                )
              })}
            </Item.Group>
            <Divider horizontal className="p-t-20">
              <Header as="h2">
                <Icon name="wrench" />
                Record Tools
              </Header>
            </Divider>
            <Item.Group className="m-h-30">
              {NavigationOptions.RecordTools(recordTypeStr, ijeDataType).map((navigationOption) => {
                return (
                  <DashboardItem
                    key={navigationOption.title}
                    icon={navigationOption.icon}
                    title={navigationOption.title}
                    description={navigationOption.description}
                    route={navigationOption.route}
                  />
                )
              })}
            </Item.Group>
            <Divider horizontal className="p-t-20">
              <Header as="h2">
                <Icon name="envelope open" />
                Message Tools
              </Header>
            </Divider>
            <Item.Group className="m-h-30">
              {NavigationOptions.MessageTools(recordTypeStr, ijeDataType).map((navigationOption) => {
                return (
                  <DashboardItem
                    key={navigationOption.title}
                    icon={navigationOption.icon}
                    title={navigationOption.title}
                    description={navigationOption.description}
                    route={navigationOption.route}
                  />
                )
              })}
            </Item.Group>
          </Container>
        </Grid.Column>
      </Grid>
    </React.Fragment>
  );
}

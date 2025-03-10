import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Breadcrumb, Button, Container, Dimmer, Divider, Dropdown, Input, Form, Grid, Header, Icon, Loader, Menu, Message, Statistic, Transition } from 'semantic-ui-react';
import { Getter } from '../misc/Getter';
import { FHIRInfo } from '../misc/info/FHIRInfo';
import { Record } from '../misc/Record';

export class MessageFshConverter extends Component {
    displayName = MessageFshConverter.name;

  constructor(props) {
    super(props);
    this.state = { ...this.props, record: null, fhirInfo: null, issues: null };
    this.updateRecord = this.updateRecord.bind(this);
  }

  updateRecord(record, issues) {
    if (record && record.fhirInfo) {
      this.setState({ record: null, fhirInfo: null, issues: null }, () => {
        this.setState({ record: record, fhirInfo: record.fhirInfo, issues: issues }, () => {
        });
      })
    } else if (issues && issues.length > 0) {
      this.setState({ issues: issues, fhirInfo: null });
    }
  }

  render() {
      return (
        
      <React.Fragment>
        <Grid>
          <Grid.Row>
            <Breadcrumb>
              <Breadcrumb.Section as={Link} to="/">
                Dashboard
              </Breadcrumb.Section>
              <Breadcrumb.Divider icon="right chevron" />
              <Breadcrumb.Section>FHIR Message to FSH Converter</Breadcrumb.Section>
            </Breadcrumb>
          </Grid.Row>
          <Grid.Row>
            <Getter updateRecord={this.updateRecord} recordType={this.props.recordType} strict allowIje={false} source={"MessageFshConverter"} />
          </Grid.Row>
                  {!!this.state.fhirInfo && (
                      <Grid.Row>
                          <Container fluid>
                              <Divider horizontal />
                              <Header as="h2" dividing id="step-2">
                                  <Icon name="download" />
                                  <Header.Content>
                                      Whole message content.  
                                      <Header.Subheader>
                                          Formatted as FSH. You can download, copy or POST this data.
                                      </Header.Subheader>
                                  </Header.Content>
                              </Header>
                              <div className="p-b-15" />
                              <Record record={this.state.record} showSave lines={20} ijeOnly={true} hideIje={true} showFsh showIssues source={"MessageFshConverter"} />
                          </Container>
                      </Grid.Row>
                  )}
          <div className="p-b-15" />
            {!!this.state.issues && this.state.issues.length > 0 && (
                <Grid.Row>
                    <Record record={null} issues={this.state.issues} showIssues source={"MessageFshConverter"} />
                </Grid.Row>
          )}
         </Grid>
      </React.Fragment>
      );
  }
}

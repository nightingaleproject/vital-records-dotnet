import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Breadcrumb, Button, Container, Dimmer, Divider, Dropdown, Input, Form, Grid, Header, Icon, Loader, Menu, Message, Statistic, Transition } from 'semantic-ui-react';
import { Getter } from '../misc/Getter';
import { FHIRInfo } from '../misc/info/FHIRInfo';
import { Record } from '../misc/Record';

export class FshSushiInspector extends Component {
  displayName = FshSushiInspector.name;

  constructor(props) {
    super(props);
    this.state = { ...this.props, record: null, fhirInfo: null, issues: null };
    this.updateRecord = this.updateRecord.bind(this);
  }

  updateRecord(record, issues) {
    if (record && record.fhirInfo) {
      this.setState({ record: null, fhirInfo: null, issues: null }, () => {
        this.setState({ record: record, fhirInfo: record.fhirInfo, issues: issues }, () => {
          document.getElementById('scroll-to').scrollIntoView({ behavior: 'smooth', block: 'start' });
        });
      })
    } else if (issues) {
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
              <Breadcrumb.Section>FSH {this.props.recordType.toUpperCase()} Inspector</Breadcrumb.Section>
            </Breadcrumb>
          </Grid.Row>
          <Grid.Row>
           <Getter updateRecord={this.updateRecord} recordType={this.props.recordType} strict allowIje={false} source={"FshSushiInspector"} />
          </Grid.Row>
                  {!!this.state.fhirInfo && (
                      <Grid.Row>
                          <Container fluid>
                              <Divider horizontal />
                              <Header as="h2" dividing id="step-2">
                                  <Icon name="download" />
                                  <Header.Content>
                                      Whole message content.  Select required format.
                                      <Header.Subheader>
                                          The errors and issues are displayed below the FSH content.
                                      </Header.Subheader>
                                  </Header.Content>
                              </Header>
                              <div className="p-b-15" />
                              <Record record={this.state.record} issues={this.state.issues} messageType={"FSH"} showSave messageValidation={true} lines={20} showIje issues={this.state.issues} showIssues showSuccess />
                          </Container>
                      </Grid.Row>
                  )}
          <div className="p-b-15" />
                <Grid.Row>
                    <Record record={null} issues={this.state.issues}  messageType={"FSH"} messageValidation={true} showIssues showSuccess />
                </Grid.Row>
          {!!this.state.fhirInfo && (
            <Grid.Row>
              <Container fluid>
                <Form size="large" id="scroll-to">
                  <FHIRInfo fhirInfo={this.state.fhirInfo} editable={false} />
                </Form>
              </Container>
              <div className="p-b-50" />
            </Grid.Row>
          )}
            </Grid>
      </React.Fragment>
      );
  }
}

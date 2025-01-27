import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Breadcrumb, Grid } from 'semantic-ui-react';
import { messageTypesVRDR, messageTypesBirth, messageTypesFetalDeath } from '../../data';
import { Getter } from '../misc/Getter';
import { Record } from '../misc/Record';

export const getMessageType = (recordType, message) => {
  if (recordType.toLowerCase() == 'vrdr' && message && message.messageType in messageTypesVRDR) {
      return messageTypesVRDR[message.messageType];
  }
  if (recordType.toLowerCase() == 'bfdr-birth' && message && message.messageType in messageTypesBirth) {
    return messageTypesBirth[message.messageType];
  }
  if (recordType.toLowerCase() == 'bfdr-fetaldeath' && message && message.messageType in messageTypesFetalDeath) {
    return messageTypesFetalDeath[message.messageType];
  }
  console.log(recordType);
  console.log(message);
  return "Unknown";
}

export class FHIRMessageSyntaxChecker extends Component {
  displayName = FHIRMessageSyntaxChecker.name;

  constructor(props) {
    super(props);
    this.state = { ...this.props, messageType: null, issues: null };
    this.updateMessage = this.updateMessage.bind(this);
  }

  updateMessage(message, issues) {
    let messageType = getMessageType(this.props.recordType, message);
    this.setState({ message: message, messageType: messageType, issues: issues });
  }

  render() {
    return (
      <React.Fragment>
        <Grid>
          <Grid.Row>
            <Breadcrumb>
              <Breadcrumb.Section as={Link} to={`/${this.props.recordType}`}>
                Dashboard
              </Breadcrumb.Section>
              <Breadcrumb.Divider icon="right chevron" />
              <Breadcrumb.Section>FHIR {this.props.recordTypeReadable} Message Syntax Checker</Breadcrumb.Section>
            </Breadcrumb>
          </Grid.Row>
          <Grid.Row>
            <Getter updateRecord={this.updateMessage} recordType={this.props.recordType} strict messageValidation={true} allowIje={false} />
          </Grid.Row>
          <div className="p-b-15" />
          {!!this.state.issues && (
            <Grid.Row>
              <Record record={null} issues={this.state.issues} messageType={this.state.messageType} messageValidation={true} showIssues showSuccess />
            </Grid.Row>
          )}
        </Grid>
      </React.Fragment>
    );
  }
}

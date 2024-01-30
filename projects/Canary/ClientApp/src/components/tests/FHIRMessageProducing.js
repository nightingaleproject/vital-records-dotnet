import axios from 'axios';
import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Breadcrumb, Button, Container, Dimmer, Divider, Form, Grid, Header, Icon, Loader, Menu, Message, Statistic, Transition } from 'semantic-ui-react';
import { responseMessageTypeIcons, messageTypeIcons, messageTypes } from '../../data';
import { connectionErrorToast } from '../../error';
import { Getter } from '../misc/Getter';
import { FHIRInfo } from '../misc/info/FHIRInfo';
import { Record } from '../misc/Record';
import report from '../report';
import { useParams } from 'react-router-dom';

export function FHIRMessageProducing(props) {

  const { id } = useParams();

  const [test, setTest] = React.useState();
  const [loading, setLoading] = React.useState(true);
  const [results] = React.useState();
  const [record, setRecord] = React.useState();
  const [running, setRunning] = React.useState();
  const [issues, setIssues] = React.useState();
  const [message, setMessage] = React.useState();
  const [actualType, setActualType] = React.useState();
  const [expectedType, setExpectedType] = React.useState();
  const [responseOptions, setResponseOptions] = React.useState();
  const [response, setResponse] = React.useState();
  const [responses, setResponses] = React.useState();

  useEffect(() => {
    if (!!id) {
      axios
        .get(`${window.API_URL}/tests/${props.recordType}/${id}`)
        .then(function (response) {
          var test = response.data;
          test.results = JSON.parse(test.results);
          setTest(test);
          setRecord(test.referenceRecord);
          setLoading(false);
        })
        .catch(function (error) {
          setLoading(false);
          connectionErrorToast(error);
        });
    } else {
      axios
        .get(`${window.API_URL}/tests/${props.recordType}/new`)
        .then(function (response) {
          setTest(response.data);
          setRecord(response.data.referenceRecord);
          setLoading(false);
        })
        .catch(function (error) {
          setLoading(false);
          connectionErrorToast(error);
        });
    }
  }, []);

  const updateMessage = (message, issues) => {
    let messageType = "Unknown";
    if (message && message.messageType in messageTypes) {
      messageType = messageTypes[message.messageType];
    }

    /*
     * Only perform this when there are no other issues, since receiving errors here means
     * the message was probably not parsed correctly.
    */
    if (messageType !== expectedType && issues instanceof Array && !issues.length) {
      issues.push({
        'message': `Unexpected message type encountered, received a message of type ${messageType} but expected a message of type ${expectedType}.`,
        'severity': 'error'
      });
    }
    setMessage(message);
    setActualType(messageType);
    setIssues(issues);
  }

  const setExpectedMessageType = (_, { name }) => {
    setExpectedType(name);

    if (name === "Void") {
      // void only provides a subset
      var voidIcons = [responseMessageTypeIcons[0], responseMessageTypeIcons[3]];
      setResponseOptions(voidIcons);
    } else {
      setResponseOptions(responseMessageTypeIcons);
    }
  }

  const setDisplayMessageResponseType = (_, { name }) => {
    setResponse(responses[name]);
  }

  const setEmptyToNull = (obj) => {
    const o = JSON.parse(JSON.stringify(obj));
    Object.keys(o).forEach(key => {
      if (o[key] && typeof o[key] === 'object') o[key] = setEmptyToNull(o[key]);
      else if (o[key] === undefined || o[key] === null || (!!!o[key] && o['Type'] !== 'Bool')) o[key] = null;
      // eslint-disable-next-line
      else o[key] = o[key];
    });
    return o;
  }

  const runTest = () => {
    setRunning(true);
    axios
      .post(`${window.API_URL}/tests/${props.recordType}/${expectedType}MessageProducing/run/${test.testId}`, message.json, { headers: { 'Content-Type': 'application/json' } })
      .then(function (response) {
        var test = response.data;
        test.results = JSON.parse(test.results);
        setTest(test);
        setRunning(false);
        return axios.post(`${window.API_URL}/tests/${props.recordType}/${expectedType}/response`, message.json, { headers: { 'Content-Type': 'application/json' } });
      })
      .then(function (response) {
        setResponses(response.data);
        setLoading(false);
        document.getElementById('scroll-to').scrollIntoView({
          behavior: 'smooth',
          block: 'start'
        });
      })
      .catch(function (error) {
        setLoading(false);
        setRunning(false)
        connectionErrorToast(error);
      });
  }

  const downloadAsFile = (contents) => {
    var element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(contents));
    element.setAttribute('download', `canary-report-${new Date().getTime()}.html`);
    element.click();
  }

  return (
    <React.Fragment>
      <Grid id="scroll-to">
        <Grid.Row>
          <Breadcrumb>
            <Breadcrumb.Section as={Link} to={`/${props.recordType}`}>
              Dashboard
            </Breadcrumb.Section>
            <Breadcrumb.Divider icon="right chevron" />
            <Breadcrumb.Section>Producing FHIR {props.recordType.toUpperCase()} Messages</Breadcrumb.Section>
          </Breadcrumb>
        </Grid.Row>
        {!!test && test.completedBool && (
          <React.Fragment>
            <Grid.Row>
              <Container fluid>
                <Divider horizontal />
                <Header as="h2" dividing id="step-5">
                  <Icon name="mail" />
                  <Header.Content>
                    View the Response Message
                    <Header.Subheader>Select the type of response message you would like to view.</Header.Subheader>
                  </Header.Content>
                </Header>
                {!!responseOptions && (
                  <Menu items={responseOptions} widths={responseOptions.length} onItemClick={setDisplayMessageResponseType} />
                )}
              </Container>
            </Grid.Row>
            <Grid.Row>
              <Container fluid>
                <Record record={response} showSave lines={20} messageValidation={false} hideIje />
              </Container>
            </Grid.Row>
            <div className="p-b-10" />
            <Grid.Row className="loader-height">
              <Container>
                <div className="p-b-10" />
                <Statistic.Group widths="three">
                  <Statistic size="large">
                    <Statistic.Value>{test.total}</Statistic.Value>
                    <Statistic.Label>Properties Checked</Statistic.Label>
                  </Statistic>
                  <Statistic size="large" color="green">
                    <Statistic.Value>
                      <Icon name="check circle" />
                      {test.correct}
                    </Statistic.Value>
                    <Statistic.Label>Correct</Statistic.Label>
                  </Statistic>
                  <Statistic size="large" color="red">
                    <Statistic.Value>
                      <Icon name="times circle" />
                      {test.incorrect}
                    </Statistic.Value>
                    <Statistic.Label>Incorrect</Statistic.Label>
                  </Statistic>
                </Statistic.Group>
                <Grid centered columns={1} className="p-t-30 p-b-15">
                  <Button icon labelPosition='left' primary onClick={() => downloadAsFile(report(test, null))}><Icon name='download' />Generate Downloadable Report</Button>
                </Grid>
                <div className="p-b-20" />
                <Form size="large">
                  <FHIRInfo fhirInfo={test.results} hideSnippets={true} editable={false} testMode={true} />
                </Form>
              </Container>
            </Grid.Row>
          </React.Fragment>
        )}
        {!(!!test && test.completedBool) && !!loading && (
          <Grid.Row className="loader-height">
            <Container>
              <Dimmer active inverted>
                {!!id && <Loader size="massive">Loading Test...</Loader>}
                {!!!id && <Loader size="massive">Initializing a New Test...</Loader>}
              </Dimmer>
            </Container>
          </Grid.Row>
        )}
        {!(!!test && test.completedBool) && !!test && (
          <React.Fragment>
            <Grid.Row>
              <Container fluid>
                <Divider horizontal />
                <Header as="h2" dividing id="step-1">
                  <Icon name="download" />
                  <Header.Content>
                    Step 1: Import Record
                    <Header.Subheader>
                      Import the generated record into your system. The below prompt allows you to copy the record, download it as a file, or POST it to
                      an endpoint.
                    </Header.Subheader>
                  </Header.Content>
                </Header>
                <div className="p-b-15" />
                <Record record={record} showSave lines={20} showIje />
              </Container>
            </Grid.Row>
            <Grid.Row>
              <Container fluid>
                <Divider horizontal />
                <Header as="h2" dividing id="step-2">
                  <Icon name="mail" />
                  <Header.Content>
                    Step 2: Choose the Message Type
                    <Header.Subheader>Select the type of message you would like Canary to validate.</Header.Subheader>
                  </Header.Content>
                </Header>
                <Menu items={messageTypeIcons} widths={messageTypeIcons.length} onItemClick={setExpectedMessageType} />
              </Container>
            </Grid.Row>
            <Grid.Row>
              {!!expectedType &&
                <div className="inherit-width">
                  <Transition transitionOnMount animation="fade" duration={1000}>
                    <div className="inherit-width">
                      <Message icon size="large" info>
                        <Icon name="info circle" />
                        <Message.Content>Canary will expect a {expectedType} Message</Message.Content>
                      </Message>
                    </div>
                  </Transition>
                </div>
              }
            </Grid.Row>
            {!!expectedType &&
              <React.Fragment>
                <Grid.Row>
                  <Container fluid>
                    <Divider horizontal />
                    <Header as="h2" dividing id="step-3">
                      <Icon name="keyboard" />
                      <Header.Content>
                        Step 3: Export Message
                        <Header.Subheader>
                          Export a {expectedType} message for the record above and import it into Canary using the tool below.
                        </Header.Subheader>
                      </Header.Content>
                    </Header>
                    <div className="p-b-10" />
                    <Getter updateRecord={updateMessage} strict messageValidation={true} allowIje={false} recordType={props.recordType}/>
                  </Container>
                </Grid.Row>
                <div className="p-b-15" />
                {!!issues &&
                  <Grid.Row>
                    <Record record={null} issues={issues} messageType={actualType} messageValidation={true} showIssues showSuccess />
                  </Grid.Row>
                }
                <Grid.Row>
                  <Container fluid>
                    <Divider horizontal />
                    <Header as="h2" dividing className="p-b-5" id="step-4">
                      <Icon name="check circle" />
                      <Header.Content>
                        Step 4: Calculate Results
                        <Header.Subheader>
                          When you have imported the message into Canary, click the button below and Canary will calculate the results of the test.
                        </Header.Subheader>
                      </Header.Content>
                    </Header>
                    <div className="p-b-10" />
                    <Button fluid size="huge" primary onClick={runTest} loading={running} disabled={!!!message}>
                      Calculate
                    </Button>
                  </Container>
                </Grid.Row>
              </React.Fragment>
            }
          </React.Fragment>
        )}
      </Grid>
    </React.Fragment>
  );
}

import axios from 'axios';
import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Breadcrumb, Button, Container, Dimmer, Divider, Form, Grid, Header, Icon, Loader, Statistic } from 'semantic-ui-react';
import { connectionErrorToast } from '../../error';
import { Getter } from '../misc/Getter';
import { FHIRInfo } from '../misc/info/FHIRInfo';
import { Record } from '../misc/Record';
import report from '../report';
import { useParams } from 'react-router-dom';

export function FHIRProducing(props) {

  const { id } = useParams();

  const [test, setTest] = React.useState();
  const [loading, setLoading] = React.useState(true);
  const [record, setRecord] = React.useState();
  const [results] = React.useState();
  const [fhirInfo, setFhirInfo] = React.useState();
  const [running, setRunning] = React.useState();
  const [issues, setIssues] = React.useState();

  useEffect(() => {
    var self = this;
    if (!!id) {
      axios
        .get(`${window.API_URL}/tests/${props.recordType}/${id}`)
        .then(function (response) {
          var test = response.data;
          test.results = JSON.parse(test.results);
          setTest(test);
          setFhirInfo(JSON.parse(response.data.referenceRecord.fhirInfo));
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
          setFhirInfo(JSON.parse(response.data.referenceRecord.fhirInfo));
          setLoading(false);
        })
        .catch(function (error) {
          setLoading(false)
          connectionErrorToast(error);
        });
    }
  }, []);

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

  const updateRecord = (record, issues) => {
    setRecord(record);
    setIssues(issues);
  }

  const updateTest = (test) => {
    setTest(test);
  }

  const runTest = () => {
    var self = this;
    setRunning(true);
    axios
      .post(`${window.API_URL}/tests/${props.recordType}/Produce/run/${test.testId}`, setEmptyToNull(record.fhirInfo))
      .then(function (response) {
        var test = response.data;
        test.results = JSON.parse(test.results);
        setTest(test);
        setRunning(false);
        document.getElementById('scroll-to').scrollIntoView({ behavior: 'smooth', block: 'start' });
      })
      .catch(function (error) {
        setLoading(false);
        setRunning(false);
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
            <Breadcrumb.Section>Producing FHIR {props.recordTypeReadable} Records</Breadcrumb.Section>
          </Breadcrumb>
        </Grid.Row>
        {!!test && test.completedBool && (
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
                <FHIRInfo fhirInfo={test.results} editable={false} testMode={true} updateFhirInfo={null} />
              </Form>
            </Container>
          </Grid.Row>
        )}
        {!(test && test.completedBool) && !!loading && (
          <Grid.Row className="loader-height">
            <Container>
              <Dimmer active inverted>
                <Loader size="massive">Initializing a New Test...</Loader>
              </Dimmer>
            </Container>
          </Grid.Row>
        )}
        {!(test && test.completedBool) && !!test && (
          <React.Fragment>
            <Grid.Row>
              <Container fluid>
                <Divider horizontal />
                <Header as="h2" dividing id="step-1">
                  <Icon name="keyboard" />
                  <Header.Content>
                    Step 1: Enter Details
                    <Header.Subheader>Enter the details listed below into your system.</Header.Subheader>
                  </Header.Content>
                </Header>
                <div className="p-b-10" />
                {!!fhirInfo && (
                  <Form size="large">
                    <FHIRInfo fhirInfo={fhirInfo} updateFhirInfo={null} editable={false} hideSnippets />
                  </Form>
                )}
              </Container>
            </Grid.Row>
            <Grid.Row>
              <Container fluid>
                <Divider horizontal />
                <Header as="h2" dividing id="step-1">
                  <Icon name="download" />
                  <Header.Content>
                    Step 2: Export Record
                    <Header.Subheader>After entering the record into your system, export it and import it into Canary using the form below.</Header.Subheader>
                  </Header.Content>
                </Header>
                <div className="p-b-10" />
                <Getter updateRecord={updateRecord} allowIje={false} recordType={props.recordType} />
              </Container>
            </Grid.Row>
            <div className="p-b-15" />
            {!!issues && (
              <Grid.Row id="scroll-to">
                <Record record={null} issues={issues} showIssues showSuccess />
              </Grid.Row>
            )}
            <Grid.Row>
              <Container fluid>
                <Divider horizontal />
                <Header as="h2" dividing className="p-b-5" id="step-1">
                  <Icon name="check circle" />
                  <Header.Content>
                    Step 3: Calculate Results
                    <Header.Subheader>
                      When you have imported the record, click the button below and Canary will calculate the results of the test.
                    </Header.Subheader>
                  </Header.Content>
                </Header>
                <div className="p-b-10" />
                <Button
                  fluid
                  size="huge"
                  primary
                  onClick={runTest}
                  loading={running}
                  disabled={!!!(record && record.xml)}
                >
                  Calculate
                </Button>
              </Container>
            </Grid.Row>
          </React.Fragment>
        )}
      </Grid>
      <div className="p-b-100" />
    </React.Fragment>
  );
}

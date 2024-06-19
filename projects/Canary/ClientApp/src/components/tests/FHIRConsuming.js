import axios from 'axios';
import _ from 'lodash';
import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Breadcrumb, Button, Container, Dimmer, Divider, Form, Grid, Header, Icon, Loader, Statistic } from 'semantic-ui-react';
import { connectionErrorToast } from '../../error';
import { FHIRInfo } from '../misc/info/FHIRInfo';
import { Record } from '../misc/Record';
import report from '../report';
import { useParams } from 'react-router-dom';

export function FHIRConsuming(props) {

  const { id } = useParams();
  const recordType = props.recordType;

  const [test, setTest] = React.useState();
  const [loading, setLoading] = React.useState(true);
  const [record, setRecord] = React.useState();
  const [results] = React.useState();
  const [fhirInfo, setFhirInfo] = React.useState();
  const [running, setRunning] = React.useState();
  const [issues, setIssues] = React.useState();

  useEffect(() => {
    if (!!id) {
      axios
        .get(`${window.API_URL}/tests/${recordType}/${id}`)
        .then(function (response) {
          var test = response.data;
          test.results = JSON.parse(test.results);
          setTest(test);
          setLoading(false);
        })
        .catch(function (error) {
          setLoading(false);
          connectionErrorToast(error);
        });
    } else {
      axios
        .get(`${window.API_URL}/records/${recordType}/description`)
        .then(function (response) {
          setFhirInfo(response.data);
          axios
            .get(`${window.API_URL}/tests/${recordType}/new`)
            .then(function (response) {
              setTest(response.data);
              setLoading(false);
            })
            .catch(function (error) {
              setLoading(false);
              connectionErrorToast(error);
            });
        })
        .catch(function (error) {
          setLoading(false);
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

  const updateFhirInfo = (path, value) => {
    var fhirInfo = { ...fhirInfo };
    _.set(fhirInfo, path, value);
    setFhirInfo(fhirInfo);
  }

  const runTest = () => {
    setRunning(true);
    axios
      .post(`${window.API_URL}/tests/${recordType}/Consume/run/${test.testId}`, setEmptyToNull(fhirInfo))
      .then(function (response) {
        var test = response.data;
        test.results = JSON.parse(test.results);
        setRunning(false);
        setTest(test);
        document.getElementById('scroll-to').scrollIntoView({ behavior: 'smooth', block: 'start' });
      })
      .catch(function (error) {
        setRunning(false);
        setLoading(false);
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
            <Breadcrumb.Section as={Link} to={`/${recordType}`}>
              Dashboard
            </Breadcrumb.Section>
            <Breadcrumb.Divider icon="right chevron" />
            <Breadcrumb.Section>Consuming FHIR {props.recordTypeReadable} Records</Breadcrumb.Section>
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
                <FHIRInfo fhirInfo={test.results} editable={false} testMode={true} />
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
              <Container>
                <Divider horizontal />
                <Header as="h2" dividing id="step-1">
                  <Icon name="download" />
                  <Header.Content>
                    Step 1: Import Record
                    <Header.Subheader>
                      Import the generated record into your system. The below prompt allows you to copy the record, download it as a file, or POST it to an
                      endpoint.
                    </Header.Subheader>
                  </Header.Content>
                </Header>
                <div className="p-b-15" />
                <Record record={test.referenceRecord} showSave lines={20} hideIje />
              </Container>
            </Grid.Row>
            <Grid.Row>
              <Container fluid>
                <Divider horizontal />
                <Header as="h2" dividing id="step-1">
                  <Icon name="keyboard" />
                  <Header.Content>
                    Step 2: Enter Details
                    <Header.Subheader>
                      Once you have imported the record into your system, enter the details that show up in your system below. When you are finished, move on
                      to step 3.
                    </Header.Subheader>
                  </Header.Content>
                </Header>
                <div className="p-b-10" />
                <Form size="large">
                  <FHIRInfo fhirInfo={fhirInfo} updateFhirInfo={updateFhirInfo} editable={true} hideSnippets />
                </Form>
              </Container>
            </Grid.Row>
            <Grid.Row>
              <Container fluid>
                <Divider horizontal />
                <Header as="h2" dividing className="p-b-5" id="step-1">
                  <Icon name="check circle" />
                  <Header.Content>
                    Step 3: Calculate Results
                    <Header.Subheader>
                      When you are finished entering the details from your system, click the button below and Canary will calculate the results of the test.
                    </Header.Subheader>
                  </Header.Content>
                </Header>
                <div className="p-b-10" />
                <Button fluid size="huge" primary onClick={runTest} loading={running}>
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

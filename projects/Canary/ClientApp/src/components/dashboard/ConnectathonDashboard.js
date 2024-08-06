import React, { Component } from 'react';
import { Container, Divider, Grid, Header, Icon, Item } from 'semantic-ui-react';
import { DashboardItem } from './DashboardItem';
import axios from 'axios';
import { connectionErrorToast } from '../../error';


export class ConnectathonDashboard extends Component {
  displayName = ConnectathonDashboard.name;

  constructor(props) {
    super(props);
    this.state = { ...this.props, records: null, loading: false };    
  }

  componentDidMount() {
    this.fetchRecords()
  }

  fetchRecords() {
    var self = this;
    axios
      .get(`${window.API_URL}/connectathon/${this.props.params.recordType}`)
      .then(function (response) {
        var records = response.data;
        self.setState({ records: records, loading: false });
      })
      .catch(function (error) {
        self.setState({ loading: false }, () => {
          connectionErrorToast(error);
        });
      });
  };

  getUrl(id) {
    if (this.state.params.type === "message") {
      return `${window.API_URL}/${this.props.params.recordType}/test-connectathon-messaging/${id}`
    }
    else {
      return `${window.API_URL}/${this.props.params.recordType}/test-connectathon/${id}`
    }
  }

  render() {
    const isVRDR = this.props.params.recordType.toLowerCase() == 'vrdr';
    const sexKey = isVRDR ? 'sexAtDeath' : 'birthSex';
    const familyNameKey = isVRDR ? 'familyName' : 'childFamilyName';
    const givenNamesKey = isVRDR ? 'givenNames' : 'childGivenNames';
    const descriptionKey = isVRDR ? 'coD1A' : 'dateOfBirth';
    return (
      <React.Fragment>
        <Grid centered columns={1}>
          <Grid.Column width={15}>
            <Container className="p-b-50">
              <Divider horizontal>
                <Header as="h2">
                  <Icon name="clipboard list" />
                  Connectathon {this.state.params.type} testing
                </Header>
              </Divider>
              <Item.Group className="m-h-30">
                {
                  !!this.state.records && this.state.records.map((x, i) =>
                    <DashboardItem
                      key={i}
                      icon={!!x[sexKey] && x[sexKey]['code'] || 'male'}
                      title={`#${i + 1}: ${x[familyNameKey] ?? '[null]'}, ${x[givenNamesKey] != '' ? x[givenNamesKey].join(' ') : "[null]"}`}
                      description={`${x[descriptionKey]}`}
                      route={this.getUrl(i + 1)}
                    />
                  )
                }
              </Item.Group>
            </Container>
          </Grid.Column>
        </Grid>
      </React.Fragment>
    );
  }
}
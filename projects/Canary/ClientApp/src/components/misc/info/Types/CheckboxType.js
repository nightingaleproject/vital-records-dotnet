import React, { Component } from 'react';
import { Form, FormField, Checkbox } from 'semantic-ui-react';

export class CheckboxType extends Component {
  displayName = CheckboxType.name;

  constructor(props) {
    super(props);
    this.state = { ...this.props };
    this.updateValue = this.updateValue.bind(this);
  }

  updateValue(event, data) {
    if (!!this.props.editable) {
      const value = !this.state.value;
      this.setState({ value: value }, () => {
        if (value) {
          this.props.updateProperty('Value', this.state.value);
        }
      });
    }
  }

  render() {
    return (
      <React.Fragment>
        <FormField>
          <Checkbox
            label={this.props.description}
            checked={this.state.value}
            onChange={(e, data) => this.updateValue(e, data)}
          />
        </FormField>
      </React.Fragment>
    );
  }
}
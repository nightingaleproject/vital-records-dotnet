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
      const value = data.children === 'True';
      this.setState({ value: value }, () => {
        if (value) {
          this.props.updateProperty('Value', this.state.value);
        }
      });
    }
  }

  render() {
    const value = true;
    return (
      <React.Fragment>
        <FormField>
          <Checkbox
            label={this.props.description}
            name='checkboxRadioGroup'
            value='this'
            checked={value === 'this'}
            onChange={(e, data) => setValue(data.value)}
          />
        </FormField>
      </React.Fragment>
    );
  }
}
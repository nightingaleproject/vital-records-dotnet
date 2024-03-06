import React, { Component } from 'react';
import { Button, Form, FormField, Header, Checkbox } from 'semantic-ui-react';

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
        <Form>
          <FormField>
            Selected value: <b>{value}</b>
          </FormField>
          <FormField>
            <Checkbox
              label={this.props.description}
              name='checkboxRadioGroup'
              value='this'
              checked={value === 'this'}
              onChange={(e, data) => setValue(data.value)}
            />
          </FormField>
        </Form>
      </React.Fragment>
    );
  }
}

// 3/5/2024
// I've set it up to programmataicllay use checkboxes for those fields that are checkboxes, based on if "section" is present in the FHIRPath. However, there is a new potential issue: Checkboxes that are grouped together in the worksheet (https://www.cdc.gov/nchs/data/dvs/facility-worksheet-2016-508.pdf) such as a Diabetes category having gestational and prepregnancy diabetes options in which JUST one, not both, can be checked, have no obvious way to programatically group them together. Because of the only-one selection requirement, I think this checkbox work is necessary and can't just stick with a true-false option. In fact, I think a radio-button option would be best. However, we can't seemingly group them together or link them, which is a requirement. I'm not sure, but this may be a limitation of the IG itself. I'm not sure if the IG enforces any one-or-none logic or way to group these subsections within the categories, which would be a prerequisite for this to work in Canary. I think these sections that exist together are, in the IG, totally seperate entities with no bearing on eachother, but the worksheet does specifify that they influence eachother.
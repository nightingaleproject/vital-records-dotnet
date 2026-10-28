import { faFeatherAlt, faMailBulk } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React from 'react';
import { Link } from 'react-router-dom';
import { Button, Dropdown, Menu } from 'semantic-ui-react';
import * as NavigationOptions from './NavigationOptions';

export function Navigation(props) {

  const scrollToSection = (targetId, block = 'start') => {
    document.getElementById(targetId)?.scrollIntoView({ behavior: 'smooth', block });
  };

  const recordTypeDropDowns = (recordType) => {
    const shouldDisplayRecordDropdowns = recordType === 'vrdr' || recordType === 'bfdr-birth' || recordType === 'bfdr-fetaldeath'
    if (shouldDisplayRecordDropdowns) {
      return (
        <Menu.Menu position="right">
          <Menu.Item name={`${props.recordTypeReadable} Dashboard`} as={Link} to={`/${recordType}`} icon="dashboard" />
          <Dropdown item text="Record Testing" direction="left">
            <Dropdown.Menu>
              {NavigationOptions.RecordTesting(props.recordTypeReadable).map((navigationOption) => {
                return (
                  <Dropdown.Item
                    key={navigationOption.title}
                    icon={navigationOption.icon}
                    text={navigationOption.title}
                    as={Link} to={`${recordType}/${navigationOption.route}`}
                  />
                )
              })}
            </Dropdown.Menu>
          </Dropdown>
          <Dropdown item text="Message Testing" direction="left">
            <Dropdown.Menu>
              {NavigationOptions.MessageTesting(props.recordTypeReadable).map((navigationOption) => {
                return (
                  <Dropdown.Item
                    key={navigationOption.title}
                    icon={navigationOption.icon || <i className="icon"><FontAwesomeIcon icon={navigationOption.faIcon} fixedWidth /></i>}
                    text={navigationOption.title}
                    as={Link} to={`${recordType}/${navigationOption.route}`}
                  />
                )
              })}
            </Dropdown.Menu>
          </Dropdown>
          <Dropdown item text="Record Tools" direction="left">
            <Dropdown.Menu>
              {NavigationOptions.RecordTools(props.recordTypeReadable, props.ijeType).map((navigationOption) => {
                return (
                  <Dropdown.Item
                    key={navigationOption.title}
                    icon={navigationOption.icon || <i className="icon"><FontAwesomeIcon icon={navigationOption.faIcon} fixedWidth /></i>}
                    text={navigationOption.title}
                    as={Link} to={`${recordType}/${navigationOption.route}`}
                  />
                )
              })}
            </Dropdown.Menu>
          </Dropdown>
          <Dropdown item text="Message Tools" direction="left">
            <Dropdown.Menu>
              {NavigationOptions.MessageTools(props.recordTypeReadable).map((navigationOption) => {
                return (
                  <Dropdown.Item
                    key={navigationOption.title}
                    icon={navigationOption.icon}
                    text={navigationOption.title}
                    as={Link} to={`${recordType}/${navigationOption.route}`}
                  />
                )
              })}
            </Dropdown.Menu>
          </Dropdown>
        </Menu.Menu>
      );
    } else {
      return <></>;
    }
  }

  return (
    <React.Fragment>
      <Menu inverted attached size="huge">
        <Menu.Item header as={Link} to="/">
          <FontAwesomeIcon icon={faFeatherAlt} size="lg" fixedWidth />
          <span className="p-l-5">
            Canary Testing Framework
          </span>
          <span className="p-l-5">
            <small>
              VRDR {window.VRDR_VERSION};
            </small>
            <small>
              BFDR {window.BFDR_VERSION};
            </small>
          </span>
        </Menu.Item>
        {recordTypeDropDowns(props.recordType)}
      </Menu>
      {props.showFHIRProducingActions && (   //Item 1.1
        <div className="canary-page-actions">
          <Button size="small" onClick={() => scrollToSection('step-1')}>
            Go to -Step 1: Enter Details
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-2')}>
            Go to -Step 2: Export Records
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-3')}>
            Go to -Step 3: Calculate Results
          </Button>
        </div>
      )}
      {props.showFHIRConsumingActions && (  //Item 1.2
        <div className="canary-page-actions">
          <Button size="small" onClick={() => scrollToSection('step-1')}>
            Go to -Step 1: Import Record
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-2')}>
            Go to -Step 2: Enter Details
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-3')}>
            Go to -Step 3: Calculate Results
          </Button>
        </div>
      )}
      {props.showEDRSConsumingActions && (  //Item 1.3     
        <div className="canary-page-actions">
          <Button size="small" onClick={() => scrollToSection('step-1')}>
            Go to -Step 1: Import FHIR Record
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-2')}>
            Go to -Step 2: Export IJE Record
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-3')}>
            Go to -Step 3: Calculate Results
          </Button>
        </div>
      )}
      {props.showEDRSProducingActions && (  //Item 1.4
        <div className="canary-page-actions">
          <Button size="small" onClick={() => scrollToSection('step-1')}>
            Go to -Step 1: Import IJE Record
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-2')}>
            Go to -Step 2: Export FHIR Record
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-3')}>
            Go to -Step 3: Calculate Results
          </Button>
        </div>
      )}
      {props.showConnectathonFHIRProducingActions && ( //Item 1.5
        <div className="canary-page-actions">
          <Button size="small" onClick={() => scrollToSection('step-1')}>
            Go to -Step 1: Set Certificate Number and Select Jurisdiction
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-2')}>
            Go to -Step 2: Enter Details
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-3')}>
            Go to -Step 3: Export Record
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-4')}>
            Go to -Step 4: Calculate Results
          </Button>
        </div>
      )}
      {props.showValidateFHIRWithIJEActions && ( //Item 1.6
        <div className="canary-page-actions">
          <Button size="small" onClick={() => scrollToSection('step-1')}>
            Go to -Step 1: Upload IJE Record
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-2')}>
            Go to -Step 2: Upload FHIR Record
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-3')}>
            Go to -Step 3: Calculate Results
          </Button>
        </div>
      )}
      {props.showProdFHIRWithMsg && (         //Item 2.1
        <div className="canary-page-actions">
          <Button size="small" onClick={() => scrollToSection('step-1')}>
            Go to -Step 1: Import Record
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-2')}>
            Go to -Step 2: Choose Message Type
          </Button>
        </div>
      )}
      {props.showTestConnectathonMsgPage && ( //Item 2.2
        <div className="canary-page-actions">
          <Button size="small" onClick={() => scrollToSection('step-1')}>
            Go to -Step 1: Set Certificate
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-2')}>
            Go to -Step 2: Enter Connection Test Case data Into Your EDRS
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-3')}>
            Go to -Step 3: Choose Message Type
          </Button>
        </div>
      )}
      {props.showToolFhirCreator && ( //Item 3.5
        <div className="canary-page-actions">
          <Button size="small" onClick={() => scrollToSection('child-demographics')}>
            Top
          </Button>
          <Button size="small" onClick={() => scrollToSection('generate-record')}>
            Generate Record
          </Button>
        </div>
      )}
      {props.showMessageCreatorActions && (  //Item 4.2
        <div className="canary-page-actions">
          <Button size="small" onClick={() => scrollToSection('step-1')}>
            Step 1: Choose the Message Type
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-2', 'center')}>
            Step 2: Upload a Record
          </Button>
          <Button size="small" onClick={() => scrollToSection('step-3', 'end')}>
            Step 3: Generate Results
          </Button>
        </div>
      )}
      <div className="p-b-30" />
    </React.Fragment>
  );
}

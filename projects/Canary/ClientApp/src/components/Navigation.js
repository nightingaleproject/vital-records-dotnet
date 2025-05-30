import { faFeatherAlt, faMailBulk } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React from 'react';
import { Link } from 'react-router-dom';
import { Dropdown, Menu } from 'semantic-ui-react';
import * as NavigationOptions from './NavigationOptions';

export function Navigation(props) {

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
      <div className="p-b-30" />
    </React.Fragment>
  );
}

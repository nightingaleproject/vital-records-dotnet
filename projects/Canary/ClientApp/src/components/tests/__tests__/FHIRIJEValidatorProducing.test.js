import renderer from 'react-test-renderer';
import { fireEvent, render, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event';
import { unmountComponentAtNode } from 'react-dom';
import { MemoryRouter } from 'react-router-dom';
import { FHIRIJEValidatorProducing } from '../FHIRIJEValidatorProducing';
import { Getter } from '../../misc/Getter';

const getterSpy = jest.spyOn(Getter.prototype, 'submitPaste');
const createTestSpy = jest.spyOn(FHIRIJEValidatorProducing.prototype, 'createTest');
let container = null;

beforeAll(() => {
  window.API_URL = '';
});

beforeEach(() => {
  container = document.createElement('div');
  document.body.appendChild(container);
});

afterEach(() => {
  unmountComponentAtNode(container);
  container.remove();
  container = null;
});

it('renders', () => {
  const tree = renderer.create(
    <MemoryRouter>
      <FHIRIJEValidatorProducing recordType={"bfdr-birth"} recordTypeReadable={"BFDR Birth"} />
    </MemoryRouter>
  ).toJSON();
  expect(tree).toMatchSnapshot();
});

it('should show the success message for IJE input when IJE issues is defined but empty', async () => {
  getterSpy.mockImplementationOnce(() => {
    const self = getterSpy.mock.instances[0];
    self.props.updateRecord({ ijeInfo: 'valid ije record' }, []);
  });
  createTestSpy.mockImplementationOnce(() => { });
  const component = render(
    <MemoryRouter>
      <FHIRIJEValidatorProducing recordType={"bfdr-birth"} recordTypeReadable={"BFDR Birth"} />
    </MemoryRouter>
    , container);
  const user = userEvent.setup();
  const ijeUpload = await component.findByTestId("ije-upload");
  const ijeSubmit = await within(ijeUpload).findByText("Submit");
  const ijeInput = await within(ijeUpload).findByRole("textbox");
  ijeInput.focus();
  userEvent.paste('valid ije record').then(() => fireEvent.click(ijeSubmit));
  const ijeResult = await component.findByTestId("ije-record");
  const successMessage = await within(ijeResult).findByText('No issues were found!');
  expect(successMessage).toBeDefined();
});

it('should show the problems for IJE input when there are IJE issues', async () => {
  getterSpy.mockImplementationOnce(() => {
    const self = getterSpy.mock.instances[0];
    self.props.updateRecord(null, [{ severity: 'error', message: 'IJE not valid.' }]);
  });
  const component = render(
    <MemoryRouter>
      <FHIRIJEValidatorProducing recordType={"bfdr-birth"} recordTypeReadable={"BFDR Birth"} />
    </MemoryRouter>
    , container);
  const user = userEvent.setup();
  const ijeUpload = await component.findByTestId("ije-upload");
  const ijeSubmit = await within(ijeUpload).findByText("Submit");
  const ijeInput = await within(ijeUpload).findByRole("textbox");
  ijeInput.focus();
  userEvent.paste('bad stuff').then(() => fireEvent.click(ijeSubmit));
  const ijeResult = await component.findByTestId("ije-record");
  const issueMessage = await within(ijeResult).findByText('IJE not valid.');
  expect(issueMessage).toBeDefined();
  await expect(within(ijeResult).findByText('No issues were found!')).rejects.toBeDefined();
});

it('should show the success message for FHIR input when FHIR issues is defined but empty', async () => {
  getterSpy.mockImplementationOnce(() => {
    const self = getterSpy.mock.instances[0];
    self.props.updateRecord({ fhirInfo: 'valid fhir record' }, []);
  });
  const component = render(
    <MemoryRouter>
      <FHIRIJEValidatorProducing recordType={"bfdr-birth"} recordTypeReadable={"BFDR Birth"} />
    </MemoryRouter>
    , container);
  const user = userEvent.setup();
  const fhirUpload = await component.findByTestId("fhir-upload");
  const fhirSubmit = await within(fhirUpload).findByText("Submit");
  const fhirInput = await within(fhirUpload).findByRole("textbox");
  fhirInput.focus();
  userEvent.paste('valid fhir record').then(() => fireEvent.click(fhirSubmit));
  const fhirResult = await component.findByTestId("fhir-record");
  const successMessage = await within(fhirResult).findByText('No issues were found!');
  expect(successMessage).toBeDefined();
});

it('should show the problems for FHIR input when there are FHIR issues', async () => {
  getterSpy.mockImplementationOnce(() => {
    const self = getterSpy.mock.instances[0];
    self.props.updateRecord(null, [{ severity: 'error', message: 'FHIR not valid.' }]);
  });
  const component = render(
    <MemoryRouter>
      <FHIRIJEValidatorProducing recordType={"bfdr-birth"} recordTypeReadable={"BFDR Birth"} />
    </MemoryRouter>
    , container);
  const user = userEvent.setup();
  const fhirUpload = await component.findByTestId("fhir-upload");
  const fhirSubmit = await within(fhirUpload).findByText("Submit");
  const fhirInput = await within(fhirUpload).findByRole("textbox");
  fhirInput.focus();
  userEvent.paste('bad stuff').then(() => fireEvent.click(fhirSubmit));
  const fhirResult = await component.findByTestId("fhir-record");
  const fhirMessage = await within(fhirResult).findByText('FHIR not valid.');
  expect(fhirMessage).toBeDefined();
  await expect(within(fhirResult).findByText('No issues were found!')).rejects.toBeDefined();
});
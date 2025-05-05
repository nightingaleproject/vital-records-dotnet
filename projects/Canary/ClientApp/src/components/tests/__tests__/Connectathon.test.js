import renderer from 'react-test-renderer';
import { fireEvent, render, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event';
import { unmountComponentAtNode } from 'react-dom';
import { MemoryRouter } from 'react-router-dom';
import { Connectathon } from '../Connectathon';
import { Getter } from '../../misc/Getter';

jest.mock('axios', () => ({
  get: async (uri) => {
    if (uri === '/tests/bfdr-birth/connectathon/1/1/DE') {
      return {
        status: 200,
        data: {
          results: '{}',
          referenceRecord: { fhirInfo: '{}' }
        }
      }
    } else {
      return {
        status: 404
      }
    }
  }
}));

const getterSpy = jest.spyOn(Getter.prototype, 'submitPaste');
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
      <Connectathon params={{ "recordType": "bfdr-birth", "id": 1 }} />
    </MemoryRouter>
  ).toJSON();
  expect(tree).toMatchSnapshot();
});

it('should show the success message when issues is defined but empty', async () => {
  getterSpy.mockImplementationOnce(() => {
    const self = getterSpy.mock.instances[0];
    self.props.updateRecord({ fhirInfo: {} }, []);
  });
  const component = render(
    <MemoryRouter>
      <Connectathon params={{ "recordType": "bfdr-birth", "id": 1 }} />
    </MemoryRouter>
    , container
  );
  const user = userEvent.setup();
  const jurisdictionDiv = await component.findByTestId('select-jurisdiction');
  const jurisdictionSelect = await within(jurisdictionDiv).findByRole('textbox');
  await userEvent.type(jurisdictionSelect, 'Delaware');
  const certificateNumber = await component.findByPlaceholderText('Enter Certificate Number');
  await userEvent.click(certificateNumber);
  const uploadArea = await component.findByTestId('fhir-upload');
  const submitButton = await within(uploadArea).findByText('Submit');
  const textArea = await within(uploadArea).findByRole('textbox');
  textArea.focus();
  await userEvent.paste('{ "good": true }');
  fireEvent.click(submitButton);
  const successMessage = await component.findByText('No issues were found!');
  expect(successMessage).toBeDefined();
});

it('should show the problems when there are issues', async () => {
  getterSpy.mockImplementationOnce(() => {
    const self = getterSpy.mock.instances[0];
    self.props.updateRecord(null, [{ severity: 'warning', message: 'I am very sleepy.' }]);
  });
  const component = render(
    <MemoryRouter>
      <Connectathon params={{ "recordType": "bfdr-birth", "id": 1 }} />
    </MemoryRouter>
    , container
  );
  const user = userEvent.setup();
  const jurisdictionDiv = await component.findByTestId('select-jurisdiction');
  const jurisdictionSelect = await within(jurisdictionDiv).findByRole('textbox');
  await userEvent.type(jurisdictionSelect, 'Delaware');
  const certificateNumber = await component.findByPlaceholderText('Enter Certificate Number');
  await userEvent.click(certificateNumber);
  const uploadArea = await component.findByTestId('fhir-upload');
  const submitButton = await within(uploadArea).findByText('Submit');
  const textArea = await within(uploadArea).findByRole('textbox');
  textArea.focus();
  await userEvent.paste('{ "good": false }');
  fireEvent.click(submitButton);
  const warningMessage = await component.findByText('I am very sleepy.');
  expect(warningMessage).toBeDefined();
  await expect(component.findByText('No issues were found!')).rejects.toBeDefined();
});
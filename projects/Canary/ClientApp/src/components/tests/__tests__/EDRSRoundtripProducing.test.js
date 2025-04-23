import renderer from 'react-test-renderer';
import { fireEvent, render } from '@testing-library/react'
import userEvent from '@testing-library/user-event';
import { unmountComponentAtNode } from 'react-dom';
import { MemoryRouter } from 'react-router-dom';
import { EDRSRoundtripProducing } from '../EDRSRoundtripProducing';
import { Getter } from '../../misc/Getter';

jest.mock('axios', () => ({
  get: async (uri) => {
    if (uri === '/tests/bfdr-birth/new') {
      return {
        status: 200,
        data: {
          recordId: 0,
          referenceRecord: { recordId: 0 }
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
      <EDRSRoundtripProducing params={{ "recordType": "bfdr-birth" }} />
    </MemoryRouter>
  ).toJSON();
  expect(tree).toMatchSnapshot();
});

it('should show the success message when issues is defined but empty', async () => {
  getterSpy.mockImplementationOnce(() => {
    const self = getterSpy.mock.instances[0];
    self.props.updateRecord('', []);
  });
  const component = render(
    <MemoryRouter>
      <EDRSRoundtripProducing params={{ "recordType": "bfdr-birth" }} />
    </MemoryRouter>
    , container);
  const user = userEvent.setup();
  const submitButton = await component.findByText('Submit');
  const textArea = await component.findByRole('textbox');
  textArea.focus();
  await userEvent.paste('{ "good": true }');
  fireEvent.click(submitButton);
  const successMessage = await component.findByText('No issues were found!');
  expect(successMessage).toBeDefined();
});

it('should show the problems when there are issues', async () => {
  getterSpy.mockImplementationOnce(() => {
    const self = getterSpy.mock.instances[0];
    self.props.updateRecord('',
      [{ severity: 'error', message: 'Problem description: data no good.' },
      { severity: 'warning', message: 'Also, I am tired.' }]);
  });
  const component = render(
    <MemoryRouter>
      <EDRSRoundtripProducing params={{ "recordType": "bfdr-birth" }} />
    </MemoryRouter>
    , container);
  const user = userEvent.setup();
  const submitButton = await component.findByText('Submit');
  const textArea = await component.findByRole('textbox');
  textArea.focus();
  await userEvent.paste('{ "good": false }');
  fireEvent.click(submitButton);
  const errorMessage = await component.findByText('Problem description: data no good.');
  expect(errorMessage).toBeDefined();
  const warningMessage = await component.findByText('Also, I am tired.');
  expect(warningMessage).toBeDefined();
  await expect(component.findByText('No issues were found!')).rejects.toBeDefined();
});
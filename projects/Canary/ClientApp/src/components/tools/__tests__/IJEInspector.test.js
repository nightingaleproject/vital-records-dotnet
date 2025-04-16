import renderer from 'react-test-renderer';
import { fireEvent, render } from '@testing-library/react'
import userEvent from '@testing-library/user-event';
import { unmountComponentAtNode } from 'react-dom';
import { MemoryRouter } from 'react-router-dom';
import { IJEInspector } from '../IJEInspector';
import { Getter } from '../../misc/Getter';

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
      <IJEInspector recordType={"bfdr-birth"} recordTypeReadable={"BFDR Birth"} />
    </MemoryRouter>
  ).toJSON();
  expect(tree).toMatchSnapshot();
});

it('should show the success message when issues is defined but empty', async () => {
  getterSpy.mockImplementationOnce(() => {
    const self = getterSpy.mock.instances[0];
    self.props.updateRecord({ ijeInfo: [] }, []);
  });
  const component = render(
    <MemoryRouter>
      <IJEInspector recordType={"bfdr-birth"} recordTypeReadable={"BFDR Birth"} />
    </MemoryRouter>
    , container);
  const user = userEvent.setup();
  const submitButton = await component.findByText('Submit');
  const textArea = await component.findByRole('textbox');
  textArea.focus();
  await userEvent.paste('VALID BIRTH IJE');
  fireEvent.click(submitButton);
  const successMessage = await component.findByText('No issues were found!');
  expect(successMessage).toBeDefined();
});

it('should show the problems when there are issues', async () => {
  getterSpy.mockImplementationOnce(() => {
    const self = getterSpy.mock.instances[0];
    self.props.updateRecord(null,
      [{ severity: 'error', message: 'Problem description: data no good.' },
      { severity: 'warning', message: 'Also, I am tired.' }]);
  });
  const component = render(<MemoryRouter>
    <IJEInspector recordType={"bfdr-birth"} recordTypeReadable={"BFDR Birth"} />
  </MemoryRouter>, container);
  const user = userEvent.setup();
  const submitButton = await component.findByText('Submit');
  const textArea = await component.findByRole('textbox');
  textArea.focus();
  userEvent.paste('SOME INCORRECT IJE').then(() => fireEvent.click(submitButton));
  const errorMessage = await component.findByText('Problem description: data no good.');
  expect(errorMessage).toBeDefined();
  const warningMessage = await component.findByText('Also, I am tired.');
  expect(warningMessage).toBeDefined();
  await expect(component.findByText('No issues were found!')).rejects.toBeDefined();
});
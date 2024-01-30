import renderer from 'react-test-renderer';
import {
    MemoryRouter,
    BrowserRouter,
    Routes, Route, useParams
} from 'react-router-dom';
import { ConnectathonDashboard } from '../ConnectathonDashboard';
import {addParams} from '../../../App'


jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useParams: () => ({
        type: 'message',
    }),
}));

it('renders', () => {
    const tree = renderer.create(
        <MemoryRouter>
            <ConnectathonDashboard params={{ recordType: "vrdr", type: "message" }} />
        </MemoryRouter>
    ).toJSON();
    expect(tree).toMatchSnapshot();
});
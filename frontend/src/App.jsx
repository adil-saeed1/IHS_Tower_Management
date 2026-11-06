import { Navigate, Route, Routes } from 'react-router-dom';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import SiteList from './pages/SiteList';
import SiteDetail from './pages/SiteDetail';
import SiteEdit from './pages/SiteEdit';

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route index element={<Dashboard />} />
        <Route path="sites" element={<SiteList />} />
        <Route path="sites/new" element={<SiteEdit />} />
        <Route path="sites/:id" element={<SiteDetail />} />
        <Route path="sites/:id/edit" element={<SiteEdit />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  );
}

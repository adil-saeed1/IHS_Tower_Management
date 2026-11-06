import { useEffect, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { api } from '../api/client';
import { useApi } from '../hooks/useApi';
import { SITE_STATUSES, label } from '../constants';
import StatusBadge from '../components/StatusBadge';
import CapacityBar from '../components/CapacityBar';

export default function SiteList() {
  const navigate = useNavigate();
  // Filters live in the URL so a filtered view can be shared or bookmarked.
  const [params, setParams] = useSearchParams();
  const country = params.get('country') ?? '';
  const status = params.get('status') ?? '';
  const [search, setSearch] = useState(params.get('search') ?? '');

  useEffect(() => {
    const timer = setTimeout(() => updateParam('search', search), 300);
    return () => clearTimeout(timer);
  }, [search]);

  const countries = useApi(() => api.getCountries());
  const sites = useApi(
    () => api.getSites({ search: params.get('search'), country, status }),
    [params.toString()]
  );

  function updateParam(key, value) {
    setParams((prev) => {
      const next = new URLSearchParams(prev);
      value ? next.set(key, value) : next.delete(key);
      return next;
    }, { replace: true });
  }

  return (
    <>
      <header className="page-header">
        <div>
          <h1>Sites</h1>
          <p className="muted">Search and filter the tower portfolio.</p>
        </div>
        <Link to="/sites/new" className="btn btn-primary">+ Register site</Link>
      </header>

      <div className="filters card">
        <input
          type="search"
          placeholder="Search by code, name or city…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <select value={country} onChange={(e) => updateParam('country', e.target.value)}>
          <option value="">All countries</option>
          {countries.data?.map((c) => <option key={c}>{c}</option>)}
        </select>
        <select value={status} onChange={(e) => updateParam('status', e.target.value)}>
          <option value="">All statuses</option>
          {SITE_STATUSES.map((s) => <option key={s} value={s}>{label(s)}</option>)}
        </select>
      </div>

      <section className="card table-card">
        {sites.error && <p className="alert">{sites.error}</p>}
        <table>
          <thead>
            <tr>
              <th>Code</th>
              <th>Name</th>
              <th>Location</th>
              <th>Status</th>
              <th>Tower</th>
              <th>Capacity</th>
            </tr>
          </thead>
          <tbody>
            {sites.data?.map((s) => (
              <tr key={s.id} className="clickable" onClick={() => navigate(`/sites/${s.id}`)}>
                <td><strong>{s.code}</strong></td>
                <td>{s.name}</td>
                <td>{s.city}, {s.country}</td>
                <td><StatusBadge status={s.status} /></td>
                <td>{s.towerType}</td>
                <td><CapacityBar active={s.activeTenants} max={s.maxTenants} /></td>
              </tr>
            ))}
            {sites.data?.length === 0 && (
              <tr><td colSpan={6} className="muted center">No sites match these filters.</td></tr>
            )}
          </tbody>
        </table>
        {sites.data && <p className="muted small table-footer">{sites.data.length} site(s)</p>}
      </section>
    </>
  );
}

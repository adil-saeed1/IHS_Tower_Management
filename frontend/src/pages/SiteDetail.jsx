import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { CircleMarker, MapContainer, TileLayer } from 'react-leaflet';
import { api } from '../api/client';
import { useApi } from '../hooks/useApi';
import { formatMoney, label } from '../constants';
import StatusBadge from '../components/StatusBadge';
import CapacityBar from '../components/CapacityBar';

export default function SiteDetail() {
  const { id } = useParams();
  const site = useApi(() => api.getSite(id), [id]);

  if (site.loading && !site.data) return <p className="muted">Loading site…</p>;
  if (site.error && !site.data) return <p className="alert">{site.error}</p>;

  const s = site.data;
  const tower = s.tower;

  return (
    <>
      <header className="page-header">
        <div>
          <Link to="/sites" className="muted small">← All sites</Link>
          <h1>{s.code} · {s.name}</h1>
          <p className="muted">{s.city}, {s.country} <StatusBadge status={s.status} /></p>
        </div>
        <Link to={`/sites/${s.id}/edit`} className="btn">Edit site</Link>
      </header>

      <div className="grid-3">
        <section className="card">
          <h3>Site</h3>
          <dl className="details">
            <dt>Country</dt><dd>{s.country}</dd>
            <dt>City</dt><dd>{s.city || '—'}</dd>
            <dt>Coordinates</dt><dd>{s.latitude}, {s.longitude}</dd>
            <dt>Status</dt><dd>{label(s.status)}</dd>
            <dt>Commissioned</dt><dd>{s.commissionedOn ?? '—'}</dd>
          </dl>
        </section>

        <section className="card">
          <h3>Tower</h3>
          <dl className="details">
            <dt>Type</dt><dd>{tower.type}</dd>
            <dt>Height</dt><dd>{tower.heightMeters} m</dd>
            <dt>Power</dt><dd>{tower.powerSource}</dd>
            <dt>Capacity</dt><dd><CapacityBar active={tower.activeTenants} max={tower.maxTenants} /></dd>
            <dt>Free slots</dt><dd>{tower.availableSlots}</dd>
          </dl>
        </section>

        <section className="card map-card">
          <MapContainer center={[s.latitude, s.longitude]} zoom={11} className="map-small" scrollWheelZoom={false}>
            <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" attribution="&copy; OpenStreetMap" />
            <CircleMarker center={[s.latitude, s.longitude]} radius={9}
              pathOptions={{ color: '#ffffff', weight: 2, fillColor: '#2a78d6', fillOpacity: 1 }} />
          </MapContainer>
        </section>
      </div>

      <section className="card">
        <div className="card-header">
          <h3>Tenants</h3>
          <span className={tower.hasCapacity ? 'pill pill-good' : 'pill pill-bad'}>
            {tower.hasCapacity ? `✓ ${tower.availableSlots} slot(s) available` : '✕ No capacity'}
          </span>
        </div>

        <TenantTable tenancies={s.tenancies} onChanged={site.reload} />

        {s.status === 'Active' && tower.hasCapacity
          ? <AddTenantForm siteId={s.id} onAdded={site.reload} />
          : <p className="muted small">
              {s.status !== 'Active' ? 'Tenants can only be added to active sites.' : 'This tower is at full capacity.'}
            </p>}
      </section>
    </>
  );
}

function TenantTable({ tenancies, onChanged }) {
  async function end(tenancy) {
    if (!confirm(`End the lease for ${tenancy.operatorName}?`)) return;
    try {
      await api.endTenancy(tenancy.id);
      onChanged();
    } catch (e) {
      alert(e.message);
    }
  }

  if (tenancies.length === 0) return <p className="muted">No tenants yet.</p>;

  return (
    <table>
      <thead>
        <tr><th>Operator</th><th>Start</th><th>End</th><th>Monthly rent</th><th>Status</th><th /></tr>
      </thead>
      <tbody>
        {tenancies.map((t) => (
          <tr key={t.id} className={t.isActive ? '' : 'inactive'}>
            <td><strong>{t.operatorName}</strong></td>
            <td>{t.startDate}</td>
            <td>{t.endDate ?? '—'}</td>
            <td>{formatMoney(t.monthlyRent)}</td>
            <td>{t.isActive ? 'Active' : 'Ended'}</td>
            <td className="right">
              {t.isActive && <button className="btn btn-ghost btn-sm" onClick={() => end(t)}>End lease</button>}
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

function AddTenantForm({ siteId, onAdded }) {
  const operators = useApi(() => api.getOperators());
  const [form, setForm] = useState({ operatorId: '', startDate: today(), monthlyRent: 1500 });
  const [error, setError] = useState(null);
  const [saving, setSaving] = useState(false);

  async function submit(e) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      await api.addTenancy(siteId, { ...form, operatorId: Number(form.operatorId), monthlyRent: Number(form.monthlyRent) });
      setForm({ ...form, operatorId: '' });
      onAdded();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  return (
    <form className="inline-form" onSubmit={submit}>
      <h4>Add tenant</h4>
      <select value={form.operatorId} onChange={(e) => setForm({ ...form, operatorId: e.target.value })} required>
        <option value="">Select operator…</option>
        {operators.data?.map((o) => <option key={o.id} value={o.id}>{o.name}</option>)}
      </select>
      <input type="date" value={form.startDate} onChange={(e) => setForm({ ...form, startDate: e.target.value })} required />
      <input type="number" min="0" step="50" value={form.monthlyRent}
        onChange={(e) => setForm({ ...form, monthlyRent: e.target.value })} aria-label="Monthly rent" required />
      <button className="btn btn-primary" disabled={saving}>{saving ? 'Adding…' : 'Add tenant'}</button>
      {error && <p className="alert">{error}</p>}
    </form>
  );
}

const today = () => new Date().toISOString().slice(0, 10);

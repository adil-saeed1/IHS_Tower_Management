import { Link } from 'react-router-dom';
import { CircleMarker, MapContainer, Popup, TileLayer } from 'react-leaflet';
import {
  Bar, BarChart, CartesianGrid, LabelList, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis,
} from 'recharts';
import { api } from '../api/client';
import { useApi } from '../hooks/useApi';
import { CAPACITY_COLORS, capacityState, formatMoney, label } from '../constants';
import StatusBadge from '../components/StatusBadge';

const LEASED = '#2a78d6';
const FREE = '#b7d3f6';
const AXIS = { fontSize: 12, fill: '#52514e' };

export default function Dashboard() {
  const { data, error, loading } = useApi(() => api.getDashboard());

  if (loading) return <p className="muted">Loading portfolio…</p>;
  if (error) return <p className="alert">{error}</p>;

  const byCountry = data.byCountry.map((c) => ({ ...c, free: c.capacity - c.tenancies }));
  const byStatus = data.byStatus.map((s) => ({ ...s, name: label(s.status) }));

  return (
    <>
      <header className="page-header">
        <div>
          <h1>Portfolio overview</h1>
          <p className="muted">Where our sites are and how well they are utilised.</p>
        </div>
      </header>

      <div className="kpis">
        <Kpi title="Total sites" value={data.totalSites} hint={`${data.activeSites} active`} />
        <Kpi title="Active tenancies" value={data.totalTenancies} hint={`of ${data.totalCapacity} slots on active towers`} />
        <Kpi title="Utilisation" value={`${data.utilisationPercent}%`} hint="leased / capacity" />
        <Kpi title="Monthly revenue" value={formatMoney(data.monthlyRevenue)} hint="active leases" />
      </div>

      <section className="card">
        <div className="card-header">
          <h3>Site map</h3>
          <MapLegend />
        </div>
        <MapContainer center={[8, 18]} zoom={3} className="map" scrollWheelZoom={false}>
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          {data.mapPoints.map((p) => (
            <CircleMarker
              key={p.id}
              center={[p.latitude, p.longitude]}
              radius={8}
              pathOptions={{ color: '#ffffff', weight: 2, fillColor: markerColor(p), fillOpacity: 0.95 }}
            >
              <Popup>
                <strong>{p.code}</strong> · {p.name}
                <br />
                <StatusBadge status={p.status} /> {p.activeTenants}/{p.maxTenants} tenants
                <br />
                <Link to={`/sites/${p.id}`}>Open site →</Link>
              </Popup>
            </CircleMarker>
          ))}
        </MapContainer>
      </section>

      <div className="grid-2">
        <section className="card">
          <h3>Tenant slots by country</h3>
          <p className="muted small">Active towers only</p>
          <ResponsiveContainer width="100%" height={280}>
            <BarChart data={byCountry} margin={{ top: 16, right: 8, left: -16, bottom: 0 }}>
              <CartesianGrid vertical={false} stroke="#e1e0d9" />
              <XAxis dataKey="country" tick={AXIS} axisLine={{ stroke: '#c3c2b7' }} tickLine={false} />
              <YAxis allowDecimals={false} tick={AXIS} axisLine={false} tickLine={false} />
              <Tooltip cursor={{ fill: 'rgba(11,11,11,0.04)' }} />
              <Legend iconType="square" wrapperStyle={{ fontSize: 12 }} />
              <Bar dataKey="tenancies" name="Leased" stackId="slots" fill={LEASED} stroke="#fcfcfb" strokeWidth={2} />
              <Bar dataKey="free" name="Free" stackId="slots" fill={FREE} stroke="#fcfcfb" strokeWidth={2} radius={[4, 4, 0, 0]}>
                <LabelList dataKey="utilisationPercent" position="top" formatter={(v) => `${v}%`} style={AXIS} />
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </section>

        <section className="card">
          <h3>Sites by status</h3>
          <p className="muted small">Whole portfolio</p>
          <ResponsiveContainer width="100%" height={280}>
            <BarChart data={byStatus} layout="vertical" margin={{ top: 16, right: 32, left: 24, bottom: 0 }}>
              <CartesianGrid horizontal={false} stroke="#e1e0d9" />
              <XAxis type="number" allowDecimals={false} tick={AXIS} axisLine={false} tickLine={false} />
              <YAxis type="category" dataKey="name" tick={AXIS} axisLine={{ stroke: '#c3c2b7' }} tickLine={false} width={120} />
              <Tooltip cursor={{ fill: 'rgba(11,11,11,0.04)' }} />
              <Bar dataKey="sites" name="Sites" fill={LEASED} radius={[0, 4, 4, 0]} barSize={22}>
                <LabelList dataKey="sites" position="right" style={AXIS} />
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </section>
      </div>
    </>
  );
}

function markerColor(point) {
  if (point.status !== 'Active') return CAPACITY_COLORS.inactive;
  return CAPACITY_COLORS[capacityState(point.activeTenants, point.maxTenants).key];
}

function Kpi({ title, value, hint }) {
  return (
    <div className="card kpi">
      <span className="muted small">{title}</span>
      <strong>{value}</strong>
      <span className="muted small">{hint}</span>
    </div>
  );
}

function MapLegend() {
  const items = [
    ['available', 'Capacity available'],
    ['limited', 'Nearly full (≥70%)'],
    ['full', 'Full'],
    ['inactive', 'Not active'],
  ];
  return (
    <ul className="legend">
      {items.map(([key, text]) => (
        <li key={key}>
          <span className="dot" style={{ background: CAPACITY_COLORS[key] }} />
          {text}
        </li>
      ))}
    </ul>
  );
}

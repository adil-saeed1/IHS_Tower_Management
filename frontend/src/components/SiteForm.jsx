import { useState } from 'react';
import { POWER_SOURCES, SITE_STATUSES, TOWER_TYPES, label } from '../constants';

const EMPTY = {
  code: '', name: '', country: '', city: '', latitude: '', longitude: '',
  status: 'Planned', commissionedOn: '',
  towerType: 'Monopole', heightMeters: 30, maxTenants: 3, powerSource: 'Grid',
};

export default function SiteForm({ initial, onSubmit, onCancel, submitting }) {
  const [form, setForm] = useState({ ...EMPTY, ...initial });

  const set = (field) => (e) => setForm({ ...form, [field]: e.target.value });

  function handleSubmit(e) {
    e.preventDefault();
    onSubmit({
      ...form,
      latitude: Number(form.latitude),
      longitude: Number(form.longitude),
      heightMeters: Number(form.heightMeters),
      maxTenants: Number(form.maxTenants),
      commissionedOn: form.commissionedOn || null,
    });
  }

  return (
    <form className="form" onSubmit={handleSubmit}>
      <section className="card">
        <h3>Site</h3>
        <div className="grid-2">
          <Field label="Site code"><input value={form.code} onChange={set('code')} placeholder="NG-LAG-010" required /></Field>
          <Field label="Name"><input value={form.name} onChange={set('name')} required /></Field>
          <Field label="Country"><input value={form.country} onChange={set('country')} required /></Field>
          <Field label="City"><input value={form.city} onChange={set('city')} /></Field>
          <Field label="Latitude"><input type="number" step="any" min="-90" max="90" value={form.latitude} onChange={set('latitude')} required /></Field>
          <Field label="Longitude"><input type="number" step="any" min="-180" max="180" value={form.longitude} onChange={set('longitude')} required /></Field>
          <Field label="Status">
            <select value={form.status} onChange={set('status')}>
              {SITE_STATUSES.map((s) => <option key={s} value={s}>{label(s)}</option>)}
            </select>
          </Field>
          <Field label="Commissioned on"><input type="date" value={form.commissionedOn ?? ''} onChange={set('commissionedOn')} /></Field>
        </div>
      </section>

      <section className="card">
        <h3>Tower</h3>
        <div className="grid-2">
          <Field label="Tower type">
            <select value={form.towerType} onChange={set('towerType')}>
              {TOWER_TYPES.map((t) => <option key={t}>{t}</option>)}
            </select>
          </Field>
          <Field label="Height (m)"><input type="number" min="1" max="200" value={form.heightMeters} onChange={set('heightMeters')} required /></Field>
          <Field label="Tenant capacity"><input type="number" min="1" max="10" value={form.maxTenants} onChange={set('maxTenants')} required /></Field>
          <Field label="Power source">
            <select value={form.powerSource} onChange={set('powerSource')}>
              {POWER_SOURCES.map((p) => <option key={p}>{p}</option>)}
            </select>
          </Field>
        </div>
      </section>

      <div className="actions">
        <button type="button" className="btn btn-ghost" onClick={onCancel}>Cancel</button>
        <button type="submit" className="btn btn-primary" disabled={submitting}>
          {submitting ? 'Saving…' : 'Save site'}
        </button>
      </div>
    </form>
  );
}

function Field({ label, children }) {
  return (
    <label className="field">
      <span>{label}</span>
      {children}
    </label>
  );
}

import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { api } from '../api/client';
import { useApi } from '../hooks/useApi';
import SiteForm from '../components/SiteForm';

// Handles both "register new site" (/sites/new) and "edit site" (/sites/:id/edit).
export default function SiteEdit() {
  const { id } = useParams();
  const isNew = !id;
  const navigate = useNavigate();
  const [error, setError] = useState(null);
  const [saving, setSaving] = useState(false);

  const existing = useApi(() => (isNew ? Promise.resolve(null) : api.getSite(id)), [id]);

  async function save(values) {
    setSaving(true);
    setError(null);
    try {
      const saved = isNew ? await api.createSite(values) : await api.updateSite(id, values);
      navigate(`/sites/${saved.id}`);
    } catch (e) {
      setError(e.message);
      setSaving(false);
    }
  }

  if (existing.loading) return <p className="muted">Loading…</p>;
  if (existing.error) return <p className="alert">{existing.error}</p>;

  return (
    <>
      <header className="page-header">
        <div>
          <h1>{isNew ? 'Register a new site' : `Edit ${existing.data.code}`}</h1>
          <p className="muted">A site is registered together with its tower.</p>
        </div>
      </header>

      {error && <p className="alert">{error}</p>}

      <SiteForm
        key={id ?? 'new'}
        initial={existing.data ? toForm(existing.data) : undefined}
        onSubmit={save}
        onCancel={() => navigate(-1)}
        submitting={saving}
      />
    </>
  );
}

function toForm(site) {
  return {
    code: site.code,
    name: site.name,
    country: site.country,
    city: site.city,
    latitude: site.latitude,
    longitude: site.longitude,
    status: site.status,
    commissionedOn: site.commissionedOn ?? '',
    towerType: site.tower.type,
    heightMeters: site.tower.heightMeters,
    maxTenants: site.tower.maxTenants,
    powerSource: site.tower.powerSource,
  };
}

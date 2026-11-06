const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5234';

async function request(path, options = {}) {
  const response = await fetch(`${BASE_URL}/api${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });

  if (!response.ok) {
    const problem = await response.json().catch(() => ({}));
    const validation = problem.errors ? Object.values(problem.errors).flat().join(' ') : null;
    throw new Error(problem.detail ?? validation ?? problem.title ?? `Request failed (${response.status})`);
  }

  return response.status === 204 ? null : response.json();
}

export const api = {
  getDashboard: () => request('/dashboard'),
  getSites: (filters) => request(`/sites?${new URLSearchParams(clean(filters))}`),
  getCountries: () => request('/sites/countries'),
  getSite: (id) => request(`/sites/${id}`),
  createSite: (site) => request('/sites', { method: 'POST', body: JSON.stringify(site) }),
  updateSite: (id, site) => request(`/sites/${id}`, { method: 'PUT', body: JSON.stringify(site) }),
  getOperators: () => request('/operators'),
  addTenancy: (siteId, tenancy) =>
    request(`/sites/${siteId}/tenancies`, { method: 'POST', body: JSON.stringify(tenancy) }),
  endTenancy: (id) => request(`/tenancies/${id}`, { method: 'DELETE' }),
};

function clean(obj = {}) {
  return Object.fromEntries(Object.entries(obj).filter(([, v]) => v !== '' && v != null));
}

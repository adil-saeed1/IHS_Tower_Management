export const SITE_STATUSES = ['Planned', 'UnderConstruction', 'Active', 'Decommissioned'];
export const TOWER_TYPES = ['Monopole', 'Lattice', 'Guyed', 'Rooftop'];
export const POWER_SOURCES = ['Grid', 'Hybrid', 'Solar', 'Diesel'];

export const label = (value) => value.replace(/([a-z])([A-Z])/g, '$1 $2');

// Capacity state drives colour on the map, list and detail views.
export function capacityState(activeTenants, maxTenants) {
  if (activeTenants >= maxTenants) return { key: 'full', text: 'Full' };
  if (activeTenants / maxTenants >= 0.7) return { key: 'limited', text: 'Limited' };
  return { key: 'available', text: 'Available' };
}

export const CAPACITY_COLORS = {
  available: '#0ca30c',
  limited: '#fab219',
  full: '#d03b3b',
  inactive: '#898781',
};

export const formatMoney = (value) =>
  new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 }).format(value);

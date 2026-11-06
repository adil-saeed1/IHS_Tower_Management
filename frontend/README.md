# TowerCo UI (frontend)

React 18 single-page app built with Vite. It talks to the TowerCo API over REST/JSON.

## Prerequisites
- [Node.js](https://nodejs.org/) 18 or newer
- The backend API running on http://localhost:5234 (see [`../backend/README.md`](../backend/README.md))

## Run

```bash
cd frontend
npm install
npm run dev
```

Open http://localhost:5173.

| Script | What it does |
|---|---|
| `npm run dev` | Start the dev server with hot reload |
| `npm run build` | Production build into `dist/` |
| `npm run preview` | Serve the production build locally |

To use a different API address, create `frontend/.env.local`:

```
VITE_API_URL=http://localhost:5234
```

## Folder structure

```
src/
├─ api/client.js    the only place that makes HTTP calls; turns API errors into messages
├─ hooks/useApi.js  loading / error / reload state for a request
├─ components/      reusable UI: Layout, SiteForm, CapacityBar, StatusBadge
├─ pages/           one file per screen (see below)
├─ constants.js     shared values such as capacity status colours
├─ App.jsx          routes
├─ main.jsx         entry point
└─ index.css        plain CSS, no UI framework
```

## Pages

| Route | Page | Purpose |
|---|---|---|
| `/` | `Dashboard.jsx` | KPIs, map of all sites, utilisation charts |
| `/sites` | `SiteList.jsx` | Search and filter by country and status (filters are kept in the URL) |
| `/sites/new` | `SiteEdit.jsx` | Register a site and tower |
| `/sites/:id` | `SiteDetail.jsx` | Everything about one site: tower, capacity, tenants |
| `/sites/:id/edit` | `SiteEdit.jsx` | Update a site and tower |

## Libraries
- `react-router-dom`: routing
- `react-leaflet` / `leaflet`: map (OpenStreetMap tiles, no API key needed)
- `recharts`: charts

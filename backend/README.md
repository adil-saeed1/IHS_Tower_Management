# TowerCo API (backend)

ASP.NET Core 8 Web API that manages tower sites, towers, operators and tenancies.
Data is stored in SQLite through EF Core.

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Run

```bash
cd backend/TowerCo.Api
dotnet run
```

- API: http://localhost:5234
- Swagger UI: http://localhost:5234/swagger

On first run the database file `towerco.db` is created and seeded with fictional demo data.
Delete `towerco.db` to reset the data.

You can also open `TowerCo.sln` (repo root) in Visual Studio and press **F5**.

## Folder structure

```
TowerCo.Api/
├─ Controllers/    HTTP endpoints only (thin, no business logic)
├─ Services/       business rules, mapping entities → DTOs, custom exceptions
├─ Domain/         entities (Site, Tower, Operator, Tenancy) + enums
├─ Data/           AppDbContext (EF Core configuration) and SeedData
├─ Dtos/           request/response records (the API never returns entities)
├─ ErrorHandler.cs maps exceptions to ProblemDetails (404 / 409)
├─ Program.cs      dependency injection, SQLite, CORS, Swagger, create + seed DB
└─ appsettings.json connection string and allowed CORS origins
```

Request flow: `Controller → Service → AppDbContext → SQLite`

## Endpoints

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/sites?search=&country=&status=` | Search / filter sites |
| GET | `/api/sites/countries` | Countries for the filter dropdown |
| GET | `/api/sites/{id}` | Site detail with tower, capacity and tenants |
| POST | `/api/sites` | Register a site and its tower |
| PUT | `/api/sites/{id}` | Update a site and its tower |
| POST | `/api/sites/{id}/tenancies` | Add an operator as tenant |
| DELETE | `/api/tenancies/{id}` | End a lease (sets `EndDate`, keeps history) |
| GET | `/api/operators` | List operators |
| GET | `/api/dashboard` | Portfolio KPIs, map points and utilisation |

## Business rules
These are enforced in `Services/` and returned as `409 Conflict`:
- a tower cannot exceed `MaxTenants`
- an operator cannot be added twice to the same tower
- tenants can only be added to **Active** sites
- site codes must be unique
- `MaxTenants` cannot be reduced below the current number of tenants

A missing resource returns `404 Not Found`.

## Configuration
`appsettings.json`:
- `ConnectionStrings:Default`: SQLite file path
- `Cors:AllowedOrigins`: frontend URL (default `http://localhost:5173`)

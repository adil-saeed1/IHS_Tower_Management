using TowerCo.Api.Domain;

namespace TowerCo.Api.Data;

// Fictional demo data, inserted only when the database is empty.
public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Sites.Any()) return;

        var operators = new[] { "Nova Mobile", "SkyLink Telecom", "Orbit Wireless", "Pulse Networks", "Zenith Cell" }
            .Select(name => new Operator { Name = name })
            .ToList();
        db.Operators.AddRange(operators);

        var sites = new List<(string Code, string Name, string Country, string City, double Lat, double Lng,
            SiteStatus Status, TowerType Type, double Height, int Max, PowerSource Power, int Tenants)>
        {
            ("NG-LAG-001", "Lekki Phase 1",      "Nigeria",      "Lagos",         6.4474,   3.4723, SiteStatus.Active,            TowerType.Monopole, 35, 3, PowerSource.Hybrid, 3),
            ("NG-LAG-002", "Ikeja GRA",          "Nigeria",      "Lagos",         6.5833,   3.3500, SiteStatus.Active,            TowerType.Lattice,  45, 4, PowerSource.Grid,   2),
            ("NG-ABJ-001", "Maitama Hill",       "Nigeria",      "Abuja",         9.0866,   7.4950, SiteStatus.Active,            TowerType.Lattice,  50, 4, PowerSource.Hybrid, 4),
            ("NG-KAN-001", "Kano Central",       "Nigeria",      "Kano",         12.0022,   8.5920, SiteStatus.UnderConstruction, TowerType.Guyed,    60, 3, PowerSource.Solar,  0),
            ("NG-PHC-001", "Trans Amadi",        "Nigeria",      "Port Harcourt", 4.8156,   7.0498, SiteStatus.Active,            TowerType.Monopole, 30, 2, PowerSource.Diesel, 1),
            ("KE-NBO-001", "Westlands",          "Kenya",        "Nairobi",      -1.2648,  36.8028, SiteStatus.Active,            TowerType.Rooftop,  15, 2, PowerSource.Grid,   2),
            ("KE-NBO-002", "Karen Ridge",        "Kenya",        "Nairobi",      -1.3197,  36.7076, SiteStatus.Active,            TowerType.Lattice,  40, 4, PowerSource.Hybrid, 1),
            ("KE-MBA-001", "Nyali Beach",        "Kenya",        "Mombasa",      -4.0435,  39.7008, SiteStatus.Active,            TowerType.Monopole, 30, 3, PowerSource.Grid,   2),
            ("KE-KSM-001", "Kisumu Lakeside",    "Kenya",        "Kisumu",       -0.0917,  34.7680, SiteStatus.Planned,           TowerType.Monopole, 35, 3, PowerSource.Solar,  0),
            ("ZA-JNB-001", "Sandton North",      "South Africa", "Johannesburg", -26.1076, 28.0567, SiteStatus.Active,            TowerType.Rooftop,  12, 3, PowerSource.Grid,   3),
            ("ZA-JNB-002", "Soweto West",        "South Africa", "Johannesburg", -26.2485, 27.8540, SiteStatus.Active,            TowerType.Lattice,  45, 4, PowerSource.Grid,   2),
            ("ZA-CPT-001", "Table View",         "South Africa", "Cape Town",    -33.8240, 18.4880, SiteStatus.Active,            TowerType.Monopole, 30, 3, PowerSource.Hybrid, 1),
            ("ZA-DUR-001", "Umhlanga Rocks",     "South Africa", "Durban",       -29.7270, 31.0840, SiteStatus.Decommissioned,    TowerType.Guyed,    55, 2, PowerSource.Grid,   0),
            ("EG-CAI-001", "Nasr City",          "Egypt",        "Cairo",         30.0561, 31.3301, SiteStatus.Active,            TowerType.Lattice,  50, 4, PowerSource.Grid,   3),
            ("EG-CAI-002", "Maadi Corniche",     "Egypt",        "Cairo",         29.9602, 31.2569, SiteStatus.Active,            TowerType.Rooftop,  18, 2, PowerSource.Grid,   1),
            ("EG-ALX-001", "Smouha",             "Egypt",        "Alexandria",    31.2156, 29.9553, SiteStatus.UnderConstruction, TowerType.Monopole, 35, 3, PowerSource.Hybrid, 0),
            ("GH-ACC-001", "Airport Residential", "Ghana",       "Accra",          5.6037, -0.1870, SiteStatus.Active,            TowerType.Monopole, 30, 3, PowerSource.Grid,   2),
            ("GH-ACC-002", "Tema Harbour",       "Ghana",        "Tema",           5.6698,  0.0166, SiteStatus.Active,            TowerType.Lattice,  45, 4, PowerSource.Diesel, 4),
            ("GH-KMS-001", "Kumasi Adum",        "Ghana",        "Kumasi",         6.6885, -1.6244, SiteStatus.Active,            TowerType.Guyed,    60, 3, PowerSource.Solar,  1),
            ("GH-TAM-001", "Tamale North",       "Ghana",        "Tamale",         9.4075, -0.8533, SiteStatus.Planned,           TowerType.Monopole, 35, 2, PowerSource.Solar,  0),
        };

        var rents = new[] { 1200m, 1450m, 1600m, 1800m };
        var i = 0;
        foreach (var s in sites)
        {
            var tower = new Tower { Type = s.Type, HeightMeters = s.Height, MaxTenants = s.Max, PowerSource = s.Power };
            for (var t = 0; t < s.Tenants; t++)
            {
                tower.Tenancies.Add(new Tenancy
                {
                    Operator = operators[(i + t) % operators.Count],
                    StartDate = new DateOnly(2020 + (i + t) % 5, 1 + t * 3, 1),
                    MonthlyRent = rents[(i + t) % rents.Length]
                });
            }

            db.Sites.Add(new Site
            {
                Code = s.Code, Name = s.Name, Country = s.Country, City = s.City,
                Latitude = s.Lat, Longitude = s.Lng, Status = s.Status,
                CommissionedOn = s.Status is SiteStatus.Active or SiteStatus.Decommissioned
                    ? new DateOnly(2015 + i % 5, 1 + i % 12, 1)
                    : null,
                Tower = tower
            });
            i++;
        }

        db.SaveChanges();
    }
}

# CRM Solution (Blazor WebAssembly + .NET 8)

This repository is a CRM (Customer Relationship Management) solution composed of a .NET 8 backend API and a Blazor WebAssembly frontend. This top-level README links to per-project READMEs that explain how to run and configure each part.

Projects
- `Backend_API` — ASP.NET Core Web API (Identity, JWT, EF Core). See `Backend_API/README.md`.
- `UI` — Blazor WebAssembly client (WASM). See `UI/README.md`.
- `Models` — Shared model types used by backend/frontend. See `Models/README.md`.
- `Resources` — Shared localization resources (resx files). See `Resources/README.md`.`

Quick start (recommended — Visual Studio)
1. Open the solution in Visual Studio 2022.
	2. Set multiple startup projects:
   - Right-click solution → __Set Startup Projects__ → choose __Multiple startup projects__ → set `Backend_API` and `UI` to `Start`.
3. Run (F5 or __Debug > Start Debugging__).

Quick start (CLI)
1. Clone:
   - `git clone https://github.com/DDuric21/CRM.git`
2. Run backend and UI in separate terminals:
   - `dotnet run --project Backend_API`
   - `dotnet run --project UI`
3. Open the UI URL printed by the `UI` project.

Where to look next
- If you want to run the frontend locally first, open `UI/README.md`.
- If you need to configure the database, JWT or CORS, open `Backend_API/README.md`.

Notes
- The `UI` project reads `appCustomSettings.json` at startup — ensure `SecureBackendUrl` points to your running API.
- The backend uses EF Core; run migrations or allow the seed routine to create the DB if configured.

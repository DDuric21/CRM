# UI (Blazor WebAssembly)

Purpose
This is the Blazor WebAssembly (WASM) frontend for the CRM. It calls the `Backend_API` for data and authentication.

Prerequisites
- .NET 8 SDK
- Browser for testing
- Backend API running and reachable

Key files
- `Program.cs` / `Startup` logic in the `UI` project
- `appCustomSettings.json` — frontend runtime settings (backend URL, profiles)
- `UI/Startup/Initializer.cs` — reads settings, sets up services and culture

Run locally (Visual Studio)
1. Ensure `Backend_API` is running or configured in `appCustomSettings.json`.
2. Set `UI` (and optionally `Backend_API`) as startup projects and run.

Run locally (CLI)
- `dotnet run --project UI`

Configuration
- Open `UI/wwwroot/appCustomSettings.json` (or the file referenced by startup) and set `SecureBackendUrl` to the backend API base URL (for example `https://localhost:5001/`).
- The app loads this file at startup (see `Initializer.SetupConfigurationAsync`).

Authentication & State
- The client uses an `AuthenticationStateProvider` and stores some data in local storage (culture, tokens).
- Ensure JWT-related responses from the backend match the client expectations.

Build & publish
- To build for production:
  - `dotnet publish -c Release --project UI -o ./publish`
- Adjust the backend CORS policy if you host the UI separately.

Troubleshooting
- If the UI cannot reach the API, double-check `appCustomSettings.json` and backend CORS configuration.
# Backend_API

## Purpose
This project is the server-side Web API for the CRM. It contains controllers, services, data access (EF Core), authentication (ASP.NET Identity + JWT), and seeding logic.

## Prerequisites
- .NET 8 SDK
- Database server (e.g., SQL Server) or local DB provider (check `appsettings.json`)

## Run locally (Visual Studio)
1. Open solution in Visual Studio.
2. Set `Backend_API` as a startup project, or set it as one of multiple startup projects (__Set Startup Projects__).
3. Run (F5 or __Debug > Start Debugging__).

## Run locally (CLI)
- `dotnet run --project Backend_API`

## Database / Migrations
- Apply EF Core migrations:
  - `dotnet tool install --global dotnet-ef` (if needed)
  - `dotnet ef database update --project Backend_API`
- The solution contains seed routines that may create sample data on startup (see seed classes in `Data/SeedData`).

## Configuration
- Edit `Backend_API/appsettings.json` or environment variables for:
  - `ConnectionStrings:DefaultConnection`
  - `JwtConfiguration` (Issuer, Audience, Key)
  - `CORS:AllowedOrigins` (allow the UI URL)
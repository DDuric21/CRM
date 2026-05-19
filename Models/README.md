# Models

Purpose
This project contains shared DTOs and domain model types used across the solution (Backend_API, UI, etc.). It is a .NET class library referenced by other projects.

Usage
- Reference the `Models` project from other projects via `<ProjectReference>`.
- When changing model contracts that cross the API boundary, update both backend and frontend usages.

Build
- `dotnet build Models`

Notes
- Keep this project stable: breaking changes to DTOs require coordinated changes in `Backend_API` and `UI`.
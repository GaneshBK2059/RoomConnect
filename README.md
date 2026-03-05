# RoomConnect

RoomConnect is a room rental and booking platform built with ASP.NET Core Web API and PostgreSQL. It provides APIs for user authentication, room listing management, bookings, and reviews.

## Platform Overview

RoomConnect supports two primary user roles:

- `HOST`: Creates and manages room listings.
- `RENTER`: Searches rooms, creates bookings, and writes reviews.

Core platform capabilities:

- JWT-based authentication and authorization
- Room discovery and detailed room views
- Booking creation and cancellation
- Review and rating system
- Swagger/OpenAPI documentation for API exploration

## Tech Stack

- Backend: ASP.NET Core (.NET 8)
- Database: PostgreSQL
- Data access: Npgsql-based infrastructure/repositories
- API docs: Swagger UI
- Solution management: `RoomConnect.sln`

## Repository Structure

```text
RoomConnect/
|-- server/
|   |-- RoomConnect.Api/            # API host (controllers, startup/config)
|   |-- RoomConnect.Application/    # Business/application logic
|   |-- RoomConnect.Domain/         # Domain models and entities
|   |-- RoomConnect.Infrastructure/ # Data access and persistence
|   |-- RoomConnect.Tests/          # Unit/integration test project
|   |-- SqlScripts/                 # Database initialization scripts
|   `-- DbInit/                     # Database initialization helpers/assets
|-- client/                         # Frontend application
|-- QUICKSTART.md
|-- DATABASE_SETUP.md
|-- CONFIGURATION_CHECKLIST.md
`-- RoomConnect.sln
```

## Quick Start

### 1. Prerequisites

- .NET 8 SDK
- PostgreSQL 12+

### 2. Create and Initialize Database

Run PostgreSQL and create the database:

```sql
CREATE DATABASE roomconnect;
```

Initialize schema/data:

```powershell
psql -U postgres -d roomconnect -f server/SqlScripts/01_init_database.sql
```

### 3. Configure API Settings

Update `server/RoomConnect.Api/appsettings.json` with your local values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=roomconnect;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "RoomConnect",
    "Audience": "RoomConnectUsers"
  }
}
```

### 4. Run the API

```powershell
cd server/RoomConnect.Api
dotnet restore
dotnet build
dotnet run
```

API base URL (default):

- `https://localhost:5001`

Swagger UI:

- `https://localhost:5001/swagger`

## Development Process

### Recommended Workflow

1. Pull latest changes and create a feature branch.
2. Apply database script changes in `server/SqlScripts` when needed.
3. Implement API/domain/infrastructure changes in the appropriate projects.
4. Build and run tests before opening a PR.
5. Manually verify key flows in Swagger (auth, rooms, bookings, reviews).

### Build and Test

From repository root:

```powershell
dotnet restore RoomConnect.sln
dotnet build RoomConnect.sln
dotnet test server/RoomConnect.Tests
```

### API Verification Checklist

- Register/Login returns valid JWT
- Protected endpoints require `Bearer <token>`
- Room creation and lookup endpoints behave as expected
- Booking conflict validation works
- Review rating validation enforces 1-5 range

## Configuration and Environment

For production, prefer environment variables over hardcoded secrets:

```powershell
$env:ConnectionStrings__DefaultConnection="Host=prod-db;Port=5432;Database=roomconnect;Username=produser;Password=SECURE_PASSWORD"
$env:Jwt__Key="LONG_SECURE_JWT_KEY_32_CHARS_MIN"
$env:Jwt__Issuer="RoomConnect"
$env:Jwt__Audience="RoomConnectUsers"
```

## Security Notes

- Never commit real credentials or secrets.
- Replace default/local passwords before deployment.
- Use a strong JWT signing key (minimum 32 characters).
- Keep HTTPS enabled in non-local environments.
- Add rate limiting, monitoring, and strict input validation for production.

## Additional Documentation

- [QUICKSTART.md](QUICKSTART.md)
- [DATABASE_SETUP.md](DATABASE_SETUP.md)
- [CONFIGURATION_CHECKLIST.md](CONFIGURATION_CHECKLIST.md)

## Roadmap Ideas

- Complete and integrate the frontend in `client/`
- Add CI checks for build/test/security scans
- Add API versioning and health checks
- Add automated database migration strategy

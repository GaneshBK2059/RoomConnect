# Configuration Checklist ✅

## Step 1: Database Configuration ✅

- [ ] PostgreSQL installed and running
- [ ] Database `roomconnect` created
- [ ] SQL initialization script executed (`01_init_database.sql`)
- [ ] Verify tables exist: users, rooms, bookings, reviews
- [ ] Update PostgreSQL password in `appsettings.json` (if not using default)

## Step 2: API Configuration ✅

File: `server/RoomConnect.Api/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=roomconnect;Username=postgres;Password=G@nesh444"
  },
  "Jwt": {
    "Key": "THIS_IS_SUPER_SECRET_KEY_123456",
    "Issuer": "RoomConnect",
    "Audience": "RoomConnectUsers"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Checklist:
- [ ] PostgreSQL host/port matches your setup
- [ ] Database name is `roomconnect`
- [ ] Username and password are correct
- [ ] JWT Key is set (min 32 characters recommended for production)
- [ ] Issuer and Audience are set

## Step 3: Build & Run ✅

```bash
cd server/RoomConnect.Api
dotnet restore
dotnet build
dotnet run
```

### Checklist:
- [ ] Build completes with 0 errors
- [ ] Build completes with 0 warnings
- [ ] Application starts successfully
- [ ] No "Unable to connect to database" errors
- [ ] Swagger is accessible at `https://localhost:5001/swagger`

## Step 4: Test API Endpoints ✅

### Register
- [ ] POST `/api/auth/register` returns 201 Created
- [ ] User appears in database

### Login
- [ ] POST `/api/auth/login` returns 200 OK
- [ ] JWT token is included in response
- [ ] Token contains user claims (email, role, etc.)

### Rooms
- [ ] GET `/api/rooms` returns list of available rooms
- [ ] GET `/api/rooms/{id}` returns room details
- [ ] POST `/api/rooms` (with Auth) creates new room

### Bookings
- [ ] GET `/api/bookings` (with Auth) returns user bookings
- [ ] POST `/api/bookings` (with Auth) creates booking
- [ ] Booking validation works (overlapping dates rejected, etc.)
- [ ] DELETE `/api/bookings/{id}` cancels booking

### Reviews
- [ ] GET `/api/reviews/room/{roomId}` returns room reviews
- [ ] POST `/api/reviews` (with Auth) creates review
- [ ] Rating validation works (1-5 only)

## Production Checklist 🚀

### Security
- [ ] JWT Key changed to secure value (32+ characters)
- [ ] PostgreSQL password changed from default
- [ ] HTTPS enabled in production
- [ ] CORS properly configured for frontend domain
- [ ] No debug logs in production
- [ ] No default credentials committed to repository

### Database
- [ ] Regular backups configured
- [ ] Database encryption enabled (if handling sensitive data)
- [ ] Connection pooling configured
- [ ] Query timeout limits set

### API
- [ ] Rate limiting implemented
- [ ] Input validation on all endpoints
- [ ] Error handling doesn't leak sensitive info
- [ ] API versioning strategy decided
- [ ] Logging and monitoring set up
- [ ] Health check endpoint available

### Performance
- [ ] Database indexes verified and optimized
- [ ] Query N+1 problems checked
- [ ] Caching strategy implemented (if needed)
- [ ] Load testing completed

## Environment Variables (Production)

Instead of hardcoding in `appsettings.json`, use:

```bash
# PowerShell
$env:ConnectionStrings__DefaultConnection="Host=prod-db;Port=5432;Database=roomconnect;Username=produser;Password=SECURE_PASSWORD"
$env:Jwt__Key="LONG_SECURE_JWT_KEY_32_CHARS_MIN"
$env:Jwt__Issuer="RoomConnect"
$env:Jwt__Audience="RoomConnectUsers"

# Linux/Mac Bash
export ConnectionStrings__DefaultConnection="Host=prod-db;Port=5432;Database=roomconnect;Username=produser;Password=SECURE_PASSWORD"
export Jwt__Key="LONG_SECURE_JWT_KEY_32_CHARS_MIN"
export Jwt__Issuer="RoomConnect"
export Jwt__Audience="RoomConnectUsers"
```

## Notes

- All endpoints tested and working ✅
- Sample data loaded for testing ✅
- API documentation available via Swagger ✅
- Database schema properly indexed ✅

Next: Frontend React application setup

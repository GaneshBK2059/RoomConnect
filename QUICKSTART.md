# RoomConnect MVP - Quick Start Guide

## Prerequisites

- .NET 8.0 SDK installed
- PostgreSQL 12+ installed and running
- Git (optional)

## Setup Instructions

### 1. Database Setup

Follow the instructions in `DATABASE_SETUP.md`:

```bash
# Create database
# CREATE DATABASE roomconnect;

# Run the SQL initialization script
# psql -U postgres -d roomconnect -f server/SqlScripts/01_init_database.sql
```

Or manually run the SQL script from `server/SqlScripts/01_init_database.sql` in pgAdmin or psql.

### 2. Update Configuration (if needed)

Edit `server/RoomConnect.Api/appsettings.json` to match your PostgreSQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=roomconnect;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY_HERE_MIN_32_CHARS",
    "Issuer": "RoomConnect",
    "Audience": "RoomConnectUsers"
  }
}
```

### 3. Run the API

```bash
cd server/RoomConnect.Api

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

The API will start at: `https://localhost:5001`

### 4. Access Swagger Documentation

Open your browser and go to:
```
https://localhost:5001/swagger
```

You'll see all available API endpoints with the ability to test them directly.

---

## API Endpoints Overview

### Authentication

**Register User**
```
POST /api/auth/register
Content-Type: application/json

{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "SecurePassword123!",
  "phone": "555-0001",
  "role": "RENTER"
}
```

**Login**
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

Response includes JWT token:
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "fullName": "John Doe",
    "email": "john@example.com",
    "role": "RENTER"
  }
}
```

### Rooms

**Get All Available Rooms**
```
GET /api/rooms
```

**Get Room Details**
```
GET /api/rooms/{roomId}
```

**Create Room** (Requires Auth)
```
POST /api/rooms
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "title": "Beautiful Apartment",
  "description": "Cozy 1BR apartment",
  "address": "123 Main St",
  "city": "New York",
  "state": "NY",
  "zipCode": "10001",
  "latitude": 40.7128,
  "longitude": -74.0060,
  "pricePerNight": 150.00,
  "maxGuests": 2,
  "bedrooms": 1,
  "bathrooms": 1,
  "wifi": true,
  "parking": false,
  "ac": true,
  "heating": true,
  "imageUrl": "https://example.com/image.jpg"
}
```

### Bookings

**Get My Bookings** (Requires Auth)
```
GET /api/bookings
Authorization: Bearer <TOKEN>
```

**Create Booking** (Requires Auth)
```
POST /api/bookings
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "roomId": 1,
  "checkInDate": "2026-02-01T00:00:00Z",
  "checkOutDate": "2026-02-05T00:00:00Z",
  "numberOfGuests": 2,
  "notes": "Early check-in requested"
}
```

**Cancel Booking** (Requires Auth)
```
DELETE /api/bookings/{bookingId}
Authorization: Bearer <TOKEN>
```

### Reviews

**Get Room Reviews**
```
GET /api/reviews/room/{roomId}
```

**Create Review** (Requires Auth)
```
POST /api/reviews
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "roomId": 1,
  "rating": 5,
  "title": "Excellent!",
  "comment": "Amazing place, highly recommend!"
}
```

---

## Testing with Swagger

1. Go to `/swagger`
2. Click **Authorize** button
3. Paste your JWT token (from login response) with `Bearer ` prefix
4. Try out all endpoints

---

## Project Structure

```
RoomConnect/
├── server/
│   ├── RoomConnect.Api/              # Web API
│   ├── RoomConnect.Application/      # Business logic
│   ├── RoomConnect.Domain/           # Models & entities
│   ├── RoomConnect.Infrastructure/   # Database access
│   ├── RoomConnect.Tests/            # Unit tests
│   └── SqlScripts/                   # Database scripts
├── client/                           # React frontend (coming next)
├── RoomConnect.sln                   # Solution file
└── README.md
```

---

## Next Steps

1. ✅ Database setup
2. ✅ API running with Swagger
3. Build React frontend (coming next)
4. Connect frontend to API
5. Deploy to cloud

---

## Troubleshooting

### API won't start
- Check PostgreSQL is running: `psql -U postgres -c "SELECT 1"`
- Verify connection string in `appsettings.json`
- Check if port 5001 is available

### Database connection failed
- Ensure PostgreSQL is running
- Verify database exists: `psql -U postgres -l`
- Check credentials match between PostgreSQL and `appsettings.json`

### HTTPS certificate error
- For local development, you can disable HTTPS:
  In `Program.cs`, comment out: `app.UseHttpsRedirection();`

---

## Security Notes ⚠️

- **Never** commit credentials to git
- Change default password from `G@nesh444` before production
- Use strong JWT key (32+ characters)
- Enable HTTPS in production
- Implement rate limiting for production
- Add input validation and sanitization

---

Enjoy building with RoomConnect! 🚀

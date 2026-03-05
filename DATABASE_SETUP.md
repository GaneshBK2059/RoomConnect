# RoomConnect MVP - Database Setup Guide

## Prerequisites

You need PostgreSQL installed and running. Download from: https://www.postgresql.org/download/

## Database Setup Steps

### Step 1: Create the Database

Open PostgreSQL command line (psql) or pgAdmin and run:

```sql
CREATE DATABASE roomconnect;
```

### Step 2: Connect to the Database

```bash
psql -U postgres -d roomconnect
```

Or in pgAdmin, right-click on databases and create a new database named `roomconnect`.

### Step 3: Run the Initialization Script

Copy the contents of `01_init_database.sql` and run it in PostgreSQL:

```bash
psql -U postgres -d roomconnect -f SqlScripts/01_init_database.sql
```

Or paste the SQL script directly in pgAdmin Query Editor.

## Configuration

The connection string in `appsettings.json` is:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=roomconnect;Username=postgres;Password=G@nesh444"
}
```

**⚠️ Important Security Notes:**
- Change the PostgreSQL password from `G@nesh444` to a secure password
- Update the `appsettings.json` to match your PostgreSQL credentials
- Never commit sensitive credentials to version control
- Use environment variables for production (see below)

## Using Environment Variables (Production)

For production, set connection string via environment variables:

```bash
# Windows PowerShell
$env:ConnectionStrings__DefaultConnection="Host=your-host;Port=5432;Database=roomconnect;Username=user;Password=pass"

# Linux/Mac
export ConnectionStrings__DefaultConnection="Host=your-host;Port=5432;Database=roomconnect;Username=user;Password=pass"
```

## JWT Configuration

The JWT settings in `appsettings.json`:

```json
"Jwt": {
  "Key": "THIS_IS_SUPER_SECRET_KEY_123456",
  "Issuer": "RoomConnect",
  "Audience": "RoomConnectUsers"
}
```

**⚠️ For Production:**
- Generate a strong, unique JWT key (at least 32 characters)
- Set via environment variables:
  ```bash
  $env:Jwt__Key="your-super-secret-key-here"
  $env:Jwt__Issuer="RoomConnect"
  $env:Jwt__Audience="RoomConnectUsers"
  ```

## Verify Database Setup

To verify the tables were created, connect to the database and run:

```sql
\dt  -- List all tables (in psql)
-- Or in pgAdmin, expand the roomconnect database in the left panel
```

Expected tables:
- `users`
- `rooms`
- `bookings`
- `reviews`

## Sample Data

The script includes sample data:

**Users:**
- John Host (host@example.com)
- Jane Renter (renter@example.com)
- Bob Smith (bob@example.com)

**Rooms:**
- 3 sample rooms in New York and San Francisco

**Bookings:**
- 2 sample bookings

**Reviews:**
- 2 sample reviews

Note: Password hashes in sample data are placeholders and won't work for login. Use the Register endpoint to create real users.

## Troubleshooting

### Connection Failed
- Ensure PostgreSQL service is running
- Check hostname, port, username, password
- Verify database name is correct

### Permission Denied
- Ensure the PostgreSQL user has permissions
- May need to adjust PostgreSQL user permissions

### Tables Already Exist
- The script uses `CREATE TABLE IF NOT EXISTS`, so it's safe to run multiple times
- To reset, run: `DROP TABLE IF EXISTS reviews, bookings, rooms, users CASCADE;`

## API Configuration

No additional configuration needed. The API will:
1. Read the connection string from `appsettings.json`
2. Use Npgsql to connect to PostgreSQL
3. Execute queries using the repositories

You're ready to start the API! 🚀

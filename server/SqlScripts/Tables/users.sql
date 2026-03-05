CREATE TYPE user_role AS ENUM ('RENTER', 'OWNER', 'ADMIN');

CREATE TABLE users (
    id              BIGSERIAL PRIMARY KEY,
    full_name       VARCHAR(100) NOT NULL,
    email           VARCHAR(255) NOT NULL UNIQUE,
    phone           VARCHAR(20) UNIQUE,
    password_hash   VARCHAR(255) NOT NULL,
    
    role            user_role NOT NULL DEFAULT 'RENTER',
    
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    email_verified  BOOLEAN NOT NULL DEFAULT FALSE,
    
    avatar_url      TEXT,
    bio             TEXT,
    sys_created      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    sys_updated      TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
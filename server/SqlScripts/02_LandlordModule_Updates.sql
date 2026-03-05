-- 02_LandlordModule_Updates.sql

-- 1. Drop constraints that depend on rooms.id
ALTER TABLE bookings DROP CONSTRAINT bookings_room_id_fkey;
ALTER TABLE reviews DROP CONSTRAINT reviews_room_id_fkey;

-- 2. Alter rooms.id to UUID
ALTER TABLE rooms ALTER COLUMN id DROP DEFAULT;
-- NOTE: If you have existing data, this might drop it or assign random UUIDs. Assuming a dev environment:
ALTER TABLE rooms ALTER COLUMN id TYPE UUID USING gen_random_uuid();
ALTER TABLE rooms ALTER COLUMN id SET DEFAULT gen_random_uuid();

-- 3. Alter bookings.room_id to UUID
ALTER TABLE bookings ALTER COLUMN room_id TYPE UUID USING gen_random_uuid();

-- 4. Alter reviews.room_id to UUID
ALTER TABLE reviews ALTER COLUMN room_id TYPE UUID USING gen_random_uuid();

-- 5. Restore constraints
ALTER TABLE bookings ADD CONSTRAINT bookings_room_id_fkey FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE;
ALTER TABLE reviews ADD CONSTRAINT reviews_room_id_fkey FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE;

-- 6. Rename columns in rooms to match new requirements
ALTER TABLE rooms RENAME COLUMN owner_id TO landlord_id;
ALTER TABLE rooms RENAME COLUMN price_per_night TO price;
ALTER TABLE rooms RENAME COLUMN sys_created TO created_at;
ALTER TABLE rooms RENAME COLUMN sys_updated TO updated_at;

-- 7. Add RoomType column to rooms
ALTER TABLE rooms ADD COLUMN IF NOT EXISTS room_type VARCHAR(50) NOT NULL DEFAULT 'Entire Place';

-- 8. Create room_images table
CREATE TABLE IF NOT EXISTS room_images (
    id BIGSERIAL PRIMARY KEY,
    room_id UUID NOT NULL REFERENCES rooms(id) ON DELETE CASCADE,
    image_url VARCHAR(500) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_room_images_room_id ON room_images(room_id);

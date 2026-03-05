-- Reviews table
CREATE TABLE IF NOT EXISTS reviews (
    id BIGSERIAL PRIMARY KEY,
    room_id BIGINT NOT NULL REFERENCES rooms(id) ON DELETE CASCADE,
    user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    rating INTEGER NOT NULL CHECK (rating >= 1 AND rating <= 5),
    title VARCHAR(255) NOT NULL,
    comment TEXT NOT NULL,
    sys_created TIMESTAMP NOT NULL DEFAULT NOW(),
    sys_updated TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_reviews_room_id ON reviews(room_id);
CREATE INDEX idx_reviews_user_id ON reviews(user_id);

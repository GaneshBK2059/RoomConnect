-- Sample data for testing

-- Insert sample users
INSERT INTO users (full_name, email, phone, password_hash, role, is_active, email_verified) 
VALUES 
    ('John Host', 'host@example.com', '555-0001', '$2a$11$YourHashedPasswordHere1', 'HOST', true, true),
    ('Jane Renter', 'renter@example.com', '555-0002', '$2a$11$YourHashedPasswordHere2', 'RENTER', true, true),
    ('Bob Smith', 'bob@example.com', '555-0003', '$2a$11$YourHashedPasswordHere3', 'HOST', true, true)
ON CONFLICT (email) DO NOTHING;

-- Insert sample rooms
INSERT INTO rooms (owner_id, title, description, address, city, state, zip_code, latitude, longitude, price_per_night, max_guests, bedrooms, bathrooms, wifi, parking, ac, heating, image_url, is_available)
VALUES
    (1, 'Cozy Downtown Apartment', 'Beautiful 1BR apartment in the heart of the city', '123 Main St', 'New York', 'NY', '10001', 40.7128, -74.0060, 150.00, 2, 1, 1, true, false, true, true, 'https://via.placeholder.com/300x200?text=Apt1', true),
    (1, 'Modern Studio with View', 'Stunning studio with city view', '456 Park Ave', 'New York', 'NY', '10022', 40.7614, -73.9776, 120.00, 1, 1, 1, true, true, true, true, 'https://via.placeholder.com/300x200?text=Studio', true),
    (3, 'Spacious Family Home', 'Large house perfect for families', '789 Oak Dr', 'San Francisco', 'CA', '94102', 37.7749, -122.4194, 250.00, 6, 3, 2, true, true, true, true, 'https://via.placeholder.com/300x200?text=House', true)
ON CONFLICT DO NOTHING;

-- Insert sample bookings
INSERT INTO bookings (user_id, room_id, check_in_date, check_out_date, number_of_guests, total_price, status)
VALUES
    (2, 1, '2026-02-01', '2026-02-05', 2, 600.00, 'CONFIRMED'),
    (2, 2, '2026-02-10', '2026-02-12', 1, 240.00, 'PENDING')
ON CONFLICT DO NOTHING;

-- Insert sample reviews
INSERT INTO reviews (room_id, user_id, rating, title, comment)
VALUES
    (1, 2, 5, 'Amazing place!', 'Beautiful apartment with great location. The host was very responsive and helpful.'),
    (1, 2, 4, 'Very comfortable', 'Great stay. Only minor issue was the wifi speed.')
ON CONFLICT DO NOTHING;

CREATE TABLE IF NOT EXISTS vehicles (
    id UUID PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    registration_number VARCHAR(50) NOT NULL,
    vehicle_type VARCHAR(100) NOT NULL DEFAULT 'unknown',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_vehicles_registration_number UNIQUE (registration_number)
);

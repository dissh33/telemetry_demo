CREATE TABLE IF NOT EXISTS devices (
    id UUID PRIMARY KEY,
    vehicle_id UUID NOT NULL REFERENCES vehicles(id) ON DELETE CASCADE,
    name VARCHAR(200) NOT NULL,
    device_type VARCHAR(100) NOT NULL,
    serial_number VARCHAR(100),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS ix_devices_vehicle_id ON devices(vehicle_id);

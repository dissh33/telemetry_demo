CREATE TABLE IF NOT EXISTS telemetry_records (
    id UUID PRIMARY KEY,
    vehicle_id UUID NOT NULL REFERENCES vehicles(id),
    device_id UUID REFERENCES devices(id),
    telemetry_type VARCHAR(50) NOT NULL,
    payload JSONB NOT NULL,
    recorded_at TIMESTAMPTZ NOT NULL,
    received_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS ix_telemetry_vehicle_id ON telemetry_records(vehicle_id);
CREATE INDEX IF NOT EXISTS ix_telemetry_recorded_at ON telemetry_records(recorded_at DESC);
CREATE INDEX IF NOT EXISTS ix_telemetry_type ON telemetry_records(telemetry_type);

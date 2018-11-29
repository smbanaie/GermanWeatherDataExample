DO $$
--
-- Schema
--
BEGIN


IF NOT EXISTS (SELECT 1 FROM information_schema.schemata WHERE schema_name = 'sample') THEN

    CREATE SCHEMA sample;

END IF;

--
-- Tables
--
IF NOT EXISTS (
	SELECT 1 
	FROM information_schema.tables 
	WHERE  table_schema = 'sample' 
	AND table_name = 'station'
) THEN

CREATE TABLE sample.station
(
    identifier VARCHAR(5) NOT NULL,
    name VARCHAR(255) NOT NULL,
    start_date TIMESTAMPTZ,
    end_date TIMESTAMPTZ,
    station_height SMALLINT,
    state VARCHAR(255),
    latitude REAL,
    longitude REAL

);

END IF;

IF NOT EXISTS (
	SELECT 1 
	FROM information_schema.tables 
	WHERE  table_schema = 'sample' 
	AND table_name = 'weather_data'
) THEN

CREATE TABLE sample.weather_data
(
    station_identifier VARCHAR(5) NOT NULL,
    timestamp TIMESTAMPTZ NOT NULL,
    quality_code SMALLINT,
    station_pressure DOUBLE PRECISION NULL,
    air_temperature_at_2m DOUBLE PRECISION NULL,
    air_temperature_at_5cm DOUBLE PRECISION NULL,
    relative_humidity DOUBLE PRECISION NULL,
    dew_point_temperature_at_2m DOUBLE PRECISION NULL        
);

END IF;


END;
$$
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
    identifier VARCHAR(5) PRIMARY KEY NOT NULL,
    name VARCHAR(255) NOT NULL,
    start_date TIMESTAMP,
    end_date TIMESTAMP,
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
    timestamp TIMESTAMP NOT NULL,
    quality_code SMALLINT,
    station_pressure REAL NULL,
    air_temperature_at_2m REAL NULL,
    air_temperature_at_5cm REAL NULL,
    relative_humidity REAL NULL,
    dew_point_temperature_at_2m REAL NULL        
);

END IF;


END;
$$
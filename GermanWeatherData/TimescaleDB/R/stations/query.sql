SELECT identifier, name, start_date, end_date, station_height, state, latitude, longitude
FROM sample.station
WHERE end_date >= $1
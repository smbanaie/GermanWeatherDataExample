SELECT s.identifier "station", s.longitude "lon", s.latitude "lat", date_trunc('hour', w.timestamp) "timestamp", avg(w.air_temperature_at_2m) "temperature"
FROM sample.weather_data w
    INNER JOIN sample.station s on w.station_identifier = s.identifier
WHERE s.identifier = '{{station}}' AND (w.timestamp >= '{{start_date}}'::date AND w.timestamp < '{{end_date}}'::date)
GROUP BY "station", "timestamp"
ORDER BY "timestamp" ASC
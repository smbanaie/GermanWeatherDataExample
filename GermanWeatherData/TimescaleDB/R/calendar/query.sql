SELECT s.identifier "station", date_trunc('day', w.timestamp)::date "day", avg(w.air_temperature_at_2m) "temperature"
FROM sample.weather_data w
    INNER JOIN sample.station s on w.station_identifier = s.identifier
WHERE s.identifier = '{{station}}' AND (w.timestamp >= '{{start_date}}'::date AND w.timestamp < '{{end_date}}'::date)
GROUP BY "station", "day"
ORDER BY "day" ASC
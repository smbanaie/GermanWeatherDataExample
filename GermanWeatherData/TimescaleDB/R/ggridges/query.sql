SELECT s.identifier as "station", date_trunc('day', w.timestamp) "day",  date_part('month', w.timestamp) "month_idx", to_char(w.timestamp, 'Month') "month",  avg(w.air_temperature_at_2m) "avg_temp"
FROM sample.weather_data w
    INNER JOIN sample.station s ON w.station_identifier = s.identifier
WHERE s.identifier = $1 AND w.timestamp >= $2 AND w.timestamp < $3
GROUP BY 1, 2, 3, 4
ORDER BY 1, 2, 3
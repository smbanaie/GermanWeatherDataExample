WITH AverageTemperatues AS(
	SELECT d.WBAN as wban, AVG(d.Temperature) as temperature
	FROM sample.LocalWeatherData d
	WHERE d.Temperature IS NOT NULL 
		AND d.Timestamp BETWEEN '{{start_date}}' AND '{{end_date}}'
	GROUP BY d.WBAN
)
SELECT s.WBAN as wban, s.Latitude as lat, s.Longitude as lon, t.Temperature as temperature
FROM sample.Station s 
	INNER JOIN AverageTemperatues t on s.WBAN = t.WBAN
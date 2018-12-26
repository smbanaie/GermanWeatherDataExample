WITH BoundedTimeSeries as (
	SELECT dateadd(hour, datediff(hour, 0, Timestamp), 0) as Timestamp, Temperature
	FROM [sample].[LocalWeatherData] weatherData
	WHERE WBAN = '{{wban}}' AND Timestamp BETWEEN '{{start_date}}' and '{{end_date}}'
),
DistinctSeries as (
	SELECT Timestamp, AVG(Temperature) as Temperature
	FROM BoundedTimeSeries b
	GROUP BY Timestamp
)
SELECT d.Timestamp as timestamp, d.Temperature as temperature
FROM DistinctSeries d
WHERE Temperature is not null
ORDER BY Timestamp ASC
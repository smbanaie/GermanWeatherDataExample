SELECT WBAN as wban, AVG(Temperature) as averageTemperature, YEAR(Timestamp) as year, MONTH(Timestamp) as month, DAY(Timestamp) as day
FROM [sample].[LocalWeatherData] weatherData
WHERE WBAN =  {{wban}}
GROUP BY WBAN,  YEAR(Timestamp), MONTH(Timestamp), DAY(Timestamp)
ORDER BY Year ASC, Month ASC, Day ASC 

## Database Size ##

```
SELECT pg_database.datname as "database_name", pg_size_pretty(pg_database_size(pg_database.datname)) AS size_in_mb
FROM pg_database 
WHERE pg_database.datname like 'sampledb'
ORDER by size_in_mb DESC;
```

### Result ###

```
database_name  size_in_mb
-------------- --------------------
sampledb       37 GB
```

## Number of Measurements ##

```
SELECT COUNT(*) 
FROM sample.weather_data;
```

### Result ###

```
count
---------
406241469
```

### Runtime ###

```
230 seconds
```



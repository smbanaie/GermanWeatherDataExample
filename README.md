# GermanWeatherDataExample #

## Project ##

Going through a lot of pain has taught me, that you have to know about the capabilities and pitfalls of a technology, before putting it in production. These days it's all about "Big Data", but how well do technologies work when we actually throw realistic data at it? What knobs have to be turned to make the technologies scale? How far does a single machine take us?

In the project I want to benchmark [TimescaleDB], [Elasticsearch], [SQL Server] and [InfluxDB] on the 10 Minute Weather Data for Germany.

## Dataset ##

The [DWD Open Data] portal of the [Deutscher Wetterdienst (DWD)] gives access to the historical weather data in Germany. I decided to analyze the available historical Air Temperature data for Germany given in a 10 minute resolution ([FTP](https://opendata.dwd.de/climate_environment/CDC/observations_germany/climate/10_minutes/air_temperature/historical/)). If you want to recreate the example, you can find the list of files here: [GermanWeatherDataExample/Resources/files.md](https://github.com/bytefish/GermanWeatherDataExample/blob/master/GermanWeatherData/Resources/files.md).

The DWD dataset is given as CSV files and has a size of approximately 25.5 GB.

## Status ##

### TimescaleDB ###

TimescaleDB was able to import the entire dataset. The final database has ``406241469`` measurements and has a file size 
of ``37 GB``. More Queries and Performance analysis to follow!

### InfluxDB ###

InfluxDB 1.7.1 is currently unable to import the entire dataset. It consumes too much RAM under load and could not write 
the batches anymore. I already tried switching to disk-based indices (``tsi1``) in the configuration, but it didn't change 
the excessive memory consumption.

[Paul Yuan](http://puyuan.github.io/influxdb-tag-cardinality-memory-performance) has written a blog post on a similar problem:

> However, there is a huge caveat here. In my implementation of a 200MB dataset, influxDB quickly consumed my 
> entire 12GB of memory. Memory usage stayed at 95% and never seemed to be declining, not until I killed the 
> process. There was some discussion about memory leak and one on WAL log not flushed fast enough.

... the post goes on to explain:

> The real problem was due to tags, especially its cardinality. [...] Basically, influxdb constructs an inverted 
> index in memory, growing with the cardinality of the tags. [...] When influxdb counts cardinality in a Measurement, 
> it counts the combination of all tags. For example, if my measurement has the following tags: 3 os, 200 devices, 
> 3 browsers, then the cardinality is 3 x 200 x 3=1800.

The thing is: I anticipated something like this and made sure I don't have too many tags. Currently the measurements only 
consist the Station Identifier (505 Stations) and a Quality Code (3 Values). So the cardinality of the tags shouldn't be too 
high and the tags are not highly dynamic.

Probably helpful links for further research:

* http://puyuan.github.io/influxdb-tag-cardinality-memory-performance
* https://github.com/influxdata/influxdb/issues/3967

## Resources ##

### SQL Server ###

* [More Clustered Columnstore Improvements in SQL Server 2016](http://www.nikoport.com/2015/09/15/columnstore-indexes-part-66-more-clustered-columnstore-improvements-in-sql-server-2016/)
* [SQL Server Python tutorials](https://docs.microsoft.com/en-us/sql/advanced-analytics/tutorials/sql-server-python-tutorials)

### TimescaleDB ###

* [TimescaleDB Documentation](https://docs.timescale.com)
* [PostgreSQL 10.6 Documentation](https://www.postgresql.org/docs/10/index.html)
* [Time-series data: Why (and how) to use a relational database instead of NoSQL](https://blog.timescale.com/time-series-data-why-and-how-to-use-a-relational-database-instead-of-nosql-d0cd6975e87c)

### InfluxDB ###

* [InfluxDB Documentation](https://docs.influxdata.com/influxdb/)
* [InfluxDB .NET Collector](https://github.com/influxdata/influxdb-csharp)

### Elasticsearch ###

* [Removal of Mapping Types in Elasticsearch 6.0](https://www.elastic.co/blog/removal-of-mapping-types-elasticsearch)
* [Elasticsearch as a Time Series Data Store](https://www.elastic.co/blog/elasticsearch-as-a-time-series-data-store)
* [NEST: Auto Mapping](https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/auto-map.html)


[TimescaleDB]: https://www.timescale.com/
[Elasticsearch]: https://www.elastic.co/
[SQL Server]: https://www.microsoft.com/de-de/sql-server/sql-server-2017
[InfluxDB]: https://www.influxdata.com/


[DWD Open Data]: https://opendata.dwd.de/
[E-Government Act - EgovG]: http://www.gesetze-im-internet.de/englisch_egovg/index.html
["Open-Data-Gesetz" (§ 12 a EGovG)]: https://www.bmi.bund.de/DE/themen/moderne-verwaltung/open-government/open-data/open-data-node.html
[Deutscher Wetterdienst (DWD)]: https://www.dwd.de

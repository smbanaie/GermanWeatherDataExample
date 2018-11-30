# GermanWeatherDataExample #

## Project ##

Going through a lot of pain has taught me, that you have to know about the capabilities and pitfalls of a technology, before putting it in production. These days it's all about "Big Data", but how well do technologies work when we actually throw realistic data at it? What knobs have to be turned to make the technologies scale? How far does a single machine take us?

In the project I want to benchmark [TimescaleDB], [Elasticsearch], [SQL Server] and [InfluxDB] on the 10 Minute Weather Data for Germany, which is available as Open Data in the [DWD Open Data](https://opendata.dwd.de/) portal. The dataset is available as CSV files and has a size of 25.5 GB.

## SQL Server ##

* [More Clustered Columnstore Improvements in SQL Server 2016](http://www.nikoport.com/2015/09/15/columnstore-indexes-part-66-more-clustered-columnstore-improvements-in-sql-server-2016/)
* [SQL Server Python tutorials](https://docs.microsoft.com/en-us/sql/advanced-analytics/tutorials/sql-server-python-tutorials)

## TimescaleDB ##

* [TimescaleDB Documentation](https://docs.timescale.com)
* [PostgreSQL 10.6 Documentation](https://www.postgresql.org/docs/10/index.html)
* [Time-series data: Why (and how) to use a relational database instead of NoSQL](https://blog.timescale.com/time-series-data-why-and-how-to-use-a-relational-database-instead-of-nosql-d0cd6975e87c)
* 

## InfluxDB ##

* [InfluxDB Documentation](https://docs.influxdata.com/influxdb/)
* [InfluxDB .NET Collector](https://github.com/influxdata/influxdb-csharp)

## Elasticsearch ##

* [Removal of Mapping Types in Elasticsearch 6.0](https://www.elastic.co/blog/removal-of-mapping-types-elasticsearch)
* [Elasticsearch as a Time Series Data Store](https://www.elastic.co/blog/elasticsearch-as-a-time-series-data-store)
* [NEST: Auto Mapping](https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/auto-map.html)

[TimescaleDB]: https://www.timescale.com/
[Elasticsearch]: https://www.elastic.co/
[SQL Server]: https://www.microsoft.com/de-de/sql-server/sql-server-2017
[InfluxDB]: https://www.influxdata.com/


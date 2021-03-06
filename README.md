# GermanWeatherDataExample #

## Project ##

Going through a lot of pain has taught me, that you have to know about the capabilities and pitfalls of a technology, before putting it in production. These days it's all about "Big Data", but how well do technologies work when we actually throw realistic data at it? What knobs have to be turned to make the technologies scale? How far does a single machine take us?

In the project I want to evaluate [TimescaleDB], [Elasticsearch], [SQL Server] and [InfluxDB] on the 10 Minute Weather Data for Germany.

## Dataset ##

The [DWD Open Data] portal of the [Deutscher Wetterdienst (DWD)] gives access to the historical weather data in Germany. I decided to analyze the available historical Air Temperature data for Germany given in a 10 minute resolution ([FTP](https://opendata.dwd.de/climate_environment/CDC/observations_germany/climate/10_minutes/air_temperature/historical/)). If you want to recreate the example, you can find the list of files here: [GermanWeatherDataExample/Resources/files.md](https://github.com/bytefish/GermanWeatherDataExample/blob/master/GermanWeatherData/Resources/files.md).

The DWD dataset is given as CSV files and has a size of approximately 25.5 GB.

## Experiment Setup ##

TODO

## Status ##

### SQL Server 2017 ###

TODO

### TimescaleDB ###

TODO


### InfluxDB ###

InfluxDB 1.7.1 is unable to import the entire dataset without changes to the default configuration.  It consumes too much 
RAM under load and could not write the batches anymore. After reading through documentation I am quite confident, that the 
Retention Policy has to be adjusted, so that the shards do not stay in memory forever: 

* https://www.influxdata.com/blog/tldr-influxdb-tech-tips-march-16-2017/
* https://docs.influxdata.com/influxdb/v1.7/guides/hardware_sizing/

It's because the default configuration of InfluxDB is optimized for realtime data with a short [retention duration] and a 
short [shard duration]. This makes InfluxDB chewing up the entire RAM, just because too many shards are created and the 
cached data is never written to disk actually.

So I am now creating the database using ``DURATION`` set to infinite (``inf``), to keep measurements forever. The 
``SHARD DURATION`` is set to 4 weeks for limiting the number of shards being created during the import:

```
CREATE DATABASE "weather_data" WITH DURATION inf REPLICATION 1 SHARD DURATION 4w NAME "weather_data_policy"
```

In the ``influxdb.conf`` I am setting the ``cache-snapshot-write-cold-duration`` to 5 seconds for flushing the caches more 
agressively:

```
cache-snapshot-write-cold-duration = "5s"
```

[retention duration]: https://docs.influxdata.com/influxdb/v1.7/concepts/glossary/#duration
[shard duration]: https://docs.influxdata.com/influxdb/v1.7/concepts/glossary/#shard-duration

### Elasticsearch ###

The default configuration of Elasticsearch 6.5.1 is not optimized for bulk loading large amounts of data into the 
database. To improve the import for the initial load, the first I did was to disable indexing and replication by 
creating the index with 0 Replicas (since it is all running local anyway) and disabling the Index Refresh Interval, 
so the index isn't built on inserts.

All the performance hints are taken from the Elasticsearch documentation at:

* https://www.elastic.co/guide/en/elasticsearch/reference/master/tune-for-indexing-speed.html

In Elasticsearch 6.5.1 these settings have to be configured as an index template apparently, instead of editing the 
``config/elasticsearch.yml``. Anyways it can be easily be achieved with the NEST, the official .NET Connector for 
Elasticsearch:

```csharp
// We are creating the Index with special indexing options for initial load, 
// as suggested in the Elasticsearch documentation at [1].
//
// We disable the performance-heavy indexing during the initial load and also 
// disable any replicas of the data. This comes at a price of not being able 
// to query the data in realtime, but it will enhance the import speed.
//
// After the initial load I will revert to the standard settings for the Index
// and set the default values for Shards and Refresh Interval.
//
// [1]: https://www.elastic.co/guide/en/elasticsearch/reference/master/tune-for-indexing-speed.html
//
client.CreateIndex(settings => settings
    .NumberOfReplicas(0)
    .RefreshInterval(-1));
```

Additionally I made sure I am running a 64-bit JVM, so the heap size can scale to more than 2 GB for fair comparisms 
with systems like InfluxDB, that aggressively take ownership of the RAM. You can configure the Elasticsearch JVM settings 
in the ``config/jvm.options`` file.

I have set the initial and maximum size of the total heap space to ``6 GB``, so Elasticsearch should be able to allocate 
enough RAM to play with:

```
# Xms represents the initial size of total heap space
# Xmx represents the maximum size of total heap space
-Xms6g
-Xmx6g
```

And finally I wanted to make sure, that the Elasticsearch process isn't swapped out. According to the Elasticsearch 
documentation this can be configured in the ``config/elasticsearch.yml`` by adding:

```
bootstrap.memory_lock: true
```

More information on Heap Sizing and Swapping can be found at:

* https://www.elastic.co/guide/en/elasticsearch/guide/current/heap-sizing.html (Guide to Heap Sizing)
* https://www.elastic.co/blog/a-heap-of-trouble (Detailed article on the maximum JVM Heap Size)

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
["Open-Data-Gesetz" (§12a EGovG)]: https://www.bmi.bund.de/DE/themen/moderne-verwaltung/open-government/open-data/open-data-node.html
[Deutscher Wetterdienst (DWD)]: https://www.dwd.de

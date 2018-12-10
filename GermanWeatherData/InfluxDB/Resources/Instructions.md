# InfluxDB #

## Creating the Database ##

The default configuration of InfluxDB is optimized for realtime data with a short [retention duration] and a short [shard duration]. This 
made InfluxDB chewing up the entire RAM in initial, just because too many shards have been created and the cached data has never been 
written to disk actually.

So I am now creating the database using ``DURATION`` set to infinite (``inf``) and a ``SHARD DURATION`` of 4 weeks:

```
CREATE DATABASE "weather_data" WITH DURATION inf REPLICATION 1 SHARD DURATION 4w NAME "weather_data_policy"
```

And in ``influxdb.conf`` the ``cache-snapshot-write-cold-duration`` has been set 5 seconds for flushing the caches more agressively:

```
cache-snapshot-write-cold-duration = "5s"
```

[retention duration]: https://docs.influxdata.com/influxdb/v1.7/concepts/glossary/#duration
[shard duration]: https://docs.influxdata.com/influxdb/v1.7/concepts/glossary/#shard-duration

## Configuring InfluxDB Directories ##

InfluxDB writes to `` C:\Users\<username>\.influxdb\data\`` by default in Windows. The data directory can be configured 
in the ``influxdb.conf`` coming with the InfluxDB executables. I have moved the all directories onto a faster disk for 
the experiments:

```
[meta]
  # Where the metadata/raft database is stored
  dir = "G:/InfluxDB/meta"
  
[data]
  # The directory where the TSM storage engine stores TSM files.
  dir = "G:/InfluxDB/data"

  # The directory where the TSM storage engine stores WAL files.
  wal-dir = "G:/InfluxDB/wal"
```

The InfluxDB daemon can then be started with a custom configuration by running:

```
G:\InfluxDB>influxd -config influxdb.conf
```

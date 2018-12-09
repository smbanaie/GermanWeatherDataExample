# InfluxDB #

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

## Creating the Database ##

Before running the experiment, the database needs to be created from the InfluxDB shell:

```
CREATE DATABASE weather_data
```
--
-- DATABASE
--
IF DB_ID('$(dbname)') IS NULL
BEGIN
    CREATE DATABASE $(dbname)
END
GO

use $(dbname)
GO 

-- 
-- SCHEMAS
--
IF NOT EXISTS (SELECT name from sys.schemas WHERE name = 'sample')
BEGIN

	EXEC('CREATE SCHEMA sample')
    
END
GO

--
-- TABLES
--
IF  NOT EXISTS 
	(SELECT * FROM sys.objects 
	 WHERE object_id = OBJECT_ID(N'[sample].[Station]') AND type in (N'U'))
	 
BEGIN

	CREATE TABLE [sample].[Station](
        [Identifier] [NVARCHAR](5) NOT NULL,
        [Name] [NVARCHAR](255) NOT NULL,
        [StartDate] [DATETIME2]NOT NULL,
        [EndDate] [DATETIME2],
        [StationHeight] SMALLINT,
        [State] [NVARCHAR](255),
        [Latitude] [REAL],
        [Longitude] [REAL]
        CONSTRAINT [PK_Station] PRIMARY KEY CLUSTERED 
        (
            [Identifier] ASC
        ),
    ) ON [PRIMARY]

END
GO

IF  NOT EXISTS 
	(SELECT * FROM sys.objects 
	 WHERE object_id = OBJECT_ID(N'[sample].[LocalWeatherData]') AND type in (N'U'))
	 
BEGIN

    CREATE TABLE [sample].[LocalWeatherData](
        [StationIdentifier] [NVARCHAR](5) NOT NULL,
        [Timestamp] [DATETIME2](7) NOT NULL,
        [QualityCode] TINYINT,
        [StationPressure] [REAL],
        [AirTemperatureAt2m] [REAL],
        [AirTemperatureAt5cm] [REAL],
        [RelativeHumidity] [REAL],
        [DewPointTemperatureAt2m] [REAL]
        CONSTRAINT [PK_LocalWeatherData] PRIMARY KEY CLUSTERED 
        (
            [StationIdentifier] ASC,
            [Timestamp] ASC
        ),
    ) ON [PRIMARY]

END
GO

--
-- STORED PROCEDURES
--
-- The InsertOrUpdate methods use the MERGE Statement to upsert the data. We have primary keys for the matching statements,
-- but it may still take additional time for matching the data. It would be unfair to compare this with the InfluxDB, 
-- Elasticsearch and TimescaleDB ingestion, where this condition isn't tested.
--
-- But it's too painful for me to delete these beauties, and maybe I need to copy and paste them for future projects.
--
IF OBJECT_ID(N'[sample].[InsertStation]', N'P') IS NOT NULL
BEGIN
    DROP PROCEDURE [sample].[InsertStation]
END
GO

IF OBJECT_ID(N'[sample].[InsertOrUpdateStation]', N'P') IS NOT NULL
BEGIN
    DROP PROCEDURE [sample].[InsertOrUpdateStation]
END
GO

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = '[sample].[StationType]')
BEGIN
    DROP TYPE [sample].[StationType]
END

CREATE TYPE [sample].[StationType] AS TABLE (
    [Identifier] [NVARCHAR](5) NOT NULL,
    [Name] [NVARCHAR](255) NOT NULL,
    [StartDate] [DATETIME2] NOT NULL,
    [EndDate] [DATETIME2],
    [StationHeight] SMALLINT,
    [State] [NVARCHAR](255),
    [Latitude] [REAL],
    [Longitude] [REAL]
);

GO

CREATE PROCEDURE [sample].[InsertStation]
  @Entities [sample].[StationType] ReadOnly
AS
BEGIN
    
    SET NOCOUNT ON;
 
    INSERT (Identifier, Name, StartDate, EndDate, StationHeight, State, Latitude, Longitude) 
    SELECT Identifier, Name, StartDate, EndDate, StationHeight, State, Latitude, Longitude 
    FROM @Entities;

END
GO

CREATE PROCEDURE [sample].[InsertOrUpdateStation]
  @Entities [sample].[StationType] ReadOnly
AS
BEGIN
    
    SET NOCOUNT ON;
 
    MERGE [sample].[Station] AS TARGET USING @Entities AS SOURCE ON (TARGET.Identifier = SOURCE.Identifier) 
    WHEN MATCHED THEN
        UPDATE SET TARGET.Identifier = SOURCE.Identifier, TARGET.Name = SOURCE.Name, Target.StartDate = SOURCE.StartDate, Target.EndDate = SOURCE.EndDate, Target.StationHeight = SOURCE.StationHeight, Target.State = SOURCE.State, Target.Latitude = SOURCE.Latitude, TARGET.Longitude = SOURCE.Longitude
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Identifier, Name, StartDate, EndDate, StationHeight, State, Latitude, Longitude) 
        VALUES (SOURCE.Identifier, SOURCE.Name, SOURCE.StartDate, SOURCE.EndDate, SOURCE.StationHeight, SOURCE.State, SOURCE.Latitude, SOURCE.Longitude);

END
GO

IF OBJECT_ID(N'[sample].[InsertLocalWeatherData]', N'P') IS NOT NULL
BEGIN
    DROP PROCEDURE [sample].[InsertLocalWeatherData]
END
GO 

IF OBJECT_ID(N'[sample].[InsertOrUpdateLocalWeatherData]', N'P') IS NOT NULL
BEGIN
    DROP PROCEDURE [sample].[InsertOrUpdateLocalWeatherData]
END
GO 

IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name = '[sample].[LocalWeatherDataType]')
BEGIN
    DROP TYPE [sample].[LocalWeatherDataType]
END

CREATE TYPE [sample].[LocalWeatherDataType] AS TABLE (
    [StationIdentifier] [NVARCHAR](5) NOT NULL,
    [Timestamp] [DATETIME2](7) NOT NULL,
    [QualityCode] TINYINT NULL,
    [StationPressure] [REAL] NULL,
    [AirTemperatureAt2m] [REAL],
    [AirTemperatureAt5cm] [REAL],
    [RelativeHumidity] [REAL],
    [DewPointTemperatureAt2m] [REAL]
);

GO

CREATE PROCEDURE [sample].[InsertLocalWeatherData]
  @Entities [sample].[LocalWeatherDataType] ReadOnly
AS
BEGIN
    
    SET NOCOUNT ON;

    INSERT (StationIdentifier, Timestamp, QualityCode, StationPressure, AirTemperatureAt2m, AirTemperatureAt5cm, RelativeHumidity, DewPointTemperatureAt2m)
    SELECT StationIdentifier, Timestamp, QualityCode, StationPressure, AirTemperatureAt2m, AirTemperatureAt5cm, RelativeHumidity, DewPointTemperatureAt2m
    FROM @Entities;

END
GO


CREATE PROCEDURE [sample].[InsertOrUpdateLocalWeatherData]
  @Entities [sample].[LocalWeatherDataType] ReadOnly
AS
BEGIN
    
    SET NOCOUNT ON;

    MERGE [sample].[LocalWeatherData] AS TARGET USING @Entities AS SOURCE ON (TARGET.StationIdentifier = SOURCE.StationIdentifier and TARGET.Timestamp = SOURCE.Timestamp) 
    WHEN MATCHED THEN
        UPDATE SET TARGET.StationIdentifier = SOURCE.StationIdentifier, TARGET.Timestamp = SOURCE.Timestamp, TARGET.QualityCode = SOURCE.QualityCode, TARGET.StationPressure = SOURCE.StationPressure, TARGET.AirTemperatureAt2m = SOURCE.AirTemperatureAt2m, TARGET.AirTemperatureAt5cm = SOURCE.AirTemperatureAt5cm,  TARGET.RelativeHumidity = SOURCE.RelativeHumidity, TARGET.DewPointTemperatureAt2m = SOURCE.DewPointTemperatureAt2m
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (StationIdentifier, Timestamp, QualityCode, StationPressure, AirTemperatureAt2m, AirTemperatureAt5cm, RelativeHumidity, DewPointTemperatureAt2m)
        VALUES (SOURCE.StationIdentifier, SOURCE.Timestamp, SOURCE.QualityCode, SOURCE.StationPressure, SOURCE.AirTemperatureAt2m, SOURCE.AirTemperatureAt5cm, SOURCE.RelativeHumidity, SOURCE.DewPointTemperatureAt2m);

END
GO

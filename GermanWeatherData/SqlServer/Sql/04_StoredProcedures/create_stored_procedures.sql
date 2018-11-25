USE $(dbname)

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

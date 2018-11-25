USE $(dbname) 
GO

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

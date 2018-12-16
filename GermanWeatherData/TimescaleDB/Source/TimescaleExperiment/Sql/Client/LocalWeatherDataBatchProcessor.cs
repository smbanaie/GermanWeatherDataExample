// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using NLog;
using Npgsql;
using NpgsqlTypes;
using PostgreSQLCopyHelper;
using TimescaleExperiment.Sql.Model;

namespace TimescaleExperiment.Sql.Client
{
    public class LocalWeatherDataBatchProcessor : IBatchProcessor<LocalWeatherData>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private class LocalWeatherCopyHelper : PostgreSQLCopyHelper<LocalWeatherData>
        {
            public LocalWeatherCopyHelper()
                : base("sample", "weather_data")
            {
                Map("station_identifier", x => x.StationIdentifier, NpgsqlDbType.Varchar);
                Map("timestamp", x => x.TimeStamp, NpgsqlDbType.TimestampTz);
                Map("quality_code", x => x.QualityCode, NpgsqlDbType.Smallint);
                MapNullable("station_pressure", x => x.StationPressure, NpgsqlDbType.Real);
                MapNullable("air_temperature_at_2m", x => x.AirTemperatureAt2m, NpgsqlDbType.Real);
                MapNullable("air_temperature_at_5cm", x => x.AirTemperatureAt5cm, NpgsqlDbType.Real);
                MapNullable("relative_humidity", x => x.RelativeHumidity, NpgsqlDbType.Real);
                MapNullable("dew_point_temperature_at_2m", x => x.RelativeHumidity, NpgsqlDbType.Real);
            }
        }

        private readonly string connectionString;

        private readonly IPostgreSQLCopyHelper<LocalWeatherData> processor;

        public LocalWeatherDataBatchProcessor(string connectionString)
            : this(connectionString, new LocalWeatherCopyHelper())
        {
        }

        public LocalWeatherDataBatchProcessor(string connectionString, IPostgreSQLCopyHelper<LocalWeatherData> processor)
        {
            this.processor = processor;
            this.connectionString = connectionString;
        }

        public void Write(IEnumerable<LocalWeatherData> measurements)
        {
            try
            {
                InternalWrite(measurements);
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error(e, "An Error occured while writing measurements");
                }
            }
        }

        private void InternalWrite(IEnumerable<LocalWeatherData> measurements)
        {
            if(measurements == null)
            {
                return;
            }
            
            using (var connection = new NpgsqlConnection(connectionString))
            {
                // Open the Connection:
                connection.Open();

                processor.SaveAll(connection, measurements);
            }
        }
    }
}
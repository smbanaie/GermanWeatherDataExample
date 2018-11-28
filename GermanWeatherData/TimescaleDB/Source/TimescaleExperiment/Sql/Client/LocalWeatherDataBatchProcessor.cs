// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;
using PostgreSQLCopyHelper;
using TimescaleExperiment.Sql.Model;

namespace TimescaleExperiment.Sql.Client
{
    public class LocalWeatherDataBatchProcessor : IBatchProcessor<LocalWeatherData>
    {
        private class LocalWeatherCopyHelper : PostgreSQLCopyHelper<LocalWeatherData>
        {
            public LocalWeatherCopyHelper()
                : base("sample", "weather_data")
            {
                Map("station_identifier", x => x.StationIdentifier, NpgsqlDbType.Varchar);
                Map("timestamp", x => x.TimeStamp, NpgsqlDbType.TimestampTz);
                Map("quality_code", x => x.QualityCode, NpgsqlDbType.Smallint);
                MapNullable("station_pressure", x => x.StationPressure, NpgsqlDbType.Double);
                Map("air_temperature_at_2m", x => x.AirTemperatureAt2m, NpgsqlDbType.Double);
                Map("air_temperature_at_5cm", x => x.AirTemperatureAt5cm, NpgsqlDbType.Double);
                Map("relative_humidity", x => x.RelativeHumidity, NpgsqlDbType.Double);
                Map("dew_point_temperature_at_2m", x => x.RelativeHumidity, NpgsqlDbType.Double);
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

        public void Write(IList<LocalWeatherData> measurements)
        {
            if(measurements == null)
            {
                return;
            }

            if (measurements.Count == 0)
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
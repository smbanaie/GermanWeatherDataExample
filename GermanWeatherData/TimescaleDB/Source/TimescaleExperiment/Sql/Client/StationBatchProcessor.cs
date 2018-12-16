// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;
using PostgreSQLCopyHelper;
using TimescaleExperiment.Sql.Model;

namespace TimescaleExperiment.Sql.Client
{
    public class StationBatchProcessor : IBatchProcessor<Station>
    {
        private class StationCopyHelper : PostgreSQLCopyHelper<Station>
        {
            public StationCopyHelper() 
                : base("sample", "station")
            {
                Map("identifier", x => x.Identifier, NpgsqlDbType.Varchar);
                Map("name", x => x.Name, NpgsqlDbType.Varchar);
                Map("start_date", x => x.StartDate, NpgsqlDbType.TimestampTz);
                MapNullable("end_date", x => x.EndDate, NpgsqlDbType.TimestampTz);
                Map("station_height", x => x.StationHeight, NpgsqlDbType.Smallint);
                Map("state", x => x.State, NpgsqlDbType.Varchar);
                Map("latitude", x => x.Latitude, NpgsqlDbType.Real);
                Map("longitude", x => x.Longitude, NpgsqlDbType.Real);
            }
        }

        private readonly string connectionString;

        private readonly IPostgreSQLCopyHelper<Station> processor;

        public StationBatchProcessor(string connectionString)
            : this(connectionString, new StationCopyHelper())
        {
        }

        public StationBatchProcessor(string connectionString, IPostgreSQLCopyHelper<Station> processor)
        {
            this.connectionString = connectionString;
            this.processor = processor;
        }

        public void Write(IEnumerable<Station> stations)
        {
            if(stations == null)
            {
                return;
            }

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                processor.SaveAll(connection, stations);
            }
        }
    }
}
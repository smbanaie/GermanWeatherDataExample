// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;
using TimescaleExperiment.Sql.Model;

namespace TimescaleExperiment.Sql.Client
{
    public class StationBatchProcessor
    {
        private readonly string connectionString;

        public StationBatchProcessor(string connectionString)
        {
            this.connectionString = connectionString;
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

                NpgsqlCommand command = new NpgsqlCommand(connection: connection, 
                    cmdText: @"INSERT INTO sample.station(identifier, name, start_date, end_date, station_height, state, latitude, longitude)
                               VALUES(@identifier, @name, @start_date, @end_date, @station_height, @state, @latitude, @longitude)
                               ON CONFLICT (identifier) 
                               DO 
                               UPDATE SET identifier = @identifier, 
                                        name = @name, 
                                        start_date = @start_date, 
                                        end_date = @end_date, 
                                        station_height = @station_height, 
                                        state = @state, 
                                        latitude = @latitude, 
                                        longitude = @longitude");

                command.Parameters.Add("identifier", NpgsqlDbType.Varchar);
                command.Parameters.Add("name", NpgsqlDbType.Varchar);
                command.Parameters.Add("start_date", NpgsqlDbType.Timestamp);
                command.Parameters.Add("end_date", NpgsqlDbType.Timestamp);
                command.Parameters.Add("station_height", NpgsqlDbType.Smallint);
                command.Parameters.Add("state", NpgsqlDbType.Varchar);
                command.Parameters.Add("latitude", NpgsqlDbType.Real);
                command.Parameters.Add("longitude", NpgsqlDbType.Real);

                command.Prepare();

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var station in stations)
                    {
                        FillParameters(station, command);
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        private static void FillParameters(Station station, NpgsqlCommand command)
        {
            command.Parameters["identifier"].Value = station.Identifier;
            command.Parameters["name"].Value = station.Name;
            command.Parameters["start_date"].Value = station.StartDate;
            command.Parameters["end_date"].Value = station.EndDate;
            command.Parameters["station_height"].Value = station.StationHeight;
            command.Parameters["state"].Value = station.State;
            command.Parameters["latitude"].Value = station.Latitude;
            command.Parameters["longitude"].Value = station.Longitude;
        }
    }
}
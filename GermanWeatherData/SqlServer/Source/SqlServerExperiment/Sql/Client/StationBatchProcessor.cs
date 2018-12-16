// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using SqlServerExperiment.Sql.Extensions;
using SqlServerExperiment.Sql.Model;

namespace SqlServerExperiment.Sql.Client
{
    public class StationBatchProcessor : IBatchProcessor<Station>
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

            using (var conn = new SqlConnection(connectionString))
            {
                // Open the Connection:
                conn.Open();

                // Execute the Batch Write Command:
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Build the Stored Procedure Command:
                    cmd.CommandText = "[sample].[InsertOrUpdateStation]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Create the TVP:
                    SqlParameter parameter = new SqlParameter();

                    parameter.ParameterName = "@Entities";
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "[sample].[StationType]";
                    parameter.Value = ToSqlDataRecords(stations);

                    // Add it as a Parameter:
                    cmd.Parameters.Add(parameter);

                    // And execute it:
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private IEnumerable<SqlDataRecord> ToSqlDataRecords(IEnumerable<Station> stations)
        {
            // Construct the Data Record with the MetaData:
            SqlDataRecord sdr = new SqlDataRecord(
                new SqlMetaData("Identifier", SqlDbType.NVarChar, 5),
                new SqlMetaData("Name", SqlDbType.NVarChar, 255),
                new SqlMetaData("StartDate", SqlDbType.DateTime2),
                new SqlMetaData("EndDate", SqlDbType.DateTime2),
                new SqlMetaData("StationHeight", SqlDbType.SmallInt),
                new SqlMetaData("State", SqlDbType.NVarChar, 255),
                new SqlMetaData("Latitude", SqlDbType.Real),
                new SqlMetaData("Longitude", SqlDbType.Real)
            );

            // Now yield the Measurements in the Data Record:
            foreach (var station in stations)
            {
                sdr.SetString(0, station.Identifier);
                sdr.SetString(1, station.Name);
                sdr.SetDateTime(2, station.StartDate);
                sdr.SetNullableDateTime(3, station.EndDate);
                sdr.SetInt16(4, station.StationHeight);
                sdr.SetString(5, station.State);
                sdr.SetFloat(6, station.Latitude);
                sdr.SetFloat(7, station.Longitude);

                yield return sdr;
            }
        }
    }
}
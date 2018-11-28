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
    public class LocalWeatherDataBatchProcessor : IBatchProcessor<LocalWeatherData>
    {
        private readonly string connectionString;

        public LocalWeatherDataBatchProcessor(string connectionString)
        {
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
            
            using (var conn = new SqlConnection(connectionString))
            {
                // Open the Connection:
                conn.Open();

                // Execute the Batch Write Command:
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Build the Stored Procedure Command:
                    cmd.CommandText = "[sample].[InsertOrUpdateLocalWeatherData]";
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Create the TVP:
                    SqlParameter parameter = new SqlParameter();

                    parameter.ParameterName = "@Entities";
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "[sample].[LocalWeatherDataType]";
                    parameter.Value = ToSqlDataRecords(measurements);

                    // Add it as a Parameter:
                    cmd.Parameters.Add(parameter);

                    // And execute it:
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private IEnumerable<SqlDataRecord> ToSqlDataRecords(IEnumerable<LocalWeatherData> localWeatherDataList)
        {
            // Construct the Data Record with the MetaData:
            SqlDataRecord sdr = new SqlDataRecord(
                new SqlMetaData("StationIdentifier", SqlDbType.NVarChar, 5),
                new SqlMetaData("Timestamp", SqlDbType.DateTime2),
                new SqlMetaData("QualityCode", SqlDbType.TinyInt),
                new SqlMetaData("StationPressure", SqlDbType.Real),
                new SqlMetaData("AirTemperatureAt2m", SqlDbType.Real),
                new SqlMetaData("AirTemperatureAt5cm", SqlDbType.Real),
                new SqlMetaData("RelativeHumidity", SqlDbType.Real),
                new SqlMetaData("DewPointTemperatureAt2m", SqlDbType.Real)
            );

            // Now yield the Measurements in the Data Record:
            foreach (var localWeatherData in localWeatherDataList)
            {
                sdr.SetString(0, localWeatherData.StationIdentifier);
                sdr.SetDateTime(1, localWeatherData.TimeStamp);
                sdr.SetByte(2, localWeatherData.QualityCode);
                sdr.SetNullableFloat(3, localWeatherData.StationPressure);
                sdr.SetNullableFloat(4, localWeatherData.AirTemperatureAt2m);
                sdr.SetNullableFloat(5, localWeatherData.AirTemperatureAt5cm);
                sdr.SetNullableFloat(6, localWeatherData.RelativeHumidity);
                sdr.SetNullableFloat(7, localWeatherData.DewPointTemperatureAt2m);

                yield return sdr;
            }
        }
    }
}
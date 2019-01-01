// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using NLog;
using Npgsql;
using NpgsqlTypes;
using PostgreSQLCopyHelper;
using TimescaleExperiment.Sql.Mapping;
using TimescaleExperiment.Sql.Model;

namespace TimescaleExperiment.Sql.Client
{
    public class LocalWeatherDataBatchProcessor
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();


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
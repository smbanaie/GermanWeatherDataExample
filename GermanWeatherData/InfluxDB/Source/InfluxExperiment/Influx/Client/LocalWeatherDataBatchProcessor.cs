// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;

namespace InfluxExperiment.Influx.Client
{
    public class LocalWeatherDataBatchProcessor
    {
        private readonly string database;
        private readonly string connectionString;

        public LocalWeatherDataBatchProcessor(string connectionString, string database)
        {
            this.database = database;
            this.connectionString = connectionString;
        }

        public LineProtocolWriteResult Write(LineProtocolPayload source)
        {
            if(source == null)
            {
                return new LineProtocolWriteResult();
            }

            var client = new LineProtocolClient(new Uri(connectionString), database);

            return client.WriteAsync(source).GetAwaiter().GetResult();
        }
    }
}
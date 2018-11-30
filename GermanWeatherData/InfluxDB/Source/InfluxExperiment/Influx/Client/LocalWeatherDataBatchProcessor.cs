// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
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

        public Task<LineProtocolWriteResult> WriteAsync(LineProtocolPayload source, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(source == null)
            {
                return Task.FromResult(new LineProtocolWriteResult(true, string.Empty));
            }

            var client = new LineProtocolClient(new Uri(connectionString), database);

            return client.WriteAsync(source, cancellationToken);
        }
    }
}
// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver.V1;
using Neo4jExperiment.Core.Neo4j.Serializer;
using Neo4jExperiment.Core.Neo4j.Settings;
using Neo4jExperiment.Graph.Model;

namespace Neo4jExperiment.Graph.Client
{
    public class Neo4JClient : IDisposable
    {
        private readonly IDriver driver;
        
        public Neo4JClient(IConnectionSettings settings)
        {
            Config config = Config.DefaultConfig;

            // Increase MaxWriteBuffer to 20MB:
            config.MaxWriteBufferSize = 20971520;

            this.driver = GraphDatabase.Driver(settings.Uri, settings.AuthToken, config);
        }

        public async Task CreateIndicesAsync()
        {
            string[] queries = {
                // Indexes:
                "CREATE INDEX ON :Station(identifier)",
                // Constraints:
                "CREATE CONSTRAINT ON (s:Station) ASSERT s.identifier IS UNIQUE"
            };

            using (var session = driver.Session())
            {
                foreach(var query in queries)
                {
                    await session.RunAsync(query);
                }
            }
        }

        public async Task CreateStationsAsync(IList<Station> stations)
        {
            string cypher = new StringBuilder()
                .AppendLine("UNWIND {stations} AS row")
                .AppendLine("MERGE (state:State {name: row.State")
                .AppendLine("MERGE (station:Station {identifier: row.identifier}),")
                .AppendLine("   (station)-[:IN_STATE]->(state)")
                .AppendLine("SET state = row")
                .ToString();

            using (var session = driver.Session())
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "stations", ParameterSerializer.ToDictionary(stations) } });
            }
        }

        public async Task CreateLocalWeatherDataAsync(IList<LocalWeatherData> localWeatherDatas)
        {
            string cypher = new StringBuilder()
                .AppendLine("UNWIND {localWeatherDatas} AS row")
                .AppendLine("MATCH (station:Station {identifier: row.station_identifier})")
                .AppendLine("CREATE (localWeatherData:LocalWeatherData {code: carrier.code})")
                .AppendLine("   (station)-[:MEASURED]->(localWeatherData)")
                .AppendLine("SET localWeatherData = row")
                .ToString();

            using (var session = driver.Session())
            {
                var result = await session.WriteTransactionAsync(tx => tx.RunAsync(cypher, new Dictionary<string, object>() {{"localWeatherDatas", ParameterSerializer.ToDictionary(localWeatherDatas) }}));

                // Get the Summary for Diagnostics:
                var summary = await result.ConsumeAsync();

                Console.WriteLine($"[{DateTime.Now}] [Carriers] #NodesCreated: {summary.Counters.NodesCreated}, #RelationshipsCreated: {summary.Counters.RelationshipsCreated}");
            }
        }

        public void Dispose()
        {
            driver?.Dispose();
        }
    }
}
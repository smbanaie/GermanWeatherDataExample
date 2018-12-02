// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Parser;
using Experiments.Common.Extensions;
using Neo4jExperiment.Converters;
using Neo4jExperiment.Core.Neo4j.Settings;
using Neo4jExperiment.Graph.Client;

namespace Neo4jExperiment.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProcessLocalWeatherData().GetAwaiter().GetResult();
        }

        private static async Task ProcessLocalWeatherData(CancellationToken cancellationToken = default(CancellationToken))
        {
            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687/db/flights", "neo4j", "test_pwd");

            using (var client = new Neo4JClient(settings))
            {
                // Create Indices for faster Lookups:
                await client.CreateIndicesAsync();
                // Insert the Base Data (Airports, Carriers, ...):
                await InsertStationsAsync(client);
                // Insert the Flight Data:
                await InsertLocalWeatherDataAsync(client);
            }
        }

        private static async Task InsertStationsAsync(Neo4JClient client)
        {
            // Read the Stations:
            string csvStationDataFile = @"D:\datasets\CDC\zehn_min_tu_Beschreibung_Stationen.txt";

            // Access to the List of Parsers:
            var batches = Parsers
                // Use the LocalWeatherData Parser:
                .StationParser
                // Read the File:
                .ReadFromFile(csvStationDataFile, Encoding.UTF8, 1)
                // Get the Valid Results:
                .Where(x => x.IsValid)
                // And get the populated Entities:
                .Select(x => x.Result)
                // Group by WBAN, Date and Time to avoid duplicates for this batch:
                .GroupBy(x => new { x.Identifier })
                // If there are duplicates then make a guess and select the first one:
                .Select(x => x.First())
                // Let's stay safe! Stop parallelism here:
                .AsEnumerable()
                // Convert to Neo4j:
                .Select(x => LocalWeatherDataConverter.Convert(x))
                // Evaluate:
                .Batch(30000)
                // As List:
                .Select(x => x.ToList());

            foreach (var batch in batches)
            {
                // Finally write them with the Batch Writer:
                await client.CreateStationsAsync(batch);
            }
        }

        private static async Task InsertLocalWeatherDataAsync(Neo4JClient client)
        {
            // Import 10 Minute CDC Weather Data:
            var csvWeatherDataFiles = GetFilesFromFolder(@"D:\datasets\CDC");

            foreach (var csvWeatherDataFile in csvWeatherDataFiles)
            {
                await ProcessLocalWeatherData(client, csvWeatherDataFile);
            }
        }

        private static async Task ProcessLocalWeatherData(Neo4JClient client, string csvFilePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine($"Processing File: {csvFilePath}");

            // Access to the List of Parsers:
            var batches = Parsers
                // Use the LocalWeatherData Parser:
                .LocalWeatherDataParser
                // Read the File:
                .ReadFromFile(csvFilePath, Encoding.UTF8, 1)
                // Get the Valid Results:
                .Where(x => x.IsValid)
                // And get the populated Entities:
                .Select(x => x.Result)
                // Group by WBAN, Date and Time to avoid duplicates for this batch:
                .GroupBy(x => new { x.StationIdentifier, x.TimeStamp })
                // If there are duplicates then make a guess and select the first one:
                .Select(x => x.First())
                // Let's stay safe! Stop parallelism here:
                .AsEnumerable()
                // Convert to Neo4j:
                .Select(x => LocalWeatherDataConverter.Convert(x))
                // Evaluate:
                .Batch(30000)
                // As List:
                .Select(x => x.ToList());

            foreach (var batch in batches)
            {
                // Finally write them with the Batch Writer:
                await client.CreateLocalWeatherDataAsync(batch);
            }
        }

        private static string[] GetFilesFromFolder(string directory)
        {
            return Directory.GetFiles(directory, "produkt_zehn_min_*.txt").ToArray();
        }
    }
}

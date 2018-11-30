// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Parser;
using Experiments.Common.Extensions;
using InfluxExperiment.Converters;
using InfluxExperiment.Influx.Client;

namespace InfluxExperiment.ConsoleApp
{
    public class Program
    {
        // The ConnectionString used to decide which database to connect to:
        private static readonly string ConnectionString = @"http://localhost:8086";

        private static readonly string Database = @"weather_data";

        public static void Main(string[] args)
        {
            ProcessLocalWeatherData().GetAwaiter().GetResult();
        }

        private static async Task ProcessLocalWeatherData()
        {
            // Import 10 Minute CDC Weather Data:
            var csvWeatherDataFiles = GetFilesFromFolder(@"D:\datasets\CDC");

            foreach (var csvWeatherDataFile in csvWeatherDataFiles)
            {
                await ProcessLocalWeatherData(csvWeatherDataFile);
            }
        }

        private static async Task ProcessLocalWeatherData(string csvFilePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine($"Processing File: {csvFilePath}");

            // Construct the Batch Processor:
            var processor = new LocalWeatherDataBatchProcessor(ConnectionString, Database);

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
                .GroupBy(x => new {x.StationIdentifier, x.TimeStamp})
                // If there are duplicates then make a guess and select the first one:
                .Select(x => x.First())
                // Evaluate:
                .Batch(30000);

            foreach (var batch in batches)
            {
                var payload = LocalWeatherDataConverter.Convert(batch);

                // Finally write them with the Batch Writer:
                var result = await processor.WriteAsync(payload, cancellationToken);

                if (!result.Success)
                {
                    // Maybe throw an exception? Maybe use a Logger here?
                    Console.WriteLine($"[ERROR] {result.ErrorMessage}");
                }
            }
        }

        private static string[] GetFilesFromFolder(string directory)
        {
            return Directory.GetFiles(directory, "produkt_zehn_min_*.txt").ToArray();
        }
    }
}

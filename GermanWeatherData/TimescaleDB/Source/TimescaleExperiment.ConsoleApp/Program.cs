// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Parser;
using Experiments.Common.Extensions;
using TimescaleExperiment.Sql.Client;

namespace TimescaleExperiment.ConsoleApp
{
    public class Program
    {
        // The ConnectionString used to decide which database to connect to:
        private static readonly string ConnectionString = @"Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;";

        public static void Main(string[] args)
        {
            // Import all Stations:
            var csvStationDataFiles = new[]
            {
                @"D:\datasets\CDC\zehn_min_tu_Beschreibung_Stationen.txt"
            };

            foreach (var csvStationDataFile in csvStationDataFiles)
            {
                ProcessStationData(csvStationDataFile);
            }

            // Import 10 Minute CDC Weather Data:
            var csvWeatherDataFiles = GetFilesFromFolder(@"D:\datasets\CDC");

            foreach (var csvWeatherDataFile in csvWeatherDataFiles)
            {
                ProcessLocalWeatherData(csvWeatherDataFile);
            }

            Console.WriteLine("[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]  Import Finished");
            Console.ReadLine();
        }

        private static void ProcessStationData(string csvFilePath)
        {
            Console.WriteLine($"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}] Processing File: {csvFilePath}");

            // Construct the Batch Processor:
            var processor = new StationBatchProcessor(ConnectionString);

            // Access to the List of Parsers:
            var batches = Parsers
                // Use the Station Parser:
                .StationParser
                // Read the File:
                .ReadFromFile(csvFilePath, Encoding.UTF8, 2)
                // Get the Valid Results:
                .Where(x => x.IsValid)
                // And get the populated Entities:
                .Select(x => x.Result)
                // If there is no WBAN, do not process the record:
                .Where(x => !string.IsNullOrWhiteSpace(x.Identifier))
                // Stop Parallelism:
                .AsEnumerable()
                // Batch in 10000 Entities / or wait 1 Second:
                .Batch(70000)
                // And subscribe to the Batch
                .Select(records =>
                {
                    var batch = records
                        // Group By WBAN to avoid duplicate Stations in the Batch:
                        .GroupBy(x => x.Identifier)
                        // Only Select the First Station:
                        .Select(x => x.First())
                        // Convert into the Sql Data Model:
                        .Select(x => Converters.Converters.Convert(x))
                        // Evaluate:
                        .ToList();

                    return batch;
                });


            foreach (var batch in batches)
            {
                // Finally write them with the Batch Writer:
                processor.Write(batch);
            }
        }


        private static void ProcessLocalWeatherData(string csvFilePath)
        {
            Console.WriteLine($"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}] Processing File: {csvFilePath}");

            // Construct the Batch Processor:
            var processor = new LocalWeatherDataBatchProcessor(ConnectionString);

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
                // As an Observable:
                .AsEnumerable()
                // Batch in 80000 Entities / or wait 1 Second:
                .Batch(80000)
                // And subscribe to the Batch
                .Select(records =>
                {
                    var batch = records
                        // Group by WBAN, Date and Time to avoid duplicates for this batch:
                        .GroupBy(x => new { x.StationIdentifier, x.TimeStamp })
                        // If there are duplicates then make a guess and select the first one:
                        .Select(x => x.First())
                        // Convert into the Sql Data Model:
                        .Select(x => Converters.Converters.Convert(x))
                        // Evaluate:
                        .ToList();

                    return batch;
                });


            foreach (var batch in batches)
            {
                // Finally write them with the Batch Writer:
                processor.Write(batch);
            }
        }

        private static string[] GetFilesFromFolder(string directory)
        {
            return Directory.GetFiles(directory, "produkt_zehn_min_*.txt").ToArray();
        }
    }
}

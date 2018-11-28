// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Parser;
using TimescaleExperiment.Sql.Client;

namespace TimescaleExperiment.ConsoleApp
{
    public class Program
    {
        // The ConnectionString used to decide which database to connect to:
        private static readonly string ConnectionString = @"Data Source=.\MSSQLSERVER2017;Integrated Security=true;Initial Catalog=GermanWeatherDatabase;";

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
        }

        private static void ProcessStationData(string csvFilePath)
        {
            Console.WriteLine($"Processing File: {csvFilePath}");

            // Construct the Batch Processor:
            var processor = new StationBatchProcessor(ConnectionString);

            // Access to the List of Parsers:
            Parsers
                // Use the Station Parser:
                .StationParser
                // Read the File:
                .ReadFromFile(csvFilePath, Encoding.UTF8, 2)
                // As an Observable:
                .ToObservable()
                // Batch in 10000 Entities / or wait 1 Second:
                .Buffer(TimeSpan.FromSeconds(1), 10000)
                // And subscribe to the Batch
                .Subscribe(records =>
                {
                    var validRecords = records
                        // Get the Valid Results:
                        .Where(x => x.IsValid)
                        // And get the populated Entities:
                        .Select(x => x.Result)
                        // If there is no WBAN, do not process the record:
                        .Where(x => !string.IsNullOrWhiteSpace(x.Identifier))
                        // Group By WBAN to avoid duplicate Stations in the Batch:
                        .GroupBy(x => x.Identifier)
                        // Only Select the First Station:
                        .Select(x => x.First())
                        // Convert into the Sql Data Model:
                        .Select(x => Converters.Converters.Convert(x))
                        // Evaluate:
                        .ToList();

                    // Finally write them with the Batch Writer:
                    processor.Write(validRecords);
                });
        }


        private static void ProcessLocalWeatherData(string csvFilePath)
        {
            Console.WriteLine($"Processing File: {csvFilePath}");

            // Construct the Batch Processor:
            var processor = new LocalWeatherDataBatchProcessor(ConnectionString);

            // Access to the List of Parsers:
            Parsers
                // Use the LocalWeatherData Parser:
                .LocalWeatherDataParser
                // Read the File:
                .ReadFromFile(csvFilePath, Encoding.UTF8, 1)
                // As an Observable:
                .ToObservable()
                // Batch in 80000 Entities / or wait 1 Second:
                .Buffer(TimeSpan.FromSeconds(1), 80000)
                // And subscribe to the Batch
                .Subscribe(records =>
                {
                    var validRecords = records
                        // Get the Valid Results:
                        .Where(x => x.IsValid)
                        // And get the populated Entities:
                        .Select(x => x.Result)
                        // Group by WBAN, Date and Time to avoid duplicates for this batch:
                        .GroupBy(x => new {x.StationIdentifier, x.TimeStamp})
                        // If there are duplicates then make a guess and select the first one:
                        .Select(x => x.First())
                        // Convert into the Sql Data Model:
                        .Select(x => Converters.Converters.Convert(x))
                        // Evaluate:
                        .ToList();

                    // Finally write them with the Batch Writer:
                    processor.Write(validRecords);
                });
        }

        private static string[] GetFilesFromFolder(string directory)
        {
            
            return Directory.GetFiles(directory, "produkt_zehn_min_*.txt").ToArray();
        }
    }
}

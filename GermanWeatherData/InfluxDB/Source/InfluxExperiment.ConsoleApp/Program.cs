// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Parser;
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
            // Import all Stations:
            var csvStationDataFiles = new[]
            {
                @"D:\datasets\CDC\zehn_min_tu_Beschreibung_Stationen.txt"
            };
            
            // Import 10 Minute CDC Weather Data:
            var csvWeatherDataFiles = GetFilesFromFolder(@"D:\datasets\CDC");

            foreach (var csvWeatherDataFile in csvWeatherDataFiles)
            {
                ProcessLocalWeatherData(csvWeatherDataFile);
            }
        }


        private static void ProcessLocalWeatherData(string csvFilePath)
        {
            Console.WriteLine($"Processing File: {csvFilePath}");

            // Construct the Batch Processor:
            var processor = new LocalWeatherDataBatchProcessor(ConnectionString, Database);

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
                        // Evaluate:
                        .ToList();

                    var payload = Converters.LocalWeatherDataConverter.Convert(validRecords);

                    // Finally write them with the Batch Writer:
                    processor.Write(payload);
                });
        }

        private static string[] GetFilesFromFolder(string directory)
        {
            
            return Directory.GetFiles(directory, "produkt_zehn_min_*.txt").ToArray();
        }
    }
}

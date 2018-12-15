// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using ElasticExperiment.Converters;
using ElasticExperiment.Elastic.Client;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Parser;
using Experiments.Common.Extensions;

namespace ElasticExperiment.ConsoleApp
{
    public class Program
    {
        // The ConnectionString used to decide which database to connect to:
        private static readonly Uri ConnectionString = new Uri("http://localhost:9200");

        public static void Main(string[] args)
        {
            // Read the Stations:
            string csvStationDataFile = @"D:\datasets\CDC\zehn_min_tu_Beschreibung_Stationen.txt";

            ProcessStations(csvStationDataFile);
            
            // Import 10 Minute CDC Weather Data:
            var csvWeatherDataFiles = GetFilesFromFolder(@"D:\datasets\CDC");

            foreach (var csvWeatherDataFile in csvWeatherDataFiles)
            {
                ProcessLocalWeatherData(csvWeatherDataFile);
            }
        }

        private static void ProcessStations(string csvStationDataFile)
        {
            // Construct the Batch Processor:
            var client = new ElasticSearchClient<Elastic.Model.Station>(ConnectionString, "stations");

            // Make Sure the weather_data Index we insert to exists:
            client.CreateIndex();

            var batches = Parsers
                .StationParser
                // Skip first two rows:
                .ReadFromFile(csvStationDataFile, Encoding.UTF8, 2)
                // Get the Valid Results:
                .Where(x => x.IsValid)
                // And get the populated Entities:
                .Select(x => x.Result)
                // Convert to the Elastic Representation:
                .Select(x => LocalWeatherDataConverter.Convert(x))
                // Batch:
                .Batch(20000);

            foreach (var batch in batches)
            {
                client.BulkInsert(batch);
            }
                   
        }

        private static void ProcessLocalWeatherData(string csvFilePath)
        {
            // Construct the Batch Processor:
            var client = new ElasticSearchClient<Elastic.Model.LocalWeatherData>(ConnectionString, "weather_data");

            // Make Sure the weather_data Index we insert to exists:
            client.CreateIndex();

            Console.WriteLine($"Processing File: {csvFilePath}");
            
            // Access to the List of Parsers:
            var batches = Parsers
                // Use the LocalWeatherData Parser:
                .LocalWeatherDataParser
                // Read the File, Skip first row:
                .ReadFromFile(csvFilePath, Encoding.UTF8, 1)
                // Get the Valid Results:
                .Where(x => x.IsValid)
                // And get the populated Entities:
                .Select(x => x.Result)
                // Convert to ElasticSearch Entity:
                .Select(x => LocalWeatherDataConverter.Convert(x))
                // Batch Entities:
                .Batch(80000);


            foreach (var batch in batches)
            {
                client.BulkInsert(batch);
            }
        }

        private static string[] GetFilesFromFolder(string directory)
        {
            return Directory.GetFiles(directory, "produkt_zehn_min_*.txt").ToArray();
        }
    }
}

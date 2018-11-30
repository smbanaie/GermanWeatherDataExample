﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using ElasticExperiment.Converters;
using ElasticExperiment.Elastic.Client;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Model;
using Experiments.Common.Csv.Parser;

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

            Parsers
                .StationParser
                .ReadFromFile(csvStationDataFile, Encoding.UTF8, 2)
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
                        .GroupBy(x => new { x.Identifier })
                        // If there are duplicates then make a guess and select the first one:
                        .Select(x => x.First())
                        // Convert to the Elastic Representation:
                        .Select(x => LocalWeatherDataConverter.Convert(x))
                        // Evaluate:
                        .ToList();

                    client.BulkInsert(validRecords);
                });
        }

        private static void ProcessLocalWeatherData(string csvFilePath)
        {
            // Construct the Batch Processor:
            var client = new ElasticSearchClient<Elastic.Model.LocalWeatherData>(ConnectionString, "weather_data");

            // Make Sure the weather_data Index we insert to exists:
            client.CreateIndex();

            Console.WriteLine($"Processing File: {csvFilePath}");
            
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
                        // Convert to the Elastic Representation:
                        .Select(x => LocalWeatherDataConverter.Convert(x))
                        // Evaluate:
                        .ToList();

                    client.BulkInsert(validRecords);
                });
        }

        private static string[] GetFilesFromFolder(string directory)
        {
            return Directory.GetFiles(directory, "produkt_zehn_min_*.txt").ToArray();
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Parser;
using Experiments.Common.Extensions;
using NLog;
using NLog.Config;
using TimescaleExperiment.Converters;
using TimescaleExperiment.Sql.Client;
using TimescaleExperiment.Sql.Model;

namespace TimescaleExperiment.ConsoleApp
{
    public class Program
    {
        // The ConnectionString used to decide which database to connect to. I decided to turn off the Npgsql built-in Connection Pooling, 
        // because of undeterministic NullReferenceException, see https://github.com/npgsql/npgsql/issues/2257:
        private static readonly string ConnectionString = @"Host=192.168.178.25;Port=5432;Database=sampledb;Pooling=false;User Id=philipp;Password=test_pwd;";

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");

            if (log.IsInfoEnabled)
            {
                log.Info("Import started");
            }

            // Import the Stations:
            var csvStationDataFile = @"G:\Datasets\CDC\zehn_min_tu_Beschreibung_Stationen.txt";

            ProcessStationData(csvStationDataFile);

            // Import 10 Minute CDC Weather Data:
            var csvWeatherDataFiles = GetFilesFromFolder(@"G:\Datasets\CDC");

            ProcessLocalWeatherData(csvWeatherDataFiles);

            if (log.IsInfoEnabled)
            {
                log.Info("Import finished");
            }
        }


        private static void ProcessStationData(string csvFilePath)
        {
            log.Info($"Processing File: {csvFilePath}");

            var batches = Parsers
                .StationParser
                .ReadFromFile(csvFilePath, Encoding.UTF8, 2)
                .Where(x => x.IsValid)
                .Select(x => x.Result)
                .Select(x => LocalWeatherDataConverter.Convert(x))
                .Batch(500);

            // Construct the Batch Processor:
            var processor = new StationBatchProcessor(ConnectionString);

            foreach (var batch in batches)
            {
                // Finally write them with the Batch Writer:
                processor.Write(batch);
            }
        }

        private static void ProcessLocalWeatherData(string[] csvFiles)
        {
            var processor = new LocalWeatherDataBatchProcessor(ConnectionString);

            csvFiles
                .AsParallel()
                .WithDegreeOfParallelism(4)
                .ForAll(file =>
                {
                    log.Info($"Processing File: {file}");

                    // Access to the List of Parsers:
                    var batches = Parsers
                        // Use the LocalWeatherData Parser:
                        .LocalWeatherDataParser
                        // Read the File:
                        .ReadFromFile(file, Encoding.UTF8, 1)
                        // Get the Valid Results:
                        .Where(x => x.IsValid)
                        // And get the populated Entities:
                        .Select(x => x.Result)
                        // Convert into the Sql Data Model:
                        .Select(x => LocalWeatherDataConverter.Convert(x))
                        // Batch:
                        .Batch(80000);

                    foreach (var batch in batches)
                    {
                        processor.Write(batch);
                    }
                });
        }

        private static string[] GetFilesFromFolder(string directory)
        {
            return Directory.GetFiles(directory, "produkt_zehn_min_*.txt").ToArray();
        }
    }
}

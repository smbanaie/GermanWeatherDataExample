// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Parser;
using Experiments.Common.Extensions;
using NLog;
using NLog.Config;
using TimescaleExperiment.Sql.Client;

namespace TimescaleExperiment.ConsoleApp
{
    public class Program
    {
        // The ConnectionString used to decide which database to connect to:
        private static readonly string ConnectionString = @"Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;";

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");
            
            if (log.IsInfoEnabled)
            {
                log.Info("Import started");
            }

            // Import 10 Minute CDC Weather Data:
            var csvWeatherDataFiles = GetFilesFromFolder(@"D:\datasets\CDC");

            foreach (var csvWeatherDataFile in csvWeatherDataFiles)
            {
                ProcessLocalWeatherData(csvWeatherDataFile);
            }

            if (log.IsInfoEnabled)
            {
                log.Info("Import finished");
            }
        }

        private static void ProcessLocalWeatherData(string csvFilePath)
        {
            log.Info($"Processing File: {csvFilePath}");

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
                // Convert into the Sql Data Model:
                .Select(x => Converters.Converters.Convert(x))
                // Sequential:
                .AsEnumerable()
                // Batch:
                .Batch(80000);
            
            // Construct the Batch Processor:
            var processor = new LocalWeatherDataBatchProcessor(ConnectionString);

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

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Linq;
using System.Text;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Parser;
using Experiments.Common.Extensions;
using NLog;
using SqlServerExperiment.Sql.Client;

namespace SqlServerExperiment.ConsoleApp
{
    public class Program
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

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

            log.Info("Import finished");
        }

        private static void ProcessStationData(string csvFilePath)
        {
            if (log.IsInfoEnabled)
            {
                log.Info($"Processing File: {csvFilePath}");
            }

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
                // Convert into the Sql Data Model:
                .Select(x => Converters.Converters.Convert(x))
                // Sequential:
                .AsEnumerable()
                // Batch:
                .Batch(80000);

            // Finally write them with the Batch Writer:
            foreach (var batch in batches)
            {
                processor.Write(batch);
            }
        }


        private static void ProcessLocalWeatherData(string csvFilePath)
        {
            if(log.IsInfoEnabled)
            {
                log.Info($"Processing File: {csvFilePath}");
            }

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
                // Convert into the Sql Data Model:
                .Select(x => Converters.Converters.Convert(x))
                // Sequential Evaluation:
                .AsEnumerable()
                // Batch in 80000 Entities / or wait 1 Second:
                .Batch(80000);

            foreach (var batch in batches)
            {
                processor.Write(batch);
            }
        }

        private static string[] GetFilesFromFolder(string directory)
        {
            return Directory
                .GetFiles(directory, "produkt_zehn_min_*.txt")
                .ToArray();
        }
    }
}

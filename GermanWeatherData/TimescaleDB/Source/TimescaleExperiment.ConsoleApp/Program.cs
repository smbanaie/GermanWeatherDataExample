// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using Experiments.Common.Csv.Extensions;
using Experiments.Common.Csv.Parser;
using Experiments.Common.Extensions;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using TimescaleExperiment.Sql.Client;

namespace TimescaleExperiment.ConsoleApp
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        // The ConnectionString used to decide which database to connect to:
        private static readonly string ConnectionString = @"Server=127.0.0.1;Port=5432;Keepalive=600;Database=sampledb;User Id=philipp;Password=test_pwd;";

        // Initialize Log4Net:
        public static void InitializeLog4Net()
        {
            Hierarchy hierarchy = (Hierarchy) LogManager.GetRepository(typeof(Program).Assembly);
            
            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender rollingFileAppender = new RollingFileAppender();
            rollingFileAppender.AppendToFile = false;
            rollingFileAppender.File = @"C:\temp\log.txt";
            rollingFileAppender.Layout = patternLayout;
            rollingFileAppender.MaxSizeRollBackups = 5;
            rollingFileAppender.MaximumFileSize = "10MB";
            rollingFileAppender.RollingStyle = RollingFileAppender.RollingMode.Size;
            rollingFileAppender.StaticLogFileName = true;
            rollingFileAppender.ActivateOptions();
            rollingFileAppender.Threshold = Level.Error;

            hierarchy.Root.AddAppender(rollingFileAppender);

            ConsoleAppender consoleAppender = new ConsoleAppender();
            consoleAppender.Layout = patternLayout;
            consoleAppender.Threshold = Level.Debug;

            hierarchy.Root.AddAppender(consoleAppender);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;
        }

        public static void Main(string[] args)
        {
            InitializeLog4Net();

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
                // Stop Parallelism:
                .AsEnumerable()
                // Batch in 10000 Entities / or wait 1 Second:
                .Batch(20000)
                // And subscribe to the Batch
                .Select(records =>
                {
                    return records
                        // Group By WBAN to avoid duplicate Stations in the Batch:
                        .GroupBy(x => x.Identifier)
                        // Only Select the First Station:
                        .Select(x => x.First())
                        // Convert into the Sql Data Model:
                        .Select(x => Converters.Converters.Convert(x))
                        // Evaluate:
                        .ToList();
                });

            foreach (var batch in batches)
            {
                // Finally write them with the Batch Writer:
                processor.Write(batch);
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
                // As an Observable:
                .AsEnumerable()
                // Batch:
                .Batch(50000)
                // And subscribe to the Batch
                .Select(records =>
                {
                    return records
                        // Group by WBAN, Date and Time to avoid duplicates for this batch:
                        .GroupBy(x => new { x.StationIdentifier, x.TimeStamp })
                        // If there are duplicates then make a guess and select the first one:
                        .Select(x => x.First())
                        // Convert into the Sql Data Model:
                        .Select(x => Converters.Converters.Convert(x))
                        // Evaluate:
                        .ToList();
                });


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

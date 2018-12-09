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
using NLog;
using NLog.Config;

namespace InfluxExperiment.ConsoleApp
{
    public class Program
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        // The ConnectionString used to decide which database to connect to:
        private static readonly string ConnectionString = @"http://localhost:8086";

        private static readonly string Database = @"weather_data";

        public static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");

            ProcessLocalWeatherData();

            if (log.IsInfoEnabled)
            {
                log.Info("Importing Data for Database weather_data has finished");
            }
        }

        private static void ProcessLocalWeatherData()
        {
            // Import 10 Minute CDC Weather Data:
            var csvWeatherDataFiles = GetFilesFromFolder(@"D:\datasets\CDC");

            foreach (var csvWeatherDataFile in csvWeatherDataFiles)
            {
                ProcessLocalWeatherData(csvWeatherDataFile);
            }
        }

        private static void ProcessLocalWeatherData(string csvFilePath)
        {
            if (log.IsInfoEnabled)
            {
                log.Info($"Processing File: {csvFilePath}");
            }

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
                // Let's stay safe! Stop parallelism here:
                .AsEnumerable()
                // Evaluate:
                .Batch(50000)
                // Convert each Batch into a LineProtocolPayload:
                .Select(measurements => LocalWeatherDataConverter.Convert(measurements));

            foreach (var batch in batches)
            {
                try
                {
                    var result = processor.WriteAsync(batch).GetAwaiter().GetResult();

                    // Log all unsuccessful writes, but do not quit execution:
                    if (!result.Success)
                    {
                        if (log.IsErrorEnabled)
                        {
                            log.Error(result.ErrorMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    // Some Pokemon Exception Handling here. I am seeing TaskCanceledExceptions with the 
                    // InfluxDB .NET Client. At the same time I do not want to quit execution, because 
                    // some batches fail:
                    if (log.IsErrorEnabled)
                    {
                        log.Error(e, "Error occured writing InfluxDB Payload");
                    }
                }
            }
        }

        private static string[] GetFilesFromFolder(string directory)
        {
            return Directory.GetFiles(directory, "produkt_zehn_min_*.txt").ToArray();
        }
    }
}

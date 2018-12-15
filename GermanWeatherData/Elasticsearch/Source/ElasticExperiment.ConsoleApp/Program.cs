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
using NLog;
using NLog.Config;

namespace ElasticExperiment.ConsoleApp
{
    public class Program
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        // The ConnectionString used to decide which database to connect to:
        private static readonly Uri ConnectionString = new Uri("http://localhost:9200");

        public static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");

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
            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// https://www.elastic.co/guide/en/elasticsearch/reference/master/tune-for-indexing-speed.html
        /// </summary>
        /// <param name="csvFilePath"></param>
        private static void ProcessLocalWeatherData(string csvFilePath)
        {
            if (log.IsInfoEnabled)
            {
                log.Info($"Processing File: {csvFilePath}");
            }

            // Construct the Batch Processor:
            var client = new ElasticSearchClient<Elastic.Model.LocalWeatherData>(ConnectionString, "weather_data");

            // We are creating the Index with special indexing options for initial load, 
            // as suggested in the Elasticsearch documentation at [1].
            //
            // We disable the performance-heavy indexing during the initial load and also 
            // disable any replicas of the data. This comes at a price of not being able 
            // to query the data in realtime, but it will enhance the import speed.
            //
            // After the initial load I will revert to the standard settings for the Index
            // and set the default values for Shards and Refresh Interval.
            //
            // [1]: https://www.elastic.co/guide/en/elasticsearch/reference/master/tune-for-indexing-speed.html
            //
            client.CreateIndex(settings => settings
                .NumberOfReplicas(0)
                .RefreshInterval(-1));
            
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
                .Batch(30000);


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

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TimescaleExperiment.Csv.Mapper;
using TimescaleExperiment.Csv.Model;
using TimescaleExperiment.Csv.Tokenizer;
using TinyCsvParser;

namespace TimescaleExperiment.Csv.Parser
{
    public static class Parsers
    {
        public static CsvParser<Station> StationParser
        {
            get
            {
                CsvParserOptions csvParserOptions = new CsvParserOptions(false, string.Empty, Tokenizers.StationsTokenizer, 1, false);

                return new CsvParser<Station>(csvParserOptions, new StationMapper());
            }
        }

        public static CsvParser<LocalWeatherData> LocalWeatherDataParser
        {
            get
            {
                CsvParserOptions csvParserOptions = new CsvParserOptions(false, string.Empty, Tokenizers.LocalWeatherDataTokenizer, 1, false);

                return new CsvParser<LocalWeatherData>(csvParserOptions, new LocalWeatherDataMapper());
            }
        }
    }
}
// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CsvStationType = Experiments.Common.Csv.Model.Station;
using CsvLocalWeatherDataType = Experiments.Common.Csv.Model.LocalWeatherData;

using ElasticStationType = ElasticExperiment.Elastic.Model.Station;
using ElasticLocalWeatherDataType = ElasticExperiment.Elastic.Model.LocalWeatherData;

namespace ElasticExperiment.Converters
{
    public static class LocalWeatherDataConverter
    {
        public static ElasticLocalWeatherDataType Convert(CsvLocalWeatherDataType localWeatherData)
        {
            return new ElasticLocalWeatherDataType
            {
                Station = localWeatherData.StationIdentifier,
                AirTemperatureAt2m = localWeatherData.AirTemperatureAt2m,
                AirTemperatureAt5cm = localWeatherData.AirTemperatureAt5cm,
                DewPointTemperatureAt2m = localWeatherData.DewPointTemperatureAt2m,
                QualityCode = localWeatherData.QualityCode,
                RelativeHumidity = localWeatherData.RelativeHumidity,
                StationPressure = localWeatherData.StationPressure,
                TimeStamp = localWeatherData.TimeStamp
            };
        }
    }
}

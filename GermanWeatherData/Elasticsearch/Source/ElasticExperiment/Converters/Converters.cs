// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Nest;
using CsvStationType = Experiments.Common.Csv.Model.Station;
using CsvLocalWeatherDataType = Experiments.Common.Csv.Model.LocalWeatherData;

using ElasticStationType = ElasticExperiment.Elastic.Model.Station;
using ElasticLocalWeatherDataType = ElasticExperiment.Elastic.Model.LocalWeatherData;

namespace ElasticExperiment.Converters
{

    public static class LocalWeatherDataConverter
    {
        public static ElasticStationType Convert(CsvStationType source)
        {
            if (source == null)
            {
                return null;
            }

            return new ElasticStationType
            {
                Identifier = source.Identifier,
                Name =  source.Name,
                GeoLocation = new GeoLocation(source.Latitude, source.Longitude),
                StartDate = source.StartDate,
                StationHeight = source.StationHeight,
                EndDate = source.EndDate,
                State = source.State
            };
        }


        public static ElasticLocalWeatherDataType Convert(CsvLocalWeatherDataType source)
        {
            if (source == null)
            {
                return null;
            }

            return new ElasticLocalWeatherDataType
            {
                Station = source.StationIdentifier,
                AirTemperatureAt2m = source.AirTemperatureAt2m,
                AirTemperatureAt5cm = source.AirTemperatureAt5cm,
                DewPointTemperatureAt2m = source.DewPointTemperatureAt2m,
                QualityCode = source.QualityCode,
                RelativeHumidity = source.RelativeHumidity,
                StationPressure = source.StationPressure,
                TimeStamp = source.TimeStamp
            };
        }
    }
}

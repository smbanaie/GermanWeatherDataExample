// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CsvStationType = Experiments.Common.Csv.Model.Station;
using CsvLocalWeatherDataType = Experiments.Common.Csv.Model.LocalWeatherData;

using Neo4jStationType = Neo4jExperiment.Graph.Model.Station;
using Neo4jLocalWeatherDataType = Neo4jExperiment.Graph.Model.LocalWeatherData;

namespace Neo4jExperiment.Converters
{

    public static class LocalWeatherDataConverter
    {
        public static Neo4jStationType Convert(CsvStationType source)
        {
            if (source == null)
            {
                return null;
            }

            return new Neo4jStationType
            {
                Identifier = source.Identifier,
                Name = source.Name,
                StartDate = source.StartDate,
                StationHeight = source.StationHeight,
                EndDate = source.EndDate,
                State = source.State,
                Latitude = source.Latitude,
                Longitude =  source.Longitude
            };
        }


        public static Neo4jLocalWeatherDataType Convert(CsvLocalWeatherDataType source)
        {
            if (source == null)
            {
                return null;
            }

            return new Neo4jLocalWeatherDataType
            {
                StationIdentifier = source.StationIdentifier,
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

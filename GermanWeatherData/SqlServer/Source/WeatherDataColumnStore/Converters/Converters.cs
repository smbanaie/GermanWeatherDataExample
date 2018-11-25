// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CsvLocalWeatherDataType = WeatherDataColumnStore.Csv.Model.LocalWeatherData;
using SqlLocalWeatherDataType = WeatherDataColumnStore.Sql.Model.LocalWeatherData;

using CsvStationDataType = WeatherDataColumnStore.Csv.Model.Station;
using SqlStationDataType = WeatherDataColumnStore.Sql.Model.Station;

namespace WeatherDataColumnStore.Converters
{
    public static class Converters
    {
        public static SqlLocalWeatherDataType Convert(CsvLocalWeatherDataType source)
        {
            return new SqlLocalWeatherDataType
            {
                StationIdentifier = source.StationIdentifier,
                AirTemperatureAt2m = source.AirTemperatureAt2m,
                StationPressure = source.StationPressure,
                TimeStamp = source.TimeStamp,
                RelativeHumidity = source.RelativeHumidity,
                DewPointTemperatureAt2m = source.DewPointTemperatureAt2m,
                AirTemperatureAt5cm = source.AirTemperatureAt5cm,
                QualityCode = source.QualityCode
            };
        }

        public static SqlStationDataType Convert(CsvStationDataType source)
        {
            return new SqlStationDataType
            {
                StartDate = source.StartDate,
                Identifier = source.Identifier,
                Longitude = source.Longitude,
                Latitude = source.Latitude,
                EndDate = source.EndDate,
                StationHeight = source.StationHeight,
                Name = source.Name,
                State = source.State
            };
        }
    }
}

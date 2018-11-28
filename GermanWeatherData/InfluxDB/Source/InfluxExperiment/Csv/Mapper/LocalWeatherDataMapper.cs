// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using InfluxExperiment.Csv.Converter;
using InfluxExperiment.Csv.Model;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace InfluxExperiment.Csv.Mapper
{
    public class LocalWeatherDataMapper : CsvMapping<LocalWeatherData>
    {
        public LocalWeatherDataMapper()
        {
            MapProperty(0, x => x.StationIdentifier);
            MapProperty(1, x => x.TimeStamp, new DateTimeConverter("yyyyMMddHHmm"));
            MapProperty(2, x => x.QualityCode);
            MapProperty(3, x => x.StationPressure, new IgnoreMissingValuesConverter("-999"));
            MapProperty(4, x => x.AirTemperatureAt2m, new IgnoreMissingValuesConverter("-999"));
            MapProperty(5, x => x.AirTemperatureAt5cm, new IgnoreMissingValuesConverter("-999"));
            MapProperty(6, x => x.RelativeHumidity, new IgnoreMissingValuesConverter("-999"));
            MapProperty(7, x => x.DewPointTemperatureAt2m, new IgnoreMissingValuesConverter("-999"));
        }
    }
}

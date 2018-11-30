// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using InfluxDB.LineProtocol.Payload;
using Experiments.Common.Csv.Model;
using CsvLocalWeatherDataType = Experiments.Common.Csv.Model.LocalWeatherData;

namespace InfluxExperiment.Converters
{
    public static class LocalWeatherDataConverter
    {
        public static LineProtocolPayload Convert(IEnumerable<CsvLocalWeatherDataType> source)
        {
            if (source == null)
            {
                return null;
            }

            LineProtocolPayload payload = new LineProtocolPayload();

            foreach (var item in source)
            {
                var point = Convert(item);

                if (point != null)
                {
                    payload.Add(point);
                }
            }

            return payload;
        }

        public static LineProtocolPoint Convert(LocalWeatherData source)
        {
            if (source == null)
            {
                return null;
            }

            var fields = new Dictionary<string, object>();

            fields.AddFieldValue("air_temperature_at_2m", source.AirTemperatureAt2m);
            fields.AddFieldValue("air_temperature_at_5cm", source.AirTemperatureAt5cm);
            fields.AddFieldValue("dew_point_temperature_at_2m", source.DewPointTemperatureAt2m);
            fields.AddFieldValue("relative_humidity", source.RelativeHumidity);

            // No Measurements to be inserted:
            if (fields.Count == 0)
            {
                return null;
            }

            var tags = new Dictionary<string, string>
                {
                    {"station_identifier", source.StationIdentifier},
                    {"quality_code", source.QualityCode.ToString(CultureInfo.InvariantCulture)}
                };

            return new LineProtocolPoint("weather_measurement", new ReadOnlyDictionary<string, object>(fields), tags, source.TimeStamp);
        }

        private static void AddFieldValue(this IDictionary<string, object> dictionary, string key, object value)
        {
            if (value == null)
            {
                return;
            }

            dictionary.Add(key, value);
        }
    }

}

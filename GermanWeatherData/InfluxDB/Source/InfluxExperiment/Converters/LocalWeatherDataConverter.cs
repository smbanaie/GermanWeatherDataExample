// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            public static LineProtocolPayload Convert(IList<CsvLocalWeatherDataType> source)
            {
                if (source == null)
                {
                    return null;
                }

                LineProtocolPayload payload = new LineProtocolPayload();

                Parallel.ForEach(source, x =>
                {
                    var point = Convert(x);

                    payload.Add(point);
                });

                return payload;
            }

            public static LineProtocolPoint Convert(LocalWeatherData source)
            {
                if (source == null)
                {
                    return null;
                }

                var fields = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>
                {
                    {"air_temperature_at_2m", source.AirTemperatureAt2m },
                    {"air_temperature_at_5cm", source.AirTemperatureAt5cm },
                    {"dew_point_temperature_at_2m", source.DewPointTemperatureAt2m },
                    {"relative_humidity", source.RelativeHumidity },
                });

                var tags = new Dictionary<string, string>
                {
                    {"station_identifier", source.StationIdentifier},
                    {"quality_code", source.QualityCode.ToString(CultureInfo.InvariantCulture)}
                };

                return new LineProtocolPoint("weather_measurement", fields, tags, source.TimeStamp);
            }
        }
    
}

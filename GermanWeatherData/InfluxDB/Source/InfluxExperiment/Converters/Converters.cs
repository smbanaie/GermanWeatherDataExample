// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using InfluxDB.LineProtocol.Payload;
using InfluxExperiment.Csv.Model;
using CsvLocalWeatherDataType = InfluxExperiment.Csv.Model.LocalWeatherData;

namespace InfluxExperiment.Converters
{
    public static class Converters
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
                    {"", "" }
                });

                var tags = new Dictionary<string, string>
                {
                    {"", ""}
                };

                return new LineProtocolPoint("weather_measurement", fields, tags, source.TimeStamp);
            }
        }
    
}

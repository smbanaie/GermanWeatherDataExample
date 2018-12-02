// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Newtonsoft.Json;

namespace Neo4jExperiment.Graph.Model
{
    public class LocalWeatherData
    {
        [JsonProperty("station_identifier")]
        public string StationIdentifier { get; set; }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("quality_code")]
        public byte QualityCode { get; set; }

        [JsonProperty("station_pressure")]
        public float? StationPressure { get; set; }

        [JsonProperty("air_temperature_at_2m")]
        public float? AirTemperatureAt2m { get; set; }

        [JsonProperty("air_temperature_at_5cm")]
        public float? AirTemperatureAt5cm { get; set; }

        [JsonProperty("relative_humidity")]
        public float? RelativeHumidity { get; set; }

        [JsonProperty("dew_point_temperature_at_2m")]
        public float? DewPointTemperatureAt2m { get; set; }
    }
}

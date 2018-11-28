// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TimescaleExperiment.Sql.Model
{
    public class LocalWeatherData
    {
        public string StationIdentifier { get; set; }

        public DateTime TimeStamp { get; set; }

        public byte QualityCode { get; set; }

        public float? StationPressure { get; set; }

        public float? AirTemperatureAt2m { get; set; }

        public float? AirTemperatureAt5cm { get; set; }

        public float? RelativeHumidity { get; set; }

        public float? DewPointTemperatureAt2m { get; set; }
    }
}
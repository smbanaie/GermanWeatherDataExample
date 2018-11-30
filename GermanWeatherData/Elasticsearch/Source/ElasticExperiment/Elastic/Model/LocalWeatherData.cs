using System;
using System.Collections.Generic;
using System.Text;
using Nest;

namespace ElasticExperiment.Elastic.Model
{
    
    public class LocalWeatherData
    {
        [Text]
        public string Station { get; set; }

        [Date]
        public DateTime TimeStamp { get; set; }

        [Number(NumberType.Byte)]
        public byte QualityCode { get; set; }

        [Number(NumberType.Float)]
        public float? StationPressure { get; set; }

        [Number(NumberType.Float)]
        public float? AirTemperatureAt2m { get; set; }

        [Number(NumberType.Float)]
        public float? AirTemperatureAt5cm { get; set; }

        [Number(NumberType.Float)]
        public float? RelativeHumidity { get; set; }

        [Number(NumberType.Float)]
        public float? DewPointTemperatureAt2m { get; set; }
    }
}

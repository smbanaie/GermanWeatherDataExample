// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Experiments.Common.Csv.Converter;
using Experiments.Common.Csv.Model;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace Experiments.Common.Csv.Mapper
{
    public class StationMapper : CsvMapping<Station>
    {
        public StationMapper()
        {
            MapProperty(0, x => x.Identifier);
            MapProperty(1, x => x.StartDate, new DateTimeConverter("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
            MapProperty(2, x => x.EndDate, new NullableDateTimeConverter("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
            MapProperty(3, x => x.StationHeight);
            MapProperty(4, x => x.Latitude);
            MapProperty(5, x => x.Longitude);
            MapProperty(6, x => x.Name);
            MapProperty(7, x => x.State);
        }
    }
}

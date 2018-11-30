// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Nest;

namespace ElasticExperiment.Elastic.Model
{
    public class Station
    {
        [Text]
        public string Identifier { get; set; }

        [Text]
        public string Name { get; set; }

        [Date]
        public DateTime StartDate { get; set; }

        [Date]
        public DateTime? EndDate { get; set; }

        [Number(NumberType.Integer)]
        public short StationHeight { get; set; }

        [Text]
        public string State { get; set; }

        [GeoPoint]
        public GeoLocation GeoLocation { get; set; }
    }
}

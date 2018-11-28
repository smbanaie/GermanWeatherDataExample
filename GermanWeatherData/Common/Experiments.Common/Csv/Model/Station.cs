// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Experiments.Common.Csv.Model
{
    public class Station
    {
        public string Identifier { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public short StationHeight { get; set; }

        public string State { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }
    }
}

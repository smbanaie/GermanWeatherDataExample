﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace Experiments.Common.Csv.Extensions
{
    public static class CsvParserExtensions
    {
        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromFile<TEntity>(this CsvParser<TEntity> csvParser, string fileName, Encoding encoding, int skip)
            where TEntity : class, new()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            var lines = File
                .ReadLines(fileName, encoding)
                .Select((line, index) => new Row(index, line))
                .Skip(skip);

            return csvParser.Parse(lines);
        }
    }
}

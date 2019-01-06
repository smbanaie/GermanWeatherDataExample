// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using TinyCsvParser.Tokenizer;

namespace Experiments.Common.Csv.Tokenizer
{
    /// <summary>
    /// Implements a Tokenizer, that makes it possible to Tokenize a CSV line using fixed length columns.
    /// </summary>
    public class CustomFixedLengthTokenizer : ITokenizer
    {
        /// <summary>
        /// A column in a CSV file, which is described by the start and end position (zero-based indices).
        /// </summary>
        public class Column
        {
            public readonly int Start;

            public readonly int End;

            public Column(int start, int end)
            {
                Start = start;
                End = end;
            }

            public override string ToString()
            {
                return string.Format("ColumnDefinition (Start = {0}, End = {1}", Start, End);
            }
        }

        public readonly Column[] Columns;

        public CustomFixedLengthTokenizer(Column[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }
            Columns = columns;
        }

        public CustomFixedLengthTokenizer(IList<Column> columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }
            Columns = columns.ToArray();
        }

        public string[] Tokenize(string input)
        {
            string[] tokenizedLine = new string[Columns.Length];

            for (int columnIndex = 0; columnIndex < Columns.Length; columnIndex++)
            {
                var column = Columns[columnIndex];
                var columnData = input.Substring(column.Start, column.End - column.Start);

                tokenizedLine[columnIndex] = columnData.Trim();
            }

            return tokenizedLine;
        }

        public override string ToString()
        {
            var columnDefinitionsString = string.Join(", ", Columns.Select(x => x.ToString()));

            return string.Format("FixedLengthTokenizer (Columns = [{0}])", columnDefinitionsString);
        }
    }
}
// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using TinyCsvParser.Tokenizer;
using ColumnDefinition = TinyCsvParser.Tokenizer.FixedLengthTokenizer.ColumnDefinition;

namespace Experiments.Common.Csv.Tokenizer
{
    public class CustomFixedLengthTokenizer : ITokenizer
    {
        private readonly bool trim;
        private readonly ITokenizer tokenizer;

        public CustomFixedLengthTokenizer(ColumnDefinition[] columns, bool trim = true)
        {
            this.tokenizer = new FixedLengthTokenizer(columns);
            this.trim = trim;
        }

        public string[] Tokenize(string input)
        {
            var tokens = tokenizer.Tokenize(input);

            if (trim)
            {
                return tokens
                    .Select(x => x.Trim())
                    .ToArray();
            }

            return tokens;
        }
    }
}
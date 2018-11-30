// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Tokenizer;

namespace Experiments.Common.Csv.Tokenizer
{
    public static class Tokenizers
    {
        public static ITokenizer StationsTokenizer
        {
            get
            {
                var columnDefinitions = new CustomFixedLengthTokenizer.ColumnDefinition[]
                {
                    new CustomFixedLengthTokenizer.ColumnDefinition(0, 6),
                    new CustomFixedLengthTokenizer.ColumnDefinition(6, 14),
                    new CustomFixedLengthTokenizer.ColumnDefinition(15, 23),
                    new CustomFixedLengthTokenizer.ColumnDefinition(32, 39),
                    new CustomFixedLengthTokenizer.ColumnDefinition(43, 51),
                    new CustomFixedLengthTokenizer.ColumnDefinition(52, 61),
                    new CustomFixedLengthTokenizer.ColumnDefinition(61, 102),
                    new CustomFixedLengthTokenizer.ColumnDefinition(102, 125),
                };

                return new CustomFixedLengthTokenizer(columnDefinitions, true);
            }
        }

        public static ITokenizer LocalWeatherDataTokenizer
        {
            get
            {
                return new StringSplitTokenizer(new [] {';' }, true);
            }
        }
    }
}

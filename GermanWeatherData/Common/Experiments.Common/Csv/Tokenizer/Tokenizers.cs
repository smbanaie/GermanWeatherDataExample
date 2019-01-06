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
                var columns = new[]
                {
                    new CustomFixedLengthTokenizer.Column(0, 6),
                    new CustomFixedLengthTokenizer.Column(6, 14),
                    new CustomFixedLengthTokenizer.Column(15, 23),
                    new CustomFixedLengthTokenizer.Column(32, 39),
                    new CustomFixedLengthTokenizer.Column(43, 51),
                    new CustomFixedLengthTokenizer.Column(52, 61),
                    new CustomFixedLengthTokenizer.Column(61, 102),
                    new CustomFixedLengthTokenizer.Column(102, 125),
                };

                return new CustomFixedLengthTokenizer(columns);
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

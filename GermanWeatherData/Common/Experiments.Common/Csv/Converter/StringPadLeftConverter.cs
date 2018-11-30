// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace Experiments.Common.Csv.Converter
{
    public class StringPadLeftConverter : ITypeConverter<string>
    {
        private readonly int totalWidth;
        private readonly char paddingChar;

        public StringPadLeftConverter(int totalWidth, char paddingChar)
        {
            this.totalWidth = totalWidth;
            this.paddingChar = paddingChar;
        }

        public bool TryConvert(string value, out string result)
        {
            result = null;

            if (value == null)
            {
                return true;
            }

            result = value.PadLeft(totalWidth, paddingChar);

            return true;
        }

        public Type TargetType
        {
            get { return typeof(string); }
        }
    }
}

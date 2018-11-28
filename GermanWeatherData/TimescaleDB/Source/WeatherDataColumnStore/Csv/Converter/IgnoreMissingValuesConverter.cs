// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TimescaleExperiment.Csv.Converter
{
    public class IgnoreMissingValuesConverter : ITypeConverter<Single?>
    {
        private readonly string missingValueRepresentation;

        private readonly NullableSingleConverter nullableSingleConverter;
        
        public IgnoreMissingValuesConverter(string missingValueRepresentation)
        {
            this.missingValueRepresentation = missingValueRepresentation;
            this.nullableSingleConverter = new NullableSingleConverter();
        }

        public IgnoreMissingValuesConverter(string missingValueRepresentation, IFormatProvider formatProvider)
        {
            this.missingValueRepresentation = missingValueRepresentation;
            this.nullableSingleConverter = new NullableSingleConverter(formatProvider);
        }

        public IgnoreMissingValuesConverter(string missingValueRepresentation, IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.missingValueRepresentation = missingValueRepresentation;
            this.nullableSingleConverter = new NullableSingleConverter(formatProvider, numberStyles);
        }


        public bool TryConvert(string value, out float? result)
        {
            if(string.Equals(missingValueRepresentation, value, StringComparison.Ordinal))
            {
                result = default(float?);

                return true;
            }

            return nullableSingleConverter.TryConvert(value, out result);
        }

        public Type TargetType
        {
            get { return typeof(Single?); }
        }
    }
}

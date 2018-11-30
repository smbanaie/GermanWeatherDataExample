// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace Experiments.Common.Csv.Converter
{
    public class CustomDateTimeConverter : ITypeConverter<DateTime>
    {
        private readonly DateTimeKind dateTimeKind;

        private readonly DateTimeConverter dateTimeConverter;

        public CustomDateTimeConverter(DateTimeKind dateTimeKind)
            : this(string.Empty, dateTimeKind)
        {
        }

        public CustomDateTimeConverter(string dateTimeFormat, DateTimeKind dateTimeKind)
            : this(dateTimeFormat, CultureInfo.InvariantCulture, dateTimeKind)
        {
        }

        public CustomDateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider, DateTimeKind dateTimeKind)
            : this(dateTimeFormat, formatProvider, DateTimeStyles.None, dateTimeKind)
        {
        }

        public CustomDateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, DateTimeKind dateTimeKind)
        {
            this.dateTimeConverter = new DateTimeConverter(dateTimeFormat, formatProvider, dateTimeStyles);
            this.dateTimeKind = dateTimeKind;
        }

        public bool TryConvert(string value, out DateTime result)
        {
            if (dateTimeConverter.TryConvert(value, out result))
            {
                result = DateTime.SpecifyKind(result, dateTimeKind);

                return true;
            }

            return false;
        }

        public Type TargetType
        {
            get { return typeof(DateTime); }
        }
    }
}

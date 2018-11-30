// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace Experiments.Common.Csv.Converter
{
    public class CustomNullableDateTimeConverter : ITypeConverter<DateTime?>
    {
        private readonly DateTimeKind dateTimeKind;

        private readonly DateTimeConverter dateTimeConverter;

        public CustomNullableDateTimeConverter(DateTimeKind dateTimeKind)
            : this(string.Empty, dateTimeKind)
        {
        }

        public CustomNullableDateTimeConverter(string dateTimeFormat, DateTimeKind dateTimeKind)
            : this(dateTimeFormat, CultureInfo.InvariantCulture, dateTimeKind)
        {
        }

        public CustomNullableDateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider, DateTimeKind dateTimeKind)
            : this(dateTimeFormat, formatProvider, DateTimeStyles.None, dateTimeKind)
        {
        }

        public CustomNullableDateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles, DateTimeKind dateTimeKind)
        {
            this.dateTimeConverter = new DateTimeConverter(dateTimeFormat, formatProvider, dateTimeStyles);
            this.dateTimeKind = dateTimeKind;
        }

        public bool TryConvert(string value, out DateTime? result)
        {
            result = default(DateTime?);

            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            DateTime dateTime;

            if (dateTimeConverter.TryConvert(value, out dateTime))
            {
                result = DateTime.SpecifyKind(dateTime, dateTimeKind);

                return true;
            }

            return false;
        }

        public Type TargetType
        {
            get { return typeof(DateTime?); }
        }
    }
}

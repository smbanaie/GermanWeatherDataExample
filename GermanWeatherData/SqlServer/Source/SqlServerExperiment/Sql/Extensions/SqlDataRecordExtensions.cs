// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.SqlServer.Server;

namespace SqlServerExperiment.Sql.Extensions
{
    public static class SqlDataRecordExtensions
    {
        public static void SetNullableFloat(this SqlDataRecord sqlDataRecord, int ordinal, float? value)
        {
            if (value.HasValue)
            {
                sqlDataRecord.SetFloat(ordinal, value.Value);
            }
            else
            {
                sqlDataRecord.SetDBNull(ordinal);
            }
        }

        public static void SetNullableDateTime(this SqlDataRecord sqlDataRecord, int ordinal, DateTime? value)
        {
            if (value.HasValue)
            {
                sqlDataRecord.SetDateTime(ordinal, value.Value);
            }
            else
            {
                sqlDataRecord.SetDBNull(ordinal);
            }
        }
    }
}

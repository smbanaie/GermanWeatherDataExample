// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace TimescaleExperiment.Sql.Client
{
    public interface IBatchProcessor<in TEntity>
    {
        void Write(IEnumerable<TEntity> measurements);
    }
}
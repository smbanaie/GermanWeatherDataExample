// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace WeatherDataColumnStore.Sql.Client
{
    public interface IBatchProcessor<TEntity>
    {
        void Write(IList<TEntity> measurements);
    }
}
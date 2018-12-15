// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Elasticsearch.Net;
using Nest;

namespace ElasticExperiment.Elastic.Client
{
    public class ElasticSearchClient<TEntity>
        where TEntity : class
    {
        public readonly string IndexName;

        protected readonly IElasticClient Client;

        public ElasticSearchClient(IElasticClient client, string indexName)
        {
            IndexName = indexName;
            Client = client;
        }

        public ElasticSearchClient(Uri connectionString, string indexName)
            : this(CreateClient(connectionString), indexName)
        {
        }

        public ICreateIndexResponse CreateIndex()
        {
            var response = Client.IndexExists(IndexName);

            if (response.Exists)
            {
                return null;

            }

            return Client.CreateIndex(IndexName, index => index.Mappings(mappings => mappings.Map<TEntity>(x => x.AutoMap())));
        }

        public IBulkResponse BulkInsert(IEnumerable<TEntity> entities)
        {
            var request = new BulkDescriptor();

            foreach (var entity in entities)
            {
                request
                    .Index<TEntity>(op => op
                        .Id(Guid.NewGuid().ToString())
                        .Index(IndexName)
                        .Document(entity));
            }

            return Client.Bulk(request);
        }

        private static IElasticClient CreateClient(Uri connectionString)
        {
            var connectionPool = new SingleNodeConnectionPool(connectionString);
            var connectionSettings = new ConnectionSettings(connectionPool);

            return new ElasticClient(connectionSettings);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;

namespace CosmosDbTest.DAL
{
    public abstract class DocumentContext<T>
        : IDisposable
        where T: class, new()
    {
        private readonly ILogger _logger;
        private readonly IDocumentConfiguration _configuration;
        protected static DocumentClient Client;

        public DocumentContext(ILogger logger, IDocumentConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            Initialize();
        }

        public void Dispose()
        {
            Client?.Dispose();
        }

        private Uri CollectionUri => UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseId, _configuration.CollectionId);

        private void Initialize()
        {
            Client = CreateClient(_configuration);
        }

        public virtual async Task<T> FindAsync(string resourceId, string partitionKey)
        {
            try
            {
                
                Document document = await Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseId, _configuration.CollectionId, resourceId), new RequestOptions() { PartitionKey = new PartitionKey(partitionKey)});
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string partitionId)
        {
            IDocumentQuery<T> query = Client
                .CreateDocumentQuery<T>(CollectionUri, new FeedOptions
                {
                    MaxItemCount = -1,
                    PartitionKey = new PartitionKey(partitionId)
                })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();

            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public virtual async Task SaveAsync(T entity, string entityId)
        {
            try
            {
                await UpdateDocumentAsync(_configuration.CollectionId, entityId, entity);
            }
            catch (DocumentClientException docEx)
            {
                if (docEx.StatusCode == HttpStatusCode.NotFound)
                {
                    await CreateAsync(entity, entityId);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public virtual async Task CreateAsync(T entity, string entityId)
        {
            var response = await Client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseId, _configuration.CollectionId), entity);
            _logger.LogInformation($"Created entity { entityId }");
        }

        public virtual async Task UpdateAsync(T entity, string entityId)
        {
            var response = await Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseId, _configuration.CollectionId, entityId), entity);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Entity { entityId } could not be found. No update performed.");
            }
            else
            {
                _logger.LogInformation($"Updated entity { entityId }");
            }
        }

        public virtual async Task DeleteAsync(string entityId, string partitionKey)
        {
            RequestOptions options = new RequestOptions { PartitionKey = new PartitionKey(partitionKey)};
            var uri = UriFactory.CreateDocumentUri(_configuration.DatabaseId, _configuration.CollectionId, entityId);
            await Client.DeleteDocumentAsync(uri, options);
        }

        private async Task<ResourceResponse<Document>> UpdateDocumentAsync(string collectionId, string documentId, object document)
        {
            return await Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseId, collectionId, documentId), document);
        }


        private async Task<ResourceResponse<Document>> CreateDocumentAsync(string collectionId, string documentId, object document)
        {
            return await Client.CreateDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseId, collectionId, documentId), document);

        }
        private DocumentClient CreateClient(IDocumentConfiguration configuration)
        {
            return new DocumentClient(
                new Uri(configuration.DatabaseEndPoint),
                configuration.AuthorizationKey,
                GetConfigurationPolicy());
        }

        private ConnectionPolicy GetConfigurationPolicy() => new ConnectionPolicy
        {
            ConnectionMode = ConnectionMode.Gateway,
            MaxConnectionLimit = -1,
            RetryOptions = new RetryOptions
            {
                MaxRetryAttemptsOnThrottledRequests = 3,
                MaxRetryWaitTimeInSeconds = 30
            }
        };
    }
}
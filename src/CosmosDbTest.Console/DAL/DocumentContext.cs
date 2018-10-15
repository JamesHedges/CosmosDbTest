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
        protected static DocumentClient _client;

        public DocumentContext(ILogger logger, IDocumentConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        private void Initialize()
        {
            _client = CreateClient(_configuration);
        }

        public virtual async Task<T> FindAsync(Guid id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseId, _configuration.CollectionId, id.ToString()));
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

        private Uri CollectionUri => UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseId, _configuration.CollectionId);

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = _client
                .CreateDocumentQuery<T>(CollectionUri, new FeedOptions { MaxItemCount = -1 })
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
            var response = await UpdateDocumentAsync(_configuration.CollectionId, entityId, entity);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var createResponse = await CreateDocumentAsync(_configuration.CollectionId, entityId, entity);
            }
        }

        public virtual async Task CreateAsync(T entity, string entityId)
        {
            var response = await _client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseId, _configuration.CollectionId), entity);
            _logger.LogInformation($"Created entity { entityId }");
        }

        public virtual async Task UpdateAsync(T entity, string entityId)
        {
            var response = await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseId, _configuration.CollectionId, entityId), entity);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Entity { entityId } could not be found. No update performed.");
            }
            else
            {
                _logger.LogInformation($"Updated entity { entityId }");
            }
        }

        private async Task<ResourceResponse<Document>> UpdateDocumentAsync(string collectionId, string documentId, object document)
        {
            return await _client.CreateDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseId, collectionId, documentId), document);
        }


        private async Task<ResourceResponse<Document>> CreateDocumentAsync(string collectionId, string documentId, object document)
        {
            return await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseId, collectionId, documentId), document);

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
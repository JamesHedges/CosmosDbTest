using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using CosmosDbTest.ConfigureDb.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;



// For samples
// git clone https://github.com/Azure/azure-cosmosdb-dotnet.git
namespace CosmosDbTest.DAL
{
    public class AlertRepository : IDisposable, IAlertRepository
    {
        private static DocumentClient _client;
        private readonly ILogger _logger;
        private readonly ICosmosDbTestConfiguration _configuration;
        private string DbName => "TestDb";
        private string CollectionName => "TestCollection";

        public AlertRepository(ILogger logger, ICosmosDbTestConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            Initialize();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        private void Initialize()
        {
            _client = CreateClient(_configuration);
        }

        public async Task<Alert> FindAsync(Guid id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseName, "Alert", id.ToString()));
                return (Alert)(dynamic)document;
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

        private Uri CollectionUri => UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseName, "Alert");

        public async Task<IEnumerable<Alert>> FindAsync(Expression<Func<Alert, bool>> predicate)
        {
            IDocumentQuery<Alert> query = _client
                .CreateDocumentQuery<Alert>(CollectionUri, new FeedOptions {MaxItemCount = -1})
                .Where(predicate)
                .AsDocumentQuery();

            List<Alert> results = new List<Alert>();

            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<Alert>());
            }

            return results;
        }

        public async Task SaveAlertAsync(Alert alert)
        {
            var response = await UpdateDocumentAsync("Alert", alert.AlertId.ToString(), alert);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var createResponse = await CreateDocumentAsync("Alert", alert.AlertId.ToString(), alert);
            }
        }

        public async Task CreateAlertAsync(Alert alert)
        {
            var response = await _client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseName, "Alert"), alert);
            _logger.LogInformation($"Created alert { alert.AlertId }");
        }

        public async Task UpdateAlertAsync(Alert alert)
        {
            var response = await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseName, "Alert", alert.AlertId.ToString()), alert);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Alert { alert.AlertId } could not be found. Not update performed.");
            }
            else
            {
                _logger.LogInformation($"Updated alert { alert.AlertId }");
            }
        }

        private async Task<ResourceResponse<Microsoft.Azure.Documents.Document>> UpdateDocumentAsync(string collectionId, string documentId, object document)
        {
            return await _client.CreateDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseName, collectionId, documentId), document);

        }


        private async Task<ResourceResponse<Microsoft.Azure.Documents.Document>> CreateDocumentAsync(string collectionId, string documentId, object document)
        {
            return await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_configuration.DatabaseName, collectionId, documentId), document);

        }
        private DocumentClient CreateClient(ICosmosDbTestConfiguration configuration)
        {
            return new DocumentClient(
                new Uri(configuration.DatabaseEndPointUrl),
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

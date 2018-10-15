using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;

namespace CosmosDbTest.DAL
{
    public class MigrateDocumentDb
    {
        private readonly ILogger _logger;
        private readonly ICosmosDbTestConfiguration _configuration;
        private readonly DocumentClient _client;

        public MigrateDocumentDb(ILogger logger, ICosmosDbTestConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _client = CreateClient(_configuration);
        }

        private DocumentClient CreateClient(ICosmosDbTestConfiguration configuration)
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

        public async Task DoMigration()
        {
            try
            {
                var db = await CreateDatabase();
                await CreateCollections();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
            // create the collections
        }

        private async Task<Database> CreateDatabase()
        {
            try
            {
                var db = await _client.CreateDatabaseIfNotExistsAsync(new Database { Id = _configuration.DatabaseId});
                _logger.LogInformation($"Created if not exists db: {_configuration.DatabaseId}");
                return db;
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed creating database {_configuration.DatabaseId}", e);
                throw;
            }
        }

        private async Task CreateCollections()
        {
            var collectionDescriptions = _configuration.CollectionDescriptions;
            foreach (CollectionDescription collectionDescription in collectionDescriptions)
            {
                await CreateCollection(collectionDescription);
            }
        }

        private async Task<DocumentCollection> CreateCollection(CollectionDescription collectionDescription)
        {
            try
            {
                Uri dbUri = UriFactory.CreateDatabaseUri(_configuration.DatabaseId);
                DocumentCollection collection = new DocumentCollection
                {
                    Id = collectionDescription.Name,
                    PartitionKey = new PartitionKeyDefinition { Paths = new Collection<string> { $"/{collectionDescription.PartitionKey}"} }
                };
                RequestOptions options = new RequestOptions { OfferThroughput = 400 };
                return await _client?.CreateDocumentCollectionIfNotExistsAsync(dbUri, collection, options);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed creating collection {collectionDescription.Name}", e);
                throw;
            }
        }

    }
}
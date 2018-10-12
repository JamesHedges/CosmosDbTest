using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;


// For samples
// git clone https://github.com/Azure/azure-cosmosdb-dotnet.git
namespace CosmosDbTest.DAL
{
    public class TestRepos : IDisposable
    {
        private static DocumentClient _client;
        private readonly ICosmosDbTestConfiguration _configuration;
        private string DbName => "TestDb";
        private string CollectionName => "TestCollection";

        public TestRepos(ICosmosDbTestConfiguration configuration)
        {
            _configuration = configuration;
            Initialize().Wait();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public async Task Initialize()
        {
            _client = CreateClient(_configuration);
            await CheckDatabase(DbName);
            await CheckCollection(CollectionName);
        }

        private DocumentClient CreateClient(ICosmosDbTestConfiguration configuration)
        {
            return new DocumentClient(
                new Uri(configuration.DatabaseEndPointUrl),
                configuration.AuthorizationKey,
                GetConfigurationPolicy());
        }

        private async Task<Database> CheckDatabase(string databaseName)
        {
            return await _client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName });
        }

        private async Task<DocumentCollection> CheckCollection(string collectionName)
        {
            Uri dbUri = UriFactory.CreateDatabaseUri(_configuration.DatabaseEndPointUrl);
            DocumentCollection collection = new DocumentCollection { Id = collectionName };
            RequestOptions options = new RequestOptions { OfferThroughput = 400 };
            return await _client?.CreateDocumentCollectionIfNotExistsAsync(dbUri, collection, options);
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

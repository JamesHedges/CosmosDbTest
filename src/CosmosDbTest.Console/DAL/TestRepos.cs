using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;


// For samples
// github clone https://github.com/Azure/azure-cosmosdb-dotnet.git
namespace CosmosDbTest.DAL
{
    public class TestRepos : IDisposable
    {
        private static DocumentClient _client;
        private readonly ICosmosDbTestConfiguration _configuration;

        public TestRepos(ICosmosDbTestConfiguration configuration)
        {
            _client = new DocumentClient(
                new Uri(configuration.DatabaseEndPointUrl), 
                configuration.AuthorizationKey, 
                GetConfigurationPolicy());
        }

        public void Dispose()
        {
            _client.Dispose();
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

        private async Task<Database> CheckDatabase(string databaseName)
        {
            return await _client.CreateDatabaseIfNotExistsAsync(new Database { Id = "TestDb" });
        }
    }
}

using CosmosDbTest.ConfigureDb.Models;
using Microsoft.Extensions.Logging;



// For samples
// git clone https://github.com/Azure/azure-cosmosdb-dotnet.git
namespace CosmosDbTest.DAL
{
    public class AlertRepository : DocumentContext<Alert>, IAlertRepository
    {
        //private string DbName => "TestDb";
        //private string CollectionName => "TestCollection";

        public AlertRepository(ILogger logger, IDocumentConfiguration configuration)
            : base(logger, configuration)
        {
        }
    }
}

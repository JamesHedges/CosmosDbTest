using System.Threading;
using System.Threading.Tasks;
using CosmosDbTest.DAL;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CosmosDbTest.ConfigureDb.Models
{
    public class ConfigureDbCommand : IRequest<ConfigureDbResponse>
    {
    }

    public class ConfigureDbResponse
    {
    }

    public class ConfigureDbHandler : IRequestHandler<ConfigureDbCommand, ConfigureDbResponse>
    {
        private readonly ICosmosDbTestConfiguration _configuration;
        private readonly ILogger _logger;

        public ConfigureDbHandler(ILogger logger, ICosmosDbTestConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ConfigureDbResponse> Handle(ConfigureDbCommand request, CancellationToken cancellationToken)
        {
            await Task.Delay(10);
            _logger.LogInformation("Handling ConfigureDbCommand");
            var migrator = new MigrateDocumentDb(_logger, _configuration);
            await migrator.DoMigration();
            return new ConfigureDbResponse();
        }
    }
}

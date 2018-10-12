using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CosmosDbTest.ConfigureDb
{
    public class SaveAlertCommand : IRequest<SaveAlertResponse>
    {

    }

    public class SaveAlertResponse
    {
    }

    public class SaveAlertHandler : IRequestHandler<SaveAlertCommand, SaveAlertResponse>
    {
        private readonly ILogger _logger;
        private readonly ICosmosDbTestConfiguration _configuration;

        public SaveAlertHandler(ILogger logger, ICosmosDbTestConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task<SaveAlertResponse> Handle(SaveAlertCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}

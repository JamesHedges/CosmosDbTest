using System.Threading;
using System.Threading.Tasks;
using CosmosDbTest.DAL;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CosmosDbTest.ConfigureDb.Models
{
    public class FindAlertCommand : IRequest<FindAlertResponse>
    {
        public string Id { get; set; }
        public string PartitionKey { get; set; }
    }

    public class FindAlertResponse
    {
        public Alert Alert { get; set; }
    }

    public class FindAlertHandler : IRequestHandler<FindAlertCommand, FindAlertResponse>
    {
        private readonly ILogger _logger;
        private readonly IAlertRepository _repo;

        public FindAlertHandler(ILogger logger, ICosmosDbTestConfiguration config)
        {
            _logger = logger;
            _repo = new AlertRepository(_logger, config.GetDocumentConfiguration("Alert")); //repo;
        }
        public async Task<FindAlertResponse> Handle(FindAlertCommand request, CancellationToken cancellationToken)
        {
            var alert = await _repo.FindAsync(request.Id, request.PartitionKey);
            return new FindAlertResponse {Alert = alert};
        }
    }
}
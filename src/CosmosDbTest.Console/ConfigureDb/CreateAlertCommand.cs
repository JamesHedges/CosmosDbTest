using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using CosmosDbTest.DAL;

namespace CosmosDbTest.ConfigureDb.Models
{
    public class CreateAlertCommand : IRequest<CreateAlertResponse>
    {
        public Guid AlertId { get; set; }
        public Guid BatchId { get; set; }
    }

    public class CreateAlertResponse
    {
        public Guid AlertId { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class CreateAlertHandler : IRequestHandler<CreateAlertCommand, CreateAlertResponse>
    {
        private readonly ILogger _logger;
        private readonly ICosmosDbTestConfiguration _configuration;
        private readonly IAlertRepository _repo;

        public CreateAlertHandler(ILogger logger, ICosmosDbTestConfiguration configuration, IAlertRepository repo)
        {
            _logger = logger;
            _configuration = configuration;
            _repo = repo;
        }

        public async Task<CreateAlertResponse> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
        {
            var alert = new Alert
            {
                AlertId = request.AlertId,
                BatchId = request.BatchId,
                StatusCode = "new",
                StatusCodeDescription = "New Alert",
                CreateDateUtc = DateTime.UtcNow,
                UpdateDateUtc = DateTime.UtcNow,
                Active = true
            };
            await _repo.CreateAlertAsync(alert);
            return new CreateAlertResponse { AlertId = request.AlertId, CreateDate = DateTime.UtcNow };
        }
    }
}

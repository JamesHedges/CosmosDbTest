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
        public Guid AnalystId { get; set; }
    }

    public class CreateAlertResponse
    {
        public Guid AlertId { get; set; }
        public Guid AnalystId { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class CreateAlertHandler : IRequestHandler<CreateAlertCommand, CreateAlertResponse>
    {
        private readonly ILogger _logger;
        private readonly IAlertRepository _repo;

        public CreateAlertHandler(ILogger logger, ICosmosDbTestConfiguration config)
        //public CreateAlertHandler(ILogger logger, IDocumentConfiguration alertRepositoryConfig)
        {
            _logger = logger;
            _repo = new AlertRepository(_logger, config.GetDocumentConfiguration("Alert")); //repo;
        }

        public async Task<CreateAlertResponse> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
        {
            var alert = new Alert
            {
                AlertId = request.AlertId,
                AnalystId = request.AnalystId,
                BatchId = request.BatchId,
                StatusCode = "new",
                StatusCodeDescription = "New Alert",
                CreateDateUtc = DateTime.UtcNow,
                UpdateDateUtc = DateTime.UtcNow,
                Active = true
            };
            await _repo.CreateAsync(alert, alert.AlertId.ToString());
            return new CreateAlertResponse { AlertId = request.AlertId, CreateDate = DateTime.UtcNow };
        }
    }
}

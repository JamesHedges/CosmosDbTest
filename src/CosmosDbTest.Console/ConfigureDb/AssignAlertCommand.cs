using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDbTest.DAL;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CosmosDbTest.ConfigureDb.Models
{
    public class AssignAlertCommand : IRequest<AssignAlertResponse>
    {
        public Guid AlertId { get; set; }
        public Guid OldAnalystId { get; set; }
        public Guid NewAnalystId { get; set; }
    }

    public class AssignAlertResponse
    {
        public Guid AlertId { get; set; }
        public Guid AnalystId { get; set; }
        public DateTime UpdateDate { get; set; }

    }

    public class AssignAlertHandler : IRequestHandler<AssignAlertCommand, AssignAlertResponse>
    {
        private readonly ILogger _logger;
        private readonly IAlertRepository _repo;

        public AssignAlertHandler(ILogger logger, ICosmosDbTestConfiguration config)
        {
            _logger = logger;
            _repo = new AlertRepository(_logger, config.GetDocumentConfiguration("Alert")); //repo;
        }
        public async Task<AssignAlertResponse> Handle(AssignAlertCommand request, CancellationToken cancellationToken)
        {
            var alerts = await _repo.FindAsync((a => a.AnalystId == request.OldAnalystId && a.AlertId == request.AlertId), request.OldAnalystId.ToString());
            var alert = alerts.FirstOrDefault();
            if (alert != null)
            {
                var oldAnalystId = alert.AnalystId.ToString();
                var oldId = alert.Id.ToString();
                alert.AnalystId = request.NewAnalystId;
                alert.UpdateDateUtc = DateTime.UtcNow;
                alert.Id = Guid.NewGuid();
                await _repo.SaveAsync(alert, alert.AnalystId.ToString());
                await _repo.DeleteAsync(oldId, oldAnalystId); //(alert.Id.ToString()); //request.OldAnalystId.ToString());
                return new AssignAlertResponse
                    {AlertId = alert.AlertId, AnalystId = alert.AnalystId, UpdateDate = alert.UpdateDateUtc};
            }
            else
            {
                throw new Exception("Alert not found");
            }
        }
    }
}
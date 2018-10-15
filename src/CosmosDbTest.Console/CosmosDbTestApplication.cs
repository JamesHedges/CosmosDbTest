using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using McMaster.Extensions.CommandLineUtils;
using CosmosDbTest.ConfigureDb.Models;

namespace CosmosDbTest
{
    public class CosmosDbTestApplication
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        //private readonly ICosmosDbTestConfiguration _configuration;

        public CosmosDbTestApplication(ILogger logger, IMediator mediator) //, ICosmosDbTestConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            //_configuration = configuration;
        }

        [Argument(0, "Command To Execute", "Command")]
        public string Command { get; set; }

        [Option("-A|--Alert")]
        public string AlertId { get; set; }

        [Option("-O|--Old-AnalystId")]
        public string OldAnalystId { get; set; }

        [Option("-N|--New-AnalystId")]
        public string NewAnalystId { get; set; }

        [Option("-I|--Id")]
        public string Id { get; set; }

        public async Task OnExecute()
        {
            switch (Command.ToLower())
            {
                case "configuredb":
                    await _mediator.Send(new ConfigureDbCommand());
                    break;
                case "addalert":
                    await _mediator.Send(new CreateAlertCommand {AlertId = Guid.NewGuid(), BatchId = Guid.NewGuid(), AnalystId = new Guid(NewAnalystId)});
                    break;
                case "assignalert":
                    await _mediator.Send(new AssignAlertCommand {AlertId = new Guid(AlertId), OldAnalystId = new Guid(OldAnalystId), NewAnalystId = new Guid(NewAnalystId) });
                    break;
                case "showalert":
                    var response = await _mediator.Send(new FindAlertCommand {Id = Id, PartitionKey = OldAnalystId});
                    _logger.LogInformation($"AlertId: {response.Alert.AlertId}, AnalystId: {response.Alert.AnalystId}");
                    break;
                default:
                    _logger.LogError($"Invalid Command: {Command}");
                    break;
            }

            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }

    }
}

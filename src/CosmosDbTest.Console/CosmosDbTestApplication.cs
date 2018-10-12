using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MediatR;
using McMaster.Extensions.CommandLineUtils;
using CosmosDbTest.ConfigureDb;

namespace CosmosDbTest
{
    public class CosmosDbTestApplication
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly ICosmosDbTestConfiguration _configuration;

        public CosmosDbTestApplication(ILogger logger, IMediator mediator, ICosmosDbTestConfiguration configuration)
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
        }

        [Argument(0, "Command To Execute", "Command")]
        public string Command { get; set; }

        public async Task OnExecute()
        {
            switch (Command.ToLower())
            {
                case "configuredb":
                    await _mediator.Send(new ConfigureDbCommand());
                    break;
                case "addalert":
                    await _mediator.Send(new SaveAlertCommand());
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

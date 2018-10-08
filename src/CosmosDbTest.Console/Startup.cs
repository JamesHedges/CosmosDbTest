using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;

namespace CosmosDbTest
{
    public class Startup
    {
        public IServiceProvider ConfigureService(IServiceCollection services)
        {
            services.AddScoped<ServiceFactory>(p => p.GetService);

            services.AddLogging(c => c.AddConsole());
            services.AddTransient<ILogger>(p => p.GetRequiredService<ILoggerFactory>().CreateLogger("Console"));
            services.AddSingleton<ICosmosDbTestConfiguration>(new CosmosDbTestConfiguration());

            services.Scan(scan =>
            {
                scan
                    .FromAssembliesOf(typeof(IMediator), typeof(ICosmosDbTestConfiguration))
                    .AddClasses(true)
                    .AsImplementedInterfaces();
            });

            return services.BuildServiceProvider();
        }

        public CommandLineApplication<CosmosDbTestApplication> Configure(IServiceProvider provider)
        {
            var app = new CommandLineApplication<CosmosDbTestApplication>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(provider);
            return app;
        }
    }
}

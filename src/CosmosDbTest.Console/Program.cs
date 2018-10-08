using System;
using Microsoft.Extensions.DependencyInjection;

namespace CosmosDbTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Startup startup = new Startup();
            IServiceProvider provider = startup.ConfigureService(new ServiceCollection());
            var app = startup.Configure(provider);
            app.Execute(args);
        }
    }
}

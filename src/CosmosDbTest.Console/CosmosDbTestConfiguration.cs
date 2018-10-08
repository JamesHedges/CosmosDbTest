using System.Security;
using Microsoft.Extensions.Configuration;

namespace CosmosDbTest
{
    public interface ICosmosDbTestConfiguration
    {
        string DatabaseEndPointUrl { get; }
        SecureString AuthorizationKey { get; }
    }

    public class CosmosDbTestConfiguration : ICosmosDbTestConfiguration
    {
        private readonly IConfiguration _configuration;

        public CosmosDbTestConfiguration()
        {
            _configuration = LoadConfiguration();
        }

        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("applicationsettings.json", false, true);
            return builder.Build();
        }

        public string DatabaseEndPointUrl => _configuration?.GetValue<string>("appsettings:databaseEndPointUrl");
        public SecureString AuthorizationKey => CreateSecureString(_configuration?.GetValue<string>("appsettings:authorizationKey"));

        private SecureString CreateSecureString(string key)
        {
            var secureString = new SecureString();
            foreach (char ch in key)
            {
                secureString.AppendChar(ch);
            }
            return secureString;
        }
    }
}

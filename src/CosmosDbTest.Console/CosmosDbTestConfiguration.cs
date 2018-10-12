using System.Collections.Generic;
using System.Linq;
using System.Security;
using Microsoft.Extensions.Configuration;

namespace CosmosDbTest
{
    public interface ICosmosDbTestConfiguration
    {
        string DatabaseEndPointUrl { get; }
        SecureString AuthorizationKey { get; }
        string DatabaseName { get; }
        List<CollectionDescription> CollectionDescriptions { get; }
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

        public SecureString AuthorizationKey =>
            CreateSecureString(_configuration?.GetValue<string>("appsettings:authorizationKey"));

        public string DatabaseName => _configuration.GetValue<string>("appsettings:databaseName");

        public List<CollectionDescription>  CollectionDescriptions
        {
            get
            {
                var collections = _configuration.GetValue<string>("appsettings:collections").Split("|");
                return collections.Select(c =>
                {
                    var e = c.Split(",");
                    return new CollectionDescription {Name = e[0], PartitionKey = e[1]};
                }).ToList();
            }}

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

    public class CollectionDescription
    {
        public string Name { get; set; }
        public string PartitionKey { get; set; }
    }
}

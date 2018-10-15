using System.Security;

namespace CosmosDbTest.DAL
{
    public class DocumentConfiguration : IDocumentConfiguration
    {
        public DocumentConfiguration(string databaseEndpoint, string authorization, string databaseId, string collectionId)
        {
            DatabaseEndPoint = databaseEndpoint;
            AuthorizationKey = authorization.SecureString();
            DatabaseId = databaseId;
            CollectionId = collectionId;
        }

        public string DatabaseEndPoint { get; }
        public SecureString AuthorizationKey { get; }
        public string DatabaseId { get; }
        public string CollectionId { get; }
    }
}
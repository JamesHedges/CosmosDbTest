using System.Security;

namespace CosmosDbTest.DAL
{
    public class DocumentConfiguration : IDocumentConfiguration
    {
        public DocumentConfiguration(string databaseEndpoint, string authorization, string databaseId, string collectionId) : this(databaseEndpoint, authorization.SecureString(), databaseId, collectionId)
        {}

        public DocumentConfiguration(string databaseEndpoint, SecureString authorizationKey, string databaseId, string collectionId)
        {
            DatabaseEndPoint = databaseEndpoint;
            AuthorizationKey = authorizationKey;
            DatabaseId = databaseId;
            CollectionId = collectionId;
        }

        public string DatabaseEndPoint { get; }
        public SecureString AuthorizationKey { get; }
        public string DatabaseId { get; }
        public string CollectionId { get; }
    }
}
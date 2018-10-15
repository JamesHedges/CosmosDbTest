using System.Security;

namespace CosmosDbTest.DAL
{
    public interface IDocumentConfiguration
    {
        string DatabaseEndPoint { get; }
        SecureString AuthorizationKey { get; }
        string DatabaseId { get; }
        string CollectionId { get; }
    }
}
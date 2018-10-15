using System.Threading.Tasks;
using CosmosDbTest.ConfigureDb.Models;

namespace CosmosDbTest.DAL
{
    public interface IAlertRepository
    {
        Task CreateAlertAsync(Alert alert);
    }
}
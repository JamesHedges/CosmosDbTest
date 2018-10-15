using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CosmosDbTest.ConfigureDb.Models;

namespace CosmosDbTest.DAL
{
    public interface IAlertRepository
    {
        Task<Alert> FindAsync(string Id, string partitionId);
        Task<IEnumerable<Alert>> FindAsync(Expression<Func<Alert, bool>> predicate, string partitionId);
        Task SaveAsync(Alert alert, string analystId);
        Task CreateAsync(Alert alert, string analystId);
        Task UpdateAsync(Alert alert, string analystId);
        Task DeleteAsync(string analystId, string partitionId);

    }
}
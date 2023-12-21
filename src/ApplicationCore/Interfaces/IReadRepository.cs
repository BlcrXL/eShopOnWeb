using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Specification;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
    Task<IEnumerable<TOut>> LoadFromSql<TOut>(string sql, object parameters);
}

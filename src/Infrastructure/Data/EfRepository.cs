using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Specification.EntityFrameworkCore;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Infrastructure.Data;

public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
    private readonly CatalogContext _catalogContext;

    public EfRepository(CatalogContext dbContext) : base(dbContext)
    {
        _catalogContext = dbContext;
    }

    public async Task<IEnumerable<TOut>> LoadFromSql<TOut>(string sql, object parameters)
    {
        var conn = _catalogContext.Database.GetDbConnection();
        return await conn.QueryAsync<TOut>(sql, parameters);
    }
}

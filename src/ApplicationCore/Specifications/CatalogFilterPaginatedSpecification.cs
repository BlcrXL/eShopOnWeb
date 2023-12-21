using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications;

public class CatalogFilterPaginatedSpecification : Specification<CatalogItem>
{
    private bool _specificationApplied;

#pragma warning disable CS8603
    // todo: somehow items querying works when Query should be null, everything is returned
    public override ISpecificationBuilder<CatalogItem> Query => _specificationApplied ? base.Query : null;
#pragma warning restore CS8603

    public CatalogFilterPaginatedSpecification(CatalogItemQuery query)
        : base()
    {
        _specificationApplied = string.IsNullOrWhiteSpace(query.ItemName);
        if (!_specificationApplied)
        {
            return;
        }

        ApplyFilter(query);
    }

    public string GetItemIndexSql(CatalogItemQuery query)
    {
        var sql = @"
--declare @BrandId int, @TypeId int, @SpecialBrandId int = 0, @OrderByApplied int = 2, @ItemName nvarchar(128) = 'O208'
select top 1 rn Item1
    -- , Id, [Name], @OrderByApplied OrderByApplied
from(
    select row_number() over(order by
            /*order_by_start*/case when @OrderByApplied = 1/*NameDescending*/ then [Name] else Id/*RecentlyAdded*/ end desc/*order_by_end*/
        ) rn
        , [Name] --, Id
            from [Catalog]

    where
            (@BrandId is null or CatalogBrandId = @BrandId)
        and(@TypeId is null or CatalogTypeId = @TypeId)
        and(@SpecialBrandId = 0 or CatalogBrandId<> @SpecialBrandId)
) a where [Name] /*order_by_start2*/ <= /*order_by_end2*/ @ItemName order by rn desc
";
        sql = query.OrderByApplied == CatalogItemOrderBy.NameAscending ? 
            Utilities.ReplaceBetween(sql, "/*order_by_start*/", "/*order_by_end*/", "[Name]") : sql;
        if (query.OrderByApplied != CatalogItemOrderBy.NameDescending)
            sql = Utilities.ReplaceBetween(sql, "/*order_by_start2*/", "/*order_by_end2*/", query.OrderByApplied == CatalogItemOrderBy.NameAscending ? " >= " : " = ");
        return sql;
    }

    public void ApplyFilter(CatalogItemQuery query)
    {
        _specificationApplied = true;
        var take = query.ItemsPage == 0 ? int.MaxValue : query.ItemsPage;
        Query
            .Where(i => (!query.BrandId.HasValue || i.CatalogBrandId == query.BrandId)
                && (!query.TypeId.HasValue || i.CatalogTypeId == query.TypeId)
                && (query.SpecialBrandId == 0 || i.CatalogBrandId != query.SpecialBrandId)
            );
        switch (query.OrderByApplied)
        {
            case CatalogItemOrderBy.NameAscending: Query.OrderBy(i => i.Name); break;
            case CatalogItemOrderBy.NameDescending: Query.OrderByDescending(i => i.Name); break;
            default: Query.OrderByDescending(i => i.Id); break;
        }

        Query.Skip(query.ItemsPage * query.PageIndex).Take(take);
    }
}

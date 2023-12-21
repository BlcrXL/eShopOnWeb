using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications;

public class CatalogItemQuery
{
    public int PageIndex { get; set; }
    public int ItemsPage { get; set; }
    public int SpecialBrandId { get; set; }
    public int? BrandId { get; set; }
    public int? TypeId { get; set; }
    public CatalogItemOrderBy? OrderByApplied { get; set; }
    public string? ItemName { get; set; }
}

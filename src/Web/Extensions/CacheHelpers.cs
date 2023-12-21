using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.eShopWeb.Infrastructure;

namespace Microsoft.eShopWeb.Web.Extensions;

public static class CacheHelpers
{
    public static TimeSpan DefaultCacheDuration = TimeSpan.FromSeconds(30);
    private static readonly string _itemsKeyTemplate = "items-{0}-{1}-{2}-{3}-{4}-{5}-{6}";

    public static string GenerateCatalogItemCacheKey(int pageIndex, int itemsPage, int? brandId, int? typeId, CatalogItemOrderBy? orderBy, string? itemName, InflFetMode mode)
    {
        return string.Format(_itemsKeyTemplate, pageIndex, itemsPage, brandId, typeId, orderBy, itemName, mode);
    }

    public static string GenerateBrandsCacheKey(InflFetMode mode)
    {
        return $"brands-{mode}";
    }

    public static string GenerateTypesCacheKey()
    {
        return "types";
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Infrastructure;
using Microsoft.eShopWeb.Web.Extensions;
using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace Microsoft.eShopWeb.Web.Services;

public class CachedCatalogViewModelService : ICatalogViewModelService
{
    private readonly IMemoryCache _cache;
    private readonly CatalogViewModelService _catalogViewModelService;
    private readonly IHttpContextAccessor _accessor;

    public CachedCatalogViewModelService(IMemoryCache cache,
        CatalogViewModelService catalogViewModelService,
        IHttpContextAccessor accessor)
    {
        _cache = cache;
        _catalogViewModelService = catalogViewModelService;
        _accessor = accessor;        
    }

    public async Task<IEnumerable<SelectListItem>> GetBrands()
    {
        return (await _cache.GetOrCreateAsync(CacheHelpers.GenerateBrandsCacheKey(Utils.GetInflFetMode(_accessor!.HttpContext!.Request)), async entry =>
                {
                    entry.SlidingExpiration = CacheHelpers.DefaultCacheDuration;
                    return await _catalogViewModelService.GetBrands();
                })) ?? new List<SelectListItem>();
    }

    public async Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId, CatalogItemOrderBy? orderBy)
    {
        var cacheKey = CacheHelpers.GenerateCatalogItemCacheKey(pageIndex, Constants.ITEMS_PER_PAGE, brandId, typeId, orderBy, null, Utils.GetInflFetMode(_accessor!.HttpContext!.Request));

        return (await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SlidingExpiration = CacheHelpers.DefaultCacheDuration;
            return await _catalogViewModelService.GetCatalogItems(pageIndex, itemsPage, brandId, typeId, orderBy);
        })) ?? new CatalogIndexViewModel();
    }
    public async Task<CatalogIndexViewModel> GetCatalogItems(CatalogItemQuery request)
    {
        var cacheKey = CacheHelpers.GenerateCatalogItemCacheKey(request.PageIndex, Constants.ITEMS_PER_PAGE, request.BrandId, request.TypeId, request.OrderByApplied, request.ItemName, Utils.GetInflFetMode(_accessor!.HttpContext!.Request));

        return (await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SlidingExpiration = CacheHelpers.DefaultCacheDuration;
            return await _catalogViewModelService.GetCatalogItems(request);
        })) ?? new CatalogIndexViewModel();
    }

    public async Task<IEnumerable<SelectListItem>> GetTypes()
    {
        return (await _cache.GetOrCreateAsync(CacheHelpers.GenerateTypesCacheKey(), async entry =>
        {
            entry.SlidingExpiration = CacheHelpers.DefaultCacheDuration;
            return await _catalogViewModelService.GetTypes();
        })) ?? new List<SelectListItem>();
    }
}

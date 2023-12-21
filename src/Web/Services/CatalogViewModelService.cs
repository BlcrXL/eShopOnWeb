using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Infrastructure;
using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopWeb.Web.Services;

/// <summary>
/// This is a UI-specific service so belongs in UI project. It does not contain any business logic and works
/// with UI-specific types (view models and SelectListItem types).
/// </summary>
public class CatalogViewModelService : ICatalogViewModelService
{
    private readonly ILogger<CatalogViewModelService> _logger;
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IRepository<CatalogBrand> _brandRepository;
    private readonly IRepository<CatalogType> _typeRepository;
    private readonly IUriComposer _uriComposer;
    private readonly CatalogSettings _catalogSettings;
    private readonly IHttpContextAccessor _accessor;

    public CatalogViewModelService(
        ILoggerFactory loggerFactory,
        IRepository<CatalogItem> itemRepository,
        IRepository<CatalogBrand> brandRepository,
        IRepository<CatalogType> typeRepository,
        IUriComposer uriComposer,
        IOptions<CatalogSettings> catalogSettings,
        IHttpContextAccessor accessor)
    {
        _logger = loggerFactory.CreateLogger<CatalogViewModelService>();
        _itemRepository = itemRepository;
        _brandRepository = brandRepository;
        _typeRepository = typeRepository;
        _uriComposer = uriComposer;
        _catalogSettings = catalogSettings.Value;
        _accessor = accessor;
    }

    public Task<CatalogIndexViewModel> GetCatalogItems(int pageIndex, int itemsPage, int? brandId, int? typeId, CatalogItemOrderBy? orderByApplied)
        => GetCatalogItems(new CatalogItemQuery { BrandId = brandId, ItemsPage = itemsPage, OrderByApplied = orderByApplied, PageIndex = pageIndex, TypeId = typeId });

    public async Task<CatalogIndexViewModel> GetCatalogItems(CatalogItemQuery request)
    {
        _logger.LogInformation("GetCatalogItems called.");
        request.SpecialBrandId = GetSpecialBrandId();
        var filterSpecification = new CatalogFilterSpecification(request.BrandId, request.TypeId);
        var filterPaginatedSpecification =
            new CatalogFilterPaginatedSpecification(request);

        if (!string.IsNullOrWhiteSpace(request.ItemName))
        {
            var itemIndex = (await ((IReadRepository<CatalogItem>)_itemRepository).LoadFromSql<Tuple<long>?>(filterPaginatedSpecification.GetItemIndexSql(request), request)).FirstOrDefault();
            if (itemIndex != null)
            {
                request.PageIndex = (int)itemIndex.Item1 / request.ItemsPage;
            }
            request.ItemName = null;
            filterPaginatedSpecification.ApplyFilter(request);
        }
        // the implementation below using ForEach and Count. We need a List.
        var itemsOnPage = await _itemRepository.ListAsync(filterPaginatedSpecification);
        var totalItems = await _itemRepository.CountAsync(filterSpecification);

        int? brandIdOverride = Utils.GetAndUpdateInflFetMode(_accessor.HttpContext!.Request, _accessor.HttpContext.Response) == InflFetMode.None ? 0 : null;
        var vm = new CatalogIndexViewModel()
        {
            CatalogItems = itemsOnPage.Select(i => new CatalogItemViewModel()
            {
                Id = i.Id,
                Name = i.Name,
                PictureUri = _uriComposer.ComposePicUri(i.PictureUri, i.Id, brandIdOverride ?? i.CatalogBrandId),
                Price = i.Price
            }).ToList(),
            Brands = (await GetBrands()).ToList(),
            Types = (await GetTypes()).ToList(),
            OrderBy = new List<SelectListItem> {
                new SelectListItem("By Name", nameof(CatalogItemOrderBy.NameAscending)),
                new SelectListItem("By name descending", nameof(CatalogItemOrderBy.NameDescending)),
                new SelectListItem("Recently added first", nameof(CatalogItemOrderBy.RecentlyAdded))
            },
            BrandFilterApplied = request.BrandId ?? 0,
            TypesFilterApplied = request.TypeId ?? 0,
            PaginationInfo = new PaginationInfoViewModel()
            {
                ActualPage = request.PageIndex,
                ItemsPerPage = itemsOnPage.Count,
                TotalItems = totalItems,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / request.ItemsPage)).ToString())
            }
        };

        vm.PaginationInfo.Next = (vm.PaginationInfo.ActualPage == vm.PaginationInfo.TotalPages - 1) ? "is-disabled" : "";
        vm.PaginationInfo.Previous = (vm.PaginationInfo.ActualPage == 0) ? "is-disabled" : "";

        return vm;
    }

    public async Task<IEnumerable<SelectListItem>> GetBrands()
    {
        _logger.LogInformation("GetBrands called.");
        var brands = await _brandRepository.ListAsync();
        var specialBrandId = GetSpecialBrandId();
        var items = brands
            .Where(b => specialBrandId == 0 || b.Id != specialBrandId)
            .Select(brand => new SelectListItem() { Value = brand.Id.ToString(), Text = brand.Brand })
            .OrderBy(b => b.Text)
            .ToList();

        var allItem = new SelectListItem() { Value = null, Text = "All", Selected = true };
        items.Insert(0, allItem);

        return items;
    }

    public async Task<IEnumerable<SelectListItem>> GetTypes()
    {
        _logger.LogInformation("GetTypes called.");
        var types = await _typeRepository.ListAsync();

        var items = types
            .Select(type => new SelectListItem() { Value = type.Id.ToString(), Text = type.Type })
            .OrderBy(t => t.Text)
            .ToList();

        var allItem = new SelectListItem() { Value = null, Text = "All", Selected = true };
        items.Insert(0, allItem);

        return items;
    }

    private int GetSpecialBrandId()
    {
        var httpContext = _accessor.HttpContext;
        return (httpContext != null && Utils.GetAndUpdateInflFetMode(httpContext.Request, httpContext.Response) != InflFetMode.Special)
            ? _catalogSettings.SpecialBrandId : 0;
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.Web.Services;
using Microsoft.eShopWeb.Web.ViewModels;

namespace Microsoft.eShopWeb.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ICatalogViewModelService _catalogViewModelService;

    public IndexModel(ICatalogViewModelService catalogViewModelService)
    {
        _catalogViewModelService = catalogViewModelService;
    }

    public required CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();

    public async Task OnGet(CatalogIndexViewModel catalogModel, int? pageId, string itemName)
    {
        CatalogModel = await _catalogViewModelService.GetCatalogItems(new CatalogItemQuery
        {
            PageIndex = pageId ?? 0,
            ItemsPage = Constants.ITEMS_PER_PAGE,
            BrandId = catalogModel.BrandFilterApplied,
            TypeId = catalogModel.TypesFilterApplied,
            OrderByApplied = catalogModel.OrderByApplied,
            ItemName = itemName
        });
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.eShopWeb.ApplicationCore;

namespace Microsoft.eShopWeb.Web.ViewModels;

public class CatalogIndexViewModel
{
    public List<CatalogItemViewModel> CatalogItems { get; set; } = new List<CatalogItemViewModel>();
    public List<SelectListItem>? Brands { get; set; } = new List<SelectListItem>();
    public List<SelectListItem>? Types { get; set; } = new List<SelectListItem>();
    public List<SelectListItem>? OrderBy { get; set; } = new List<SelectListItem>();
    public int? BrandFilterApplied { get; set; }
    public int? TypesFilterApplied { get; set; }
    public CatalogItemOrderBy? OrderByApplied { get; set; }
    public PaginationInfoViewModel? PaginationInfo { get; set; }
}

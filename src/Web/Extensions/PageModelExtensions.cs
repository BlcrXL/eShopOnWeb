using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.eShopWeb.Infrastructure;

namespace Microsoft.eShopWeb.Web.Extensions;

public static class PageModelExtensions
{
    public static InflFetMode GetAndUpdateInflFetMode(this PageModel pageModel) => Utils.GetAndUpdateInflFetMode(pageModel.Request, pageModel.Response);

    public static string GetAndUpdateInflFetSuffix(this PageModel pageModel) => GetAndUpdateInflFetMode(pageModel) != InflFetMode.None ? "_if" : "";
}

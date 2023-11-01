using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace Microsoft.eShopWeb.Web.Extensions;

public static class PageModelExtensions
{
    private const string InflFetModeKey = "ifxl";

    public static bool IsInflFetMode(this PageModel pageModel)
    {
        if (pageModel.Request.Query[InflFetModeKey] != default(StringValues))
        {
            if (pageModel.Request.Cookies[InflFetModeKey] == null)
            {
                pageModel.Response.Cookies.Append(InflFetModeKey, "true", new CookieOptions { Expires = DateTime.UtcNow.AddYears(10) });
            }
            return true;
        }
        return pageModel.Request.Cookies[InflFetModeKey] != null;
    }
}

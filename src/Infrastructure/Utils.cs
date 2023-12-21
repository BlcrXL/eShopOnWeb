using System;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.Extensions.Primitives;

namespace Microsoft.eShopWeb.Infrastructure;
public class Utils
{
    private const string InflFetModeKey = "ifxl";
    private const string ResetInflFetModeKey = "ifxlno";
    private const string InflFetSpecialMode = "special";

    public static InflFetMode GetInflFetMode(HttpRequest req) {
        if (req.Query[ResetInflFetModeKey] != default(StringValues))
        {
            return InflFetMode.None;
        }
        var s = req.Cookies[InflFetModeKey] ?? req.Query[ResetInflFetModeKey];
        return s == InflFetSpecialMode ? InflFetMode.Special : InflFetMode.Common;
    }

    public static InflFetMode GetAndUpdateInflFetMode(HttpRequest req, HttpResponse resp)
    {
        if (req.Query[ResetInflFetModeKey] != default(StringValues))
        {
            resp.Cookies.Delete(InflFetModeKey);
            return InflFetMode.None;
        }

        var values = req.Query[InflFetModeKey];
        if (values != default(StringValues))
        {
            var specialMode = values == InflFetSpecialMode;
            if (req.Cookies[InflFetModeKey] == null)
            {
                resp.Cookies.Append(InflFetModeKey, specialMode ? InflFetSpecialMode : "true", new CookieOptions { Expires = DateTime.UtcNow.AddYears(10) });
            }
            else
            {
                if (req.Cookies[InflFetModeKey] != values)
                {
                    resp.Cookies.Delete(InflFetModeKey);

                }
            }
            return specialMode ? InflFetMode.Special : InflFetMode.Common;
        }
        return req.Cookies[InflFetModeKey] == InflFetSpecialMode ? InflFetMode.Special : InflFetMode.Common;
    }

}

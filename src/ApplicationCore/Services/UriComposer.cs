using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class UriComposer : IUriComposer
{
    private readonly CatalogSettings _catalogSettings;

    public UriComposer(CatalogSettings catalogSettings) => _catalogSettings = catalogSettings;

    public string ComposePicUri(string uriTemplate, int itemId, int brandId)
    {
        string? url = "";
        if ((!_catalogSettings!.SpecialBrandPictureUrls?.TryGetValue(brandId, out url!)).GetValueOrDefault())
            url = _catalogSettings!.PictureBaseUrl;
        return brandId == 0 ? $"{url}/{itemId % 12 + 1}.png" : uriTemplate.Replace("catalogbaseurltobereplaced", url);
    }
}

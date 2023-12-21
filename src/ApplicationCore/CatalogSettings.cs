using System.Collections.Generic;

namespace Microsoft.eShopWeb;

public class CatalogSettings
{
    public string? CatalogBaseUrl { get; set; }

    public string? PictureBaseUrl { get; set; }

    public bool DoMigration { get; set; }

    public string? ReCaptchaPublicKey { get; set; }

    public string? ReCaptchaPrivateKey { get; set; }

    public string? SpecialBrandName { get; set; }

    public int SpecialBrandId { get; set; }

    public Dictionary<int, string>? SpecialBrandPictureUrls { get; set; }
}

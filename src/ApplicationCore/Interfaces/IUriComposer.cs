﻿namespace Microsoft.eShopWeb.ApplicationCore.Interfaces;

public interface IUriComposer
{
    string ComposePicUri(string uriTemplate, int itemId, int brandId);
}

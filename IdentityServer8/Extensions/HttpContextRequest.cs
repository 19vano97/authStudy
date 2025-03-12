using System;
using IdentityServer8.Models;

namespace IdentityServer8.Extensions;

public static class HttpContextRequest
{
    public static string GetReturnUrl(this HttpRequest request)
    {
        var returnUrl = request.Query.FirstOrDefault(p => p.Key.Equals(Constants.IdentityCustomOpenId.LoginPage.RETURN_URL, StringComparison.OrdinalIgnoreCase)).Value;
        
        return returnUrl;
    }
}

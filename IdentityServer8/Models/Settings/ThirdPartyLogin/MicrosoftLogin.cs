using System;
using IdentityServer4.Models;

namespace IdentityServer8.Models.Settings.ThirdPartyLogin;

public class MicrosoftLogin
{
    public JwtMsLogin JwtMsLogin { get; set; }
    public OidcMsLogin OidcMsLogin { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string TenantId { get; set; }
}

public class JwtMsLogin
{
    public string Name { get; set; }
    public string Authority { get; set; }
    public string Audience { get; set; }
}

public class OidcMsLogin
{
    public string Name { get; set; }
    public string SignInScheme { get; set; }
    public string Resource { get; set; }
    public string Authority { get; set; }
    public string ResponseType { get; set; }
    public string CallbackPath { get; set; }
    public bool SaveTokens { get; set; }
}
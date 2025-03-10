using System;

namespace IdentityServer8.Models.Settings;

public class ClientConfig
{
    public string ClientId { get; set; }
    public List<string> AllowedGrantTypes { get; set; }
    public bool RequireClientSecret { get; set; }
    public List<string> RedirectUris { get; set; }
    public List<string> PostLogoutRedirectUris { get; set; }
    public List<string> AllowedScopes { get; set; }
    public bool AllowOfflineAccess { get; set; }
    public bool RequirePkce { get; set; }
}

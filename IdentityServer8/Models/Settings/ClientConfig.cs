using System;
using IdentityServer4.Models;

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
    public int AccessTokenLifetime { get; set; }
    public int AbsoluteRefreshTokenLifetime { get; set; }
    public TokenUsage RefreshTokenUsage { get; set; }
}

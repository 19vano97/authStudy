using System;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using ApiScope = IdentityServer4.Models.ApiScope;
using Client = IdentityServer4.Models.Client;
using IdentityResource = IdentityServer4.Models.IdentityResource;

namespace IdentityServer8.Configs;

public class IdentityServerConfig : IClientStore
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("api.read", "Read Access to API"),
            new ApiScope("api.write", "Write Access to API")
        };

    public static IEnumerable<Client> Clients =>
    new List<Client>
    {
        new Client
        {
            ClientId = "react-client",
            AllowedGrantTypes = GrantTypes.Code,
            RequireClientSecret = false,
            RedirectUris = { "https://localhost:7124/callback", "https://localhost:7124/signin-oidc" },
            PostLogoutRedirectUris = { "https://localhost:7124/" },
            AllowedScopes = { "openid", "profile", "api.read", "offline_access" },
            AllowOfflineAccess = true,
            RequirePkce = true 
        }
    };

    public Task<Client> FindClientByIdAsync(string clientId)
    {
        return Task.FromResult(Clients.FirstOrDefault(c => c.ClientId == clientId)!);
    }
}

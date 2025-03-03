using System;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using ApiScope = IdentityServer4.Models.ApiScope;
using Client = IdentityServer4.Models.Client;
using IdentityResource = IdentityServer4.Models.IdentityResource;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServerTest.Configs;

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
                //ClientSecrets = { new Secret("secret".Sha256()) },
                RedirectUris = { "http://localhost:3000/callback" },
                PostLogoutRedirectUris = { "http://localhost:3000" },
                AllowedScopes = { "openid", "profile", "api.read", "offline_access" },
                AllowOfflineAccess = true,
                RequirePkce = true 
            }
        };

    public Task<Client> FindClientByIdAsync(string clientId)
    {
        return Task.FromResult(Clients.FirstOrDefault(c => c.ClientId == clientId));
    }
}

using System;

namespace IdentityServer8.Models.Settings;

public class IdentityServerSettings
{
    public List<string> IdentityResources { get; set; }
    public List<ApiScopeSettings> ApiScopes { get; set; }
    public List<ClientConfig> Clients { get; set; }
    public List<ApiResourcesConfig> ApiResources { get; set; }
    public string DefaultReturnUri { get; set; }
}

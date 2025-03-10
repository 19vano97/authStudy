using System;

namespace IdentityServer8.Models.Settings;

public class ApiResourcesConfig
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public ICollection<string> Scopes { get; set; }
}

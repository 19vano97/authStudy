using System;

namespace IdentityServer8.Models.ModelViewModels;

public class LogoutViewModel
{
    public string? LogoutId { get; set; }
    public bool ShowLogoutPrompt { get; set; }
}

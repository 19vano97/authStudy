using System;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Account;
using IdentityServer8.Models.ModelViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityServer8.Services.Interfaces;

public interface IAccountService
{
    Task<SignInResult> LoginAsync(LoginViewModel model);
    Task<IdentityResult> RegisterAsync(RegisterViewModel model);
    Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model);
    Task<string> LogoutAsync(LogoutInputModel model);
    Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId);
}

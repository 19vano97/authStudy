using System;
using System.Reflection.Metadata;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models;
using IdentityServer8.Models.Account;
using IdentityServer8.Models.ModelViewModels;
using IdentityServer8.Models.Settings;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityServer8.Services.Implemenrations;

public class AccountService : IAccountService
{
    private readonly SignInManager<Account> _signInManager;
    private readonly UserManager<Account> _userManager;
    private readonly IClientStore _clientStore;
    private readonly IdentityServerSettings _settings;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IAccountHelper _accountValidator;

    public AccountService(SignInManager<Account> signInManager,
                          UserManager<Account> userManager,
                          IClientStore clientStore,
                          IOptions<IdentityServerSettings> settings,
                          IIdentityServerInteractionService interaction,
                          IAccountHelper accountValidator)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _clientStore = clientStore;
        _settings = settings.Value;
        _interaction = interaction;
        _accountValidator = accountValidator;
    }

    public async Task<SignInResult> LoginAsync(LoginViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Username);
        if (user == null)
            return SignInResult.Failed;

        return await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
    }

    public async Task<string> LogoutAsync(string logoutId)
    {
        var logoutRequest = await _interaction.GetLogoutContextAsync(logoutId);
        var postLogoutRedirectUri = !string.IsNullOrEmpty(_settings.DefaultReturnUri)
            ? _settings.DefaultReturnUri
            : "/";

        if (!string.IsNullOrEmpty(logoutRequest?.ClientId))
        {
            var client = await _clientStore.FindClientByIdAsync(logoutRequest.ClientId);

            if (client != null && client.PostLogoutRedirectUris.Any())
                postLogoutRedirectUri = client.PostLogoutRedirectUris.First();
        }

        await _signInManager.SignOutAsync();

        return !string.IsNullOrEmpty(postLogoutRedirectUri) ? postLogoutRedirectUri : "/";
    }

    public async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
    {
        var ctx = await _interaction.GetLogoutContextAsync(logoutId);
        return new LogoutViewModel
        {
            LogoutId = logoutId,
            ShowLogoutPrompt = ctx?.ShowSignoutPrompt ?? true
        };
    }

    public async Task<string> LogoutAsync(LogoutInputModel model)
    {
        await _signInManager.SignOutAsync();

        var ctx = await _interaction.GetLogoutContextAsync(model.LogoutId);
        string postLogoutRedirectUri = _settings.DefaultReturnUri ?? "/";

        if (!string.IsNullOrEmpty(ctx?.ClientId))
        {
            var client = await _clientStore.FindClientByIdAsync(ctx.ClientId);
            if (client?.PostLogoutRedirectUris.Any() == true)
            {
                postLogoutRedirectUri = client.PostLogoutRedirectUris.First();
            }
        }

        return postLogoutRedirectUri;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
    {
        if (await _userManager.FindByEmailAsync(model.Username) != null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = Constants.Statuses.Register.AccountExists.CODE,
                Description = Constants.Statuses.Register.AccountExists.DESCRIPTION
            });
        }

        var account = new Account
        {
            UserName = model.Username,
            Email = model.Username,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await _userManager.CreateAsync(account, model.Password!);

        if (!result.Succeeded)
            return result;

        await _signInManager.SignInAsync(account, isPersistent: false);
        return IdentityResult.Success; ;
    }

    public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Username);
        if (user != null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = Constants.Statuses.Find.ByEmail.NotFound.CODE,
                Description = Constants.Statuses.Find.ByEmail.NotFound.DESCRIPTION
            });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
        if (!result.Succeeded)
            return result;

        await _signInManager.SignInAsync(user, isPersistent: false);
        return IdentityResult.Success;
    }
}
using System;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer8.Entities.Account;
using IdentityServer8.Enums;
using IdentityServer8.Models.Account;
using IdentityServer8.Models.ModelViewModels;
using IdentityServer8.Models.Settings;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IdentityServer8.Services.Implemenrations;

public class AccountService(
        SignInManager<Account> signInManager, 
        UserManager<Account> userManager,
        IClientStore clientStore,
        IOptions<IdentityServerSettings> settings,
        IIdentityServerInteractionService interaction) : IAccountService
{
    public async Task<AccountStatusDto> EmailValidation(EmailValidationViewModel model)
    {
        var user = await GeneralMethods.IsAccountExisted(userManager, email: model.Username);

        if (user is null)
            return GeneralMethods.SetAccountStatusFromAccount();

        return GeneralMethods.SetAccountStatusFromAccount(
                await GeneralMethods.IsAccountExisted(userManager, email: model.Username),
                    AccountStatusEnum.Existed);
    }

    public async Task<AccountStatusDto> Login(LoginViewModel model)
    {
        var user = await GeneralMethods.IsAccountExisted(userManager, email: model.Username);

        if (user is null)
            return GeneralMethods.SetAccountStatusFromAccount();

        var loginProcess = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if (!loginProcess.Succeeded)
            return GeneralMethods.SetAccountStatusFromAccount(user, AccountStatusEnum.IssueWithLogin);

        return GeneralMethods.SetAccountStatusFromAccount(user, AccountStatusEnum.Existed);
    }

    public async Task<string> Logout(string logoutId)
    {
        var logoutRequest = await interaction.GetLogoutContextAsync(logoutId);
        var postLogoutRedirectUri = settings.Value.DefaultReturnUri;

        if (!string.IsNullOrEmpty(logoutRequest?.ClientId))
        {
            var client = clientStore.FindClientByIdAsync(logoutRequest.ClientId).Result;
            
            if (client != null && client.PostLogoutRedirectUris.Any())
                postLogoutRedirectUri = client.PostLogoutRedirectUris.First();
        }

        await signInManager.SignOutAsync();

        return postLogoutRedirectUri;
    }

    public async Task<AccountStatusDto> Register(RegisterViewModel model)
    {
        var user = await GeneralMethods.IsAccountExisted(userManager, email: model.Username);

        if (user != null)
            return GeneralMethods.SetAccountStatusFromAccount(user, AccountStatusEnum.Existed);

        var accountToAdd = new Account
        {
            UserName = model.Username,
            Email = model.Username,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await userManager.CreateAsync(accountToAdd, model.Password);

        if (!result.Succeeded)
            return GeneralMethods.SetAccountStatusFromAccount(accountToAdd, AccountStatusEnum.IssueWithLogin);

        await signInManager.SignInAsync(accountToAdd, isPersistent: false);

        return GeneralMethods.SetAccountStatusFromAccount(accountToAdd, AccountStatusEnum.Existed);;
    }

    public async Task<AccountStatusDto> ResetPassword(ResetPasswordViewModel model)
    {
        var user = await GeneralMethods.IsAccountExisted(userManager, email: model.Username);
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, model.Password);
        
        if (!result.Succeeded || user is null)
            return GeneralMethods.SetAccountStatusFromAccount(user, AccountStatusEnum.IssueWithLogin);

        await signInManager.SignInAsync(user, isPersistent: false);
        
        return GeneralMethods.SetAccountStatusFromAccount(user, AccountStatusEnum.Existed);
    }
}

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
        IIdentityServerInteractionService identityServerInteractionService,
        IClientStore clientStore,
        IOptions<IdentityServerSettings> settings) : IAccountService
{
    public async Task<AccountStatusDto> EmailValidation(EmailValidationViewModel model)
    {
        var user = await IsAccountExisted(email: model.Username);

        if (user is null)
            return SetAccountStatusFromAccount();

        return SetAccountStatusFromAccount(await IsAccountExisted(email: model.Username), AccountStatusEnum.Existed);
    }

    public async Task<AccountStatusDto> Login(LoginViewModel model)
    {
        var user = await IsAccountExisted(email: model.Username);

        if (user is null)
            return SetAccountStatusFromAccount();

        var loginProcess = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if (!loginProcess.Succeeded)
            return SetAccountStatusFromAccount(user, AccountStatusEnum.IssueWithLogin);

        return SetAccountStatusFromAccount(user, AccountStatusEnum.Existed);
    }

    public Task<ActionResult> Logout(string logoutId)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountStatusDto> Register(RegisterViewModel model)
    {
        var user = await IsAccountExisted(email: model.Username);

        if (user != null)
            return SetAccountStatusFromAccount(user, AccountStatusEnum.Existed);

        var accountToAdd = new Account
        {
            UserName = model.Username,
            Email = model.Username,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        var result = await userManager.CreateAsync(accountToAdd, model.Password);

        if (!result.Succeeded)
            return SetAccountStatusFromAccount(accountToAdd, AccountStatusEnum.IssueWithLogin);

        await signInManager.SignInAsync(accountToAdd, isPersistent: false);

        return SetAccountStatusFromAccount(accountToAdd, AccountStatusEnum.Existed);;
    }

    public Task<AccountStatusDto> ResetPassword(ResetPasswordViewModel model)
    {
        throw new NotImplementedException();
    }

    private static AccountStatusDto SetAccountStatusFromAccount(Account account = null, 
        AccountStatusEnum accountStatus = AccountStatusEnum.NotExisted)
    {
        if (account is null)
            return new AccountStatusDto
            {
                Account = null,
                AccountStatusEnum = accountStatus
            };

        return new AccountStatusDto
        {
            Account = new AccountDto
            {
                Id = Guid.Parse(account.Id),
                Email = account.Email,
                FirstName = account.FirstName,
                LastName = account.LastName,
                CreateDate = (DateTime)account.CreateDate
            },
            AccountStatusEnum = accountStatus
        };
    }

    public async Task<Account> IsAccountExisted(string id = null, string email = null)
    {
        if (id != null)
            return await userManager.FindByIdAsync(id) ?? null;

        if (email != null)
            return await userManager.FindByEmailAsync(email) ?? null;
            

        return null;
    }
}

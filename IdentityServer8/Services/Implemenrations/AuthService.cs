using System;
using System.Net;
using System.Security.Claims;
using IdentityServer8.Data;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models;
using IdentityServer8.Models.Account;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static IdentityServer8.Models.Constants;

namespace IdentityServer8.Services.Implemenrations;

public class AuthService : IAuthService
{
    private readonly UserManager<Account> _userManager;
    private readonly IdentityServer8DbContext _context;
    private readonly IAccountHelper _accountHelper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<Account> userManager,
                       IdentityServer8DbContext context,
                       IAccountHelper accountHelper,
                       ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _context = context;
        _accountHelper = accountHelper;
        _logger = logger;
    }

    public async Task<AccountDto> GetAccountDetailsByIdAsync(ClaimsPrincipal user, Guid userToFind, CancellationToken cancellationToken)
    {
        var userId = user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)?.Value;

        if (string.IsNullOrEmpty(userId))
            return new AccountDto
            {
                Id = Guid.TryParse(userId, out Guid id) == false ? Guid.Empty : id,
                IsValid = false
            };

        var res = await _accountHelper.FindByEmailOrIdAsync(accountId: userToFind);

        if (res is null)
            return new AccountDto { IsValid = false };

        return _accountHelper.ConvertToDto(res);
    }

    public async Task<AccountDto> GetCurrentAccountDetailsAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        return await GetAccountDetailsByIdAsync(user,
            Guid.Parse(user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)?.Value!), cancellationToken);
    }

    public async Task<AccountDto> InviteNewAccountAsync(AccountDto account, CancellationToken cancellationToken)
    {
        if (account.Email == null)
        {
            return null!;
        }
        string callbackUrl = "https://localhost:7270/";
        var accountCheck = await _accountHelper.FindByEmailOrIdAsync(email: account.Email);

        if (accountCheck is not null)
            return _accountHelper.ConvertToDto(accountCheck);

        var newAccount = new Account
        {
            UserName = account.Email,
            Email = account.Email,
            FirstName = account.FirstName == null ? null : account.FirstName,
            LastName = account.LastName == null ? null : account.LastName
        };

        var createResult = await _userManager.CreateAsync(newAccount);
        if (!createResult.Succeeded)
            return null!;

        var newUser = await _accountHelper.FindByEmailOrIdAsync(email: newAccount.Email);
        var token = await _userManager.GeneratePasswordResetTokenAsync(newAccount);
        var encodedToken = WebUtility.UrlEncode(token);
        string resString = string.Format(callbackUrl + $"Account/CreatePassword?token={encodedToken}&username={newUser.UserName}");
        _logger.LogInformation(resString);
        
        return _accountHelper.ConvertToDto(await _accountHelper.FindByEmailOrIdAsync(email: account.Email));
    }

    public async Task<AccountDto> UpdateAccountDetailsAsync(ClaimsPrincipal user, AccountDto account, CancellationToken cancellationToken)
    {
        var userId = user.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)?.Value;
        if (string.IsNullOrEmpty(userId) || Guid.Parse(userId) != account.Id)
        {
            return new AccountDto
            {
                Id = Guid.TryParse(userId, out Guid id) == false ? Guid.Empty : id,
                IsValid = false
            };
        }

        try
        {
            var accountFromDb = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id.ToString(), cancellationToken);
            if (accountFromDb.FirstName != account.FirstName || account.FirstName != null)
                accountFromDb.FirstName = account.FirstName;
            if (accountFromDb.LastName != account.LastName || account.LastName != null)
                accountFromDb.LastName = account.LastName;

            _context.Update(accountFromDb);
            await _context.SaveChangesAsync();

            return _accountHelper.ConvertToDto(await _accountHelper.FindByEmailOrIdAsync(accountId: (Guid)account.Id));
        }
        catch (System.Exception err)
        {
            return new AccountDto
            {
                Id = Guid.TryParse(userId, out Guid id) == false ? Guid.Empty : id,
                IsValid = false
            };
        }
    }
}

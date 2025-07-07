using System.Security.Claims;
using IdentityServer4.Services;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Account;
using IdentityServer8.Services.Implemenrations;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static IdentityServer8.Models.Constants;

namespace IdentityServer8.Controllers.Api
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("details")]
        public async Task<ActionResult<AccountDto>> GetAccountDetails()
        {
            var userId = User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)?.Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var user = await _authService.GetCurrentAccountDetailsAsync(User);

            if (!user.IsValid)
                return BadRequest();

            return Ok(user);
        }

        [HttpPost("details/accounts")]
        public async Task<ActionResult<List<AccountDto>>> GetAllAccounts(List<Guid> accounts = null)
        {
            var userId = User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)?.Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            if (accounts.Count == 0 || accounts is null)
                return NoContent();

            var accountsResult = new List<AccountDto>();

            foreach (var accountId in accounts)
            {
                var user = await _authService.GetAccountDetailsByIdAsync(User, accountId);

                if (!user.IsValid)
                    return BadRequest();

                accountsResult.Add(user);
            }

            if (accountsResult is null || !accountsResult.Any())
                return NotFound();

            return Ok(accountsResult);
        }

        [HttpPost("details")]
        public async Task<ActionResult<AccountDto>> PostAccountDetails(AccountDto account)
        {
            var userId = User.FindFirst(IdentityCustomOpenId.DetailsFromToken.ACCOUNT_ID)?.Value;
            if (string.IsNullOrEmpty(userId) || Guid.Parse(userId) != account.Id)
            {
                return new AccountDto
                {
                    Id = Guid.TryParse(userId, out Guid id) == false ? Guid.Empty : id,
                    IsValid = false
                };
            }

            var updatedAccount = await _authService.UpdateAccountDetailsAsync(User, account);

            if (!updatedAccount.IsValid)
                return BadRequest();

            return Ok(updatedAccount);
        }

        [HttpPost("invite")]
        public async Task<ActionResult<AccountDto>> PrecreateInvitedAccount(AccountDto account)
        {
            var res = await _authService.InviteNewAccountAsync(account);
            System.Console.WriteLine(res);
            //string.Format(callbackUrl + $"Account/Login?returnUrl=https://localhost:5173/signin-oidc")
            // var encodedToken = WebUtility.UrlEncode(token);
            //string resString = string.Format(callbackUrl + $"Account/CreatePassword?token={encodedToken}&username={newUser.UserName}");

            return Ok(res);
        }
    }
}

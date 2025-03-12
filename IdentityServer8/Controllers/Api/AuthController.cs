using System.Security.Claims;
using IdentityServer4.Services;
using IdentityServer8.Entities.Account;
using IdentityServer8.Models.Account;
using IdentityServer8.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer8.Controllers.Api
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpGet("details")]
        public async Task<ActionResult<AccountDto>> GetAccountDetails()
        {
            var user = await authService.GetCurrentAccountDetails(User);

            if (user.AccountStatusEnum is Enums.AccountStatusEnum.IssueWithLogin
                || user.AccountStatusEnum is Enums.AccountStatusEnum.NotExisted)
                return BadRequest();

            return Ok(user.Account);
        }
    }
}

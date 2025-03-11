using System.Security.Claims;
using IdentityServer4.Services;
using IdentityServer8.Entities.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer8.Controllers.Api
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;
        private readonly IIdentityServerInteractionService _interaction;

        public AuthController(UserManager<Account> userManager, IIdentityServerInteractionService interaction)
        {
            _userManager = userManager;
            _interaction = interaction;
        }
    
        [HttpGet("details")]
        public async Task<ActionResult<Account>> GetAccountDetails()
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var res = await _userManager.FindByIdAsync(userId);

            return Ok(res);
        }
    }
}

using IdentityServer8.Models.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer8.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;

        public AuthController(UserManager<Account> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register account)
        {
            var user = new Account
            {
                UserName = account.Email,
                Email = account.Email,
            };

            var result = await _userManager.CreateAsync(user, account.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }
    }
}

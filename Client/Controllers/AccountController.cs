using System;
using System.IdentityModel.Tokens.Jwt;
using Client.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

public class AccountController : Controller
{
    public IActionResult Login(string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "oidc");
    }

    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" }, "Cookies", "oidc");
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("check")]
    public IActionResult CheckAuth()
    {
        var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(accessToken))
        {
            return View("Access token is missing");
        }

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);

        var userId = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var userName = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

        return View(new Token { AccessToken = accessToken, RefreshToken = token.Issuer});
    }
}

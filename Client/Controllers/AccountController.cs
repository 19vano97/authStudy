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

    [Authorize]
    [HttpGet("check")]
    public async Task<IActionResult> CheckAsync()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Challenge("oidc");
        }

        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

        if (string.IsNullOrEmpty(accessToken))
        {
            return View(new Token { AccessToken = "nothing", RefreshToken = "token.Issuer"});
        }

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync("https://localhost:7099/api/auth/check");
                return View(new Token { AccessToken = accessToken, RefreshToken = response.StatusCode.ToString()});
            }
        }
        catch (System.Exception)
        {
            return View(new Token { AccessToken = accessToken, RefreshToken = refreshToken});
        }
    }
}

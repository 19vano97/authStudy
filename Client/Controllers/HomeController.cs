using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Client.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Client.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> IndexAsync()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

        if (string.IsNullOrEmpty(accessToken))
        {
            return View(new Token { AccessToken = "nothing", RefreshToken = "token.Issuer"});
        }

        // try
        // {
        //     using (HttpClient client = new HttpClient())
        //     {
        //         client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        //         var responce = await client.GetAsync("https://localhost:7099/api/auth/check");
        //         return View(new Token { AccessToken = accessToken, RefreshToken = responce.StatusCode.ToString()});
        //     }
        // }
        // catch (System.Exception)
        // {
        //     return View(new Token { AccessToken = accessToken, RefreshToken = refreshToken});
        // }

        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            // return Content($"Authenticated user ID: {userId}");
            return View(new Token { AccessToken = accessToken, RefreshToken = userId.ToString()});
        }
        else
        {
            return View(new Token { AccessToken = accessToken, RefreshToken = refreshToken});
        }

        return View(new Token { AccessToken = accessToken, RefreshToken = refreshToken});
    }

    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

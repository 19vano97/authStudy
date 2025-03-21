using System;
using System.Text.Json;
using Client.Models;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace Client.Services.Implementations;

public class TokenRefreshService : ITokenRefreshService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;

    public TokenRefreshService(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var accessToken = await httpContext.GetTokenAsync("access_token");
        var expiresAt = await httpContext.GetTokenAsync("expires_at");
        var refreshToken = await httpContext.GetTokenAsync("refresh_token");

        if (string.IsNullOrEmpty(accessToken) || TokenExpired(expiresAt))
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                return await RefreshAccessToken(refreshToken);
            }
            return null; // No valid token available
        }

        return accessToken;
    }

    private bool TokenExpired(string expiresAt)
    {
        if (DateTime.TryParse(expiresAt, out var expiry))
        {
            return expiry < DateTime.UtcNow;
        }
        return true;
    }

    private async Task<string> RefreshAccessToken(string refreshToken)
    {
        var tokenEndpoint = "https://localhost:7270/connect/token"; // IdentityServer Token Endpoint

        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", "mvc-client" },
            { "refresh_token", refreshToken }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestBody)
        };

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<Token>(responseContent);

            var httpContext = _httpContextAccessor.HttpContext;
            var authInfo = await httpContext.AuthenticateAsync();
            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);
            authInfo.Properties.UpdateTokenValue("expires_at", DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresAt).ToString());

            await httpContext.SignInAsync(authInfo.Principal, authInfo.Properties);

            return tokenResponse.AccessToken;
        }

        return null; // Refresh failed
    }
}

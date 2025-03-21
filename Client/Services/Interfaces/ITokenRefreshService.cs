using System;

namespace Client.Services.Interfaces;

public interface ITokenRefreshService
{
    Task<string> GetAccessTokenAsync();
}

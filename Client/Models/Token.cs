using System;

namespace Client.Models;

public class Token
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public int ExpiresAt { get; set; }
}

using System;

namespace Chat.Web.Models.ResponseModel;

public class TokenResponse
{
    public TokenResponse(string username, string token, DateTime expiresIn)
    {
        Username = username;
        Token = token;
        ExpiresIn = expiresIn;
    }
    public string Username { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresIn { get; set; }
}
namespace api.Services;

using api.Models;

public interface IAuthManager
{
    public Task<ApiUser> ValidateUser(string email);
    public Task<string> CreateToken();
    public Task<string> CreateRefreshToken();
    public bool ValidateRefreshToken(string token, string email);
}
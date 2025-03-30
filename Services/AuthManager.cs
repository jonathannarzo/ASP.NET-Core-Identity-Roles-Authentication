namespace api.Services;

using api.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

public class AuthManager : IAuthManager
{
    private readonly UserManager<ApiUser> _userManager;
    private readonly IConfiguration _configuration;
    private ApiUser _user;

    public AuthManager(UserManager<ApiUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<string> CreateToken()
    {
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims, string expiry = "lifetime")
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection(expiry).Value));
        // var expiration = DateTime.Now.AddSeconds(Convert.ToDouble(jwtSettings.GetSection(expiry).Value));
        var token = new JwtSecurityToken(
            issuer: jwtSettings.GetSection("Issuer").Value,
            claims: claims,
            expires: expiration,
            signingCredentials: signingCredentials
        );

        return token;
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>();

        claims.Add(new Claim(ClaimTypes.Name, _user.UserName));

        var roles = await _userManager.GetRolesAsync(_user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private SigningCredentials GetSigningCredentials(string tkey = "SHOPKEY")
    {
        var key = Environment.GetEnvironmentVariable(tkey);
        var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    public async Task<ApiUser> ValidateUser(string email)
    {
        _user = await _userManager.FindByNameAsync(email);
        return _user;
        // return (_user != null);
    }

    public async Task<string> CreateRefreshToken()
    {
        var signingCredentials = GetSigningCredentials("RSHOPKEY");
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims, "refreshtoken");

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public bool ValidateRefreshToken(string token, string email)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Environment.GetEnvironmentVariable("RSHOPKEY");
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                LifetimeValidator = TokenLifetimeValidator.Validate,
                ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var tokenEmail = jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value;

            // check if token belongs to the user
            if (email != tokenEmail)
            {
                return false;
            }

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
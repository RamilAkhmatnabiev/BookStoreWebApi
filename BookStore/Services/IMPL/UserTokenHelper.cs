using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStore.Extensions;
using BookStore.Services.Interfaces;
using BookStoreWebApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace BookStore.Services.IMPL;

public class UserTokenHelper : IUserTokenHelper
{
    private JWTSettings _jwtSettings;

    public UserTokenHelper(JWTSettings jwtSettings)
    {
        this._jwtSettings = jwtSettings;
    }

    public void ApplyJWTToken(UserWithToken userWithToken)
    {
        userWithToken.Token = this.CreateToken(userWithToken);
    }

    public UserWithToken GetUserWithAppliedJWTToken(User user)
    {
        var token = this.CreateToken(user);
        return user.ToUserWithToken(token);
    }

    public string? CreateToken(User user)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        var symmetricKey = new SymmetricSecurityKey(keyBytes);

        var signingCredentials = new SigningCredentials(
            symmetricKey,
            // 👇 one of the most popular. 
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim("sub", user.EmailAddress),
            new Claim("name", user.EmailAddress),
            new Claim("aud", _jwtSettings.Audience)
        };
    
        // var roleClaims = permissions.Select(x => new Claim("role", x));
        // claims.AddRange(roleClaims);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.Add(TimeSpan.FromSeconds(_jwtSettings.ExpirationSeconds)),
            signingCredentials: signingCredentials);

        var rawToken = new JwtSecurityTokenHandler().WriteToken(token);
        return rawToken;
    }
}
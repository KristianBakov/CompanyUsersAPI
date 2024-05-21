using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace CompanyUsersAPI.Helpers;

public class AuthHelper
{
    private readonly IConfiguration _config;
    public AuthHelper(IConfiguration config)
    {
        _config = config;
    }

    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
            Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1000000,
            numBytesRequested: 256 / 8
        );
    }

    public string CreateToken(int userId)
    {
        Claim[] claims =
        {
                new Claim("userId", userId.ToString())
            };

        string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

        // Check if the token key is null or empty and handle it accordingly
        if (string.IsNullOrEmpty(tokenKeyString) || tokenKeyString.Length < 64)
        {
            throw new InvalidOperationException("Token key is missing, empty, or not long enough. It must be at least 64 characters long.");
        }

        // Create the symmetric security key
        SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(tokenKeyString)
        );

        SigningCredentials credentials = new(tokenKey, SecurityAlgorithms.HmacSha512Signature);

        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(1)
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }
}
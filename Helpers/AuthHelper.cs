using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CompanyUsersAPI.Data;
using CompanyUsersAPI.Dtos;
using Dapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace CompanyUsersAPI.Helpers;

public class AuthHelper
{
    private readonly IConfiguration _config;
    private readonly DataContextDapper _dapper;
    public AuthHelper(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
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

    public bool SetPassword(UserForLoginDto userForSetPassword)
    {

        byte[] passwordSalt = new byte[128 / 8];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(passwordSalt);
        }

        byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);

        string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert 
                    @Email = @EmailParam,
                    @PasswordHash = @PasswordHashParam,
                    @PasswordSalt = @PasswordSaltParam";

        // List<SqlParameter> sqlParameters = new List<SqlParameter>();

        // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar)
        // {
        //     Value = userForSetPassword.Email
        // };
        // sqlParameters.Add(emailParameter);

        // SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary);
        // passwordHashParameter.Value = passwordHash;
        // sqlParameters.Add(passwordHashParameter);

        // SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary);
        // passwordSaltParameter.Value = passwordSalt;
        // sqlParameters.Add(passwordSaltParameter);
        DynamicParameters sqlParameters = new();
        sqlParameters.Add("@EmailParam", userForSetPassword.Email, DbType.String);
        sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
        sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);

        return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);
    }
}
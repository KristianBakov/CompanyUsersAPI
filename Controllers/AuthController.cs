using System.Data;
using AutoMapper;
using CompanyUsersAPI.Data;
using CompanyUsersAPI.Dtos;
using CompanyUsersAPI.Helpers;
using CompanyUsersAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyUsersAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        private readonly ReusableSql _reusableSql;
        private readonly IMapper _mapper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
            _reusableSql = new ReusableSql(config);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserForRegistrationDto, User>();
            }));
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirmation)
            {
                string sqlCheckUserExists = "SELECT * FROM TutorialAppSchema.Auth WHERE Email = '"
                 + userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {
                    UserForLoginDto userForSetPassword = new UserForLoginDto
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };

                    if (_authHelper.SetPassword(userForSetPassword))
                    {
                        User user = _mapper.Map<User>(userForRegistration);
                        user.Active = true;

                        if (_reusableSql.UpsertUser(user))
                        {
                            return Ok();
                        }

                        throw new Exception("Failed to add user");
                    }
                    throw new Exception("Failed to register user");
                }
                throw new Exception("User with this Email already exists");
            }
            throw new Exception("Passwords do not match");
        }

        [HttpPut("ResetPassowrd")]
        public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Failed to update password");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = "EXEC TutorialAppSchema.spLoginConfirmation_Get @Email = @EmailParam";


            DynamicParameters sqlParameters = new();
            // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            // emailParameter.Value = userForLogin.Email;
            // sqlParameters.Add(emailParameter);
            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

            UserForLoginConfirmationDto userForConfirmation = _dapper
            .LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            // if (passwordHash.SequenceEqual(userForConfirmation.PasswordHash))
            // {
            //     return Ok();
            // }

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Incorrect password");
                }
            }

            string userIdSql = @"SELECT userId FROM TutorialAppSchema.Users WHERE Email = '" +
                            userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userId) }
            });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";

            string userIdSql = "SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = " + userId;
            int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userIdFromDB) }
            });
        }
    }
}
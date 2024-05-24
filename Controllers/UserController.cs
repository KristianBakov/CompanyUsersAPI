using System.Data;
using Dapper;
using CompanyUsersAPI.Data;
using CompanyUsersAPI.Helpers;
using CompanyUsersAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyUsersAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly ReusableSql _reusableSql;
        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _reusableSql = new ReusableSql(config);
        }

        [HttpGet("GetUsers/{userId}/{isActive}")]
        public IEnumerable<User> GetUsers(int userId, bool isActive = true)
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";
            string stringParameters = "";
            DynamicParameters sqlParameters = new();

            if (userId != 0)
            {
                stringParameters += ", @UserId=@UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }

            stringParameters += ", @Active=@ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);

            if (stringParameters.Length > 0)
            {
                sql += stringParameters.Substring(1);
            }

            IEnumerable<User> users = _dapper.LoadDataWithParameters<User>(sql, sqlParameters);
            return users;
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(User user)
        {
            if (_reusableSql.UpsertUser(user))
            {
                return Ok();
            }

            throw new Exception("Failed to update User");
        }


        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"EXEC TutorialAppSchema.spUser_Delete
                         @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new();
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to delete User");
        }
    }
}
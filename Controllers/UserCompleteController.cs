using CompanyUsersAPI.Data;
using CompanyUsersAPI.Dtos;
using CompanyUsersAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyUsersAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserCompleteController : ControllerBase
    {
        DataContextDapper _dapper;
        public UserCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }


        [HttpGet("GetUsers/{userId}/{isActive}")]
        public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";
            string parameters = "";
            if (userId != 0)
            {
                parameters += ", @UserId=" + userId.ToString();
            }

            if (isActive)
            {
                parameters += ", @Active=" + isActive.ToString();
            }

            sql += parameters.Substring(1);

            IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
            return users;
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            string sql = @"UPDATE TutorialAppSchema.Users
                            SET [FirstName] = '" + user.FirstName +
                            "',[LastName] = '" + user.LastName +
                            "', [Email] = '" + user.Email +
                            "', [Gender] = '" + user.Gender +
                            "', [Active] = '" + user.Active +
                           "' WHERE UserId =  " + user.UserId;
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to update User");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserDto user)
        {
            string sql = @"INSERT INTO TutorialAppSchema.Users(
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active]
                            ) VALUES (" +
                            "'" + user.FirstName +
                            "', '" + user.LastName +
                            "', '" + user.Email +
                            "', '" + user.Gender +
                            "', '" + user.Active +
                            "')";


            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to add User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.Users
                         WHERE UserId = " + userId.ToString();
            Console.WriteLine(sql);
            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete User");
        }

        /* User Job Info */

        [HttpPut("EditUserJobInfo")]
        public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
        {
            string sql = @"UPDATE TutorialAppSchema.UserJobInfo
                            SET [JobTitle] = '" + userJobInfo.JobTitle +
                            "', [Department] = '" + userJobInfo.Department +
                            "' WHERE UserId = " + userJobInfo.UserId;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to update user job info for user " + userJobInfo.UserId);
        }

        [HttpPost("AddUserJobInfo")]
        public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
        {
            string sql = @"INSERT INTO TutorialAppSchema.UserJobInfo(
                            [UserId],
                            [JobTitle],
                            [Department]
                            ) VALUES (" +
                            userJobInfo.UserId +
                            ", '" + userJobInfo.JobTitle +
                            "', '" + userJobInfo.Department +
                            "')";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to add user job info for user " + userJobInfo.UserId);
        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.UserJobInfo
                           WHERE UserId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete user job info for user " + userId);
        }

        /* User Salary */

        [HttpPut("EditUserSalary")]
        public IActionResult EditUserSalary(UserSalary userSalary)
        {
            string sql = @"UPDATE TutorialAppSchema.UserSalary
                            SET [Salary] = " + userSalary.Salary +
                            " WHERE UserId = " + userSalary.UserId;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to update user salary for user " + userSalary.UserId);
        }

        [HttpPost("AddUserSalary")]
        public IActionResult AddUserSalary(UserSalary userSalary)
        {
            string sql = @"INSERT INTO TutorialAppSchema.UserSalary(
                            [UserId],
                            [Salary]
                            ) VALUES (" +
                            userSalary.UserId +
                            ", " + userSalary.Salary +
                            ")";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to add user salary for user " + userSalary.UserId);
        }

        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.UserSalary
                           WHERE UserId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to delete user salary for user " + userId);
        }
    }
}
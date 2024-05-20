using CompanyUsersAPI.Data;
using CompanyUsersAPI.Dtos;
using CompanyUsersAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyUsersAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        DataContextDapper _dapper;
        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }


        [HttpGet("GetUsers/")]
        public IEnumerable<User> GetUsers()
        {
            string sql = @"SELECT [UserId],
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active] 
                            FROM TutorialAppSchema.Users";
            IEnumerable<User> users = _dapper.LoadData<User>(sql);
            return users;
        }

        [HttpGet("GetSingleUser/{userId}")]
        public User GetSingleUser(int userId)
        {
            string sql = @"SELECT [UserId],
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active] 
                            FROM TutorialAppSchema.Users
                            WHERE UserId = " + userId.ToString();
            User user = _dapper.LoadDataSingle<User>(sql);
            return user;
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
            Console.WriteLine(sql);
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

        [HttpGet("GetUserJobInfo/{userId}")]
        public UserJobInfo GetUserJobInfo(int userId)
        {
            string sql = @"SELECT [UserId],
                            [JobTitle],
                            [Department]
                            FROM TutorialAppSchema.UserJobInfo
                            WHERE UserId = " + userId.ToString();

            UserJobInfo userJobInfo = _dapper.LoadDataSingle<UserJobInfo>(sql);
            return userJobInfo;
        }

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

        [HttpGet("GetUserSalary/{userId}")]
        public UserSalary GetUserSalary(int userId)
        {
            string sql = @"SELECT [UserId],
                            [Salary] 
                            FROM TutorialAppSchema.UserSalary
                            WHERE UserId = " + userId.ToString();

            UserSalary userSalary = _dapper.LoadDataSingle<UserSalary>(sql);
            return userSalary;
        }

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
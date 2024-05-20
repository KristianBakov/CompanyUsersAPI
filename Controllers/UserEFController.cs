using AutoMapper;
using CompanyUsersAPI.Data;
using CompanyUsersAPI.Dtos;
using CompanyUsersAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyUsersAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserEFController : ControllerBase
    {
        DataContextEF _entityFramework;
        IMapper _mapper;
        public UserEFController(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDto, User>();
            }));
        }


        [HttpGet("GetUsers/")]
        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entityFramework.Users.ToList();
            return users;
        }

        [HttpGet("GetSingleUser/{userId}")]
        public User GetSingleUser(int userId)
        {
            User? user = _entityFramework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault();
            if (user != null) return user;

            throw new Exception("Failed to get User " + userId);
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            User? userDb = _entityFramework.Users
            .Where(u => u.UserId == user.UserId)
            .FirstOrDefault();
            if (userDb != null)
            {
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Gender = user.Gender;
                userDb.Active = user.Active;
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }

                throw new Exception("Failed to update user " + user.UserId);
            }

            throw new Exception("Failed to get " + user.UserId);
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserDto user)
        {
            var existingUser = _entityFramework.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Conflict("A user with the same email already exists.");
            }

            User userDb = _mapper.Map<User>(user);

            _entityFramework.Users.Add(userDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }


            throw new Exception("Failed to add user " + user.FirstName + " " + user.LastName);
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User? userDb = _entityFramework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault();
            if (userDb != null)
            {
                _entityFramework.Users.Remove(userDb);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }


                throw new Exception("Failed to delete user " + userId);
            }

            throw new Exception("Failed to get " + userId);
        }

        /* User Job Info */
        [HttpGet("GetUserJobInfo/{userId}")]
        public UserJobInfo GetUserJobInfo(int userId)
        {
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

            if (userJobInfo != null) return userJobInfo;
            throw new Exception("Failed to get User Job Info " + userId);
        }

        [HttpPut("EditUserJobInfo")]
        public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
        {
            UserJobInfo? userDb = _entityFramework.UserJobInfo
            .Where(u => u.UserId == userJobInfo.UserId)
            .FirstOrDefault();
            if (userDb != null)
            {
                userDb.JobTitle = userJobInfo.JobTitle;
                userDb.Department = userJobInfo.Department;

                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }

                throw new Exception("Failed to update user " + userJobInfo.UserId);
            }

            throw new Exception("Failed to get " + userJobInfo.UserId);
        }

        [HttpPost("AddUserJobInfo")]
        public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
        {
            _entityFramework.UserJobInfo.Add(userJobInfo);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to add user job info for user " + userJobInfo.UserId);
        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

            if (userJobInfo != null)
            {
                _entityFramework.UserJobInfo.Remove(userJobInfo);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }

                throw new Exception("Failed to delete User Job Info");
            }

            throw new Exception("Failed to get " + userId);
        }

        /* User Salary */

        [HttpGet("GetUserSalary/{userId}")]
        public UserSalary GetUserSalary(int userId)
        {
            UserSalary? userSalary = _entityFramework.UserSalary
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

            if (userSalary != null) return userSalary;
            throw new Exception("Failed to get User Salary " + userId);
        }

        [HttpPut("EditUserSalary")]
        public IActionResult EditUserSalary(UserSalary userSalary)
        {
            UserSalary? userDb = _entityFramework.UserSalary
            .Where(u => u.UserId == userSalary.UserId)
            .FirstOrDefault();
            if (userDb != null)
            {
                userDb.Salary = userSalary.Salary;

                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }

                throw new Exception("Failed to update user salary for user " + userSalary.UserId);
            }

            throw new Exception("Failed to get " + userSalary.UserId);
        }

        [HttpPost("AddUserSalary")]
        public IActionResult AddUserSalary(UserSalary userSalary)
        {
            _entityFramework.UserSalary.Add(userSalary);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to add user salary for user " + userSalary.UserId);
        }

        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            UserSalary? userSalary = _entityFramework.UserSalary
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

            if (userSalary != null)
            {
                _entityFramework.UserSalary.Remove(userSalary);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }

                throw new Exception("Failed to delete User Salary");
            }

            throw new Exception("Failed to get " + userId);
        }
    }
}
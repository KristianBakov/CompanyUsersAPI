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
        public UserEFController(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
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

            User userDb = new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = user.Gender,
                Active = user.Active
            };

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
    }
}
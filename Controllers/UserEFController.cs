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
        IUserRepository _userRepository;
        IMapper _mapper;
        public UserEFController(IConfiguration config, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserDto, User>();
            }));
        }


        [HttpGet("GetUsers/")]
        public IEnumerable<User> GetUsers()
        {
            return _userRepository.GetUsers();
        }

        [HttpGet("GetSingleUser/{userId}")]
        public User GetSingleUser(int userId)
        {
            return _userRepository.GetSingleUser(userId);
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            User? userDb = _userRepository.GetSingleUser(user.UserId);
            if (userDb != null)
            {
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Gender = user.Gender;
                userDb.Active = user.Active;
                if (_userRepository.SaveChanges())
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
            User userDb = _mapper.Map<User>(user);

            _userRepository.AddEntity<User>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }


            throw new Exception("Failed to add user " + user.FirstName + " " + user.LastName);
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User? userDb = _userRepository.GetSingleUser(userId);
            if (userDb != null)
            {
                _userRepository.RemoveEntity(userDb);
                if (_userRepository.SaveChanges())
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
            UserJobInfo? userJobInfo = _userRepository.GetSingleUserJobInfo(userId);

            if (userJobInfo != null) return userJobInfo;
            throw new Exception("Failed to get User Job Info " + userId);
        }

        [HttpPut("EditUserJobInfo")]
        public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
        {
            UserJobInfo? userDb = _userRepository.GetSingleUserJobInfo(userJobInfo.UserId);
            if (userDb != null)
            {
                userDb.JobTitle = userJobInfo.JobTitle;
                userDb.Department = userJobInfo.Department;

                if (_userRepository.SaveChanges())
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
            _userRepository.AddEntity(userJobInfo);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Failed to add user job info for user " + userJobInfo.UserId);
        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            UserJobInfo? userJobInfo = _userRepository.GetSingleUserJobInfo(userId);

            if (userJobInfo != null)
            {
                _userRepository.RemoveEntity(userJobInfo);
                if (_userRepository.SaveChanges())
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
            UserSalary? userSalary = _userRepository.GetSingleUserSalary(userId);

            if (userSalary != null) return userSalary;
            throw new Exception("Failed to get User Salary " + userId);
        }

        [HttpPut("EditUserSalary")]
        public IActionResult EditUserSalary(UserSalary userSalary)
        {
            UserSalary? userDb = _userRepository.GetSingleUserSalary(userSalary.UserId);
            if (userDb != null)
            {
                userDb.Salary = userSalary.Salary;

                if (_userRepository.SaveChanges())
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
            _userRepository.AddEntity(userSalary);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Failed to add user salary for user " + userSalary.UserId);
        }

        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            UserSalary? userSalary = _userRepository.GetSingleUserSalary(userId);

            if (userSalary != null)
            {
                _userRepository.RemoveEntity(userSalary);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Failed to delete User Salary");
            }

            throw new Exception("Failed to get " + userId);
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace CompanyUsersAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        public UserController(IConfiguration config)
        {

        }

        [HttpGet("GetUsers/{testValue}")]
        public string[] GetUsers(string testValue)
        {
            string[] responseArray = new string[]
            {
                "test1",
                "test2",
                testValue
            };
            return responseArray;
        }
    }


}
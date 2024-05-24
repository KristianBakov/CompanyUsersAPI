using System.Data;
using CompanyUsersAPI.Data;
using CompanyUsersAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyUsersAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    public PostController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
    public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get";

        DynamicParameters sqlParams = new();

        string stringParameters = "";
        if (postId != 0)
        {
            sqlParams.Add("@PostIdParam", postId, DbType.Int32);
            stringParameters += ", @PostId=@PostIdParam";
        }
        if (userId != 0)
        {
            sqlParams.Add("@UserIdParam", userId, DbType.Int32);
            stringParameters += ", @UserId=@UserIdParam";
        }
        if (searchParam.ToLower() != "none")
        {
            sqlParams.Add("@SearchValueParam", searchParam, DbType.String);
            stringParameters += ", @SearchValue=@SearchValueParam";
        }

        if (stringParameters.Length > 0)
        {
            sql += stringParameters.Substring(1);
        }

        return _dapper.LoadDataWithParameters<Post>(sql, sqlParams);
    }


    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = @UserIdParam";

        DynamicParameters sqlParams = new();
        sqlParams.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);

        return _dapper.LoadDataWithParameters<Post>(sql, sqlParams);
    }


    [HttpPut("UpsertPost")]
    public IActionResult UpsertPost(Post postToUpsert)
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                        @UserId = @UserIdParam,
                        @PostTitle = @PostTitleParam,
                        @PostContent = @PostContentParam";

        DynamicParameters sqlParams = new();
        sqlParams.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);
        sqlParams.Add("@PostTitleParam", postToUpsert.PostTitle, DbType.String);
        sqlParams.Add("@PostContentParam", postToUpsert.PostContent, DbType.String);

        if (postToUpsert.PostId > 0)
        {
            sqlParams.Add("@PostIdParam", postToUpsert.PostId, DbType.Int32);
            sql += ", @PostId = @PostIdParam";
        }

        if (_dapper.ExecuteSqlWithParameters(sql, sqlParams))
        {
            return Ok();
        }

        throw new Exception("Failed to upsert post!");
    }

    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        string sql = @"EXEC TutorialAppSchema.spPost_Delete @PostId = @PostIdParam, @UserId = @UserIdParam";

        DynamicParameters sqlParams = new();
        sqlParams.Add("@PostIdParam", postId, DbType.Int32);
        sqlParams.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to delete post with id " + postId.ToString());
    }
}
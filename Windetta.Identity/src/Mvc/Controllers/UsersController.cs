﻿using Microsoft.AspNetCore.Mvc;
using Windetta.Identity.Models;

namespace Windetta.Identity.Controllers;

[ApiController]
[Route("")]
public class UsersController : BaseController
{
    /// <summary>
    /// Get user info by ID
    /// </summary>
    /// <param name="id">User id</param>
    /// <returns>User information object</returns>
    [Route("users/{userId:Guid}")]
    [HttpGet]
    public async Task<ActionResult<UserViewModel>> User(Guid userId)
    {
        return Ok();
    }

}

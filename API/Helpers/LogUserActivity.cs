using System;
using System.Security.Claims;
using API.Data;
using API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class LogUserActivity: IAsyncActionFilter
{
    // you should register this class in the dependency injection container in Program.cs 
    // you should also add [ServiceFilter(typeof(LogUserActivity))] attribute to the base api controller 
    // or specific controllers/actions where you want to log user activity.
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Code runs BEFORE the action
        var resultContext = await next();
        // Code runs AFTER the action
        if (resultContext.HttpContext.User.Identity?.IsAuthenticated != true) return;


        // if authenticated, get the user id from the token, and update the last active field in the database
        // you access it in current middle ware  this because you made ( options.SaveToken = true; ) in 
        // Program.cs when configuring JWT bearer authentication
        var userId = int.Parse(resultContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "-1");
        if(userId == -1)
        {
            throw new Exception("there's a problem with the token");
        }
        var dbContext = resultContext.HttpContext.RequestServices.GetService<AppDbContext>();

        // it is more efficient to use ExecuteUpdateAsync to update the field without loading the entity
        // so instead of making 2 queries (one to get the user and one to update), it makes a single update query
        await dbContext!.Members
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u.SetProperty(m => m.LastActive, m => DateTime.UtcNow));
    }
}
using System;
using API.Data;
using API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class LogUserActivity: IAsyncActionFilter
{
    // you should register this class in the dependency injection container in Program.cs 
    // you should also add [ServiceFilter(typeof(LogUserActivity))] attribute to the base api controller or specific controllers/actions where you want to log user activity.
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // after the request is handled in backend you check if the user is authenticated
        var resultContext = await next();
        if (resultContext.HttpContext.User.Identity?.IsAuthenticated != true) return;


        // if authenticated, get the user id from the token, and update the last active field in the database
        var userId = resultContext.HttpContext.User.GetMemberId();
        var dbContext = resultContext.HttpContext.RequestServices.GetService<AppDbContext>();

        // it is more efficient to use ExecuteUpdateAsync to update the field without loading the entity
        await dbContext!.Members
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u.SetProperty(m => m.LastActive, m => DateTime.UtcNow));
    }

}

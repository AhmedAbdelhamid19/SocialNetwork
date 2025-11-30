using API.Data;
using API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public class LogUserActivity: IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Code runs BEFORE the action
        var resultContext = await next();
        // Code runs AFTER the action
        if (resultContext.HttpContext.User.Identity?.IsAuthenticated != true) return;

        var userId = resultContext.HttpContext.User.GetMemberId();
        var dbContext = resultContext.HttpContext.RequestServices.GetService<AppDbContext>();

        // it is more efficient to use ExecuteUpdateAsync to update the field without loading the entity
        // so instead of making 2 queries (one to get the user and one to update), it makes a single update query
        await dbContext!.Members
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u.SetProperty(m => m.LastActive, m => DateTime.UtcNow));
    }
}
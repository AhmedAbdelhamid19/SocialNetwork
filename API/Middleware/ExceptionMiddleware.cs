using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;
// Instead of letting ASP.NET crash and show ugly error pages, 
// you control the error format and always respond with clean JSON.
// type this in program.cs to add this middleware: app.UseMiddleware<ExceptionMiddleware>();
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware>logger, IHostEnvironment env)
{
    // when you but this in middleware dotnet search for Invoke or InvokeAsync method and call it
    public async Task InvokeAsync(HttpContext context) {
        try
        {
            await next(context);
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, ex.Message); // log the error first
            context.Response.ContentType = "application/json"; // specify the type of the response (json format)
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError; // status code is internal server error

            var response = env.IsDevelopment() // construct general response and detect message based on develpment or production
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace) 
                : new ApiException(context.Response.StatusCode, ex.Message, "Internal server error");
            
            var options = new JsonSerializerOptions // to return json response in camel case
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // convert APiException object to json
            var json = JsonSerializer.Serialize(response, options);
            // Writes the given json to the response body
            await context.Response.WriteAsync(json);
        }
    }
}

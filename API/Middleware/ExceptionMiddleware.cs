using System;
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware>logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context) {
        try
        {
            await next(context);
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "{message}", ex.Message); // log the error first
            context.Response.ContentType = "application/json"; // specify the type of the response (json format)
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError; // status code is internal server error

            var response = env.IsDevelopment() // construct general response and detect message based on develpment or production
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace) 
                : new ApiException(context.Response.StatusCode, ex.Message, "Internal server error");
            
            var options = new JsonSerializerOptions // to return json response in camel case
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options); // return the general response error as json
            await context.Response.WriteAsync(json);
        }
    }
}

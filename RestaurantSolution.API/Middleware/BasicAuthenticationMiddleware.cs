using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantSolution.API.Middleware
{
    /// <summary>
    /// Middleware that implements Basic Authentication
    /// Basic Authentication uses the Authorization header with a value of "Basic <base64-encoded-credentials>"
    /// The credentials are encoded as "username:password" in base64
    /// </summary>
    public class BasicAuthenticationMiddleware
    {
        // Hardcoded credentials - in a real application, these would come from a database
        private const string USERNAME = "john.doe";
        private const string PASSWORD = "VerySecret!";
        
        // The next middleware in the pipeline
        private readonly RequestDelegate _next;
        
        public BasicAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            // Allow anonymous endpoints to bypass authentication
            // This checks if the endpoint has the [AllowAnonymous] attribute
            if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }
            
            // 1. Try to retrieve the Authorization header
            string? authHeader = context.Request.Headers["Authorization"];
            
            // 2. If not found, return Unauthorized response
            if (authHeader == null)
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Authorization Header value not provided");
                return;
            }
            
            // 3. Extract the encoded credentials by splitting on space
            // The value looks like "Basic am9obi5kb2U6VmVyeVNlY3JldCE="
            var auth = authHeader.Split([' '])[1];
            
            // 4. Decode the base64 encoded credentials back to plain text
            var usernameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
            
            // 5. Extract username and password which are separated by a colon
            var username = usernameAndPassword.Split([':'])[0];
            var password = usernameAndPassword.Split([':'])[1];
            
            // 6. Check if credentials match the expected values
            if (username == USERNAME && password == PASSWORD)
            {
                // If they match, continue to the next middleware
                await _next(context);
            }
            else
            {
                // If they don't match, return Unauthorized
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Incorrect credentials provided");
                return;
            }
        }
    }
    
    /// <summary>
    /// Extension method to easily add this middleware to the pipeline
    /// </summary>
    public static class BasicAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthenticationMiddleware>();
        }
    }
}
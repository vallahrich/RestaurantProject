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
            // Allow Swagger UI to bypass authentication
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }
            
            // Allow Restaurant endpoints to bypass authentication
            if (context.Request.Path.StartsWithSegments("/api/Restaurant"))
            {
                await _next(context);
                return;
            }
            
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
            var parts = authHeader.Split(' ');
            if (parts.Length != 2 || parts[0] != "Basic")
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid Authorization Header format");
                return;
            }
            
            var base64Credentials = parts[1];
            
            // 4. Decode the base64 encoded credentials back to plain text
            string decodedCredentials;
            try
            {
                var credentialsBytes = Convert.FromBase64String(base64Credentials);
                decodedCredentials = Encoding.UTF8.GetString(credentialsBytes);
            }
            catch (FormatException)
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid Base64 format in credentials");
                return;
            }
            
            // 5. Extract username and password which are separated by a colon
            var credentialParts = decodedCredentials.Split(':');
            if (credentialParts.Length != 2)
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid credential format");
                return;
            }
            
            var username = credentialParts[0];
            var password = credentialParts[1];
            
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
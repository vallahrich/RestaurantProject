/// <summary>
/// Middleware component that implements Basic Authentication for the API.
/// Intercepts HTTP requests and validates credentials before allowing access to protected endpoints.
/// </summary>
/// <remarks>
/// This middleware:
/// - Allows unauthenticated access to endpoints marked with [AllowAnonymous]
/// - Checks for the presence of the Authorization header
/// - Decodes and validates credentials against the user database
/// - Stores authenticated user information in HttpContext.Items for downstream components
/// - Returns appropriate 401 Unauthorized responses for invalid/missing credentials
/// 
/// The class follows the standard ASP.NET Core middleware pattern with constructor injection
/// and an async Invoke method. It also provides an extension method for easy registration
/// in the application's middleware pipeline.
/// </remarks>

using Microsoft.AspNetCore.Authorization;
using RestaurantSolution.Model.Repositories;

namespace RestaurantSolution.API.Middleware
{
    public class BasicAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        
        public BasicAuthenticationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider; 
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            // Bypass authentication for [AllowAnonymous]
            if (context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }
            
            // 1. Try to retrieve the Request Header containing our secret value
            string? authHeader = context.Request.Headers["Authorization"];
            
            // 2. If not found, then return with Unauthorized response
            if (authHeader == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Authorization Header value not provided");
                return;
            }
/* 
            // 3. Extract the username and password from the value by splitting it on space,
            // as the value looks something like 'Basic am9obi5kb2U6VmVyeVNlY3JldCE='
            var auth = authHeader.Split(' ')[1];
            
            // 4. Convert it from Base64 encoded text, back to normal text
            var usernameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
            
            // 5. Extract username and password, which are separated by a colon
            var username = usernameAndPassword.Split(':')[0];
            var password = usernameAndPassword.Split(':')[1];
*/
            AuthenticationHelper.Decrypt(authHeader, out string username, out string password);

            // 6. Check credentials against database
            using (var scope = _serviceProvider.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                
                // Get user from database
                var user = userRepository.GetUserByUsername(username);
                
                // Check if user exists and password matches
                if (user != null && user.PasswordHash == password)
                {
                    // Store user information in HttpContext for later use
                    context.Items["UserId"] = user.UserId;
                    context.Items["Username"] = user.Username;
                    await _next(context);
                }
                else
                {
                    // If not, then send Unauthorized response
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Incorrect credentials provided");
                    return;
                }
            }
        }
    }
    
    public static class BasicAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseBasicAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthenticationMiddleware>();
        }
    }
}
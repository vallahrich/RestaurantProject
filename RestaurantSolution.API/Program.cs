/// <summary>
/// Program entry point and configuration for the Copenhagen Restaurant Explorer API.
/// Sets up the ASP.NET Core application, configures services, and establishes the HTTP request pipeline.
/// </summary>
/// <remarks>
/// Key configuration components:
/// - Registers repository services using dependency injection with scoped lifetime
/// - Configures Swagger/OpenAPI documentation with Basic Authentication support
/// - Enables CORS for the Angular frontend (http://localhost:4200)
/// - Sets up the HTTP request pipeline with appropriate middleware:
///   - Swagger UI for API documentation (in Development environment only)
///   - CORS middleware to handle cross-origin requests
///   - Custom Basic Authentication middleware for securing API endpoints
///   - Standard controller routing
/// 
/// This configuration establishes a RESTful API for restaurant exploration with
/// authentication, documentation, and proper cross-origin resource sharing.
/// </remarks>

using RestaurantSolution.Model.Repositories;
using RestaurantSolution.API.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI with authentication
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Copenhagen Restaurant Explorer API", Version = "v1" });
    
    // Add security definition for Basic Authentication
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authentication. Format: username:password encoded in Base64. Default: john.doe:VerySecret!"
    });
    
    // Make Swagger UI use Basic Authentication
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IBookmarkRepository, BookmarkRepository>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Copenhagen Restaurant Explorer API v1");
        c.RoutePrefix = "swagger";
    });
}

// Enable CORS
app.UseCors();

// Add Basic Authentication Middleware
app.UseBasicAuthenticationMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
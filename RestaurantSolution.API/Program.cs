using RestaurantSolution.Model.Repositories;
using RestaurantSolution.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Copenhagen Restaurant Explorer API", Version = "v1" });
});

// Register repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RestaurantRepository>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<BookmarkRepository>();

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
// Note: This should come before UseAuthorization but after UseCors
app.UseBasicAuthenticationMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
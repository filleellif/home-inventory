using HomeInventory.Application;
using HomeInventory.Infrastructure;
using HomeInventory.WebApi;
using HomeInventory.WebApi.Configuration;
using HomeInventory.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
var applicationOptions = builder.Configuration.Get<ApplicationOptions>() ?? throw new InvalidOperationException("");

builder.Services.ConfigureLogging(applicationOptions.OpenTelemetry);

// Add services to the container
builder.Services.AddControllers();

// Register Application layer services (MediatR, AutoMapper, Validation)
builder.Services.AddApplication();

// Register Infrastructure layer services (EF Core, Repositories, File Storage)
builder.Services.AddInfrastructure(builder.Configuration);

// Configure CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Home Inventory API",
        Version = "v1",
        Description = "API for managing household inventory items",
        Contact = new()
        {
            Name = "Home Inventory",
        }
    });

    // Enable XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Home Inventory API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

// Add global exception handling
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Starting Home Inventory API");
app.Run();
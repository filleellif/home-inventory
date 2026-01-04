using HomeInventory.Application;
using HomeInventory.Infrastructure;
using HomeInventory.Queries;
using HomeInventory.WebApi;
using HomeInventory.WebApi.Configuration;
using HomeInventory.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
var applicationOptions = builder.Configuration.Get<ApplicationOptions>() ?? throw new InvalidOperationException();

builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(applicationOptions.Postgres);
builder.Services.AddQueries();
builder.Services.ConfigureLogging(applicationOptions.OpenTelemetry);

builder.Services.AddCors(options =>
{
    foreach (var cors in applicationOptions.Cors)
    {
        options.AddPolicy(cors.Name, policy =>
        {
            policy
                .WithOrigins(cors.AllowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    }
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
app.UseMiddleware<ExceptionHandlingMiddleware>();

// app.UseHttpsRedirection();

// Enable CORS
foreach (var cors in applicationOptions.Cors)
{
    app.UseCors(cors.Name);
}

app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Starting Home Inventory API");
app.Run();
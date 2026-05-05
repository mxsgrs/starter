using MassTransit;
using Microsoft.EntityFrameworkCore;
using Network.Application.Users.Events;
using Sales.WebApi.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Message bus
builder.Services.AddMassTransit(registration =>
{
    registration.AddConsumer<UserCreatedEventConsumer>();
    registration.AddConsumer<UserDeletedEventConsumer>();

    registration.UsingRabbitMq((context, rabbitMqConfiguration) =>
    {
        string connectionString = builder.Configuration.GetConnectionString("RabbitMq")
            ?? throw new Exception("Connection string for RabbitMQ is missing");

        rabbitMqConfiguration.Host(new Uri(connectionString));

        rabbitMqConfiguration.ConfigureEndpoints(context);
    });
});

string? aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

if ((builder.Environment.IsDevelopment() && aspNetCoreEnvironment is not null) || builder.Environment.IsProduction())
{
    string connectionString = builder.Configuration.GetConnectionString(
        builder.Environment.IsProduction() ? "SqlServer" : "SalesDb")
            ?? throw new Exception("Connection string for SQL Server is missing");

    builder.Services.AddDbContext<SalesDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "Sales API";
    settings.Version = "v1";
});

WebApplication app = builder.Build();

app.MapDefaultEndpoints();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(c => c.Path = "/api/sales/swagger/{documentName}/swagger.json");
    app.UseSwaggerUi(c =>
    {
        c.Path = "/api/sales/swagger";
        c.DocumentPath = "/api/sales/swagger/v1/swagger.json";
    });
}

app.UseHttpsRedirection();

app.Run();

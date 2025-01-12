using MassTransit;
using Starter.Infrastructure.Messages;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Message bus
builder.Services.AddMassTransit(registration =>
{
    registration.AddConsumer<CheckUserAddressConsumer>();

    registration.UsingAzureServiceBus((context, configurator) =>
    {
        string connectionString = builder.Configuration.GetConnectionString("AzureServiceBus")
            ?? throw new Exception("Azure Service Bus connection string is missing");

        configurator.Host(connectionString);

        configurator.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Fake endpoint
app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

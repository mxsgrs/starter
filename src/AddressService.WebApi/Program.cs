using MassTransit;
using Starter.Infrastructure.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Message bus
builder.Services.AddMassTransit(registration =>
{
    registration.AddConsumer<CheckUserAddressConsumer>();

    registration.UsingRabbitMq((context, rabbitMqConfiguration) =>
    {
        string connectionString = builder.Configuration.GetConnectionString("RabbitMq")
            ?? throw new Exception("Connection string for RabbitMQ is missing");

        rabbitMqConfiguration.Host(new Uri(connectionString));

        rabbitMqConfiguration.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

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

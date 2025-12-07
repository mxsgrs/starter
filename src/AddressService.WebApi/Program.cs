using MassTransit;
using UserService.Application.Commands.UserCommands;
using UserService.Infrastructure.Messaging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Message bus
builder.Services.AddMassTransit(registration =>
{
    registration.AddConsumer<CheckUserAddressConsumer>();
    registration.AddConsumer<UserCreatedEventConsumer>();

    registration.UsingRabbitMq((context, rabbitMqConfiguration) =>
    {
        string connectionString = builder.Configuration.GetConnectionString("RabbitMq")
            ?? throw new Exception("Connection string for RabbitMQ is missing");

        rabbitMqConfiguration.Host(new Uri(connectionString));

        rabbitMqConfiguration.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

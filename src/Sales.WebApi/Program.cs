using MassTransit;
using Network.Domain.Aggregates.UserAggregate;
using Network.Infrastructure.Messaging;

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

builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "Sales API";
    settings.Version = "v1";
});

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseHttpsRedirection();

app.Run();

using MassTransit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

public class CheckUserAddressConsumer : IConsumer<CheckUserAddress>
{
    public async Task Consume(ConsumeContext<CheckUserAddress> context)
    {
        CheckUserAddressResult result = new()
        {
            IsValid = true
        };

        await context.RespondAsync(result);
    }
}

public class CheckUserAddress
{
    public string Address { get; set; } = "";
}

public class CheckUserAddressResult
{
    public bool IsValid { get; set; } = false;
}

using MassTransit;

// Keep this namespace intact otherwise MassTransit will break.
namespace Network.Infrastructure.Messaging;

public record CheckUserAddress
{
    public string Address { get; init; } = "";
}

public record CheckUserAddressResult
{
    public bool IsValid { get; init; } = false;
}

public class CheckUserAddressConsumer(ILogger<CheckUserAddressConsumer> logger) : IConsumer<CheckUserAddress>
{
    public async Task Consume(ConsumeContext<CheckUserAddress> context)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Checking the user address {Address}", context.Message.Address);
        }

        CheckUserAddressResult result = new()
        {
            IsValid = true
        };

        await context.RespondAsync(result);
    }
}

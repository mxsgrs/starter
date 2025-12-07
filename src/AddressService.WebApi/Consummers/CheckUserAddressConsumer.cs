using MassTransit;

namespace UserService.Infrastructure.Messaging;

public class CheckUserAddress
{
    public string Address { get; set; } = "";
}

public class CheckUserAddressResult
{
    public bool IsValid { get; set; } = false;
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

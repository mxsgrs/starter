using MassTransit;

namespace Starter.Infrastructure.Messages;

public class CheckUserAddress
{
    public string Address { get; set; } = "";
}

public class CheckUserAddressResult
{
    public bool IsValid { get; set; } = false;
}

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

using MassTransit;
using UserService.Domain.Aggregates.UserAggregate;

namespace UserService.Infrastructure.Messages;

public class CheckUserAddress
{
    public string Address { get; set; } = "";
}

public class CheckUserAddressResult
{
    public bool IsValid { get; set; } = false;
}

public class CheckUserAddressService(IRequestClient<CheckUserAddress> client) : ICheckUserAddressService
{
    private readonly IRequestClient<CheckUserAddress> _client = client;

    public async Task<bool> Check(string address, CancellationToken cancellationToken = default)
    {
        CheckUserAddress check = new()
        {
            Address = address
        };

        Response<CheckUserAddressResult>? response = await _client
            .GetResponse<CheckUserAddressResult>(check, cancellationToken);

        return response.Message?.IsValid ?? false;
    }
}

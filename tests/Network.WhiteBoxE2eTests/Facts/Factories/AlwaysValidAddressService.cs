namespace Network.WhiteBoxE2eTests.Facts.Factories;

public class AlwaysValidAddressService : ICheckUserAddressService
{
    public Task<bool> Check(string address, CancellationToken cancellationToken = default)
        => Task.FromResult(true);
}

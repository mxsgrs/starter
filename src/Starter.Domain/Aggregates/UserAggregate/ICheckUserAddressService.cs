namespace Starter.Domain.Aggregates.UserAggregate
{
    public interface ICheckUserAddressService
    {
        Task<bool> Check(string address, CancellationToken cancellationToken = default);
    }
}
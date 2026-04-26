using Network.Application.Users.Dtos;

namespace Network.ModelBuilders;

public class UserAddressDtoBuilder
{
    private string _addressLine = "123 Main St";
    private string? _addressSupplement = null;
    private string _city = "Metropolis";
    private string _zipCode = "12345";
    private string? _stateProvince = null;
    private string _country = "USA";

    public UserAddressDtoBuilder WithAddressLine(string v) { _addressLine = v; return this; }
    public UserAddressDtoBuilder WithAddressSupplement(string? v) { _addressSupplement = v; return this; }
    public UserAddressDtoBuilder WithCity(string v) { _city = v; return this; }
    public UserAddressDtoBuilder WithZipCode(string v) { _zipCode = v; return this; }
    public UserAddressDtoBuilder WithStateProvince(string? v) { _stateProvince = v; return this; }
    public UserAddressDtoBuilder WithCountry(string v) { _country = v; return this; }

    public UserAddressDto Build() => new()
    {
        AddressLine = _addressLine,
        AddressSupplement = _addressSupplement,
        City = _city,
        ZipCode = _zipCode,
        StateProvince = _stateProvince,
        Country = _country
    };
}

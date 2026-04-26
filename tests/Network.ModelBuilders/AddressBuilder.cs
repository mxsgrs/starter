using Network.Domain.Aggregates.UserAggregate;

namespace Network.ModelBuilders;

public class AddressBuilder
{
    private string _addressLine = "123 Main St";
    private string _city = "City";
    private string _zipCode = "12345";
    private string _country = "Country";
    private string? _addressSupplement = null;
    private string? _stateProvince = null;

    public AddressBuilder WithAddressLine(string v) { _addressLine = v; return this; }
    public AddressBuilder WithCity(string v) { _city = v; return this; }
    public AddressBuilder WithZipCode(string v) { _zipCode = v; return this; }
    public AddressBuilder WithCountry(string v) { _country = v; return this; }
    public AddressBuilder WithAddressSupplement(string? v) { _addressSupplement = v; return this; }
    public AddressBuilder WithStateProvince(string? v) { _stateProvince = v; return this; }

    public Address Build() =>
        Address.Create(_addressLine, _city, _zipCode, _country, _addressSupplement, _stateProvince).Value;
}

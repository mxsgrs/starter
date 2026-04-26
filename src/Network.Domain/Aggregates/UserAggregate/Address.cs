using Network.Domain;

namespace Network.Domain.Aggregates.UserAggregate;

public class Address : ValueObject<Address>
{
    #region Properties

    public string AddressLine { get; private set; } = "";
    public string? AddressSupplement { get; private set; }
    public string City { get; private set; } = "";
    public string ZipCode { get; private set; } = "";
    public string? StateProvince { get; private set; }
    public string Country { get; private set; } = "";

    #endregion

    #region Create

    /// <summary>
    /// Method for creating an instance under business rules
    /// </summary>
    public static Result<Address> Create(string addressLine, string city, string zipCode,
        string country, string? addressSupplement = null, string? stateProvince = null)
    {
        Address address = new()
        {
            AddressLine = addressLine,
            AddressSupplement = addressSupplement,
            City = city,
            ZipCode = zipCode,
            StateProvince = stateProvince,
            Country = country
        };

        Result validationResult = Validate(address);

        return validationResult.IsSuccess
            ? Result.Ok(address)
            : Result.Fail<Address>(validationResult.Errors);
    }

    #endregion

    #region UpdateFrom

    internal void UpdateFrom(Address other)
    {
        AddressLine = other.AddressLine;
        AddressSupplement = other.AddressSupplement;
        City = other.City;
        ZipCode = other.ZipCode;
        StateProvince = other.StateProvince;
        Country = other.Country;
    }

    #endregion

    #region GetEqualityComponents

    /// <summary>
    /// Two value objects are equal if all their properties are equal.
    /// </summary>
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return AddressLine;
        yield return AddressSupplement ?? string.Empty;
        yield return City;
        yield return ZipCode;
        yield return StateProvince ?? string.Empty;
        yield return Country;
    }

    #endregion

    #region Private Constructor

    /// <summary>
    /// Constructor for EF Core
    /// </summary>
    private Address() { }

    #endregion
}

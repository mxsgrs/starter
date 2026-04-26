using System.Text.Json.Serialization;

namespace Network.Application.Dtos.UserDtos;

public record UserDto
{
    public Guid Id { get; init; }
    public required string EmailAddress { get; init; }
    public required string HashedPassword { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public DateOnly Birthday { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Role Role { get; init; }

    public required string Phone { get; init; }
    public UserAddressDto? Address { get; init; }
}

public record UserWriteDto
{
    public required string EmailAddress { get; init; }
    public required string HashedPassword { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public DateOnly Birthday { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Role Role { get; init; }

    public required string Phone { get; init; }
    public UserAddressDto? Address { get; init; }
}

public record UserAddressDto
{
    public required string AddressLine { get; init; }
    public string? AddressSupplement { get; init; }
    public required string City { get; init; }
    public required string ZipCode { get; init; }
    public string? StateProvince { get; init; }
    public required string Country { get; init; }
}

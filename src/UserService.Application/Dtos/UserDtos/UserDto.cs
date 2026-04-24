using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UserService.Domain.Validations;

namespace UserService.Application.Dtos.UserDtos;

public record UserDto
{
    public Guid Id { get; init; }

    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address.")]
    public required string EmailAddress { get; init; }
           
    public required string HashedPassword { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    [NotInFuture]
    public DateOnly Birthday { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Role Role { get; init; }

    [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "The phone number must be between 10 and 15 digits and may include a leading +.")]
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

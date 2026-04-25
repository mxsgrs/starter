namespace UserService.Application.Dtos.UserDtos.Helpers;

public static class UserDtoHelper
{
    /// <summary>
    /// Converts a user address data transfer object to a domain address instance.
    /// </summary>
    /// <remarks>Use this method to transform data received from external sources into a validated domain
    /// address object. The returned result indicates whether the conversion and validation succeeded.</remarks>
    /// <param name="dto">The user address data transfer object containing address information to convert. Cannot be null.</param>
    /// <returns>A result containing the created address if the conversion is successful; otherwise, a result with validation
    /// errors.</returns>
    public static Result<Address> ToAddress(UserAddressDto dto)
    {
        return Address.Create(
            dto.AddressLine,
            dto.City,
            dto.ZipCode,
            dto.Country,
            dto.AddressSupplement,
            dto.StateProvince
        );
    }

    /// <summary>
    /// Converts a <see cref="UserDto"/> instance to a <see cref="User"/> domain object, validating and mapping all
    /// required fields.
    /// </summary>
    /// <remarks>If the address conversion fails, the returned result will contain the corresponding errors.
    /// All required fields in <paramref name="dto"/> must be populated for successful conversion.</remarks>
    /// <param name="dto">The data transfer object containing user information to be converted. Cannot be null.</param>
    /// <returns>A <see cref="Result{User}"/> containing the created <see cref="User"/> if the conversion succeeds; otherwise, a
    /// failed result with error details.</returns>
    public static Result<User> ToUser(UserDto dto)
    {
        Result<Address> address = ToAddress(dto.Address!);

        if (address.IsFailed)
        {
            return Result.Fail<User>(address.Errors);
        }

        return User.Create(
            dto.Id,
            dto.EmailAddress,
            dto.HashedPassword,
            dto.FirstName,
            dto.LastName,
            dto.Birthday,
            dto.Gender,
            dto.Role,
            dto.Phone,
            address.Value
        );
    }
}

namespace Network.Application.Dtos.UserDtos.Helpers;

public static class UserDtoHelper
{
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
    
    public static Result<User> ToUser(Guid id, UserWriteDto dto)
    {
        Result<Address> address = ToAddress(dto.Address!);

        if (address.IsFailed)
        {
            return Result.Fail<User>(address.Errors);
        }

        return User.Create(
            id,
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

    public static Result ApplyUpdate(User user, UserWriteDto dto)
    {
        Result<Address> address = ToAddress(dto.Address!);

        if (address.IsFailed)
        {
            return Result.Fail(address.Errors);
        }

        return user.Update(
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

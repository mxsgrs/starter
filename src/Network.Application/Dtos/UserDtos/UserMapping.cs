namespace Network.Application.Dtos.UserDtos;

public static class UserMapping
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Address, UserAddressDto>();
        config.NewConfig<User, UserDto>();
    }
}

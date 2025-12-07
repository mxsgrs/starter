namespace UserService.Application.Dtos.UserDtos;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();

        CreateMap<Address, UserAddressDto>();
        CreateMap<UserAddressDto, Address>();
    }
}
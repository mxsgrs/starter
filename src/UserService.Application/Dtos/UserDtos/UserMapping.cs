namespace UserService.Application.Dtos.UserDtos;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>();

        CreateMap<Address, UserAddressDto>();
    }
}
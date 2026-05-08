using Network.Application.Users.Dtos;
using Network.Domain.Aggregates.UserAggregate;
using Network.ModelBuilders.Shared;

namespace Network.ModelBuilders.Dtos.UserDtos;

public class UserDtoBuilder : IModelBuilder<UserDto>
{
    private Guid _id = Guid.NewGuid();
    private string _emailAddress = "test@example.com";
    private string _hashedPassword = "hashedPassword";
    private string _firstName = "John";
    private string _lastName = "Doe";
    private DateOnly _birthday = new(1990, 1, 1);
    private Gender _gender = Gender.Male;
    private Role _role = Role.User;
    private string _phone = "+1234567890";
    private UserAddressDto? _address = new UserAddressDtoBuilder().Build();

    public UserDtoBuilder WithId(Guid v) { _id = v; return this; }
    public UserDtoBuilder WithEmailAddress(string v) { _emailAddress = v; return this; }
    public UserDtoBuilder WithHashedPassword(string v) { _hashedPassword = v; return this; }
    public UserDtoBuilder WithFirstName(string v) { _firstName = v; return this; }
    public UserDtoBuilder WithLastName(string v) { _lastName = v; return this; }
    public UserDtoBuilder WithBirthday(DateOnly v) { _birthday = v; return this; }
    public UserDtoBuilder WithGender(Gender v) { _gender = v; return this; }
    public UserDtoBuilder WithRole(Role v) { _role = v; return this; }
    public UserDtoBuilder WithPhone(string v) { _phone = v; return this; }
    public UserDtoBuilder WithAddress(UserAddressDto? v) { _address = v; return this; }

    public UserDto Build() => new()
    {
        Id = _id,
        EmailAddress = _emailAddress,
        HashedPassword = _hashedPassword,
        FirstName = _firstName,
        LastName = _lastName,
        Birthday = _birthday,
        Gender = _gender,
        Role = _role,
        Phone = _phone,
        Address = _address
    };
}

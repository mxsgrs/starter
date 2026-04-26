using Network.Application.Dtos.UserDtos;
using Network.Domain.Aggregates.UserAggregate;

namespace Network.ModelBuilders;

public class UserWriteDtoBuilder
{
    private string _emailAddress = "test@example.com";
    private string _hashedPassword = "hashedPassword";
    private string _firstName = "John";
    private string _lastName = "Doe";
    private DateOnly _birthday = new(1990, 1, 1);
    private Gender _gender = Gender.Male;
    private Role _role = Role.User;
    private string _phone = "+1234567890";
    private UserAddressDto? _address = new UserAddressDtoBuilder().Build();

    public UserWriteDtoBuilder WithEmailAddress(string v) { _emailAddress = v; return this; }
    public UserWriteDtoBuilder WithHashedPassword(string v) { _hashedPassword = v; return this; }
    public UserWriteDtoBuilder WithFirstName(string v) { _firstName = v; return this; }
    public UserWriteDtoBuilder WithLastName(string v) { _lastName = v; return this; }
    public UserWriteDtoBuilder WithBirthday(DateOnly v) { _birthday = v; return this; }
    public UserWriteDtoBuilder WithGender(Gender v) { _gender = v; return this; }
    public UserWriteDtoBuilder WithRole(Role v) { _role = v; return this; }
    public UserWriteDtoBuilder WithPhone(string v) { _phone = v; return this; }
    public UserWriteDtoBuilder WithAddress(UserAddressDto? v) { _address = v; return this; }

    public UserWriteDto Build() => new()
    {
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

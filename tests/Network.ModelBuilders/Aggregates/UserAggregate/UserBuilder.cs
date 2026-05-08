using FluentResults;
using Network.Domain.Aggregates.UserAggregate;
using Network.ModelBuilders.Shared;

namespace Network.ModelBuilders.Aggregates.UserAggregate;

public class UserBuilder : IModelBuilder<User>
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
    private Address _address = new AddressBuilder().Build();

    public UserBuilder WithId(Guid v) { _id = v; return this; }
    public UserBuilder WithEmailAddress(string v) { _emailAddress = v; return this; }
    public UserBuilder WithHashedPassword(string v) { _hashedPassword = v; return this; }
    public UserBuilder WithFirstName(string v) { _firstName = v; return this; }
    public UserBuilder WithLastName(string v) { _lastName = v; return this; }
    public UserBuilder WithBirthday(DateOnly v) { _birthday = v; return this; }
    public UserBuilder WithGender(Gender v) { _gender = v; return this; }
    public UserBuilder WithRole(Role v) { _role = v; return this; }
    public UserBuilder WithPhone(string v) { _phone = v; return this; }
    public UserBuilder WithAddress(Address v) { _address = v; return this; }

    public Result<User> BuildResult() =>
        User.Create(_id, _emailAddress, _hashedPassword, _firstName, _lastName,
            _birthday, _gender, _role, _phone, _address);

    public Result<User> BuildUpdateResult()
    {
        Result<User> createResult = User.Create(_id, _emailAddress, _hashedPassword, _firstName, _lastName,
            _birthday, _gender, _role, _phone, _address);
        if (createResult.IsFailed) return createResult;

        User user = createResult.Value;
        user.ClearDomainEvents();
        Result updateResult = user.Update(_emailAddress, _hashedPassword, _firstName, _lastName,
            _birthday, _gender, _role, _phone, _address);

        return updateResult.IsFailed ? Result.Fail<User>(updateResult.Errors) : Result.Ok(user);
    }

    public User Build() =>
        User.Create(_id, _emailAddress, _hashedPassword, _firstName, _lastName,
            _birthday, _gender, _role, _phone, _address).Value;
}

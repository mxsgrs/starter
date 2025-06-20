using System.ComponentModel.DataAnnotations;
using UserService.Domain.Validations;

namespace UserService.Domain.Aggregates.UserAggregate;

public class User : AggregateRoot
{
    public Guid Id { get; private set; }

    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address.")]
    public string EmailAddress { get; private set; } = "";

    [MaxLength(256)]
    public string HashedPassword { get; private set; } = "";

    [MaxLength(128)]
    public string FirstName { get; private set; } = "";

    [MaxLength(128)]
    public string LastName { get; private set; } = "";

    [NotInFuture]
    public DateOnly Birthday { get; private set; }

    public Gender Gender { get; private set; }

    public Role Role { get; private set; }

    [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "The phone number must be between 10 and 15 digits and may include a leading +.")]
    public string Phone { get; private set; } = "";

    public Address Address { get; private set; }

    #region Constructors
    /// <summary>
    /// Standard constructor for creating a new user
    /// </summary>
    public Result<User> Create(Guid id, string emailAddress, string hashedPassword, string firstName,
        string lastName, DateOnly birthday, Gender gender, Role role, string phone, Address address)
    {
        Id = id;
        EmailAddress = emailAddress;
        HashedPassword = hashedPassword;
        FirstName = firstName;
        LastName = lastName;
        Birthday = birthday;
        Gender = gender;
        Role = role;
        Phone = phone;
        Address = address;

        Result validationResult = Validate(this);

        return validationResult.IsSuccess
            ? Result.Ok(this)
            : Result.Fail<User>(validationResult.Errors);
    }

    /// <summary>
    /// Public constructor for automatic mapping
    /// </summary>
    public User(Guid id, string emailAddress, string hashedPassword, string firstName,
        string lastName, DateOnly birthday, Gender gender, Role role, string phone, Address address)
    {
        Id = id;
        EmailAddress = emailAddress;
        HashedPassword = hashedPassword;
        FirstName = firstName;
        LastName = lastName;
        Birthday = birthday;
        Gender = gender;
        Role = role;
        Phone = phone;
        Address = address;

        ThrowIfInvalid(this);
    }

    /// <summary>
    /// Constructor for EF migration
    /// </summary>
    private User() : this(Guid.NewGuid(), null!, null!, null!, null!, new DateOnly(),
        Gender.Male, Role.Admin, null!, null!)
    { } 
    #endregion
}

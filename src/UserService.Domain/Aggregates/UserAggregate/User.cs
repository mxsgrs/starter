using System.ComponentModel.DataAnnotations;
using UserService.Domain.Validations;

namespace UserService.Domain.Aggregates.UserAggregate;

public class User : AggregateRoot
{
    #region Properties

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

    public Address Address { get; private set; } = null!;

    #endregion

    #region Methods

    /// <summary>
    /// Method for creating an instance under business rules
    /// </summary>
    public static Result<User> Create(Guid id, string emailAddress, string hashedPassword,
        string firstName, string lastName, DateOnly birthday, Gender gender, Role role,
        string phone, Address address)
    {
        User user = new()
        {
            Id = id,
            EmailAddress = emailAddress,
            HashedPassword = hashedPassword,
            FirstName = firstName,
            LastName = lastName,
            Birthday = birthday,
            Gender = gender,
            Role = role,
            Phone = phone,
            Address = address
        };

        Result validationResult = Validate(user);

        return validationResult.IsSuccess
            ? Result.Ok(user)
            : Result.Fail<User>(validationResult.Errors);
    }

    /// <summary>
    /// Constructor for EF Core
    /// </summary>
    private User() { }

    #endregion
}

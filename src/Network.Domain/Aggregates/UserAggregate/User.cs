namespace Network.Domain.Aggregates.UserAggregate;

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

    public int Age
    {
        get
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - Birthday.Year;
            if (Birthday > today.AddYears(-age)) age--;
            return age;
        }
    }

    #endregion

    #region Create

    /// <summary>
    /// Method for creating an instance under business rules
    /// </summary>
    public static Result<User> Create(
        string emailAddress,
        string hashedPassword,
        string firstName,
        string lastName,
        DateOnly birthday,
        Gender gender,
        Role role,
        string phone,
        Address address
    )
    {
        User user = new()
        {
            Id = Guid.NewGuid(),
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

        if (!validationResult.IsSuccess) return Result.Fail<User>(validationResult.Errors);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
        return Result.Ok(user);
    }

    #endregion

    #region Update

    /// <summary>
    /// Updates the entity's values under business rules without changing its identity.
    /// </summary>
    public Result Update(
        string emailAddress,
        string hashedPassword,
        string firstName,
        string lastName,
        DateOnly birthday,
        Gender gender,
        Role role,
        string phone,
        Address address
    )
    {
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

        if (!validationResult.IsSuccess) return Result.Fail(validationResult.Errors);

        RaiseDomainEvent(new UserUpdatedDomainEvent(Id));
        return Result.Ok();
    }

    #endregion

    #region Delete

    /// <summary>
    /// Marks the user for deletion and raises the corresponding domain event.
    /// </summary>
    public void Delete()
    {
        RaiseDomainEvent(new UserDeletedDomainEvent(Id));
    }

    #endregion

    #region Private Constructor

    /// <summary>
    /// Constructor for EF Core
    /// </summary>
    private User() { }

    #endregion
}

namespace UserService.Domain.UnitTests.Facts.Aggregates;

public class UserUpdateTests
{
    private static User CreateValidUser() => new UserBuilder().Build();

    [Fact]
    public void Update_ShouldModifyValues_WhenInputsAreValid()
    {
        // Arrange
        User user = CreateValidUser();
        Address newAddress = new AddressBuilder().Build();

        // Act
        Result result = user.Update(
            "valid@example.com", "hashedPassword123", "Jane", "Smith",
            new DateOnly(1985, 6, 15), Gender.Female, Role.Admin, "+9876543210", newAddress);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("valid@example.com", user.EmailAddress);
        Assert.Equal("Jane", user.FirstName);
        Assert.Equal("Smith", user.LastName);
        Assert.Equal(new DateOnly(1985, 6, 15), user.Birthday);
        Assert.Equal(Gender.Female, user.Gender);
        Assert.Equal(Role.Admin, user.Role);
        Assert.Equal("+9876543210", user.Phone);
    }

    [Fact]
    public void Update_ShouldNotChangeId_WhenInputsAreValid()
    {
        // Arrange
        User user = CreateValidUser();
        Guid originalId = user.Id;
        Address newAddress = new AddressBuilder().Build();

        // Act
        user.Update(
            user.EmailAddress, user.HashedPassword, "Jane", user.LastName,
            user.Birthday, user.Gender, user.Role, user.Phone, newAddress);

        // Assert
        Assert.Equal(originalId, user.Id);
    }

    [Fact]
    public void Update_ShouldReturnFail_WhenBirthdayIsInFuture()
    {
        // Arrange
        User user = CreateValidUser();
        Address address = new AddressBuilder().Build();

        // Act
        Result result = user.Update(
            user.EmailAddress, user.HashedPassword, user.FirstName, user.LastName,
            DateOnly.FromDateTime(DateTime.Today.AddYears(1)), user.Gender, user.Role, user.Phone, address);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Update_ShouldReturnFail_WhenEmailIsInvalid()
    {
        // Arrange
        User user = CreateValidUser();
        Address address = new AddressBuilder().Build();

        // Act
        Result result = user.Update(
            "invalid-email", user.HashedPassword, user.FirstName, user.LastName,
            user.Birthday, user.Gender, user.Role, user.Phone, address);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Update_ShouldReturnFail_WhenPhoneIsInvalid()
    {
        // Arrange
        User user = CreateValidUser();
        Address address = new AddressBuilder().Build();

        // Act
        Result result = user.Update(
            user.EmailAddress, user.HashedPassword, user.FirstName, user.LastName,
            user.Birthday, user.Gender, user.Role, "123", address);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Update_ShouldReturnFail_WhenFirstNameExceedsMaxLength()
    {
        // Arrange
        User user = CreateValidUser();
        Address address = new AddressBuilder().Build();

        // Act
        Result result = user.Update(
            user.EmailAddress, user.HashedPassword, new string('a', 129), user.LastName,
            user.Birthday, user.Gender, user.Role, user.Phone, address);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Update_ShouldRaiseUserUpdatedDomainEvent_WhenInputsAreValid()
    {
        // Arrange
        User user = CreateValidUser();
        user.ClearDomainEvents();
        Address address = new AddressBuilder().Build();

        // Act
        user.Update(
            user.EmailAddress, user.HashedPassword, user.FirstName, user.LastName,
            user.Birthday, user.Gender, user.Role, user.Phone, address);

        // Assert
        UserUpdatedDomainEvent domainEvent = Assert.Single(user.DomainEvents.OfType<UserUpdatedDomainEvent>());
        Assert.Equal(user.Id, domainEvent.UserId);
        Assert.NotEqual(Guid.Empty, domainEvent.Id);
        Assert.True(domainEvent.CreatedOn <= DateTime.UtcNow);
    }
}

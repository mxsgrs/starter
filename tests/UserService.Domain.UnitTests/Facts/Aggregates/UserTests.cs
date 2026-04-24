namespace UserService.Domain.UnitTests.Facts.Aggregates;

public class UserTests
{
    [Fact]
    public void Create_ShouldReturnOk_WhenInputsAreValid()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Address address = new AddressBuilder().Build();

        // Act
        Result<User> result = new UserBuilder()
            .WithId(id)
            .WithEmailAddress("valid@example.com")
            .WithHashedPassword("hashedPassword123")
            .WithFirstName("Jane")
            .WithLastName("Smith")
            .WithBirthday(new DateOnly(1985, 6, 15))
            .WithGender(Gender.Female)
            .WithRole(Role.Admin)
            .WithPhone("+9876543210")
            .WithAddress(address)
            .BuildResult();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal("valid@example.com", result.Value.EmailAddress);
        Assert.Equal("Jane", result.Value.FirstName);
        Assert.Equal("Smith", result.Value.LastName);
        Assert.Equal(new DateOnly(1985, 6, 15), result.Value.Birthday);
        Assert.Equal(Gender.Female, result.Value.Gender);
        Assert.Equal(Role.Admin, result.Value.Role);
        Assert.Equal("+9876543210", result.Value.Phone);
        Assert.Equal(address, result.Value.Address);
    }

    [Fact]
    public void Create_ShouldReturnFail_WhenBirthdayIsInFuture()
    {
        // Act
        Result<User> result = new UserBuilder()
            .WithBirthday(DateOnly.FromDateTime(DateTime.Today.AddYears(1)))
            .BuildResult();

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Create_ShouldReturnFail_WhenEmailIsInvalid()
    {
        // Act
        Result<User> result = new UserBuilder()
            .WithEmailAddress("invalid-email")
            .BuildResult();

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Create_ShouldReturnFail_WhenPhoneIsInvalid()
    {
        // Act
        Result<User> result = new UserBuilder()
            .WithPhone("123")
            .BuildResult();

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Create_ShouldReturnFail_WhenFirstNameExceedsMaxLength()
    {
        // Act
        Result<User> result = new UserBuilder()
            .WithFirstName(new string('a', 129))
            .BuildResult();

        // Assert
        Assert.True(result.IsFailed);
    }
}

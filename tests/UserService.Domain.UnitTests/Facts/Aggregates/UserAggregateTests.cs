using UserService.Domain.Aggregates.UserAggregate;
using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.UnitTests.Facts.Aggregates;

public class UserTests
{
    [Fact]
    public void Constructor_WithFutureBirthday_ShouldThrowValidationException()
    {
        // Arrange
        DateOnly futureDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

        // Act & Assert
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new UserBuilder().WithBirthday(futureDate).Build()
        );

        Assert.Equal("User is not valid: Birthday date can't be in the future.", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidEmail_ShouldThrowValidationException()
    {
        // Arrange
        string invalidEmail = "invalid-email";

        // Act & Assert
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new UserBuilder().WithEmailAddress(invalidEmail).Build()
        );

        Assert.Equal("User is not valid: Invalid email address.", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidPhone_ShouldThrowValidationException()
    {
        // Arrange
        string invalidPhone = "123";

        // Act & Assert
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new UserBuilder().WithPhone(invalidPhone).Build()
        );

        Assert.Equal("User is not valid: The phone number must be between 10 and 15 digits and may include a leading +.", exception.Message);
    }

    [Fact]
    public void Constructor_WithLongFirstName_ShouldThrowValidationException()
    {
        // Arrange
        string longFirstName = new('a', 129); // More than 128 characters

        // Act & Assert
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new UserBuilder().WithFirstName(longFirstName).Build()
        );

        Assert.Equal("User is not valid: The field FirstName must be a string or array type with a maximum length of '128'.", exception.Message);
    }

    [Fact]
    public void Constructor_WithValidInputs_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string emailAddress = "valid@example.com";
        string hashedPassword = "hashedPassword123";
        string firstName = "Jane";
        string lastName = "Smith";
        DateOnly birthday = new(1985, 6, 15);
        Gender gender = Gender.Female;
        Role role = Role.Admin;
        string phone = "+9876543210";
        Address address = new AddressBuilder()
            .WithAddressLine("42 Oak Avenue")
            .WithCity("Springfield")
            .WithZipCode("67890")
            .WithCountry("Canada")
            .Build();

        // Act
        User user = new UserBuilder()
            .WithId(id)
            .WithEmailAddress(emailAddress)
            .WithHashedPassword(hashedPassword)
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .WithBirthday(birthday)
            .WithGender(gender)
            .WithRole(role)
            .WithPhone(phone)
            .WithAddress(address)
            .Build();

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(emailAddress, user.EmailAddress);
        Assert.Equal(hashedPassword, user.HashedPassword);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(birthday, user.Birthday);
        Assert.Equal(gender, user.Gender);
        Assert.Equal(role, user.Role);
        Assert.Equal(phone, user.Phone);
        Assert.Equal(address, user.Address);
    }
}

﻿namespace UserService.Infrastructure.UnitTests.Facts.Repositories;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task CreateUser_ShouldAddUserToDatabase()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        User user = new(
            Guid.NewGuid(),
            "test@example.com",
            "hashedPassword",
            "John",
            "Doe",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            Role.User,
            "+1234567890",
            new Address("Street", "City", "State", "12345", "Country")
        );

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        _ = await repository.CreateUser(user);

        // Assert
        User? storedUser = await dbContext.Users.FindAsync(user.Id);
        Assert.NotNull(storedUser);
        Assert.Equal("test@example.com", storedUser.EmailAddress);
    }

    [Fact]
    public async Task ReadUser_ById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        User user = new(
            Guid.NewGuid(),
            "test@example.com",
            "hashedPassword",
            "John",
            "Doe",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            Role.User,
            "+1234567890",
            new Address("Street", "City", "State", "12345", "Country")
        );

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.ReadUser(user.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.EmailAddress, result.Value.EmailAddress);
    }

    [Fact]
    public async Task ReadUser_ById_ShouldReturnsFailure_WhenUserDoesNotExist()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        Guid userId = Guid.NewGuid();

        // Act
        Result<User> result = await repository.ReadUser(userId);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("User not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task ReadUser_ByEmailAndPassword_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();

        User user = new(
            Guid.NewGuid(),
            "test@example.com",
            "hashedPassword",
            "John",
            "Doe",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            Role.User,
            "+1234567890",
            new Address("Street", "City", "State", "12345", "Country")
        );

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.ReadUser("test@example.com", "hashedPassword");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Value.Id);
    }

    [Fact]
    public async Task ReadUser_ByEmailAndPassword_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        string email = "nonexistent@example.com";
        string password = "wrongPassword";

        // Act
        Result<User> result = await repository.ReadUser(email, password);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("User not found", result.Errors[0].Message);
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUserDetails()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();

        User user = new(
            Guid.NewGuid(),
            "test@example.com",
            "hashedPassword",
            "John",
            "Doe",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            Role.User,
            "+1234567890",
            new Address("Street", "City", "State", "12345", "Country")
        );

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        User updatedUser = new(
            user.Id,
            "test@example.com",
            "hashedPassword",
            "Jane",
            "Doe",
            new DateOnly(1991, 2, 2),
            Gender.Female,
            Role.Admin,
            "+0987654321",
            new Address("New Street", "New City", "New State", "54321", "New Country")
        );

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.UpdateUser(user.Id, updatedUser);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Jane", result.Value.FirstName);
        Assert.Equal("New Street", result.Value.Address.AddressLine);
    }
}
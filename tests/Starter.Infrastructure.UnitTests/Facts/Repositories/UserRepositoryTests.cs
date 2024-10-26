using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Starter.Domain.Aggregates.UserAggregate;
using Starter.Infrastructure.Persistance.Repositories;
using Starter.Infrastructure.Persistance;
using Moq;

namespace Starter.Infrastructure.UnitTests.Facts.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly StarterDbContext _context;
    private readonly Mock<ILogger<UserRepository>> _loggerMock;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StarterDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new StarterDbContext(options);
        _loggerMock = new Mock<ILogger<UserRepository>>();
        _repository = new UserRepository(_loggerMock.Object, _context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateUser_ShouldAddUserToDatabase()
    {
        // Arrange
        var user = new User(
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

        // Act
        var result = await _repository.CreateUser(user);

        // Assert
        var storedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(storedUser);
        Assert.Equal("test@example.com", storedUser.EmailAddress);
    }

    [Fact]
    public async Task ReadUser_ById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User(
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

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ReadUser(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.EmailAddress, result.EmailAddress);
    }

    [Fact]
    public async Task ReadUser_ById_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => _repository.ReadUser(userId));
    }

    [Fact]
    public async Task ReadUser_ByEmailAndPassword_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User(
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

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ReadUser("test@example.com", "hashedPassword");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task ReadUser_ByEmailAndPassword_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "wrongPassword";

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => _repository.ReadUser(email, password));
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUserDetails()
    {
        // Arrange
        var user = new User(
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

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var updatedUser = new User(
            user.Id,
            "updated@example.com",
            "newHashedPassword",
            "Jane",
            "Doe",
            new DateOnly(1991, 2, 2),
            Gender.Female,
            Role.Admin,
            "+0987654321",
            new Address("New Street", "New City", "New State", "54321", "New Country")
        );

        // Act
        var result = await _repository.UpdateUser(user.Id, updatedUser);

        // Assert
        Assert.Equal("updated@example.com", result.EmailAddress);
        Assert.Equal("newHashedPassword", result.HashedPassword);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("New Street", result.Address.AddressLine);
    }
}
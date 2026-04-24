namespace UserService.Infrastructure.UnitTests.Facts.Repositories;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task CreateUser_ShouldAddUserToDatabase()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();

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
        User user = new UserBuilder().Build();

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
        User user = new UserBuilder().Build();

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
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        User updatedUser = new UserBuilder()
            .WithId(user.Id)
            .WithFirstName("Jane")
            .WithBirthday(new DateOnly(1991, 2, 2))
            .WithGender(Gender.Female)
            .WithRole(Role.Admin)
            .WithPhone("+0987654321")
            .WithAddress(new AddressBuilder()
                .WithAddressLine("New Street")
                .WithCity("New City")
                .WithStateProvince("New State")
                .WithZipCode("54321")
                .WithCountry("New Country")
                .Build())
            .Build();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        Result<User> result = await repository.UpdateUser(user.Id, updatedUser);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Jane", result.Value.FirstName);
        Assert.Equal("New Street", result.Value.Address.AddressLine);
    }
}

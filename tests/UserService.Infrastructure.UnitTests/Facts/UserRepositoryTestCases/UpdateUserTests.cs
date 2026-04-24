namespace UserService.Infrastructure.UnitTests.Facts.UserRepositoryTestCases;

public class UpdateUserTests
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

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

    [Fact]
    public async Task UpdateUser_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        User nonExistent = new UserBuilder()
            .WithEmailAddress("nonexistent@example.com")
            .WithHashedPassword("anyHash")
            .Build();

        // Act
        Result<User> result = await repository.UpdateUser(Guid.NewGuid(), nonExistent);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("User not found", result.Errors[0].Message);
    }
}

namespace Network.Infrastructure.UnitTests.Facts.UserRepositoryTestCases;

public class SaveChangesTests
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task SaveChanges_ShouldUpdateUserDetails()
    {
        // Arrange
        UserDbContext dbContext = SharedFixture.CreateDatabaseContext();
        User user = new UserBuilder().Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        Result<User> readResult = await repository.ReadTrackedUser(user.Id);
        User trackedUser = readResult.Value;

        Address newAddress = new AddressBuilder()
            .WithAddressLine("New Street")
            .WithCity("New City")
            .WithStateProvince("New State")
            .WithZipCode("54321")
            .WithCountry("New Country")
            .Build();

        trackedUser.Update(
            trackedUser.EmailAddress, trackedUser.HashedPassword,
            "Jane", trackedUser.LastName,
            new DateOnly(1991, 2, 2), Gender.Female, Role.Admin, "+0987654321", newAddress);

        // Act
        Result result = await repository.SaveChanges();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Jane", trackedUser.FirstName);
        Assert.Equal("New Street", trackedUser.Address.AddressLine);
    }
}

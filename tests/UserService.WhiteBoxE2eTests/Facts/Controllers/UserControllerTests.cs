using System.Net.Http.Json;
using UserService.Application.Dtos.UserDtos;
using UserService.WhiteBoxE2eTests.Facts.Factories;

namespace UserService.WhiteBoxE2eTests.Facts.Controllers;

public class UserControllerTests(StarterWebApplicationFactory factory)
    : IClassFixture<StarterWebApplicationFactory>
{
    //[Fact]
    //public async Task CreateUser_ShouldReturnOk_WhenUserIsCreated()
    //{
    //    // Arrange
    //    factory.MigrateDbContext();
    //    HttpClient client = factory.CreateClient();
    //    UserWriteDto writeDto = new UserWriteDtoBuilder().Build();

    //    // Act
    //    HttpResponseMessage response = await client.PostAsJsonAsync("/api/user", writeDto);

    //    // Assert
    //    response.EnsureSuccessStatusCode();
    //    Guid createdId = await response.Content.ReadFromJsonAsync<Guid>(JsonOptions.Default);
    //    Assert.NotEqual(Guid.Empty, createdId);
    //}

    [Fact]
    public async Task ReadUser_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        UserDbContext dbContext = factory.MigrateDbContext();
        User user = new UserBuilder()
            .WithEmailAddress("john.doe@example.com")
            .WithHashedPassword("TWF0cml4UmVsb2FkZWQh")
            .WithFirstName("FirstName")
            .WithLastName("LastName")
            .WithAddress(new AddressBuilder()
                .WithAddressLine("123 Test St")
                .WithCity("TestCity")
                .WithZipCode("12345")
                .WithCountry("TestCountry")
                .Build())
            .Build();

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        HttpClient client = factory.CreateAuthorizedClient();

        // Act
        HttpResponseMessage response = await client.SendAsync(new(HttpMethod.Get, $"/api/user/{user.Id}"));

        // Assert
        response.EnsureSuccessStatusCode();
        UserDto? dto = await response.Content.ReadFromJsonAsync<UserDto>(JsonOptions.Default);
        Assert.NotNull(dto);
        Assert.Equal(user.Id, dto.Id);
        Assert.Equal("john.doe@example.com", dto.EmailAddress);
        Assert.Equal("FirstName", dto.FirstName);
        Assert.Equal("123 Test St", dto.Address!.AddressLine);
    }

    //[Fact]
    //public async Task UpdateUser_ShouldReturnNoContent_WhenUserIsUpdated()
    //{
    //    // Arrange
    //    UserDbContext dbContext = factory.MigrateDbContext();
    //    User user = new UserBuilder().Build();
    //    await dbContext.Users.AddAsync(user);
    //    await dbContext.SaveChangesAsync();

    //    HttpClient client = factory.CreateAuthorizedClient();
    //    UserWriteDto updateDto = new UserWriteDtoBuilder().WithFirstName("Jane").Build();

    //    // Act
    //    HttpResponseMessage response = await client.PutAsJsonAsync($"/api/user/{user.Id}", updateDto);

    //    // Assert
    //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    //}
}

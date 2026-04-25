using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using UserService.Application.Dtos.UserDtos;
using UserService.WhiteBoxE2eTests.Facts.Factories;

namespace UserService.WhiteBoxE2eTests.Facts.Controllers;

public class UserControllerTests(StarterWebApplicationFactory factory)
    : IClassFixture<StarterWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateAuthorizedClient();
    private readonly UserDbContext _dbContext = factory.MigrateDbContext();

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Users.ExecuteDeleteAsync();
        factory.DomainEventPublisher.Clear();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnOk_WhenUserIsCreated()
    {
        // Arrange
        UserWriteDto writeDto = new UserWriteDtoBuilder().Build();

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/user", writeDto);

        // Assert
        response.EnsureSuccessStatusCode();
        Guid createdId = await response.Content.ReadFromJsonAsync<Guid>(JsonOptions.Default);
        Assert.NotEqual(Guid.Empty, createdId);
        IEnumerable<UserCreatedDomainEvent> publishedEvents = factory.DomainEventPublisher
            .PublishedEvents.OfType<UserCreatedDomainEvent>();
        UserCreatedDomainEvent domainEvent = Assert.Single(publishedEvents);
        Assert.Equal(createdId, domainEvent.UserId);
    }

    [Fact]
    public async Task ReadUser_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
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

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage response = await _client.SendAsync(new(HttpMethod.Get, $"/api/user/{user.Id}"));

        // Assert
        response.EnsureSuccessStatusCode();
        UserDto? dto = await response.Content.ReadFromJsonAsync<UserDto>(JsonOptions.Default);
        Assert.NotNull(dto);
        Assert.Equal(user.Id, dto.Id);
        Assert.Equal("john.doe@example.com", dto.EmailAddress);
        Assert.Equal("FirstName", dto.FirstName);
        Assert.Equal("123 Test St", dto.Address!.AddressLine);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnNoContent_WhenUserIsUpdated()
    {
        // Arrange
        User user = new UserBuilder().Build();
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        UserWriteDto updateDto = new UserWriteDtoBuilder().WithFirstName("Jane").Build();

        // Act
        HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/user/{user.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        IEnumerable<UserUpdatedDomainEvent> publishedEvents = factory.DomainEventPublisher
            .PublishedEvents.OfType<UserUpdatedDomainEvent>();
        UserUpdatedDomainEvent domainEvent = Assert.Single(publishedEvents);
        Assert.Equal(user.Id, domainEvent.UserId);
    }
}

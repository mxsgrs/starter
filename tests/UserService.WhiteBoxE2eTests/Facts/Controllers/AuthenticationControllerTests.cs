using UserService.Application.Dtos;
using UserService.WhiteBoxE2eTests.Facts.Factories;

namespace UserService.WhiteBoxE2eTests.Facts.Controllers;

public class AuthenticationControllerTests(StarterWebApplicationFactory factory)
    : IClassFixture<StarterWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateAuthorizedClient();
    private readonly UserDbContext _dbContext = factory.MigrateDbContext();

    [Fact]
    public async Task Token_ShouldReturnOk_WhenLoginIsSuccessful()
    {
        // Arrange
        User user = new UserBuilder().Build();
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        HashedLoginRequestDto hashedLoginRequest = new()
        {
            EmailAddress = "test@example.com",
            HashedPassword = "hashedPassword"
        };

        string json = JsonSerializer.Serialize(hashedLoginRequest);
        StringContent content = new(json, Encoding.UTF8, "application/json");
        HttpRequestMessage request = new(HttpMethod.Post, "/api/authentication/token")
        {
            Content = content
        };

        // Act
        HttpResponseMessage response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        string jsonResponse = await response.Content.ReadAsStringAsync();
        Assert.Contains("accessToken", jsonResponse);
    }

    [Fact]
    public async Task Token_ShouldReturnBadRequest_WhenLoginFails()
    {
        // Arrange
        HashedLoginRequestDto hashedLoginRequest = new()
        {
            EmailAddress = "testuser@gmail.com",
            HashedPassword = "testpasswordhash"
        };

        string json = JsonSerializer.Serialize(hashedLoginRequest);
        StringContent content = new(json, Encoding.UTF8, "application/json");
        HttpRequestMessage request = new(HttpMethod.Post, "/api/authentication/token")
        {
            Content = content
        };

        // Act
        HttpResponseMessage response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

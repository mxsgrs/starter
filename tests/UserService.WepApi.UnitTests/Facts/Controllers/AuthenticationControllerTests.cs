using UserService.Application.Commands.AuthCommands;
using UserService.Application.Dtos;

namespace UserService.WepApi.UnitTests.Facts.Controllers;

public class AuthenticationControllerTests
{
    [Fact]
    public async Task Token_ReturnsOkResult_WithValidResponse()
    {
        // Arrange
        Mock<ISender> mockSender = new();
        HashedLoginRequestDto hashedLoginRequest = new()
        {
            EmailAddress = "test@example.com",
            HashedPassword = "hashedPassword"
        };

        LoginResponseDto expectedResponse = new()
        {
            AccessToken = "mockAccessToken"
        };

        mockSender
            .Setup(sender => sender.Send(It.IsAny<CreateTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        AuthenticationController? controller = new(mockSender.Object);

        // Act
        IActionResult result = await controller.Token(hashedLoginRequest);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        LoginResponseDto actualResponse = Assert.IsType<LoginResponseDto>(okResult.Value);
        Assert.Equal(expectedResponse.AccessToken, actualResponse.AccessToken);

        // Verify the Send method was called with the expected command
        mockSender.Verify(
            sender => sender.Send(
                It.Is<CreateTokenCommand>(command =>
                    command.EmailAddress == hashedLoginRequest.EmailAddress &&
                    command.HashedPassword == hashedLoginRequest.HashedPassword),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

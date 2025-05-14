using MediatR;
using Starter.Application.Commands.AuthCommands;
using Starter.Application.Dtos;

namespace Starter.WebApi.UnitTests.Facts.Controllers;

public class AuthenticationControllerTests
{
    [Fact]
    public async Task Token_ReturnsOkResult_WithValidResponse()
    {
        // Arrange
        var mockSender = new Mock<ISender>();
        var hashedLoginRequest = new HashedLoginRequestDto
        {
            EmailAddress = "test@example.com",
            HashedPassword = "hashedPassword"
        };

        var expectedResponse = new LoginResponseDto
        {
            AccessToken = "mockAccessToken"
        };

        mockSender
            .Setup(sender => sender.Send(It.IsAny<CreateTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var controller = new AuthenticationController(mockSender.Object);

        // Act
        var result = await controller.Token(hashedLoginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualResponse = Assert.IsType<LoginResponseDto>(okResult.Value);
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

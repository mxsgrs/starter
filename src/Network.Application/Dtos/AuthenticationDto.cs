namespace Network.Application.Dtos;

public record HashedLoginRequestDto
{
    public required string EmailAddress { get; init; }
    public required string HashedPassword { get; init; }
}

public record PlainLoginRequestDto
{
    public required string EmailAddress { get; init; }
    public required string PlainPassword { get; init; }
    public bool RememberMe { get; init; } = false;
}

public record LoginResponseDto
{
    public required string AccessToken { get; init; }
}

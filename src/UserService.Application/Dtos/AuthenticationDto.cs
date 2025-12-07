namespace UserService.Application.Dtos;

public record HashedLoginRequestDto
{
    public string EmailAddress { get; init; } = "";
    public string HashedPassword { get; init; } = "";
}

public record PlainLoginRequestDto
{
    public string EmailAddress { get; init; } = "";
    public string PlainPassword { get; init; } = "";
    public bool RememberMe { get; init; } = false;
}

public record LoginResponseDto
{
    public string AccessToken { get; init; } = "";
}

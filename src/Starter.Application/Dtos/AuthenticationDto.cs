namespace Starter.Application.Dtos;

public class HashedLoginRequestDto
{
    public string EmailAddress { get; set; } = "";
    public string HashedPassword { get; set; } = "";
}

public class PlainLoginRequestDto
{
    public string EmailAddress { get; set; } = "";
    public string PlainPassword { get; set; } = "";
    public bool RememberMe { get; set; } = false;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = "";
}

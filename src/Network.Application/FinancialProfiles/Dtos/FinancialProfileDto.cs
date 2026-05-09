namespace Network.Application.FinancialProfiles.Dtos;

public record FinancialProfileDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public decimal RiskScore { get; init; }
    public IReadOnlyList<AssetDto> Assets { get; init; } = [];
}

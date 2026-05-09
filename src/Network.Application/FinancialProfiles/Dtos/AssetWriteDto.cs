using Network.Domain.Aggregates.FinancialProfileAggregate;

namespace Network.Application.FinancialProfiles.Dtos;

public record AssetWriteDto
{
    public required string Name { get; init; }
    public AssetType AssetType { get; init; }
    public decimal Value { get; init; }
    public decimal RiskFactor { get; init; }
}

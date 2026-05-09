using Network.Application.FinancialProfiles.Dtos;
using Network.ModelBuilders.Shared;

namespace Network.ModelBuilders.Dtos.FinancialProfileDtos;

public class FinancialProfileDtoBuilder : IModelBuilder<FinancialProfileDto>
{
    private Guid _id = Guid.NewGuid();
    private Guid _userId = Guid.NewGuid();
    private decimal _riskScore = 0m;
    private IReadOnlyList<AssetDto> _assets = [];

    public FinancialProfileDtoBuilder WithId(Guid v) { _id = v; return this; }
    public FinancialProfileDtoBuilder WithUserId(Guid v) { _userId = v; return this; }
    public FinancialProfileDtoBuilder WithRiskScore(decimal v) { _riskScore = v; return this; }
    public FinancialProfileDtoBuilder WithAssets(IReadOnlyList<AssetDto> v) { _assets = v; return this; }

    /// <summary>
    /// Build a FinancialProfileDto with the configured values.
    /// </summary>
    public FinancialProfileDto Build() => new()
    {
        Id = _id,
        UserId = _userId,
        RiskScore = _riskScore,
        Assets = _assets
    };
}

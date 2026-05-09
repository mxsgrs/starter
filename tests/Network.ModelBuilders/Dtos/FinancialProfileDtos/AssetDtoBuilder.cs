using Network.Application.FinancialProfiles.Dtos;
using Network.Domain.Aggregates.FinancialProfileAggregate;
using Network.ModelBuilders.Shared;

namespace Network.ModelBuilders.Dtos.FinancialProfileDtos;

public class AssetDtoBuilder : IModelBuilder<AssetDto>
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Asset";
    private AssetType _assetType = AssetType.Stock;
    private decimal _value = 1000m;
    private decimal _riskFactor = 0.5m;

    public AssetDtoBuilder WithId(Guid v) { _id = v; return this; }
    public AssetDtoBuilder WithName(string v) { _name = v; return this; }
    public AssetDtoBuilder WithAssetType(AssetType v) { _assetType = v; return this; }
    public AssetDtoBuilder WithValue(decimal v) { _value = v; return this; }
    public AssetDtoBuilder WithRiskFactor(decimal v) { _riskFactor = v; return this; }

    /// <summary>
    /// Build an AssetDto with the configured values.
    /// </summary>
    public AssetDto Build() => new()
    {
        Id = _id,
        Name = _name,
        AssetType = _assetType,
        Value = _value,
        RiskFactor = _riskFactor
    };
}

using FluentResults;
using Network.Domain.Aggregates.FinancialProfileAggregate;
using Network.ModelBuilders.Shared;

namespace Network.ModelBuilders.Aggregates.FinancialProfileAggregate;

public class FinancialProfileBuilder : IEntityModelBuilder<FinancialProfile>
{
    private Guid _userId = Guid.NewGuid();
    private readonly List<(string Name, AssetType AssetType, decimal Value, decimal RiskFactor)> _assets = [];

    public FinancialProfileBuilder WithUserId(Guid v) { _userId = v; return this; }

    public FinancialProfileBuilder WithAsset(string name, AssetType assetType, decimal value, decimal riskFactor)
    {
        _assets.Add((name, assetType, value, riskFactor));
        return this;
    }

    /// <summary>
    /// Build and return the Result produced by the factory method.
    /// </summary>
    public Result<FinancialProfile> BuildResult() => FinancialProfile.Create(_userId);

    /// <summary>
    /// Build a FinancialProfile with any configured assets applied via domain methods.
    /// </summary>
    public FinancialProfile Build()
    {
        FinancialProfile profile = FinancialProfile.Create(_userId).Value;
        foreach ((string name, AssetType assetType, decimal value, decimal riskFactor) in _assets)
            profile.AddAsset(name, assetType, value, riskFactor);
        return profile;
    }
}

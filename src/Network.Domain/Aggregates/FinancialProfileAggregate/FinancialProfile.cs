namespace Network.Domain.Aggregates.FinancialProfileAggregate;

public class FinancialProfile : AggregateRoot
{
    #region Properties

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    /// <summary>
    /// Weighted average of asset risk factors by value. Range 0–1. Recomputed on every asset mutation.
    /// </summary>
    public decimal RiskScore { get; private set; }

    private readonly List<Asset> _assets = [];

    public IReadOnlyList<Asset> Assets => _assets.AsReadOnly();

    #endregion

    #region Create

    /// <summary>
    /// Create a new financial profile for a user with no assets and a risk score of zero.
    /// </summary>
    public static Result<FinancialProfile> Create(Guid userId)
    {
        return Result.Ok(new FinancialProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RiskScore = 0
        });
    }

    #endregion

    #region Asset mutations

    /// <summary>
    /// Add a new asset to the profile and recalculate the risk score.
    /// </summary>
    public Result AddAsset(string name, AssetType assetType, decimal value, decimal riskFactor)
    {
        Result<Asset> assetResult = Asset.Create(Id, name, assetType, value, riskFactor);
        if (!assetResult.IsSuccess) return Result.Fail(assetResult.Errors);

        _assets.Add(assetResult.Value);
        RecalculateRiskScore();
        return Result.Ok();
    }

    /// <summary>
    /// Update an existing asset and recalculate the risk score.
    /// </summary>
    public Result UpdateAsset(Guid assetId, string name, AssetType assetType, decimal value, decimal riskFactor)
    {
        Asset? asset = _assets.FirstOrDefault(a => a.Id == assetId);
        if (asset is null) return Result.Fail($"Asset {assetId} not found.");

        Result updateResult = asset.Update(name, assetType, value, riskFactor);
        if (!updateResult.IsSuccess) return updateResult;

        RecalculateRiskScore();
        return Result.Ok();
    }

    /// <summary>
    /// Remove an asset from the profile and recalculate the risk score.
    /// </summary>
    public Result RemoveAsset(Guid assetId)
    {
        Asset? asset = _assets.FirstOrDefault(a => a.Id == assetId);
        if (asset is null) return Result.Fail($"Asset {assetId} not found.");

        _assets.Remove(asset);
        RecalculateRiskScore();
        return Result.Ok();
    }

    #endregion

    #region Private helpers

    private void RecalculateRiskScore()
    {
        decimal totalValue = _assets.Sum(a => a.Value);
        RiskScore = totalValue > 0
            ? _assets.Sum(a => a.Value * a.RiskFactor) / totalValue
            : 0;
    }

    #endregion

    #region Private Constructor

    /// <summary>
    /// Constructor for EF Core
    /// </summary>
    private FinancialProfile() { }

    #endregion
}

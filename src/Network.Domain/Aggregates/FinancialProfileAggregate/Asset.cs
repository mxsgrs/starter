namespace Network.Domain.Aggregates.FinancialProfileAggregate;

public class Asset : Entity
{
    #region Properties

    public Guid Id { get; private set; }

    public Guid FinancialProfileId { get; private set; }

    [MaxLength(128)]
    public string Name { get; private set; } = "";

    public AssetType AssetType { get; private set; }

    [Range(0, double.MaxValue)]
    public decimal Value { get; private set; }

    /// <summary>
    /// Risk weight of this asset. 0 = no risk, 1 = maximum risk.
    /// </summary>
    [Range(0.0, 1.0)]
    public decimal RiskFactor { get; private set; }

    #endregion

    #region Create

    /// <summary>
    /// Create a new asset owned by a financial profile.
    /// </summary>
    internal static Result<Asset> Create(Guid financialProfileId, string name, AssetType assetType, decimal value, decimal riskFactor)
    {
        Asset asset = new()
        {
            Id = Guid.NewGuid(),
            FinancialProfileId = financialProfileId,
            Name = name,
            AssetType = assetType,
            Value = value,
            RiskFactor = riskFactor
        };

        Result validationResult = Validate(asset);
        if (!validationResult.IsSuccess) return Result.Fail<Asset>(validationResult.Errors);

        return Result.Ok(asset);
    }

    #endregion

    #region Update

    /// <summary>
    /// Update the asset's values without changing its identity.
    /// </summary>
    internal Result Update(string name, AssetType assetType, decimal value, decimal riskFactor)
    {
        Name = name;
        AssetType = assetType;
        Value = value;
        RiskFactor = riskFactor;

        Result validationResult = Validate(this);
        if (!validationResult.IsSuccess) return Result.Fail(validationResult.Errors);

        return Result.Ok();
    }

    #endregion

    #region Private Constructor

    /// <summary>
    /// Constructor for EF Core
    /// </summary>
    private Asset() { }

    #endregion
}

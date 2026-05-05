namespace Sales.WebApi.Persistence;

public class Contract
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid FinancialProductId { get; private set; }
    public DateTime SignedAt { get; private set; }

    public User User { get; private set; } = null!;
    public FinancialProduct FinancialProduct { get; private set; } = null!;

    /// <summary>
    /// Create a new contract between a user and a financial product
    /// </summary>
    public static Contract Create(Guid userId, Guid financialProductId) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        FinancialProductId = financialProductId,
        SignedAt = DateTime.UtcNow
    };

    /// <summary>
    /// Change the financial product associated with this contract
    /// </summary>
    public void UpdateFinancialProduct(Guid financialProductId)
        => FinancialProductId = financialProductId;

    private Contract() { }
}

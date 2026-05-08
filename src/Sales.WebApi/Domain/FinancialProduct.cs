namespace Sales.WebApi.Domain;

public class FinancialProduct
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = "";
    public string Description { get; private set; } = "";
    public decimal Price { get; private set; }

    /// <summary>
    /// Create a new financial product
    /// </summary>
    public static FinancialProduct Create(string name, string description, decimal price) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Description = description,
        Price = price
    };

    private FinancialProduct() { }
}

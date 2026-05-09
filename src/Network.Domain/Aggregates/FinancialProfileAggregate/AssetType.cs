using System.Text.Json.Serialization;

namespace Network.Domain.Aggregates.FinancialProfileAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssetType
{
    Stock,
    Bond,
    RealEstate,
    Cash,
    Crypto,
    Other
}

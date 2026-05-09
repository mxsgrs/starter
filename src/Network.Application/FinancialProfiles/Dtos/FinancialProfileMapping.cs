using Network.Domain.Aggregates.FinancialProfileAggregate;

namespace Network.Application.FinancialProfiles.Dtos;

public static class FinancialProfileMapping
{
    /// <summary>
    /// Register Mapster type configurations for the FinancialProfile feature.
    /// </summary>
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Asset, AssetDto>();
        config.NewConfig<FinancialProfile, FinancialProfileDto>();
    }
}

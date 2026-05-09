using Network.Domain.Aggregates.FinancialProfileAggregate;

namespace Network.Application.FinancialProfiles.UseCases;

public record CreateAssetCommand(Guid UserId, string Name, AssetType AssetType, decimal Value, decimal RiskFactor) : ICommand;

/// <summary>
/// Add a new asset to a user's financial profile and recalculate the risk score.
/// </summary>
public interface ICreateAssetCommandHandler : ICommandHandler<CreateAssetCommand> { }

public class CreateAssetCommandHandler(
    IFinancialProfileRepository financialProfileRepository
) : ICreateAssetCommandHandler
{
    public async Task<Result> HandleAsync(CreateAssetCommand request)
    {
        Result<FinancialProfile> profileResult = await financialProfileRepository.FindByUserIdAsync(request.UserId);
        if (profileResult.IsFailed) return Result.Fail(profileResult.Errors);

        Result addResult = profileResult.Value.AddAsset(request.Name, request.AssetType, request.Value, request.RiskFactor);
        if (addResult.IsFailed) return addResult;

        return await financialProfileRepository.UpdateAsync(profileResult.Value.Id);
    }
}

using Network.Domain.Aggregates.FinancialProfileAggregate;

namespace Network.Application.FinancialProfiles.UseCases;

public record UpdateAssetCommand(Guid UserId, Guid AssetId, string Name, AssetType AssetType, decimal Value, decimal RiskFactor) : ICommand;

/// <summary>
/// Update an existing asset on a user's financial profile and recalculate the risk score.
/// </summary>
public interface IUpdateAssetCommandHandler : ICommandHandler<UpdateAssetCommand> { }

public class UpdateAssetCommandHandler(
    IFinancialProfileRepository financialProfileRepository
) : IUpdateAssetCommandHandler
{
    public async Task<Result> HandleAsync(UpdateAssetCommand request)
    {
        Result<FinancialProfile> profileResult = await financialProfileRepository.FindByUserIdAsync(request.UserId);
        if (profileResult.IsFailed) return Result.Fail(profileResult.Errors);

        Result updateResult = profileResult.Value.UpdateAsset(request.AssetId, request.Name, request.AssetType, request.Value, request.RiskFactor);
        if (updateResult.IsFailed) return updateResult;

        return await financialProfileRepository.UpdateAsync(profileResult.Value.Id);
    }
}

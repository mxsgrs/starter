using Network.Domain.Aggregates.FinancialProfileAggregate;

namespace Network.Application.FinancialProfiles.UseCases;

public record DeleteAssetCommand(Guid UserId, Guid AssetId) : ICommand;

/// <summary>
/// Remove an asset from a user's financial profile and recalculate the risk score.
/// </summary>
public interface IDeleteAssetCommandHandler : ICommandHandler<DeleteAssetCommand> { }

public class DeleteAssetCommandHandler(
    IFinancialProfileRepository financialProfileRepository
) : IDeleteAssetCommandHandler
{
    public async Task<Result> HandleAsync(DeleteAssetCommand request)
    {
        Result<FinancialProfile> profileResult = await financialProfileRepository.FindByUserIdAsync(request.UserId);
        if (profileResult.IsFailed) return Result.Fail(profileResult.Errors);

        Result removeResult = profileResult.Value.RemoveAsset(request.AssetId);
        if (removeResult.IsFailed) return removeResult;

        return await financialProfileRepository.UpdateAsync(profileResult.Value.Id);
    }
}

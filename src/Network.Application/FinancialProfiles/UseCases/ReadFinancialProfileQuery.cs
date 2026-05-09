using Network.Application.FinancialProfiles.Dtos;
using Network.Domain.Aggregates.FinancialProfileAggregate;

namespace Network.Application.FinancialProfiles.UseCases;

public interface IReadFinancialProfileQueryHandler : IQueryByIdHandler<Result<FinancialProfileDto>> { }

public class ReadFinancialProfileQueryHandler(
    IFinancialProfileRepository financialProfileRepository
) : IReadFinancialProfileQueryHandler
{
    /// <summary>
    /// Return the financial profile for the given user.
    /// </summary>
    public async Task<Result<FinancialProfileDto>> HandleAsync(Guid userId)
    {
        Result<FinancialProfile> profileResult = await financialProfileRepository.FindByUserIdAsync(userId);
        if (profileResult.IsFailed) return Result.Fail<FinancialProfileDto>(profileResult.Errors);

        FinancialProfileDto dto = profileResult.Value.Adapt<FinancialProfileDto>();
        return Result.Ok(dto);
    }
}

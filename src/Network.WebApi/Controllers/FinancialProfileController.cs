namespace Network.WebApi.Controllers;

public class FinancialProfileController : NetworkControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> ReadFinancialProfile(
        Guid userId,
        [FromServices] IReadFinancialProfileQueryHandler readFinancialProfileQueryHandler
    ) => CorrespondingStatus(await readFinancialProfileQueryHandler.HandleAsync(userId));

    [HttpPost("{userId:guid}/asset")]
    public async Task<IActionResult> CreateAsset(
        Guid userId,
        [FromServices] ICreateAssetCommandHandler createAssetCommandHandler,
        AssetWriteDto assetWriteDto
    ) => CorrespondingStatus(await createAssetCommandHandler.HandleAsync(
        new CreateAssetCommand(userId, assetWriteDto.Name, assetWriteDto.AssetType, assetWriteDto.Value, assetWriteDto.RiskFactor)));

    [HttpPut("{userId:guid}/asset/{assetId:guid}")]
    public async Task<IActionResult> UpdateAsset(
        Guid userId,
        Guid assetId,
        [FromServices] IUpdateAssetCommandHandler updateAssetCommandHandler,
        AssetWriteDto assetWriteDto
    ) => CorrespondingStatus(await updateAssetCommandHandler.HandleAsync(
        new UpdateAssetCommand(userId, assetId, assetWriteDto.Name, assetWriteDto.AssetType, assetWriteDto.Value, assetWriteDto.RiskFactor)));

    [HttpDelete("{userId:guid}/asset/{assetId:guid}")]
    public async Task<IActionResult> DeleteAsset(
        Guid userId,
        Guid assetId,
        [FromServices] IDeleteAssetCommandHandler deleteAssetCommandHandler
    ) => CorrespondingStatus(await deleteAssetCommandHandler.HandleAsync(
        new DeleteAssetCommand(userId, assetId)));
}

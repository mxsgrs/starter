namespace Network.WebApi.Controllers.Abstracts;

[Authorize]
[ApiController]
[Route("api/network/[controller]")]
public class NetworkControllerBase() : ControllerBase 
{
    /// <summary>
    /// Returns http status code corresponding to the result of the operation.
    /// </summary>
    [NonAction]
    public IActionResult CorrespondingStatus<TEntity>(Result<TEntity> result)
    {
        if (result.IsFailed)
        {
            string firstErrorMessage = ReadFirstErrorMessage(result.Errors);
            return BadRequest(firstErrorMessage);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Returns http status code corresponding to the result of the operation.
    /// </summary>
    [NonAction]
    public IActionResult CorrespondingStatus(Result result)
    {
        if (result.IsFailed)
        {
            string firstErrorMessage = ReadFirstErrorMessage(result.Errors);
            return BadRequest(firstErrorMessage);
        }

        return NoContent();
    }

    private static string ReadFirstErrorMessage(IReadOnlyList<IError> errorList)
    {
        return errorList.Select(error => error.Message).FirstOrDefault() ?? "An error occurred.";
    }
}

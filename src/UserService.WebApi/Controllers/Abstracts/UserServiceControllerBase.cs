namespace UserService.WebApi.Controllers.Abstracts;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserServiceControllerBase() : ControllerBase 
{
    /// <summary>
    /// Returns http status code corresponding to the result of the operation.
    /// </summary>
    public IActionResult CorrespondingStatus<TEntity>(Result<TEntity> result)
    {
        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(error => error.Message).FirstOrDefault() ?? "An error occurred.");
        }

        return Ok(result.Value);
    }

    public IActionResult CorrespondingStatus(Result result)
    {
        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(error => error.Message).FirstOrDefault() ?? "An error occurred.");
        }

        return NoContent();
    }
}

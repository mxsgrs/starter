using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace UserService.WebApi.Utilities;

public sealed class GlobalExceptionHandler() : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails? problemDetails = CreateProblemDetailFromException(exception);

        if (problemDetails is null)
        {
            return false; // Not handled by this handler
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails? CreateProblemDetailFromException(Exception exception)
    {
        return exception switch
        {
            // 400
            BadRequestException => new ProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad request",
                Detail = exception.Message
            },
            // 401
            UnauthorizedException => new ProblemDetails()
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = exception.Message
            },
            // 404
            NotFoundException => new ProblemDetails()
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not found",
                Detail = exception.Message
            },
            // 500
            _ => null,
        };
    }
}

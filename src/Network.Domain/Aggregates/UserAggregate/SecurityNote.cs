using System.ComponentModel.DataAnnotations;

namespace Network.Domain.Aggregates.UserAggregate;

public class SecurityNote : Entity
{
    #region Properties

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }

    [MaxLength(250)]
    public string Reason { get; private set; } = string.Empty;
    public DateTime CreatedOn { get; private set; }

    #endregion

    #region Create

    /// <summary>
    /// Create a security note for a user whose age is 30 or above
    /// </summary>
    public static Result<SecurityNote> Create(Guid userId, string reason)
    {
        SecurityNote note = new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Reason = reason,
            CreatedOn = DateTime.UtcNow
        };

        Result validationResult = Validate(note);
        if (!validationResult.IsSuccess) return Result.Fail<SecurityNote>(validationResult.Errors);

        return Result.Ok(note);
    }

    #endregion

    #region Private Constructor

    private SecurityNote() { }

    #endregion
}

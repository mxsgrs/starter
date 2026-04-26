using Network.Domain.Events;
using System.ComponentModel.DataAnnotations;

namespace Network.Domain;

public interface IAggregateRoot { }

public abstract class AggregateRoot : IAggregateRoot
{
    #region Domain Events

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    #endregion

    #region Validation
    protected static Result Validate(object instance)
    {
        List<ValidationResult> validationResults = GetValidationResults(instance);

        if (validationResults.Count != 0)
        {
            IEnumerable<string> errors = validationResults
                .Where(vr => !string.IsNullOrEmpty(vr.ErrorMessage))
                .Select(vr => vr.ErrorMessage!);

            return Result.Fail(errors);
        }

        return Result.Ok();
    }

    private static List<ValidationResult> GetValidationResults(object instance)
    {
        ValidationContext validationContext = new(instance);
        List<ValidationResult> validationResults = [];

        Validator.TryValidateObject(instance, validationContext, validationResults, true);

        return validationResults;
    }
    #endregion
}

using System.ComponentModel.DataAnnotations;

namespace UserService.Domain;

public interface IAggregateRoot { }

public abstract class AggregateRoot : IAggregateRoot
{
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

    protected static void ThrowIfInvalid(object instance)
    {
        List<ValidationResult> validationResults = GetValidationResults(instance);

        if (validationResults.Count > 0)
        {
            throw new ValidationException($"{instance.GetType().Name} is not valid: " +
                string.Join(", ", validationResults.Select(vr => vr.ErrorMessage)));
        }
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

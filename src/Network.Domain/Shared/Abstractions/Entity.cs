namespace Network.Domain.Shared.Abstractions;

public abstract class Entity
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

    private static List<ValidationResult> GetValidationResults(object instance)
    {
        ValidationContext validationContext = new(instance);
        List<ValidationResult> validationResults = [];

        Validator.TryValidateObject(instance, validationContext, validationResults, true);

        return validationResults;
    }

    #endregion
}

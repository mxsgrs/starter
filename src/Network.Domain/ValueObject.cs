using System.ComponentModel.DataAnnotations;

namespace Network.Domain;

public abstract class ValueObject<T> where T : ValueObject<T>
{
    public abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
        => obj is not null &&
           obj is T valueObject &&
           obj.GetType() == GetType() &&
           GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());

    public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        => left.Equals(right);

    public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        => !left.Equals(right);

    public override int GetHashCode()
        => GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);

    protected static Result Validate(object instance)
    {
        ValidationContext validationContext = new(instance);
        List<ValidationResult> validationResults = [];

        Validator.TryValidateObject(instance, validationContext, validationResults, true);

        if (validationResults.Count != 0)
        {
            IEnumerable<string> errors = validationResults
                .Where(vr => !string.IsNullOrEmpty(vr.ErrorMessage))
                .Select(vr => vr.ErrorMessage!);

            return Result.Fail(errors);
        }

        return Result.Ok();
    }
}

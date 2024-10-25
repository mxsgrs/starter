using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Starter.Domain.Validations;

public class NotInFutureAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime > DateTime.Now)
            {
                return new($"{validationContext.MemberName} date can't be in the future");
            }
        }

        return ValidationResult.Success;
    }
}

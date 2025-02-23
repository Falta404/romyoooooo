using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class EnumStringValidationAttribute : ValidationAttribute
{
    public Type EnumType { get; }

    public EnumStringValidationAttribute(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("EnumType must be an enum type.", nameof(enumType));

        EnumType = enumType;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return ValidationResult.Success; // Empty or null value can be handled separately (e.g., RequiredAttribute)
        }

        bool isValid = Enum.IsDefined(EnumType, value.ToString());

        if (!isValid)
        {
            return new ValidationResult($"{value} is not a valid value for enum {EnumType.Name}.", new[] { validationContext.MemberName });
        }

        return ValidationResult.Success;
    }
}

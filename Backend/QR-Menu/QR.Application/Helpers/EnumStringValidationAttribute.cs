using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class EnumStringValidationAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public EnumStringValidationAttribute(Type enumType)
    {
        if (enumType == null || !enumType.IsEnum)
            throw new ArgumentException("Type must be a valid enum", nameof(enumType));
        _enumType = enumType;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        string inputValue = value.ToString();
        var enumNames = Enum.GetNames(_enumType);
        var enumValuesWithDisplay = _enumType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => new
            {
                Name = f.Name,
                DisplayName = f.GetCustomAttribute<DisplayAttribute>()?.Name
            })
            .ToList();

        if (enumNames.Any(name => string.Equals(name, inputValue, StringComparison.OrdinalIgnoreCase)))
        {
            return ValidationResult.Success;
        }

        if (enumValuesWithDisplay.Any(e => string.Equals(e.DisplayName, inputValue, StringComparison.OrdinalIgnoreCase)))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(
            $"{inputValue} is not a valid value for enum {_enumType.Name}.",
            new[] { validationContext.MemberName }
        );
    }
}

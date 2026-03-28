using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class NonWhiteSpaceAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null)
            return false;

        if (value is string s)
            return !string.IsNullOrWhiteSpace(s);

        return false;
    }
}


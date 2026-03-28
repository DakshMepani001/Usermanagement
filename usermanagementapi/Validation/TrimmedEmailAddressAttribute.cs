using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class TrimmedEmailAddressAttribute : ValidationAttribute
{
    private readonly EmailAddressAttribute _inner = new();

    public override bool IsValid(object? value)
    {
        if (value is null)
            return false;

        if (value is not string s)
            return false;

        var trimmed = s.Trim();
        if (trimmed.Length == 0)
            return false;

        // Validate the trimmed value so clients can send emails with leading/trailing spaces.
        return _inner.IsValid(trimmed);
    }
}


using System.ComponentModel.DataAnnotations;
using UserManagementAPI.Validation;

namespace UserManagementAPI.Models;

public class UserUpsertRequest
{
    [Required]
    [NonWhiteSpace]
    [StringLength(50, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [NonWhiteSpace]
    [StringLength(50, MinimumLength = 1)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [TrimmedEmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }
}


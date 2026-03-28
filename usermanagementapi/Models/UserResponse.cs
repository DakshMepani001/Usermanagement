namespace UserManagementAPI.Models;

public record UserResponse(Guid Id, string FirstName, string LastName, string Email, string? Phone);


using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Blazor.Models;

public sealed class RegisterRequest
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Owner;
}

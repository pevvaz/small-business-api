using System.ComponentModel.DataAnnotations;

public class LoginSessionDTO
{
    [Required(ErrorMessage = "Role in body is required")]
    public required string Role { get; set; }

    [Required(ErrorMessage = "Password in body is required")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Email in body is required")]
    public required string Email { get; set; }
}
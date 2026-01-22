using System.ComponentModel.DataAnnotations;

public class SessionDTO
{
    public class LoginSessionDTO
    {
        [Required(ErrorMessage = "Name in body is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Password in body is required")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Email in body is required")]
        public required string Email { get; set; }
    }
}
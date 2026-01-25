using System.ComponentModel.DataAnnotations;

public class UserDTO
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "Name in body is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Email in body is required")]
        [EmailAddress(ErrorMessage = "Email in body needs to be a valid email address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password in body is required")]
        public required string Password { get; set; }
    }

    public class UpdateUserDTO
    {
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Email in body needs to be a valid email address")]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
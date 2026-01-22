using System.ComponentModel.DataAnnotations;

public class UserDTO
{
    public class CreateUserDTO
    {
        public required string Role { get; set; }
        public required ContextModels.UserContextModel.EnumUserRoles RoleDois { get; set; } // fazer test no swagger com string e enum

        [Required(ErrorMessage = "Name in body is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Password in body is required")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Email in body is required")]
        public required string Email { get; set; }
    }

    public class UpdateUserDTO
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
    }
}
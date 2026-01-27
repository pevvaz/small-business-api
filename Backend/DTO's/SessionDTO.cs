using System.ComponentModel.DataAnnotations;

public class SessionDTO
{
    public class LoginSessionDTO
    {
        public required string? NameOrEmail { get; set; }

        [Required(ErrorMessage = "Password in body is required")]
        public required string Password { get; set; }

    }

    public class CreateRefreshTokenDTO
    {
        public string? Token { get; set; }
        public DateTime? Expire { get; set; }
    }
}
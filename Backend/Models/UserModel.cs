using System.ComponentModel.DataAnnotations;

public class CreateUserDTO
{
    [Required] public required string Role { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required string Password { get; set; }
    [Required] public required string Email { get; set; }
}

public class UpdateUserDTO
{
    [Range(1, int.MaxValue)]
    public int? Id { get; set; }

    public string? Role { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }

}
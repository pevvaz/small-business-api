using System.ComponentModel.DataAnnotations;

public class EmployeeModel
{
    [Required] public int Id { get; set; }
    [Required] public required string Role { get; set; }
    [Required] public required string Name { get; set; }
    [Required] public required string Password { get; set; }
    [Required] public required string Email { get; set; }
}
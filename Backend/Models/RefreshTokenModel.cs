using System.ComponentModel.DataAnnotations;

public class RefreshTokenModel
{
    [Required] public int Id { get; set; }
    [Required] public required int User { get; set; }
    [Required] public required string Token { get; set; }
    [Required] public required DateTime Expire { get; set; }
    [Required] public required bool Valid { get; set; }
}

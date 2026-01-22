using System.ComponentModel.DataAnnotations;

public class RefreshTokenDTO
{
    public class CreateRefreshTokenDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "UserId in body is out of range")]
        public int? UserId { get; set; }

        public string? Token { get; set; }
        public DateTime? Expire { get; set; }
        public bool? Valid { get; set; }
    }
}
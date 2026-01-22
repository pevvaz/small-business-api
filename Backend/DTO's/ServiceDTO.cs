using System.ComponentModel.DataAnnotations;

public class ServiceDTO
{
    public class CreateServiceDTO
    {
        [Required(ErrorMessage = "Name in body is requred")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Price in body is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price in body is out of range")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Duration in body is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration in body is out of range")]
        public int? Duration { get; set; }

        [Required(ErrorMessage = "Status in body is required")]
        public required string Status { get; set; }
    }

    public class UpdateServiceDTO
    {
        public string? Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price in body is out of range")]
        public decimal? Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Duration in body is out of range")]
        public int? Duration { get; set; }

        public string? Status { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

public class ServiceModel
{
    [Required] public required int Id { get; set; } = 0;
    [Required] public required string Name { get; set; }
    [Required] public required int Duration { get; set; }
    [Required] public required float Price { get; set; }
    [Required] public required string Status { get; set; }
}

public class UpdateServiceModel
{
    [Required] public required int Id { get; set; }
    public string? Name { get; set; }
    public double? Duration { get; set; }
    public float? Price { get; set; }
    public string? Status { get; set; }
}
using System.ComponentModel.DataAnnotations;

public class CreateAppointmentDTO
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Employee { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Client { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Service { get; set; }

    [Required] public required DateTime? StartDate { get; set; }
    [Required] public required DateTime? EndDate { get; set; }
}

public class UpdateAppointmentDTO
{
    [Range(1, int.MaxValue)]
    public int? Employee { get; set; }

    [Range(1, int.MaxValue)]
    public int? Client { get; set; }

    [Range(1, int.MaxValue)]
    public int? Service { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? Resolved { get; set; }
    public string? Status { get; set; }
}
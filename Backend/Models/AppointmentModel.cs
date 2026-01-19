using System.ComponentModel.DataAnnotations;

public class AppointmentModel
{
    [Required] public int Id { get; set; }
    [Required] public required int Employee { get; set; }
    [Required] public required int Client { get; set; }
    [Required] public required int Service { get; set; }
    [Required] public required DateTime StartDate { get; set; }
    [Required] public required DateTime EndDate { get; set; }
    [Required] public required string Status { get; set; } = "scheduled";
}

public class UpdateAppointmentModel
{
    public required int Id { get; set; }
    public ClientModel? Client { get; set; }
    public EmployeeModel? Employee { get; set; }
    public ServiceModel? Service { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
}
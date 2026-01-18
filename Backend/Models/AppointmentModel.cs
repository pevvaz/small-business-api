using System.ComponentModel.DataAnnotations;

public class AppointmentModel
{
    [Required] public int Id { get; set; }
    [Required] public required ClientModel Client { get; set; }
    [Required] public required EmployeeModel Employee { get; set; }
    [Required] public required ServiceModel Service { get; set; }
    [Required] public required DateTime StartDate { get; set; }
    [Required] public required DateTime EndDate { get; set; }
    [Required] public required string Status { get; set; }
}
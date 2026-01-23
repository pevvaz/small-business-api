using System.ComponentModel.DataAnnotations;

public class AppointmentDTO
{
    public class CreateAppointmentDTO
    {
        [Required(ErrorMessage = "Employee in body is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Employee in body is out of range")]
        public int? EmployeeId { get; set; }

        [Required(ErrorMessage = "Client in body is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Client in body is out of range")]
        public int? ClientId { get; set; }

        [Required(ErrorMessage = "Service in body is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Service in body is out of range")]
        public int? ServiceId { get; set; }

        [Required(ErrorMessage = "StartDate in body is required")]
        public required DateTime? StartDate { get; set; }
    }

    public class UpdateAppointmentDTO
    {
        public DateTime? StartDate { get; set; }
        public string? Status { get; set; }
        public bool? Resolved { get; set; }
    }
}
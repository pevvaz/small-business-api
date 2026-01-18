public class AppointmentModel
{
    public ClientModel Client { get; set; }
    public EmployeeModel Employee { get; set; }
    public ServiceModel Service { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
}
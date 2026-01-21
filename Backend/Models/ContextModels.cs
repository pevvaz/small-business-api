using System.ComponentModel.DataAnnotations.Schema;

public class ContextModels
{
    public class SessionContextModel
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        public required UserContextModel User { get; set; }
    }

    public class UserContextModel
    {
        public int Id { get; set; }

        public required string Role { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }

    public class ServiceContextModel
    {
        public int Id { get; set; }

        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required int Duration { get; set; }
        public required string Status { get; set; }
    }

    public class AppointmentContextModel
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public required UserContextModel Employee { get; set; }

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        public required UserContextModel Client { get; set; }

        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public required UserContextModel Service { get; set; }

        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public bool Resolved { get; set; } = false;
        public string Status { get; set; } = "scheduled";
    }

    public class RefreshTokenContextModel
    {
        public int Id { get; set; }

        public required int UserId { get; set; }
        public required string Token { get; set; }
        public required DateTime Exprire { get; set; }
        public required bool Valid { get; set; }
    }
}
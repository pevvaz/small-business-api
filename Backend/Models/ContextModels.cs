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
        public enum EnumUserRoles
        {
            Admin,
            Employee,
            Client
        }

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
        public enum EnumAppointmentStatus
        {
            Scheduled,
            Canceled,
            Expired,
            Done,
        }

        public int Id { get; set; }

        public int HistoryEmployeeId { get; set; }
        public int HistoryClientId { get; set; }
        public int HistoryServiceId { get; set; }
        public required string HistoryEmployeeName { get; set; }
        public required string HistoryClientName { get; set; }
        public required string HistoryServiceName { get; set; }

        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public EnumAppointmentStatus Status { get; set; } = EnumAppointmentStatus.Scheduled;
        public bool Resolved { get; set; } = false;
    }

    public class RefreshTokenContextModel
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public required int UserId { get; set; }
        public required UserContextModel User { get; set; }

        public required string Token { get; set; }
        public required DateTime Exprire { get; set; }
        public required bool Valid { get; set; }
    }
}
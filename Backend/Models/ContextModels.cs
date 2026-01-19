public class ContextModels
{
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
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public required string Status { get; set; }
    }

    public class AppointmentContextModel
    {
        public int Id { get; set; }

        public int Employee { get; set; }
        public int Client { get; set; }
        public int Service { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required string Status { get; set; }
    }

    public class RefreshTokenContextModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public required string Token { get; set; }
        public DateTime Exprire { get; set; }
        public bool Valid { get; set; }
    }
}
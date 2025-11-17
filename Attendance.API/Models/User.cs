namespace Attendance.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public string MobileNo { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int SubscriptionId { get; set; }
    }
}

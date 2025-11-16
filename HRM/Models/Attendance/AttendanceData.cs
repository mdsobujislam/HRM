namespace HRM.Models.Attendance
{
    public class AttendanceData
    {
        public DateTime? CurrentDate { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string EnrollID { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public TimeSpan InTime { get; set; }
        public TimeSpan OutTime { get; set; }
        public Decimal LateIn { get; set; }
        public Decimal WorkingHour { get; set; }
        public Decimal EarlyOUt { get; set; }
        public string Remarks { get; set; }
    }
}

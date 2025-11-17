namespace Attendance.API.Models.Attendance
{
    public class ManualAttendanceDto
    {
        public int UserId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan? InTime { get; set; }
        public TimeSpan? OutTime { get; set; }
    }
}

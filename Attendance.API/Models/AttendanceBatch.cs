namespace Attendance.API.Models
{
    public class AttendanceBatch
    {
        public long Id { get; set; }

        public string BatchId { get; set; }
        public string DeviceIp { get; set; }
        public int MachineNumber { get; set; }
        public DateTime SentAt { get; set; }
        public ICollection<AttendanceRecord> Records { get; set; } = new List<AttendanceRecord>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

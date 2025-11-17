namespace Attendance.API.Models
{
    public class AttendanceRecord
    {
        public long Id { get; set; }
        public long AttendanceBatchId { get; set; }
        public AttendanceBatch? AttendanceBatch { get; set; }
        public string EnrollId { get; set; }
        public string UserIdFromDevice { get; set; }
        public int VerifyMode { get; set; }
        public int InOutMode { get; set; }
        public DateTime DeviceTimestamp { get; set; }
        public DateTime Timestamp { get; set; }
        public int WorkCode { get; set; }
        public string RecordKey { get; set; }
        //public int? UserId { get; set; }
        //public User User { get; set; }
    }
}

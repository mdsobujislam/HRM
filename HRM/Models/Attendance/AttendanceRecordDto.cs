using HRM.Helpers;
using System.Text.Json.Serialization;

namespace HRM.Models.Attendance
{
    public class AttendanceRecordDto
    {
        [JsonPropertyName("enroll_id")]
        public string EnrollId { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("verify_mode")]
        public int VerifyMode { get; set; }

        [JsonPropertyName("inout_mode")]
        public int InOutMode { get; set; }

        [JsonPropertyName("device_timestamp")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DeviceTimestamp { get; set; }

        [JsonPropertyName("timestamp")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("work_code")]
        public int WorkCode { get; set; }

        [JsonPropertyName("record_key")]
        public string RecordKey { get; set; }
    }
}

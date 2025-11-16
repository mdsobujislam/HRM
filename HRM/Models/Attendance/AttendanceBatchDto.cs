using HRM.Helpers;
using System.Text.Json.Serialization;

namespace HRM.Models.Attendance
{
    public class AttendanceBatchDto
    {
        [JsonPropertyName("batch_id")]
        public string BatchId { get; set; }

        [JsonPropertyName("device_ip")]
        public string DeviceIp { get; set; }

        [JsonPropertyName("machine_number")]
        public int MachineNumber { get; set; }

        [JsonPropertyName("sent_at")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime SentAt { get; set; }

        [JsonPropertyName("records")]
        public List<AttendanceRecordDto> Records { get; set; } = new();
    }
}

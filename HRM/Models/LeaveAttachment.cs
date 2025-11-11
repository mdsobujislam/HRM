namespace HRM.Models
{
    public class LeaveAttachment
    {
        public int Id { get; set; }
        public int LeaveRecordId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

namespace HRM.Models
{
    public class LeaveApplication
    {
        public int Id { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int NumberOfDays { get; set; }
        public string ApprovedByImmediateBossStatus { get; set; }
        public string ApprovedByHRStatus { get; set; }
        public int CreatedBy { get; set; }
        public string Reason { get; set; }

        public List<LeaveAttachment> Attachments { get; set; } = new List<LeaveAttachment>();
        public string FilePaths { get; set; }
    }
}

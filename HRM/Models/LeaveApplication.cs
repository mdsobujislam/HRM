namespace HRM.Models
{
    public class LeaveApplication
    {
        public int Id { get; set; }
        public int LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int NumberOfDays { get; set; }
        public int ApprovedByImmediateBossStatus { get; set; }
        public int ApprovedByHRStatus { get; set; }
        public int CreatedBy { get; set; }
        public string Reason { get; set; }
    }
}

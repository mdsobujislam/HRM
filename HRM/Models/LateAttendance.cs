namespace HRM.Models
{
    public class LateAttendance
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
        public int LateMin { get; set; }
        public int NoOfLate { get; set; }
        public string AdjustmentType { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public string Basic { get; set; }
    }
}

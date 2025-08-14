namespace HRM.Models
{
    public class Overtime
    {
        public int Id { get; set; }
        public int OverHour { get; set; }
        public string AdjustmentType { get; set; }
        public string Basic { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
    }
}

namespace HRM.Models
{
    public class LeaveAllotment
    {
        public int Id { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public string NoOfLeaves { get; set; }
        public string YearOrMonth { get; set; }
        public string LeaveCarry { get; set; }
        public int NoOfYears { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
    }
}

namespace HRM.Models
{
    public class LeaveType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string Documents { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
    }
}

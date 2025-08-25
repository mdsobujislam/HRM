namespace HRM.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string Branch { get; set; }
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public int SubscriptionId { get; set; }
    }
}

namespace HRM.Models
{
    public class Designation
    {
        public int Id { get; set; }
        public string DesignationName { get; set; }
        public string Branch { get; set; }
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public int SubscriptionId { get; set; }
    }
}

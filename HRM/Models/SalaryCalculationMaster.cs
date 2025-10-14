namespace HRM.Models
{
    public class SalaryCalculationMaster
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string ApplyDate { get; set; }
        public List<SalaryCalculationDetail> SalaryItems { get; set; }
    }
}

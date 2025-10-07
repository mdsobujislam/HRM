namespace HRM.Models
{
    public class GratuityCalculate
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public double LastBasicSalary { get; set; }
        public double EmploymentYears { get; set; }
        public int BranchId { get; set; }
    }
}

namespace HRM.Models
{
    public class GratuityCalculate
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public double LastBasicSalary { get; set; }
        public double EmploymentYears { get; set; }
        public int BranchId { get; set; }
    }
}

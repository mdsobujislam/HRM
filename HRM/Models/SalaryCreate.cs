namespace HRM.Models
{
    public class SalaryCreate
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public string Image { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public double BasicSalary { get; set; }
        public double HouseRent { get; set; }
        public double MedicalAllowance { get; set; }
        public double TADA { get; set; }
        public double Conveyance { get; set; }
        public double SPECIALALLOW { get; set; }
        public double ProvidentFund { get; set; }
        public double IncomeTax { get; set; }
        public double LoanDeduction { get; set; }
        public double Total { get; set; }
        public double GrossSalary { get; set; }
        public double Tax { get; set; }
        public double NetSalary { get; set; }
        public double Bonus { get; set; }
        public double AmountPayable { get; set; }
        public int WorkingDays { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
    }
}

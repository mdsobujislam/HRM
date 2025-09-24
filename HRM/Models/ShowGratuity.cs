namespace HRM.Models
{
    public class ShowGratuity
    {
        public int EmpId { get; set; }
        public string Photo { get; set; }
        public string Name { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
        public string Designation { get; set; }
        public string AppoinmentDate { get; set; }
        public string NoOfYears { get; set; }
        public double LastBasicSalary { get; set; }
    }
}

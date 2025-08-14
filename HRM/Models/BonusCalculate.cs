namespace HRM.Models
{
    public class BonusCalculate
    {
        public int Id { get; set; }
        public int BonusTypeId { get; set; }
        public string BonusType { get; set; }
        public DateTime BonusDate { get; set; }
        public double Percentage { get; set; }
        public int EmployeeId { get; set; }
        public string Employee { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
        public int DesignationId { get; set; }
        public string Designation { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
    }
}

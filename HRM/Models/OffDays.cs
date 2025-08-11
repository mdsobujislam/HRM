namespace HRM.Models
{
    public class OffDays
    {
        public int Id { get; set; }
        public string OffDay { get; set; }
        public int DesignationId { get; set; }
        public string Designation { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
    }
}

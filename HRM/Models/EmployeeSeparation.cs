namespace HRM.Models
{
    public class EmployeeSeparation
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int BrnachId { get; set; }
        public string Brnach { get; set; }
        public int SeparationReasonId { get; set; }
        public string SeparationReason { get; set; }
        public string SeparationDate { get; set; }
    }
}

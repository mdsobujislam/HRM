namespace HRM.Models
{
    public class ExcludeTax
    {
        public int Id { get; set; }
        public int MonthIndex { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string GenDate { get; set; }
        public int EmployeeId { get; set; }
        public int BranchId { get; set; }
    }
}

namespace HRM.Models.ViewModels
{
    public class StopExcludeTaxViewModel
    {
        public int MonthIndex { get; set; }     // 1..12
        public int Year { get; set; }           // e.g. 2025
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? GenDate { get; set; }
        public List<int> StopEmployeeIds { get; set; } = new List<int>();
        public List<int> StartEmployeeIds { get; set; } = new List<int>();
        public int BranchId { get; set; }       // <-- add this so form can post branch
    }
}

namespace HRM.Models.ViewModels
{
    public class AvailableLeaveVM
    {
        public string LeaveType { get; set; }
        public int Allowed { get; set; }
        public int Taken { get; set; }
        public int Remaining { get; set; }
    }
}

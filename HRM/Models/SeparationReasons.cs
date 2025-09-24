namespace HRM.Models
{
    public class SeparationReasons
    {
        public int Id { get; set; }
        public string Sep_Reason { get; set; }
        public string Branch { get; set; }
        public int BranchId { get; set; }
    }
}

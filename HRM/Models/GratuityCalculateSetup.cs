namespace HRM.Models
{
    public class GratuityCalculateSetup
    {
        public int Id { get; set; }
        public int SalaryHeadId { get; set; }
        public string SalaryHead { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
        public int MinYear { get; set; }
    }


}

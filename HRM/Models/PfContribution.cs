namespace HRM.Models
{
    public class PfContribution
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }   // use DateTime instead of string
        public double PFContributionPercentage { get; set; }
        public int PFContributionYear { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }
        public int DesignationId { get; set; }
        public string Designation { get; set; }
    }

}

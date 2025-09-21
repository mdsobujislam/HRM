namespace HRM.Models
{
    public class PFInterest
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public double Interest { get; set; }
        public int BranchId { get; set; }
        public string Branch { get; set; }

    }
}
